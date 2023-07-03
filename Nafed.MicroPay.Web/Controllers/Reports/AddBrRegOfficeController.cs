using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers.Reports
{
    public class AddBrRegOfficeController : BaseController
    {
        public ActionResult Index()
        {
            log.Info($"AddBrRegOfficeController/Index");
            try
            {
                userAccessRight.ControllerName = "AddBrRegOffice";
                userAccessRight.PageTitle = "REGION WISE LIST OF OFFICES OF NAFED";
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
            log.Info($"AddBrRegOfficeController/SubmitForm");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                reportModel.rptName = "AddBrRegOffi.rpt";
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