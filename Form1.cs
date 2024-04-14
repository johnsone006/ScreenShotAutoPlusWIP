using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing.Text;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.Diagnostics.Eventing.Reader;
using System.Collections.Generic;

namespace ScreenShotAutoPlus
{
    public partial class Form1 : Form
    {
        private bool isPaused;
        //This next variable needs to be a global instance variable because I do not want to have to select the correct folder for multiple methods.
        string filePath;
        //The following two need to be global variables because they need to be equal to the size of topPaneL and panel1 WHEN THE PROGRAM FIRST OPENS.
        private Size topPanel_OS;
        private Size panel1_OS;
        private TaskCompletionSource<bool> pauseCompletionSource;
        Size theSize;
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.TopMost = true;
            pauseCompletionSource = new TaskCompletionSource<bool>();
            pauseCompletionSource.SetResult(false);
            topPanel_OS = new Size(topPanel.Width, topPanel.Height);
            panel1_OS = new Size(panel1.Width, panel1.Height);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //I have officially gotten tired of typing in my values every time I hit debug. So the next statement will only remain here until I get this program working correctly.
            colorCode_TB.Text = "#f72ee0";
            //the rest of the code in this block is what fixed the fact that the ui tended to sometimes either overlap or be overlaped by the taskbar.
            Size formSize = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = formSize;
            this.Location = new Point(0, 0);
        }
        private string OpenAndValidateFP()
        {
            try
            {
                using (FolderBrowserDialog fBD = new FolderBrowserDialog())
                {
                    fBD.Description = "Please select the folder that contains the files to be screenshotted. Remember, this program is not meant to be used to engage in copyright infringement.";
                    fBD.ShowNewFolderButton = false;
                    DialogResult result = fBD.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fBD.SelectedPath))
                    {
                        filePath = fBD.SelectedPath;
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: " +
                    $"{f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + " +
                    $"TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
            return filePath;
        }
        private string OpenAndValidateSP(ref string savePath)
        {
            //making savePath a ref variable so that if the user selects a file path, the program will be able to change the value of filepath from "" to the actual file path in the start method. 
            try
            {
                //the else loop is meant to prevent the screenshot process from even starting if the person did not select a savepath.
                using (FolderBrowserDialog fbd2 = new FolderBrowserDialog())
                {
                    fbd2.Description = "Select a folder for the screenshots to be saved to.";
                    fbd2.ShowNewFolderButton = false;
                    DialogResult result = fbd2.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        savePath = fbd2.SelectedPath;
                        
                    }
                    else
                    {
                     
                        return null;
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: " +
                    $"{f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + " +
                    $"TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
            return savePath;
        }

        bool NumbersValidated(int waitB4, int waitAfter)
        {
            //This method is meant to ensure that the user did not enter in a negative number or a decimal number into the textboxes for the wait periods.
            try
            {
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
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: " +
                    $"{f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + StackTrace: {f.StackTrace}\n + " +
                    $"TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }
        private void Import_BTN_Click(object sender, EventArgs e)
        {
            int waitB4 = 0;
            int waitAfter = 0;
            if (NumbersValidated(waitB4, waitAfter) == true)
            {
                List<string> invalidFiles = new List<string>();
                OpenAndValidateFP();
                try
                {
                    DirectoryInfo di = new DirectoryInfo(filePath);
                    //Make sure it's nif files that are being added.
                    foreach(var file in di.GetFiles())
                    {
                        if(file.Extension == ".nif")
                        {
                            fileName_LB.Items.Add(file.Name);
                            filePath_LB.Items.Add(file.FullName);
                        }
                        else
                        {
                            invalidFiles.Add(file.Name);
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
                    if (invalidFiles.Count > 0)
                    {
                        MessageBox.Show("Some of the files that were in the file path you chose were not imported because the file extension was incorrect. This app is meant for .nif files specifically.", "Information", MessageBoxButtons.OK);
                    }
                }
            }
        }
        private  void CalculateTimeBTN_Click(object sender, EventArgs e)
        {
            int waitB4 = 0;
            int waitAfter = 0;
            int xCoord = 0;
            int yCoord = 0;
            if (NumbersValidated(waitB4, waitAfter) == true)
            {
                string fileInfo = new DirectoryInfo(filePath).GetFiles().OrderByDescending(file => file.Length).FirstOrDefault().FullName;
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = fileInfo,
                    WindowStyle = ProcessWindowStyle.Maximized, 
                };
                using (Process theProcess = new Process { StartInfo = startInfo })
                {
                    DialogResult result= MessageBox.Show("Do you plan to allow the program to automatically press the load view button prior to each screenshot?", "Question", MessageBoxButtons.YesNo);
                    if(result == DialogResult.Yes)
                    {
                        try
                        {
                            theProcess.Start();
                            theProcess.WaitForInputIdle();
                            Thread.Sleep(1000);
                            Bitmap initialSS = FirstScreenshot();
                            
                            Color bingoColor = ConvertHex();

                            int[] coords =  CalculateCoords(ref xCoord, ref yCoord, initialSS, bingoColor);
                            int[]ssSize = ResizeUI(coords[0], coords[1]);
                            theSize = CalculateScreenshotSize(ssSize[0], ssSize[1], initialSS);
                            initialSS.Dispose();
                        }
                        catch (Exception f)
                        {
                            MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + " +
                            $"InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n +" +
                            $" StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                        }
                    }
                }
                MessageBox.Show("Stuff has been calculated, so after you close out the window of Nifskope that opened, you can press the start button now.");
            }
        }
        private Bitmap FirstScreenshot()
        {
            int initialWidth = this.Width; //This will work because I set the window state of rthe form to be maximized.
            int initialHeight = this.Height;
            Bitmap initialSS = new Bitmap(initialWidth, initialHeight);
            Graphics firstSSGraphic = Graphics.FromImage(initialSS);
            Size size = new Size(initialWidth, initialHeight);
            firstSSGraphic.CopyFromScreen(0, 0, 0, 0, size);
            return initialSS;
        }
        private Color ConvertHex()
        {
            string hexadecimalNumber = colorCode_TB.Text;
            if (hexadecimalNumber.StartsWith("#"))
                hexadecimalNumber = hexadecimalNumber.Substring(1);
            int r = Convert.ToInt32(hexadecimalNumber.Substring(0, 2), 16);
            int g = Convert.ToInt32(hexadecimalNumber.Substring(2, 2), 16);
            int b = Convert.ToInt32(hexadecimalNumber.Substring(4, 2), 16);
            Color bingoColor = Color.FromArgb(255, r, g, b);
            return bingoColor;
        }
        private int[] CalculateCoords(ref int xCoord, ref int yCoord, Bitmap initialSS, Color bingoColor)
        {
            Color menuColor;
            int tempY = 2;
            try
            {
                int starterX = initialSS.Width - 5;

                //Figure out y coordinate of location for future screenshots
                for (tempY = 2; tempY < initialSS.Height;)
                {
                    tempY++;
                    menuColor = initialSS.GetPixel(starterX, tempY);
                    if (bingoColor.Equals(menuColor))
                    {
                        yCoord = tempY;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                //Figure out x coordinate of location for future screenshots
                tempY += 5;
                for (int tempX = 0; tempX < initialSS.Width;)
                {
                    tempX++;
                    menuColor = initialSS.GetPixel(tempX, tempY);
                    if (bingoColor.Equals(menuColor))
                    {
                        xCoord = tempX;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n +" +
                   $" InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: " +
                   $"{f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite} \n {tempY}\n{bingoColor}\n {tempY}", "Error", MessageBoxButtons.OK);
            }
            int[] coords = { xCoord, yCoord };
            return coords;
        }
        private Size CalculateScreenshotSize(int width, int height, Bitmap initialSS)
        {
            int ssWidth = initialSS.Width - width;
            int ssHeight = initialSS.Height - height;
            Size theSize = new Size(ssWidth, ssHeight);
            return theSize;
        }
        private int[] ResizeUI(int xCoord, int yCoord)
        {
            Screen thisScreen = Screen.FromControl(this);
            Rectangle workingArea = thisScreen.WorkingArea;
            float xMultiply = xCoord / panel1_OS.Width; //This is how much the size of all controls (except topPanel) will change in width.
            float yMultiply = yCoord / topPanel_OS.Height;//this is how much the size of all controls (except topPanel) will change in height.
            if (yCoord == topPanel_OS.Height)
            {
                if (xCoord == panel1_OS.Width)
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
                int height = workingArea.Height - yCoord;
                panel1.Size = new Size(xCoord, height);
                panel1.Location = new Point(0, yCoord);
            }
            else
            {
                //height needs to change. 
                if (xCoord == panel1_OS.Width)
                {
                    //only height needs to change. 
                    foreach (Control con in panel1.Controls)
                    {

                        con.Anchor = AnchorStyles.Top;
                        int newHeight = (int)(con.Height * yMultiply);
                        con.Size = new Size(con.Width, newHeight);

                    }
                    topPanel.Height = yCoord;
                    int height = (topPanel_OS.Height + panel1_OS.Height) - yCoord;
                    panel1.Size = new Size(xCoord, height);
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
                    topPanel.Height = yCoord;
                    int height = this.Height - yCoord;
                    panel1.Size = new Size(xCoord, height);
                }
            }
            int[] newSizes = new int[2];
            newSizes[0] = panel1.Width;
            newSizes[1] = topPanel.Height;
            return newSizes;
        }
        private async void Start_BTN_Click(object sender, EventArgs e)
        {
            int totalScreenshots = filePath_LB.Items.Count;
            int waitB4 = int.Parse(waitB4_TB.Text);
            int waitAfter = int.Parse(waitAfter_TB.Text);
            int screenshotsLeft;
            string savePath = "";
            int completedScreenshots =0;
          
            if(OpenAndValidateSP(ref savePath) != null)
            {
                if (NumbersValidated(waitB4, waitAfter) == true)
                {
                    DialogResult result = MessageBox.Show("If you press yes, this program will automatically " +
                        "click once on nifskope and then click the load view button in nifskope for every screenshot. " +
                        "Is this acceptable?", "Important question", MessageBoxButtons.YesNoCancel);
                    ProcessStartInfo processStartInfo = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Maximized,

                    };
                    if (result == DialogResult.Yes)
                    {
                        
                        for (int i = 0; i < totalScreenshots; i++)
                        {
                           
                            if (GetSpaceAvailability(savePath)== true)
                            {
                                await pauseCompletionSource.Task;
                                using (Process myProcess = new Process { StartInfo = processStartInfo })
                                {
                                    try
                                    {
                                        DateTime startTime = DateTime.Now;
                                        myProcess.StartInfo.FileName = (string)filePath_LB.Items[completedScreenshots];
                                        myProcess.Start();
                                        myProcess.WaitForInputIdle();
                                        Thread.Sleep(waitB4);
                                        Task<bool> button = AutomaticButtonPress();
                                        bool buttonPressed = await button;
                                        Task<int> screenshot = ScreenshotMethod(ref completedScreenshots, savePath);
                                        completedScreenshots = await screenshot;
                                        DateTime screenshotDone = DateTime.Now;
                                        Thread.Sleep(waitAfter);
                                        TimeSpan screenshotTime = (screenshotDone - startTime);
                                        int screenshotsRemaining = CalculateRemainingScreenshots(completedScreenshots, totalScreenshots);
                                        int timeRemaining = EstimateTimeRemaining(ref screenshotsRemaining, screenshotTime);
                                        screenshotsLeft = screenshotsRemaining;
                                        Task<bool> ui = UpdateUI(ref screenshotsRemaining, completedScreenshots, screenshotsLeft, timeRemaining);
                                        bool updateDone = await ui;
                                        myProcess.Kill();
                                    }
                                    catch (Exception f)
                                    {
                                        MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source} \n+ StackTrace: {f.StackTrace} \n+ TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Ooops! You have run out of room for screenshots!", "Error", MessageBoxButtons.OK);
                                return;
                            }
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        while (completedScreenshots != totalScreenshots)
                        {
                            if (GetSpaceAvailability(savePath)== true)
                            {
                                await pauseCompletionSource.Task;
                                using (Process myProcess = new Process { StartInfo = processStartInfo })
                                {
                                    try
                                    {
                                        DateTime startTime = DateTime.Now;
                                        myProcess.StartInfo.FileName = (string)filePath_LB.Items[completedScreenshots];
                                        myProcess.Start();
                                        Thread.Sleep(waitB4);
                                        Task<int> screenshot = ScreenshotMethod(ref completedScreenshots, savePath);
                                        completedScreenshots = await screenshot;
                                        DateTime screenshotDone = DateTime.Now;
                                        Thread.Sleep(waitAfter);
                                        TimeSpan screenshotTime = (screenshotDone - startTime);
                                        int screenshotsRemaining = CalculateRemainingScreenshots(completedScreenshots, totalScreenshots);
                                        screenshotsLeft = screenshotsRemaining;
                                        int timeRemaining = EstimateTimeRemaining(ref screenshotsRemaining, screenshotTime);
                                        Task<bool> ui = UpdateUI(ref screenshotsRemaining, completedScreenshots, screenshotsLeft, timeRemaining);
                                        bool updateDone = await ui;
                                        myProcess.Kill();
                                    }
                                    catch (Exception f)
                                    {
                                        MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source} \n+ StackTrace: {f.StackTrace} \n+ TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Ooops! You have run out of room for screenshots!", "Error", MessageBoxButtons.OK);
                                return;
                            }
                        }
                    }
                }
            }
        }
        private bool GetSpaceAvailability(string savePath)
        {
            //This method is meant to anticipate how much memory the screenshot will take up prior to taking it, and then comparing that value to the total amount of space on the drive. If the space available is too small, the method will return false and the screenshot will not be taken.
            int bitsPerPixel = Screen.FromControl(this).BitsPerPixel;
            int bytesPerPixel = bitsPerPixel / 8;
            int bytesPerRow = theSize.Width * bytesPerPixel;
            int paddingPerRow = (4 - (bytesPerRow % 4)) % 4;
            int totalRowBytes = (bytesPerRow + paddingPerRow) * theSize.Height;
            int headerSize = 54;
            long totalFileSize = headerSize + totalRowBytes;
            string driveRoot = Path.GetPathRoot(savePath);
            DriveInfo driveInfo = new DriveInfo(driveRoot);
            long spaceLeft = driveInfo.AvailableFreeSpace;
            if(spaceLeft < totalFileSize)
            {
                return false;
            }
            return true;
        }
        private Task<bool> AutomaticButtonPress()
        {
            bool buttonPressed;
            try
            {
                MouseEventArgs args = new MouseEventArgs(MouseButtons.Left, 1, 810, 533, 0);
                OnMouseDown(args);
                OnMouseUp(args);
                SendKeys.SendWait("{F8}");
                SendKeys.SendWait("z");
                buttonPressed = true;
            }
            catch (Exception f)
            {
                MessageBox.Show("Button stack trace:" + f.StackTrace + "Message" + f.Message + "Inner exception" + f.InnerException);
                buttonPressed = false;
            }
            return Task.FromResult(buttonPressed);
        }
        private Task<int> ScreenshotMethod(ref int completedScreenshots, string savePath)
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
                string TreeNodeNameRevised = fileName_LB.Items[completedScreenshots].ToString().Replace(".nif", string.Empty);
                nif.Save(savePath + "\\" + TreeNodeNameRevised + ".bmp", ImageFormat.Bmp);
                imageList1.ImageSize = screenshot_PB.Size;
                imageList1.Images.Add(nif);
                completedScreenshots++;
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n +" +
                    $" InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: " +
                    $"{f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
            return Task.FromResult(completedScreenshots);
        }
        private int CalculateRemainingScreenshots(int completedScreenshots, int totalScreenshots)
        {
            int screenshotsRemaining = totalScreenshots - completedScreenshots;
            return screenshotsRemaining;
        }
        private int EstimateTimeRemaining(ref int screenshotsRemaining, TimeSpan screenshotTime)
        {
            int timeRemaining = (int)screenshotTime.TotalSeconds * screenshotsRemaining;
            return timeRemaining;
        }
        private Task<bool> UpdateUI(ref int screenshotsRemaining, int completedScreenshots, int screenshotsLeft, int timeRemaining)
        {
            screenshotsLeft = screenshotsRemaining;
            string screenshotFile = fileName_LB.Items[completedScreenshots].ToString();
            screenshotFiles_LB.Items.Add(screenshotFile);
            bool invokeRequired = false;
            if (InvokeRequired)
            {
                BeginInvoke((Action)(() =>
                {
                    screenshotNumber_LBL.Text = $"{screenshotsLeft}";
                    timeLeft_LBL.Text = $"{timeRemaining:N} seconds";
                    invokeRequired = true;
                }));
            }
            else
            {
                screenshotNumber_LBL.Text = $"{screenshotsLeft}";
                timeLeft_LBL.Text = $"{timeRemaining:N} seconds";
                invokeRequired = false;
            }
            return Task.FromResult(invokeRequired);
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
                Image img = imageList1.Images[screenshotFiles_LB.SelectedIndex];
                screenshot_PB.Image = img;
            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n +" +
                    $" InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: " +
                    $"{f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
        }

    }
}
