using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ConfigEditor.DataModel;
using System.Drawing;
using System.Text;

namespace ConfigEditor.Controls
{
    public partial class TestControl : UserControl
    {
        private Test _test = null;
        private TestEditorControl _testEditorControl = null;
        ContextMenu _expectedDataContextmenu = null;
        ContextMenu _shellCommandContextmenu = null;

        #region Constructors
        public TestControl()
        {
            InitializeComponent();
            _addExpectedDataContexMenu();
            _addShellCommandContextMenu();
        }
        public TestControl(Test test, dynamic testEditorControl)
        {
            InitializeComponent();
            if (null != test)
            {
                this.Test = test;
                var expectedDataTooltip = frmConfigEditor.CreateTooltip();
                expectedDataTooltip.SetToolTip(this.scrollableControlExpectedData, "Right click to add/delete an expected data (name/value pair).");
                var shellCommandTooltip = frmConfigEditor.CreateTooltip();
                shellCommandTooltip.SetToolTip(this.scrollableControlShellCommand, "Right click to add/delete a shell command.");
                _addExpectedDataContexMenu();
                _addShellCommandContextMenu();
            }
            this._testEditorControl = testEditorControl;
            this.EditorForm = testEditorControl.FrmConfigEditor;
        }
        #endregion

        #region Public Methods
        public void DoDataExchange()
        {
            if (null == _test)
                return;

            _test.powercycle = !rPowerCycleFalse.Checked;
            _test.skip = !rSkipFalse.Checked;
            _test.waitkeystroke = !rWaitKeystrokeFalse.Checked;

            _test.objective = richTextObjective.Text;
            _test.passFailCreterial = richTextPassFail.Text;
            _test.TestSetup = richTextTestSetup.Text;
            _test.testProcedure = richTextTestProcedure.Text;

            var shellCommandList = new List<Dictionary<string, string>>();
            foreach (var control in scrollableControlShellCommand.Controls)//shellCommands)
            {
                if (!(control is ShellCommandControl))
                    continue;
                var commandControl = control as ShellCommandControl;
                var command = commandControl.command;
                if (string.IsNullOrEmpty(command))
                    continue;
                var dic = new Dictionary<string, string>();
                dic.Add("command", command.Trim());
                shellCommandList.Add(dic);
            }
            _test.shellCommand = shellCommandList.ToArray();

            var expecteddata = new List<Dictionary<string, string>>();
            foreach(ExpectedDataControl e in scrollableControlExpectedData.Controls)
            {
                e.DoDataExchange();
                var name = e.expectedData.name.Trim();
                var val = e.expectedData.value.Trim();
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(val))
                {
                    var dic = new Dictionary<string, string>();
                    dic.Add("name", name);
                    dic.Add("value", val);
                    expecteddata.Add(dic);
                }
            }
            _test.expecteddata = expecteddata.ToArray();
        }
        #endregion

        #region Properties
        public frmConfigEditor EditorForm { get; set; }

        public Test Test
        {
            get { return _test; }
            set
            {
                _test = value;
                _updateUI();
            }
        }
        #endregion

        #region Private Methods
        private void _updateUI()
        {
            timer.Enabled = false;
            if (_test != null)
            {
                textTestId.Text = _test.testid;
                richTextObjective.Text = _test.objective;
                richTextPassFail.Text = _test.passFailCreterial;
                richTextTestSetup.Text = _test.TestSetup;
                richTextTestProcedure.Text = _test.testProcedure;
                scrollableControlShellCommand.Controls.Clear();
                var commandControlList = new List<ShellCommandControl>();
                foreach (var c in _test.shellCommand)
                {
                    var itr = c.GetEnumerator();
                    while (itr.MoveNext())
                    {
                        var shellCommandControl = new ShellCommandControl(_testEditorControl, itr.Current.Value);
                        shellCommandControl.ConfigChanged += onConfigChanged;
                        commandControlList.Insert(0, shellCommandControl);
                    }
                }
                foreach (var control in commandControlList)
                {
                    scrollableControlShellCommand.Controls.Add(control);
                    control.Dock = DockStyle.Top;
                }
                scrollableControlExpectedData.Controls.Clear();
                var expControlList = new List<ExpectedDataControl>();
                foreach (var d in _test.expecteddata)
                {
                    var idx = 1;
                    var dataName = string.Empty;
                    var itr = d.GetEnumerator();
                    while (itr.MoveNext())
                    {
                        if (idx % 2 == 0)
                        {
                            var expectedDataControl = new ExpectedDataControl(_testEditorControl, new ExpectedData(dataName, itr.Current.Value));
                            expectedDataControl.ConfigChanged += onConfigChanged;
                            expControlList.Insert(0, expectedDataControl);
                        }
                        else
                        {
                            dataName = itr.Current.Value;
                        }
                        idx += 1;
                    }
                }
                foreach(var control in expControlList)
                {
                    scrollableControlExpectedData.Controls.Add(control);
                    control.Dock = DockStyle.Top;
                }
                rPowerCycleFalse.Checked = !_test.powercycle;
                rPowerCycleTrue.Checked = _test.powercycle;
                rSkipFalse.Checked = !_test.skip;
                rSkipTrue.Checked = _test.skip;
                rWaitKeystrokeFalse.Checked = !_test.waitkeystroke;
                rWaitKeystrokeTrue.Checked = _test.waitkeystroke;
            }
        }

