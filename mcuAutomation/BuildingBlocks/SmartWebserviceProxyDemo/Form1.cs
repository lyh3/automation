using System;
using System.Windows.Forms;
using System.ServiceModel.Channels;
using System.Collections.Generic;

using McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy;
using McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy.Schema;

namespace SmartWebserviceProxyDemo
{
    public partial class Form1 : Form
    {
        private int _currentbadendipointcount;
        private int _endpointcount;
        private static bool _configchanged;
        private List<SmartProxyController> _controllerList = new List<SmartProxyController>();

        public Form1()
        {
            InitializeComponent();

            SmartProxyController proxycontroller;

            var ssccontroller = new TestSSCProxyController(contractName: "SSCClient.ISampleSubmission");
            ssccontroller.ConfigChangedRelay += OnConfigChanged;
            _controllerList.Add(ssccontroller);
            ssccontroller.HealthyCheckInterval = 5.0 * 1000.0;

            //var caspercontroller = new TestCasperScanProxyController(contractName: "CasperScanSoapClient.CasperScanSoap");
            //caspercontroller.ConfigChangedRelay += OnConfigChanged;
            //_controllerList.Add(caspercontroller);

            //proxycontroller = InitializeMetaDataController(OnConfigChanged);
            //_controllerList.Add(proxycontroller);

            InitializeGui();
        }

        private SmartProxyController InitializeMetaDataController(EventHandler configChangedEventhandler = null)
        {
            var metadatacontroller = new TestMetaDataServiceProxyController(contractName: "MetadatServiceClient.IMetaDataService")
            {
                VerifyProxyParameters = new object[] { new[] { @"ftp://ftpuser:mcafee!624@beaqaftp1/test/0759d5857898980d7fa238c43c4aa97f.zip" }, 
                                                               @"0", 
                                                               true}
            };

            if(null != configChangedEventhandler)
                metadatacontroller.ConfigChangedRelay += configChangedEventhandler;

            return metadatacontroller;
        }

        private void InitializeGui()
        {
            _controllerList.ForEach(controller =>
            {
                _currentbadendipointcount += controller.BadEndpointList.Count;
                _endpointcount += controller.EndpointCount;
                checkBoxLazyUpdate.Checked = controller.LazyConfigUpdate;
                FactoryProxyStatusControl();
            });
            _configchanged = false;
        }

        void OnConfigChanged(object sender, EventArgs e)
        {
            _configchanged = true;
        }

        private void OnLazyUpdateCheckedChanged(object sender, EventArgs e)
        {
            btnSync.Enabled = checkBoxLazyUpdate.Checked;
            _controllerList.ForEach(controller =>
            {
                controller.LazyConfigUpdate = checkBoxLazyUpdate.Checked;
            });
        }

        private void FactoryProxyStatusControl()
        {
            scrollableControl.Controls.Clear();

            _controllerList.ForEach(controller =>
            {
                for (int i = controller.EndpointCount - 1; i >= 0; --i)
                {
                    var statuscontrol = new ProxyStatusControl(i, controller.EndpointInfo(i), controller, imageList);
                    scrollableControl.Controls.Add(statuscontrol);
                    statuscontrol.Dock = DockStyle.Top;
                }
            }); 
        }

        private void OnStartClick(object sender, EventArgs e)
        {
            _controllerList.ForEach(controller =>
            {
                for (int i = 0; i < _repeatUpDown.Value; ++i)
                {
                    Application.DoEvents();
                    var proxy = (controller as IDynamicProxy).ServiceProxy;
                    var endpoint = controller.EndpointInfo(controller.CurrentAssignedIndex);
                    messageBox.AppendText(string.Format(@"Connect to endpoint <{0}>, metadata identifier:{1}", endpoint.address, Environment.NewLine));
                    controller.VerifyProxy(new ProxyWrapper
                    {
                        Proxy = proxy as IChannel,
                        EndPoint = endpoint
                    });

                    if (string.IsNullOrEmpty(controller.VerifyProxyResults)) continue;

                    messageBox.AppendText(controller.VerifyProxyResults);
                    messageBox.ScrollToCaret();
                }
            });
        }
        
        private void OnTimerTick(object sender, EventArgs e)
        {
            _controllerList.ForEach(controller =>
            {
                if (_configchanged)
                {
                    messageBox.Clear();
                    messageBox.AppendText(@"--- Received configuration changed event.");
                    InitializeGui();
                }
                else
                {
                    textBoxBadEndpointCount.Text = controller.BadEndpointList.Count.ToString();
                    textBoxCurrentSelectedEndpoint.Text = controller.EndpointInfo(controller.CurrentAssignedIndex).address;
                }
            });
        }

        private void OnSyncClick(object sender, EventArgs e)
        {
            timer.Enabled = false;
            _controllerList.ForEach(controller =>
            {
                controller.ConfigUpdateRequest();
            });
            timer.Enabled = true;
        }
    }
}
