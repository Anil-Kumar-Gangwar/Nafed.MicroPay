using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.Salary;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Controllers.Reports.Payroll
{
    public class EmpWiseSalaryReportController : BaseController
    {
        private readonly ISalaryReportService salRepService;
        public EmpWiseSalaryReportController(ISalaryReportService salRepService)
        {
            this.salRepService = salRepService;
        }
        // GET: EmpWiseSalaryReport
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _EmpWiseSalaryFilters()
        {
            log.Info($"EmpWiseSalaryReport/_EmpWiseSalaryFilters");
            try
            {
                
                return PartialView("_ReportFilters");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        //public JsonResult GetMonthofSelectedYear(int salYear)
        //{
        //    log.Info($"EmpWiseSalaryReport/GetMonthofSelectedYear");
        //    try
        //    {
                                
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        [HttpPost]
        public ActionResult _EmpWiseSalaryReport(CommonFilter cFilter)
        {
            log.Info($"EmpWiseSalaryReport/_EmpWiseSalaryReport");
            try
            {
                ModelState.Remove("EndDate");
                
                if (cFilter.Month == null)
                    ModelState.AddModelError("monthRequired", "Please Select Month.");
                if (cFilter.Year ==null || cFilter.Year==0)
                    ModelState.AddModelError("yearRequired", "Please Select Year.");
                if (ModelState.IsValid)
                {
                    var salmonth = (int)cFilter.Month;
                    if (!salRepService.CheckSalaryReportPublish(cFilter.Year.Value, salmonth,(int)userDetail.EmployeeID))
                    {                        
                        return Content("<span class='text-danger'>SalarySlip not approved to download for this month.</span>");
                    }
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();
                    reportModel.reportType = 2;
                    parameterList.Add(new ReportParameter { name = "SalMonth", value = salmonth });
                    parameterList.Add(new ReportParameter { name = "SalYear", value = cFilter.Year.Value });
                    parameterList.Add(new ReportParameter { name = "EMPID", value = userDetail.EmployeeID });
                    reportModel.reportParameters = parameterList;
                    reportModel.rptName = "EmpWisePaySlip.rpt";
                    Session["ReportModel"] = reportModel;
                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Content("<span class='text-danger'>Please select Month and Year.</span>");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}