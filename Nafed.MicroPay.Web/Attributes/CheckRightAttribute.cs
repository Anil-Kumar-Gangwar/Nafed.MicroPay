using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MicroPay.Web.Attributes
{
    public class CheckRightAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName.ToLower();
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
           
            UserDetail user = (UserDetail)HttpContext.Current.Session["user"];
            var indexPageURL = $"{controllerName}/index";

            if (!SharedControllers.controllers.Contains(controllerName))
            {
                
                
                if (user != null && actionName != "index" && user.userMenuRights.Count > 0)
                {
                    var menuRights = user.userMenuRights;
                    if (menuRights.Any(x => x.menu.URL.ToLower() == indexPageURL))
                    {
                        var menuRight = menuRights.Where(x => x.menu.URL.ToLower() == indexPageURL).Select(
                            y => new { ShowMenuRight = y.ShowMenu, CreateRight = y.Create, EditRight = y.Edit }).FirstOrDefault();

                        if (actionName == "index" && !menuRight.ShowMenuRight)
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "UnAuthorizeAccess", controller = "MenuRender" }));

                        if (actionName == "edit" && !menuRight.EditRight)
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "UnAuthorizeAccess", controller = "MenuRender" }));

                        if (actionName == "create" && !menuRight.CreateRight)
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "UnAuthorizeAccess", controller = "MenuRender" }));
                    }
                    else
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "UnAuthorizeAccess", controller = "MenuRender" }));
                }
                else
                {
                    if (user != null)

                    {
                        if (user.userMenuRights.Count > 0)
                        {
                            if (!user.userMenuRights.Any(x => x.menu.URL.ToLower() == indexPageURL))
                                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "UnAuthorizeAccess", controller = "MenuRender" }));
                        }
                        else
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "UnAuthorizeAccess", controller = "MenuRender" }));

                        base.OnActionExecuting(filterContext);
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Login" }));
                        base.OnActionExecuting(filterContext);

                    }
                }
            }

        }
    }
}