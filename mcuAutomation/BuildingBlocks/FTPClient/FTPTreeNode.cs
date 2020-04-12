using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace McAfeeLabs.Engineering.Automation.Base.FTPClient
{
    public class FTPTreeNode : FTPClient
    {
        #region Declaration

        protected IFTPFolder _ftpFolder = null;
        protected List<FTPTreeNode> _ftpTreeNodeList = new List<FTPTreeNode>();
        protected List<IFTPFile> _ftpFileList = new List<IFTPFile>();
        protected string _localFolderRoot;
        protected bool _isDownload;

        #endregion

        #region Constructors

        public FTPTreeNode() { }

        public FTPTreeNode(string uri) : base(uri) { }

        public FTPTreeNode(string uri, string userName, string password)
        {
            _ftpuri = uri;
            _username = userName;
            _password = password;
        }

        public FTPTreeNode(string uri,
                            string userName,
                            string password,
                            string localFolderPath,
                            bool isDownload = true)
            : this(uri, userName, password)
        {
            _isDownload = isDownload;
            LocalFolderRoot = localFolderPath;
        }

        #endregion

        #region Properties

        public IFTPFolder Folder { get { return _ftpFolder; } }
        public List<IFTPFile> FileList { get { return _ftpFileList; } }
        public List<FTPTreeNode> SubNodeList { get { return _ftpTreeNodeList; } }
        public string LocalFolderRoot
        {
            get { return _localFolderRoot; }
            set
            {
                _localFolderRoot = value;
                if (_isDownload
                   && !string.IsNullOrEmpty(_localFolderRoot)
                   && !Directory.Exists(_localFolderRoot))
                {
                    Directory.CreateDirectory(_localFolderRoot);
                }
            }
        }
        public bool IsDownload
        {
            get { return _isDownload; }
            set { _isDownload = value; }
        }

        #endregion

        override public void Initialize(string uri)
        {
            base.InitializeUri(uri);

            _ftpFolder = new FTPFolder(_ftpuri.FormatFtpUri(_username, _password)) { PassiveMode = false, IsBubbleUpException = _isDownload };

            var strFolderList = new List<string>();
            var strFileList = new List<string>();
            _ftpFolder.ListDirectory(out strFolderList, out strFileList);

            strFileList.ForEach(fileName =>
            {
                var ftpFile = new FTPFile(_ftpuri.FormatFtpUri(_username, _password, fileName)) { PassiveMode = false, IsBubbleUpException = _isDownload };
                if (null != TimeOut)
                    ftpFile.TimeOut = TimeOut;

                _ftpFileList.Add(ftpFile);
                if (_isDownload)
                {
                    ftpFile.DownloadFile(Path.Combine(LocalFolderRoot, fileName));
                }
            });

            strFolderList.ForEach(subfolder =>
            {
                var subUri = string.Format(@"{0}/{1}", _ftpuri, subfolder);
                var localFolder = Path.Combine(_localFolderRoot, subfolder);
                var ftpTreeNode = new FTPTreeNode(subUri,
                                                   _username,
                                                   _password,
                                                   localFolder,
                                                   IsDownload) { PassiveMode = false, TimeOut = TimeOut };
                _ftpTreeNodeList.Add(ftpTreeNode);
                ftpTreeNode.Initialize(ComposeInitialUriString(subUri, _username, _password));
            });
        }

        override public int CompareTo(object obj)
        {
            var o = obj as FTPTreeNode;
            int results = -1;
            if (null != _ftpFolder)
                results = o.Folder == this._ftpFolder ? 1 : 0;

            _ftpFileList.ForEach(x =>
            {
                results &= null != o.FileList.FirstOrDefault(n => n == x) ? 1 : 0;
            });

            return results;
        }
    }
}
