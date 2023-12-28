using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Model;
using CrystalDecisions.CrystalReports.Engine;
using Nafed.MicroPay.Common;
using System.IO;
using Nafed.MicroPay.Services.Salary;

namespace MicroPay.Web.Controllers.Reports.Payroll
{
    public class SalaryReportController : BaseController
    {
        private readonly IDropdownBindService dropDownService;
        private readonly ISalaryReportService salService;
        public SalaryReportController(IDropdownBindService dropDownService, ISalaryReportService salService)
        {
            this.dropDownService = dropDownService;
            this.salService = salService;
        }
        public ActionResult Index()
        {
            log.Info($"SalaryReportController/Index");
            try
            {
                SalaryReportFilterViewModel salaryReportFilter = new SalaryReportFilterViewModel();
                return View(salaryReportFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _ReportFilter()
        {
            log.Info($"SalaryReportController/_ReportFilter");
            try
            {
                SalaryReportFilterViewModel salaryReportFilterVM = new SalaryReportFilterViewModel();
                salaryReportFilterVM.employeeList = dropDownService.GetEmployee();
                salaryReportFilterVM.branchList = dropDownService.ddlBranchList();
                salaryReportFilterVM.employeeTypeList = dropDownService.ddlEmployeeTypeList();
                salaryReportFilterVM.AllBranch = true;
                salaryReportFilterVM.AllEmployee = true;
                salaryReportFilterVM.employeeTypeID = 5;
                salaryReportFilterVM.yearID = DateTime.Now.Year;
                salaryReportFilterVM.monthID = DateTime.Now.Month;
                return PartialView("_ReportFilter", salaryReportFilterVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetEmployeeDetails(int? branchId, int? employeeTypeId)
        {
            log.Info($"SalaryReportController/GetEmployeeDetails");
            try
            {
                var result = dropDownService.GetCurrExEmployeeDetailsByEmployeeType(branchId, employeeTypeId);
                return Json(new { employees = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult SalaryReportDetails(SalaryReportFilterViewModel salaryReportFilter)
        {
            log.Info($"SalaryReportController/SalaryReportDetails");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();
                int branchId = 0, month = 0, year = 0, employeeTypeId = 0, employeeId = 0;
                string rptName = string.Empty;
                string branchName = string.Empty;

                reportModel.reportType = 2;
                branchId = (salaryReportFilter.branchID == null ? 0 : salaryReportFilter.branchID.Value);
                month = (salaryReportFilter.monthID == null ? 0 : salaryReportFilter.monthID.Value);
                year = (salaryReportFilter.yearID == null ? 0 : salaryReportFilter.yearID.Value);
                employeeTypeId = (salaryReportFilter.employeeTypeID == null ? 0 : salaryReportFilter.employeeTypeID.Value);
                employeeId = (salaryReportFilter.employeeID == null ? 0 : salaryReportFilter.employeeID.Value);
                if (branchId != 0)
                {
                    var branchdetails = dropDownService.ddlBranchList(null, null);
                    branchName = branchdetails.Where(x => x.id == branchId).FirstOrDefault().value;
                }
                //month = 2;

                if (salaryReportFilter.employeeTypeID == 5)
                {
                    if (salaryReportFilter.salaryReportRadio == SalaryReportRadio.PaySlip)
                    {
                        parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                        parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                        parameterList.Add(new ReportParameter { name = "EMPID", value = employeeId });
                        if (branchId == 0 && salaryReportFilter.AllEmployee)
                            rptName = "PaySlipBR.rpt";
                        else
                        {
                            if (branchId != 44)
                                rptName = "PaySlipBR.rpt";
                            else
                                rptName = "PaySlipHO.rpt";
                        }
                    }

                    if (salaryReportFilter.salaryReportRadio == SalaryReportRadio.CoveringLetter)
                    {
                        parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                        parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                        parameterList.Add(new ReportParameter { name = "Emps", value = 0 });
                        parameterList.Add(new ReportParameter { name = "Br", value = " " });
                        parameterList.Add(new ReportParameter { name = "flag", value = " " });
                        rptName = "Paysummarydetail.rpt";
                    }

                    if (salaryReportFilter.salaryReportRadio == SalaryReportRadio.PaySummary)
                    {
                        if (branchId == 0)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                            parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "Emps", value = 0 });
                            parameterList.Add(new ReportParameter { name = "Br", value = " " });
                            parameterList.Add(new ReportParameter { name = "flag", value = " " });
                            rptName = "PaySummaryAllBranch.rpt";
                        }
                        else
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                            parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "Emps", value = 0 });
                            parameterList.Add(new ReportParameter { name = "Br", value = branchName });
                            parameterList.Add(new ReportParameter { name = "flag", value = " " });
                            rptName = "PaySummary.rpt";
                        }
                    }
                    if (salaryReportFilter.salaryReportRadio == SalaryReportRadio.PaySummaryAllBranch)
                    {
                        if (salaryReportFilter.AllBranch)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                            parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "Emps", value = 0 });
                            parameterList.Add(new ReportParameter { name = "Br", value = " " });
                            parameterList.Add(new ReportParameter { name = "flag", value = "P" });
                            rptName = "PaySummary.rpt";
                        }
                        else if (branchId == 44)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                            parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "Emps", value = 0 });
                            parameterList.Add(new ReportParameter { name = "Br", value = " " });
                            parameterList.Add(new ReportParameter { name = "flag", value = " " });
                            rptName = "PaySummary.rpt";
                        }
                    }
                }
                else if (salaryReportFilter.employeeTypeID == 9)
                {
                    if (salaryReportFilter.salaryReportRadio == SalaryReportRadio.PaySlip)
                    {
                        parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                        parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                        parameterList.Add(new ReportParameter { name = "EMPID", value = employeeId });
                        rptName = "PaySlipBR.rpt";
                    }
                }
                else if (salaryReportFilter.employeeTypeID == 7)
                {
                    if (salaryReportFilter.salaryReportRadio == SalaryReportRadio.PaySlip)
                    {
                        parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                        parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                        parameterList.Add(new ReportParameter { name = "EMPID", value = employeeId });
                        rptName = "PaySlipBR.rpt";
                    }
                }
                else if (salaryReportFilter.employeeTypeID == 4 || salaryReportFilter.employeeTypeID == 1 || salaryReportFilter.employeeTypeID == 2 || salaryReportFilter.employeeTypeID == 6)
                {
                    if (salaryReportFilter.salaryReportRadio == SalaryReportRadio.PaySlip)
                    {
                        parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                        parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                        parameterList.Add(new ReportParameter { name = "EMPID", value = employeeId });
                        rptName = "PaySlipBR.rpt";
                    }

                    if (salaryReportFilter.salaryReportRadio == SalaryReportRadio.PaySummary)
                    {
                        if (branchId == 0)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                            parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "Emps", value = 0 });
                            parameterList.Add(new ReportParameter { name = "Br", value = " " });
                            parameterList.Add(new ReportParameter { name = "flag", value = " " });
                            rptName = "PaySummaryAllBranch.rpt";
                        }
                        else
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                            parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "Emps", value = 0 });
                            parameterList.Add(new ReportParameter { name = "Br", value = branchName });
                            parameterList.Add(new ReportParameter { name = "flag", value = " " });
                            rptName = "PaySummary.rpt";
                        }
                    }

                   else if (salaryReportFilter.salaryReportRadio == SalaryReportRadio.PaySummaryAllBranch)
                    {
                        if (salaryReportFilter.AllBranch)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                            parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "Emps", value = 0 });
                            parameterList.Add(new ReportParameter { name = "Br", value = " " });
                            parameterList.Add(new ReportParameter { name = "flag", value = "P" });
                            rptName = "PaySummary.rpt";
                        }
                        else if (branchId == 44)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                            parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "Emps", value = 0 });
                            parameterList.Add(new ReportParameter { name = "Br", value = " " });
                            parameterList.Add(new ReportParameter { name = "flag", value = " " });
                            rptName = "PaySummary.rpt";
                        }
                    }
                }
                else if (salaryReportFilter.employeeTypeID == 3)
                {
                    if (salaryReportFilter.salaryReportRadio == SalaryReportRadio.PaySlip)
                    {
                        parameterList.Add(new ReportParameter { name = "BranchID", value = branchId });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                        parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = employeeTypeId });
                        parameterList.Add(new ReportParameter { name = "EMPID", value = employeeId });
                        rptName = "PaySlipBR.rpt";
                    }
                }
                reportModel.reportParameters = parameterList;
                reportModel.rptName = rptName;
                Session["ReportModel"] = reportModel;
                return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult SalaryReport()
        {
            log.Info($"SalaryReportController/SalaryReport");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public PartialViewResult GetFilter()
        {
            log.Info("TrainingController/GetFilter");
            try
            {
                CommonFilter cFilter = new CommonFilter();
                return PartialView("_Filter", cFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public ActionResult SalarySlip(int? year, int? month)
        {
            log.Info($"SalaryReportController/SalarySlip");
            try
            {
                if (year.HasValue && month.HasValue)
                {
                    SalarySlipModel ssModel = new SalarySlipModel();
                    SalaryReportFilter sFilter = new SalaryReportFilter();
                    sFilter.salMonth = Convert.ToByte(month.Value);
                    sFilter.salYear = Convert.ToInt16(year.Value);
                    sFilter.employeeID = userDetail.EmployeeID.Value;
                    ssModel = salService.GetSalarySlip(sFilter);
                    return PartialView("_SalarySlip",ssModel);
                }
                else
                {
                    
                    return Content("");
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult DownloadSalarySlip(int ? year,int ? month)
        {
            log.Info("SalaryReportController/DownloadSalarySlip");
            try
            {
                if (year.HasValue && month.HasValue)
                {
                    int branchId = 0;
                    string rptName = string.Empty;
                    branchId = userDetail.BranchID;
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();

                    parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                    parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(year) });
                    parameterList.Add(new ReportParameter { name = "EMPID", value = (int)userDetail.EmployeeID });
                    reportModel.reportParameters = parameterList;
                    reportModel.rptName = "EmpWisePaySlipMob.rpt";                    
                    reportModel.reportType = 2;

                    ReportDocument objReport = new ReportDocument();
                    var rptModel = reportModel;
                    string path = Path.Combine(Server.MapPath(rptModel.ReportFilePath), rptModel.rptName);
                    objReport.Load(path);
                    objReport.SetDatabaseLogon(ConfigManager.Value("odbcUser"), ConfigManager.Value("odbcPassword"), @"odbcNAFEDHRTEST", ConfigManager.Value("odbcDatabase"));
                    objReport.Refresh();
                    if (rptModel.reportParameters?.Count > 0)
                    {
                        foreach (var item in rptModel.reportParameters)
                            objReport.SetParameterValue($"@{item.name}", item.value);
                    }
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();


                    objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperLegal;

                    Stream stream = objReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "Pay Slip.pdf");

                }
                else {
                    return View("SalaryReport");
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}