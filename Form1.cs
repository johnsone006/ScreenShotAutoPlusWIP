using System;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Automation;



namespace ScreenShotAuto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.TopMost = true;
            BackgroundWorker1.WorkerReportsProgress = true;
            BackgroundWorker1.WorkerSupportsCancellation = true;
        }

        Size screenshotSize;
        int completedScreenshots;
        readonly List<string> fileNames = new List<string>();
        string filePath;
        private void Form1_Load(object sender, EventArgs e)
        {
            //the rest of the code in this block is what fixed the fact that the ui tended to sometimes either overlap or be overlaped by the taskbar.
            System.Drawing.Size formSize = new System.Drawing.Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = formSize;
            this.Location = new System.Drawing.Point(0, 0);

            Process[] candidates = Process.GetProcesses();
            foreach (Process process in candidates)
            {
                if (process.ProcessName == "ScreenshotAutoPlus.exe")
                {
                    Process screenshotAutoProcess = Process.GetProcessById(process.Id);
                    break;
                }
            }
        }

        private void Import_BTN_Click(object sender, EventArgs e)
        {
            processStatus_LBL.Text = "Waiting to process files...";
            try
            {
                using (FolderBrowserDialog dialogBD = new FolderBrowserDialog())
                {
                    dialogBD.Description = "Please select the folder that has the files you want to be screenshotted.";
                    dialogBD.ShowNewFolderButton = false;
                    dialogBD.ShowDialog();
                    filePath_TB.Text = dialogBD.SelectedPath;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: " +
                  $"{f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + " +
                  $"TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
            finally
            {
                DirectoryInfo fileInfo = new DirectoryInfo(filePath_TB.Text);
                foreach (var file in fileInfo.GetFiles())
                {
                    if (file.Extension == ".nif")
                    {
                        fileNames.Add(file.Name);
                    }
                }
            }
            processStatus_LBL.Text = "Files processed!";

        }
        private void Save_FolderBTN_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog fBD = new FolderBrowserDialog())
                {
                    fBD.Description = "Please select the folder that you want all the screenshots to be saved to.";
                    fBD.ShowNewFolderButton = false;
                    fBD.ShowDialog();
                    savePath_TB.Text = fBD.SelectedPath;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: " +
                   $"{f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + " +
                   $"TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
        }
        private void NifSkopeFP_BTN_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog nifskopeDialog = new FolderBrowserDialog())
            {
                nifskopeDialog.Description = "Please navigate to and select the folder that contains the executable file of the version of nifskope you want to use";
                nifskopeDialog.ShowDialog();
                nifskopeFilePath_TB.Text = nifskopeDialog.SelectedPath;
            }
        }
        private bool StoreUserInputInClassVariables(ref int waitB4, ref string savePath, ref string nifskopePath)
        {
  
            string localSavePath = null;
            string localNifSkopePath = null;
            int localWait = 0;
            bool waitValueIsAcceptableInteger = true;
            this.Invoke((MethodInvoker)(() =>
            {
               
                localSavePath = savePath_TB.Text;
                localNifSkopePath = string.Join("\\", nifskopeFilePath_TB.Text, "NifSkope.exe");
                if (int.TryParse(waitB4_TB.Text, out localWait) == false || localWait < 0 || filePath_TB.TextLength ==0 || savePath_TB.TextLength == 0 || nifskopeFilePath_TB.TextLength == 0)
                {
                    waitValueIsAcceptableInteger = false;
                    MessageBox.Show("Prior to starting the screenshot process, you must input a number that is equal to or greater than 0 in the first. Also, now is a good time to verify that you have already chosen two file paths. If you did, there will be text in the next two textboxes.");
                }
            }));
            if(waitValueIsAcceptableInteger == true)
            {
    
                filePath = filePath_TB.Text;
                savePath = localSavePath;
                nifskopePath = localNifSkopePath;
                return true;
            }
            
            return false;
            
        }
        private void PrepProcess_BTN_Click(object sender, EventArgs e)
        {
           
            if (filePath_TB.TextLength == 0 || savePath_TB.TextLength == 0)
            {
                MessageBox.Show("Before ScreenshotAuto starts to prepare for the screenshot process, you must choose two file  paths: one that has the files to be screeenshotted, and one to save the screenshots to.");
                return;
            }
            using (Process prepProcess = new Process())
            {
          
                int totalScreenshots = fileNames.Count;
                AutomationElement OpenNifskope()
                {

                    prepProcess.StartInfo.FileName = filePath_TB.Text + "\\" + fileNames[0];
                    prepProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    prepProcess.Start();
                

                    while (prepProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        prepProcess.WaitForInputIdle();
                    }
                    AutomationElement nifskopeWindow = AutomationElement.FromHandle(prepProcess.MainWindowHandle);
                    //also make sure that nifskope is actually completely loaded.
                    AutomationElement elementThatWillBeUnenabledUntilTheNifFileLoads = AutomationElement.FromHandle(prepProcess.MainWindowHandle).FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, "Save View"));
                    bool enabled = false;
                    do
                    {
                        enabled = elementThatWillBeUnenabledUntilTheNifFileLoads.Current.IsEnabled;
                    }
                    while (enabled == false);
                    return nifskopeWindow;
                }
                AutomationElement mainWindow = OpenNifskope();
                System.Windows.Rect GetViewPortInfo()
                {
                    AutomationElement viewport = mainWindow.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ClassNameProperty, "Qt5QWindowOwnDCIcon"));
                    System.Windows.Rect viewportRectangle = viewport.Current.BoundingRectangle;
                    screenshotSize = new Size((int)viewportRectangle.Width, (int)viewportRectangle.Height);
                    return viewportRectangle;
                }
                System.Windows.Rect viewportRect = GetViewPortInfo();
                bool EnsureSufficientMemory()
                {
                    int bitsPerPixel = Screen.FromControl(this).BitsPerPixel;
                    double totalBitsPerScreenshot = (screenshotSize.Width * bitsPerPixel) + (screenshotSize.Height * bitsPerPixel);
                    double totalMegabytesPerScreenshot = totalBitsPerScreenshot / 0.00000125;
                    DriveInfo computerMemory = new DriveInfo(Path.GetPathRoot(savePath_TB.Text));
                    double spaceAvailableInMegabytes = computerMemory.AvailableFreeSpace / 0.000001;
                    double totalSpaceRequired = totalScreenshots * totalMegabytesPerScreenshot;
                    if (spaceAvailableInMegabytes < totalSpaceRequired)
                    {
                        double additionalMemoryNeeded = totalSpaceRequired - spaceAvailableInMegabytes;
                        MessageBox.Show($"It looks like you do not have enough memory to save that many screenshots. You need {additionalMemoryNeeded} more megabytes worth of space. Please fix this prior to pressing the start button.");
                        return false;
                    }
                    return true;
                }
                if (EnsureSufficientMemory() == false)
                    return;
                void MakeSureUiOfScreenshotAutoDoesNotOverlapViewport()
                {
                    int sidePanelRightSideCoordinate = panel1.Bounds.Right;
                    int topPanelBottomCoordinate = topPanel.Bounds.Bottom;

                    if (sidePanelRightSideCoordinate > viewportRect.Left)
                    {
                        double numberToMultiplyByWidthOfControls = viewportRect.X / panel1.Width;
                        foreach (Control con in panel1.Controls)
                        {
                            con.Anchor = AnchorStyles.Left & AnchorStyles.Top;
                            int newWidth = (int)(con.Width * numberToMultiplyByWidthOfControls);
                            con.Size = new Size(newWidth, con.Height);
                        }
                        int newWidthOfPanel = (int)viewportRect.Left;
                        panel1.Size = new Size(newWidthOfPanel, panel1.Height);

                    }
                }
                MakeSureUiOfScreenshotAutoDoesNotOverlapViewport();
                prepProcess.Kill();
            }
        }
        private void StartBTN_Click(object sender, EventArgs e)
        {
            BackgroundWorker1.RunWorkerAsync();
        }

        private void CancelBackgroundOperation_BTN_Click(object sender, EventArgs e)
        {
            BackgroundWorker1.CancelAsync();
        }
        private async void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string savePath = null;
      
            string nifskopePath = null;
            int waitB4 = 0;
            if (StoreUserInputInClassVariables(ref waitB4, ref savePath, ref nifskopePath) == false)
                return;
      
            int totalScreenshots = fileNames.Count;
            

            Stopwatch screenshotStopWatch = new Stopwatch();
            Stopwatch uiStopWatch = new Stopwatch();
            Stopwatch calculateStopWatch = new Stopwatch();
            using (Process screenshotProcess = new Process())
            {

                screenshotProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                screenshotProcess.StartInfo.FileName = nifskopePath;

                screenshotProcess.Start();
                while (totalScreenshots != completedScreenshots)
                {
                    screenshotStopWatch.Start();
                    screenshotProcess.StartInfo.Arguments = filePath + "\\" + fileNames[completedScreenshots].ToString();


                    Task OpenNifskope = Task.Run(() =>
                    {
                        while(screenshotProcess.MainWindowHandle == IntPtr.Zero)
                        {
                            screenshotProcess.WaitForInputIdle();
                        }
                        AutomationElement elementThatWillNotBeEnabledUntilFileLoads = AutomationElement.FromHandle(screenshotProcess.MainWindowHandle).FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, "Save View"));
                        bool enabled = false;
                        do
                        {
                            enabled = elementThatWillNotBeEnabledUntilFileLoads.Current.IsEnabled;
                        }
                        while (enabled == false);
                        return Task.CompletedTask;
                    });
                    await OpenNifskope;
                    await Task.Run(() => screenshotProcess.WaitForInputIdle(waitB4));
                    Task<Bitmap> ScreenshotNifskope = Task.Run<Bitmap>(() =>
                    {
                        AutomationElement viewportElement = AutomationElement.FromHandle(screenshotProcess.MainWindowHandle).FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ClassNameProperty, "Qt5QWindowOwnDCIcon"));
                        System.Windows.Rect rectangleOfViewport = viewportElement.Current.BoundingRectangle;
                        System.Drawing.Point upperLeftOfScreenshotArea = new System.Drawing.Point((int)rectangleOfViewport.Left, (int)rectangleOfViewport.Top);
                        System.Drawing.Point destinationOfScreenshot = new System.Drawing.Point(0, 0);
                        Bitmap screenshotOfNif = new Bitmap(screenshotSize.Width, screenshotSize.Height);
                        Graphics theGraphicsOfTheScreenshot = Graphics.FromImage(screenshotOfNif);
                        theGraphicsOfTheScreenshot.CopyFromScreen(upperLeftOfScreenshotArea, destinationOfScreenshot, screenshotSize);
                        return Task.FromResult(screenshotOfNif);
                    });
                    Bitmap theScreenshot = await ScreenshotNifskope;
                    Task<int> SaveScreenshot = Task.Run<int>(() =>
                    {
                        int completedScreenshotsToReturn = completedScreenshots;
                        string screenshotName = fileNames[completedScreenshots].ToString().Replace(".nif", string.Empty);
                        theScreenshot.Save(filePath + "\\" + fileNames[completedScreenshots]);
                        completedScreenshotsToReturn++;
                        return completedScreenshotsToReturn;
                    });
                    completedScreenshots = await SaveScreenshot;
                    theScreenshot.Dispose();
                    Task<int> CalculateScreenshotsRemaining = Task.Run<int>(() =>
                    {
                        int screenshotsRemainingToReturn = totalScreenshots - completedScreenshots;
                        return screenshotsRemainingToReturn;
                    });
                    int remainingScreenshots = await CalculateScreenshotsRemaining;
                    screenshotStopWatch.Stop();
                    //I stopped the timer here because I'm going to start a new one to time the first method that updates the ui. Reason being is that I'm assuming the second method to update the ui will take approximately as long as the first, so by multiplying the number I'll get a time span that equals the execution time of both combined.
                    await Task.Run(() => uiStopWatch.Start());
                    Task UpdateUiWithScreenshotsRemaining = Task.Run(() =>
                    {

                        this.Invoke((MethodInvoker)(() =>
                        {
                            screenshotNumber_LBL.Text = "Screenshots remaining:" + remainingScreenshots;
                        }));
                        return Task.CompletedTask;
                    });
                    await Task.Run(() => uiStopWatch.Stop());
                    await Task.Run(() => calculateStopWatch.Start());
                    Task<double> CalculateTimePart1 = Task.Run<double>(() =>
                    {
                        double timeInMilliseconds = screenshotStopWatch.ElapsedMilliseconds + (uiStopWatch.ElapsedMilliseconds * 2);
                        TimeSpan timeSpanInMilliseconds = TimeSpan.FromMilliseconds(timeInMilliseconds);
                        double days = (int)timeSpanInMilliseconds.Days;
                        double hours = (int)timeSpanInMilliseconds.Hours;
                        double minutes = (int)timeSpanInMilliseconds.Minutes;
                        double seconds = (int)timeSpanInMilliseconds.Seconds;
                        double millisecondsTS = (int)timeSpanInMilliseconds.Milliseconds;
                        string incompleteTimeSpan = days.ToString() + ":" + hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString() + ":" + millisecondsTS.ToString();
                        return timeInMilliseconds;
                    });
                    double milliseconds = await CalculateTimePart1;
                    await Task.Run(() => calculateStopWatch.Stop());
                    Task UpdateUI = Task.Run(() =>
                    {
                        double totalTimeForOneScreenshot = (calculateStopWatch.ElapsedMilliseconds * 2) + milliseconds + 1000;
                        double totalTime = totalTimeForOneScreenshot * remainingScreenshots;
                        TimeSpan span = TimeSpan.FromMilliseconds(totalTime);
                        double days = (int)span.Days;
                        double hours = (int)span.Hours;
                        double minutes = (int)span.Minutes;
                        double seconds = (int)span.Seconds;
                        double mS = (int)span.Milliseconds;
                        string totalTimeString = string.Join(":", days, hours, minutes, seconds, mS);
                        this.Invoke((MethodInvoker)(() =>
                        {
                            timeLeft_LBL.Text = "Approximately" + totalTimeString;
                        }));
                        Task.Delay(1000);
                        return Task.CompletedTask;
                    });
                    await UpdateUI;
                    await Task.Run(() =>
                    {
                        screenshotStopWatch.Reset();
                        uiStopWatch.Reset();
                        calculateStopWatch.Reset();
                    });
                }//end of while loop for completed screenshots.
            }//end of using loop
        }//End of background worker method.

    
            
        private void Exit_BTN_Click(object sender, EventArgs e)
        {
            BackgroundWorker1.Dispose();
            Application.Exit();
        }


    }
}