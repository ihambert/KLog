using System;
using Core;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web;

namespace KLog
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Logger.Register("hambert");
        }

        void Application_End(object sender, EventArgs e)
        {
            Logger.EndProcess();
        }

        void Application_Error(object sender, EventArgs e)
        {
            Logger.Error(null, Server.GetLastError(), HttpContext.Current.Request.Path);
        }
    }
}
