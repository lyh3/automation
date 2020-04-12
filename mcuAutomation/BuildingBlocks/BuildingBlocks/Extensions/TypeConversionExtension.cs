// Copyright (C) 2012 McAfee, Inc. All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation.Base.BuildingBlocks
{
    public static class TypeConversionExtension
    {
        /// <summary>
        /// Converts the specified ASCII string into a byte array
        /// </summary>
        /// <param name="string_to_convert">The String value to convert</param>
        /// <returns>a byte array representation of the String array</returns>
        public static byte[] ToByteArray(this String string_to_convert)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(string_to_convert);
        }

        /// <summary>
        /// Converts an IEnumerable to a Queue of the same type
        /// </summary>
        /// <typeparam name="T">The Type</typeparam>
        /// <param name="source">The Enumerable source</param>
        /// <returns></returns>
        public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
        {
            Queue<T> retval = new Queue<T>();

            foreach (T itm in source)
            {
                retval.Enqueue(itm);
            }

            return retval;
        }

        /// <summary>
        /// Convert an enumeration from one type to another using the reflection class
        /// </summary>
        /// <typeparam name="SOURCE_TYPE">The type converting from</typeparam>
        /// <typeparam name="TARGET_TYPE">the type converting to</typeparam>
        /// <param name="source">The siurce object</param>
        /// <returns></returns>
        public static IEnumerable<TARGET_TYPE> ConvertTo<SOURCE_TYPE, TARGET_TYPE>(this IEnumerable<SOURCE_TYPE> source)
        {
            List<TARGET_TYPE> nlist = new List<TARGET_TYPE>();
            source.ToList().ForEach(z => nlist.Add(CustomReflection.ConvertObject<TARGET_TYPE>(z)));

            return nlist;
        }

        /// <summary>
        /// Use reflection to create a new object of the specified type and deep copy matching members 
        /// </summary>
        /// <typeparam name="TARGET_TYPE">Target type to create</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TARGET_TYPE ConvertTo<TARGET_TYPE>(this Object obj)
        {
            return CustomReflection.ConvertObject<TARGET_TYPE>(obj);
        }

        /// <summary>
        /// Use reflection to create a new object of the specified type and deep copy matching members 
        /// </summary>
        /// <typeparam name="TARGET_TYPE">Target type to create</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void ConvertFrom(this Object TargetObject, Object SourceObject)
        {
            CustomReflection.CopyObject(SourceObject, TargetObject);
        }

        /// <summary>
        /// Returns a string representation of the integer in bytes, KB and MB depending on the size
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String ToSize(this int s)
        {
            return ((long)s).ToSize();
        }

        /// <summary>
        /// Returns a string representation of the long in bytes, KB and MB depending on the size
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String ToSize(this long s)
        {
            if (s < 1024)
                return String.Format("{0} bytes", s);

            if (s < 10240)
                return String.Format("{0} KB", Math.Round((double)s / 1024, 1));

            if (s < 1048576)
                return String.Format("{0} KB", Math.Round((double)s / 1024, 0));

            if (s < 10485760)
                return String.Format("{0} MB", Math.Round((double)s / 1048576, 1));

            return String.Format("{0} MB", Math.Round((double)s / 1048576, 0));
        }

        /// <summary>
        /// Convert a string to a base64 representation
        /// </summary>
        /// <param name="source">Text string to encode</param>
        /// <returns>base64 encoded as string</returns>
        public static String ToBase64(this String source)
        {
            return Convert.ToBase64String(source.ToByteArray());
        }

        /// <summary>
        /// Convert a byte array to base64 string
        /// </summary>
        /// <param name="source">array of bytes to base64 encode</param>
        /// <returns>base64 encoded as string</returns>
        public static String ToBase64(this byte[] source)
        {
            return Convert.ToBase64String(source);
        }

        /// <summary>
        /// Convert a base64 encoded string to a byte array
        /// </summary>
        /// <param name="source">Base64 encoded string</param>
        /// <returns></returns>
        public static byte[] FromBase64(this String source)
        {
            return Convert.FromBase64String(source);
        }

        /// <summary>
        /// Convert a byte array to an ascii string
        /// </summary>
        /// <param name="source">Array of bytes</param>
        /// <returns></returns>
        public static String ToAsciiString(this byte[] source)
        {
            return System.Text.ASCIIEncoding.ASCII.GetString(source);
        }

        /// <summary>
        /// Convert a byte array to a unicode string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static String ToUnicodeString(this byte[] source)
        {
            return System.Text.UnicodeEncoding.Unicode.GetString(source);
        }

        /// <summary>
        /// Convert a byte array to a utf8 string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static String ToUtf8String(this byte[] source)
        {
            return System.Text.UTF8Encoding.UTF8.GetString(source);
        }

        /// <summary>
        /// Attempt to determine the string encoding contained in the byte array
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Encoding GetStringEncoding(this byte[] source)
        {
            if (source[0] == 239 && source[1] == 187 && source[2] == 191)
                return Encoding.UTF8;
            else
            {
                if (source.Where(z => z == 0 || z > 127).Count() > 0)
                    return Encoding.Unicode;
                else
                    return Encoding.ASCII;
            }
        }
    }

}
