namespace ScreenShotAutoPlus
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.topPanel = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.waitB4_TB = new System.Windows.Forms.TextBox();
            this.waitAfter_TB = new System.Windows.Forms.TextBox();
            this.colorCode_TB = new System.Windows.Forms.TextBox();
            this.import_BTN = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.fileName_LB = new System.Windows.Forms.ListBox();
            this.filePath_LB = new System.Windows.Forms.ListBox();
            this.Start_BTN = new System.Windows.Forms.Button();
            this.pauseAndPlay_BTN = new System.Windows.Forms.Button();
            this.processStatus_LBL = new System.Windows.Forms.Label();
            this.screenshotNumber_LBL = new System.Windows.Forms.Label();
            this.timeLeft_LBL = new System.Windows.Forms.Label();
            this.CalculateTimeBTN = new System.Windows.Forms.Button();
            this.screenshotFiles_LB = new System.Windows.Forms.ListBox();
            this.screenshot_PB = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.waitB4_LBL = new System.Windows.Forms.Label();
            this.waitAfter_LBL = new System.Windows.Forms.Label();
            this.Exit_BTN = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.screenshot_PB)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1980, 91);
            this.topPanel.TabIndex = 22;
            // 
            // waitB4_TB
            // 
            this.waitB4_TB.AccessibleDescription = "Type in the number of miliseconds you want the program to wait prior to each scre" +
    "enshot here.";
            this.waitB4_TB.AccessibleName = "Milisecond wait before each screenshot text box.";
            this.waitB4_TB.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.waitB4_TB.Location = new System.Drawing.Point(57, 62);
            this.waitB4_TB.Name = "waitB4_TB";
            this.waitB4_TB.Size = new System.Drawing.Size(100, 22);
            this.waitB4_TB.TabIndex = 1;
            this.toolTip1.SetToolTip(this.waitB4_TB, "Type the number of miliseconds you want the program to wait prior to taking each " +
        "screenshot");
            this.waitB4_TB.WordWrap = false;
            // 
            // waitAfter_TB
            // 
            this.waitAfter_TB.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.waitAfter_TB.Location = new System.Drawing.Point(57, 138);
            this.waitAfter_TB.Name = "waitAfter_TB";
            this.waitAfter_TB.Size = new System.Drawing.Size(100, 22);
            this.waitAfter_TB.TabIndex = 2;
            this.toolTip1.SetToolTip(this.waitAfter_TB, "Type the number of miliseconds you want the program to wait after taking each scr" +
        "eenshot");
            // 
            // colorCode_TB
            // 
            this.colorCode_TB.AccessibleDescription = "Type in the hex color code for the viewport of nifskope here.";
            this.colorCode_TB.AccessibleName = "Color code text box";
            this.colorCode_TB.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.colorCode_TB.Location = new System.Drawing.Point(57, 226);
            this.colorCode_TB.Name = "colorCode_TB";
            this.colorCode_TB.Size = new System.Drawing.Size(100, 22);
            this.colorCode_TB.TabIndex = 3;
            this.toolTip1.SetToolTip(this.colorCode_TB, "Type  in hexadecimal color code for the background of the viewport in Nifskope he" +
        "re");
            // 
            // import_BTN
            // 
            this.import_BTN.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.import_BTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.import_BTN.Location = new System.Drawing.Point(22, 267);
            this.import_BTN.Name = "import_BTN";
            this.import_BTN.Size = new System.Drawing.Size(186, 23);
            this.import_BTN.TabIndex = 4;
            this.import_BTN.Text = "Import file names and paths";
            this.toolTip1.SetToolTip(this.import_BTN, "This is the button you press to select the file folder that contains the files yo" +
        "u want to screenshot.");
            this.import_BTN.UseVisualStyleBackColor = true;
            this.import_BTN.Click += new System.EventHandler(this.Import_BTN_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // fileName_LB
            // 
            this.fileName_LB.AccessibleDescription = "This is the list box that will contain the list of file names of the files to be " +
    "screenshotted.";
            this.fileName_LB.AccessibleName = "List of file names";
            this.fileName_LB.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
            this.fileName_LB.FormattingEnabled = true;
            this.fileName_LB.ItemHeight = 16;
            this.fileName_LB.Location = new System.Drawing.Point(18, 364);
            this.fileName_LB.Name = "fileName_LB";
            this.fileName_LB.Size = new System.Drawing.Size(190, 52);
            this.fileName_LB.TabIndex = 16;
            // 
            // filePath_LB
            // 
            this.filePath_LB.AccessibleDescription = "This is the list box that will contain all the file paths of files to be screensh" +
    "otted.";
            this.filePath_LB.AccessibleName = "List of file paths";
            this.filePath_LB.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
            this.filePath_LB.BackColor = System.Drawing.SystemColors.Window;
            this.filePath_LB.FormattingEnabled = true;
            this.filePath_LB.HorizontalScrollbar = true;
            this.filePath_LB.ItemHeight = 16;
            this.filePath_LB.Location = new System.Drawing.Point(18, 308);
            this.filePath_LB.Name = "filePath_LB";
            this.filePath_LB.Size = new System.Drawing.Size(190, 36);
            this.filePath_LB.TabIndex = 17;
            // 
            // Start_BTN
            // 
            this.Start_BTN.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Start_BTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Start_BTN.Location = new System.Drawing.Point(72, 451);
            this.Start_BTN.Name = "Start_BTN";
            this.Start_BTN.Size = new System.Drawing.Size(61, 29);
            this.Start_BTN.TabIndex = 0;
            this.Start_BTN.Text = "Start";
            this.Start_BTN.UseVisualStyleBackColor = true;
            this.Start_BTN.Click += new System.EventHandler(this.Start_BTN_Click);
            // 
            // pauseAndPlay_BTN
            // 
            this.pauseAndPlay_BTN.AccessibleDescription = "Push this button if you want the screenshot process to stop temporarily if it is " +
    "still going, or to continue if it is currently paused.";
            this.pauseAndPlay_BTN.AccessibleName = "Pause and continue button";
            this.pauseAndPlay_BTN.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.pauseAndPlay_BTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pauseAndPlay_BTN.Location = new System.Drawing.Point(48, 486);
            this.pauseAndPlay_BTN.Name = "pauseAndPlay_BTN";
            this.pauseAndPlay_BTN.Size = new System.Drawing.Size(109, 28);
            this.pauseAndPlay_BTN.TabIndex = 1;
            this.pauseAndPlay_BTN.Text = "Restart/Pause ";
            this.pauseAndPlay_BTN.UseVisualStyleBackColor = true;
            this.pauseAndPlay_BTN.Click += new System.EventHandler(this.PauseAndPlay_BTN_Click);
            // 
            // processStatus_LBL
            // 
            this.processStatus_LBL.AutoSize = true;
            this.processStatus_LBL.Location = new System.Drawing.Point(69, 789);
            this.processStatus_LBL.Name = "processStatus_LBL";
            this.processStatus_LBL.Size = new System.Drawing.Size(61, 16);
            this.processStatus_LBL.TabIndex = 2;
            this.processStatus_LBL.Text = "Waiting...";
            // 
            // screenshotNumber_LBL
            // 
            this.screenshotNumber_LBL.AutoSize = true;
            this.screenshotNumber_LBL.Location = new System.Drawing.Point(19, 843);
            this.screenshotNumber_LBL.Name = "screenshotNumber_LBL";
            this.screenshotNumber_LBL.Size = new System.Drawing.Size(148, 16);
            this.screenshotNumber_LBL.TabIndex = 3;
            this.screenshotNumber_LBL.Text = "Number of screenshots:";
            // 
            // timeLeft_LBL
            // 
            this.timeLeft_LBL.AutoSize = true;
            this.timeLeft_LBL.Location = new System.Drawing.Point(19, 816);
            this.timeLeft_LBL.Name = "timeLeft_LBL";
            this.timeLeft_LBL.Size = new System.Drawing.Size(157, 16);
            this.timeLeft_LBL.TabIndex = 4;
            this.timeLeft_LBL.Text = "Time left (in miliseconds):";
            // 
            // CalculateTimeBTN
            // 
            this.CalculateTimeBTN.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.CalculateTimeBTN.Location = new System.Drawing.Point(40, 422);
            this.CalculateTimeBTN.Name = "CalculateTimeBTN";
            this.CalculateTimeBTN.Size = new System.Drawing.Size(136, 23);
            this.CalculateTimeBTN.TabIndex = 22;
            this.CalculateTimeBTN.Text = "Calculation Time";
            this.CalculateTimeBTN.UseVisualStyleBackColor = true;
            this.CalculateTimeBTN.Click += new System.EventHandler(this.CalculateTimeBTN_Click);
            // 
            // screenshotFiles_LB
            // 
            this.screenshotFiles_LB.AccessibleDescription = "Click an item on this list to populate the picturebox with a preview of that scre" +
    "enshot.";
            this.screenshotFiles_LB.AccessibleName = "Screenshotted file names list box";
            this.screenshotFiles_LB.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
            this.screenshotFiles_LB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.screenshotFiles_LB.FormattingEnabled = true;
            this.screenshotFiles_LB.HorizontalScrollbar = true;
            this.screenshotFiles_LB.Location = new System.Drawing.Point(18, 529);
            this.screenshotFiles_LB.Name = "screenshotFiles_LB";
            this.screenshotFiles_LB.Size = new System.Drawing.Size(178, 69);
            this.screenshotFiles_LB.TabIndex = 23;
            this.screenshotFiles_LB.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ScreenshotFiles_LB_MouseClick);
            // 
            // screenshot_PB
            // 
            this.screenshot_PB.AccessibleDescription = "This is a picture box that shows the screenshot of a file that you click in scree" +
    "nshotFiles_LB";
            this.screenshot_PB.AccessibleName = "Screenshot that was taken";
            this.screenshot_PB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.screenshot_PB.Location = new System.Drawing.Point(18, 618);
            this.screenshot_PB.Name = "screenshot_PB";
            this.screenshot_PB.Size = new System.Drawing.Size(174, 156);
            this.screenshot_PB.TabIndex = 24;
            this.screenshot_PB.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.Exit_BTN);
            this.panel1.Controls.Add(this.colorCode_TB);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.screenshot_PB);
            this.panel1.Controls.Add(this.screenshotFiles_LB);
            this.panel1.Controls.Add(this.CalculateTimeBTN);
            this.panel1.Controls.Add(this.timeLeft_LBL);
            this.panel1.Controls.Add(this.waitB4_LBL);
            this.panel1.Controls.Add(this.screenshotNumber_LBL);
            this.panel1.Controls.Add(this.processStatus_LBL);
            this.panel1.Controls.Add(this.pauseAndPlay_BTN);
            this.panel1.Controls.Add(this.Start_BTN);
            this.panel1.Controls.Add(this.waitAfter_LBL);
            this.panel1.Controls.Add(this.import_BTN);
            this.panel1.Controls.Add(this.filePath_LB);
            this.panel1.Controls.Add(this.fileName_LB);
            this.panel1.Controls.Add(this.waitAfter_TB);
            this.panel1.Controls.Add(this.waitB4_TB);
            this.panel1.Location = new System.Drawing.Point(0, 91);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(236, 949);
            this.panel1.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(19, 176);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 36);
            this.label1.TabIndex = 25;
            this.label1.Text = "Hex color code of background 4 viewport of nifskope:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // waitB4_LBL
            // 
            this.waitB4_LBL.AutoEllipsis = true;
            this.waitB4_LBL.Location = new System.Drawing.Point(3, 3);
            this.waitB4_LBL.Name = "waitB4_LBL";
            this.waitB4_LBL.Size = new System.Drawing.Size(221, 56);
            this.waitB4_LBL.TabIndex = 6;
            this.waitB4_LBL.Text = "Number of miliseconds for program to wait prior to each screenshot";
            this.waitB4_LBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // waitAfter_LBL
            // 
            this.waitAfter_LBL.Location = new System.Drawing.Point(3, 87);
            this.waitAfter_LBL.Name = "waitAfter_LBL";
            this.waitAfter_LBL.Size = new System.Drawing.Size(213, 48);
            this.waitAfter_LBL.TabIndex = 7;
            this.waitAfter_LBL.Text = "Number of miliseconds for program to wait  after each screenshot";
            this.waitAfter_LBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Exit_BTN
            // 
            this.Exit_BTN.Location = new System.Drawing.Point(18, 880);
            this.Exit_BTN.Name = "Exit_BTN";
            this.Exit_BTN.Size = new System.Drawing.Size(139, 23);
            this.Exit_BTN.TabIndex = 26;
            this.Exit_BTN.Text = "Exit";
            this.Exit_BTN.UseVisualStyleBackColor = true;
            this.Exit_BTN.Click += new System.EventHandler(this.Exit_BTN_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1980, 1040);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.topPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.Color.Silver;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.screenshot_PB)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox waitB4_TB;
        private System.Windows.Forms.TextBox waitAfter_TB;
        private System.Windows.Forms.ListBox fileName_LB;
        private System.Windows.Forms.ListBox filePath_LB;
        private System.Windows.Forms.Button import_BTN;
        private System.Windows.Forms.Button Start_BTN;
        private System.Windows.Forms.Button pauseAndPlay_BTN;
        private System.Windows.Forms.Label processStatus_LBL;
        private System.Windows.Forms.Label screenshotNumber_LBL;
        private System.Windows.Forms.Label timeLeft_LBL;
        private System.Windows.Forms.Button CalculateTimeBTN;
        private System.Windows.Forms.ListBox screenshotFiles_LB;
        private System.Windows.Forms.PictureBox screenshot_PB;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox colorCode_TB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label waitB4_LBL;
        private System.Windows.Forms.Label waitAfter_LBL;
        private System.Windows.Forms.Button Exit_BTN;
    }
}

