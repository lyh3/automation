using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity.Configuration;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Base
{
    [TestFixture]
    public class SMSDownloadExtensionTest : TestHarness
    {
        [Test]
        public void MD5DownloadTest()
        {

            for (int i = 0; i < 10; ++i)
            {
                string md5 = "03c51628bba3bd8ecb969eb2ec4a812b";// "123d7534e3aabc6b7f37dd4c6345d139";
                var folderName = string.Format(@"D:\Data\Testing\BulkScannerTest\SMSDownload\{0}", i + 1);
                if (System.IO.Directory.Exists(folderName))
                    Directory.Delete(folderName, true);
                Directory.CreateDirectory(folderName);
                var errormessage = new System.Text.StringBuilder();
                md5.SMSDownload(folderName, string.Empty, errormessage);
                System.Threading.Thread.Sleep(2000);
            }

        }
    }
}
