using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelDCGSpsWebService.Models
{
    public abstract class SpsToolsModel 
    {
        protected string _title = string.Empty;
        public SpsToolsModel() { }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
    }
}