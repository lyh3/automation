using System;
using System.Windows.Forms;

namespace ConfigEditor.Controls
{
    public partial class ShellCommandControl : UserControl
    {
        private event EventHandler _configChanged;
        private TestEditorControl _testEditorControl = null;
        public ShellCommandControl()
        {
            InitializeComponent();
        }
        public ShellCommandControl(TestEditorControl testEditorControl, string command)
        {
            InitializeComponent();
            _testEditorControl = testEditorControl;
            this.command = command;
            textShellCommand.Text = command;
        }
        public event EventHandler ConfigChanged
        {
            add { _configChanged = (EventHandler)Delegate.Combine(_configChanged, value); }
            remove { _configChanged = (EventHandler)Delegate.Remove(_configChanged, value); }
        }
        public string command { get; set; }
        public bool IsSelected { get; set; }
        public void DoDataExchange()
        {
            command = textShellCommand.Text.Trim();
            if (null != _configChanged)
            {
                _configChanged(this, null);
            }
        }

        private void textShellCommand_TextChanged(object sender, EventArgs e)
        {
            DoDataExchange();
            if (null != _testEditorControl)
                _testEditorControl.IsDirty = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSelected = checkBox1.Checked;
            if (null != _testEditorControl)
                _testEditorControl.IsDirty = true;
        }
    }
}
