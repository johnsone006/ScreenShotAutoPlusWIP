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
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

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
        bool buttonPress;
        private bool isPaused;
        string savePath;
        private void Form1_Load(object sender, EventArgs e)
        {
            //I have officially gotten tired of typing in my values every time I hit debug. So the next statement will only remain here until I get this program working correctly.
            colorCode_TB.Text = "#f72ee0";
            //the rest of the code in this block is what fixed the fact that the ui tended to sometimes either overlap or be overlaped by the taskbar.
            Size formSize = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = formSize;
            this.Location = new Point(0, 0);
        }
        private string OpenFilesValidated(ref int totalScreenshots, ref List<string> fileNames, ref List<string> filePaths, ref string filePath)
        {
            //We are going to have to call this method twice, since one of the methods that needs it is the import method and the other one is the background worker do work
            //and we shouldn't use anything from the UIAutomationClient namespace on the Ui thread.
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
                    if (localFilePath != string.Empty)
                    {
                        DirectoryInfo di = new DirectoryInfo(localFilePath);
                        foreach (var file in di.GetFiles())
                        {
                            localFileNames.Add(file.Name);
                            localFilePaths.Add(file.FullName);
                        }
                    }
                }));
                fileNames = localFileNames;
                filePaths = localFilePaths;
                filePath = localFilePath;
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
            totalScreenshots = filePaths.Count;
            return filePath;
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
        private bool ValidateNumbers(ref string hexString, ref int waitAfter, ref int waitB4, ref int bytesPerPixel)
        {
            bool localValid = true;
            if (InvokeRequired)
            {
                string localHexString = null;
                int localWaitAfter = 0;
                int localWaitB4 = 0;
                int localBytesPerPixel = 0;

                this.Invoke((MethodInvoker)(() =>
                {
                    if (string.IsNullOrEmpty(colorCode_TB.Text) || colorCode_TB.TextLength != 7)
                    {
                        MessageBox.Show("Please enter in a valid hex color code for the background of the viewport in NifSkope.");
                        localValid = false;
                    }
                    try
                    {
                        Color color = ColorTranslator.FromHtml(colorCode_TB.Text);
                        localHexString = colorCode_TB.Text;
                    }
                    catch
                    {
                        localValid = false;
                    }
                    if (int.TryParse(waitAfter_TB.Text, out int waitA) == true && int.TryParse(waitB4_TB.Text, out int waitB) == true)
                    {
                        if (waitA >= 0 && waitB >= 0)
                        {
                            waitA = localWaitAfter;
                            waitB = localWaitB4;
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
                hexString = localHexString;
                localWaitAfter = waitAfter;
                localWaitB4 = waitB4;
            }
            return true;
        }
        private void Import_BTN_Click(object sender, EventArgs e)
        {
            string filePath = string.Empty;
            List<string> fileNames = null;
            List<string> filePaths = null;
            int totalScreenshots = 0;
            if (OpenFilesValidated(ref totalScreenshots, ref fileNames, ref filePaths, ref filePath) != null)
            {
                DialogResult answer = MessageBox.Show("Do you plan to let the program press the load view button prior to each screenshot automatically?", "Question", MessageBoxButtons.YesNo);
                if (answer == DialogResult.Yes)
                {
                    buttonPress = true;
                }
                else if (answer == DialogResult.No)
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
        Task<bool> IsNifskopeReady(Process nifskopeProcess)
        {
            //Get an interface for nifskope.
            bool isReady = false;
            IUIAutomation processInterface = new CUIAutomation8();
            while(nifskopeProcess.MainWindowHandle == IntPtr.Zero)
            {
                nifskopeProcess.WaitForInputIdle();
            }
            //Now make the window of nifskope an element.
            IUIAutomationElement window = processInterface.ElementFromHandle(nifskopeProcess.MainWindowHandle);
            //Get a window pattern to use to determine the window interaction state of nifskope.
            IUIAutomationWindowPattern state = null;
            try
            {
                state = window.GetCurrentPattern(10009) as IUIAutomationWindowPattern; //WindowPattern id

                do
                {
                    Task.Delay(1000);
                }
                while (state.CurrentWindowInteractionState != WindowInteractionState.WindowInteractionState_ReadyForUserInteraction);
            }
            catch(Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + " +
                                        $"InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + " +
                                        $"StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
            isReady = true;
            return Task.FromResult(isReady);
        }
        private async void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string filePath = null;
            string hexString = null;
            int waitB4 = 0;
            int waitAfter = 0;
            int bytesPerPixel = 0;
            List<string> fileNames = new List<string>();
            List<string> filePaths = new List<String>();
            int totalScreenshots = 0;
            if (OpenFilesValidated(ref totalScreenshots, ref fileNames, ref filePaths, ref filePath) != null)
            {
                if (ValidateNumbers(ref hexString, ref waitAfter, ref waitB4, ref bytesPerPixel) == true && ValidateSavePath(ref savePath) != null)
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
                        Task<bool> isReadyNow = IsNifskopeReady(nifskopeProcess);
                        bool readyToContinue = await isReadyNow; //which part of AWAIT do you not understand background worker
                        Task<Color> GetBingoColor()
                        {
                            //Convert 
                            Color bingoColorTemp = ColorTranslator.FromHtml(hexString);
                            int red = bingoColorTemp.R;
                            int green = bingoColorTemp.G;
                            int blue = bingoColorTemp.B;
                            Color bingoColorTemp2 = Color.FromArgb(255, red, green, blue);
                            return Task.FromResult<Color>(bingoColorTemp2);
                        }
                        Task<Color> GetBingoColorTask = GetBingoColor();
                        Color bingoColor = await GetBingoColor();
                        Bitmap initialSS = new Bitmap(500, 500);
                        Task<Bitmap> GetInitialSS()
                        {
                            Size nonLocalSize = new Size();

                            if (InvokeRequired)
                            {

                                Size localSize = new Size(500, 500);
                                Bitmap initialSSLocal = new Bitmap(1920, 1040);
                                this.Invoke((MethodInvoker)(() =>
                                {
                                    localSize = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
                                    initialSSLocal = new Bitmap(localSize.Width, localSize.Height);

                                    Graphics initiaLGraph = Graphics.FromImage(initialSSLocal);
                                    initiaLGraph.CopyFromScreen(0, 0, 0, 0, localSize);

                                    screenshot_PB.Image = initialSSLocal;

                                }));
                                nonLocalSize = localSize;
                                initialSS = initialSSLocal;
                            }
                            else
                            {
                                nonLocalSize = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
                            }


                            return Task.FromResult(initialSS);
                        }

                        Bitmap initialSS_Final = await GetInitialSS();

                        Point viewPortPoint = new Point();
                        Point GetViewPort(Point viewPortPoint2)
                        {

                            Size windowSize2 = new Size();
                            if (InvokeRequired)
                            {
                                Size localSize = new Size();
                                this.Invoke((MethodInvoker)(() =>
                                {
                                    localSize = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
                                }));
                                windowSize2 = localSize;
                            }
                            else
                            {
                                windowSize2 = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
                            }
                            int starterX = windowSize2.Width - 5;
                            int starterY = (windowSize2.Height / 2);
                            Color menuColor;
                            //Start with finding the edge of side menu.
                            for (int x = 2; x < windowSize2.Width;)
                            {
                                x++;
                                menuColor = initialSS_Final.GetPixel(x, starterY);
                                if (bingoColor.Equals(menuColor))
                                {
                                    viewPortPoint.X = x;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            //now find the height of the button area.
                            for (int y = 2; y < windowSize2.Height;)
                            {
                                y++;
                                menuColor = initialSS_Final.GetPixel(starterX, y);
                                if (bingoColor.Equals(menuColor))
                                {
                                    viewPortPoint2.Y = y;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            return viewPortPoint2;
                        }
                        viewPortLocation = GetViewPort(viewPortPoint);

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
                        Task ResizeUi = Task.Run(() =>
                        {
                            if (InvokeRequired)
                            {
                                this.Invoke((MethodInvoker)(() =>
                                {
                                    Size panel1_OS = new Size(panel1.Width, panel1.Height);
                                    Size topPanel_OS = new Size(topPanel.Width, topPanel.Height);
                                    Size theSizeToMatch = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
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
                                                con.Size = new Size(newWidth, newHeight);
                                            }
                                        }
                                        int height = theSizeToMatch.Height - viewPortLocation.Y;
                                        panel1.Size = new Size(viewPortLocation.X, height);
                                        panel1.Location = new Point(0, viewPortLocation.Y);
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
                                                con.Size = new Size(con.Width, newHeight);
                                            }
                                            topPanel.Height = viewPortLocation.Y;
                                            int height = (topPanel_OS.Height + panel1_OS.Height) - viewPortLocation.Y;
                                            panel1.Size = new Size(viewPortLocation.X, height);
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
                                            topPanel.Height = viewPortLocation.Y;
                                            int height = this.Height - viewPortLocation.Y;
                                            panel1.Size = new Size(viewPortLocation.X, height);
                                        }
                                    }
                                }));
                            }
                        });
                    }
                    MessageBox.Show("Everything has been calculated.", "Message", MessageBoxButtons.OK);
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
                                int[] progressInformation = CalculateRemainingScreenshotsAndTime(screenshotTime, completedScreenshots, totalScreenshots);
                                int screenshotsRemaining = progressInformation[0];
                                int timeRemaining = progressInformation[1];
                                bool ui = UpdateUI(screenshotsRemaining, timeRemaining, completedScreenshots);
                                screenshotProcess.Kill();
                            }
                        }
                        else if (buttonPress == false && sufficientSpace == true)
                        {
                            while (completedScreenshots != totalScreenshots)
                            {
                                screenshotProcess.StartInfo.FileName = fileNames[completedScreenshots];
                                List<string> buttonNames = new List<string>();
                                buttonNames = GetButtonsAndNames(screenshotProcess, buttonNames);
                                completedScreenshots = ScreenshotMethod(theSize, fileNames, ref completedScreenshots, savePath);
                                DateTime screenshotDone = DateTime.Now;
                                TimeSpan screenshotTime = (screenshotDone - startTime);
                                int[] progressInformation = CalculateRemainingScreenshotsAndTime(screenshotTime, completedScreenshots, totalScreenshots);
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
                    MessageBox.Show("There is something wrong with the text you inputted into the program. ");
                }
            }
        }
        //I'm adding a seprate method to check if the process has loaded completely to isolate the UIAutomation elements to inside a method.

        private List<string> GetButtonsAndNames(Process nifskopeProcess, List<string> buttonNames)
        {
            IUIAutomation processInterface = new CUIAutomation8();
            IntPtr nifskopeHandle = nifskopeProcess.MainWindowHandle;
            IUIAutomationElement9 nifskopeWindow = (IUIAutomationElement9)processInterface.ElementFromHandle(nifskopeHandle);
            var controlTypeCondition = processInterface.CreatePropertyCondition(30003, 50000);

            IUIAutomationElementArray buttons = nifskopeWindow.FindAll(TreeScope.TreeScope_Element, controlTypeCondition);
            for (int i = 0; i < buttons.Length; i++)
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
                if (InvokeRequired)
                {
                    DirectoryInfo dInfo = new DirectoryInfo(savePath);
                    int completedScreenshotsLocal = dInfo.GetFiles().Count();
                    this.Invoke((MethodInvoker)(() =>
                    {
                        Point coords = new Point(theSize.Width, theSize.Height);
                        Point dest = new Point(0, 0);
                        Bitmap nif = new Bitmap(theSize.Width, theSize.Height);
                        Graphics graphicsNif = Graphics.FromImage(nif);
                        graphicsNif.CopyFromScreen(coords, dest, theSize);
                        string TreeNodeNameRevised = fileNames[completedScreenshotsLocal].ToString().Replace(".nif", string.Empty);
                        nif.Save(savePath + "\\" + TreeNodeNameRevised + ".bmp", ImageFormat.Bmp);
                        imageList1.ImageSize = screenshot_PB.Size;
                        imageList1.Images.Add(nif);
                        completedScreenshotsLocal++;
                    }));
                    completedScreenshots = completedScreenshotsLocal;
                }
                else
                {
                    Point coords = new Point(theSize.Width, theSize.Height);
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
            }
            catch (Exception f)
            {
                MessageBox.Show("Button stack trace:" + f.StackTrace + "Message" + f.Message + "Inner exception" + f.InnerException);
            }

            return completedScreenshots;
        }
        private int[] CalculateRemainingScreenshotsAndTime(TimeSpan screenshotTime, int completedScreenshots, int totalScreenshots)
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
                    int index = screenshotFiles_LB.SelectedIndex;
                    if (imageList1.Images[index] != null)
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
}