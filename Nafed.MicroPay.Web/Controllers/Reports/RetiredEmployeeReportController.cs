using System;
using System.Web.Mvc;
using MicroPay.Web.Models;
using System.Collections.Generic;

namespace MicroPay.Web.Controllers.Reports
{
    public class RetiredEmployeeReportController : BaseController
    {
        // GET: RetiredEmployeeReport
        public ActionResult Index()
        {
            log.Info($"RetiredEmployeeReportController/Index");
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
            log.Info($"RetiredEmployeeReport/GetReportFilters");

            try
            {
                RetiredEmployeeReportFilter retiredEmpFilter = new RetiredEmployeeReportFilter();
                return PartialView("_ReportFilter", retiredEmpFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ShowReport(RetiredEmployeeReportFilter filters)
        {
            log.Info($"RetiredEmployeeReport/ShowReport");

            if (ModelState.IsValid)
            {
                if(filters.fromDate.HasValue && filters.toDate.HasValue)
                {
                    if(filters.fromDate.Value > filters.toDate.Value)
                    {
                        ModelState.AddModelError("ToDateRangeValidation", "To Date cannot less than From Date.");
                        return PartialView("_ReportFilter", filters);
                    }
                }
                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();

                parameterList.Add(new ReportParameter { name = "fromdate", value = filters.fromDate.Value });
                parameterList.Add(new ReportParameter { name = "todate", value = filters.toDate.Value });
                parameterList.Add(new ReportParameter { name = "test", value = !filters.allEmployee ? "test" : "fail" });
                reportModel.reportParameters = parameterList;
                reportModel.rptName = "RetiredEmployeesReport.rpt";
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