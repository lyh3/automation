using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;
using System.Configuration;
using System.Threading;
using System.Text;

using McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy.Schema;
using McAfeeLabs.Engineering.Automation.AOP;
#if LOG // TODO : Remove when .NET 4.0 log manager is used 
using Avert.Common.Logging.Interfaces;
using Avert.Common.Logging.Factory;
#endif


namespace McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy
{
    abstract public partial class SmartProxyController
    {
        #region Declarations

        protected static object _syncObj = new object();
        protected const double Interval = 5.0 * 1000.0;// default value 20 sec
        #if LOG // TODO : Remove when .NET 4.0 log manager is used 
        protected static ILogger _logger = DefaultLogManagerFactory.GetInstance().GetLogManager().GetLogger(typeof(SmartProxyController));
        #endif
        protected Dictionary<int, ProxyWrapper> _proxydictionary = new Dictionary<int, ProxyWrapper>();
        protected List<endpoint> _endpointList = new List<endpoint>();
        protected List<ProxyWrapper> _badProxyAddressList = new List<ProxyWrapper>();
        protected int _currentassignedIndex = 0;
        protected System.Timers.Timer _proxyHelthymonitorTimer;
        protected ConfigurationChangeMonitor _configmonitor;
        protected XmlDocConfigProperty _wcfClientConfigProperty;
        protected double _healthyCheckInterval = Interval;
        protected string _contractName;
        protected event EventHandler _eventControllerStatusChanged;
        protected event EventHandler _eventConfigChangedRelay;
        protected bool _canupdatetimer = true;
        protected string _configPath;

        #endregion

        #region Constructors

        public SmartProxyController(string contractName, string configPath=null)
        {
            _contractName = contractName;
            _configPath = configPath;
            InitializeController();
        }

        #endregion

        #region Properties

        public IChannel this[int index]
        {
            get
            {
                IChannel channel = null;

                if (index > 0 && index < _proxydictionary.Count)
                    channel = _proxydictionary[index].Proxy;

                return channel;
            }
        }

        protected IChannel Proxy
        {
            get
            {
                IChannel proxy = null;
                _proxyHelthymonitorTimer.Enabled = false;
                bool success = false;
                var indexsnapshot = _currentassignedIndex;

                for (int i = 0; i < _proxydictionary.Count && !success; ++i)
                {
                    if (_currentassignedIndex + 1 < _proxydictionary.Count)
                        System.Threading.Interlocked.Increment(ref _currentassignedIndex);
                    else
                        _currentassignedIndex = 0;

                    if (_proxydictionary.Count > 0)
                        proxy = _proxydictionary[_currentassignedIndex].Proxy;

                    if (null == _badProxyAddressList.FirstOrDefault<ProxyWrapper>(x => x.EndPoint.address == _proxydictionary[_currentassignedIndex].EndPoint.address))
                        success = true;
                }

                if (indexsnapshot != _currentassignedIndex
                    || (_proxydictionary.Count - _badProxyAddressList.Count == 1 && _currentassignedIndex == 0))
                    NotifyControllerStatusChanged();

                if (null == proxy)
                    throw new ApplicationException(string.Format(@"---There is no client proxy available. Source = <{0}>", GetType().Name));

                _proxyHelthymonitorTimer.Enabled = true;

                return proxy;
            }

        }

        public bool IsProxyAvailable { get { return _proxydictionary.Count > 0 && _proxydictionary.Count > _badProxyAddressList.Count; } }

        [DoubleRangeValidator(MaxVal = 600.0 * 1000.0, MinVal = Interval)]
        public double HealthyCheckInterval
        {
            get { return _healthyCheckInterval; }
            set
            {
                var currentinterval = _healthyCheckInterval;
                if (currentinterval != value)
                {
                    _healthyCheckInterval = value;
                    InitializeTimer();
                }
            }
        }

        public bool LazyConfigUpdate
        {
            get { return _configmonitor.LazyUpdate; }
            set { _configmonitor.LazyUpdate = value; }
        }

        public event EventHandler ControllerStatusChanged
        {
            add { _eventControllerStatusChanged = (EventHandler)Delegate.Combine(_eventControllerStatusChanged, value); }
            remove { _eventControllerStatusChanged = (EventHandler)Delegate.Remove(_eventControllerStatusChanged, value); }
        }

        public event EventHandler ConfigChangedRelay
        {
            add { _eventConfigChangedRelay = (EventHandler)Delegate.Combine(_eventConfigChangedRelay, value); }
            remove { _eventConfigChangedRelay = (EventHandler)Delegate.Remove(_eventConfigChangedRelay, value); }
        }

        public List<ProxyWrapper> BadEndpointList { get { return _badProxyAddressList; } }
        public int EndpointCount { get { return _proxydictionary.Count; } }
        public int CurrentAssignedIndex { get { return _currentassignedIndex; } }
        public string VerifyProxyResults { get; set; }
        public string ContractName { get { return _contractName; } }
        public object[] VerifyProxyParameters { get; set; }

        #endregion

        #region Public Methods

        public void ConfigUpdateRequest()
        {
            _configmonitor.RequestUpdate();
        }

