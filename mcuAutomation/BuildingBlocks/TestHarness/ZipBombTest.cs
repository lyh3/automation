using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;
using Xceed.Zip;

namespace McAfeeLabs.Engineering.Automation.Base.Common
{
    [TestFixture]
    public class ZipBombTest : TestHarness
    {
        private ZipBomb _zipbomb;

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(TestFilepath))
                File.Delete(TestFilepath);
            if (null != _zipbomb)
                _zipbomb.Dispose();
        }

        [Test]
        public void MultiThreadMimic()
        {
            TestFileName = TestZipBombFileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            var tasklist = new List<Task>();
            var sourcefiles = new List<string>();

            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = 10 }, () =>
            {
                tasklist.Add(Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < 2; ++i)
                    {
                        var sourcefileName = string.Format(@"{0}_{1}.zip", Path.GetFullPath(TestFilepath).Replace(@".zip", string.Empty), i);
                        File.Copy(TestFilepath, sourcefileName, true);
                        sourcefiles.Add(sourcefileName);
                        using (var zipbomb = new ZipBomb(TestFilepath,
                                                  3,
                                                  GlobalDefinitions.DefaultInfectedPassword))
                        {
                            zipbomb.Inspect();
                            Assert.IsTrue(zipbomb.IsZipFile);
                            Assert.IsTrue(zipbomb.IsZipBomb);
                            zipbomb.CanDispose = true;
                        };
                    }
                }
                ));
            });
            Task.WaitAll(tasklist.ToArray(), new TimeSpan(0, 0, 100));

            sourcefiles.ForEach(x =>
            {
                if (File.Exists(x))
                    File.Delete(x);
            });

        }

        [Test]
        public void FileIsZipBomb()
        {
            TestFileName = TestZipBombFileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            using(_zipbomb = new ZipBomb( TestFilepath,
                                          3,
                                          GlobalDefinitions.DefaultInfectedPassword))
            {
                _zipbomb.Inspect();
                _zipbomb.CanDispose = true;
            };
            Assert.IsTrue(_zipbomb.IsZipFile);
            Assert.IsTrue(_zipbomb.IsZipBomb);
        }

        [Test]
        public void SizeSetTooSmalTheNormaZipFileShowAsZipbomb()
        {
            TestFileName = TestZipFileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            using(_zipbomb = new ZipBomb(TestFilepath,
                                    5,
                                    GlobalDefinitions.DefaultInfectedPassword, 
                                    100000))
            {
                _zipbomb.Inspect();
                _zipbomb.CanDispose = true;
            };
            Assert.IsTrue(_zipbomb.IsZipFile);
            Assert.IsTrue(_zipbomb.IsZipBomb);
            Assert.IsTrue(_zipbomb.IsUnpackedSizeTooLarge);
        }

        [Test]
        public void FileIsNormalZipFile()
        {
            TestFileName = TestZipFileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            using (_zipbomb = new ZipBomb(TestFilepath, 7))
            {
                _zipbomb.Inspect();
                _zipbomb.CanDispose = true;
            };
            Assert.IsTrue(_zipbomb.IsZipFile);
            Assert.IsFalse(_zipbomb.IsZipBomb);
        }

        [Test]
        public void UnpackedSizeTooLarge()
        {
            TestFileName = TestZipFileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            using(_zipbomb = new ZipBomb(TestFilepath,
                                    5,
                                    null,
                                    400000))
            {
                _zipbomb.Inspect();
                _zipbomb.CanDispose = true;
            };
            Assert.IsTrue(_zipbomb.IsZipFile);
            Assert.IsTrue(_zipbomb.IsUnpackedSizeTooLarge);
        }

        [Test]
        public void UnpackedSizeAcceptable()
        {
            TestFileName = TestZipBombFileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            using(_zipbomb = new ZipBomb( TestFilepath,
                                    5, 
                                    null, 
                                    600000))
            {
                _zipbomb.Inspect();
                _zipbomb.CanDispose = true;
            };
            Assert.IsTrue(_zipbomb.IsZipFile);
            Assert.IsFalse(_zipbomb.IsUnpackedSizeTooLarge);
        }

        [Test, ExpectedException(typeof(ApplicationException))]
        public void FileIsZipBombPassWrongPasswordShouldThrowException()
        {
            TestFileName = TestZipBombFileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            using(_zipbomb = new ZipBomb(TestFilepath, 5, "---", null, true))
            {
                _zipbomb.ThrowException = true;
                _zipbomb.Inspect();
                _zipbomb.CanDispose = true;
            };
            Assert.IsTrue(_zipbomb.IsZipFile);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void SetThreasholdTooSmallShouldThrowException()
        {
            TestFileName = TestZipBombFileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            using(_zipbomb = new ZipBomb(TestFilepath, 1))
            {
                _zipbomb.ThrowException = true;
                _zipbomb.Inspect();
                _zipbomb.CanDispose = true;
            };
            Assert.IsTrue(_zipbomb.IsZipFile);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void SetThreasholdTooLargeShouldThrowException()
        {
            TestFileName = TestZipBombFileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath);
            using(_zipbomb = new ZipBomb(TestFilepath, 2001))
            {
                _zipbomb.ThrowException = true;
                _zipbomb.Inspect();
                _zipbomb.CanDispose = true;
            };
            Assert.IsTrue(_zipbomb.IsZipFile);
        }

        [Test]
        public void XmlFileIsNotZipFile()
        {
            TestFileName = _testXmlFileName;
            TestFileName.CreateFileFromEmbeddedStream(TestFilepath, EmbeddedTestFilePath); 
            using(_zipbomb = new ZipBomb(TestFilepath, 5))
            {
                _zipbomb.Inspect();
                _zipbomb.CanDispose = true;
            };
            Assert.IsFalse(_zipbomb.IsZipFile);
            Assert.IsFalse(_zipbomb.IsZipBomb);
        }
    }
}
