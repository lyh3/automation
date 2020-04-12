using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using log4net;

namespace McAfeeLabs.Engineering.Automation.Base
{
    public class CommonUtility
    {
        public const string WindowsInstallEnvironmentVariableName = "windir";

        public static bool IsFileUsedbyAnotherProcess(string filename,
                                                       ILog logger = null,
                                                       string environmentInfo = null)
        {
            var results = false;
            Exception ex = null;
            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
            }
            catch (UnauthorizedAccessException exUnAuth) { ex = exUnAuth; results = true; }
            catch (IOException exIO) { ex = exIO; results = true; }

            if (null != ex && null != logger && !string.IsNullOrEmpty(environmentInfo))
                logger.Warn(string.Format(@"--- Exception caught at IsFileUsedbyAnotherProcess, sample in processing : {3}, the error was:{2}{0}, stack trace:{2}{1}",
                                              null != ex.InnerException ? ex.InnerException.Message : ex.Message,
                                              ex.StackTrace,
                                              Environment.NewLine,
                                              filename).StringConcat(environmentInfo, true));
            return results;

        }

        public static FileDetails CreateFileDetails( string path,
                                                     string runner,
                                                     ILog logger = null,
                                                     string environmentInfo = null)
        {

            FileDetails sample = null;

            var shouldSkip = !DemandFileAccessPersission(path, logger, environmentInfo)
                            || CommonUtility.IsFileUsedbyAnotherProcess(path, logger, environmentInfo);
            if (!shouldSkip)
            {
                shouldSkip = true;
                var info = new FileInfo(path);
                Exception ex = null;
                try
                {
                    sample = new FileDetails
                    {
                        Hash = info.MD5().ToString(),
                        FileNameWithoutPath = info.Name,
                        FilePath = path,
                        FileSize = info.Length,
                        Status = StatusType.UnSubmitted,
                        Date = DateTime.Now,
                        Runner = runner
                    };
                    shouldSkip = false;
                }
                catch (UnauthorizedAccessException exUnAuth) { ex = exUnAuth; }
                catch (System.IO.IOException exIO) { ex = exIO; }
                if (null != ex)
                    logger.Warn(string.Format(@"--- Exception caught at Get File Details, sample in processing : {3}, the error was:{2}{0}, stack trace:{2}{1}",
                                                  null != ex.InnerException ? ex.InnerException.Message : ex.Message,
                                                  ex.StackTrace,
                                                  Environment.NewLine,
                                                  path).StringConcat(environmentInfo, true));
            }

            return sample;
        }

