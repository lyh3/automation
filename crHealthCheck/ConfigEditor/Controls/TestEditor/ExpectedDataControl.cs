using System;
using System.Windows.Forms;
using ConfigEditor.DataModel;

namespace ConfigEditor.Controls
{
    public partial class ExpectedDataControl : UserControl
    {
        private event EventHandler _configChanged;
        private frmConfigEditor _editorForm = null;
        private TestEditorControl _testEditorControl = null;
        public ExpectedDataControl()
        {
            InitializeComponent();
        }
        public ExpectedDataControl(TestEditorControl testEditorControl, ExpectedData data = null)
        {
            InitializeComponent();
            _testEditorControl = testEditorControl;
            if (null != data)
            {
                textName.Text = data.name;
                textValue.Text = data.value;
                expectedData = data;
            }
        }
        public event EventHandler ConfigChanged
        {
            add { _configChanged = (EventHandler)Delegate.Combine(_configChanged, value); }
            remove { _configChanged = (EventHandler)Delegate.Remove(_configChanged, value); }
        }
        public ExpectedData expectedData { get; set; }
        public bool IsSelected { get; set; }
        public void DoDataExchange()
        {
            if(null != expectedData)
            {
                expectedData.name = textName.Text.Trim();
                expectedData.value = textValue.Text.Trim();
                if(null != _configChanged)
                {
                    _configChanged(this, null);
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            this.IsSelected = checkBox1.Checked;
        }

        private void onTextChanged(object sender, System.EventArgs e)
        {
            DoDataExchange();
            if (null != _testEditorControl)
                _testEditorControl.IsDirty = true;
        }
    }
}
