using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services.Salary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports.Payroll
{
    public class IndividualHeadReportController : BaseController
    {
        private readonly ISalaryHeadRuleService salaryService;
        private readonly IDropdownBindService dropdownBindService;

        public IndividualHeadReportController(ISalaryHeadRuleService salaryService, IDropdownBindService dropdownBindService)
        {
            this.salaryService = salaryService;
            this.dropdownBindService = dropdownBindService;
        }

        public ActionResult Index()
        {
            log.Info("IndividualHeadReport/Index");
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

        [HttpGet]
        public ActionResult _ReportFilter()
        {
            log.Info("IndividualHeadReport/_ReportFilter");
            try
            {
                IndividualHeadReportFilters objHeadReportFilters = new IndividualHeadReportFilters();
                objHeadReportFilters.monthlyInputHeadList = dropdownBindService.GetSalaryHeadForIndividualHead(5);
                objHeadReportFilters.branchList = dropdownBindService.ddlBranchList();
                objHeadReportFilters.employeeTypesList = dropdownBindService.ddlEmployeeTypeList();
                objHeadReportFilters.ViewPFNo = true;
                return PartialView("_ReportFilter", objHeadReportFilters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult PrintIndividualHeadReport(IndividualHeadReportFilters objHeadReportFilters)
        {
            log.Info("IndividualHeadReport/PrintIndividualHeadReport");
            try
            {
                //monthlyInputHead

                if (objHeadReportFilters.AllBranchWithHo || objHeadReportFilters.AllBranch)
                    objHeadReportFilters.selectedEmployeeTypeID = 5;

                if (objHeadReportFilters.selectedEmployeeTypeID == 0)
                    ModelState.AddModelError("EmployeeType", "Please Select Employee Type");
                if (objHeadReportFilters.chkgrp)
                {
                    ModelState.Remove("salYear");
                    ModelState.Remove("salMonth");
                    ModelState.Remove("monthlyInputHead");


                    if (objHeadReportFilters.selectedInputHeads?.Count > 6)
                        ModelState.AddModelError("selectedInputHeads", "You cannot select more than 6 heads at a time because it will exceed the report. Please select 6 or Less than 6 heads");
                    if (objHeadReportFilters.fMonth == 0)
                        ModelState.AddModelError("fMonth", "Please Select From Month");
                    if (objHeadReportFilters.tMonth == 0)
                        ModelState.AddModelError("tMonth", "Please Select To Month");
                    if (objHeadReportFilters.fYear == 0)
                        ModelState.AddModelError("fYear", "Please Select From Year");
                    if (objHeadReportFilters.tYear == 0)
                        ModelState.AddModelError("tYear", "Please Select To Year");
                }

                if (ModelState.IsValid)
                {
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();
                    reportModel.reportType = 2;
                    objHeadReportFilters.isValid = true;
                    string branchCode = string.Empty, INSTNO = string.Empty, ELINSTALLNO = string.Empty, GLINSTALLNO = string.Empty, payslipno = string.Empty, slcode = string.Empty, headName = string.Empty, headDesc = string.Empty, brcd = string.Empty, fMonth = string.Empty, tMonth = string.Empty, fYear = string.Empty, tYear = string.Empty, rptName = string.Empty;
                    int? branchId = null;
                    List<string> LST = new List<string>();
                    var month = objHeadReportFilters.salMonth;
                    var year = objHeadReportFilters.salYear;
                    headName = objHeadReportFilters.monthlyInputHead;
                    headDesc = objHeadReportFilters.HeadDescription;

                    fMonth = Convert.ToString(objHeadReportFilters.fMonth).Length == 1 ? "0" + Convert.ToString(objHeadReportFilters.fMonth) : Convert.ToString(objHeadReportFilters.fMonth);
                    tMonth = Convert.ToString(objHeadReportFilters.tMonth).Length == 1 ? "0" + Convert.ToString(objHeadReportFilters.tMonth) : Convert.ToString(objHeadReportFilters.tMonth);
                    fYear = Convert.ToString(objHeadReportFilters.fYear) + fMonth;
                    tYear = Convert.ToString(objHeadReportFilters.tYear) + tMonth;

                    if (objHeadReportFilters.chkgrp)
                    {
                        
                        if (objHeadReportFilters.AllBranchWithHo)
                        {
                            branchCode = "T";
                        }
                        else if (objHeadReportFilters.AllBranch)
                        {
                            branchCode = "A";
                        }
                        else
                        {
                            if (objHeadReportFilters.branchId == 0)
                                branchCode = "";
                        }

                        if (objHeadReportFilters.AllBranch || objHeadReportFilters.AllBranchWithHo)
                        {
                            branchId = null;
                        }
                        else
                        {
                            branchId = objHeadReportFilters.branchId;
                        }

                        if (objHeadReportFilters.licTcsGis)
                        {
                            branchCode = "";
                            rptName = "LIC_TCS_GIS.rpt";
                        }
                        else
                        {
                            if (objHeadReportFilters.hblLoan)
                            {
                                rptName = "LIC_TCS_GIS.rpt";
                                branchCode = "e";
                            }
                            if (objHeadReportFilters.pfMonthly)
                            {
                                rptName = "PF_monthly.rpt";
                                if (objHeadReportFilters.AllBranch)
                                    branchCode = "";
                                else
                                    branchCode = "99";

                            }
                            else
                            {
                                for (int i = 0; i < objHeadReportFilters.selectedInputHeads?.Count; i++)
                                {
                                    LST.Add(Convert.ToString(objHeadReportFilters.selectedInputHeads[i]) + "_A");
                                }
                                if (LST.Count < 6)
                                {
                                    for (int i = LST.Count; i < 6; i++)
                                    {
                                        LST.Add("");
                                    }
                                }
                               
                                if(Convert.ToString(LST[0])== "E_06_A")
                                {
                                    if (objHeadReportFilters.AllBranch)
                                    {
                                        branchCode = "";                                       
                                    }
                                    else
                                    {
                                        branchCode = dropdownBindService.GetBranchCode(objHeadReportFilters.branchId);
                                    }
                                    parameterList.Add(new ReportParameter { name = "FROMPERIOD", value = fYear });
                                    parameterList.Add(new ReportParameter { name = "TOPERIOD", value = tYear });
                                    parameterList.Add(new ReportParameter { name = "Brcode", value = branchCode });
                                    reportModel.reportParameters = parameterList;
                                    reportModel.rptName = "RptCHAlw.rpt";
                                    Session["ReportModel"] = reportModel;
                                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);

                                }
                                else
                                {
                                    var report = salaryService.GeneralReportsSummation(branchCode, fMonth, fYear, tMonth, tYear, LST, objHeadReportFilters.selectedEmployeeTypeID);

                                    parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                                    parameterList.Add(new ReportParameter { name = "SALMONTHF", value = fMonth });
                                    parameterList.Add(new ReportParameter { name = "SALYEARF", value = fYear });
                                    parameterList.Add(new ReportParameter { name = "SALMONTHT", value = tMonth });
                                    parameterList.Add(new ReportParameter { name = "SALYEART", value = tYear });
                                    parameterList.Add(new ReportParameter { name = "HD1", value = Convert.ToString(LST[0]) });
                                    parameterList.Add(new ReportParameter { name = "HD2", value = Convert.ToString(LST[1]) });
                                    parameterList.Add(new ReportParameter { name = "HD3", value = Convert.ToString(LST[2]) });
                                    parameterList.Add(new ReportParameter { name = "HD4", value = Convert.ToString(LST[3]) });
                                    parameterList.Add(new ReportParameter { name = "HD5", value = Convert.ToString(LST[4]) });
                                    parameterList.Add(new ReportParameter { name = "HD6", value = Convert.ToString(LST[5]) });

                                    reportModel.reportParameters = parameterList;
                                    reportModel.rptName = "GeneralReportSum.rpt";
                                    Session["ReportModel"] = reportModel;
                                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                                }

                            }
                        }
                        parameterList.Add(new ReportParameter { name = "FROMPERIOD", value = fYear });
                        parameterList.Add(new ReportParameter { name = "TOPERIOD", value = tYear });
                        parameterList.Add(new ReportParameter { name = "EMPTYPE", value = objHeadReportFilters.selectedEmployeeTypeID });
                        parameterList.Add(new ReportParameter { name = "BRANCHID", value = branchId });
                        parameterList.Add(new ReportParameter { name = "BRANCHCODE", value = branchCode });
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {

                        if (objHeadReportFilters.ViewPaySlipNo)
                            payslipno = "PAYSLIPNO";

                        if (objHeadReportFilters.ViewPFNo)
                            slcode = "PFNO";

                        if (objHeadReportFilters.AllBranchWithHo)
                        {
                            branchCode = "T";
                            objHeadReportFilters.selectedEmployeeTypeID = 5;
                        }
                        else
                        {
                            if (objHeadReportFilters.AllBranch)
                            {
                                branchCode = "A";
                                objHeadReportFilters.selectedEmployeeTypeID = 5;
                            }
                            else
                                branchCode = dropdownBindService.GetBranchCode(objHeadReportFilters.branchId);
                        }

                        if (headName == "D_15_A" || headName == "D_17_A" || headName == "D_18_A" || headName == "D_19_A" || headName == "D_21_A")
                        {
                            INSTNO = "InstNo";
                            ELINSTALLNO = "";
                            GLINSTALLNO = "";
                        }
                        else if (headName == "D_03_A")
                        {
                            INSTNO = "";
                            ELINSTALLNO = "ELINSTALLNO";
                            GLINSTALLNO = "GLINSTALLNO";
                        }

                        var employeeTypeId = objHeadReportFilters.selectedEmployeeTypeID;
                        var report = salaryService.GeneralReports(branchCode, month, year, headName, headDesc, slcode, payslipno, employeeTypeId);

                        parameterList.Add(new ReportParameter { name = "BRANCHCODE", value = branchCode });
                        parameterList.Add(new ReportParameter { name = "SALMONTH", value = month });
                        parameterList.Add(new ReportParameter { name = "SALYEAR", value = year });
                        parameterList.Add(new ReportParameter { name = "HEADNAME", value = headName });
                        parameterList.Add(new ReportParameter { name = "HEADDESC", value = headDesc });
                        parameterList.Add(new ReportParameter { name = "INST", value = INSTNO });
                        parameterList.Add(new ReportParameter { name = "ELINSTALLNO", value = ELINSTALLNO });
                        parameterList.Add(new ReportParameter { name = "GLINSTALLNO", value = GLINSTALLNO });
                        parameterList.Add(new ReportParameter { name = "SLCODE", value = slcode });
                        parameterList.Add(new ReportParameter { name = "PAYSLIPNO", value = payslipno });
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = "GeneralReport.rpt";
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    objHeadReportFilters.isValid = false;
                    objHeadReportFilters.monthlyInputHeadList = dropdownBindService.GetSalaryHead(5);
                    objHeadReportFilters.branchList = dropdownBindService.ddlBranchList();
                    objHeadReportFilters.employeeTypesList = dropdownBindService.ddlEmployeeTypeList();
                    objHeadReportFilters.ViewPFNo = true;
                    objHeadReportFilters.chkgrp = false;
                    return PartialView("_ReportFilter", objHeadReportFilters);
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