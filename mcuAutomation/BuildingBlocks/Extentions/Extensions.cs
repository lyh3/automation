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
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Management;
using System.Configuration;

using log4net;

namespace McAfeeLabs.Engineering.Automation.Base
{
    static public class Extensions
    {
        [ThreadStatic]
        private static Random _random = null;

        public static bool IsPrintableCharacters(this string input)
        {
            var validPattern = @"^[a-zA-Z0-9{}[\]\\;':,./?!@#$%&*()_+=-~^`\u0022 ]+$";
            bool isValid = false;
            Match match = null; 
            var regEx = new Regex(validPattern,
                                  RegexOptions.IgnoreCase
                                | RegexOptions.CultureInvariant
                                | RegexOptions.IgnorePatternWhitespace
                                | RegexOptions.Compiled);

            match = regEx.Match(input);

            if (null != match && match.Success)
            {
                isValid = true;
            }

            return isValid;
        }

        public static Configuration GetExternalConfiguration(this string configFilePath)
        {
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };
            return ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        }

        public static dynamic GetFreeDiskSpace(this string diskName)
        {
            var disk = new ManagementObject(string.Format(@"win32_logicaldisk.deviceid={0}{1}{0}","\"", diskName.TrimEnd('\\') ));
            disk.Get();
            return disk["FreeSpace"];
        }

        public static string ResolveSingleFilePath(this string sourcePath)
        {
            var results = string.Empty;

            if (!string.IsNullOrEmpty(sourcePath))
            {
                if (sourcePath.Length < GlobalDefinitions.MAX_PATH)
                {
                    if (System.IO.File.Exists(sourcePath))
                    {
                        var path = System.IO.Path.GetDirectoryName(sourcePath);
                        if (sourcePath.StartsWith(@".\"))
                        {
                            path = string.Empty;
                            sourcePath = sourcePath.Replace(@".\", string.Empty);
                        }
                        path = string.IsNullOrEmpty(path) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sourcePath)
                                                          : sourcePath;
                        results = path;
                    }
                }

                if(string.IsNullOrEmpty(results))
                {
                    if (sourcePath.StartsWith(@".\"))
                        sourcePath = sourcePath.Replace(@".\", AppDomain.CurrentDomain.BaseDirectory); 
                    var longformat = string.Format(@"\\?\{0}", sourcePath);
                    var fileInfo = new Delimon.Win32.IO.FileInfo(longformat);
                    var dirInfo = new Delimon.Win32.IO.DirectoryInfo(sourcePath);
                    if (Delimon.Win32.IO.File.Exists(longformat)
                        && fileInfo.Length > 0)
                        results = longformat;
                }
            }

            return results;
        }

        public static string PingWorkstation(this string workstation)
        {
            var ipAddress = string.Empty;
            if (!string.IsNullOrEmpty(workstation))
            {
                var pingSender = new Ping();
                var options = new PingOptions();// Use the default Ttl value which is 128
                options.DontFragment = true;
                var data = @"********************************";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                PingReply reply = pingSender.Send(workstation,
                                                   timeout,
                                                   buffer,
                                                   options);
                if (reply.Status == IPStatus.Success)
                {
                    ipAddress = reply.Address.ToString();
                    if (-1 != ipAddress.IndexOf(@"::"))
                        ipAddress = @"localhost";//@"127.0.0.1";//loop back address
                }
            }

            return ipAddress;
        }

        public static string FormatFtpUri(this string ftpuri,
                                          string userName,
                                          string passWord,
                                          string subName = null)
        {
            return string.IsNullOrEmpty(subName)
                      ? string.Format(@"{0}{1}:{2}@{3}", GlobalDefinitions.FtpPrefix, userName, passWord, ftpuri.Replace(GlobalDefinitions.FtpPrefix, string.Empty))
                      : string.Format(@"{0}{1}:{2}@{3}/{4}", GlobalDefinitions.FtpPrefix, userName, passWord, ftpuri.Replace(GlobalDefinitions.FtpPrefix, string.Empty), subName);
        }

        public static string ByteArrayTOHexString(this byte[] bytes)
        {
            return String.Concat(Array.ConvertAll(bytes, x => x.ToString("X2")));
        }

        public static bool IsTimeOut(this TimeSpan timeOut, DateTime timeSince)
        {
            return DateTime.Now - timeSince > timeOut;
        }

        public static bool OpenSqlConnection( this SqlConnection connection,
                                              int timeout = 5, //in seconds
                                              ILog logger = null,
                                              string errorMessage = null )
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
                    if (null != logger)
                        logger.Error(errors.ToString());
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

        public static void LoadEmbeddedXml( this string sourceFileName, 
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
                catch{ success = false; }
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
            return Path.Combine(CommonUtility.GetExecutedPath(), fileName);
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

        //[DllImport("KERNEL32.DLL", EntryPoint = "MoveFileW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        //public static bool MoveFile(string src, string dst);

        //[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        //public static bool MoveFileEx(string lpExistingFileName, string lpNewFileName, Win32Enums.MoveFileFlags dwFlags);

        //var securityDescriptorLength = /* Win32 Call */ GetSecurityDescriptorLength( pSecurityDescriptor );

        //// Define array to copy
        //var securityDescriptorDataArray = new byte[ securityDescriptorLength ];

        //// Copy by marshal to defined array
        ///* Win32 Call */ Marshal.Copy( pSecurityDescriptor, securityDescriptorDataArray, 0, ( int ) securityDescriptorLength );

        //// If path is directory
        //var securityInfo = new DirectorySecurity( );
        //securityInfo.SetSecurityDescriptorBinaryForm( securityDescriptorDataArray );
    }
}
