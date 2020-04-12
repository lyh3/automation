namespace SwitchConfigOnFly
{
    partial class DownloadSampleDemoControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadSampleDemoControl));
            this.listBoxMD5 = new System.Windows.Forms.ListBox();
            this.txtMD5Source = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnBrowseMD5List = new System.Windows.Forms.Button();
            this.treeViewFolderExplorer = new System.Windows.Forms.TreeView();
            this.btnDownload = new System.Windows.Forms.Button();
            this.listViewFoder = new System.Windows.Forms.ListView();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // listBoxMD5
            // 
            this.listBoxMD5.BackColor = System.Drawing.Color.Black;
            this.listBoxMD5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxMD5.ForeColor = System.Drawing.Color.Goldenrod;
            this.listBoxMD5.FormattingEnabled = true;
            this.listBoxMD5.Location = new System.Drawing.Point(6, 136);
            this.listBoxMD5.Name = "listBoxMD5";
            this.listBoxMD5.Size = new System.Drawing.Size(332, 121);
            this.listBoxMD5.TabIndex = 4;
            // 
            // txtMD5Source
            // 
            this.txtMD5Source.BackColor = System.Drawing.Color.Black;
            this.txtMD5Source.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMD5Source.ForeColor = System.Drawing.Color.White;
            this.txtMD5Source.Location = new System.Drawing.Point(3, 287);
            this.txtMD5Source.Name = "txtMD5Source";
            this.txtMD5Source.Size = new System.Drawing.Size(335, 20);
            this.txtMD5Source.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 271);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "MD5 hash list file:";
            // 
            // _openFileDialog
            // 
            this._openFileDialog.FileName = "ModuleRegistor.Resources.dll";
            this._openFileDialog.Filter = "All Files|*.*|MD5 Listl Files|*.txt";
            // 
            // btnBrowseMD5List
            // 
            this.btnBrowseMD5List.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseMD5List.Image")));
            this.btnBrowseMD5List.Location = new System.Drawing.Point(361, 282);
            this.btnBrowseMD5List.Name = "btnBrowseMD5List";
            this.btnBrowseMD5List.Size = new System.Drawing.Size(26, 26);
            this.btnBrowseMD5List.TabIndex = 8;
            this.btnBrowseMD5List.UseVisualStyleBackColor = true;
            this.btnBrowseMD5List.Click += new System.EventHandler(this.OnBrowseMD5ListClick);
            // 
            // treeViewFolderExplorer
            // 
            this.treeViewFolderExplorer.BackColor = System.Drawing.Color.Black;
            this.treeViewFolderExplorer.ForeColor = System.Drawing.Color.Yellow;
            this.treeViewFolderExplorer.LineColor = System.Drawing.Color.Gainsboro;
            this.treeViewFolderExplorer.Location = new System.Drawing.Point(6, -4);
            this.treeViewFolderExplorer.Name = "treeViewFolderExplorer";
            this.treeViewFolderExplorer.Size = new System.Drawing.Size(332, 134);
            this.treeViewFolderExplorer.TabIndex = 9;
            this.treeViewFolderExplorer.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnNodeMouseClick);
            // 
            // btnDownload
            // 
            this.btnDownload.Enabled = false;
            this.btnDownload.Location = new System.Drawing.Point(560, 281);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 11;
            this.btnDownload.Text = "&Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.OnDownloadClick);
            // 
            // listViewFoder
            // 
            this.listViewFoder.BackColor = System.Drawing.Color.DimGray;
            this.listViewFoder.ForeColor = System.Drawing.Color.White;
            this.listViewFoder.FullRowSelect = true;
            this.listViewFoder.Location = new System.Drawing.Point(361, 3);
            this.listViewFoder.Name = "listViewFoder";
            this.listViewFoder.Size = new System.Drawing.Size(287, 265);
            this.listViewFoder.TabIndex = 13;
            this.listViewFoder.UseCompatibleStateImageBehavior = false;
            this.listViewFoder.View = System.Windows.Forms.View.List;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 2000;
            this.timer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // DownloadSampleDemoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.listViewFoder);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.treeViewFolderExplorer);
            this.Controls.Add(this.btnBrowseMD5List);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMD5Source);
            this.Controls.Add(this.listBoxMD5);
            this.Name = "DownloadSampleDemoControl";
            this.Size = new System.Drawing.Size(651, 310);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxMD5;
        private System.Windows.Forms.TextBox txtMD5Source;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog _openFileDialog;
        private System.Windows.Forms.Button btnBrowseMD5List;
        private System.Windows.Forms.TreeView treeViewFolderExplorer;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ListView listViewFoder;
        private System.Windows.Forms.Timer timer;
    }
}
