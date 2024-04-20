using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing.Text;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.Diagnostics.Eventing.Reader;
using System.Collections.Generic;
using UIAutomationClient;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms.VisualStyles;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
namespace ScreenShotAutoPlus
{
    public partial class Form1 : Form
    {
        private bool isPaused;
        //This next variable needs to be a class instance variable because I do not want to pass savePath
        private string savePath;
        private TaskCompletionSource<bool> pauseCompletionSource;
        Size theSize;
        private int completedScreenshots;
        private int totalScreenshots;
        private bool sufficientSpace;
        private string filePath;
        private readonly long[] info = new long[2];
        private readonly IUIAutomation processInterface;

        private readonly List<string> filePaths = new List<string>();
        private readonly List<string> fileNames = new List<string>();

        public Form1()
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.TopMost = true;
            pauseCompletionSource = new TaskCompletionSource<bool>();
            pauseCompletionSource.SetResult(false);
            BackgroundWorker1.WorkerReportsProgress = false;
            BackgroundWorker1.WorkerSupportsCancellation = false;
            processInterface = new CUIAutomation8();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //I have officially gotten tired of typing in my values every time I hit debug. So the next statement will only remain here until I get this program working correctly.
            colorCode_TB.Text = "#f72ee0";
            //the rest of the code in this block is what fixed the fact that the ui tended to sometimes either overlap or be overlaped by the taskbar.
            Size formSize = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = formSize;
            this.Location = new Point(0, 0);
        }
        private bool OpenAndValidateFP(ref string filePath)
        {
            try
            {
                using (FolderBrowserDialog fBD = new FolderBrowserDialog())
                {
                    fBD.Description = "Please select the folder that contains the files to be screenshotted. Remember, this program is not meant to be used to engage in copyright infringement.";
                    fBD.ShowNewFolderButton = false;
                    DialogResult result = fBD.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fBD.SelectedPath))
                    {
                        filePath = fBD.SelectedPath;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n Help link: {f.HelpLink}\n + Inner Exception: " +
                     $"{f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + " +
                     $"TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
            return true;
        }
        bool NumbersValidated(int waitB4, int waitAfter)
        {
            //This method is meant to ensure that the user did not enter in a negative number or a decimal number into the textboxes for the wait periods.
            try
            {
                if (int.TryParse(waitAfter_TB.Text, out int waitA) == true && int.TryParse(waitB4_TB.Text, out int waitB) == true)
                {
                    if (waitA >= 0 && waitB >= 0)
                    {
                        waitA = waitAfter;
                        waitB = waitB4;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: " +
                    $"{f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + " +
                    $"TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }
        private void Import_BTN_Click(object sender, EventArgs e)
        {
            int waitB4 = 0;
            int waitAfter = 0;
            if (NumbersValidated(waitB4, waitAfter) == true && OpenAndValidateFP(ref filePath) == true)
            {
                List<string> invalidFiles = new List<string>();
                try
                {
                    DirectoryInfo di = new DirectoryInfo(filePath);
                    //Make sure it's nif files that are being added.
                    foreach (var file in di.GetFiles())
                    {
                        if (file.Extension == ".nif")
                        {
                            screenshotFiles_LB.Items.Add((screenshotFiles_LB.Items.Count + 1) + ". " + file.Name);
                            filePaths.Add(file.FullName);
                            fileNames.Add(file.Name);
                        }
                        else
                        {
                            invalidFiles.Add(file.Name);
                            continue;
                        }
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + " +
                        $"InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + " +
                        $"StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                }
                finally
                {
                    if (invalidFiles.Count > 0)
                    {
                        MessageBox.Show("Some of the files that were in the file path you chose were not imported because the file extension was incorrect. This app is meant for .nif files specifically.", "Information", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("All the in the folder you selected has been imported!", "Notification", MessageBoxButtons.OK);
                    }
                }
            }
            totalScreenshots = screenshotFiles_LB.Items.Count;
            GetSavePath();
        }
        private void GetSavePath()
        {
            try
            {//the else loop is meant to prevent the screenshot process from even starting if the person did not select a save path.
                using (FolderBrowserDialog fbd2 = new FolderBrowserDialog())
                {
                    fbd2.Description = "Select a folder for the screenshots to be saved to.";
                    fbd2.ShowNewFolderButton = false;
                    DialogResult result = fbd2.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        savePath = fbd2.SelectedPath;
                    }
                    else
                    {
                        savePath = null;
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: {f.InnerException}\n + Message: {f.Message}\n " +
                    $"Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
        }
        private void CalculateTimeBTN_Click(object sender, EventArgs e)
        {
            string fileInfo = new DirectoryInfo(filePath).GetFiles().OrderByDescending(file => file.Length).FirstOrDefault().FullName;

            Point viewPortLocation = new Point();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileInfo,
                WindowStyle = ProcessWindowStyle.Maximized,
            };
            

            using (Process theProcess = new Process { StartInfo = startInfo })
            {
                //I'm using local function thingie because otherwise nifskope won't open properly before the IUIAutomationElement tries to get instantiated. It'll throw an exception.
                theProcess.Start();
                void openAction(bool open)
                {
                    while (theProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        theProcess.WaitForInputIdle();
                    }

                }
                bool isOpen = false;
                openAction(isOpen);
                IntPtr nifskopeHandle = theProcess.MainWindowHandle;
                List<Point> buttonCenterPoints = new List<Point>();
                ControlsMethod(theProcess, nifskopeHandle, ref buttonCenterPoints);
                nifskopeBTNsCenter_LB.DataSource = buttonCenterPoints;
                
            }
            MessageBox.Show("Stuff has been calculated.");

        }
        private void ControlsMethod(Process theProcess, IntPtr nifskopeHandle, ref List<Point> buttonCenterPoints)
        {
            try
            {
                IUIAutomationElement nifskope = processInterface.ElementFromHandle(nifskopeHandle);
                int windowState = nifskope.GetCurrentPropertyValue(30076);
                while(windowState != 2)
                {
                    processStatus_LBL.Text = "Preparing to get buttons...";
                }
                processStatus_LBL.Text = "Getting buttons...";
                //Create an array of buttons.
                var controlTypeCondition = processInterface.CreatePropertyCondition(30003, 50000); //this is a condition that checks that the element in question is a button.
                IUIAutomationElementArray buttons = nifskope.FindAll(TreeScope.TreeScope_Descendants, controlTypeCondition);
                for (int i = 0; i < buttons.Length; i++)
                {
                    //get names of buttons. 
                    IUIAutomationElement button = buttons.GetElement(i);
                    string name = button.CurrentName;
                    if (name == null || name == string.Empty)
                        name = "Unspecified";
                    nifskopeBTNsNames_LB.Items.Add(name);
                    tagRECT buttonRect = button.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
                    Rectangle theRectangle = new Rectangle(buttonRect.left, buttonRect.right, (buttonRect.right - buttonRect.left), (buttonRect.bottom - buttonRect.top));
                    Point centerPoint = new Point(buttonRect.left, buttonRect.top);
                    

                }

            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source} \n+ StackTrace: {f.StackTrace} \n+ TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }

        }
        private void Start_BTN_Click(object sender, EventArgs e)
        {
            int waitB4 = int.Parse(waitB4_TB.Text);
            int waitAfter = int.Parse(waitAfter_TB.Text);
            if (NumbersValidated(waitB4, waitAfter) == true)
            {
                DialogResult result = MessageBox.Show("If you press yes, this program will automatically press the load view button and use the key shortcut for the center command foreach shortcut automatically. Is this ok?", "Question", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (BackgroundWorker1.IsBusy != true)
                    {

                        BackgroundWorker1.RunWorkerAsync();
                    }

                }
                else
                {
                    if (backgroundWorker2.IsBusy != true)
                    {
                        if (backgroundWorker2.IsBusy != true)
                        {
                            backgroundWorker2.RunWorkerAsync();
                        }
                    }
                }
            }
        }

        private async void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int waitB4 = int.Parse(waitB4_TB.Text);
            int waitAfter = int.Parse(waitAfter_TB.Text);
            if (savePath != null)
            {
                if (sufficientSpace == true)
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Maximized,

                    };
                    while (totalScreenshots != completedScreenshots)
                    {
                        DateTime startTime = DateTime.Now;
                        await pauseCompletionSource.Task;
                        using (Process myProcess = new Process { StartInfo = processStartInfo })
                        {
                            try
                            {
                                myProcess.StartInfo.FileName = (string)filePaths[completedScreenshots];
                                myProcess.Start();
                                myProcess.WaitForInputIdle();
                                Thread.Sleep(waitB4);
                                Task<bool> button = AutomaticButtonPress(myProcess);
                                bool buttonPress = await button;
                                Task<int> screenshot = ScreenshotMethod(savePath);
                                completedScreenshots = await screenshot;
                                Thread.Sleep(waitAfter);
                                DateTime screenshotDone = DateTime.Now;
                                TimeSpan screenshotTime = (screenshotDone - startTime);
                                int[] processInformation = CalculateRemainingScreenshotsAndTime(screenshotTime);
                                int screenshotsRemaining = processInformation[0];
                                int timeRemaining = processInformation[1];
                                Task<bool> ui = UpdateUI(screenshotsRemaining, timeRemaining);
                                bool updateDone = await ui;
                                myProcess.Kill();
                            }
                            catch (Exception f)
                            {
                                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source} \n+ StackTrace: {f.StackTrace} \n+ TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"It appears that you do not have enough space on the drive of your save folder. It appears you have {info[0]} bytes left, but you need {info[1]} bytes.");
                }
            }
            else
            {
                return;
            }
        }
        private Task<bool> AutomaticButtonPress(Process myProcess)
        {
            bool buttonPressed;
            try
            {
                IntPtr mainWindowHandle = myProcess.MainWindowHandle;
                CUIAutomation setFocus = new CUIAutomation();
                IUIAutomationElement windowElement = setFocus.ElementFromHandle(mainWindowHandle);
                SendKeys.SendWait("{F8}");
                SendKeys.SendWait("Z");
                buttonPressed = true;
            }
            catch (Exception f)
            {
                MessageBox.Show("Button stack trace:" + f.StackTrace + "Message" + f.Message + "Inner exception" + f.InnerException);
                buttonPressed = false;
            }
            return Task.FromResult(buttonPressed);
        }
        private Task<int> ScreenshotMethod(string savePath)
        {
            try
            {
                int xCoordinate = panel1.Width;
                int yCoordinate = topPanel.Height;
                Point coords = new Point(xCoordinate, yCoordinate);
                Point dest = new Point(0, 0);
                Bitmap nif = new Bitmap(theSize.Width, theSize.Height);
                Graphics graphicsNif = Graphics.FromImage(nif);
                graphicsNif.CopyFromScreen(coords, dest, theSize);
                string TreeNodeNameRevised = fileNames[completedScreenshots].ToString().Replace(".nif", string.Empty);
                nif.Save(savePath + "\\" + TreeNodeNameRevised + ".bmp", ImageFormat.Bmp);
                imageList1.ImageSize = screenshot_PB.Size;
                imageList1.Images.Add(nif);
                completedScreenshots++;
            }
            catch (Exception f)
            {
                MessageBox.Show("Button stack trace:" + f.StackTrace + "Message" + f.Message + "Inner exception" + f.InnerException);
            }
            return Task.FromResult(completedScreenshots);
        }
        private int[] CalculateRemainingScreenshotsAndTime(TimeSpan screenshotTime)
        {
            int screenshotsRemaining = totalScreenshots - completedScreenshots;
            int timeRemaining = (int)screenshotTime.TotalSeconds * screenshotsRemaining;
            int[] progressInformation = new int[2];
            progressInformation[0] = screenshotsRemaining;
            progressInformation[1] = timeRemaining;
            return progressInformation;
        }
        private Task<bool> UpdateUI(int screenshotsRemaining, int timeRemaining)
        {
            bool invokeRequired = false;
            if (InvokeRequired)
            {
                BeginInvoke((Action)(() =>
                {
                    screenshotNumber_LBL.Text = $"{screenshotsRemaining}";
                    timeLeft_LBL.Text = $"{timeRemaining:N} seconds";
                    completedScreenshots_LBL.Text = $"{completedScreenshots}";
                    invokeRequired = true;
                }));
            }
            else
            {
                screenshotNumber_LBL.Text = $"{screenshotsRemaining}";
                timeLeft_LBL.Text = $"{timeRemaining:N} seconds";
                completedScreenshots_LBL.Text = $"+{completedScreenshots}";
                invokeRequired = false;
            }
            return Task.FromResult(invokeRequired);
        }
        private void PauseAndPlay_BTN_Click(object sender, EventArgs e)
        {//This was supposed to be the method meant to enable the user to pause and the restart teh process where it left off....
            isPaused = !isPaused;
            if (isPaused)
            {
                processStatus_LBL.Text = "Paused";
                pauseCompletionSource = new TaskCompletionSource<bool>();
            }
            else
            {
                processStatus_LBL.Text = "Screenshotting;";
                pauseCompletionSource.TrySetResult(false);
            }
        }
        private void ScreenshotFiles_LB_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                BeginInvoke((Action)(() =>
                {
                    if (screenshotFiles_LB.SelectedIndex <= completedScreenshots)
                    {
                        Image img = imageList1.Images[screenshotFiles_LB.SelectedIndex];
                    }
                }));
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n +" +
                    $" InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: " +
                    $"{f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
        }
        private void Exit_BTN_Click(object sender, EventArgs e)
        {
            if (totalScreenshots != completedScreenshots)
            {
                totalScreenshots = completedScreenshots;
                Application.Exit();
            }
            else
            {
                MessageBox.Show("Please either wait until all the screenshots have been taken or pause the process prior to exiting Screenshot Auto Plus. Thanks.", "Error", MessageBoxButtons.OK);
            }
        }

        private async void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {

        }
        private void ListControls_BTN_Click(object sender, EventArgs e)
        {


        }
    }
}