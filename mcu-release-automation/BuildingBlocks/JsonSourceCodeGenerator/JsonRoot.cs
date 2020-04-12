using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace McAfeeLabs.Engineering.Automation.Base.Json
{
    public class JsonRoot
    {   
        #region Properties

        public bool IsUserDefinedType { get; private set; }
        public Type ElementType { get; private set; }
        public string UserDefinedTypeName { get; private set; }
        public int ArrayRank { get; private set; }
        public IDictionary<string, JsonRoot> Members { get; private set; }

        private JsonRoot Parent { get; set; }

        #endregion

        #region Public Methods

        public static JsonRoot ParseJsonIntoDataContract(JToken root, string rootTypeName)
        {
            JsonRoot results = null;
            if (root == null || root.Type == JTokenType.Null)
            {
                results = new JsonRoot(null, 0);
            }
            else
            {
                switch (root.Type)
                {
                    case JTokenType.Boolean:
                        results = new JsonRoot(typeof(bool), 0);
                        break;
                    case JTokenType.String:
                        results = new JsonRoot(typeof(string), 0);
                        break;
                    case JTokenType.Float:
                        results = new JsonRoot(typeof(double), 0);
                        break;
                    case JTokenType.Date:
                        results = new JsonRoot(typeof(DateTime), 0);
                        break;
                    case JTokenType.TimeSpan:
                        results = new JsonRoot(typeof(TimeSpan), 0);
                        break;
                    case JTokenType.Integer:
                        results = new JsonRoot(GetClrIntegerType(root.ToString()), 0);
                        break;
                    case JTokenType.Object:
                        results = ParseJObjectIntoDataContract((JObject)root, rootTypeName);
                        break;
                    case JTokenType.Array:
                        results = ParseJArrayIntoDataContract((JArray)root, rootTypeName);
                        break;
                    default:
                        throw new ArgumentException("Cannot work with JSON token of type " + root.Type);
                }
            }

            return results;
        }

        public List<string> GetAncestors()
        {
            List<string> result = new List<string>();
            JsonRoot temp = this;
            while (temp != null)
            {
                if (temp.Parent != null)
                {
                    result.Insert(0, temp.UserDefinedTypeName);
                }

                temp = temp.Parent;
            }

            result.Insert(0, "<<root>>");
            return result;
        }
        
        #endregion

        #region Private Mehtods

        private JsonRoot(Type elementType, int arrayRank)
        {
            this.Members = new Dictionary<string, JsonRoot>();
            this.IsUserDefinedType = false;
            this.ElementType = elementType;
            this.ArrayRank = arrayRank;
        }

        private JsonRoot(string userDefinedTypeName, int arrayRank, IDictionary<string, JsonRoot> members)
        {
            this.IsUserDefinedType = true;
            this.UserDefinedTypeName = userDefinedTypeName;
            this.ArrayRank = arrayRank;
            this.Members = members;
        }

        private static JsonRoot ParseJArrayIntoDataContract(JArray root, string rootTypeName)
        {
            if (root.Count == 0)
            {
                return new JsonRoot(null, 1);
            }

            JsonRoot first = ParseJsonIntoDataContract(root[0], rootTypeName);
            for (int i = 1; i < root.Count; i++)
            {
                JsonRoot next = ParseJsonIntoDataContract(root[i], rootTypeName);
                JsonRoot mergedType;
                if (first.CanMerge(next, out mergedType))
                {
                    first = mergedType;
                }
                else
                {
                    throw new ArgumentException(string.Format("Cannot merge array elements {0} ({1}) and {2} ({3})",
                        0, root[0], i, root[i]));
                }
            }

            if (first.IsUserDefinedType)
            {
                return new JsonRoot(first.UserDefinedTypeName, first.ArrayRank + 1, first.Members);
            }
            else
            {
                return new JsonRoot(first.ElementType, first.ArrayRank + 1);
            }
        }

        private static JsonRoot ParseJObjectIntoDataContract(JObject root, string rootTypeName)
        {
            Dictionary<string, JsonRoot> fields = new Dictionary<string, JsonRoot>();
            foreach (JProperty property in root.Properties())
            {
                JsonRoot fieldType = ParseJsonIntoDataContract(property.Value, property.Name);
                fields.Add(property.Name, fieldType);
            }

            JsonRoot result = new JsonRoot(rootTypeName, 0, fields);

            foreach (var field in fields.Values)
            {
                field.Parent = result;
            }

            return result;
        }

        private static Type GetClrIntegerType(string value)
        {
            int temp;
            if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out temp))
            {
                return typeof(int);
            }

            long temp2;
            if (long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out temp2))
            {
                return typeof(long);
            }

            // treat it as a double, may lose precision but at least we have a value
            return typeof(double);
        }

        private bool CanMerge(JsonRoot other, out JsonRoot mergedType)
        {
            mergedType = null;

            if (this.ArrayRank != other.ArrayRank)
            {
                return false;
            }

            if (this.IsUserDefinedType != other.IsUserDefinedType)
            {
                return false;
            }

            if (this.CanMergeInto(other, out mergedType))
            {
                return true;
            }

            if (other.CanMergeInto(this, out mergedType))
            {
                return true;
            }

            return false;
        }

        private bool CanMergeInto(JsonRoot other, out JsonRoot mergedType)
        {
            if (this.IsUserDefinedType)
            {
                return this.CanMergeIntoUserDefinedType(other, out mergedType);
            }
            else
            {
                return this.CanMergeIntoPrimitiveType(other, out mergedType);
            }
        }

        private bool CanMergeIntoUserDefinedType(JsonRoot other, out JsonRoot mergedType)
        {
            bool sameAsThis = true;
            mergedType = null;
            Dictionary<string, JsonRoot> members = new Dictionary<string, JsonRoot>();
            foreach (var memberName in this.Members.Keys.Union(other.Members.Keys))
            {
                if (this.Members.ContainsKey(memberName) && other.Members.ContainsKey(memberName))
                {
                    JsonRoot member1 = this.Members[memberName];
                    JsonRoot member2 = other.Members[memberName];
                    JsonRoot merged;
                    if (!member1.CanMerge(member2, out merged))
                    {
                        return false;
                    }
                    else
                    {
                        if (merged != member1)
                        {
                            sameAsThis = false;
                        }

                        members.Add(memberName, merged);
                    }
                }
                else if (this.Members.ContainsKey(memberName))
                {
                    members.Add(memberName, this.Members[memberName]);
                }
                else
                {
                    sameAsThis = false;
                    members.Add(memberName, other.Members[memberName]);
                }
            }

            if (sameAsThis)
            {
                mergedType = this;
            }
            else
            {
                mergedType = new JsonRoot(this.UserDefinedTypeName, this.ArrayRank, members);
            }

            return true;
        }

        private bool CanMergeIntoPrimitiveType(JsonRoot other, out JsonRoot mergedType)
        {
            if (this.ElementType == other.ElementType)
            {
                mergedType = this;
                return true;
            }

            bool isThisNullable = this.IsNullableType();
            bool isOtherNullable = other.IsNullableType();

            Type thisElementType = this.ElementType != null && this.ElementType.IsGenericType && this.ElementType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                this.ElementType.GetGenericArguments()[0] : this.ElementType;

            Type otherElementType = other.ElementType != null && other.ElementType.IsGenericType && other.ElementType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                other.ElementType.GetGenericArguments()[0] : other.ElementType;

            if (thisElementType == otherElementType)
            {
                // one nullable, the other not
                if (isThisNullable)
                {
                    mergedType = this;
                }
                else
                {
                    mergedType = other;
                }

                return true;
            }

            if (thisElementType == null)
            {
                if (isOtherNullable || otherElementType == typeof(string))
                {
                    mergedType = other;
                    return true;
                }

                mergedType = new JsonRoot(typeof(Nullable<>).MakeGenericType(otherElementType), this.ArrayRank);
                return true;
            }

            if (otherElementType == null)
            {
                if (isThisNullable || thisElementType == typeof(string))
                {
                    mergedType = this;
                    return true;
                }

                mergedType = new JsonRoot(typeof(Nullable<>).MakeGenericType(thisElementType), this.ArrayRank);
                return true;
            }

            // Number coercions
            if (this.ElementType == typeof(int))
            {
                if (other.ElementType == typeof(long) || other.ElementType == typeof(double))
                {
                    mergedType = other;
                    if (!mergedType.IsNullableType() && isThisNullable)
                    {
                        mergedType = new JsonRoot(typeof(Nullable<>).MakeGenericType(mergedType.ElementType), mergedType.ArrayRank);
                    }

                    return true;
                }
            }
            else if (this.ElementType == typeof(long))
            {
                if (other.ElementType == typeof(double))
                {
                    mergedType = other;
                    if (!mergedType.IsNullableType() && isThisNullable)
                    {
                        mergedType = new JsonRoot(typeof(Nullable<>).MakeGenericType(mergedType.ElementType), mergedType.ArrayRank);
                    }

                    return true;
                }
            }

            mergedType = null;
            return false;
        }

        private bool IsNullableType()
        {
            return !this.IsUserDefinedType &&
                ((this.ElementType == null) ||
                    (this.ElementType.IsGenericType &&
                    this.ElementType.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        #endregion
    }

}
