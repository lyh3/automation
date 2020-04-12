using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Base.BuildingBlocks
{
    [TestFixture]
    public class LongNamePathTest
    {
        private string _longFileName;
        private string _shortFileName;
        private const int Length = 250;
        private const string TestPath = @"D:\bin\f1\FnvTWAwvPncmDNmkwRHjfFLmeQfoHxLZqgAdoXAhXVJRHYXYRZ\qXGtBZBiPguhNrCcWwniIhOs\cALIcRFhhrdGDJnnKejO\yaCFVRWzTGyjDpKpSPmq\UYTkIhEoUvUfJrVHTivt\FIIDWZBmXnWzkNNOytBEIoEma\AWKvYuuUwGBBmhomwirNZsimNQkyOe";

        [SetUp]
        public void CreateFiles()
        {
            var longPathString = Length.RandomValue(false, true);
            _longFileName = string.Format(@"{0}\{1}.txt", TestPath, longPathString);
            _longFileName = _longFileName.LongPathFormat();
            using (var filestream = Delimon.Win32.IO.File.Create(_longFileName))
            {
                byte[] buffer = UTF8Encoding.UTF8.GetBytes(longPathString);
                filestream.Write(buffer, 0, buffer.Length);
                filestream.Flush();
                filestream.Close();
            }
            _shortFileName = string.Format(@"{0}\Ready.txt", AppDomain.CurrentDomain.BaseDirectory);
            _shortFileName = _shortFileName.LongPathFormat();
            using (var filestream = Delimon.Win32.IO.File.Create(_shortFileName))
            {
                filestream.Close();
            }
        }

        [TearDown]
        public void Dispost()
        {
            if (Delimon.Win32.IO.File.Exists(_longFileName))
                Delimon.Win32.IO.File.Delete(_longFileName);
            if (Delimon.Win32.IO.File.Exists(_shortFileName))
                Delimon.Win32.IO.File.Delete(_shortFileName);
        }

        [Test]
        public void GetLongNameFileTest()
        {
            var files = new List<string>();
            files.AddRange(TestPath.GetFilesWithLongNames());
            Assert.IsTrue(null != files.FirstOrDefault(x => Delimon.Win32.IO.File.Exists(_longFileName)));
            Assert.IsTrue(Delimon.Win32.IO.File.Exists(_longFileName));
        }

        [Test]
        public void GetShortNameFileTest()
        {
            var files = new List<string>();
            var currentPath = AppDomain.CurrentDomain.BaseDirectory;
            files.AddRange(currentPath.GetFilesWithLongNames());
            Assert.IsTrue(null != files.FirstOrDefault(x => Delimon.Win32.IO.File.Exists(_shortFileName)));
            Assert.IsTrue(Delimon.Win32.IO.File.Exists(_shortFileName));
        }
    }
}
