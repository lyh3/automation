using System;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Text.RegularExpressions;
using log4net;
using Newtonsoft.Json;
using WindowService.DataModel;
using Automation.Base.BuildingBlocks;

namespace McuReleaseSubmitApp
{
    class Program
    {
        private static bool _isRunnint = true;
        static void Main(string[] args)
        {
            Console.WriteLine(@"....... Start McuReleaseSubmitApp, press Ctl-C to exit ......");
            var dropbox = Path.Combine(ConfigurationManager.AppSettings["AutomationPath"], @"ReleaseDropBox");
            for (var i = 0; i < 3; i++)
            {
                if (Directory.Exists(dropbox))
                {
                    break;
                }
                try
                {
                    Directory.CreateDirectory(dropbox);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(string.Format(@"Exception caught at create dropbox error = {0}, retry = {1}", ex.Message, i));
                }
            }
            if (!Directory.Exists(dropbox))
            {
                throw new IOException("Failed to create ReleaseDropBox.");
            }
            while (_isRunnint)
            {
                Console.CancelKeyPress += Console_CancelKeyPress;
                WorkflowConfigModel model = null;
                var json = string.Empty;
                var jsonConfigPath = ConfigurationManager.AppSettings["ConfigJsonPath"];
                if (jsonConfigPath.IsFileLocked())
                {
                    continue;
                }
                using (StreamReader file = File.OpenText(jsonConfigPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    model = (WorkflowConfigModel)serializer.Deserialize(file, typeof(WorkflowConfigModel));
                }
                using (StreamReader file = File.OpenText(jsonConfigPath))
                {
                    json = file.ReadToEnd();
                }
                var regex = new Regex(@"^(McuReleaseRequest_(HSD|Local)_([a-z0-9]{24}).json)$");
                foreach (var f in Directory.GetFiles(dropbox))
                {
                    var fileName = Path.GetFileName(f);
                    if (regex.Match(fileName).Success)
                    {
                        var split = fileName.Split('_');
                        var releaseSource = split[1];
                        var sessionId = split[2].Replace(@".json", string.Empty);
                        var fRequest = Path.Combine(ConfigurationManager.AppSettings["AutomationPath"], fileName);
                        var invokeParameterFormat = @"-j {0} -r {1} --worksheet {2} --source {3}";
                        if (!File.Exists(fRequest))
                        {
                            File.Move(f, fRequest);
                        }
                        var jsonpath = ConfigurationManager.AppSettings["ConfigJsonPath"].Replace("MicrocodeRelease", string.Format("MicrocodeRelease_{0}", sessionId));
                        var workspace = string.Format("MCUWorkspace_{0}", sessionId);
                        var worksheet = fRequest.Replace("Request", string.Empty);
                        json = json.Replace("MCUWorkspace", workspace);
                        json = json.Replace("McuReleaseAutomation", string.Format(@"McuReleaseAutomation_{0}", sessionId));
                        File.WriteAllText(jsonpath, json);
                        var invokeParameters = string.Format(invokeParameterFormat,
                                                             jsonpath,
                                                             fRequest,
                                                             worksheet,
                                                             releaseSource);
                        var mcuReleaseThread = new McuReleaseSubmitWorkerThread(LogManager.GetLogger("Default"))
                        {
                            AutomationDir = ConfigurationManager.AppSettings["AutomationPath"],
                            Request = fRequest,
                            Workspace = workspace,
                            InvokeParameters = invokeParameters,
                            ReleaseSource = releaseSource
                        };
                        mcuReleaseThread.Start();
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _isRunnint = false;
        }
    }
}
