using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfigEditor.Controls
{
    public partial class BiosConfigControl : UserControl
    {
        private BiosEditorControl _parentControl = null;
         public BiosConfigControl(string biosconfig, BiosEditorControl parentControl)
        {
            InitializeComponent();
            _parentControl = parentControl;
            BiosConfig = biosconfig;
            this.textBiosConfig.Text = biosconfig;
        }

        public string BiosConfig { get; set; }
        public bool IsSelected { get { return this.checkBox1.Checked; } }
        public void DoDataExchange()
        {
            var config = textBiosConfig.Text.Trim();
            _parentControl.ConfigEditorForm.ShowMessage(string.Empty);
            if (string.IsNullOrEmpty(config))
            {
                _parentControl.ConfigEditorForm.ShowMessage("Please input BIOS config name");
                return;
            }
            this.BiosConfig = config;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //this.BackColor = checkBox1.Checked ? Color.LightBlue : Color.Transparent;
        }

        private void textBiosConfig_TextChanged(object sender, EventArgs e)
        {
            DoDataExchange();
        }
        private void textBiosConfig_DoubleClick(object sender, EventArgs e)
        {
            //this.BackColor = textBiosConfig.Focused ? Color.LightBlue : Color.Transparent;
            _parentControl.SelectBiosItem(this.BiosConfig);
        }
    }
}
