using System.Drawing;
using System.Windows.Forms;

using ConfigEditor.Controls;

namespace ConfigEditor
{
    public partial class frmConfigEditor : Form
    {
        #region Constructors
        public frmConfigEditor()
        {
            InitializeComponent();
            MaximizeBox = false;
            tabControl.TabPages["tabPageHealthTest"].Controls.Add(new TestEditorControl(this));
            tabControl.TabPages["tabPageBIOSSettings"].Controls.Add(new BiosEditorControl(this));
        }
        #endregion

        #region Properties
        public ToolStripStatusLabel ToolStripStatusLabel
        {
            get { return this.toolStripStatusLabel; }
        }
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
        static public ToolTip CreateTooltip()
        {
            var tooltip = new ToolTip();
            tooltip.AutoPopDelay = 5000;
            tooltip.InitialDelay = 1000;
            tooltip.ReshowDelay = 500;
            tooltip.ShowAlways = true;
            return tooltip;
        }
        #endregion

    }
}
