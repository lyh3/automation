using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;
using SevenZip;

namespace McAfeeLabs.Engineering.Automation.Base
{
    [TestFixture]
    public class SevenZipSharpWrapperTest : TestHarness
    {
        const string SourceZipfileName = @"D:\Data\PasswordInfected.zip";// @"D:\Download\en_office_communicator_2005.iso";// @"D:\Data\MetaDataMerge\MetaDataMerge.zip";//
        const string PasswordProtectedSourceZipfileName = @"D:\Data\PasswordInfected.zip";
        private string _distinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SevenZipSharpResults");

        [TearDown]
        public void DisposeExtractedFiles()
        {
            if (Directory.Exists(_distinationPath))
                Directory.Delete(_distinationPath, true);
        }

        [Test]
        public void ArchiveFormatTest()
        {
            string [] sourceFiles = { @"D:\Data\MetaDataMerge\MetaDataMerge.zip",
                                      @"D:\Data\MigrationTools-2_2_TO_2_2_5.exe",
                                      @"D:\Download\AnkhSvn-2.1.10129.msi",
                                      @"D:\Download\en_office_communicator_2005.iso",
                                      @"D:\Download\MemProfilerInstaller4_0_119.exe",
                                    };
            InArchiveFormat[] ExpectedType = { InArchiveFormat.Zip,
                                               InArchiveFormat.SevenZip,
                                               InArchiveFormat.Cab,
                                               InArchiveFormat.Iso,
                                               InArchiveFormat.PE
                                            };
            for (int i = 0; i < sourceFiles.Length; ++i)
            {
                var sevenZipWrapper = new SevenZipSharpWrapper(sourceFiles[i]);
                Assert.AreEqual(sevenZipWrapper.InputArchiveFormat, ExpectedType[i]);
            }
        }

        [Test]
        public void ExtractPasswordProtectedZipFileTest()
        {
            var sevenZipWrapper = new SevenZipSharpWrapper(PasswordProtectedSourceZipfileName, GlobalDefinitions.DefaultInfectedPassword);

            sevenZipWrapper.ExtractArchiveTo(_distinationPath);
            var contents = Directory.GetFiles(_distinationPath);
            Assert.IsTrue(null != contents && contents.Length > 0);
        }

        [Test, ExpectedException(typeof(ExtractionFailedException))]
        public void ExtractPasswordProtectedZipFileShouldThrowException()
        {
            var sevenZipWrapper = new SevenZipSharpWrapper(PasswordProtectedSourceZipfileName) { ThrowException = true };

            sevenZipWrapper.ExtractArchiveTo(_distinationPath);
        }

        [Test, ExpectedException(typeof(ExtractionFailedException))]
        public void ExtractPasswordProtectedZipFilePasswordMismatchShouldThrowException()
        {
            var sevenZipWrapper = new SevenZipSharpWrapper(PasswordProtectedSourceZipfileName, @"clean") { ThrowException = true };

            sevenZipWrapper.ExtractArchiveTo(_distinationPath);
        }
    }
}