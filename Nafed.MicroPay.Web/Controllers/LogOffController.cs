using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Model;
using System;

namespace MicroPay.Web.Controllers
{
    public class LogOffController : Controller
    {
        private readonly ILoginService loginService;
        public LogOffController(ILoginService loginService)
        {
            this.loginService = loginService;
        }
        // GET: LogOff
        public ActionResult Index()
        {
            //===== Insert user login details==============
            UserDetail uDetail = (UserDetail)Session["User"];
            System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
            string SessionId = manager.GetSessionID(System.Web.HttpContext.Current);
            Nafed.MicroPay.Model.UserLoginDetails userLoginDetails = new Nafed.MicroPay.Model.UserLoginDetails();
            if (uDetail != null)
            {
                userLoginDetails.UserName = uDetail.UserName;
                userLoginDetails.LogOutTime = DateTime.Now;
                userLoginDetails.SessionId = SessionId;
                loginService.InsertLogOutDetials(userLoginDetails);
            }
            //===== Insert user login details==============

           
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            Session.RemoveAll();

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty));
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);

            }
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                Response.Cookies.Add(new HttpCookie("AuthToken", ""));
            }

            return View();
        }

    }
}