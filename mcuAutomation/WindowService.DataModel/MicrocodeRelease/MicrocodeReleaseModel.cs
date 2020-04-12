using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Configuration;
using Newtonsoft.Json;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    [Serializable]
    public class MicrocodeReleaseModel
    {
        protected string _title = string.Empty;
        private string _errorMessage = string.Empty;
        private string _notificationMessage = string.Empty;
        private string _hsdArticleIDs = string.Empty;
        private List<ReleaseLee> _releaseLeeList = new List<ReleaseLee>();
        private ReleaseLee _selectedReleaseLee = null;
        private TransactionStatus _tran = TransactionStatus.Idle;
        private ReleaseSource _releaseMcuSource = ReleaseSource.HSD;
        private ReadmeDocument _doc = null;

        public MicrocodeReleaseModel()
        {
            var readmeTemplate = string.Format(@"{0}\README_Template.md", ConfigurationManager.AppSettings["MicrocodeReleaseWorkingDirectory"]);
            _doc = new ReadmeDocument(readmeTemplate);
        }
        public MicrocodeReleaseModel(ReleaseLee releaseLee) : this()
        {
            _releaseLeeList.Add(releaseLee);
        }
        public ReleaseLee this[string releaseto]
        { get { return _releaseLeeList.FirstOrDefault<ReleaseLee>(x => x.ReleaseTo == releaseto); } }
        public ReleaseLee[] ReleaseLees
        {
            get { return _releaseLeeList.ToArray(); }
            set
            {
                _releaseLeeList.Clear();
                _releaseLeeList.AddRange(value);
            }
        }
        public void UpdateReleaseLee(ReleaseLee release)
        {
            if(null != release)
            {
                var releaseLeeList = new List<ReleaseLee>();
                foreach (ReleaseLee r in _releaseLeeList)
                {
                    if (r.ReleaseTo != release.ReleaseTo)
                    {
                        releaseLeeList.Add(r);
                    }
                    else
                    {
                        releaseLeeList.Add(release);
                    }
                }
                this.ReleaseLees = releaseLeeList.ToArray();
            }
        }
        [JsonIgnore]
        public string ReleaseMcuSource
        {
            get { return _releaseMcuSource.ToString(); }
            set
            {
                _releaseMcuSource = (ReleaseSource)Enum.Parse(typeof(ReleaseSource), value);
            }
        }
        [JsonIgnore]
        public string ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; } }
        [JsonIgnore]
        public string TransStatus
        {
            get { return _tran.ToString(); }
            set
            {
                _tran = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), value);
            }
        }
        [JsonIgnore]
        public string NotificationMessage
        {
            get { return _notificationMessage; }
            set { _notificationMessage = value; }
        }
        [JsonIgnore]
        public ReleaseLee SelectedReleaseLee
        {
            get { return _selectedReleaseLee; }
            set
            {
                _selectedReleaseLee = value;
            }
        }
        [JsonIgnore]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        [JsonIgnore]
        public bool ProcessFailed { get; set; }
        [JsonIgnore]
        [Display(Name = "Release to name:")]
        public string NewReleaseToName { get; set; }
        [JsonIgnore]
        public string HsdArticleIDs {
            get { return _hsdArticleIDs; }
            set { _hsdArticleIDs = value; }
        }

        public void UpdateVolunteerInputs(  )
        {
            foreach (var releaseLee in _releaseLeeList)
            {
                foreach (McuRelease mcu in releaseLee.Mcus)
                {
                    if(string.IsNullOrEmpty(mcu.CpuID) || string.IsNullOrEmpty(mcu.CpuSegment))
                    {
                        continue;
                    }
                    var agreegation = _doc.EntriesByCpuIdAndCpuSegment(mcu.CpuID, mcu.CpuSegment);
                    mcu.SteppingOptions = agreegation[McuTableColumn.CpuCoreStepping.ToString()];
                    mcu.McuFileNameOptions = agreegation[McuTableColumn.McuFileName.ToString()];
                    mcu.CpuCodeNameOptions = agreegation[McuTableColumn.CpuCodeName.ToString()];
                    mcu.IsReademReferenceAvailable = true;
                }
            }
        }
        public string AddReleaseLee(string releaseto)
        {
            ReleaseLee releaselee = null;
            var msg = string.Empty;
            if (!string.IsNullOrEmpty(releaseto) && null == this[releaseto])
            {
                releaselee = new ReleaseLee
                {
                    ReleaseTo = releaseto
                };
                releaselee.AddNewRelease(releaseto);
                _releaseLeeList.Add(releaselee);
                _selectedReleaseLee = releaselee;
                return string.Format("Add the releae to [ {0} ] success.", releaseto);
            }
            return string.Format("The releae to [ {0} ] alrady exists.", releaseto);
        }
        public string RemoveReleaseLee(string releaseto)
        {
            var release = this[releaseto];
            if (release != null)
            {
                var releaseTo = release.ReleaseTo;
                _releaseLeeList.Remove(release);
                _selectedReleaseLee = null;
                return string.Format("The releae to [ {0} ] has been removed successfully.", releaseTo);
            }
            return string.Format("Cannot find the specified releae to [ {0} ] for removing.", releaseto);
        }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }

    [Serializable]
    public class ReleaseLee
    {
        private TransactionStatus _status = TransactionStatus.Idle;
        private string _releaseTo = string.Empty;
        private List<McuRelease> _mcuReleaseList = new List<McuRelease>(Definitions.MAX_ONE_TIME_MCU_RELEASE);
        public string status
        {
            get { return _status.ToString(); }
            set
            {
                _status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), value);
            }
        }
        public string ReleaseTo
        {
            get { return _releaseTo; }
            set { _releaseTo = value; }
        }
        public McuRelease this[string mcu]
        {
            get { return _mcuReleaseList.FirstOrDefault<McuRelease>(x => x.Mcu == mcu); }
        }
        public McuRelease[] Mcus
        {
            get { return _mcuReleaseList.ToArray(); }
            set
            {
                _mcuReleaseList.Clear();
                _mcuReleaseList.AddRange(value);
            }
        }

        public void UpdateMcu(McuRelease mcu)
        {
            if (null != mcu)
            {
                var mcuList = new List<McuRelease>();
                foreach(McuRelease m in _mcuReleaseList)
                {
                    if(m.Mcu != mcu.Mcu)
                    {
                        mcuList.Add(m);
                    }
                    else
                    {
                        mcuList.Add(mcu);
                    }
                }
                this.Mcus = mcuList.ToArray();
            }

        }
        public string AddNewRelease(string releaseTo)
        {
            var msg = string.Empty;
            var idx = _mcuReleaseList.Count;
            if (!(idx + 1 > Definitions.MAX_ONE_TIME_MCU_RELEASE))
            {
                var mcu = string.Format(Definitions.MCU_DEFAULT_FORMAT, Definitions.GENERIC_MCU_BINARY_NAME, idx);
                _mcuReleaseList.Add(new McuRelease(mcu, releaseTo));
                msg = string.Format("A mcu request has been added to - {0}. Please provide the required release information.", ReleaseTo);
            }
            else
            {
                msg = string.Format("<b>CAUTION</b> : Sorry, the maximum supported mcu at one time release is <b>{0}</b>.", Definitions.MAX_ONE_TIME_MCU_RELEASE);
            }
            return msg;
        }
        public void RemoveReleases()
        {
            try
            {
                foreach (var x in _mcuReleaseList)
                {
                    if (x.Selected)
                    {
                        _mcuReleaseList.Remove(x);
                    }
                }
            }
            catch (Exception ex)
            {
                //MvcApplication.Logger.WarnFormat(@"Exception cauth at RemoveReleases, error = {0}", ex.Message);
            }

        }
        public void RemoveRelease(McuRelease release)
        {
            if (null != this[release.Mcu])
            {
                _mcuReleaseList.Remove(release);
            }
        }
        public void UpdateMcu(McuRelease mcu, string mcuBinary)
        {
            foreach (var x in _mcuReleaseList)
            {
                if (x == mcu)
                {
                    x.Mcu = mcuBinary;
                    break;
                }
            }
        }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }

    [Serializable]
    public class McuRelease : IComparable<McuRelease>
    {
        private string _mcu = string.Empty;
        private bool _selectOptionalParameters = false;
        private List<string> _steppingOptions = new List<string>();
        private List<string> _mcuFileNameOptions = new List<string>();
        private List<string> _cpuCodeNameOptions = new List<string>();
        public McuRelease()
        {
            this.Scope = "RestrictedPkg";
            CPUCodeName = string.Empty;
            CpuSegment = DataModel.CpuSegment.Desktop.ToString();
            ReleaseTarget = @"debug";
            Stepping = string.Empty;
            CpuID = string.Empty;
            PlatformID = string.Empty;
            MicroCode = string.Empty;
            PkgPath = string.Empty;
        }
        public McuRelease(string mcu, string releaseTo) : this()
        {
            _mcu = mcu;
            if (releaseTo == "NDA" || releaseTo == "Public")
            {
                this.Scope = releaseTo;
            }
        }
        private bool _selected = false;
        [Display(Name = "*MCU file name")]
        public string Mcu
        {
            get { return _mcu; }
            set { _mcu = value; }
        }
        [Display(Name = "*CPU code name:")]
        public string CPUCodeName { get; set; }
        [DefaultValue("Desktop")]
        [Display(Name = "*CPU segment:")]
        public string CpuSegment { get; set; }
        [DefaultValue("debug")]
        [Display(Name = "*Release target:")]
        public string ReleaseTarget { get; set; }
        [Display(Name = "*Stepping:")]
        public string Stepping { get; set; }
        [Display(Name = "*Scope (read only):")]
        public string Scope { get; set; }
        [Display(Name = "*CPU ID (if blank, will read from Header):")]
        public string CpuID { get; set; }
        [Display(Name = "*Platform ID:")]
        public string PlatformID { get; set; }
        [Display(Name = "*Microcode file name:")]
        public string MicroCode { get; set; }
        public string PkgPath { get; set; }
        [Display(Name = "CPU Public Spec Update:")]
        [DefaultValue("https://ark.intel.com/content/www/us/en/ark.html")]
        public string CpuPublicSpecUpdate { get; set; }
        [Display(Name = "Intel Product Spec:")]
        [DefaultValue("https://ark.intel.com/content/www/us/en/ark.html")]
        public string IntelProductSpec { get; set; }
        [Display(Name = "CPU NDA Spec Update:")]
        [DefaultValue("https://ark.intel.com/content/www/us/en/ark.html")]
        public string CpuNdaSpecUpdate { get; set; }
        [DefaultValue("N/A")]
        [Display(Name = "Products:")]
        public string Products { get; set; }
        [Display(Name = "Process Model:")]
        [DefaultValue("N/A")]
        public string ProcessorModel { get; set; }


        [Display(Name = "Select for removing:")]
        [JsonIgnore]
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
        [JsonIgnore]
        public bool SelectOptionalParameters
        {
            get { return _selectOptionalParameters; }
            set { _selectOptionalParameters = value; }
        }
        [JsonIgnore]
        public bool IsReademReferenceAvailable { get; set; }
        public int CompareTo(McuRelease other)
        {
            return (CPUCodeName == other.CPUCodeName ? 1 : 0) &
                    (CpuSegment == other.CpuSegment ? 1 : 0) &
                    (ReleaseTarget == other.ReleaseTarget ? 1 : 0) &
                    (Stepping == other.Stepping ? 1 : 0) &
                    (Scope == other.Scope ? 1 : 0) &
                    (CpuID == other.CpuID ? 1 : 0) &
                    (PlatformID == other.PlatformID ? 1 : 0) &
                    (MicroCode == other.MicroCode ? 1 : 0);
        }
        [JsonIgnore]
        public string[] CpuSegments
        {           
            get
            {
                var cpusegments = new List<string>();
                if (!string.IsNullOrEmpty(this.CpuSegment))
                {
                    cpusegments.Add(this.CpuSegment);
                }
                for (var i = 0; i < (int)DataModel.CpuSegment.MAX_CPU_SEGMENT; i++)
                {
                    var s = ((DataModel.CpuSegment)i).ToString();
                    if(this.CpuSegment.Equals(s))
                    {
                        continue;
                    }
                    cpusegments.Add(s);
                }
                return cpusegments.ToArray();
            }
        }
        [JsonIgnore]
        public string[] ReleaseTargets
        {
            get
            {
                var defaultData = new string[] { "debug", "production", "alpha", "beta" };
                var releaseTarget = new List<string>();
                if (!string.IsNullOrEmpty(this.ReleaseTarget))
                {
                    releaseTarget.Add(this.ReleaseTarget);
                }
                foreach(var s in defaultData)
                {
                    if (s.Equals(this.ReleaseTarget))
                    {
                        continue;
                    }
                    releaseTarget.Add(s);
                }
                return releaseTarget.ToArray();
            }
        }
        [JsonIgnore]
        public string[] Scopes
        {
            get { return new string[] { "RestrictedPkg", "NDA", "public" }; }
        }

        [JsonIgnore]
        public string[] PlatformID_1
        {
            get { return new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" }; }
        }

        [JsonIgnore]
        public string[] PlatformID_2
        {
            get { return new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" }; }
        }
        [JsonIgnore]
        public string[] SteppingOptions
        {
            get {return _steppingOptions.ToArray();}
            set { _steppingOptions.Clear(); _steppingOptions.AddRange(value); }
        }
        [JsonIgnore]
        public string[] McuFileNameOptions
        {
            get { return _mcuFileNameOptions.ToArray(); }
            set { _mcuFileNameOptions.Clear(); _mcuFileNameOptions.AddRange(value); }
        }
        [JsonIgnore]
        public string[] CpuCodeNameOptions
        {
            get { return _cpuCodeNameOptions.ToArray(); }
            set { _cpuCodeNameOptions.Clear(); _cpuCodeNameOptions.AddRange(value); }
        }

        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}