// Copyright (C) 2012 McAfee, Inc. All rights reserved
//
// Class to aid in copying data from one object to another even if the object types are different. Any
// member fields or properties in the source object which match the name of member fields or properties
// in the destination object will be copied. 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
//using Automation.Foundation.Core.Extensions;

namespace Automation.Base.BuildingBlocks
{
    public static class CustomReflection
    {
        /// <summary>
        /// Create a new object of type T and copy any data with identical names to the new object.
        /// </summary>
        /// <typeparam name="T">Target object type</typeparam>
        /// <param name="src">Source object</param>
        /// <returns></returns>
        public static T ConvertObject<T>(Object src)
        {
            if (src != null)
            {
                Object dest = Activator.CreateInstance(typeof(T));
                CopyObject(src, dest);

                return (T)dest;
            }

            return (default(T));
        }

        /// <summary>
        /// Copy any data with identical names from the source object to the target object.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="target">Target object</param>
        public static void CopyObject(object source, object target)
        {
            CopyObjectData(source, target);
        }

        /// <summary>
        /// Copy an objects fields and properties to a target object
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static void CopyObjectData(object source, object target)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            Dictionary<String, MemberInfo> sourceMembers = GetObjectDetails(sourceType, true);
            Dictionary<String, MemberInfo> targetMembers = GetObjectDetails(targetType, false);

