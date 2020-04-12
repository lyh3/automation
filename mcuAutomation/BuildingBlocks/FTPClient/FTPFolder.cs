using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Reflection;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

using McAfeeLabs.Engineering.Automation.AOP;

namespace McAfeeLabs.Engineering.Automation.Base.FTPClient
{
    public partial class FTPFolder : FTPClient, IFTPFolder
    {
        #region Declaration

        public const string PerfofmanceCounterCategory = @"AOP Ftp Folder Performance Counters";
        private string _folderName;

        #endregion

        #region Constructors

        public FTPFolder() { }
        public FTPFolder(string uri) : base(uri) { }

        /// <summary>
        /// To use these two constructors the input uri mus be in format ftp://{directory}
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="port"></param>

        public FTPFolder(string uri, string userName, string password)
        {
            var split = Regex.Split(uri.Replace(GlobalDefinitions.FtpPrefix, string.Empty), PathSeparator.ToString());
            if (split.Length < 1)
                throw new ArgumentException(@"The subfolder name must be specified."); var uriDetails = new UriBuilder(uri);
            Initialize(ComposeInitialUriString(uri, userName, password));
        }

        #endregion

        #region Properties

        public bool Exists
        {
            get
            {
                bool exists = false;
                try
                {
                    var ErrorMessageHeader = string.Format(@"Chech folder exit = <{0}>", _folderName);
                    var request = CreateRequest(WebRequestMethods.Ftp.ListDirectory);
                    using (var response = (FtpWebResponse)request.GetResponse())
                    {
                        var responseStream = response.GetResponseStream();
                        if (null != responseStream)
                        {
                            using (var streamreader = new StreamReader(responseStream))
                            {
                                try
                                {
                                    var content = streamreader.ReadToEnd();
                                    exists = true;
                                }
                                catch (WebException webex)
                                {
                                    _lastexception = webex;
                                    _lastErrorMessage = string.Format(ErrorMessageFormat,
                                                                       ErrorMessageHeader,
                                                                       null != webex.InnerException ? webex.InnerException.Message : webex.Message,
                                                                       webex.StackTrace);
                                }
                                catch (Exception ex)
                                {
                                    _lastexception = ex;
                                    _lastErrorMessage = string.Format(ErrorMessageFormat,
                                                                       ErrorMessageHeader,
                                                                       null != ex.InnerException ? ex.InnerException.Message : ex.Message,
                                                                       ex.StackTrace);
                                }
                            }
                        }
                        response.Close();
                    }
                }
                catch (WebException) { }

                return exists;
            }
        }

        public string FolderName { get { return _folderName; } }

        #endregion

        #region Public Methods

        override public void Initialize(string uri)
        {
            _ftpuri = uri;
            base.InitializeUri(uri);

            var split = Regex.Split(uri.Replace(GlobalDefinitions.FtpPrefix, string.Empty).TrimEnd(new[] { PathSeparator }),
                                    PathSeparator.ToString());
            if (split.Length > 1)
                _folderName = split[split.Length - 1];
        }

