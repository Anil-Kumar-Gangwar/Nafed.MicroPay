using MicroPay.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports
{
    public class ConfirmationDueReportController : BaseController
    {
        // GET: ConfirmationDueReport
        public ActionResult Index()
        {
            log.Info($"ConfirmationDueReport/Index");
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
            log.Info($"ConfirmationDueReport/GetReportFilters");
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
            log.Info($"ConfirmationDueReport/ShowReport");

            if (!filters.fromDate.HasValue)
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
                reportModel.rptName = "ConfirmationPeriodwiseReport.rpt";

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