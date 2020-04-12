using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using log4net;
using Automation.WorkerThreadModel;

namespace WindowService.DataModel
{
    public class InvokeWorkflowState : McuSubmitState
    {
        public InvokeWorkflowState(WorkerThread parent, ILog logger):base(parent, logger)
        {
        }

        protected override void DoWork()
        {
            try
            {
                McuReleaseSubmitWorkerThread parent = (McuReleaseSubmitWorkerThread)ParentThread;
                var workingdirectory = parent.AutomationDir;
                var startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = false;
                startInfo.FileName = "python";
                var backendPythonApp = @"McuReleaseAutomation.py";
                var argus = string.Format(@"{0} {1}", backendPythonApp, parent.InvokeParameters);
                startInfo.Arguments = argus;
                startInfo.Verb = "runas";
                Directory.SetCurrentDirectory(workingdirectory);
                var process = new Process() { StartInfo = startInfo };
                process.Start();
                if (null != process && !process.HasExited)
                {
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(@"Exception caught at InvokeWorkflow, error = {0}", ex.Message);
                _success = false;
            }
        }
    }
}
