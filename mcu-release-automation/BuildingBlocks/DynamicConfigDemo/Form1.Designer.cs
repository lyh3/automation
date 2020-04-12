namespace DynamicConfigDemo
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
            this.checkBoxLazyUpdate = new System.Windows.Forms.CheckBox();
            this.btnSync = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textSubmissionGroup = new System.Windows.Forms.TextBox();
            this.textMaxFileSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxLazyUpdate
            // 
            this.checkBoxLazyUpdate.AutoSize = true;
            this.checkBoxLazyUpdate.Location = new System.Drawing.Point(229, 37);
            this.checkBoxLazyUpdate.Name = "checkBoxLazyUpdate";
            this.checkBoxLazyUpdate.Size = new System.Drawing.Size(83, 17);
            this.checkBoxLazyUpdate.TabIndex = 1;
            this.checkBoxLazyUpdate.Text = "LazyUpdate";
            this.checkBoxLazyUpdate.UseVisualStyleBackColor = true;
            this.checkBoxLazyUpdate.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(229, 74);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.TabIndex = 2;
            this.btnSync.Text = "&Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.OnSyncClick);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textMaxFileSize);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textSubmissionGroup);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 129);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(306, 75);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Submission Group";
            // 
            // textSubmissionGroup
            // 
            this.textSubmissionGroup.Location = new System.Drawing.Point(105, 17);
            this.textSubmissionGroup.Name = "textSubmissionGroup";
            this.textSubmissionGroup.Size = new System.Drawing.Size(187, 20);
            this.textSubmissionGroup.TabIndex = 1;
            // 
            // textMaxFileSize
            // 
            this.textMaxFileSize.Location = new System.Drawing.Point(106, 44);
            this.textMaxFileSize.Name = "textMaxFileSize";
            this.textMaxFileSize.Size = new System.Drawing.Size(187, 20);
            this.textMaxFileSize.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "MaxFileSize";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 208);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.checkBoxLazyUpdate);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxLazyUpdate;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textMaxFileSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textSubmissionGroup;
        private System.Windows.Forms.Label label1;
    }
}

