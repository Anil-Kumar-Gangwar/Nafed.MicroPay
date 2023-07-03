using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
namespace MicroPay.Web.Controllers
{
    public class LTCController : BaseController
    {
        private readonly ILTCService LTCService;
        private readonly IDropdownBindService ddlService;
        private readonly IConfirmationFormService confirmationService;

        public LTCController(IDropdownBindService ddlService, ILTCService LTCService, IConfirmationFormService confirmationService)
        {
            this.ddlService = ddlService;
            this.LTCService = LTCService;
            this.confirmationService = confirmationService;
        }
        public ActionResult Index()
        {
            log.Info($"LTCController/Index");
            return View(userAccessRight);
        }

        public ActionResult LTCGridView(FormCollection formCollection)
        {
            log.Info($"LTCController/LTCGridView");
            try
            {
                LTCViewModel LTCVM = new LTCViewModel();
                int? empID = null;
                if (userDetail.UserName == "Admin")
                    empID = null;
                else empID = (int)userDetail.EmployeeID;
                LTCVM.LTCList = LTCService.GetLTCList(empID);
                LTCVM.userRights = userAccessRight;
                var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.LTC);
                LTCVM.approvalSetting = approvalSettings ?? new Nafed.MicroPay.Model.EmployeeProcessApproval();
                return PartialView("_LTCGridView", LTCVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("LTCController/Create");
            try
            {
                BindDropdowns();
                Model.LTC objLTC = new Model.LTC();
                objLTC.EmployeeId = (int)userDetail.EmployeeID;
                objLTC.BranchId = userDetail.BranchID;
                objLTC.Employeename = userDetail.FullName;
                objLTC.Employeecode = userDetail.EmployeeCode;
                objLTC.BranchName = userDetail.Location;
                objLTC.DesignationName = userDetail.Designation;
                objLTC.DepartmentName = userDetail.DepartmentName;
                objLTC.ReportingYear = DateTime.Now.GetFinancialYr();
                objLTC.loggedInEmpID = (int)userDetail.EmployeeID;
                objLTC.FormStatus = 1;
                var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.LTC);
                objLTC.approvalSetting = approvalSettings ?? new Nafed.MicroPay.Model.EmployeeProcessApproval();
                if ((objLTC.approvalSetting.ReportingTo == objLTC.approvalSetting.ReviewingTo) && (objLTC.approvalSetting.ReviewingTo == objLTC.approvalSetting.AcceptanceAuthority))
                    objLTC.ApprovalHierarchy = 3;
                else if (((objLTC.approvalSetting.ReportingTo != objLTC.approvalSetting.ReviewingTo) && (objLTC.approvalSetting.ReviewingTo == objLTC.approvalSetting.AcceptanceAuthority))
                    && objLTC.loggedInEmpID == objLTC.approvalSetting.ReviewingTo)
                    objLTC.ApprovalHierarchy = 2.1;
                else if (((objLTC.approvalSetting.ReportingTo == objLTC.approvalSetting.ReviewingTo) && (objLTC.approvalSetting.ReviewingTo != objLTC.approvalSetting.AcceptanceAuthority))
                    && (objLTC.loggedInEmpID == objLTC.approvalSetting.ReportingTo || objLTC.loggedInEmpID == objLTC.approvalSetting.AcceptanceAuthority))
                    objLTC.ApprovalHierarchy = 2.0;
                else
                    objLTC.ApprovalHierarchy = 1;

                TempData["LTCView"] = objLTC;
                TempData.Keep("LTCView");
                return View(objLTC);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.LTC createLTC, string ButtonType)
        {
            log.Info("LTCController/Create");
            try
            {
                BindDropdowns();
                if (createLTC.HomeTown == 0)
                {
                    ModelState.AddModelError("HomeTownRequired", "Please select Where Want to Go, Whether HomeTown Or AnyWhere Else");

                    createLTC = (Model.LTC)TempData["LTCView"];
                    TempData.Keep("LTCView");
                    return View(createLTC);
                }
                if (createLTC.WhetherSelf == false && createLTC.Dependents == null)
                {
                    TempData["Error"] = "Please Specify that Who Want to Avail the LTC.";
                    createLTC = (Model.LTC)TempData["LTCView"];
                    TempData.Keep("LTCView");
                    return View(createLTC);
                }
                if (createLTC.HomeTown == 2 && createLTC.WhereDetail == null)
                {
                    TempData["Error"] = "Please Enter Where He/She Want to Go on LTC.";
                    createLTC = (Model.LTC)TempData["LTCView"];
                    TempData.Keep("LTCView");
                    return View(createLTC);
                }
                if (createLTC.DateofReturn < createLTC.DateAvailLTC)
                {
                    TempData["Error"] = "Date of Return cann't be Less than Date of Avail of LTC.";
                    createLTC = (Model.LTC)TempData["LTCView"];
                    TempData.Keep("LTCView");
                    return View(createLTC);
                }
                if (createLTC.Dependentid != null && createLTC.Dependentid.Length > 0)
                {
                    for (int i = 0; i < createLTC.Dependentid.Length; i++)
                    {
                        createLTC.Dependents += createLTC.Dependentid[i] + ",";
                    }
                }
                if (createLTC.Dependents != null && createLTC.Dependents != string.Empty)
                {
                    createLTC.Dependents = createLTC.Dependents.Substring(0, createLTC.Dependents.Length - 1);
                }
                if (createLTC.WhetherSelf == true && createLTC.EmployeeId == createLTC.loggedInEmpID && createLTC.LTCNo == 0)
                {
                    DateTime dateofreturn = LTCService.GetLastDateOfreturn(createLTC.EmployeeId);
                    if (createLTC.DateAvailLTC < dateofreturn.AddYears(3))
                    {
                        TempData["Error"] = "This employee cann't take LTC before " + dateofreturn.AddYears(3).Date.ToString("dd-MM-yyyy");
                        createLTC = (Model.LTC)TempData["LTCView"];
                        TempData.Keep("LTCView");
                        return View(createLTC);
                    }
                }
                if (createLTC.approvalSetting.ReportingTo == 0)
                {
                    TempData["Error"] = "You can not apply for LTC right now because either your Reporting or Reviewing Manager is not set.";
                    createLTC = (Model.LTC)TempData["LTCView"];
                    TempData.Keep("LTCView");
                    return View(createLTC);
                }
                //double ElClBalance = 0;
                //TimeSpan t = createLTC.DateofReturn.Subtract(createLTC.DateAvailLTC);
                //if(createLTC.Natureofleave==4)
                //{
                //    var ElClBal = LTCService.GetELLeaveBal(createLTC.EmployeeId, createLTC.DateAvailLTC);
                //    ElClBalance = ElClBal;
                //    if (t.TotalDays > ElClBal)
                //    {

                //        TempData["Error"] = "You Don't have enough  Balance with you, So you can Avail LTC by other leave if available.";
                //        return View(createLTC);
                //    }
                //}
                //if (createLTC.Natureofleave == 2)
                //{
                //    var ElClBal = LTCService.GetCLLeaveBal(createLTC.EmployeeId, createLTC.DateAvailLTC);
                //    ElClBalance = ElClBal;
                //    if (t.TotalDays > ElClBal)
                //    {
                //        TempData["Error"] = "You Don't have enough  Balance with you, So you can Avail LTC by other leave if available.";
                //        return View(createLTC);
                //    }
                //}
                //double totaldays = t.TotalDays;

                if (ModelState.IsValid)
                {
                    var formNewAttributes = GetFormAttributes(createLTC, ButtonType);
                    createLTC.FormStatus = Convert.ToInt16(formNewAttributes.FormState);

                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                    {
                        SenderID = createLTC.loggedInEmpID,
                        ReceiverID = formNewAttributes.ReciverID,
                        SenderDepartmentID = userDetail.DepartmentID,
                        SenderDesignationID = userDetail.DesignationID,
                        CreatedBy = userDetail.UserID,
                        EmployeeID = createLTC.EmployeeId,
                        Scomments = $"LTC Form Updated By : {(Model.SubmittedBy)(int)formNewAttributes.SubmittedBy}",
                        ProcessID = (int)WorkFlowProcess.LTC,
                        StatusID = (int)createLTC.FormStatus
                    };

                    if (createLTC.LTCNo == 0)
                    {
                        var LTCNo = LTCService.GetLastLTCNo(createLTC.EmployeeId, false);
                        createLTC.LTCNo = LTCNo + 1;
                        createLTC.EmployeeId = createLTC.EmployeeId;
                        createLTC.CreatedOn = DateTime.Now;
                        createLTC.CreatedBy = userDetail.UserID;
                        createLTC.DateofApplication = DateTime.Now;



                        int LTCID = LTCService.InsertLTC(createLTC, workFlow);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        createLTC.UpdatedBy = userDetail.UserID;
                        createLTC.UpdatedOn = DateTime.Now;
                        int LTCID = LTCService.UpdateLTC(createLTC, workFlow);
                        if (workFlow.StatusID > 2)
                        {
                            TempData["Message"] = "Succesfully Updated";
                            return RedirectToAction("Index", "ApprovalRequest");
                        }
                        else
                        {
                            TempData["Message"] = "Succesfully Updated";
                            return RedirectToAction("Index");
                        }
                    }
                    //if (LTCID > 0)
                    //{
                    //    double bal;
                    //    int leaveID = LTCService.InsertEmployeeLeave(createLTC, totaldays);
                    //    bool isSaveLeaveBal = LTCService.InsertLeaveTrans(createLTC, totaldays, leaveID);
                    //    TempData["Message"] = "Successfully Created";
                    //    return RedirectToAction("Index");
                    //}
                    //}
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createLTC);
        }
        
        public void BindDropdowns()
        {
            //var ddlBranchList = ddlService.ddlBranchList(userDetail.BranchID, userDetail.UserTypeID);
            //var branch = ddlService.ddlBranchList();
            //Model.SelectListModel selectBranch = new Model.SelectListModel();
            //selectBranch.id = 0;
            //selectBranch.value = "Select";
            //ddlBranchList.Insert(0, selectBranch);
            //ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            //var ddlEmployeeList = LTCService.GetAllEmployee();
            //ddlEmployeeList.OrderBy(x => x.value);
            //Model.SelectListModel employee = new Model.SelectListModel();
            //employee.id = 0;
            //employee.value = "Select";
            //ddlEmployeeList.Insert(0, employee);
            //ViewBag.EmployeeDetails = new SelectList(ddlEmployeeList, "id", "value");


            List<Model.SelectListModel> selectLeave = new List<Model.SelectListModel>();
            selectLeave.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            selectLeave.Add(new Model.SelectListModel() { value = "CL", id = 2 });
            selectLeave.Add(new Model.SelectListModel() { value = "EL", id = 4 });
            ViewBag.LeaveNature = new SelectList(selectLeave, "id", "value");

            var ddldependentList = LTCService.GetAlldependent(userDetail.EmployeeID);
            ddldependentList.OrderBy(x => x.value);
            Model.SelectListModel Dependent = new Model.SelectListModel();
            Dependent.id = 0;
            Dependent.value = "Select";
            ddldependentList.Insert(0, Dependent);
            ViewBag.DependentDetails = new SelectList(ddldependentList, "id", "value");

            List<Model.SelectListModel> selectHome = new List<Model.SelectListModel>();
            selectHome.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            selectHome.Add(new Model.SelectListModel() { value = "Home", id = 1 });
            selectHome.Add(new Model.SelectListModel() { value = "Anywhere In India", id = 2 });
            ViewBag.HomeDetails = new SelectList(selectHome, "id", "value");



        }

        public ActionResult BindDependentlist(int EmployeeID)
        {
            log.Info($"LTCController/BindDependentlist/EmployeeID");
            try
            {
                var Dependentlist = LTCService.GetDependentlist(EmployeeID);
                return Json(new { Dependentlist = Dependentlist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int LTCID, int? reportingTo, int? reviewingTo, int? acceptanceAuthority, int empID)
        {
            log.Info("LTCController/Edit");
            try
            {
                BindDropdowns();
                Model.LTC objLTC = new Model.LTC();
                objLTC = LTCService.GetLTCByID(LTCID, empID);

                objLTC.approvalSetting.ReportingTo = Convert.ToInt32(reportingTo);
                objLTC.approvalSetting.ReviewingTo = Convert.ToInt32(reviewingTo);
                objLTC.approvalSetting.AcceptanceAuthority = Convert.ToInt32(acceptanceAuthority);
                objLTC.loggedInEmpID = (int)userDetail.EmployeeID;

                if ((objLTC.approvalSetting.ReportingTo == objLTC.approvalSetting.ReviewingTo) && (objLTC.approvalSetting.ReviewingTo == objLTC.approvalSetting.AcceptanceAuthority))
                    objLTC.ApprovalHierarchy = 3;
                else if (((objLTC.approvalSetting.ReportingTo != objLTC.approvalSetting.ReviewingTo) && (objLTC.approvalSetting.ReviewingTo == objLTC.approvalSetting.AcceptanceAuthority))
                    && objLTC.loggedInEmpID == objLTC.approvalSetting.ReviewingTo)
                    objLTC.ApprovalHierarchy = 2.1;
                else if (((objLTC.approvalSetting.ReportingTo == objLTC.approvalSetting.ReviewingTo) && (objLTC.approvalSetting.ReviewingTo != objLTC.approvalSetting.AcceptanceAuthority))
                    && (objLTC.loggedInEmpID == objLTC.approvalSetting.ReportingTo || objLTC.loggedInEmpID == objLTC.approvalSetting.AcceptanceAuthority))
                    objLTC.ApprovalHierarchy = 2.0;
                else
                    objLTC.ApprovalHierarchy = 1;

                var ddldependentList = LTCService.GetAlldependent(objLTC.EmployeeId);
                ddldependentList.OrderBy(x => x.value);
                Model.SelectListModel Dependent = new Model.SelectListModel();
                Dependent.id = 0;
                Dependent.value = "Select";
                ddldependentList.Insert(0, Dependent);
                ViewBag.DependentDetails = new SelectList(ddldependentList, "id", "value");

                if (objLTC.Dependents != null && objLTC.Dependents != string.Empty)
                {
                    objLTC.Dependentid = Array.ConvertAll(objLTC.Dependents.Split(','), int.Parse);
                }
                objLTC.ReportingYear = DateTime.Now.GetFinancialYr();
                var dpName = string.Empty;

                for (int i = 1; i < ddldependentList.Count; i++)
                {
                    dpName = dpName + ddldependentList[i].value + ",";
                }
                dpName = dpName.Substring(0, dpName.Length - 1);
                objLTC.DependentName = dpName;
                return View("Create", objLTC);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.LTC createLTC)
        {
            log.Info("LTCController/Edit");
            try
            {
                BindDropdowns();
                createLTC.Dependents = "";
                for (int i = 0; i < createLTC.Dependentid.Length; i++)
                {
                    createLTC.Dependents += createLTC.Dependentid[i] + ",";
                }

                if (createLTC.Dependents != string.Empty)
                {
                    createLTC.Dependents = createLTC.Dependents.Substring(0, createLTC.Dependents.Length - 1);
                }
                if (ModelState.IsValid)
                {
                    createLTC.UpdatedBy = userDetail.UserID;
                    createLTC.UpdatedOn = DateTime.Now;
                    //   int LTCID = LTCService.UpdateLTC(createLTC);
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(createLTC);
        }
        public JsonResult GetBranchEmployee(int branchID)
        {
            try
            {
                var employeedetails = ddlService.employeeByBranchID(branchID, userDetail.EmployeeID, userDetail.UserTypeID);
                Model.SelectListModel selectemployeeDetails = new Model.SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "Select";
                employeedetails.Insert(0, selectemployeeDetails);
                var emp = new SelectList(employeedetails, "id", "value");
                return Json(new { employees = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        public ActionResult Delete(int LTCID)
        {
            log.Info("LTCController/Delete");
            try
            {
                LTCService.DeleteLTC(LTCID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

        [NonAction]
        private Model.FormRulesAttributes GetFormAttributes(Model.LTC createLTC, string buttonType)
        {
            log.Info($"LTCController/GetFormAttributes");
            try
            {
                Model.FormRulesAttributes formRules = new Model.FormRulesAttributes();

                if (createLTC.ApprovalHierarchy == 1) // if reporting+reviewer+acceptance all are different
                {
                    if (createLTC.loggedInEmpID == createLTC.approvalSetting.ReportingTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByReporting : (int)Model.AppraisalFormState.SubmitedByReporting);
                        formRules.SubmittedBy = Model.SubmittedBy.ReportingOfficer;
                        formRules.SenderID = createLTC.approvalSetting.ReportingTo;
                        formRules.ReciverID = createLTC.approvalSetting.ReviewingTo;
                    }
                    else if (createLTC.loggedInEmpID == createLTC.approvalSetting.ReviewingTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                        formRules.SubmittedBy = Model.SubmittedBy.ReviewingOfficer;
                        formRules.SenderID = createLTC.approvalSetting.ReviewingTo;
                        formRules.ReciverID = createLTC.approvalSetting.AcceptanceAuthority;
                    }
                    else if (createLTC.loggedInEmpID == createLTC.approvalSetting.AcceptanceAuthority)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = createLTC.approvalSetting.AcceptanceAuthority;
                        formRules.ReciverID = createLTC.approvalSetting.AcceptanceAuthority;
                    }
                    else
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByEmployee : (int)Model.AppraisalFormState.SubmitedByEmployee);
                        formRules.SubmittedBy = Model.SubmittedBy.Employee;
                        formRules.SenderID = createLTC.approvalSetting.EmployeeID;
                        formRules.ReciverID = createLTC.approvalSetting.ReportingTo;
                    }

                }
                else if (createLTC.ApprovalHierarchy == 2)
                {
                    if (createLTC.loggedInEmpID == createLTC.approvalSetting.ReviewingTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                        formRules.SubmittedBy = Model.SubmittedBy.ReviewingOfficer;
                        formRules.SenderID = createLTC.approvalSetting.ReviewingTo;
                        formRules.ReciverID = createLTC.approvalSetting.AcceptanceAuthority;
                    }
                    //else if (createLTC.loggedInEmpID == createLTC.approvalSetting.ReviewingTo)
                    //{
                    //    formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                    //    formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                    //    formRules.SenderID = createLTC.approvalSetting.AcceptanceAuthority;
                    //    formRules.ReciverID = createLTC.approvalSetting.AcceptanceAuthority;
                    //}
                    else if (createLTC.loggedInEmpID == createLTC.approvalSetting.AcceptanceAuthority)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = createLTC.approvalSetting.AcceptanceAuthority;
                        formRules.ReciverID = createLTC.approvalSetting.AcceptanceAuthority;
                    }
                }
                else if (createLTC.ApprovalHierarchy == 2.1 || createLTC.ApprovalHierarchy == 3)
                {
                    if (createLTC.loggedInEmpID == createLTC.approvalSetting.AcceptanceAuthority)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = createLTC.approvalSetting.AcceptanceAuthority;
                        formRules.ReciverID = createLTC.approvalSetting.AcceptanceAuthority;
                    }
                }
                else
                    formRules.FormState = 1;
                return formRules;
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ex.Message} StackTrace-{ex.StackTrace} DatetimeStamp-{DateTime.Now}");
                throw ex;
            }
        }

        #region LTC STATUS
        [HttpGet]
        public ActionResult LTCStatus()
        {
            log.Info($"LTCController/LTCStatus");
            return View();
        }

        [HttpGet]
        public ActionResult LTCStatusGridView(FormCollection formCollection)
        {
            log.Info($"LTCController/LTCStatusGridView");
            try
            {
                LTCViewModel LTCVM = new LTCViewModel();
                int? empID = null;
                //if (userDetail.UserName == "Admin")
                //    empID = null;
                //else empID = (int)userDetail.EmployeeID;
                LTCVM.LTCList = LTCService.GetLTCList(empID);
                LTCVM.userRights = userAccessRight;
                var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.LTC);
                LTCVM.approvalSetting = approvalSettings ?? new Nafed.MicroPay.Model.EmployeeProcessApproval();
                return PartialView("_LTCStatusGridView", LTCVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult View(int LTCID, int? reportingTo, int? reviewingTo, int? acceptanceAuthority, int empID)
        {
            log.Info($"LTCController/View");
            try
            {
                List<Model.LTCDependentList> dependentList = new List<Model.LTCDependentList>();
                var ltcDetails = LTCService.GetLTCReportByID(LTCID, empID);
                if (reportingTo != null && reportingTo != 0)
                {
                    var reportingDetails = confirmationService.GetRTRVDetails(reportingTo);
                    ltcDetails.ReportingTo = reportingDetails.Name;
                    ltcDetails.ReportingDesignation = reportingDetails.Designation;
                    ltcDetails.ReportingDepartment = reportingDetails.Department;
                }
                else
                {
                    ltcDetails.ValidationMessage = "You can not apply for ltc report right now because either your Reporting or Reviewing Manager is not set.";
                    ltcDetails.Validate = false;
                }

                if (reviewingTo != null && reviewingTo != 0)
                {
                    var reviewingDetails = confirmationService.GetRTRVDetails(reviewingTo);
                    ltcDetails.ReviewerTo = reviewingDetails.Name;
                    ltcDetails.ReviewerDesignation = reviewingDetails.Designation;
                    ltcDetails.ReviewerDepartment = reviewingDetails.Department;
                }
                if (acceptanceAuthority != null && acceptanceAuthority != 0)
                {
                    var acceptanceDetails = confirmationService.GetRTRVDetails(acceptanceAuthority);
                    ltcDetails.AcceptanceAuth = acceptanceDetails.Name;
                    ltcDetails.AcceptanceDesignation = acceptanceDetails.Designation;
                    ltcDetails.AcceptanceDepartment = acceptanceDetails.Department;
                }

                var dates = Enumerable.Range(0, 1 + ltcDetails.ReturnDate.Value.Subtract(ltcDetails.StartDate.Value).Days)
                            .Select(offset => ltcDetails.StartDate.Value.Date.AddDays(offset)).ToList();

                var holidayList = LTCService.GetHolidayList(ltcDetails.StartDate, ltcDetails.ReturnDate).Select(x => x.HolidayDate.Value.Date).ToList();

                var actualLeave = dates.Where(x => !holidayList.Contains(x)).ToList();
                var holiday = holidayList.Where(x => dates.Contains(x)).ToList();
                ltcDetails.TotalLeaveDays = actualLeave.Count;

                var leaveDetails = Array.ConvertAll(actualLeave.ToArray(), s => Convert.ToString(s.ToString("ddd, dd MMM yyyy"))).ToList();
                var holidayDetails = Array.ConvertAll(holiday.ToArray(), s => Convert.ToString(s.ToString("ddd, dd MMM yyyy"))).ToList();

                ltcDetails.AppliedLeave = String.Join(" to ", leaveDetails);
                ltcDetails.HolidayLeave = String.Join(" to ", holidayDetails);

                var ltcDependent = ltcDetails.Dependent.Split(',').ToArray<string>();
                int[] myInts = Array.ConvertAll(ltcDependent, s => int.Parse(s));

                var ddldependentList = LTCService.GetAlldependent(empID);

                dependentList.Add(new Model.LTCDependentList { DependentID = 0, DependentName = ltcDetails.Employeename, RelationName = "Self", Age = Nafed.MicroPay.Common.ExtensionMethods.CalculateAge(ltcDetails.EmployeeDOB) + " Yr(s)" });

                foreach (var item in ddldependentList.Where(x => myInts.Contains(x.id)))
                {
                    Model.LTCDependentList dependent = new Model.LTCDependentList
                    {
                        DependentID = item.id,
                        DependentName = item.value.Split('-')[0],
                        RelationName = item.value.Split('-')[1],
                        Age = item.value.Split('-')[2]
                    };
                    dependentList.Add(dependent);
                }
                ltcDetails.listDependentList = dependentList;
                return View(ltcDetails);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _GETLTCReferenceNumber(int LTCID, int? reportingTo, int? reviewingTo, int? acceptanceAuthority, int empID)
        {
            log.Info($"LTCController/_GETLTCReferenceNumber");
            try
            {
                Model.LTCReport LTCReport = new Model.LTCReport();
                LTCReport.LTCID = LTCID;
                LTCReport.ReportingToID = reportingTo;
                LTCReport.ReviewerToID = reviewingTo;
                LTCReport.AcceptanceAuthID = acceptanceAuthority;
                LTCReport.EmployeeId = empID;
                return PartialView("_LTCReferenceNumber", LTCReport);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult PostLTCReferenceNumber(Model.LTCReport LTCReport)
        {
            log.Info($"LTCController/PostLTCReferenceNumber");
            try
            {
                if (ModelState.IsValid)
                {
                    var result = LTCService.PostLTCReferenceNumber(LTCReport);
                    if (result)
                    {
                        LTCViewModel LTCVM = new LTCViewModel();
                        int? empID = null;
                        LTCVM.LTCList = LTCService.GetLTCList(empID);
                        LTCVM.userRights = userAccessRight;
                        var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.LTC);
                        LTCVM.approvalSetting = approvalSettings ?? new Nafed.MicroPay.Model.EmployeeProcessApproval();
                        return Json(new { msgType = "success", htmlData = ConvertViewToString("_LTCStatusGridView", LTCVM), LTCID = LTCReport.LTCID, reportingTo = LTCReport.ReportingToID, reviewingTo = LTCReport.ReviewerToID, acceptanceAuthority = LTCReport.AcceptanceAuthID, empID = LTCReport.EmployeeId }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { msgType = "error", msg = "Something is wrong remark not saved", htmlData = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { msgType = "required", htmlData = ConvertViewToString("_LTCReferenceNumber", LTCReport) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion LTC STATUS
    }
}