        public endpoint EndpointInfo(int index)
        {
            endpoint ep = null;

            if (index >= 0 && index < _endpointList.Count)
                ep = _endpointList[index];

            return ep;
        }

        public static Binding WcfBindingFactory(string bindingtype, string bidingconfigname)
        {
            Binding binding = null;

            if (!string.IsNullOrEmpty(bindingtype) && !string.IsNullOrEmpty(bidingconfigname))
            {
                switch (bindingtype)
                {
                    case "basicHttpBinding":
                        binding = new BasicHttpBinding(bidingconfigname);
                        break;
                    case "basicHttpContextBinding":
                        binding = new BasicHttpContextBinding(bidingconfigname);
                        break;
                    case "wsHttpBinding":
                        binding = new WSHttpBinding(bidingconfigname);
                        break;
                    case "wsHttpContextBinding":
                        binding = new WSHttpContextBinding(bidingconfigname);
                        break;
                    case "netTcpBinding":
                        binding = new NetTcpBinding(bidingconfigname);
                        break;
                    case "netTcpContextBinding":
                        binding = new NetTcpContextBinding(bidingconfigname);
                        break;
                    case "netPeerTcpBinding":
                        binding = new NetPeerTcpBinding(bidingconfigname);
                        break;
                    case "netNamedPipeBinding":
                        binding = new NetNamedPipeBinding(bidingconfigname);
                        break;
                    case "webHttpBinding":
                        binding = new WSHttpBinding(bidingconfigname);
                        break;
                    case "wsDualHttpBinding":
                        binding = new WSDualHttpBinding(bidingconfigname);
                        break;
                    case "customBinding":
                        binding = new CustomBinding(bidingconfigname);
                        break;
                }
            }

            return binding;
        }

        #endregion

        #region Private Methods

        private void InitializeController()
        {
            InitializeTimer();

            _configmonitor = new ConfigurationChangeMonitor(!string.IsNullOrEmpty(_configPath)?_configPath:CommonUtility.GetExecutedPath());
            _wcfClientConfigProperty = new XmlDocConfigProperty { XPath = @"/configuration/system.serviceModel/client" };
            _configmonitor.Add(_wcfClientConfigProperty);
            _configmonitor.PropertyChanged += OnWcfClientPropertyChanged;

            CollectEndpointInfo();
            if (_endpointList.Count == 0)
                throw new ConfigurationErrorsException("There is no endpoint been configurated from the configuration.");
            
            HealthyCheck();

            _proxyHelthymonitorTimer.Enabled = true;
        }

        private void InitializeTimer()
        {
            var t = new Thread(() => CreateTimer());
            t.Start();
        }

        private void CreateTimer()
        {
            for (; !_canupdatetimer; ) ;

            var enabled = false;
            if (null != _proxyHelthymonitorTimer)
            {
                enabled = _proxyHelthymonitorTimer.Enabled;
                _proxyHelthymonitorTimer.Elapsed -= OnTimerTick;
            }

            _proxyHelthymonitorTimer = new System.Timers.Timer(_healthyCheckInterval);
            _proxyHelthymonitorTimer.Elapsed += OnTimerTick;
            _proxyHelthymonitorTimer.Enabled = enabled;
        }

        private void UpdateProxyDictionary(List<endpoint> EndpointList, bool usedefaultVerifier = false)
        {
            _badProxyAddressList.Clear();
            _proxydictionary.Clear();
            _proxyHelthymonitorTimer.Enabled = false;

            var idx = 0;
            EndpointList.ForEach(endpoint =>
            {
                var proxywrapper = new ProxyWrapper
                {
                    Proxy = CreateProxy(endpoint),
                    EndPoint = endpoint
                };

                UpdateBadProxyList(proxywrapper, usedefaultVerifier);

                if (!_proxydictionary.ContainsKey(idx))
                {
                    _proxydictionary.Add(idx, proxywrapper);
                    idx++;
                }
            });
            _proxyHelthymonitorTimer.Enabled = true;
        }

        void OnTimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            _canupdatetimer = false;

            HealthyCheck();

            _canupdatetimer = true;
        }

        private void HealthyCheck()
        {
            var badendpointcount = _badProxyAddressList.Count;
            var checklist = new List<ProxyWrapper>();
            checklist.AddRange(_badProxyAddressList);

            var itr = _proxydictionary.GetEnumerator();
            while (itr.MoveNext())
            {
                if (null == checklist.FirstOrDefault<ProxyWrapper>(x => x.EndPoint.address == itr.Current.Value.EndPoint.address))
                    checklist.Add(itr.Current.Value);
            }

            checklist.ForEach(p =>
            {
                UpdateBadProxyList(p, usedefaultVerifier: false);
            });

            if (_badProxyAddressList.Count != badendpointcount)
                NotifyControllerStatusChanged();
        }

