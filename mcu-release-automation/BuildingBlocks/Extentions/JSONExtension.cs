using System;
using System.Text;
using System.Web.Script.Serialization;
using System.IO;

using Newtonsoft.Json.Linq;
using McAfeeLabs.Engineering.Automation.Base.Json;

namespace McAfeeLabs.Engineering.Automation.Base
{
    static public class JSONExtension
    {
        [ThreadStatic]
        private static JavaScriptSerializer _serializer = null;

        public static JavaScriptSerializer Instance
        {
            get
            {
                if (_serializer == null)
                    _serializer = new JavaScriptSerializer();
                return _serializer;
            }
        }

        public static string ToJSON(this object obj)
        {
            var results = Instance.Serialize(obj);
            return results;
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            return Instance.Serialize(obj);
        }

        public static StringBuilder ToJSONStringBuilder(this object obj)
        {
            var sb = new StringBuilder();
            Instance.Serialize(obj, sb);

            return sb;
        }

        public static string ToDotNetSourceCode(this string json,
                                                string rootTypeName,
                                                SerializationModel serializetype = SerializationModel.DataContractJsonSerializer,
                                                string language = @"CS")
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(json))
            {
                var jtoken = JObject.Parse(json) as JToken;
                if (null != jtoken)
                {
                    var root = JsonRoot.ParseJsonIntoDataContract(jtoken, rootTypeName);
                    using (var stringWriter = new StringWriter(sb))
                    {
                        JsonRootCompiler compiler = new JsonRootCompiler(language, serializetype);
                        compiler.GenerateCode(root, stringWriter);
                    }
                }
            }

            return sb.ToString();
        }

        public static object ToObject(this string json)
        {
            return Instance.DeserializeObject(json);
        }

        public static T ToObject<T>(this string json)
        {
            return (T)Instance.DeserializeObject(json);
        }

        public static T ConvertToObject<T>(this string json)
        {
            return Instance.ConvertToType<T>(json);
        }

        public static dynamic ConvertToDynamicObject(this string json)
        {
            Instance.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });
            return Instance.Deserialize(json, typeof(object)) as dynamic;
        }
    }
}
