using System;
using Delimon.Win32.IO;

namespace McAfeeLabs.Engineering.Automation.Base
{
    static public class DelimonWin32IOExtension
    {
        private const string UncPrefix = "\\\\?\\"; 
        public static string LongPathFormat(this string path)
        {
            var results = path;
            if (!string.IsNullOrEmpty(results))
            {
                if (results.StartsWith(@".\"))
                    results = results.Replace(@".\", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\\"));
                else if (results.StartsWith(@"."))
                    results = results.Replace(@".", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\\"));


                if (!results.StartsWith(UncPrefix) && results.StartsWith("\\\\"))
                    results = results.Replace("\\\\", UncPrefix);
                else if (results.Length >= GlobalDefinitions.MAX_PATH)
                    results = results.StartsWith(UncPrefix) ? results : string.Format(@"{0}{1}", UncPrefix, results);
            }
            return results;
        }

        public static string TrimLongPathPrefix(this string path)
        {
            return path.Replace(UncPrefix, string.Empty);
        }

        public static string[] GetFilesWithLongNames(this string path)
        {
            return Directory.GetFiles(path);
        }
        public static string[] GetFilesWithLongNames(this string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }
        public static string[] GetFilesWithLongNames(this string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }
    }
}
