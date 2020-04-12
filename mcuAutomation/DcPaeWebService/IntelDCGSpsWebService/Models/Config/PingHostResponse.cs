using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelDCGSpsWebService.Models
{
    public class PingHostResponse
    {
        public bool hostUp { get; set; }
        public bool updateStatus { get; set; }
    }
}