// Copyright (C) 2012 McAfee, Inc. All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Automation.Base.BuildingBlocks
{
    public static class HashExtension
    {
        public static void WriteOutHasheList(this List<string> hashlist, string filepath)
        {
            if (null == hashlist || hashlist.Count <= 0) return;

            using (var streamwriter = new StreamWriter(filepath))
            {
                if (null != streamwriter)
                {
                    hashlist.ForEach(h => streamwriter.WriteLine(h));
                    streamwriter.Close();
                }
            }
        }

		#region MD5

        public static bool IsValidMD5HashString(this string hash)
        {
            if (string.IsNullOrEmpty(hash)) return false;
            else if (Regex.Match(hash.ToLower(), @"^([a-f0-9]{32})$").Success) return true;
            else return false;
        }

		/// <summary>
		/// Calculate a MD5 hash from a string
		/// </summary>
		/// <param name="stringToConvert">The string to calculate</param>
		/// <returns></returns>
		public static MD5Hash MD5(this String stringToConvert)
		{
			return MD5Hash.FromMemory(stringToConvert.ToByteArray());
		}

		/// <summary>
		/// Calculate an MD5 hash of the file referenced by the FileInfo object
		/// </summary>
		/// <param name="fileInfoObject">The file to calculate</param>
		/// <returns></returns>
		public static MD5Hash MD5(this FileInfo fileInfoObject)
		{
			return MD5Hash.FromFile(fileInfoObject.FullName);
		}

		/// <summary>
		/// Calculate an MD5 hash from a byte array
		/// </summary>
		/// <param name="b">Byte array to calculate</param>
		/// <returns></returns>
		public static MD5Hash MD5(this byte[] b)
		{
			return MD5Hash.FromMemory(b);
		}

		/// <summary>
		/// Calculate an MD5 hash from a stream
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static MD5Hash MD5(this Stream s)
		{
			return MD5Hash.FromStream(s);
		}

		#endregion

		#region SHA1

		/// <summary>
		/// Calculate a SHA1 hash from a string
		/// </summary>
		/// <param name="stringToConvert">The string to calculate</param>
		/// <returns></returns>
		public static SHA1Hash SHA1(this String stringToConvert)
		{
			return SHA1Hash.FromMemory(stringToConvert.ToByteArray());
		}

		/// <summary>
		/// Calculate an SHA1 hash of the file referenced by the FileInfo object
		/// </summary>
		/// <param name="fileInfoObject">The file to calculate</param>
		/// <returns></returns>
		public static SHA1Hash SHA1(this FileInfo fileInfoObject)
		{
			return SHA1Hash.FromFile(fileInfoObject.FullName);
		}

		/// <summary>
		/// Calculate an SHA1 hash from a byte array
		/// </summary>
		/// <param name="b">Byte array to calculate</param>
		/// <returns></returns>
		public static SHA1Hash SHA1(this byte[] b)
		{
			return SHA1Hash.FromMemory(b);
		}

		/// <summary>
		/// Calculate an SHA1 hash from a stream
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static SHA1Hash SHA1(this Stream s)
		{
			return SHA1Hash.FromStream(s);
		}

		#endregion

		#region SHA256

		/// <summary>
		/// Calculate a SHA256 hash from a string
		/// </summary>
		/// <param name="stringToConvert">The string to calculate</param>
		/// <returns></returns>
		public static SHA256Hash SHA256(this String stringToConvert)
		{
			return SHA256Hash.FromMemory(stringToConvert.ToByteArray());
		}

		/// <summary>
		/// Calculate an SHA256 hash of the file referenced by the FileInfo object
		/// </summary>
		/// <param name="fileInfoObject">The file to calculate</param>
		/// <returns></returns>
		public static SHA256Hash SHA256(this FileInfo fileInfoObject)
		{
			return SHA256Hash.FromFile(fileInfoObject.FullName);
		}

		/// <summary>
		/// Calculate an SHA256 hash from a byte array
		/// </summary>
		/// <param name="b">Byte array to calculate</param>
		/// <returns></returns>
		public static SHA256Hash SHA256(this byte[] b)
		{
			return SHA256Hash.FromMemory(b);
		}

		/// <summary>
		/// Calculate an SHA256 hash from a stream
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static SHA256Hash SHA256(this Stream s)
		{
			return SHA256Hash.FromStream(s);
		}

		#endregion
	}
}
