using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using IntelDCGSpsWebService.Models;


namespace DcPaeWebServiceUnitest
{
    [TestFixture]
    public class MicrocodeModelUnitest
    {
        const string REQUEST_JSON_FILE = @"D:\Projects\DcPaeWebService\IntelDCGSpsWebService\App_Data\McuReleaseRequestTemplate.json";
        const string CONFIG_JSON_FILE = @"D:\Projects\ReleaseAutomation\McuRelease.json";
        [Test]
        public void ReleaseRequestSerializationTest()
        {
            var mcuInfo = new McuSelectInfo
            {
                IsChecked = true,
                Id = "Mcu|id_0"
            };
            var s = mcuInfo.ToString();
            JsonSerializer ser = new JsonSerializer();
            var reader = new JsonTextReader(new StringReader(s));
            var mcu = ser.Deserialize<McuSelectInfo>(reader);


            using (StreamReader file = File.OpenText(REQUEST_JSON_FILE))
            {
                JsonSerializer serializer = new JsonSerializer();
                var m = (MicrocodeReleaseModel)serializer.Deserialize(file, typeof(MicrocodeReleaseModel));
                Assert.True(null != m);
                m.Title = "Microcode Release";
                var json = m.ToString();
                File.WriteAllText(@"D:\Temp\McuReleaseRequest.json", json);
            }
        }
        [Test]
        public void ConfigJsonSerializationTest()
        {
            using (StreamReader file = File.OpenText(CONFIG_JSON_FILE))
            {
                JsonSerializer serializer = new JsonSerializer();
                var m = (WorkflowConfigModel)serializer.Deserialize(file, typeof(WorkflowConfigModel));
                Assert.True(null != m);
            }
        }
        [Test]
        public void ReleaseLaunchTest()
        {
            try
            {
                var workingdirectory = @"D:\Projects\McuRelease\Dev\ReleaseAutomation";
                var startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = false;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.FileName = "python";
                var argus = string.Format(@"McuReleaseAutomation.py {0}", @" --repo testing");
                startInfo.Arguments = argus;
                startInfo.Verb = "runas";
                Directory.SetCurrentDirectory(workingdirectory);
                var process = new Process() { StartInfo = startInfo };
                process.Start();
            }
            catch(Exception)
            {

            }
        }
    }
}
