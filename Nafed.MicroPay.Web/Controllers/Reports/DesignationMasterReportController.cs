using MicroPay.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports
{
    public class DesignationMasterReportController : BaseController
    {
        // GET: DesignationMasterReport
        public ActionResult Index()
        {
            log.Info($"DesignationMasterReport/Index");
            try
            {
                userAccessRight.PageTitle = "Designation Master Report";
                userAccessRight.ControllerName = "DesignationMasterReport";
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
            log.Info($"DesignationMasterReport/SubmitForm");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                reportModel.rptName = "DesgnationMasterReport.rpt";
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter { name = "designationID", value = null });
                reportModel.reportParameters = parameterList;
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