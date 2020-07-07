using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security.Permissions;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Text;

namespace Automation.Base.BuildingBlocks
{
    static public class Extensions
    {
        [ThreadStatic]
        private static Random _random = null;
        public static bool FindProcess(this string name)
        {
            var processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                if (p.ProcessName.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }
        public static string ByteArrayTOHexString(this byte[] bytes)
        {
            return String.Concat(Array.ConvertAll(bytes, x => x.ToString("X2")));
        }

        public static bool IsTimeOut(this TimeSpan timeOut, DateTime timeSince)
        {
            return DateTime.Now - timeSince > timeOut;
        }

        public static bool OpenSqlConnection(this SqlConnection connection,
                                              int timeout = 5, //in seconds
                                              string errorMessage = null)
        {
            bool success = true;
            var timestart = DateTime.Now;
            var errors = new StringBuilder();

            if (null != connection)
            {
                var handle = new ManualResetEvent(false);
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    errors.AppendLine(string.Format(@"---Exception caught at OpenSqlConnection, error was : {0}", ex.ToString()));
                }
                handle.Set();
                if (!handle.WaitOne(new TimeSpan(0, 0, timeout)))
                {
                    success = false;
                    errors.AppendLine(string.Format(@"---Time out experienced from openning sql connection, timeout in <{0}> seconds", timeout));
                }
                else
                    success = connection.State == ConnectionState.Open;
                if (!string.IsNullOrEmpty(errors.ToString()))
                {
                    Trace.WriteLine(errors.ToString());
                    if (!string.IsNullOrEmpty(errorMessage))
                        errorMessage = errors.ToString();
                }

            }
            return success;
        }

        public static dynamic RandomValue(this int size, bool digits = false, bool alphabatical = false)
        {
            if (_random == null)
                _random = new Random();

            size = size > 0 ? size : 1;
            var sourcestring = alphabatical ? "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" : "0123456789";

            var resutls = new string(Enumerable.Repeat(sourcestring, size)
                                     .Select(s => s[_random.Next(s.Length)])
                                     .ToArray());
            if (digits)
                return Convert.ToInt32(resutls.ToString());
            else
                return resutls.ToString();
        }

        public static long ElapsedTimeConvert(this Stopwatch stopwatch)
        {
            return ((stopwatch.Elapsed.Hours * 60
                    + stopwatch.Elapsed.Minutes) * 60
                    + stopwatch.Elapsed.Seconds) * 1000
                    + stopwatch.Elapsed.Milliseconds;
        }

