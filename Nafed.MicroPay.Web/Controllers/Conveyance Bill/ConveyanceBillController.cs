using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static Nafed.MicroPay.Common.ExtensionMethods;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Controllers.Conveyance_Bill
{
    public class ConveyanceBillController : BaseController
    {
        // GET: ConveyanceBill
        private readonly IConveyanceBillService conveyanceBillService;
        private readonly IConfirmationFormService confirmationService;

        public ConveyanceBillController(IConveyanceBillService conveyanceBillService, IConfirmationFormService confirmationService)
        {
            this.conveyanceBillService = conveyanceBillService;
            this.confirmationService = confirmationService;
        }

        public ActionResult Index()
        {
            log.Info($"ConveyanceBill/Index");
            try
            {
                ConveyanceBillViewModel conveyanceBillVM = new Models.ConveyanceBillViewModel();
                var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.ConveyanceBill);
                conveyanceBillVM.approvalSetting = approvalSettings ?? new Model.EmployeeProcessApproval();
                return View(conveyanceBillVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult MyConveyanceBillList()
        {
            log.Info($"ConveyanceBillController/MyConveyanceBillList");
            try
            {
                ConveyanceBillViewModel empConveyanceBillVM = new ConveyanceBillViewModel();
                var selfFormList = conveyanceBillService.GetEmployeeSelfConveyanceList((int)userDetail.EmployeeID, null, null);
                empConveyanceBillVM.ConveyanceformList = selfFormList;
                return PartialView("_MyConveyanceBillList", empConveyanceBillVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult Edit(int? empID, int conveyanceDetailID, int? reportingTo, int? reviewingTo, int? acceptanceAuthority)
        {
            log.Info($"ConveyanceBill/Edit");
            try
            {
                Model.ConveyanceBillForm conveyanceBillForm = new Model.ConveyanceBillForm();
                conveyanceBillForm.ReportingTo = reportingTo;
                conveyanceBillForm.ReviewingTo = reviewingTo;
                conveyanceBillForm.EmployeeID = (int)empID;
                conveyanceBillForm.AcceptanceAuthorityTo = acceptanceAuthority;
                conveyanceBillForm.loggedInEmpID = userDetail.EmployeeID;
                conveyanceBillForm.ConveyanceDetailID = conveyanceDetailID;

                if (conveyanceBillForm.ReportingTo != null)
                {
                    var reportingDetails = confirmationService.GetRTRVDetails(conveyanceBillForm.ReportingTo);
                    conveyanceBillForm.ReportingName = reportingDetails.Name;
                    conveyanceBillForm.ReportingDesignation = reportingDetails.Designation;
                }
                if (conveyanceBillForm.ReviewingTo != null)
                {
                    var reviewingDetails = confirmationService.GetRTRVDetails(conveyanceBillForm.ReviewingTo);
                    conveyanceBillForm.ReviewingName = reviewingDetails.Name;
                    conveyanceBillForm.ReviewingDesignation = reviewingDetails.Designation;
                }
                if (conveyanceBillForm.AcceptanceAuthorityTo != null)
                {
                    var reviewingDetails = confirmationService.GetAADetails(conveyanceBillForm.AcceptanceAuthorityTo);
                    conveyanceBillForm.AAName = reviewingDetails.Name;
                    conveyanceBillForm.AADesignation = reviewingDetails.Designation;
                }

                conveyanceBillForm.frmAttributes = conveyanceBillService.GetConveyanceFormRulesAttributes(conveyanceBillForm.EmployeeID, conveyanceDetailID);

                if (conveyanceBillForm.frmAttributes != null)
                {
                    if ((conveyanceBillForm.ReportingTo == conveyanceBillForm.ReviewingTo) && (conveyanceBillForm.ReviewingTo == conveyanceBillForm.AcceptanceAuthorityTo))
                        conveyanceBillForm.ApprovalHierarchy = 3;
                    else if (((conveyanceBillForm.ReportingTo != conveyanceBillForm.ReviewingTo) && (conveyanceBillForm.ReviewingTo == conveyanceBillForm.AcceptanceAuthorityTo)) && conveyanceBillForm.loggedInEmpID == conveyanceBillForm.ReviewingTo)
                        conveyanceBillForm.ApprovalHierarchy = 2.1;
                    else if (((conveyanceBillForm.ReportingTo == conveyanceBillForm.ReviewingTo) && (conveyanceBillForm.ReviewingTo != conveyanceBillForm.AcceptanceAuthorityTo))
                            && (conveyanceBillForm.loggedInEmpID == conveyanceBillForm.ReportingTo || conveyanceBillForm.loggedInEmpID == conveyanceBillForm.AcceptanceAuthorityTo))
                        conveyanceBillForm.ApprovalHierarchy = 2.0;
                    else
                        conveyanceBillForm.ApprovalHierarchy = 1;

                    if (conveyanceBillForm.frmAttributes.FormState == 1)
                    {
                        if (conveyanceBillForm.ApprovalHierarchy == 1 && conveyanceBillForm.loggedInEmpID == conveyanceBillForm.ReportingTo)
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                        else if (conveyanceBillForm.ApprovalHierarchy == 1 && conveyanceBillForm.loggedInEmpID == conveyanceBillForm.ReviewingTo)
                        {
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 1 && conveyanceBillForm.loggedInEmpID == conveyanceBillForm.AcceptanceAuthorityTo)
                        {
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.0 && conveyanceBillForm.loggedInEmpID == reviewingTo)
                        {
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.1 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 3 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                    }
                    else if (conveyanceBillForm.frmAttributes.FormState == 2 || conveyanceBillForm.frmAttributes.FormState == 3)
                    {
                        if (conveyanceBillForm.ApprovalHierarchy == 2.0 && conveyanceBillForm.loggedInEmpID == reviewingTo)
                        {
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.0 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.1 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 3 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                    }
                    else if (conveyanceBillForm.frmAttributes.FormState == 4 || conveyanceBillForm.frmAttributes.FormState == 5 || conveyanceBillForm.frmAttributes.FormState == 6 || conveyanceBillForm.frmAttributes.FormState == 7)
                    {
                        if (conveyanceBillForm.ApprovalHierarchy == 1 && conveyanceBillForm.loggedInEmpID == reviewingTo)
                        {
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.0 && conveyanceBillForm.loggedInEmpID == reviewingTo)
                        {
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.1 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                    }
                }
                return View("ConveyanceFormContainer", conveyanceBillForm);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _GetConveyanceBillForm(Model.ConveyanceBillForm conveyanceBillForm)
        {
            log.Info($"ConveyanceBill/_GetConveyanceBillForm");
            try
            {
                var reportingYr = DateTime.Now.GetFinancialYr();
                conveyanceBillForm.conveyanceBillDetails = conveyanceBillService.GetConveyanceBillDetail(conveyanceBillForm.EmployeeID, conveyanceBillForm.ConveyanceDetailID);
                conveyanceBillForm.EmployeeID = conveyanceBillForm.conveyanceBillDetails.EmployeeID;
                conveyanceBillForm.conveyanceBillDetails.ReportingYear = (conveyanceBillForm.conveyanceBillDetails.ReportingYear != null && conveyanceBillForm.conveyanceBillDetails.ReportingYear.Length > 0) ? conveyanceBillForm.conveyanceBillDetails.ReportingYear : reportingYr;
                conveyanceBillForm.conveyanceBillDescriptionList = conveyanceBillService.GetConveyanceBillDescription(conveyanceBillForm.EmployeeID, conveyanceBillForm.conveyanceBillDetails.ConveyanceBillDetailID);

                if (conveyanceBillForm.conveyanceBillDescriptionList.Count > 0)
                {
                    var sno = 1;
                    conveyanceBillForm.conveyanceBillDescriptionList.ForEach(x =>
                    {
                        x.sno = sno++;
                        x.conveyanceBillDesc = (Model.ConveyanceBillDesc)x.VehicleID;
                    });
                }
                TempData["ConveyanceBill"] = conveyanceBillForm;
                TempData.Keep("ConveyanceBill");
                return PartialView("_FormConveyanceBill", conveyanceBillForm);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostConveyanceBillFormData(Model.ConveyanceBillForm roformData, string ButtonType, FormCollection frm)
        {
            log.Info($"ConveyanceBill/_PostConveyanceBillFormData");
            bool flag;
            try
            {
                if (ButtonType == "Add Row")
                {
                    if (roformData.conveyanceBillDescriptionList == null)
                        roformData.conveyanceBillDescriptionList = new List<Model.ConveyanceBillDescription>() {
                            new Model.ConveyanceBillDescription() { sno = 1, } };
                    else
                    {
                        if (roformData.conveyanceBillDescriptionList.Count == 1)
                            roformData.conveyanceBillDescriptionList.FirstOrDefault().sno = 1;
                        else
                        {
                            var s_no = 1;
                            roformData.conveyanceBillDescriptionList.ForEach(x =>
                            {
                                x.sno = s_no++;
                            });
                        }
                        if (roformData.conveyanceBillDescriptionList.Count < 10)
                            roformData.conveyanceBillDescriptionList.Add(new Model.ConveyanceBillDescription()
                            {
                                sno = roformData.conveyanceBillDescriptionList.Count + 1
                            });
                    }
                    TempData["ConveyanceBillDescriptionData"] = roformData.conveyanceBillDescriptionList;
                    return Json(new { part = 1, htmlData = ConvertViewToString("_ConveyanceDescriptionGridView", roformData) }, JsonRequestBehavior.AllowGet);
                }
                else if (ButtonType != "Add Row")
                {
                    if (!roformData.ReportingTo.HasValue || roformData.ReportingTo.Value == 0)
                    {
                        var errormsg = "You can not apply for conveyance bill approval right now because either your Reporting or Reviewing Manager is not set.";
                        return Json(new { part = 0, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormConveyanceBill", roformData) }, JsonRequestBehavior.AllowGet);
                    }

                    if (roformData.loggedInEmpID == roformData.ReportingTo)
                    {
                        if (roformData.conveyanceBillDetails.ReportingAcceptedRejected == Model.AcceptedOrRejected.Accepted)
                            roformData.conveyanceBillDetails.IsReportingRejected = false;
                        else if (roformData.conveyanceBillDetails.ReportingAcceptedRejected == Model.AcceptedOrRejected.Rejected)
                            roformData.conveyanceBillDetails.IsReportingRejected = true;

                        if (roformData.ReviewingTo != roformData.ReportingTo && string.IsNullOrEmpty(roformData.conveyanceBillDetails.ReportingRemarks))
                        {
                            ModelState.AddModelError("ReqReportingRemarks", "Please enter remark.");
                        }
                    }
                    if (roformData.loggedInEmpID == roformData.ReviewingTo)
                    {
                        if (roformData.conveyanceBillDetails.ReviewingAcceptedRejected == Model.AcceptedOrRejected.Accepted)
                            roformData.conveyanceBillDetails.IsReviewingRejected = false;
                        else if (roformData.conveyanceBillDetails.ReviewingAcceptedRejected == Model.AcceptedOrRejected.Rejected)
                            roformData.conveyanceBillDetails.IsReviewingRejected = true;

                        if (string.IsNullOrEmpty(roformData.conveyanceBillDetails.ReviewingRemarks))
                        {
                            ModelState.AddModelError("ReqReviewingRemarks", "Please enter remark.");
                        }
                    }

                    var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                    roformData.submittedBy = formNewAttributes.SubmittedBy;
                    roformData.conveyanceBillDetails.FormState = formNewAttributes.FormState;

                    if (!conveyanceBillService.ConveyanceDataExists(roformData.EmployeeID, roformData.conveyanceBillDetails.ConveyanceBillDetailID))
                    {
                        roformData.conveyanceBillDetails.CreatedBy = userDetail.UserID;
                        roformData.conveyanceBillDetails.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                        roformData.conveyanceBillDetails.CreatedOn = DateTime.Now;

                        Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                        {
                            SenderID = userDetail.EmployeeID,
                            ReceiverID = formNewAttributes.ReciverID,
                            SenderDepartmentID = userDetail.DepartmentID,
                            SenderDesignationID = userDetail.DesignationID,
                            CreatedBy = userDetail.UserID,
                            EmployeeID = roformData.EmployeeID,
                            Scomments = $"Form Conveyance Bill Submited By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                            ProcessID = (int)WorkFlowProcess.ConveyanceBill,
                            StatusID = (int)roformData.conveyanceBillDetails.FormState
                        };
                        roformData._ProcessWorkFlow = workFlow;
                        roformData.conveyanceBillDetails.DesignationID = (int)userDetail.DesignationID;
                        roformData.conveyanceBillDetails.DepartmentID = (int)userDetail.DepartmentID;
                        int conveyanceDetailId = 0;
                        flag = conveyanceBillService.InsertConveyanceBillData(roformData, out conveyanceDetailId);
                        var msgs = ButtonType == "Save" ? "Bill Saved successfully" : "Bill Submitted successfully";
                        return Json(new { part = 0, msgType = "success", msg = msgs }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (roformData.EmployeeID == roformData.loggedInEmpID)
                        {
                            if (roformData.conveyanceBillDetails.Dated == null)
                                ModelState.AddModelError("dateRequired", "Please enter date");
                            if (roformData.conveyanceBillDescriptionList == null || roformData.conveyanceBillDescriptionList.Count == 0)
                                ModelState.AddModelError("ConveyanceDescription", "Please add at least one conveyance description.");


                            if (ModelState.IsValid)
                            {
                                roformData.conveyanceBillDetails.UpdatedBy = userDetail.UserID;
                                roformData.conveyanceBillDetails.UpdatedOn = DateTime.Now;

                                Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                {
                                    SenderID = userDetail.EmployeeID,
                                    ReceiverID = formNewAttributes.ReciverID,
                                    SenderDepartmentID = userDetail.DepartmentID,
                                    SenderDesignationID = userDetail.DesignationID,
                                    CreatedBy = userDetail.UserID,
                                    EmployeeID = roformData.EmployeeID,
                                    Scomments = $"Form Conveyance Bill Updated By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                    ProcessID = (int)WorkFlowProcess.ConveyanceBill,
                                    StatusID = (int)roformData.conveyanceBillDetails.FormState
                                };
                                roformData._ProcessWorkFlow = workFlow;
                                flag = conveyanceBillService.UpdateConveyanceBillData(roformData);
                                var msgs = ButtonType == "Save" ? "Bill Saved successfully" : "Bill Submitted successfully";
                                return Json(new { part = 0, msgType = "success", formState = roformData.conveyanceBillDetails.FormState, msg = msgs }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                roformData = (Model.ConveyanceBillForm)TempData["ConveyanceBill"];
                                roformData.conveyanceBillDescriptionList = (List<Model.ConveyanceBillDescription>)TempData["ConveyanceBillDescriptionData"];
                                TempData.Keep("ConveyanceBill");
                                TempData.Keep("ConveyanceBillDescriptionData");
                                return Json(new { part = 8, htmlData = ConvertViewToString("_FormConveyanceBill", roformData) }, JsonRequestBehavior.AllowGet);
                                //return PartialView("_FormConveyanceBill", roformData);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < roformData.conveyanceBillDescriptionList.Count; i++)
                            {
                                var str = "conveyanceBillDescriptionList" + "[" + i + "]" + ".VehicleID";
                                ModelState.Remove(str);
                            }
                            var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();

                            if (ModelState.IsValid)
                            {
                                roformData.conveyanceBillDetails.UpdatedBy = userDetail.UserID;
                                roformData.conveyanceBillDetails.UpdatedOn = DateTime.Now;

                                if ((roformData.conveyanceBillDetails.FormState == 3 || roformData.conveyanceBillDetails.FormState == 6) || ((roformData.conveyanceBillDetails.FormState == 4 && !roformData.conveyanceBillDetails.IsReportingRejected) || (roformData.conveyanceBillDetails.FormState == 7 && !roformData.conveyanceBillDetails.IsReviewingRejected)))
                                {
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        ReceiverID = formNewAttributes.ReciverID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form Conveyance Bill Updated By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.ConveyanceBill,
                                        StatusID = (int)roformData.conveyanceBillDetails.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;
                                }
                                else if (roformData.conveyanceBillDetails.IsReportingRejected && roformData.loggedInEmpID == roformData.ReportingTo && roformData.ApprovalHierarchy == 1)
                                {
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        ReceiverID = roformData.EmployeeID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form Conveyance Bill Updated By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.ConveyanceBill,
                                        StatusID = (int)roformData.conveyanceBillDetails.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;
                                }
                                else if (roformData.conveyanceBillDetails.IsReviewingRejected && roformData.loggedInEmpID == roformData.ReviewingTo && roformData.ApprovalHierarchy == 1)
                                {
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        ReceiverID = roformData.EmployeeID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form Conveyance Bill Rejected By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.ConveyanceBill,
                                        StatusID = (int)roformData.conveyanceBillDetails.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;
                                }
                                else if (roformData.ApprovalHierarchy == 2 && roformData.loggedInEmpID == roformData.ReviewingTo && roformData.conveyanceBillDetails.IsReviewingRejected)
                                {
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        ReceiverID = roformData.EmployeeID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form Conveyance Bill Rejected By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.ConveyanceBill,
                                        StatusID = (int)roformData.conveyanceBillDetails.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;
                                }

                                flag = conveyanceBillService.UpdateConveyanceBillData(roformData);
                                var msgs = ButtonType == "Save" ? "Bill Saved successfully" : "Bill Submitted successfully";
                                return Json(new { part = 0, formState = roformData.conveyanceBillDetails.FormState, msgType = "success", msg = msgs }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                roformData = (Model.ConveyanceBillForm)TempData["ConveyanceBill"];
                                roformData.conveyanceBillDescriptionList = (List<Model.ConveyanceBillDescription>)TempData["ConveyanceBillDescriptionData"];
                                TempData.Keep("ConveyanceBill");
                                TempData.Keep("ConveyanceBillDescriptionData");
                                return Json(new { part = 8, htmlData = ConvertViewToString("_FormConveyanceBill", roformData) }, JsonRequestBehavior.AllowGet);
                                //return PartialView("_FormConveyanceBill", roformData);
                            }
                        }
                    }
                }
                else
                {
                    return Json(new { part = 1, htmlData = ConvertViewToString("_ConveyanceDescriptionGridView", roformData) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _RemoveConveyanceRow(int sNo)
        {
            try
            {
                Model.ConveyanceBillForm modelDetails = new Model.ConveyanceBillForm();
                var modelData = (List<Model.ConveyanceBillDescription>)TempData["ConveyanceBillDescriptionData"];
                if (modelData != null)
                {
                    var deletedRow = modelData.Where(x => x.sno == sNo).FirstOrDefault();
                    modelData.Remove(deletedRow);
                    TempData["ConveyanceBillDescriptionData"] = modelData;
                    modelDetails.conveyanceBillDescriptionList = modelData;
                    TempData.Keep("ConveyanceBillDescriptionData");
                    return PartialView("_ConveyanceDescriptionGridView", modelDetails);
                }
                return Content("");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [NonAction]
        private Model.ConveyanceRulesAttributes GetFormAttributes(Model.ConveyanceBillForm appForm, string buttonType)
        {
            log.Info($"AppraisalController/GetFormAttributes");
            try
            {
                var reportingYr = DateTime.Now.GetFinancialYr();
                Model.ConveyanceRulesAttributes formRules = new Model.ConveyanceRulesAttributes();
                formRules = conveyanceBillService.GetConveyanceFormRulesAttributes(appForm.EmployeeID, appForm.ConveyanceDetailID);
                if (appForm.ApprovalHierarchy == 1)
                {
                    if (appForm.loggedInEmpID == appForm.ReportingTo)
                    {
                        //formRules.FormState = (buttonType == "Save" ? (int)Model.ConveyanceFormState.SavedByReporting : (int)Model.ConveyanceFormState.SubmitedByReporting);

                        formRules.FormState = (buttonType == "Save" ? (int)Model.ConveyanceFormState.SavedByReporting : !appForm.conveyanceBillDetails.IsReportingRejected ? (int)Model.ConveyanceFormState.SubmitedByReporting : (int)Model.ConveyanceFormState.RejectedByReporting);

                        formRules.SubmittedBy = Model.ConveyanceSubmittedBy.ReportingOfficer;
                        formRules.SenderID = appForm.ReportingTo;
                        formRules.ReciverID = appForm.ReviewingTo;
                    }
                    else if (appForm.loggedInEmpID == appForm.ReviewingTo)
                    {
                        //formRules.FormState = (buttonType == "Save" ? (int)Model.ConveyanceFormState.SavedByReviewer : (int)Model.ConveyanceFormState.SubmitedByReviewer);
                        formRules.FormState = (buttonType == "Save" ? (int)Model.ConveyanceFormState.SavedByReviewer : !appForm.conveyanceBillDetails.IsReviewingRejected ? (int)Model.ConveyanceFormState.SubmitedByReviewer : (int)Model.ConveyanceFormState.RejectedByReviewer);

                        formRules.SubmittedBy = Model.ConveyanceSubmittedBy.ReviewingOfficer;
                        formRules.SenderID = appForm.ReviewingTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
                    }
                    else if (appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.ConveyanceFormState.SavedByAcceptanceAuth : (int)Model.ConveyanceFormState.SubmitedByAcceptanceAuth);
                        formRules.SubmittedBy = Model.ConveyanceSubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = appForm.AcceptanceAuthorityTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
                    }
                    else
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.ConveyanceFormState.SavedByEmployee : (int)Model.ConveyanceFormState.SubmitedByEmployee);
                        formRules.SubmittedBy = Model.ConveyanceSubmittedBy.Employee;
                        formRules.SenderID = appForm.EmployeeID;
                        formRules.ReciverID = appForm.ReportingTo;
                    }

                }
                else if (appForm.ApprovalHierarchy == 2)
                {
                    if (appForm.loggedInEmpID == appForm.ReviewingTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.ConveyanceFormState.SavedByReviewer : !appForm.conveyanceBillDetails.IsReviewingRejected ? (int)Model.ConveyanceFormState.SubmitedByReviewer : (int)Model.ConveyanceFormState.RejectedByReviewer);
                        formRules.SubmittedBy = Model.ConveyanceSubmittedBy.ReviewingOfficer;
                        formRules.SenderID = appForm.ReviewingTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
                    }
                    else if (appForm.loggedInEmpID == appForm.ReviewingTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.ConveyanceFormState.SavedByAcceptanceAuth : (int)Model.ConveyanceFormState.SubmitedByAcceptanceAuth);
                        formRules.SubmittedBy = Model.ConveyanceSubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = appForm.AcceptanceAuthorityTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
                    }
                    else if (appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.ConveyanceFormState.SavedByAcceptanceAuth : (int)Model.ConveyanceFormState.SubmitedByAcceptanceAuth);
                        formRules.SubmittedBy = Model.ConveyanceSubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = appForm.AcceptanceAuthorityTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
                    }
                }
                else if (appForm.ApprovalHierarchy == 2.1 || appForm.ApprovalHierarchy == 3)
                {
                    if (appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.ConveyanceFormState.SavedByAcceptanceAuth : (int)Model.ConveyanceFormState.SubmitedByAcceptanceAuth);
                        formRules.SubmittedBy = Model.ConveyanceSubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = appForm.AcceptanceAuthorityTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
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

        public ActionResult ViewConveyance(int? empID, int conveyanceDetailID, int? reportingTo, int? reviewingTo, int? acceptanceAuthority)
        {
            log.Info($"ConveyanceBill/ViewConveyance");
            try
            {
                Model.ConveyanceBillForm conveyanceBillForm = new Model.ConveyanceBillForm();
                conveyanceBillForm.ReportingTo = reportingTo;
                conveyanceBillForm.ReviewingTo = reviewingTo;
                conveyanceBillForm.EmployeeID = (int)empID;
                conveyanceBillForm.AcceptanceAuthorityTo = acceptanceAuthority;
                conveyanceBillForm.loggedInEmpID = userDetail.EmployeeID;
                conveyanceBillForm.ConveyanceDetailID = conveyanceDetailID;

                conveyanceBillForm.isAdmin = true;


                if (reviewingTo.HasValue)
                    conveyanceBillForm.loggedInEmpID = reviewingTo;
                else
                    conveyanceBillForm.loggedInEmpID = reportingTo;

                var reportingDetails = confirmationService.GetRTRVDetails(conveyanceBillForm.ReportingTo);
                conveyanceBillForm.ReportingName = reportingDetails.Name;
                conveyanceBillForm.ReportingDesignation = reportingDetails.Designation;
                if (conveyanceBillForm.ReviewingTo != null)
                {
                    var reviewingDetails = confirmationService.GetRTRVDetails(conveyanceBillForm.ReviewingTo);
                    conveyanceBillForm.ReviewingName = reviewingDetails.Name;
                    conveyanceBillForm.ReviewingDesignation = reviewingDetails.Designation;
                }
                if (conveyanceBillForm.AcceptanceAuthorityTo != null)
                {
                    var reviewingDetails = confirmationService.GetAADetails(conveyanceBillForm.AcceptanceAuthorityTo);
                    conveyanceBillForm.AAName = reviewingDetails.Name;
                    conveyanceBillForm.AADesignation = reviewingDetails.Designation;
                }

                conveyanceBillForm.frmAttributes = conveyanceBillService.GetConveyanceFormRulesAttributes(conveyanceBillForm.EmployeeID, conveyanceDetailID);

                if (conveyanceBillForm.frmAttributes != null)
                {
                    if ((conveyanceBillForm.ReportingTo == conveyanceBillForm.ReviewingTo) && (conveyanceBillForm.ReviewingTo == conveyanceBillForm.AcceptanceAuthorityTo))
                        conveyanceBillForm.ApprovalHierarchy = 3;
                    else if (((conveyanceBillForm.ReportingTo != conveyanceBillForm.ReviewingTo) && (conveyanceBillForm.ReviewingTo == conveyanceBillForm.AcceptanceAuthorityTo)) && conveyanceBillForm.loggedInEmpID == conveyanceBillForm.ReviewingTo)
                        conveyanceBillForm.ApprovalHierarchy = 2.1;
                    else if (((conveyanceBillForm.ReportingTo == conveyanceBillForm.ReviewingTo) && (conveyanceBillForm.ReviewingTo != conveyanceBillForm.AcceptanceAuthorityTo))
                            && (conveyanceBillForm.loggedInEmpID == conveyanceBillForm.ReportingTo || conveyanceBillForm.loggedInEmpID == conveyanceBillForm.AcceptanceAuthorityTo))
                        conveyanceBillForm.ApprovalHierarchy = 2.0;
                    else
                        conveyanceBillForm.ApprovalHierarchy = 1;

                    if (conveyanceBillForm.frmAttributes.FormState == 1)
                    {
                        if (conveyanceBillForm.ApprovalHierarchy == 1 && conveyanceBillForm.loggedInEmpID == conveyanceBillForm.ReportingTo)
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                        else if (conveyanceBillForm.ApprovalHierarchy == 1 && conveyanceBillForm.loggedInEmpID == conveyanceBillForm.ReviewingTo)
                        {
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 1 && conveyanceBillForm.loggedInEmpID == conveyanceBillForm.AcceptanceAuthorityTo)
                        {
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.0 && conveyanceBillForm.loggedInEmpID == reviewingTo)
                        {
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.1 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 3 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.EmployeeSection = true;
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                    }
                    else if (conveyanceBillForm.frmAttributes.FormState == 2 || conveyanceBillForm.frmAttributes.FormState == 3)
                    {
                        if (conveyanceBillForm.ApprovalHierarchy == 2.0 && conveyanceBillForm.loggedInEmpID == reviewingTo)
                        {
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.0 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.1 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 3 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                    }
                    else if (conveyanceBillForm.frmAttributes.FormState == 4 || conveyanceBillForm.frmAttributes.FormState == 5 || conveyanceBillForm.frmAttributes.FormState == 6)
                    {
                        if (conveyanceBillForm.ApprovalHierarchy == 1 && conveyanceBillForm.loggedInEmpID == reviewingTo)
                        {
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.0 && conveyanceBillForm.loggedInEmpID == reviewingTo)
                        {
                            conveyanceBillForm.frmAttributes.ReportingSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerSection = true;
                            conveyanceBillForm.frmAttributes.ReviewerButton = true;
                        }
                        else if (conveyanceBillForm.ApprovalHierarchy == 2.1 && conveyanceBillForm.loggedInEmpID == acceptanceAuthority)
                        {
                            conveyanceBillForm.frmAttributes.AcceptanceButton = true;
                        }
                    }
                }
                return View("ConveyanceFormContainer", conveyanceBillForm);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult Create(int empID, int? reportingTo, int? reviewingTo, int? acceptanceAuthority)
        {
            log.Info($"ConveyanceBillController/Create");
            bool flag;
            try
            {
                var reportingYr = DateTime.Now.GetFinancialYr();
                Model.ConveyanceBillForm roformData = new Model.ConveyanceBillForm();
                roformData.conveyanceBillDetails.CreatedBy = userDetail.UserID;
                roformData.conveyanceBillDetails.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                roformData.conveyanceBillDetails.CreatedOn = DateTime.Now;
                roformData.conveyanceBillDetails.CreatedBy = userDetail.UserID;
                roformData.EmployeeID = empID;
                roformData.conveyanceBillDetails.FormState = 1;
                roformData.conveyanceBillDetails.ReportingYear = (roformData.conveyanceBillDetails.ReportingYear != null && roformData.conveyanceBillDetails.ReportingYear.Length > 0) ? roformData.conveyanceBillDetails.ReportingYear : reportingYr;
                roformData.conveyanceBillDetails.EmployeeID = empID;
                roformData.conveyanceBillDetails.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                roformData.conveyanceBillDetails.VehicleProv = false;

                Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                {
                    SenderID = userDetail.EmployeeID,
                    ReceiverID = reportingTo,
                    SenderDepartmentID = userDetail.DepartmentID,
                    SenderDesignationID = userDetail.DesignationID,
                    CreatedBy = userDetail.UserID,
                    EmployeeID = roformData.EmployeeID,
                    Scomments = $"Form Conveyance Bill Saved By : {(Model.SubmittedBy)(int)1}",
                    ProcessID = (int)WorkFlowProcess.ConveyanceBill,
                    StatusID = 1
                };
                roformData._ProcessWorkFlow = workFlow;
                roformData.conveyanceBillDetails.DesignationID = (int)userDetail.DesignationID;
                roformData.conveyanceBillDetails.DepartmentID = (int)userDetail.DepartmentID;
                int conveyanceDetailId = 0;
             flag = conveyanceBillService.InsertConveyanceBillData(roformData, out conveyanceDetailId);
                var msgs = "Bill Saved successfully";
                return Json(new { part = 4, msgType = "Inserted", empID = empID, reportingTo = reportingTo, reviewingTo = reviewingTo, acceptanceAuthority = acceptanceAuthority, conveyanceDetailId = conveyanceDetailId, msg = msgs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}