using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
using System.Linq;
using System.Collections.Generic;
using Nafed.MicroPay.Services.Salary;

namespace MicroPay.Web.Controllers.Reports.Payroll
{
    public class YearlyReportController : BaseController
    {
        // GET: YearlyReport
        private readonly IDropdownBindService dropdownBindService;
        private readonly IEmployeeService employeeService;
        private readonly IAdjustOldLoanService adjustOldLoanService;

        public YearlyReportController(IDropdownBindService dropdownBindService, IEmployeeService employeeService, IAdjustOldLoanService adjustOldLoanService)
        {
            this.dropdownBindService = dropdownBindService;
            this.employeeService = employeeService;
            this.adjustOldLoanService = adjustOldLoanService;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _YearlyRegisterReportDetails()
        {
            log.Info("YearlyReport/_YearlyRegisterReportDetails");
            try
            {
                YearlyReportFilters objYearlyReportFilters = new YearlyReportFilters();
                objYearlyReportFilters.branchList = dropdownBindService.ddlBranchList();
                objYearlyReportFilters.employeeTypesList = dropdownBindService.ddlEmployeeTypeList();
                var financialYear = ExtensionMethods.GetYearBetweenYearsList(2004, 2044);
                objYearlyReportFilters.financialYearList = financialYear.Select(x => new SelectListModel { id = 0, value = x }).ToList();
                objYearlyReportFilters.employeeList = dropdownBindService.GetAllEmployee();
                objYearlyReportFilters.yearly = true;
                return PartialView("_ReportFilters", objYearlyReportFilters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _YearlyRegisterReport(YearlyReportFilters objYearlyReportFilters)
        {
            log.Info("YearlyReport/_YearlyRegisterReport");
            try
            {
                bool dateRngFlg = false;
                if ((!objYearlyReportFilters.AllBranch) && objYearlyReportFilters.branchId == 0)
                    ModelState.AddModelError("BranchIdRequired", "Branch Required");
                if ((!objYearlyReportFilters.AllEmployee) && objYearlyReportFilters.employeeId == 0)
                    ModelState.AddModelError("EmployeeIdRequired", "Employee Required");
                if (objYearlyReportFilters.fltrRadio == FilterRadio.rbdyearly && objYearlyReportFilters.financialYear == "Select")
                    ModelState.AddModelError("finanicalYearRequired", "Finanical Year Required");
                //if (objYearlyReportFilters.selectedEmployeeTypeID == 0)
                //    ModelState.AddModelError("employeeTypeRequired", "Employee Type Required");
                if (objYearlyReportFilters.fromYear == 0 || objYearlyReportFilters.fromMonth == 0 || objYearlyReportFilters.toMonth == 0 || objYearlyReportFilters.toYear == 0)
                    dateRngFlg = true;
                if (objYearlyReportFilters.fltrRadio == FilterRadio.rbddateRange && dateRngFlg)
                    ModelState.AddModelError("DateRangeRequired", "Date Range Required");

                if (ModelState.IsValid)
                {
                    string rptName = string.Empty, branchCode = string.Empty, fYear = string.Empty,
                        toYear = string.Empty, employeeCode = string.Empty;
                    decimal? bankRate;
                    int? empId;
                    int? employeeTypeId;
                    int salMonth = 0;
                    if (objYearlyReportFilters.branchId != 0)
                        branchCode = dropdownBindService.GetBranchCode(objYearlyReportFilters.branchId);
                    else
                        branchCode = null;

                    salMonth = objYearlyReportFilters.salMonth;
                    if (objYearlyReportFilters.fltrRadio == FilterRadio.rbdyearly)
                    {
                        fYear = objYearlyReportFilters.financialYear.Substring(0, 4);
                        toYear = objYearlyReportFilters.financialYear.Substring(7, 4);
                    }
                    if (objYearlyReportFilters.employeeId != 0)
                    {
                        empId = objYearlyReportFilters.employeeId;
                        employeeCode = employeeService.GetEmployeeByID(objYearlyReportFilters.employeeId).EmployeeCode;
                    }
                    else
                    {
                        empId = null;
                        employeeCode = null;
                    }

                    employeeTypeId = objYearlyReportFilters.selectedEmployeeTypeID == 0 ? null : (int?)objYearlyReportFilters.selectedEmployeeTypeID;

                    bankRate = objYearlyReportFilters.bankRate;

                    //if (objYearlyReportFilters.selectedEmployeeTypeID == 5)
                    {
                        BaseReportModel reportModel = new BaseReportModel();
                        List<ReportParameter> parameterList = new List<ReportParameter>();
                        reportModel.reportType = 2;

                        #region ======  LICRegister  =============

                        if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.LICRegister)
                        {
                            rptName = "LICRegister.rpt";
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });

                        }

                        #endregion

                        #region === TCSRecoveryRegister ====== 

                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.TCSRegister)
                        {
                            rptName = "TCSRecoveryRegister.rpt";
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                        }
                        #endregion

                        #region === Welfare Fund ================ 
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.WelfareFundRegister)
                        {
                            rptName = "BENEFUNDREGISTER.rpt";
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                        }
                        #endregion

                        #region === Car Loan Register ==========
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.CarLoanRegister)
                        {
                            rptName = "CarRegister.rpt";
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                        }
                        #endregion

                        #region === Scooter Loan Register ========
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.ScooterLoanRegister)
                        {
                            rptName = "ScooterRegister.rpt";
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                        }

                        #endregion

                        #region === Festival Loan Register ===========
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.FestivalLoanRegister)
                        {
                            rptName = "FestivalLoanRegister.rpt";
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                        }
                        #endregion

