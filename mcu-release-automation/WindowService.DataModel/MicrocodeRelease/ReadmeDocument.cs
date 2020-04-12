using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    public class ReadmeDocument
    {
        private List<McuTableRow> _cpuTableRows = new List<McuTableRow>();
        public ReadmeDocument(string readmePath)
        {
            if (File.Exists(readmePath))
            {
                var line = string.Empty;
                using (var streamReader = new StreamReader(readmePath))
                {
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        line = line.TrimEnd('\n');
                        var split = line.Split('|');
                        if (split.Length >= 7)
                        {
                            split = split.Where((val, idx) => idx != 0).ToArray();
                            CpuSegment segment;
                            if (Enum.TryParse<CpuSegment>(split[0].Trim(), out segment))
                            {
                                var row = new McuTableRow();
                                for (var i = 0; i < split.Length; i++)
                                {
                                    row.AddColumn((McuTableColumn)i, split[i].TrimStart(' ').TrimEnd(' '));
                                }
                                _cpuTableRows.Add(row);
                            }
                        }
                    }
                }
            }
        }
        public List<McuTableRow> CpuTableRows { get { return _cpuTableRows; } }
        public McuCpuIdAndCpuSegmentAgreegation EntriesByCpuIdAndCpuSegment(string cpuId, string cpuSegment)
        {
            return new McuCpuIdAndCpuSegmentAgreegation(this, cpuId, cpuSegment); 
        }
    }
    [Serializable]
    public class McuCpuIdAndCpuSegmentAgreegation
    {
        protected Dictionary<string, string[]> _columnValuesDictionary = new Dictionary<string, string[]>();
        protected List<McuTableRow> _entries = new List<McuTableRow>();
        public McuCpuIdAndCpuSegmentAgreegation(ReadmeDocument doc, string cpuId, string cpuSegment)
        {
            var cpuid = cpuId.PadLeft(8, '0').ToUpper();
            foreach (McuTableRow row in doc.CpuTableRows)
            {
                if (row[McuTableColumn.CpuSegment] == cpuSegment && row[McuTableColumn.CpuId].PadLeft(8, '0').ToUpper() == cpuid)
                {
                    _entries.Add(row);
                }
            }

            for (var i = 0; i < (int)McuTableColumn.IntelProductSpecs; i++)
            {
                var key = ((McuTableColumn)i).ToString();
                _columnValuesDictionary[key] = this[key];
            }
        }

        public McuTableRow[] Entries { get { return _entries.ToArray(); } }
        public string [] this[string colum]
        {
            get
            {
                var results = new List<string>();
                McuTableColumn col;
                if (Enum.TryParse<McuTableColumn>(colum, out col))
                {
                    _entries.ForEach(x => results.Add(x[col]));
                }
                return results.Distinct().ToArray();
            }
        }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this._columnValuesDictionary)).Format();
        }
    }

    public class McuTableRow
    {
        private Dictionary<McuTableColumn, string> _columnDictionary = new Dictionary<McuTableColumn, string>();
        public string this[McuTableColumn col]
        {
            get
            {
                if (_columnDictionary.ContainsKey(col))
                {
                    return _columnDictionary[col];
                }
                return string.Empty;
            }
        }
        public void AddColumn(McuTableColumn col, string val)
        {
            _columnDictionary[col] = val;
        }
        public int Count { get { return _columnDictionary.Count; } }
        public override string ToString()
        {
            var itr = _columnDictionary.GetEnumerator();
            var ret = string.Empty;
            while (itr.MoveNext())
            {
                ret = string.Format(@" {0} | {0}", ret, itr.Current.Value);
            }
            return base.ToString();
        }
    }
}
