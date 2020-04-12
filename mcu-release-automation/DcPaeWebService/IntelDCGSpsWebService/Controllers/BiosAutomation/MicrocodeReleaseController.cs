using System;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Generic;
using IntelDCGSpsWebService.Models;
using System.Web.Configuration;
using System.Diagnostics;
using System.Threading;

using Newtonsoft.Json;
using Automation.Base.BuildingBlocks;
using WindowService.DataModel;

namespace IntelDCGSpsWebService.Controllers
{
    public class MicrocodeReleaseController : Controller
    {
        private static object _updateModelSyncObj = new object();
        private static object _syncObj = new object();
        private static object _invokeSyncObj = new object();
        private WorkflowConfigModel _configModelCache = null;
        private StringBuilder _sbErrorReceivedBuffer = new StringBuilder();

        public MicrocodeReleaseController()
        {
        }
        public ActionResult Index()
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;

            if (null == model)
            {
                model = _loadDefaultReleaseRequest();
                _UpdateSessionModel(model);

                var readmeFilewatcher = new FileSystemWatcher();
                var workingdirectory = WebConfigurationManager.AppSettings["MicrocodeReleaseWorkingDirectory"];
                readmeFilewatcher.Path = workingdirectory;
                readmeFilewatcher.NotifyFilter = NotifyFilters.LastAccess
                                                | NotifyFilters.LastWrite;
                readmeFilewatcher.Filter = "*.md";
                readmeFilewatcher.Changed += _onReadmeFChanged;
                readmeFilewatcher.EnableRaisingEvents = true;
                Session[Definitions.README_WATCH_KEY] = readmeFilewatcher;

                var hsdMapFilewatcher = new FileSystemWatcher();
                var jsondirectory = string.Format(@"{0}\Json", workingdirectory);
                hsdMapFilewatcher.Path = jsondirectory;
                hsdMapFilewatcher.NotifyFilter = NotifyFilters.LastAccess
                                                | NotifyFilters.LastWrite;
                hsdMapFilewatcher.Filter = "*.json";
                hsdMapFilewatcher.Changed += _onHsdMapFilewatcherChanged; ;
                hsdMapFilewatcher.EnableRaisingEvents = true;
                Session[Definitions.HSDMAP_WATCH_KEY] = hsdMapFilewatcher;

                this._RetriveHsdMcuArticalIDs(model);
            }
            return View(model);
        }
        public ActionResult SyncHsdIds()
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            if (null != model)
            {
                this._RetriveHsdMcuArticalIDs(model);
            }
            return RedirectToAction("Index");
        }
        public JsonResult VerifyMiddleWareServiceAvailable()
        {
            var jsonData = new JsonDataContener { JsonString = string.Empty };
            if (!@"McuReleaseSubmitApp".FindProcess())
            {
                jsonData.JsonString = @"The Middleware service [McuReleaseSubmitApp] is not functional, please contact admin personal to resolve.";
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult StartMicrododeReleaseHSD(string GenericBinaryName_0,
                                                     string GenericBinaryName_1,
                                                     string GenericBinaryName_2,
                                                     string GenericBinaryName_3,
                                                     string GenericBinaryName_4,
                                                     string GenericBinaryName_5,
                                                     string GenericBinaryName_6,
                                                     string GenericBinaryName_7,
                                                     string GenericBinaryName_8,
                                                     string GenericBinaryName_9)
        {
            if (WebConfigurationManager.AppSettings["MiddlewareServiceName"].FindProcess())
            {
                var configJsonWorksheet = string.Empty;
                lock (_invokeSyncObj)
                {
                    lock (this)
                    {
                        var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
                        var releaseto = string.Empty;
                        try
                        {
                            if (null != model)
                            {
                                model.ErrorMessage = string.Empty;
                                model.ProcessFailed = false;
                                var workingdirectory = WebConfigurationManager.AppSettings["MicrocodeReleaseWorkingDirectory"];
                                var config = WebConfigurationManager.AppSettings["MicrocodeReleaseDefaultConfig"];
                                var configModel = _GetConfigModel(config);
                                if (null == configModel)
                                {
                                    return RedirectToAction("Index");
                                }
                                var dropboxPath = Path.Combine(workingdirectory, configModel.McuDropBox);

                                releaseto = model.SelectedReleaseLee.ReleaseTo;
                                var mcuFileNames = new List<string>();
                                mcuFileNames.AddRange(new[] { GenericBinaryName_0,GenericBinaryName_1,GenericBinaryName_2, GenericBinaryName_3,
                                GenericBinaryName_4, GenericBinaryName_5, GenericBinaryName_6, GenericBinaryName_7, GenericBinaryName_8,
                                GenericBinaryName_9
                            });
                                var isValiudUplods = false;
                                for (var i = 0; i < mcuFileNames.Count; i++)
                                {
                                    var mcuFileName = mcuFileNames[i];
                                    if (!string.IsNullOrEmpty(mcuFileName))
                                    {
                                        var mcu = model.SelectedReleaseLee[string.Format(@"GenericBinaryName_{0}", i)];
                                        mcu.Mcu = mcuFileName;
                                    }
                                }

                                var formatedJsonFiles = _formatedJsonFiels;
                                var jsonRequest = formatedJsonFiles.Item1.Replace("ReleaseAutomation", @"ReleaseAutomation\ReleaseDropBox");
                                configJsonWorksheet = formatedJsonFiles.Item2;
                                if (System.IO.File.Exists(configJsonWorksheet))
                                {
                                    System.IO.File.Delete(configJsonWorksheet);
                                }
                                var otherWorksheet = configJsonWorksheet.Replace(@"HSD", @"Local");
                                if (System.IO.File.Exists(otherWorksheet))
                                {
                                    System.IO.File.Delete(otherWorksheet);
                                }
                                var releaseLeeList = new List<ReleaseLee>();
                                releaseLeeList.Add(model.SelectedReleaseLee);
                                model.ReleaseLees = releaseLeeList.ToArray();
                                var json = model.ToString();
                                System.IO.File.WriteAllText(jsonRequest, json);
                            }
                        }
                        catch (Exception ex)
                        {
                            MvcApplication.Logger.ErrorFormat(@"Exception caught at submit mcu, error = {0}", ex.Message);
                        }
                        finally
                        {
                            var defaultModel = this._loadDefaultReleaseRequest();
                            defaultModel.ErrorMessage = this._sbErrorReceivedBuffer.ToString();
                            defaultModel.AddReleaseLee(releaseto);
                            defaultModel.ReleaseMcuSource = model.ReleaseMcuSource;
                            this._RetriveHsdMcuArticalIDs(defaultModel);
                            this._UpdateSessionModel(defaultModel);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult StartMicrododeReleaseLocal(HttpPostedFileBase GenericBinaryName_0,
                                                       HttpPostedFileBase GenericBinaryName_1,
                                                       HttpPostedFileBase GenericBinaryName_2,
                                                       HttpPostedFileBase GenericBinaryName_3,
                                                       HttpPostedFileBase GenericBinaryName_4,
                                                       HttpPostedFileBase GenericBinaryName_5,
                                                       HttpPostedFileBase GenericBinaryName_6,
                                                       HttpPostedFileBase GenericBinaryName_7,
                                                       HttpPostedFileBase GenericBinaryName_8,
                                                       HttpPostedFileBase GenericBinaryName_9)
        {
            if (WebConfigurationManager.AppSettings["MiddlewareServiceName"].FindProcess())
            {
                var configJsonWorksheet = string.Empty;
                lock (_invokeSyncObj)
                {
                    lock (this)
                    {
                        var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
                        var releaseto = string.Empty;
                        try
                        {
                            if (null != model)
                            {
                                model.ErrorMessage = string.Empty;
                                model.ProcessFailed = false;
                                var workingdirectory = WebConfigurationManager.AppSettings["MicrocodeReleaseWorkingDirectory"];
                                var config = WebConfigurationManager.AppSettings["MicrocodeReleaseDefaultConfig"];
                                var configModel = _GetConfigModel(config);
                                if (null == configModel)
                                {
                                    return RedirectToAction("Index");
                                }
                                var dropboxPath = Path.Combine(workingdirectory, configModel.McuDropBox);

                                releaseto = model.SelectedReleaseLee.ReleaseTo;
                                var uploads = new List<HttpPostedFileBase>();
                                uploads.AddRange(new[] { GenericBinaryName_0,GenericBinaryName_1,GenericBinaryName_2, GenericBinaryName_3,
                                GenericBinaryName_4, GenericBinaryName_5, GenericBinaryName_6, GenericBinaryName_7, GenericBinaryName_8,
                                GenericBinaryName_9
                            });
                                var isValiudUplods = false;
                                foreach (var x in uploads)
                                {
                                    isValiudUplods |= null != x && x.ContentLength > 0;
                                }
                                if (!isValiudUplods)
                                {
                                    model.ErrorMessage = "Cannot find any mcu binary to submit. Please select a mcu binary and try again.";
                                    model.ProcessFailed = true;
                                    return RedirectToAction("Index");
                                }
                                for (var i = 0; i < uploads.Count; i++)
                                {
                                    var upload = uploads[i];
                                    if (null != upload && upload.ContentLength > 0)
                                    {
                                        var fileName = Path.GetFileName(upload.FileName);
                                        var mcu = model.SelectedReleaseLee[string.Format(@"GenericBinaryName_{0}", i)];
                                        upload.SaveAs(Path.Combine(dropboxPath, fileName));
                                        mcu.Mcu = fileName;
                                    }
                                }

                                var formatedJsonFiles = _formatedJsonFiels;
                                var jsonRequest = formatedJsonFiles.Item1.Replace("ReleaseAutomation", @"ReleaseAutomation\ReleaseDropBox");
                                configJsonWorksheet = formatedJsonFiles.Item2;
                                if (System.IO.File.Exists(configJsonWorksheet))
                                {
                                    System.IO.File.Delete(configJsonWorksheet);
                                }
                                var otherWorksheet = configJsonWorksheet.Replace(@"Local", @"HSD");
                                if (System.IO.File.Exists(otherWorksheet))
                                {
                                    System.IO.File.Delete(otherWorksheet);
                                }
                                var releaseLeeList = new List<ReleaseLee>();
                                releaseLeeList.Add(model.SelectedReleaseLee);
                                model.ReleaseLees = releaseLeeList.ToArray();
                                var json = model.ToString();
                                System.IO.File.WriteAllText(jsonRequest, json);
                            }
                        }
                        catch (Exception ex)
                        {
                            MvcApplication.Logger.ErrorFormat(@"Exception caught at submit mcu, error = {0}", ex.Message);
                        }
                        finally
                        {
                            var defaultModel = this._loadDefaultReleaseRequest();
                            defaultModel.ErrorMessage = this._sbErrorReceivedBuffer.ToString();
                            defaultModel.AddReleaseLee(releaseto);
                            defaultModel.ReleaseMcuSource = model.ReleaseMcuSource;
                            this._UpdateSessionModel(defaultModel);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
        public JsonResult VolunteerInputUpdate()
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            if (null != model)
            {
                model.UpdateVolunteerInputs();
                _UpdateSessionModel(model);
            }

            return Json("N/A");
        }
        public JsonResult Reset()
        {
            System.IO.File.Delete(_formatedJsonFiels.Item2);
            System.IO.File.Delete(string.Format(@"{0}\McuReleaseAutomation_{1}.log",
                                                WebConfigurationManager.AppSettings["MicrocodeReleaseWorkingDirectory"],
                                                System.Web.HttpContext.Current.Session.SessionID));
            this._sbErrorReceivedBuffer.Clear();
            this._UpdateSessionModel(_loadDefaultReleaseRequest());
            Session[Definitions.RELEASE_TO_SESSION_KEY] = string.Empty;
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            if (null != model)
            {
                this._RetriveHsdMcuArticalIDs(model);
            }

            return Json("N/A");
        }
        public JsonResult UpdateReleasSource(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            if (model != null && null != jsonData
                && !string.IsNullOrEmpty(jsonData.JsonString))
            {
                model.ReleaseMcuSource = jsonData.JsonString;
                _UpdateSessionModel(model);
            }
            return Json("N/A");
        }
        public JsonResult UpdateReleaseTo(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            ReleaseLee releaselee = null;
            if (model != null && null != jsonData
                && !string.IsNullOrEmpty(jsonData.JsonString))
            {
                this.Reset();
                model.NotificationMessage = string.Empty;
                var releaseTo = jsonData.JsonString;
                releaselee = model[releaseTo];
                if (null != releaselee)
                {
                    foreach (var mcu in releaselee.Mcus)
                    {
                        if (releaseTo == "NDA" || releaseTo == "Public")
                        {
                            mcu.Scope = releaseTo;
                        }
                        else
                        {
                            mcu.Scope = "RestrictedPkg";
                        }
                    }
                    model.SelectedReleaseLee = releaselee;
                    model.NotificationMessage = string.Format(@"The release to [ {0} ] has been selected.", model.SelectedReleaseLee.ReleaseTo);
                    Session[Definitions.RELEASE_TO_SESSION_KEY] = releaseTo;
                    _UpdateSessionModel(model);
                }
            }
            return Json(releaselee);
        }
        public JsonResult AddReleaseTo(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            if (model != null && null != jsonData
                && !string.IsNullOrEmpty(jsonData.JsonString))
            {
                this.Reset();
                model.NotificationMessage = string.Empty;
                model.NotificationMessage = model.AddReleaseLee(jsonData.JsonString);
                _UpdateSessionModel(model);
            }
            return Json(model.NotificationMessage);
        }
        public JsonResult RemoveSelectedReleaseTo()
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            if (model != null)
            {
                model.NotificationMessage = string.Empty;
                if (null != model.SelectedReleaseLee)
                {
                    model.NotificationMessage = model.RemoveReleaseLee(model.SelectedReleaseLee.ReleaseTo);
                }
                else
                {
                    model.NotificationMessage = "There is no active ReleaseTo been selected for removing.";
                }
                _UpdateSessionModel(model);
            }
            return Json("N/A");
        }
        public JsonResult GetReleaseLee(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            ReleaseLee releaselee = null;
            if (model != null && null != jsonData
                && !string.IsNullOrEmpty(jsonData.JsonString))
            {
                model.NotificationMessage = string.Empty;
                if (null != releaselee)
                {
                    releaselee = new ReleaseLee
                    {
                        ReleaseTo = jsonData.JsonString
                    };
                    model.SelectedReleaseLee = releaselee;
                    _UpdateSessionModel(model);
                }
            }
            return Json(releaselee);
        }
        public JsonResult AddMcu(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                model.NotificationMessage = model.SelectedReleaseLee.AddNewRelease(jsonData.JsonString);
                _UpdateSessionModel(model);
            }
            return Json("N/A");
        }
        public JsonResult RemoveMcu()
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                model.SelectedReleaseLee.RemoveReleases();
                _UpdateSessionModel(model);
            }
            return Json("N/A");
        }
        public JsonResult UpdateCPUCodeName(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var cpuCodeNameInfo = serializer.Deserialize<CpuCodeNametInfo>(reader);
                var mcu = model[cpuCodeNameInfo.ReleaseTo][cpuCodeNameInfo.SelectedMcu.TrimEnd(',')];
                mcu.CPUCodeName = cpuCodeNameInfo.CpuCodeName;

                msg = string.Format("Update mcu {0} Cpu Code Name = {1}", cpuCodeNameInfo.SelectedMcu,
                    !string.IsNullOrEmpty(cpuCodeNameInfo.CpuCodeName) ? cpuCodeNameInfo.CpuCodeName : Definitions.REQUIRED_FIELD_MESSAGE);
                model.NotificationMessage = msg;
                _UpdateSessionModel(model);
            }
            return Json(msg);
        }
        public JsonResult UpdateStepping(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var steppingInfo = serializer.Deserialize<SteppingInfo>(reader);
                var mcu = model[steppingInfo.ReleaseTo][steppingInfo.SelectedMcu.TrimEnd(',')];
                mcu.Stepping = steppingInfo.Stepping;

                msg = string.Format("Update mcu {0} Stepping = {1}", steppingInfo.SelectedMcu,
                   !string.IsNullOrEmpty(steppingInfo.Stepping) ? steppingInfo.Stepping : Definitions.REQUIRED_FIELD_MESSAGE);
                model.NotificationMessage = msg;
                _UpdateSessionModel(model);
            }
            return Json(msg);
        }
        public JsonResult UpdateCpuID(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            McuRelease mcu = null;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var cpuIdInfo = serializer.Deserialize<CpuIdInfo>(reader);
                mcu = model[cpuIdInfo.ReleaseTo][cpuIdInfo.SelectedMcu.TrimEnd(',')];
                mcu.CpuID = cpuIdInfo.CpuID;

                _SyncMcu(model, model[cpuIdInfo.ReleaseTo], mcu);

                msg = string.Format("Update mcu {0} CPU Id = {1}", cpuIdInfo.SelectedMcu,
                    !string.IsNullOrEmpty(cpuIdInfo.CpuID) ? cpuIdInfo.CpuID : Definitions.OPTIONAL_FIELD_MESSAGE);
                model.NotificationMessage = msg;
                _UpdateSessionModel(model);
            }
            return Json(mcu);
        }
        public JsonResult UpdatePlatformID_1(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var platformIdInfo = serializer.Deserialize<PlatformIdInfo>(reader);
                var mcu = model[platformIdInfo.ReleaseTo][platformIdInfo.SelectedMcu.TrimEnd(',')];
                mcu.PlatformID = platformIdInfo.PlatformID;

                msg = string.Format("Update mcu {0} Platform ID = {1}", platformIdInfo.SelectedMcu,
                    !string.IsNullOrEmpty(platformIdInfo.PlatformID) ? platformIdInfo.PlatformID : Definitions.OPTIONAL_FIELD_MESSAGE);
                model.NotificationMessage = msg;
                _UpdateSessionModel(model);
            }
            return Json(msg);
        }
        public JsonResult UpdatePlatformID_2(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var platformIdInfo = serializer.Deserialize<PlatformIdInfo>(reader);
                var mcu = model[platformIdInfo.ReleaseTo][platformIdInfo.SelectedMcu.TrimEnd(',')];
                mcu.PlatformID = platformIdInfo.PlatformID;

                msg = string.Format("Update mcu {0} Platform ID = {1}", platformIdInfo.SelectedMcu,
                    !string.IsNullOrEmpty(platformIdInfo.PlatformID) ? platformIdInfo.PlatformID : Definitions.OPTIONAL_FIELD_MESSAGE);
                model.NotificationMessage = msg;
                _UpdateSessionModel(model);
            }
            return Json(msg);
        }
        public JsonResult UpdateMicrocode(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var microcodeInfo = serializer.Deserialize<MicrocodeInfo>(reader);
                var mcu = model[microcodeInfo.ReleaseTo][microcodeInfo.SelectedMcu.TrimEnd(',')];
                mcu.MicroCode = microcodeInfo.Microcode;

                msg = string.Format("Update mcu {0} Microcode file name = {1}", microcodeInfo.SelectedMcu,
                   !string.IsNullOrEmpty(microcodeInfo.Microcode) ? microcodeInfo.Microcode : Definitions.OPTIONAL_FIELD_MESSAGE);
                model.NotificationMessage = msg;
                _UpdateSessionModel(model);
            }
            return Json(msg);
        }
        public JsonResult UpdateScope(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var scopeInfo = serializer.Deserialize<ScopeInfo>(reader);

                var mcu = model[scopeInfo.ReleaseTo][scopeInfo.SelectedMcu.TrimEnd(',')];
                mcu.Scope = scopeInfo.Scope;

                msg = string.Format("Update mcu {0} Scope = {1}", scopeInfo.SelectedMcu,
                    !string.IsNullOrEmpty(scopeInfo.Scope) ? scopeInfo.Scope : Definitions.REQUIRED_FIELD_MESSAGE);
                model.NotificationMessage = msg;
                _UpdateSessionModel(model);
            }
            return Json(msg);
        }
        public JsonResult UpdateMcuSelected(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var mcuInfo = serializer.Deserialize<McuSelectInfo>(reader);
                var mcu = model.SelectedReleaseLee[mcuInfo.Id.Replace("_Check", string.Empty)];
                mcu.Selected = mcuInfo.IsChecked;

                _UpdateSessionModel(model);
                msg = string.Format("Update mcu {0} IsChecked = {1}", mcuInfo.Id, mcuInfo.IsChecked);
                model.NotificationMessage = msg;
            }
            return Json(msg);
        }
        public JsonResult UpdateCpuSegment(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            McuRelease mcu = null;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var cpuSegmentInfo = serializer.Deserialize<CpuSegmentInfo>(reader);
                mcu = model[cpuSegmentInfo.ReleaseTo][cpuSegmentInfo.SelectedMcu.TrimEnd(',')];
                mcu.CpuSegment = cpuSegmentInfo.CpuSegment;

                _SyncMcu(model, model[cpuSegmentInfo.ReleaseTo], mcu);

                msg = string.Format("Update mcu {0} CpuSegment = {1}", cpuSegmentInfo.SelectedMcu,
                    !string.IsNullOrEmpty(cpuSegmentInfo.CpuSegment) ? cpuSegmentInfo.CpuSegment : Definitions.REQUIRED_FIELD_MESSAGE);
                model.NotificationMessage = msg;
                _UpdateSessionModel(model);
            }
            return Json(mcu);
        }
        public JsonResult UpdateReleaseTarget(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var releaseTargetInfo = serializer.Deserialize<ReleaseTargetInfo>(reader);
                var mcu = model[releaseTargetInfo.ReleaseTo][releaseTargetInfo.SelectedMcu.TrimEnd(',')];
                mcu.ReleaseTarget = releaseTargetInfo.ReleaseTarget;

                _UpdateSessionModel(model);
                msg = string.Format("Update mcu {0} ReleaseTarget = {1}", releaseTargetInfo.SelectedMcu,
                    !string.IsNullOrEmpty(releaseTargetInfo.ReleaseTarget) ? releaseTargetInfo.ReleaseTarget : Definitions.REQUIRED_FIELD_MESSAGE);
                model.NotificationMessage = msg;
            }
            return Json(msg);
        }
        public JsonResult UpdateCpuPubliSpecUpdate(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var cpuPubliSpecUpdateInfo = serializer.Deserialize<CpuPubliSpecUpdateInfo>(reader);
                var mcu = model[cpuPubliSpecUpdateInfo.ReleaseTo][cpuPubliSpecUpdateInfo.SelectedMcu.TrimEnd(',')];
                mcu.CpuSegment = cpuPubliSpecUpdateInfo.CpuPubliSpecUpdate;

                _UpdateSessionModel(model);
                msg = string.Format("Update mcu {0} CpuPubliSpecUpdate = {1}", cpuPubliSpecUpdateInfo.SelectedMcu,
                    !string.IsNullOrEmpty(cpuPubliSpecUpdateInfo.CpuPubliSpecUpdate) ? cpuPubliSpecUpdateInfo.CpuPubliSpecUpdate : Definitions.REQUIRED_FIELD_MESSAGE);
                model.NotificationMessage = msg;
            }
            return Json(msg);
        }
        public JsonResult UpdateIntelProductSpec(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var intelProductSpecInfo = serializer.Deserialize<IntelProductSpecInfo>(reader);
                var mcu = model[intelProductSpecInfo.ReleaseTo][intelProductSpecInfo.SelectedMcu.TrimEnd(',')];
                mcu.ReleaseTarget = intelProductSpecInfo.IntelProductSpec;

                _UpdateSessionModel(model);
                msg = string.Format("Update mcu {0} IntelProductSpec = {1}", intelProductSpecInfo.SelectedMcu,
                    !string.IsNullOrEmpty(intelProductSpecInfo.IntelProductSpec) ? intelProductSpecInfo.IntelProductSpec : Definitions.REQUIRED_FIELD_MESSAGE);
                model.NotificationMessage = msg;
            }
            return Json(msg);
        }
        public JsonResult UpdateCpuNDASpecUpdate(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var cpuNDASpecUpdateInfo = serializer.Deserialize<CpuNDASpecUpdateInfo>(reader);
                var mcu = model[cpuNDASpecUpdateInfo.ReleaseTo][cpuNDASpecUpdateInfo.SelectedMcu.TrimEnd(',')];
                mcu.ReleaseTarget = cpuNDASpecUpdateInfo.CpuNDASpecUpdate;

                _UpdateSessionModel(model);
                msg = string.Format("Update mcu {0} CpuNDASpecUpdate = {1}", cpuNDASpecUpdateInfo.SelectedMcu,
                    !string.IsNullOrEmpty(cpuNDASpecUpdateInfo.CpuNDASpecUpdate) ? cpuNDASpecUpdateInfo.CpuNDASpecUpdate : Definitions.REQUIRED_FIELD_MESSAGE);
                model.NotificationMessage = msg;
            }
            return Json(msg);
        }
        public JsonResult UpdateProducts(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var productsInfo = serializer.Deserialize<ProductsInfo>(reader);
                var mcu = model[productsInfo.ReleaseTo][productsInfo.SelectedMcu.TrimEnd(',')];
                mcu.ReleaseTarget = productsInfo.Products;

                _UpdateSessionModel(model);
                msg = string.Format("Update mcu {0} Products = {1}", productsInfo.SelectedMcu,
                    !string.IsNullOrEmpty(productsInfo.Products) ? productsInfo.Products : Definitions.REQUIRED_FIELD_MESSAGE);
                model.NotificationMessage = msg;
            }
            return Json(msg);
        }
        public JsonResult UpdateProcessorModel(JsonDataContener jsonData)
        {
            var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
            var msg = string.Empty;
            if (model != null && model.SelectedReleaseLee != null)
            {
                model.NotificationMessage = string.Empty;
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var processorModelInfo = serializer.Deserialize<ProcessorModelInfo>(reader);
                var mcu = model[processorModelInfo.ReleaseTo][processorModelInfo.SelectedMcu.TrimEnd(',')];
                mcu.ReleaseTarget = processorModelInfo.ProcessorModel;

                _UpdateSessionModel(model);
                msg = string.Format("Update mcu {0} ProcessorModel = {1}", processorModelInfo.SelectedMcu,
                    !string.IsNullOrEmpty(processorModelInfo.ProcessorModel) ? processorModelInfo.ProcessorModel : Definitions.REQUIRED_FIELD_MESSAGE);
                model.NotificationMessage = msg;
            }
            return Json(msg);
        }
        public JsonResult GetLogContents()
        {
            var config = WebConfigurationManager.AppSettings["MicrocodeReleaseDefaultConfig"];
            if (config.IsFileLocked())
            {
                return Json(null);
            }
            var formatedJsonFiles = _formatedJsonFiels;
            var configJsonWorksheet = formatedJsonFiles.Item2;
            if (System.IO.File.Exists(configJsonWorksheet))
            {
                config = configJsonWorksheet;
            }

            var configModel = _GetConfigModel(config);
            configModel.SessionLog = string.Format(@"{0}_{1}.log", configModel.LogName, System.Web.HttpContext.Current.Session.SessionID);

            return Json(configModel);
        }
        public JsonResult GetHsdArticleIdMap()
        {
            var hsdMcuAricleMapJsonPath = WebConfigurationManager.AppSettings["HsdMcuAricleMapJsonPath"];
            var sb = new StringBuilder();
            HsdMcuArticleIdMap hsdMcuArticleMap = null;
            using (StreamReader file = System.IO.File.OpenText(hsdMcuAricleMapJsonPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                hsdMcuArticleMap = (HsdMcuArticleIdMap)serializer.Deserialize(file, typeof(HsdMcuArticleIdMap));
            }
            return Json(hsdMcuArticleMap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTransactionState()
        {
            var config = WebConfigurationManager.AppSettings["MicrocodeReleaseDefaultConfig"];
            if (config.IsFileLocked())
            {
                return Json(null);
            }
            lock (_syncObj)
            {
                var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
                WorkflowConfigModel configModel = null;
                var requiredParamsSatisfied = false;
                dynamic formatedJsonFiles = null;
                dynamic jsonRequest = null;
                dynamic configJsonWorksheet = null;

                if (null != model && null != model.SelectedReleaseLee)
                {
                    requiredParamsSatisfied = true;
                    foreach (var mcu in model.SelectedReleaseLee.Mcus)
                    {
                        requiredParamsSatisfied &= !(//string.IsNullOrEmpty(mcu.CPUCodeName)
                                                    string.IsNullOrEmpty(mcu.CpuSegment)
                                                    //|| string.IsNullOrEmpty(mcu.Stepping)
                                                    || string.IsNullOrEmpty(mcu.ReleaseTarget)
                                                    //|| string.IsNullOrEmpty(mcu.Scope)
                                                    //|| string.IsNullOrEmpty(mcu.Stepping)
                                                    //|| string.IsNullOrEmpty(mcu.CpuID)
                                                    //|| string.IsNullOrEmpty(mcu.PlatformID) || mcu.PlatformID.Length != 2
                                                    //|| string.IsNullOrEmpty(mcu.MicroCode)
                                                    );
                    }
                    model.TransStatus = TransactionStatus.Progress.ToString();
                }

                if (null == model)
                {
                    model = this._loadDefaultReleaseRequest();
                    this._RetriveHsdMcuArticalIDs(model);
                    this._UpdateSessionModel(model);
                }

                formatedJsonFiles = _formatedJsonFiels;
                jsonRequest = formatedJsonFiles.Item1;
                configJsonWorksheet = formatedJsonFiles.Item2;

                if (System.IO.File.Exists(configJsonWorksheet))
                {
                    config = configJsonWorksheet;
                }

                configModel = _GetConfigModel(config);
                if (null != configModel)
                {
                    model.ProcessFailed = configModel.TransStatus == TransactionStatus.Failed.ToString();
                    if (!_submissionPreviligeVerification())
                    {
                        configModel.TransError = this._sbErrorReceivedBuffer.ToString();
                        configModel.TransStatus = TransactionStatus.Failed.ToString();
                    }
                    model.ErrorMessage = configModel.TransError;
                    var modelPakage = new ModelPackage { modelRequest = model, modelConfig = configModel };
                    _UpdateDefaultRequestJson(model, configModel);
                    return Json(modelPakage);
                }
                return Json(null);
            }
        }
        public JsonResult RefreshHsdMap()
        {
            try
            {
                var hsdMcuAricleMapJsonPath = WebConfigurationManager.AppSettings["HsdMcuAricleMapJsonPath"];
                var sb = new StringBuilder();
                HsdMcuArticleIdMap hsdMcuArticleMap = null;
                using (StreamReader file = System.IO.File.OpenText(hsdMcuAricleMapJsonPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    hsdMcuArticleMap = (HsdMcuArticleIdMap)serializer.Deserialize(file, typeof(HsdMcuArticleIdMap));
                    DateTime lastrun = DateTime.Parse(hsdMcuArticleMap.Schedule.LastRun);
                    var schedule = hsdMcuArticleMap.Schedule;
                    var delta = schedule.Every * -1;
                    switch (schedule.Frequency)
                    {
                        case "Monthly":
                            lastrun = lastrun.AddMonths(delta);
                            break;
                        case "Dayly":
                            lastrun = lastrun.AddDays(delta);
                            break;
                        case "Hourly":
                        default:
                            lastrun = lastrun.AddHours(delta);
                            break;
                    }
                    hsdMcuArticleMap.Schedule.LastRun = lastrun.ToString(@"yyy-MM-dd HH:mm:ss");
                }
                var json = hsdMcuArticleMap.ToString();
                System.IO.File.WriteAllText(hsdMcuAricleMapJsonPath, json);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    Thread.Sleep(1000);
                } while (stopwatch.Elapsed.Minutes < hsdMcuArticleMap.Schedule.AlertReportGracePeriodInMin);
            }
            catch (Exception ex)
            {
                MvcApplication.Logger.ErrorFormat(@"Error caught at RefreshHsdMap, error = {0}", ex.Message);
            }
            return Json(new JsonDataContener { JsonString = "HSD ID map sync completed." });
        }
        #region Private methods
        private bool _submissionPreviligeVerification()
        {
            bool grant = true;
            var clientMachineName = (Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName);
            var mcuAdmin = WebConfigurationManager.AppSettings["mcu_administrators"].Split(';');
            var user = Request.LogonUserIdentity.Name;
            if (null == mcuAdmin.FirstOrDefault(x => x.Equals(user, StringComparison.OrdinalIgnoreCase)))
            {
                this._sbErrorReceivedBuffer.Append(string.Format(@"The user account [{0}] logon at system [{1}] is not granted to release the microcode, please contact admin personals.", user, clientMachineName));
                grant = false;
            }

            return grant;
        }
        private void _UpdateDefaultRequestJson(MicrocodeReleaseModel model, WorkflowConfigModel configModel)
        {
            var releaseRequestFileName = Path.Combine(Server.MapPath(@"~/App_Data/"), "McuReleaseRequestTemplate.json");
            MicrocodeReleaseModel m = null;
            if (null == model.SelectedReleaseLee
                || string.IsNullOrEmpty(model.SelectedReleaseLee.ReleaseTo)
                || configModel.TransStatus != TransactionStatus.Completed.ToString()
                || !string.IsNullOrEmpty(configModel.TransError))
            {
                return;
            }
            var add = false;
            using (StreamReader file = System.IO.File.OpenText(releaseRequestFileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                m = (MicrocodeReleaseModel)serializer.Deserialize(file, typeof(MicrocodeReleaseModel));
                if (configModel.TransStatus == TransactionStatus.Completed.ToString())
                {
                    if (null == m.ReleaseLees.FirstOrDefault<ReleaseLee>(x => x.ReleaseTo == model.SelectedReleaseLee.ReleaseTo))
                    {
                        m.AddReleaseLee(model.SelectedReleaseLee.ReleaseTo);
                        add = true;
                    }
                }
            }
            if (null != m && add)
            {
                System.IO.File.WriteAllText(releaseRequestFileName, m.ToString());
            }
        }
        private void submitRequestProcess(string configJsonWorksheet,
                                          MicrocodeReleaseModel model,
                                          string workingdirectory,
                                          string jsonRequest)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = workingdirectory;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            var userName = WebConfigurationManager.AppSettings["MicrocodeReleaseRepoUserName"];
            var parameterFormat = WebConfigurationManager.AppSettings["MicrocodeReleaseCommandlineFormat"];
            var commadParameters = string.Empty;
            if (string.IsNullOrEmpty(userName))
            {
                commadParameters = string.Format(parameterFormat.Replace(@"--user {2}", string.Empty),
                                                 jsonRequest,
                                                 configJsonWorksheet);
            }
            else
            {
                commadParameters = string.Format(parameterFormat, jsonRequest, configJsonWorksheet, userName);
            }
            startInfo.FileName = "python";
            var argus = string.Format(@"McuReleaseAutomation.py {0}", commadParameters);
            startInfo.Arguments = argus;
            startInfo.Verb = "runas";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;

            Directory.SetCurrentDirectory(workingdirectory);
            var process = new Process() { StartInfo = startInfo };
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            this._sbErrorReceivedBuffer.Clear();
            process.Start();
            process.BeginErrorReadLine();
            // process.BeginOutputReadLine();
            //WindowHelper.BringProcessToFront(process);
            if (null != process && !process.HasExited)
            {
                process.WaitForExit();
            }
        }
        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            this._sbErrorReceivedBuffer.AppendLine(e.Data);
            MvcApplication.Logger.ErrorFormat(@"Microcode release --- receive error data :{0}", e.Data);
        }
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            MvcApplication.Logger.DebugFormat(@"Microcode release --- receive error data :{0}", e.Data);
        }
        private Tuple<string, string> _formatedJsonFiels
        {
            get
            {
                var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
                if (null == model)
                    return null;
                var sessionId = System.Web.HttpContext.Current.Session.SessionID;
                var workingdir = WebConfigurationManager.AppSettings["MicrocodeReleaseWorkingDirectory"];
                var jsonRequest = string.Format(@"{0}\McuReleaseRequest_{1}_{2}.json", workingdir, model.ReleaseMcuSource, sessionId);
                var configJsonWorksheet = string.Format(@"{0}\McuRelease_{1}_{2}.json", workingdir, model.ReleaseMcuSource, sessionId);
                return new Tuple<string, string>(jsonRequest, configJsonWorksheet);
            }
        }
        private WorkflowConfigModel _GetConfigModel(string config)
        {
            if (!System.IO.File.Exists(config) || config.IsFileLocked())
            {
                return null;
            }
            WorkflowConfigModel configModel = null;
            try
            {
                var fileInfo = new FileInfo(config);
                if (fileInfo.Length > 0) {
                    using (StreamReader file = System.IO.File.OpenText(config))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        configModel = (WorkflowConfigModel)serializer.Deserialize(file, typeof(WorkflowConfigModel));
                        configModel.SessionId = System.Web.HttpContext.Current.Session.SessionID;
                        _configModelCache = configModel.DeepClone<WorkflowConfigModel>();
                    }
                }
                else if (null != _configModelCache)
                {
                    configModel = _configModelCache;
                    var json = configModel.ToString();
                    System.IO.File.WriteAllText(config, json);
                }
            }
            catch{ }
            return configModel;
        }
        private MicrocodeReleaseModel _loadDefaultReleaseRequest()
        {
            MicrocodeReleaseModel model;
            var releaseRequestFileName = Path.Combine(Server.MapPath(@"~/App_Data/"), "McuReleaseRequestTemplate.json");
            using (StreamReader file = System.IO.File.OpenText(releaseRequestFileName))
            {
                var serializer = new JsonSerializer();
                model = (MicrocodeReleaseModel)serializer.Deserialize(file, typeof(MicrocodeReleaseModel));
                model.Title = "Microcode Release";
            }

            return model;
        }
        private void _UpdateSessionModel(MicrocodeReleaseModel model)
        {
            if (null == model)
            {
                return;
            }
            lock (_updateModelSyncObj)
            {
                for (int i = 0; i < 3; ++i)
                {
                    try
                    {
                        try
                        {
                            Session.Remove(Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY);
                        }
                        catch { }
                        Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] = model;
                    }
                    catch
                    {
                    }
                }
            }
        }
        private void _onReadmeFChanged(object sender, FileSystemEventArgs e)
        {
            var workingdirectory = WebConfigurationManager.AppSettings["MicrocodeReleaseWorkingDirectory"];
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = false;
            startInfo.FileName = "python";
            var argus = string.Format(@"RefreshReadmeJson.py");
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
        private void _SyncMcu(MicrocodeReleaseModel model, ReleaseLee releaseLee, McuRelease mcu)
        {
            releaseLee.UpdateMcu(mcu);
            model.UpdateReleaseLee(releaseLee);
            model.UpdateVolunteerInputs();
        }
        private void _RetriveHsdMcuArticalIDs(MicrocodeReleaseModel model)
        {
            try
            {
                var hsdMcuAricleMapJsonPath = WebConfigurationManager.AppSettings["HsdMcuAricleMapJsonPath"];
                var sb = new StringBuilder();
                using (StreamReader file = System.IO.File.OpenText(hsdMcuAricleMapJsonPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var hsdMcuArticleMap = (HsdMcuArticleIdMap)serializer.Deserialize(file, typeof(HsdMcuArticleIdMap));
                    foreach (var item in hsdMcuArticleMap.HsdArticle)
                    {
                        sb.Append(string.Format(@"{0},", item[0]));
                    }
                    var idsString = sb.ToString().TrimEnd(',');
                    model.HsdArticleIDs = idsString;
                    this._UpdateSessionModel(model);
                }
            }
            catch { }
        }
        #endregion
        private void _onHsdMapFilewatcherChanged(object sender, FileSystemEventArgs e)
        {
            if (e.Name == "HsdMcuMap.json")
            {
                if (!e.FullPath.IsFileLocked() && null != Session)
                {
                    lock (this)
                    {
                        var model = Session[Definitions.MICROCODE_RELEASE_ORCHSTRATION_KEY] as MicrocodeReleaseModel;
                        if (null != model)
                        {
                            this._RetriveHsdMcuArticalIDs(model);
                        }
                    }
                }
            }
        }
    }

    public static class WindowHelper
    {
        public static void BringProcessToFront(Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }

            SetForegroundWindow(handle);
        }

        const int SW_RESTORE = 9;

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);
    }
}