                        #region =======House Building Register ===========
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.HouseBuildingRegister)
                        {
                            rptName = "HouseBuildingRegister.rpt";
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                        }

                        #endregion

                        #region ===== PF Loan Register ==========
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.PFLoanRegister)
                        {
                            rptName = "PFLoanRegister.rpt";

                            parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                        }
                        #endregion

                        #region === Leave Encashment ==================


                        #endregion

                        #region === Gratuity LIC New Employees=====

                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.GratuityLICNewEmployees)
                        {
                            parameterList.Add(new ReportParameter { name = "year", value = fYear });
                            // parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            // parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                            //  parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                            // parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "GRATUTITY_LIC_IN.rpt";
                        }
                        #endregion

                        #region =======  Gratuity LIC Employees Left =============

                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.GratuityLICEmployeesLeft)
                        {
                            rptName = "GRATUTITY_LIC_OUT.rpt";
                            parameterList.Add(new ReportParameter { name = "BRANCHCODE", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "YEAR1", value = fYear });
                            parameterList.Add(new ReportParameter { name = "YEAR2", value = toYear });
                            //parameterList.Add(new ReportParameter { name = "EMPTYPE", value = objYearlyReportFilters.selectedEmployeeTypeID });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });

                        }
                        #endregion

                        #region === Leave Encashment Register(For 1 Year) === 

                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.LeaveEncashmentRegisterFor1Year)
                        {
                            DateTime cutOffDate = new DateTime(int.Parse(toYear), 03, 31);

                            if (objYearlyReportFilters.AllEmployee && objYearlyReportFilters.AllBranch)
                            {
                                rptName = "LEnCASH.rpt";
                                parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                                parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                                parameterList.Add(new ReportParameter { name = "cutoffdate", value = cutOffDate });
                                parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                                parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            }
                            else
                            {
                                rptName = "LeaveEncashment.rpt";
                                parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                                parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                                parameterList.Add(new ReportParameter { name = "cutoffdate", value = cutOffDate });
                                parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                                parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            }

                        }
                        #endregion


                        #region  ====   Leave Encashment Register(for Total Year) =============
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.LeaveEncashmentRegisterForTotalYear)
                        {
                            DateTime cutOffDate = new DateTime(int.Parse(toYear), 03, 31);

                            rptName = "LeaveEncashment.rpt";
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "cutoffdate", value = cutOffDate });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                        }

                        #endregion


                        #region Income Tax Register
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.IncomeTaxRegister)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = Convert.ToInt16(fYear) });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = Convert.ToInt16(toYear) });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "IncomeTaxRegister.rpt";
                        }
                        #endregion Income Tax Register

                        #region Certificate of Interest of HBA
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.CertificateofInterestofHBA)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = Convert.ToInt16(fYear) });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = Convert.ToInt16(toYear) });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "HBALOANSTATEMENT.rpt";
                        }
                        #endregion

                        #region Form 7 (Singe Year)
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.Form7SingeYear)
                        {
                            fYear = objYearlyReportFilters.financialYear.Substring(0, 4) + "03";
                            toYear = objYearlyReportFilters.financialYear.Substring(7, 4) + "02";

                            parameterList.Add(new ReportParameter { name = "YEAR1", value = Convert.ToInt32(fYear) });
                            parameterList.Add(new ReportParameter { name = "YEAR2", value = Convert.ToInt32(toYear) });
                            parameterList.Add(new ReportParameter { name = "PENSIONRATE", value = "8.33%" });
                            parameterList.Add(new ReportParameter { name = "BRANCHCODE", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "FORM7PS.rpt";
                        }
                        #endregion

                        #region Form 8 (PS)
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.Form8PS)
                        {
                            fYear = objYearlyReportFilters.financialYear.Substring(0, 4) + "03";
                            toYear = objYearlyReportFilters.financialYear.Substring(7, 4) + "02";

                            parameterList.Add(new ReportParameter { name = "YEAR1", value = Convert.ToInt32(fYear) });
                            parameterList.Add(new ReportParameter { name = "YEAR2", value = Convert.ToInt32(toYear) });
                            parameterList.Add(new ReportParameter { name = "PENSIONRATE", value = "8.33%" });
                            parameterList.Add(new ReportParameter { name = "EMPTYPE", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "BRANCHCODE", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "FORMS8PS.rpt";
                        }
                        #endregion

                        #region Form 3 (PS)
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.Form3PS)
                        {
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "FORM3PS.rpt";
                        }

                        #endregion

                        #region PF Total
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.PFTotal)
                        {
                            fYear = objYearlyReportFilters.financialYear.Substring(0, 4) + "03";
                            toYear = objYearlyReportFilters.financialYear.Substring(7, 4) + "02";
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "Rate", value = bankRate });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "RPTPFTotalEmployee.rpt";
                        }
                        #endregion

                        #region PF Yearly Report
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.PFYEARLYREPORT)
                        {
                            fYear = objYearlyReportFilters.financialYear.Substring(0, 4) + "03";
                            toYear = objYearlyReportFilters.financialYear.Substring(7, 4) + "02";
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = Convert.ToInt32(fYear) });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = Convert.ToInt32(toYear) });
                            parameterList.Add(new ReportParameter { name = "Rate", value = bankRate });
                            parameterList.Add(new ReportParameter { name = "empCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "rptPFIndividual.rpt";
                        }
                        #endregion

                        #region AnuualPFRegister -- Procedure Not Found
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.AnuualPFRegister)
                        {
                            var fromYear = objYearlyReportFilters.financialYear.Substring(0, 4) + "06";
                            var tYear = objYearlyReportFilters.financialYear.Substring(7, 4) + "02";

                            fYear = objYearlyReportFilters.financialYear.Substring(0, 4);
                            toYear = objYearlyReportFilters.financialYear.Substring(7, 4);
                            var tot = adjustOldLoanService.AnuualPFRegister(fromYear, tYear);

                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = Convert.ToInt16(fYear) });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = Convert.ToInt16(toYear) });

                            rptName = "REPORT_PF_ANNUAL_NEW.rpt";
                        }
                        #endregion

                        #region PFIUYearlyReport -- IUyearlyreport, sp_PFYearlyLoanLedgerIU
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.PFIUYearlyReport)
                        {
                            var fromYear = objYearlyReportFilters.financialYear.Substring(0, 4) + "03";
                            var tYear = objYearlyReportFilters.financialYear.Substring(7, 4) + "02";

                            fYear = objYearlyReportFilters.financialYear.Substring(0, 4);
                            toYear = objYearlyReportFilters.financialYear.Substring(7, 4);
                            var tot = adjustOldLoanService.PFIUYearlyReport(fromYear, tYear);

                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "EmployeeCode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "fyear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "toyear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "tot", value = tot });
                            parameterList.Add(new ReportParameter { name = "PnsnRate", value = null });
                            rptName = "IUyearlyreport.rpt";
                        }
                        #endregion

                        #region EDLIStatement -- rptEDLINafed, SP_Nafed_EDLI
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.EDLIStatement)
                        {
                            var mon = salMonth;
                            var yr1 = DateTime.Now.Year;
                            var flag = adjustOldLoanService.EDLIStatement(mon, yr1);
                            parameterList.Add(new ReportParameter { name = "mon", value = mon });
                            parameterList.Add(new ReportParameter { name = "fromyear", value = yr1 });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "rptEDLINafed.rpt";
                        }
                        #endregion

                        #region FORM7PSNew -- Form7PSNew, Form7PS
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.Form7MultipleYears)
                        {
                            string fryear = string.Empty, tryear = string.Empty, fryear1 = string.Empty, tryear1 = string.Empty;
                            for (int i = objYearlyReportFilters.fromYear; i <= objYearlyReportFilters.toYear - 1; i++)
                                fryear = fryear + "," + i.ToString() + "03";
                            for (int i = objYearlyReportFilters.fromYear + 1; i <= objYearlyReportFilters.toYear; i++)
                                tryear = tryear + "," + i.ToString() + "02";

                            fryear = fryear.Substring(1);
                            tryear = tryear.Substring(1);
                            fryear1 = (Convert.ToString(objYearlyReportFilters.fromMonth).Length == 1 ? "0" + Convert.ToString(objYearlyReportFilters.fromMonth) : Convert.ToString(objYearlyReportFilters.fromMonth));
                            fryear1 = fryear1 + "-" + Convert.ToString(objYearlyReportFilters.fromYear);
                            tryear1 = (Convert.ToString(objYearlyReportFilters.toMonth).Length == 1 ? "0" + Convert.ToString(objYearlyReportFilters.toMonth) : Convert.ToString(objYearlyReportFilters.toMonth));
                            tryear1 = tryear1 + "-" + Convert.ToString(objYearlyReportFilters.toYear);

                            adjustOldLoanService.Update_FORM7PSdata(fryear, tryear);
                            parameterList.Add(new ReportParameter { name = "fryear1", value = "2018" });
                            parameterList.Add(new ReportParameter { name = "tryear1", value = tryear1 });

                            rptName = "FORM7PSNew.rpt";
                        }
                        #endregion

                        #region InterestcalculationonHouseBuildingLoan -- RptInterestHBA, SP_HBLInterestCalculation,PFLOAN2
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.InterestcalculationonHouseBuildingLoan)
                        {
                            var finYear = objYearlyReportFilters.financialYear;
                            int fperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(0, 4) + "04");
                            int tperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(7, 4) + "03");
                            parameterList.Add(new ReportParameter { name = "Fyear", value = finYear });
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "fperiod", value = fperiod });
                            parameterList.Add(new ReportParameter { name = "tperiod", value = tperiod });
                            parameterList.Add(new ReportParameter { name = "rate", value = Convert.ToInt32(bankRate) });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "RptInterestHBA.rpt";
                        }
                        #endregion

                        #region InterestcalculationonCarLoan -- rptinterestCARrpt, SP_CarInterestCalculation
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.InterestcalculationonCarLoan)
                        {
                            var finYear = objYearlyReportFilters.financialYear;
                            int fperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(0, 4) + "04");
                            int tperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(7, 4) + "03");
                            parameterList.Add(new ReportParameter { name = "Fyear", value = finYear });
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "fperiod", value = fperiod });
                            parameterList.Add(new ReportParameter { name = "tperiod", value = tperiod });
                            parameterList.Add(new ReportParameter { name = "brcode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "rate", value = Convert.ToInt32(bankRate) });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "rptinterestCARrpt.rpt";
                        }
                        #endregion

                        #region InterestcalculationonScooterLoan -- RptInterestScooter, SP_ScooterInterestCalculation
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.InterestcalculationonScooterLoan)
                        {
                            var finYear = objYearlyReportFilters.financialYear;
                            int fperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(0, 4) + "04");
                            int tperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(7, 4) + "03");
                            parameterList.Add(new ReportParameter { name = "Fyear", value = finYear });
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "fperiod", value = fperiod });
                            parameterList.Add(new ReportParameter { name = "tperiod", value = tperiod });
                            parameterList.Add(new ReportParameter { name = "brcode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "rate", value = Convert.ToInt32(bankRate) });

                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });

                            rptName = "RptInterestScooter.rpt";
                        }
                        #endregion

                        #region HouseBuildingLoanDeductionStatement -- RptInterestScooter, SP_ScooterInterestCalculation
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.HouseBuildingLoanDeductionStatement)
                        {
                            var finYear = objYearlyReportFilters.financialYear;
                            int fperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(0, 4) + "04");
                            int tperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(7, 4) + "03");
                            parameterList.Add(new ReportParameter { name = "Fyear", value = finYear });
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "fperiod", value = fperiod });
                            parameterList.Add(new ReportParameter { name = "tperiod", value = tperiod });
                            parameterList.Add(new ReportParameter { name = "brcode", value = branchCode });
                            parameterList.Add(new ReportParameter { name = "rate", value = Convert.ToInt32(bankRate) });

                            rptName = "HbLoanDeduction.rpt";
                        }
                        #endregion

                        #region HBACERTIFICATION -- RptHBACertificate, SP_HBLInterestCalculation
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.HBACERTIFICATION)
                        {
                            var finYear = objYearlyReportFilters.financialYear;
                            int fperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(0, 4) + "04");
                            int tperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(7, 4) + "03");
                            parameterList.Add(new ReportParameter { name = "Fyear", value = finYear });
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "fperiod", value = fperiod });
                            parameterList.Add(new ReportParameter { name = "tperiod", value = tperiod });
                            parameterList.Add(new ReportParameter { name = "rate", value = Convert.ToInt32(bankRate) });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "RptHBACertificate.rpt";
                        }
                        #endregion

                        #region PFSummary -- RptPFSummaryNew, sp_PFIndividual
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.PFSummary)
                        {
                            int fperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(0, 4) + "03");
                            int tperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(7, 4) + "02");
                            parameterList.Add(new ReportParameter { name = "SalFYear", value = fperiod });
                            parameterList.Add(new ReportParameter { name = "SalToYear", value = tperiod });
                            parameterList.Add(new ReportParameter { name = "Rate", value = Convert.ToDecimal(bankRate) });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "RptPFSummaryNew.rpt";
                        }
                        #endregion

                        #region HBACERTIFICATIONBRANCHWISE -- RptHBACertificateBranchwise, SP_HBLInterestCalculation
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.HBACERTIFICATIONBRANCHWISE)
                        {
                            var finYear = objYearlyReportFilters.financialYear;
                            int fperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(0, 4) + "04");
                            int tperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(7, 4) + "03");
                            parameterList.Add(new ReportParameter { name = "Fyear", value = finYear });
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                            parameterList.Add(new ReportParameter { name = "fperiod", value = fperiod });
                            parameterList.Add(new ReportParameter { name = "tperiod", value = tperiod });
                            parameterList.Add(new ReportParameter { name = "rate", value = Convert.ToInt32(bankRate) });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });
                            rptName = "RptHBACertificateBranchwise.rpt";
                        }
                        #endregion

                        #region PFLoanYearly -- PFLoanLedgerYearly, SP_PF_loan_ledger_yearly
                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.PFLoanYearly)
                        {
                            int fperiod;
                            int tperiod;

                            DateTime fdate;
                            DateTime tdate;
                            string sfdate = string.Empty;
                            string stdate = string.Empty;

                            if (objYearlyReportFilters.fltrRadio == FilterRadio.rbddateRange)
                            {
                                int lastDayOfMonth = DateTime.DaysInMonth(objYearlyReportFilters.toYear, objYearlyReportFilters.toMonth);
                                if (objYearlyReportFilters.fromMonth != 10 && objYearlyReportFilters.fromMonth != 11 && objYearlyReportFilters.fromMonth != 12)
                                {
                                    fperiod = Convert.ToInt32(objYearlyReportFilters.fromYear.ToString() + "0" + objYearlyReportFilters.fromMonth.ToString());

                                }
                                else
                                {
                                    fperiod = Convert.ToInt32(objYearlyReportFilters.fromYear.ToString() + objYearlyReportFilters.fromMonth.ToString());


                                }

                                if (objYearlyReportFilters.toMonth != 10 && objYearlyReportFilters.toMonth != 11 && objYearlyReportFilters.toMonth != 12)
                                {

                                    tperiod = Convert.ToInt32(objYearlyReportFilters.toYear.ToString() + "0" + objYearlyReportFilters.toMonth.ToString());
                                }
                                else
                                {

                                    tperiod = Convert.ToInt32(objYearlyReportFilters.toYear.ToString() + objYearlyReportFilters.toMonth.ToString());

                                }

                                fdate = Convert.ToDateTime(objYearlyReportFilters.fromYear.ToString() + "-" + objYearlyReportFilters.fromMonth.ToString() + "-01");

                                sfdate = fdate.Year + "-" + (fdate.Month.ToString().Length == 1 ? "0" + fdate.Month.ToString() : fdate.Month.ToString()) + "-01";

                                tdate = Convert.ToDateTime(objYearlyReportFilters.toYear.ToString() + "-" + objYearlyReportFilters.toMonth.ToString() + "-" + lastDayOfMonth.ToString());

                                stdate = tdate.Year + "-" + (tdate.Month.ToString().Length == 1 ? "0" + tdate.Month.ToString() : tdate.Month.ToString()) + "-" + (tdate.Day.ToString().Length == 1 ? "0" + tdate.Day.ToString() : tdate.Day.ToString());

                            }
                            else
                            {
                                fperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(0, 4) + "03");
                                tperiod = Convert.ToInt32(objYearlyReportFilters.financialYear.Substring(7, 4) + "02");

                                fdate = Convert.ToDateTime(objYearlyReportFilters.financialYear.Substring(0, 4) + "-04-01");

                                sfdate = fdate.Year + "-04-01";
                                tdate = Convert.ToDateTime(objYearlyReportFilters.financialYear.Substring(7, 4) + "-03-31");
                                stdate = tdate.Year + "-03-31";
                            }


                            parameterList.Add(new ReportParameter { name = "FROMPERIOD", value = fperiod });
                            parameterList.Add(new ReportParameter { name = "TOPERIOD", value = tperiod });
                            parameterList.Add(new ReportParameter { name = "fromdate", value = sfdate });
                            parameterList.Add(new ReportParameter { name = "todate", value = stdate });
                            parameterList.Add(new ReportParameter { name = "empId", value = empId });

                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });

                            rptName = "PFLoanLedgerYearly.rpt";
                        }
                        #endregion

                        #region EMPLOYEES OF BALANCES LYING MORE THAN THREE YEARS

                        else if (objYearlyReportFilters.yearlyReportRadio == YearlyReportRadio.PFBalMoreThan3Years)
                        {
                            parameterList.Add(new ReportParameter { name = "fYear", value = fYear });
                            parameterList.Add(new ReportParameter { name = "toYear", value = toYear });
                            parameterList.Add(new ReportParameter { name = "emptype", value = employeeTypeId });
                            parameterList.Add(new ReportParameter { name = "employeeid", value = empId });

                            rptName = "PFEMployeeLyingMoreThan3Years.rpt";
                        }

                        #endregion

                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                    }

                    objYearlyReportFilters.branchList = dropdownBindService.ddlBranchList();
                    objYearlyReportFilters.employeeTypesList = dropdownBindService.ddlEmployeeTypeList();
                    var financialYear = ExtensionMethods.GetYearBetweenYearsList(2004, 2044);
                    objYearlyReportFilters.financialYearList = financialYear.Select(x => new SelectListModel { id = 0, value = x }).ToList();
                    objYearlyReportFilters.employeeList = dropdownBindService.GetAllEmployee();
                    return PartialView("_ReportFilters", objYearlyReportFilters);

                }
                else
                {
                    objYearlyReportFilters.branchList = dropdownBindService.ddlBranchList();
                    objYearlyReportFilters.employeeTypesList = dropdownBindService.ddlEmployeeTypeList();
                    var financialYear = ExtensionMethods.GetYearBetweenYearsList(2004, 2044);
                    objYearlyReportFilters.financialYearList = financialYear.Select(x => new SelectListModel { id = 0, value = x }).ToList();
                    objYearlyReportFilters.employeeList = dropdownBindService.GetAllEmployee();
                    return PartialView("_ReportFilters", objYearlyReportFilters);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public JsonResult GetEmployeeByBranch(int brnId)
        {
            log.Info($"YearlyReport/GetEmployeeByBranch/branchId={brnId}");
            try
            {
                var employeeList = brnId == 0 ? dropdownBindService.GetAllEmployee() : dropdownBindService.employeeByBranchID(brnId);
                SelectListModel selectemployeeDetails = new SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "Select";
                employeeList.Insert(0, selectemployeeDetails);
                var emp = new SelectList(employeeList, "id", "value");

                return Json(new { emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }


    }
}