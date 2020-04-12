using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using System.Threading;

using Newtonsoft.Json;
using IntelDCGSpsWebService.Models;
using IntelDCGSpsWebService.Models.Buildingblocks;
using WindowService.DataModel;

namespace IntelDCGSpsWebService.Controllers
{
    public class SpsToolsController : Controller
    {
        const string uploadFolder = @"~/App_Data/Upload";
        const string downFolder = @"~/App_Data/Download";
        public ActionResult Index(SpsToolsType selectedTool)
        {
            var error = string.Empty;
            var model = this._factoryDataModel(selectedTool, error);
            return RedirectToAction(selectedTool.ToString());
        }

        #region HealthEventDecode

        public ActionResult HealthEventDecode()
        {
            var model = Session[Definitions.SPS_TOOLS_MODEL_KEY] as HealthEventViewModel;
            return View(model);
        }

        public ActionResult DecodeHealthEvent(JsonDataContener jsonData)
        {
            var sb = new StringBuilder();
            var regex = new Regex("^[0-9a-fA-F]{6}$");
            var errorMessage = string.Empty;

            if (null == jsonData || string.IsNullOrEmpty(jsonData.JsonString) || !regex.Match(jsonData.JsonString).Success)
                this._ComposeErrorHtml("Please enter a valid health event data (must be 6 Hex digits [0-9, a-f or A-F] string and no white space between) and try again.", sb);
            else
            {
                var itemformat = "value code - {0} : description - {1}";
                var model = Session[Definitions.SPS_TOOLS_MODEL_KEY] as HealthEventViewModel;
                var results = utils.DecodeFromJson(jsonData.JsonString, model, itemformat);
                if (null == results)
                    this._ComposeErrorHtml(@"Session exired/failed, please press F5 to refresh page and try again.", sb);
                else
                {
                    sb.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
                    sb.Append("<body>");
                    sb.Append("<table cellspacing='0' cellpadding='0' border='0' style='width: 96%; aling:center;margin-left:20px;'>");
                    sb.Append("<tr style='border:none;'><td>&nbsp;</td></tr>");
                    sb.Append("<tr style='border:none;'>");
                    sb.Append("<th colspan='8' style='border:1px; width:100%; height: 40px;background-color:steelblue; font-family:arial,helvetica; font-size:Large; font-weight:bold; color:white;text-align:center;border-radius:5px;'>");
                    sb.Append("    ME Health Event Decoding Results");
                    sb.Append("</th>");
                    sb.Append("</tr>");
                    sb.Append("</table>");
                    sb.Append("<table cellspacing='1' border='1' style='width: 96%; border-color:silver;border-radius:5px;align:center;margin-left:20px;'>");
                    sb.Append("<tr style='height: 28px;background-color:silver;font-weight: bold;font-size:smaller;font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;text-align:center;color: black'>");
                    sb.Append("<td style='width:40%;'>Value Code</td>");
                    sb.Append("<td style='width:60%;'>description</td>");
                    sb.Append("</tr>");
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        var toggle = true;
                        foreach (var line in results)
                        {
                            var bkcolor = "silver";
                            if (toggle)
                                bkcolor = "white";

                            var split = line.Split(':');
                            sb.Append(string.Format("<tr style='height: 28px;background-color:{0};font-size:small;font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;text-align:center;color:black'>", bkcolor));
                            sb.Append(string.Format("<td style='width:40%;'>{0}</td>", split[0].Trim()));
                            sb.Append(string.Format("<td style='width:60%;'>{0}</td>", split[1].Trim()));
                            sb.Append("</tr>");
                            toggle = !toggle;
                        }
                        errorMessage = string.Empty;
                    }
                    else
                        sb.Append(errorMessage);
                    sb.Append("</table>");
                    sb.Append("</body>");
                    sb.Append("</html>");
                }
            }
            var htmlContent = sb.ToString();

            return Json(new { html = htmlContent, error = errorMessage });
        }

        private void _ComposeErrorHtml(string errorMessage, StringBuilder sb)
        {
            sb.Clear();
            sb.Append("<div>&nbsp;</dev>");
            sb.Append("<div id='divErrorMessage' class='alert alert-dismissible alert-danger' style='width:98%;margin-left:10px;'>");
            sb.Append(string.Format("<span id='spError'>{0}</span>", errorMessage));
            sb.Append("</div>");
        }


        #endregion

        #region SpiImageMerge

