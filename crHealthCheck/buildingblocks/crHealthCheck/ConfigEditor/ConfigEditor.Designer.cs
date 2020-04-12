namespace ConfigEditor
{
    partial class frmConfigEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfigEditor));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageHealthTest = new System.Windows.Forms.TabPage();
            this.tabPageBIOSSettings = new System.Windows.Forms.TabPage();
            this.statusStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.DimGray;
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 713);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 13, 0);
            this.statusStrip.Size = new System.Drawing.Size(1092, 25);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.toolStripStatusLabel.ForeColor = System.Drawing.Color.White;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(151, 20);
            this.toolStripStatusLabel.Text = "toolStripStatusLabel1";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageHealthTest);
            this.tabControl.Controls.Add(this.tabPageBIOSSettings);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1092, 713);
            this.tabControl.TabIndex = 3;
            // 
            // tabPageHealthTest
            // 
            this.tabPageHealthTest.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageHealthTest.BackgroundImage")));
            this.tabPageHealthTest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabPageHealthTest.Location = new System.Drawing.Point(4, 25);
            this.tabPageHealthTest.Name = "tabPageHealthTest";
            this.tabPageHealthTest.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHealthTest.Size = new System.Drawing.Size(1084, 684);
            this.tabPageHealthTest.TabIndex = 0;
            this.tabPageHealthTest.Text = "Health Test Settings";
            this.tabPageHealthTest.UseVisualStyleBackColor = true;
            // 
            // tabPageBIOSSettings
            // 
            this.tabPageBIOSSettings.Location = new System.Drawing.Point(4, 25);
            this.tabPageBIOSSettings.Name = "tabPageBIOSSettings";
            this.tabPageBIOSSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBIOSSettings.Size = new System.Drawing.Size(1084, 684);
            this.tabPageBIOSSettings.TabIndex = 1;
            this.tabPageBIOSSettings.Text = "BIOS settings";
            this.tabPageBIOSSettings.UseVisualStyleBackColor = true;
            // 
            // frmConfigEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1092, 738);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmConfigEditor";
            this.Text = "Apache Pass Health Check";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageHealthTest;
        private System.Windows.Forms.TabPage tabPageBIOSSettings;
    }
}

