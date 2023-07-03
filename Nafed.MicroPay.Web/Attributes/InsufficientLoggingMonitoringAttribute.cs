using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MicroPay.Web.Attributes
{
    public class InsufficientLoggingMonitoringAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName.ToLower();
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();

            UserDetail user = (UserDetail)HttpContext.Current.Session["user"];

            if (actionName.ToLower() == "index")
            {
                string ipAddress = HttpContext.Current.Request.UserHostAddress;
                var userAgent = HttpContext.Current.Request.UserAgent;
                var UrlReferrer = HttpContext.Current.Request.UrlReferrer?.AbsoluteUri;
                var isLocal = HttpContext.Current.Request.IsLocal;
                System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
                string SessionId = manager.GetSessionID(System.Web.HttpContext.Current);

                var insufficientLoggingMonitoring = new InsufficientLoggingMonitoring();
                insufficientLoggingMonitoring.UserName = user.UserName;
                insufficientLoggingMonitoring.SessionId = SessionId;
                insufficientLoggingMonitoring.IP_address = HttpContext.Current.Request.UserHostAddress;
                insufficientLoggingMonitoring.DateTime = System.DateTime.Now;
                insufficientLoggingMonitoring.Referrer = HttpContext.Current.Request.UrlReferrer?.AbsoluteUri;
                insufficientLoggingMonitoring.URL = $"{controllerName}/{actionName}";
                insufficientLoggingMonitoring.UserAgent = HttpContext.Current.Request.UserAgent;
                insufficientLoggingMonitoring.Country = "India";
                SaveData(insufficientLoggingMonitoring);
            }
        }

        private void SaveData(InsufficientLoggingMonitoring insufficientLoggingMonitoring)
        {
            BaseService baseService = new BaseService();            
            baseService.InsertInsufficientLoggingMonitoring(insufficientLoggingMonitoring);
        }
    }
}