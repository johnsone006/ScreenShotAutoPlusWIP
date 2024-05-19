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
using Open.Numeric.Primes;
using System.Numerics;
using Open.Numeric.Primes.Extensions;
using System.Globalization;
using Open.Collections;
namespace ScreenshotAuto
{
    public partial class ScreenshotAutoUI : Form
    {
        Process screenshotAuto;
        private int totalScreenshots;
        private Size screenshotSize;
        private int waitB4;
        private string savePath;
        private int completedScreenshots;
        private readonly List<string> fileNames = new List<string>();
        private readonly List<string> filePaths = new List<string>();


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
               
                int waitB4 = int.Parse(waitB4_TB.Text);
            }
            processStatus_LBL.Text = "Files processed!";
        }
        private void SavePath_BTN_Click(object sender, EventArgs e)
        {
           using(FolderBrowserDialog saveDB = new FolderBrowserDialog())
            {
                saveDB.Description = "Please select a folder that you want the screenshots to be saved to. Though not required, I would recommend that you pick an empty one. But it does need to be not the same folder that contains the files.";
                saveDB.ShowNewFolderButton = true;
                saveDB.ShowDialog();
                savePath_TB.Text = saveDB.SelectedPath;
                savePath = savePath_TB.Text;
            }
        }
        private void GetTotalNumberOfFiles()
        {
            string filePath = filePath_TB.Text;
            DirectoryInfo files = new DirectoryInfo(filePath);
            int totalScreenshots = files.GetFiles().Count();
        }


        private void Prepare_BTN_Click(object sender, EventArgs e)
        {
            long availableSpaceInBytes;
            double totalMemoryForScreenshots;
            List<BigInteger> factors = new List<BigInteger>();
            int bytesPerPixel;
            GetTotalNumberOfFiles();
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
                List<Size> DeterminePossibleSizes()
                {
                    List<Size> possibleSizes = new List<Size>();
                    int i = 0;
                    double width = viewportRect.Width;
                    double height = viewportRect.Height;
                    List<Size> sizeCandidatesToReturn = new List<Size>();
                    DriveInfo saveDrive = new DriveInfo(savePath);
                    availableSpaceInBytes = saveDrive.AvailableFreeSpace;
                    bytesPerPixel = Screen.FromHandle(prepProcess.MainWindowHandle).BitsPerPixel / 8;
                    totalMemoryForScreenshots = viewportRect.Width * viewportRect.Height * bytesPerPixel * totalScreenshots;
                    bool thereWillBeEnoughSpaceOnDriveIfSceenshotSizeEqualsViewportSize = Math.Max(totalMemoryForScreenshots, availableSpaceInBytes).Equals(availableSpaceInBytes);
                    factors.AddRange(Prime.CommonFactors((int)viewportRect.Width, (int)viewportRect.Height));
                    Size possibleSize = new Size();
                    switch (thereWillBeEnoughSpaceOnDriveIfSceenshotSizeEqualsViewportSize)
                    {
                       
                        case true:
                            while (totalMemoryForScreenshots < availableSpaceInBytes);
                            {
                                i++;
                                width *= i;
                                height *= i;
                                totalMemoryForScreenshots = width * height * bytesPerPixel * totalScreenshots;
                                possibleSize = new Size((int)width, (int)height);
                                sizeCandidatesToReturn.Add(possibleSize);
                            }
                            i = 0;
                            foreach (BigInteger factor in factors)
                            {
                               
                                width = width / (double)factors[i];
                                height = height / (double)factors[i];
                                possibleSize = new Size((int)width, (int)height);
                                sizeCandidatesToReturn.Add(possibleSize);
                                i++;
                            }
                            break;
                        case false:
                            i = 0;
                            foreach (BigInteger factor in factors)
                            {
                                width /= (int)factors[i];
                                height /= (int)factors[i];
                                if (height * width * bytesPerPixel * totalScreenshots < availableSpaceInBytes)
                                {
                                    possibleSize = new Size((int)width, (int)height);
                                    sizeCandidatesToReturn.Add(possibleSize);
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
                    foreach(Size size in sizeCandidates)
                    {
                        totalMemoryForScreenshots = sizeCandidates[i].Width * sizeCandidates[i].Height * bytesPerPixel * totalScreenshots;
                        decimal percent = (((decimal)totalMemoryForScreenshots) / availableSpaceInBytes) * 100;
                        string percentString = string.Join("", percent, "%");
                        percentagesToReturn.Add(percentString);
                        i++;
                    }
                    return percentagesToReturn;

                }
                List<string> percentages = CalculatePercentOfMemoryScreenshotsWillTakeUpAtEachSize();
                void FillOutDataGridViews()
                {
                    possibleSizes_DGV.DataSource = sizeCandidates;
                    percentOfMemory_DGV.DataSource = percentages;
                }
                FillOutDataGridViews();
                MessageBox.Show("On the ui of ScreenshotAuto, you should  see two data grid views. The one on the left tells you the possible sizes each screenshot could be while respecting the aspect ratio of the viewport, and the one on the left tells you the percentage of the total available space the screenshots will take up. When you are ready to start the process, go ahead and select one of the percentages, then click start.");
            }

        }

       
    }
}

