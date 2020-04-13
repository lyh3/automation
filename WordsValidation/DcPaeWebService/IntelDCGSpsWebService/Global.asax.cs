using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using log4net;

namespace IntelDCGSpsWebService
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static ILog _logger; 

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(typeof(MvcApplication)); 
        }
        public static ILog Logger { get { return _logger; } }
    }
}
