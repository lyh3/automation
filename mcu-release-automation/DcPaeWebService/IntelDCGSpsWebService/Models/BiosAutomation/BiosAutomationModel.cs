using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace IntelDCGSpsWebService.Models
{
    public class BiosAutomationModel
    {
        protected string _title = string.Empty;
        protected string _ifwiImage = string.Empty;
        protected string _outputName = string.Empty;
        protected string _resultsFile = string.Empty;

        public BiosAutomationModel() { }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        [Required(ErrorMessage = "* Please enter the output file name.")]
        [Display(Name = "Output File:")]
        public string OutputName
        {
            get { return _outputName; }
            set { _outputName = value; }
        }
        [Required(ErrorMessage = "* Please select IFWI image file name.")]
        [Display(Name = "IFWI Image File:")]
        public string IfwiImage
        {
            get { return _ifwiImage; }
            set { _ifwiImage = value; }
        }

        public string ResultsFile
        {
            get { return _resultsFile; }
            set
            {
                _resultsFile = value;
            }
        }

        virtual public bool IsDownloadAvailable
        {
            get
            {
                return !string.IsNullOrEmpty(_resultsFile)
                       && File.Exists(_resultsFile);
            }
        }

        public bool ProcessFailed { get; set; }
    }
}