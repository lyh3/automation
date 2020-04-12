using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy.Schema;
using McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy;

namespace SmartWebserviceProxyDemo
{
    public partial class ProxyStatusControl : UserControl
    {
        enum HelthyStatus { OK = 0, UNAVAILABLE = 1 }
        private int _index;
        private SmartProxyController _proxycontroller;
        private ImageList _imagelist;
        int _count;
        private Color _backcolor;
        private HelthyStatus _healthystatus;

        public ProxyStatusControl()
        {
            InitializeComponent();
        }

        public ProxyStatusControl(  int index, 
                                    endpoint endpoint,
                                    SmartProxyController proxycontroller,
                                    ImageList imagelist) 
            : this()
        {
            _index = index;
            textBoxAddress.Text = endpoint.address;
            _proxycontroller = proxycontroller;
            _imagelist = imagelist;
            proxycontroller.ControllerStatusChanged += new EventHandler(OnControllerStatusChanged);
            _backcolor = this.BackColor;
        }

        void OnControllerStatusChanged(object sender, EventArgs e)
        {
            if (_index == _proxycontroller.CurrentAssignedIndex)
                _count++;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            lblControllername.Text = _proxycontroller.ContractName;
            _healthystatus = null == _proxycontroller.BadEndpointList.FirstOrDefault<ProxyWrapper>(w => w.EndPoint.address == textBoxAddress.Text) ? HelthyStatus.OK : HelthyStatus.UNAVAILABLE;
            pictureBoxStatus.Image = _imagelist.Images[(int)_healthystatus];
            textBoxHitCount.Text = _count.ToString();
            this.BackColor = _healthystatus == HelthyStatus.OK ? _backcolor : Color.Gold;
        }
    }
}
