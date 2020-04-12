using System.Collections.Generic;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Base.Common
{
    [TestFixture]
    public class ZipWalkerTest : TestHarness, IZipWalkerVisitor
    {
        const string SourceZipfileName = @"MetaDataMerge.zip";//@"D:\Data\zip_bomb.zip";//@"D:\Data\MetaDataMerge\MetaDataMerge.zip";//@"D:\Data\RSA_SETUP_ZIP\RSA_9.0_EVL_Setup.zip"; //
        //To test use custom data instead of data from embedded resource, replace the [TestFilepath] value with full path from a specifice test case.
        [Test]
        public void RecurseUnZipTest()
        {
            TestFileName = SourceZipfileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            using (var zipWalker = new ZipWalker(this,
                                                  new[] { TestFilepath },
                                                  2))
            {
                zipWalker.Inspect();
                Assert.AreEqual(zipWalker.CurrentDeepLevel, 1);
                zipWalker.CanDispose = false;//keep the results
            }
        }

        [Test]
        public void RecurseUnZipMultipeArchiveTypesTest()
        {
            TestFileName = TestZipFileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            using (var zipWalker = new ZipWalker(this,
                                                  new[] { TestFilepath },
                                                  2))
            {
                zipWalker.Inspect();
                Assert.AreEqual(zipWalker.CurrentDeepLevel, 1);
                zipWalker.CanDispose = true;
            }
        }

        [Test]
        public void RecurseZipWalkTest()
        {
            TestFileName = SourceZipfileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            using (var zipWalker = new ZipWalker(this,
                                                  new[] { TestFilepath },
                                                  2))
            {
                zipWalker.Inspect();
                Assert.AreEqual(zipWalker.CurrentDeepLevel, 1);
                zipWalker.CanDispose = true;//clear the results
            }

        }

        public void Visit(List<string> files)
        {
        }
    }
}
