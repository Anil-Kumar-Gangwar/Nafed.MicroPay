using Nafed.MicroPay.Model;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MicroPay.Web.Attributes
{
    public class SessionTimeoutAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UserDetail user = (UserDetail)HttpContext.Current.Session["User"];

            if (user == null && !filterContext.IsChildAction)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.ClearContent();
                    filterContext.HttpContext.Response.StatusCode = 308;//?
                    filterContext.HttpContext.Response.End();
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Login" }));
                    base.OnActionExecuting(filterContext);
                }
            }
            else
            {
                if (user.DeviceTypeIsMobile != true)
                {
                    //if (filterContext.HttpContext.Request.UrlReferrer == null
                    //    || filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
                    //{
                    //    filterContext.Result =
                    //        new RedirectToRouteResult(new RouteValueDictionary(new
                    //        {
                    //            controller = "LogOff",
                    //            action = "Index",
                    //            area = ""
                    //        }));
                    //}
                    if(HttpContext.Current.Session["ASP.NET_SessionId"] != null && HttpContext.Current.Request.Cookies["ASP.NET_SessionId"] != null)
                    {
                        if (!HttpContext.Current.Session["ASP.NET_SessionId"].ToString().Equals(HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value))
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Login" }));
                            base.OnActionExecuting(filterContext);
                        }
                    }

                }
            }

        }
    }
}