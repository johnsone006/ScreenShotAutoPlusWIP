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
                        while(nifskopeProcess.MainWindowHandle == IntPtr.Zero)
                        {
                            nifskopeProcess.WaitForInputIdle();
                        }
                        Task IsNifskopeReady()
                        {
                            
                            //Get the desktop.
                            IUIAutomation auto= new CUIAutomation8();
                            IUIAutomationElement nifskope = auto.ElementFromHandle(nifskopeProcess.MainWindowHandle);
                            IUIAutomationCondition cond = auto.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, 50000);
                            IUIAutomationCondition cond2 = auto.CreatePropertyCondition(UIA_PropertyIds.UIA_IsEnabledPropertyId, false);
                            var andCondition = auto.CreateAndCondition(cond, cond2);

                            IUIAutomationElementArray array = nifskope.FindAll(TreeScope.TreeScope_Descendants, andCondition);
                            IUIAutomationElement button =  null;
                            for(int i = 0; i < array.Length; i++)
                            {
                                IUIAutomationElement candidate = array.GetElement(i);
                                bool isEnabled = candidate.GetCurrentPropertyValue(UIA_PropertyIds.UIA_IsEnabledPropertyId);
                                if(isEnabled == false)
                                {
                                    button = candidate;
                                    break;
                                }

                            }
                            bool isEnabled2 = false;
                            do
                            {
                                object propertyValue = button.GetCurrentPropertyValue(UIA_PropertyIds.UIA_IsEnabledPropertyId);
                                isEnabled2 = (bool)(propertyValue);

                            }
                            while (isEnabled2 == false);
                            return Task.CompletedTask;
                        };
                        Task nifskopeChecker = IsNifskopeReady();
                        await nifskopeChecker;
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
                                    viewPortPoint2.X = x;
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
                        MessageBox.Show("Everything was calculated. If you would like, you can change the background color of the viewport of nifskope back to a normal color. Otherwise, please close out of nifskope so the screenshot process can continue.", "Notification", MessageBoxButtons.OK);
                        nifskopeProcess.WaitForExit();
                    }
                    using (Process nifskopeProcess2 = new Process { StartInfo = startInfo })
                    {
                        int completedScreenshots = 0;
                        pauseCompletionSource.SetResult(false);
                        while(completedScreenshots != totalScreenshots)
                        {
                            nifskopeProcess2.StartInfo.FileName = fileNames[completedScreenshots];
                            
                        }
                        nifskopeProcess2.Start();
                        DateTime startTime = DateTime.Now;
                        Task button = AutomaticButtonPress(nifskopeProcess2);
                        await button;
                        completedScreenshots = await ScreenshotMethod(theSize, fileNames, ref completedScreenshots, savePath);
                        DateTime screenshotDone = DateTime.Now;
                        TimeSpan screenshotTime = (screenshotDone - startTime);
                        int[] progressInformation = CalculateRemainingScreenshotsAndTime(screenshotTime, completedScreenshots, totalScreenshots);
                        int screenshotsRemaining = progressInformation[0];
                        int timeRemaining = progressInformation[1];
                        bool ui = UpdateUI(screenshotsRemaining, timeRemaining, completedScreenshots);
                        nifskopeProcess2.Kill();
                    }
                }
                else
                {
                    MessageBox.Show("There is something wrong with the text you inputted into the program. ");
                }
            }
        }
        //I'm adding a seprate method to check if the process has loaded completely to isolate the UIAutomation elements to inside a method.

       
        private Task AutomaticButtonPress(Process nifskopeProcess2)
        {

            //Get the desktop.
            IUIAutomation automation = new CUIAutomation8();
            IUIAutomationElement desktop = automation.GetRootElement();
            IUIAutomationCondition processIDCond = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ProcessIdPropertyId, nifskopeProcess2.Id);
            IUIAutomationCondition controlType = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, 50032);
            var conditionAnd = automation.CreateAndCondition(processIDCond,controlType);
            IUIAutomationElement nifskope = desktop.FindFirst(TreeScope.TreeScope_Children, conditionAnd);

            //Now lets find the load view button.
            IUIAutomationCondition buttonCondtion = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, 50000); //button condition
            IUIAutomationCondition nameCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "Load View");
            var conditionAnd2 = automation.CreateAndCondition(buttonCondtion, nameCondition);
            IUIAutomationElement loadView = nifskope.FindFirst(TreeScope.TreeScope_Children, conditionAnd2);

            IUIAutomationInvokePattern invoke = loadView.GetCurrentPattern(10000);
            invoke.Invoke();
            return Task.CompletedTask;
        }
        private Task<int> ScreenshotMethod(Size theSize, List<string> fileNames, ref int completedScreenshots, string savePath)
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

            return Task.FromResult(completedScreenshots) ;
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