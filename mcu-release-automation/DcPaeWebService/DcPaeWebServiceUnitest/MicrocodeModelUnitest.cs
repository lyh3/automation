using System;
using NUnit.Framework;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using WindowService.DataModel;


namespace DcPaeWebServiceUnitest
{
    [TestFixture]
    public class MicrocodeModelUnitest
    {
        const string REQUEST_JSON_FILE = @"C:\mcu-release-automation\DcPaeWebService\IntelDCGSpsWebService\App_Data\McuReleaseRequestTemplate.json";
        const string CONFIG_JSON_FILE = @"C:\mcu-release-automation\ReleaseAutomation\McuRelease.json";
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
                var workingdirectory = @"C:\mcu-release-automation\ReleaseAutomation";
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
        [Test]
        public void ReadmeDocumentTest()
        {
            var workingdirectory = @"C:\mcu-release-automation\ReleaseAutomation\README_Template.md";
            var doc = new ReadmeDocument(workingdirectory);
            var cpuid = "406f1";//.PadLeft(8, '0');
            var agreegation = doc.EntriesByCpuIdAndCpuSegment(cpuid, CpuSegment.Server.ToString());
            var s = agreegation.ToString();
            var cpuCodeNames = agreegation[McuTableColumn.CpuCodeName.ToString()];
        }
    }
}
