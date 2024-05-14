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
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        Size screenshotSize;
        int completedScreenshots;
        int totalScreenshots;
        DateTime startTime;
        readonly List<string> fileNames = new List<string>();
        readonly List<string> filePaths = new List<string>();
        Process screenshotAuto;
        private void Form1_Load(object sender, EventArgs e)
        {
            //the rest of the code in this block is what fixed the fact that the ui tended to sometimes either overlap or be overlaped by the taskbar.
            System.Drawing.Size formSize = new System.Drawing.Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = formSize;
            this.Location = new System.Drawing.Point(0, 0);
            InitializeBackgroundWorker();
            //Get process for ScreenshotAutoPlus
            Process[] getProcess = Process.GetProcesses();
            foreach(Process candidate in getProcess)
            {
                if(candidate.ProcessName == "ScreenShotAutoPlus.exe")
                {
                    screenshotAuto = candidate;
                    break;
                }
            }
        }
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork +=
               new DoWorkEventHandler(BackgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            BackgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged +=
                new ProgressChangedEventHandler(BackgroundWorker1_ProgressChanged);
            
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
                        filePaths.Add(file.FullName);
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
      
        private bool StoreUserInputInClassVariables(ref int waitB4, ref string filePath, ref string savePath)
        {
  
            string localSavePath = null;
            
            int localWait = 0;
            bool waitValueIsAcceptableInteger = true;
            this.Invoke((MethodInvoker)(() =>
            {
                localSavePath = savePath_TB.Text;
                if (int.TryParse(waitB4_TB.Text, out localWait) == false || localWait < 0 || filePath_TB.TextLength ==0 || savePath_TB.TextLength == 0)
                {
                    waitValueIsAcceptableInteger = false;
                    MessageBox.Show("Prior to starting the screenshot process, you must input a number that is equal to or greater than 0 in the first. Also, now is a good time to verify that you have already chosen two file paths. If you did, there will be text in the next two textboxes.");
                }
            }));
            if(waitValueIsAcceptableInteger == true)
            {
                filePath = filePath_TB.Text;
                savePath = localSavePath;
                
                waitB4 = localWait;
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

                totalScreenshots = filePaths.Count;
                AutomationElement OpenNifskope()
                {

                    prepProcess.StartInfo.FileName = filePaths[completedScreenshots];
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
            backgroundWorker1.RunWorkerAsync();
        }

        private void CancelBackgroundOperation_BTN_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }
        private async void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            string savePath = null;
            string filePath = null;
            int waitB4 = 0;
            if (StoreUserInputInClassVariables(ref waitB4, ref filePath, ref savePath) == false)
                return;
       
            int totalScreenshots = fileNames.Count;
            
            using (Process screenshotProcess = new Process())
            {

                while (totalScreenshots != completedScreenshots)
                {
                    if(worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        startTime = DateTime.Now;
                        Task OpenNifskope = Task.Run(() =>
                        {
                            screenshotProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                            screenshotProcess.StartInfo.FileName = filePaths[completedScreenshots];
                            screenshotProcess.Start();
                            while (screenshotProcess.MainWindowHandle == IntPtr.Zero)
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
                        await Task.Run(() => screenshotProcess.Kill());
                    }
                    
                }//end of while loop for completed screenshots.
            }//end of using loop
        }//End of background worker method.

      
         

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Double TimeForOneScreenshotInMilliseconds()
            {
                double timeForOneScreenshotInMS = (DateTime.Now - startTime).TotalMilliseconds;
                return timeForOneScreenshotInMS;
            }
            Double millisecondsPerScreenshot = TimeForOneScreenshotInMilliseconds();
            Double TimeForTheRemainingScreenshotsInMilliseconds()
            {
                double timeForTheRestOfTheScreenshots = millisecondsPerScreenshot * (totalScreenshots - completedScreenshots);
                return timeForTheRestOfTheScreenshots;
            }
            double estimatedTimeForRemainingScreenshots = TimeForTheRemainingScreenshotsInMilliseconds();
            TimeSpan TimeSpanOfRemainingScreenshots()
            {
                TimeSpan remainingTime = TimeSpan.FromMilliseconds(estimatedTimeForRemainingScreenshots);
                return remainingTime;
            }
            TimeSpan timeLeft = TimeSpanOfRemainingScreenshots();
            string FormattedStringOfTimeSpan()
            {
                double days = timeLeft.Days;
                double hours = timeLeft.Hours;
                double minutes = timeLeft.Minutes;
                double seconds = timeLeft.Seconds;
                double milliseconds = timeLeft.Milliseconds;
                string formattedTimeLeftString = string.Join(":", days, hours, minutes, seconds, milliseconds);
                return formattedTimeLeftString;
            }
            string estimatedTimeRemaining = FormattedStringOfTimeSpan();
            //Also, 
            this.Invoke((MethodInvoker)(() =>
            {
                screenshotProcess_ProgressBar.Value = e.ProgressPercentage;
                screenshotNumber_LBL.Text = (totalScreenshots - completedScreenshots).ToString();
                timeLeft_LBL.Text = "Approximately" + estimatedTimeRemaining;
                if(screenshotAuto.Responding == false)
                {
                    processStatus_LBL.Text = "ScreenshotAutoPlus has stopped responding.";
                }
                
            }));
            
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled == true)
            {
                processStatus_LBL.Text = "Cancelled. To restart the process on the screenshot that would've been next, make sure to click start before you exit.";

            }
            else if(e.Error != null)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    MessageBox.Show($"{e.Error}");
                }));
            }
            else
            {
                processStatus_LBL.Text = "All done!";
            }
        }

        private void Exit_BTN_Click(object sender, EventArgs e)
        {
            backgroundWorker1.Dispose();
            screenshotAuto.Dispose();
            Application.Exit();
        }
    }
}