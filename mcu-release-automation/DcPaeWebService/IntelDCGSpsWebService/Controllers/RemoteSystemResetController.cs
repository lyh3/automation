using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;

using IntelDCGSpsWebService.Models;
using Newtonsoft.Json;
using WindowService.DataModel;
using CookComputing.XmlRpc;

namespace IntelDCGSpsWebService.Controllers
{
    public class RemoteSystemResetController : Controller
    {
        private Regex _regexDigits = new Regex("[0-9]");
        private System.Web.HttpContext _httpContext = null;
        private bool _shouldTerminate = false;
        private static object _updateModelSyncObj = new object();
        private static object _invokeSyncObj = new object();

        public RemoteSystemResetController()
        {
            _httpContext = System.Web.HttpContext.Current;
        }

        public ActionResult Index()
        {
            var model = Session[Definitions.SYSTEM_RESET_MODEL_KEY] as IpmiResetModel;
            var commandMapFileName = Path.Combine(Server.MapPath(@"~/App_Data/"), "IpmiCommandList.json");
            using (StreamReader file = System.IO.File.OpenText(commandMapFileName))
            {
                var serializer = new JsonSerializer();
                var map = (CommandMaps)serializer.Deserialize(file, typeof(CommandMaps));
                Session[Definitions.COMMAND_MAP_KEY] = map;
            }

            if (null == model)
            {
                model = new IpmiResetModel();
                this._UpdateSessionModel(model);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult LoadConfigJson(HttpPostedFileBase configJsonFile)
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != model)
            {
                model.ConfigJson = string.Empty;
                var uploadValid = null != model && null != configJsonFile && configJsonFile.ContentLength > 0;
                if (uploadValid)
                {
                    var success = false;
                    configJsonFile.InputStream.Seek(0, SeekOrigin.Begin);
                    var serializer = new JsonSerializer();
                    var streamReader = new StreamReader(configJsonFile.InputStream);
                    try
                    {
                        model = (IpmiResetModel)serializer.Deserialize(streamReader, typeof(IpmiResetModel));
                        if (model.BMCCfig.CanSubmit)
                        {
                            success = true;
                            model.ConfigJson = configJsonFile.FileName;
                        }
                    }
                    catch { }
                    if (!success)
                    {
                        model.ErrorMessage = string.Format("Failed to process the file : {0}, please confirm the file in correct format and contents.", configJsonFile.FileName);
                    }
                }
                this._UpdateSessionModel(model);
            }
            return RedirectToAction("Index");
        }

        public FileResult SaveConfigJson(string file = @"SystemResetConfig.json")
        {
            var model = Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null == model)
                return null;
            var json = ((IpmiResetModel)model).ToString();
            var fileName = Path.Combine(Server.MapPath(@"~/App_Data/Download"), file);
            try
            {
                System.IO.File.WriteAllText(fileName, json);
                var buffer = Encoding.UTF8.GetBytes(json);
                return File(buffer, System.Net.Mime.MediaTypeNames.Application.Octet, System.IO.Path.GetFileName(fileName));
                //return File(fileName, "application/vnd.ms-excel", System.IO.Path.GetFileName(fileName));
            }
            catch { }
            return null;
        }

        public ActionResult UpdateRepeatConfig(JsonDataContener jsonData)
        {
            if (null != jsonData
                && !string.IsNullOrEmpty(jsonData.JsonString)
                && _regexDigits.Match(jsonData.JsonString).Success)
            {
                var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
                if (null != model)
                {
                    model.Repeat = jsonData.JsonString;
                    model.CurrentCount = 0;
                    this._UpdateSessionModel(model);
                }
            }
            return RedirectToAction("Index");
        }

