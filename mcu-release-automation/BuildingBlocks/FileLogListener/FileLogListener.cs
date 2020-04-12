using System;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace McAfeeLabs.Engineering.Automation.Base.FileLogListener
{
    [Serializable]
    public class FileLogListener : TraceListener, IDisposable
    {
        #region Declarations

        public const string LOGFILE_TIME_STAMP_FORMAT = @"MMddyyyy";
        protected string _logFilename = string.Empty;
        protected string _filePath = @".\";
        protected FileStream _fileStream = null;
        protected DateTime _lastRenewDate;
        protected int _refreshDays = 10;
        protected int _garbageDays = 20;

        #endregion

        #region Constructor

        protected FileLogListener(string logFilename)
        {
            _logFilename = logFilename;
        }

        ~FileLogListener()
        {
            Dispose();
        }

        #endregion

        #region Properties

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public string LogFileName 
        {
            get
            {
                return string.Format(@"{0}\{1}_{2}.LOG",
                                    LogFileDirectory,
                                    _logFilename,
                                    DateTime.Now.ToString(LOGFILE_TIME_STAMP_FORMAT));
            }
        }

        private string LogFileDirectory { get { return string.Format(@"{0}\{1}", _filePath, _logFilename); } }
        
        #endregion

        #region Public Methods

        static public FileLogListener AddFileLogListener(object client)
        {
            if (null == client) return null;
            return AddListner(client.GetType().Name);
        }

        static public FileLogListener AddFileLogListener(string logFilename)
        {
            return AddListner(logFilename);
        }

        public override void Write(string message)
        {
            this.WriteLine(message);
        }

        override public void WriteLine(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            byte[] buffer = UTF8Encoding.UTF8.GetBytes(message);
            var stream = LogFileStream;
            if (null != stream)
            {
                lock (_fileStream)
                {
                    try
                    {
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Flush();
                        stream.Close();
                    }
                    catch(Exception ex) 
                    { 
                    }
                }
            }
        }
        
        #endregion

        #region Private Methods

        private static FileLogListener AddListner(string logFilename)
        {
            var fileLogListener = new FileLogListener(logFilename);
            Trace.Listeners.Add(fileLogListener);
            return fileLogListener;
        }

        protected override void Dispose(bool disposing)
        {
            if (null != _fileStream)
                _fileStream.Dispose();
            base.Dispose(disposing);
        }

        private FileStream LogFileStream
        {
            get
            {
                var createStream = false;

                var timeNow = DateTime.Now;
                if (null == _fileStream
                    || _lastRenewDate.Year != timeNow.Year
                    || timeNow.Day - _lastRenewDate.Day >= _refreshDays)
                    createStream = true;

                try
                {
                    if (createStream)
                    {
                        _CreateFileStream();
                    }
                    else
                    {
                        var fileName = _fileStream.Name;
                        _fileStream.Dispose();
                        _fileStream = File.Open(fileName, FileMode.Append, FileAccess.Write);
                    }
                }
                catch (Exception ex)
                {
                    createStream = true;
                    Trace.WriteLine(string.Format("--FileLogListener:LogFileStream, exception caught : {0}", ex.Message));
                }

                string filePath = _fileStream.Name;
                filePath = filePath.Replace(Path.GetFileName(filePath), string.Empty);
                _FileGarbageCollection(filePath);

                return _fileStream;
            }
        }

        protected void _CreateFileStream()
        {
            var fileName = LogFileName;
            var directory = LogFileDirectory;
            var sb = new StringBuilder();

            try
            {
                if (null != _fileStream)
                {
                    _fileStream.Dispose();
                }

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                _fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                _lastRenewDate = File.GetCreationTime(fileName);
            }
            catch (Exception ex)
            {
                sb.Append(string.Format("---{0}: CreateFileStream failed, fileName = <{1}>, exception = {2}.The log will be redirected to current working directory :", this.ToString(), fileName, ex.Message));
                directory = string.Format(@"{0}Log\{1}",
                    AppDomain.CurrentDomain.BaseDirectory,
                    _logFilename);
                fileName = string.Format(@"{0}\{1}_{2}.LOG",
                    directory,
                    _logFilename,
                    DateTime.Now.ToString(LOGFILE_TIME_STAMP_FORMAT));
                sb.Append(fileName);
            }
        }

        private void _FileGarbageCollection(string path)
        {
            var timeNow = DateTime.Now;
            try
            {
                var fileCollection = Directory.GetFiles(path);
                foreach (var fileName in fileCollection)
                {
                    var createTime = File.GetCreationTime(fileName);
                    if (createTime.Year != timeNow.Year
                        || timeNow.Day - createTime.Day >= _garbageDays)
                    {
                        File.Delete(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("---Exception caught at FileGarbageCollection, error was: {0}", ex.Message));
            }
        }
        #endregion
    }
}
