using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IntelDCGSpsWebService.Models
{
    [Serializable]
    public class CommandMap
    {
        public string Name { get; set; }
        public string IpmiCommand { get; set; }
        public string CommandCode { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }

    [Serializable]
    public class CommandMaps
    {
        private List<CommandMap> _commandMapList = new List<CommandMap>();
        private List<CommandDoc> _commandCocList = new List<CommandDoc>();
        public List<CommandMap> CommandMapList
        {
            get { return _commandMapList; }
            set { _commandMapList = value; }
        }
        public List<CommandDoc> CommandDocList
        {
            get { return _commandCocList; }
            set { _commandCocList = value; }
        }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }

    [Serializable]
    public class CommandDoc
    {
        public string CommandCode { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public override string ToString()
        {
            return new JsonFormatter(JsonConvert.SerializeObject(this)).Format();
        }
    }
}