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

namespace ScreenShotAutoPlus
{
    public partial class Form1 : Form
    {
        //This is a bool that I have been attempting to use to pause the screenshot process.
        //In other words, make the program cease opening, screenshotting, and closing nifskope.
        private bool isPaused;
        //This line of code is meant to ensure that nifskope always opens on the same monitor. 
        //self explanatory
        int completedScreenshots;
        private string savePath;
        int totalScreenshots;
        int timeRemaining;
  
        TimeSpan screenshotTime;
        bool buttonPressed;
     
        
        private TaskCompletionSource<bool> pauseCompletionSource;
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            /*This was implemented back when it was necessary for ScreenshotAuto to
            be the topmost app because the automatic button press relied on a transparent button.
            Microsofts website told me that if I were to position that button above another button in Nifksope
            and then set the backcolor to the transparency key and then press that transparent button,
            it'd be the same thing as pressing that button in nifskope.
            It most certainly did NOT turn out that way.*/
            this.TopMost = true;
            pauseCompletionSource = new TaskCompletionSource<bool>();
            pauseCompletionSource.SetResult(false);
            toolTip1.SetToolTip(filePath_TB, "Textbox to type the file path of the folder containing the files you want to have screenshoted.");
            toolTip1.SetToolTip(saveFilePath_TB, "Textbox to type the file path of the folder you want all the screenshots to be saved to.");
            toolTip1.SetToolTip(screenshotWidth_TB, "Textbox to type the width of the screenshots.");
            toolTip1.SetToolTip(screenshotHeight_TB, "Textbox to type the height of the screenshots.");
            toolTip1.SetToolTip(xCoord_TB, "Textbox to type the xCoordinate for screenshoot.");
            toolTip1.SetToolTip(yCoord_TB, "Textbox to type the y coordinate for screenshot.");
            toolTip1.SetToolTip(waitB4_TB, "Textbox to type the amount of miliseconds you want the program to wait prior to each screenshot (to convert seconds to miliseconds, multiply seconds by 1000");
            toolTip1.SetToolTip(waitAfter_TB, "Textbox to type the amount of miliseconds you want the program to wait after each screenshot.");
            imageList1.ImageSize = new Size(screenshot_PB.Width, screenshot_PB.Height);
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            filePath_TB.Text = "C:\\Users\\Johns\\Desktop\\SkyrimSEArchitecture\\meshes\\architectureInSameFolder";
            saveFilePath_TB.Text = "C:\\Users\\Johns\\Pictures\\NifImages\\NifImagesTest";
            screenshotWidth_TB.Text = "1512";
            screenshotHeight_TB.Text = "922";
            xCoord_TB.Text = "408";
            yCoord_TB.Text = "93";
            waitB4_TB.Text = "2000";
            waitAfter_TB.Text = "2000";
            filePath_LB.BackColor = System.Drawing.Color.White;
        }
        bool NumbersValidated(int ssHeight, int ssWidth, int xCoord, int yCoord, int waitB4, int waitAfter, Rectangle monitorBounds, string filePath)
        {
            try
            {
                monitorBounds = Screen.AllScreens[0].WorkingArea;
                /*Since these values are going to be used to take a screeneshot, 
                I should make sure that the user doesn't input any values that would mean the rectangle would extend past the monitor bounds.*/
                if (int.TryParse(screenshotHeight_TB.Text, out int height) == true && int.TryParse(screenshotWidth_TB.Text, out int width) == true && int.TryParse(xCoord_TB.Text, out int xC) == true && int.TryParse(yCoord_TB.Text, out int yC) == true)
                {
                    if (height <= monitorBounds.Height && width <= monitorBounds.Width && xCoord <= monitorBounds.Width &&
                           yCoord <= monitorBounds.Height)
                    {
                        height = ssHeight;
                        width = ssWidth;
                        xC = xCoord;
                        yC = yCoord;
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong because you did not satisfy one or more of these conditions: \n 1. the width and x " +
                            "coordinate of the screenshot must be smaller than or equal to the resolution width of the monitor the screenshot " +
                            "will be taken on; \n 2. The height and y coordinate of the screenshot must be smaller than or equal to " +
                            "the height resolution of the monitor the screenshot will be taken on.");
                    }
                }
                else
                {
                    return false;
                }
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
                DirectoryInfo fileD = new DirectoryInfo(filePath_TB.Text);
                DirectoryInfo saveD = new DirectoryInfo(saveFilePath_TB.Text);
                if (fileD.Exists)
                {
                    filePath = fileD.FullName;
                }
                else
                {
                    return false;
                }
                if (saveD.Exists)
                {
                    savePath = saveD.FullName;
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
            int ssHeight = 0;
            int ssWidth = 0;
            int xCoord = 0;
            int yCoord = 0;
            int waitB4 = 0;
            int waitAfter = 0;
            Rectangle monitorBounds = Screen.AllScreens[0].WorkingArea;
            string filePath = filePath_TB.Text;
            if (NumbersValidated(ssHeight, ssWidth, xCoord, yCoord, waitB4, waitAfter, monitorBounds, filePath) == true)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(filePath);
                    foreach (var file in di.GetFiles())
                    {
                        fileName_LB.Items.Add(file.Name);
                        filePath_LB.Items.Add(file.FullName);
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + " +
                        $"InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + " +
                        $"StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                }

                //I'm implementing a try catch block here so I can make sure the filePath_LB is filled before I do this:
                totalScreenshots = filePath_LB.Items.Count;
            }
        }
       
        private async void Start_BTN_Click(object sender, EventArgs e)
        {   
            int ssHeight = int.Parse(screenshotHeight_TB.Text);
            int ssWidth = int.Parse(screenshotWidth_TB.Text);
            int xCoord = int.Parse(xCoord_TB.Text);
            int yCoord = int.Parse(yCoord_TB.Text);
            int waitB4 = int.Parse(waitB4_TB.Text);
            int waitAfter = int.Parse(waitAfter_TB.Text);
            Rectangle monitorBounds = Screen.AllScreens[0].WorkingArea;
            int screenshotsLeft;
            string filePath = filePath_TB.Text;
            if (NumbersValidated(ssHeight, ssWidth, xCoord, yCoord, waitB4, waitAfter, monitorBounds, filePath) == true)
            {
                DialogResult result = MessageBox.Show("If you press yes, this program will automatically " +
                    "click once on nifskope and then click the load view button in nifskope for every screenshot. " +
                    "Is this acceptable?", "Important question", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    for (int i = 0; i< totalScreenshots; i++)
                    {
                        Task<long> space = GetSpaceAvailability();
                        long spaceAvailable = await space;
                        if (spaceAvailable != 0)
                        {
                            await pauseCompletionSource.Task;
                            using (Process myProcess = new Process())
                            {
                                try
                                {

                                    DateTime startTime = DateTime.Now;
                                    myProcess.StartInfo.FileName = (string)filePath_LB.Items[completedScreenshots];
                                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                                    myProcess.StartInfo.Arguments = $"-left {monitorBounds} -top {monitorBounds.Top} -width {monitorBounds.Width} -height {monitorBounds.Height}";
                                    myProcess.Start();
                                    Thread.Sleep(waitB4);
                                    Task<bool> button = AutomaticButtonPress();
                                    buttonPressed = await button;
                                    Task<int> screenshot = ScreenshotMethod(ref ssWidth, ref ssHeight, ref xCoord, ref yCoord, ref startTime);
                                    completedScreenshots = await screenshot;
                                    DateTime screenshotDone = DateTime.Now;
                                    Thread.Sleep(waitAfter);
                                    screenshotTime = (screenshotDone - startTime);
                                    int screenshotsRemaining = CalculateRemainingScreenshots();
                                    int time = EstimateTimeRemaining(ref screenshotsRemaining);
                                    screenshotsLeft = screenshotsRemaining;
                                    Task<bool> ui = UpdateUI(ref screenshotsRemaining, ref completedScreenshots, screenshotsLeft);
                                    bool updateDone = await ui;
             
                                    myProcess.Kill();
                                }
                                catch (Exception f)
                                {
                                    MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source} \n+ StackTrace: {f.StackTrace} \n+ TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ooops! You have run out of room for screenshots!", "Error", MessageBoxButtons.OK);
                            return;
                        }
                    }
                }
                else if (result == DialogResult.No)
                {
                    while (completedScreenshots != totalScreenshots)
                    {
                        Task<long> space = GetSpaceAvailability();
                        long spaceAvailable = await space;
                        if (spaceAvailable != 0)
                        {
                            await pauseCompletionSource.Task;
                            using (Process myProcess = new Process())
                            {
                                try
                                {
                                    DateTime startTime = DateTime.Now;
                                    myProcess.StartInfo.FileName = (string)filePath_LB.Items[completedScreenshots];
                                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                                    myProcess.StartInfo.Arguments = $"-left {monitorBounds} -top {monitorBounds.Top} -width {monitorBounds.Width} -height {monitorBounds.Height}";
                                    myProcess.Start();
                                    Thread.Sleep(waitB4);
                                    Task<int> screenshot = ScreenshotMethod(ref ssWidth, ref ssHeight, ref xCoord, ref yCoord, ref startTime);
                                    completedScreenshots = await screenshot;
                                    DateTime screenshotDone = DateTime.Now;
                                    Thread.Sleep(waitAfter);
                                    screenshotTime = (screenshotDone - startTime);
                                    int screenshotsRemaining = CalculateRemainingScreenshots();
                                    screenshotsLeft = screenshotsRemaining;
                                    int time= EstimateTimeRemaining(ref screenshotsRemaining);
                                    Task<bool> ui = UpdateUI(ref screenshotsRemaining, ref completedScreenshots, screenshotsLeft);
                                    bool updateDone = await ui;
                                    myProcess.Kill();
                                }
                                catch (Exception f)
                                {
                                    MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source} \n+ StackTrace: {f.StackTrace} \n+ TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ooops! You have run out of room for screenshots!", "Error", MessageBoxButtons.OK);
                            return;
                        }
                    }
                }
            }
        }
        Task<long> GetSpaceAvailability()
        {
            string driveRoot = Path.GetPathRoot(savePath);
            DriveInfo driveInfo = new DriveInfo(driveRoot);
            long spaceLeft = driveInfo.AvailableFreeSpace;
            return Task.FromResult(spaceLeft);
        }
        private Task<bool> AutomaticButtonPress()
        {
            try
            {
                MouseEventArgs args = new MouseEventArgs(MouseButtons.Left, 1, 810, 533, 0);
                OnMouseDown(args);
                OnMouseUp(args);
                SendKeys.SendWait("{F8}");
                buttonPressed = true;
            }
            catch (Exception f)
            {
                MessageBox.Show("Button stack trace:" + f.StackTrace + "Message" + f.Message + "Inner exception" + f.InnerException);
                buttonPressed = false;
            }
            return Task.FromResult(buttonPressed);
        }
        private Task<int> ScreenshotMethod(ref int ssWidth, ref int ssHeight, ref int xCoord, ref int yCoord, ref DateTime startTime)
        {
            try
            {
                Point coords = new Point(xCoord, yCoord);
                Point dest = new Point(0, 0);
                Size size = new Size(ssWidth, ssHeight);
                Rectangle rect = new Rectangle(coords, size);
                Bitmap nif = new Bitmap(ssWidth, ssHeight);   
                Graphics graphicsNif = Graphics.FromImage(nif);
                graphicsNif.CopyFromScreen(coords, dest, size);
                string TreeNodeNameRevised = fileName_LB.Items[completedScreenshots].ToString().Replace(".nif", string.Empty);
                nif.Save(savePath + "\\" + TreeNodeNameRevised + ".bmp", ImageFormat.Bmp);
                imageList1.Images.Add(nif);
                screenshotTime = (DateTime.Now - startTime);
                completedScreenshots++;
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n +" +
                    $" InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: " +
                    $"{f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
            return Task.FromResult(completedScreenshots);
        }
        private int CalculateRemainingScreenshots()
        {
           int screenshotsRemaining = totalScreenshots - completedScreenshots;
            return screenshotsRemaining;
        }
        private int EstimateTimeRemaining(ref int screenshotsRemaining)
        {
            timeRemaining = (int)screenshotTime.TotalSeconds * screenshotsRemaining;
            return timeRemaining;
        }
        private Task<bool> UpdateUI(ref int screenshotsRemaining, ref int completedScreenshots, int screenshotsLeft)
        {
            screenshotsLeft = screenshotsRemaining;
            string screenshotFile = fileName_LB.Items[completedScreenshots].ToString();
            screenshotFiles_LB.Items.Add(screenshotFile);
    
            bool invokeRequired = false;
            if (InvokeRequired)
            {
                BeginInvoke((Action)(() =>
                {
                    screenshotNumber_LBL.Text = $"{screenshotsLeft}";
                    timeLeft_LBL.Text = $"{timeRemaining:N} seconds";
                    invokeRequired = true;
                }));
            }
            else
            {
                screenshotNumber_LBL.Text = $"{screenshotsLeft}";
                timeLeft_LBL.Text = $"{timeRemaining:N} seconds";
                invokeRequired = false;
            }
            return Task.FromResult(invokeRequired);
        }
        private void PauseAndPlay_BTN_Click(object sender, EventArgs e)
        {//This was supposed to be the method meant to enable the user to pause and the restart teh process where it left off....
            isPaused = !isPaused;
            if(isPaused)
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
        private async void CalculateTimeBTN_Click(object sender, EventArgs e)
        {
            int ssHeight = 0;
            int ssWidth = 0;
            int xCoord = 0;
            int yCoord = 0;
            int waitB4 = 0;
            int waitAfter = 0;
            DateTime startCalcTime;
            DateTime endCalcTime;
            string filePath = filePath_TB.Text;
            Rectangle monitorBounds = Screen.AllScreens[0].WorkingArea;
            //MAKE ABSOLUTELY SURE TO MAKE THE VALUE OF WAITB4 to wT!!
            if (NumbersValidated(ssHeight, ssWidth, xCoord, yCoord, waitB4, waitAfter, monitorBounds, filePath) == true)
            {
                try
                {
                    string fileInfo = new DirectoryInfo(filePath).GetFiles().OrderByDescending(file => file.Length).FirstOrDefault().FullName;
                    using (Process theProcess = new Process())
                    {
                        theProcess.StartInfo.FileName = fileInfo;
                        theProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        theProcess.Start();
                        startCalcTime = DateTime.Now;
                        Thread.Sleep(waitAfter);
                        await AutomaticButtonPressForCalculation();

                        theProcess.WaitForInputIdle();
                        Thread.Sleep(500);
                        endCalcTime = DateTime.Now;
                        theProcess.Kill();
                    }
                    int wT = (int)(endCalcTime - startCalcTime).TotalSeconds;
                    stateTxt.Text = wT.ToString();
                }
                catch (Exception f)
                {
                    MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + " +
                        $"InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n +" +
                        $" StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                }
                MessageBox.Show("Time calculated!");
            }
        }
        private Task AutomaticButtonPressForCalculation()
        {
            try
            {
                MouseEventArgs args2 = new MouseEventArgs(MouseButtons.Left, 1, 810, 533, 0);
                OnMouseDown(args2);
                OnMouseUp(args2);
                SendKeys.SendWait("{F8}");

            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n +" +
                    $" InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: " +
                    $"{f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);

            }
            return Task.CompletedTask;
        }

        private void screenshotFiles_LB_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                Image img = imageList1.Images[screenshotFiles_LB.SelectedIndex];
                screenshot_PB.Image  = img; 
                
            }
            catch(Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n +" +
                    $" InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: " +
                    $"{f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
        }
    }
}
