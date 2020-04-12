namespace ConfigEditor.Controls
{
    partial class SmokeTestControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SmokeTestControl));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblRunTest = new System.Windows.Forms.Label();
            this.lbleSelectSmokeJson = new System.Windows.Forms.Label();
            this.textBoxSmokeJson = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxLoops = new System.Windows.Forms.ComboBox();
            this.checkBoxWR = new System.Windows.Forms.CheckBox();
            this.checkBoxMM = new System.Windows.Forms.CheckBox();
            this.checkBoxAD = new System.Windows.Forms.CheckBox();
            this.checkBoxAC = new System.Windows.Forms.CheckBox();
            this.checkBoxSkipInitialAC = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 7;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.91489F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 68.08511F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 174F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 173F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 157F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 225F));
            this.tableLayoutPanel.Controls.Add(this.lblRunTest, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.lbleSelectSmokeJson, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxSmokeJson, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.comboBoxLoops, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.checkBoxWR, 3, 3);
            this.tableLayoutPanel.Controls.Add(this.checkBoxMM, 5, 2);
            this.tableLayoutPanel.Controls.Add(this.checkBoxAD, 4, 2);
            this.tableLayoutPanel.Controls.Add(this.checkBoxAC, 3, 2);
            this.tableLayoutPanel.Controls.Add(this.checkBoxSkipInitialAC, 4, 3);
            this.tableLayoutPanel.Controls.Add(this.label2, 6, 1);
            this.tableLayoutPanel.Controls.Add(this.pictureBox1, 6, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.94118F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.05882F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 244F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1084, 471);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // lblRunTest
            // 
            this.lblRunTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRunTest.AutoSize = true;
            this.lblRunTest.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRunTest.Location = new System.Drawing.Point(3, 0);
            this.lblRunTest.Name = "lblRunTest";
            this.lblRunTest.Size = new System.Drawing.Size(97, 49);
            this.lblRunTest.TabIndex = 12;
            this.lblRunTest.Text = "&Run Test";
            this.lblRunTest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbleSelectSmokeJson
            // 
            this.lbleSelectSmokeJson.AutoSize = true;
            this.lbleSelectSmokeJson.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbleSelectSmokeJson.ForeColor = System.Drawing.Color.Gray;
            this.lbleSelectSmokeJson.Location = new System.Drawing.Point(3, 49);
            this.lbleSelectSmokeJson.Name = "lbleSelectSmokeJson";
            this.lbleSelectSmokeJson.Size = new System.Drawing.Size(97, 43);
            this.lbleSelectSmokeJson.TabIndex = 9;
            this.lbleSelectSmokeJson.Text = "Selecte Json";
            this.lbleSelectSmokeJson.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxSmokeJson
            // 
            this.textBoxSmokeJson.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.textBoxSmokeJson.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxSmokeJson, 5);
            this.textBoxSmokeJson.ForeColor = System.Drawing.Color.DarkGray;
            this.textBoxSmokeJson.Location = new System.Drawing.Point(106, 51);
            this.textBoxSmokeJson.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxSmokeJson.Name = "textBoxSmokeJson";
            this.textBoxSmokeJson.Size = new System.Drawing.Size(749, 22);
            this.textBoxSmokeJson.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(3, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 54);
            this.label1.TabIndex = 10;
            this.label1.Text = "Loops";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBoxLoops
            // 
            this.comboBoxLoops.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxLoops.FormattingEnabled = true;
            this.comboBoxLoops.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100"});
            this.comboBoxLoops.Location = new System.Drawing.Point(106, 95);
            this.comboBoxLoops.Name = "comboBoxLoops";
            this.comboBoxLoops.Size = new System.Drawing.Size(214, 24);
            this.comboBoxLoops.TabIndex = 11;
            // 
            // checkBoxWR
            // 
            this.checkBoxWR.AutoSize = true;
            this.checkBoxWR.ForeColor = System.Drawing.Color.Orange;
            this.checkBoxWR.Location = new System.Drawing.Point(357, 149);
            this.checkBoxWR.Name = "checkBoxWR";
            this.checkBoxWR.Size = new System.Drawing.Size(144, 21);
            this.checkBoxWR.TabIndex = 17;
            this.checkBoxWR.Text = "Warm Reboot test";
            this.checkBoxWR.UseVisualStyleBackColor = true;
            // 
            // checkBoxMM
            // 
            this.checkBoxMM.AutoSize = true;
            this.checkBoxMM.ForeColor = System.Drawing.Color.Orange;
            this.checkBoxMM.Location = new System.Drawing.Point(704, 95);
            this.checkBoxMM.Name = "checkBoxMM";
            this.checkBoxMM.Size = new System.Drawing.Size(145, 21);
            this.checkBoxMM.TabIndex = 16;
            this.checkBoxMM.Text = "MemoryModel test";
            this.checkBoxMM.UseVisualStyleBackColor = true;
            // 
            // checkBoxAD
            // 
            this.checkBoxAD.AutoSize = true;
            this.checkBoxAD.ForeColor = System.Drawing.Color.Orange;
            this.checkBoxAD.Location = new System.Drawing.Point(531, 95);
            this.checkBoxAD.Name = "checkBoxAD";
            this.checkBoxAD.Size = new System.Drawing.Size(119, 21);
            this.checkBoxAD.TabIndex = 15;
            this.checkBoxAD.Text = "AppDirect test";
            this.checkBoxAD.UseVisualStyleBackColor = true;
            // 
            // checkBoxAC
            // 
            this.checkBoxAC.AutoSize = true;
            this.checkBoxAC.ForeColor = System.Drawing.Color.Orange;
            this.checkBoxAC.Location = new System.Drawing.Point(357, 95);
            this.checkBoxAC.Name = "checkBoxAC";
            this.checkBoxAC.Size = new System.Drawing.Size(75, 21);
            this.checkBoxAC.TabIndex = 19;
            this.checkBoxAC.Text = "AC test";
            this.checkBoxAC.UseVisualStyleBackColor = true;
            // 
            // checkBoxSkipInitialAC
            // 
            this.checkBoxSkipInitialAC.AutoSize = true;
            this.checkBoxSkipInitialAC.ForeColor = System.Drawing.Color.Lime;
            this.checkBoxSkipInitialAC.Location = new System.Drawing.Point(531, 149);
            this.checkBoxSkipInitialAC.Name = "checkBoxSkipInitialAC";
            this.checkBoxSkipInitialAC.Size = new System.Drawing.Size(115, 21);
            this.checkBoxSkipInitialAC.TabIndex = 20;
            this.checkBoxSkipInitialAC.Text = "Skip Initial AC";
            this.checkBoxSkipInitialAC.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label2.Location = new System.Drawing.Point(861, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 17);
            this.label2.TabIndex = 13;
            this.label2.Text = "Run SAT test";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Location = new System.Drawing.Point(861, 95);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 48);
            this.pictureBox1.TabIndex = 18;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.Silver;
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(1084, 28);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(120, 26);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.onOpen);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "*.json";
            this.openFileDialog.Filter = "json files (*.json)|*.txt|All files (*.*)|*.*";
            // 
            // SmokeTestControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "SmokeTestControl";
            this.Size = new System.Drawing.Size(1084, 471);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxSmokeJson;
        private System.Windows.Forms.Label lbleSelectSmokeJson;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxLoops;
        private System.Windows.Forms.Label lblRunTest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.CheckBox checkBoxAD;
        private System.Windows.Forms.CheckBox checkBoxMM;
        private System.Windows.Forms.CheckBox checkBoxWR;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBoxAC;
        private System.Windows.Forms.CheckBox checkBoxSkipInitialAC;
    }
}
