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
        private Rectangle monitorBounds = Screen.AllScreens[0].WorkingArea;
        //self explanatory
        int completedScreenshots;
        private string filePath;
        private string savePath;
        int totalScreenshots;
        int screenshotsRemaining;
        int timeRemaining;
        readonly DateTime startTime;
        TimeSpan screenshotTime;
        bool buttonPressed;
        long spaceAvailable;
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
        }
        bool NumbersValidated(int ssHeight, int ssWidth, int xCoord, int yCoord, int waitB4, int waitAfter)
        {
            try
            {
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

            if (NumbersValidated(ssHeight, ssWidth, xCoord, yCoord, waitB4, waitAfter) == true)
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
            if (NumbersValidated(ssHeight, ssWidth, xCoord, yCoord, waitB4, waitAfter) == true)
            {
                DialogResult result = MessageBox.Show("If you press yes, this program will automatically " +
                    "click once on nifskope and then click the load view button in nifskope for every screenshot. " +
                    "Is this acceptable?", "Important question", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    Task<long> space = GetSpaceAvailability();
                    spaceAvailable = await space;
                    if (spaceAvailable != 0)
                    {
                        while (completedScreenshots != filePath_LB.Items.Count)
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
                                        Task<int> screenshot = ScreenshotMethod(ref ssWidth, ref ssHeight, ref xCoord, ref yCoord);
                                        completedScreenshots = await screenshot;
                                        DateTime screenshotDone = DateTime.Now;
                                        Thread.Sleep(waitAfter);
                                        screenshotTime = (screenshotDone - startTime);
                                        Task<int> remaining = CalculateRemainingScreenshots();
                                        screenshotsRemaining = await remaining;
                                        Task<int> time = EstimateTimeRemaining();
                                        timeRemaining = await time;
                                        Task<bool> ui = UpdateUI();
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
                        MessageBox.Show("Ooops! You have run out of room for screenshots!", "Error", MessageBoxButtons.OK);
                        return;
                    }
                }
                else if (result == DialogResult.No)
                {

                    using (Process myProcess = new Process())
                    {
                        foreach (var file in filePath_LB.Items)
                        {
                            DateTime startTime2 = DateTime.Now;
                            myProcess.StartInfo.FileName = (string)filePath_LB.Items[completedScreenshots];
                            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                            myProcess.StartInfo.Arguments = $"-left {monitorBounds} -top {monitorBounds.Top} " +
                                $"-width {monitorBounds.Width} -height {monitorBounds.Height}";
                            myProcess.Start();
                            Task<int> screenshot = ScreenshotMethod(ref ssWidth, ref ssHeight, ref xCoord, ref yCoord);
                            completedScreenshots = await screenshot;
                            DateTime screenshotDone = DateTime.Now;
                            Thread.Sleep(waitAfter);
                            screenshotTime = (screenshotDone - startTime2);
                            Task<int> remaining = CalculateRemainingScreenshots();
                            screenshotsRemaining = await remaining;
                            Task<int> time = EstimateTimeRemaining();
                            timeRemaining = await time;
                            Task<bool> ui = UpdateUI();
                            bool invokeRequired = await ui;
                            Thread.Sleep(500);
                            myProcess.Kill();

                        }
                    }
                }
                else
                {
                    return;
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
        private Task<int> ScreenshotMethod(ref int ssWidth, ref int ssHeight, ref int xCoord, ref int yCoord)
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
        private Task<int> CalculateRemainingScreenshots()
        {
            int screenshotsRemaining = totalScreenshots - completedScreenshots;
            return Task.FromResult(screenshotsRemaining);
        }
        private Task<int> EstimateTimeRemaining()
        {

            timeRemaining = (int)screenshotTime.TotalSeconds * screenshotsRemaining;
            return Task.FromResult(timeRemaining);
        }
        private Task<bool> UpdateUI()
        {
            bool invokeRequired = false;
            if (InvokeRequired)
            {
                BeginInvoke((Action)(() =>
                {
                    screenshotNumber_LBL.Text = $"{screenshotsRemaining}";
                    timeLeft_LBL.Text = $"{timeRemaining:N} seconds";
                    invokeRequired = true;
                }));
            }
            else
            {
                screenshotNumber_LBL.Text = $"{screenshotsRemaining}";
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
            //MAKE ABSOLUTELY SURE TO MAKE THE VALUE OF WAITB4 to wT!!
            if (NumbersValidated(ssHeight, ssWidth, xCoord, yCoord, waitB4, waitAfter) == true)
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
    }
}