        public static string AppconfigPath
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly(); 
                string appconfig = string.Format(@"{0}{1}.config", AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(assembly.Location));
                return appconfig;
            }
        }

        public static XmlDocument LoadAppConfigXml(string path = null)
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(!string.IsNullOrEmpty(path) ? path : AppconfigPath);
            return xmldoc;
        }

        static public void PingFtpSite(string uri)
        {
            UriBuilder ftpDetails = new UriBuilder(uri);

            var request = (FtpWebRequest)WebRequest.Create(ftpDetails.Uri);
            request.Credentials = new NetworkCredential(ftpDetails.UserName, ftpDetails.Password);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                }
            }
        }

        public static string GetExecutedPath()
        {
            string executePath = null;
            var asm = Assembly.GetExecutingAssembly();
            executePath = Path.GetDirectoryName(asm.CodeBase).Replace(@"file:\", string.Empty);
            return executePath;
        }

        public static T XmlRetrieve<T>(XmlDocument xmlDoc)
        {
            object o = null;

            if (null != xmlDoc && null != xmlDoc.DocumentElement)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                using (StringReader reader = new StringReader(xmlDoc.OuterXml))
                {
                    o = serializer.Deserialize(reader);
                }
            }

            return (T)o;
        }

        public static String XmlPersist<T>(object o)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, o);
            }
            string xml = sb.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", string.Empty).Replace("xmlns=\"\"", string.Empty);
            return xml;
        }

        public static IEnumerable<FileInfo> InspectDirectories(List<string> directoriesToSearch,
                                                            bool recurseIntoSubdirectories,
                                                            List<FileInfo> inspectList)
        {
            var searchOption = recurseIntoSubdirectories ?
                SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;


            var allFilePaths = from directory in directoriesToSearch
                               from file in GetAllFilesInDirectory(directory, searchOption, inspectList)
                               select file;

            var fileDetails = from filePath in allFilePaths
                              let info = new FileInfo(filePath)
                              where info != null
                              select info as FileInfo;

            return (IEnumerable<FileInfo>)fileDetails;
        }

        public static IEnumerable<string> GetAllFilesInDirectory(string directoryPath, SearchOption searchOption, List<FileInfo> resultList)
        {
            IEnumerable<string> files = null;
            IEnumerable<string> subdirectories = null;

            files = Directory.EnumerateFiles(directoryPath);
            subdirectories = Directory.EnumerateDirectories(directoryPath);

            if (files != null)
            {
                foreach (string file in files)
                {
                    //if (file.ShouldLoad(submittedList))
                        yield return file;
                }
            }
            if (subdirectories != null && searchOption == SearchOption.AllDirectories)
            {
                foreach (string subdirectory in subdirectories)
                {
                    // skip reparse points, aka junctions
                    if ((File.GetAttributes(subdirectory) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                    {
                        foreach (string file in GetAllFilesInDirectory(subdirectory, searchOption, resultList))
                        {
                            yield return file;
                        }
                    }
                }
            }
        }

        public static void InstallServices(bool install, Assembly assembly, string dotnetversion = "v4.0.30319")
        {
            try
            {
                string installUtilPath = string.Format(@"{0}\Microsoft.NET\Framework\{1}\installutil.exe",
                                                       Environment.GetEnvironmentVariable(WindowsInstallEnvironmentVariableName),
                                                       dotnetversion);
                if (null != assembly)
                {
                    var argFormat = string.Format(@"{0} {1}", "{0}.exe", install ? "" : " /u");

                    var installUtilProc = new Process();
                    installUtilProc.StartInfo.FileName = installUtilPath;
                    installUtilProc.StartInfo.Arguments = string.Format(argFormat, assembly.FullName.Substring(0, assembly.FullName.IndexOf(',')));
                    installUtilProc.Start();
                }
            }
            catch { }
        }

        public static bool BounceServices(string serviceName, string machineName)
        {
            bool success = false;
            int retrynumber = 3;

            for (int i = 0; i < retrynumber && !success; ++i)
            {
                var serviceControllerStop = GetServiceController(serviceName, machineName);
                if (null != serviceControllerStop)
                {

                    try
                    {
                        serviceControllerStop.Stop();
                        System.Threading.Thread.Sleep((i + 1) * 10 * 1000);
                        if (serviceControllerStop.Status == ServiceControllerStatus.Stopped)
                        {
                            success = true;
                            Trace.WriteLine(string.Format("---StopServices <{0}> on machine <{1}> with retry = <{2}>", serviceName, machineName, i));
                            break;
                        }
                    }
                    catch { }
                }
            }

            success = false;

            for (int i = 0; i < retrynumber && !success; ++i)
            {
                var serviceControllerStart = GetServiceController(serviceName, machineName);
                if (null != serviceControllerStart)
                {
                    try
                    {
                        serviceControllerStart.Start();
                        System.Threading.Thread.Sleep((i + 1) * 10 * 1000);
                        if (serviceControllerStart.Status == ServiceControllerStatus.Running)
                        {
                            success = true;
                            Trace.WriteLine(string.Format("---StartServices <{0}> on machine <{1}> with retry = <{2}>", serviceName, machineName, i));
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(string.Format("--!--BounceServices, exception caught : {0}", null == ex.InnerException ? ex.Message : ex.InnerException.Message));
                        if (null != ex.InnerException && ex.InnerException.Message.Trim().StartsWith("An instance of the service is already running"))
                            success = true;
                    }
                }
            }

            return success;
        }
        private static ServiceController GetServiceController(string serviceName, string machineName)
        {
            var serviceController = new ServiceController();
            serviceController.MachineName = machineName;
            serviceController.ServiceName = serviceName;
            return serviceController;
        }

        public static bool StopServices(string serviceName, string machineName)
        {
            bool success = true;
            var serviceController = new ServiceController();
            serviceController.MachineName = machineName;
            serviceController.ServiceName = serviceName;

            if (null != serviceController)
            {
                try
                {
                    serviceController.Stop();
                    Trace.WriteLine(string.Format("---StopServices <{0}> on machine <{1}>", serviceName, machineName));
                }
                catch (Exception ex)
                {
                    success = false;
                    Trace.WriteLine(string.Format("--!--Helper:StopServices, exception caught : {0}", ex.Message));
                }
            }

            return success;
        }

        public static bool StartServices(string serviceName, string machineName)
        {
            bool success = true;
            var serviceController = new ServiceController();
            serviceController.MachineName = machineName;
            serviceController.ServiceName = serviceName;

            if (null != serviceController)
            {
                try
                {
                    serviceController.Start();
                    Trace.WriteLine(string.Format("---StartServices <{0}> on machine <{1}>", serviceName, machineName));
                }
                catch (Exception ex)
                {
                    success = false;
                    Trace.WriteLine(string.Format("--!--StartServices, exception caught : {0}", ex.Message));
                }

            }

            return success;
        }

        public static IEnumerable<T> Unfold<T, State>(State seed, Func<State, Tuple<T, State>> f)
        {
            Tuple<T, State> res;
            while ((res = f(seed)) != null)
            {
                yield return res.Item1;
                seed = res.Item2;
            }
        }

        private static bool DemandFileAccessPersission(string filepath,
                                                        ILog logger = null,
                                                        string environmentInfo = null)
        {
            var success = true;

            try
            {
                new FileIOPermission(FileIOPermissionAccess.AllAccess, filepath).Demand();
            }
            catch (Exception ex)
            {
                if(null != logger && !string.IsNullOrEmpty(environmentInfo))
                    logger.Warn(string.Format(@"--- Exception caught at DemandFileAccessPersission, sample in processing : {3}, the error was:{2}{0}, stack trace:{2}{1}",
                                                  null != ex.InnerException ? ex.InnerException.Message : ex.Message,
                                                  ex.StackTrace,
                                                  Environment.NewLine,
                                                  filepath).StringConcat(environmentInfo, true));
                success = false;
            }

            return success;
        }
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static extern bool SetProcessWorkingSetSize(IntPtr process,
                                                            UIntPtr minimumWorkingSetSize,
                                                            UIntPtr maximumWorkingSetSize);
    }
}
