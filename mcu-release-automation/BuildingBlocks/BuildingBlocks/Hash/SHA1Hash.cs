// Copyright (C) 2012 McAfee, Inc. All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;

namespace Automation.Base.BuildingBlocks
{
    public class SHA1Hash : Hash
    {
        private static Regex validRegex = new Regex(@"^[A-Fa-f0-9]{40}$", RegexOptions.Compiled);

        public SHA1Hash(String hash)
            : base(hash)
        {
            ValidateHash(validRegex);
        }

        public SHA1Hash(byte[] hash)
            : base(FromHexByteArray(hash))
        {
            ValidateHash(validRegex);
        }

        public static implicit operator SHA1Hash(String s)
        {
            return new SHA1Hash(s);
        }

        public static SHA1Hash FromFile(String filename)
        {
            using (FileStream file = new FileStream(filename, FileMode.Open))
            {
                using (SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider())
                {
                    return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(file)).Replace("-", "");
                }
            }
        }

        public static SHA1Hash FromMemory(byte[] memory)
        {
            using (SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider())
            {
                return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(memory)).Replace("-", "");
            }
        }

        public static SHA1Hash FromStream(Stream stream)
        {
            using (SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider())
            {
                return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(stream)).Replace("-", "");
            }
        }
    }

}
