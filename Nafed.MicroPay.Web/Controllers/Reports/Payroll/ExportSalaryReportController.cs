using System;
using System.Linq;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Services.Salary;
using Nafed.MicroPay.Services.IServices;
using System.Collections.Generic;

namespace MicroPay.Web.Controllers.Reports.Payroll
{
    public class ExportSalaryReportController : BaseController
    {
        private readonly ISalaryReportService salReportService;
        private readonly IDropdownBindService dropDownService;
        public ExportSalaryReportController(ISalaryReportService salReportService, IDropdownBindService dropDownService)
        {
            this.salReportService = salReportService;
            this.dropDownService = dropDownService;
        }

        // GET: ExportSalaryReport
        public ActionResult Index()
        {
            log.Info($"ExportSalaryReport/Index");
            return View();
        }

        [HttpGet]
        public ActionResult _ReportFilter()
        {
            log.Info($"ExportSalaryReport/_ReportFilter");
            try
            {
                SalaryReportFilter filter = new SalaryReportFilter();
                var financialYear = ExtensionMethods.GetYearBetweenYearsList(2005, 2044);
                filter.financialYearList = financialYear.Select(x => new SelectListModel { id = 0, value = x }).ToList();
                filter.employeeList = dropDownService.GetEmployee();
                filter.branchList = dropDownService.ddlBranchList();
                filter.employeeTypeList = dropDownService.ddlEmployeeTypeList();
                filter.employeeTypeID = 5;
                return PartialView("_ReportFilters", filter);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult _ReportFilter(SalaryReportFilter reportFilter)
        {
            log.Info("ExportSalaryReport/_ReportFilter");

            string fileName = string.Empty, msg = string.Empty, result = string.Empty;
            string fullPath = Server.MapPath("~/FileDownload/");

            if (reportFilter.enumSalReportType == SalaryReportType.EmployeeWiseAnnual)
            {
                //if (reportFilter.branchID == 0)
                //    ModelState.AddModelError("branchRequired", "Please Select Branch");
                if (reportFilter.financialYear == "Select")
                    ModelState.AddModelError("financialYearRequired", "Please Select Financial Year");
                if (reportFilter.employeeTypeID == 0)
                    ModelState.AddModelError("employeeTypeRequired", "Please Select Employee Type");
            }
            else if (reportFilter.enumSalReportType == SalaryReportType.BranchWiseAnnual)
            {
                if (reportFilter.financialYear == "Select")
                    ModelState.AddModelError("financialYearRequired", "Please Select Financial Year");
                if (reportFilter.employeeTypeID == 0)
                    ModelState.AddModelError("employeeTypeRequired", "Please Select Employee Type");
            }
            else if (reportFilter.enumSalReportType == SalaryReportType.MonthlyBranchWise)
            {
                if (reportFilter.salMonth == 0)
                    ModelState.AddModelError("monthRequired", "Please Select Month");
                if (reportFilter.salYear == 0)
                    ModelState.AddModelError("yearRequired", "Please Select Year");

                if (reportFilter.employeeTypeID == 0)
                    ModelState.AddModelError("employeeTypeRequired", "Please Select Employee Type");
            }
            else if (reportFilter.enumSalReportType == SalaryReportType.MonthlyEmployeeWise)
            {
                //if (reportFilter.branchID == 0)
                //    ModelState.AddModelError("branchRequired", "Please Select Branch");

                if (reportFilter.salMonth == 0)
                    ModelState.AddModelError("monthRequired", "Please Select Month");
                if (reportFilter.salYear == 0)
                    ModelState.AddModelError("yearRequired", "Please Select Year");

                if (reportFilter.employeeTypeID == 0)
                    ModelState.AddModelError("employeeTypeRequired", "Please Select Employee Type");
            }
            else
                ModelState.AddModelError("ReportTypeRequired", "Please Select Report Type.");
          
            if (ModelState.IsValid)
            {
                if (reportFilter.enumSalReportType == SalaryReportType.MonthlyBranchWise)  //=== =
                {
                    fileName = ExtensionMethods.SetUniqueFileName($"{reportFilter.enumSalReportType.GetDisplayName()}-", FileExtension.xlsx);
                    reportFilter.fileName = fileName;
                    reportFilter.filePath = fullPath;
                    result = salReportService.GetMonthlyBranchWiseReport(reportFilter);

                    if (result == "notfound")
                        ModelState.AddModelError("ReportTypeRequired", "No Record Found.");
                    else
                        return Json(new
                        {
                            fileName = fileName,
                            fullPath = fullPath + fileName,
                            message = "success"
                        });
                }
                else if (reportFilter.enumSalReportType == SalaryReportType.MonthlyEmployeeWise)
                {
                    fileName = ExtensionMethods.SetUniqueFileName($"{reportFilter.enumSalReportType.GetDisplayName()}-", FileExtension.xlsx);
                    reportFilter.fileName = fileName;
                    reportFilter.filePath = fullPath;

                    result = salReportService.GetMonthlyEmployeeWiseReport(reportFilter);

                    if (result == "notfound")
                        ModelState.AddModelError("ReportTypeRequired", "No Record Found.");
                    else
                        return Json(new
                        {
                            fileName = fileName,
                            fullPath = fullPath + fileName,
                            message = "success"
                        });

                }
                else if (reportFilter.enumSalReportType == SalaryReportType.EmployeeWiseAnnual)
                {
                    fileName = ExtensionMethods.SetUniqueFileName($"{SalaryReportType.EmployeeWiseAnnual.GetDisplayName().Trim().ToString()}-", FileExtension.xlsx);

                    reportFilter.fileName = fileName;
                    reportFilter.filePath = fullPath;
                    reportFilter.branchID = reportFilter.branchID == 0 ? null : reportFilter.branchID;
                    reportFilter.employeeID = reportFilter.employeeID == 0 ? null : reportFilter.employeeID;

                    reportFilter.financialFrom = Convert.ToInt32(reportFilter.financialYear.Substring(0, 4) + "04");
                    reportFilter.financialTo = Convert.ToInt32(reportFilter.financialYear.Substring(7, 4) + "03");

                    var res = salReportService.GetEmployeeWiseAnnualReport(reportFilter);
                    return Json(new
                    {
                        fileName = fileName,
                        fullPath = fullPath + fileName,
                        message = "success"
                    });

                }
                else if (reportFilter.enumSalReportType == SalaryReportType.BranchWiseAnnual)
                {
                    fileName = ExtensionMethods.SetUniqueFileName($"{SalaryReportType.BranchWiseAnnual.GetDisplayName().Trim().ToString()}-", FileExtension.xlsx);
                    reportFilter.fileName = fileName;
                    reportFilter.filePath = fullPath;
                    reportFilter.branchID = reportFilter.branchID == 0 ? null : reportFilter.branchID;
                    reportFilter.financialFrom = Convert.ToInt32(reportFilter.financialYear.Substring(0, 4) + "04");
                    reportFilter.financialTo = Convert.ToInt32(reportFilter.financialYear.Substring(7, 4) + "03");
                    var res = salReportService.GetBranchWiseAnnualReport(reportFilter);

                    return Json(new
                    {
                        fileName = fileName,
                        fullPath = fullPath + fileName,
                        message = "success"
                    });
                }
            }
            //else
            //{
            reportFilter.branchList = dropDownService.ddlBranchList();
            reportFilter.employeeTypeList = dropDownService.ddlEmployeeTypeList();
            var financialYear = ExtensionMethods.GetYearBetweenYearsList(2005, 2044);
            reportFilter.financialYearList = financialYear.Select(x => new SelectListModel { id = 0, value = x }).ToList();
            reportFilter.employeeList = dropDownService.GetEmployee();
            return Json(new
            {
                part = 1,
                htmlData = ConvertViewToString("_ReportFilters", reportFilter),
                message = "error"
            }, JsonRequestBehavior.AllowGet);

            // }
        }

      
    }

}