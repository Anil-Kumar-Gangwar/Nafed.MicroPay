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
    public class OfficerStaffCountController : BaseController
    {
        public OfficerStaffCountController()
        {
        }

        public ActionResult Index()
        {
            log.Info($"OfficerStaffCountController/Index");
            try
            {
                return View(userAccessRight);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _ReportFilter()
        {
            log.Info($"OfficerStaffCountController/_ReportFilter");
            try
            {
                OfficerStaffViewModel OSVM = new OfficerStaffViewModel();
                return PartialView("_ReportFilter", OSVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult OfficerStaffDetails(OfficerStaffViewModel OSVM)
        {
            log.Info($"OfficerStaffCountController/OfficerStaffDetails");
            try
            {
                if (OSVM.reportType == ReportType.Officer)
                    OSVM.officerCount = true;
                else if (OSVM.reportType == ReportType.Staff)
                    OSVM.staffCount = true;

                if (!OSVM.officerCount && !OSVM.staffCount)
                    ModelState.AddModelError("SelectAnyOne", "Please select any one of them");
                if (ModelState.IsValid)
                {
                    BaseReportModel reportModel = new BaseReportModel();
                    if (OSVM.staffCount)
                    {
                        reportModel.rptName = string.Empty;
                        reportModel.rptName = "staffcount.rpt";
                        Session["ReportModel"] = reportModel;
                    }
                    else if (OSVM.officerCount)
                    {
                        reportModel.rptName = string.Empty;
                        reportModel.rptName = "OfficerCountReport.rpt";
                        Session["ReportModel"] = reportModel;
                    }
                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return PartialView("_ReportFilter", OSVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}