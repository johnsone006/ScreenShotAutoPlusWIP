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

namespace ScreenShotAutoPlus
{
    public partial class Form1 : Form
    {
        //This is a bool that I have been attempting to use to pause the screenshot process.
        //In other words, make the program cease opening, screenshotting, and closing nifskope.
        private bool isPaused;
        //This line of code is meant to ensure that nifskope always opens on the same monitor. 
        //self explanatory
        int completedScreenshots;

        int totalScreenshots;
        int timeRemaining;

        TimeSpan screenshotTime;
        bool buttonPressed;
        string savePath;
        string filePath;
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
            imageList1.ImageSize = new Size(screenshot_PB.Width, screenshot_PB.Height);
        
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //I have officially gotten tired of typing in my values every time I hit debug. So the following statement will only remain here until I get this program working correctly.
            colorCode_TB.Text = "#f72ee0";
           
        }
        private string OpenAndValidateFP()
        {
            try
            {

                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Please select the folder that contains the files to be screenshotted. Remember, this program is not meant to be used to engage in copyright infringement.";
                    fbd.ShowNewFolderButton = false;
                    DialogResult result = fbd.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        filePath = fbd.SelectedPath;
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
        private string OpenAndValidateSP()
        {
            try
            {
                using (FolderBrowserDialog fbd2 = new FolderBrowserDialog())
                {
                    fbd2.Description = "Select a folder for the screenshots to be saved to.";
                    fbd2.ShowNewFolderButton = false;
                    DialogResult result = fbd2.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        savePath = fbd2.SelectedPath;
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

        bool NumbersValidated(int waitB4, int waitAfter, Rectangle monitorBounds)
        {
            try
            {
                monitorBounds = Screen.FromControl(this).WorkingArea;

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

            Rectangle monitorBounds = Screen.FromControl(this).WorkingArea;
            OpenAndValidateFP();


            if (NumbersValidated(waitB4, waitAfter, monitorBounds) == true)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(filePath);
                    foreach (var file in di.GetFiles())
                    {
                        fileName_LB.Items.Add(file.Name);
                        filePath_LB.Items.Add(file.FullName);
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + " +
                        $"InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n + " +
                        $"StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                }

                //I'm implementing a try catch block here so I can make sure the filePath_LB is filled before I do this:
                totalScreenshots = filePath_LB.Items.Count;
            }
        }

        private async void Start_BTN_Click(object sender, EventArgs e)
        {

            int waitB4 = int.Parse(waitB4_TB.Text);
            int waitAfter = int.Parse(waitAfter_TB.Text);
            Rectangle monitorBounds = Screen.AllScreens[0].WorkingArea;
            int screenshotsLeft;
            OpenAndValidateSP();
            if (NumbersValidated(waitB4, waitAfter, monitorBounds) == true)
            {
                DialogResult result = MessageBox.Show("If you press yes, this program will automatically " +
                    "click once on nifskope and then click the load view button in nifskope for every screenshot. " +
                    "Is this acceptable?", "Important question", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    for (int i = 0; i < totalScreenshots; i++)
                    {
                        long spaceAvailable = GetSpaceAvailability();
                        if (spaceAvailable != 0)
                        {
                            await pauseCompletionSource.Task;
                            using (Process myProcess = new Process())
                            {
                                try
                                {

                                    DateTime startTime = DateTime.Now;
                                    myProcess.StartInfo.FileName = (string)filePath_LB.Items[completedScreenshots];
                                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                                    myProcess.StartInfo.Arguments = $"-left {monitorBounds} -top {monitorBounds.Top} -width {monitorBounds.Width} -height {monitorBounds.Height}";
                                    myProcess.Start();
                                    Thread.Sleep(waitB4);
                                    Task<bool> button = AutomaticButtonPress();
                                    buttonPressed = await button;
                                    Task<int> screenshot = ScreenshotMethod(ref startTime);
                                    completedScreenshots = await screenshot;
                                    DateTime screenshotDone = DateTime.Now;
                                    Thread.Sleep(waitAfter);
                                    screenshotTime = (screenshotDone - startTime);
                                    int screenshotsRemaining = CalculateRemainingScreenshots();
                                    int time = EstimateTimeRemaining(ref screenshotsRemaining);
                                    screenshotsLeft = screenshotsRemaining;
                                    Task<bool> ui = UpdateUI(ref screenshotsRemaining, ref completedScreenshots, screenshotsLeft);
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
                        long spaceAvailable = GetSpaceAvailability();
                        if (spaceAvailable != 0)
                        {
                            await pauseCompletionSource.Task;
                            using (Process myProcess = new Process())
                            {
                                try
                                {
                                    DateTime startTime = DateTime.Now;
                                    myProcess.StartInfo.FileName = (string)filePath_LB.Items[completedScreenshots];
                                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                                    myProcess.StartInfo.Arguments = $"-left {monitorBounds} -top {monitorBounds.Top} -width {monitorBounds.Width} -height {monitorBounds.Height}";
                                    myProcess.Start();
                                    Thread.Sleep(waitB4);
                                    Task<int> screenshot = ScreenshotMethod(ref startTime);
                                    completedScreenshots = await screenshot;
                                    DateTime screenshotDone = DateTime.Now;
                                    Thread.Sleep(waitAfter);
                                    screenshotTime = (screenshotDone - startTime);
                                    int screenshotsRemaining = CalculateRemainingScreenshots();
                                    screenshotsLeft = screenshotsRemaining;
                                    int time = EstimateTimeRemaining(ref screenshotsRemaining);
                                    Task<bool> ui = UpdateUI(ref screenshotsRemaining, ref completedScreenshots, screenshotsLeft);
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
        private long GetSpaceAvailability()
        {
            string driveRoot = Path.GetPathRoot(savePath);
            DriveInfo driveInfo = new DriveInfo(driveRoot);
            long spaceLeft = driveInfo.AvailableFreeSpace;
            return spaceLeft;
        }
        private Task<bool> AutomaticButtonPress()
        {
            try
            {
                MouseEventArgs args = new MouseEventArgs(MouseButtons.Left, 1, 810, 533, 0);
                OnMouseDown(args);
                OnMouseUp(args);
                SendKeys.SendWait("{F8}");
                buttonPressed = true;
            }
            catch (Exception f)
            {
                MessageBox.Show("Button stack trace:" + f.StackTrace + "Message" + f.Message + "Inner exception" + f.InnerException);
                buttonPressed = false;
            }
            return Task.FromResult(buttonPressed);
        }
        private Task<int> ScreenshotMethod(ref DateTime startTime)
        {
            try
            {
                OpenAndValidateSP();
                //Graphics graphicsNif = Graphics.FromImage(nif);
                //graphicsNif.CopyFromScreen(coords, dest, size);

                string TreeNodeNameRevised = fileName_LB.Items[completedScreenshots].ToString().Replace(".nif", string.Empty);
                //nif.Save(savePath + "\\" + TreeNodeNameRevised + ".bmp", ImageFormat.Bmp);
                //imageList1.Images.Add(nif);
                screenshotTime = (DateTime.Now - startTime);
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
        private int CalculateRemainingScreenshots()
        {
            int screenshotsRemaining = totalScreenshots - completedScreenshots;
            return screenshotsRemaining;
        }
        private int EstimateTimeRemaining(ref int screenshotsRemaining)
        {
            timeRemaining = (int)screenshotTime.TotalSeconds * screenshotsRemaining;
            return timeRemaining;
        }
        private Task<bool> UpdateUI(ref int screenshotsRemaining, ref int completedScreenshots, int screenshotsLeft)
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
        private async void CalculateTimeBTN_Click(object sender, EventArgs e)
        {

            //measure time
            int waitB4 = 0;
            int waitAfter = 0;
            DateTime startCalcTime;
            DateTime endCalcTime;
            int xCoord = 0;
            int yCoord = 0;
            Rectangle monitorBounds = Screen.AllScreens[0].WorkingArea;
            //MAKE ABSOLUTELY SURE TO MAKE THE VALUE OF WAITB4 to wT!!
            if (NumbersValidated(waitB4, waitAfter, monitorBounds) == true)
            {
                try
                {
                    string fileInfo = new DirectoryInfo(filePath).GetFiles().OrderByDescending(file => file.Length).FirstOrDefault().FullName;
                    using (Process theProcess = new Process())
                    {
                        theProcess.StartInfo.FileName = fileInfo;
                        theProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        theProcess.Start();
                        startCalcTime = DateTime.Now;
                        Thread.Sleep(waitAfter);
                        await AutomaticButtonPressForCalculation();

                        theProcess.WaitForInputIdle();
                        Thread.Sleep(500);
                        endCalcTime = DateTime.Now;
                        Task<Bitmap> img = FirstScreenshot();
                        Bitmap initialSS = await img;
                        Task<Point> coordinateCalculation = CalculateCoords(ref xCoord, ref yCoord, ref initialSS);
                        Point thePoint = await coordinateCalculation;
                        theSize = CalculateScreenshotSize(xCoord, yCoord);
                        await ResizeUI(xCoord, yCoord);
                        initialSS.Dispose();
                        theProcess.Kill();
                    }
                    int wT = (int)(endCalcTime - startCalcTime).TotalSeconds;
                    stateTxt.Text = wT.ToString();
                }
                catch (Exception f)
                {
                    MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n + " +
                        $"InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: {f.Source}\n +" +
                        $" StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
                }
                MessageBox.Show("Time calculated!");
            }
        }
        private Task<Bitmap> FirstScreenshot()
        {
            int initialWidth = this.Width; //This will work because I set the window state of rthe form to be maximized.
            int initialHeight = this.Height;
            Bitmap initialSS = new Bitmap(initialWidth, initialHeight);
            Graphics firstSSGraphic = Graphics.FromImage(initialSS);
            Size size = new Size(initialWidth, initialHeight);
            firstSSGraphic.CopyFromScreen(0, 0, 0, 0, size);
    
            return Task.FromResult(initialSS);
        }
        private Task<Point> CalculateCoords(ref int xCoord, ref int yCoord, ref Bitmap initialSS)
        {
            string hexadecimalCode = colorCode_TB.Text;
            Color bingoColor = ColorTranslator.FromHtml(hexadecimalCode);
            Screen theS = Screen.FromControl(this);
            int starterX = initialSS.Width - 5;

            Color menuColor = initialSS.GetPixel(starterX, 2);
            //Figure out y coordinate of location for future screenshots
            for (int tempY = 2; tempY < initialSS.Height;)
            {
                tempY++;
                menuColor = initialSS.GetPixel(starterX, tempY);
                if (bingoColor.Equals(menuColor))
                {
                    yCoord = tempY - 1;
                    break;
                }
                else
                {
                    continue;
                }
            }
            //Figure out x coordinate of location for future screenshots
            for (int tempX = 0; tempX < initialSS.Width;)
            {
                tempX++;
                menuColor = initialSS.GetPixel(tempX, (initialSS.Height / 2));
                if (bingoColor.Equals(menuColor))
                {
                    xCoord = tempX - 1;
                    break;
                }
                else
                {
                    continue;
                }
            }
            Point thePoint = new Point(xCoord, yCoord);
            return Task.FromResult(thePoint);
        }
        private Size CalculateScreenshotSize(int xCoord, int yCoord)
        {

            Screen theS = Screen.FromControl(this);
            int height = theS.WorkingArea.Height;
            int width = theS.WorkingArea.Width;
            int ssWidth = width - xCoord;
            int ssHeight = height - yCoord;
            Size theSize = new Size(ssWidth, ssHeight);
            return theSize;
        }
        private Task ResizeUI(int xCoord, int yCoord)
        {
            Size originalPanel1Size = panel1.Size;
            Size originalTopPanelSize = topPanel.Size;
            float xMultiply = xCoord / originalPanel1Size.Width;
            float yMultiply = yCoord / originalTopPanelSize.Height;
            if(yCoord == originalTopPanelSize.Height)
            {
                if (xCoord == originalPanel1Size.Width)
                {
                    //Somehow, everything is sized perfectly already so change nothing.
                }
                else
                {
                    foreach(Control con in panel1.Controls)
                    {
                        //this is the foreach  that **SHOULD** apply to mine. see the commit commit to see what happens instead.
                         
                        con.Anchor = AnchorStyles.Left;
                        con.Size = new Size((int)(xMultiply * con.Width),(con.Height*1));
                    }
                }
            }
            else
            {
                //height needs to change. 
                if(xCoord == originalPanel1Size.Width)
                {
                    //only height needs to change. 
                    foreach(Control con in panel1.Controls)
                    {
                      
                        con.Anchor = AnchorStyles.Top;
                        con.Size = new Size(con.Width,(int)(yMultiply * con.Height));
                    }
                }
                else
                {
                    //Both height and width needs to change.
                    foreach(Control con in panel1.Controls)
                    {
                        con.Anchor = AnchorStyles.Left & AnchorStyles.Top;
                        con.Size = new Size((int)(xMultiply * con.Width), (int)(yMultiply * con.Height));
                    }
                }
            }
            if(yCoord > originalTopPanelSize.Height)
            {
                //panel1 needs to be shorter. 
               
                int toSubtract = yCoord - originalTopPanelSize.Height;
                panel1.Size =  new Size(xCoord, panel1.Height - toSubtract);
                topPanel.Height = yCoord;
            }
            else if (yCoord < originalTopPanelSize.Height)
            {
               int toAdd = originalTopPanelSize.Height - yCoord;
                panel1.Size = new Size(xCoord, (panel1.Height + toAdd));
            }
            filePathSize_TB.Text = processStatus_LBL.Size.ToString();
            panel1Size_TB.Text = panel1.Width.ToString();
            return Task.CompletedTask;
        }

        private Task AutomaticButtonPressForCalculation()
        {
            try
            {
                MouseEventArgs args2 = new MouseEventArgs(MouseButtons.Left, 1, 810, 533, 0);
                OnMouseDown(args2);
                OnMouseUp(args2);
                SendKeys.SendWait("{F8}");
                SendKeys.SendWait("{z}");

            }
            catch (Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n +" +
                    $" InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: " +
                    $"{f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);

            }
            return Task.CompletedTask;
        }
        private void screenshotFiles_LB_MouseClick(object sender, MouseEventArgs e)
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
