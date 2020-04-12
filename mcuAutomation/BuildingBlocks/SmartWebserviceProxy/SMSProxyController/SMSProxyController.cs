using System.ServiceModel;
using System.ServiceModel.Channels;

using Microsoft.Practices.Unity;
using McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy.Schema;
using McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient;

namespace McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy
{
    public class SMSProxyController : SmartProxyController, IDynamicProxy
    {
        [InjectionConstructor]
        public SMSProxyController(string contractName) : base(contractName) { }
        public dynamic ServiceProxy { get { lock (_syncObj) return Proxy as SampleManagementServiceSoap; } }
        public bool IsServiceAvailable { get { return base.IsProxyAvailable; } }

        override public bool VerifyProxy(ProxyWrapper proxywrapper)
        {
            var success = true;
            success = RetrieveMetadata(proxywrapper);
            return success;
        }

        public RetrieveSamplesResponse RetrieveSamples(RetrieveSamplesRequest request, SampleManagementServiceSoap seriveProxy = null)
        {
            dynamic proxy = seriveProxy != null ? seriveProxy : ServiceProxy;
            return proxy.RetrieveSamples(request);
        }

        protected override IChannel CreateProxy(endpoint endpoint)
        {
            var channelfactory = new ChannelFactory<SampleManagementServiceSoap>(WcfBindingFactory(endpoint.binding, endpoint.bindingConfiguration), new EndpointAddress(endpoint.address));
            return channelfactory.CreateChannel() as IChannel;
        }
    }
}
