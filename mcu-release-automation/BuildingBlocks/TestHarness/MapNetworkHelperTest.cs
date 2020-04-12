using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

using NUnit.Framework;
using McAfeeLabs.Engineering.Automation.Base;

namespace McAfeeLabs.Engineering.Automation.Base.Extension
{
    [TestFixture]
    public class MapNetworkHelperTest : TestHarness
    {
        private const string RemotePath = @"\\10.40.128.131\Submissions\NSP\SamplesIn";
        private const string helloFileName = @"Hello.txt";
        private string _networkPath = RemotePath;
        private string _helloFileName;
        private MapNetworkHelper _mapNetworkHelper;

        [SetUp]
        public void InitialHelperTest()
        {
            _testXmlFileName = @"SampleSubmissionDropBox.config";
            _testFileName = @"MapNetworkHelperTest.config";
            base.Initial();
            CreateXmlTestFileFromEmbeddedStream();

            var error = _networkPath.ConnectToRemote(@"raiden", @"Ra1den@mcafee");
            _mapNetworkHelper = new MapNetworkHelper();
        }

        [TearDown]
        public void DisposeHelperTest()
        {
            if (!string.IsNullOrEmpty(TestFilepath) && File.Exists(TestFilepath)) 
                File.Delete(TestFilepath);

            _networkPath.DisconnectRemote();
            if (File.Exists(_helloFileName))
                File.Delete(_helloFileName);

            _mapNetworkHelper.DisConnectRemoteSource();
        }

        [Test]
        public void DirInfoTest()
        {
            var dirInfo = new DirectoryInfo(_networkPath);
            Assert.IsNotNull(dirInfo);
        }

        [Test]
        public void CreateFileTest()
        {
            _helloFileName = Path.Combine(_networkPath, helloFileName);
            CreateHelloFile(_helloFileName);
            var fileList = new List<string>();
            fileList.AddRange(Directory.GetFiles(_networkPath));

            Assert.IsNotNull(fileList.FirstOrDefault(x => x == _helloFileName));
        }

        [Test]
        public void HelperTest()
        {
            if (null == _assembly) InitialHelperTest();

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(TestFilepath);
            _mapNetworkHelper.ConnectRemoteSource(xmlDoc, new [] {"SamplesIn", "HashesIn", "ErrorOut" });

            _helloFileName = Path.Combine(RemotePath, helloFileName);
            CreateHelloFile(_helloFileName);
            var fileList = new List<string>();
            fileList.AddRange(Directory.GetFiles(RemotePath));

            Assert.IsNotNull(fileList.FirstOrDefault(x => x == _helloFileName));
        }

        private void CreateHelloFile(string path)
        {
            using (FileStream filestream = File.Create(path))
            {
                filestream.Close();
            }
        }
    }
}
