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

namespace MicroPay.Web.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IDropdownBindService ddlService;
        private readonly IDependentService dependentService;
        private readonly IUserService userService;
        private readonly IDesignationService designationService;
        public EmployeeController(IEmployeeService employeeService, IDropdownBindService ddlService, IDependentService dependentService, IUserService userService, IDesignationService designationService)
        {
            this.designationService = designationService;
            this.employeeService = employeeService;
            this.ddlService = ddlService;
            this.dependentService = dependentService;
            this.userService = userService;
        }
        public ActionResult Index()
        {
            log.Info($"EmployeeController/Index");

            EmployeeViewModel empVM = new EmployeeViewModel();
            empVM.userRights = userAccessRight;
            empVM.employeeType = ddlService.ddlEmployeeTypeList();
            //  empVM.branchList = ddlService.ddlBranchList();
            empVM.designation = ddlService.ddlDesignationList();
            return View(empVM);
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("EmployeeController/Create");
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

        #region Edit Employee Data 

        [HttpGet]
        public ActionResult Edit(int employeeID)
        {
            log.Info($"EmployeeController/Edit/{employeeID}");
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

        #region Tab 1

        [HttpGet]
        public ActionResult _EditGeneralDetails(int employeeID)
        {
            log.Info("EmployeeController/_EditGeneralDetails");
            try
            {
                BindDropdowns();
             
                Model.Employee employee = new Model.Employee();
                employee = employeeService.GetEmployeeByID(employeeID);

                if (employee.IsBM)
                    employee.divison = Model.Divison.IsBM;
                if (employee.IsRM)
                    employee.divison = Model.Divison.IsRM;

                employee.EmpProfilePhotoUNCPath = employeeService.GetEmployeeProfilePath(employee.EmployeeCode);
                ViewBag.ActiveTab = 0;

                //====  Get Employee Mode Of PAyment Information======  Dated On- 17-Apr-2020 - SG



                var empSalRow = employeeService.GetEmployeePaymentModeDtls(employee.EmployeeCode);
                if (empSalRow != null)
                {
                    employee.BankAcNo = empSalRow.BankAcNo; employee.BankCode = empSalRow.BankCode;
                    employee.modOfPayment = empSalRow.modOfPayment.ToString().ToUpper().Trim() == "CASH" ? Model.ModeOfPayment.Cash : Model.ModeOfPayment.Bank;
                }
                ///========End===========s




                //var ddlEmployee = ddlService.GetAllEmployee();
                //Model.SelectListModel selectEmployee = new Model.SelectListModel();
                //selectEmployee.id = 0;
                //selectEmployee.value = "Select";
                //ddlEmployee.RemoveAll(x => x.id == employeeID);
                //ddlEmployee.Insert(0, selectEmployee);
                //ViewBag.Employee = new SelectList(ddlEmployee, "id", "value");

                return PartialView("_EditGeneralDetails", employee);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult _EditGeneralDetails(Model.Employee employee)
        {
            log.Info("EmployeeController/_EditGeneralDetails");

            try
            {
                BindDropdowns();
            
                var selectedDeptID = !string.IsNullOrEmpty(Request.Form["SelectedDepartmentID"]) ? (int?)int.Parse(Request.Form["SelectedDepartmentID"].ToString()) : null;

                //var ddlEmployee = ddlService.GetAllEmployee();
                //Model.SelectListModel selectEmployee = new Model.SelectListModel();
                //selectEmployee.id = 0;
                //selectEmployee.value = "Select";
                //ddlEmployee.RemoveAll(x => x.id == employee.EmployeeID);
                //ddlEmployee.Insert(0, selectEmployee);
                //ViewBag.Employee = new SelectList(ddlEmployee, "id", "value");

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
                    ModelState.AddModelError("DepartmentRequired", "Select Department");
                if (employee.SectionID == 0)
                    ModelState.AddModelError("SectionRequired", "Select Section");

                if (employee.divison == Model.Divison.IsRM)
                    employee.IsRM = true;
                else if (employee.divison == Model.Divison.IsBM)
                    employee.IsBM = true;

                ModelState.Remove("DOB");
                //if (employee.modOfPayment == Nafed.MicroPay.Model.ModeOfPayment.Bank)
                //{

                //    if (string.IsNullOrEmpty(employee.BankCode))
                //        ModelState.AddModelError("BankRequired", "Select Bank Name");
                //    if (string.IsNullOrEmpty(employee.BankAcNo))
                //        ModelState.AddModelError("BankAcNoRequired", "Please Enter Bank Account No.");
                //}
                if (ModelState.IsValid)
                {
                    employee.UpdatedBy = userDetail.UserID;
                    employee.UpdatedOn = DateTime.Now;
                    employee.ReviewerTo = employee.ReviewerTo == 0 ? null : employee.ReviewerTo;
                    employee.ReportingToID = employee.ReportingToID == 0 ? null : employee.ReportingToID;
                    employee.AcceptanceAuthority = employee.AcceptanceAuthority == 0 ? null : employee.AcceptanceAuthority;
                    employee.DepartmentID = selectedDeptID.HasValue ? selectedDeptID.Value : employee.DepartmentID;

                    bool isUpdated = employeeService.UpdateEmployeeGeneralDetails(employee);
                    TempData.Keep("EmployeeID");
                    ViewBag.ActiveTab = 1;
                    TempData["Message"] = "Successfully Updated";
                    return PartialView("_EditGeneralDetails", employee);
                }
                //else
                //    employee.EmpProfilePhotoUNCPath = employeeService.GetEmployeeProfilePath(employee.EmployeeCode);

                return PartialView("_EditGeneralDetails", employee);
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
        public ActionResult _EditPersonalDetails(int? employeeID)
        {
            log.Info($"EmployeeController/_EditPeronalDetails/{employeeID}");
            try
            {
                BindDropdowns();
                Model.Employee employee = new Model.Employee();

                if (employeeID.HasValue)
                    employee = employeeService.GetEmployeeByID(employeeID.Value);

                if (string.IsNullOrEmpty(employee.AadhaarCardFilePath))
                {
                    // employee.AadhaarCardFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.AadhaarCardFilePath + "/AadhaarNo.jpg");
                    employee.AadhaarCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.AadhaarCardFilePath + "/AadhaarNo.jpg");
                }
                else
                {
                    employee.AadhaarCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, employee.AadhaarCardFilePath);
                    if (!System.IO.File.Exists(employee.AadhaarCardUNCFilePath))
                    {
                        employee.AadhaarCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.AadhaarCardFilePath + "/AadhaarNo.jpg");
                        // employee.AadhaarCardFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.AadhaarCardFilePath + "/AadhaarNo.jpg");
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

                var empSalInfo = employeeService.GetEmployeeSalaryDtls(employee.EmployeeCode);
                employee.BankAcNo = empSalInfo?.BankAcNo ?? employee.BankAcNo;
                employee.BankCode = empSalInfo?.BankCode ?? employee.BankCode;

                #endregion

                return PartialView("_EditPersonalDetails", employee);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _EditPersonalDetails(Model.Employee employee, IEnumerable<HttpPostedFileBase> files)
        {
            log.Info("EmployeeController/_EditPersonalDetails");
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
                    //   var panCardFile = files == null ? null : files.FirstOrDefault();
                    //   var aadhaarCardFile = files == null ? null : files.LastOrDefault();

                    employee.EmployeeID = employee.EmployeeID == 0 ? Convert.ToInt32(TempData["EmployeeID"]) : employee.EmployeeID;

                    if (employee.DOB.HasValue)
                    {
                        DateTime enteredDOB = employee.DOB.Value;
                        var yrs = (DateTime.Today - enteredDOB).TotalDays / 365.2425;

                        if (yrs < 18)
                        {
                            ModelState.AddModelError("InValidDOB", "Employee age should be greater than 18 yr .");
                            TempData["Error"] = "InValidDOB";
                            return View("_EditPersonalDetails", employee);
                        }
                        if (employee.PPEDate.HasValue && !employee.PPIDate.HasValue)
                        {
                            ModelState.AddModelError("InValidPPIDate", "Please enter passport issue date.");
                            return View("_EditPersonalDetails", employee);
                        }

                        if (!employee.PPIDate.HasValue && employee.PPEDate.HasValue)
                        {
                            ModelState.AddModelError("InValidPPEDate", "Please enter passport expire date.");
                            return View("_EditPersonalDetails", employee);
                        }

                        if (employee.PPIDate.HasValue && employee.PPEDate.HasValue)
                        {
                            if (employee.PPIDate.Value > employee.PPEDate.Value)
                            {
                                ModelState.AddModelError("InValidPPIDate", "Passport issue date should not be greater than passport expiry date.");
                                return View("_EditPersonalDetails", employee);
                            }
                            if (employee.PPEDate.Value < employee.PPIDate.Value)
                            {
                                ModelState.AddModelError("InValidPPEDate", "Passport expiry date should not be less than passport issue date.");
                                return View("_EditPersonalDetails", employee);
                            }
                        }

                        employee.UpdatedBy = userDetail.UserID;
                        employee.UpdatedOn = DateTime.Now;

                        bool isUpdated = employeeService.UpdatetEmployeePersonalDetails(employee);


                        TempData.Keep("EmployeeID");
                        ViewBag.ActiveTab = 2;
                        TempData["Message"] = "Successfully Updated";
                        return PartialView("_EditPersonalDetails", employee);
                    }
                }
                return PartialView("_EditPersonalDetails", employee);
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
        public ActionResult _EditOtherDetails(int? employeeID)
        {
            log.Info("EmployeeController/_EditOtherDetails");
            try
            {
                BindDropdowns();
                Model.Employee empOtherDetails = new Model.Employee();

                if (employeeID.HasValue)
                {
                    empOtherDetails = employeeService.GetEmployeeByID(employeeID.Value);

                    if (empOtherDetails.IsVRS)
                        empOtherDetails.wayOfLeaving = Model.WayOfLeavingOrg.IsVRS;
                    if (empOtherDetails.IsTermination)
                        empOtherDetails.wayOfLeaving = Model.WayOfLeavingOrg.IsTermination;
                    if (empOtherDetails.IsDismissal)
                        empOtherDetails.wayOfLeaving = Model.WayOfLeavingOrg.IsDismissal;
                    if (empOtherDetails.IsResigned)
                        empOtherDetails.wayOfLeaving = Model.WayOfLeavingOrg.IsResigned;
                    if (empOtherDetails.IsExpire)
                        empOtherDetails.wayOfLeaving = Model.WayOfLeavingOrg.IsExpire;
                    if (empOtherDetails.IsForceFully)
                        empOtherDetails.wayOfLeaving = Model.WayOfLeavingOrg.IsForceFully;

                    //if (empOtherDetails.IsRM)
                    //    empOtherDetails.wayOfLeaving = Model.WayOfLeavingOrg.IsRM;

                    //if (empOtherDetails.IsBM)
                    //    empOtherDetails.wayOfLeaving = Model.WayOfLeavingOrg.IsBM;

                    if (empOtherDetails.OtaCode.HasValue)
                    {
                        if (empOtherDetails.OtaCode.Value == (int?)Model.Cota.Direct)
                            empOtherDetails.cota = Model.Cota.Direct;
                        if (empOtherDetails.OtaCode.Value == (int?)Model.Cota.LTC)
                            empOtherDetails.cota = Model.Cota.LTC;
                        if (empOtherDetails.OtaCode.Value == (int?)Model.Cota.Promotion)
                            empOtherDetails.cota = Model.Cota.Promotion;
                        if (empOtherDetails.OtaCode.Value == (int?)Model.Cota.VRS)
                            empOtherDetails.cota = Model.Cota.VRS;
                    }
                }

                return PartialView("_EditOtherDetails", empOtherDetails);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult _EditOtherDetails(Model.Employee empOtherDetails)
        {
            log.Info("EmployeeController/_EditOtherDetails");
            try
            {
                BindDropdowns();

                if (empOtherDetails.cota == Model.Cota.LTC)
                    empOtherDetails.OtaCode = (int?)2;
                else if (empOtherDetails.cota == Model.Cota.Direct)
                    empOtherDetails.OtaCode = (int?)1;
                else if (empOtherDetails.cota == Model.Cota.Promotion)
                    empOtherDetails.OtaCode = (int?)3;
                else if (empOtherDetails.cota == Model.Cota.VRS)
                    empOtherDetails.OtaCode = (int?)4;
                else
                    empOtherDetails.OtaCode = null;

                if (empOtherDetails.wayOfLeaving == Model.WayOfLeavingOrg.IsVRS)
                    empOtherDetails.IsVRS = true;
                else if (empOtherDetails.wayOfLeaving == Model.WayOfLeavingOrg.IsTermination)
                    empOtherDetails.IsTermination = true;
                else if (empOtherDetails.wayOfLeaving == Model.WayOfLeavingOrg.IsResigned)
                    empOtherDetails.IsResigned = true;
                else if (empOtherDetails.wayOfLeaving == Model.WayOfLeavingOrg.IsExpire)
                    empOtherDetails.IsExpire = true;
                else if (empOtherDetails.wayOfLeaving == Model.WayOfLeavingOrg.IsDismissal)
                    empOtherDetails.IsDismissal = true;
                else if (empOtherDetails.wayOfLeaving == Model.WayOfLeavingOrg.IsForceFully)
                    empOtherDetails.IsForceFully = true;
                else if (empOtherDetails.wayOfLeaving == Model.WayOfLeavingOrg.IsSuperAnnuated)
                    empOtherDetails.IsSuperAnnuated = true;
                else if (empOtherDetails.wayOfLeaving == Model.WayOfLeavingOrg.NotApplicable)
                    empOtherDetails.NotApplicable = true;


                ModelState.Remove("EmployeeCode");
                ModelState.Remove("Name");
                ModelState.Remove("DOB");
                ModelState.Remove("MobileNo");
                ModelState.Remove("OfficialEmail");
                ModelState.Remove("Pr_Loc_DOJ");
                ModelState.Remove("FirstDesg");
                ModelState.Remove("FirstBranch");
                ModelState.Remove("DOJ");
                //  var dd = ModelState.Where(x => x.Value.Errors.Count>0);
                if (ModelState.IsValid)
                {
                    empOtherDetails.UpdatedBy = userDetail.UserID;
                    empOtherDetails.UpdatedOn = DateTime.Now;
                    empOtherDetails.EmployeeID = empOtherDetails.EmployeeID == 0 ? Convert.ToInt32(TempData["EmployeeID"]) : empOtherDetails.EmployeeID;

                    if (empOtherDetails.IsSuperAnnuated)  ///==== 
                    {
                        empOtherDetails.DOSupAnnuating1 = empOtherDetails.DOSupAnnuating;
                        empOtherDetails.IsVRS = false;
                        empOtherDetails.IsTermination = false;
                        empOtherDetails.IsResigned = false;
                        empOtherDetails.IsExpire = false;
                        empOtherDetails.IsDismissal = false;
                        empOtherDetails.IsForceFully = false;
                    }

                    if (empOtherDetails.NotApplicable)
                    {
                        empOtherDetails.DOLeaveOrg = null;
                        empOtherDetails.IsVRS = false;
                        empOtherDetails.IsTermination = false;
                        empOtherDetails.IsResigned = false;
                        empOtherDetails.IsExpire = false;
                        empOtherDetails.IsDismissal = false;
                        empOtherDetails.IsForceFully = false;
                    }
                    bool isUpdated = employeeService.UpdateEmployeeOtherDetails(empOtherDetails);

                    TempData.Keep("EmployeeID");
                    ViewBag.ActiveTab = 3;
                    TempData["Message"] = "Successfully Updated";
                    return View("_EditOtherDetails", empOtherDetails);
                }
                return PartialView("_EditOtherDetails", empOtherDetails);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        #endregion

        #region Tab 4

        [HttpGet]
        public ActionResult _EditProfessionalDetails(int? employeeID)
        {
            log.Info($"EmployeeController/_EditProfessionalDetails/{employeeID}");
            try
            {
                EmployeeViewModel empVM = new EmployeeViewModel();
                if (employeeID.HasValue)
                {
                    var employee = employeeService.GetQualificationDetail(employeeID.Value);
                    empVM._Employee = employee._Employee;
                    List<Model.SelectListModel> Type = new List<Model.SelectListModel>();

                    var selectedCheckboxesacademicqual = new List<CheckBox>();
                    var getFieldsValueademicqual = ddlService.ddlAcedamicAndProfDtls(1);

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    empVM.CheckBoxListAcademic.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueademicqual);
                    empVM.CheckBoxListAcademic.SelectedFields = selectedCheckboxesacademicqual;
                    empVM.CheckBoxListAcademic.SelectedFields = employee.EmployeeQualificationList.Where(x => x.TypeID == 1).ToList().Select(y => new CheckBox { Id = y.QID });
                    //-----------------------------------
                    var selectedCheckboxesProfessionalqual = new List<CheckBox>();
                    var getFieldsValueProfessionalqual = ddlService.ddlAcedamicAndProfDtls(2);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    empVM.CheckBoxListProfessional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueProfessionalqual);
                    empVM.CheckBoxListProfessional.SelectedFields = selectedCheckboxesProfessionalqual;
                    empVM.CheckBoxListProfessional.SelectedFields = employee.EmployeeQualificationList.Where(x => x.TypeID == 2).ToList().Select(y => new CheckBox { Id = y.QID });
                }
                else
                {


                    var selectedCheckboxesacademicqual = new List<CheckBox>();
                    var getFieldsValueademicqual = ddlService.ddlAcedamicAndProfDtls(1);

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    empVM.CheckBoxListAcademic.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueademicqual);
                    empVM.CheckBoxListAcademic.SelectedFields = selectedCheckboxesacademicqual;

                    //-----------------------------------
                    var selectedCheckboxesProfessionalqual = new List<CheckBox>();
                    var getFieldsValueProfessionalqual = ddlService.ddlAcedamicAndProfDtls(2);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    empVM.CheckBoxListProfessional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueProfessionalqual);
                    empVM.CheckBoxListProfessional.SelectedFields = selectedCheckboxesProfessionalqual;
                }

                return PartialView("_EditProfessionalDetails", empVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult _EditProfessionalDetails(EmployeeViewModel empProfessionalDtls)
        {
            log.Info($"EmployeeController/_EditProfessionalDetails");
            try
            {
                //   BindDropdowns();

                ModelState.Remove("EmployeeCode");
                ModelState.Remove("OfficialEmail");
                ModelState.Remove("MobileNo");
                ModelState.Remove("Name");
                ModelState.Remove("DOB");
                ModelState.Remove("Pr_Loc_DOJ");
                ModelState.Remove("FirstDesg");
                ModelState.Remove("FirstBranch");
                ModelState.Remove("_Employee.EmployeeID");

                //if (empProfessionalDtls.CheckBoxListAcademic.PostedFields == null && empProfessionalDtls.CheckBoxListAcademic.PostedFields == null)
                //{
                //    ModelState.AddModelError("AcademinQualificationRequired", "Atleast select NIL,  if  there is no information availabe for your Academin Qualification");
                //}
                if (ModelState.IsValid)
                {
                    Model.EmployeeQualification empQual = new Model.EmployeeQualification();
                    empQual._Employee = new Model.Employee();
                    empQual._Employee = empProfessionalDtls._Employee;
                    if (empProfessionalDtls.CheckBoxListAcademic.PostedFields != null)
                        empQual.CheckBoxListAcademicvalue = empProfessionalDtls.CheckBoxListAcademic.PostedFields.fieldIds;
                    if (empProfessionalDtls.CheckBoxListProfessional.PostedFields != null)
                        empQual.CheckBoxListProfessionalvalue = empProfessionalDtls.CheckBoxListProfessional.PostedFields.fieldIds;
                    //empQual._Employee.CreatedBy = userDetail.UserID;
                    //empQual._Employee.CreatedOn  = DateTime.Now;
                    //empQual._Employee.EmployeeID = empProfessionalDtls.EmployeeID == 0 ? Convert.ToInt32(TempData["EmployeeID"]) : empProfessionalDtls.EmployeeID;

                    bool isUpdated = employeeService.UpdateEmployeeProfessionalDtls(empQual, empProfessionalDtls.EmployeeID);
                    TempData.Keep("EmployeeID");
                    ViewBag.ActiveTab = 4;
                    TempData["Message"] = "Successfully Updated";
                    // return View("_EditProfessionalDetails", empProfessionalDtls);
                    return Json(new { status = "1", empID = empProfessionalDtls.EmployeeID, activeTab = 4, msg = "Successfully Updated" }, JsonRequestBehavior.AllowGet);
                }
                return View("_EditProfessionalDetails", empProfessionalDtls);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        #endregion

        #region Tab  5

        [HttpGet]
        public ActionResult _EditPromotionAndIncrementDtls(int? employeeID)
        {
            log.Info($"EmployeeController/_EditPromotionAndIncrementDtls/{employeeID}");
            try
            {
                Model.Employee employee = new Model.Employee();
                if (employeeID.HasValue)
                    employee = employeeService.GetEmployeeByID(employeeID.Value);
                employee.E_Basic = employeeService.GetEmployeeSalaryDtls(employee.EmployeeCode)?.E_Basic ?? employee.E_Basic;
                return PartialView("_EditPromotionAndIncrementDetails", employee);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _EditPromotionAndIncrementDtls(Model.Employee employee)
        {
            log.Info($"EmployeeController/_EditPromotionAndIncrementDtls");
            try
            {
                //   BindDropdowns();

                ModelState.Remove("EmployeeCode");
                ModelState.Remove("OfficialEmail");
                ModelState.Remove("MobileNo");
                ModelState.Remove("Name");
                ModelState.Remove("DOB");
                ModelState.Remove("Pr_Loc_DOJ");
                ModelState.Remove("FirstDesg");
                ModelState.Remove("FirstBranch");
                ModelState.Remove("DOJ");
                if (ModelState.IsValid)
                {
                    int? incrementMonth = Request.Form["incrMnth"] != null ? (int?)Convert.ToInt32(Request.Form["incrMnth"]) : null;
                    employee.UpdatedBy = userDetail.UserID;
                    employee.UpdatedOn = DateTime.Now;
                    employee.EmployeeID = employee.EmployeeID == 0 ? Convert.ToInt32(TempData["EmployeeID"]) : employee.EmployeeID;
                    employee.IncrementMonth = incrementMonth;
                    bool isUpdated = employeeService.UpdateEmployeePromotionalAndIncrementDtls(employee);

                    TempData.Keep("EmployeeID");
                    ViewBag.ActiveTab = 5;
                    TempData["Message"] = "Successfully Updated";
                    return PartialView("_EditPromotionAndIncrementDetails", employee);

                }
                return PartialView("_EditPromotionAndIncrementDtls", employee);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }

        }
        #endregion

        #region Tab 6
        [HttpGet]
        public ActionResult _EditPayScaleDetails(int? employeeID)
        {
            log.Info($"EmployeeController/_EditPayScaleDetails/{employeeID}");
            try
            {
                Model.EmployeePayScale empPayScale = new Model.EmployeePayScale();

                if (employeeID.HasValue)
                {
                    Model.Employee emp = employeeService.GetEmployeeByID(employeeID.Value);
                    empPayScale.designationID = emp.DesignationID;
                    if (!string.IsNullOrEmpty(emp.EmpPayScale))
                    {
                        var payScale = emp.EmpPayScale.Split(new char[] { '-' }).Select(x => int.Parse(x)).AsEnumerable();

                        int[] basics = payScale.Where((x, i) => i % 2 == 0).ToArray<int>();
                        int[] increments = payScale.Where((x, i) => i % 2 != 0).ToArray<int>();

                        #region Basic 

                        empPayScale.B1 = basics.Length >= 1 ? basics[0] : 0;
                        empPayScale.B2 = basics.Length >= 2 ? basics[1] : 0;
                        empPayScale.B3 = basics.Length >= 3 ? basics[2] : 0;
                        empPayScale.B4 = basics.Length >= 4 ? basics[3] : 0;
                        empPayScale.B5 = basics.Length >= 5 ? basics[4] : 0;
                        empPayScale.B6 = basics.Length >= 6 ? basics[5] : 0;
                        empPayScale.B7 = basics.Length >= 7 ? basics[6] : 0;
                        empPayScale.B8 = basics.Length >= 8 ? basics[7] : 0;
                        empPayScale.B9 = basics.Length >= 9 ? basics[8] : 0;
                        empPayScale.B10 = basics.Length >= 10 ? basics[9] : 0;
                        empPayScale.B11 = basics.Length > 11 ? basics[10] : 0;
                        empPayScale.B12 = basics.Length >= 12 ? basics[11] : 0;
                        empPayScale.B13 = basics.Length >= 13 ? basics[12] : 0;
                        empPayScale.B14 = basics.Length >= 14 ? basics[13] : 0;
                        empPayScale.B15 = basics.Length >= 15 ? basics[14] : 0;
                        empPayScale.B16 = basics.Length >= 16 ? basics[15] : 0;
                        empPayScale.B17 = basics.Length >= 17 ? basics[16] : 0;
                        empPayScale.B18 = basics.Length >= 18 ? basics[17] : 0;
                        empPayScale.B19 = basics.Length >= 19 ? basics[18] : 0;
                        empPayScale.B20 = basics.Length >= 20 ? basics[19] : 0;
                        empPayScale.B21 = basics.Length >= 21 ? basics[20] : 0;
                        empPayScale.B22 = basics.Length >= 22 ? basics[21] : 0;
                        empPayScale.B23 = basics.Length >= 23 ? basics[22] : 0;
                        empPayScale.B24 = basics.Length >= 24 ? basics[23] : 0;
                        empPayScale.B25 = basics.Length >= 25 ? basics[24] : 0;
                        empPayScale.B26 = basics.Length >= 26 ? basics[25] : 0;
                        empPayScale.B27 = basics.Length >= 27 ? basics[26] : 0;
                        empPayScale.B28 = basics.Length >= 28 ? basics[27] : 0;
                        empPayScale.B29 = basics.Length >= 29 ? basics[28] : 0;
                        empPayScale.B30 = basics.Length >= 30 ? basics[29] : 0;
                        empPayScale.B31 = basics.Length >= 31 ? basics[30] : 0;
                        empPayScale.B32 = basics.Length >= 32 ? basics[31] : 0;
                        empPayScale.B33 = basics.Length >= 33 ? basics[32] : 0;
                        empPayScale.B34 = basics.Length >= 34 ? basics[33] : 0;
                        empPayScale.B35 = basics.Length >= 35 ? basics[34] : 0;
                        empPayScale.B36 = basics.Length >= 36 ? basics[35] : 0;
                        empPayScale.B37 = basics.Length >= 37 ? basics[36] : 0;
                        empPayScale.B38 = basics.Length >= 38 ? basics[37] : 0;
                        empPayScale.B39 = basics.Length >= 39 ? basics[38] : 0;
                        empPayScale.B40 = basics.Length >= 40 ? basics[39] : 0;

                        #endregion

                        #region Increment
                        empPayScale.I1 = increments.Length >= 1 ? increments[0] : 0;
                        empPayScale.I2 = increments.Length >= 2 ? increments[1] : 0;
                        empPayScale.I3 = increments.Length >= 3 ? increments[2] : 0;
                        empPayScale.I4 = increments.Length >= 4 ? increments[3] : 0;
                        empPayScale.I5 = increments.Length >= 5 ? increments[4] : 0;
                        empPayScale.I6 = increments.Length >= 6 ? increments[5] : 0;
                        empPayScale.I7 = increments.Length >= 7 ? increments[6] : 0;
                        empPayScale.I8 = increments.Length >= 8 ? increments[7] : 0;
                        empPayScale.I9 = increments.Length >= 9 ? increments[8] : 0;
                        empPayScale.I10 = increments.Length >= 10 ? increments[9] : 0;
                        empPayScale.I11 = increments.Length >= 11 ? increments[10] : 0;
                        empPayScale.I12 = increments.Length >= 12 ? increments[11] : 0;
                        empPayScale.I13 = increments.Length >= 13 ? increments[12] : 0;
                        empPayScale.I14 = increments.Length >= 14 ? increments[13] : 0;
                        empPayScale.I15 = increments.Length >= 15 ? increments[14] : 0;
                        empPayScale.I16 = increments.Length >= 16 ? increments[15] : 0;
                        empPayScale.I17 = increments.Length >= 17 ? increments[16] : 0;
                        empPayScale.I18 = increments.Length >= 18 ? increments[17] : 0;
                        empPayScale.I19 = increments.Length >= 19 ? increments[18] : 0;
                        empPayScale.I20 = increments.Length >= 20 ? increments[19] : 0;
                        empPayScale.I21 = increments.Length >= 21 ? increments[20] : 0;
                        empPayScale.I22 = increments.Length >= 22 ? increments[21] : 0;
                        empPayScale.I23 = increments.Length >= 23 ? increments[22] : 0;
                        empPayScale.I24 = increments.Length >= 24 ? increments[23] : 0;
                        empPayScale.I25 = increments.Length >= 25 ? increments[24] : 0;
                        empPayScale.I26 = increments.Length >= 26 ? increments[25] : 0;
                        empPayScale.I27 = increments.Length >= 27 ? increments[26] : 0;
                        empPayScale.I28 = increments.Length >= 28 ? increments[27] : 0;
                        empPayScale.I29 = increments.Length >= 29 ? increments[28] : 0;
                        empPayScale.I30 = increments.Length >= 30 ? increments[29] : 0;
                        empPayScale.I31 = increments.Length >= 31 ? increments[30] : 0;
                        empPayScale.I32 = increments.Length >= 32 ? increments[31] : 0;
                        empPayScale.I33 = increments.Length >= 33 ? increments[32] : 0;
                        empPayScale.I34 = increments.Length >= 34 ? increments[33] : 0;
                        empPayScale.I35 = increments.Length >= 35 ? increments[34] : 0;
                        empPayScale.I36 = increments.Length >= 36 ? increments[35] : 0;
                        empPayScale.I37 = increments.Length >= 37 ? increments[36] : 0;
                        empPayScale.I38 = increments.Length >= 38 ? increments[37] : 0;
                        empPayScale.I39 = increments.Length >= 39 ? increments[38] : 0;
                        empPayScale.I40 = increments.Length >= 40 ? increments[39] : 0;

                        #endregion}

                        //  return PartialView("_EditPayScaleDetails", empPayScale);
                    }
                    return PartialView("_EditPayScaleDetails", empPayScale);
                }
                return PartialView("_EditPayScaleDetails", empPayScale);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _EditPayScaleDetails(Model.EmployeePayScale payScale)
        {
            log.Info($"EmployeeController/_EditPayScaleDetails");
            try
            {
                ModelState.Remove("FirstDesg");
                ModelState.Remove("FirstBranch");
                ModelState.Remove("DOJ");
                if (ModelState.IsValid)
                {
                    if (payScale.level != null)
                    {
                        bool isUpdated = employeeService.UpdateEmployeePayScales(payScale);
                        TempData.Keep("EmployeeID");
                        ViewBag.ActiveTab = 6;
                        TempData["Message"] = "Successfully Updated";
                        return View("_EditPayScaleDetails", payScale);
                    }
                    else
                    {
                        payScale.AlertMessage = "Please check level value, it can not be blank.";
                        ViewBag.ActiveTab = 5;
                        return PartialView("_EditPayScaleDetails", payScale);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return PartialView("_EditPayScaleDetails", payScale);
        }
        #endregion

        #region Tab 7
        [HttpGet]
        public ActionResult _EditDeputationGridView(int employeeID)
        {

            Model.EmployeeDeputation empDeptation = new Model.EmployeeDeputation();
            log.Info($"EmployeeController/_EditDeputationGridView");
            try
            {
                empDeptation.EmployeeID = employeeID;
                //empDeptationList = employeeService.GetEmployeeDeputationDtls(employeeID, null);
                ViewBag.employeeID = employeeID;
                return PartialView("_EditDeputationGridView", empDeptation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _GetEditDeputationGridView(int employeeID)
        {
            log.Info($"EmployeeController/_GetEditDeputationGridView");
            IEnumerable<Model.EmployeeDeputation> empDeptationList = new List<Model.EmployeeDeputation>();
            try
            {
                empDeptationList = employeeService.GetEmployeeDeputationDtls(employeeID, null);
                return PartialView("_DeputationGridView", empDeptationList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _GetEmployeeDeputation(int employeeID, int? empDeputationID = null)
        {
            ModelState.Clear();
            Model.EmployeeDeputation empDeputation = new Model.EmployeeDeputation();

            if (empDeputationID.HasValue)
                empDeputation = employeeService.GetEmployeeDeputationDtls(employeeID, empDeputationID.Value).FirstOrDefault();
            else
            {
                empDeputation.FromDate = DateTime.Now.Date;
                empDeputation.ToDate = DateTime.Now.Date;
            }
            empDeputation.EmployeeID = employeeID;
            empDeputation.EmpDeputationID = empDeputationID == null ? 0 : (int)empDeputationID;
            return PartialView("_DeputationPopup", empDeputation);
        }

        [HttpPost]
        public ActionResult _PostEmployeeDeputation(Model.EmployeeDeputation empDeputation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ModelState.Clear();
                    var mode = empDeputation.EmpDeputationID > 0 ? "Edit" : "Insert";

                    if (empDeputation.FromDate > empDeputation.ToDate)
                    {
                        ModelState.AddModelError("InValidFromDate", "From date should be always less than To date.");
                        return PartialView("_DeputationPopup", empDeputation);
                    }
                    if (empDeputation.ToDate < empDeputation.FromDate)
                    {
                        ModelState.AddModelError("InValidToDate", "To date should be always greater than From date.");
                        return PartialView("_DeputationPopup", empDeputation);
                    }

                    empDeputation.CreatedBy = userDetail.UserID;
                    empDeputation.CreatedOn = DateTime.Now;

                    var result = employeeService.AddAndUpdateEmpDeputationInfo(empDeputation);
                    if (result)
                        return Json(new { mode = mode, status = result }, JsonRequestBehavior.AllowGet);
                    else
                        empDeputation.AlertMessage = "Record already exists";
                    return PartialView("_DeputationPopup", empDeputation);
                    //return Json(new { mode = mode, status = result }, JsonRequestBehavior.AllowGet);
                }
                return PartialView("_DeputationPopup", empDeputation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        public ActionResult DeleteEmployeeDeputationInfo(int employeeID, int empDeputationID)
        {
            log.Info($"EmployeeController/DeleteEmployeeDeputationInfo/{employeeID}/{empDeputationID}");
            try
            {
                employeeService.DeleteEmployeeDeputationInfo(employeeID, empDeputationID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Edit", new { employeeID = employeeID });
        }

        #endregion

        #region Tab 8

        [HttpGet]
        public ActionResult _GetEmpProcessApproval(int employeeID, int? userTypeID)
        {
            log.Info($"EmployeeController/_GetEmpProcessApproval/{employeeID}");
            try
            {
                Model.EmployeeProcessApprovalVM empVM = new Model.EmployeeProcessApprovalVM();

                if (!userTypeID.HasValue)
                {
                    var rr = userService.GetUserTypeByEmployeeID((int?)employeeID);
                    userTypeID = rr;
                }

                var empProcessApprovaList = employeeService.GetEmpApprovalProcesses(employeeID).OrderBy(x => x.ProcessID).ToList();
                empProcessApprovaList.ForEach(x =>
                {
                    x.RoleID = (int)userTypeID;
                });

                empVM.EmployeeList = ddlService.GetAllEmployee();
                empVM.EmployeeList.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });

                var ddlEmployee = ddlService.GetAllEmployee();
                Model.SelectListModel selectEmployee = new Model.SelectListModel();
                selectEmployee.id = 0;
                selectEmployee.value = "Select";
                ddlEmployee.RemoveAll(x => x.id == employeeID);
                ddlEmployee.Insert(0, selectEmployee);
                ViewBag.Employee = new SelectList(ddlEmployee, "id", "value");

                empVM.empProcessApp = empProcessApprovaList;
                return PartialView("_ApprovalProcess", empVM);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult _PostEmpProcessApproval(Model.EmployeeProcessApprovalVM empVM)
        {
            log.Info($"EmployeeController/_PostEmpProcessApprova");
            try
            {
                if (ModelState.IsValid)
                {
                    if (!empVM.empProcessApp.Any(x => x.EmpProcessAppID > 0))
                    {
                        empVM.empProcessApp.ForEach(em =>
                        {
                            em.CreatedBy = userDetail.UserID;
                            em.CreatedOn = DateTime.Now;
                            em.Fromdate = DateTime.Now;
                            em.ReviewingTo = em.ReviewingTo == 0 ? null : em.ReviewingTo;
                            em.AcceptanceAuthority = em.AcceptanceAuthority == 0 ? null : em.AcceptanceAuthority;
                        });

                        empVM.empProcessApp = empVM.empProcessApp.Where(x => x.ReportingTo != 0).ToList();


                        var result = employeeService.InsertEmpProcessApproval(empVM.empProcessApp);
                        if (result)
                            return Json(new { employeeID = empVM.empProcessApp[0].EmployeeID, userTypeID = empVM.empProcessApp[0].RoleID, part = 0, msgType = "success", msg = "Employee process approval successfully configured." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Model.EmployeeProcessApprovalVM newempVM = new Model.EmployeeProcessApprovalVM();
                        var result = false;
                        empVM.empProcessApp.ForEach(em =>
                        {
                            em.ReviewingTo = em.ReviewingTo == 0 ? null : em.ReviewingTo;
                            em.AcceptanceAuthority = em.AcceptanceAuthority == 0 ? null : em.AcceptanceAuthority;
                            em.OldReviewingTo = em.OldReviewingTo == 0 ? null : em.OldReviewingTo;
                            em.OldAcceptanceAuthority = em.OldAcceptanceAuthority == 0 ? null : em.OldAcceptanceAuthority;

                            if (em.OldReportingTo == em.ReportingTo && em.OldReviewingTo == em.ReviewingTo && em.OldAcceptanceAuthority == em.AcceptanceAuthority)
                            {
                            }
                            else
                            {
                                newempVM.empProcessApp.Add(new Model.EmployeeProcessApproval()
                                {
                                    AcceptanceAuthority = em.AcceptanceAuthority,
                                    ReportingTo = em.ReportingTo,
                                    ReviewingTo = em.ReviewingTo,
                                    EmpProcessAppID = em.EmpProcessAppID,
                                    ProcessID = em.ProcessID,
                                    ToDate = DateTime.Now,
                                    EmployeeID = em.EmployeeID,
                                    UpdatedBy = userDetail.UserID,
                                    UpdatedOn = DateTime.Now,
                                    CreatedBy = userDetail.UserID,
                                    CreatedOn = DateTime.Now,
                                    Fromdate = DateTime.Now,
                                    RoleID = em.RoleID
                                });
                            }

                        });

                        newempVM.empProcessApp = newempVM.empProcessApp.Where(x => x.ReportingTo != 0).ToList();
                        if (newempVM.empProcessApp.Count > 0)
                        {
                            result = employeeService.UpdateEmpProcessApproval(newempVM.empProcessApp);
                        }
                        else
                        {
                            result = true;
                        }
                        if (result)
                        {
                            //RedirectToAction("_GetEmpProcessApproval", new { employeeID = newempVM.empProcessApp[0].EmployeeID, userTypeID = 0 });
                            return Json(new { employeeID = empVM.empProcessApp[0].EmployeeID, userTypeID = empVM.empProcessApp[0].RoleID, part = 0, msgType = "success", msg = "Employee process approval successfully configured." }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            empVM.EmployeeList = ddlService.GetAllEmployee();
                            empVM.EmployeeList.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });
                        }
                    }
                }
                else
                {
                    empVM.EmployeeList = ddlService.GetAllEmployee();
                    empVM.EmployeeList.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });
                }
                return PartialView("_ApprovalProcess", empVM);


            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        #endregion

        #endregion

        [HttpGet]
        public ActionResult CreateGeneralDetails()
        {
            log.Info("EmployeeController/CreateGeneralDetails");
            BindDropdowns();
            BindDesignationBranch();         
            Model.Employee employeeDetails = new Model.Employee();
   
            return PartialView("_CreateGeneralDetails", employeeDetails);
        }

        [HttpPost]
        public ActionResult CreateGeneralDetails(Model.Employee employee)
        {
            log.Info("EmployeeController/CreateGeneralDetails");
            try
            {
                BindDropdowns();
                BindDesignationBranch();
                //var designationID = frm.Get("HdnDesignationID");
                //var branchID = frm.Get("HdnBranchID");

                ModelState.Remove("EmployeeCode");

                if (employee.TitleID == 0)
                    ModelState.AddModelError("TitleRequired", "Select Title");
                if (employee.EmployeeTypeID == 0)
                    ModelState.AddModelError("EmployeeTypeRequired", "Select Employee Type");
                if (employee.GenderID == 0)
                    ModelState.AddModelError("EmployeeGenderRequired", "Select Gender");
                //if (employee.DesignationID == 0)
                //    ModelState.AddModelError("EmployeeDesignationRequired", "Select Employee Designation");

                //if (employee.CadreID == 0)
                //    ModelState.AddModelError("CadreRequired", "Select Cadre");

                //if (employee.BranchID == 0)
                //    ModelState.AddModelError("BranchRequired", "Select Branch");

                if (employee.DepartmentID == 0)
                    ModelState.AddModelError("DepartmentRequired", "Select Department");
                if (employee.DesignationID == 0)
                    ModelState.AddModelError("DepartmentRequired", "Select Department");
                if (employee.SectionID == 0)
                    ModelState.AddModelError("SectionRequired", "Select Section");
                if (!employee.DOJ.HasValue)
                    ModelState.AddModelError("", "");
                ModelState.Remove("DOB");

                if (employee.modOfPayment == Nafed.MicroPay.Model.ModeOfPayment.Bank)
                {
                    if (string.IsNullOrEmpty(employee.BankCode))
                        ModelState.AddModelError("BankRequired", "Select Bank Name");
                    if (string.IsNullOrEmpty(employee.BankAcNo))
                        ModelState.AddModelError("BankAcNoRequired", "Please Enter Bank Account No.");
                }
                if (ModelState.IsValid)
                {

                    //var joinYear =  employee.Pr_Loc_DOJ.Value.Year;
                    //var year = Convert.ToString(joinYear) + "-" + Convert.ToString(Convert.ToString(joinYear + 1).Substring(2, 2));
                    //if (!employeeService.IsStaffBudgetAvailable(employee.DesignationID, year, "D"))
                    //{
                    //    return Json(new { status = "0", msg = "There is no any vacant post for the selected designation for this year." }, JsonRequestBehavior.AllowGet);
                    //}


                    ////////employee.EmployeeCode = employee.EmployeeCode.Trim();
                    ////////if (employeeService.EmployeeDetailsExists(employee.EmployeeID, employee.EmployeeCode))
                    ////////    ModelState.AddModelError("EmployeeCodeAlreadyExist", "Employee Code Already Exist");
                    ////////else
                    ////////{
                        employee.CreatedBy = userDetail.UserID;
                        employee.CreatedOn = DateTime.Now;
                        //  employee.DOJ = DateTime.Now;
                        employee.ReviewerTo = employee.ReviewerTo == 0 ? null : employee.ReviewerTo;
                        employee.ReportingToID = employee.ReportingToID == 0 ? null : employee.ReportingToID;
                        employee.AcceptanceAuthority = employee.AcceptanceAuthority == 0 ? null : employee.AcceptanceAuthority;
                        //employee.DesignationID = int.Parse(designationID);
                        //employee.BranchID = int.Parse(branchID);

                        int employeeDetailsID = employeeService.InsertEmployeeDetails(employee);

                        TempData["EmployeeID"] = employeeDetailsID;

                        TempData.Keep("EmployeeID");
                        ViewBag.ActiveTab = 1;
                        ViewBag.EmployeeID = employeeDetailsID;
                        //  TempData["Message"] = "Successfully Created";
                        // return PartialView("_CreateGeneralDetails", employee);

                        return Json(new { status = "1", empID = employeeDetailsID, activeTab = 1, msg = "Successfully Created" }, JsonRequestBehavior.AllowGet);
                   ////////}
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

        public ActionResult Delete(int employeeID)
        {
            log.Info($"EmployeeController/Delete/{employeeID}");
            try
            {
                employeeService.DeleteEmployee(employeeID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

        #region P R I V A T E    M E T H O D S
        /// <summary>
        /// use this funtion at the time of creation of employee, bcoz in this function we had used only active branch/Designation
        /// </summary>
        private void BindDesignationBranch() 
        {
            var ddlDesignationList = ddlService.ddlDesignationList();
            ddlDesignationList.OrderBy(x => x.value);
            Model.SelectListModel selectDesignation = new Model.SelectListModel();
            selectDesignation.id = 0;
            selectDesignation.value = "Select";
            ddlDesignationList.Insert(0, selectDesignation);
            ViewBag.Designation = new SelectList(ddlDesignationList, "id", "value");

            var ddlBranchList = ddlService.ddlBranchList();
            ddlBranchList.OrderBy(x => x.value);
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");
        }

        private void BindDropdowns()
        {
            var ddlTitleList = ddlService.ddlTitleList();
            ddlTitleList.OrderBy(x => x.value);
            Model.SelectListModel selectTitle = new Model.SelectListModel();
            selectTitle.id = 0;
            selectTitle.value = "Select";
            ddlTitleList.Insert(0, selectTitle);
            ViewBag.Title = new SelectList(ddlTitleList, "id", "value");


            var ddlEmployeeTypeList = ddlService.ddlEmployeeTypeList();
            ddlEmployeeTypeList.OrderBy(x => x.value);
            Model.SelectListModel selectEmployeeType = new Model.SelectListModel();
            selectEmployeeType.id = 0;
            selectEmployeeType.value = "Select";
            ddlEmployeeTypeList.Insert(0, selectEmployeeType);
            ViewBag.EmployeeType = new SelectList(ddlEmployeeTypeList, "id", "value");

            var ddlGenderList = ddlService.ddlGenderList();
            ddlGenderList.OrderBy(x => x.value);
            Model.SelectListModel selectGender = new Model.SelectListModel();
            selectGender.id = 0;
            selectGender.value = "Select";
            ddlGenderList.Insert(0, selectGender);
            ViewBag.Gender = new SelectList(ddlGenderList, "id", "value");

            var ddlDesignationList = ddlService.ddlFirstDesignationList();
            ddlDesignationList.OrderBy(x => x.value);
            Model.SelectListModel selectDesignation = new Model.SelectListModel();
            selectDesignation.id = 0;
            selectDesignation.value = "Select";
            ddlDesignationList.Insert(0, selectDesignation);
            ViewBag.Designation = new SelectList(ddlDesignationList, "id", "value");

            var ddlCadreCodeList = ddlService.ddlCadreCodeList();
            ddlCadreCodeList.OrderBy(x => x.value);
            Model.SelectListModel selectCadreCode = new Model.SelectListModel();
            selectCadreCode.id = 0;
            selectCadreCode.value = "Select";
            ddlCadreCodeList.Insert(0, selectCadreCode);
            ViewBag.CadreCode = new SelectList(ddlCadreCodeList, "id", "value");

            var ddlBranchList = ddlService.ddlFirstBranchList();
            ddlBranchList.OrderBy(x => x.value);
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            var ddlSectionList = ddlService.ddlSectionList();
            ddlSectionList.OrderBy(x => x.value);
            Model.SelectListModel selectSection = new Model.SelectListModel();
            selectSection.id = 0;
            selectSection.value = "Select";
            ddlSectionList.Insert(0, selectSection);
            ViewBag.Section = new SelectList(ddlSectionList, "id", "value");

            var ddlCategoryList = ddlService.ddlCategoryList();
            ddlCategoryList.OrderBy(x => x.value);
            Model.SelectListModel selectCategory = new Model.SelectListModel();
            selectCategory.id = 0;
            selectCategory.value = "Select";
            ddlCategoryList.Insert(0, selectCategory);
            ViewBag.Category = new SelectList(ddlCategoryList, "id", "value");

            var ddlDepartmentList = ddlService.ddlDepartmentList();
            ddlDepartmentList.OrderBy(x => x.value);
            Model.SelectListModel selectDepartment = new Model.SelectListModel();
            selectDepartment.id = 0;
            selectDepartment.value = "Select";
            ddlDepartmentList.Insert(0, selectDepartment);
            ViewBag.Department = new SelectList(ddlDepartmentList, "id", "value");


            var ddlReligionList = ddlService.ddlReligionList();
            ddlReligionList.OrderBy(x => x.value);
            Model.SelectListModel selectReligion = new Model.SelectListModel();
            selectReligion.id = 0;
            selectReligion.value = "Select";
            ddlReligionList.Insert(0, selectReligion);
            ViewBag.Religion = new SelectList(ddlReligionList, "id", "value");

            var ddlMotherTongueList = ddlService.ddlMotherTongueList();
            ddlMotherTongueList.OrderBy(x => x.value);
            Model.SelectListModel motherTongue = new Model.SelectListModel();
            motherTongue.id = 0;
            motherTongue.value = "Select";
            ddlMotherTongueList.Insert(0, motherTongue);
            ViewBag.MotherTongue = new SelectList(ddlMotherTongueList, "id", "value");

            var ddlMaritalStsList = ddlService.ddlMaritalStsList();
            ddlMaritalStsList.OrderBy(x => x.value);
            Model.SelectListModel maritalSts = new Model.SelectListModel();
            maritalSts.id = 0;
            maritalSts.value = "Select";
            ddlMaritalStsList.Insert(0, maritalSts);
            ViewBag.MaritalSts = new SelectList(ddlMaritalStsList, "id", "value");

            var ddlBloodGroupList = ddlService.ddlBloodGroupList();
            ddlBloodGroupList.OrderBy(x => x.value);
            Model.SelectListModel bloodGroup = new Model.SelectListModel();
            bloodGroup.id = 0;
            bloodGroup.value = "Select";
            ddlBloodGroupList.Insert(0, bloodGroup);
            ViewBag.BloodGroup = new SelectList(ddlBloodGroupList, "id", "value");


            var ddlRelationList = ddlService.ddlRelationList();
            ddlRelationList.OrderBy(x => x.value);
            Model.SelectListModel relation = new Model.SelectListModel();
            relation.id = 0;
            relation.value = "Select";
            ddlRelationList.Insert(0, relation);
            ViewBag.Relation = new SelectList(ddlRelationList, "id", "value");


            var ddlEmpCategory = ddlService.ddlEmployeeCategoryList();
            ddlEmpCategory.OrderBy(x => x.value);
            Model.SelectListModel empCateogry = new Model.SelectListModel();
            empCateogry.id = 0;
            empCateogry.value = "Select";
            ddlEmpCategory.Insert(0, empCateogry);
            ViewBag.EmpCategory = new SelectList(ddlEmpCategory, "id", "value");


            var ddlBanks = ddlService.ddlBanks();
            ddlBanks.OrderBy(x => x.value);
            ViewBag.Banks = ddlBanks;

            var ddlState = ddlService.ddlStateList();
            ddlState.OrderBy(x => x.value);
            Model.SelectListModel State = new Model.SelectListModel();
            State.id = 0;
            State.value = "Select";
            ddlState.Insert(0, State);
            ViewBag.PresentState = new SelectList(ddlState, "id", "value");

        }

        private bool SaveFile(HttpPostedFileBase file, Common.DocumentType docType, string documentFile)
        {
            log.Info($"EmployeeController/SaveFile");

            string fileName = string.Empty; string filePath = string.Empty;
            try
            {
                if (file != null)
                {
                    fileName = Path.GetFileName(file.FileName);

                    var contentType = GetFileContentType(file.InputStream);
                    var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                    if (!IsValidFileName(file.FileName))
                    {
                        return false;
                    }
                    if (dicValue != contentType)
                    {                        
                        return false;
                    }

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (docType == Common.DocumentType.PanCard)
                        {
                            fileName = documentFile;
                            // Common.ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(file.FileName),
                            //Path.GetExtension(file.FileName));
                            filePath = "~/" + Common.DocumentUploadFilePath.PanCardFilePath;
                        }

                        if (docType == Common.DocumentType.AadhaarCard)
                        {
                            fileName = documentFile;
                            filePath = "~/" + Common.DocumentUploadFilePath.AadhaarCardFilePath;
                        }
                        string uploadedFilePath = Server.MapPath(filePath);
                        string sPhysicalPath = Path.Combine(uploadedFilePath, fileName);

                        file.SaveAs(sPhysicalPath);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                throw ex;
            }
            return true;
        }

        #endregion

        public JsonResult CheckEmployeeCode(string empCode)
        {
            log.Info($"EmployeeController/CheckEmployeeCode/{empCode}");
            var isExists = employeeService.CheckEmployeeCodeExistance(empCode);
            return Json(new { isExists = isExists }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCadreByDesignation(int designationID)
        {
            log.Info($"EmployeeController/GetCadreByDesignation/{designationID}");
            var res = employeeService.GetDesignationDtls(designationID);
            return Json(new { cadreID = res.CadreID }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult EmployeeGridView(FormCollection formCollection)
        //{
        //    log.Info($"EmployeeController/EmployeeGridView");
        //    try
        //    {
        //        EmployeeViewModel employeeVM = new EmployeeViewModel();
        //        var emp = employeeService.GetEmployeeList();
        //        emp.ForEach(x =>
        //        {
        //            x.EmpProfilePhotoUNCPath =
        //               x.EmpProfilePhotoUNCPath == null ?
        //               Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png")
        //               :

        //              Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + $"/{x.EmpProfilePhotoUNCPath}")) ?

        //               Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + $"/{x.EmpProfilePhotoUNCPath}") :
        //               Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");
        //        });
        //        employeeVM.Employee = emp;
        //        employeeVM.userRights = userAccessRight;
        //        return PartialView("_EmployeeGridView", employeeVM);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //}


        [HttpGet]
        public PartialViewResult _GetEmployeeGridView(EmployeeViewModel empVM)
        {
            EmployeeViewModel employeeVM = new EmployeeViewModel();
            var emp = employeeService.GetEmployeeList(empVM.EmployeeName, empVM.EmployeeCode, empVM.DesignationID, empVM.EmployeeTypeID);
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

            //  var p = System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + emp.FirstOrDefault().EmpProfilePhotoUNCPath));
            //   var dd = Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/"+"Pic-20201151412721.jpg"));
            employeeVM.userRights = userAccessRight;
            return PartialView("_EmployeeGridView", employeeVM);
        }


        [HttpGet]
        public ActionResult GetEmployeeDependent(int employeeID)
        {
            log.Info($"EmployeeController/GetEmployeeDependent/{employeeID}");
            try
            {
                var dependentModel = dependentService.GetDependentList(employeeID);
                return PartialView("_ViewDependent", dependentModel);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return Content("refresh-Data");
        }


        public ActionResult CalculateBasic(Model.EmployeePayScale payScale)
        {
            log.Info($"EmployeeController/CalculateBasic");
            try
            {
                var level = employeeService.GetDesignationDtls(payScale.designationID).Level;
                payScale.level = level;
                if (payScale.B1 != 0)
                {
                    if (level != null)
                    {
                        payScale.level = level.Length < 2 ? "0" + level : level;
                        var payScaleDetails = designationService.CalculateBasicAndIncrement(payScale);
                        ViewBag.ActiveTab = 5;
                        return PartialView("_EditPayScaleDetails", payScaleDetails);
                    }
                    else
                    {
                        payScale.AlertMessage = "Please check level value, it can not be blank.";
                        ViewBag.ActiveTab = 5;
                        return PartialView("_EditPayScaleDetails", payScale);
                    }
                }
                else
                {
                    return PartialView("_EditPayScaleDetails", payScale);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult _GetEmployeeAchievement(int? employeeID)
        {
            log.Info($"Employee/_GetEmployeeAchievement/employeeID:{employeeID}");

            if (employeeID.HasValue)
            {
                var dataList = employeeService.GetEmployeeAchievement(employeeID.Value);

                if (dataList?.Count > 0)
                {
                    var dItems = dataList.Where(x => x.documents.Count > 0).ToList();

                    foreach (var item in dItems)
                    {
                        item.documents.ToList().ForEach(x =>
                        {
                            x.DocumentUNCPath =
                             System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                             Common.DocumentUploadFilePath.EmployeeAchievement + "/" + x.DocumentFilePath)) ?

                           Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.EmployeeAchievement + "/" + x.DocumentFilePath) :
                           "#";
                        });
                    }
                }

                return PartialView("_EmployeeAcheivementGridView", dataList);
            }
            else
            {
                return PartialView("_EditEmployeeAcheivement");
            }
        }


        [HttpGet]
        public ActionResult _GetEmployeeCertification(int? employeeID)
        {
            log.Info($"Employee/_GetEmployeeCertification/employeeID:{employeeID}");

            if (employeeID.HasValue)
            {
                var dataList = employeeService.GetEmployeeCertification(employeeID.Value);

                if (dataList?.Count > 0)
                {
                    var dItems = dataList.Where(x => x.documents.Count > 0).ToList();

                    foreach (var item in dItems)
                    {
                        item.documents.ToList().ForEach(x =>
                        {
                            x.DocumentUNCPath =
                             System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                             Common.DocumentUploadFilePath.EmployeeCertification + "/" + x.DocumentFilePath)) ?

                           Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.EmployeeCertification + "/" + x.DocumentFilePath) :
                           "#";
                        });
                    }
                }
                return PartialView("_EmployeeCertificationGridView", dataList);
            }
            else
            {
                return PartialView("_EditEmployeeCertification");
            }

        }
        [HttpGet]
        public ActionResult _GetEmployeeAchievementForm(int? empAcheivementID)
        {
            log.Info($"Employee/_GetEmployeeAchievementForm/empAcheivementID:{empAcheivementID}");

            Model.EmployeeAchievement form = new Model.EmployeeAchievement();

            form.EmpAchievementAndCertificationDocuments = new Nafed.MicroPay.Model.EmpAchievementAndCertificationDocument();


            return PartialView("_EmployeeAcheivementForm", form);
        }


        [HttpGet]
        public ActionResult _GetEmployeeCertificationForm(int? empCertificationID)
        {
            log.Info($"Employee/_GetEmployeeCertificationForm/empCertificationID:{empCertificationID}");

            Model.EmployeeCertification form = new Model.EmployeeCertification();
            form.EmpAchievementAndCertificationDocuments = new Model.EmpAchievementAndCertificationDocument();

            return PartialView("_EmployeeCertificationForm", form);
        }

        public ActionResult _UploadAchievementOrCertificationSection(int maxNoOfFiles)
        {
            log.Info($"Employee/maxNoOfFiles/maxNoOfFiles={maxNoOfFiles}");

            List<Model.EmpAchievementAndCertificationDocument> files = new List<Model.EmpAchievementAndCertificationDocument>();
            var sno = 0;
            while (sno < 5)
            {
                files.Add(new Nafed.MicroPay.Model.EmpAchievementAndCertificationDocument());
                sno++;
            }
            return PartialView("_EmpAcheivementOrCertificationFiles", files);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult _PostEmployeeAchievement(FormCollection collection)
        {
            Model.EmployeeAchievement empAchievement = new Model.EmployeeAchievement();

            empAchievement.EmployeeID = Convert.ToInt32(((string[])collection.GetValue("EmployeeID").RawValue)[0]);
            List<Model.EmpAchievementAndCertificationDocument> documents = new List<Model.EmpAchievementAndCertificationDocument>();

            if (Request.Files.Count > 0)
            {
                var docname = collection.Get("FileName").ToString();
                var documentName = docname.Split(',');
                string fileName = string.Empty; string filePath = string.Empty;
                string fname = "";
                //     var DocNames = collection.GetValue("FileName").RawValue;
                //  Get all files from Request object  
                HttpFileCollectionBase files = Request.Files;

                #region Check Mime Type
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    if (!IsValidFileName(file.FileName))
                    {
                        stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                        stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                        stringBuilder.Append($"I. a to z characters.");
                        stringBuilder.Append($"II. numbers(0 to 9).");
                        stringBuilder.Append($"III. - and _ with space.");
                    }
                    var contentType = GetFileContentType(file.InputStream);
                    var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                    if (dicValue != contentType)
                    {
                        stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                        stringBuilder.Append("<br>");
                    }
                }
                if (stringBuilder.ToString() != "")
                {
                    return Json(new
                    {
                        status = 1,
                        type = "error",
                        msg = stringBuilder.ToString()
                    }, JsonRequestBehavior.AllowGet);
                   
                }
                #endregion

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];

                    fname = Common.ExtensionMethods.SetUniqueFileName("Document-",
                 Path.GetExtension(file.FileName));
                    fileName = fname;
                    string fullPath = Request.MapPath("~/Document/EmployeeAchievement/" + fileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    fname = Path.Combine(Server.MapPath("~/Document/EmployeeAchievement/"), fname);
                    file.SaveAs(fname);

                    documents.Add(new Model.EmpAchievementAndCertificationDocument()
                    {
                        DocumentTitle = documentName[i],
                        DocumentFilePath = fileName,
                        CreatedBy = userDetail.UserID,
                        CreatedOn = DateTime.Now,
                        Sno = 1
                    });
                }
            }
            var AchievementRemark = ((string[])(collection.GetValue("AchievementRemark").RawValue))[0];
            var AchievementName = ((string[])(collection.GetValue("AchievementName").RawValue))[0];
            var doA = ((string[])(collection.GetValue("DateOfAchievement").RawValue))[0];
            //string newDOA = doA.Substring(0, doA.LastIndexOf("G") - 1);

            empAchievement.DateOfAchievement = Convert.ToDateTime(doA);
            empAchievement.AchievementRemark = AchievementRemark;
            empAchievement.AchievementName = AchievementName;
            empAchievement.CreatedBy = userDetail.UserID;
            empAchievement.CreatedOn = DateTime.Now;

            if (ValidateAntiXSS(empAchievement))
            {
                return Json(new
                {
                    status = 1,
                    type = "error",
                    msg = "Updation failed, A potentially dangerous request value was detected.",
                }, JsonRequestBehavior.AllowGet);
            }

            employeeService.UpdateEmployeeAcheivement(empAchievement, documents);

            return Json(new
            {
                status = 1,
                type = "success",
                msg = "Record updated successfully."
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult _PostEmployeeCertification(FormCollection collection)
        {
            Model.EmployeeCertification empCertification = new Model.EmployeeCertification();

            empCertification.EmployeeID = Convert.ToInt32(((string[])collection.GetValue("EmployeeID").RawValue)[0]);
            List<Model.EmpAchievementAndCertificationDocument> documents = new List<Model.EmpAchievementAndCertificationDocument>();

            if (Request.Files.Count > 0)
            {
                var docname = collection.Get("FileName").ToString();
                var documentName = docname.Split(',');
                string fileName = string.Empty; string filePath = string.Empty;
                string fname = "";
                //     var DocNames = collection.GetValue("FileName").RawValue;
                //  Get all files from Request object  
                HttpFileCollectionBase files = Request.Files;

                #region Check Mime Type
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    if (!IsValidFileName(file.FileName))
                    {
                        stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                        stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                        stringBuilder.Append($"I. a to z characters.");
                        stringBuilder.Append($"II. numbers(0 to 9).");
                        stringBuilder.Append($"III. - and _ with space.");
                    }
                    var contentType = GetFileContentType(file.InputStream);
                    var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));

                    if (dicValue != contentType)
                    {
                        stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                        stringBuilder.Append("<br>");
                    }
                }
                if (stringBuilder.ToString() != "")
                {
                    return Json(new
                    {
                        status = 1,
                        type = "error",
                        msg = stringBuilder.ToString()

                    }, JsonRequestBehavior.AllowGet);
                   
                }
                #endregion

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];

                    fname = Common.ExtensionMethods.SetUniqueFileName("Document-",
                 Path.GetExtension(file.FileName));
                    fileName = fname;
                    string fullPath = Request.MapPath("~/Document/EmployeeCertification/" + fileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    fname = Path.Combine(Server.MapPath("~/Document/EmployeeCertification/"), fname);
                    file.SaveAs(fname);

                    documents.Add(new Model.EmpAchievementAndCertificationDocument()
                    {
                        DocumentTitle = documentName[i],
                        DocumentFilePath = fileName,
                        CreatedBy = userDetail.UserID,
                        CreatedOn = DateTime.Now,
                        Sno = 1
                    });
                }
            }
            var CertificationRemark = ((string[])(collection.GetValue("CertificationRemark").RawValue))[0];
            var CertificationName = ((string[])(collection.GetValue("CertificationName").RawValue))[0];

            var doI = ((string[])(collection.GetValue("DateOfIssue").RawValue))[0];
            //string newDOI = doI.Substring(0, doI.LastIndexOf("G") - 1);

            empCertification.DateOfIssue = Convert.ToDateTime(doI);
            empCertification.CertificationName = CertificationName;
            empCertification.CertificationRemark = CertificationRemark;
            empCertification.CreatedBy = userDetail.UserID;
            empCertification.CreatedOn = DateTime.Now;

            if (ValidateAntiXSS(empCertification))
            {
                return Json(new
                {
                    status = 1,
                    type = "error",
                    msg = "Updation failed, A potentially dangerous request value was detected.",
                }, JsonRequestBehavior.AllowGet);
            }

            employeeService.UpdateEmployeeCertification(empCertification, documents);

            return Json(new
            {
                status = 1,
                type = "success",
                msg = "Record updated successfully."

            }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult _AddDocumentRow()
        {
            Model.EmpAchievementAndCertificationDocument newDocument = new Model.EmpAchievementAndCertificationDocument();
            return Json(new { htmlData = ConvertViewToString("_EmpAcheivementOrCertificationFiles", newDocument) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteEmpAcheivement(int empAcheivementID, int employeeID)
        {
            log.Info($"Employee/DeleteEmpAcheivement/empAcheivementID:{empAcheivementID}/employeeID:{employeeID}");
            try
            {
                var result = employeeService.DeleteEmpAchievement(empAcheivementID, userDetail.UserID);
                if (result)
                    return Json(new
                    {
                        type = "success",
                        msg = "Record deleted successfully.",
                        htmlData =
                        ConvertViewToString("_EmployeeAcheivementGridView", employeeService.GetEmployeeAchievement(employeeID))
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return Content("");
        }

        public ActionResult DeleteEmpCertificate(int empCertificateID, int employeeID)
        {
            log.Info($"Employee/DeleteEmpCertificate/empCertificateID:{empCertificateID}/employeeID:{employeeID}");
            try
            {
                var result = employeeService.DeleteEmpCertificate(empCertificateID, userDetail.UserID);
                if (result)
                    return Json(new
                    {
                        type = "success",
                        msg = "Record deleted successfully.",
                        htmlData =
                        ConvertViewToString("_EmployeeCertificationGridView", employeeService.GetEmployeeCertification(employeeID))
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return Content("");
        }



    }
}