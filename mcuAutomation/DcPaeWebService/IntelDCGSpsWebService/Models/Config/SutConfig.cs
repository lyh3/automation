using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IntelDCGSpsWebService.Models
{
    [Serializable]
    public class SutConfig
    {
        [Required(ErrorMessage = "* Please enter SUT IP address.")]
        [Display(Name = "SUT IP Arress:")]
        public string IpAddress { get; set; }
        [Display(Name = "SUT Connected:")]
        public bool IsPowerOn { get; set; }
        [Display(Name = "Abort automation when failure been captured")]
        public bool ifAboutAtFailure { get; set; }
    }
}