            foreach (MemberInfo sourceMember in sourceMembers.Values)
            {
                // If in the target object, a matching member name does not exist then continue
                if (!targetMembers.ContainsKey(sourceMember.Name))
                    continue;

                // Extract the types and values
                MemberInfo targetMember = targetMembers[sourceMember.Name];

                Object sourceMemberObject = sourceMember.GetValue(source);
                Object targetMemberObject = targetMember.GetValue(target);

                Type sourceMemberType = sourceMember.GetMemberType();
                Type targetMemberType = targetMember.GetMemberType();

                // If the source member is an array then create a new array and cop
                if (sourceMember.IsArray())
                {
                    if (!targetMember.IsArray())
                        throw new Exception(String.Format("ReflectionCopy : Cannot copy array to non-array member:{0} of type:{1} and value:{2} from object:{3} into object:{2}, Member:{3}, Type:{4}",
                            sourceMember.Name, sourceMemberType.Name, sourceMemberObject, sourceType.Name, targetType.Name, targetMember.Name, targetMemberType.Name));

                    if (sourceMemberObject == null)
                    {
                        targetMember.SetValue(target, null);
                        continue;
                    }

                    int sourceLength = (int)sourceMemberType.InvokeMember("Length", BindingFlags.GetProperty, null, sourceMemberObject, null);
                    Array targetArray = Array.CreateInstance(targetMemberType.GetElementType(), sourceLength);
                    Array array = (Array)sourceMemberObject;

                    for (int i = 0; i < array.Length; i++)
                    {
                        object o = array.GetValue(i);

                        Type arrayTargetType = targetMemberType.GetElementType();
                        if (o is String || o.GetType().IsValueType)
                        {
                            if (arrayTargetType.IsAssignableFrom(o.GetType()))
                                targetArray.SetValue(o, i);
                            else
                                targetArray.SetValue(Convert.ChangeType(o, arrayTargetType), i);
                        }
                        else
                        {
                            object tempTarget = Activator.CreateInstance(arrayTargetType);
                            CopyObjectData(o, tempTarget);
                            targetArray.SetValue(tempTarget, i);
                        }
                    }

                    targetMember.SetValue(target, targetArray);
                    continue;
                }

                // Are we copyring a basic object. If yes then recurively copy 
                if (sourceMember.IsObject())
                {
                    if (targetMemberObject == null)
                    {
                        try
                        {
                            targetMemberObject = Activator.CreateInstance(targetMemberType);
                            targetMember.SetValue(target, targetMemberObject);
                        }
                        catch (Exception ex)
                        {
                            // If we get here then it usually means that a Object doesn't have parameterless constructor. If the type
                            // name begins with System. then we are trying to copy a system object that requires parameters in the 
                            // constructor so we can silently ignore them. It it's a user object then we need to throw an exception
                            if (targetMemberType.FullName.StartsWith("System."))
                                continue;

                            throw new Exception(String.Format("ReflectionCopy : Cannot assign Member Class:{0}, Member:{1}, Value:{2} into  Class:{3}, Member:{4}",
                                sourceType.Name, sourceMember.Name, sourceMemberObject, targetType.Name, targetMember.Name), ex);
                        }
                    }

                    CopyObjectData(sourceMemberObject, targetMemberObject);

                }
                else
                {
                    if (targetMember.IsAssignableFrom(sourceMember))
                    {
                        try
                        {
                            targetMember.SetValue(target, sourceMemberObject);
                            continue;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(String.Format("ReflectionCopy : Cannot assign Member Class:{0}, Member:{1}, Value:{2} into  Class:{3}, Member:{4}",
                                sourceType.Name, sourceMember.Name, sourceMemberObject, targetType.Name, targetMember.Name), ex);
                        }
                    }

                    if (sourceMember.IsEnum())
                    {
                        if (targetMember.IsEnum())
                        {
                            try
                            {
                                targetMember.SetValue(target, Enum.Parse(targetMemberType, Enum.GetName(sourceMemberType, sourceMemberObject)));
                                continue;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(String.Format("ReflectionCopy : Error assigning enum Class:{0}, Member:{1}, Value:{2} into  Class:{3}, Member:{4}",
                                    sourceType.Name, sourceMember.Name, sourceMemberObject, targetType.Name, targetMember.Name), ex);
                            }
                        }

                        Type targetMemberEnum;
                        if ((targetMemberEnum = targetMember.IsNullableEnum()) != null)
                        {
                            try
                            {
                                if (sourceMemberObject == null)
                                    targetMember.SetValue(target, null);
                                else
                                    targetMember.SetValue(target, Enum.Parse(targetMemberEnum, Enum.GetName(sourceMemberType, sourceMemberObject)));
                                continue;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(String.Format("ReflectionCopy : Error assigning enum Class:{0}, Member:{1}, Value:{2} into  Class:{3}, Member:{4}",
                                    sourceType.Name, sourceMember.Name, sourceMemberObject, targetType.Name, targetMember.Name), ex);
                            }
                        }

                        if (targetMemberType.IsAssignableFrom(typeof(String)))
                        {
                            try
                            {
                                targetMember.SetValue(target, Enum.GetName(sourceMemberType, sourceMemberObject));
                                continue;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(String.Format("ReflectionCopy : Error assigning enum Class:{0}, Member:{1}, Value{2} into  Class:{3}, Member:{4}",
                                    sourceType.Name, sourceMember.Name, sourceMemberObject, targetType.Name, targetMember.Name), ex);
                            }
                        }

                        throw new Exception(String.Format("ReflectionCopy : Error assigning enum Class:{0}, Member:{1}, Value{2} into  Class:{3}, Member:{4}",
                            sourceType.Name, sourceMember.Name, sourceMemberObject, targetType.Name, targetMember.Name));
                    }

                    Type sourceMemberEnum;
                    if ((sourceMemberEnum = sourceMember.IsNullableEnum()) != null)
                    {
                        Type targetMemberEnum;
                        if ((targetMemberEnum = targetMember.IsNullableEnum()) != null)
                        {
                            try
                            {
                                if (sourceMemberObject == null)
                                    targetMember.SetValue(target, null);
                                else
                                    targetMember.SetValue(target, Enum.Parse(targetMemberEnum, Enum.GetName(sourceMemberEnum, sourceMemberObject)));
                                continue;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(String.Format("ReflectionCopy : Error assigning nullable enum Class:{0}, Member:{1}, Value:{2} into  Class:{3}, Member:{4}",
                                    sourceType.Name, sourceMember.Name, sourceMemberObject, targetType.Name, targetMember.Name), ex);
                            }
                        }

                        if (targetMemberType.IsAssignableFrom(typeof(String)))
                        {
                            try
                            {
                                targetMember.SetValue(target, Enum.GetName(sourceMemberEnum, sourceMemberObject));
                                continue;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(String.Format("ReflectionCopy : Error assigning nummable enum Class:{0}, Member:{1}, Value:{2} into  Class:{3}, Member:{4}",
                                    sourceType.Name, sourceMember.Name, sourceMemberObject, targetType.Name, targetMember.Name), ex);
                            }
                        }

                        throw new Exception(String.Format("ReflectionCopy : Error assigning nullable enum Class:{0}, Member:{1}, Value:{2} into  Class:{3}, Member:{4}",
                            sourceType.Name, sourceMember.Name, sourceMemberObject, targetType.Name, targetMember.Name));
                    }

                    if (targetMember.IsEnum())
                    {
                        try
                        {
                            targetMember.SetValue(target, Enum.Parse(targetMemberType, Convert.ToString(sourceMemberObject)));
                            continue;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(String.Format("ReflectionCopy : Error assigning enum Class:{0}, Member:{1}, Value:{2} into  Class:{3}, Member:{4}",
                                sourceType.Name, sourceMember.Name, sourceMemberObject, targetType.Name, targetMember.Name), ex);
                        }
                    }

                    try
                    {
                        targetMember.SetValue(target, Convert.ChangeType(sourceMemberObject, targetMemberType));
                        continue;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("ReflectionCopy : Error converting member Class:{0}, Member:{1}, Value:{2} into  Class:{3}, Member:{4}",
                            sourceType.Name, sourceMember.Name, sourceMemberObject, targetType.Name, targetMember.Name), ex);
                    }
                }
            }
        }

        public static Dictionary<String, MemberInfo> GetObjectDetails(Type t, Boolean source)
        {
            MemberInfo[] fields = t.GetFields().Where(z => z.IsPublic).ToArray();
            MemberInfo[] properties = t.GetProperties().Where(z => source ? z.CanRead : z.CanWrite).ToArray();

            Dictionary<String, MemberInfo> retval = new Dictionary<String, MemberInfo>();
            fields.ToList().ForEach(z => retval[z.Name] = z);
            properties.ToList().ForEach(z => retval[z.Name] = z);

            return retval;
        }
    }
}
