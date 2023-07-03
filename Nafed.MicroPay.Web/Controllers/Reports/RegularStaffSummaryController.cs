using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers.Reports
{
    public class RegularStaffSummaryController : BaseController
    {
        public ActionResult Index()
        {
            log.Info($"RegularStaffSummaryController/Index");
            try
            {
                userAccessRight.ControllerName = "RegularStaffSummary";
                userAccessRight.PageTitle = "Staff Summary Report";
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
            log.Info($"RegularStaffSummaryController/SubmitForm");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                reportModel.rptName = "RegStfStr.rpt";
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