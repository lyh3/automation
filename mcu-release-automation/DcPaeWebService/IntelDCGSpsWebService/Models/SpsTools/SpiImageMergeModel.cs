using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    public class SpiImageMergeModel : SpsToolsModel
    {
        private string _sourceImage = string.Empty;
        private string _regionImage = string.Empty;
        private string _mapFile = string.Empty;
        private string _regionName = string.Empty;
        private string _outputName = string.Empty;
        private string _resultsFile = string.Empty;
        private Dictionary<SpiMergeFileType, string> _uploadedFiles = new Dictionary<SpiMergeFileType, string>();

        public SpiImageMergeModel() 
        {
            base._title = "SPI Image Merge";
        }
        [Required(ErrorMessage = "* Please select source image file name.")]
        [Display(Name = "Source Image File:")]
        public string SourceImage
        {
            get { return _sourceImage; }
            set { _sourceImage = value; }
        }
        [Required(ErrorMessage = "* Please select region image file name.")]
        [Display(Name = "Region Image File:")]
        public string RegionImage
        {
            get { return _regionImage; }
            set { _regionImage = value; }
        }
        [Required(ErrorMessage = "* Please select the map file name.")]
        [Display(Name = "Output Region Map File:")]
        public string MapFile
        {
            get { return _mapFile; }
            set { _mapFile = value; }
        }
        [Required(ErrorMessage = "* Please specificy the region to update.")]
        [Display(Name = "Region Name:")]
        public string RegionName
        {
            get { return _regionName; }
            set { _regionName = value; }
        }
        [Required(ErrorMessage="* Please enter the output file name.")]
        [Display(Name = "Output File:")]
        public string OutputName
        {
            get { return _outputName; }
            set { _outputName = value; }
        }
        public string ResultsFile
        {
            get { return _resultsFile; }
            set { _resultsFile = value; }
        }
        public Dictionary<SpiMergeFileType, string> UploadedFiles
        {
            get { return _uploadedFiles; }
            set 
            { _uploadedFiles = value; }
        }
        public void Reset()
        {
            var itr = _uploadedFiles.GetEnumerator();
            while(itr.MoveNext())
            {
                try
                {
                    if (System.IO.File.Exists(itr.Current.Value))
                        System.IO.File.Delete(itr.Current.Value);
                }
                catch { }
            }

            //if (System.IO.File.Exists(_resultsFile))
            //    System.IO.File.Delete(_resultsFile);

            _uploadedFiles.Clear();
            _resultsFile = string.Empty;
        }
        public bool IsSpiFilesAvailable { get { return _uploadedFiles.Count != 0; } }
    }
}