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
            this.screenshotAutoTop_Panel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.waitB4_TB = new System.Windows.Forms.TextBox();
            this.Import_BTN = new System.Windows.Forms.Button();
            this.filePath_TB = new System.Windows.Forms.TextBox();
            this.savePath_TB = new System.Windows.Forms.TextBox();
            this.processStatus_LBL = new System.Windows.Forms.Label();
            this.Prepare_BTN = new System.Windows.Forms.Button();
            this.possibleSizes_DGV = new System.Windows.Forms.DataGridView();
            this.percentOfMemory_DGV = new System.Windows.Forms.DataGridView();
            this.SavePath_BTN = new System.Windows.Forms.Button();
            this.screenshotAutoSide_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.possibleSizes_DGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.percentOfMemory_DGV)).BeginInit();
            this.SuspendLayout();
            // 
            // screenshotAutoSide_Panel
            // 
            this.screenshotAutoSide_Panel.BackColor = System.Drawing.Color.LightSlateGray;
            this.screenshotAutoSide_Panel.Controls.Add(this.SavePath_BTN);
            this.screenshotAutoSide_Panel.Controls.Add(this.percentOfMemory_DGV);
            this.screenshotAutoSide_Panel.Controls.Add(this.possibleSizes_DGV);
            this.screenshotAutoSide_Panel.Controls.Add(this.Prepare_BTN);
            this.screenshotAutoSide_Panel.Controls.Add(this.processStatus_LBL);
            this.screenshotAutoSide_Panel.Controls.Add(this.savePath_TB);
            this.screenshotAutoSide_Panel.Controls.Add(this.filePath_TB);
            this.screenshotAutoSide_Panel.Controls.Add(this.waitB4_TB);
            this.screenshotAutoSide_Panel.Controls.Add(this.label1);
            this.screenshotAutoSide_Panel.Controls.Add(this.Import_BTN);
            this.screenshotAutoSide_Panel.Location = new System.Drawing.Point(0, 92);
            this.screenshotAutoSide_Panel.Name = "screenshotAutoSide_Panel";
            this.screenshotAutoSide_Panel.Size = new System.Drawing.Size(320, 947);
            this.screenshotAutoSide_Panel.TabIndex = 0;
            // 
            // screenshotAutoTop_Panel
            // 
            this.screenshotAutoTop_Panel.Location = new System.Drawing.Point(0, 0);
            this.screenshotAutoTop_Panel.Name = "screenshotAutoTop_Panel";
            this.screenshotAutoTop_Panel.Size = new System.Drawing.Size(1980, 92);
            this.screenshotAutoTop_Panel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Millisecond wait before each screenshot";
            // 
            // waitB4_TB
            // 
            this.waitB4_TB.Location = new System.Drawing.Point(69, 73);
            this.waitB4_TB.Name = "waitB4_TB";
            this.waitB4_TB.Size = new System.Drawing.Size(100, 21);
            this.waitB4_TB.TabIndex = 2;
            // 
            // Import_BTN
            // 
            this.Import_BTN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.Import_BTN.Location = new System.Drawing.Point(80, 123);
            this.Import_BTN.Name = "Import_BTN";
            this.Import_BTN.Size = new System.Drawing.Size(79, 23);
            this.Import_BTN.TabIndex = 0;
            this.Import_BTN.Text = "Import files";
            this.Import_BTN.UseVisualStyleBackColor = true;
            this.Import_BTN.Click += new System.EventHandler(this.Import_BTN_Click);
            // 
            // filePath_TB
            // 
            this.filePath_TB.Location = new System.Drawing.Point(69, 177);
            this.filePath_TB.Name = "filePath_TB";
            this.filePath_TB.ReadOnly = true;
            this.filePath_TB.Size = new System.Drawing.Size(100, 21);
            this.filePath_TB.TabIndex = 3;
            // 
            // savePath_TB
            // 
            this.savePath_TB.Location = new System.Drawing.Point(69, 256);
            this.savePath_TB.Name = "savePath_TB";
            this.savePath_TB.ReadOnly = true;
            this.savePath_TB.Size = new System.Drawing.Size(100, 21);
            this.savePath_TB.TabIndex = 4;
            // 
            // processStatus_LBL
            // 
            this.processStatus_LBL.AutoSize = true;
            this.processStatus_LBL.Location = new System.Drawing.Point(77, 717);
            this.processStatus_LBL.Name = "processStatus_LBL";
            this.processStatus_LBL.Size = new System.Drawing.Size(101, 16);
            this.processStatus_LBL.TabIndex = 5;
            this.processStatus_LBL.Text = "Process status: ";
            // 
            // Prepare_BTN
            // 
            this.Prepare_BTN.Location = new System.Drawing.Point(38, 361);
            this.Prepare_BTN.Name = "Prepare_BTN";
            this.Prepare_BTN.Size = new System.Drawing.Size(158, 44);
            this.Prepare_BTN.TabIndex = 6;
            this.Prepare_BTN.Text = "Make calculations to prepare for process";
            this.Prepare_BTN.UseVisualStyleBackColor = true;
            this.Prepare_BTN.Click += new System.EventHandler(this.Prepare_BTN_Click);
            // 
            // possibleSizes_DGV
            // 
            this.possibleSizes_DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.possibleSizes_DGV.Location = new System.Drawing.Point(15, 433);
            this.possibleSizes_DGV.Name = "possibleSizes_DGV";
            this.possibleSizes_DGV.Size = new System.Drawing.Size(102, 229);
            this.possibleSizes_DGV.TabIndex = 7;
            // 
            // percentOfMemory_DGV
            // 
            this.percentOfMemory_DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.percentOfMemory_DGV.Location = new System.Drawing.Point(184, 433);
            this.percentOfMemory_DGV.Name = "percentOfMemory_DGV";
            this.percentOfMemory_DGV.Size = new System.Drawing.Size(102, 229);
            this.percentOfMemory_DGV.TabIndex = 8;
            // 
            // SavePath_BTN
            // 
            this.SavePath_BTN.Location = new System.Drawing.Point(48, 210);
            this.SavePath_BTN.Name = "SavePath_BTN";
            this.SavePath_BTN.Size = new System.Drawing.Size(130, 23);
            this.SavePath_BTN.TabIndex = 9;
            this.SavePath_BTN.Text = "Select save path";
            this.SavePath_BTN.UseVisualStyleBackColor = true;
            this.SavePath_BTN.Click += new System.EventHandler(this.SavePath_BTN_Click);
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
            ((System.ComponentModel.ISupportInitialize)(this.possibleSizes_DGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.percentOfMemory_DGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel screenshotAutoSide_Panel;
        private System.Windows.Forms.Panel screenshotAutoTop_Panel;
        private System.Windows.Forms.TextBox waitB4_TB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox savePath_TB;
        private System.Windows.Forms.TextBox filePath_TB;
        private System.Windows.Forms.Button Import_BTN;
        private System.Windows.Forms.Label processStatus_LBL;
        private System.Windows.Forms.Button Prepare_BTN;
        private System.Windows.Forms.DataGridView possibleSizes_DGV;
        private System.Windows.Forms.DataGridView percentOfMemory_DGV;
        private System.Windows.Forms.Button SavePath_BTN;
    }
}

