using MicroPay.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers
{
    public class RetiredEmpHomePageController : Controller
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: RetiredEmpHomePage
        private readonly IDashBoardService dashBoardService;

        public RetiredEmpHomePageController(IDashBoardService dashBoardService)
        {
            this.dashBoardService = dashBoardService;
        }
        [SessionTimeout]
       // [CheckRight]
        public ActionResult Index()
        {
            log.Info($"RetiredEmpHomePage/Index");
            if (Session["User"] != null)
            {
                var userDetail = (UserDetail)Session["User"];
                var dashboard = dashBoardService.GetDashBoardForRetiredEmployee(userDetail.EmployeeID.Value); 
                return View(dashboard);
            }
            else
                return RedirectToAction("Index", "Login");
        }
    }
}