using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Web.Script.Serialization;
using System.Diagnostics;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;

using ConfigEditor.DataModel;
namespace ConfigEditor.Controls
{
    public partial class BiosEditorControl : UserControl
    {
        private string _selectedJsonFile = string.Empty;
        private frmConfigEditor _frmConfigEditor = null;
        private BiosItemControl _selectedBiosItemControl = null;
        ContextMenu _biosEditContextmenu = null;
        private BIOSConfigEdit _biosConfigEdit = null;
        private string _selectedKey = null;
        public BiosEditorControl()
        {
            InitializeComponent();
        }
        public BiosEditorControl(frmConfigEditor parentForm)
        {
            InitializeComponent();
            this._frmConfigEditor = parentForm;
            _biosEditContextmenu = new ContextMenu();
            var addBiosConfigCommand = new MenuItem("Add");
            addBiosConfigCommand.Click += AddBiosCommand_Click;
            _biosEditContextmenu.MenuItems.Add(addBiosConfigCommand);
            var deleteBiosConfiglCommand = new MenuItem("Delete");
            deleteBiosConfiglCommand.Click += DeleteBiosConfiglCommand_Click;
            _biosEditContextmenu.MenuItems.Add(deleteBiosConfiglCommand);

            scrollableControl_biosConfig.ContextMenu = _biosEditContextmenu;
        }

        public frmConfigEditor ConfigEditorForm { get { return _frmConfigEditor; } }
        public void SelectBiosItem(string key)
        {
            this._selectedKey = key;
            panelBiosItem.Controls.Clear();
            foreach (BiosConfigControl biosconfigControl in this.scrollableControl_biosConfig.Controls)
            {
                if (biosconfigControl.BiosConfig == key)
                {
                    biosconfigControl.BackColor = Color.DimGray;
                    _selectedBiosItemControl = new BiosItemControl(key, _biosConfigEdit.BIOConfigDictionary[key]);
                    panelBiosItem.Controls.Add(_selectedBiosItemControl);
                    _selectedBiosItemControl.Dock = DockStyle.Fill;
                }
                else { biosconfigControl.BackColor = Color.Transparent; }
            }
        }
        private void DeleteBiosConfiglCommand_Click(object sender, EventArgs e)
        {
        }

        private void AddBiosCommand_Click(object sender, EventArgs e)
        {
        }

        private void onOpenClick(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this._reset();
                _selectedJsonFile = openFileDialog.FileName;
                this.lblSelectedBiosJson.Text = _selectedJsonFile;
                try
                {
                    var json = File.ReadAllText(_selectedJsonFile);
                    var javaScriptSer = new JavaScriptSerializer();
                    this._biosConfigEdit = javaScriptSer.Deserialize<BIOSConfigEdit>(json);

                    var dic = _biosConfigEdit.BIOConfigDictionary;
                    foreach(KeyValuePair<string, dynamic> item in javaScriptSer.Deserialize<dynamic>(json))
                    {
                        foreach(KeyValuePair<string, dynamic> n in item.Value)
                        {
                            var biosItemList = new List<BIOSItem>();
                            foreach (var b in n.Value)
                            {
                                var attrList = new List<string>();
                                foreach (KeyValuePair<string, dynamic> x in b)
                                {
                                    attrList.Add((string)x.Value);
                                }
                                var biosItem = new BIOSItem()
                                {
                                    to = attrList[0],
                                    navigation = attrList[1],
                                    recuring = attrList[2],
                                    invoke = attrList[3]
                                };
                                biosItemList.Add(biosItem);
                            }
                            if (biosItemList.Count > 0 && !_biosConfigEdit.BIOConfigDictionary.ContainsKey(item.Key))
                            {
                                var biosConfig = new BIOSConfig() { navs = biosItemList.ToArray() };
                                dic.Add(item.Key, biosConfig);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _frmConfigEditor.ShowMessage(ex.Message, true);
                }
            }
            this._updateView();
        }
        private void _reset()
        {
            this.scrollableControl_biosConfig.Controls.Clear();
        }
        private void _updateView()
        {
            this.scrollableControl_biosConfig.Controls.Clear();
            var controlList = new List<BiosConfigControl>();
            var itr = _biosConfigEdit.BIOConfigDictionary.GetEnumerator();
            while(itr.MoveNext())
            {
                var biosConfigCotrol = new BiosConfigControl(itr.Current.Key, this);
                controlList.Insert(0, biosConfigCotrol);
            }
            foreach(var control in controlList)
            {
                scrollableControl_biosConfig.Controls.Add(control);
                control.Dock = DockStyle.Top;
            }
            this.textIP.Text = _biosConfigEdit.Client.ip;
            this.textUser.Text = _biosConfigEdit.Client.user;
            this.textPassword.Text = _biosConfigEdit.Client.password;
            this.textPort.Text = _biosConfigEdit.serial.port;
            this.textBaud.Text = _biosConfigEdit.serial.baud;
            this.textTimeout.Text = _biosConfigEdit.timeout;
        }

        private void scrollableControl1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        _biosEditContextmenu.Show(this, new Point(scrollableControl_biosConfig.Right, scrollableControl_biosConfig.Bottom));
                    }
                    break;
            }
        }

        private void onSaveClick(object sender, EventArgs e)
        {
            try
            {
                DoDataExchange();
                var json = _biosConfigEdit.ToString();
                File.WriteAllText(_selectedJsonFile, json);
                //_isDirty = false;
                _frmConfigEditor.ShowMessage(string.Format("Save [{0}] success.", _selectedJsonFile));
            }
            catch (IOException ex)
            {
                _frmConfigEditor.ShowMessage(ex.Message, true);
            }
        }

        private void onSaveAsClick(object sender, EventArgs e)
        {
            try
            {
                var result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    DoDataExchange();
                    this._selectedJsonFile = saveFileDialog.FileName;
                    lblSelectedBiosJson.Text = _selectedJsonFile;
                    var json = _biosConfigEdit.ToString();
                    File.WriteAllText(saveFileDialog.FileName, json);
                    //_isDirty = false;
                    _frmConfigEditor.ShowMessage(string.Format("Save [{0}] success.", _selectedJsonFile));
                }
            }
            catch (IOException ex)
            {
                _frmConfigEditor.ShowMessage(ex.Message, true);
            }
        }

        private void DoDataExchange()
        {
            _biosConfigEdit.Client.ip = this.textIP.Text.Trim();
            _biosConfigEdit.Client.user = this.textUser.Text.Trim();
            _biosConfigEdit.Client.password = this.textPassword.Text.Trim();
            _biosConfigEdit.serial.port = this.textPort.Text.Trim();
            _biosConfigEdit.serial.baud = this.textBaud.Text.Trim();
            _biosConfigEdit.timeout = this.textTimeout.Text.Trim();
            foreach (BiosConfigControl biosConfigControl in scrollableControl_biosConfig.Controls)
            {
                biosConfigControl.DoDataExchange();
            }
        }

        private void onRunBiosSettingsClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this._selectedJsonFile)
                ||string.IsNullOrEmpty(_selectedKey))
                return;
            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = textBoxToolPath.Text.Trim();
                startInfo.FileName = "python";
                var args = "  biosSettings.py"
                            + string.Format(" -j {0} -s {2}{1}{2}",
                             this._selectedJsonFile,
                             this._selectedKey, 
                            '"');
                startInfo.Arguments = args;
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                _frmConfigEditor.ShowMessage(ex.Message, true);
            }
        }

        private void onShowPasswordCheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowPassword.Checked)
                textPassword.PasswordChar = new char();
            else
                textPassword.PasswordChar = '*';
        }
    }
}