        public JsonResult UpdateResetTypeConfig(JsonDataContener jsonData)
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != model
                && null != jsonData
                && !string.IsNullOrEmpty(jsonData.JsonString))
            {
                model.RestType = jsonData.JsonString;
                if (model.RestType != CommandType.CustomerDefined.ToString())
                {
                    model.CustomerDefinedCommand = string.Empty;
                    model.Repeat = "1";
                }
                this._UpdateSessionModel(model);
            }
            return Json(model);
        }

        public ActionResult UpdateBmcIPAddressConfig(JsonDataContener jsonData)
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != model && null != jsonData)
            {
                model.BMCCfig.BmcIpAddress = string.IsNullOrEmpty(jsonData.JsonString) ? string.Empty : jsonData.JsonString;
                model.ErrorMessage = string.Empty;
                model.BMCCfig.IpmiAccessable = false;
                model.BMCCfig.Connected = false;
                model.BMCCfig.MEMode = MeMode.Recovery.ToString();
                if (!Utils.IsValidIpV4(model.BMCCfig.BmcIpAddress))
                {
                    model.ErrorMessage = "Please enter a valid BMC IP address.";
                }
                this._UpdateSessionModel(model);
            }
            return Redirect("Index");
        }

        public JsonResult UpdateBmcUsersConfig(JsonDataContener jsonData)
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != model && null != jsonData)
            {
                model.CurrentCount = 0;
                model.ErrorMessage = string.Empty;
                model.BMCCfig.MEMode = MeMode.Recovery.ToString();
                model.BMCCfig.User = string.IsNullOrEmpty(jsonData.JsonString) ? string.Empty : jsonData.JsonString;
                model.BMCCfig.IpmiAccessable = true;
                this._UpdateSessionModel(model);
            }
            if (null == model)
            {
                model = new IpmiResetModel();
            }
            return Json(model);
        }

        public JsonResult UpdateBmcPasswordConfig(JsonDataContener jsonData)
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != model && null != jsonData)
            {
                model.CurrentCount = 0;
                model.ErrorMessage = string.Empty;
                model.BMCCfig.MEMode = MeMode.Recovery.ToString();
                model.BMCCfig.Password = string.IsNullOrEmpty(jsonData.JsonString) ? string.Empty : jsonData.JsonString;
                model.BMCCfig.IpmiAccessable = true;
                this._UpdateSessionModel(model);
            }
            if (null == model)
            {
                model = new IpmiResetModel();
            }
            return Json(model);
        }

        public JsonResult UpdateSuTConfig(JsonDataContener jsonData)
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != model && null != jsonData)
            {
                model.SutCfig.IpAddress = string.IsNullOrEmpty(jsonData.JsonString) ? string.Empty : jsonData.JsonString;
                model.ErrorMessage = string.Empty;
                if (!Utils.IsValidIpV4(model.SutCfig.IpAddress))
                {
                    model.SutCfig.IsPowerOn = false;
                    model.ErrorMessage = "Please enter a valid SUT IP address. NOTE:If you select the Customer Defined option and like to send BMC comman, please click on the <Refresh> icon.";
                }
                this._UpdateSessionModel(model);
            }
            if (null == model)
            {
                model = new IpmiResetModel();
            }
            return Json(model);
        }


        public JsonResult UpdateTimeout(JsonDataContener jsonData)
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != jsonData
                && !string.IsNullOrEmpty(jsonData.JsonString)
                && _regexDigits.Match(jsonData.JsonString).Success)
            {
                var timeout = 0;
                int.TryParse(jsonData.JsonString, out timeout);
                model.TimeoutInMimutes = timeout;
                model.CurrentCount = 0;

                this._UpdateSessionModel(model);
            }
            if (null == model)
            {
                model = new IpmiResetModel();
            }
            return Json(model);
        }
        public JsonResult UpdateDelay(JsonDataContener jsonData)
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != jsonData
                && !string.IsNullOrEmpty(jsonData.JsonString)
                && _regexDigits.Match(jsonData.JsonString).Success)
            {
                var delay = 0;
                if (int.TryParse(jsonData.JsonString, out delay))
                    model.Delay = delay.ToString();
                this._UpdateSessionModel(model);
            }
            if (null == model)
            {
                model = new IpmiResetModel();
            }
            return Json(model);
        }

        public JsonResult UpdateCustomerDefinedCommand(JsonDataContener jsonData)
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != jsonData)
            {
                model.CustomerDefinedCommand = string.IsNullOrEmpty(jsonData.JsonString) ? string.Empty : jsonData.JsonString;
                this._UpdateSessionModel(model);
            }
            if (null == model)
            {
                model = new IpmiResetModel();
            }
            return Json(model);
        }

        public JsonResult UpdateIfAbortAtFailure(JsonDataContener jsonData)
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != jsonData
                && !string.IsNullOrEmpty(jsonData.JsonString))
            {
                var ischecked = false;
                bool.TryParse(jsonData.JsonString, out ischecked);
                model.SutCfig.ifAboutAtFailure = ischecked;
                this._UpdateSessionModel(model);
            }
            if (null == model)
            {
                model = new IpmiResetModel();
            }
            return Json(model);
        }

        public JsonResult PingSut()
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != model && null != model.SutCfig && model.Status != ProcessStatus.Alive.ToString())
            {
                //var proxy = XmlRpcProxyGen.Create<IXmlRpcInterface>();
                //var json = proxy.PingHost(model.SutCfig.IpAddress);
                //var serializer = new JsonSerializer();
                //var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
                //var reader = new StreamReader(stream);
                //var respons = (PingHostResponse)serializer.Deserialize(reader, typeof(PingHostResponse));
                //model.SutCfig.IsPowerOn = respons.hostUp;
                var reply = this._PingIPAddress(ref model);
            }
            if (null == model)
            {
                model = new IpmiResetModel();
            }
            return Json(model);
        }

        public JsonResult PingBMC()
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != model && model.BMCCfig.CanSubmit && model.Status != ProcessStatus.Alive.ToString())
            {
                var sb = _InvokeIpmiCommand(model,
                                            model.BMCCfig.BmcIpAddress,
                                            @"-b 6 -t 0x2c raw 6 1");
                var s = sb.ToString();
                if (!string.IsNullOrEmpty(s))
                {
                    if (s.EndsWith("01"))
                    {
                        model.BMCCfig.MEMode = MeMode.Operational.ToString();
                    }
                    else
                    {
                        model.BMCCfig.MEMode = MeMode.Recovery.ToString();
                    }
                }
            }
            if (null == model)
            {
                model = new IpmiResetModel();
            }
            return Json(model.BMCCfig);
        }

        public JsonResult ResetErrorMessage()
        {
            var model = (IpmiResetModel)Session[Definitions.SYSTEM_RESET_MODEL_KEY];
            if (null != model)
            {
                model.ErrorMessage = string.Empty;
                model.IpmiResponse = string.Empty;
                model.CurrentCount = 0;
                model.BMCCfig.IpmiAccessable = true;
                this._UpdateSessionModel(model);
            }
            else
            {
                model = new IpmiResetModel();
            }
            return Json(model);
        }
        public ActionResult Abort()
        {
            var model = Session[Definitions.SYSTEM_RESET_MODEL_KEY] as IpmiResetModel;
            if (null != model && null != Session[Definitions.RESET_WORK_THREAD_KEY])
            {
                model.UserCanceled = true;
                model.ErrorMessage = @"User terminated. Press the [Refresh] icon to coninue.";
                _shouldTerminate = true;
                model.Status = ProcessStatus.Terminated.ToString();
                this._UpdateSessionModel(model);
                _StopReset();
            }
            return RedirectToAction("Index");
        }


        private void _ResetAction(object m)
        {
            var model = (IpmiResetModel)m;
            if (null != model && model.CanSubmit)
            {
                //model.ErrorMessage = string.Empty;
                //model.CurrentCount = 0;
                //model.UserCanceled = false;
                //model.Status = ProcessStatus.Alive.ToString();
                var count = 0;
                int.TryParse(model.Repeat, out count);
                var failureCaptured = false;
                for (var idx = 0; idx < count; ++idx)
                {
                    if (_shouldTerminate)
                    {
                        break;
                    }
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    _InvokeIpmiCommand(model, model.BMCCfig.BmcIpAddress, model.IpmiCommand);

                    if (_shouldTerminate)
                    {
                        break;
                    }

                    if (!string.IsNullOrEmpty(model.SutCfig.IpAddress) && Utils.IsValidIpV4(model.SutCfig.IpAddress))
                    {
                        for (; ; )
                        {
                            if (stopwatch.Elapsed.Minutes >= model.TimeoutInMimutes)
                            {
                                model.ErrorMessage = string.Format(@"{0} minutes timeout reached.", model.TimeoutInMimutes);
                                this._UpdateSessionModel(model);
                                _shouldTerminate = true;
                                break;
                            }
                            var reply = this._PingIPAddress(ref model);
                            if (model.UserCanceled)
                            {
                                model.ErrorMessage = "User canceled.";
                            }
                            if (model.SutCfig.IsPowerOn)
                            {
                                model.CurrentCount += 1;
                                this._UpdateSessionModel(model);
                                break;
                            }
                        }
                    }
                    else
                    {
                        model.CurrentCount += 1;
                        this._UpdateSessionModel(model);
                    }

                    failureCaptured = this._IsFailureCaptured(ref model);
                    this._shouldTerminate = failureCaptured;

                    if (failureCaptured)
                    {
                        this._shouldTerminate = true;
                        model.Status = ProcessStatus.Terminated.ToString();
                        this._UpdateSessionModel(model);
                        _StopReset();
                        break;
                    }
                    if (!failureCaptured)
                    {
                        var delay = 0;
                        int.TryParse(model.Delay, out delay);
                        if (delay > 0)
                        {
                            var delayStopWatch = new Stopwatch();
                            delayStopWatch.Start();
                            while (true)
                            {
                                if (delayStopWatch.Elapsed.Seconds > delay)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                model.Status = ProcessStatus.ShutDown.ToString();
                this._UpdateSessionModel(model);
            }
        }

        private bool _IsFailureCaptured(ref IpmiResetModel model)
        {
            var results = false;
            var meMode = (MeMode)Enum.Parse(typeof(MeMode), model.BMCCfig.MEMode);
            if (meMode == MeMode.Recovery)
            {
                results = true;
                model.ErrorMessage = string.Format(@"The failure has been captured due to the ME in [{0}] mode.", model.BMCCfig.MEMode);
            }
            return results;
        }

        private StringBuilder _InvokeIpmiCommand(IpmiResetModel model, string ipaddress, string command)
        {
            var sb = new StringBuilder();
            lock (_invokeSyncObj)
            {
                var errorIgnoreFilter = (string)WebConfigurationManager.AppSettings["ipmitoolErrorIgnoreFilter"];
                var ipmiCommandTimeout = int.Parse(WebConfigurationManager.AppSettings["ipmiCommandTimeoutInMin"]);
                if (string.IsNullOrEmpty(errorIgnoreFilter))
                {
                    errorIgnoreFilter = "(Timeout)|(Node busy)";
                }


                var ignore = new Regex(errorIgnoreFilter);
                if (model.BMCCfig.CanSubmit && model.BMCCfig.IpmiAccessable)
                {
                    var ipmitoolPath = (string)WebConfigurationManager.AppSettings["ipmitoolPath"];
                    var ipmiCommand = string.Format(@" -I lanplus -H {0} -U {1} -P {2} {3}",
                                                    ipaddress,
                                                    model.BMCCfig.User,
                                                    model.BMCCfig.Password,
                                                    command,
                                                    "\"");
                    var startInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        WorkingDirectory = ipmitoolPath,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        FileName = Path.Combine(ipmitoolPath, "ipmitool.exe"),
                        Arguments = ipmiCommand
                    };
                    bool succss = false;
                    bool isCustomerDefinedCommand = model.RestType == CommandType.CustomerDefined.ToString();
                    for (int i = 0; i < 3 && !succss; ++i)
                    {
                        try
                        {
                            var process = new Process() { StartInfo = startInfo };
                            process.Start();

                            var proc = Process.GetProcessesByName("ipmitool");
                            if (null != proc)
                            {
                                var t = ipmiCommandTimeout * 3600 * 1000;
                                foreach (var p in proc)
                                {
                                    if (!p.WaitForExit(t))
                                    {
                                        p.Kill();
                                        model.ErrorMessage = string.Format("Time out {0} min reached, forces to terminat the process for ipmi command [{1}].", ipmiCommandTimeout, command);
                                        break;
                                    }
                                }
                            }

                            var err = process.StandardError.ReadToEnd();

                            if (string.IsNullOrEmpty(err))
                            {
                                var msg = process.StandardOutput.ReadToEnd();
                                if (isCustomerDefinedCommand
                                    && command == model.CustomerDefinedCommand)
                                {
                                    model.IpmiResponse = model.FormatCustomerDefinedIpmiResponse(command, msg);
                                    this._UpdateSessionModel(model);
                                }
                                else
                                {
                                    sb.Append(msg.Replace("\r\n", string.Empty));
                                }
                            }
                            else
                            {
                                if (ignore.Match(err).Success)
                                {
                                    Thread.Sleep(1000);
                                    continue;
                                }
                                model.BMCCfig.IpmiAccessable = false;
                                model.ErrorMessage = err;
                                this._UpdateSessionModel(model);
                            }
                            succss = true;
                        }
                        catch (Exception e)
                        {
                            model.ErrorMessage = e.Message;
                            model.Status = ProcessStatus.ShutDown.ToString();
                            this._UpdateSessionModel(model);
                            _shouldTerminate = true;
                        }
                    }
                }
            }
            return sb;
        }

        [HttpPost]
        public ActionResult Submit()
        {
            var model = _httpContext.Session[Definitions.SYSTEM_RESET_MODEL_KEY] as IpmiResetModel;
            if (null != model && model.BMCCfig.IpmiAccessable)
            {
                model.ErrorMessage = string.Empty;
                model.IpmiResponse = string.Empty;
                model.CurrentCount = 0;
                model.UserCanceled = false;
                model.Status = ProcessStatus.Alive.ToString();
                this._UpdateSessionModel(model);
                var resetThread = new Thread(_ResetAction)
                {
                    IsBackground = true
                };
                Session[Definitions.RESET_WORK_THREAD_KEY] = resetThread;
                resetThread.Start(model);
                //_resetThread.Join();
            }
            return RedirectToAction("Index");
        }

        private PingReply _PingIPAddress(ref IpmiResetModel model)
        {
            var reply = Utils.PingIPAddress(model.SutCfig.IpAddress);
            model.SutCfig.IsPowerOn = false;

            if (null != reply)
            {
                if (reply.Status == IPStatus.Success)
                {
                    model.SutCfig.IsPowerOn = true;
                }
                else
                {
                    model.ErrorMessage = string.Format("Failed to ping IP [{0}], status = {1}.", model.SutCfig.IpAddress, reply.Status.ToString());
                }
            }
            reply = Utils.PingIPAddress(model.BMCCfig.BmcIpAddress);
            model.BMCCfig.Connected = false;
            if (null != reply)
            {
                if (reply.Status == IPStatus.Success)
                {
                    model.BMCCfig.Connected = true;
                }
                else
                {
                    model.ErrorMessage = string.Format("Failed to ping BMC [{0}], status = {1}.", model.BMCCfig.BmcIpAddress, reply.Status.ToString());
                }
            }

            this._UpdateSessionModel(model);
            return reply;
        }

        private void _UpdateSessionModel(IpmiResetModel model)
        {
            lock (_updateModelSyncObj)
            {
                for (int i = 0; i < 3; ++i)
                {
                    try
                    {
                        try
                        {
                            Session.Remove(Definitions.SYSTEM_RESET_MODEL_KEY);
                        }
                        catch { }
                        model.CmdMaps = (CommandMaps)Session[Definitions.COMMAND_MAP_KEY];
                        Session[Definitions.SYSTEM_RESET_MODEL_KEY] = model;
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void _StopReset()
        {
            try
            {
                var resetThread = (Thread)Session[Definitions.RESET_WORK_THREAD_KEY];
                if (null != resetThread
                    && resetThread.IsAlive)
                {
                    resetThread.Interrupt();
                    lock (this)
                    {
                        Monitor.Pulse(this);
                    }
                    resetThread.Abort();
                    Session.Remove(Definitions.RESET_WORK_THREAD_KEY);
                }
            }
            catch { }
        }
    }
}