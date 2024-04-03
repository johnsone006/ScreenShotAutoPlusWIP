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
using System.Windows.Forms.VisualStyles;
using System.Dynamic;
using System.Security.AccessControl;

namespace ScreenShotAutoPlus
{
    public partial class Form1 : Form
    {
        private Size originalPanel1Size;
       
        //This is a bool that I have been attempting to use to pause the screenshot process.
        //In other words, make the program cease opening, screenshotting, and closing nifskope.
        private bool isPaused;
        //This line of code is meant to ensure that nifskope always opens on the same monitor. 
        //self explanatory
        int completedScreenshots;
        private string savePath;
        int totalScreenshots;
        int timeRemaining;
        //Make folllowing 3 global for now.
        int ssHeight;
        int ssWidth;
        Size theSize;
        TimeSpan screenshotTime;
        bool buttonPressed;


        private TaskCompletionSource<bool> pauseCompletionSource;
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            /*This was implemented back when it was necessary for ScreenshotAuto to
            be the topmost app because the automatic button press relied on a transparent button.
            Microsofts website told me that if I were to position that button above another button in Nifksope
            and then set the backcolor to the transparency key and then press that transparent button,
            it'd be the same thing as pressing that button in nifskope.
            It most certainly did NOT turn out that way.*/
            this.TopMost = true;
            pauseCompletionSource = new TaskCompletionSource<bool>();
            pauseCompletionSource.SetResult(false);
            originalPanel1Size = panel1.Size;

            toolTip1.SetToolTip(waitB4_TB, "Textbox to type the amount of miliseconds you want the program to wait prior to each screenshot (to convert seconds to miliseconds, multiply seconds by 1000");
            toolTip1.SetToolTip(waitAfter_TB, "Textbox to type the amount of miliseconds you want the program to wait after each screenshot.");
            //Rectangles to resize stuff with

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Set initial size and location of panel1
            panel1.Size = new Size(Screen.FromControl(this).WorkingArea.Width, Screen.FromControl(this).WorkingArea.Height);
            waitB4_TB.Text = "2000";
            waitAfter_TB.Text = "2000";

        }
        bool NumbersValidated(int waitB4, int waitAfter, Rectangle monitorBounds, string filePath)
        {
            try
            {
                using (var folderBrowserDialog = new FolderBrowserDialog())
                {
                    folderBrowserDialog.Description = "Select the folder that contains the files you want to screenshot";
                    DialogResult result = folderBrowserDialog.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                    {
                        filePath = folderBrowserDialog.SelectedPath;
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong");
                        return false;
                    }
                }
                using (var folderBD = new FolderBrowserDialog())
                {
                    folderBD.Description = "Select the folder that contains the files you want to screenshot";
                    DialogResult result = folderBD.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBD.SelectedPath))
                    {
                        savePath = folderBD.SelectedPath;
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong");
                        return false;
                    }
                }
                monitorBounds = Screen.AllScreens[0].WorkingArea;
                /*Since these values are going to be used to take a screeneshot, 
                I should make sure that the user doesn't input any values that would mean the rectangle would extend past the monitor bounds.*/

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
            string filePath = "";
            
            
            int waitB4 = 0;
            int waitAfter = 0;
            Rectangle monitorBounds = Screen.AllScreens[0].WorkingArea;
     
            if (NumbersValidated(waitB4, waitAfter, monitorBounds, filePath) == true)
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
        private async Task<Size> CalculateStuff(int[] info)
        {
            Screen theScreen = Screen.FromControl(this);
            int initialWidth = theScreen.WorkingArea.Width;
            int initialHeight = theScreen.WorkingArea.Height;

            int xCoord = 0;
            int yCoord = 0;
            Size theSize = new Size(initialWidth, initialHeight);
            using (Process firstProcess = new Process())
            {
                firstProcess.StartInfo.FileName = "nifskope.exe";
                firstProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                firstProcess.Start();
                Task<Bitmap> fSS = FirstScreenshot(ref initialWidth, ref initialHeight);
                Bitmap initialSS = await fSS;
                Task<Point> calc = CalculateCoords(xCoord, yCoord, ref initialSS);
                Point thePoint = await calc;
                theSize = CalculateScreenshotSize(ref xCoord, ref yCoord);
                firstProcess.Kill();
            }

            info[0] = xCoord;
            info[1] = yCoord;
            Task resize = ResizeUi(ref xCoord, ref yCoord);
            await resize;

            return theSize; //size the ss will be, NOT the ui
        }


        private Task<Bitmap> FirstScreenshot(ref int initialWidth, ref int initialHeight)
        {
            Bitmap initialSS = new Bitmap(initialWidth, initialHeight);
            Graphics firstSSGraphics = Graphics.FromImage(initialSS);

            Size size = new Size(initialWidth, initialHeight);
            firstSSGraphics.CopyFromScreen(0, 0, 0, 0, size);
            return Task.FromResult(initialSS);

        }
        private Task<Point> CalculateCoords(int xCoord, int yCoord, ref Bitmap initialSS)
        {

            Color bingoColor = Color.FromArgb(255, 46, 46, 46);
            Screen theS = Screen.FromControl(this);
            int halfX = theS.WorkingArea.Width / 2;
            int halfY = theS.WorkingArea.Height / 2;
            Color menuColor = initialSS.GetPixel(halfX, 2);
            for (int tempY = 2; halfY < theS.WorkingArea.Height;)
            {
                tempY++;
                menuColor = initialSS.GetPixel(halfX, tempY);
                if (menuColor != bingoColor)
                {
                    continue;
                }
                else
                {
                    yCoord = tempY - 1;
                }
            }
            for (int tempX = 2; tempX < theS.WorkingArea.Width;)
            {
                tempX++;
                menuColor = initialSS.GetPixel(tempX, halfY);
                if (menuColor != bingoColor)
                {
                    continue;
                }
                else
                {
                    xCoord = tempX - 1;
                }
                MessageBox.Show("Found coordinates!", "Success!", MessageBoxButtons.OK);

            }
            Point thePoint = new Point(xCoord, yCoord);

            return Task.FromResult(thePoint);
        }
        private Size CalculateScreenshotSize(ref int xToSubtract, ref int yToSubtract)
        {
            Screen theS = Screen.FromControl(this);
            int height = theS.WorkingArea.Height;
            int width = theS.WorkingArea.Width;
            int ssHeight = height - xToSubtract;
            int ssWidth = width - yToSubtract;
            Size theSize = new Size(ssWidth, ssHeight);
            return theSize;
        }
     
