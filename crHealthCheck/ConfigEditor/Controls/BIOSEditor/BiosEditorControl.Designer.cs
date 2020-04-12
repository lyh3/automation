namespace ConfigEditor.Controls
{
    partial class BiosEditorControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BiosEditorControl));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolPathTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxToolPath = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxRebootSUT = new System.Windows.Forms.CheckBox();
            this.pictureBoxRun = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanelList = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelSelectedBiosJson = new System.Windows.Forms.TableLayoutPanel();
            this.lblSelectedBiosJson = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.scrollableControl_biosConfig = new System.Windows.Forms.ScrollableControl();
            this.panelBiosItem = new System.Windows.Forms.Panel();
            this.tableLayoutPanelClient = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelPassword = new System.Windows.Forms.TableLayoutPanel();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.checkBoxShowPassword = new System.Windows.Forms.CheckBox();
            this.textBaud = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textPort = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textTimeout = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.menuStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolPathTableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRun)).BeginInit();
            this.tableLayoutPanelList.SuspendLayout();
            this.tableLayoutPanelSelectedBiosJson.SuspendLayout();
            this.tableLayoutPanelClient.SuspendLayout();
            this.tableLayoutPanelPassword.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "*.json";
            this.openFileDialog.Filter = "json files (*.json)|*.txt|All files (*.*)|*.*";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "json files (*.json)|*.txt|All files (*.*)|*.*";
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
            this.menuStrip.Size = new System.Drawing.Size(1081, 28);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.NewConfig,
            this.saveAsToolStripMenuItem,
            this.saveAsToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(131, 26);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.onOpenClick);
            // 
            // NewConfig
            // 
            this.NewConfig.Name = "NewConfig";
            this.NewConfig.Size = new System.Drawing.Size(131, 26);
            this.NewConfig.Text = "&New";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(131, 26);
            this.saveAsToolStripMenuItem.Text = "&Save";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.onSaveClick);
            // 
            // saveAsToolStripMenuItem1
            // 
            this.saveAsToolStripMenuItem1.Name = "saveAsToolStripMenuItem1";
            this.saveAsToolStripMenuItem1.Size = new System.Drawing.Size(131, 26);
            this.saveAsToolStripMenuItem1.Text = "Save&As";
            this.saveAsToolStripMenuItem1.Click += new System.EventHandler(this.onSaveAsClick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.57289F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.42711F));
            this.tableLayoutPanel1.Controls.Add(this.toolPathTableLayoutPanel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelList, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelBiosItem, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelClient, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 28);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 11F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1081, 657);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // toolPathTableLayoutPanel
            // 
            this.toolPathTableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel1.SetColumnSpan(this.toolPathTableLayoutPanel, 2);
            this.toolPathTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.97674F));
            this.toolPathTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 89.02325F));
            this.toolPathTableLayoutPanel.Controls.Add(this.textBoxToolPath, 1, 0);
            this.toolPathTableLayoutPanel.Controls.Add(this.label9, 0, 0);
            this.toolPathTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolPathTableLayoutPanel.Location = new System.Drawing.Point(3, 619);
            this.toolPathTableLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.toolPathTableLayoutPanel.Name = "toolPathTableLayoutPanel";
            this.toolPathTableLayoutPanel.RowCount = 1;
            this.toolPathTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolPathTableLayoutPanel.Size = new System.Drawing.Size(1075, 25);
            this.toolPathTableLayoutPanel.TabIndex = 8;
            // 
            // textBoxToolPath
            // 
            this.textBoxToolPath.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.textBoxToolPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxToolPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxToolPath.ForeColor = System.Drawing.Color.DarkGray;
            this.textBoxToolPath.Location = new System.Drawing.Point(120, 2);
            this.textBoxToolPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxToolPath.Name = "textBoxToolPath";
            this.textBoxToolPath.Size = new System.Drawing.Size(952, 22);
            this.textBoxToolPath.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Gray;
            this.label9.Location = new System.Drawing.Point(3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(111, 25);
            this.label9.TabIndex = 0;
            this.label9.Text = "ToolPath";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.91753F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.08247F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 127F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tableLayoutPanel2.Controls.Add(this.checkBoxRebootSUT, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.pictureBoxRun, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 546);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(389, 68);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // checkBoxRebootSUT
            // 
            this.checkBoxRebootSUT.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxRebootSUT.AutoSize = true;
            this.checkBoxRebootSUT.ForeColor = System.Drawing.Color.DarkGray;
            this.checkBoxRebootSUT.Location = new System.Drawing.Point(188, 9);
            this.checkBoxRebootSUT.Name = "checkBoxRebootSUT";
            this.checkBoxRebootSUT.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxRebootSUT.Size = new System.Drawing.Size(121, 48);
            this.checkBoxRebootSUT.TabIndex = 2;
            this.checkBoxRebootSUT.Text = "Reboot SUT";
            this.checkBoxRebootSUT.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.checkBoxRebootSUT.UseVisualStyleBackColor = true;
            // 
            // pictureBoxRun
            // 
            this.pictureBoxRun.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxRun.BackgroundImage")));
            this.pictureBoxRun.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBoxRun.Location = new System.Drawing.Point(121, 8);
            this.pictureBoxRun.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBoxRun.Name = "pictureBoxRun";
            this.pictureBoxRun.Size = new System.Drawing.Size(56, 48);
            this.pictureBoxRun.TabIndex = 1;
            this.pictureBoxRun.TabStop = false;
            this.pictureBoxRun.Click += new System.EventHandler(this.onRunBiosSettingsClick);
            // 
            // tableLayoutPanelList
            // 
            this.tableLayoutPanelList.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanelList.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanelList.ColumnCount = 1;
            this.tableLayoutPanelList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelList.Controls.Add(this.tableLayoutPanelSelectedBiosJson, 0, 0);
            this.tableLayoutPanelList.Controls.Add(this.scrollableControl_biosConfig, 0, 1);
            this.tableLayoutPanelList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelList.ForeColor = System.Drawing.Color.White;
            this.tableLayoutPanelList.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelList.Name = "tableLayoutPanelList";
            this.tableLayoutPanelList.RowCount = 2;
            this.tableLayoutPanelList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.219662F));
            this.tableLayoutPanelList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 92.78033F));
            this.tableLayoutPanelList.Size = new System.Drawing.Size(389, 537);
            this.tableLayoutPanelList.TabIndex = 2;
            // 
            // tableLayoutPanelSelectedBiosJson
            // 
            this.tableLayoutPanelSelectedBiosJson.ColumnCount = 2;
            this.tableLayoutPanelSelectedBiosJson.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.8F));
            this.tableLayoutPanelSelectedBiosJson.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.2F));
            this.tableLayoutPanelSelectedBiosJson.Controls.Add(this.lblSelectedBiosJson, 1, 0);
            this.tableLayoutPanelSelectedBiosJson.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanelSelectedBiosJson.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSelectedBiosJson.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanelSelectedBiosJson.Name = "tableLayoutPanelSelectedBiosJson";
            this.tableLayoutPanelSelectedBiosJson.RowCount = 1;
            this.tableLayoutPanelSelectedBiosJson.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelSelectedBiosJson.Size = new System.Drawing.Size(381, 32);
            this.tableLayoutPanelSelectedBiosJson.TabIndex = 4;
            // 
            // lblSelectedBiosJson
            // 
            this.lblSelectedBiosJson.AutoSize = true;
            this.lblSelectedBiosJson.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSelectedBiosJson.ForeColor = System.Drawing.Color.White;
            this.lblSelectedBiosJson.Location = new System.Drawing.Point(112, 0);
            this.lblSelectedBiosJson.Name = "lblSelectedBiosJson";
            this.lblSelectedBiosJson.Size = new System.Drawing.Size(266, 32);
            this.lblSelectedBiosJson.TabIndex = 1;
            this.lblSelectedBiosJson.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Silver;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "BIOS Json:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // scrollableControl_biosConfig
            // 
            this.scrollableControl_biosConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.scrollableControl_biosConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollableControl_biosConfig.Location = new System.Drawing.Point(4, 43);
            this.scrollableControl_biosConfig.Name = "scrollableControl_biosConfig";
            this.scrollableControl_biosConfig.Size = new System.Drawing.Size(381, 490);
            this.scrollableControl_biosConfig.TabIndex = 5;
            this.scrollableControl_biosConfig.Text = "scrollableControl";
            this.scrollableControl_biosConfig.MouseDown += new System.Windows.Forms.MouseEventHandler(this.scrollableControl1_MouseDown);
            // 
            // panelBiosItem
            // 
            this.panelBiosItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBiosItem.Location = new System.Drawing.Point(398, 3);
            this.panelBiosItem.Name = "panelBiosItem";
            this.panelBiosItem.Size = new System.Drawing.Size(680, 537);
            this.panelBiosItem.TabIndex = 3;
            // 
            // tableLayoutPanelClient
            // 
            this.tableLayoutPanelClient.ColumnCount = 8;
            this.tableLayoutPanelClient.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.87129F));
            this.tableLayoutPanelClient.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 87.12872F));
            this.tableLayoutPanelClient.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 147F));
            this.tableLayoutPanelClient.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanelClient.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 164F));
            this.tableLayoutPanelClient.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 71F));
            this.tableLayoutPanelClient.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 157F));
            this.tableLayoutPanelClient.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanelClient.Controls.Add(this.tableLayoutPanelPassword, 6, 0);
            this.tableLayoutPanelClient.Controls.Add(this.textBaud, 4, 1);
            this.tableLayoutPanelClient.Controls.Add(this.label12, 3, 1);
            this.tableLayoutPanelClient.Controls.Add(this.textPort, 2, 1);
            this.tableLayoutPanelClient.Controls.Add(this.label11, 1, 1);
            this.tableLayoutPanelClient.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanelClient.Controls.Add(this.textIP, 2, 0);
            this.tableLayoutPanelClient.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanelClient.Controls.Add(this.textUser, 4, 0);
            this.tableLayoutPanelClient.Controls.Add(this.label4, 5, 0);
            this.tableLayoutPanelClient.Controls.Add(this.textTimeout, 6, 1);
            this.tableLayoutPanelClient.Controls.Add(this.label5, 5, 1);
            this.tableLayoutPanelClient.Location = new System.Drawing.Point(398, 546);
            this.tableLayoutPanelClient.Name = "tableLayoutPanelClient";
            this.tableLayoutPanelClient.RowCount = 2;
            this.tableLayoutPanelClient.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.85714F));
            this.tableLayoutPanelClient.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.14286F));
            this.tableLayoutPanelClient.Size = new System.Drawing.Size(680, 68);
            this.tableLayoutPanelClient.TabIndex = 5;
            // 
            // tableLayoutPanelPassword
            // 
            this.tableLayoutPanelPassword.ColumnCount = 2;
            this.tableLayoutPanelClient.SetColumnSpan(this.tableLayoutPanelPassword, 2);
            this.tableLayoutPanelPassword.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 86.44068F));
            this.tableLayoutPanelPassword.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.55932F));
            this.tableLayoutPanelPassword.Controls.Add(this.textPassword, 0, 0);
            this.tableLayoutPanelPassword.Controls.Add(this.checkBoxShowPassword, 1, 0);
            this.tableLayoutPanelPassword.Location = new System.Drawing.Point(493, 3);
            this.tableLayoutPanelPassword.Name = "tableLayoutPanelPassword";
            this.tableLayoutPanelPassword.RowCount = 1;
            this.tableLayoutPanelPassword.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelPassword.Size = new System.Drawing.Size(177, 29);
            this.tableLayoutPanelPassword.TabIndex = 0;
            // 
            // textPassword
            // 
            this.textPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.textPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textPassword.ForeColor = System.Drawing.Color.DodgerBlue;
            this.textPassword.Location = new System.Drawing.Point(3, 2);
            this.textPassword.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '*';
            this.textPassword.Size = new System.Drawing.Size(147, 22);
            this.textPassword.TabIndex = 6;
            this.textPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkBoxShowPassword
            // 
            this.checkBoxShowPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxShowPassword.AutoSize = true;
            this.checkBoxShowPassword.ForeColor = System.Drawing.Color.Gray;
            this.checkBoxShowPassword.Location = new System.Drawing.Point(156, 2);
            this.checkBoxShowPassword.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBoxShowPassword.Name = "checkBoxShowPassword";
            this.checkBoxShowPassword.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxShowPassword.Size = new System.Drawing.Size(18, 25);
            this.checkBoxShowPassword.TabIndex = 1;
            this.checkBoxShowPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxShowPassword.UseVisualStyleBackColor = true;
            this.checkBoxShowPassword.CheckedChanged += new System.EventHandler(this.onShowPasswordCheckedChanged);
            // 
            // textBaud
            // 
            this.textBaud.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBaud.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.textBaud.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBaud.ForeColor = System.Drawing.Color.DodgerBlue;
            this.textBaud.Location = new System.Drawing.Point(258, 37);
            this.textBaud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBaud.Name = "textBaud";
            this.textBaud.Size = new System.Drawing.Size(158, 22);
            this.textBaud.TabIndex = 4;
            this.textBaud.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.DimGray;
            this.label12.Location = new System.Drawing.Point(198, 35);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 33);
            this.label12.TabIndex = 4;
            this.label12.Text = "Baud : ";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textPort
            // 
            this.textPort.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.textPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textPort.ForeColor = System.Drawing.Color.DodgerBlue;
            this.textPort.Location = new System.Drawing.Point(51, 37);
            this.textPort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(141, 22);
            this.textPort.TabIndex = 4;
            this.textPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.DimGray;
            this.label11.Location = new System.Drawing.Point(9, 35);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(36, 33);
            this.label11.TabIndex = 4;
            this.label11.Text = "Port : ";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(9, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 35);
            this.label2.TabIndex = 1;
            this.label2.Text = "IP : ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textIP
            // 
            this.textIP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textIP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.textIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textIP.ForeColor = System.Drawing.Color.DodgerBlue;
            this.textIP.Location = new System.Drawing.Point(51, 2);
            this.textIP.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textIP.Name = "textIP";
            this.textIP.Size = new System.Drawing.Size(141, 22);
            this.textIP.TabIndex = 2;
            this.textIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(198, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 35);
            this.label3.TabIndex = 3;
            this.label3.Text = "user : ";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textUser
            // 
            this.textUser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.textUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textUser.ForeColor = System.Drawing.Color.DodgerBlue;
            this.textUser.Location = new System.Drawing.Point(258, 2);
            this.textUser.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textUser.Name = "textUser";
            this.textUser.Size = new System.Drawing.Size(158, 22);
            this.textUser.TabIndex = 5;
            this.textUser.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(422, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 35);
            this.label4.TabIndex = 4;
            this.label4.Text = "pass word : ";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textTimeout
            // 
            this.textTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textTimeout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.textTimeout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textTimeout.ForeColor = System.Drawing.Color.DodgerBlue;
            this.textTimeout.Location = new System.Drawing.Point(493, 37);
            this.textTimeout.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textTimeout.Name = "textTimeout";
            this.textTimeout.Size = new System.Drawing.Size(151, 22);
            this.textTimeout.TabIndex = 3;
            this.textTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.Location = new System.Drawing.Point(422, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 33);
            this.label5.TabIndex = 6;
            this.label5.Text = "timeout (sec) : ";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BiosEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip);
            this.Name = "BiosEditorControl";
            this.Size = new System.Drawing.Size(1081, 685);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.toolPathTableLayoutPanel.ResumeLayout(false);
            this.toolPathTableLayoutPanel.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRun)).EndInit();
            this.tableLayoutPanelList.ResumeLayout(false);
            this.tableLayoutPanelSelectedBiosJson.ResumeLayout(false);
            this.tableLayoutPanelSelectedBiosJson.PerformLayout();
            this.tableLayoutPanelClient.ResumeLayout(false);
            this.tableLayoutPanelClient.PerformLayout();
            this.tableLayoutPanelPassword.ResumeLayout(false);
            this.tableLayoutPanelPassword.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NewConfig;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSelectedBiosJson;
        private System.Windows.Forms.Label lblSelectedBiosJson;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ScrollableControl scrollableControl_biosConfig;
        private System.Windows.Forms.Panel panelBiosItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox pictureBoxRun;
        private System.Windows.Forms.CheckBox checkBoxRebootSUT;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelClient;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelPassword;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textTimeout;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textPort;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBaud;
        private System.Windows.Forms.CheckBox checkBoxShowPassword;
        private System.Windows.Forms.TableLayoutPanel toolPathTableLayoutPanel;
        private System.Windows.Forms.TextBox textBoxToolPath;
        private System.Windows.Forms.Label label9;
    }
}