        override public int CompareTo(object obj)
        {
            var o = obj as FTPFolder;
            return FtpUri == o.FtpUri
                   && FolderName == o.FolderName ? 1 : 0;
        }
#if USE_PERFORMANCE_COUNTER
        [IncrementCounter(Increment = "FtpListDirectory")]
        [DurationCounter(Clock = "FtpListDirectory")]
#endif
        public bool ListDirectory(out List<String> folderList, out List<String> fileList, string folderName = "")
        {
            _lastexception = null;
            bool success = false;
            const string dirDelimiterPattern = @"<DIR>";
            const string fileDelimiterPattern = @" ";
            List<string> folders = new List<string>(), files = new List<string>();
            string ErrorMessageHeader = string.Format(@"ListDirectory <{0}>", folderName);

            try
            {
                var request = CreateRequest(WebRequestMethods.Ftp.ListDirectoryDetails, folderName);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    var ftpstream = response.GetResponseStream();
                    {
                        if (null != ftpstream)
                        {
                            using (var txtreader = new StreamReader(ftpstream))
                            {
                                var line = string.Empty;
                                while (line != null)
                                {
                                    line = txtreader.ReadLine();
                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        var dirSplit = Regex.Split(line, dirDelimiterPattern);
                                        if (dirSplit.Length == 2)
                                            folders.Add(dirSplit[1].Trim());
                                        else
                                        {
                                            //NOTE: Due to the limitatin of ftp, the remote file name can't be parsed correctly if contains white space
                                            var fileSplit = Regex.Split(line, fileDelimiterPattern);
                                            if (dirSplit.Length > 0)
                                                files.Add(fileSplit[fileSplit.Count() - 1]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                success = true;
            }
            catch (WebException webex)
            {
                _lastexception = webex;
                _lastErrorMessage = string.Format(ErrorMessageFormat,
                                                   ErrorMessageHeader,
                                                   null != webex.InnerException ? webex.InnerException.Message : webex.Message,
                                                   webex.StackTrace);
            }
            catch (Exception ex)
            {
                _lastexception = ex;
                _lastErrorMessage = string.Format(ErrorMessageFormat,
                                                   ErrorMessageHeader,
                                                   null != ex.InnerException ? ex.InnerException.Message : ex.Message,
                                                   ex.StackTrace);
            }

            if (_isBubbleUpException && null != _lastexception)
                throw _lastexception;

            folderList = folders;
            fileList = files;

            return success;
        }

#if USE_PERFORMANCE_COUNTER
        [IncrementCounter(Increment = "FtpCreateSubFolder")]
        [DurationCounter(Clock = "FtpCreateSubFolder")]
#endif
        public bool CreateSubFolder(string folderName, int retry = 0)
        {
            _lastexception = null;
            string ErrorMessageHeader = string.Format(@"CreateSubFolder folder name = <{0}>", folderName);
            bool success = false;
            var retrynumber = retry <= 0 ? 0 : retry;

            if (string.IsNullOrEmpty(folderName)) throw new ArgumentException("The folder name is invalid for creating subfolder");

            for (int i = 0; i < retrynumber + 1 && !success; ++i)
            {
                try
                {
                    var request = CreateRequest(WebRequestMethods.Ftp.MakeDirectory, folderName);
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { success = true; response.Close(); }
                }
                catch (WebException webex)
                {
                    _lastexception = webex;
                    _lastErrorMessage = string.Format(ErrorMessageFormat,
                                                       ErrorMessageHeader,
                                                       null != webex.InnerException ? webex.InnerException.Message : webex.Message,
                                                       webex.StackTrace);
                }
                catch (Exception ex)
                {
                    _lastexception = ex;
                    _lastErrorMessage = string.Format(ErrorMessageFormat,
                                                       ErrorMessageHeader,
                                                       null != ex.InnerException ? ex.InnerException.Message : ex.Message,
                                                       ex.StackTrace);
                }
            }

            if (_isBubbleUpException && null != _lastexception)
                throw _lastexception;

            return success;
        }

#if USE_PERFORMANCE_COUNTER
        [IncrementCounter(Increment = "FtpDeleteSubFolder")]
        [DurationCounter(Clock = "FtpDeleteSubFolder")]
#endif
        public bool DeleteSubFolder(string folderName)
        {
            bool success = false;
            if (string.IsNullOrEmpty(folderName)) throw new ArgumentException("The folder name is invalid for deleting subfolder");
            var folderList = new List<string>();
            var fileList = new List<string>();

            string message = string.Empty;
            if (IsSubFoderExisted(folderName))
            {
                ListDirectory(out folderList, out fileList, folderName);

                if (folderList.Count > 0) throw new ApplicationException(string.Format(@"The folder <{0}> contains sub folders and can't be deleted.", folderName));

                fileList.ForEach(file =>
                {
                    var uri = string.Format(@"{0}{1}{2}{1}{3}",
                                            FtpUri,
                                            PathSeparator,
                                            folderName,
                                            file);
                    var ftpFile = new FTPFile(uri);
                    ftpFile.DeleteFile();
                });

                var request = CreateRequest(WebRequestMethods.Ftp.RemoveDirectory, folderName);
                using (var response = request.GetResponse())
                {
                    success = true;
                    response.Close();
                }
            }
            else
                message = @"Folder is not existed";

            return success;
        }

        #endregion

        #region Private Methods

        private bool IsSubFoderExisted(string folderName)
        {
            var folderList = new List<string>();
            var fileList = new List<string>();
            ListDirectory(out folderList, out fileList);

            return null != folderList.FirstOrDefault<string>(x => String.Compare(x, folderName, ignoreCase: true) == 0);
        }

        #endregion
    }
}