        public ActionResult SpiImageMerge()
        {
            var model = Session[Definitions.SPS_TOOLS_MODEL_KEY] as SpiImageMergeModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult UploadSpiImages(HttpPostedFileBase sourceImage,
                                            HttpPostedFileBase regionImage,
                                            HttpPostedFileBase mapFile,
                                            string regionName,
                                            string outputName)
        {
            const string uploadFolder = @"~/App_Data/Upload";
            const string downFolder = @"~/App_Data/Download";
            var model = Session[Definitions.SPS_TOOLS_MODEL_KEY] as SpiImageMergeModel;
            model.RegionName = regionName;
            model.OutputName = outputName;
            if (!(null == sourceImage || sourceImage.ContentLength == 0
                || null == regionImage || regionImage.ContentLength == 0
                || null == mapFile || mapFile.ContentLength == 0
                || string.IsNullOrEmpty(regionName)
                || string.IsNullOrEmpty(outputName)))
            {
                Dictionary<SpiMergeFileType, string> uploadedFiles = new Dictionary<SpiMergeFileType, string>();
                model.RegionName = regionName;
                model.OutputName = outputName;
                var resultsFile = Path.Combine(Server.MapPath(downFolder), outputName);
                try
                {
                    sourceImage.SaveAs(Path.Combine(Server.MapPath(uploadFolder), Path.GetFileName(sourceImage.FileName)));
                    uploadedFiles.Add(SpiMergeFileType.SourceImage, sourceImage.FileName);
                    regionImage.SaveAs(Path.Combine(Server.MapPath(uploadFolder), Path.GetFileName(regionImage.FileName)));
                    uploadedFiles.Add(SpiMergeFileType.RegionImage, regionImage.FileName);
                    mapFile.SaveAs(Path.Combine(Server.MapPath(uploadFolder), Path.GetFileName(mapFile.FileName)));
                    uploadedFiles.Add(SpiMergeFileType.MapFile, mapFile.FileName);
                    model.UploadedFiles = uploadedFiles;

                    var commadParameters = string.Format(@" -s {5}{0}{5} -u {5}{1}{5} -o {5}{2}{5} -m {5}{3}{5} -r {5}{4}{5}",
                                                        uploadedFiles[SpiMergeFileType.SourceImage],
                                                        uploadedFiles[SpiMergeFileType.RegionImage],
                                                        resultsFile,
                                                        uploadedFiles[SpiMergeFileType.MapFile],
                                                        model.RegionName,
                                                        '"');
                    var startInfo = new ProcessStartInfo();
                    startInfo.UseShellExecute = true;
                    startInfo.CreateNoWindow = true;
                    startInfo.WorkingDirectory = Server.MapPath(uploadFolder);
                    startInfo.FileName = "python";
                    var argus = string.Format("ImageRegionUpdate.py {0}", commadParameters);
                    startInfo.Arguments = argus;
                    var process = new Process() { StartInfo = startInfo };
                    process.Start();

                    Thread.Sleep(3000);

                    if (System.IO.File.Exists(resultsFile))
                    {
                        model.ResultsFile = resultsFile;
                        model.SourceImage = uploadedFiles[SpiMergeFileType.SourceImage];
                        model.RegionImage = uploadedFiles[SpiMergeFileType.RegionImage];
                        model.MapFile = uploadedFiles[SpiMergeFileType.MapFile];
                        ViewBag.Message = "Image merge successfully!!";
                    }
                }
                catch
                {
                    ViewBag.Message = "File upload failed!!";
                }
            }

            Session[Definitions.SPS_TOOLS_MODEL_KEY] = model;
            return RedirectToAction("SpiImageMerge");
        }

        public FileResult DownloadSpiImage()
        {
            var model = Session[Definitions.SPS_TOOLS_MODEL_KEY] as SpiImageMergeModel;
            if (null == model || string.IsNullOrEmpty(model.ResultsFile) || !System.IO.File.Exists(model.ResultsFile))
                return null;

            var fileName = model.ResultsFile;
            byte[] buffer = System.IO.File.ReadAllBytes(fileName);

            Session[Definitions.SPS_TOOLS_MODEL_KEY] = model;

            return File(buffer, System.Net.Mime.MediaTypeNames.Application.Octet, System.IO.Path.GetFileName(fileName));
            //return File(fileName, "multipart/form-data", System.IO.Path.GetFileName(fileName));
        }

#if USE_WebClient
        public void DownloadSpiImage()
        {
            var model = Session[Definitions.SPS_TOOLS_MODEL_KEY] as SpiImageMergeModel;
            if (null == model || string.IsNullOrEmpty(model.ResultsFile) || !System.IO.File.Exists(model.ResultsFile))
                return;

            var fileName = model.ResultsFile;
            try
            {
                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("content-disposition", string.Format("attachment;filename={0}", model.OutputName));
                byte[] data = req.DownloadData(fileName);
                response.BinaryWrite(data);
                response.End();
            }
            catch { }
        }
#endif
        #endregion

        private SpsToolsModel _factoryDataModel(SpsToolsType selectedTool, string error)
        {
            SpsToolsModel model = Session[Definitions.SPS_TOOLS_MODEL_KEY] as SpsToolsModel;
            error = string.Empty;
            try
            {
                switch (selectedTool)
                {
                    case SpsToolsType.HealthEventDecode:
                        using (StreamReader file = System.IO.File.OpenText(string.Format(@"{0}\App_Data\{1}", Server.MapPath("~"), Definitions.HEALTH_EVENT_JSON_FILE)))
                        {
                            var serializer = new JsonSerializer();
                            model = (HealthEventViewModel)serializer.Deserialize(file, typeof(HealthEventViewModel));
                        }
                        break;
                    case SpsToolsType.SpiImageMerge:
                        if (null == model || model.GetType() != typeof(SpiImageMergeModel))
                        {
                            model = new SpiImageMergeModel();
                        }
                        break;
                    case SpsToolsType.MeFWStatusDecode:
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            Session[Definitions.SPS_TOOLS_MODEL_KEY] = model;
            return model;
        }

        private string _getVirtualPath(string physicalPath)
        {
            string rootpath = Server.MapPath("~/");

            physicalPath = physicalPath.Replace(rootpath, "");
            physicalPath = physicalPath.Replace("\\", "/");

            return "~/" + physicalPath;
        }
    }
}