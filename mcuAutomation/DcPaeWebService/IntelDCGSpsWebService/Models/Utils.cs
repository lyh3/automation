using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.NetworkInformation;

namespace IntelDCGSpsWebService.Models
{
    static public class Utils
    {
        public static PingReply PingIPAddress(string ip)
        {
            int timeout = 2000;
            IPAddress address;
            PingReply replay = null;
            if (!string.IsNullOrEmpty(ip) && IPAddress.TryParse(ip, out address))
            {
                var pingSender = new Ping();
                // Set options for transmission:
                // The data can go through 64 gateways or routers
                // before it is destroyed, and the data packet
                // cannot be fragmented.
                var options = new PingOptions(64, true);
                byte[] buffer = Encoding.ASCII.GetBytes("Hello");
                try
                {
                    replay = pingSender.Send(ip, timeout, buffer, options);
                }
                catch(Exception)
                { 
                }               
            }
            return replay;
        }

        public static bool IsValidIpV4(string ip)
        {
            var isvalid = false;
            var regex = new Regex(@"(?=(^\s*(https?:(\/\/))?(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4})::([0-9a-fA-F]{4})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{3}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{1}):([0-9a-fA-F]{3}):([0-9a-fA-F]{4}):([0-9a-fA-F]{1}):([0-9a-fA-F]{1}):([0-9a-fA-F]{1})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{3}):([0-9a-fA-F]{3}):([0-9a-fA-F]{3}):([0-9a-fA-F]{4}):([0-9a-fA-F]{1}):([0-9a-fA-F]{1}):([0-9a-fA-F]{3})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{3}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{1})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{3}):([0-9a-fA-F]{3}):([0-9a-fA-F]{3}):([0-9a-fA-F]{4}):([0-9a-fA-F]{1}):([0-9a-fA-F]{1}):([0-9a-fA-F]{2})])|(\[([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4}):([0-9a-fA-F]{4})])(:\d{4})?\s*$)|(^\s*(((https?(?![0-9][a-zA-Z]):)?(//)((w{3}?).)?)?)([\w-]+\.)+[\w-]+(\/[\w- ./?%&amp;=]*)?\s*$))");
            if(!string.IsNullOrEmpty(ip) && regex.Match(ip).Success)
            {
                isvalid = true;
            }
            return isvalid;
        }
    }
}