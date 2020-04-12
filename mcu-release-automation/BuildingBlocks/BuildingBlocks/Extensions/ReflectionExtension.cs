// Copyright (C) 2012 McAfee, Inc. All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Automation.Base
{
    public static class ReflectionExtension
    {
        /// <summary>
        /// Gets all public fields and properties within the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<MemberInfo> TypeMembers(this Type type)
        {
            List<MemberInfo> retval = new List<MemberInfo>();

            foreach (FieldInfo field in type.GetFields())
                retval.Add(field);

            foreach (PropertyInfo property in type.GetProperties())
                retval.Add(property);

            return retval;
        }

        /// <summary>
        /// Gets all public fields and properties within the specified object
        /// </summary>
        /// <param name="obj">Object to search</param>
        /// <returns></returns>
        public static List<MemberInfo> Members(this Object obj)
        {
            List<MemberInfo> retval = new List<MemberInfo>();

            foreach (FieldInfo field in obj.GetType().GetFields())
                retval.Add(field);

            foreach (PropertyInfo property in obj.GetType().GetProperties())
                retval.Add(property);

            return retval;
        }

        /// <summary>
        /// Gets the named field or property from the specified object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static MemberInfo Member(this Object obj, String objectName)
        {
            List<MemberInfo> retval = new List<MemberInfo>();

            foreach (FieldInfo field in obj.GetType().GetFields())
                if (field.Name == objectName)
                    return field;

            foreach (PropertyInfo property in obj.GetType().GetProperties())
                if (property.Name == objectName)
                    return property;

            return null;
        }

        /// <summary>
        /// Returns the value contained in the containing object referenced by this member info
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="containingObject">The object comtaining the member</param>
        /// <returns></returns>
        public static Object GetValue(this MemberInfo obj, Object containingObject)
        {
            if (obj.MemberType == MemberTypes.Field)
                return ((FieldInfo)obj).GetValue(containingObject);
            else
                if (obj.MemberType == MemberTypes.Property)
                    return ((PropertyInfo)obj).GetValue(containingObject, null);

            return null;
        }

        /// <summary>
        /// Sets the value of a field/property of a containing object to the supplied value. Must be compatible types
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="containingObject">Object containing the member field/property</param>
        /// <param name="newValue">Value to set</param>
        public static void SetValue(this MemberInfo obj, Object containingObject, Object newValue)
        {
            if (obj.MemberType == MemberTypes.Field)
                ((FieldInfo)obj).SetValue(containingObject, newValue);
            else
                if (obj.MemberType == MemberTypes.Property)
                    ((PropertyInfo)obj).SetValue(containingObject, newValue, null);
        }


        /// <summary>
        /// Determines if member field/property is an array
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsArray(this MemberInfo obj)
        {
            if (obj.MemberType == MemberTypes.Field && ((FieldInfo)obj).FieldType.IsArray)
                return true;

            if (obj.MemberType == MemberTypes.Property && ((PropertyInfo)obj).PropertyType.IsArray)
                return true;

            return false;
        }

        /// <summary>
        /// Determines if member field/property is not a value type or a string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsObject(this MemberInfo obj)
        {
            if (obj.MemberType == MemberTypes.Field && !((FieldInfo)obj).FieldType.IsValueType && ((FieldInfo)obj).FieldType != typeof(string))
                return true;

            if (obj.MemberType == MemberTypes.Property && !((PropertyInfo)obj).PropertyType.IsValueType && ((PropertyInfo)obj).PropertyType != typeof(string))
                return true;

            return false;
        }

        /// <summary>
        /// Determines if field/property is an enumeration
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsEnum(this MemberInfo obj)
        {
            if (obj.MemberType == MemberTypes.Field && ((FieldInfo)obj).FieldType.IsEnum)
                return true;

            if (obj.MemberType == MemberTypes.Property && ((PropertyInfo)obj).PropertyType.IsEnum)
                return true;

            return false;
        }

        /// <summary>
        /// Determines if the field/property is a nullable enumeration
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type IsNullableEnum(this MemberInfo obj)
        {
            Type[] tps = null;

            if (obj.MemberType == MemberTypes.Field)
                tps = ((FieldInfo)obj).FieldType.GetGenericArguments();
            else
                if (obj.MemberType == MemberTypes.Property)
                    tps = ((PropertyInfo)obj).PropertyType.GetGenericArguments();

            if (tps != null && tps.Length > 0 && tps[0].IsEnum)
                return tps[0];
            else
                return null;
        }

        /// <summary>
        /// Returns the type of the field/property
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type GetMemberType(this MemberInfo obj)
        {

            if (obj.MemberType == MemberTypes.Field)
                return ((FieldInfo)obj).FieldType;
            else
                if (obj.MemberType == MemberTypes.Property)
                    return ((PropertyInfo)obj).PropertyType;

            return null;
        }

        /// <summary>
        /// Determines if the <paramref name="interfaceName"/> is supported by <paramref name="type"/>
        /// </summary>
        /// <param name="type">The object type to check if interface <paramref name="interfaceName"/> is
        /// supported</param>
        /// <param name="interfaceName">The full name (i.e. System.ICollection) to check support for</param>
        /// <returns>true if <paramref name="type"/> supports the interface <paramref name="interfaceName"/> otherwise false</returns>
        public static bool SupportsInterface(this Type type, string interfaceName)
        {
            Type[] matchingInterfaces = type.FindInterfaces(delegate(Type t, object criteriaObj)
            {
                if (t.FullName == interfaceName)
                    return (true);

                return (false);
            }, interfaceName);

            if (null == matchingInterfaces || matchingInterfaces.Length == 0)
                return (false);

            return (true);
        }

        /// <summary>
        /// Determines if one field/property is assignable from another field/property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fromMember"></param>
        /// <returns></returns>
        public static bool IsAssignableFrom(this MemberInfo obj, MemberInfo fromMember)
        {
            return fromMember.GetMemberType().IsAssignableFrom(obj.GetMemberType());
        }

        /// <summary>
        /// Determines if a field/property is assignable from the specified type
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fromType"></param>
        /// <returns></returns>
        public static bool IsAssignableFromType(this MemberInfo obj, Type fromType)
        {
            Type[] tps = obj.GetMemberType().GetGenericArguments();
            if (tps != null && tps.Length > 0)
                return fromType.IsAssignableFrom(tps[0]);
            else
                return fromType.IsAssignableFrom(obj.GetMemberType());
        }

    }

}
