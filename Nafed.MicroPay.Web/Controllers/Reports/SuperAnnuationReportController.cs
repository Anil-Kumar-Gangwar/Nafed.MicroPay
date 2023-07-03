using MicroPay.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports
{
    public class SuperAnnuationReportController : BaseController
    {
        // GET: SuperAnnuationReport
        public ActionResult Index()
        {
            log.Info($"SuperAnnuationReport/Index");
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
            log.Info($"SuperAnnuationReport/GetReportFilters");
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
            log.Info($"SuperAnnuationReport/ShowReport");

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

                string mo= string.Empty;
                string dy = string.Empty;
                string mo1 = string.Empty;
                string dy1 = string.Empty;
                string test = string.Empty;
                int From = 0;
                int Todate = 0;

                mo = filters.fromDate.Value.Month.ToString().Length == 1 ? "0" + Convert.ToString(filters.fromDate.Value.Month) : Convert.ToString(filters.fromDate.Value.Month);
                dy = filters.fromDate.Value.Day.ToString().Length == 1 ? "0" + Convert.ToString(filters.fromDate.Value.Day) : Convert.ToString(filters.fromDate.Value.Day);

                mo1 = filters.toDate.Value.Month.ToString().Length == 1 ? "0" + Convert.ToString(filters.toDate.Value.Month) : Convert.ToString(filters.toDate.Value.Month);
                dy1 = filters.toDate.Value.Day.ToString().Length == 1 ? "0" + Convert.ToString(filters.toDate.Value.Day) : Convert.ToString(filters.toDate.Value.Day);

                From = Convert.ToInt32(Convert.ToString(filters.fromDate.Value.Year) + mo + dy);
                Todate = Convert.ToInt32(Convert.ToString(filters.toDate.Value.Year) + mo1 + dy1);

                if (filters.AllEmployee)
                    test = "Fail";
                else
                    test = "test";

                parameterList.Add(new ReportParameter { name = "fromdate", value = From });
                parameterList.Add(new ReportParameter { name = "todate", value = Todate });
                parameterList.Add(new ReportParameter { name = "test", value = test });

                reportModel.reportParameters = parameterList;
                reportModel.rptName = "dosuppanu.rpt";
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