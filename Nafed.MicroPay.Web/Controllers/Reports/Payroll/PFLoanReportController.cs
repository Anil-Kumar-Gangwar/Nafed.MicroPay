using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
using System.Linq;
using System.Collections.Generic;
using Nafed.MicroPay.Services.Salary;
using System.Data;

namespace MicroPay.Web.Controllers.Reports.Payroll
{
    public class PFLoanReportController : BaseController
    {
        // GET: PFLoanReport
        private readonly IDropdownBindService dropdownBindService;
        private readonly IAdjustOldLoanService adjustOldLoanService;

        public PFLoanReportController(IDropdownBindService dropdownBindService, IAdjustOldLoanService adjustOldLoanService)
        {
            this.dropdownBindService = dropdownBindService;
            this.adjustOldLoanService = adjustOldLoanService;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult _PFLoanReportDetails()
        {
            log.Info("PFLoanReport/_PFLoanReportDetails");
            try
            {
                PFLoanReportFilters objPFLoanReportFilters = new PFLoanReportFilters();
                objPFLoanReportFilters.branchList = dropdownBindService.ddlBranchList();
                objPFLoanReportFilters.employeeTypesList = dropdownBindService.ddlEmployeeTypeList();
                var financialYear = ExtensionMethods.GetYearBetweenYearsList(2004, 2044);
                objPFLoanReportFilters.financialYearList = financialYear.Select(x => new SelectListModel { id = 0, value = x }).ToList();

                objPFLoanReportFilters.employeeList = dropdownBindService.GetAllEmployee();

                return PartialView("_ReportFilters", objPFLoanReportFilters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PFLoanReport(PFLoanReportFilters objPFLoanReportFilters)
        {
            log.Info("PFLoanReport/_PFLoanReport");
            try
            {
                //if (objPFLoanReportFilters.selectedEmployeeTypeID == 0)
                //    ModelState.AddModelError("employeeTypeRequired", "Select Employee Type");
                if ((objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPFStatement || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPDSummary || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPFLDSatement || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.LOERefundable || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.LOENonRefundable || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.LOERefundableFinished || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPFLLedger || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFLLIAdjust || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFLTRecovered || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFLPPayment) && objPFLoanReportFilters.salYear == 0)
                    ModelState.AddModelError("salYearRequired", "Select Year");
                if ((objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPFStatement || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPFLDSatement || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.LOERefundable || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.LOENonRefundable || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.LOERefundableFinished || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPFLLedger || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFLLIAdjust || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFLTRecovered || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFLPPayment) && objPFLoanReportFilters.salMonth == 0)
                    ModelState.AddModelError("salMonthRequired", "Select Month");
                if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPDSummary && objPFLoanReportFilters.dateTypeRadio == DateTypeRadio.rbddateRange)
                {
                    if (objPFLoanReportFilters.fromMonth == 0 || objPFLoanReportFilters.toMonth == 0)
                        ModelState.AddModelError("dateRangeRequired", "Please select From and To Month");
                    if (objPFLoanReportFilters.branchId == 0 && !objPFLoanReportFilters.AllBranch)
                        ModelState.AddModelError("branchId", "Please select Branch");
                }
                if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPDSummary && objPFLoanReportFilters.dateTypeRadio != DateTypeRadio.rbddateRange && objPFLoanReportFilters.salMonth == 0)
                    ModelState.AddModelError("salMonthRequired", "Select Month");


                if (objPFLoanReportFilters.financialYear == "Select" && (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFChecklist || objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFNSummary))
                    ModelState.AddModelError("finanicalYearRequired", "Select Financial Type");
                if (ModelState.IsValid)
                {
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();
                    reportModel.reportType = 2;
                    string branchCode = string.Empty, vMonth = string.Empty, currentyear = string.Empty;
                    int salMonth = 0, salYear = 0; int? EmployeeId; int? toMonth; int? employeeTypeId;
                    if (objPFLoanReportFilters.branchId != 0)
                        branchCode = dropdownBindService.GetBranchCode(objPFLoanReportFilters.branchId);
                    else
                        branchCode = null;
                    EmployeeId = objPFLoanReportFilters.employeeId == 0 ? null : (int?)objPFLoanReportFilters.employeeId;

                    employeeTypeId = objPFLoanReportFilters.selectedEmployeeTypeID == 0 ? null : (int?)objPFLoanReportFilters.selectedEmployeeTypeID;

                    salMonth = objPFLoanReportFilters.salMonth;
                    salYear = objPFLoanReportFilters.salYear;
                    vMonth = objPFLoanReportFilters.salMonth.ToString().Length > 1 ? objPFLoanReportFilters.salMonth.ToString() : "0" + Convert.ToString(objPFLoanReportFilters.salMonth);
                    currentyear = Convert.ToString(salYear) + vMonth;



                    //if (objPFLoanReportFilters.selectedEmployeeTypeID == 5)
                    {
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPFStatement)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = salMonth });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = salYear });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = EmployeeId });
                            parameterList.Add(new ReportParameter { name = "isPfNo", value = objPFLoanReportFilters.IsPfNo });


                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "PFStatement.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPDSummary)
                        {
                            salMonth = objPFLoanReportFilters.salMonth == 0 ? objPFLoanReportFilters.fromMonth : objPFLoanReportFilters.salMonth;
                            toMonth = objPFLoanReportFilters.AllBranch ? null : (int?)objPFLoanReportFilters.toMonth;
                            var reportLoanSummary = adjustOldLoanService.PFLoanSummary(salMonth, salYear, toMonth);
                            if (reportLoanSummary.Count > 0)
                            {
                                for (int i = 0; i < reportLoanSummary.Count - 1; i++)
                                {
                                    adjustOldLoanService.UpdatePFLoanSummary(reportLoanSummary[i].cpension, reportLoanSummary[i].EMPLOYEECODE, reportLoanSummary[i].SalMonth, salYear, userDetail.UserID);
                                }
                            }

                            if (objPFLoanReportFilters.dateTypeRadio == DateTypeRadio.rbddateRange)
                            {
                                if (objPFLoanReportFilters.AllBranch)
                                    branchCode = null;

                                parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                                parameterList.Add(new ReportParameter { name = "SalMonth", value = objPFLoanReportFilters.fromMonth });
                                parameterList.Add(new ReportParameter { name = "SalYear", value = salYear });
                                reportModel.reportParameters = parameterList;
                                parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                                parameterList.Add(new ReportParameter { name = "employeeid", value = EmployeeId });
                                parameterList.Add(new ReportParameter { name = "isPfNo", value = objPFLoanReportFilters.IsPfNo });
                                reportModel.rptName = "PFSummary.rpt";
                            }


                            //    if (objPFLoanReportFilters.dateTypeRadio == DateTypeRadio.rbddateRange && !objPFLoanReportFilters.AllBranch)
                            //{

                            //    parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            //    parameterList.Add(new ReportParameter { name = "SalFromMonth", value = objPFLoanReportFilters.fromMonth });
                            //    parameterList.Add(new ReportParameter { name = "SalToMonth", value = objPFLoanReportFilters.toMonth });
                            //    parameterList.Add(new ReportParameter { name = "SalYear", value = salYear });
                            //    parameterList.Add(new ReportParameter { name = "empId", value = EmployeeId });
                            //    reportModel.reportParameters = parameterList;
                            //    reportModel.rptName = "PFSummary_EmpWise.rpt";

                            //}
                            //else
                            //{
                            //    if (objPFLoanReportFilters.AllBranch)
                            //    {
                            //        parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            //        parameterList.Add(new ReportParameter { name = "SalMonth", value = objPFLoanReportFilters.fromMonth });
                            //        parameterList.Add(new ReportParameter { name = "SalYear", value = salYear });
                            //        reportModel.reportParameters = parameterList;
                            //        parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            //        parameterList.Add(new ReportParameter { name = "employeeid", value = EmployeeId });
                            //        reportModel.rptName = "PFSummary.rpt";
                            //    }
                            //}

                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPFLDSatement)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = salMonth });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = salYear });
                            parameterList.Add(new ReportParameter { name = "MONTH", value = vMonth });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = EmployeeId });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "PFLoanDeduc.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.LOERefundable)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = salMonth });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = salYear });
                            parameterList.Add(new ReportParameter { name = "CHK", value = "P" });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = EmployeeId });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "PFLoanSanction.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.LOENonRefundable)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = salMonth });
                            parameterList.Add(new ReportParameter { name = "YEAR1", value = salYear });
                            parameterList.Add(new ReportParameter { name = "YEAR2", value = 0 });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = EmployeeId });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "NRPFLoanSanction.rpt";
                            Session["ReportModel"] = reportModel;
                        }

                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.LOERefundableFinished)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = salMonth });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = salYear });
                            parameterList.Add(new ReportParameter { name = "chk", value = "P" });
                            parameterList.Add(new ReportParameter { name = "month", value = vMonth });
                            parameterList.Add(new ReportParameter { name = "preperiod", value = currentyear });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = EmployeeId });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "PFLoanfinish1.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.MPFLLedger)
                        {
                            parameterList.Add(new ReportParameter { name = "PERIODOFPAYMENT", value = currentyear });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = salMonth });
                            parameterList.Add(new ReportParameter { name = "MONTH", value = vMonth });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = salYear });
                            parameterList.Add(new ReportParameter { name = "branchcode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = EmployeeId });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "PFLEDGERNEW.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFLLIAdjust)
                        {
                            parameterList.Add(new ReportParameter { name = "period", value = currentyear });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = EmployeeId });

                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "PF_Adjustinst.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.DReport)
                        {
                            reportModel.rptName = "DependentReport.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.IDReport)
                        {
                            reportModel.rptName = "InDependentReport.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFLTRecovered)
                        {
                            parameterList.Add(new ReportParameter { name = "PeriodofPayment", value = currentyear });
                            parameterList.Add(new ReportParameter { name = "transactiontype", value = "T" });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "rptloantrans.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFLPPayment)
                        {
                            parameterList.Add(new ReportParameter { name = "PeriodofPayment", value = currentyear });
                            parameterList.Add(new ReportParameter { name = "transactiontype", value = "P" });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "rptloantrans.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFLPPayment)
                        {
                            parameterList.Add(new ReportParameter { name = "PeriodofPayment", value = currentyear });
                            parameterList.Add(new ReportParameter { name = "transactiontype", value = "O" });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "rptloantrans.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFChecklist)
                        {
                            var financialYearDetails = objPFLoanReportFilters.financialYear;
                            string fromyear1 = string.Empty, toyear1 = string.Empty;

                            fromyear1 = financialYearDetails.ToString().Substring(0, 4) + "03";
                            toyear1 = financialYearDetails.ToString().Substring(7, 4) + "02";

                            parameterList.Add(new ReportParameter { name = "branchcode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "fyear", value = fromyear1 });
                            parameterList.Add(new ReportParameter { name = "tyear", value = toyear1 });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = EmployeeId });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "pfnaf_chk.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.PFNSummary)
                        {
                            var financialYearDetails = objPFLoanReportFilters.financialYear;
                            string fromyear1 = string.Empty, toyear1 = string.Empty;
                            fromyear1 = financialYearDetails.ToString().Substring(0, 4);
                            toyear1 = financialYearDetails.ToString().Substring(7, 4);
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = null });
                            parameterList.Add(new ReportParameter { name = "fyear", value = fromyear1 });
                            parameterList.Add(new ReportParameter { name = "toyear", value = toyear1 });
                            parameterList.Add(new ReportParameter { name = "brcode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "empid", value = null });
                            parameterList.Add(new ReportParameter { name = "salyear", value = fromyear1 });
                            parameterList.Add(new ReportParameter { name = "tyr", value = toyear1 });

                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "pfsummarynafed.rpt";
                            Session["ReportModel"] = reportModel;
                        }
                        if (objPFLoanReportFilters.pfLoanReportRadio == PFLoanReportRadio.ECRReport)
                        {


                        }
                    }
                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    objPFLoanReportFilters.branchList = dropdownBindService.ddlBranchList();
                    objPFLoanReportFilters.employeeTypesList = dropdownBindService.ddlEmployeeTypeList();
                    var financialYear = ExtensionMethods.GetYearBetweenYearsList(2004, 2044);
                    objPFLoanReportFilters.employeeList = objPFLoanReportFilters.branchId == 0 ? dropdownBindService.GetAllEmployee() : dropdownBindService.employeeByBranchID(objPFLoanReportFilters.branchId);
                    objPFLoanReportFilters.financialYearList = financialYear.Select(x => new SelectListModel { id = 0, value = x }).ToList();
                    return PartialView("_ReportFilters", objPFLoanReportFilters);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ECRExport(int salMonth, int salYear)
        {
            string fileName = string.Empty, msg = string.Empty;
            string fullPath = Server.MapPath("~/FileDownload/");
            fileName = "ECR" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;

            var getdsTable = adjustOldLoanService.GetECRData(salMonth, salYear, null);
            if (getdsTable.Tables.Count > 0)
            {

                DataTable exportData = new DataTable();
                #region DataTable Column Name
                DataColumn dtc0 = new DataColumn("S.No.");
                DataColumn dtc1 = new DataColumn("Member ID");
                DataColumn dtc2 = new DataColumn("PENSION SCHNO");
                DataColumn dtc3 = new DataColumn("PENSION UAN NO");
                DataColumn dtc4 = new DataColumn("Member Name");
                DataColumn dtc5 = new DataColumn("Gross Wages");
                DataColumn dtc6 = new DataColumn("EPF Wages");
                DataColumn dtc7 = new DataColumn("EPS Wages");
                DataColumn dtc8 = new DataColumn("EPF Contr Due");
                DataColumn dtc9 = new DataColumn("EPF Contr being remitted");
                DataColumn dtc10 = new DataColumn("EPS Contr due");
                DataColumn dtc11 = new DataColumn("EPS Contr being remitted");
                DataColumn dtc12 = new DataColumn("Diff EPF and EPS Contr due");
                DataColumn dtc13 = new DataColumn("Diff EPF and EPS Contr being remitted");
                DataColumn dtc14 = new DataColumn("NCP Days");
                DataColumn dtc15 = new DataColumn("Refund of Advance");
                DataColumn dtc16 = new DataColumn("Arrear EPF Wages");
                DataColumn dtc17 = new DataColumn("Arrear EPF EE Share");
                DataColumn dtc18 = new DataColumn("Arrear EPF ER Share");
                DataColumn dtc19 = new DataColumn("Arrear EPS");
                DataColumn dtc20 = new DataColumn("Fathre's/Husband's Name");
                DataColumn dtc21 = new DataColumn("Relationship with the Member");
                DataColumn dtc22 = new DataColumn("Date of Birth");
                DataColumn dtc23 = new DataColumn("Gender");
                DataColumn dtc25 = new DataColumn("Date of Joining EPF");
                DataColumn dtc26 = new DataColumn("Date of Joining EPS");
                DataColumn dtc27 = new DataColumn("Date of Exit From EPF");
                DataColumn dtc28 = new DataColumn("Date of Exit From EPS");
                DataColumn dtc29 = new DataColumn("Reason for Leaving");
                DataColumn dtc30 = new DataColumn("EmployeeCode");
                #endregion

                #region Add column in datetable
                exportData.Columns.Add(dtc0);
                exportData.Columns.Add(dtc1);
                exportData.Columns.Add(dtc2);
                exportData.Columns.Add(dtc3);
                exportData.Columns.Add(dtc4);
                exportData.Columns.Add(dtc5);
                exportData.Columns.Add(dtc6);
                exportData.Columns.Add(dtc7);
                exportData.Columns.Add(dtc8);
                exportData.Columns.Add(dtc9);
                exportData.Columns.Add(dtc10);
                exportData.Columns.Add(dtc11);
                exportData.Columns.Add(dtc12);
                exportData.Columns.Add(dtc13);
                exportData.Columns.Add(dtc14);
                exportData.Columns.Add(dtc15);
                exportData.Columns.Add(dtc16);
                exportData.Columns.Add(dtc17);
                exportData.Columns.Add(dtc18);
                exportData.Columns.Add(dtc19);
                exportData.Columns.Add(dtc20);
                exportData.Columns.Add(dtc21);
                exportData.Columns.Add(dtc22);
                exportData.Columns.Add(dtc23);
                //  exportData.Columns.Add(dtc24);
                exportData.Columns.Add(dtc25);
                exportData.Columns.Add(dtc26);
                exportData.Columns.Add(dtc27);
                exportData.Columns.Add(dtc28);
                exportData.Columns.Add(dtc29);
                exportData.Columns.Add(dtc30);
                #endregion
                for (int i = 0; i < getdsTable.Tables[0].Rows.Count; i++)
                {
                    DataRow row = exportData.NewRow();
                    row[0] = i + 1;
                    row[1] = getdsTable.Tables[0].Rows[i]["PFNO"].ToString();
                    row[2] = getdsTable.Tables[0].Rows[i]["PENSIONSCHNO"].ToString();
                    row[3] = getdsTable.Tables[0].Rows[i]["PENSIONUAN"].ToString();
                    row[4] = getdsTable.Tables[0].Rows[i]["Name"].ToString();
                    row[5] = getdsTable.Tables[0].Rows[i]["Gross Wages"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[0].Rows[i]["Gross Wages"]).ThousandSeprator();
                    row[6] = getdsTable.Tables[0].Rows[i]["EPF Wages"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[0].Rows[i]["EPF Wages"]).ThousandSeprator();
                    row[7] = getdsTable.Tables[0].Rows[i]["EPS Wages"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[0].Rows[i]["EPS Wages"]).ThousandSeprator();
                    row[8] = getdsTable.Tables[0].Rows[i]["EPF Contr Due"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[0].Rows[i]["EPF Contr Due"]).ThousandSeprator();
                    row[9] = getdsTable.Tables[0].Rows[i]["EPF Contr Due"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[0].Rows[i]["EPF Contr Due"]).ThousandSeprator();
                    row[10] = getdsTable.Tables[0].Rows[i]["EPS Contr Due"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[0].Rows[i]["EPS Contr Due"]).ThousandSeprator();
                    row[11] = getdsTable.Tables[0].Rows[i]["EPS Contr Due"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[0].Rows[i]["EPS Contr Due"]).ThousandSeprator();
                    row[12] = getdsTable.Tables[0].Rows[i]["Diff EPF and EPS Contr due"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[0].Rows[i]["Diff EPF and EPS Contr due"]).ThousandSeprator();
                    row[13] = getdsTable.Tables[0].Rows[i]["Diff EPF and EPS Contr due"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[0].Rows[i]["Diff EPF and EPS Contr due"]).ThousandSeprator();
                    row[14] = getdsTable.Tables[0].Rows[i]["NCP Days"].ToString() == "" ? 0 : Convert.ToInt16(getdsTable.Tables[0].Rows[i]["NCP Days"]);
                    // row[15] =  getdsTable.Tables[0].Rows[i][""].ToString();
                    //  row[16] =  getdsTable.Tables[0].Rows[i][""].ToString();
                    //  row[17] =  getdsTable.Tables[0].Rows[i][""].ToString();
                    //  row[18] =  getdsTable.Tables[0].Rows[i][""].ToString();
                    //  row[19] =  getdsTable.Tables[0].Rows[i][""].ToString();
                    row[20] = getdsTable.Tables[0].Rows[i]["Gaurdianname"].ToString();
                    //row[21] =  getdsTable.Tables[0].Rows[i][""].ToString();
                    row[22] = getdsTable.Tables[0].Rows[i]["DOB"].ToString();
                    row[23] = getdsTable.Tables[0].Rows[i]["Gender"].ToString();
                    row[24] = getdsTable.Tables[0].Rows[i]["DateEntitlementps"].ToString();
                    row[25] = getdsTable.Tables[0].Rows[i]["DateEntitlementps"].ToString();
                    row[26] = getdsTable.Tables[0].Rows[i]["doleaveorg"].ToString();
                    row[27] = getdsTable.Tables[0].Rows[i]["doleaveorg"].ToString();
                    row[28] = getdsTable.Tables[0].Rows[i]["LeavingRemarks"].ToString();
                    //row[29] =  getdsTable.Tables[0].Rows[i][""].ToString();
                    row[29] = getdsTable.Tables[0].Rows[i]["EmployeeCode"].ToString();
                    exportData.Rows.Add(row);
                }

                if (getdsTable.Tables[1].Rows.Count > 0)
                {
                    // DataView dvFilter = new DataView(getdsTable.Tables[0]);
                    for (int i = 0; i < getdsTable.Tables[1].Rows.Count; i++) // search whole table
                    {
                        DataRow dr = exportData.AsEnumerable().Where(r => ((string)r["EmployeeCode"]).Equals(getdsTable.Tables[1].Rows[i]["employeecode"])).First();
                        dr["Arrear EPF EE Share"] = getdsTable.Tables[1].Rows[i]["Arrear EPF EE Share"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[1].Rows[i]["Arrear EPF EE Share"]).ThousandSeprator();
                        dr["Arrear EPF ER Share"] = getdsTable.Tables[1].Rows[i]["Arrear EPF ER Share"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[1].Rows[i]["Arrear EPF ER Share"]).ThousandSeprator();
                        dr["Arrear EPF Wages"] = getdsTable.Tables[1].Rows[i]["Arrear EPF Wages"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[1].Rows[i]["Arrear EPF Wages"]).ThousandSeprator();
                    }
                }
                if (getdsTable.Tables[2].Rows.Count > 0)
                {
                    for (int i = 0; i < getdsTable.Tables[2].Rows.Count; i++) // search whole table
                    {
                        DataRow dr = exportData.AsEnumerable().Where(r => ((string)r["EmployeeCode"]).Equals(getdsTable.Tables[2].Rows[i]["employeecode"])).First();
                        dr["Arrear EPF EE Share"] = getdsTable.Tables[2].Rows[i]["Arrear EPF EE Share"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[2].Rows[i]["Arrear EPF EE Share"]).ThousandSeprator();
                        dr["Arrear EPF ER Share"] = getdsTable.Tables[2].Rows[i]["Arrear EPF ER Share"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[2].Rows[i]["Arrear EPF ER Share"]).ThousandSeprator();
                        dr["Arrear EPF Wages"] = getdsTable.Tables[2].Rows[i]["Arrear EPF Wages"].ToString() == "" ? "0" : Convert.ToDecimal(getdsTable.Tables[2].Rows[i]["Arrear EPF Wages"]).ThousandSeprator();
                    }
                }
                //DataSet ds = new DataSet();
                //ds.Tables.Add(exportData);

                exportData.Columns.Remove("EmployeeCode");
                exportData.AcceptChanges();
                adjustOldLoanService.ExportECR(exportData, fullPath, fileName);

                fullPath = $"{fullPath}{fileName}";
                //  return Json(new { fileName = fileName, fullPath = fullPath, message = "success" });
                //  RedirectToAction("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath });
                //return Json(new { IsValidFilter = false }, JsonRequestBehavior.AllowGet);
                return JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");
            }
            else
                return Json(new { message = "error" });
        }
    }

}