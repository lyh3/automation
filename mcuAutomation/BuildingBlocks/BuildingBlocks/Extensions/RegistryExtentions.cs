using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Automation.Base
{
    static public class RegistryExtentions
    {
        #region Public Methods

        public static object GetValue(this string regpath, string valueName)
        {
            object value = null;

            var key = OpenRegistryKey(regpath, false);
            if (null != key)
            {
                try
                {
                    value = key.GetValue(valueName, null);
                }
                finally { key.Close(); }
            }

            return value;
        }

        public static bool SetValue(this string regpath, 
                                    string valueName, 
                                    dynamic valueData, 
                                    bool create = true)
        {
            var key = OpenRegistryKey(regpath, create);

            if ((key == null) && (!create))
                return false;

            try
            {
                if (null == key && create)
                    key = CreateRegistryKeyByPath(regpath);

                if(null != key)
                    key.SetValue(valueName, valueData);
            }
            finally { key.Close(); }

            return true;
        }

        public static RegistryKey CreateRegistryKeyByPath(this string regpath)
        {
            RegistryKey regkey = null;

            if (!string.IsNullOrEmpty(regpath))
            {
                var hive = ParseRegistryPathHKEYvalue(regpath);

                if (hive != null)
                {
                    regpath = regpath.Substring(regpath.IndexOf(@"\") + 1);
                    try { regkey = hive.CreateSubKey(regpath); }
                    catch { }
                }
            }

            return regkey;
        }

        public static bool DeleteSubkey(this string regpath, string subkeyName)
        {
            var success = false;

            if (!string.IsNullOrEmpty(regpath) && !string.IsNullOrEmpty(subkeyName))
            {
                var regkey = OpenRegistryKey(regpath, true);
                if (null != regkey)
                {
                    try
                    {
                        regkey.DeleteSubKey(subkeyName);
                        success = true;
                    }
                    finally { regkey.Close(); }
                }
            }

            return success;
        }

        public static bool DeleteSubkeyTree(this string regpath, string subkeyName)
        {
            var success = false;

            if (!string.IsNullOrEmpty(regpath) && !string.IsNullOrEmpty(subkeyName))
            {
                var regkey = OpenRegistryKey(regpath, true);
                if (null != regkey)
                {
                    try
                    {
                        regkey.DeleteSubKeyTree(subkeyName);
                        success = true;
                    }
                    finally { regkey.Close(); }
                }
            }

            return success;
        }

        public static string[] TraverseRegistryPath(this string traversePath)
        {
            var traverseKey = OpenRegistryKey(traversePath);
            string traverseKeyPath = traversePath.TrimEnd("\\".ToCharArray());

            var foundRegKeys = new List<string>();
            var processedKeys = new List<string>();

            foundRegKeys.Add(traverseKey.Name);

            while (processedKeys.Count != foundRegKeys.Count)
            {
                if (traverseKey != null)
                {
                    foreach (string subKey in traverseKey.GetSubKeyNames())
                    {
                        foundRegKeys.Add(string.Format(@"{0}\{1}", traverseKey.Name, subKey));
                    }
                }

                processedKeys.Add(traverseKeyPath);

                foreach (string regKey in foundRegKeys)
                {
                    if (traverseKey != null && !processedKeys.Contains(regKey))
                    {
                        if (traverseKey != null)
                            traverseKey.Close();
                        try { traverseKey = OpenRegistryKey(regKey); }
                        finally { traverseKeyPath = regKey; }
                        break;
                    }
                }
            }
            return (foundRegKeys.ToArray());
        }

        public static bool DeleteValue(this string regpath, dynamic value)
        {
            bool success = false;
            var key = OpenRegistryKey(regpath, writable:true);

            if (null != key)
            {
                try
                {
                    key.DeleteValue(value);
                    success = true;
                }
                finally { key.Close(); }
            }

            return success;
        }

        #endregion

        #region Private Methods

        private static RegistryKey OpenRegistryKey(string regpath, bool writable = false)
        {
            RegistryKey regkey = null;

            if (!string.IsNullOrEmpty(regpath))
            {

                var hive = ParseRegistryPathHKEYvalue(regpath);

                if (hive != null)
                {
                    regpath = regpath.Substring(regpath.IndexOf(@"\") + 1);
                    regkey = hive.OpenSubKey(regpath, writable);
                }
            }

            return regkey;
        }

        private static RegistryKey ParseRegistryPathHKEYvalue(string regpath)
        {
            RegistryKey regkey = null;

            Dictionary<string, RegistryKey> hivDictionary = new Dictionary<string, RegistryKey>();
            hivDictionary.Add(Registry.LocalMachine.ToString(), Registry.LocalMachine);
            hivDictionary.Add(Registry.CurrentUser.ToString(), Registry.CurrentUser);
            hivDictionary.Add(Registry.Users.ToString(), Registry.Users);
            hivDictionary.Add(Registry.ClassesRoot.ToString(), Registry.ClassesRoot);
            hivDictionary.Add(Registry.CurrentConfig.ToString(), Registry.CurrentConfig);

            if (!string.IsNullOrEmpty(regpath))
            {
                var split = regpath.Split( new [] { '\\' });
                if (split.Length > 0)
                    hivDictionary.TryGetValue(split[0].ToUpper(), out regkey);
            }

            return regkey;
        }

        #endregion
    }
}
