using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics.Eventing.Reader;
using System.Dynamic;
using System.Windows.Automation;
using System.Media;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Remoting.Messaging;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Windows;
using System.Text;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;

namespace ScreenShotAutoPlus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.TopMost = true;
            BackgroundWorker1.WorkerReportsProgress = false;
            BackgroundWorker1.WorkerSupportsCancellation = false;
        }
        TaskCompletionSource<bool> pauseCompletionSource = new TaskCompletionSource<bool>();

        private bool isPaused;
        string savePath;
        string filePath;
        AutomationElement subElement;
        AutomationEventHandler handlerEvent;
        AutomationPropertyChangedEventHandler propertyChanged;
        AutomationFocusChangedEventHandler focusChanged;
        IntPtr handle;

        private void Form1_Load(object sender, EventArgs e)
        {
            //I have officially gotten tired of typing in my values every time I hit debug. So the next statement will only remain here until I get this program working correctly.

            //the rest of the code in this block is what fixed the fact that the ui tended to sometimes either overlap or be overlaped by the taskbar.
            System.Drawing.Size formSize = new System.Drawing.Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = formSize;
            this.Location = new System.Drawing.Point(0, 0);
        }
        private string ValidateFilePath()
        {
            if (this.InvokeRequired)
            {
                string localFilePath = null;
                this.Invoke((MethodInvoker)(() =>
                {
                    using (FolderBrowserDialog fBD = new FolderBrowserDialog())
                    {
                        fBD.Description = "Please select the folder that contains all the files you wish to screenshot.";
                        fBD.ShowNewFolderButton = false;
                        DialogResult result = fBD.ShowDialog();
                        if (result == DialogResult.OK && !string.IsNullOrEmpty(fBD.SelectedPath))
                        {
                            localFilePath = fBD.SelectedPath;
                        }
                        else
                        {
                            localFilePath = null;

                        }
                    }
                }));
                filePath = localFilePath;
                return filePath;
            }
            else
            {
                using (FolderBrowserDialog fBD = new FolderBrowserDialog())
                {
                    fBD.Description = "Please select the folder that contains all the files you wish to screenshot.";
                    fBD.ShowNewFolderButton = false;
                    DialogResult result = fBD.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrEmpty(fBD.SelectedPath))
                    {
                        filePath = fBD.SelectedPath;
                    }
                    else
                    {
                        filePath = null;

                    }
                }
            }
            return filePath;
        }
        private bool MakeFileLists(ref int totalScreenshots, ref List<string> fileNames, ref List<string> filePaths)
        {
            //We are going to have to call this method twice, since one of the methods that needs it is the import method and the other one is the background worker do work
            //and we shouldn't use anything from the UIAutomationClient namespace on the Ui thread.
            fileNames = new List<string>();
            filePaths = new List<string>();
            try
            {
                DirectoryInfo fileInfo = new DirectoryInfo(filePath);
                if (this.InvokeRequired)
                {
                    List<string> localFileNames = new List<string>();
                    List<string> localFilePaths = new List<string>();
                    this.Invoke((MethodInvoker)(() =>
                    {
                        foreach (var file in fileInfo.GetFiles())
                        {
                            localFileNames.Add(file.Name);
                            localFilePaths.Add(file.FullName);
                        }
                    }));
                    fileNames = localFileNames;
                    filePaths = localFilePaths;
                }
                else
                {
                    foreach (var file in fileInfo.GetFiles())
                    {
                        fileNames.Add(file.Name);
                        filePaths.Add(file.FullName);
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: " +
                       $"{f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + " +
                       $"TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                return false;
            }
            totalScreenshots = filePaths.Count;
            return true;
        }
        private string ValidateSavePath(ref string savePath)
        {
            if (this.InvokeRequired)
            {
                string localSavePath = string.Empty;
                this.Invoke((MethodInvoker)(() =>
                {

                    try
                    {
                        //Get save path.
                        using (FolderBrowserDialog fBD2 = new FolderBrowserDialog())
                        {

                            fBD2.Description = "Select a folder to save the screenshots to.";
                            fBD2.ShowNewFolderButton = false;
                            DialogResult result2 = fBD2.ShowDialog();
                            if (result2 == DialogResult.OK && !string.IsNullOrEmpty(fBD2.SelectedPath))
                            {
                                localSavePath = fBD2.SelectedPath;

                            }
                            else
                            {
                                localSavePath = null;
                            }
                        }
                    }
                    catch (Exception f)
                    {
                        MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: " +
                       $"{f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + " +
                       $"TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                    }
                }));
                savePath = localSavePath;
            }
            else
            {
                try
                {
                    //Get save path.
                    using (FolderBrowserDialog fBD2 = new FolderBrowserDialog())
                    {

                        fBD2.Description = "Select a folder to save the screenshots to.";
                        fBD2.ShowNewFolderButton = false;
                        DialogResult result2 = fBD2.ShowDialog();
                        if (result2 == DialogResult.OK && !string.IsNullOrEmpty(fBD2.SelectedPath))
                        {
                            savePath = fBD2.SelectedPath;
                        }
                        else
                        {
                            savePath = null;
                        }
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: " +
                   $"{f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + " +
                   $"TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                }
            }
            return savePath;
        }
        private bool ValidateNumbers(ref int waitAfter, ref int waitB4, ref int bytesPerPixel)
        {
            bool localValid = true;
            if (InvokeRequired)
            {
                int localWaitAfter = 0;
                int localWaitB4 = 0;
                int localBytesPerPixel = 0;

                this.Invoke((MethodInvoker)(() =>
                {

                    if (int.TryParse(waitAfter_TB.Text, out int waitA) == true && int.TryParse(waitB4_TB.Text, out int waitB) == true)
                    {
                        if (waitA >= 0 && waitB >= 0)
                        {
                            localWaitAfter = waitA;
                            localWaitB4 = waitB;
                        }
                    }
                    else
                    {
                        localValid = false;
                    }

                    //Get bits per pixel.
                    int localBitsPerPixel = Screen.FromControl(this).BitsPerPixel;
                    localBytesPerPixel = localBitsPerPixel / 8;

                }));
                if (localValid == false)
                {
                    return false;
                }
                bytesPerPixel = localBytesPerPixel;
                waitAfter = localWaitAfter;
                waitB4 = localWaitB4;
            }
            else
            {
                if (int.TryParse(waitAfter_TB.Text, out int waitA) == true && int.TryParse(waitB4_TB.Text, out int waitB) == true)
                {
                    if (waitA >= 0 && waitB >= 0)
                    {
                        waitAfter = waitA;
                        waitB4 = waitB;
                    }
                }
                else
                {
                    localValid = false;
                }

                //Get bits per pixel.
                int bitsPerPixel = Screen.FromControl(this).BitsPerPixel;
                bytesPerPixel = bitsPerPixel / 8;
            }
            return true;
        }
        private void Import_BTN_Click(object sender, EventArgs e)
        {
            if (ValidateFilePath() != null)
            {
                int invalidFiles = 0;
                try
                {
                    DirectoryInfo di = new DirectoryInfo(filePath);
                    //Make sure it's nif files that are being added.
                    foreach (var file in di.GetFiles())
                    {
                        if (file.Extension == ".nif")
                        {
                            screenshotFiles_LB.Items.Add((screenshotFiles_LB.Items.Count + 1) + ". " + file.Name);
                        }
                        else
                        {
                            invalidFiles++;
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
                    if (invalidFiles > 0)
                    {
                        MessageBox.Show("Some of the files that were in the file path you chose were not imported because the file extension was incorrect. This app is meant for .nif files specifically.", "Information", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("All the in the folder you selected has been imported!", "Notification", MessageBoxButtons.OK);
                    }
                }
            }

        }
        private void Start_BTN_Click(object sender, EventArgs e)
        {
            if (BackgroundWorker1.IsBusy != true)
            {

                BackgroundWorker1.RunWorkerAsync();
            }
        }
        private async void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //setting some variables we'll need for these group of variables here.
            int waitB4 = 0;
            int waitAfter = 0;
            int bytesPerPixel = 0;
            List<string> fileNames = new List<string>();
            List<string> filePaths = new List<string>();
            int totalScreenshots = 0;
            int completedScreenshots = 0;
            TaskCompletionSource<bool> eventHandled = new TaskCompletionSource<bool>();
            pauseCompletionSource.SetResult(false);
            System.Drawing.Size theSize = new System.Drawing.Size();
            bool sufficientSpace = false;

            if (MakeFileLists(ref totalScreenshots, ref fileNames, ref filePaths) != false)
            {
                if (ValidateNumbers(ref waitAfter, ref waitB4, ref bytesPerPixel) == true && ValidateSavePath(ref savePath) != null)
                {
                    DialogResult permission = MessageBox.Show("Keep in mind that if you have nifskope open already, someone will need to close it prior to when the progam does what it needs to do. Is it ok if the program does that for you?", "Question", MessageBoxButtons.YesNo);
                    if (permission == DialogResult.Yes)
                    {
                        Task nifskopeCheck = Task.Run(() =>
                        {
                            List<string> processList = new List<string>();
                            Process[] processes = Process.GetProcesses();
                            foreach (Process process in processes)
                            {
                                if (process.ProcessName == "NifSkope")
                                {
                                    process.Kill();
                                    break;
                                }
                            }
                            return Task.CompletedTask;
                        });
                        await nifskopeCheck;
                    }
                    else
                    {
                        MessageBox.Show("Please close out of nifskope prior to clicking the start button again.");
                        return;
                    }


                    string fileInfo = new DirectoryInfo(filePath).GetFiles().OrderByDescending(file => file.Length).FirstOrDefault().FullName;
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Maximized,
                    };
                    using (Process nifskopeProcess = new Process { StartInfo = startInfo })
                    {
                        System.Drawing.Point viewPortLocation = new System.Drawing.Point();
                        nifskopeProcess.StartInfo.FileName = fileInfo;
                        nifskopeProcess.Start();
                        while (nifskopeProcess.MainWindowHandle == IntPtr.Zero)
                        {
                            nifskopeProcess.WaitForInputIdle();
                        }
                        Task StallUntilNifskopeIsReadyAndGetViewPortPoint = Task.Run(() =>
                        {
                            AutomationElement nifskopeWindow = AutomationElement.FromHandle(nifskopeProcess.MainWindowHandle);
                            PropertyCondition className = new PropertyCondition(AutomationElement.ClassNameProperty, "Qt5QWindowOwnDCIcon");
                            AutomationElement viewPort = nifskopeWindow.FindFirst(TreeScope.Subtree, className);
                            if (viewPort == null)
                            {
                                do
                                {
                                    viewPort = nifskopeWindow.FindFirst(TreeScope.Subtree, className);
                                }
                                while (viewPort == null);
                            }
                            string name = viewPort.Current.Name;
                            string controlType = viewPort.Current.LocalizedControlType;
                            string classNameIs = viewPort.Current.ClassName;
                            System.Windows.Rect theRect = viewPort.Current.BoundingRectangle;
                            viewPortLocation = new System.Drawing.Point((int)theRect.Left, (int)theRect.Top);
                            theSize = new System.Drawing.Size((int)theRect.Width, (int)theRect.Height);
                            return Task.CompletedTask;
                        });
                        await StallUntilNifskopeIsReadyAndGetViewPortPoint;
                        Task<bool> DetermineSpace = Task<bool>.Run<bool>(() =>
                        {
                            int bytesPerRow = 0;
                            int paddingPerRow = 0;
                            int totalRowBytes = 0;
                            int headerSize = 54;
                            long totalFileSize = 0;
                            string driveRoot;
                            long spaceLeft;
                            long totalSpaceRequired;
                            DriveInfo driveInfo;
                            if (this.InvokeRequired)
                            {
                                int localBytesPerPixel = bytesPerPixel;
                                this.Invoke((MethodInvoker)(() =>
                                {
                                    bytesPerRow = theSize.Width * localBytesPerPixel;
                                    paddingPerRow = (4 - (bytesPerRow % 4)) % 4;
                                    totalRowBytes = (bytesPerRow + paddingPerRow) * theSize.Height;
                                    totalFileSize = headerSize + totalRowBytes;
                                    driveRoot = Path.GetPathRoot(savePath);
                                    driveInfo = new DriveInfo(driveRoot);
                                    spaceLeft = driveInfo.AvailableFreeSpace;
                                    totalSpaceRequired = totalFileSize * totalScreenshots;
                                    if (spaceLeft >= totalSpaceRequired)
                                    {
                                        sufficientSpace = true;
                                    }
                                    else
                                    {
                                        MessageBox.Show($"It appears that you do not have enough space on your computer for the screenshots. You have {spaceLeft} bytes but you need {totalSpaceRequired} bytes.");
                                        sufficientSpace = false;
                                    }
                                }));
                            }
                            else
                            {
                                bytesPerRow = theSize.Width * bytesPerPixel;
                                paddingPerRow = (4 - (bytesPerRow % 4)) % 4;
                                totalRowBytes = (bytesPerRow + paddingPerRow) * theSize.Height;
                                totalFileSize = headerSize + totalRowBytes;
                                driveRoot = Path.GetPathRoot(savePath);
                                driveInfo = new DriveInfo(driveRoot);
                                spaceLeft = driveInfo.AvailableFreeSpace;
                                totalSpaceRequired = totalFileSize * totalScreenshots;
                                if (spaceLeft >= totalSpaceRequired)
                                {
                                    sufficientSpace = true;
                                }
                                else
                                {
                                    MessageBox.Show($"It appears that you do not have enough space on your computer for the screenshots. You have {spaceLeft} bytes but you need {totalSpaceRequired} bytes.");
                                    sufficientSpace = false;
                                }
                            }
                            return Task.FromResult(sufficientSpace);
                        });
                        await DetermineSpace;
                        Task ResizeUI = Task.Run(() =>
                        {
                            Task ResizeUi = Task.Run(() =>
                            {
                                if (InvokeRequired)
                                {
                                    this.Invoke((MethodInvoker)(() =>
                                    {
                                        System.Drawing.Size panel1_OS = new System.Drawing.Size(panel1.Width, panel1.Height);
                                        System.Drawing.Size topPanel_OS = new System.Drawing.Size(topPanel.Width, topPanel.Height);
                                        System.Drawing.Size theSizeToMatch = new System.Drawing.Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
                                        float xMultiply = viewPortLocation.X / panel1_OS.Width;
                                        float yMultiply = viewPortLocation.Y / topPanel_OS.Height;
                                        if (viewPortLocation.Y == topPanel_OS.Height)
                                        {
                                            if (viewPortLocation.X == panel1_OS.Width)
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
                                                    con.Size = new System.Drawing.Size(newWidth, newHeight);
                                                }
                                            }
                                            int height = theSizeToMatch.Height - viewPortLocation.Y;
                                            panel1.Size = new System.Drawing.Size(viewPortLocation.X, height);
                                            panel1.Location = new System.Drawing.Point(0, viewPortLocation.Y);
                                        }
                                        else
                                        {
                                            //height needs to change. 
                                            if (viewPortLocation.X == panel1_OS.Width)
                                            {
                                                //only height needs to change. 
                                                foreach (Control con in panel1.Controls)
                                                {
                                                    con.Anchor = AnchorStyles.Top;
                                                    int newHeight = (int)(con.Height * yMultiply);
                                                    con.Size = new System.Drawing.Size(con.Width, newHeight);
                                                }
                                                topPanel.Height = viewPortLocation.Y;
                                                int height = (topPanel_OS.Height + panel1_OS.Height) - viewPortLocation.Y;
                                                panel1.Size = new System.Drawing.Size(viewPortLocation.X, height);
                                            }
                                            else
                                            {
                                                //Both height and width of panel1 needs to change.
                                                foreach (Control con in panel1.Controls)
                                                {
                                                    con.Anchor = AnchorStyles.Left & AnchorStyles.Top;
                                                    int newHeight = (int)(con.Height * yMultiply);
                                                    int newWidth = (int)(con.Width * xMultiply);
                                                    con.Size = new System.Drawing.Size(newWidth, newHeight);
                                                }
                                                topPanel.Height = viewPortLocation.Y;
                                                int height = this.Height - viewPortLocation.Y;
                                                panel1.Size = new System.Drawing.Size(viewPortLocation.X, height);
                                            }
                                        }
                                    }));
                                }
                                else
                                {
                                    System.Drawing.Size panel1_OS = new System.Drawing.Size(panel1.Width, panel1.Height);
                                    System.Drawing.Size topPanel_OS = new System.Drawing.Size(topPanel.Width, topPanel.Height);
                                    System.Drawing.Size theSizeToMatch = new System.Drawing.Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
                                    float xMultiply = viewPortLocation.X / panel1_OS.Width;
                                    float yMultiply = viewPortLocation.Y / topPanel_OS.Height;
                                    if (viewPortLocation.Y == topPanel_OS.Height)
                                    {
                                        if (viewPortLocation.X == panel1_OS.Width)
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
                                                con.Size = new  System.Drawing.Size(newWidth, newHeight);
                                            }
                                        }
                                        int height = theSizeToMatch.Height - viewPortLocation.Y;
                                        panel1.Size = new System.Drawing.Size(viewPortLocation.X, height);
                                        panel1.Location = new System.Drawing.Point(0, viewPortLocation.Y);
                                    }
                                    else
                                    {
                                        //height needs to change. 
                                        if (viewPortLocation.X == panel1_OS.Width)
                                        {
                                            //only height needs to change. 
                                            foreach (Control con in panel1.Controls)
                                            {
                                                con.Anchor = AnchorStyles.Top;
                                                int newHeight = (int)(con.Height * yMultiply);
                                                con.Size = new System.Drawing.Size(con.Width, newHeight);
                                            }
                                            topPanel.Height = viewPortLocation.Y;
                                            int height = (topPanel_OS.Height + panel1_OS.Height) - viewPortLocation.Y;
                                            panel1.Size = new System.Drawing.Size(viewPortLocation.X, height);
                                        }
                                        else
                                        {
                                            //Both height and width of panel1 needs to change.
                                            foreach (Control con in panel1.Controls)
                                            {
                                                con.Anchor = AnchorStyles.Left & AnchorStyles.Top;
                                                int newHeight = (int)(con.Height * yMultiply);
                                                int newWidth = (int)(con.Width * xMultiply);
                                                con.Size = new System.Drawing.Size(newWidth, newHeight);
                                            }
                                            topPanel.Height = viewPortLocation.Y;
                                            int height = this.Height - viewPortLocation.Y;
                                            panel1.Size = new System.Drawing.Size(viewPortLocation.X, height);
                                        }
                                    }
                                }
                            });
                        });
                        MessageBox.Show("Everything that needs to be calculated prior to starting the screenshot process has been calculated. However, if the backgorund color of the viewport of nifskope is a bright color at the moment, you might want to change that prior to exiting Nifskope. Because depending on how fast you told the program to take screenshots, the opening and closing of nifskope might simulate bright lights. Either way, close out of nifskope when you are ready for the process to begin.");
                        Task nifskopeProcess_Exited = Task.Run(() =>
                        {
                            nifskopeProcess.WaitForExit();
                            eventHandled.TrySetResult(true);
                            return Task.CompletedTask;
                        });
                        await Task.WhenAll(nifskopeProcess_Exited);
                    }//end of using statement.
                    DialogResult answer = MessageBox.Show("Does the program have your permission to automatically press the Load view button for each screenshot?", "Question", MessageBoxButtons.YesNoCancel);
                    if (answer == DialogResult.Cancel)
                    {
                        return;
                    }
                    else if (answer == DialogResult.No)
                    {

                        using (Process nifskopeProcess2 = new Process { StartInfo = startInfo })
                        {
                            while (completedScreenshots != totalScreenshots)
                            {
                                nifskopeProcess2.StartInfo.FileName = filePaths[completedScreenshots].ToString();
                                nifskopeProcess2.Start();
                                DateTime startTime = DateTime.Now;
                                Task WaitUntilNifskopeOpens = Task.Run(() =>
                                {
                                    while (nifskopeProcess2.MainWindowHandle == IntPtr.Zero || nifskopeProcess2.MainWindowHandle == null)
                                    {
                                        nifskopeProcess2.WaitForInputIdle();
                                    }
                                });

                                await WaitUntilNifskopeOpens;
                                handle = nifskopeProcess2.MainWindowHandle;
                                Task WaitBefore = Task.Delay(waitB4);
                                Task<int> ScreenshotMethod()
                                {
                                    try
                                    {
                                        System.Drawing.Point coords = new System.Drawing.Point(theSize.Width, theSize.Height);
                                        System.Drawing.Point dest = new System.Drawing.Point(0, 0);
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
                                };
                                Task<int> screenshot = ScreenshotMethod();
                                completedScreenshots = await screenshot;
                                Task WaitAfter = Task.Delay(waitAfter);
                                await WaitAfter;
                                DateTime screenshotDone = DateTime.Now;
                                TimeSpan screenshotTime = (screenshotDone - startTime);
                                Task<int> CalculateScreenshotsRemaining = Task.Run<int>(() =>
                                {
                                    int screenshotsRemainingInternal = totalScreenshots - completedScreenshots;
                                    return Task.FromResult(screenshotsRemainingInternal);
                                });
                                int screenshotsRemaining = await CalculateScreenshotsRemaining;
                                Task<int> RemainingTime = Task.Run<int>(() =>
                                {
                                    int timeRemainingInternal = (int)screenshotTime.TotalSeconds * screenshotsRemaining;
                                    return Task.FromResult(timeRemainingInternal);
                                });

                                int timeRemaining = await RemainingTime;
                                Task ui = UpdateUI(screenshotsRemaining, timeRemaining, completedScreenshots);
                                await Task.WhenAll(ui);
                                nifskopeProcess2.Kill();
                            }
                        }//End of using block
                    }
                    else if (answer == DialogResult.Yes)
                    {
                        while(completedScreenshots != totalScreenshots)
                        {
                            using(Process nifskopeProcess2 = new Process())
                            {
                                nifskopeProcess2.StartInfo.FileName = @"C:\Users\Johns\Downloads\NifSkope_2_0_2018-02-22-x64\NifSkope_2_0_2018-02-22-x64\NifSkope.exe";
                                nifskopeProcess2.StartInfo.Arguments = filePaths[completedScreenshots];
                                nifskopeProcess2.StartInfo.UseShellExecute = false;
                                nifskopeProcess2.StartInfo.RedirectStandardInput = true;
                                nifskopeProcess2.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                                nifskopeProcess2.Start();
                                Task<AutomationElement> WaitUntilNifskopeIsReadyAndGetLoadViewCheckbox = Task.Run<AutomationElement>(() =>
                                {
                                    while (nifskopeProcess2.MainWindowHandle == IntPtr.Zero)
                                    {
                                        nifskopeProcess2.WaitForInputIdle();
                                    }
                                    
                                    AutomationElement nifskopeWindowInternal = AutomationElement.FromHandle(nifskopeProcess2.MainWindowHandle);
                                    
                                    AutomationElement loadViewInternal = nifskopeWindowInternal.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, "Load View"));
                                    bool enabled = false;
                                    do
                                    {
                                        enabled = loadViewInternal.Current.IsEnabled;
                                    }
                                    while (enabled == false);
                                    return Task.FromResult(loadViewInternal);
                                });
                                AutomationElement loadView = await WaitUntilNifskopeIsReadyAndGetLoadViewCheckbox;
                                Task Subscribe = Task.Run(() =>
                                {
                                    SubscribeFocusChange(subElement);
                                    SubscribeToInvoke(subElement);
                                    SubscribePropertyChange(subElement);
                                    SubscribeToStructureChange(subElement);
                                    return Task.CompletedTask;
                                });
                                await Subscribe;
                                Task LoadViewInvoke = Task.Run(() =>
                                {
                                    Automation.AddAutomationPropertyChangedEventHandler(loadView, TreeScope.Element, LoadViewInvokeMethod, TogglePattern.ToggleStateProperty);
                                    TogglePattern pat = loadView.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;
                                    pat.Toggle();
                                });
                                await LoadViewInvoke;

                                completedScreenshots++;
                                nifskopeProcess2.Kill();
                            }
                            
                        }
                    }
                }
            }
        }//End of background worker method.
        public void LoadViewInvokeMethod(object sender, AutomationPropertyChangedEventArgs t)
        {
            object newV = t.NewValue;
        }
        private void SubscribeToStructureChange(AutomationElement element)
        {
            Automation.AddStructureChangedEventHandler(element, TreeScope.Children, new StructureChangedEventHandler(OnStructureChange));
        }
        private void OnStructureChange(object sender, StructureChangedEventArgs e)
        {
            //Get the process.

            object value = e.StructureChangeType;
            int[] runtimeIDs = e.GetRuntimeId();
            AutomationElement nifskope = AutomationElement.FromHandle(handle);
            List<AutomationElement> things = new List<AutomationElement>();
            foreach(int id in runtimeIDs)
            {
                AutomationElement theThing = nifskope.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.RuntimeIdProperty, id));
                things.Add(theThing);
            }
            

        }
        public void SubscribeToInvoke(AutomationElement element)
        {
            Automation.AddAutomationEventHandler(InvokePattern.InvokedEvent, element, TreeScope.Element, handlerEvent = new AutomationEventHandler(OnUIAutomationEvent));
            subElement = element;

        }
        private void OnUIAutomationEvent(object src, AutomationEventArgs f)
        {
            AutomationElement sourceE; 
            try
            {
                sourceE = src as AutomationElement;
            }
            catch
            {
                MessageBox.Show("Error");
            }
            if(f.EventId == InvokePattern.InvokedEvent)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    processStatus_LBL.Text = "Invoked!";
                }));
            }
        }
        private void SubscribePropertyChange(AutomationElement element)
        {
            Automation.AddAutomationPropertyChangedEventHandler(element, TreeScope.Element, propertyChanged = new AutomationPropertyChangedEventHandler(OnPropertyChange), AutomationElement.OrientationProperty, AutomationElement.BoundingRectangleProperty, AutomationElement.IsEnabledProperty);
        }
        private void SubscribeFocusChange(AutomationElement element)
        {
            focusChanged = new AutomationFocusChangedEventHandler(OnFocusChange);
            Automation.AddAutomationFocusChangedEventHandler(focusChanged);
        }
        private void OnFocusChange(object src, AutomationFocusChangedEventArgs s)
        {
            int id = s.ChildId;
            this.Invoke((MethodInvoker)(() =>
            {
                timeLeft_LBL.Text = $"FocusChanged to {id}";
            }));
        }
        private void OnPropertyChange(object src, AutomationPropertyChangedEventArgs g)
        {
            AutomationElement source = src as AutomationElement;
            object property = g.Property;
        }
        private Task UpdateUI(int screenshotsRemaining, int timeRemaining, int completedScreenshots)
        {

            if (InvokeRequired)
            {
                BeginInvoke((Action)(() =>
                {
                    screenshotNumber_LBL.Text = $"{screenshotsRemaining}";
                    timeLeft_LBL.Text = $"{timeRemaining:N} seconds";
                    completedScreenshots_LBL.Text = $"{completedScreenshots}";

                }));
            }
            else
            {
                screenshotNumber_LBL.Text = $"{screenshotsRemaining}";
                timeLeft_LBL.Text = $"{timeRemaining:N} seconds";
                completedScreenshots_LBL.Text = $"+{completedScreenshots}";

            }
            return Task.CompletedTask;
        }
        private void PauseAndPlay_BTN_Click(object sender, EventArgs e)
        {

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
                while (screenshotFiles_LB.SelectedIndex != -1)
                {
                    if (InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            Image img = imageList1.Images[screenshotFiles_LB.SelectedIndex];
                        }));
                    };

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
            int completedScreenshots = 0;
            int totalScreenshots = 0;
            if (InvokeRequired)
            {
                int localTotalScreenshots = 0;
                int localCompletedScreenshots = 0;
                this.Invoke((MethodInvoker)(() =>
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(savePath);
                    localTotalScreenshots = screenshotFiles_LB.Items.Count;
                    localCompletedScreenshots = dirInfo.GetFiles().Count();
                }));
                completedScreenshots = localCompletedScreenshots;
                totalScreenshots = localTotalScreenshots;
                if (totalScreenshots != completedScreenshots)
                {
                    completedScreenshots = totalScreenshots;
                    Application.Exit();
                }
                else
                {
                    Application.Exit();
                }
            }
            else
            {
                DirectoryInfo dirInfo = new DirectoryInfo(savePath);
                completedScreenshots = dirInfo.GetFiles().Count();
                totalScreenshots = screenshotFiles_LB.Items.Count;
                if (totalScreenshots != completedScreenshots)
                {
                    totalScreenshots = completedScreenshots;
                    Application.Exit();
                }
                else
                {
                    Application.Exit();
                }
            }

        }

        private void panel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }
    }
}