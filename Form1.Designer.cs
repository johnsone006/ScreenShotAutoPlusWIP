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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.waitB4_TB = new System.Windows.Forms.TextBox();
            this.waitAfter_TB = new System.Windows.Forms.TextBox();
            this.fileName_LB = new System.Windows.Forms.ListBox();
            this.filePath_LB = new System.Windows.Forms.ListBox();
            this.import_BTN = new System.Windows.Forms.Button();
            this.waitAfter_LBL = new System.Windows.Forms.Label();
            this.Start_BTN = new System.Windows.Forms.Button();
            this.pauseAndPlay_BTN = new System.Windows.Forms.Button();
            this.processStatus_LBL = new System.Windows.Forms.Label();
            this.screenshotNumber_LBL = new System.Windows.Forms.Label();
            this.waitB4_LBL = new System.Windows.Forms.Label();
            this.timeLeft_LBL = new System.Windows.Forms.Label();
            this.stateTxt = new System.Windows.Forms.TextBox();
            this.CalculateTimeBTN = new System.Windows.Forms.Button();
            this.screenshotFiles_LB = new System.Windows.Forms.ListBox();
            this.screenshot_PB = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.screenshot_PB)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // waitB4_TB
            // 
            this.waitB4_TB.Location = new System.Drawing.Point(49, 61);
            this.waitB4_TB.Margin = new System.Windows.Forms.Padding(4);
            this.waitB4_TB.Name = "waitB4_TB";
            this.waitB4_TB.Size = new System.Drawing.Size(124, 26);
            this.waitB4_TB.TabIndex = 14;
            // 
            // waitAfter_TB
            // 
            this.waitAfter_TB.Location = new System.Drawing.Point(49, 141);
            this.waitAfter_TB.Margin = new System.Windows.Forms.Padding(4);
            this.waitAfter_TB.Name = "waitAfter_TB";
            this.waitAfter_TB.Size = new System.Drawing.Size(124, 26);
            this.waitAfter_TB.TabIndex = 15;
            // 
            // fileName_LB
            // 
            this.fileName_LB.FormattingEnabled = true;
            this.fileName_LB.ItemHeight = 20;
            this.fileName_LB.Location = new System.Drawing.Point(9, 249);
            this.fileName_LB.Margin = new System.Windows.Forms.Padding(4);
            this.fileName_LB.Name = "fileName_LB";
            this.fileName_LB.Size = new System.Drawing.Size(219, 104);
            this.fileName_LB.TabIndex = 16;
            // 
            // filePath_LB
            // 
            this.filePath_LB.BackColor = System.Drawing.SystemColors.Window;
            this.filePath_LB.FormattingEnabled = true;
            this.filePath_LB.HorizontalScrollbar = true;
            this.filePath_LB.ItemHeight = 20;
            this.filePath_LB.Location = new System.Drawing.Point(9, 361);
            this.filePath_LB.Margin = new System.Windows.Forms.Padding(4);
            this.filePath_LB.Name = "filePath_LB";
            this.filePath_LB.Size = new System.Drawing.Size(215, 124);
            this.filePath_LB.TabIndex = 17;
            // 
            // import_BTN
            // 
            this.import_BTN.Location = new System.Drawing.Point(7, 175);
            this.import_BTN.Margin = new System.Windows.Forms.Padding(4);
            this.import_BTN.Name = "import_BTN";
            this.import_BTN.Size = new System.Drawing.Size(237, 29);
            this.import_BTN.TabIndex = 18;
            this.import_BTN.Text = "Import file names and paths";
            this.import_BTN.UseVisualStyleBackColor = true;
            this.import_BTN.Click += new System.EventHandler(this.Import_BTN_Click);
            // 
            // waitAfter_LBL
            // 
            this.waitAfter_LBL.Location = new System.Drawing.Point(11, 91);
            this.waitAfter_LBL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.waitAfter_LBL.Name = "waitAfter_LBL";
            this.waitAfter_LBL.Size = new System.Drawing.Size(279, 46);
            this.waitAfter_LBL.TabIndex = 7;
            this.waitAfter_LBL.Text = "Number of miliseconds you want to wait after each screenshot:";
            this.waitAfter_LBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Start_BTN
            // 
            this.Start_BTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Start_BTN.Location = new System.Drawing.Point(9, 527);
            this.Start_BTN.Margin = new System.Windows.Forms.Padding(4);
            this.Start_BTN.Name = "Start_BTN";
            this.Start_BTN.Size = new System.Drawing.Size(76, 36);
            this.Start_BTN.TabIndex = 0;
            this.Start_BTN.Text = "Start";
            this.Start_BTN.UseVisualStyleBackColor = true;
            this.Start_BTN.Click += new System.EventHandler(this.Start_BTN_Click);
            // 
            // pauseAndPlay_BTN
            // 
            this.pauseAndPlay_BTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pauseAndPlay_BTN.Location = new System.Drawing.Point(11, 571);
            this.pauseAndPlay_BTN.Margin = new System.Windows.Forms.Padding(4);
            this.pauseAndPlay_BTN.Name = "pauseAndPlay_BTN";
            this.pauseAndPlay_BTN.Size = new System.Drawing.Size(136, 35);
            this.pauseAndPlay_BTN.TabIndex = 1;
            this.pauseAndPlay_BTN.Text = "Restart/Pause ";
            this.pauseAndPlay_BTN.UseVisualStyleBackColor = true;
            this.pauseAndPlay_BTN.Click += new System.EventHandler(this.PauseAndPlay_BTN_Click);
            // 
            // processStatus_LBL
            // 
            this.processStatus_LBL.AutoSize = true;
            this.processStatus_LBL.Location = new System.Drawing.Point(24, 923);
            this.processStatus_LBL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.processStatus_LBL.Name = "processStatus_LBL";
            this.processStatus_LBL.Size = new System.Drawing.Size(77, 20);
            this.processStatus_LBL.TabIndex = 2;
            this.processStatus_LBL.Text = "Waiting...";
            // 
            // screenshotNumber_LBL
            // 
            this.screenshotNumber_LBL.AutoSize = true;
            this.screenshotNumber_LBL.Location = new System.Drawing.Point(11, 998);
            this.screenshotNumber_LBL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.screenshotNumber_LBL.Name = "screenshotNumber_LBL";
            this.screenshotNumber_LBL.Size = new System.Drawing.Size(189, 20);
            this.screenshotNumber_LBL.TabIndex = 3;
            this.screenshotNumber_LBL.Text = "Number of screenshots:";
            // 
            // waitB4_LBL
            // 
            this.waitB4_LBL.Location = new System.Drawing.Point(11, 9);
            this.waitB4_LBL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.waitB4_LBL.Name = "waitB4_LBL";
            this.waitB4_LBL.Size = new System.Drawing.Size(258, 48);
            this.waitB4_LBL.TabIndex = 6;
            this.waitB4_LBL.Text = "Number of miliseconds you want to wait prior to each screenshot:";
            this.waitB4_LBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timeLeft_LBL
            // 
            this.timeLeft_LBL.AutoSize = true;
            this.timeLeft_LBL.Location = new System.Drawing.Point(9, 958);
            this.timeLeft_LBL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.timeLeft_LBL.Name = "timeLeft_LBL";
            this.timeLeft_LBL.Size = new System.Drawing.Size(203, 20);
            this.timeLeft_LBL.TabIndex = 4;
            this.timeLeft_LBL.Text = "Time left (in miliseconds):";
            // 
            // stateTxt
            // 
            this.stateTxt.Location = new System.Drawing.Point(11, 493);
            this.stateTxt.Margin = new System.Windows.Forms.Padding(4);
            this.stateTxt.Name = "stateTxt";
            this.stateTxt.Size = new System.Drawing.Size(124, 26);
            this.stateTxt.TabIndex = 21;
            // 
            // CalculateTimeBTN
            // 
            this.CalculateTimeBTN.Location = new System.Drawing.Point(10, 212);
            this.CalculateTimeBTN.Margin = new System.Windows.Forms.Padding(4);
            this.CalculateTimeBTN.Name = "CalculateTimeBTN";
            this.CalculateTimeBTN.Size = new System.Drawing.Size(219, 29);
            this.CalculateTimeBTN.TabIndex = 22;
            this.CalculateTimeBTN.Text = "CalculateTime";
            this.CalculateTimeBTN.UseVisualStyleBackColor = true;
            this.CalculateTimeBTN.Click += new System.EventHandler(this.CalculateTimeBTN_Click);
            // 
            // screenshotFiles_LB
            // 
            this.screenshotFiles_LB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.screenshotFiles_LB.FormattingEnabled = true;
            this.screenshotFiles_LB.HorizontalScrollbar = true;
            this.screenshotFiles_LB.ItemHeight = 17;
            this.screenshotFiles_LB.Location = new System.Drawing.Point(13, 614);
            this.screenshotFiles_LB.Margin = new System.Windows.Forms.Padding(4);
            this.screenshotFiles_LB.Name = "screenshotFiles_LB";
            this.screenshotFiles_LB.Size = new System.Drawing.Size(475, 89);
            this.screenshotFiles_LB.TabIndex = 23;
            this.screenshotFiles_LB.MouseClick += new System.Windows.Forms.MouseEventHandler(this.screenshotFiles_LB_MouseClick);
            // 
            // screenshot_PB
            // 
            this.screenshot_PB.Location = new System.Drawing.Point(15, 711);
            this.screenshot_PB.Margin = new System.Windows.Forms.Padding(4);
            this.screenshot_PB.Name = "screenshot_PB";
            this.screenshot_PB.Size = new System.Drawing.Size(218, 195);
            this.screenshot_PB.TabIndex = 24;
            this.screenshot_PB.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.screenshot_PB);
            this.panel1.Controls.Add(this.screenshotFiles_LB);
            this.panel1.Controls.Add(this.CalculateTimeBTN);
            this.panel1.Controls.Add(this.stateTxt);
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
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(540, 1055);
            this.panel1.TabIndex = 21;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1924, 1055);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
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
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox waitB4_TB;
        private System.Windows.Forms.TextBox waitAfter_TB;
        private System.Windows.Forms.ListBox fileName_LB;
        private System.Windows.Forms.ListBox filePath_LB;
        private System.Windows.Forms.Button import_BTN;
        private System.Windows.Forms.Label waitAfter_LBL;
        private System.Windows.Forms.Button Start_BTN;
        private System.Windows.Forms.Button pauseAndPlay_BTN;
        private System.Windows.Forms.Label processStatus_LBL;
        private System.Windows.Forms.Label screenshotNumber_LBL;
        private System.Windows.Forms.Label waitB4_LBL;
        private System.Windows.Forms.Label timeLeft_LBL;
        private System.Windows.Forms.TextBox stateTxt;
        private System.Windows.Forms.Button CalculateTimeBTN;
        private System.Windows.Forms.ListBox screenshotFiles_LB;
        private System.Windows.Forms.PictureBox screenshot_PB;
        private System.Windows.Forms.Panel panel1;
    }
}

