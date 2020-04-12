using System;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace ConfigEditor.Controls  
{
    public partial class SmokeTestControl : UserControl
    {
        private frmConfigEditor _parentForm = null;
        public SmokeTestControl()
        {
            InitializeComponent();
            //tableLayoutPanel.Visible = false;
            this.comboBoxLoops.SelectedIndex = 0;
        }
        public SmokeTestControl(frmConfigEditor parentForm)
        {
            InitializeComponent();
            //tableLayoutPanel.Visible = false;
            this.comboBoxLoops.SelectedIndex = 0;
            _parentForm = parentForm;
            this.textBoxSmokeJson.Text = string.Format(@"{0}\SATTest.json", _parentForm.Apppath);
        }


        private void onOpen(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                tableLayoutPanel.Visible = true;
                this.textBoxSmokeJson.Text = openFileDialog.FileName;
                this.comboBoxLoops.SelectedIndex = 0;
                this.comboBoxLoops.Focus();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            _parentForm.ShowMessage(string.Empty);
            var jsonFile = this.textBoxSmokeJson.Text;
            if (!File.Exists(jsonFile))
            {
                _parentForm.ShowMessage(string.Format("Selected Json file [{0}] not exists.", jsonFile), true);
                return;
            }
            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = _parentForm.Apppath;
                startInfo.FileName = "python";

                var sb = new StringBuilder();
                if (checkBoxAC.Checked)
                    sb.Append("AC,");
                if (checkBoxAD.Checked)
                    sb.Append("AD,");
                if (checkBoxMM.Checked)
                    sb.Append("MM,");
                if (checkBoxWR.Checked)
                    sb.Append("WR,");
                var filter = sb.ToString();

                var loops = 1;
                if (!string.IsNullOrEmpty(this.comboBoxLoops.SelectedText))
                    loops = int.Parse(this.comboBoxLoops.SelectedText);
                var arguments = string.Format("SAT.py -j {0} -l {1}", jsonFile, loops);
                if (!string.IsNullOrEmpty(filter))
                {
                    arguments = string.Format("{0} -f \"{1}\"", arguments, filter.TrimEnd(','));
                }
                if (checkBoxSkipInitialAC.Checked)
                {
                    arguments = string.Format("{0} -s True", arguments);
                }

                startInfo.Arguments = arguments;

                var process = new Process() { StartInfo = startInfo };
                _parentForm.EnableProgressBar(true);
                process.Start();
                Application.DoEvents();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                _parentForm.ShowMessage(ex.Message, true);
            }
            finally
            {
                _parentForm.EnableProgressBar(false);
            }
        }
    }
}
