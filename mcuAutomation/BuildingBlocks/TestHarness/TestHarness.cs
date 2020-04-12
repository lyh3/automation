using System;
using System.IO;
using System.Reflection;
using System.Xml;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Base
{
    abstract public class TestHarness
    {
        protected const string EmbeddedTestFilePath = @"McAfeeLabs.Engineering.Automation.Base.EmbeddedTestData";
        protected string _testFileName = @"TestBytes.txt";
        protected string _testXmlFileName = @"Test.xml";
        protected const string TestZipBombFileName = @"zip_bomb.zip";
        protected const string TestZipFileName = @"DebugView.zip";
        protected Assembly _assembly;
      
        protected byte[] Buffer { get; set; }
        protected string TestFileName { get; set; }
        protected string TestFilepath { get { return string.Format(@"{0}\{1}", CommonUtility.GetExecutedPath(), TestFileName); } }
        protected string EmbeddedXmlFilename { get { return string.Format(string.Format(@"{1}.{0}0{2}", '{', EmbeddedTestFilePath, '}'), _testXmlFileName); } }

        public TestHarness()
        {
            MapVirutalDriveHelper.Instance.ResetAllVirtualDrives();
        }

        [SetUp]
        public void Initial()
        {
            TestFileName = _testXmlFileName;
            _assembly = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestHarness.dll"));
        }

        [TearDown]
        public void Cleanup()
        {
            MapVirutalDriveHelper.Instance.ResetAllVirtualDrives();
        }

        protected void CreateTestFile()
        {
            using (var filestream = new FileStream(TestFilepath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                filestream.Write(Buffer, 0, Buffer.Length);
                filestream.Flush();
            }
        }
      
        protected void CreateBinaryTestFileFromEmbeddedStream()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(EmbeddedXmlFilename))
            {
                if (null != stream)
                {
                    var reader = new BinaryReader(stream);
                    Buffer = new byte[stream.Length];
                    reader.Read(Buffer, 0, (int)stream.Length);
                }
            }

            CreateTestFile();
        }

        protected void CreateXmlTestFileFromEmbeddedStream()
        {
            var sourceFileName = EmbeddedXmlFilename;
            var xmlDoc = new XmlDocument();
            sourceFileName.LoadEmbeddedXml(xmlDoc, _assembly);
            xmlDoc.Save(TestFilepath);
        }

        protected void Dispose()
        {
            if (File.Exists(TestFilepath))
                File.Delete(TestFilepath);
        }
    }
}
