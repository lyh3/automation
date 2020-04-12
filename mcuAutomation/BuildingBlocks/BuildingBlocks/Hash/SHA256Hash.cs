// Copyright (C) 2012 McAfee, Inc. All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;

namespace Automation.Base.BuildingBlocks
{
    public class SHA256Hash : Hash
    {
        private static Regex validRegex = new Regex(@"^[A-Fa-f0-9]{64}$", RegexOptions.Compiled);

        public SHA256Hash(String hash)
            : base(hash)
        {
            ValidateHash(validRegex);
        }

        public SHA256Hash(byte[] hash)
            : base(FromHexByteArray(hash))
        {
            ValidateHash(validRegex);
        }

        public static implicit operator SHA256Hash(String s)
        {
            return new SHA256Hash(s);
        }

        public static SHA256Hash FromFile(String filename)
        {
            using (FileStream file = new FileStream(filename, FileMode.Open))
            {
                using (SHA256CryptoServiceProvider cryptoTransformSHA1 = new SHA256CryptoServiceProvider())
                {
                    return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(file)).Replace("-", "");
                }
            }
        }

        public static SHA256Hash FromMemory(byte[] memory)
        {
            using (SHA256CryptoServiceProvider cryptoTransformSHA1 = new SHA256CryptoServiceProvider())
            {
                return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(memory)).Replace("-", "");
            }
        }

        public static SHA256Hash FromStream(Stream stream)
        {
            using (SHA256CryptoServiceProvider cryptoTransformSHA1 = new SHA256CryptoServiceProvider())
            {
                return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(stream)).Replace("-", "");
            }
        }
    }

}
