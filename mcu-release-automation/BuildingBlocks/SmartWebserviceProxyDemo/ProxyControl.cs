using System;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;
using System.Threading.Tasks;
using System.Collections.Generic;

using McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy;
using McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy.Schema;

namespace SmartWebserviceProxyDemo
{
    public class ProxyControl : UserControl
    {
        protected SmartProxyController _proxycontroller;
        protected int _currentbadendipointcount;
        protected int _endpointcount;
        protected static bool _configchanged;

        public ProxyControl()
        {
        }

        public ImageList ImageList { get; set; }
        public string ControlName { get { return this.GetType().Name.Replace(@"Control", string.Empty); } }
        virtual public CheckBox CheckBoxLazyUpdate { get { return null; } }
        virtual public ScrollableControl ScrollableControlContainer { get { return null; } }
        virtual public Button BtnSync { get { return null; } }
        virtual public RichTextBox MessageBox { get { return null; } }
        virtual public NumericUpDown RepeatUpDown { get { return null; } }
        virtual public TextBox TextBoxBadEndpointCount { get { return null; } }
        virtual public TextBox TextBoxCurrentSelectedEndpoint { get { return null; } }

        public void InitializeGui()
        {
            _configchanged = false;
            _currentbadendipointcount = _proxycontroller.BadEndpointList.Count;
            _endpointcount = _proxycontroller.EndpointCount;
            CheckBoxLazyUpdate.Checked = _proxycontroller.LazyConfigUpdate;
            FactoryProxyStatusControl();
            _configchanged = false;
        }

        protected void FactoryProxyStatusControl()
        {
            ScrollableControlContainer.Controls.Clear();

            for (int i = _proxycontroller.EndpointCount - 1; i >= 0; --i)
            {
                var statuscontrol = new ProxyStatusControl(i, _proxycontroller.EndpointInfo(i), _proxycontroller, ImageList);
                ScrollableControlContainer.Controls.Add(statuscontrol);
                statuscontrol.Dock = DockStyle.Top;
            }
        }

        protected void OnConfigChanged(object sender, EventArgs e)
        {
            _configchanged = true;
        }

        protected void UpdateLazyGui()
        {
            BtnSync.Enabled = _proxycontroller.LazyConfigUpdate = CheckBoxLazyUpdate.Checked;
        }

        protected void Start()
        {
            for (int i = 0; i < RepeatUpDown.Value; ++i)
            {
                Application.DoEvents();
                var proxy = (_proxycontroller as TestSSCProxyController).ServiceProxy;
                var endpoint = _proxycontroller.EndpointInfo(_proxycontroller.CurrentAssignedIndex);
                MessageBox.AppendText(string.Format(@"Connect to endpoint <{0}>, endpoint name:{1}{2}", endpoint.address, endpoint.name, Environment.NewLine));
                _proxycontroller.VerifyProxy(new ProxyWrapper
                {
                    Proxy = proxy as IChannel,
                    EndPoint = endpoint
                });

                if (string.IsNullOrEmpty(_proxycontroller.VerifyProxyResults)) break;

                MessageBox.AppendText(_proxycontroller.VerifyProxyResults);
                MessageBox.ScrollToCaret();
            }
        }

        protected void UpdateGui()
        {
            if (_configchanged)
            {
                MessageBox.Clear();
                MessageBox.AppendText(@"--- Received configuration changed event.");
                InitializeGui();
            }
            else
            {
                TextBoxBadEndpointCount.Text = _proxycontroller.BadEndpointList.Count.ToString();
                TextBoxCurrentSelectedEndpoint.Text = _proxycontroller.EndpointInfo(_proxycontroller.CurrentAssignedIndex).address;
            }
        }
    }
}
