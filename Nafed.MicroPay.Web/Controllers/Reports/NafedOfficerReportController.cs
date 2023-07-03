using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;
 

namespace MicroPay.Web.Controllers.Reports
{
    public class NafedOfficerReportController : BaseController
    {
        public ActionResult Index()
        {
            log.Info($"NafedOfficerReportController/Index");
            try
            {
                userAccessRight.PageTitle = "Officer in Nafed Report";
                userAccessRight.ControllerName = "NafedOfficerReport";
                return PartialView("_CommonReportViewer",userAccessRight);
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
            log.Info($"NafedOfficerReportController/SubmitForm");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                reportModel.rptName = "RegNFDoffi.rpt";
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