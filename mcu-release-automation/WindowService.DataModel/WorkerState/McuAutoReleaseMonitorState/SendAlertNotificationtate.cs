using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Linq;

using log4net;
using Automation.WorkerThreadModel;
using Automation.Base.BuildingBlocks;

namespace WindowService.DataModel
{
    public class SendAlertNotificationtate : HsdReleaseMonitorState
    {
        public SendAlertNotificationtate(WorkerThread parent, ILog logger)
            : base(parent, logger)
        {
        }

        protected override void DoWork()
        {
            var parentThread = ParentThread as McuReleaseMonitorWorkerThread;
            try
            {
                var hsdMcuReleaseConfig = parentThread.HsdMcuReleaseJsonConfig;
                var emailservice = new EmailService
                {
                    Logger = _logger
                };
                SMTPEmailConfig emailParameters;
                foreach (Tuple<string, State> tpl in hsdMcuReleaseConfig.Processing.ResultsTuple)
                {
                    if (tpl.Item1 == @"send_notification")
                    {
                        var state = (SendEmailState)tpl.Item2;
                        var stateData = (SendNotificationStateData)state.stateData;
                        emailParameters = stateData.parameters[0];
                        if (emailservice.SendEmail(emailParameters.Server,
                                               emailParameters.From,
                                               string.Join("; ", emailParameters.To),
                                               @"HSD MCU auto release alert",
                                               string.Format(@"HSD MCU auto release service [{0}] is not functional. Please contact admin personal to resolve the issue.", parentThread.ServiceName)))
                        {
                            _logger.Info(@"--- Sent MCU auto release service down alert notification ------");
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(@"Exception caught at SendAlertNotificationtate, error = {0}", ex.Message);
                _success = false;
            }
        }
    }
}
