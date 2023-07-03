using MicroPay.Web.Models;
using System;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports
{
    public class AMDtoAMNFDReportController : BaseController
    {
        // GET: AMDtoAMNFDReport
        public ActionResult Index()
        {
            log.Info($"AMDtoAMNFDReport/Index");
            try
            {
                userAccessRight.PageTitle = "List Of Officers From AMD To AM";
                userAccessRight.ControllerName = "AMDtoAMNFDReport";
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
            log.Info($"AMDtoAMNFDReport/SubmitForm");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                reportModel.rptName = "AMDtoAMNFD.rpt";
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