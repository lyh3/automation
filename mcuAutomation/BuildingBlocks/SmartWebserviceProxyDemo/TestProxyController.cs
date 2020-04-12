using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

using McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy.Schema;
using McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy;
using SmartWebserviceProxyDemo.SSCClient;
using SmartWebserviceProxyDemo.CasperScanSoapClient;
using SmartWebserviceProxyDemo.MetadatServiceClient;

namespace SmartWebserviceProxyDemo
{
    public class TestSSCProxyController : SmartProxyController, IDynamicProxy
    {
        public TestSSCProxyController(string contractName):base(contractName){}

        public dynamic ServiceProxy { get { return Proxy as ISampleSubmission; } }
        public bool IsServiceAvailable { get { return base.IsProxyAvailable; } }

        override public bool VerifyProxy(ProxyWrapper proxywrapper)
        {
            //return RetrieveMetadata(proxywrapper);
            try
            {
                var serviceStatus = (proxywrapper.Proxy as ISampleSubmission).ServiceStatus(new ServiceStatusRequest());
                return serviceStatus != null
                       && serviceStatus.ServiceStatus.systemstatus != null
                       && serviceStatus.ServiceStatus.systemstatus.systemstatus == statusType.OK;
            }
            catch { return false; }
        }

        protected override IChannel CreateProxy(endpoint endpoint)
        {
            var channelfactory = new ChannelFactory<ISampleSubmission>(WcfBindingFactory(endpoint.binding, endpoint.bindingConfiguration), new EndpointAddress(endpoint.address));
            return channelfactory.CreateChannel() as IChannel;
        }
    }

    public class TestCasperScanProxyController : SmartProxyController, IDynamicProxy
    {
        public TestCasperScanProxyController(string contractName) : base(contractName) { }

        public dynamic ServiceProxy { get { return Proxy as CasperScanSoap; } }
        public bool IsServiceAvailable { get { return base.IsProxyAvailable; } }

        override public bool VerifyProxy(ProxyWrapper proxywrapper)
        {
            var success = true;
            success = RetrieveMetadata(proxywrapper);
            return success;
        }

        protected override IChannel CreateProxy(endpoint endpoint)
        {
            var channelfactory = new ChannelFactory<CasperScanSoap>(WcfBindingFactory(endpoint.binding, endpoint.bindingConfiguration), new EndpointAddress(endpoint.address));
            return channelfactory.CreateChannel() as IChannel;
        }
    }

    public class TestMetaDataServiceProxyController : SmartProxyController, IDynamicProxy
    {
        public TestMetaDataServiceProxyController(string contractName) : base(contractName) { }

        public dynamic ServiceProxy { get { return Proxy as IMetaDataService; } }
        public bool IsServiceAvailable { get { return base.IsProxyAvailable; } }

        override public bool VerifyProxy(ProxyWrapper proxywrapper)
        {
            var success = true;

            //success = RetrieveMetadata(proxywrapper);

            try 
            {
                var response = (proxywrapper.Proxy as IMetaDataService).GetMetadata((string[])VerifyProxyParameters[0],
                                                                                            (string)VerifyProxyParameters[1],
                                                                                            (bool)VerifyProxyParameters[2]);
                if (null != response)
                {
                    VerifyProxyResults = (response as metadataResponse).resultMessage;
                }
            }
            catch { success = false; }

            return success;
        }

        protected override IChannel CreateProxy(endpoint endpoint)
        {
            var channelfactory = new ChannelFactory<IMetaDataService>(WcfBindingFactory(endpoint.binding, endpoint.bindingConfiguration), new EndpointAddress(endpoint.address));
            return channelfactory.CreateChannel() as IChannel;
        }
    }
}
