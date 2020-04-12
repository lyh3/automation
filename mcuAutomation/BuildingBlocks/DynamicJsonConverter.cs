using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize( IDictionary<string, object> dictionary, 
                                            Type type, 
                                            JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) })); }
        }

        public static object GetDynamicMember(object obj, string memberName)
        {         
            var binder = Binder.GetMember(CSharpBinderFlags.None, memberName, obj.GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            return callsite.Target(callsite, obj);
        }

        #region DynamicJsonObject

        public sealed class DynamicJsonObject : DynamicObject
        {
            private readonly IDictionary<string, object> _dictionary;

            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                _dictionary = dictionary;
            }

            public override string ToString()
            {
                var sb = new StringBuilder("{");
                ToString(sb);
                return sb.ToString();
            }

            private void ToString(StringBuilder sb)
            {
                var firstInDictionary = true;
                foreach (var pair in _dictionary)
                {
                    if (!firstInDictionary)
                        sb.Append(",");
                    firstInDictionary = false;
                    var value = pair.Value;
                    var name = pair.Key;
                    if (value is string)
                    {
                        sb.AppendFormat("{0}:\"{1}\"", name, value);
                    }
                    else if (value is IDictionary<string, object>)
                    {
                        new DynamicJsonObject((IDictionary<string, object>)value).ToString(sb);
                    }
                    else if (value is ArrayList)
                    {
                        sb.Append(name + ":[");
                        var firstInArray = true;
                        foreach (var arrayValue in (ArrayList)value)
                        {
                            if (!firstInArray)
                                sb.Append(",");
                            firstInArray = false;
                            if (arrayValue is IDictionary<string, object>)
                                new DynamicJsonObject((IDictionary<string, object>)arrayValue).ToString(sb);
                            else if (arrayValue is string)
                                sb.AppendFormat("\"{0}\"", arrayValue);
                            else
                                sb.AppendFormat("{0}", arrayValue);

                        }
                        sb.Append("]");
                    }
                    else
                    {
                        sb.AppendFormat("{0}:{1}", name, value);
                    }
                }
                sb.Append("}");
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                object retVal = null;
                bool containsKey = false;
                if (_dictionary.ContainsKey(binder.Name))
                {
                    retVal = _dictionary[binder.Name];

                    if (retVal is IDictionary<string, object>)
                        retVal = new DynamicJsonObject(retVal as IDictionary<string, object>);
                    else if (retVal is ArrayList)
                    {
                        var resultList = (retVal as ArrayList);
                        if (resultList.Count > 0 && resultList[0] is IDictionary<string, object>)
                            retVal = new List<DynamicJsonObject>((retVal as ArrayList).ToArray().Select(x => new DynamicJsonObject(x as IDictionary<string, object>)));
                        else
                            retVal = new List<object>((retVal as ArrayList).ToArray());
                    }
                    containsKey = true;
                }

                result = retVal;

                return containsKey;
            }
        }

        #endregion
    }
}
