using System;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using WindowService.DataModel;
using IntelDCGSpsWebService.Models;

namespace IntelDCGSpsWebService.Controllers
{
    public class SmartConfigController : Controller
    {
        private static object _updateModelSyncObj = new object();
        private static object _syncObj = new object();
        private static ViewDataDictionary _viewDataDictionary = new ViewDataDictionary();

        public ActionResult Index()
        {
            var model = Session[Definitions.SMART_CONFIG_SESSION_KEY] as SmartConfigDataModel;
            if (null == model)
            {
                using (model = new SmartConfigDataModel())
                {
                    this._UpdateSessionModel(model);
                }
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult LoadJsonConfig(HttpPostedFileBase jsonConfig)
        {
            if (null != jsonConfig && jsonConfig.ContentLength > 0)
            {
                SmartConfigDataModel model = new SmartConfigDataModel();
                try
                {
                    const string uploadFolder = @"~/App_Data/Upload";
                    var fileInfo = new FileInfo(jsonConfig.FileName);
                    var jsonFile = Path.Combine(Server.MapPath(uploadFolder),
                                                string.Format(@"{0}_{1}" + fileInfo.Extension,
                                                fileInfo.Name.Replace(fileInfo.Extension, string.Empty),
                                                System.Web.HttpContext.Current.Session.SessionID));
                    jsonConfig.SaveAs(jsonFile);

                    using (StreamReader file = System.IO.File.OpenText(jsonFile))
                    {
                        var serializer = new JsonSerializer();

                        model = (SmartConfigDataModel)serializer.Deserialize(file, typeof(SmartConfigDataModel));
                        if (model.Root.Count == 0)
                        {
                            model.LastErrorMessage = string.Format(@"-- There is no node been loaded, please verify the json file [{0}] is valid. ---", fileInfo.FullName);
                            this._UpdateSessionModel(model);
                            return RedirectToAction("Index");
                        }
                        model.BuildTree();
                        model.ConfigLoaded = true;
                        model.JsonConfig = jsonFile;
                        this._UpdateSessionModel(model);
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = string.Format(@"SmartConfig - Excepton caught at LoadJsonConfig", ex.Message);
                    model.LastErrorMessage = errorMsg;
                    MvcApplication.Logger.Error(errorMsg);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult GetSubNode(JsonContainer jsonData)
        {
            var model = Session[Definitions.SMART_CONFIG_SESSION_KEY] as SmartConfigDataModel;
            if (!string.IsNullOrEmpty(jsonData.JsonString) && null != model)
            {
                if (jsonData.JsonString.StartsWith("N/A"))
                {
                    return Json(new JsonContainer { JsonString = "<div/>" });
                }
                var node = model.GetConfigNodeByJPath(jsonData.JsonString);
                if (null != node)
                {
                    var subNodeHtml = _RenderToHtmlString(PartialView("_ConfigTreeNode", node));
                    var idx = jsonData.JsonString.LastIndexOf('.');
                   if (-1 != idx)
                    {
                        var parentNode = model.GetConfigNodeByJPath(jsonData.JsonString.Substring(0, idx));
                        if (null != parentNode)
                        {
                            node.CurrentSelectedSubPath = jsonData.JsonString;
                        }
                    }
                    var subNodeInfo = new SubnodeInfo();
                    subNodeInfo.SubnodeHtml = subNodeHtml;
                    subNodeInfo.SelectedNextSubNodePath = node.CurrentSelectedSubPath;
                    this._UpdateSessionModel(model);
                    return Json(subNodeInfo);
                }
            }
            return null;
        }
        [HttpPost]
        public JsonResult UpdateNode(JsonContainer jsonData)
        {
            var model = Session[Definitions.SMART_CONFIG_SESSION_KEY] as SmartConfigDataModel;
            ConfigTreeNode node = null;
            if (null != model)
            {
                node = model.GetConfigNodeByGuid(jsonData.JsonString);
                if (null != node && node.NodeEditStatus == ConfigNodeStatus.Modified.ToString())
                {
                    node.NodeEditStatus = ConfigNodeStatus.Updated.ToString();
                    this._UpdateSessionModel(model);
                }
            }
            return Json("N/A");
        }
        [HttpPost]
        public JsonResult GetNodeEditStatus(JsonContainer jsonData)
        {
            var statusInfo = new ModelEdiStatusInfo();
            var model = Session[Definitions.SMART_CONFIG_SESSION_KEY] as SmartConfigDataModel;
            if (null != model && null != jsonData && !string.IsNullOrEmpty(jsonData.JsonString))
            {
                statusInfo.ConfigLoaded = model.ConfigLoaded;

                var ids = jsonData.JsonString.TrimEnd(',').Split(',');

                foreach (var id in ids)
                {
                    var node = model.GetConfigNodeByGuid(id);
                    if (null != node)
                    {
                        statusInfo.Status.Add(node.NodeEditStatus);
                        statusInfo.CurrentSelectedSubPath.Add(node.CurrentSelectedSubPath);
                        statusInfo.RawDataEditType.Add(node.RawDataMap.EditType);
                        statusInfo.RawDataClass.Add(node.RawDataMap.RawDataClass);
                        statusInfo.LastErrorMessage = model.LastErrorMessage;
                        statusInfo.PropertiesDefaultValue.Add(node.Properties.Default);
                        statusInfo.PropertiesCurrentValue.Add(node.Properties.CurrentValue);
                        statusInfo.RawDataOffset.Add(node.RawDataMap.Offset);
                        statusInfo.RawDataSize.Add(node.RawDataMap.Size);
                        statusInfo.RawDataValue.Add(node.RawDataMap.Value);
                    }
                    else
                    {
                        statusInfo.Status.Add(ConfigNodeStatus.Idle.ToString());
                    }
                }
            }
            return new JsonResult { Data = statusInfo, MaxJsonLength = 86753090 };  //Json(statusInfo);
        }
        [HttpPost]
        public ActionResult LoadBinary(HttpPostedFileBase binarySource)
        {
            var model = Session[Definitions.SMART_CONFIG_SESSION_KEY] as SmartConfigDataModel;
            if (null != binarySource && binarySource.ContentLength > 0 && null != model)
            {
                model.LastErrorMessage = string.Empty;
                model.TargetBinaryFile = string.Empty;
                this._UpdateSessionModel(model);
                try
                {
                    var fileInfo = new FileInfo(binarySource.FileName);
                    if (!(fileInfo.Extension.ToLower() == ".bin" || fileInfo.Extension.ToLower() == ".rom"))
                    {
                        model.LastErrorMessage = string.Format(@"The file [{0}] is not supported binary file.", fileInfo.Name);
                        this._UpdateSessionModel(model);
                        return RedirectToAction("Index");
                    }
                    const string uploadFolder = @"~/App_Data/Upload";
                    var binaryFile = Path.Combine(Server.MapPath(uploadFolder), 
                                                  string.Format(@"{0}_{1}" + fileInfo.Extension,
                                                  fileInfo.Name.Replace(fileInfo.Extension, string.Empty), 
                                                  System.Web.HttpContext.Current.Session.SessionID));
                    binarySource.SaveAs(binaryFile);
                    var isAscii = true;
                    var buffer = new byte[1024];
                    using (FileStream fs = System.IO.File.OpenRead(binaryFile))
                    {                        
                        while (fs.Read(buffer, 0, buffer.Length) > 0 && isAscii)
                        {
                            foreach(var b in buffer)
                            {
                                isAscii &= b <= 127;
                                if (!isAscii)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (isAscii)
                    {
                        model.LastErrorMessage = string.Format(@"The file [{0}] is not supported binary file.", fileInfo.Name);
                        this._UpdateSessionModel(model);
                        return RedirectToAction("Index");
                    }

                    if (null != model)
                    {
                        model.LastErrorMessage = string.Empty;
                        model.TargetBinaryFile = binaryFile;
                        this._UpdateSessionModel(model);
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = String.Format(@"SmartConfig - Excepton caught at LoadBinary, error = {0}", ex.Message);
                    if(null != model)
                    {
                        model.LastErrorMessage = errorMsg;
                        this._UpdateSessionModel(model);
                    }
                    MvcApplication.Logger.Error(errorMsg);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Reset()
        {
            var model = Session[Definitions.SMART_CONFIG_SESSION_KEY] as SmartConfigDataModel;
            if(null != model)
            {
                model.LastErrorMessage = string.Empty;
                model.TargetBinaryFile = string.Empty;
                this._UpdateSessionModel(model);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Apply(HttpPostedFileBase binaryImage)
        {
            var model = Session[Definitions.SMART_CONFIG_SESSION_KEY] as SmartConfigDataModel;
            if (null != model)
            {
                model.TransactionStatus(ConfigNodeStatus.Updated, ConfigNodeStatus.Applied);
                this._UpdateSessionModel(model);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult UpdateRawDataContents(JsonContainer jsonData)
        {
            var model = Session[Definitions.SMART_CONFIG_SESSION_KEY] as SmartConfigDataModel;
            ConfigTreeNode node = null;
            var jsonContainer = new JsonContainer();
            if (null != model)
            {
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var updateInfo = serializer.Deserialize<RawDataMapInfo>(reader);
                node = model.GetConfigNodeByGuid(updateInfo.Id);
                if (null != node)
                {
                    if (!string.IsNullOrEmpty(updateInfo.RawDataMap.Offset))
                    {
                        node.RawDataMap.Offset = updateInfo.RawDataMap.Offset;
                        node.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                    }
                    if (!string.IsNullOrEmpty(updateInfo.RawDataMap.Size))
                    {
                        node.RawDataMap.Size = updateInfo.RawDataMap.Size;
                        node.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                    }
                    if (!string.IsNullOrEmpty(updateInfo.RawDataMap.Value))
                    {
                        var val = updateInfo.RawDataMap.Value;
                        if (val == EditDataType.DataSizeEdit.ToString()
                            || val == EditDataType.RawDataEdit.ToString())
                        {
                            node.RawDataMap.EditType = val;
                        }
                        else
                        {
                            node.RawDataMap.Value = val;
                        }
                        node.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                    }

                    jsonContainer.JsonString = _RenderToHtmlString(PartialView("_ConfigTreeNodeContents", node));
                }
                this._UpdateSessionModel(model);

            }
            return Json(jsonContainer);
        }
        [HttpPost]
        public JsonResult UpdatePropertiesContents(JsonContainer jsonData)
        {
            var model = Session[Definitions.SMART_CONFIG_SESSION_KEY] as SmartConfigDataModel;
            var jsonContainer = new JsonContainer();
            if (null != model)
            {
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(jsonData.JsonString));
                var updateInfo = serializer.Deserialize<PropertiesInfo>(reader);
                var node = model.GetConfigNodeByGuid(updateInfo.Id);
                if (null != node)
                {
                    if (!string.IsNullOrEmpty(updateInfo.Properties.pType))
                    {
                        node.Properties.pType = updateInfo.Properties.pType;
                        node.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                    }
                    if (!string.IsNullOrEmpty(updateInfo.Properties.Default))
                    {
                        node.Properties.Default = updateInfo.Properties.Default;
                        node.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                    }
                    if (!string.IsNullOrEmpty(updateInfo.Properties.CurrentValue))
                    {
                        node.Properties.CurrentValue = updateInfo.Properties.CurrentValue;
                        node.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                    }
                    if (!string.IsNullOrEmpty(updateInfo.Properties.SelectedValue))
                    {
                        node.Properties.CurrentValue = updateInfo.Properties.SelectedValue;
                        node.NodeEditStatus = ConfigNodeStatus.Modified.ToString();
                    }
                    jsonContainer.JsonString = _RenderToHtmlString(PartialView("_ConfigTreeNodeContents", node));
                }
                this._UpdateSessionModel(model);

            }
            return Json(jsonContainer);
        }
        private string _RenderToHtmlString(PartialViewResult partialView)
        {
            var httpContext = System.Web.HttpContext.Current;
            if (httpContext == null)
            {
                throw new NotSupportedException("An HTTP context is required to render the partial view to a string");
            }
            var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory().CreateController(httpContext.Request.RequestContext, controllerName);
            var controllerContext = new ControllerContext(httpContext.Request.RequestContext, controller);
            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    view.Render(new ViewContext(controllerContext, view, partialView.ViewData, partialView.TempData, tw), tw);
                }
            }
            return sb.ToString();
        }
        public FileResult DownloadSpiImage()
        {
            var model = Session[Definitions.SMART_CONFIG_SESSION_KEY] as SmartConfigDataModel;
            if (null == model || string.IsNullOrEmpty(model.ResultsFile) || !System.IO.File.Exists(model.ResultsFile))
                return null;

            var fileName = model.ResultsFile;
            byte[] buffer = System.IO.File.ReadAllBytes(fileName);

            Session[Definitions.BIOS_IMAGE_OPERATIONS_KEY] = model;

            return File(buffer, System.Net.Mime.MediaTypeNames.Application.Octet, System.IO.Path.GetFileName(fileName));
        }
        private void _UpdateSessionModel(SmartConfigDataModel model)
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
                            Session.Remove(Definitions.SMART_CONFIG_SESSION_KEY);
                        }
                        catch { }
                        Session[Definitions.SMART_CONFIG_SESSION_KEY] = model;
                    }
                    catch(Exception ex)
                    {
                        model.LastErrorMessage = ex.Message;
                        MvcApplication.Logger.Error(ex.Message);
                    }
                }
            }
        }

    }
}