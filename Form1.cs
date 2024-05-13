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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


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
        //I do not want to have to invoke the UI  of ScreenshotAuto every time a task in the background worker do work  method needs to access either of the user specified file paths or the user specified waitB4. So...I'm storing them in member variables.
        TaskCompletionSource<bool> pauseCompletionSource = new TaskCompletionSource<bool>();
        string filePath;
        string savePath;
        int waitB4;
        private bool isPaused;

        readonly List<string> fileNames = new List<string>();
        private void Form1_Load(object sender, EventArgs e)
        {
            //the rest of the code in this block is what fixed the fact that the ui tended to sometimes either overlap or be overlaped by the taskbar.
            System.Drawing.Size formSize = new System.Drawing.Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = formSize;
            this.Location = new System.Drawing.Point(0, 0);
        }
        
        private void Import_BTN_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    using(FolderBrowserDialog dialogBD = new FolderBrowserDialog())
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
            }));
        }
        private void Save_FolderBTN_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)(() =>
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
            }));
        }
        private bool StoreUserInputInClassVariables()
        {
            if (int.TryParse(waitB4_TB.Text, out int wait) == true && filePath_TB.TextLength != 0 && savePath_TB.TextLength != 0)
            {
                filePath = filePath_TB.Text;
                savePath = savePath_TB.Text;
                waitB4 = wait;
                return true;
            }
            else
            {
                return false;
            }
        }
        private void Start_BTN_Click(object sender, EventArgs e)
        {
            if(StoreUserInputInClassVariables() == false)
            {
                MessageBox.Show("Prior to starting, you must select a folder that contains the files to be screenshotted, a folder for files to be saved (not the same folder!) && the wait time must be equal to or greater than 0.");
                return;
            }
            if (BackgroundWorker1.IsBusy != true)
            {
                BackgroundWorker1.RunWorkerAsync();
            }
        }
        private async void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            pauseCompletionSource.SetResult(false);
            System.Drawing.Size theSizeOfScreenshots = new System.Drawing.Size();
            bool sufficientSpace = true;
            Task<int> CountScreenshots = Task.Run<int>(() =>
            {
                int totalScreenshotsLocal = fileNames.Count;
                return totalScreenshotsLocal;
            });
            int totalScreenshots = await CountScreenshots;
            //Open initial window to use to make sure the visible part of the ui of Screenshot Auto won't overlap the viewport once the screenshots have started being taken.
            AutomationElement mainWindowOfNifskope = null;
            using (Process initialProcess = new Process())
            {
                Task openNifSkopeInitially = Task.Run(() =>
                {
                    initialProcess.StartInfo.FileName = filePath + "\\" + fileNames[0];
                    initialProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    initialProcess.Start();
                    //Make sure that NifSkope has loaded enough to have a main window handle.
                    while (initialProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        initialProcess.WaitForInputIdle();
                    }
                    //also make sure that Nifskope has actually loaded the file.
                    AutomationElement elementThatWillNotBeEnabledUntilNifSkopeLoadsTheFile = AutomationElement.FromHandle(initialProcess.MainWindowHandle).FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, "Save View"));
                    bool enabled = false;
                    do
                    {
                        enabled = elementThatWillNotBeEnabledUntilNifSkopeLoadsTheFile.Current.IsEnabled;
                    }
                    while (enabled == false);
                    return Task.CompletedTask;
                });
                await openNifSkopeInitially;
                mainWindowOfNifskope = AutomationElement.FromHandle(initialProcess.MainWindowHandle);
                //Get the viewport size and location.
                Task<System.Windows.Rect> GetViewPortInfo = Task.Run<System.Windows.Rect>(() =>
                {
                    AutomationElement viewPortToReturn = mainWindowOfNifskope.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ClassNameProperty, "Qt5QWindowOwnDCIcon"));
                    System.Windows.Rect theRectangleOfTheViewPort = viewPortToReturn.Current.BoundingRectangle;
                    theSizeOfScreenshots = new System.Drawing.Size((int)theRectangleOfTheViewPort.Width, (int)theRectangleOfTheViewPort.Height);
                    //Nifskope converts the bounding rectangle values to doubles

                    return theRectangleOfTheViewPort;
                });
                System.Windows.Rect viewPortRect = await GetViewPortInfo;
                Task<bool> EnsureUserHasEnoughMemoryForScreenshots = Task.Run<bool>(() =>
                {
                    bool localSufficientSpace;

                    this.Invoke((MethodInvoker)(() =>
                    {

                        int bitsPerPixel = Screen.FromControl(this).BitsPerPixel;
                        double totalBitsPerScreenshot = (theSizeOfScreenshots.Width * bitsPerPixel) + (theSizeOfScreenshots.Height * bitsPerPixel);
                        double totalMegaBytesPerScreenshot = totalBitsPerScreenshot / 0.00000125;
                        string driveRoot = Path.GetPathRoot(savePath_TB.Text);
                        DriveInfo driveInfo = new DriveInfo(driveRoot);
                        double spaceAvailableInBytes = driveInfo.TotalFreeSpace;
                        double spaceAvailableInMegabytes = spaceAvailableInBytes / 0.000001;
                        double totalSpaceRequired = totalScreenshots * totalMegaBytesPerScreenshot;
                        if (spaceAvailableInMegabytes < totalSpaceRequired)
                        {
                            double additionalSpaceNeededInMegaBytes = totalSpaceRequired - spaceAvailableInMegabytes;
                            MessageBox.Show($"It looks like you do not have enough memory to save that many screenshots. You need {additionalSpaceNeededInMegaBytes} more megabytes worth of space.");
                            localSufficientSpace = false;
                            sufficientSpace = localSufficientSpace;
                        }

                    }));

                    return Task.FromResult(sufficientSpace);
                });
                sufficientSpace = await EnsureUserHasEnoughMemoryForScreenshots;
                Task MakeSureUIDoesNotOverlapViewPort = Task.Run(() =>
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        int sidePanelRight = panel1.Bounds.Right;

                        if (sidePanelRight >= viewPortRect.Left)
                        {
                            double xValueToMultiply = viewPortRect.X / panel1.Width;
                            //Resize child controls of panel first.
                            foreach (Control con in panel1.Controls)
                            {
                                con.Anchor = AnchorStyles.Left & AnchorStyles.Top;
                                int newWidth = (int)(con.Width * xValueToMultiply);
                                con.Size = new System.Drawing.Size(newWidth, con.Height);
                            }
                            int newWidthOfPanel = (int)viewPortRect.Left;
                            panel1.Size = new System.Drawing.Size(newWidthOfPanel, panel1.Height);
                        }
                    }));
                    //If user complain about the side panel of Screenshot auto overlapping the top tool bar, the most recent commmit in the git repo that fixes that was posted on 5/6/2024. The 28th commit.
                    return Task.CompletedTask;
                });
                Task exitNifSkope = Task.Run(() =>
                {
                    initialProcess.Kill();
                    return Task.CompletedTask;

                });
                await Task.WhenAll(exitNifSkope);
            }//End of first using block.
            if (sufficientSpace == false)
                {
                    return;
                }
                else
                {
                    
                using (Process nifskopeScreenshotProcess = new Process())
                {
                    //I want the ui of this program to report the total time remaining as accurately as possible. So I've divided the process into three time intervals: the screenshot time, the calculate time, and the update ui time. You'll see.
                    int completedScreenshots = 0;
                    while (completedScreenshots != totalScreenshots)
                    {
                        DateTime screenshotStartTime = DateTime.Now;
                        Task OpenNifSkope = Task.Run(() =>
                        {
                            nifskopeScreenshotProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                            nifskopeScreenshotProcess.StartInfo.FileName = filePath + "\\" + fileNames[completedScreenshots];
                            nifskopeScreenshotProcess.Start();
                            //Wait for nifskope to have a mainwindow handle that is not a bunch of zeros.
                            while (nifskopeScreenshotProcess.MainWindowHandle == IntPtr.Zero)
                            {
                                Task.Delay(1);
                            }
                            //Make sure that this task does not return until the nif has actually loaded.
                            AutomationElement anElementOfTheUiOfNifSkopeThatWillNotBeEnabledUntilNifSkopeHasLoadedTheFile = AutomationElement.FromHandle(nifskopeScreenshotProcess.MainWindowHandle).FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, "Save View"));
                            bool isEnabled = false;
                            do
                            {
                                isEnabled = anElementOfTheUiOfNifSkopeThatWillNotBeEnabledUntilNifSkopeHasLoadedTheFile.Current.IsEnabled;
                            }
                            while (isEnabled == false);
                            return Task.CompletedTask;
                        });
                        await OpenNifSkope;
                        await Task.Run(() => nifskopeScreenshotProcess.WaitForInputIdle(waitB4));
                        Task<Bitmap> TakeScreenshot = Task.Run<Bitmap>(() =>
                        {
                            AutomationElement viewportElement = AutomationElement.FromHandle(nifskopeScreenshotProcess.MainWindowHandle).FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ClassNameProperty, "Qt5QWindowOwnDCIcon"));
                            System.Windows.Rect rectangleOfViewport = viewportElement.Current.BoundingRectangle;
                            System.Drawing.Point upperLeftOfScreenshotArea = new System.Drawing.Point((int)rectangleOfViewport.Left, (int)rectangleOfViewport.Top);
                            System.Drawing.Point destinationOfScreenshot = new System.Drawing.Point(0, 0);
                            Bitmap screenshotOfNif = new Bitmap(theSizeOfScreenshots.Width, theSizeOfScreenshots.Height);
                            Graphics theGraphicsOfTheScreenshot = Graphics.FromImage(screenshotOfNif);
                            theGraphicsOfTheScreenshot.CopyFromScreen(upperLeftOfScreenshotArea, destinationOfScreenshot, theSizeOfScreenshots);
                            return screenshotOfNif;
                        });
                        Bitmap theScreenshot = await TakeScreenshot;
                        Task<int> SaveScreenshot = Task.Run<int>(() =>
                        {
                            int localScreenshotCount = completedScreenshots;
                            string screenshotName = fileNames[completedScreenshots].ToString().Replace(".nif", string.Empty);
                            theScreenshot.Save(savePath + "\'" + screenshotName);
                            localScreenshotCount++;
                            return localScreenshotCount;
                        });
                        completedScreenshots = await SaveScreenshot;
                        await Task.Run(() => theScreenshot.Dispose());
                        await Task.Run(() => nifskopeScreenshotProcess.Kill());
                        DateTime screenshotDone = DateTime.Now;
                        DateTime calculateTimeStart = DateTime.Now;
                        Task<int> calculateRemainingScreenshots = Task.Run<int>(() =>
                        {
                            int remainingScreenshotsIntegerToReturn = totalScreenshots - completedScreenshots;
                            return remainingScreenshotsIntegerToReturn;
                        });
                        int screenshotsRemaining = calculateRemainingScreenshots.Result;
                        DateTime calculateTimeDone = DateTime.Now;
                        DateTime updateUIStart = DateTime.Now;
                        Task UpdateUi = Task.Run(() =>
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                screenshotNumber_LBL.Text = $"{screenshotsRemaining}";
                            }));
                        });
                        DateTime updateUIEnd = DateTime.Now;
                        Stopwatch stopWatchForCalculatingTimeSpans = Stopwatch.StartNew();
                        Task<double> CalculateAndAddUpTimeSpans = Task.Run<double>(() =>
                        {
                            double screenshotTime = (screenshotDone - screenshotStartTime).TotalMilliseconds * screenshotsRemaining;
                            double calculateTime = (calculateTimeDone - calculateTimeStart).TotalMilliseconds;
                            double updateUITime = 2 * (updateUIEnd - updateUIStart).TotalMilliseconds;
                            //i multiplied that by two to account for the amount of time it'll take to update the ui with the time remaining.
                            double totalOveralTime = screenshotTime + calculateTime + updateUITime;
                            return totalOveralTime;
                        });
                        double totalOverallTimeInMilliseconds = await CalculateAndAddUpTimeSpans;
                        Task<string> FormatTimeSpan = Task.Run<string>(() =>
                        {
                            stopWatchForCalculatingTimeSpans.Stop();
                            double doubleMilliseconds = totalOverallTimeInMilliseconds + stopWatchForCalculatingTimeSpans.ElapsedMilliseconds;
                            TimeSpan timeSpanInMilliseconds = TimeSpan.FromMilliseconds(doubleMilliseconds);
                            double days = (int)timeSpanInMilliseconds.Days;
                            double hours = (int)timeSpanInMilliseconds.Hours;
                            double minutes = (int)timeSpanInMilliseconds.Minutes;
                            double seconds = (int)timeSpanInMilliseconds.Seconds;
                            double milliseconds = (int)timeSpanInMilliseconds.Milliseconds;
                            string formmattedTimeSpan = days.ToString() + ":" + hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString() + ":" + milliseconds.ToString();
                            return formmattedTimeSpan;
                        });
                        string timeSpan = await FormatTimeSpan;

                        Task updateUIWithRemainingTime = Task.Run(() =>
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                timeLeft_LBL.Text = "Approximately " + timeSpan;
                            }));
                        });
                    }//end of while loop.
                }
                
            }//end of second using block.
        }//End of background worker method.
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
        private void Exit_BTN_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}