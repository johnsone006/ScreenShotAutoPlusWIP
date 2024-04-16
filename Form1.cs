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
        long[] info = new long[2];
        readonly List<string> backendFilePathList = new List<string>();
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.TopMost = true;
            pauseCompletionSource = new TaskCompletionSource<bool>();
            pauseCompletionSource.SetResult(false);
            

            BackgroundWorker1.WorkerReportsProgress = false;
            BackgroundWorker1.WorkerSupportsCancellation = false;
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
            if (NumbersValidated(waitB4, waitAfter) == true && OpenAndValidateFP(ref filePath)==true)
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
                            fileName_LB.Items.Add(file.Name);

                            screenshotFiles_LB.Items.Add((screenshotFiles_LB.Items.Count+1) +". "+ file.Name);
                            backendFilePathList.Add(file.FullName);
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
                }
            }
            totalScreenshots = fileName_LB.Items.Count;
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
             Size topPanel_OS = new Size(topPanel.Width, topPanel.Height);
             Size panel1_OS = new Size(panel1.Width, panel1.Height); 
            int waitB4 = 0;
            int waitAfter = 0;
            int xCoord = 0;
            int yCoord = 0;
            if (NumbersValidated(waitB4, waitAfter) == true)
            {
                string fileInfo = new DirectoryInfo(filePath).GetFiles().OrderByDescending(file => file.Length).FirstOrDefault().FullName;
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = fileInfo,
                    WindowStyle = ProcessWindowStyle.Maximized,
                };
                using (Process theProcess = new Process { StartInfo = startInfo })
                {
                    try
                    {
                        theProcess.Start();
                        theProcess.WaitForInputIdle();
                        Thread.Sleep(1000);
                        Bitmap initialSS = FirstScreenshot();
                        Color bingoColor = ConvertHex();
                        int[] coords = CalculateCoords(ref xCoord, ref yCoord, initialSS, bingoColor);
                        int[] ssSize = ResizeUI(coords[0], coords[1], panel1_OS, topPanel_OS);
                        theSize = CalculateScreenshotSize(ssSize[0], ssSize[1], initialSS);
                        VerifySpaceAmount();
                        initialSS.Dispose();
                    }
                    catch (Exception f)
                    {
                        MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + " +
                        $"InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n +" +
                        $" StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                    }
                }
                MessageBox.Show("Stuff has been calculated, so after you close out the window of Nifskope that opened, you can press the start button now.");
            }
        }
        private Bitmap FirstScreenshot()
        {
            int initialWidth = this.Width; //This will work because I set the window state of rthe form to be maximized.
            int initialHeight = this.Height;
            Bitmap initialSS = new Bitmap(initialWidth, initialHeight);
            Graphics firstSSGraphic = Graphics.FromImage(initialSS);
            Size size = new Size(initialWidth, initialHeight);
            firstSSGraphic.CopyFromScreen(0, 0, 0, 0, size);
            return initialSS;
        }
        private Color ConvertHex()
        {
            string hexadecimalNumber = colorCode_TB.Text;
            if (hexadecimalNumber.StartsWith("#"))
                hexadecimalNumber = hexadecimalNumber.Substring(1);
            int r = Convert.ToInt32(hexadecimalNumber.Substring(0, 2), 16);
            int g = Convert.ToInt32(hexadecimalNumber.Substring(2, 2), 16);
            int b = Convert.ToInt32(hexadecimalNumber.Substring(4, 2), 16);
            Color bingoColor = Color.FromArgb(255, r, g, b);
            return bingoColor;
        }
        private int[] CalculateCoords(ref int xCoord, ref int yCoord, Bitmap initialSS, Color bingoColor)
        {
            Color menuColor;
            int tempY = 2;
            try
            {
                int starterX = initialSS.Width - 5;

                //Figure out y coordinate of location for future screenshots
                for (tempY = 2; tempY < initialSS.Height;)
                {
                    tempY++;
                    menuColor = initialSS.GetPixel(starterX, tempY);
                    if (bingoColor.Equals(menuColor))
                    {
                        yCoord = tempY;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                //Figure out x coordinate of location for future screenshots
                tempY += 5;
                for (int tempX = 0; tempX < initialSS.Width;)
                {
                    tempX++;
                    menuColor = initialSS.GetPixel(tempX, tempY);
                    if (bingoColor.Equals(menuColor))
                    {
                        xCoord = tempX;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n +" +
                   $" InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: " +
                   $"{f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite} \n {tempY}\n{bingoColor}\n {tempY}", "Error", MessageBoxButtons.OK);
            }
            int[] coords = { xCoord, yCoord };
            return coords;
        }
        private Size CalculateScreenshotSize(int width, int height, Bitmap initialSS)
        {
            int ssWidth = initialSS.Width - width;
            int ssHeight = initialSS.Height - height;
            Size theSize = new Size(ssWidth, ssHeight);
            return theSize;
        }
        private int[] ResizeUI(int xCoord, int yCoord, Size panel1_OS, Size topPanel_OS)
        {
            Screen thisScreen = Screen.FromControl(this);
            Rectangle workingArea = thisScreen.WorkingArea;
            float xMultiply = xCoord / panel1_OS.Width; //This is how much the size of all controls (except topPanel) will change in width.
            float yMultiply = yCoord / topPanel_OS.Height;//this is how much the size of all controls (except topPanel) will change in height.
            if (yCoord == topPanel_OS.Height)
            {
                if (xCoord == panel1_OS.Width)
                {
                    //Somehow, everything is sized perfectly already so change nothing.

                }
                else
                {
                    foreach (Control con in panel1.Controls)
                    {
                        con.Anchor = AnchorStyles.Left & AnchorStyles.Top;
                        int newWidth = (int)(con.Width * xMultiply);
                        int newHeight = (int)(con.Height * yMultiply);
                        con.Size = new Size(newWidth, newHeight);
                    }
                }
                int height = workingArea.Height - yCoord;
                panel1.Size = new Size(xCoord, height);
                panel1.Location = new Point(0, yCoord);
            }
            else
            {
                //height needs to change. 
                if (xCoord == panel1_OS.Width)
                {
                    //only height needs to change. 
                    foreach (Control con in panel1.Controls)
                    {

                        con.Anchor = AnchorStyles.Top;
                        int newHeight = (int)(con.Height * yMultiply);
                        con.Size = new Size(con.Width, newHeight);

                    }
                    topPanel.Height = yCoord;
                    int height = (topPanel_OS.Height + panel1_OS.Height) - yCoord;
                    panel1.Size = new Size(xCoord, height);
                }
                else
                {
                    //Both height and width of panel1 needs to change.
                    foreach (Control con in panel1.Controls)
                    {
                        con.Anchor = AnchorStyles.Left & AnchorStyles.Top;
                        int newHeight = (int)(con.Height * yMultiply);
                        int newWidth = (int)(con.Width * xMultiply);
                        con.Size = new Size(newWidth, newHeight);
                    }
                    topPanel.Height = yCoord;
                    int height = this.Height - yCoord;
                    panel1.Size = new Size(xCoord, height);
                }
            }
            int[] newSizes = new int[2];
            newSizes[0] = panel1.Width;
            newSizes[1] = topPanel.Height;
            return newSizes;
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
            int screenshotsLeft;
           
            if (savePath != null)
            {
                if(sufficientSpace == true)
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Maximized,
                        FileName = (string)backendFilePathList[completedScreenshots],
                    };
                    while (totalScreenshots != completedScreenshots)
                    {
                        DateTime startTime = DateTime.Now;
                        await pauseCompletionSource.Task;
                        using (Process myProcess = new Process { StartInfo = processStartInfo })
                        {
                            try
                            {
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
                                int screenshotsRemaining = CalculateRemainingScreenshots();
                                screenshotsLeft = screenshotsRemaining;
                                int timeRemaining = CalculateRemainingTime(screenshotsRemaining, screenshotTime);
                                Task<bool> ui = UpdateUI(screenshotsRemaining, timeRemaining, screenshotsLeft);
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
        private void VerifySpaceAmount()
        {
                int bitsPerPixel = Screen.FromControl(this).BitsPerPixel;
                int bytesPerPixel = bitsPerPixel / 8;
                int bytesPerRow = theSize.Width * bytesPerPixel;
                int paddingPerRow = (4 - (bytesPerRow % 4)) % 4;
                int totalRowBytes = (bytesPerRow + paddingPerRow) * theSize.Height;
                int headerSize = 54;
                long totalFileSize = headerSize + totalRowBytes;
                string driveRoot = Path.GetPathRoot(savePath);
                DriveInfo driveInfo = new DriveInfo(driveRoot);
                long spaceLeft = driveInfo.AvailableFreeSpace;
                long totalSpaceRequired = totalFileSize * totalScreenshots;
                if (spaceLeft >= totalSpaceRequired)
                    sufficientSpace = true;
                else
                    sufficientSpace = false;
 
            info[0] = spaceLeft;
            info[1] = totalSpaceRequired;
 
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
                string TreeNodeNameRevised = fileName_LB.Items[completedScreenshots].ToString().Replace(".nif", string.Empty);
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
        private int CalculateRemainingScreenshots()
        {
            int screenshotsRemaining = totalScreenshots - completedScreenshots;
            return screenshotsRemaining;
        }
        private int CalculateRemainingTime(int screenshotsRemaining, TimeSpan screenshotTime)
        {
            int timeRemaining = (int)screenshotTime.TotalSeconds * screenshotsRemaining;
            return timeRemaining;
        }
        private Task<bool> UpdateUI(int screenshotsRemaining, int timeRemaining, int screenshotsLeft)
        {
            screenshotsLeft = screenshotsRemaining;

            bool invokeRequired = false;
            if (InvokeRequired)
            {
                BeginInvoke((Action)(() =>
                {
                    screenshotNumber_LBL.Text = $"+ {screenshotsLeft}";
                    timeLeft_LBL.Text = $"+{timeRemaining:N} seconds";
                    completedScreenshots_LBL.Text = $"+{completedScreenshots}";
                    invokeRequired = true;
                }));
            }
            else
            {
                screenshotNumber_LBL.Text = $"+ {screenshotsLeft}";
                timeLeft_LBL.Text = $"+{timeRemaining:N} seconds";
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
                if(screenshotFiles_LB.SelectedIndex <= completedScreenshots)
                {
                    Image img = imageList1.Images[screenshotFiles_LB.SelectedIndex];
                    screenshot_PB.Image = img;
                }
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
    }
}