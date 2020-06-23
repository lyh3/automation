using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.IO.Compression;
using System.Web.Configuration;
using Newtonsoft.Json;
using WindowService.DataModel;
using IntelDCGSpsWebService.Models;

namespace IntelDCGSpsWebService.Controllers
{
    public class BkcImageParserController : Controller
    {
        private string bkcImageAutomationPath = WebConfigurationManager.AppSettings["FDBinAutomationPath"]; 
        private static object _updateModelSyncObj = new object();
        private static object _invokeSyncObj = new object();

        public ActionResult Index()
        {
            var model = Session[Definitions.BKC_IMAGE_PARSER_KEY] as BkcImageParserModel;
            if (null == model)
            {
                model = new BkcImageParserModel(bkcImageAutomationPath)
                {
                    Title = "BKC Image BIOS Parser"
                };
                _UpdateSessionModel(model);
            } 
            return View(model);
        }

        public ActionResult UpdateSelectedViewType(JsonContainer jsonData)
        {
            var model = Session[Definitions.BKC_IMAGE_PARSER_KEY] as BkcImageParserModel;
            model.SelectedParserViewType = jsonData.JsonString;
            Session[Definitions.BKC_IMAGE_PARSER_KEY] = model;
            return Json(model);
        }

        public ActionResult UpdateSelectedConfigJson(JsonContainer jsonData)
        {
            var model = Session[Definitions.BKC_IMAGE_PARSER_KEY] as BkcImageParserModel;
            if (null != jsonData && jsonData.JsonString != "N/A")
            {
                model.SelectedEmbeddedJson = jsonData.JsonString;
                _UpdateSessionModel(model);
            }
            return Json(model);
        }


        [HttpPost]
        public ActionResult UploadIfwiImages(HttpPostedFileBase ifwiImage,
                                             HttpPostedFileBase userSelectedJson)
        {
            try
            {
                lock (_invokeSyncObj)
                {
                    lock (this)
                    {
                        const string downFolder = @"~/App_Data/Download";
                        const string BKCBINARIES_FOLDER = "BKCBinaries";
                        var model = Session[Definitions.BKC_IMAGE_PARSER_KEY] as BkcImageParserModel;
                        model.ProcessFailed = true;
                        model.ResultsFile = string.Empty;
                        var userJsonPath = string.Empty;

                        var workdir = @"c:/Temp";// Server.MapPath(uploadFolder);
                        var configJson = model.SelectedConfigJson;
                        if (!(null == userSelectedJson || userSelectedJson.ContentLength == 0))
                        {
                            configJson = Path.Combine(workdir, Path.GetFileName(userSelectedJson.FileName));
                            userSelectedJson.SaveAs(configJson);
                        }
                        var jsonPath = Path.Combine(bkcImageAutomationPath, configJson);
                        var configText = string.Empty;
                        var inputVal = string.Empty;
                        var outputVal = string.Empty;
                        using (StreamReader file = System.IO.File.OpenText(jsonPath))
                        {
                            var serializer = new JsonSerializer();
                            var fdbinConfig = (FDBinConfig)serializer.Deserialize(file, typeof(FDBinConfig));
                            inputVal = fdbinConfig.IfwiBuild.Input;
                            outputVal = fdbinConfig.IfwiBuild.BKCBinaries;
                            configText = System.IO.File.ReadAllText(jsonPath);
                            configText = configText.Replace(inputVal, workdir).Replace(outputVal, workdir).Replace(@"\", "/");
                        }
                        configJson = Path.Combine(workdir, Path.GetFileName(configJson));
                        System.IO.File.WriteAllText(configJson, configText);

                        var commadParameters = string.Format(@" -j {0}", configJson);

                        var isValidPost = !(null == ifwiImage || ifwiImage.ContentLength == 0);
                        if (isValidPost)
                        {
                            var zipPath = Path.Combine(Server.MapPath(downFolder), Path.GetFileNameWithoutExtension(ifwiImage.FileName));
                            zipPath = zipPath + ".zip";
                            if (System.IO.File.Exists(zipPath))
                            {
                                System.IO.File.Delete(zipPath);
                            }
                            _UpdateSessionModel(model);
                            try
                            {
                                var outputFolder = Path.Combine(workdir, BKCBINARIES_FOLDER);
                                if (Directory.Exists(outputFolder))
                                    Directory.Delete(outputFolder, true);
                                var sourcePath = string.IsNullOrEmpty(workdir) ?
                                        Path.Combine(bkcImageAutomationPath, Path.GetFileName(ifwiImage.FileName))
                                    : Path.Combine(workdir, Path.GetFileName(ifwiImage.FileName));
                                ifwiImage.SaveAs(sourcePath);
                                model.UploadedFile = ifwiImage.FileName;
                                var startInfo = new ProcessStartInfo();
                                startInfo.UseShellExecute = false;
                                startInfo.CreateNoWindow = true;
                                startInfo.WorkingDirectory = bkcImageAutomationPath;
                                startInfo.FileName = @"C:\Python27\python.exe";
                                var argus = string.Format(@"bkc_image_automation.py {0}", commadParameters);
                                startInfo.Arguments = argus;
                                var process = new Process() { StartInfo = startInfo };
                                process.Start();
                                Thread.Sleep(40000);

                                if (Directory.Exists(outputFolder))
                                {
                                    ZipFile.CreateFromDirectory(outputFolder, zipPath);
                                    if (System.IO.File.Exists(zipPath))
                                    {
                                        model.ResultsFile = zipPath;
                                        ViewBag.Message = "Image parsing successfully!";
                                    }
                                    Directory.Delete(outputFolder, true);
                                }
                            }
                            catch (Exception) { }
                            finally
                            {
                                if (System.IO.File.Exists(configJson))
                                {
                                    System.IO.File.Delete(configJson);
                                }
                            }
                            model.ProcessFailed = !model.IsDownloadAvailable;
                            this._UpdateSessionModel(model);
                        }
                    }
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        public FileResult DownloadSpiImage()
        {
            var model = (BkcImageParserModel)Session[Definitions.BKC_IMAGE_PARSER_KEY];
            if (null == model || string.IsNullOrEmpty(model.ResultsFile) || !System.IO.File.Exists(model.ResultsFile))
                return null;

            var fileName = model.ResultsFile;
            byte[] buffer = System.IO.File.ReadAllBytes(fileName);

            Session[Definitions.BIOS_IMAGE_OPERATIONS_KEY] = model;

            return File(buffer, System.Net.Mime.MediaTypeNames.Application.Octet, System.IO.Path.GetFileName(fileName));
        }

        [HttpPost]
        public ActionResult Reset()
        {
            Session[Definitions.BKC_IMAGE_PARSER_KEY] = null;
            return RedirectToAction("Index");
        }

        private void _UpdateSessionModel(BkcImageParserModel model)
        {
            lock (_updateModelSyncObj)
            {
                for (int i = 0; i < 3; ++i)
                {
                    try
                    {
                        try
                        {
                            Session.Remove(Definitions.BKC_IMAGE_PARSER_KEY);
                        }
                        catch { }
                        Session[Definitions.BKC_IMAGE_PARSER_KEY] = model;
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}