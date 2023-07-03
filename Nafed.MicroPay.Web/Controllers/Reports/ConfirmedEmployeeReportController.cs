using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers.Reports
{
    public class ConfirmedEmployeeReportController : BaseController
    {
        // GET: ConfirmedEmployeeReport
        public ActionResult Index()
        {
            log.Info($"ConfirmedEmployeeReport/Index");
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

        public ActionResult GetReportFilters()
        {
            log.Info($"ConfirmedEmployeeReport/GetReportFilters");

            try
            {
                EmployeeViewModel filters = new EmployeeViewModel();
                return PartialView("_ReportFilter", filters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ShowReport(EmployeeViewModel filters)
        {
            log.Info($"RetiredEmployeeReport/ShowReport");

            if(!filters.fromDate.HasValue)
            {
                ModelState.AddModelError("FromDateRequired", "Please enter from date.");
                return PartialView("_ReportFilter", filters);
            }
            if (!filters.toDate.HasValue)
            {
                ModelState.AddModelError("ToDateRequired", "Please enter to date.");
                return PartialView("_ReportFilter", filters);
            }
            if (filters.fromDate.HasValue)
            {
                if (filters.fromDate.HasValue && filters.toDate.HasValue)
                {
                    if (filters.fromDate.Value > filters.toDate.Value)
                    {
                        ModelState.AddModelError("ToDateRangeValidation", "To Date cannot less than From Date.");
                        return PartialView("_ReportFilter", filters);
                    }
                }
                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();

                parameterList.Add(new ReportParameter { name = "fromdate", value = filters.fromDate.Value });
                parameterList.Add(new ReportParameter { name = "todate", value = filters.toDate.Value });
               
                reportModel.reportParameters = parameterList;
                reportModel.rptName = "ConfirmedEmployeeReport.rpt";
                Session["ReportModel"] = reportModel;
                return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return PartialView("_ReportFilter", filters);
            }
        }
    }
}