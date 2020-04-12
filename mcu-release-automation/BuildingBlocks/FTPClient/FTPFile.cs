using System;
using System.Net;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension;

using McAfeeLabs.Engineering.Automation.AOP;

namespace McAfeeLabs.Engineering.Automation.Base.FTPClient
{
    public partial class FTPFile : FTPClient, IFTPFile
    {
        #region Declarations

        public const string PerfofmanceCounterCategory = @"AOP Ftp File Performance Counters";
        private const int DefaultChunckThreashold = 2 * 1024 * 1024;
        private const int MaxSize = 100 * 1024 * 1024;
        private int _chunkSize = DefaultChunckThreashold;
        private string _fileName;

        #endregion

        #region Constructors

        public FTPFile() { }

        public FTPFile(string uri) : base(uri) { }

        /// <summary>
        /// To use these two constructors the input uri mus be in format ftp://{directory}/{fileName}
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="port"></param>
        public FTPFile(string uri, string userName, string password)
        {
            _ftpuri = uri;
            _username = userName;
            _password = password;
            var split = Regex.Split(uri.Replace(GlobalDefinitions.FtpPrefix, string.Empty), PathSeparator.ToString());
            if (split.Length < 3)
                throw new ArgumentException(@"The subfolder and file name must be specified.");

            Initialize(ComposeInitialUriString(uri, userName, password));
        }
        #endregion

        #region Properties


        public bool Exists
        {
            get
            {
                string ErrorMessageHeader = string.Format(@"Check file <{0}> existing", _fileName);
                _lastErrorMessage = string.Empty;

                bool exists = false;
                FtpWebResponse response = null;
                try
                {
                    var request = CreateRequest(WebRequestMethods.Ftp.GetFileSize);
                    response = (FtpWebResponse)request.GetResponse();
                    if (null != response)
                    {
                        exists = true;
                        response.Close();
                    }
                }
                catch (WebException webex)
                {
                    _lastErrorMessage = string.Format(ErrorMessageFormat,
                                                       ErrorMessageHeader,
                                                       null != webex.InnerException ? webex.InnerException.Message : webex.Message,
                                                       webex.StackTrace);
                }
                catch (Exception ex)
                {
                    _lastErrorMessage = string.Format(ErrorMessageFormat,
                                                       ErrorMessageHeader,
                                                       null != ex.InnerException ? ex.InnerException.Message : ex.Message,
                                                       ex.StackTrace);
                }
                finally { if (null != response) response.Close(); }

                return exists;
            }
        }

        public int ChunkSize
        {
            get { return _chunkSize; }
            set
            {
                if (value != _chunkSize)
                {
                    if (value < DefaultChunckThreashold)
                        _chunkSize = DefaultChunckThreashold;
                    else if (value > MaxSize)
                        _chunkSize = MaxSize;
                    else
                        _chunkSize = value;
                }
            }
        }

        public string FileName { get { return _fileName; } }

        #endregion

        #region Public Methods

        override public void Initialize(string uri)
        {
            base.InitializeUri(uri);

            int idxPathSeparator = uri.LastIndexOf(PathSeparator);
            int idxDoubleSeparator = uri.IndexOf("//");
            if (-1 != idxPathSeparator && -1 != idxDoubleSeparator)
            {
                if (idxPathSeparator - 1 != idxDoubleSeparator)
                    _fileName = uri.Substring(idxPathSeparator, uri.Length - idxPathSeparator).TrimStart(PathSeparator);
            }

            if (!string.IsNullOrEmpty(_fileName) && _fileName.Contains(@" ")) throw new ArgumentException(@"White space in file name is not allowed");
        }

        override public int CompareTo(object obj)
        {
            var o = obj as FTPFile;
            return FtpUri == o.FtpUri
                   && FileName == o.FileName ? 1 : 0;
        }

