using System;
using Newtonsoft.Json;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    [Serializable]
    public class McuSelectInfo
    {
        public bool IsChecked { get; set; }
        public string Id { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class McuInfo
    {
        public string SelectedMcu { get; set; }
        public string ReleaseTo { get; set; }
    }
    [Serializable]
    public class CpuCodeNametInfo : McuInfo
    {
        public string CpuCodeName { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class SteppingInfo : McuInfo
    {
        public string Stepping { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class CpuIdInfo : McuInfo
    {
        public string CpuID { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class PlatformIdInfo : McuInfo
    {
        private string _id1 = string.Empty;
        private string _id2 = string.Empty;
        //public string PlatformID { get; set; }
        public string PlatformID { get { return string.Format(@"{0}{1}", _id1, _id2); } }
        public string PlatformID_1 { get { return _id1; } set { _id1 = value; } }
        public string PlatformID_2 { get { return _id2; } set { _id2 = value; } }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }

    [Serializable]
    public class MicrocodeInfo : McuInfo
    {
        public string Microcode { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class CpuSegmentInfo : McuInfo
    {
        public string CpuSegment { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class ReleaseTargetInfo : McuInfo
    {
        public string ReleaseTarget { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class ScopeInfo : McuInfo
    {
        public string Scope { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class CpuPubliSpecUpdateInfo : McuInfo
    {
        public string CpuPubliSpecUpdate { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class IntelProductSpecInfo : McuInfo
    {
        public string IntelProductSpec { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class CpuNDASpecUpdateInfo : McuInfo
    {
        public string CpuNDASpecUpdate { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class ProductsInfo : McuInfo
    {
        public string Products { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
    [Serializable]
    public class ProcessorModelInfo : McuInfo
    {
        public string ProcessorModel { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}