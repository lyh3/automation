using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;
using ConfigEditor.DataModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO.Ports;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ConfigEditor.Controls
{
    public partial class TestEditorControl : UserControl
    {
        private const string CombinedIPV4IPV6Pattern = @"(?=(^\s*(https?:(\/\/))?(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4})::([0-9a-fA-F]{4})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{3}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{1}):([0-9a-fA-F]{3}):([0-9a-fA-F]{4}):([0-9a-fA-F]{1}):([0-9a-fA-F]{1}):([0-9a-fA-F]{1})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{3}):([0-9a-fA-F]{3}):([0-9a-fA-F]{3}):([0-9a-fA-F]{4}):([0-9a-fA-F]{1}):([0-9a-fA-F]{1}):([0-9a-fA-F]{3})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{3}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{1})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{3}):([0-9a-fA-F]{3}):([0-9a-fA-F]{3}):([0-9a-fA-F]{4}):([0-9a-fA-F]{1}):([0-9a-fA-F]{1}):([0-9a-fA-F]{2})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4})])(:\d{4})?\s*$)|(^\s*(((https?(?![0-9][a-zA-Z]):)?(//)((w{3}?).)?)?)([\w-]+\.)+[\w-]+(\/[\w- ./?%&amp;=]*)?\s*$))";
        private bool _allowToRun = true;
        private frmConfigEditor _parentForm = null;
        private bool _isDirty = false;
        public Color InputBackColor = Color.Olive;
        public Color ErrorBackColor = Color.Pink;

        #region Constructors
        public TestEditorControl()
        {
            InitializeComponent();
            this.Size = new Size(1081, 685);
            InputBackColor = textIP.BackColor;
            onShowPasswordCheckedChanged(this, null);
            #if !DEBUG
            textBoxToolPath.Text = @".\";
            #endif
            _reset();
        }
        public TestEditorControl(frmConfigEditor parentForm)
        {
            InitializeComponent();
            this._parentForm = parentForm;
            InputBackColor = textIP.BackColor;
            onShowPasswordCheckedChanged(this, null);
            #if !DEBUG
            textBoxToolPath.Text = @".\";
            #endif
            _reset();
        }
        #endregion

        #region Properties
        public string SelectedJsonFile { get; set; }
        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }
        public frmConfigEditor FrmConfigEditor { get { return this._parentForm; } }
        public TreeNode SelectedGroupNode { get; set; }
        public TreeNode SelectedTestNode { get; set; }
        private TreeNode _rootNode { get; set; }
        private TestControl _testControl { get; set; }
        #endregion

        #region Public Methods
        public void AllowToRun(bool allow)
        {
            if (_isUnknownGroupSelected())
                allow = false;
            pictureBoxRun.Enabled = allow;
            _loadImageRun(allow);
            _allowToRun = allow;
        }
        #endregion

        #region Private Methods
        private bool _isUnknownGroupSelected()
        {
            if (null != SelectedGroupNode && SelectedGroupNode.Text == TestGroup.UNKNOWN_GROUP)
            {
                _parentForm.ShowMessage(string.Format("Please rename the group [{0}].", TestGroup.UNKNOWN_GROUP), true);
                return true;
            }

            return false;
        }
        private bool _validate()
        {
            _parentForm.ShowMessage("");
            var errorMsg = new StringBuilder();
            var regEx = new Regex(CombinedIPV4IPV6Pattern,
                    RegexOptions.IgnoreCase
                   | RegexOptions.CultureInvariant
                   | RegexOptions.IgnorePatternWhitespace
                   | RegexOptions.Compiled);
            var ip = textIP.Text.Trim();
            Match match = regEx.Match(ip);
            if (!match.Success)
            {
                errorMsg.Append(string.Format("The IP address [{0}] is invalid.", ip));
            }
            var needInput = string.Empty;
            TextBox[] requiredInputBoxes = {  textTimeout,
                                              textRecuring,
                                              textReportFile,
                                              textIP,
                                              textUser,
                                              textPassword};
            foreach (var t in requiredInputBoxes)
            {
                t.BackColor = InputBackColor;
                if (string.IsNullOrEmpty(t.Text.Trim()))
                {
                    t.BackColor = ErrorBackColor;
                    needInput = string.Format("{0} {1},", needInput, t.Name);
                }
            }
            var needDigits = string.Empty;
            TextBox[] requireDigitsBoxes = { textTimeout, textRecuring, textBaud };
            foreach (var d in requireDigitsBoxes)
            {
                var text = d.Text.Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    foreach (char c in text)
                    {
                        if (!Char.IsDigit(c))
                        {
                            needDigits = string.Format("{0} {1},", needDigits, d.Name);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(needInput))
            {
                errorMsg.Append(string.Format("{0} need input.", needInput.TrimEnd(',')));
            }
            if (!string.IsNullOrEmpty(needDigits))
            {
                errorMsg.Append(string.Format("[{0}] must be digits.", needDigits.TrimEnd(',')));
            }
            var s = errorMsg.ToString();
            if (!string.IsNullOrEmpty(s))
            {
                _parentForm.ShowMessage(s, true);
                return false;
            }
            return true;
        }
        private void _loadImageRun(bool allow)
        {
            #if DEBUG
            var file = @"C:\PythonSV\crHealthCheck\ConfigEditor\images";
            #else
            var file = @".\ConfigEditor\images";
        #endif
            if (allow)
                file = string.Format(@"{0}\{1}", file, "ArrowRight_Blue.png");
            else
                file = string.Format(@"{0}\{1}", file, "ArrowRight_White.png");
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                pictureBoxRun.BackgroundImage = Image.FromStream(stream);
            }
        }
        private void _reset()
        {
            _parentForm.ShowMessage("");
            SelectedGroupNode = null;
            SelectedTestNode = null;
            addDeleteTableLayoutPanel.Visible = false;
            testTreeView.Visible = false;
            splitContainer1.Visible = false;
            // runTableLayoutPanel.Visible = false;
            pictureBoxRun.Visible = false;
            lblRunTest.Visible = false;
            toolPathTableLayoutPanel.Visible = false;
            this.lblSelectedJson.Text = string.Empty;
            var testControl = new TestControl();
            splitContainer1.SplitterDistance = testControl.Height;
            splitContainer1.Panel1.Controls.Clear();
            _isDirty = false;
        }
        private void onImgFindClick(object sender, EventArgs e)
        {
            _parentForm.ShowMessage("");
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this._reset();
                SelectedJsonFile = openFileDialog.FileName;
                try
                {
                    var json = File.ReadAllText(SelectedJsonFile);
                    var javaScriptSer = new JavaScriptSerializer();
                    var healthCheck = javaScriptSer.Deserialize<HealthCheck>(json);

                    var testGroupDictionary = new Dictionary<string, TestGroup>();
                    var bsonDoc = BsonSerializer.Deserialize<BsonDocument>(json);
                    foreach (var element in bsonDoc.Elements)
                    {
                        if (!element.Value.IsBsonDocument)
                            continue;
                        var testGroup = new TestGroup(element);
                        if (testGroup.tests.Length > 0)
                        {
                            if (!testGroupDictionary.ContainsKey(element.Name))
                            {
                                testGroupDictionary.Add(element.Name, testGroup);
                            }
                        }
                    }
                    healthCheck.TestDictionary = testGroupDictionary;
                    if (testGroupDictionary.Count == 0)
                    {
                        _parentForm.ShowMessage(string.Format("The selected file [{0}] is invalid, nothing to do.", SelectedJsonFile), true);
                        return;
                    }
                    _enableGUI(healthCheck);
                }
                catch (Exception ex)
                {
                    _parentForm.ShowMessage(ex.Message, true);
                }
            }
        }
        private void _enableGUI(HealthCheck healthCheck)
        {
            splitContainer1.Visible = true;
            addDeleteTableLayoutPanel.Visible = true;
            testTreeView.Visible = true;
            //runTableLayoutPanel.Visible = true;
            pictureBoxRun.Visible = true;
            lblRunTest.Visible = true;
            #if DEBUG
            picturePythonCode.Visible = true;
            labelPythonCode.Visible = true;
            #endif
            toolPathTableLayoutPanel.Visible = true;
            UpdateView(healthCheck);
            _isDirty = false;
        }
        private bool _isValidGroupSelected()
        {
            _allowToRun = false;
            _parentForm.ShowMessage("");

            if (null == SelectedGroupNode
                || SelectedGroupNode == _rootNode
                || string.IsNullOrEmpty(SelectedGroupNode.Text))
            {
                _parentForm.ShowMessage("Please select group.", true);
            }
            else
            {
                _allowToRun = true;
            }
            return _allowToRun;
        }
        private void onImgMinusClick(object sender, EventArgs e)
        {
            if (null != _rootNode && null != SelectedGroupNode)
            {
                _parentForm.ShowMessage("");
                (_rootNode.Tag as HealthCheck).RemoveTestGroup(SelectedGroupNode.Tag as TestGroup);
                _rootNode.Nodes.Remove(SelectedGroupNode);
                splitContainer1.Panel1.Controls.Clear();
            }
            else
            {
                _parentForm.ShowMessage("Please load a Json file.", true);
            }
        }
        private void onImgAddClick(object sender, EventArgs e)
        {
            if (null != _rootNode)
            {
                _parentForm.ShowMessage("");
                var healthCheck = _rootNode.Tag as HealthCheck;
                var testGroup = healthCheck.InsertNewTestGroup();
                if (null != testGroup)
                {
                    var groupNode = new TreeNode(testGroup.key);
                    groupNode.Tag = testGroup;
                    SelectedGroupNode = groupNode;
                    _rootNode.Nodes.Add(groupNode);
                    splitContainer1.Panel1.Controls.Clear();
                    SelectedGroupNode.BeginEdit();
                }
                else
                {
                    _parentForm.ShowMessage(healthCheck.errorMessage, true);
                }
            }
            else
            {
                _parentForm.ShowMessage("Please load a Json file.", true);
            }
        }
        private void UpdateView(HealthCheck healthCheck)
        {
            this.textTimeout.Text = healthCheck.timeout.ToString();
            this.textRecuring.Text = healthCheck.recurring.ToString();
            this.textReportFile.Text = healthCheck.reporthtmlfilename;
            this.textIP.Text = healthCheck.Client.ip;
            this.textUser.Text = healthCheck.Client.user;
            this.textPassword.Text = healthCheck.Client.password;
            this.textBaud.Text = healthCheck.serialportbaudrate;
            this.textPort.Text = healthCheck.serialport;
            var ports = SerialPort.GetPortNames();
            if (ports.Length > 0)
            {
                this.textPort.Text = healthCheck.serialport = ports[0];
            }
            this.testTreeView.Nodes.Clear();
            var jsonFileName = Path.GetFileName(SelectedJsonFile);
            lblSelectedJson.Text = jsonFileName;
            var tooltip = frmConfigEditor.CreateTooltip();
            tooltip.SetToolTip(lblSelectedJson, SelectedJsonFile);
            this._rootNode = new TreeNode(jsonFileName);
            testTreeView.Nodes.Add(_rootNode);
            this._rootNode.Tag = healthCheck;

            var itr = healthCheck.TestDictionary.GetEnumerator();
            while (itr.MoveNext())
            {
                var groupNode = new TreeNode(itr.Current.Key);
                groupNode.Checked = !itr.Current.Value.skip;
                groupNode.Tag = itr.Current.Value;
                AddTestNode(groupNode, itr.Current.Value);
                _rootNode.Nodes.Add(groupNode);
                SelectedGroupNode = groupNode;
            }
            _rootNode.Expand();
            if (null != SelectedGroupNode)
            {
                testTreeView.SelectedNode = SelectedGroupNode;
                testTreeView.Focus();
            }
        }
        private void AddTestControl(Test test = null)
        {
            splitContainer1.Panel1.Controls.Clear();
            _testControl = new TestControl(test, this);
            splitContainer1.SplitterDistance = _testControl.Height;
            splitContainer1.Panel1.Controls.Add(_testControl);
            _testControl.Dock = DockStyle.Fill;
        }
        private void AddTestNode(TreeNode node, TestGroup group)
        {
            foreach (var t in group.tests)
            {
                var n = new TreeNode(t.testid);
                n.Checked = !t.skip;
                n.Tag = t;
                node.Nodes.Add(n);
            }
        }
        private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_isUnknownGroupSelected())
                return;

            if (this._isValidGroupSelected() && !_allowToRun)
            {
                e.Cancel = true;
            }
        }
        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _parentForm.ShowMessage("");
            SelectedTestNode = null;
            SelectedGroupNode = null;
            if (e.Node.Text == TestGroup.UNKNOWN_GROUP)
            {
                _parentForm.ShowMessage(string.Format("Please rename the [{0}] group.", TestGroup.UNKNOWN_GROUP), true);
                SelectedGroupNode = e.Node;
                SelectedGroupNode.BeginEdit();
                return;
            }
            if (e.Node != _rootNode && e.Node.Parent == _rootNode)
            {
                SelectedGroupNode = e.Node;
                splitContainer1.Panel1.Controls.Clear();
                _parentForm.ShowMessage(string.Format("{0} group is selected.", SelectedGroupNode.Text));
            }
            else if (e.Node != _rootNode)
            {
                SelectedTestNode = e.Node;
                SelectedGroupNode = e.Node.Parent;
                AddTestControl(SelectedTestNode.Tag as Test);
            }
        }
        private void onAddTest_Click(object sender, EventArgs e)
        {
            if (_isUnknownGroupSelected())
                return;
            if (!_isValidGroupSelected())
                return;

            if (null != SelectedGroupNode)
            {
                var testGroup = SelectedGroupNode.Tag as TestGroup;
                var test = testGroup.InsertNewTest();
                if (null != test)
                {
                    var node = new TreeNode(test.testid);
                    node.Tag = test;
                    testGroup.Add(test);
                    SelectedGroupNode.Nodes.Add(node);
                    SelectedTestNode = node;
                    AddTestControl(test);
                    SelectedGroupNode.ExpandAll();
                    testTreeView.SelectedNode = node;
                    testTreeView.Focus();
                }
                else
                {
                    _parentForm.ShowMessage(testGroup.errorMessage, true);
                }
            }
            else
            {
                _parentForm.ShowMessage("Please select a group.", true);
            }
        }
        private void onDeleteTest_Click(object sender, EventArgs e)
        {
            if (_isValidGroupSelected() && null != SelectedTestNode)
            {
                (SelectedGroupNode.Tag as TestGroup).RemoveTest(SelectedTestNode.Tag as Test);
                SelectedGroupNode.Nodes.Remove(SelectedTestNode);
                splitContainer1.Panel1.Controls.Clear();
            }

        }
        private void treeView_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (null == SelectedTestNode && null == SelectedGroupNode)
            {
                _parentForm.ShowMessage("Please select a group node or a test node before changed the skip value.", true);
                e.Cancel = true;
            }
            TreeNode n = null;
            var found = false;
            foreach (TreeNode gn in this._rootNode.Nodes)
            {
                if (found)
                    break;
                if(gn.Text == e.Node.Text)
                {
                    this.SelectedGroupNode = n = gn;
                    break;
                }
                else
                {
                    foreach (TreeNode tn in gn.Nodes)
                    {
                        if (tn.Text == e.Node.Text)
                        {
                            this.SelectedTestNode = n = tn;
                            found = true;
                            break;
                        }
                    }
                }
            }
            if (null != n)
            {
                testTreeView.SelectedNode = n;
            }
        }
        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            _parentForm.ShowMessage("");
            var g = e.Node.Tag as TestGroup;
            if (null != g && null != SelectedGroupNode)
            {
                g.skip = !SelectedGroupNode.Checked;
            }
            var t = e.Node.Tag as Test;
            if (null != t && null != SelectedTestNode)
            {
                t.skip = !SelectedTestNode.Checked;
                _testControl.Test = t;
            }
            _isDirty = true;
        }
        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag is TestGroup)
            {
                var healthCheck = _rootNode.Tag as HealthCheck;
                var unknowGroup = healthCheck[TestGroup.UNKNOWN_GROUP];
                if (null != unknowGroup && null != e.Label)
                {
                    unknowGroup.key = e.Label;
                    healthCheck.TestDictionary.Remove(TestGroup.UNKNOWN_GROUP);
                    healthCheck.TestDictionary[e.Label] = unknowGroup;
                    _parentForm.ShowMessage("");
                }
            }
        }
        private void onRunTestClick(object sender, EventArgs e)
        {
            if (_isDirty)
            {
                MessageBox.Show("Please save changes!");
                return;
            }
            _parentForm.ShowMessage("");
            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = textBoxToolPath.Text.Trim();
                startInfo.FileName = "python";
                startInfo.Arguments = string.Format("healthCheck.py -j {0}", this.SelectedJsonFile);
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                _parentForm.ShowMessage(ex.Message, true);
            }
        }
        private void onNewConfigClick(object sender, EventArgs e)
        {
            try
            {
                this._reset();
                var healthCheck = new HealthCheck();
                var group = healthCheck.InsertNewTestGroup();
                var testGroupDictionary = new Dictionary<string, TestGroup>();
                testGroupDictionary.Add(TestGroup.UNKNOWN_GROUP, group);
                healthCheck.TestDictionary = testGroupDictionary;
                var folder = @"C:\Temp";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var idx = 0;
                var defaultJsonFileName = HealthCheck.DEFAULT_JSON_FILE_NAME.Replace(".json", string.Empty);
                foreach (var f in Directory.GetFiles(folder, string.Format("{0}_*.json", defaultJsonFileName)))
                {
                    var ns = f.Replace(string.Format(@"{0}\{1}_", folder, defaultJsonFileName), string.Empty).Replace(".json", string.Empty);
                    if (string.IsNullOrEmpty(ns))
                        ns = "0";
                    var n = int.Parse(ns);
                    if (n >= idx)
                        idx = n;
                }
                SelectedJsonFile = Path.Combine(@"C:\Temp", string.Format("{0}_{1}.json", defaultJsonFileName, (idx + 1).ToString("d3")));
                _enableGUI(healthCheck);
                AllowToRun(_validate());
            }
            catch (Exception ex)
            {
                _parentForm.ShowMessage(ex.Message, true);
            }
        }
        private void onSaveClick(object sender, EventArgs e)
        {
            if (!this._validate())
            {
                var errorMsg = this._parentForm.ToolStripStatusLabel.Text;
                _parentForm.ShowMessage(string.Format("Please resolve the error before save failes. {0}", errorMsg), true);
                return;
            }

            _parentForm.ShowMessage("");
            if (string.IsNullOrEmpty(SelectedJsonFile) || null == _rootNode)
            {
                _parentForm.ShowMessage("No active json file in editing, nothing to do.", true);
                return;
            }
            try
            {
                DoDataExchange();
                var json = (_rootNode.Tag as HealthCheck).ToString();
                File.WriteAllText(SelectedJsonFile, json);
                _isDirty = false;
            }
            catch (IOException ex)
            {
                _parentForm.ShowMessage(ex.Message, true);
            }
        }
        private void onSaveAsClick(object sender, EventArgs e)
        {
            if (!this._validate())
            {
                var errorMsg = this._parentForm.ToolStripStatusLabel.Text;
                _parentForm.ShowMessage(string.Format("Please resolve the error before save failes. {0}", errorMsg), true);
                return;
            }

            _parentForm.ShowMessage("");
            if (string.IsNullOrEmpty(SelectedJsonFile) || null == _rootNode)
            {
                _parentForm.ShowMessage("No active json file in editing, nothing to do.", true);
                return;
            }
            try
            {
                var result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    DoDataExchange();
                    this.SelectedJsonFile = saveFileDialog.FileName;
                    lblSelectedJson.Text = Path.GetFileName(SelectedJsonFile);
                    var tooltip = frmConfigEditor.CreateTooltip();
                    tooltip.SetToolTip(lblSelectedJson, SelectedJsonFile);
                    var json = (_rootNode.Tag as HealthCheck).ToString();
                    File.WriteAllText(saveFileDialog.FileName, json);
                    _isDirty = false;
                }
            }
            catch (IOException ex)
            {
                _parentForm.ShowMessage(ex.Message, true);
            }
        }
        private void onTextChanged(object sender, EventArgs e)
        {
            _isDirty = true;
            var allowToRun = this._validate();
            this.AllowToRun(allowToRun);
        }
        private void onShowPasswordCheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowPassword.Checked)
                textPassword.PasswordChar = new char();
            else
                textPassword.PasswordChar = '*';
        }
        private void DoDataExchange()
        {
            var healthCheck = _rootNode.Tag as HealthCheck;
            healthCheck.timeout = this.textTimeout.Text.Trim();
            healthCheck.recurring = this.textRecuring.Text.Trim();
            healthCheck.reporthtmlfilename = this.textReportFile.Text.Trim();
            healthCheck.serialport = this.textPort.Text.Trim();
            healthCheck.serialportbaudrate = string.IsNullOrEmpty(this.textBaud.Text.Trim()) ? "115200" : textBaud.Text.Trim();
            healthCheck.Client.ip = this.textIP.Text.Trim();
            healthCheck.Client.user = this.textUser.Text.Trim();
            healthCheck.Client.password = this.textPassword.Text.Trim();
            if (null != _testControl)
                _testControl.DoDataExchange();
        }
        private void pictureBoxPythonCode_Click(object sender, EventArgs e)
        {
            _parentForm.ShowMessage(string.Empty);
#if DEBUG
            var targetpath = @"..\..\..\workstates\CrTestState";
            var templatepath = Directory.GetFiles(@"..\..\..\", "crHealth_Template_State.py", SearchOption.AllDirectories);
            var exitingTestsList = new List<string>();
            exitingTestsList.AddRange(Directory.GetFiles(@"..\..\..\workstates\CrTestState", "crHealth_*_State.py", SearchOption.AllDirectories));
#else
            var templatepath = Directory.GetFiles(@".\", "crHealth_Template_State.py", SearchOption.AllDirectories);
            var targetpath = @".\workstates\CrTestState";
            var exitingTestsList = new List<string>();
            exitingTestsList.AddRange(Directory.GetFiles(@".\workstates\CrTestState", "crHealth_*_State.py", SearchOption.AllDirectories));
#endif
            if (templatepath.Length > 0)
            {
                var pythonCode = File.ReadAllText(templatepath[0]);
                foreach (TreeNode groupNode in _rootNode.Nodes)
                {
                    foreach (TreeNode testNode in groupNode.Nodes)
                    {
                        var pythonTestFileName = string.Format("crHealth_{0}_State.py", testNode.Text);
                        if (null == exitingTestsList.FirstOrDefault(x => x.EndsWith(pythonTestFileName)))
                        {
                            try
                            {
                                var pythonTestModule = string.Format(@"{0}\{1}", targetpath, pythonTestFileName);
                                File.WriteAllText(pythonTestModule, pythonCode.Replace("Sample", testNode.Text));
                                _parentForm.ShowMessage(string.Format("Generate test [{0}] success.", pythonTestFileName));
                            }
                            catch (IOException ex)
                            {
                                _parentForm.ShowMessage(ex.Message, true);
                            }
                        }
                    }
                }
            }
        }
#endregion
    }
}
