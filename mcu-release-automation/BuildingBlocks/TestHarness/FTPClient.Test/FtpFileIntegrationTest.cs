//#define SkipTest
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;

using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Base.FTPClient
{
    /// <summary>
    /// NOTE: Due to the limitations that the FtpWebRequest and FtpWebResponse can't be simulated using the existing Mock, NMock, Rhaino Mock etc. 
    /// technologies, the unitests for FTPClien are tightly depend upon the insrastructures.  The integration tests have been done using
    /// the ftp service configured at McAfee lab enginering department, Henry Li's test machine.  The testing ought to be skipped if 
    /// the infrastructure is not available.
    /// 
    /// To skip the test just simple un-commend out the compliler switch from the first line of this source file.  
    /// If you have any question please contact Ying_Li@mcafee.com
    /// </summary>
#if !SkipTest
    [TestFixture]
    public class FtpClientIntegrationTest 
    {
        private const string RootFolderName = @"BenchMarkTest";
        private const string FtpUri = @"ftp://beaylidt2/BenchMarkTest";
        private const string uriConfigKey = @"FTPClientIntegrationTestUri";
        const string TempsubFolderName = @"Temp";
        private string _fileName = @"SystemTrayIcon.png";
        private object _ftpuris = ConfigurationManager.GetSection(@"FtpUris");
        private UriBuilder _ftpUriConfigDetails;
        private IFTPFile _ftpFile;
        private IFTPFolder _ftpFolder;
        private static IUnityContainer _ftpFileContainer = null;
        private static IUnityContainer _ftpFolderContainer = null;

        public FtpClientIntegrationTest()
        {
            _ftpUriConfigDetails = new UriBuilder(ConfigurationManager.AppSettings[uriConfigKey]);

            _ftpFileContainer = FTPFile.UnityContainerFactory();
            _ftpFolderContainer = FTPFolder.UnityContainerFactory();
        }

        private string FtpFileUri { get { return string.Format(@"{0}/{1}", _ftpUriConfigDetails.Uri.AbsoluteUri, _fileName); } }

        [SetUp]
        protected void Setup()
        {
            _ftpFile = _ftpFileContainer.Resolve<IFTPFile>();
            _ftpFile.Initialize(FtpFileUri);

            _ftpFolder = _ftpFolderContainer.Resolve<IFTPFolder>();
            _ftpFolder.Initialize(_ftpUriConfigDetails.Uri.AbsoluteUri);
        }

        [Test]
        public void CompareTest()
        {
            var tempUri1 = string.Format(@"{0}/{1}_1", _ftpUriConfigDetails.Uri.AbsoluteUri, TempsubFolderName);
            var tempUri2 = string.Format(@"{0}/{1}_2", _ftpUriConfigDetails.Uri.AbsoluteUri, TempsubFolderName);
            var ftpFile1 = new FTPFile();
            var ftpFile2 = new FTPFile();
            ftpFile1.Initialize(tempUri1);
            ftpFile2.Initialize(tempUri1);
            Assert.AreEqual(1, ftpFile1.CompareTo(ftpFile2));
            ftpFile2.Initialize(tempUri2);
            Assert.AreEqual(0, ftpFile1.CompareTo(ftpFile2));
            var ftpTFolder1 = new FTPFolder();
            var ftpTFolder2 = new FTPFolder();
            ftpTFolder1.Initialize(tempUri1);
            ftpTFolder2.Initialize(tempUri1);
            Assert.AreEqual(1, ftpTFolder1.CompareTo(ftpTFolder2));
            ftpTFolder2.Initialize(tempUri2);
            Assert.AreEqual(0, ftpTFolder1.CompareTo(ftpTFolder2));
        }

        //FTPTree Test

        [Test]
        public void FTPTreeCreationTest()
        {
            var ftpTreeRoot = new FTPTreeNode(FtpUri, @"ftpuser", @"mcafee!624", @"D:\Temp\FtpDownload2") { TimeOut = 5 * 1000 };//time out in 5 seconds
            try
            {
                ftpTreeRoot.Initialize(FTPClient.ComposeInitialUriString(ftpTreeRoot.FtpUri, ftpTreeRoot.UserName, ftpTreeRoot.PassWord));
            }
            catch { }
        }

        #region ------------ Test cases for FTPFile -------------------
        
        [Test]
        public void FTPFileConstructors()
        {
            var ftpFile1 = new FTPFile(_ftpUriConfigDetails.Uri.AbsoluteUri, _ftpUriConfigDetails.UserName, _ftpUriConfigDetails.Password);
            Assert.IsTrue(!string.IsNullOrEmpty(ftpFile1.GetParentDirectory()));
            Assert.IsTrue(ftpFile1 != null);

            var ftpFile2 = new FTPFile(_ftpUriConfigDetails.Uri.AbsoluteUri, _ftpUriConfigDetails.UserName, _ftpUriConfigDetails.Password);
            Assert.IsTrue(!string.IsNullOrEmpty(ftpFile2.GetParentDirectory()));
            Assert.IsTrue(ftpFile2 != null);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void InvalidUri()
        {
            var ftpFile = new FTPFile(@"ftp:/");
        }


        [Test, ExpectedException(typeof(ArgumentException))]
        public void FilenameContainsWhitespace()
        {
            var ftpFile = new FTPFile(string.Format(@"{0}/Copy of Test.xml", _ftpUriConfigDetails.Uri.AbsoluteUri));
            ftpFile.PassiveMode = true;
            Assert.IsTrue(ftpFile.PassiveMode);
        }

        [Test]
        public void CheckNotExistsFile()
        {
            var ftpFile = new FTPFile(string.Format(@"{0}/Foo.txt", _ftpUriConfigDetails.Uri.AbsoluteUri));
            Assert.IsFalse(ftpFile.Exists);
        }

        [Test]
        public void FileName()
        {
            Assert.IsTrue(!string.IsNullOrEmpty(_ftpFile.FileName));
            Assert.IsFalse(string.IsNullOrEmpty(_ftpFile.LastErrorMessage));
        }
        
        [Test]
        public void DownloadFileBytes()
        {
            _ftpFile.UploadFile(new byte[] { 1, 2, 3, 4, 5 });
            var bytes = _ftpFile.DownloadFile(retry: 2);
            Assert.IsTrue(bytes.Length == 5);
        }

        [Test, ExpectedException(typeof(WebException))]
        public void DownloadFileBytesShouldThrowException()
        {
            _ftpFile = _ftpFileContainer.Resolve<IFTPFile>();
            _ftpFile.Initialize(_ftpUriConfigDetails.Uri.AbsoluteUri);
            _ftpFile.IsBubbleUpException = true;
            var bytes = _ftpFile.DownloadFile();
        }

        [Test]
        public void DownloadFile()
        {
            UploadFile();

            var localpath = Path.Combine(Environment.CurrentDirectory, _fileName);
            _ftpFile.TimeOut = 100000;
            var success = _ftpFile.DownloadFile(localpath, retry: 3);
            Assert.IsTrue(success);
        }

        [Test]
        public void ChecFileNeverExists()
        {
            _ftpFile.Initialize(@"ftp://fakesite/junkfile");
            Assert.IsFalse(_ftpFile.Exists);
        }

        [Test, ExpectedException(typeof(WebException))]
        public void GetFileSizeNeverExists()
        {
            _ftpFile.Initialize(@"ftp://fakesite/junkfile");
            _ftpFile.IsBubbleUpException = true;
            var size = _ftpFile.GetFileSize();
        }

        [Test, ExpectedException(typeof(WebException))]
        public void GetFileSizeWithInvalidFileName()
        {
            _ftpFile.Initialize(@"ftp://fakesite");
            _ftpFile.IsBubbleUpException = true;
            var size = _ftpFile.GetFileSize();
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void UploadWithNullBytes()
        {
            byte[] bytes = null;
            _ftpFile.UploadFile(bytes);
        }

        [Test]
        public void UploadFile()
        {
            _ftpFile.ChunkSize = 32 * 1024;
            Assert.IsTrue(_ftpFile.UploadFile(string.Format(@"D:\Document\FTPBenchmark\{0}", _fileName)));
        }

        [Test]
        public void UploadFileWithDifferentName()
        {
            _ftpFile.ChunkSize = 32000 * 1024;
            Assert.IsTrue(_ftpFile.UploadFile(string.Format(@"D:\Document\FTPBenchmark\{0}_Copy", _fileName)));
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void UploadFileShouldRaiseException()
        {
            _ftpFile.ChunkSize = 0;
            _ftpFile.UploadFile(@"D:\fake.zip");
        }

        [Test]
        public void UploadStream()
        {
            var source = new byte[] { 1, 2, 3, 4, 5 };
            var stream = new MemoryStream(source);
            _ftpFile.ChunkSize = 2;
            stream.Seek(2, SeekOrigin.Current);
            Assert.IsTrue(_ftpFile.UploadStream(stream));
            var results = _ftpFile.DownloadFile();
            Assert.AreEqual(source, results);
        }

        [Test]
        public void GetFileSize()
        {
            UploadFile();
            var size = _ftpFile.GetFileSize();
            Assert.IsTrue( size > 0);
        }

        [Test]
        public void DeleteFile()
        {
            Setup();
            Assert.IsTrue(_ftpFile.DeleteFile());          
        }

        [Test]
        public void DeleteFilNotExists()
        {
            var ftpFile = new FTPFile(string.Format(@"{0}/Foo.txt", _ftpUriConfigDetails.Uri.AbsoluteUri));
            Assert.IsFalse(ftpFile.DeleteFile());
        }

        [Test]
        public void CheckFolderNeverCreated()
        {
            Setup();
            _ftpFolder.Initialize(@"ftp://fakesite/Foo");
            Assert.IsFalse(_ftpFolder.Exists);
        }

        #endregion

        #region ------------ Test cases for FTPFolder -------------------

        [Test]
        public void FTPFolderConstructors()
        {
            var FTPFolder1 = new FTPFolder(_ftpUriConfigDetails.Uri.AbsoluteUri, _ftpUriConfigDetails.UserName, _ftpUriConfigDetails.Password);
            Assert.IsTrue(!string.IsNullOrEmpty(FTPFolder1.GetParentDirectory()));
            Assert.IsTrue(FTPFolder1 != null);

            var FTPFolder2 = new FTPFolder(_ftpUriConfigDetails.Uri.AbsoluteUri, _ftpUriConfigDetails.UserName, _ftpUriConfigDetails.Password);
            Assert.IsTrue(!string.IsNullOrEmpty(FTPFolder2.GetParentDirectory()));
            Assert.IsTrue(FTPFolder2 != null);
        }

        [Test]
        public void ListDirectory()
        {
            var folderName = string.Empty;
            var folderList = new List<string>();
            var fileList = new List<string>();
            var success = _ftpFolder.ListDirectory(out folderList, out fileList, folderName);
            Assert.IsTrue(success);
        }

        [Test]
        public void ListDirectoryNotExists()
        {
            Setup();
            var folderName = string.Empty;
            var folderList = new List<string>();
            var fileList = new List<string>();
            var ftpFolder = new FTPFolder(string.Format(@"{0}/Crasy", _ftpFolder.FtpUri));
            ftpFolder.IsBubbleUpException = true;
            ftpFolder.ListDirectory(out folderList, out fileList, folderName);
            Assert.AreEqual(0, folderList.Count);
            Assert.AreEqual(0, fileList.Count);
        }

        [Test]
        public void FolderName()
        {
            Assert.IsTrue(!string.IsNullOrEmpty(_ftpFolder.FolderName));
        }

        [Test]
        public void CheckExistsFolder()
        {
            var ftpFolder = new FTPFolder(string.Format(@"{0}/Foo", _ftpUriConfigDetails.Uri.AbsoluteUri));
            Assert.IsTrue(ftpFolder.Exists);
        }

        [Test]
        public void CheckNotExistsFolder()
        {
            var ftpFolder = new FTPFolder(string.Format(@"{0}/Engine", _ftpUriConfigDetails.Uri.AbsoluteUri));
            Assert.IsFalse(ftpFolder.Exists);
        }

        [Test]
        public void CreateSubFolder()
        {
            Setup();
            _ftpFolder.TimeOut = 10000;
            Assert.IsTrue(_ftpFolder.CreateSubFolder(TempsubFolderName));

            DeleteTempFolder();

            var tempUri = string.Format(@"{0}/{1}/{2}", _ftpFolder.FtpUri, TempsubFolderName, @"foo1.txt");
            var ftpFile = _ftpFileContainer.Resolve<IFTPFile>();
            ftpFile.Initialize(tempUri);
            Assert.IsTrue(ftpFile.UploadFile(new byte[] { 1, 2, 3, 4, 5 }));
        }

        [Test]
        [ExpectedException(typeof(WebException))]
        public void DeleteSubFolderShouldThrowException()
        {
            Setup();
            CreateAfolderShouldFail(true);
        }

        [Test]
        public void DeleteSubFolderShouldNotThrowException()
        {
            Setup();
            Assert.IsFalse(CreateAfolderShouldFail(false));
        }

        private bool CreateAfolderShouldFail(bool throwexception)
        {
            var tempUri = string.Format(@"{0}/{1}", _ftpUriConfigDetails.Uri.AbsoluteUri, TempsubFolderName);
            var ftpTempFolder = _ftpFolderContainer.Resolve<IFTPFolder>();
            ftpTempFolder.Initialize(tempUri);

            var barFolderName = @"bar";
            ftpTempFolder.IsBubbleUpException = throwexception;
            Assert.AreEqual(ftpTempFolder.IsBubbleUpException, throwexception);
            return ftpTempFolder.CreateSubFolder(barFolderName);
        }

        [Test]
        public void DeleteSubFolderNotExists()
        {
            Setup();
            var tempUri = string.Format(@"{0}/{1}", _ftpUriConfigDetails.Uri.AbsoluteUri, TempsubFolderName);
            var ftpTempFolder = _ftpFolderContainer.Resolve<IFTPFolder>();
            ftpTempFolder.Initialize(tempUri);
            var farFolderName = @"Far";
            Assert.IsTrue(ftpTempFolder.DeleteSubFolder(farFolderName));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateSubFolderShouldReturnError()
        {
            var subFolder = string.Empty;
            _ftpFolder.CreateSubFolder(subFolder);
        }

        private void DeleteTempFolder()
        {
            Setup();
            var success = _ftpFolder.DeleteSubFolder(TempsubFolderName);
            Assert.IsTrue(success);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void DeleteSubFolderShouldReturnError()
        {
            var subFolder = string.Empty;
            _ftpFolder.DeleteSubFolder(subFolder);
        }
        
        #endregion
    }
#endif
}
