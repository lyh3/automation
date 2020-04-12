namespace SwitchConfigOnFly
{
    partial class SwitchConfigOnFlyControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwitchConfigOnFlyControl));
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.listBoxConfigSource = new System.Windows.Forms.ListBox();
            this.txtConfigSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLogfileName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(506, 21);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(140, 32);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "&Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.OnStart);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(506, 71);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(140, 32);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "&Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.OnStop);
            // 
            // listBoxConfigSource
            // 
            this.listBoxConfigSource.BackColor = System.Drawing.Color.Black;
            this.listBoxConfigSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxConfigSource.ForeColor = System.Drawing.Color.Goldenrod;
            this.listBoxConfigSource.FormattingEnabled = true;
            this.listBoxConfigSource.ItemHeight = 16;
            this.listBoxConfigSource.Location = new System.Drawing.Point(3, 3);
            this.listBoxConfigSource.Name = "listBoxConfigSource";
            this.listBoxConfigSource.Size = new System.Drawing.Size(482, 196);
            this.listBoxConfigSource.TabIndex = 3;
            this.listBoxConfigSource.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            // 
            // txtConfigSource
            // 
            this.txtConfigSource.BackColor = System.Drawing.Color.Black;
            this.txtConfigSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConfigSource.ForeColor = System.Drawing.Color.White;
            this.txtConfigSource.Location = new System.Drawing.Point(3, 229);
            this.txtConfigSource.Name = "txtConfigSource";
            this.txtConfigSource.Size = new System.Drawing.Size(482, 20);
            this.txtConfigSource.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 207);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Target configuration file name:";
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Image = ((System.Drawing.Image)(resources.GetObject("btnApply.Image")));
            this.btnApply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnApply.Location = new System.Drawing.Point(506, 259);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(140, 42);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "&Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.OnApply);
            // 
            // _openFileDialog
            // 
            this._openFileDialog.FileName = "ModuleRegistor.Resources.dll";
            this._openFileDialog.Filter = "All Files|*.*|Config Files|*.config|Xml Files|*.xml";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowse.Image")));
            this.btnBrowse.Location = new System.Drawing.Point(506, 227);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(26, 26);
            this.btnBrowse.TabIndex = 7;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.OnBrowseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 273);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Log file output path:";
            // 
            // txtLogfileName
            // 
            this.txtLogfileName.Location = new System.Drawing.Point(117, 270);
            this.txtLogfileName.Name = "txtLogfileName";
            this.txtLogfileName.Size = new System.Drawing.Size(368, 20);
            this.txtLogfileName.TabIndex = 9;
            // 
            // SwitchConfigOnFlyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.txtLogfileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtConfigSource);
            this.Controls.Add(this.listBoxConfigSource);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "SwitchConfigOnFlyControl";
            this.Size = new System.Drawing.Size(655, 314);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ListBox listBoxConfigSource;
        private System.Windows.Forms.TextBox txtConfigSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.OpenFileDialog _openFileDialog;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLogfileName;
    }
}
