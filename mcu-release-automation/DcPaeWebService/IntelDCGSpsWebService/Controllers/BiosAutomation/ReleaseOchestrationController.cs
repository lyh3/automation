using System.Web.Mvc;
using WindowService.DataModel;
using IntelDCGSpsWebService.Models;
using Automation.Base.BuildingBlocks;

namespace IntelDCGSpsWebService.Controllers
{
    public class ReleaseOchestrationController : Controller
    {
        private static object _updateModelSyncObj = new object();
        private static object _syncObj = new object();
        
        public ActionResult Index()
        {
            var model = Session[Definitions.RELEASE_ORCHSTRATION_KEY] as ReleaseOrchstrationModel;

            if (null == model)
            {
                model = new ReleaseOrchstrationModel()
                {
                    Title = "UI Design Demo (BKC Release Orchestration)"
                };
                _UpdateSessionModel(model);
            }
            return View(model);
        }

        public ActionResult UpdateSelectedRelease(JsonContainer jsonData)
        {
            var model = Session[Definitions.RELEASE_ORCHSTRATION_KEY] as ReleaseOrchstrationModel;
            if (null != jsonData && jsonData.JsonString != "N/A")
            {
                var platform = model[model.SelectedPlatform];
                if (null != platform)
                {
                    platform.SelectedRelease = jsonData.JsonString;
                    _UpdateSessionModel(model);
                }
            }
            return Json(model[model.SelectedPlatform.ToString()]);
        }

        public ActionResult UpdateSelectedPlatform(JsonContainer jsonData)
        {
            var model = Session[Definitions.RELEASE_ORCHSTRATION_KEY] as ReleaseOrchstrationModel;
            if (null != jsonData)
            {
                model.SelectedPlatform = jsonData.JsonString;
                _UpdateSessionModel(model);
            }
            return Json(model[model.SelectedPlatform.ToString()]);
        }

        [HttpPost]
        public ActionResult StartOchestration()
        {
            lock (_syncObj)
            {
                var model = Session[Definitions.RELEASE_ORCHSTRATION_KEY] as ReleaseOrchstrationModel;
                var releaseWorkThread = new BiosReleaseOrchestrationThread(MvcApplication.Logger);
                releaseWorkThread.Start();
                model.Workthread = releaseWorkThread;
                _UpdateSessionModel(model);
                return Json(model);
            }
        }

        [HttpPost]
        public ActionResult StopOrchestration()
        {
            var model = Session[Definitions.RELEASE_ORCHSTRATION_KEY] as ReleaseOrchstrationModel;
            var releaseWorkThread = model.Workthread;
            BiosReleaseState curretnState = null;
            if(null != releaseWorkThread)
            {
                curretnState = releaseWorkThread.CurrentTransaction;
                curretnState.TranStatus = TransactionStatus.Aborted.ToString();
                curretnState.NotificationMessages = "User aborted.";
                _UpdateSessionModel(model);
                releaseWorkThread.Reset();
            }
            return Json(curretnState);
        }
        [HttpGet]
        public JsonResult GetTransactionState()
        {
            var model = Session[Definitions.RELEASE_ORCHSTRATION_KEY] as ReleaseOrchstrationModel;
            return Json(model);
        }
        public void Reset()
        {
        }
        private void _UpdateSessionModel(ReleaseOrchstrationModel model)
        {
            lock (_updateModelSyncObj)
            {
                for (int i = 0; i < 3; ++i)
                {
                    try
                    {
                        try
                        {
                            Session.Remove(Definitions.RELEASE_ORCHSTRATION_KEY);
                        }
                        catch { }
                        Session[Definitions.RELEASE_ORCHSTRATION_KEY] = model;
                    }
                    catch
                    {
                    }
                }
            }
        }
	}
}