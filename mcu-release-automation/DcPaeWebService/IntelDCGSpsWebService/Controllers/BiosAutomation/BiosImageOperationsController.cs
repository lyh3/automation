using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Diagnostics;
using System.Threading;
using WindowService.DataModel;
using IntelDCGSpsWebService.Models;

namespace IntelDCGSpsWebService.Controllers
{
    public class BiosImageOperationsController : Controller
    {
        private static object _updateModelSyncObj = new object();
        private static object _invokeSyncObj = new object();

        public ActionResult Index()
        {
            var model = Session[Definitions.BIOS_IMAGE_OPERATIONS_KEY] as BiosImageOperationsModel;
            if (null == model)
            {
                model = new BiosImageOperationsModel { Title = "BIOS Image Operations" };
                Session[Definitions.BIOS_IMAGE_OPERATIONS_KEY] = model;
            }

            return View(model);
        }

        public ActionResult UpdateOperationType(JsonContainer jsonData)
        {
            var model = (BiosImageOperationsModel)Session[Definitions.BIOS_IMAGE_OPERATIONS_KEY];
            if (null != model
                && null != jsonData
                && !string.IsNullOrEmpty(jsonData.JsonString))
            {
                model.OperationType = jsonData.JsonString;
                this._UpdateSessionModel(model);
            }
            return Json(model);
            //return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Reset()
        {
            Session[Definitions.BIOS_IMAGE_OPERATIONS_KEY] = null;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadSpiImages(HttpPostedFileBase ifwiImage,
                                            HttpPostedFileBase biosImage,
                                            string outputName)
        {
            const string uploadFolder = @"~/App_Data/Upload";
            const string downFolder = @"~/App_Data/Download";
            var isValidPost = !(null == ifwiImage || ifwiImage.ContentLength == 0);
            var model = Session[Definitions.BIOS_IMAGE_OPERATIONS_KEY] as BiosImageOperationsModel;
            if(null != model)
            {
                if(model.OperationType != BiosImageOperationType.ExtractBios.ToString())
                {
                    isValidPost &= !(null == biosImage || biosImage.ContentLength == 0);
                }
            }
            if (isValidPost)
            {
                try 
                {
                    var file = Path.Combine(Server.MapPath(uploadFolder), outputName);
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                    Dictionary<BiosOperationFileType, string> uploadedFiles = new Dictionary<BiosOperationFileType, string>();
                    var ifwiPath = Path.Combine(Server.MapPath(uploadFolder), Path.GetFileName(ifwiImage.FileName));
                    ifwiImage.SaveAs(ifwiPath);
                    uploadedFiles.Add(BiosOperationFileType.IfwiImage, ifwiImage.FileName);
                    if (null != biosImage)
                    {
                        biosImage.SaveAs(Path.Combine(Server.MapPath(uploadFolder), Path.GetFileName(biosImage.FileName)));
                        uploadedFiles.Add(BiosOperationFileType.BiosImage, biosImage.FileName);
                    }
                    else
                    {
                        uploadedFiles.Add(BiosOperationFileType.BiosImage, string.Empty);
                    }
                    model.UploadedFiles = uploadedFiles;
                    model.IfwiImage = model.UploadedFiles[BiosOperationFileType.IfwiImage];
                    model.BiosImage = model.UploadedFiles[BiosOperationFileType.BiosImage];
                    model.OutputName = outputName;
                    model.ProcessFailed = true;
                    this._UpdateSessionModel(model);

                    var commadParameters = model.ComposedCommandParameters;
                    var startInfo = new ProcessStartInfo();
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    startInfo.WorkingDirectory = Server.MapPath(uploadFolder);
                    startInfo.FileName = @"C:\Python27\python.exe";
                    var argus = string.Format(@"BiosImageOperationEx.py {0}", commadParameters);
                    startInfo.Arguments = argus;
                    var process = new Process() { StartInfo = startInfo };
                    process.Start();
                    Thread.Sleep(3000);

                    if (System.IO.File.Exists(file))
                    {
                        var resultsFile = Path.Combine(Server.MapPath(downFolder), outputName);
                        if (System.IO.File.Exists(resultsFile))
                        {
                            System.IO.File.Delete(resultsFile);
                        }
                        System.IO.File.Copy(file, resultsFile);
                        System.IO.File.Delete(file);
                        model.ResultsFile = resultsFile;
                        model.IfwiImage = uploadedFiles[BiosOperationFileType.IfwiImage];
                        model.BiosImage = uploadedFiles[BiosOperationFileType.BiosImage];
                        ViewBag.Message = "Image merge successfully!!";
                    }
                }
                catch { }
                model.ProcessFailed = !model.IsDownloadAvailable;
                this._UpdateSessionModel(model);
            }
            return RedirectToAction("Index");
        }

        public FileResult DownloadSpiImage()
        {
            var model = (BiosImageOperationsModel)Session[Definitions.BIOS_IMAGE_OPERATIONS_KEY];
            if (null == model || string.IsNullOrEmpty(model.ResultsFile) || !System.IO.File.Exists(model.ResultsFile))
                return null;

            var fileName = model.ResultsFile;
            byte[] buffer = System.IO.File.ReadAllBytes(fileName);

            Session[Definitions.BIOS_IMAGE_OPERATIONS_KEY] = model;

            return File(buffer, System.Net.Mime.MediaTypeNames.Application.Octet, System.IO.Path.GetFileName(fileName));
        }

        private void _UpdateSessionModel(BiosImageOperationsModel model)
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
                        Session[Definitions.BIOS_IMAGE_OPERATIONS_KEY] = model;
                    }
                    catch
                    {
                    }
                }
            }
        }

    }
}