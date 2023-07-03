using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers.Reports
{
    public class OffiDelOHOController : BaseController
    {
        public ActionResult Index()
        {
            log.Info($"OffiDelOHOController/Index");
            try
            {
                userAccessRight.ControllerName = "OffiDelOHO";
                userAccessRight.PageTitle = "OffiDelOHO Report";
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
            log.Info($"OffiDelOHOController/SubmitForm");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                reportModel.rptName = "OffiDelOHO.rpt";
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