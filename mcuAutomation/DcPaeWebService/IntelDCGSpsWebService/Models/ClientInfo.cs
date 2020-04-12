using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelDCGSpsWebService.Models
{
    [Serializable]
    public class ClientInfo
    {
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public string DomainName { get; set; }
    }
}