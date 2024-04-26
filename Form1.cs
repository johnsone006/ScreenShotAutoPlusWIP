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
using System.Diagnostics.Eventing.Reader;
using System.Dynamic;

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
        private void Form1_Load(object sender, EventArgs e)
        {
            //I have officially gotten tired of typing in my values every time I hit debug. So the next statement will only remain here until I get this program working correctly.

            //the rest of the code in this block is what fixed the fact that the ui tended to sometimes either overlap or be overlaped by the taskbar.
            Size formSize = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = formSize;
            this.Location = new Point(0, 0);
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
            Size theSize = new Size();
            bool sufficientSpace = false;

            if (MakeFileLists(ref totalScreenshots, ref fileNames, ref filePaths) != false)
            {
                if (ValidateNumbers(ref waitAfter, ref waitB4, ref bytesPerPixel) == true && ValidateSavePath(ref savePath) != null)
                {
                    string fileInfo = new DirectoryInfo(filePath).GetFiles().OrderByDescending(file => file.Length).FirstOrDefault().FullName;
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Maximized,

                    };
                    using (Process nifskopeProcess = new Process { StartInfo = startInfo })
                    {
                        Point viewPortLocation = new Point();
                        nifskopeProcess.StartInfo.FileName = fileInfo;

                        nifskopeProcess.Start();
                        while (nifskopeProcess.MainWindowHandle == IntPtr.Zero)
                        {
                            nifskopeProcess.WaitForInputIdle();
                        }

                        Point viewPortPoint = new Point();
                        Task<Point> GetViewPortPoint()
                        {
                            //Get the desktop. 
                            IUIAutomation auto = new CUIAutomation8();
                            IUIAutomationElement desktop = auto.GetRootElement();

                            IUIAutomationCondition classCondition = auto.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, "Qt5QWindowOwnDCIcon");
                            IUIAutomationElement viewPort = desktop.FindFirst(TreeScope.TreeScope_Descendants, classCondition);
                            double[] boundingRectArray = viewPort.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
                            int width = (int)boundingRectArray[2] - (int)boundingRectArray[0];

                            int height = (int)boundingRectArray[3] - (int)boundingRectArray[1];
                            theSize = new Size(width, height);
                            viewPortPoint = new Point((int)boundingRectArray[0], (int)boundingRectArray[1]);

                            return Task.FromResult(viewPortPoint);
                        }
                        Task<Point> viewPortPointTask = GetViewPortPoint();
                        viewPortLocation = await viewPortPointTask;
                        Task<bool> DetermineSpace()
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
                            return Task.FromResult(sufficientSpace);
                        }
                        sufficientSpace = await DetermineSpace();

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
                            else
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
                            }
                        });
                        MessageBox.Show("Everything that needs to be calculated has been calculated. If you changed the background color of nifskope's viewport for the sake of this program, it is ok to change it back now. Otherwise, depending on how quickly you told the program to take screenshots and the color you chose, the background color might end up resembling a bright flashing light. Either way, please close out of nifskope when you are ready for the screenshot process to start.", "Notification", MessageBoxButtons.OK);
                        Task myProcess_Exited = Task.Run(() =>
                        {
                            nifskopeProcess.WaitForExit();
                            eventHandled.TrySetResult(true);
                            return Task.CompletedTask;
                        });
                        await Task.WhenAll(myProcess_Exited);
                    }//End of first using statement.
                    using (Process nifskopeProcess2 = new Process { StartInfo = startInfo })
                    {
                        DialogResult answer = MessageBox.Show("Do you plan to let the program press the load view button prior to each screenshot automatically?", "Question", MessageBoxButtons.YesNoCancel);
                        if (answer == DialogResult.Yes)
                        {
                            if (sufficientSpace == true)
                            {
                                while (completedScreenshots != totalScreenshots)
                                {
                                    nifskopeProcess2.StartInfo.FileName = filePaths[completedScreenshots];
                                    nifskopeProcess2.Start();
                                    DateTime startTime = DateTime.Now;
                                    Task<bool> StallUntilNifskopeIsReady = Task.Run<bool>(() =>
                                    {
                                        IUIAutomation automation = new CUIAutomation8();
                                        IUIAutomationElement desktop = automation.GetRootElement();
                                        while (nifskopeProcess2.MainWindowHandle == IntPtr.Zero)
                                        {
                                            nifskopeProcess2.WaitForInputIdle();
                                        }
                                        IUIAutomationCondition processIDCond = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ProcessIdPropertyId, nifskopeProcess2.Id);
                                        IUIAutomationElement nifskopeLocal = desktop.FindFirst(TreeScope.TreeScope_Descendants, processIDCond);

                                        object controlType = nifskopeLocal.GetCurrentPropertyValue(UIA_PropertyIds.UIA_ControlTypePropertyId);
                                        //Now lets get an array of unenabled buttons.
                                        IUIAutomationCondition buttonsCond = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, 50000);
                                        IUIAutomationCondition notEnabledCond = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_IsEnabledPropertyId, false);
                                        var andCondition = automation.CreateAndCondition(buttonsCond, notEnabledCond);
                                        IUIAutomationElementArray buttons = nifskopeLocal.FindAll(TreeScope.TreeScope_Element, andCondition);
                                        if (buttons.Length > 0)
                                        {
                                            IUIAutomationElement chosenButton = null;
                                            for (int i = 0; i < buttons.Length; i++)
                                            {
                                                IUIAutomationElement candidate = buttons.GetElement(i);
                                                bool isEnabled = candidate.GetCurrentPropertyValue(UIA_PropertyIds.UIA_IsEnabledPropertyId);
                                                if (isEnabled == false)
                                                {
                                                    string name = string.Empty;
                                                    object candidateName = candidate.GetCurrentPropertyValue(UIA_PropertyIds.UIA_NamePropertyId);
                                                    if (!candidateName.Equals(name))
                                                    {
                                                        if ((string)candidateName != "Close" && (string)candidateName != "Float" && (string)candidateName != "Undo" && (string)candidateName != "Redo")
                                                        {
                                                            chosenButton = candidate;
                                                            break;
                                                        }
                                                    }
                                                }

                                            }
                                            bool enableChecker = false;
                                            do
                                            {
                                                object enableValue = chosenButton.GetCurrentPropertyValue(UIA_PropertyIds.UIA_IsEnabledPropertyId);
                                                enableChecker = (bool)enableValue;
                                            }
                                            while (enableChecker == false);
                                        }
                                        return Task.FromResult(true);
                                    });
                                    await StallUntilNifskopeIsReady;
                                    Task LoadView = Task.Run(() =>
                                    {
                                        IUIAutomation auto = new CUIAutomation8();

                                        IUIAutomationElement desktop = auto.GetRootElement();
                                        IUIAutomationCondition processIDCond = auto.CreatePropertyCondition(UIA_PropertyIds.UIA_ProcessIdPropertyId, nifskopeProcess2.Id);
                                        IUIAutomationElement nifskope = desktop.FindFirst(TreeScope.TreeScope_Subtree, processIDCond);
                                        IUIAutomationCondition controlType = auto.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, 50002);
                                        IUIAutomationCondition name = auto.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "Load View");
                                        var andCondition = auto.CreateAndCondition(controlType, name);
                                        IUIAutomationElement loadView = nifskope.FindFirst(TreeScope.TreeScope_Subtree, andCondition);

                                        if (loadView.GetCurrentPattern(10000) != null)
                                        {
                                            IUIAutomationInvokePattern invoke = loadView.GetCurrentPattern(10000) as IUIAutomationInvokePattern;
                                            invoke.Invoke();
                                        }

                                        return Task.CompletedTask;
                                    });
                                    await LoadView;
                                    Task WaitBefore = Task.Run(() =>
                                    {
                                        Task.Delay(waitB4);
                                        return Task.CompletedTask;
                                    });
                                    await WaitBefore;
                                    Task<int> ScreenshotMethod = Task.Run<int>(() =>
                                    {
                                        try
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
                                        catch (Exception f)
                                        {
                                            MessageBox.Show("Button stack trace:" + f.StackTrace + "Message" + f.Message + "Inner exception" + f.InnerException);
                                        }

                                        return Task.FromResult(completedScreenshots);
                                    });
                                    await ScreenshotMethod;
                                    Task WaitAfter = Task.Run(() =>
                                    {
                                        Task.Delay(waitAfter);
                                        return Task.CompletedTask;
                                    });
                                    await WaitAfter;

                                    DateTime screenshotDone = DateTime.Now;
                                    TimeSpan screenshotTime = (screenshotDone - startTime);
                                    Task<int> CalculateRemainingScreenshots = Task.Run<int>(() =>
                                    {
                                        int screenshotsLeft = totalScreenshots - completedScreenshots;
                                        return Task.FromResult(screenshotsLeft);
                                    });
                                    int screenshotsRemaining = await CalculateRemainingScreenshots;

                                    Task<int> TimeLeft = Task.Run(() =>
                                    {
                                        int timeLeft = (int)screenshotTime.TotalSeconds * screenshotsRemaining;
                                        return Task.FromResult(timeLeft);
                                    });
                                    int timeRemaining = await TimeLeft;

                                    bool ui = UpdateUI(screenshotsRemaining, timeRemaining, completedScreenshots);
                                    Task t = Task.WhenAll(StallUntilNifskopeIsReady, LoadView, WaitBefore, ScreenshotMethod, WaitAfter, CalculateRemainingScreenshots, TimeLeft);

                                    nifskopeProcess2.Kill();

                                }
                            }
                        }
                        else if (answer == DialogResult.No)
                        {
                            while (completedScreenshots != totalScreenshots)
                            {
                                nifskopeProcess2.StartInfo.FileName = fileNames[completedScreenshots];
                                nifskopeProcess2.Start();
                                DateTime startTime = DateTime.Now;
                                Task WaitBefore = Task.Delay(waitB4);
                                await WaitBefore;
                                Task<int> ScreenshotMethod = Task.Run<int>(() =>
                                {
                                    try
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

                                    catch (Exception f)
                                    {
                                        MessageBox.Show("Button stack trace:" + f.StackTrace + "Message" + f.Message + "Inner exception" + f.InnerException);
                                    }

                                    return Task.FromResult(completedScreenshots);
                                });
                                await ScreenshotMethod;
                                Task WaitAfter = Task.Delay(waitAfter);
                                await WaitAfter;

                                DateTime screenshotDone = DateTime.Now;
                                TimeSpan screenshotTime = (screenshotDone - startTime);
                                Task<int> CalculateRemainingScreenshots = Task.Run<int>(() =>
                                {
                                    int screenshotsLeft = totalScreenshots - completedScreenshots;
                                    return Task.FromResult(screenshotsLeft);
                                });
                                int screenshotsRemaining = await CalculateRemainingScreenshots;

                                Task<int> TimeLeft = Task.Run(() =>
                                {
                                    int timeLeft = (int)screenshotTime.TotalSeconds * screenshotsRemaining;
                                    return Task.FromResult(timeLeft);
                                });
                                int timeRemaining = await TimeLeft;

                                bool ui = UpdateUI(screenshotsRemaining, timeRemaining, completedScreenshots);
                                nifskopeProcess2.Kill();



                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"A variety of errors could have happened here. I'll list them: 1. You did not select a folder for the files to be saved to. \n 2. You either did not enter the hex color code of the viewport of nifskope into the correct text box, or the code you did enter is not a valid hex code. \n 3. You told the program to wait less than 0 miliseconds before and/or after each screenshot (or you didn't enter in a value into those text boxes at all). \n In any case, this must be remedied prior to the program continuing.", "Error", MessageBoxButtons.OK);
                    return;
                }
            }
            else
            {
                MessageBox.Show("The program needs to know where the files you want to screenshot are at before it can continue.", "Error", MessageBoxButtons.OK);
                return;

            }
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
    }
}