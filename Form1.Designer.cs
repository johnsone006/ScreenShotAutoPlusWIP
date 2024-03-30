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
            this.timeLeft_LBL = new System.Windows.Forms.Label();
            this.screenshotNumber_LBL = new System.Windows.Forms.Label();
            this.processStatus_LBL = new System.Windows.Forms.Label();
            this.pauseAndPlay_BTN = new System.Windows.Forms.Button();
            this.Start_BTN = new System.Windows.Forms.Button();
            this.import_BTN = new System.Windows.Forms.Button();
            this.filePath_LB = new System.Windows.Forms.ListBox();
            this.fileName_LB = new System.Windows.Forms.ListBox();
            this.waitAfter_TB = new System.Windows.Forms.TextBox();
            this.waitB4_TB = new System.Windows.Forms.TextBox();
            this.yCoord_TB = new System.Windows.Forms.TextBox();
            this.xCoord_TB = new System.Windows.Forms.TextBox();
            this.screenshotHeight_TB = new System.Windows.Forms.TextBox();
            this.screenshotWidth_TB = new System.Windows.Forms.TextBox();
            this.saveFilePath_TB = new System.Windows.Forms.TextBox();
            this.filePath_TB = new System.Windows.Forms.TextBox();
            this.waitAfter_LBL = new System.Windows.Forms.Label();
            this.waitB4_LBL = new System.Windows.Forms.Label();
            this.yCoord_LBL = new System.Windows.Forms.Label();
            this.xCoord_LBL = new System.Windows.Forms.Label();
            this.heightScreenshot_LBL = new System.Windows.Forms.Label();
            this.ScreenshotWidth_LBL = new System.Windows.Forms.Label();
            this.saveFilePath_LBL = new System.Windows.Forms.Label();
            this.filePath_LBL = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.stateTxt = new System.Windows.Forms.TextBox();
            this.hiddenTB = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.CalculateTime_BTN = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timeLeft_LBL
            // 
            this.timeLeft_LBL.AutoSize = true;
            this.timeLeft_LBL.Location = new System.Drawing.Point(164, 983);
            this.timeLeft_LBL.Name = "timeLeft_LBL";
            this.timeLeft_LBL.Size = new System.Drawing.Size(157, 16);
            this.timeLeft_LBL.TabIndex = 4;
            this.timeLeft_LBL.Text = "Time left (in miliseconds):";
            // 
            // screenshotNumber_LBL
            // 
            this.screenshotNumber_LBL.AutoSize = true;
            this.screenshotNumber_LBL.Location = new System.Drawing.Point(164, 1015);
            this.screenshotNumber_LBL.Name = "screenshotNumber_LBL";
            this.screenshotNumber_LBL.Size = new System.Drawing.Size(148, 16);
            this.screenshotNumber_LBL.TabIndex = 3;
            this.screenshotNumber_LBL.Text = "Number of screenshots:";
            // 
            // processStatus_LBL
            // 
            this.processStatus_LBL.AutoSize = true;
            this.processStatus_LBL.Location = new System.Drawing.Point(28, 995);
            this.processStatus_LBL.Name = "processStatus_LBL";
            this.processStatus_LBL.Size = new System.Drawing.Size(61, 16);
            this.processStatus_LBL.TabIndex = 2;
            this.processStatus_LBL.Text = "Waiting...";
            // 
            // pauseAndPlay_BTN
            // 
            this.pauseAndPlay_BTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pauseAndPlay_BTN.Location = new System.Drawing.Point(98, 945);
            this.pauseAndPlay_BTN.Name = "pauseAndPlay_BTN";
            this.pauseAndPlay_BTN.Size = new System.Drawing.Size(109, 28);
            this.pauseAndPlay_BTN.TabIndex = 1;
            this.pauseAndPlay_BTN.Text = "Restart/Pause ";
            this.pauseAndPlay_BTN.UseVisualStyleBackColor = true;
            this.pauseAndPlay_BTN.Click += new System.EventHandler(this.PauseAndPlay_BTN_Click);
            // 
            // Start_BTN
            // 
            this.Start_BTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Start_BTN.Location = new System.Drawing.Point(12, 944);
            this.Start_BTN.Name = "Start_BTN";
            this.Start_BTN.Size = new System.Drawing.Size(61, 29);
            this.Start_BTN.TabIndex = 0;
            this.Start_BTN.Text = "Start";
            this.Start_BTN.UseVisualStyleBackColor = true;
            this.Start_BTN.Click += new System.EventHandler(this.Start_BTN_Click);
            // 
            // import_BTN
            // 
            this.import_BTN.Location = new System.Drawing.Point(6, 434);
            this.import_BTN.Name = "import_BTN";
            this.import_BTN.Size = new System.Drawing.Size(262, 23);
            this.import_BTN.TabIndex = 18;
            this.import_BTN.Text = "Import file names and paths";
            this.import_BTN.UseVisualStyleBackColor = true;
            this.import_BTN.Click += new System.EventHandler(this.Import_BTN_Click);
            // 
            // filePath_LB
            // 
            this.filePath_LB.FormattingEnabled = true;
            this.filePath_LB.HorizontalScrollbar = true;
            this.filePath_LB.ItemHeight = 16;
            this.filePath_LB.Location = new System.Drawing.Point(10, 489);
            this.filePath_LB.Name = "filePath_LB";
            this.filePath_LB.Size = new System.Drawing.Size(173, 420);
            this.filePath_LB.TabIndex = 17;
            // 
            // fileName_LB
            // 
            this.fileName_LB.FormattingEnabled = true;
            this.fileName_LB.ItemHeight = 16;
            this.fileName_LB.Location = new System.Drawing.Point(189, 489);
            this.fileName_LB.Name = "fileName_LB";
            this.fileName_LB.Size = new System.Drawing.Size(202, 420);
            this.fileName_LB.TabIndex = 16;
            // 
            // waitAfter_TB
            // 
            this.waitAfter_TB.Location = new System.Drawing.Point(31, 406);
            this.waitAfter_TB.Name = "waitAfter_TB";
            this.waitAfter_TB.Size = new System.Drawing.Size(100, 22);
            this.waitAfter_TB.TabIndex = 15;
            // 
            // waitB4_TB
            // 
            this.waitB4_TB.Location = new System.Drawing.Point(31, 362);
            this.waitB4_TB.Name = "waitB4_TB";
            this.waitB4_TB.Size = new System.Drawing.Size(100, 22);
            this.waitB4_TB.TabIndex = 14;
            // 
            // yCoord_TB
            // 
            this.yCoord_TB.Location = new System.Drawing.Point(31, 318);
            this.yCoord_TB.Name = "yCoord_TB";
            this.yCoord_TB.Size = new System.Drawing.Size(100, 22);
            this.yCoord_TB.TabIndex = 13;
            // 
            // xCoord_TB
            // 
            this.xCoord_TB.Location = new System.Drawing.Point(31, 254);
            this.xCoord_TB.Name = "xCoord_TB";
            this.xCoord_TB.Size = new System.Drawing.Size(100, 22);
            this.xCoord_TB.TabIndex = 12;
            // 
            // screenshotHeight_TB
            // 
            this.screenshotHeight_TB.Location = new System.Drawing.Point(31, 195);
            this.screenshotHeight_TB.Name = "screenshotHeight_TB";
            this.screenshotHeight_TB.Size = new System.Drawing.Size(100, 22);
            this.screenshotHeight_TB.TabIndex = 11;
            // 
            // screenshotWidth_TB
            // 
            this.screenshotWidth_TB.Location = new System.Drawing.Point(31, 140);
            this.screenshotWidth_TB.Name = "screenshotWidth_TB";
            this.screenshotWidth_TB.Size = new System.Drawing.Size(100, 22);
            this.screenshotWidth_TB.TabIndex = 10;
            // 
            // saveFilePath_TB
            // 
            this.saveFilePath_TB.Location = new System.Drawing.Point(31, 89);
            this.saveFilePath_TB.Name = "saveFilePath_TB";
            this.saveFilePath_TB.Size = new System.Drawing.Size(100, 22);
            this.saveFilePath_TB.TabIndex = 9;
            // 
            // filePath_TB
            // 
            this.filePath_TB.Location = new System.Drawing.Point(31, 45);
            this.filePath_TB.Name = "filePath_TB";
            this.filePath_TB.Size = new System.Drawing.Size(100, 22);
            this.filePath_TB.TabIndex = 8;
            // 
            // waitAfter_LBL
            // 
            this.waitAfter_LBL.AutoSize = true;
            this.waitAfter_LBL.Location = new System.Drawing.Point(9, 387);
            this.waitAfter_LBL.Name = "waitAfter_LBL";
            this.waitAfter_LBL.Size = new System.Drawing.Size(373, 16);
            this.waitAfter_LBL.TabIndex = 7;
            this.waitAfter_LBL.Text = "Number of miliseconds you want to wait after each screenshot:";
            this.waitAfter_LBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // waitB4_LBL
            // 
            this.waitB4_LBL.AutoSize = true;
            this.waitB4_LBL.Location = new System.Drawing.Point(7, 343);
            this.waitB4_LBL.Name = "waitB4_LBL";
            this.waitB4_LBL.Size = new System.Drawing.Size(388, 16);
            this.waitB4_LBL.TabIndex = 6;
            this.waitB4_LBL.Text = "Number of miliseconds you want to wait prior to each screenshot:";
            this.waitB4_LBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // yCoord_LBL
            // 
            this.yCoord_LBL.AutoSize = true;
            this.yCoord_LBL.Location = new System.Drawing.Point(28, 290);
            this.yCoord_LBL.Name = "yCoord_LBL";
            this.yCoord_LBL.Size = new System.Drawing.Size(319, 16);
            this.yCoord_LBL.TabIndex = 5;
            this.yCoord_LBL.Text = "Y coordinate for upper left hand corner of screenshot:";
            this.yCoord_LBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // xCoord_LBL
            // 
            this.xCoord_LBL.Location = new System.Drawing.Point(3, 228);
            this.xCoord_LBL.Name = "xCoord_LBL";
            this.xCoord_LBL.Size = new System.Drawing.Size(379, 23);
            this.xCoord_LBL.TabIndex = 4;
            this.xCoord_LBL.Text = "X coordinate for upper left hand corner of screenshot:";
            this.xCoord_LBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // heightScreenshot_LBL
            // 
            this.heightScreenshot_LBL.AutoSize = true;
            this.heightScreenshot_LBL.Location = new System.Drawing.Point(23, 165);
            this.heightScreenshot_LBL.Name = "heightScreenshot_LBL";
            this.heightScreenshot_LBL.Size = new System.Drawing.Size(132, 16);
            this.heightScreenshot_LBL.TabIndex = 3;
            this.heightScreenshot_LBL.Text = "Height of screenshot:";
            this.heightScreenshot_LBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ScreenshotWidth_LBL
            // 
            this.ScreenshotWidth_LBL.AutoSize = true;
            this.ScreenshotWidth_LBL.Location = new System.Drawing.Point(28, 119);
            this.ScreenshotWidth_LBL.Name = "ScreenshotWidth_LBL";
            this.ScreenshotWidth_LBL.Size = new System.Drawing.Size(127, 16);
            this.ScreenshotWidth_LBL.TabIndex = 2;
            this.ScreenshotWidth_LBL.Text = "Width of screenshot:";
            // 
            // saveFilePath_LBL
            // 
            this.saveFilePath_LBL.AutoSize = true;
            this.saveFilePath_LBL.Location = new System.Drawing.Point(7, 70);
            this.saveFilePath_LBL.Name = "saveFilePath_LBL";
            this.saveFilePath_LBL.Size = new System.Drawing.Size(176, 16);
            this.saveFilePath_LBL.TabIndex = 1;
            this.saveFilePath_LBL.Text = "File path of files to be saved:";
            // 
            // filePath_LBL
            // 
            this.filePath_LBL.AutoSize = true;
            this.filePath_LBL.Location = new System.Drawing.Point(23, 19);
            this.filePath_LBL.Name = "filePath_LBL";
            this.filePath_LBL.Size = new System.Drawing.Size(220, 16);
            this.filePath_LBL.TabIndex = 2;
            this.filePath_LBL.Text = "Filepath of files to be screenshotted:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.CalculateTime_BTN);
            this.panel1.Controls.Add(this.stateTxt);
            this.panel1.Controls.Add(this.filePath_TB);
            this.panel1.Controls.Add(this.timeLeft_LBL);
            this.panel1.Controls.Add(this.waitB4_LBL);
            this.panel1.Controls.Add(this.screenshotNumber_LBL);
            this.panel1.Controls.Add(this.yCoord_LBL);
            this.panel1.Controls.Add(this.processStatus_LBL);
            this.panel1.Controls.Add(this.hiddenTB);
            this.panel1.Controls.Add(this.pauseAndPlay_BTN);
            this.panel1.Controls.Add(this.Start_BTN);
            this.panel1.Controls.Add(this.waitAfter_LBL);
            this.panel1.Controls.Add(this.xCoord_LBL);
            this.panel1.Controls.Add(this.import_BTN);
            this.panel1.Controls.Add(this.heightScreenshot_LBL);
            this.panel1.Controls.Add(this.filePath_LBL);
            this.panel1.Controls.Add(this.saveFilePath_TB);
            this.panel1.Controls.Add(this.filePath_LB);
            this.panel1.Controls.Add(this.ScreenshotWidth_LBL);
            this.panel1.Controls.Add(this.fileName_LB);
            this.panel1.Controls.Add(this.screenshotWidth_TB);
            this.panel1.Controls.Add(this.saveFilePath_LBL);
            this.panel1.Controls.Add(this.waitAfter_TB);
            this.panel1.Controls.Add(this.screenshotHeight_TB);
            this.panel1.Controls.Add(this.xCoord_TB);
            this.panel1.Controls.Add(this.waitB4_TB);
            this.panel1.Controls.Add(this.yCoord_TB);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(406, 1040);
            this.panel1.TabIndex = 21;
            // 
            // stateTxt
            // 
            this.stateTxt.Location = new System.Drawing.Point(212, 113);
            this.stateTxt.Name = "stateTxt";
            this.stateTxt.Size = new System.Drawing.Size(100, 22);
            this.stateTxt.TabIndex = 21;
            // 
            // hiddenTB
            // 
            this.hiddenTB.Location = new System.Drawing.Point(270, 16);
            this.hiddenTB.Name = "hiddenTB";
            this.hiddenTB.Size = new System.Drawing.Size(100, 22);
            this.hiddenTB.TabIndex = 19;
            this.hiddenTB.Visible = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Location = new System.Drawing.Point(406, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1573, 93);
            this.panel2.TabIndex = 22;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Silver;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(1467, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(45, 21);
            this.button2.TabIndex = 24;
            this.button2.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Silver;
            this.button1.Location = new System.Drawing.Point(184, 49);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(43, 43);
            this.button1.TabIndex = 23;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // CalculateTime_BTN
            // 
            this.CalculateTime_BTN.Location = new System.Drawing.Point(14, 915);
            this.CalculateTime_BTN.Name = "CalculateTime_BTN";
            this.CalculateTime_BTN.Size = new System.Drawing.Size(181, 23);
            this.CalculateTime_BTN.TabIndex = 22;
            this.CalculateTime_BTN.Text = "CalculateTimes";
            this.CalculateTime_BTN.UseVisualStyleBackColor = true;
            this.CalculateTime_BTN.Click += new System.EventHandler(this.CalculateTime_BTN_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1980, 1040);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.Color.Silver;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button pauseAndPlay_BTN;
        private System.Windows.Forms.Button Start_BTN;
        private System.Windows.Forms.Label filePath_LBL;
        private System.Windows.Forms.Label waitAfter_LBL;
        private System.Windows.Forms.Label waitB4_LBL;
        private System.Windows.Forms.Label yCoord_LBL;
        private System.Windows.Forms.Label xCoord_LBL;
        private System.Windows.Forms.Label heightScreenshot_LBL;
        private System.Windows.Forms.Label ScreenshotWidth_LBL;
        private System.Windows.Forms.Label saveFilePath_LBL;
        private System.Windows.Forms.ListBox fileName_LB;
        private System.Windows.Forms.TextBox waitAfter_TB;
        private System.Windows.Forms.TextBox waitB4_TB;
        private System.Windows.Forms.TextBox yCoord_TB;
        private System.Windows.Forms.TextBox xCoord_TB;
        private System.Windows.Forms.TextBox screenshotHeight_TB;
        private System.Windows.Forms.TextBox screenshotWidth_TB;
        private System.Windows.Forms.TextBox saveFilePath_TB;
        private System.Windows.Forms.TextBox filePath_TB;
        private System.Windows.Forms.ListBox filePath_LB;
        private System.Windows.Forms.Button import_BTN;
        private System.Windows.Forms.Label timeLeft_LBL;
        private System.Windows.Forms.Label screenshotNumber_LBL;
        private System.Windows.Forms.Label processStatus_LBL;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox hiddenTB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox stateTxt;
        private System.Windows.Forms.Button CalculateTime_BTN;
    }
}

