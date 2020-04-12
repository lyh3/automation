using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using IntelDCGSpsWebService.Models;

namespace IntelDCGSpsWebService.Models.Buildingblocks
{
    public class utils
    {
        public static string[] DecodeEventData(string jsonpath, string eventData, string itemformat)
        {
            string[] results = {};
            if (!string.IsNullOrEmpty(jsonpath) && File.Exists(jsonpath))
            {
                using (StreamReader file = File.OpenText(jsonpath))
                {
                    var serializer = new JsonSerializer();
                    var model = (HealthEventViewModel)serializer.Deserialize(file, typeof(HealthEventViewModel));
                    results = DecodeFromJson(eventData, model, itemformat);
                }
            }
            return results;
        }
   

        public static string[] DecodeFromJson(string data, HealthEventViewModel decodingConfig, string itemformat)
        {
            if (string.IsNullOrEmpty(data)||decodingConfig == null || string.IsNullOrEmpty(itemformat))
                return null;

            var results = new List<string>();
            var dependencyDic = new Dictionary<string, string>();
            var response = Hex2BinConverter(data, " ").Split(' ');
            var idxH = 0;
            var idxL = 2;
            foreach(var d in decodingConfig.responsedatas)
            {
                var dependenceValue = string.Empty;
                var dependenceKey = d.dependence.Trim();
                var buffer = new List<string>();
                var values = new List<valueData>();
                var ishex = d.hex;
                for (var i = idxH; i < idxL; ++i)
                    buffer.Add(response[i]);
                if (!ishex)
                {
                    buffer.Reverse();
                }
                foreach(var p in d.positions)
                {
                   if (!string.IsNullOrEmpty(dependenceKey) && dependencyDic.ContainsKey(dependenceKey))
                    {
                        dependenceValue = dependencyDic[dependenceKey];
                        foreach(var px in p.values)
                        {
                            if(px.value == dependenceValue)
                            {
                                values.AddRange(px.values);
                                break;
                            }
                        }
                    }
                    else
                    {
                        values.AddRange(p.values);
                    }

                    if (values.Count == 0)
                        continue;

                    foreach(var x in values)
                    {
                        var lookup = Hex2BinConverter(x.value);
                        var match = false;
                        if (ishex)
                            match = buffer[0] + buffer[1] == lookup;
                        else
                        {
                            var position = p.pos.Trim().TrimStart('[').TrimEnd(']').Split(':');
                            var s = string.Join(string.Empty, buffer.ToArray());
                            var val = new StringBuilder();
                            for (var i = int.Parse(position[0]); i < int.Parse(position[1]) + 1; ++i)
                                val.Append(s[i].ToString()); 
                            match = val.ToString() == lookup;
                        }
                        if (match)
                        {
                            var v = x.value;
                            if (!string.IsNullOrEmpty(dependenceValue))
                                v = string.Format("{0} - {1}[{2}]", dependenceValue, d.description, v);
                            var s = string.Format(itemformat, v, x.description);
                            if(!results.Contains(s))
                                results.Add(s);
                            if (!dependencyDic.ContainsKey(d.description))
                                dependencyDic[d.description] = v;
                        }
                    }
                }

                idxH += 2;
                idxL += 2;
            }
            return results.ToArray();
        }
    
        public static string Hex2BinConverter(string source, string delimiter="")
        {
            var s = source.ToLower();
            if (s.EndsWith("b"))
                return s.TrimEnd('b');
            string binarystring = String.Join(delimiter,
                                  s.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'))
                                  );
            return binarystring;
        }
    }
}
