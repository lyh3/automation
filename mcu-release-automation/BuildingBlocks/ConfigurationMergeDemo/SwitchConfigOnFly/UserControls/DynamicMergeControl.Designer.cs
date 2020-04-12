namespace SwitchConfigOnFly
{
    partial class DynamicMergeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DynamicMergeControl));
            this.xmlTreeViewTarget = new McAfeeLabs.Engineering.Automation.Profile.XmlControls.XmlTreeView();
            this.btnMerge = new System.Windows.Forms.Button();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.txtMergeScript = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMergeTarget = new System.Windows.Forms.TextBox();
            this.btnBrowseMergeScript = new System.Windows.Forms.Button();
            this.btnBrowseMergeTarget = new System.Windows.Forms.Button();
            this._saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.btnSave = new System.Windows.Forms.Button();
            this.mergeScriptSourceTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // xmlTreeViewTarget
            // 
            this.xmlTreeViewTarget.BackColor = System.Drawing.Color.Black;
            this.xmlTreeViewTarget.ForeColor = System.Drawing.Color.DodgerBlue;
            this.xmlTreeViewTarget.Location = new System.Drawing.Point(358, 6);
            this.xmlTreeViewTarget.Name = "xmlTreeViewTarget";
            this.xmlTreeViewTarget.Size = new System.Drawing.Size(290, 270);
            this.xmlTreeViewTarget.TabIndex = 2;
            this.xmlTreeViewTarget.Xml = "";
            this.xmlTreeViewTarget.XmlFile = "";
            // 
            // btnMerge
            // 
            this.btnMerge.Image = ((System.Drawing.Image)(resources.GetObject("btnMerge.Image")));
            this.btnMerge.Location = new System.Drawing.Point(297, 78);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(57, 49);
            this.btnMerge.TabIndex = 3;
            this.btnMerge.UseVisualStyleBackColor = true;
            this.btnMerge.Click += new System.EventHandler(this.OnMergeClick);
            // 
            // _openFileDialog
            // 
            this._openFileDialog.FileName = "ModuleRegistor.Resources.dll";
            this._openFileDialog.Filter = "All Files|*.*|Resource dlls|*.dll|Config Files|*.config|Xml Files|*.xml";
            // 
            // txtMergeScript
            // 
            this.txtMergeScript.BackColor = System.Drawing.Color.Black;
            this.txtMergeScript.ForeColor = System.Drawing.Color.DodgerBlue;
            this.txtMergeScript.Location = new System.Drawing.Point(73, 285);
            this.txtMergeScript.Name = "txtMergeScript";
            this.txtMergeScript.Size = new System.Drawing.Size(218, 20);
            this.txtMergeScript.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 288);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Merge Script:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(355, 288);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Target:";
            // 
            // txtMergeTarget
            // 
            this.txtMergeTarget.BackColor = System.Drawing.Color.Black;
            this.txtMergeTarget.ForeColor = System.Drawing.Color.DodgerBlue;
            this.txtMergeTarget.Location = new System.Drawing.Point(399, 285);
            this.txtMergeTarget.Name = "txtMergeTarget";
            this.txtMergeTarget.Size = new System.Drawing.Size(218, 20);
            this.txtMergeTarget.TabIndex = 11;
            // 
            // btnBrowseMergeScript
            // 
            this.btnBrowseMergeScript.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseMergeScript.Image")));
            this.btnBrowseMergeScript.Location = new System.Drawing.Point(299, 280);
            this.btnBrowseMergeScript.Name = "btnBrowseMergeScript";
            this.btnBrowseMergeScript.Size = new System.Drawing.Size(26, 26);
            this.btnBrowseMergeScript.TabIndex = 12;
            this.btnBrowseMergeScript.UseVisualStyleBackColor = true;
            this.btnBrowseMergeScript.Click += new System.EventHandler(this.OnBrowseMergeScriptClick);
            // 
            // btnBrowseMergeTarget
            // 
            this.btnBrowseMergeTarget.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseMergeTarget.Image")));
            this.btnBrowseMergeTarget.Location = new System.Drawing.Point(621, 280);
            this.btnBrowseMergeTarget.Name = "btnBrowseMergeTarget";
            this.btnBrowseMergeTarget.Size = new System.Drawing.Size(26, 26);
            this.btnBrowseMergeTarget.TabIndex = 13;
            this.btnBrowseMergeTarget.UseVisualStyleBackColor = true;
            this.btnBrowseMergeTarget.Click += new System.EventHandler(this.OnBrowseMergeTargetClick);
            // 
            // _saveFileDialog
            // 
            this._saveFileDialog.Filter = "All Files|*.*|Xml Files|*.xml";
            // 
            // btnSave
            // 
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(297, 189);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(57, 41);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "&Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.OnSaveClick);
            // 
            // mergeScriptSourceTextBox
            // 
            this.mergeScriptSourceTextBox.BackColor = System.Drawing.Color.Black;
            this.mergeScriptSourceTextBox.ForeColor = System.Drawing.Color.Gold;
            this.mergeScriptSourceTextBox.Location = new System.Drawing.Point(4, 6);
            this.mergeScriptSourceTextBox.Multiline = true;
            this.mergeScriptSourceTextBox.Name = "mergeScriptSourceTextBox";
            this.mergeScriptSourceTextBox.Size = new System.Drawing.Size(287, 270);
            this.mergeScriptSourceTextBox.TabIndex = 15;
            // 
            // DynamicMergeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.mergeScriptSourceTextBox);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnBrowseMergeTarget);
            this.Controls.Add(this.btnBrowseMergeScript);
            this.Controls.Add(this.txtMergeTarget);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMergeScript);
            this.Controls.Add(this.btnMerge);
            this.Controls.Add(this.xmlTreeViewTarget);
            this.Name = "DynamicMergeControl";
            this.Size = new System.Drawing.Size(651, 310);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private McAfeeLabs.Engineering.Automation.Profile.XmlControls.XmlTreeView xmlTreeViewTarget;
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.OpenFileDialog _openFileDialog;
        private System.Windows.Forms.TextBox txtMergeScript;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMergeTarget;
        private System.Windows.Forms.Button btnBrowseMergeScript;
        private System.Windows.Forms.Button btnBrowseMergeTarget;
        private System.Windows.Forms.SaveFileDialog _saveFileDialog;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox mergeScriptSourceTextBox;
    }
}