        private void onConfigChanged(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void rSkipFalse_CheckedChanged(object sender, System.EventArgs e)
        {
            rSkipTrue.Checked = !rSkipFalse.Checked;
            _test.skip = rSkipTrue.Checked;
            if (null != _testEditorControl)
                _testEditorControl.IsDirty = true;
        }

        private void rSkipTrue_CheckedChanged(object sender, System.EventArgs e)
        {
            rSkipFalse.Checked = !rSkipTrue.Checked;
            _test.skip = rSkipTrue.Checked;
            if (null != _testEditorControl)
                _testEditorControl.IsDirty = true;
        }

        private void rPowerCycleFalse_CheckedChanged(object sender, System.EventArgs e)
        {
            rPowerCycleTrue.Checked = !rPowerCycleFalse.Checked;
            _test.powercycle = rPowerCycleTrue.Checked;
            if (null != _testEditorControl)
                _testEditorControl.IsDirty = true;
        }

        private void rPowerCycleTrue_CheckedChanged(object sender, System.EventArgs e)
        {
            rPowerCycleFalse.Checked = !rPowerCycleTrue.Checked;
            _test.powercycle = rPowerCycleTrue.Checked;
            if (null != _testEditorControl)
                _testEditorControl.IsDirty = true;
        }

        private void rWaitKeystrokeFalse_CheckedChanged(object sender, System.EventArgs e)
        {
            rWaitKeystrokeTrue.Checked = !rWaitKeystrokeFalse.Checked;
            _test.waitkeystroke = rWaitKeystrokeTrue.Checked;
            if (null != _testEditorControl)
                _testEditorControl.IsDirty = true;
        }

        private void rWaitKeystrokeTrue_CheckedChanged(object sender, System.EventArgs e)
        {
            rWaitKeystrokeFalse.Checked = !rWaitKeystrokeTrue.Checked;
            _test.waitkeystroke = rWaitKeystrokeTrue.Checked;
            if (null != _testEditorControl)
                _testEditorControl.IsDirty = true;
        }
        private void scrollableControl_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        _expectedDataContextmenu.Show(this, new Point(scrollableControlExpectedData.Right, scrollableControlExpectedData.Bottom));
                    }
                    break;
            }
        }
        private void scrollableControlShellCommand_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        _shellCommandContextmenu.Show(this, new Point(scrollableControlShellCommand.Right, scrollableControlShellCommand.Bottom));
                    }
                    break;
            }
        }
        private void _addExpectedDataContexMenu()
        {
            _expectedDataContextmenu = new ContextMenu();
            var addExpectedData = new MenuItem("Add");
            addExpectedData.Click += AddExpectedData_Click;
            _expectedDataContextmenu.MenuItems.Add(addExpectedData);
            var deleteData = new MenuItem("Delete");
            deleteData.Click += DeleteExpectedData_Click;
            _expectedDataContextmenu.MenuItems.Add(deleteData);

            scrollableControlExpectedData.ContextMenu = _expectedDataContextmenu;
        }

        private void _addShellCommandContextMenu()
        {
            _shellCommandContextmenu = new ContextMenu();
            var addChellCommand = new MenuItem("Add");
            addChellCommand.Click += AddShellCommand_Click;
            _shellCommandContextmenu.MenuItems.Add(addChellCommand);
            var deleteShellCommand = new MenuItem("Delete");
            deleteShellCommand.Click += DeleteShellCommand_Click;
            _shellCommandContextmenu.MenuItems.Add(deleteShellCommand);

            scrollableControlShellCommand.ContextMenu = _shellCommandContextmenu;
        }
        private void AddShellCommand_Click(object sender, System.EventArgs e)
        {
            var count = scrollableControlShellCommand.Controls.Count;
            var shellCommandControl = new ShellCommandControl(_testEditorControl, string.Empty);
            scrollableControlShellCommand.Controls.Add(shellCommandControl);
            scrollableControlShellCommand.Controls.SetChildIndex(shellCommandControl, count);
            shellCommandControl.Dock = DockStyle.Top;
            shellCommandControl.ConfigChanged += onConfigChanged;
            timer.Enabled = true;
        }
        private void DeleteShellCommand_Click(object sender, System.EventArgs e)
        {
            for (var i = 0; i < 3; ++i)
            {
                foreach (ShellCommandControl control in scrollableControlShellCommand.Controls)
                {
                    if (control.IsSelected)
                    {
                        if (!string.IsNullOrEmpty(control.Name))
                        {
                            _test.RemoveShellCommand(control.command);
                        }
                        scrollableControlShellCommand.Controls.Remove(control);
                    }
                }
                System.Threading.Thread.Sleep(500);
            }
            _testEditorControl.IsDirty = true;
        }
        private void AddExpectedData_Click(object sender, System.EventArgs e)
        {
            var count = scrollableControlExpectedData.Controls.Count;
            var expectedDataControl = new ExpectedDataControl(this._testEditorControl, new ExpectedData(string.Empty, string.Empty));
            scrollableControlExpectedData.Controls.Add(expectedDataControl);
            scrollableControlExpectedData.Controls.SetChildIndex(expectedDataControl, count);
            expectedDataControl.Dock = DockStyle.Top;
            expectedDataControl.ConfigChanged += onConfigChanged;
            timer.Enabled = true;
            _testEditorControl.IsDirty = true;
        }

        private void DeleteExpectedData_Click(object sender, System.EventArgs e)
        {
            for (var i = 0; i < 3; ++i)
            {
                foreach (ExpectedDataControl control in scrollableControlExpectedData.Controls)
                {
                    if (control.IsSelected)
                    {
                        if (null != control.expectedData && null != _test)
                        {
                            if (!string.IsNullOrEmpty(control.Name))
                            {
                                _test.RemoveExpectedData(control.expectedData.name);
                            }
                        }
                        scrollableControlExpectedData.Controls.Remove(control);
                    }
                }
                System.Threading.Thread.Sleep(500);
            }
            _testEditorControl.IsDirty = true;
        }
        private void timer_Tick(object sender, System.EventArgs e)
        {
            var valid = true;
            var errorMsg = new StringBuilder();
            for (var idx = scrollableControlShellCommand.Controls.Count - 1; idx >= 0; --idx)
            {
                ShellCommandControl shellCommandControl = scrollableControlShellCommand.Controls[idx] as ShellCommandControl;
                shellCommandControl.BackColor = Color.Transparent;
                if(string.IsNullOrEmpty(shellCommandControl.command))
                {
                    errorMsg = errorMsg.Append("Please input your command.  ");
                    valid = false;
                    this._testEditorControl.AllowToRun(false);
                    shellCommandControl.BackColor = Color.Pink;
                    EditorForm.ShowMessage(errorMsg.ToString(), true);
                }
                if(_test.IsShellCommandExist(shellCommandControl.command))
                {
                    valid = false;
                    shellCommandControl.BackColor = Color.Pink;
                    errorMsg.Append(string.Format("The command [{0}] already exists, please resolve the duplicate.   ", shellCommandControl.command));
                }
                else if(!string.IsNullOrEmpty(shellCommandControl.command.Trim()))
                {
                    valid &= true;
                    this.DoDataExchange();
                }
            }
            for (var idx = scrollableControlExpectedData.Controls.Count - 1; idx >= 0; --idx)
            {
                ExpectedDataControl expectedDataControl = scrollableControlExpectedData.Controls[idx] as ExpectedDataControl;
                expectedDataControl.BackColor = Color.Transparent;
                ExpectedData data = expectedDataControl.expectedData;
                if (string.IsNullOrEmpty(data.name) || string.IsNullOrEmpty(data.value))
                {
                    errorMsg.Append("Please input your expected data.  ");
                    valid &= false;
                    expectedDataControl.BackColor = Color.Pink;
                }
                if (_test.IsExpectedDataExist(data.name))
                {
                    valid &= false;
                    errorMsg.Append(string.Format("The expected data [{0}] already exists, please resolve the duplicate.   ", data.name));
                    expectedDataControl.BackColor = Color.Pink;
                }
                else if (!string.IsNullOrEmpty(data.value.Trim()))
                {
                    valid &= true;
                    this.DoDataExchange();
                }
            }
            if (!valid)
            {
                EditorForm.ShowMessage(errorMsg.ToString(), true);
                this._testEditorControl.AllowToRun(false);
            }
            else
            {
                timer.Enabled = false;
            }
            if (null != EditorForm)
            {
                EditorForm.ShowMessage(errorMsg.ToString(), !valid);
                this._testEditorControl.AllowToRun(valid);
            }
        }
        #endregion
    }
}
