using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    public class BiosImageOperationsModel : BiosAutomationModel
    {
        private string _biosImage = string.Empty;
        private BiosImageOperationType _selectedOperationType = BiosImageOperationType.ExtractBios;
        protected Dictionary<BiosOperationFileType, string> _uploadedFiles = new Dictionary<BiosOperationFileType, string>();

        public BiosImageOperationsModel() { }

        [Required(ErrorMessage = "* Please select BIOS image file name.")]
        [Display(Name = "BIOS Image File:")]
        public string BiosImage
        {
            get { return _biosImage; }
            set { _biosImage = value; }
        }
        public string OperationType
        {
            get { return this._selectedOperationType.ToString(); }
            set
            {
                _selectedOperationType = (BiosImageOperationType)Enum.Parse(typeof(BiosImageOperationType), value);
            }
        }

        public Dictionary<BiosOperationFileType, string> UploadedFiles
        {
            get { return _uploadedFiles; }
            set
            { _uploadedFiles = value; }
        }
        public string ComposedCommandParameters
        {
            get
            {
                var parameters = string.Empty;
                switch (_selectedOperationType)
                {
                    case BiosImageOperationType.MergeBios:
                        parameters = string.Format(" -f mergeBios -ifwi {3}{0}{3} -bios {3}{1}{3} -o {3}{2}{3}", _ifwiImage, _biosImage, _outputName, '"');
                        break;
                    case BiosImageOperationType.SwapBios:
                        parameters = string.Format(" -f swapBios -ifwi {3}{0}{3} -bios {3}{1}{3} -o {3}{2}{3}", _ifwiImage, _biosImage, _outputName, '"');
                        break;
                    default:
                        parameters = string.Format(" -f extractBios -ifwi {2}{0}{2} -o {2}{1}{2}", _ifwiImage, _outputName, '"');
                        break;
                }
                return parameters;
            }
        }

        public void Reset()
        {
            var itr = _uploadedFiles.GetEnumerator();
            while (itr.MoveNext())
            {
                try
                {
                    if (System.IO.File.Exists(itr.Current.Value))
                        System.IO.File.Delete(itr.Current.Value);
                }
                catch { }
            }

            _uploadedFiles.Clear();
            _resultsFile = string.Empty;
        }
        public bool IsSpiFilesAvailable { get { return _uploadedFiles.Count != 0; } }
        override public bool IsDownloadAvailable
        {
            get
            {
                return !string.IsNullOrEmpty(_resultsFile)
                       && File.Exists(_resultsFile)
                       && (new FileInfo(_resultsFile)).Length > 0;
            }
        }
    }
}