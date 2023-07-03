using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using AutoMapper;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers
{
    public class EmpSalaryConfigController : BaseController
    {
        private readonly IDropdownBindService dropdownService;
        private readonly IEmployeeService employeeService;
        public EmpSalaryConfigController(IDropdownBindService dropdownService, IEmployeeService employeeService)
        {
            this.dropdownService = dropdownService;
            this.employeeService = employeeService;
        }
        // GET: SalaryHead
        public ActionResult Index()
        {
            log.Info($"EmpSalaryConfigController/Index");
            SalaryConfigurationViewModel salaryConfigVM = new SalaryConfigurationViewModel();
            salaryConfigVM.branchList = GetBranchList();
            return View(salaryConfigVM);
        }

        [HttpPost]
        public ActionResult _GetSalaryConfigGridView(SalaryConfigurationViewModel salaryHeadVM, string ButtonType)
        {
            log.Info($"EmpSalaryConfigController/_GetSalaryConfigGridView");
            List<Employee> objEmpList = new List<Employee>();

            if (ButtonType.Equals("Search", StringComparison.OrdinalIgnoreCase))
            {
                if (salaryHeadVM.BranchID > 0)
                {
                    if (ButtonType.Equals("Search", StringComparison.OrdinalIgnoreCase))
                        objEmpList = employeeService.GetEmployeesByBranchID(salaryHeadVM.BranchID);
                }
                else
                {
                    objEmpList = employeeService.GetEmployeesByBranchID(0);  /// ==== Get all employee's of selected branch
                    //  ModelState.AddModelError("BranchIDRequired", "Please select branch.");
                    //  return PartialView("Index", salaryHeadVM);
                }

                return PartialView("_SalaryConfigGridView", objEmpList);
            }
            else
            {
                string fileName = string.Empty, msg = string.Empty, result = string.Empty;
                string fullPath = Server.MapPath("~/FileDownload/");
                fileName = ExtensionMethods.SetUniqueFileName($"EmpSalaryConfig-", FileExtension.xlsx);

                result = employeeService.ExportBrachWiseSalaryConfiguration(salaryHeadVM.BranchID, fileName, fullPath);

                if (result == "notfound")
                    ModelState.AddModelError("ReportTypeRequired", "No Record Found.");
                else
                    return Json(new
                    {
                        fileName = fileName,
                        fullPath = fullPath + fileName,
                        message = "success"
                    });
                return PartialView("_SalaryConfigGridView", objEmpList);
            }
        }

        [HttpGet]
        public ActionResult _GetSalaryConfigDtls(int branchID, string empCode)
        {
            log.Info($"EmpSalaryConfigController/_GetSalaryConfigDtls");
            EmpSalaryConfiguration salHead = new EmpSalaryConfiguration();
            var empSalaryData = employeeService.GetEmployeeSalaryDtls(empCode);
            if (empSalaryData != null)
            {
                Mapper.Initialize(cfg =>
                {

                    cfg.CreateMap<EmployeeSalary, EmpSalaryConfiguration>()

                     .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.EmployeeName))
                         .ForMember(d => d.SuspensionPeriods, o => o.MapFrom(s => s.SuspensionPeriods))
                         .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                         .ForMember(d => d.EmpCode, o => o.MapFrom(s => s.EmployeeCode))
                         .ForMember(d => d.E_Basic, o => o.MapFrom(s => s.E_Basic))
                         .ForMember(d => d.HRA, o => o.MapFrom(s => s.HRA ?? false))
                         .ForMember(d => d.IsFlatDeduction, o => o.MapFrom(s => s.IsFlatDeduction))
                         .ForMember(d => d.None, o => o.MapFrom(s => s.None))
                         .ForMember(d => d.EmpBranchID, o => o.MapFrom(s => s.BranchID))
                         .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                         .ForMember(d => d.CCA, o => o.MapFrom(s => s.CCA))
                         .ForMember(d => d.WASHING, o => o.MapFrom(s => s.WASHING))
                         .ForMember(d => d.UnionFee, o => o.MapFrom(s => s.UnionFee))
                         .ForMember(d => d.ProfTax, o => o.MapFrom(s => s.ProfTax))
                         .ForMember(d => d.SportClub, o => o.MapFrom(s => s.SportClub))
                         .ForMember(d => d.IsRateVPF, o => o.MapFrom(s => s.IsRateVPF))
                         .ForMember(d => d.OTACode, o => o.MapFrom(s => s.OTACode))
                         .ForMember(d => d.VPFValueRA, o => o.MapFrom(s => s.VPFValueRA))
                         .ForMember(d => d.IsSuspended, o => o.MapFrom(s => s.IsSuspended))
                         .ForMember(d => d.IsOTACode, o => o.MapFrom(s => s.IsOTACode))
                         .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                         .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                         .ForMember(d => d.D_VPF, o => o.MapFrom(s => s.D_VPF))
                         .ForMember(d => d.IsPensionDeducted, o => o.MapFrom(s => s.IsPensionDeducted))
                         .ForMember(d => d.BasicSalPercentageForSuspendedEmp, o => o.MapFrom(s => s.BasicSalPercentageForSuspendedEmp))
                         .ForMember(d => d.E_Basic_Pay, o => o.MapFrom(s => s.E_Basic_Pay))
                         .ForMember(d => d.BranchID_Pay, o => o.MapFrom(s => s.BranchID_Pay))
                          .ForMember(d => d.DOJ, o => o.MapFrom(s => s.DOJ))
                   .ForMember(d => d.DOLeaveOrg, o => o.MapFrom(s => s.DOLeaveOrg))
                         .ForAllOtherMembers(d => d.Ignore());
                });
                salHead = Mapper.Map<EmpSalaryConfiguration>(empSalaryData);

                //if (!salHead.IsRateVPF)
                //    salHead.VPFValueRA = salHead.D_VPF;

                if ((bool)salHead.HRA)
                    salHead.deduction = Deduction.HRA;
                else if ((bool)salHead.IsFlatDeduction)
                    salHead.deduction = Deduction.FlatDeduction;
                else if ((bool)salHead.None)
                    salHead.deduction = Deduction.None;

                if (salHead.BasicSalPercentageForSuspendedEmp.HasValue)
                {
                    var percentage = salHead.BasicSalPercentageForSuspendedEmp.Value;
                    if (Decimal.ToInt32(percentage) == 50)
                        salHead.salPerForSuspendedEmp = SalPercetageRatioForSuspendedEmployee.Fifty;
                    else if (Decimal.ToInt32(percentage) == 75)
                        salHead.salPerForSuspendedEmp = SalPercetageRatioForSuspendedEmployee.SeventyFive;
                }
                if (salHead.E_Basic.HasValue && salHead.salPerForSuspendedEmp != 0)
                {
                    if (salHead.salPerForSuspendedEmp == SalPercetageRatioForSuspendedEmployee.Fifty)
                        salHead.ActualSalary = (salHead.E_Basic.Value) * 50 / (decimal)100.00;
                    else if (salHead.salPerForSuspendedEmp == SalPercetageRatioForSuspendedEmployee.SeventyFive)
                        salHead.ActualSalary = (salHead.E_Basic.Value) * 75 / (decimal)100.00;
                }
                if (salHead.E_Basic.HasValue && !salHead.BasicSalPercentageForSuspendedEmp.HasValue)
                {
                    salHead.ActualSalary = salHead.E_Basic.Value;
                }
            }
            ViewBag.ddlBranchPay = GetBranchList();
            return PartialView("_SalaryConfigPopup", salHead);
        }

        [HttpPost]
        public ActionResult _PostSalaryConfigDtls(EmpSalaryConfiguration salaryHead)
        {
            log.Info($"EmpSalaryConfigController/_PostSalaryConfigDtls");

            if (salaryHead.deduction == Deduction.HRA)
                salaryHead.HRA = true;
            if (salaryHead.deduction == Deduction.FlatDeduction)
                salaryHead.IsFlatDeduction = true;
            if (salaryHead.deduction == Deduction.None)
                salaryHead.None = true;

            if (salaryHead.salPerForSuspendedEmp == SalPercetageRatioForSuspendedEmployee.Fifty)
            {
                salaryHead.BasicSalPercentageForSuspendedEmp = 50;
                salaryHead.ActualSalary = (salaryHead.E_Basic.Value) * 50 / (decimal)100.00;
            }
            else if (salaryHead.salPerForSuspendedEmp == SalPercetageRatioForSuspendedEmployee.SeventyFive)
            {
                salaryHead.BasicSalPercentageForSuspendedEmp = 75;
                salaryHead.ActualSalary = (salaryHead.E_Basic.Value) * 75 / (decimal)100.00;
            }
            else if (!salaryHead.IsSuspended)
            {
                salaryHead.BasicSalPercentageForSuspendedEmp = null;
            }

            if (salaryHead.E_Basic.HasValue && !salaryHead.BasicSalPercentageForSuspendedEmp.HasValue)
            {
                salaryHead.ActualSalary = salaryHead.E_Basic.Value;
            }

            if (salaryHead.IsSuspended)
            {
                if (salaryHead.SuspensionPeriodFrom.HasValue && salaryHead.SuspensionPeriodTo.HasValue)
                {
                    if (salaryHead.SuspensionPeriodTo.Value < salaryHead.SuspensionPeriodFrom.Value)
                        ModelState.AddModelError("SuspensionPeriodDateValidation", "Invalid Period, From date must be less than to date.");
                    else
                    {
                        //======  Does new suspension period overlap in existing period list  ? ======

                        var notOverlapped = employeeService.DoesNewPeriodOverLapped(salaryHead.EmployeeID,
                            salaryHead.SuspensionPeriodFrom.Value, salaryHead.SuspensionPeriodTo.Value);

                        if (!notOverlapped)
                            ModelState.AddModelError("SuspensionPeriodDateValidation", "Invalid Period, The selected period already overlapped with existing defined suspension periods.");
                    }
                }
                else
                    ModelState.AddModelError("SuspensionPeriodRequired", "Please Select Suspension Period.");

                if (salaryHead.salPerForSuspendedEmp == 0)
                    ModelState.AddModelError("SuspensionPeriodBasicRatio", "Please Select Salary Percentage ");


            }

            if (ModelState.IsValid)
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<EmpSalaryConfiguration, EmployeeSalary>()
                     .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmpCode))
                         .ForMember(d => d.E_Basic, o => o.MapFrom(s => s.E_Basic))
                         .ForMember(d => d.HRA, o => o.MapFrom(s => s.HRA))
                         .ForMember(d => d.E_02, o => o.MapFrom(s => (s.HRA == true ? (decimal?)1 : (decimal?)0)))
                         .ForMember(d => d.IsFlatDeduction, o => o.MapFrom(s => s.IsFlatDeduction))
                         .ForMember(d => d.D_07, o => o.MapFrom(s => (s.IsFlatDeduction == true ? (decimal?)1 : (decimal?)0)))
                         .ForMember(d => d.None, o => o.MapFrom(s => s.None))

                         .ForMember(d => d.BranchID, o => o.MapFrom(s => s.EmpBranchID))
                         .ForMember(d => d.CCA, o => o.MapFrom(s => s.CCA))
                         .ForMember(d => d.E_05, o => o.MapFrom(s => (s.CCA == true ? (decimal?)1 : (decimal?)0)))
                         .ForMember(d => d.WASHING, o => o.MapFrom(s => s.WASHING))
                          .ForMember(d => d.E_07, o => o.MapFrom(s => (s.WASHING == true ? (decimal?)1 : (decimal?)0)))

                         .ForMember(d => d.UnionFee, o => o.MapFrom(s => s.UnionFee))
                           .ForMember(d => d.D_14, o => o.MapFrom(s => (s.UnionFee == true ? (decimal?)1 : (decimal?)0)))
                         .ForMember(d => d.ProfTax, o => o.MapFrom(s => s.ProfTax))
                         .ForMember(d => d.D_02, o => o.MapFrom(s => (s.ProfTax == true ? (decimal?)1 : (decimal?)0)))

                         .ForMember(d => d.SportClub, o => o.MapFrom(s => s.SportClub))
                           .ForMember(d => d.D_06, o => o.MapFrom(s => (s.SportClub == true ? (decimal?)1 : (decimal?)0)))

                         .ForMember(d => d.IsRateVPF, o => o.MapFrom(s => s.IsRateVPF))
                         .ForMember(d => d.D_VPF, o => o.MapFrom(s => s.D_VPF))

                         .ForMember(d => d.OTACode, o => o.MapFrom(s => s.OTACode))
                         .ForMember(d => d.VPFValueRA, o => o.MapFrom(s => s.VPFValueRA))
                         .ForMember(d => d.IsSuspended, o => o.MapFrom(s => s.IsSuspended))
                         .ForMember(d => d.IsOTACode, o => o.MapFrom(s => s.IsOTACode))
                         .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                         .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                         .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                         .ForMember(d => d.BasicSalPercentageForSuspendedEmp, o => o.MapFrom(s => s.BasicSalPercentageForSuspendedEmp))

                         .ForMember(d => d.IsPensionDeducted, o => o.MapFrom(s => s.IsPensionDeducted))

                         .ForMember(d => d.SuspensionFromData, o => o.MapFrom(s => s.SuspensionPeriodFrom))
                         .ForMember(d => d.SuspensionToData, o => o.MapFrom(s => s.SuspensionPeriodTo))
                         .ForMember(d => d.E_Basic_Pay, o => o.MapFrom(s => s.E_Basic_Pay))
                         .ForMember(d => d.BranchID_Pay, o => o.MapFrom(s => s.BranchID_Pay))
                         .ForAllOtherMembers(d => d.Ignore());
                });

                //if (salaryHead.ActualSalary.HasValue)
                //    salaryHead.E_Basic = salaryHead.ActualSalary.Value;
                var empSalary = Mapper.Map<EmployeeSalary>(salaryHead);

                empSalary.UpdatedBy = userDetail.UserID;
                empSalary.UpdatedOn = System.DateTime.Now;

                if (!(empSalary.IsRateVPF ?? false))
                    empSalary.VPFValueRA = empSalary.D_VPF;


                bool result = employeeService.InsertUpdateEmployeeSalary(empSalary);

                return Json(new { status = result }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.ddlBranchPay = GetBranchList();
            return PartialView("_SalaryConfigPopup", salaryHead);
        }

        private List<SelectListModel> GetBranchList()
        {
            var ddlBranchList = dropdownService.ddlBranchList();
            return ddlBranchList;
        }

        [HttpGet]
        public ActionResult _GetEmpSuspnesionHistory(int employeeID)
        {
            log.Info($"EmpSalaryConfigController/_GetEmpSuspnesionHistory/{employeeID}");
            try
            {
                var model = employeeService.GetEmployeeSuspensionHistory(employeeID);
                return PartialView("_EmpSuspensionGridView", model);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}