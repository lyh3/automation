using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Base.Extension
{
    [TestFixture]
    public class ExtensionTest : TestHarness
    {
        [SetUp]
        public void Initial()
        {
            TestFileName = _testFileName;
        }

        [TearDown]
        public void Cleanup()
        {
            Dispose();
        }

        [Test]
        public void GetFileContenBytes()
        {
            Buffer = new byte[] { 1, 2, 3, 4, 5 };
            CreateTestFile();

            var resutls = TestFilepath.GetFileContenBytes(chunksize : 64 * 1024);
            Assert.IsTrue(resutls.Count == 1);
        }

        [Test]
        public void GetFileContenBytesGreaterThanChuncksize()
        {
            CreateBinaryTestFileFromEmbeddedStream();

            var resutls = TestFilepath.GetFileContenBytes();
            Assert.IsTrue(resutls.Count > 1);
        }

        [Test]
        public void GetFileFullPath()
        {
            Buffer = new byte[] { 1, 2, 3, 4, 5 };
            CreateTestFile();

            Assert.IsTrue(_testFileName.GetFileFullPath() == TestFilepath);
        }

        [Test]
        public void PageQuery()
        {
            var path = CommonUtility.GetExecutedPath();
            var dir = new DirectoryInfo(path);
            var results = new List<FileInfo>();
            var source = new List<string>();
            source.Add(dir.Parent.FullName);

            var files = CommonUtility.InspectDirectories( source,
                                                    true,
                                                    results).Select(t => t.FullName).ToList();

            Assert.IsNotNull(files.Page(1, 2));
        }

        [Test]
        public void GetFileContent()
        {
            var filePath = @"C:\Windows\System32\notepad.exe";
            var content = filePath.GetFileContenBytes();
            Assert.IsTrue(null != content && content.ToArray().Length > 0);
        }

        [Test]
        public void GetFileContentSizeLessThanDefault()
        {
            var filePath = @"C:\Windows\System32\hid.dll";
            var content = filePath.GetFileContenBytes();
            Assert.IsTrue(null != content && content.ToArray().Length > 0);
        }
    }
}
