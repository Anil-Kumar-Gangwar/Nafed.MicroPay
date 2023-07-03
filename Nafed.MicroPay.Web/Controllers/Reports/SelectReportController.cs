using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports
{
    public class SelectReportController : BaseController
    {
        // GET: SelectReport
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetReportOption()
        {
            log.Info("SelectReportController/GetReportOption");
            try
            {
                CommonFilter rFilters = new CommonFilter();
                return PartialView("_ReportFilters", rFilters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

    
        public ActionResult GetReportType(CommonFilter cFilter)
        {
            log.Info("SelectReportController/GetReportType");
            try
            {
                if (cFilter.ReportType == "1")
                {
                   if(userDetail.DeviceTypeIsMobile)
                        return RedirectToAction("DownloadSalarySlip", "SalaryReport");
                    else
                        return RedirectToAction("Index", "EmpWiseSalaryReport");
                }
                else if (cFilter.ReportType == "2")
                    return RedirectToAction("Index", "DAArrearReports");
                else if (cFilter.ReportType == "3")
                    return RedirectToAction("Index", "PayArrearReports");
                else if (cFilter.ReportType == "4")
                    return RedirectToAction("Index", "PFLoanReport");

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}