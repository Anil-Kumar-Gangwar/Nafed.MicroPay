using MicroPay.Web.Models;
using System;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports
{
    public class BranchPostedEmployeeController : BaseController
    {
        // GET: BranchPostedEmployee
        public ActionResult Index()
        {
            log.Info($"BranchPostedEmployee/Index");
            try
            {
                userAccessRight.PageTitle = "List of Employees Posted In Different Branches";
                userAccessRight.ControllerName = "BranchPostedEmployee";
                return PartialView("_CommonReportViewer", userAccessRight);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult SubmitForm()
        {
            log.Info($"BranchPostedEmployee/SubmitForm");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                reportModel.rptName = "EmployeePosted.rpt";
                Session["ReportModel"] = reportModel;
                return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}