using McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy.Schema;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace McAfeeLabs.Engineering.Automation.Base.SmartWebserviceProxy
{
    public class ProxyWrapper
    {
        public IChannel Proxy { get; set; }
        public endpoint EndPoint { get; set; }
    }
}
