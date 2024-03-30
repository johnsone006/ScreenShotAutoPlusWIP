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
        //This is a bool that I have been attempting to use to pause the screenshot process. In other words, make the program cease opening, screenshotting, and closing nifskope.
        private bool isPaused = false;
        //This line of code is meant to ensure that nifskope always opens on the same monitor. 
        private Rectangle monitorBounds = Screen.AllScreens[0].WorkingArea;
        //self explanatory
        int completedScreenshots;
        private int ssHeight;
        private int ssWidth;
        private int xCoord;
        private int yCoord;
        private int waitB4;
        private int waitAfter;
        private string filePath;
        private string savePath;

        int totalScreenshots;
        int screenshotsRemaining;
        int timeRemaining;
        int timeToWait;

        DateTime startTime;
        TimeSpan screenshotTime;
        bool buttonPressed = false;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            //This was implemented back when it was necessary for ScreenshotAuto to be the topmost app because the automatic button press relied on a transparent button. Microsofts website told me that if I were to position that button above another button in Nifksope and then set the backcolor to the transparency key and then press that transparent button, it'd be the same thing as pressing that button in nifskope.
            //It most certainly did NOT turn out that way.
            this.TopMost = true;


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
        bool NumbersValidated()
        {
            try
            {
                if (filePath_TB.TextLength != 0)
                {
                    DirectoryInfo fileDi = new DirectoryInfo(filePath_TB.Text);

                    if (!fileDi.Exists)
                    {
                        MessageBox.Show("The filepath you inserted for files to be screenshotted doesn't exist.", "Error", MessageBoxButtons.OK);
                        return false;
                    }
                    else
                    {
                        filePath = filePath_TB.Text;
                    }
                }
                else
                {
                    MessageBox.Show("Please provide a file path for the files you want screenshotted.", "Error", MessageBoxButtons.OK);
                    return false;
                }
                if (saveFilePath_TB.TextLength != 0)
                {
                    DirectoryInfo saveDI = new DirectoryInfo(saveFilePath_TB.Text);
                    if (!saveDI.Exists)
                    {
                        MessageBox.Show("The location you provided for the files to be saved at doesn't exist.", "Error", MessageBoxButtons.OK);
                        return false;
                    }
                    else
                    {
                        savePath = saveFilePath_TB.Text;
                    }
                }
                else
                {
                    MessageBox.Show("Please provide a file path for to save the screenshots at.", "Error", MessageBoxButtons.OK);
                    return false;
                }
                ssWidth = int.Parse(screenshotWidth_TB.Text);
                ssHeight = int.Parse(screenshotHeight_TB.Text);
                xCoord = int.Parse(xCoord_TB.Text);
                yCoord = int.Parse(yCoord_TB.Text);
                waitB4 = int.Parse(waitB4_TB.Text);
                waitAfter = int.Parse(waitAfter_TB.Text);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Inner exception:" + ex.InnerException + "Message" + ex.Message + "Source:" + ex.Source);
                return false;
            }
        }
        private void Import_BTN_Click(object sender, EventArgs e)
        {
            if (NumbersValidated() == true)
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
                catch
                {

                }
                //I'm implementing a try catch block here so I can make sure the filePath_LB is filled before I do this:
                totalScreenshots = filePath_LB.Items.Count;
            }
        }
        void PauseOperation()
        {
            if (isPaused == false)
            {
                return;
            }
            else
            {
                while (isPaused == true)
                {
                    Task.Delay(-1);
                }
            }
            return;

        }
        private  void CalculateTime_BTN_Click(object sender, EventArgs e)
        {//Method to figure out how long it would take for the largest file to load, have the button pressed, ald load the view;
            if (NumbersValidated() == true)
            {
                 
                try
                {
                    string fileInfo = new DirectoryInfo(filePath).GetFiles().OrderByDescending(file => file.Length).FirstOrDefault().FullName;
                    Process theProcess = new Process();
                    theProcess.StartInfo.FileName = fileInfo;
                    theProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    theProcess.Start();
                    DateTime startCalTime = DateTime.Now;
                    Thread.Sleep(waitB4);
                    Task<bool> buttonCalc = AutomaticButtonPress();
                    buttonCalc.Wait();
                    theProcess.WaitForInputIdle();
                    Thread.Sleep(500);
                    DateTime endCalTime = DateTime.Now;
                    theProcess.Kill();
                    timeToWait = (int)(endCalTime - startCalTime).TotalSeconds;
                    stateTxt.Text = timeToWait.ToString();
                }
                catch (Exception f)
                {
                    MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                }
                MessageBox.Show("Time calculated!");
            }
        }
        private async void Start_BTN_Click(object sender, EventArgs e)
        {
            if (NumbersValidated() == true)
            {
                DialogResult result = MessageBox.Show("If you press yes, this program will automatically click once on nifskope and then click the load view button in nifskope for every screenshot. Is this acceptable?", "Important question", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {

                    using (Process myProcess = new Process())
                    {
                        foreach (var file in filePath_LB.Items)
                        {
                            DateTime startTime = DateTime.Now;
                            myProcess.StartInfo.FileName = (string)filePath_LB.Items[completedScreenshots];
                            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                            myProcess.StartInfo.Arguments = $"-left {monitorBounds} -top {monitorBounds.Top} -width {monitorBounds.Width} -height {monitorBounds.Height}";
                            myProcess.Start();
                            Task<bool> button = AutomaticButtonPress();
                            buttonPressed = await button;
                            Thread.Sleep(2000);
                            Task<int> screenshot = ScreenshotMethod();
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
                            Thread.Sleep(500);
                            myProcess.Kill();
                        }
                    }
                }
                else if (result == DialogResult.No)
                {
                    DateTime startTime = DateTime.Now;
                    using (Process myProcess = new Process())
                    {
                        foreach (var file in filePath_LB.Items)
                        {
                            myProcess.StartInfo.FileName = (string)filePath_LB.Items[completedScreenshots];
                            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                            myProcess.StartInfo.Arguments = $"-left {monitorBounds} -top {monitorBounds.Top} -width {monitorBounds.Width} -height {monitorBounds.Height}";
                            myProcess.Start();
                            Thread.Sleep(waitB4);
                            Task<int> screenshot = ScreenshotMethod();
                            completedScreenshots = await screenshot;
                            Thread.Sleep(waitAfter);

                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }
        private Task<int> WaitTask()
        {
            Thread.Sleep(timeToWait);
            return Task.FromResult(timeToWait);
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
        private Task<int> ScreenshotMethod()
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
            catch (Exception e)
            {
                MessageBox.Show($"Data: {e.Data}\n HResult: + {e.HResult} \n helplink: {e.HelpLink}\n + InnerException: {e.InnerException}\n + Message: {e.Message}\n +Source: {e.Source} + StackTrace: {e.StackTrace} + TargetSite: {e.TargetSite}", "Error", MessageBoxButtons.OK);
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
                    timeLeft_LBL.Text = $"{timeRemaining:F2} seconds";
                    invokeRequired = true;
                }));
            }
            else
            {
                screenshotNumber_LBL.Text = $"{screenshotsRemaining}";
                timeLeft_LBL.Text = $"{timeRemaining:F2} seconds";
                invokeRequired = false;
            }
            return Task.FromResult(invokeRequired);
        }
        private void PauseAndPlay_BTN_Click(object sender, EventArgs e)
        {//This was supposed to be the method meant to enable the user to pause and the restart teh process where it left off....
            if (isPaused == true)
            {
                isPaused = false;
            }
            else if (isPaused == false)
            {
                isPaused = true;
            }
        }



    }
}