        private void CollectEndpointInfo()
        {
            var xmldoc = McAfeeLabs.Engineering.Automation.Base.CommonUtility.LoadAppConfigXml();

            var xpath = @"/configuration/system.serviceModel/client/endpoint";
            if (!string.IsNullOrEmpty(_contractName))
                xpath = string.Format(@"{0}[@contract = '{1}']", xpath, _contractName);

            var xmlnodes = xmldoc.SelectNodes(xpath);
            if (null == xmlnodes || xmlnodes.Count == 0)
                throw new ArgumentException(string.Format(@"--- The web service contract <{0}> cannot be found from the configuration file.", _contractName));

            ParseEndpoints(xmlnodes);
        }

        void OnWcfClientPropertyChanged(object source, System.EventArgs args)
        {
            lock (_syncObj)
            {
                var xmldoc = (XmlDocument)_configmonitor[_wcfClientConfigProperty.Id];
                if (null != xmldoc)
                {
                    var xpath = @"/client/endpoint";
                    if (!string.IsNullOrEmpty(_contractName))
                        xpath = string.Format(@"{0}[@contract = '{1}']", xpath, _contractName); 
                    
                    var xmlnodes = xmldoc.SelectNodes(xpath);

                    if (null != xmlnodes && xmlnodes.Count > 0)
                    {
                        _endpointList.Clear();
                        _proxydictionary.Clear();
                        ParseEndpoints(xmlnodes);
                        UpdateProxyDictionary(_endpointList);
                    }
                }

                if (null != _eventConfigChangedRelay)
                    _eventConfigChangedRelay(this, null);
            }

        }

        private void ParseEndpoints(XmlNodeList nodes)
        {
            lock (_syncObj)
            {
                var idx = 0;
                foreach (XmlNode node in nodes)
                {
                    var xmldocEndpoint = new XmlDocument();
                    xmldocEndpoint.LoadXml(node.OuterXml);
                    var endpoint = CommonUtility.XmlRetrieve<endpoint>(xmldocEndpoint);

                    if (null == _endpointList.FirstOrDefault<endpoint>(x => x.address == endpoint.address || x.name == endpoint.name))
                    {
                        _endpointList.Add(endpoint);

                        _proxydictionary.Add(idx, new ProxyWrapper
                        {
                            Proxy = CreateProxy(endpoint),
                            EndPoint = endpoint
                        });
                        ++idx;

                    #if LOG // TODO : Remove when .NET 4.0 log manager is used 
                    _logger.Debug(string.Format("Configuration end point address {0}", endpoint.address));
                    #endif
                    }
                    else
                    {
                        var message = string.Format(@"Duplicate endpoint configuration name = <{0}>, address = <{1}> detected", endpoint.name, endpoint.address);
                    #if LOG // TODO : Remove when .NET 4.0 log manager is used 
                    _logger.Debug(message);
                    #endif
                        throw new ConfigurationErrorsException(message);
                    }
                }

            #if LOG // TODO : Remove when .NET 4.0 log manager is used 
            if (_endpointList.Count == 0)
                _logger.Warning(string.Format(@"--- There is no endpoint configuration found.  source = <{0}>", GetType().Name));
            #endif
            }
        }

        private void NotifyControllerStatusChanged()
        {
            if (null != _eventControllerStatusChanged)
                _eventControllerStatusChanged(this, null);
        }

        protected void UpdateBadProxyList(ProxyWrapper proxywrapper, bool usedefaultVerifier = false)
        {
            var success = false;
            bool isInbadlist = null != _badProxyAddressList.FirstOrDefault<ProxyWrapper>(w => w.EndPoint.address == proxywrapper.EndPoint.address);

            if (usedefaultVerifier)
                success = RetrieveMetadata(proxywrapper);
            else
                success = VerifyProxy(proxywrapper);

            if (!(success || isInbadlist))
                _badProxyAddressList.Add(proxywrapper);
            else if (success && isInbadlist)
                _badProxyAddressList.Remove(proxywrapper);
        }

        protected bool RetrieveMetadata(ProxyWrapper proxywrapper)
        {
            var success = true;
            MetadataSet metadataset = null;
            bool isInbadlist = null != _badProxyAddressList.FirstOrDefault<ProxyWrapper>(w => w.EndPoint.address == proxywrapper.EndPoint.address);
            if (proxywrapper.Proxy != null)
            {
                try
                {
                    var address = string.Format(@"{0}?wsdl", proxywrapper.EndPoint.address);
                    var mexClient = new MetadataExchangeClient(new Uri(address), MetadataExchangeClientMode.HttpGet);
                    metadataset = mexClient.GetMetadata();
                }
                catch (Exception e)
                {
                    #if LOG // TODO : Remove when .NET 4.0 log manager is used 
                    _logger.Warning(e.Message);
                    #endif
                    success = false;
                }
            }
            if (null != metadataset)
            {
                var sb = new StringBuilder();
                foreach (var section in metadataset.MetadataSections)
                    sb.AppendLine(string.Format(@"{0}{1}", section.Identifier.ToString(), Environment.NewLine));
                VerifyProxyResults = sb.ToString();
            }

            return success;
        }

        #endregion

        #region Abstract Methods

        abstract protected IChannel CreateProxy(endpoint endpoint);
        abstract public bool VerifyProxy(ProxyWrapper proxywrapper);

        #endregion
    }
}