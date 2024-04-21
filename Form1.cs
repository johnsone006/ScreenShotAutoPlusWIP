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
using UIAutomationClient;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;

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
        private int completedScreenshots;
        bool buttonPress;
        private bool isPaused;
        int totalScreenshots;

        private void Form1_Load(object sender, EventArgs e)
        {
            //I have officially gotten tired of typing in my values every time I hit debug. So the next statement will only remain here until I get this program working correctly.
            colorCode_TB.Text = "#f72ee0";
            //the rest of the code in this block is what fixed the fact that the ui tended to sometimes either overlap or be overlaped by the taskbar.
            Size formSize = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = formSize;
            this.Location = new Point(0, 0);
        }

        private string OpenFilesValidated(ref string filePath,  List<string> fileNames, List<string> filePaths)
        {
            //We are going to have to call this method twice, since one of the methods that needs it is the import method and the other one is the background worker do work
            //and we shouldn't use anything from the UIAutomationClient namespace on a ui thread.
            fileNames = new List<string>();
            filePaths = new List<string>();

            if (this.InvokeRequired)
            {
                string localFilePath = string.Empty;
                List<string> localFileNames = new List<string>();
                List<string> localFilePaths = new List<string>();
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
                    if(localFilePath != string.Empty)
                    {
                        DirectoryInfo di = new DirectoryInfo(localFilePath);
                        foreach(var file in di.GetFiles())
                        {
                            localFileNames.Add(file.Name);
                            localFilePaths.Add(file.FullName);
                        }
                    }
                    

                }));
                localFileNames = fileNames;
                localFilePaths = filePaths;
                localFilePath = filePath;
            }
            else
            {
                try
                {
                    //Get file paths. 
                    using (FolderBrowserDialog fBD = new FolderBrowserDialog())
                    {
                        fBD.Description = "Please select the folder that contains the files to be screenshotted.";
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
                catch (Exception f)
                {
                    MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: " +
                   $"{f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + " +
                   $"TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                }
                DirectoryInfo dI = new DirectoryInfo(filePath);

                foreach (var file in dI.GetFiles())
                {
                    filePaths.Add(file.FullName);
                    fileNames.Add(file.Name);
                }
            
            }
            return filePath;
        }
        private string ValidateSavePath(string savePath)
        {
            if (this.InvokeRequired)
            {
                string localSavePath = string.Empty;
                this.Invoke((MethodInvoker)(() =>
                {
                    

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
        private bool ValidateNumbers(string hexString, int waitAfter, int waitB4, int bytesPerPixel)
        {
            if (this.InvokeRequired)
            {
                
               this.Invoke((MethodInvoker)(() => ValidateNumbers(hexString, waitAfter,waitB4, bytesPerPixel)));    
            }
            if (string.IsNullOrEmpty(colorCode_TB.Text) || colorCode_TB.TextLength != 7)
            {
                MessageBox.Show("Please enter in a valid hex color code for the background of the viewport in NifSkope.");
                return false;
            }
            hexString = colorCode_TB.Text;
            for (int i = 1; i < hexString.Length; i++)
            {
                char c = colorCode_TB.Text[i];
                bool isHexDigit = (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c <= 'a' && c <= 'f');
                if (isHexDigit == false)
                {
                    return false;
                }
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
            //Get bits per pixel.
            int bitsPerPixel = Screen.FromControl(this).BitsPerPixel;
            bytesPerPixel = bitsPerPixel / 8;

            return true;
        }
        private void Import_BTN_Click(object sender, EventArgs e)
        {
            string filePath = string.Empty;
            List<string> fileNames = null;
            List<string> filePaths = null;

            if (OpenFilesValidated(ref filePath, fileNames, filePaths) != null)
            {
                DialogResult answer = MessageBox.Show("Do you plan to let the program press the load view button prior to each screenshot automatically?", "Question", MessageBoxButtons.YesNo);
                if(answer == DialogResult.Yes)
                {
                    buttonPress = true;
                }
                else if(answer == DialogResult.No)
                {
                    buttonPress = false;
                }
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
            totalScreenshots = screenshotFiles_LB.Items.Count;
        }
      

        private void Start_BTN_Click(object sender, EventArgs e)
        {

            if (BackgroundWorker1.IsBusy != true)
            {

                BackgroundWorker1.RunWorkerAsync();
            }

        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
      
            string filePath = null;
            string savePath = null;
            string hexString = null;
            long[] info = new long[2];
            int waitB4 = int.Parse(waitB4_TB.Text);
            int waitAfter = int.Parse(waitAfter_TB.Text);
            int bytesPerPixel = 0;
            List<string> fileNames = new List<string>();
            List<string> filePaths = new List<String>();
            //So the garbage collector will collect fileNames after the if loop is done.

            if (OpenFilesValidated(ref filePath, fileNames, filePaths) != null)
            {
                if(ValidateNumbers(hexString,waitAfter, waitB4, bytesPerPixel) == true && ValidateSavePath(savePath) != null)
                {
                    Size theSize = new Size();
                    bool sufficientSpace = false;
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Maximized,
                    };
                    using (Process nifskopeProcess = new Process { StartInfo = startInfo })
                    {
                        Point viewPortLocation = new Point();

                        string fileInfo = new DirectoryInfo(filePath).GetFiles().OrderByDescending(file => file.Length).FirstOrDefault().FullName;
                        nifskopeProcess.StartInfo.FileName = fileInfo;
                        nifskopeProcess.Start();

                        Task IsOpen()
                        {
                            if (nifskopeProcess.MainWindowHandle == IntPtr.Zero)
                            {
                                nifskopeProcess.WaitForInputIdle();
                            }
                            return Task.CompletedTask;
                        }
                        IsOpen();
                        Task<Color> GetBingoColor()
                        {
                            //Convert 
                            Color bingoColorTemp = ColorTranslator.FromHtml(hexString);
                            int red = bingoColorTemp.R;
                            int green = bingoColorTemp.G;
                            int blue = bingoColorTemp.B;
                            Color bingoColorTemp2 = Color.FromArgb(255, red, green, blue);
                            return Task.FromResult(bingoColorTemp2);

                        }
                        Task<Color> bingoColor = GetBingoColor();
                        Task<Bitmap> GetInitialSS()
                        {
                            IUIAutomation pre_Auto2 = new CUIAutomation8();
                            pre_Auto2 = new CUIAutomation8();
                            IUIAutomationElement window = pre_Auto2.ElementFromHandle(nifskopeProcess.MainWindowHandle);
                            Size windowSize = window.GetCurrentPropertyValue(UIA_PropertyIds.UIA_SizePropertyId);

                            Bitmap initialSS = new Bitmap(windowSize.Width, windowSize.Height);
                            Graphics initialGraphics = Graphics.FromImage(initialSS);
                            initialGraphics.CopyFromScreen(0, 0, 0, 0, windowSize);

                            return Task.FromResult(initialSS);
                        }
                        Task<Bitmap> ssMethod = GetInitialSS();
                        Bitmap initialSS_Final = ssMethod.Result;

                        Point viewPortPoint2 = new Point();
                        Point GetViewPort(Point viewPortPoint)
                        {
                            IUIAutomation uIAutomation = new CUIAutomation8();
                            IUIAutomationElement window = uIAutomation.ElementFromHandle(nifskopeProcess.MainWindowHandle);
                            Size windowSize2 = window.GetCurrentPropertyValue(UIA_PropertyIds.UIA_SizePropertyId);

                            int starterX = windowSize2.Width - 5;
                            int tempY;
                            Color menuColor;
                            for (tempY = 2; tempY < windowSize2.Height;)
                            {
                                tempY++;
                                menuColor = initialSS_Final.GetPixel(starterX, tempY);
                                if (bingoColor.Equals(menuColor))
                                {
                                    viewPortPoint.X = tempY;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }

                            }
                            tempY = windowSize2.Height / 2;
                            for (starterX = 0; starterX < windowSize2.Width;)
                            {
                                starterX++;
                                menuColor = initialSS_Final.GetPixel(starterX, tempY);
                                if (bingoColor.Equals(menuColor))
                                {
                                    viewPortPoint.Y = starterX;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            viewPortLocation = new Point(viewPortPoint.X, viewPortPoint.Y);
                            return viewPortLocation;
                        }
                        viewPortLocation = GetViewPort(viewPortPoint2);

                        Size SizeOfScreenshots()
                        {
                            //Get size of Nifskope window.
                            int initialWidth = initialSS_Final.Width;
                            int initialHeight = initialSS_Final.Height;
                            //Subtract viewPortLocation x and y. 
                            int newWidth = initialWidth - viewPortLocation.X;
                            int newHeight = initialHeight - viewPortLocation.Y;
                            Size theSizeTemp = new Size(newWidth, newHeight);
                            return theSizeTemp;
                        }
                        theSize = SizeOfScreenshots();
                        bool DetermineSpace()
                        {
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
                            {
                                MessageBox.Show($"It appears that you do not have enough space on your computer for the screenshots. You have {spaceLeft} bytes but you need {totalSpaceRequired} bytes");
                                sufficientSpace = false;
                            }
                            return sufficientSpace;
                        }
                        sufficientSpace = DetermineSpace();
                        initialSS_Final.Dispose();
                    }
                    MessageBox.Show("Everything has been calculated. If you would like, you can go ahead", "Message", MessageBoxButtons.OK);
                    using (Process screenshotProcess = new Process { StartInfo = startInfo })
                    {
                        int completedScreenshots = 0;
                        pauseCompletionSource.SetResult(false);
                        DateTime startTime = DateTime.Now;
                        if (buttonPress == true && sufficientSpace == true)
                        {
                            while (completedScreenshots != totalScreenshots)
                            {
                                screenshotProcess.StartInfo.FileName = fileNames[completedScreenshots];
                                List<string> buttonNames = new List<string>();
                                buttonNames = GetButtonsAndNames(screenshotProcess, buttonNames);
                                Task button = AutomaticButtonPress(screenshotProcess, buttonNames);
                                button.Wait();
                                completedScreenshots = ScreenshotMethod(theSize, fileNames, ref completedScreenshots, savePath);
                                DateTime screenshotDone = DateTime.Now;
                                TimeSpan screenshotTime = (screenshotDone - startTime);
                                int[] progressInformation = CalculateRemainingScreenshotsAndTime(screenshotTime, completedScreenshots);
                                int screenshotsRemaining = progressInformation[0];
                                int timeRemaining = progressInformation[1];
                                bool ui = UpdateUI(screenshotsRemaining, timeRemaining, completedScreenshots);

                                screenshotProcess.Kill();
                            }
                        }
                    }

                }
                else
                {
                    //Temp 
                    MessageBox.Show("Error", "Message", MessageBoxButtons.OK);
                }
            }
                
        }
        private List<string> GetButtonsAndNames(Process nifskopeProcess, List<string>buttonNames)
        {
            IUIAutomation processInterface = new CUIAutomation8();
            
            IntPtr nifskopeHandle = nifskopeProcess.MainWindowHandle;
            IUIAutomationElement9 nifskopeWindow = (IUIAutomationElement9)processInterface.ElementFromHandle(nifskopeHandle);
            var controlTypeCondition = processInterface.CreatePropertyCondition(30003, 50000);

            IUIAutomationElementArray buttons = nifskopeWindow.FindAll(TreeScope.TreeScope_Element, controlTypeCondition);
            for(int i = 0; i < buttons.Length; i++)
            {
                IUIAutomationElement buttonElement = buttons.GetElement(i);
                string name = buttonElement.CurrentName;
                if (name == null || name == string.Empty)
                    name = "Unspecified";
            }
            return buttonNames;
        }
        private Task AutomaticButtonPress(Process nifskopeProcess, List<string> buttonNames)
        {
            //Get list of buttons again. We want to each object from the UIAutomationClient namespace **inside** methods as much as possible to ensure they'll be grabbed by the GC.
            IUIAutomation processInterFace = new CUIAutomation8();
            IUIAutomationElement window = processInterFace.ElementFromHandle(nifskopeProcess.MainWindowHandle);
            var controlTypeCondition = processInterFace.CreatePropertyCondition(30003, 50000);
            IUIAutomationElementArray buttonArray = window.FindAll(TreeScope.TreeScope_Descendants, controlTypeCondition);
            int index = buttonNames.IndexOf("Load View");

            IUIAutomationElement loadView = buttonArray.GetElement(index);
            object patternObj = null;
            //invoke pattern id.
            if (loadView.GetCurrentPattern(UIA_PropertyIds.UIA_IsInvokePatternAvailablePropertyId) != 0)
            {
                try
                {
                    IUIAutomationInvokePattern invokePattern = patternObj as IUIAutomationInvokePattern;
                    invokePattern.Invoke();
                    
                }
                catch (Exception f)
                {
                    MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + " +
                        $"InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + " +
                        $"StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                    
                }
                
            }
            return Task.CompletedTask;
        }
        private int ScreenshotMethod(Size theSize, List<string> fileNames, ref int completedScreenshots, string savePath)
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

            return completedScreenshots;
        }
        private int[] CalculateRemainingScreenshotsAndTime(TimeSpan screenshotTime, int completedScreenshots)
        {
            int screenshotsRemaining = totalScreenshots - completedScreenshots;
            int timeRemaining = (int)screenshotTime.TotalSeconds * screenshotsRemaining;
            int[] progressInformation = new int[2];
            progressInformation[0] = screenshotsRemaining;
            progressInformation[1] = timeRemaining;
            return progressInformation;
        }
        private bool UpdateUI(int screenshotsRemaining, int timeRemaining, int completedScreenshots)
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
            return invokeRequired;
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
                while (screenshotFiles_LB.SelectedIndex != -1)
                {
                    BeginInvoke((Action)(() =>
                {
                    if (screenshotFiles_LB.SelectedIndex <= completedScreenshots)
                    {
                        Image img = imageList1.Images[screenshotFiles_LB.SelectedIndex];
                    }
                }));
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

    }
}