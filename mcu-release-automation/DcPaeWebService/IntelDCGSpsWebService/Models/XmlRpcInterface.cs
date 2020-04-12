using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CookComputing.XmlRpc;

namespace IntelDCGSpsWebService.Models
{
    [XmlRpcUrl("http://localhost:8001/RPC2")]
    public interface IXmlRpcInterface : IXmlRpcProxy
    {
        [XmlRpcMethod]
        dynamic PingHost(string ipAddress);
    }
}