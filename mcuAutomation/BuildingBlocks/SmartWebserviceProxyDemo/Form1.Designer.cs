namespace SmartWebserviceProxyDemo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSync = new System.Windows.Forms.Button();
            this.checkBoxLazyUpdate = new System.Windows.Forms.CheckBox();
            this.scrollableControl = new System.Windows.Forms.ScrollableControl();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.messageBox = new System.Windows.Forms.RichTextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this._repeatUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxBadEndpointCount = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.textBoxCurrentSelectedEndpoint = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._repeatUpDown)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Silver;
            this.groupBox1.Controls.Add(this.btnSync);
            this.groupBox1.Controls.Add(this.checkBoxLazyUpdate);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(392, 48);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Endpoints Config";
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(276, 15);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.TabIndex = 3;
            this.btnSync.Text = "&Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.OnSyncClick);
            // 
            // checkBoxLazyUpdate
            // 
            this.checkBoxLazyUpdate.AutoSize = true;
            this.checkBoxLazyUpdate.Location = new System.Drawing.Point(31, 19);
            this.checkBoxLazyUpdate.Name = "checkBoxLazyUpdate";
            this.checkBoxLazyUpdate.Size = new System.Drawing.Size(83, 17);
            this.checkBoxLazyUpdate.TabIndex = 2;
            this.checkBoxLazyUpdate.Text = "LazyUpdate";
            this.checkBoxLazyUpdate.UseVisualStyleBackColor = true;
            this.checkBoxLazyUpdate.CheckedChanged += new System.EventHandler(this.OnLazyUpdateCheckedChanged);
            // 
            // scrollableControl
            // 
            this.scrollableControl.AutoScroll = true;
            this.scrollableControl.BackColor = System.Drawing.Color.Gainsboro;
            this.scrollableControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.scrollableControl.Location = new System.Drawing.Point(0, 48);
            this.scrollableControl.Name = "scrollableControl";
            this.scrollableControl.Size = new System.Drawing.Size(392, 204);
            this.scrollableControl.TabIndex = 1;
            this.scrollableControl.Text = "scrollableControl1";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "iconOk.png");
            this.imageList.Images.SetKeyName(1, "iconWarning.png");
            // 
            // messageBox
            // 
            this.messageBox.BackColor = System.Drawing.Color.Black;
            this.messageBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.messageBox.ForeColor = System.Drawing.Color.Gold;
            this.messageBox.Location = new System.Drawing.Point(0, 347);
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(392, 160);
            this.messageBox.TabIndex = 3;
            this.messageBox.Text = "";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // _repeatUpDown
            // 
            this._repeatUpDown.BackColor = System.Drawing.Color.Gray;
            this._repeatUpDown.ForeColor = System.Drawing.Color.PaleTurquoise;
            this._repeatUpDown.Location = new System.Drawing.Point(324, 21);
            this._repeatUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this._repeatUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._repeatUpDown.Name = "_repeatUpDown";
            this._repeatUpDown.Size = new System.Drawing.Size(53, 20);
            this._repeatUpDown.TabIndex = 21;
            this._repeatUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._repeatUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(276, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Repeat";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Bad endpoint count";
            // 
            // textBoxBadEndpointCount
            // 
            this.textBoxBadEndpointCount.BackColor = System.Drawing.Color.DimGray;
            this.textBoxBadEndpointCount.ForeColor = System.Drawing.Color.Peru;
            this.textBoxBadEndpointCount.Location = new System.Drawing.Point(119, 19);
            this.textBoxBadEndpointCount.Name = "textBoxBadEndpointCount";
            this.textBoxBadEndpointCount.Size = new System.Drawing.Size(50, 20);
            this.textBoxBadEndpointCount.TabIndex = 24;
            this.textBoxBadEndpointCount.Text = "0";
            this.textBoxBadEndpointCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(305, 65);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 26;
            this.btnStart.Text = "&Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.OnStartClick);
            // 
            // textBoxCurrentSelectedEndpoint
            // 
            this.textBoxCurrentSelectedEndpoint.BackColor = System.Drawing.Color.DimGray;
            this.textBoxCurrentSelectedEndpoint.ForeColor = System.Drawing.Color.LightBlue;
            this.textBoxCurrentSelectedEndpoint.Location = new System.Drawing.Point(17, 67);
            this.textBoxCurrentSelectedEndpoint.Name = "textBoxCurrentSelectedEndpoint";
            this.textBoxCurrentSelectedEndpoint.Size = new System.Drawing.Size(282, 20);
            this.textBoxCurrentSelectedEndpoint.TabIndex = 28;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Current Selected Endpoint";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Silver;
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxCurrentSelectedEndpoint);
            this.groupBox2.Controls.Add(this.btnStart);
            this.groupBox2.Controls.Add(this.textBoxBadEndpointCount);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this._repeatUpDown);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 252);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(392, 95);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Peek Service Status";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 507);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.scrollableControl);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Smart Proxy Demo";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._repeatUpDown)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxLazyUpdate;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.ScrollableControl scrollableControl;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.RichTextBox messageBox;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.NumericUpDown _repeatUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxBadEndpointCount;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox textBoxCurrentSelectedEndpoint;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

