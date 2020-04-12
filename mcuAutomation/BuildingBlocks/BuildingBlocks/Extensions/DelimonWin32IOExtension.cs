using Delimon.Win32.IO;

namespace Automation.Base.BuildingBlocks
{
    static public class DelimonWin32IOExtension
    {
        private const string UncPrefix = "\\\\?\\";
        public static string LongPathFormat(this string path)
        {

            return path.StartsWith(UncPrefix) ? path : string.Format(@"{0}{1}", UncPrefix, path);
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