        /// <summary>
        /// The default value for retry = 0 implys that the actio only takes once
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="retry"></param>
        /// <returns></returns>
#if USE_PERFORMANCE_COUNTER
        [IncrementCounter(Increment = "FtpUploadFile")]
        [DurationCounter(Clock = "FtpUploadFile")]
#endif
        public bool UploadFile(string localPath, int retry = 0)
        {
            _lastexception = null;
            string ErrorMessageHeader = string.Format(@"Upload file <{0}>", localPath);
            bool success = false;
            if (string.IsNullOrEmpty(localPath) || !File.Exists(localPath)) throw new ArgumentException("The filePath name is invalid or the file is not existed for uploading");
            var fileInfo = new FileInfo(localPath);
            var currentFilename = _fileName;
            var fileName = Path.GetFileName(localPath);
            var retrynumber = retry <= 0 ? 0 : retry;
#if OVER_RIDE //Currently the FTPFile require user provide the file name when instanciate the class and the file name cann't be change afterwards
                          //if the behavior need to be changed, the code below need to be used.
            if (fileName != currentFilename)
                Initialize(FtpUri.Replace(currentFilename, fileName));
#endif

            var buffers = localPath.GetFileContenBytes(chunksize: ChunkSize);
            var append = buffers.Count > 1 ? true : false;
            if (!append && Exists) DeleteFile();

            for (int i = 0; i < retrynumber + 1 && !success; ++i)
            {
                try
                {
                    buffers.ForEach(buffer => { UploadFile(buffer, append); });
                    if (Exists && GetFileSize() == fileInfo.Length)
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
            }

            if (_isBubbleUpException && null != _lastexception)
                throw _lastexception;

            return success;
        }

#if USE_PERFORMANCE_COUNTER
        [IncrementCounter(Increment = "FtpUploadFileByBytes")]
        [DurationCounter(Clock = "FtpUploadFileByBytes")]
#endif
        public bool UploadFile(byte[] content, bool append = false, int retry = 0)
        {
            _lastexception = null;
            const string ErrorMessageHeader = @"Upload file from byte array";
            bool success = false;
            if (content == null) throw new ArgumentException("binary content is null.");
            Stream stream = null;
            var retrynumber = retry <= 0 ? 0 : retry;

            for (int i = 0; i < retrynumber + 1 && !success; ++i)
            {
                try
                {
                    var request = CreateRequest(append ? WebRequestMethods.Ftp.AppendFile
                                                       : WebRequestMethods.Ftp.UploadFile);
                    stream = request.GetRequestStream();
                    if (null != stream)
                    {
                        var bw = new BinaryWriter(stream);
                        if (null != bw)
                        {
                            var size = content.Length;
                            var writtenSize = 0;
                            var index = 0;
                            while (writtenSize < size)
                            {
                                var chunks = (index + 1) * _chunkSize;
                                var writeSize = size >= chunks ? _chunkSize : (size - writtenSize);

                                bw.Write(content, index * _chunkSize, writeSize);
                                bw.Flush();

                                index++;
                                writtenSize += writeSize;
                            }
                            success = true;
                        }
                    }
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
                finally
                {
                    if (null != stream)
                        stream.Close();
                }
            }

            if (_isBubbleUpException && null != _lastexception)
                throw _lastexception;

            return success;
        }

        public bool UploadStream(Stream stream, int retry = 0)
        {
            bool success = false;
            var page = 0;
            var takencount = 0;
            var buffers = new List<byte[]>();

            var size = stream.Length; stream.Seek(0, SeekOrigin.Begin);

            while (takencount < size)
            {
                var chunks = (page + 1) * ChunkSize;
                var takeSize = size >= chunks ? ChunkSize : (size - takencount);

                byte[] buffer = new byte[takeSize];
                stream.Read(buffer, 0, (int)takeSize);
                buffers.Add(buffer);

                page++;
                takencount += (int)takeSize;
            }

            buffers.ForEach(buffer =>
            {
                success = UploadFile(buffer, buffers.Count > 1, retry: retry);
            });

            return success;
        }

        /// <summary>
        /// The default value for retry = 0 implys that the actio only takes once
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="retry"></param>
        /// <returns></returns>
#if USE_PERFORMANCE_COUNTER
        [IncrementCounter(Increment = "FtpDownloadFile")]
        [DurationCounter(Clock = "FtpDownloadFile")]
#endif
        public bool DownloadFile(String localfilepath, int retry = 0)
        {
            _lastexception = null;
            bool success = false;
            var retrynumber = retry <= 0 ? 0 : retry;

            string ErrorMessageHeader = string.Format(@"Download file to local folder <{0}>", localfilepath);

            for (int i = 0; i <= retrynumber && !success; ++i)
            {
                FtpWebResponse response = null;
                try
                {
                    var request = CreateRequest(WebRequestMethods.Ftp.DownloadFile);
                    response = (FtpWebResponse)request.GetResponse();

                    using (var responseStream = response.GetResponseStream())
                    {
                        if (null != responseStream)
                        {
                            using (FileStream fileStream = File.Create(localfilepath))
                            {
                                if (null != fileStream)
                                {
                                    var br = new BinaryReader(responseStream);
                                    byte[] outbyte = new byte[_chunkSize];
                                    int retval;
                                    retval = br.Read(outbyte, 0, _chunkSize);
                                    while (retval > 0)
                                    {
                                        fileStream.Write(outbyte, 0, retval);
                                        fileStream.Flush();

                                        retval = br.Read(outbyte, 0, _chunkSize);
                                    }
                                    success = true;
                                }
                            }
                        }
                    }
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
                finally
                {
                    if (null != response)
                        response.Close();
                }
            }

            if (_isBubbleUpException && null != _lastexception)
                throw _lastexception;

            return success;
        }

        /// <summary>
        /// The default value for retry = 0 implys that the actio only takes once
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="retry"></param>
        /// <returns></returns>
#if USE_PERFORMANCE_COUNTER
        [IncrementCounter(Increment = "FtpDownloadFileByBytes")]
        [DurationCounter(Clock = "FtpDownloadFileByBytes")]
#endif
        public byte[] DownloadFile(int retry = 0)
        {
            _lastexception = null;
            bool success = false;
            byte[] bytes = null;
            string message = string.Empty;
            var retrynumber = retry <= 0 ? 0 : retry;
            const string ErrorMessageHeader = @"DownloadFile to byte array";

            for (int i = 0; i <= retry && !success; ++i)
            {
                try
                {
                    var request = CreateRequest(WebRequestMethods.Ftp.DownloadFile);
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        var stream = response.GetResponseStream();
                        if (null != stream)
                        {
                            using (var br = new BinaryReader(stream))
                            {
                                using (var mem = new MemoryStream())
                                {
                                    byte[] outbyte = new byte[ChunkSize];
                                    int retval;
                                    retval = br.Read(outbyte, 0, ChunkSize);
                                    while (retval > 0)
                                    {
                                        mem.Write(outbyte, 0, retval);
                                        mem.Flush();
                                        retval = br.Read(outbyte, 0, ChunkSize);
                                    }

                                    bytes = new byte[mem.Length];
                                    mem.Seek(0, SeekOrigin.Begin);
                                    mem.Read(bytes, 0, (int)mem.Length);
                                }
                            }
                        }
                        if (bytes.Length == GetFileSize())
                            success = true;
                    }
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

            return bytes;
        }

#if USE_PERFORMANCE_COUNTER
        [IncrementCounter(Increment = "FtpDeleteFile")]
        [DurationCounter(Clock = "FtpDeleteFile")]
#endif
        public bool DeleteFile(int retry = 0)
        {
            _lastexception = null;
            bool success = false;
            var retrynumber = retry <= 0 ? 0 : retry;

            const string ErrorMessageHeader = @"DeleteFile";

            for (int i = 0; i <= retry && !success; ++i)
            {
                try
                {
                    var request = CreateRequest(WebRequestMethods.Ftp.DeleteFile);
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        success = true;
                    }
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
        [IncrementCounter(Increment = "FtpGetFileSize")]
        [DurationCounter(Clock = "FtpGetFileSize")]
#endif
        public long GetFileSize(int retry = 0)
        {
            _lastexception = null;
            bool success = false;
            if (string.IsNullOrEmpty(_fileName)) throw new ArgumentException("The fileName name is invalid for file size");
            var size = -1L;
            const string ErrorMessageHeader = @"GetFileSize";

            var request = CreateRequest(WebRequestMethods.Ftp.GetFileSize);
            for (int i = 0; i <= retry && !success; ++i)
            {
                try
                {
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        size = (long)response.ContentLength;
                        success = true;
                    }
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

            return size;
        }

        #endregion

    }
}