        public static void LoadEmbeddedXml(this string sourceFileName,
                                            XmlDocument xmlDoc,
                                            Assembly asm = null)
        {
            var assembly = null != asm ? asm : Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(sourceFileName))
            {
                if (null != stream)
                {
                    var reader = new StreamReader(stream);
                    var xml = reader.ReadToEnd();
                    xmlDoc.LoadXml(xml);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="targetpath"></param>
        /// <param name="embeddedFilePathFormat">AssemblyName.FolderName</param>
        public static void CreateFileFromEmbeddedStream(this string filename,
                                                        string targetpath,
                                                        string embeddedFilePath)
        {
            const int ChunkSize = 32 * 1024;
            byte[] buffer = new byte[ChunkSize];
            var assembly = Assembly.GetCallingAssembly();
            var embeddedFilePathFormat = string.Format(@"{0}.{1}0{2}", embeddedFilePath, '{', '}');
            using (var writer = new BinaryWriter(File.Open(targetpath, FileMode.OpenOrCreate)))
            {
                var stream = assembly.GetManifestResourceStream(string.Format(embeddedFilePathFormat, filename));//EmbeddedZipBombFilename);
                if (null != stream)
                {
                    var reader = new StreamReader(stream);
                    var br = new BinaryReader(stream);
                    int retval;
                    retval = br.Read(buffer, 0, ChunkSize);
                    while (retval > 0)
                    {
                        writer.Write(buffer, 0, retval);
                        writer.Flush();
                        retval = br.Read(buffer, 0, ChunkSize);
                    }
                    stream.Close();
                }
                else
                    throw new ArgumentException(string.Format(@"Can not fitch out the embedded stream <{0}>", embeddedFilePath));
            }
        }

        public static bool RemoveFileReadOnlyAttribute(this string filepath)
        {
            var success = true;

            if (!string.IsNullOrEmpty(filepath) && File.Exists(filepath))
            {
                try
                {
                    var attibutes = File.GetAttributes(filepath);

                    if ((attibutes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        attibutes = attibutes & ~FileAttributes.Hidden;
                    if ((attibutes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        attibutes = attibutes & ~FileAttributes.ReadOnly;

                    File.SetAttributes(filepath, attibutes);
                }
                catch { success = false; }
            }

            return success;
        }

        public static Exception TryDemandFullAccess(this string path)
        {
            Exception exception = null;
            try
            {
                AuthorizationRuleCollection collection = Directory.GetAccessControl(path).GetAccessRules(true,
                                                                                                         true,
                                                                                                         typeof(NTAccount));
                foreach (FileSystemAccessRule rule in collection)
                {
                    if (rule.AccessControlType == AccessControlType.Allow)
                        break;
                }

                var iopermission = new FileIOPermission(PermissionState.None);
                iopermission.AllLocalFiles = FileIOPermissionAccess.AllAccess;
                iopermission.Demand();
            }
            catch (UnauthorizedAccessException ex)
            {
                exception = ex;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return exception;
        }

        public static string StringConcat(this string source, string param, bool front = false)
        {
            var results = source;

            if (!string.IsNullOrEmpty(param))
                results = string.Format(front ? @"{1}{0}" : @"{0}{1}", source, param);

            return results;
        }

        public static ulong HexstringToUInt64(this string source)
        {
            ulong results;
            ulong.TryParse(source, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out results);
            return results;
        }

        public static List<byte[]> GetFileContenBytes(this string filepath, int chunksize = 32 * 1024)
        {
            var fileInfo = new FileInfo(filepath);

            var bufferList = new List<byte[]>();
            long filebytecount = fileInfo.Length;
            if (filebytecount < chunksize)
            {
                bufferList.Add(File.ReadAllBytes(filepath));
            }
            else
            {
                using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    var SizeTaken = 0;
                    var index = 0;
                    while (SizeTaken < filebytecount)
                    {
                        var chunks = (index + 1) * chunksize;
                        var takeSize = filebytecount >= chunks ? chunksize : (filebytecount - SizeTaken);

                        byte[] buffer = new byte[takeSize];
                        filestream.Read(buffer, 0, (int)takeSize);
                        bufferList.Add(buffer);

                        index++;
                        SizeTaken += (int)takeSize;
                    }
                }
            }

            return bufferList;
        }

        public static string GetFileFullPath(this string fileName)
        {
            return Path.Combine(Utility.GetExecutedPath(), fileName);
        }

        public static bool IsFileLocked(this string filepath)
        {
            var isfilelocked = false;

            try
            {
                using (var f = File.Open(filepath, FileMode.Open))
                {
                    f.Close();
                }
            }
            catch (IOException ioex)
            {
                var errorCode = Marshal.GetHRForException(ioex) & ((1 << 16) - 1);
                isfilelocked = errorCode == 32 || errorCode == 33;
            }
            catch (Exception) { }

            return isfilelocked;
        }

        public static void FileLock(this string filepath, bool lockfile)
        {
            using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                if (null != filestream)
                {
                    filestream.Seek(0, SeekOrigin.Begin);

                    if (lockfile)
                        filestream.Lock(0, filestream.Length);
                    else
                        filestream.Unlock(0, filestream.Length);
                }
            }
        }
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
        {
            return source.Skip(page * pageSize).Take(pageSize);
        }
        public static string IntToBinary(this int number)
        {
            return Enumerable.Range(0, (int)Math.Log(number, 2) + 1).Aggregate(string.Empty, (collected, bitshifts) => ((number >> bitshifts) & 1) + collected);
        }
        public static byte IntToByte(this int number)
        {
            return Convert.ToByte(number.ToString(), 16);
        }
        public static int? HexToInt(this string hex)
        {
            int? ret = null;
            try
            {
                return Convert.ToInt32(hex, 16);
            }
            catch { }
            return ret;
        }
        public static long? HexToLong(this string hex)
        {
            long? ret = null;
            try
            {
                return Convert.ToInt64(hex, 16);
            }
            catch { }
            return ret;
        }
        public static string ByteToHex(this byte bInput)
        {
            return string.Format("{0:X2}", bInput);
        }
        public static string IntToHexString(this int number)
        {
            return $"0x{number:X}";
        }
        public static string LongToHexString(this long number)
        {
            return $"0x{number:X}";
        }
        public static byte[] HexStringToByteArray(this string hexString)
        {
            if (!string.IsNullOrEmpty(hexString))
            {
                return Enumerable.Range(0, hexString.Length)
                                 .Where(x => x % 2 == 0)
                                 .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                                 .ToArray();
            }
            return null;
        }
    }
}
