using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using MicroPay.Web.Models;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using Common = Nafed.MicroPay.Common;
using static Nafed.MicroPay.Common.DocumentUploadFilePath;
using static Nafed.MicroPay.Common.FileHelper;
using System.IO;
using AutoMapper;
using System.Text;

namespace MicroPay.Web.Controllers.Employees
{
    public class NonRegularEmployeesController : BaseController
    {
        // GET: NonRegularEmployee

        private readonly INonRegularEmployeesService _employeeService;
        private readonly IDropdownBindService _ddlService;
        private readonly IUserService _userService;
        private readonly IDesignationService _designationService;
        private readonly IDependentService _dependentService;
        public NonRegularEmployeesController(INonRegularEmployeesService employeeService, IDropdownBindService ddlService, IUserService userService, IDesignationService designationService, IDependentService dependentService)
        {
            this._designationService = designationService;
            this._employeeService = employeeService;
            this._ddlService = ddlService;
            this._userService = userService;
            this._dependentService = dependentService;
        }
        public ActionResult Index()
        {
            log.Info($"NonRegularEmployeesController/Index");

            Models.Employees.NREmployeeViewModel empVM = new Models.Employees.NREmployeeViewModel();
            empVM.userRights = userAccessRight;
            empVM.employeeType = _ddlService.ddlEmployeeTypeList().Where(x => x.id != 5).ToList();
            empVM.designation = _ddlService.ddlDesignationList();
            return View(empVM);
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("NonRegularEmployeesController/Create");
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
        public ActionResult _CreateGeneralDetails()
        {
            log.Info("NonRegularEmployeesController/_CreateGeneralDetails");
            BindDropdowns();
            BindDesignationBranch();
            Model.Employees.NonRegularEmployee employeeDetails = new Model.Employees.NonRegularEmployee();
            employeeDetails.EmployeeTypeID = 1; // for contract type employee
            return PartialView("_CreateGeneralDetails", employeeDetails);
        }

        [HttpPost]
        public ActionResult _CreateGeneralDetails(Model.Employees.NonRegularEmployee employee)
        {
            log.Info("NonRegularEmployeesController/CreateGeneralDetails");
            try
            {
                BindDropdowns();
                BindDesignationBranch();
                ModelState.Remove("EmployeeCode");

                if (employee.TitleID == 0)
                    ModelState.AddModelError("TitleRequired", "Select Title");
                if (employee.EmployeeTypeID == 0)
                    ModelState.AddModelError("EmployeeTypeRequired", "Select Employee Type");
                if (employee.GenderID == 0)
                    ModelState.AddModelError("EmployeeGenderRequired", "Select Gender");

                if (employee.DepartmentID == 0)
                    ModelState.AddModelError("DepartmentRequired", "Select Department");
                if (employee.DesignationID == 0)
                    ModelState.AddModelError("DesignationRequired", "Select Designation");
                if (employee.SectionID == 0)
                    ModelState.AddModelError("SectionRequired", "Select Section");
                if (!employee.DOJ.HasValue)
                    ModelState.AddModelError("", "");
                ModelState.Remove("DOB");

                if (employee.modOfPayment == Model.Employees.ModeOfPayment.Bank)
                {
                    if (string.IsNullOrEmpty(employee.BankCode))
                        ModelState.AddModelError("BankRequired", "Select Bank Name");
                    if (string.IsNullOrEmpty(employee.BankAcNo))
                        ModelState.AddModelError("BankAcNoRequired", "Please Enter Bank Account No.");
                }
                if (ModelState.IsValid)
                {
                    employee.CreatedBy = userDetail.UserID;
                    employee.CreatedOn = DateTime.Now;
                    employee.ReviewerTo = employee.ReviewerTo == 0 ? null : employee.ReviewerTo;
                    employee.ReportingToID = employee.ReportingToID == 0 ? null : employee.ReportingToID;
                    employee.AcceptanceAuthority = employee.AcceptanceAuthority == 0 ? null : employee.AcceptanceAuthority;

                    int employeeDetailsID = _employeeService.InsertEmployeeDetails(employee);

                    TempData["EmployeeID"] = employeeDetailsID;

                    TempData.Keep("EmployeeID");
                    ViewBag.ActiveTab = 1;
                    ViewBag.EmployeeID = employeeDetailsID;

                    return Json(new { status = "1", empID = employeeDetailsID, activeTab = 1, msg = "Successfully Created" }, JsonRequestBehavior.AllowGet);

                }
                return PartialView("_CreateGeneralDetails", employee);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        #region Edit Employee Data 

        [HttpGet]
        public ActionResult Edit(int employeeID)
        {
            log.Info($"NonRegularEmployeesController/Edit/{employeeID}");
            try
            {
                TempData["employeeID"] = employeeID;
                TempData.Keep("employeeID");
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region Tab 1

        [HttpGet]
        public ActionResult _EditGeneralDetails(int employeeID)
        {
            log.Info("NonRegularEmployeesController/_EditGeneralDetails");
            try
            {
                BindDropdowns();

                Model.Employees.NonRegularEmployee employee = new Model.Employees.NonRegularEmployee();
                employee = _employeeService.GetEmployeeByID(employeeID);

                if (employee.IsBM)
                    employee.divison = Model.Employees.Divison.IsBM;
                if (employee.IsRM)
                    employee.divison = Model.Employees.Divison.IsRM;

                employee.EmpProfilePhotoUNCPath = _employeeService.GetEmployeeProfilePath(employee.EmployeeCode);
                ViewBag.ActiveTab = 0;

                //====  Get Employee Mode Of PAyment Information======
                var empSalRow = _employeeService.GetEmployeePaymentModeDtls(employee.EmployeeCode);
                if (empSalRow != null)
                {
                    employee.BankAcNo = empSalRow.BankAcNo; employee.BankCode = empSalRow.BankCode;
                    employee.modOfPayment = empSalRow.modOfPayment.ToString().ToUpper().Trim() == "CASH" ? Model.Employees.ModeOfPayment.Cash : Model.Employees.ModeOfPayment.Bank;
                }
                ///========End===========
                return PartialView("_GeneralDetails", employee);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult _EditGeneralDetails(Model.Employees.NonRegularEmployee employee)
        {
            log.Info("NonRegularEmployeesController/_EditGeneralDetails");

            try
            {
                BindDropdowns();

                var selectedDeptID = !string.IsNullOrEmpty(Request.Form["SelectedDepartmentID"]) ? (int?)int.Parse(Request.Form["SelectedDepartmentID"].ToString()) : null;

                if (employee.TitleID == 0)
                    ModelState.AddModelError("TitleRequired", "Select Title");
                if (employee.EmployeeTypeID == 0)
                    ModelState.AddModelError("EmployeeTypeRequired", "Select Employee Type");
                if (employee.GenderID == 0)
                    ModelState.AddModelError("EmployeeGenderRequired", "Select Gender");
                if (employee.DesignationID == 0)
                    ModelState.AddModelError("EmployeeDesignationRequired", "Select Employee Designation");
                if (employee.CadreID == 0)
                    ModelState.AddModelError("CadreRequired", "Select Cadre");
                if (employee.BranchID == 0)
                    ModelState.AddModelError("BranchRequired", "Select Branch");
                if (employee.DepartmentID == 0)
                    ModelState.AddModelError("DepartmentRequired", "Select Department");
                if (employee.DesignationID == 0)
                    ModelState.AddModelError("DesignationRequired", "Select Designation");
                if (employee.SectionID == 0)
                    ModelState.AddModelError("SectionRequired", "Select Section");

                if (employee.divison == Model.Employees.Divison.IsRM)
                    employee.IsRM = true;
                else if (employee.divison == Model.Employees.Divison.IsBM)
                    employee.IsBM = true;

                ModelState.Remove("DOB");

                if (ModelState.IsValid)
                {
                    employee.UpdatedBy = userDetail.UserID;
                    employee.UpdatedOn = DateTime.Now;
                    employee.ReviewerTo = employee.ReviewerTo == 0 ? null : employee.ReviewerTo;
                    employee.ReportingToID = employee.ReportingToID == 0 ? null : employee.ReportingToID;
                    employee.AcceptanceAuthority = employee.AcceptanceAuthority == 0 ? null : employee.AcceptanceAuthority;
                    employee.DepartmentID = selectedDeptID.HasValue ? selectedDeptID.Value : employee.DepartmentID;

                    bool isUpdated = _employeeService.UpdateEmployeeGeneralDetails(employee);
                    TempData.Keep("EmployeeID");
                    ViewBag.ActiveTab = 1;
                    TempData["Message"] = "Successfully Updated";
                    return PartialView("_GeneralDetails", employee);
                }
                return PartialView("_GeneralDetails", employee);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        #endregion

        #region Tab 2

        [HttpGet]
        public ActionResult _PersonalDetails(int? employeeID)
        {
            log.Info($"NonRegularEmployeesController/_PersonalDetails/{employeeID}");
            try
            {
                BindDropdowns();
                Model.Employees.NonRegularEmployee employee = new Model.Employees.NonRegularEmployee();

                if (employeeID.HasValue)
                    employee = _employeeService.GetEmployeeByID(employeeID.Value);

                if (string.IsNullOrEmpty(employee.AadhaarCardFilePath))
                {
                    employee.AadhaarCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.AadhaarCardFilePath + "/AadhaarNo.jpg");
                }
                else
                {
                    employee.AadhaarCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, employee.AadhaarCardFilePath);
                    if (!System.IO.File.Exists(employee.AadhaarCardUNCFilePath))
                    {
                        employee.AadhaarCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.AadhaarCardFilePath + "/AadhaarNo.jpg");
                    }
                }
                if (string.IsNullOrEmpty(employee.PanCardFilePath))

                    employee.PanCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.PanCardFilePath + "/SamplePAN_Card.jpg");
                else
                {
                    employee.PanCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, employee.PanCardFilePath);

                    if (!System.IO.File.Exists(employee.PanCardUNCFilePath))
                    {
                        employee.PanCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.PanCardFilePath + "/SamplePAN_Card.jpg");
                    }
                }

                #region Get Employee Bank Details ====================

                var empSalInfo = _employeeService.GetEmployeeSalaryDtls(employee.EmployeeCode);
                employee.BankAcNo = empSalInfo?.BankAcNo ?? employee.BankAcNo;
                employee.BankCode = empSalInfo?.BankCode ?? employee.BankCode;

                #endregion

                return PartialView("_PersonalDetails", employee);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult _PersonalDetails(Model.Employees.NonRegularEmployee employee, IEnumerable<HttpPostedFileBase> files)
        {
            log.Info("NonRegularEmployeesController/_PersonalDetails");
            try
            {
                BindDropdowns();

                ModelState.Remove("EmployeeCode");
                ModelState.Remove("Name");
                ModelState.Remove("MobileNo");
                ModelState.Remove("OfficialEmail");
                ModelState.Remove("Pr_Loc_DOJ");
                ModelState.Remove("FirstDesg");
                ModelState.Remove("FirstBranch");
                ModelState.Remove("DOJ");
                ViewBag.ActiveTab = 1;

                string panCardFilePath = string.Empty, aadharCardFilePath = string.Empty;
                if (ModelState.IsValid)
                {
                    employee.EmployeeID = employee.EmployeeID == 0 ? Convert.ToInt32(TempData["EmployeeID"]) : employee.EmployeeID;

                    if (employee.DOB.HasValue)
                    {
                        DateTime enteredDOB = employee.DOB.Value;
                        var yrs = (DateTime.Today - enteredDOB).TotalDays / 365.2425;

                        if (yrs < 18)
                        {
                            ModelState.AddModelError("InValidDOB", "Employee age should be greater than 18 yr .");
                            TempData["Error"] = "InValidDOB";
                            return View("_PersonalDetails", employee);
                        }
                        if (employee.PPEDate.HasValue && !employee.PPIDate.HasValue)
                        {
                            ModelState.AddModelError("InValidPPIDate", "Please enter passport issue date.");
                            return View("_PersonalDetails", employee);
                        }

                        if (!employee.PPIDate.HasValue && employee.PPEDate.HasValue)
                        {
                            ModelState.AddModelError("InValidPPEDate", "Please enter passport expire date.");
                            return View("_PersonalDetails", employee);
                        }

                        if (employee.PPIDate.HasValue && employee.PPEDate.HasValue)
                        {
                            if (employee.PPIDate.Value > employee.PPEDate.Value)
                            {
                                ModelState.AddModelError("InValidPPIDate", "Passport issue date should not be greater than passport expiry date.");
                                return View("_PersonalDetails", employee);
                            }
                            if (employee.PPEDate.Value < employee.PPIDate.Value)
                            {
                                ModelState.AddModelError("InValidPPEDate", "Passport expiry date should not be less than passport issue date.");
                                return View("_PersonalDetails", employee);
                            }
                        }

                        employee.UpdatedBy = userDetail.UserID;
                        employee.UpdatedOn = DateTime.Now;

                        bool isUpdated = _employeeService.UpdatetEmployeePersonalDetails(employee);


                        TempData.Keep("EmployeeID");
                        ViewBag.ActiveTab = 1;
                        TempData["Message"] = "Successfully Updated";
                        return PartialView("_PersonalDetails", employee);
                    }
                }
                return PartialView("_PersonalDetails", employee);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }
        #endregion

        #region Tab 3
        [HttpGet]
        public ActionResult _ContractExtensionList(int employeeID)
        {
            log.Info($"NonRegularEmployeesController/_ContractExtensionList/{employeeID}");
            try
            {
                var extension = _employeeService.GetEmployeeContractExtensionList(employeeID);
                return PartialView("_ContractExtensionHistory", extension);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _ContractExtension(int? tId, int employeeID)
        {
            log.Info($"NonRegularEmployeesController/_ContractExtension");
            try
            {
                Model.Employees.NonRegularEmployeesExtension extension = new Model.Employees.NonRegularEmployeesExtension();
                extension.EmployeeId = employeeID;
                if (tId > 0)
                    extension = _employeeService.GetEmployeeContractExtension(employeeID, tId.Value);
                return PartialView("_ContractExtensionForm", extension);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult _PostExtension(Model.Employees.NonRegularEmployeesExtension frmData)
        {
            log.Info($"NonRegularEmployeesController/_PostExtension/");
            if ((frmData.ExtentionNoticeDate.HasValue && frmData.ExtentionFromDate.HasValue) && (frmData.ExtentionNoticeDate.Value > frmData.ExtentionFromDate.Value))
            {
                ModelState.AddModelError("ExtentionNoticeDate", "Notice date can't be greater than from date.");
            }
            if (ModelState.IsValid)
            {
                if (frmData.Id == 0)
                {
                    frmData.CreatedBy = userDetail.UserID;
                    frmData.CreatedOn = DateTime.Now;

                }
                else
                {
                    frmData.UpdatedBy = userDetail.UserID;
                    frmData.UpdatedOn = DateTime.Now;
                }
                if (frmData.ExtentionFromDate.Value > frmData.ExtentionToDate.Value)
                {
                    ModelState.AddModelError("ToDateRangeValidation", "Extension to date can't be less than from date.");
                    return PartialView("_ContractExtensionForm", frmData);
                }
                var result = _employeeService.InsertUpdateContractExtension(frmData);

                if (result)
                {
                    var msg = string.Empty;
                    if (frmData.CreatedBy > 0)
                        msg = "Successfully Saved";
                    else
                        msg = "Successfully updated.";

                    var extension = _employeeService.GetEmployeeContractExtensionList(frmData.EmployeeId);
                    var html = ConvertViewToString("_ContractExtensionHistory", extension);
                    return Json(new { success = result, html = html, msg = msg }, JsonRequestBehavior.AllowGet);

                }
                return Json(new { success = result }, JsonRequestBehavior.AllowGet);

            }
            return PartialView("_ContractExtensionForm", frmData);
        }
        public ActionResult DeleteExtension(int employeeID, int tId)
        {
            log.Info($"NonRegularEmployeesController/DeleteExtension/{tId}");
            try
            {
                var flag = _employeeService.DeleteContractExtension(tId);
                if (flag)
                {
                    var extension = _employeeService.GetEmployeeContractExtensionList(employeeID);
                    var html = ConvertViewToString("_ContractExtensionHistory", extension);
                    return Json(new { success = true, html = html, msg = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        [HttpGet]
        public PartialViewResult _GetEmployeeGridView(EmployeeViewModel empVM)
        {
            Models.Employees.NREmployeeViewModel employeeVM = new Models.Employees.NREmployeeViewModel();
            var emp = _employeeService.GetEmployeeList(empVM.EmployeeName, empVM.EmployeeCode, empVM.DesignationID, empVM.EmployeeTypeID);
            emp.ForEach(x =>
            {
                x.EmpProfilePhotoUNCPath =
                 x.EmpProfilePhotoUNCPath == null ?
                 Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png")
                 :

                System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + x.EmpProfilePhotoUNCPath)) ?

                 Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + x.EmpProfilePhotoUNCPath) :
                 Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");

            });
            employeeVM.Employee = emp;
            employeeVM.userRights = userAccessRight;
            return PartialView("_EmployeeGrid", employeeVM);
        }


        public ActionResult Delete(int employeeID)
        {
            log.Info($"NonRegularEmployeesController/Delete/{employeeID}");
            try
            {
                _employeeService.DeleteEmployee(employeeID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }
        private void BindDesignationBranch()
        {
            var ddlDesignationList = _ddlService.ddlDesignationList();
            ddlDesignationList.OrderBy(x => x.value);
            Model.SelectListModel selectDesignation = new Model.SelectListModel();
            selectDesignation.id = 0;
            selectDesignation.value = "Select";
            ddlDesignationList.Insert(0, selectDesignation);
            ViewBag.Designation = new SelectList(ddlDesignationList, "id", "value");

            var ddlBranchList = _ddlService.ddlBranchList();
            ddlBranchList.OrderBy(x => x.value);
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");
        }

        private void BindDropdowns()
        {
            var ddlTitleList = _ddlService.ddlTitleList();
            ddlTitleList.OrderBy(x => x.value);
            Model.SelectListModel selectTitle = new Model.SelectListModel();
            selectTitle.id = 0;
            selectTitle.value = "Select";
            ddlTitleList.Insert(0, selectTitle);
            ViewBag.Title = new SelectList(ddlTitleList, "id", "value");


            var ddlEmployeeTypeList = _ddlService.ddlEmployeeTypeList().Where(x => x.id != 5).ToList();
            ddlEmployeeTypeList.OrderBy(x => x.value);
            Model.SelectListModel selectEmployeeType = new Model.SelectListModel();
            selectEmployeeType.id = 0;
            selectEmployeeType.value = "Select";
            ddlEmployeeTypeList.Insert(0, selectEmployeeType);
            ViewBag.EmployeeType = new SelectList(ddlEmployeeTypeList, "id", "value");

            var ddlGenderList = _ddlService.ddlGenderList();
            ddlGenderList.OrderBy(x => x.value);
            Model.SelectListModel selectGender = new Model.SelectListModel();
            selectGender.id = 0;
            selectGender.value = "Select";
            ddlGenderList.Insert(0, selectGender);
            ViewBag.Gender = new SelectList(ddlGenderList, "id", "value");

            var ddlDesignationList = _ddlService.ddlFirstDesignationList();
            ddlDesignationList.OrderBy(x => x.value);
            Model.SelectListModel selectDesignation = new Model.SelectListModel();
            selectDesignation.id = 0;
            selectDesignation.value = "Select";
            ddlDesignationList.Insert(0, selectDesignation);
            ViewBag.Designation = new SelectList(ddlDesignationList, "id", "value");

            var ddlCadreCodeList = _ddlService.ddlCadreCodeList();
            ddlCadreCodeList.OrderBy(x => x.value);
            Model.SelectListModel selectCadreCode = new Model.SelectListModel();
            selectCadreCode.id = 0;
            selectCadreCode.value = "Select";
            ddlCadreCodeList.Insert(0, selectCadreCode);
            ViewBag.CadreCode = new SelectList(ddlCadreCodeList, "id", "value");

            var ddlBranchList = _ddlService.ddlFirstBranchList();
            ddlBranchList.OrderBy(x => x.value);
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            var ddlSectionList = _ddlService.ddlSectionList();
            ddlSectionList.OrderBy(x => x.value);
            Model.SelectListModel selectSection = new Model.SelectListModel();
            selectSection.id = 0;
            selectSection.value = "Select";
            ddlSectionList.Insert(0, selectSection);
            ViewBag.Section = new SelectList(ddlSectionList, "id", "value");

            var ddlCategoryList = _ddlService.ddlCategoryList();
            ddlCategoryList.OrderBy(x => x.value);
            Model.SelectListModel selectCategory = new Model.SelectListModel();
            selectCategory.id = 0;
            selectCategory.value = "Select";
            ddlCategoryList.Insert(0, selectCategory);
            ViewBag.Category = new SelectList(ddlCategoryList, "id", "value");

            var ddlDepartmentList = _ddlService.ddlDepartmentList();
            ddlDepartmentList.OrderBy(x => x.value);
            Model.SelectListModel selectDepartment = new Model.SelectListModel();
            selectDepartment.id = 0;
            selectDepartment.value = "Select";
            ddlDepartmentList.Insert(0, selectDepartment);
            ViewBag.Department = new SelectList(ddlDepartmentList, "id", "value");


            var ddlReligionList = _ddlService.ddlReligionList();
            ddlReligionList.OrderBy(x => x.value);
            Model.SelectListModel selectReligion = new Model.SelectListModel();
            selectReligion.id = 0;
            selectReligion.value = "Select";
            ddlReligionList.Insert(0, selectReligion);
            ViewBag.Religion = new SelectList(ddlReligionList, "id", "value");

            var ddlMotherTongueList = _ddlService.ddlMotherTongueList();
            ddlMotherTongueList.OrderBy(x => x.value);
            Model.SelectListModel motherTongue = new Model.SelectListModel();
            motherTongue.id = 0;
            motherTongue.value = "Select";
            ddlMotherTongueList.Insert(0, motherTongue);
            ViewBag.MotherTongue = new SelectList(ddlMotherTongueList, "id", "value");

            var ddlMaritalStsList = _ddlService.ddlMaritalStsList();
            ddlMaritalStsList.OrderBy(x => x.value);
            Model.SelectListModel maritalSts = new Model.SelectListModel();
            maritalSts.id = 0;
            maritalSts.value = "Select";
            ddlMaritalStsList.Insert(0, maritalSts);
            ViewBag.MaritalSts = new SelectList(ddlMaritalStsList, "id", "value");

            var ddlBloodGroupList = _ddlService.ddlBloodGroupList();
            ddlBloodGroupList.OrderBy(x => x.value);
            Model.SelectListModel bloodGroup = new Model.SelectListModel();
            bloodGroup.id = 0;
            bloodGroup.value = "Select";
            ddlBloodGroupList.Insert(0, bloodGroup);
            ViewBag.BloodGroup = new SelectList(ddlBloodGroupList, "id", "value");


            var ddlRelationList = _ddlService.ddlRelationList();
            ddlRelationList.OrderBy(x => x.value);
            Model.SelectListModel relation = new Model.SelectListModel();
            relation.id = 0;
            relation.value = "Select";
            ddlRelationList.Insert(0, relation);
            ViewBag.Relation = new SelectList(ddlRelationList, "id", "value");


            var ddlEmpCategory = _ddlService.ddlEmployeeCategoryList();
            ddlEmpCategory.OrderBy(x => x.value);
            Model.SelectListModel empCateogry = new Model.SelectListModel();
            empCateogry.id = 0;
            empCateogry.value = "Select";
            ddlEmpCategory.Insert(0, empCateogry);
            ViewBag.EmpCategory = new SelectList(ddlEmpCategory, "id", "value");


            var ddlBanks = _ddlService.ddlBanks();
            ddlBanks.OrderBy(x => x.value);
            ViewBag.Banks = ddlBanks;

            var ddlState = _ddlService.ddlStateList();
            ddlState.OrderBy(x => x.value);
            Model.SelectListModel State = new Model.SelectListModel();
            State.id = 0;
            State.value = "Select";
            ddlState.Insert(0, State);
            ViewBag.PresentState = new SelectList(ddlState, "id", "value");

        }

        [HttpGet]
        public ActionResult GetEmployeeDependent(int employeeID)
        {
            log.Info($"NonRegularEmployeesController/GetEmployeeDependent/{employeeID}");
            try
            {
                var dependentModel = _dependentService.GetDependentList(employeeID);
                return PartialView("_ViewDependent", dependentModel);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return Content("refresh-Data");
        }
    }
}