    private Task ResizeUi(ref int xCoord, ref int yCoord)
    {
        float xRatio = (float) xCoord/(float)originalPanel1Size.Width;
        float yRatio = (float)yCoord/ (float)originalPanel1Size.Height;

            foreach(Control con in panel1.Controls)
            {
                int sizeW = (int)(con.Size.Width * xRatio);
                int sizeH = (int)(con.Size.Height * yRatio);
                int newX = (int)(con.Location.X * xRatio);
                int newY = (int)(con.Location.Y * yRatio);
                con.Size = new Size(sizeW, sizeH);
                con.Location = new Point(newX, newY);
            }
            //resize panel
            int panelW = (int)(panel1.Size.Width * xRatio);
            int panelH = (int)(panel1.Size.Height *yRatio);
             panel1.Location = new Point(0, xCoord);
            panel1.Size= new Size(panelW,panelH);
        return Task.CompletedTask;
    }
        private async void Start_BTN_Click(object sender, EventArgs e)
        {
            int[] info = new int[2];
           
            int waitB4 = int.Parse(waitB4_TB.Text);
            int waitAfter = int.Parse(waitAfter_TB.Text);
            Rectangle monitorBounds = Screen.AllScreens[0].WorkingArea;
            int screenshotsLeft;
            string filePath = "";
            if (NumbersValidated(waitB4, waitAfter, monitorBounds, filePath) == true)
            {
                DialogResult result = MessageBox.Show("If you press yes, this program will automatically " +
                    "click once on nifskope and then click the load view button in nifskope for every screenshot. " +
                    "Is this acceptable?", "Important question", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    for (int i = 0; i< totalScreenshots; i++)
                    {
                        Task<long> space = GetSpaceAvailability();
                        long spaceAvailable = await space;
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
                                    Task<int> screenshot = ScreenshotMethod(ref theSize, ref info, ref startTime);
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
                        Task<long> space = GetSpaceAvailability();
                        long spaceAvailable = await space;
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
                                    Task<int> screenshot = ScreenshotMethod(ref theSize, ref info, ref startTime);
                                    completedScreenshots = await screenshot;
                                    DateTime screenshotDone = DateTime.Now;
                                    Thread.Sleep(waitAfter);
                                    screenshotTime = (screenshotDone - startTime);
                                    int screenshotsRemaining = CalculateRemainingScreenshots();
                                    screenshotsLeft = screenshotsRemaining;
                                    int time= EstimateTimeRemaining(ref screenshotsRemaining);
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
        Task<long> GetSpaceAvailability()
        {
            string driveRoot = Path.GetPathRoot(savePath);
            DriveInfo driveInfo = new DriveInfo(driveRoot);
            long spaceLeft = driveInfo.AvailableFreeSpace;
            return Task.FromResult(spaceLeft);
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
        private Task<int> ScreenshotMethod(ref Size theSize, ref int[] info, ref DateTime startTime)
        {
            try
            {
                Point coords = new Point(info[0], info[1]);
                Point dest = new Point(0, 0);
                
  
                Bitmap nif = new Bitmap(ssWidth, ssHeight);   
                Graphics graphicsNif = Graphics.FromImage(nif);
                graphicsNif.CopyFromScreen(coords, dest, theSize);
                string TreeNodeNameRevised = fileName_LB.Items[completedScreenshots].ToString().Replace(".nif", string.Empty);
                nif.Save(savePath + "\\" + TreeNodeNameRevised + ".bmp", ImageFormat.Bmp);
                imageList1.Images.Add(nif);
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
            if(isPaused)
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
            int[] info = new int[2];
            Task<Size> calcThings = CalculateStuff(info);
            Size theSize = await calcThings;
            int waitB4 = 0;
            int waitAfter = 0;
            DateTime startCalcTime;
            DateTime endCalcTime;
            string filePath = "";
            Rectangle monitorBounds = Screen.AllScreens[0].WorkingArea;
            //MAKE ABSOLUTELY SURE TO MAKE THE VALUE OF WAITB4 to wT!!
            if (NumbersValidated(waitB4, waitAfter, monitorBounds, filePath) == true)
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
        private Task AutomaticButtonPressForCalculation()
        {
            try
            {
                MouseEventArgs args2 = new MouseEventArgs(MouseButtons.Left, 1, 810, 533, 0);
                OnMouseDown(args2);
                OnMouseUp(args2);
                SendKeys.SendWait("{F8}");

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
                screenshot_PB.Image  = img; 
                
            }
            catch(Exception f)
            {
                MessageBox.Show($"Data: {f.Data}\n HResult: + {f.HResult} \n helplink: {f.HelpLink}\n +" +
                    $" InnerException: {f.InnerException}\n + Message: {f.Message}\n +Source: " +
                    $"{f.Source}\n + StackTrace: {f.StackTrace}\n + TargetSite: {f.TargetSite}", "Error", MessageBoxButtons.OK);
            }
        }
    }
}
