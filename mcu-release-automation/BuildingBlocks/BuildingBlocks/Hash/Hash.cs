// Copyright (C) 2010 McAfee, Inc. All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Automation.Base.BuildingBlocks
{
    public class InvalidHashException : Exception
    {
        public InvalidHashException(String message, params object[] param)
            : base(String.Format(message, param))
        {
        }
    }

    public class Hash : IComparable
    {
        protected String rawHash;

        public Hash(String hash)
        {
            if (String.IsNullOrEmpty(hash))
                throw new InvalidHashException("Invalid hash - null value cannot be a valid hash");

            rawHash = hash.Trim().ToLower();

            if (rawHash.StartsWith("0x"))
                rawHash = rawHash.Substring(2);
        }

        /// <summary>
        /// Converts hexadecimal byte array to string
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns>String</returns>
        public static string FromHexByteArray(byte[] byteArray)
        {
            if (byteArray == null)
                throw new InvalidHashException("Invalid hash - null value cannot be a valid hash");
            return BitConverter.ToString(byteArray).Replace("-", "").ToLower();
        }

        public void ValidateHash(Regex validRegex)
        {
            if (!validRegex.IsMatch(rawHash))
                throw new InvalidHashException("Invalid hash '{0}' - Hash contains invalid characters", rawHash);
        }

        public override string ToString()
        {
            return rawHash;
        }

        public static bool operator ==(Hash lhs, Hash rhs)
        {
            if (System.Object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            return lhs.rawHash == rhs.rawHash;
        }

        public static bool operator ==(Hash lhs, String rhs)
        {
            return lhs.rawHash == rhs;
        }

        public static bool operator !=(Hash lhs, Hash rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator !=(Hash lhs, String rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return rawHash.GetHashCode();
        }

        public int CompareTo(object rhs)
        {
            return this.rawHash.CompareTo(rhs.ToString());
        }

        public override bool Equals(Object rhs)
        {
            if (rhs is Hash)
                return this.rawHash == (Hash)rhs.ToString();
            else
                if (rhs is String)
                    return this.rawHash == (String)rhs;
                else
                    return false;
        }

        public static implicit operator Hash(String s)
        {
            return new Hash(s);
        }

    }
}
