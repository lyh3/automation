using System;
using System.ComponentModel.DataAnnotations;
using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    [Serializable]
    public class BMCConfig
    {
        private string _bmcIpAddress = string.Empty;
        private string _user = string.Empty;
        private string _password = string.Empty;
        private bool _ipmiAccessable = true;
        private bool _connected = false;
        [Required(ErrorMessage = "* Please enter BMC IP address.")]
        [Display(Name = "BMC IP Arress:")]
        public string BmcIpAddress 
        {
            get { return _bmcIpAddress; }
            set { _bmcIpAddress = value; }
        }
        [Required(ErrorMessage = "* Please enter BMC user name.")]
        [Display(Name = "BMC User Name:")]
        public string User 
        {
            get { return _user; }
            set { _user = value; }
        }
        [Required(ErrorMessage = "* Please enter BMC Password.")]
        [Display(Name = "BMC Password:")]
        public string Password 
        {
            get { return _password; }
            set { _password = value; }
        }
        [Display(Name = "BMC Connected:")]
        public bool Connected 
        {
            get { return _connected; }
            set { _connected = value; }
        }
        public bool IpmiAccessable 
        {
            get { return _ipmiAccessable; }
            set { _ipmiAccessable = value; }
        }
        private MeMode _meMode;
        public string MEMode
        {
            get { return _meMode.ToString(); }
            set { _meMode = (MeMode)Enum.Parse(typeof(MeMode), value); }
        }
        public bool CanSubmit
        {
            get
            {
                return !string.IsNullOrEmpty(BmcIpAddress) 
                     && Utils.IsValidIpV4(BmcIpAddress)
                     && !string.IsNullOrEmpty(User)
                     && !string.IsNullOrEmpty(Password);
            }
        }
    }
}