using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using System;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers
{
    public class PRHeaderController : BaseController
    {
        private readonly IPRService PRService;
        private readonly IDropdownBindService ddlService;
        // GET: PRHeader

        public PRHeaderController(IPRService PRService, IDropdownBindService ddlService)
        {
            this.PRService = PRService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            log.Info($"PRHeaderController/Index");
            PropertyReturnViewModel PRVM = new PropertyReturnViewModel();
            PRVM.userRights = userAccessRight;
            return View(PRVM);
        }

        [HttpGet]
        public ActionResult _GetPRGridView(PropertyReturnViewModel PRVM)
        {
            PropertyReturnViewModel PRHVM = new PropertyReturnViewModel();
            var PR = PRService.GetPRHList((int)userDetail.EmployeeID);
            PRHVM.PRList = PR;
            PRHVM.userRights = userAccessRight;
            return PartialView("_PRHeaderGridView", PRHVM);
        }


        [HttpGet]
        public ActionResult Create()
        {
            log.Info("PRHeaderController/Create");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}