using Nafed.MicroPay.Model;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MicroPay.Web.Attributes
{
    public class CandidateSessionTimeoutAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (HttpContext.Current.Session["RegistrationID"] == null && !filterContext.IsChildAction)
            //{
            //    if (filterContext.HttpContext.Request.IsAjaxRequest())
            //    {
            //        filterContext.HttpContext.Response.ClearContent();
            //        filterContext.HttpContext.Response.StatusCode = 308;//?
            //        filterContext.HttpContext.Response.End();
            //        base.OnActionExecuting(filterContext);
            //    }
            //    else
            //    {
            //        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Candidatelogin" }));
            //        base.OnActionExecuting(filterContext);
            //    }
            //} 

            if (HttpContext.Current.Session["ASP.NET_SessionId"] != null && HttpContext.Current.Request.Cookies["ASP.NET_SessionId"] != null)
            {
                if (!HttpContext.Current.Session["ASP.NET_SessionId"].ToString().Equals(HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Candidatelogin" }));
                    base.OnActionExecuting(filterContext);
                }
            }

        }
    }
}