using MicroPay.Web.Models;
using System;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports
{
    public class ResOffHOReportController : BaseController
    {
        // GET: ResOffHOReport
        public ActionResult Index()
        {
            log.Info($"ResOffHOReport/Index");
            try
            {
                userAccessRight.PageTitle = "List Of Officers With Residential Addresses";
                userAccessRight.ControllerName = "ResOffHOReport";
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
            log.Info($"ResOffHOReport/SubmitForm");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                reportModel.rptName = "ResOffHO.rpt";
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