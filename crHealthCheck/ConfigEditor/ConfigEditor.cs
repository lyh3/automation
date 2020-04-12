using System.Drawing;
using System.Windows.Forms;
using System.IO;

using ConfigEditor.Controls;

namespace ConfigEditor
{
    public partial class frmConfigEditor : Form
    {
        private bool _isProgressBarEnabled = false;
        string _appPath = Path.GetDirectoryName(Application.ExecutablePath);
        #region Constructors
        public frmConfigEditor()
        {
            InitializeComponent();
            MaximizeBox = false;
            tabControl.TabPages["tabPageHealthTest"].Controls.Add(new TestEditorControl(this));
            tabControl.TabPages["tabPageSATSmokeTest"].Controls.Add(new SmokeTestControl(this));
            tabControl.TabPages["tabPageBIOSSettings"].Controls.Add(new BiosEditorControl(this));
        }
        #endregion

        #region Properties
        public ToolStripStatusLabel ToolStripStatusLabel
        {
            get { return this.toolStripStatusLabel; }
        }
        public bool IsProgressBarEnabled
        {
            get { return _isProgressBarEnabled; }
            set { _isProgressBarEnabled = value; }
        }
        public string Apppath { get { return _appPath; } }
        #endregion

        #region Public Methods
        public void ShowMessage(string msg, bool error = false)
        {
            var color = Color.DimGray;
            if (error)
            {
                color = Color.Red;
            }
            toolStripStatusLabel.Text = msg;
            statusStrip.BackColor = color;
        }
        public void EnableProgressBar(bool enable)
        {
            IsProgressBarEnabled = enable;
        }
        static public ToolTip CreateTooltip()
        {
            var tooltip = new ToolTip();
            tooltip.AutoPopDelay = 5000;
            tooltip.InitialDelay = 1000;
            tooltip.ReshowDelay = 500;
            tooltip.ShowAlways = true;
            return tooltip;
        }

        private void timer_Tick(object sender, System.EventArgs e)
        {
            this.progressBar.Visible = IsProgressBarEnabled;
            Application.DoEvents();
        }
        #endregion
    }
}
