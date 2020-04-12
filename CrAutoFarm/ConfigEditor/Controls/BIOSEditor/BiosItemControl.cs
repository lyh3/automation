using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ConfigEditor.DataModel;
namespace ConfigEditor.Controls
{
    public partial class BiosItemControl : UserControl
    {
        private BIOSConfig _biosConfig = null;
        private NavigationControl _selectedNavControl = null;
        public BiosItemControl(string key, BIOSConfig biosConfig)
        {
            InitializeComponent();
            _biosConfig = biosConfig;
            this.labelBiosItem.Text = key;
            var navCount = biosConfig.navs.Count();
            for (int i = navCount - 1; i >= 0; i--)
            {
                BIOSItem item = biosConfig.navs[i];
                var navControl = new NavigationControl(item, this) { Idx = i };
                this.scrollableControl.Controls.Add(navControl);
                navControl.Dock = DockStyle.Top;
            }
        }

        public void UpdateSelectedNavControl(NavigationControl navControl)
        {
            if (null == _selectedNavControl)
                _selectedNavControl = navControl;

            foreach (NavigationControl control in this.scrollableControl.Controls)
            {
                control.IsSelected = !this._selectedNavControl.IsSelected;
                if (control.Idx == navControl.Idx)
                {
                    this._selectedNavControl = navControl;
                    control.IsSelected = true;
                }
                else
                {
                    control.IsSelected = false;
                }
            }
        }

        private void onUpClick(object sender, EventArgs e)
        {
            if (null == _selectedNavControl || _selectedNavControl.Idx == 0)
                return;

            bool swapNext = false;
            var controlList = new List<NavigationControl>();

            foreach (NavigationControl control in scrollableControl.Controls)
            {
                if (swapNext)
                {
                    control.Idx += 1;
                    controlList.Add(control);
                    _selectedNavControl.Idx -= 1;
                    controlList.Add(_selectedNavControl);
                    swapNext = false;
                    continue;
                }
                if (control.Idx == _selectedNavControl.Idx)
                {
                    swapNext = true;
                    continue;
                }
                controlList.Add(control);
            }

            _updateScrollableControls(controlList);
        }
        private void onDownClick(object sender, EventArgs e)
        {
            if (null == _selectedNavControl || _selectedNavControl.Idx == scrollableControl.Controls.Count - 1)
                return;

            bool swapNext = false;
            NavigationControl tempControl = null;
            var controlList = new List<NavigationControl>();

            foreach (NavigationControl control in scrollableControl.Controls)
            {
                if (swapNext)
                {
                    _selectedNavControl.Idx += 1;
                    controlList.Add(_selectedNavControl);
                    tempControl.Idx -= 1;
                    controlList.Add(tempControl);
                    swapNext = false;
                    continue;
                }
                if (control.Idx == _selectedNavControl.Idx + 1)
                {
                    tempControl = control;
                    swapNext = true;
                    continue;
                }
                controlList.Add(control);
            }

            _updateScrollableControls(controlList);
        }
        private void _updateScrollableControls(List<NavigationControl> controlList)
        {
            var navList = new List<BIOSItem>();
            scrollableControl.Controls.Clear();
            for (int i = 0; i < controlList.Count; i++)
            {
                var control = controlList[i];
                scrollableControl.Controls.Add(control);
                control.Dock = DockStyle.Top;
                navList.Insert(0, control.Item);
            }
            _biosConfig.navs = navList.ToArray();
        }
    }
}
