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
using System.Drawing.Imaging;
using System.Runtime.Remoting.Messaging;
using System.Linq.Expressions;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using System.Numerics;


namespace ScreenshotAuto
{
    public partial class ScreenshotAutoUI : Form
    {
        Process screenshotAuto;
        private int totalScreenshots;
        private Size evenedOutViewportSize;
        private int waitB4;
        private string savePath;
        private readonly List<string> fileNames = new List<string>();
        private readonly List<string> filePaths = new List<string>();
        Size selectedSize;

        public ScreenshotAutoUI()
        {
            InitializeComponent();

        }
        private void ScreenshotAutoUI_Load(object sender, EventArgs e)
        {
            System.Drawing.Size formSize = new System.Drawing.Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = formSize;
            this.Location = new System.Drawing.Point(0, 0);
            this.TopMost = true;
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

                //Make sure all the files in the folder they selected for filePath are nif files, because that is the only file type nifskope can open. 

                foreach (var file in fileInfo.GetFiles())
                {
                    if (file.Extension.Equals(".nif"))
                    {
                        fileNames.Add(file.Name);
                        filePaths.Add(file.FullName);
                    }
                }

    
            }
            processStatus_LBL.Text = "Files processed!";
        }
        private void SavePath_BTN_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog saveDB = new FolderBrowserDialog())
            {
                saveDB.Description = "Please select a folder that you want the screenshots to be saved to. Though not required, I would recommend that you pick an empty one. But it does need to be not the same folder that contains the files.";
                saveDB.ShowNewFolderButton = true;
                saveDB.ShowDialog();
                savePath_TB.Text = saveDB.SelectedPath;
                savePath = savePath_TB.Text;
            }
        }
        private void Prepare_BTN_Click(object sender, EventArgs e)
        {
           if(savePath_TB.TextLength == 0 ||filePath_TB.TextLength == 0 || waitB4_TB.TextLength == 0 || int.TryParse(waitB4_TB.Text, out int waitB4)== false)
            {
                MessageBox.Show("Before the program can continue, you need to pick two file paths- one for the files that need to be screenshotted, and one to save the screenshots to- and also enter in a valid value for the pause prior to each screenshot. A valid value is anything above or equal to 0.");
                return;
            }
            waitB4 = int.Parse(waitB4_TB.Text);
            int bytesPerPixel;
            totalScreenshots = filePaths.Count;
            using (Process prepProcess = new Process())
            {
                IntPtr OpenNifskope()
                {
                    prepProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    prepProcess.StartInfo.FileName = filePaths[0];
                    prepProcess.Start();
                    while (prepProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        prepProcess.WaitForInputIdle();
                    }
                    AutomationElement elementThatWillNotBeEnabledUntilNifLoads = AutomationElement.FromHandle(prepProcess.MainWindowHandle).FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, "Save View"));
                    while (elementThatWillNotBeEnabledUntilNifLoads.Current.IsEnabled == false)
                    {
                        prepProcess.WaitForInputIdle();
                    }
                    IntPtr handleToReturn = prepProcess.MainWindowHandle;
                    return handleToReturn;
                }
                IntPtr nifskopeMainWindowHandle = OpenNifskope();
                System.Windows.Rect GetViewportRect()
                {
                    AutomationElement viewportElement = AutomationElement.FromHandle(nifskopeMainWindowHandle).FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ClassNameProperty, "Qt5QWindowOwnDCIcon"));
                    System.Windows.Rect rectToReturn = viewportElement.Current.BoundingRectangle;
                    return rectToReturn;
                }
                System.Windows.Rect viewportRect = GetViewportRect();
                void DetermineIfViewportWidthAndHeightIsEvenAndAdjustAccordingly()
                {
                    double widthOfRect = viewportRect.Width;
                    double heightOfRect = viewportRect.Height;
                    if (widthOfRect % 2 != 0)
                        widthOfRect -= 1;
                    if (heightOfRect % 2 != 0)
                        heightOfRect -= 1;
                    evenedOutViewportSize = new Size((int)widthOfRect, (int)heightOfRect);
                }
               DetermineIfViewportWidthAndHeightIsEvenAndAdjustAccordingly();
                List<int> GetCommonFactors()
                {
                    List<int> widthFactors = new List<int>();
                    List<int> commonFactorsToReturn = new List<int>();
                    for(int i = 1; i < Math.Sqrt(evenedOutViewportSize.Width); i++)
                    {
                        if (evenedOutViewportSize.Width % i == 0)
                        {
                            widthFactors.Add(i);
                        }
                    }
                    for(int i = 1; i < evenedOutViewportSize.Height; i++)
                    {
                        if(evenedOutViewportSize.Height % i == 0 && widthFactors.Contains(i))
                        {
                           commonFactorsToReturn.Add(i);
                        }
                    }
                    //remove 1 from list
                    int indexToRemove = commonFactorsToReturn.IndexOf(1);
                    commonFactorsToReturn.RemoveAt(indexToRemove);

                    return commonFactorsToReturn;
                    
                }
                List<int> commonFactors = GetCommonFactors();
                long DetermineTotalMemoryForScreenshotsAtTheSizeOfTheViewport()
                {
                    bytesPerPixel = Screen.FromHandle(prepProcess.MainWindowHandle).BitsPerPixel / 8;
                    short width = (short)evenedOutViewportSize.Width;
                    short height = (short)evenedOutViewportSize.Height;
                    long memorySizeToReturn = (long)width * (long)height * (long)totalScreenshots * (long)bytesPerPixel ;
                    return memorySizeToReturn;
                }
                long totalMemorySizeOfAllScreenshotsInitially = DetermineTotalMemoryForScreenshotsAtTheSizeOfTheViewport();
                long DetermineHowMuchFreeSpaceIsOnDrive()
                {
                    DriveInfo saveDrive = new DriveInfo(savePath);
                    long availableSpaceToReturn = saveDrive.AvailableFreeSpace;
                    return availableSpaceToReturn;
                }
                long availableSpaceInBytes = DetermineHowMuchFreeSpaceIsOnDrive();
                bool DetermineIfThereWillBeEnoughMemoryForTotalScreenshotsIfEachIsSameSizeAsInitialScreenshotSize()
                {
                    bool enoughSpaceToReturn = BigInteger.Max(availableSpaceInBytes, totalMemorySizeOfAllScreenshotsInitially).Equals(availableSpaceInBytes);
                    return enoughSpaceToReturn;
                    
                }
                bool enoughSpace = DetermineIfThereWillBeEnoughMemoryForTotalScreenshotsIfEachIsSameSizeAsInitialScreenshotSize();
                List<Size> DeterminePossibleSizes()
                {
                    Size candidateSize = new Size();

                    List<Size> sizeCandidatesToReturn = new List<Size>();
                    DriveInfo saveDrive = new DriveInfo(savePath);
                    
                    int widthTemp = evenedOutViewportSize.Width;
                    int heightTemp = evenedOutViewportSize.Height;
                    
                    
                    switch (enoughSpace)
                    {
                        case true:
                            //Figure out how many times the screenshot size can be increased and still have enough memory to have totalScreenshots screenshots of that size.
                            double firstQuotient = (availableSpaceInBytes / evenedOutViewportSize.Width) / evenedOutViewportSize.Height;
                            double secondQuotient = (firstQuotient / totalScreenshots) / bytesPerPixel;
                            double squareRoot = Math.Sqrt(secondQuotient);

                            for (int i = 1; i < squareRoot; i++)
                            {
                                widthTemp *= i;
                                heightTemp *= i;
                                candidateSize = new Size(widthTemp, heightTemp);
                                sizeCandidatesToReturn.Add(candidateSize);
                                widthTemp = evenedOutViewportSize.Width;
                                heightTemp = evenedOutViewportSize.Height;
                            }
                            foreach (int number in commonFactors)
                            {
                                widthTemp /= number;
                                heightTemp /= number;
                                candidateSize = new Size(widthTemp, heightTemp);
                                sizeCandidatesToReturn.Add(candidateSize);
                                widthTemp = evenedOutViewportSize.Width;
                                heightTemp = evenedOutViewportSize.Height;
                            }
                            break;
                        case false:
                            foreach (int number in commonFactors)
                            {
                                heightTemp /= number;
                                widthTemp /= number;
                                if (widthTemp * heightTemp * bytesPerPixel * totalScreenshots < availableSpaceInBytes)
                                {
                                    candidateSize = new Size(widthTemp, heightTemp);
                                    sizeCandidatesToReturn.Add(candidateSize);
                                }
                            }
                            break;
                    }
                    return sizeCandidatesToReturn;
                }
                List<Size> sizeCandidates = DeterminePossibleSizes();
                List<string> CalculatePercentOfMemoryScreenshotsWillTakeUpAtEachSize()
                {
                    int i = 0;
                    List<string> percentagesToReturn = new List<string>();
                    foreach (Size size in sizeCandidates)
                    {
                        int sizeCandidateWidth = sizeCandidates[i].Width;
                        int sizeCandidateHeight = sizeCandidates[i].Height;
                        totalMemorySizeOfAllScreenshotsInitially= (long)sizeCandidateWidth * (long)sizeCandidateHeight * (long)bytesPerPixel * (long)totalScreenshots;
                        double percentAsDecimal = (double)totalMemorySizeOfAllScreenshotsInitially / availableSpaceInBytes;
                        double percentAsNumber = (double)percentAsDecimal * 100;
                        string percentString = string.Join("", percentAsNumber, "%");
                        percentagesToReturn.Add(percentString);
                        i++;
                    }
                    return percentagesToReturn;

                }
                List<string> percentages = CalculatePercentOfMemoryScreenshotsWillTakeUpAtEachSize();
                void FillOutDGV()
                {
                    screenshotInformation_DGV.RowCount = percentages.Count;
                    for(int i = 0; i < sizeCandidates.Count; i++)
                    {
                        string size = string.Join("x", sizeCandidates[i].Width.ToString(), sizeCandidates[i].Height.ToString());
                        screenshotInformation_DGV[0, i].Value = i;
                        screenshotInformation_DGV[1,i].Value = size;
                        screenshotInformation_DGV[2, i].Value = percentages[i];
                    }
                    
                }
                FillOutDGV();
                MessageBox.Show("At this point, the data grid view should be filled with potential sizes of screenshots that will be able to fit on the drive you have chosen. Please enter in the ID of the size youw want into the numberic up down, and then click start.");
                prepProcess.Kill();
                sizeID_NUD.Maximum = screenshotInformation_DGV.RowCount-1;
            }

        }
        private void Start_BTN_Click(object sender, EventArgs e)
        {
            //Get the chosen size.;
            int id = (int)sizeID_NUD.Value;
            string size = screenshotInformation_DGV.Rows[id].Cells[1].Value.ToString();
            string[] sizeArray = size.Split('x');
            int width = int.Parse(sizeArray[0]);
            int height = int.Parse(sizeArray[1]);
            selectedSize = new Size(width, height);
            while(backgroundWorker1.IsBusy == false)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }
        private async void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int completedScreenshots = 0;
            BackgroundWorker worker = sender as BackgroundWorker;
            if(savePath == null || filePaths.Count == 0)
            {
                return;
            }
            while(completedScreenshots != totalScreenshots)
            {
                using(Process screenshotProcess = new Process())
                {
                    if(worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    Task OpenNifskope =  Task.Run(() =>
                    {
                        screenshotProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        screenshotProcess.StartInfo.FileName = filePaths[completedScreenshots];
                        screenshotProcess.Start();
                        while (screenshotProcess.MainWindowHandle == IntPtr.Zero)
                        {
                            Task.Delay(10);
                        }
                        //make sure that the nif has actually loaded before this method returns too.
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
                    Task<Bitmap> TakeAndResizeScreenshot = Task.Run<Bitmap>(() =>
                    {
                        using (Bitmap screenshotOfNif = new Bitmap(evenedOutViewportSize.Width, evenedOutViewportSize.Height))
                        {
                            AutomationElement viewportElement = AutomationElement.FromHandle(screenshotProcess.MainWindowHandle).FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ClassNameProperty, "Qt5QWindowOwnDCIcon"));
                            System.Windows.Rect viewportRect = viewportElement.Current.BoundingRectangle;
                            System.Drawing.Point upperLeftPointOfScreenshot = new Point((int)viewportRect.Left, (int)viewportRect.Top);
                            System.Drawing.Point destinationOfScreenshot = new System.Drawing.Point(0, 0);
                            Graphics theGraphicsOfScreenshot = Graphics.FromImage(screenshotOfNif);
                            theGraphicsOfScreenshot.CopyFromScreen(upperLeftPointOfScreenshot, destinationOfScreenshot, evenedOutViewportSize);
                            Bitmap finalBitmapToReturn = new Bitmap(screenshotOfNif, new Size(selectedSize.Width, selectedSize.Height));
                            return finalBitmapToReturn;
                        }
                    });
                    Bitmap finalBitmap = await TakeAndResizeScreenshot;
                    Task<int> SaveScreenshot = Task.Run<int>(() =>
                    {
                        int completedScreenshotsToReturn = completedScreenshots;
                        string screenshotName = fileNames[completedScreenshots].ToString().Replace(".nif", ".bmp");
                        finalBitmap.Save(savePath + "\\" + screenshotName, ImageFormat.Bmp);
                        completedScreenshotsToReturn++;
                        return completedScreenshotsToReturn;
                    });
                    completedScreenshots = await SaveScreenshot;
                    Task<double> CalculateTimeThisWillTakeInMilliseconds = Task.Run(() =>
                    {
                        double timeInMillisecondsSoFar = screenshotProcess.TotalProcessorTime.TotalMilliseconds;
                        double totalMillisecondsToReturn = timeInMillisecondsSoFar * totalScreenshots;
                        return totalMillisecondsToReturn;
                    });
                    double totalMilliseconds = await CalculateTimeThisWillTakeInMilliseconds;
                    Task<string> FormattedTimeSpan = Task.Run(() =>
                    {
                        TimeSpan totalTimeSpan = TimeSpan.FromMilliseconds(totalMilliseconds);
                        double days = totalTimeSpan.Days;
                        double hours = totalTimeSpan.Hours;
                        double minutes = totalTimeSpan.Minutes;
                        double seconds = totalTimeSpan.Seconds;
                        double milliseconds = totalTimeSpan.Milliseconds;
                        string timeSpanToReturn = string.Join(":", days, hours, minutes, seconds, milliseconds);
                        return timeSpanToReturn;
                    });
                    string timeSpan = await FormattedTimeSpan;

                    Task UpdateUI = Task.Run((() =>
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                           
                            timeLeft_LBL.Text += timeSpan;
                            completedScreenshotsCount_LBL.Text += completedScreenshots.ToString();
                        }));
                    }));
                    await UpdateUI;
                    screenshotProcess.Kill();
                }//end of using block
            }//end of while loop

        }//end of background worker method

    }
}