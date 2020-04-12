// Copyright (C) 2010 McAfee, Inc. All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class MD5Hash : Hash
    {
        private static Regex validRegex = new Regex(@"^[A-Fa-f0-9]{32}$", RegexOptions.Compiled);

        public MD5Hash(String hash)
            : base(hash)
        {

            ValidateHash(validRegex);
        }

        public MD5Hash(byte[] hash)
            : base(FromHexByteArray(hash))
        {
            ValidateHash(validRegex);
        }

        public static implicit operator MD5Hash(String s)
        {
            return new MD5Hash(s);
        }

        public static MD5Hash FromFile(String filename)
        {
            using (FileStream file = new FileStream(filename, FileMode.Open))
            {
                using (MD5CryptoServiceProvider cryptoTransformMD5 = new MD5CryptoServiceProvider())
                {
                    return BitConverter.ToString(cryptoTransformMD5.ComputeHash(file)).Replace("-", "");
                }
            }
        }

        public static MD5Hash FromMemory(byte[] memory)
        {
            using (MD5CryptoServiceProvider cryptoTransformMD5 = new MD5CryptoServiceProvider())
            {
                return BitConverter.ToString(cryptoTransformMD5.ComputeHash(memory)).Replace("-", "");
            }
        }

        public static MD5Hash FromStream(Stream stream)
        {
            using (MD5CryptoServiceProvider cryptoTransformMD5 = new MD5CryptoServiceProvider())
            {
                return BitConverter.ToString(cryptoTransformMD5.ComputeHash(stream)).Replace("-", "");
            }
        }


        /// <summary>
        /// Converts string to hexadecimal byte array (16-element byte array representation of 32 character MD5 string)
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns>Hexadecimal byte array</returns>
        public static byte[] ToHexByteArray(string inputString)
        {
            byte[] retByte = new byte[inputString.Length / 2];
            for (int i = 0; i < inputString.Length; i += 2)
                retByte[i / 2] = Convert.ToByte(inputString.Substring(i, 2), 16);
            return retByte;
        }
    }

}
