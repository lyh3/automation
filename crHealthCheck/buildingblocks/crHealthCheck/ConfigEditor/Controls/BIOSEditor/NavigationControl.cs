using System.Windows.Forms;
using ConfigEditor.DataModel;
using System.Drawing;

namespace ConfigEditor.Controls
{
    public partial class NavigationControl : UserControl
    {
        private int _idx = -1;
        private BiosItemControl _parentControl = null;
        private bool _isSelected = false;

        public NavigationControl()
        {
            InitializeComponent();
        }
        public NavigationControl(BIOSItem item, BiosItemControl parentControl)
        {
            InitializeComponent();
            _parentControl = parentControl;
            this.Item = item;
            this.textTo.Text = item.to;
            this.textRecuring.Text = item.recuring;
            var navControl = new SerialInvokeControl(item.navigation);
            this.panelNav.Controls.Add(navControl);
            navControl.Dock = DockStyle.Fill;
            var serialInvokeControl = new SerialInvokeControl(item.invoke);
            this.panelInvoke.Controls.Add(serialInvokeControl);
            serialInvokeControl.Dock = DockStyle.Fill;

            //checkBox.Checked = true;
        }
        public BIOSItem Item { get; private set; }
        public int Idx
        {
            get { return _idx; }
            set { _idx = value; }
        }
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.checkBox.Checked = _isSelected;
                this.BackColor = _isSelected ? Color.Black : Color.Transparent;
            }
        }
        private void onCheckBoxClick(object sender, System.EventArgs e)
        {
            this.BackColor = checkBox.Checked ? Color.DarkGray : Color.Transparent;
            _parentControl.UpdateSelectedNavControl(this);
        }
    }
}
