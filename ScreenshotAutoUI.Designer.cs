namespace ScreenshotAuto
{
    partial class ScreenshotAutoUI
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
            this.screenshotAutoSide_Panel = new System.Windows.Forms.Panel();
            this.screenshotID_LBL = new System.Windows.Forms.Label();
            this.sizeID_NUD = new System.Windows.Forms.NumericUpDown();
            this.screenshotInformation_DGV = new System.Windows.Forms.DataGridView();
            this.Start_BTN = new System.Windows.Forms.Button();
            this.SavePath_BTN = new System.Windows.Forms.Button();
            this.Prepare_BTN = new System.Windows.Forms.Button();
            this.processStatus_LBL = new System.Windows.Forms.Label();
            this.savePath_TB = new System.Windows.Forms.TextBox();
            this.filePath_TB = new System.Windows.Forms.TextBox();
            this.waitB4_TB = new System.Windows.Forms.TextBox();
            this.waitB4_LBL = new System.Windows.Forms.Label();
            this.Import_BTN = new System.Windows.Forms.Button();
            this.screenshotAutoTop_Panel = new System.Windows.Forms.Panel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.IdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SizesColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PercentageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeLeft_LBL = new System.Windows.Forms.Label();
            this.completedScreenshotsCount_LBL = new System.Windows.Forms.Label();
            this.screenshotAutoSide_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeID_NUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.screenshotInformation_DGV)).BeginInit();
            this.SuspendLayout();
            // 
            // screenshotAutoSide_Panel
            // 
            this.screenshotAutoSide_Panel.BackColor = System.Drawing.Color.LightSlateGray;
            this.screenshotAutoSide_Panel.Controls.Add(this.completedScreenshotsCount_LBL);
            this.screenshotAutoSide_Panel.Controls.Add(this.timeLeft_LBL);
            this.screenshotAutoSide_Panel.Controls.Add(this.screenshotID_LBL);
            this.screenshotAutoSide_Panel.Controls.Add(this.sizeID_NUD);
            this.screenshotAutoSide_Panel.Controls.Add(this.screenshotInformation_DGV);
            this.screenshotAutoSide_Panel.Controls.Add(this.Start_BTN);
            this.screenshotAutoSide_Panel.Controls.Add(this.SavePath_BTN);
            this.screenshotAutoSide_Panel.Controls.Add(this.Prepare_BTN);
            this.screenshotAutoSide_Panel.Controls.Add(this.processStatus_LBL);
            this.screenshotAutoSide_Panel.Controls.Add(this.savePath_TB);
            this.screenshotAutoSide_Panel.Controls.Add(this.filePath_TB);
            this.screenshotAutoSide_Panel.Controls.Add(this.waitB4_TB);
            this.screenshotAutoSide_Panel.Controls.Add(this.waitB4_LBL);
            this.screenshotAutoSide_Panel.Controls.Add(this.Import_BTN);
            this.screenshotAutoSide_Panel.Location = new System.Drawing.Point(0, 92);
            this.screenshotAutoSide_Panel.Name = "screenshotAutoSide_Panel";
            this.screenshotAutoSide_Panel.Size = new System.Drawing.Size(330, 947);
            this.screenshotAutoSide_Panel.TabIndex = 0;
            // 
            // screenshotID_LBL
            // 
            this.screenshotID_LBL.AutoSize = true;
            this.screenshotID_LBL.Location = new System.Drawing.Point(17, 645);
            this.screenshotID_LBL.Name = "screenshotID_LBL";
            this.screenshotID_LBL.Size = new System.Drawing.Size(178, 16);
            this.screenshotID_LBL.TabIndex = 13;
            this.screenshotID_LBL.Text = "Id of chosen screenshot size:";
            // 
            // sizeID_NUD
            // 
            this.sizeID_NUD.Location = new System.Drawing.Point(46, 675);
            this.sizeID_NUD.Name = "sizeID_NUD";
            this.sizeID_NUD.Size = new System.Drawing.Size(97, 21);
            this.sizeID_NUD.TabIndex = 12;
            // 
            // screenshotInformation_DGV
            // 
            this.screenshotInformation_DGV.AllowUserToAddRows = false;
            this.screenshotInformation_DGV.AllowUserToDeleteRows = false;
            this.screenshotInformation_DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.screenshotInformation_DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdColumn,
            this.SizesColumn,
            this.PercentageColumn});
            this.screenshotInformation_DGV.Location = new System.Drawing.Point(0, 313);
            this.screenshotInformation_DGV.Name = "screenshotInformation_DGV";
            this.screenshotInformation_DGV.ReadOnly = true;
            this.screenshotInformation_DGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.screenshotInformation_DGV.Size = new System.Drawing.Size(327, 281);
            this.screenshotInformation_DGV.TabIndex = 11;
            // 
            // Start_BTN
            // 
            this.Start_BTN.Location = new System.Drawing.Point(110, 735);
            this.Start_BTN.Name = "Start_BTN";
            this.Start_BTN.Size = new System.Drawing.Size(75, 23);
            this.Start_BTN.TabIndex = 10;
            this.Start_BTN.Text = "Start";
            this.Start_BTN.UseVisualStyleBackColor = true;
            this.Start_BTN.Click += new System.EventHandler(this.Start_BTN_Click);
            // 
            // SavePath_BTN
            // 
            this.SavePath_BTN.Location = new System.Drawing.Point(197, 95);
            this.SavePath_BTN.Name = "SavePath_BTN";
            this.SavePath_BTN.Size = new System.Drawing.Size(130, 23);
            this.SavePath_BTN.TabIndex = 9;
            this.SavePath_BTN.Text = "Select save path";
            this.SavePath_BTN.UseVisualStyleBackColor = true;
            this.SavePath_BTN.Click += new System.EventHandler(this.SavePath_BTN_Click);
            // 
            // Prepare_BTN
            // 
            this.Prepare_BTN.Location = new System.Drawing.Point(76, 167);
            this.Prepare_BTN.Name = "Prepare_BTN";
            this.Prepare_BTN.Size = new System.Drawing.Size(158, 44);
            this.Prepare_BTN.TabIndex = 6;
            this.Prepare_BTN.Text = "Make calculations to prepare for process";
            this.Prepare_BTN.UseVisualStyleBackColor = true;
            this.Prepare_BTN.Click += new System.EventHandler(this.Prepare_BTN_Click);
            // 
            // processStatus_LBL
            // 
            this.processStatus_LBL.AutoSize = true;
            this.processStatus_LBL.Location = new System.Drawing.Point(94, 789);
            this.processStatus_LBL.Name = "processStatus_LBL";
            this.processStatus_LBL.Size = new System.Drawing.Size(101, 16);
            this.processStatus_LBL.TabIndex = 5;
            this.processStatus_LBL.Text = "Process status: ";
            // 
            // savePath_TB
            // 
            this.savePath_TB.Location = new System.Drawing.Point(197, 124);
            this.savePath_TB.Name = "savePath_TB";
            this.savePath_TB.ReadOnly = true;
            this.savePath_TB.Size = new System.Drawing.Size(130, 21);
            this.savePath_TB.TabIndex = 4;
            // 
            // filePath_TB
            // 
            this.filePath_TB.Location = new System.Drawing.Point(12, 124);
            this.filePath_TB.Name = "filePath_TB";
            this.filePath_TB.ReadOnly = true;
            this.filePath_TB.Size = new System.Drawing.Size(100, 21);
            this.filePath_TB.TabIndex = 3;
            // 
            // waitB4_TB
            // 
            this.waitB4_TB.Location = new System.Drawing.Point(110, 66);
            this.waitB4_TB.Name = "waitB4_TB";
            this.waitB4_TB.Size = new System.Drawing.Size(76, 21);
            this.waitB4_TB.TabIndex = 2;
            // 
            // waitB4_LBL
            // 
            this.waitB4_LBL.AutoSize = true;
            this.waitB4_LBL.Location = new System.Drawing.Point(43, 17);
            this.waitB4_LBL.Name = "waitB4_LBL";
            this.waitB4_LBL.Size = new System.Drawing.Size(245, 16);
            this.waitB4_LBL.TabIndex = 1;
            this.waitB4_LBL.Text = "Millisecond wait before each screenshot";
            // 
            // Import_BTN
            // 
            this.Import_BTN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.Import_BTN.Location = new System.Drawing.Point(12, 95);
            this.Import_BTN.Name = "Import_BTN";
            this.Import_BTN.Size = new System.Drawing.Size(79, 23);
            this.Import_BTN.TabIndex = 0;
            this.Import_BTN.Text = "Import files";
            this.Import_BTN.UseVisualStyleBackColor = true;
            this.Import_BTN.Click += new System.EventHandler(this.Import_BTN_Click);
            // 
            // screenshotAutoTop_Panel
            // 
            this.screenshotAutoTop_Panel.Location = new System.Drawing.Point(0, 0);
            this.screenshotAutoTop_Panel.Name = "screenshotAutoTop_Panel";
            this.screenshotAutoTop_Panel.Size = new System.Drawing.Size(1980, 92);
            this.screenshotAutoTop_Panel.TabIndex = 1;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            // 
            // IdColumn
            // 
            this.IdColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.IdColumn.HeaderText = "Id of screenshot size:";
            this.IdColumn.Name = "IdColumn";
            this.IdColumn.ReadOnly = true;
            // 
            // SizesColumn
            // 
            this.SizesColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SizesColumn.HeaderText = "Potential size of screenshot:";
            this.SizesColumn.Name = "SizesColumn";
            this.SizesColumn.ReadOnly = true;
            // 
            // PercentageColumn
            // 
            this.PercentageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PercentageColumn.HeaderText = "Percentage of memory all screenshots will take up at that size:";
            this.PercentageColumn.Name = "PercentageColumn";
            this.PercentageColumn.ReadOnly = true;
            // 
            // timeLeft_LBL
            // 
            this.timeLeft_LBL.AutoSize = true;
            this.timeLeft_LBL.Location = new System.Drawing.Point(107, 830);
            this.timeLeft_LBL.Name = "timeLeft_LBL";
            this.timeLeft_LBL.Size = new System.Drawing.Size(64, 16);
            this.timeLeft_LBL.TabIndex = 2;
            this.timeLeft_LBL.Text = "Time left: ";
            // 
            // completedScreenshotsCount_LBL
            // 
            this.completedScreenshotsCount_LBL.AutoSize = true;
            this.completedScreenshotsCount_LBL.Location = new System.Drawing.Point(27, 865);
            this.completedScreenshotsCount_LBL.Name = "completedScreenshotsCount_LBL";
            this.completedScreenshotsCount_LBL.Size = new System.Drawing.Size(218, 16);
            this.completedScreenshotsCount_LBL.TabIndex = 14;
            this.completedScreenshotsCount_LBL.Text = "Number of completed screenshots: ";
            // 
            // ScreenshotAutoUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1980, 1040);
            this.Controls.Add(this.screenshotAutoTop_Panel);
            this.Controls.Add(this.screenshotAutoSide_Panel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ScreenshotAutoUI";
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.Color.Silver;
            this.Load += new System.EventHandler(this.ScreenshotAutoUI_Load);
            this.screenshotAutoSide_Panel.ResumeLayout(false);
            this.screenshotAutoSide_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeID_NUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.screenshotInformation_DGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel screenshotAutoSide_Panel;
        private System.Windows.Forms.Panel screenshotAutoTop_Panel;
        private System.Windows.Forms.TextBox waitB4_TB;
        private System.Windows.Forms.Label waitB4_LBL;
        private System.Windows.Forms.TextBox savePath_TB;
        private System.Windows.Forms.TextBox filePath_TB;
        private System.Windows.Forms.Button Import_BTN;
        private System.Windows.Forms.Label processStatus_LBL;
        private System.Windows.Forms.Button Prepare_BTN;
        private System.Windows.Forms.Button SavePath_BTN;
        private System.Windows.Forms.Button Start_BTN;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.DataGridView screenshotInformation_DGV;
        private System.Windows.Forms.Label screenshotID_LBL;
        private System.Windows.Forms.NumericUpDown sizeID_NUD;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SizesColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PercentageColumn;
        private System.Windows.Forms.Label timeLeft_LBL;
        private System.Windows.Forms.Label completedScreenshotsCount_LBL;
    }
}

