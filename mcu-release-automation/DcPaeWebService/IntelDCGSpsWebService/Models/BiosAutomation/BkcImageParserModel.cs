using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Newtonsoft.Json;
using WindowService.DataModel;

namespace IntelDCGSpsWebService.Models
{
    public class BkcImageParserModel : BiosAutomationModel
    {
        private string _uploadedFile = string.Empty;
        private IfwiBkcParserViewType _selectedViewType = IfwiBkcParserViewType.EmbeddedConfig;
        private string _selectedConfigJson = string.Empty;
        private Dictionary<string, string> _embeddedJsonDict = new Dictionary<string, string>();
        private string _selectedEmbeddedJson = string.Empty;

        public BkcImageParserModel()
        {
        }

        public BkcImageParserModel(string configJsonFolder) : this()
        {
            for (int i = 0; i < 3; ++i)
            {
                _embeddedJsonDict.Clear();
                var dir = Path.Combine(configJsonFolder, @"Json");
                if (Directory.Exists(dir))
                {
                    foreach (var file in Directory.GetFiles(dir))
                    {
                        _embeddedJsonDict.Add(Path.GetFileName(file), file);
                    }
                    SelectedEmbeddedJson = _embeddedJsonDict.Keys.First();
                }
            }
        } 
        
        public string SelectedParserViewType
        {
            get { return this._selectedViewType.ToString(); }
            set
            {
                _selectedViewType = (IfwiBkcParserViewType)Enum.Parse(typeof(IfwiBkcParserViewType), value);
            }
        }

        [Required(ErrorMessage = "* Please select the config json file.")]
        [Display(Name = "Config Json File:")]
        public string SelectedConfigJson
        {
            get { return _selectedConfigJson; }
            set { _selectedConfigJson = value; }
        }

        public string UploadedFile
        {
            get { return _uploadedFile; }
            set { _uploadedFile = value; }
        }

        public Dictionary<string, string> EmbeddedJsonDictionary { get { return _embeddedJsonDict; } }

        public string SelectedEmbeddedJson
        {
            get { return _selectedEmbeddedJson; }
            set
            {
                _selectedEmbeddedJson = _selectedConfigJson = _embeddedJsonDict[value];
            }
        }

        public bool IsIfwiFileAvailable { get { return !string.IsNullOrEmpty(_uploadedFile); } }
    }
    [Serializable]
    public class FDBinConfig
    {
        public IfwiBuild IfwiBuild { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class IfwiBuild
    {
        public string Input { get; set; }
        public string SourceBkcFileNamePattern { get; set; }
        public string ExtractedBiosImage { get; set; }
        public string BKCBinaries { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}