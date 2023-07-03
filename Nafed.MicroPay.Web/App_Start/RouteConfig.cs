using System.Web.Mvc;
using System.Web.Routing;

namespace MicroPay.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

           
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapRoute(
            //           name: "Login",
            //           url: "{prefix}/{controller}/{action}/{id}",
            //           defaults: new { prefix = "hrms", controller = "Login", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute(
                        name: "Login",
                        url: "{controller}/{action}/{id}",
                        defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional });

        }
    }
}
