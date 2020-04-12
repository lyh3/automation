using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Automation.Base.BuildingBlocks;
using WindowService.DataModel;

namespace WindowService.DataModel
{
    [Serializable]
    public class HsdMcuArticleIdMap : JsonConfig
    {
        public List<List<string>> HsdArticle { get; set; }
        public ProcessingGroup Processing { get; set; }
        public ReleaseSchedule Schedule { get; set; }
        public string HsdEsRestUrl { get; set; }
        public long QueryId { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}