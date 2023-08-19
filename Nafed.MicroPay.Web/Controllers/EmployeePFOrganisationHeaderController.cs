using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using System.Data;
using System.IO;
using static Nafed.MicroPay.Common.FileHelper;
using Common = Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers
{
    public class EmployeePFOrganisationHeaderController : BaseController
    {
        private readonly IEmployeePFOrganisationService EmployeePFOrganisationService;
        private readonly IDropdownBindService ddlService;

        public EmployeePFOrganisationHeaderController(IEmployeePFOrganisationService EmployeePFOrganisationService, IDropdownBindService ddlService)
        {
            this.EmployeePFOrganisationService = EmployeePFOrganisationService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            log.Info($"EmployeePFOrganisationHeaderController/Index");
            EmpPFOrgViewModel EMPPFVM = new EmpPFOrgViewModel();
            EMPPFVM.userRights = userAccessRight;
            return View(EMPPFVM);
        }
        public void BindDropdowns()
        {

            List<Model.SelectListModel> select = new List<Model.SelectListModel>();
            select.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            select.Add(new Model.SelectListModel() { value = "Yes", id = 1 });
            select.Add(new Model.SelectListModel() { value = "No", id = 2 });
            ViewBag.select = new SelectList(select, "id", "value");
        }

        [HttpGet]
        public ActionResult _GetEmpPFOrganisationGridView(EmpPFOrgViewModel EMPPFVM)
        {
            try
            {

                EmpPFOrgViewModel EMPPFHVM = new EmpPFOrgViewModel();
                int? empID = null;
                if (userDetail.UserName == "Admin")
                    empID = null;
                else empID = (int)userDetail.EmployeeID;
                var PR = EmployeePFOrganisationService.GetEmpPFHList((int)userDetail.EmployeeID);
                EMPPFHVM.EmpPFOrgList = PR;
                EMPPFHVM.userRights = userAccessRight;
                var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, Common.WorkFlowProcess.Form11);
                EMPPFHVM.approvalSetting = approvalSettings ?? new Nafed.MicroPay.Model.EmployeeProcessApproval();
                return PartialView("_EmpPFOrganisationGridView", EMPPFHVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Create(int EmpPFID, int statusID, int? reportingTo, int? reviewingTo, int? acceptanceAuthority, int? empID)
        {
            log.Info("EmployeePFOrganisationHeaderController/Edit");
            try
            {
                BindDropdowns();               
                Model.EmployeePFORG EMPPFHVM = new Model.EmployeePFORG();

                var getexgratiaList = EmployeePFOrganisationService.checkexistdata(EmpPFID);
                DataTable dt = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(getexgratiaList);

                if (dt.Rows.Count > 0)
                {


                    EMPPFHVM = EmployeePFOrganisationService.GetEMPPFOrgDetails(Convert.ToInt32(dt.Rows[0]["ID"]), EmpPFID, statusID);
                    EMPPFHVM.EmpProceeApproval.ReportingTo = (reportingTo == null ? 0 : (int)reportingTo);
                    EMPPFHVM.EmpProceeApproval.ReviewingTo = (reviewingTo == null ? 0 : (int)reviewingTo);
                    EMPPFHVM.EmpProceeApproval.AcceptanceAuthority = (acceptanceAuthority == null ? 0 : (int)acceptanceAuthority);
                    EMPPFHVM.EmployeeId = (empID == null ? 0 : (int)empID);
                    EMPPFHVM.loggedInEmpID = (int)userDetail.EmployeeID;
                    EMPPFHVM.FormStatus = (short)statusID;

                }
                else
                {
                    EMPPFHVM = EmployeePFOrganisationService.GetEMPPFOrgDetails(0, EmpPFID, statusID);
                    EMPPFHVM.FormStatus = 1;
                    var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, Common.WorkFlowProcess.Form11);
                    EMPPFHVM.EmpProceeApproval = approvalSettings ?? new Nafed.MicroPay.Model.EmployeeProcessApproval();
                    EMPPFHVM.loggedInEmpID = (int)userDetail.EmployeeID;
                }

                #region KYC Details
                if (string.IsNullOrEmpty(EMPPFHVM.AadhaarCardFilePath))
                {

                }
                else
                {
                    EMPPFHVM.AadhaarCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.AadhaarCardFilePath + "/" + EMPPFHVM.AadhaarCardFilePath);
                    if (!System.IO.File.Exists(EMPPFHVM.AadhaarCardUNCFilePath))
                    {
                        EMPPFHVM.AadhaarCardUNCFilePath = Path.Combine(Common.DocumentUploadFilePath.AadhaarCardFilePath + "/AadhaarNo.jpg");

                    }
                    else
                    {
                        EMPPFHVM.AadhaarCardUNCFilePath = Path.Combine(Common.DocumentUploadFilePath.AadhaarCardFilePath + "/" + EMPPFHVM.AadhaarCardFilePath);
                    }
                }
                if (string.IsNullOrEmpty(EMPPFHVM.PanCardFilePath))
                {

                }
                else
                {
                    EMPPFHVM.PanCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.PanCardFilePath + "/" + EMPPFHVM.PanCardFilePath);
                    if (!System.IO.File.Exists(EMPPFHVM.PanCardUNCFilePath))
                    {
                        EMPPFHVM.PanCardUNCFilePath = Path.Combine(Common.DocumentUploadFilePath.PanCardFilePath + "/SamplePAN_Card.jpg");
                    }
                    else
                    {
                        EMPPFHVM.PanCardUNCFilePath = Path.Combine(Common.DocumentUploadFilePath.PanCardFilePath + "/" + EMPPFHVM.PanCardFilePath);                       
                    }
                }

                if (string.IsNullOrEmpty(EMPPFHVM.PassportFilePath))
                {

                }
                else
                {
                    EMPPFHVM.PassportUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.PassportFilePath + "/" + EMPPFHVM.PassportFilePath);
                    if (!System.IO.File.Exists(EMPPFHVM.PassportUNCFilePath))
                    {

                    }
                }
                if (string.IsNullOrEmpty(EMPPFHVM.BankAccFilePath))
                {

                }
                
                else
                {
                    EMPPFHVM.BankAccUNCFilePath = Path.Combine(Common.DocumentUploadFilePath.BankAccFilePath + "/" + EMPPFHVM.BankAccFilePath);
                    if (!System.IO.File.Exists(EMPPFHVM.BankAccUNCFilePath))
                    {

                    }
                }
                #endregion


                if ((EMPPFHVM.EmpProceeApproval.ReportingTo == EMPPFHVM.EmpProceeApproval.ReviewingTo) && (EMPPFHVM.EmpProceeApproval.ReviewingTo == EMPPFHVM.EmpProceeApproval.AcceptanceAuthority))
                    EMPPFHVM.ApprovalHierarchy = 3;
                else if (((EMPPFHVM.EmpProceeApproval.ReportingTo != EMPPFHVM.EmpProceeApproval.ReviewingTo) && (EMPPFHVM.EmpProceeApproval.ReviewingTo == EMPPFHVM.EmpProceeApproval.AcceptanceAuthority))
                    && EMPPFHVM.loggedInEmpID == EMPPFHVM.EmpProceeApproval.ReviewingTo)
                    EMPPFHVM.ApprovalHierarchy = 2.1;
                else if (((EMPPFHVM.EmpProceeApproval.ReportingTo == EMPPFHVM.EmpProceeApproval.ReviewingTo) && (EMPPFHVM.EmpProceeApproval.ReviewingTo != EMPPFHVM.EmpProceeApproval.AcceptanceAuthority))
                    && (EMPPFHVM.loggedInEmpID == EMPPFHVM.EmpProceeApproval.ReportingTo || EMPPFHVM.loggedInEmpID == EMPPFHVM.EmpProceeApproval.AcceptanceAuthority))
                    EMPPFHVM.ApprovalHierarchy = 2.0;
                else
                    EMPPFHVM.ApprovalHierarchy = 1;

                return View(EMPPFHVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult Create(Model.EmployeePFORG createEmpPF, string ButtonType, IEnumerable<HttpPostedFileBase> files)
        {
            log.Info("EmployeePFOrganisationHeaderController/Create");
            try
            {
                var stID = createEmpPF.StatusID;
                BindDropdowns();
                ModelState.Remove("Dateof_Exit_Previos_Employment");
                ModelState.Remove("Previous_PF_Acc_No");
                ModelState.Remove("Employee_PF_Scheme_1952");
                string panCardFilePath = string.Empty, aadharCardFilePath = string.Empty, passportFilePath = string.Empty, bankDetailsFilePath = string.Empty;

                var aadhaarCardFile = files == null ? null : Request.Files[2];
                var passportFile = files == null ? null : Request.Files[0];
                var panCardFile = files == null ? null : Request.Files[3];
                var bankDetailsFile = files == null ? null : Request.Files[1];
                if (createEmpPF.EmpProceeApproval.ReportingTo == 0)
                {
                    TempData["Error"] = "You can not apply for appraisal right now because either your Reporting or Reviewing Manager is not set.";
                    return View(createEmpPF);
                }

                var getexgratiaList = EmployeePFOrganisationService.checkexistdata(createEmpPF.EmpPFID);
                DataTable dt = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(getexgratiaList);
                if (dt.Rows.Count > 0)
                {

                }
                else
                {
                    if (bankDetailsFile.FileName == "" || aadhaarCardFile.FileName == "" || panCardFile.FileName == "")
                    {

                        TempData["Error"] = "Please Upload KYC Details.";
                        return View("Create", createEmpPF);
                    }

                    #region Bank details
                    if (bankDetailsFile.FileName != "")
                    {
                        string fileExtension = Path.GetExtension(bankDetailsFile.FileName);
                        var contentType = GetFileContentType(bankDetailsFile.InputStream);
                        var dicValue = GetDictionaryValueByKeyName(".jpg");

                        if (!((dicValue == contentType) || (fileExtension == ".jpg" || fileExtension == ".jpeg")))
                        {
                            TempData["Error"] = "Please select valid image file.";
                            return View(createEmpPF);
                        }
                        else
                        {
                            bankDetailsFilePath = Common.ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(bankDetailsFile.FileName),
                            Path.GetExtension(bankDetailsFile.FileName));
                            createEmpPF.BankAccFilePath = Common.DocumentUploadFilePath.BankAccFilePath + "/" + bankDetailsFilePath;
                        }
                    }

                    #endregion

                    #region AADHAR No
                    if (aadhaarCardFile.FileName != "")
                    {
                        string fileExtension = Path.GetExtension(aadhaarCardFile.FileName);
                        var contentType = GetFileContentType(aadhaarCardFile.InputStream);
                        var dicValue = GetDictionaryValueByKeyName(".jpg");

                        if (!((dicValue == contentType) || (fileExtension == ".jpg" || fileExtension == ".jpeg")))
                        {
                            TempData["Error"] = "Please select valid image file.";
                            return View(createEmpPF);
                        }
                        else
                        {
                            aadharCardFilePath = Common.ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(aadhaarCardFile.FileName),
                            Path.GetExtension(aadhaarCardFile.FileName));
                            createEmpPF.AadhaarCardFilePath = Common.DocumentUploadFilePath.AadhaarCardFilePath + "/" + aadharCardFilePath;
                        }
                    }

                    #endregion

                    #region PAN No
                    if (panCardFile.FileName != "")
                    {
                        string fileExtension = Path.GetExtension(panCardFile.FileName);
                        var contentType = GetFileContentType(panCardFile.InputStream);
                        var dicValue = GetDictionaryValueByKeyName(".jpg");

                        if (((fileExtension != ".jpg" && fileExtension != ".jpeg")))
                        {
                            TempData["Error"] = "Please select valid image file.";
                            return View(createEmpPF);
                        }
                        else
                        {
                            panCardFilePath = Common.ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(panCardFile.FileName),
                             Path.GetExtension(panCardFile.FileName));
                            createEmpPF.PanCardFilePath = Common.DocumentUploadFilePath.PanCardFilePath + "/" + panCardFilePath;
                        }
                    }

                    #endregion


                }

                if (passportFile.FileName != "")
                {
                    string fileExtension = Path.GetExtension(passportFile.FileName);
                    var contentType = GetFileContentType(passportFile.InputStream);
                    var dicValue = GetDictionaryValueByKeyName(".jpg");

                    if (((fileExtension != ".jpg" && fileExtension != ".jpeg")))
                    {
                        TempData["Error"] = "Please select valid image file.";
                        return View(createEmpPF);
                    }
                    else
                    {
                        passportFilePath = Common.ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(passportFile.FileName),
                         Path.GetExtension(passportFile.FileName));
                        createEmpPF.PassportFilePath = Common.DocumentUploadFilePath.PassportFilePath + "/" + passportFilePath;
                    }
                }

                if (ModelState.IsValid)
                {
                    createEmpPF.loggedInEmpID = userDetail.EmployeeID;
                    createEmpPF.EmployeeId = userDetail.EmployeeID;
                    var formNewAttributes = GetFormAttributes(createEmpPF, ButtonType);
                    createEmpPF.FormStatus = Convert.ToInt16(formNewAttributes.FormState);

                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                    {
                        SenderID = createEmpPF.loggedInEmpID,
                        ReceiverID = formNewAttributes.ReciverID,
                        SenderDepartmentID = userDetail.DepartmentID,
                        SenderDesignationID = userDetail.DesignationID,
                        CreatedBy = userDetail.UserID,
                        EmployeeID = (int)userDetail.EmployeeID,
                        Scomments = $"Form 11 Updated By : {(Model.SubmittedBy)(int)formNewAttributes.SubmittedBy}",
                        ProcessID = (int)Common.WorkFlowProcess.Form11,
                        StatusID = (int)createEmpPF.FormStatus
                    };

                    if (ButtonType == "Save")
                    {
                        if (bankDetailsFile.FileName != "")
                            SaveFile(bankDetailsFile, Nafed.MicroPay.Common.DocumentType.BankDetails, bankDetailsFilePath);

                        if (panCardFile.FileName != "")
                            SaveFile(panCardFile, Nafed.MicroPay.Common.DocumentType.PanCard, panCardFilePath);

                        if (aadhaarCardFile.FileName != "")
                            SaveFile(aadhaarCardFile, Nafed.MicroPay.Common.DocumentType.AadhaarCard, aadharCardFilePath);

                        if (passportFile.FileName != "")
                            SaveFile(passportFile, Nafed.MicroPay.Common.DocumentType.PassportNo, passportFilePath);

                        if (dt.Rows.Count > 0)
                        {
                            createEmpPF.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
                            createEmpPF.EmployeeId = userDetail.EmployeeID;
                            createEmpPF.UpdatedBy = userDetail.UserID;
                            createEmpPF.UpdatedOn = DateTime.Now;

                            EmployeePFOrganisationService.UpdateEmployeePFDetails(createEmpPF);
                        }
                        else
                        {

                            createEmpPF.CreatedBy = userDetail.UserID;
                            createEmpPF.CreatedOn = DateTime.Now;
                            createEmpPF.BankAccFilePath = bankDetailsFilePath;
                            createEmpPF.AadhaarCardFilePath = aadharCardFilePath;
                            createEmpPF.PanCardFilePath = panCardFilePath;
                            createEmpPF.StatusID = 1;
                            EmployeePFOrganisationService.InsertEmployeePFDetails(createEmpPF, workFlow);
                            createEmpPF.EmployeeId = userDetail.EmployeeID;
                            createEmpPF.UpdatedBy = userDetail.UserID;
                            createEmpPF.UpdatedOn = DateTime.Now;
                            EmployeePFOrganisationService.UpdateEmployeePFStatus(createEmpPF, workFlow);
                        }
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                    else if (ButtonType == "Submit")
                    {
                        createEmpPF.FormStatus = Convert.ToInt16(formNewAttributes.FormState);
                        createEmpPF.StatusID = Convert.ToInt16(formNewAttributes.FormState);
                        createEmpPF.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
                        createEmpPF.EmployeeId = Convert.ToInt32(dt.Rows[0]["EmployeeId"]);
                        createEmpPF.UpdatedBy = userDetail.UserID;
                        createEmpPF.UpdatedOn = DateTime.Now;
                        if (createEmpPF.PFNo != 0)
                        {
                            EmployeePFOrganisationService.UpdateEmployeePFDetails(createEmpPF);
                        }

                        bool result = EmployeePFOrganisationService.UpdateEmployeePFStatus(createEmpPF, workFlow);

                        if (createEmpPF.StatusID == 5)
                        {
                            bool sendmail = EmployeePFOrganisationService.SendMail(createEmpPF);
                        }
                        if (result == true)
                        {
                            return RedirectToAction("Index", "ApprovalRequest");
                            //return Json(new { msgType = "success", msg = "Form submitted successfully." }, JsonRequestBehavior.AllowGet);
                        }

                    }

                    else if (ButtonType == "Rejected")
                    {

                        createEmpPF.FormStatus = Convert.ToInt16(formNewAttributes.FormState);
                        createEmpPF.StatusID = Convert.ToInt16(formNewAttributes.FormState);
                        createEmpPF.EmployeeId = userDetail.EmployeeID;
                        createEmpPF.UpdatedBy = userDetail.UserID;
                        createEmpPF.UpdatedOn = DateTime.Now;
                        bool result = EmployeePFOrganisationService.UpdateEmployeePFStatus(createEmpPF, workFlow);
                        if (result == true)
                        {
                            return RedirectToAction("Index", "ApprovalRequest");
                        }

                    }

                }
                else
                {
                    createEmpPF.StatusID = stID;
                    createEmpPF.FormStatus = (short)stID;
                    return View(createEmpPF);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createEmpPF);
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

                    

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        #region Check Mime Type
                        if (!IsValidFileName(file.FileName))
                        {
                            return false;
                        }
                        var contentType = GetFileContentType(file.InputStream);
                        var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                        if (dicValue != contentType)
                        {
                            return false;
                        }
                        #endregion

                        if (docType == Common.DocumentType.PanCard)
                        {
                            fileName = documentFile;
                            filePath = "~/" + Common.DocumentUploadFilePath.PanCardFilePath;
                        }

                        if (docType == Common.DocumentType.AadhaarCard)
                        {
                            fileName = documentFile;
                            filePath = "~/" + Common.DocumentUploadFilePath.AadhaarCardFilePath;
                        }

                        if (docType == Common.DocumentType.PassportNo)
                        {
                            fileName = documentFile;
                            filePath = "~/" + Common.DocumentUploadFilePath.PassportFilePath;
                        }

                        if (docType == Common.DocumentType.BankDetails)
                        {
                            fileName = documentFile;
                            filePath = "~/" + Common.DocumentUploadFilePath.BankAccFilePath;
                        }

                        if (docType == Common.DocumentType.ProfileImage)
                        {

                            fileName = documentFile;
                            filePath = "~/" + Common.DocumentUploadFilePath.ProfilePicFilePath;
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

        [HttpGet]
        public new FileResult GetImage(string imgPath)
        {
            try
            {
                if (imgPath != null)
                {
                    log.Info($"Get Item image path {imgPath}");
                    byte[] imageByteData = ImageService.GetImage(imgPath);
                    return File(imageByteData, "image/jpg;base64");
                }
                else
                {
                    byte[] imageByteData = ImageService.GetImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png"));
                    return File(imageByteData, "image/jpg;base64");
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [NonAction]
        private Model.FormRulesAttributes GetFormAttributes(Model.EmployeePFORG createPFOrg, string buttonType)
        {
            log.Info($"LTCController/GetFormAttributes");
            try
            {
                Model.FormRulesAttributes formRules = new Model.FormRulesAttributes();

                if (createPFOrg.ApprovalHierarchy == 1)
                {
                    if (createPFOrg.loggedInEmpID == createPFOrg.EmpProceeApproval.ReportingTo)
                    {

                        if (buttonType == "Submit")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.AcceptedByReporting : (int)Model.form11FormState.AcceptedByReporting);
                        }
                        else if (buttonType == "Rejected")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.RejectedByReporting : (int)Model.form11FormState.RejectedByReporting);
                        }
                        formRules.SubmittedBy = Model.SubmittedBy.ReportingOfficer;
                        formRules.SenderID = createPFOrg.EmpProceeApproval.ReportingTo;
                        formRules.ReciverID = createPFOrg.EmpProceeApproval.ReviewingTo;
                    }
                    else if (createPFOrg.loggedInEmpID == createPFOrg.EmpProceeApproval.ReviewingTo)
                    {

                        if (buttonType == "Submit")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.AcceptedByReviewer : (int)Model.form11FormState.AcceptedByReviewer);
                        }
                        else if (buttonType == "Rejected")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.RejectedByReviewer : (int)Model.form11FormState.RejectedByReviewer);
                        }

                        formRules.SubmittedBy = Model.SubmittedBy.ReviewingOfficer;
                        formRules.SenderID = createPFOrg.EmpProceeApproval.ReviewingTo;
                        formRules.ReciverID = createPFOrg.EmpProceeApproval.AcceptanceAuthority;
                    }
                    else if (createPFOrg.loggedInEmpID == createPFOrg.EmpProceeApproval.AcceptanceAuthority)
                    {

                        if (buttonType == "Submit")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.SubmitedByAcceptanceAuth : (int)Model.form11FormState.SubmitedByAcceptanceAuth);
                        }
                        else if (buttonType == "Rejected")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.RejectedByAcceptanceAuthority : (int)Model.form11FormState.RejectedByAcceptanceAuthority);
                        }

                        formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = createPFOrg.EmpProceeApproval.AcceptanceAuthority;
                        formRules.ReciverID = createPFOrg.EmpProceeApproval.AcceptanceAuthority;
                    }
                    else
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.SubmitedByEmployee : (int)Model.form11FormState.SubmitedByEmployee);
                        formRules.SubmittedBy = Model.SubmittedBy.Employee;
                        formRules.SenderID = createPFOrg.EmpProceeApproval.EmployeeID;
                        formRules.ReciverID = createPFOrg.EmpProceeApproval.ReportingTo;
                    }

                }
                else if (createPFOrg.ApprovalHierarchy == 2)
                {
                    if (createPFOrg.loggedInEmpID == createPFOrg.EmpProceeApproval.ReviewingTo)
                    {
                        if (buttonType == "Submit")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.AcceptedByReviewer : (int)Model.form11FormState.AcceptedByReviewer);
                        }
                        else if (buttonType == "Rejected")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.RejectedByReviewer : (int)Model.form11FormState.RejectedByReviewer);
                        }

                        formRules.SubmittedBy = Model.SubmittedBy.ReviewingOfficer;
                        formRules.SenderID = createPFOrg.EmpProceeApproval.ReviewingTo;
                        formRules.ReciverID = createPFOrg.EmpProceeApproval.AcceptanceAuthority;
                    }
                    else if (createPFOrg.loggedInEmpID == createPFOrg.EmpProceeApproval.ReviewingTo)
                    {
                        if (buttonType == "Submit")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.SubmitedByAcceptanceAuth : (int)Model.form11FormState.SubmitedByAcceptanceAuth);
                        }
                        else if (buttonType == "Rejected")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.RejectedByAcceptanceAuthority : (int)Model.form11FormState.RejectedByAcceptanceAuthority);
                        }

                        formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = createPFOrg.EmpProceeApproval.AcceptanceAuthority;
                        formRules.ReciverID = createPFOrg.EmpProceeApproval.AcceptanceAuthority;
                    }
                    else if (createPFOrg.loggedInEmpID == createPFOrg.EmpProceeApproval.AcceptanceAuthority)
                    {
                        if (buttonType == "Submit")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.SubmitedByAcceptanceAuth : (int)Model.form11FormState.SubmitedByAcceptanceAuth);
                        }
                        else if (buttonType == "Rejected")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.RejectedByAcceptanceAuthority : (int)Model.form11FormState.RejectedByAcceptanceAuthority);
                        }

                        formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = createPFOrg.EmpProceeApproval.AcceptanceAuthority;
                        formRules.ReciverID = createPFOrg.EmpProceeApproval.AcceptanceAuthority;
                    }
                }
                else if (createPFOrg.ApprovalHierarchy == 2.1 || createPFOrg.ApprovalHierarchy == 3)
                {
                    if (createPFOrg.loggedInEmpID == createPFOrg.EmpProceeApproval.AcceptanceAuthority)
                    {
                        if (buttonType == "Submit")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.SubmitedByAcceptanceAuth : (int)Model.form11FormState.SubmitedByAcceptanceAuth);
                        }
                        else if (buttonType == "Rejected")
                        {
                            formRules.FormState = (buttonType == "Save" ? (int)Model.form11FormState.RejectedByAcceptanceAuthority : (int)Model.form11FormState.RejectedByAcceptanceAuthority);
                        }

                        formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = createPFOrg.EmpProceeApproval.AcceptanceAuthority;
                        formRules.ReciverID = createPFOrg.EmpProceeApproval.AcceptanceAuthority;
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
    }



}