using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using IntelDCGSpsWebService.Models;
using IntelDCGSpsWebService.Models.Buildingblocks;
using WindowService.DataModel;

namespace IntelDCGSpsWebService.Controllers
{
    public class EventDataDecodeController : Controller
    {
        public ActionResult Index()
        {
            HealthEventViewModel model = null;
            using (StreamReader file = System.IO.File.OpenText(string.Format(@"{0}\App_Data\{1}", Server.MapPath("~"), Definitions.HEALTH_EVENT_JSON_FILE)))
            {
                var serializer = new JsonSerializer();
                model = (HealthEventViewModel)serializer.Deserialize(file, typeof(HealthEventViewModel));
            }
            Session[Definitions.HEALTH_EVENT_MODEL_KEY] = model;
            return View(model);
        }

        public ActionResult DecodeHealthEvent(JsonDataContener jsonData)
        {        
            var sb = new StringBuilder();
            var regex = new Regex("^[0-9a-fA-F]{6}$");

            if (null == jsonData || string.IsNullOrEmpty(jsonData.JsonString) || !regex.Match(jsonData.JsonString).Success)
                this._ComposeErrorHtml("Please enter a valid health event data (must be 6 Hex digits [0-9, a-f or A-F] string and no white space between) and try again.", sb);
            else
            {
                var errorMessage = string.Empty;
                var itemformat = "value code - {0} : description - {1}";
                var model = Session[Definitions.HEALTH_EVENT_MODEL_KEY] as HealthEventViewModel;
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

            return Json(new { html = htmlContent, error = "" });
         }

        private void _ComposeErrorHtml(string errorMessage, StringBuilder sb)
        {
            sb.Clear();
            sb.Append("<div>&nbsp;</dev>");
            sb.Append("<div id='divErrorMessage' class='alert alert-dismissible alert-danger' style='width:98%;margin-left:10px;'>");
            sb.Append(string.Format("<span id='spError'>{0}</span>", errorMessage));
            sb.Append("</div>");
        }
	}
}