using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services.Salary;
using System;
using System.IO;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
using System.Linq;
using System.Diagnostics;

namespace MicroPay.Web.Controllers.Salary
{
    public class GenerateSalaryController : BaseController
    {
        // GET: GenerateSalary
        private readonly IDropdownBindService ddlService;
        private readonly IRegularEmpSalaryService regEmpSalService;
        private readonly IOTAService otaService;
        private readonly IWashingAllowanceService washingAllowanceService;
        private readonly IBankRatesService bankRateSerive;
        private readonly IPayrollApprovalSettingService approvalSettingService;
        public GenerateSalaryController(IDropdownBindService ddlService, IRegularEmpSalaryService regEmpSalService,
            IOTAService otaService, IWashingAllowanceService washingAllowanceService, IBankRatesService bankRateSerive,
            IPayrollApprovalSettingService approvalSettingService)
        {
            this.regEmpSalService = regEmpSalService;
            this.ddlService = ddlService;
            this.otaService = otaService;
            this.washingAllowanceService = washingAllowanceService;
            this.bankRateSerive = bankRateSerive;
            this.approvalSettingService = approvalSettingService;
        }
        public ActionResult Index()
        {
            log.Info($"GenerateSalaryController/Index");
            return View();
        }

        [HttpGet]
        public ActionResult _GenerateSalaryForm()
        {
            log.Info($"GenerateSalary/_GenerateSalaryForm");

            try
            {
                var loggedInEmployeeID = userDetail.EmployeeID.Value;
                var approvalSetting = approvalSettingService.GetApprovalSetting();

                if (!approvalSetting.Any(y => y.ProcessID == (int)WorkFlowProcess.SalaryGenerate
                         && y.Reporting1 == loggedInEmployeeID))
                {
                    return Content(@"<div class='col-lg-12 col-md-12 col-sm-12 col-xs-12 font-weight-bold text-center alert alert-warning'>
                           You are not authorized to access this page.
                      </div>");
                }

                RegularEmployeeSalary regEmpSalary = new RegularEmployeeSalary();
                regEmpSalary.Employees = ddlService.GetAllEmployee();
                regEmpSalary.Branchs = ddlService.ddlBranchList();
                regEmpSalary.EmployeeTypes = ddlService.ddlEmployeeTypeList();
                regEmpSalary.enumEmpCategory = EnumEmpCategory.AllEmployees;
                regEmpSalary.enumBranch = EnumBranch.BranchesExcecptHO;
                regEmpSalary.AllEmployees = true;
                regEmpSalary.BranchesExcecptHO = true;
                regEmpSalary.TCSFilePeriod = (regEmpSalary.salMonth).ToString("00") + regEmpSalary.salYear;

                // Get various allowances rates , that will be used in further salary calculation ===

                var otaMaxRate = otaService.GetMaxOTARate();
                regEmpSalary.otaMaxAmt = otaMaxRate.MaxAmt;
                regEmpSalary.otaMaxRatePerHour = otaMaxRate.MaxRateperHour;

                var washingAllowanceRate = washingAllowanceService.GetLatestWashingAllowance();
                regEmpSalary.washingAllowanceRate = washingAllowanceRate?.Rate;

                var bankRate = bankRateSerive.GeLatestBankRate();
                regEmpSalary.pFLoanAccuralRate = bankRate?.PFLoanAccuralRate ?? 0;
                PayrollMaster.PFLoanRate = bankRate?.PFLoanRate ?? null;

                //  ===== End== ============

                return PartialView("_GenerateSalary", regEmpSalary);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _GenerateSalaryForm(RegularEmployeeSalary regEmpSalary, string ButtonType)
        {
            log.Info($"GenerateSalary/_GenerateSalaryForm");

            regEmpSalary.Employees = ddlService.GetAllEmployee();
            regEmpSalary.Branchs = ddlService.ddlBranchList();
            regEmpSalary.EmployeeTypes = ddlService.ddlEmployeeTypeList();

            try
            {
                // var cc = Convert.ToDecimal("dddd");
                if (ButtonType == "NegSal")
                {
                    if (regEmpSalary.salMonth > 0)
                    {
                        var redirectUrl = new UrlHelper(Request.RequestContext).Action("AdjustSalary", new { mn = regEmpSalary.salMonth, yr = regEmpSalary.salYear });
                        return Json(new { rdct = 1, Url = redirectUrl });
                    }
                    else
                        return PartialView("_GenerateSalary", regEmpSalary);
                }
                if (ButtonType == "Generate Salary")
                {
                    if (regEmpSalary.enumEmpCategory == EnumEmpCategory.SingleEmployee)
                    {
                        ModelState.Remove("selectedEmployeeTypeID");

                        if (!regEmpSalary.selectedEmployeeID.HasValue)
                            ModelState.AddModelError("EmployeeIDRequired", "Please Select Employee.");
                    }
                    if (regEmpSalary.enumBranch == EnumBranch.SingleBranch)
                    {
                        if (!regEmpSalary.selectedBranchID.HasValue)
                            ModelState.AddModelError("BranchIDRequired", "Please Select Branch.");
                    }

                    if (ModelState.IsValid)
                    {
                        //=====  Validation 1 -  To check whether TCS file for this month is uploaded or not ==
                        var tcsFileFound = false;
                        regEmpSalary.TCSFilePeriod = (regEmpSalary.salMonth).ToString("00") + regEmpSalary.salYear;
                        var tcsFileName = regEmpSalService.GetMonthlyFileName(regEmpSalary.TCSFilePeriod);

                        if (!string.IsNullOrEmpty(tcsFileName))
                            tcsFileFound = System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                DocumentUploadFilePath.TCSFilePath + "/" + tcsFileName));

                        if (!string.IsNullOrEmpty(tcsFileName))
                        {
                            if (tcsFileFound)
                            {
                                //=== Validation 2- To ensure whether the required data inputs are available or not ===
                                //        Stopwatch sw = new Stopwatch();
                                //    sw.Start();
                                var baseModel = regEmpSalService.DataInputsValidation(regEmpSalary);
                                log.Info($"Successed={baseModel.IsSalaryCalculationDone}");

                                if (baseModel.IsSalaryCalculationDone)
                                {
                                    if (baseModel.NegativeSalEmp != null && baseModel.NegativeSalEmp.Count > 0)
                                    {
                                        regEmpSalary.CustomErrorFound = true;
                                        regEmpSalary.CustomErrorMsg += $"<ul> <b>Salary Generated Successfully.</b> <br> Net Salary of the following employee code(s) is in negative - ";
                                        foreach (var item in baseModel.NegativeSalEmp)
                                            regEmpSalary.CustomErrorMsg += $"<li>{item}</li>";
                                        regEmpSalary.CustomErrorMsg += "Please click on Adjust Negative Salary Button, to adjust salary.</ul>";
                                        return PartialView("_GenerateSalary", regEmpSalary);
                                    }
                                    else
                                    {
                                        //Update BranchId_Pay
                                        regEmpSalService.UpdateDateBranch_Pay(regEmpSalary.salMonth, regEmpSalary.salYear);

                                        return Json(new { success = "1", msg = "Salary calculated successfully." }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                //===  End of Validation 2 =============================================================
                                //     sw.Stop();
                                //    var timeTakenInMS = sw.ElapsedMilliseconds;
                            }
                            else
                            {
                                regEmpSalary.CustomErrorFound = true;
                                regEmpSalary.CustomErrorMsg = $"TCS file does not exists.";
                            }
                        }
                        else
                        {
                            regEmpSalary.CustomErrorFound = true;
                            regEmpSalary.CustomErrorMsg = $"Please Upload TCS File for this month.";
                        }
                    }
                    else
                    {
                        return PartialView("_GenerateSalary", regEmpSalary);
                    }
                }
                else if (ButtonType == "Send for Approval")
                {
                    regEmpSalary.loggedInUserID = userDetail.UserID;
                    if (regEmpSalary.enumEmpCategory == EnumEmpCategory.SingleEmployee)
                    {
                        ModelState.Remove("selectedEmployeeTypeID");

                        if (!regEmpSalary.selectedEmployeeID.HasValue)
                            ModelState.AddModelError("EmployeeIDRequired", "Please Select Employee.");
                    }
                    if (regEmpSalary.enumBranch == EnumBranch.SingleBranch)
                    {
                        if (!regEmpSalary.selectedBranchID.HasValue)
                            ModelState.AddModelError("BranchIDRequired", "Please Select Branch.");
                    }
                    if (ModelState.IsValid)
                    {
                        var baseModel = regEmpSalService.SendApprovalRequest(regEmpSalary);
                    }
                }
                else
                {
                    //=== = == = 
                    regEmpSalary.loggedInUserID = userDetail.UserID;
                    if (regEmpSalary.enumEmpCategory == EnumEmpCategory.SingleEmployee)
                    {
                        ModelState.Remove("selectedEmployeeTypeID");

                        if (!regEmpSalary.selectedEmployeeID.HasValue)
                            ModelState.AddModelError("EmployeeIDRequired", "Please Select Employee.");
                    }
                    if (regEmpSalary.enumBranch == EnumBranch.SingleBranch)
                    {
                        if (!regEmpSalary.selectedBranchID.HasValue)
                            ModelState.AddModelError("BranchIDRequired", "Please Select Branch.");
                    }
                    if (ModelState.IsValid)
                        regEmpSalary = regEmpSalService.RevertProcessedLoanEntries(regEmpSalary);


                    if (regEmpSalary.Reverted)
                        // return Json(new { success="1",HTMLDATA= ConvertViewToString("_GenerateSalary", baseModel) });
                        return Json(new { success = "1", msg = "Loan entries reversed successfully." }, JsonRequestBehavior.AllowGet);
                }
                return PartialView("_GenerateSalary", regEmpSalary);
            }
            catch (Exception ex)
            {
                regEmpSalary.CustomErrorFound = true;
                if (ex.Message == "Input string was not in a correct format.")
                    regEmpSalary.CustomErrorMsg = "TCS File - You entered a string in a numeric data field or formula, Please check and remove it and generate salary after reuploding TCS file.";
                else
                    regEmpSalary.CustomErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.StackTrace;
                return PartialView("_GenerateSalary", regEmpSalary);
            }
        }

        //  [HttpPost]
        public ActionResult _GeneratedSalaryGridView(FormCollection frm)
        {
            log.Info($"GenerateSalary/_GeneratedSalaryGridView");
            try
            {
                var selectedSalYear = frm?.Get("salYear") != null ? int.Parse(frm.Get("salYear").ToString()) : (int?)null;

                var loggedInEmployeeID = userDetail.EmployeeID.Value;
                var approvalSetting = approvalSettingService.GetApprovalSetting();

                if (!approvalSetting.Any(y => y.ProcessID == (int)WorkFlowProcess.SalaryGenerate
                         && y.Reporting1 == loggedInEmployeeID))
                {
                    return Content(@"<script>$('#dv-right-section').addClass('hide');$('#dv-left-section').removeClass('col-lg-6 col-md-6').addClass('col-lg-12 col-md-12'); </script>");
                }

                if (selectedSalYear.HasValue)
                {
                    var objList = regEmpSalService.GeneratedSalaryList(selectedSalYear.Value);
                    return PartialView("_GeneratedSalaryGridView", objList);
                }
                else
                    return Content("No Records Found");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult AdjustSalary(int mn, int yr)
        {
            ViewBag.ddlEmployee = ddlService.GetNegativeSalEmployee(yr, mn);
            ViewBag.salMonth = mn;
            ViewBag.salYear = yr;
            return View();
        }
        public PartialViewResult _GetEmployeeSalary(RegularEmployeeSalary salFilter)
        {
            log.Info($"GenerateSalary/_GetEmployeeSalary");
            try
            {
                if (!salFilter.selectedEmployeeID.HasValue)
                {
                    ModelState.AddModelError("EmployeeError", "Please select Employee Code.");
                    return PartialView("_Filter", salFilter);
                }
                var empSal = regEmpSalService.GetEmployeeSalary(salFilter.selectedEmployeeID.Value, salFilter.salYear, salFilter.salMonth);
                return PartialView("_NegativeSalaryFields", empSal);

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut]
        public JsonResult UpdateEmployeeSalary(FinalMonthlySalary objSalary)
        {
            log.Info($"GenerateSalary/_GetEmployeeSalary");
            try
            {
                var res = regEmpSalService.UpdateEmployeeSalary(objSalary);
                return Json(new { success = res });

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}