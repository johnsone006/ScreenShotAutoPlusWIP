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
            this.topPanel = new System.Windows.Forms.Panel();
            this.import_BTN = new System.Windows.Forms.Button();
            this.Start_BTN = new System.Windows.Forms.Button();
            this.pauseAndPlay_BTN = new System.Windows.Forms.Button();
            this.processStatus_LBL = new System.Windows.Forms.Label();
            this.screenshotNumber_LBL = new System.Windows.Forms.Label();
            this.timeLeft_LBL = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Save_FolderBTN = new System.Windows.Forms.Button();
            this.filePath_TB = new System.Windows.Forms.TextBox();
            this.savePath_TB = new System.Windows.Forms.TextBox();
            this.completedScreenshots_LBL = new System.Windows.Forms.Label();
            this.Exit_BTN = new System.Windows.Forms.Button();
            this.waitB4_LBL = new System.Windows.Forms.Label();
            this.waitB4_TB = new System.Windows.Forms.TextBox();
            this.BackgroundWorker1 = new System.ComponentModel.BackgroundWorker();
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
            // import_BTN
            // 
            this.import_BTN.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.import_BTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.import_BTN.Location = new System.Drawing.Point(57, 101);
            this.import_BTN.Name = "import_BTN";
            this.import_BTN.Size = new System.Drawing.Size(89, 23);
            this.import_BTN.TabIndex = 4;
            this.import_BTN.Text = "Import files";
            this.import_BTN.UseVisualStyleBackColor = true;
            this.import_BTN.Click += new System.EventHandler(this.Import_BTN_Click);
            // 
            // Start_BTN
            // 
            this.Start_BTN.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Start_BTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Start_BTN.Location = new System.Drawing.Point(72, 299);
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
            this.pauseAndPlay_BTN.Location = new System.Drawing.Point(48, 422);
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
            this.timeLeft_LBL.Size = new System.Drawing.Size(137, 16);
            this.timeLeft_LBL.TabIndex = 4;
            this.timeLeft_LBL.Text = "Time left (in seconds):";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.Save_FolderBTN);
            this.panel1.Controls.Add(this.filePath_TB);
            this.panel1.Controls.Add(this.savePath_TB);
            this.panel1.Controls.Add(this.completedScreenshots_LBL);
            this.panel1.Controls.Add(this.Exit_BTN);
            this.panel1.Controls.Add(this.timeLeft_LBL);
            this.panel1.Controls.Add(this.waitB4_LBL);
            this.panel1.Controls.Add(this.screenshotNumber_LBL);
            this.panel1.Controls.Add(this.processStatus_LBL);
            this.panel1.Controls.Add(this.pauseAndPlay_BTN);
            this.panel1.Controls.Add(this.Start_BTN);
            this.panel1.Controls.Add(this.import_BTN);
            this.panel1.Controls.Add(this.waitB4_TB);
            this.panel1.Location = new System.Drawing.Point(0, 91);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(236, 949);
            this.panel1.TabIndex = 21;
            // 
            // Save_FolderBTN
            // 
            this.Save_FolderBTN.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Save_FolderBTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Save_FolderBTN.Location = new System.Drawing.Point(25, 169);
            this.Save_FolderBTN.Name = "Save_FolderBTN";
            this.Save_FolderBTN.Size = new System.Drawing.Size(156, 23);
            this.Save_FolderBTN.TabIndex = 31;
            this.Save_FolderBTN.Text = "Folder for screenshots";
            this.Save_FolderBTN.UseVisualStyleBackColor = true;
            this.Save_FolderBTN.Click += new System.EventHandler(this.Save_FolderBTN_Click);
            // 
            // filePath_TB
            // 
            this.filePath_TB.Location = new System.Drawing.Point(57, 141);
            this.filePath_TB.Name = "filePath_TB";
            this.filePath_TB.ReadOnly = true;
            this.filePath_TB.Size = new System.Drawing.Size(78, 22);
            this.filePath_TB.TabIndex = 30;
            // 
            // savePath_TB
            // 
            this.savePath_TB.Location = new System.Drawing.Point(57, 215);
            this.savePath_TB.Name = "savePath_TB";
            this.savePath_TB.ReadOnly = true;
            this.savePath_TB.Size = new System.Drawing.Size(78, 22);
            this.savePath_TB.TabIndex = 28;
            // 
            // completedScreenshots_LBL
            // 
            this.completedScreenshots_LBL.AutoSize = true;
            this.completedScreenshots_LBL.Location = new System.Drawing.Point(22, 872);
            this.completedScreenshots_LBL.Name = "completedScreenshots_LBL";
            this.completedScreenshots_LBL.Size = new System.Drawing.Size(152, 16);
            this.completedScreenshots_LBL.TabIndex = 27;
            this.completedScreenshots_LBL.Text = "Completed screenshots:";
            // 
            // Exit_BTN
            // 
            this.Exit_BTN.Location = new System.Drawing.Point(22, 903);
            this.Exit_BTN.Name = "Exit_BTN";
            this.Exit_BTN.Size = new System.Drawing.Size(139, 23);
            this.Exit_BTN.TabIndex = 26;
            this.Exit_BTN.Text = "Exit";
            this.Exit_BTN.UseVisualStyleBackColor = true;
            this.Exit_BTN.Click += new System.EventHandler(this.Exit_BTN_Click);
            // 
            // waitB4_LBL
            // 
            this.waitB4_LBL.AutoEllipsis = true;
            this.waitB4_LBL.Location = new System.Drawing.Point(3, 13);
            this.waitB4_LBL.Name = "waitB4_LBL";
            this.waitB4_LBL.Size = new System.Drawing.Size(218, 36);
            this.waitB4_LBL.TabIndex = 6;
            this.waitB4_LBL.Text = "Milisecond pause prior to each screenshot:";
            this.waitB4_LBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.waitB4_TB.WordWrap = false;
            // 
            // BackgroundWorker1
            // 
            this.BackgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
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
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Button import_BTN;
        private System.Windows.Forms.Button Start_BTN;
        private System.Windows.Forms.Button pauseAndPlay_BTN;
        private System.Windows.Forms.Label processStatus_LBL;
        private System.Windows.Forms.Label screenshotNumber_LBL;
        private System.Windows.Forms.Label timeLeft_LBL;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Exit_BTN;
        private System.Windows.Forms.Label completedScreenshots_LBL;
        private System.ComponentModel.BackgroundWorker BackgroundWorker1;
        private System.Windows.Forms.TextBox savePath_TB;
        private System.Windows.Forms.Label waitB4_LBL;
        private System.Windows.Forms.TextBox waitB4_TB;
        private System.Windows.Forms.TextBox filePath_TB;
        private System.Windows.Forms.Button Save_FolderBTN;
    }
}

