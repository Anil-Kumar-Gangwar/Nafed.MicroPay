using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using static Nafed.MicroPay.Common.ExtensionMethods;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Controllers.Appraisal
{
    public class AppraisalController : BaseController
    {
        private readonly IAppraisalFormService appraisalService;
        private readonly IPRService prService;
        private readonly IDropdownBindService ddlServices;
        public AppraisalController(IAppraisalFormService appraisalService, IPRService prService, IDropdownBindService ddlServices)
        {
            this.appraisalService = appraisalService;
            this.prService = prService;
            this.ddlServices = ddlServices;
        }
        // GET: Appraisal
        public ActionResult Index()
        {
            log.Info($"AppraisalController/Index");
            EmployeeAppraisalViewModel empAppraisalVM = new Models.EmployeeAppraisalViewModel();
            empAppraisalVM.empAppraisalFormID = userDetail.AppraisalFormID;

            if (userDetail.AppraisalFormID.HasValue)
            {
                var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.Appraisal);
                empAppraisalVM.approvalSetting = approvalSettings ?? new Model.EmployeeProcessApproval();
            }
            return View(empAppraisalVM);
        }

        public ActionResult Edit(int? appraisalFormID, int? reportingTo, int? reviewingTo, int? acceptanceAuthority, int? empID, string reportingYr)
        {
            log.Info($"AppraisalController/AppraisalForm");
            try
            {

                //if (!appraisalService.IsTrainingSubmitted((int)empID, reportingYr))
                //{
                //    //   var errormsg = "You can not apply for appraisal right now, please first apply for skill assessment.";
                //    return Json(new { type = 1, msgType = "error" }, JsonRequestBehavior.AllowGet);
                //}

                if (appraisalFormID.HasValue)
                {
                    //   var reportingYr = DateTime.Now.GetFinancialYr();
                    Model.AppraisalForm appForm = new Model.AppraisalForm();
                    appForm.FormID = appraisalFormID.Value;
                    appForm.ReportingTo = reportingTo;
                    appForm.ReviewingTo = reviewingTo;
                    appForm.EmployeeID = (int)empID;
                    appForm.AcceptanceAuthorityTo = acceptanceAuthority;
                    appForm.loggedInEmpID = userDetail.EmployeeID;
                    appForm.ReportingYr = reportingYr;
                    appForm.frmAttributes = appraisalService.GetFormRulesAttributes((AppraisalForm)appraisalFormID.Value, appForm.EmployeeID, reportingYr);
                    if (appForm.frmAttributes != null)
                    {
                        if ((appForm.ReportingTo == appForm.ReviewingTo) && (appForm.ReviewingTo == appForm.AcceptanceAuthorityTo))
                            appForm.ApprovalHierarchy = 3;
                        else if (((appForm.ReportingTo != appForm.ReviewingTo) && (appForm.ReviewingTo == appForm.AcceptanceAuthorityTo))
                            && appForm.loggedInEmpID == appForm.ReviewingTo)
                            appForm.ApprovalHierarchy = 2.1;
                        else if (((appForm.ReportingTo == appForm.ReviewingTo) && (appForm.ReviewingTo != appForm.AcceptanceAuthorityTo))
                            && (appForm.loggedInEmpID == appForm.ReportingTo || appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo))
                            appForm.ApprovalHierarchy = 2.0;
                        else
                            appForm.ApprovalHierarchy = 1;

                        if (appForm.frmAttributes.FormState == 1)
                        {
                            if (appForm.ApprovalHierarchy == 1 && appForm.loggedInEmpID == appForm.ReportingTo && DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate && DateTime.Now.Date < appForm.frmAttributes.ReportingSubmissionDate)
                                appForm.frmAttributes.EmployeeSection = true;
                            else if (appForm.ApprovalHierarchy == 1 && appForm.loggedInEmpID == appForm.ReviewingTo &&
                                (DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate && DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate) && DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate)
                            {
                                appForm.frmAttributes.EmployeeSection = true;
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 1 && appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo &&
                                (DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate && DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate && DateTime.Now.Date > appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.EmployeeSection = true;
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 2.0 && appForm.loggedInEmpID == reviewingTo && (DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate && DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.EmployeeSection = true;
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.ReviewerButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 2.1 && appForm.loggedInEmpID == acceptanceAuthority &&
                                (DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate && DateTime.Now.Date < appForm.frmAttributes.AcceptanceSubmissionDate))
                            {
                                appForm.frmAttributes.EmployeeSection = true;
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 3 && appForm.loggedInEmpID == acceptanceAuthority && (DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate && DateTime.Now.Date < appForm.frmAttributes.AcceptanceSubmissionDate))
                            {
                                appForm.frmAttributes.EmployeeSection = true;
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }
                            else if (appForm.loggedInEmpID == appForm.EmployeeID && DateTime.Now.Date < appForm.frmAttributes.EmployeeSubmissionDate)
                            {
                                appForm.frmAttributes.EmployeeSection = true;
                            }

                            if (appForm.loggedInEmpID == appForm.ReportingTo && DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate)
                            {
                                appForm.frmAttributes.EmployeeSection = false;
                            }

                            if (appForm.loggedInEmpID == appForm.ReviewingTo && DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate)
                            {
                                appForm.frmAttributes.EmployeeSection = false;
                                appForm.frmAttributes.ReportingSection = false;
                            }

                            if (appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo && DateTime.Now.Date > appForm.frmAttributes.ReviewerSubmissionDate)
                            {
                                appForm.frmAttributes.EmployeeSection = false;
                                appForm.frmAttributes.ReportingSection = false;
                                appForm.frmAttributes.ReviewerSection = false;
                            }
                        }
                        else if (appForm.frmAttributes.FormState == 2 || appForm.frmAttributes.FormState == 3)
                        {
                            if (appForm.ApprovalHierarchy == 2.0 && appForm.loggedInEmpID == reviewingTo && (DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.ReviewerButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 2.0 && appForm.loggedInEmpID == acceptanceAuthority && (DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 2.1 && appForm.loggedInEmpID == acceptanceAuthority && (DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate))
                            {
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 3 && appForm.loggedInEmpID == acceptanceAuthority && DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate)
                            {
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }

                            if (appForm.loggedInEmpID == appForm.ReviewingTo && DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate)
                            {
                                appForm.frmAttributes.EmployeeSection = false;
                                appForm.frmAttributes.ReportingSection = false;
                                appForm.frmAttributes.ReportingButton = false;
                                appForm.frmAttributes.ReviewerButton = true;
                            }

                            if (appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo && DateTime.Now.Date > appForm.frmAttributes.ReviewerSubmissionDate)
                            {
                                appForm.frmAttributes.EmployeeSection = false;
                                appForm.frmAttributes.ReportingSection = false;
                                appForm.frmAttributes.ReviewerSection = false;
                            }

                            //else if (appForm.ApprovalHierarchy == 1 && (appForm.loggedInEmpID == reportingTo) && appForm.frmAttributes.FormState == 3 && DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate && DateTime.Now.Date < appForm.frmAttributes.ReportingSubmissionDate)
                            //{
                            //    appForm.frmAttributes.EmployeeSection = true;
                            //}

                            //if (appForm.ApprovalHierarchy == 1 && (appForm.loggedInEmpID == reviewingTo) && DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate && appForm.frmAttributes.FormState == 2)
                            //{
                            //    appForm.frmAttributes.ReportingSection = true;
                            //}
                        }
                        else if (appForm.frmAttributes.FormState == 4 || appForm.frmAttributes.FormState == 5)
                        {
                            if (appForm.ApprovalHierarchy == 1 && appForm.loggedInEmpID == reviewingTo && (DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.ReviewerButton = true;

                                //if (appForm.frmAttributes.FormState == 5)
                                //{
                                //    if (DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate)
                                //    {
                                //        appForm.frmAttributes.EmployeeSection = true;
                                //    }
                                //    if (DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate)
                                //    {
                                //        appForm.frmAttributes.ReportingSection = true;
                                //    }
                                //}
                            }
                            else if (appForm.ApprovalHierarchy == 2.0 && appForm.loggedInEmpID == reviewingTo && (DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.ReviewerButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 2.1 && appForm.loggedInEmpID == acceptanceAuthority && (DateTime.Now.Date > appForm.frmAttributes.ReviewerSubmissionDate && DateTime.Now.Date < appForm.frmAttributes.AcceptanceSubmissionDate))
                            {
                                appForm.frmAttributes.AcceptanceButton = true;
                            }

                            if (appForm.ApprovalHierarchy == 1 && (appForm.loggedInEmpID == acceptanceAuthority) && DateTime.Now.Date > appForm.frmAttributes.ReviewerSubmissionDate && appForm.frmAttributes.FormState == 4)
                            {
                                appForm.frmAttributes.ReviewerSection = false;
                            }
                        }
                        else if (appForm.frmAttributes.FormState == 6 || appForm.frmAttributes.FormState == 7)
                        {
                            if (appForm.frmAttributes.FormState == 7)
                            {
                                if (appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo && DateTime.Now.Date > appForm.frmAttributes.ReviewerSubmissionDate)
                                {
                                    appForm.frmAttributes.EmployeeSection = false;
                                    appForm.frmAttributes.ReportingSection = false;
                                    appForm.frmAttributes.ReviewerSection = false;
                                }
                            }
                        }

                        //else if (appForm.frmAttributes.FormState == 7)
                        //{
                        //    if (DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate)
                        //    {
                        //        appForm.frmAttributes.EmployeeSection = true;
                        //    }
                        //    if (DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate)
                        //    {
                        //        appForm.frmAttributes.ReportingSection = true;
                        //    }
                        //    if (DateTime.Now.Date > appForm.frmAttributes.ReviewerSubmissionDate)
                        //    {
                        //        appForm.frmAttributes.ReviewerSection = true;
                        //    }
                        //}
                    }
                    if (appraisalFormID.Value == (int)AppraisalForm.FormA)
                        ViewBag.Title = "A.P.A.R - Form A";
                    else if (appraisalFormID.Value == (int)AppraisalForm.FormB)
                        ViewBag.Title = "A.P.A.R - Form B";
                    else if (appraisalFormID.Value == (int)AppraisalForm.FormC)
                        ViewBag.Title = "A.P.A.R - Form C";
                    else if (appraisalFormID.Value == (int)AppraisalForm.FormD)
                        ViewBag.Title = "A.P.A.R - Form D";
                    else if (appraisalFormID.Value == (int)AppraisalForm.FormE)
                        ViewBag.Title = "A.P.A.R - Form E";

                    else if (appraisalFormID.Value == (int)AppraisalForm.FormF)
                        ViewBag.Title = "A.P.A.R - Form F";

                    else if (appraisalFormID.Value == (int)AppraisalForm.FormG)
                        ViewBag.Title = "A.P.A.R - Form G";
                    else if (appraisalFormID.Value == (int)AppraisalForm.FormH)
                        ViewBag.Title = "A.P.A.R - Form H";
                    else
                        return Content("");

                    return View("AppraisalFormContainer", appForm);
                }
                else
                    return Content("");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult MyAppraisalList()
        {
            log.Info($"AppraisalController/MyAppraisalList");
            try
            {
                EmployeeAppraisalViewModel empAppraisalVM = new Models.EmployeeAppraisalViewModel();
                var selfFormList = appraisalService.GetEmployeeSelfAppraisalList((int)userDetail.EmployeeID, null, null);
                empAppraisalVM.empAppraisalFormID = userDetail.AppraisalFormID;
                if (selfFormList.Count == 0)
                {
                    List<Model.AppraisalFormHdr> frmList = new List<Model.AppraisalFormHdr>();
                    empAppraisalVM.formList = frmList;
                }
                else
                    empAppraisalVM.formList = selfFormList;
                return PartialView("_MyAppraisalFormList", empAppraisalVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [NonAction]
        private int GetWorkFlowReceiverID(Model.AppraisalForm roformData)
        {
            int? receiverID = 0;
            if (roformData.EmployeeID == roformData.loggedInEmpID)
            {
                if (roformData.ReportingTo.HasValue)
                {
                    receiverID = roformData.ReportingTo;
                }
            }
            else if (roformData.ReportingTo == roformData.loggedInEmpID)
            {
                if (roformData.ReviewingTo.HasValue)
                {
                    receiverID = roformData.ReviewingTo;
                }
                else if (!roformData.ReviewingTo.HasValue)
                {
                    receiverID = roformData.ReportingTo;

                }
            }
            else if (roformData.ReviewingTo == roformData.loggedInEmpID)
            {
                if (roformData.AcceptanceAuthorityTo.HasValue)
                {
                    receiverID = roformData.AcceptanceAuthorityTo;
                }
                else if (!roformData.AcceptanceAuthorityTo.HasValue)
                {
                    receiverID = roformData.ReviewingTo;
                }
            }
            else if (roformData.AcceptanceAuthorityTo == roformData.loggedInEmpID)
            {
                receiverID = roformData.AcceptanceAuthorityTo;
            }

            return receiverID ?? 0;
        }

        #region Form Group A

        public ActionResult _GetFormGroupA(Model.AppraisalForm appraisal)
        {
            log.Info($"AppraisalController/_GetFormGroupA");
            try
            {
                var reportingYr = appraisal.ReportingYr; // string.IsNullOrEmpty(appraisal.ReportingYr) ? DateTime.Now.GetFinancialYr() : appraisal.ReportingYr;
                //appraisal.loggedInEmpID = userDetail.EmployeeID;
                if (!appraisal.isAdmin)
                    appraisal.loggedInEmpID = userDetail.EmployeeID;
                else
                {
                    if (appraisal.AcceptanceAuthorityTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.AcceptanceAuthorityTo.Value;
                    else if (appraisal.ReviewingTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.ReviewingTo.Value;
                    else
                        appraisal.loggedInEmpID = appraisal.ReportingTo.Value;
                }

                appraisal.formGroupAHdr = appraisalService.GetFormGroupHdrDetail("FormGroupAHdr", null, appraisal.EmployeeID, reportingYr, null);
                appraisal.formGroupAHdr.FormID = appraisal.FormID;
                appraisal.EmployeeID = appraisal.formGroupAHdr.EmployeeID;
                appraisal.formGroupAHdr.ReportingYr = reportingYr; // (appraisal.formGroupAHdr.ReportingYr != null && appraisal.formGroupAHdr.ReportingYr.Length > 0) ? appraisal.formGroupAHdr.ReportingYr : reportingYr;
                appraisal.formGroupAHdr.Part4_1_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_1_Grade;
                appraisal.formGroupAHdr.Part4_2_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_2_Grade;
                appraisal.formGroupAHdr.Part4_3_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_3_Grade;
                appraisal.formGroupAHdr.FormPart4Integrity = (Model.FormPart4Integrity)appraisal.formGroupAHdr.PART4_4_Grade;
                appraisal.formGroupADetail1List = appraisalService.GetFormGroupDetail1("FormGroupAHdr", "FormGroupADetail1", appraisal.EmployeeID, appraisal.formGroupAHdr.FormGroupID).ToList();
                appraisal.formGroupATrainingDtls = appraisalService.GetFormTrainingDetail("FormGroupAHdr", "FormGroupTrainingDtls", appraisal.EmployeeID, appraisal.formGroupAHdr.FormGroupID, appraisal.FormID, appraisal.formGroupAHdr.ReportingYr).ToList();
                appraisal.ReportingYr = appraisal.formGroupAHdr.ReportingYr;

                var currentYr = int.Parse(appraisal.ReportingYr.Substring(0, 4));
                var prHeader = prService.GetPropertyReturn(currentYr, appraisal.EmployeeID);

                if (prHeader != null)
                {
                    appraisal.AnnualPropertyReturnSubmitted = prHeader.StatusID == 1 ? true : false;
                    appraisal.AnnualPropertySubmittedOn = prHeader.StatusID == 1 ? prHeader.UpdatedOn : null;
                }
                if (appraisal.formGroupATrainingDtls.Count > 0)
                {
                    var sno = 1;
                    appraisal.formGroupATrainingDtls.ForEach(x =>
                    {
                        x.sno = sno++;
                        x.FormTraining = (Model.FormPart4Training)x.TrainingID;
                    });
                }

                if (appraisal.formGroupADetail1List.Count > 0)
                {
                    var sno = 1;
                    appraisal.formGroupADetail1List.ForEach(x =>
                    {
                        x.sno = sno++;
                    });
                }
                appraisal.formGroupADetail2List = appraisalService.GetFormGroupDetail2("FormGroupAHdr", "FormGroupADetail2", (int)userDetail.EmployeeID, appraisal.formGroupAHdr.FormGroupID).ToList();
                appraisal.isValid = true;
                TempData["appraisal"] = appraisal;
                TempData.Keep("appraisal");
                return PartialView("_FormGroupA", appraisal);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostROFormData(Model.AppraisalForm roformData, string ButtonType, FormCollection frm)
        {
            log.Info($"AppraisalController/_PostROFormData");
            bool flag;
            try
            {
                if (ButtonType == "Add Row")
                {
                    if (roformData.formGroupADetail1List == null)
                        roformData.formGroupADetail1List = new List<Model.FormGroupDetail1>() {
                            new Model.FormGroupDetail1() { sno = 1, } };
                    else
                    {
                        if (roformData.formGroupADetail1List.Count == 1)
                            roformData.formGroupADetail1List.FirstOrDefault().sno = 1;
                        else
                        {
                            var s_no = 1;
                            roformData.formGroupADetail1List.ForEach(x =>
                            {
                                x.sno = s_no++;
                            });

                        }
                        if (roformData.formGroupADetail1List.Count < 10)
                            roformData.formGroupADetail1List.Add(new Model.FormGroupDetail1()
                            {
                                sno = roformData.formGroupADetail1List.Count + 1
                            });
                    }
                    TempData["frmGroupPart2Data"] = roformData;
                    return Json(new { part = 1, htmlData = ConvertViewToString("_FormGroupPart2GridView", roformData) }, JsonRequestBehavior.AllowGet);

                }
                else if (ButtonType == "Add New Training Row")
                {
                    if (roformData.formGroupATrainingDtls == null)
                        roformData.formGroupATrainingDtls = new List<Model.FormTrainingDtls>()
                        { new Model.FormTrainingDtls() { sno = 1  } };
                    else
                    {
                        if (roformData.formGroupATrainingDtls.Count == 1)
                            roformData.formGroupATrainingDtls.FirstOrDefault().sno = 1;

                        if (roformData.formGroupATrainingDtls.Count < 3)
                            roformData.formGroupATrainingDtls.Add(new Model.FormTrainingDtls()
                            {
                                sno = roformData.formGroupATrainingDtls.Count + 1
                            });
                    }
                    TempData["frmGroupTrainingData"] = roformData;
                    return Json(new { part = 2, htmlData = ConvertViewToString("_FormGroupTrainingGridView", roformData) }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    if (!roformData.ReportingTo.HasValue || roformData.ReportingTo.Value == 0)
                    {
                        var errormsg = "You can not apply for appraisal right now because either your Reporting or Reviewing Manager is not set.";
                        return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupA", roformData) }, JsonRequestBehavior.AllowGet);
                    }

                    if (ButtonType == "Save")
                    {
                        var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                        roformData.submittedBy = formNewAttributes.SubmittedBy;
                        roformData.formGroupAHdr.FormState = formNewAttributes.FormState;

                        if (!appraisalService.AppraisalDataExists(roformData.EmployeeID, roformData.FormID, roformData.formGroupAHdr.ReportingYr))
                        {
                            roformData.formGroupAHdr.CreatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                            roformData.formGroupAHdr.CreatedOn = DateTime.Now;
                            roformData.formGroupAHdr.DesignationID = (int)userDetail.DesignationID;
                            roformData.formGroupAHdr.DepartmentID = (int)userDetail.DepartmentID;
                            flag = appraisalService.InsertFormAData(roformData);
                            var redirect = ButtonType == "Save" ? 0 : 1;
                            return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = "Form Saved successfully.", redirect = redirect }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {

                            roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                            flag = appraisalService.UpdateFormAData(roformData, "RO");
                            var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                            var redirect = ButtonType == "Save" ? 0 : 1;
                            return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg, redirect = redirect }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue && roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                        {
                            if (roformData.formGroupAHdr.WorkedPeriodUnderROTo.Value < roformData.formGroupAHdr.WorkedPeriodUnderROFrom.Value)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROFrom", "From date should be always less than To date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupA", roformData);
                            }
                        }

                        //if (!appraisalService.IsTrainingSubmitted(roformData.EmployeeID, roformData.formGroupAHdr.ReportingYr))
                        //{
                        //    var errormsg = "You can not apply for appraisal right now, please first apply for skill assessment.";
                        //    return Json(new { isTraining = 1, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupA", roformData) }, JsonRequestBehavior.AllowGet);
                        //}

                        var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                        roformData.submittedBy = formNewAttributes.SubmittedBy;
                        roformData.formGroupAHdr.FormState = formNewAttributes.FormState;

                        //if (roformData.EmployeeID == roformData.loggedInEmpID)
                        //{
                        //    roformData.submittedBy = Model.SubmittedBy.Employee;
                        //    roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByEmployee : (int)Model.AppraisalFormState.SubmitedByEmployee);

                        //}
                        //else 
                        if (roformData.ReportingTo == roformData.loggedInEmpID)
                        {
                            // roformData.submittedBy = roformData.ApprovalHierarchy == 1 ? Model.SubmittedBy.ReportingOfficer :
                            // roformData.ApprovalHierarchy == 2.0 ? Model.SubmittedBy.ReviewingOfficer : roformData.ApprovalHierarchy == 2.1 ?
                            //  Model.SubmittedBy.AcceptanceAuthority : roformData.ApprovalHierarchy == 3 ? Model.SubmittedBy.AcceptanceAuthority : Model.SubmittedBy.ReportingOfficer;

                            //   roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReporting : (int)Model.AppraisalFormState.SubmitedByReporting);

                            if (roformData.ReportingTo != roformData.ReviewingTo)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_1");
                                ModelState.Remove("formGroupADetail2List[0].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[1].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[2].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[3].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[4].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[5].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[6].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[7].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[8].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[9].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[10].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[11].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[12].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[13].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[14].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[15].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[16].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[17].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[18].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[19].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[20].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[21].ReviewingAuthorityWeightage");
                            }
                            else
                            {
                            }
                            ModelState.Remove("formGroupAHdr.PART5_2");
                            ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            if (roformData.ReportingTo == roformData.ReviewingTo && roformData.ReviewingTo == roformData.AcceptanceAuthorityTo)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                                {
                                    ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                                }
                            }
                            else
                                ModelState.Remove("formGroupAHdr.Remarks");

                        }
                        else if (roformData.ReviewingTo == roformData.loggedInEmpID)
                        {
                            if (roformData.ReviewingTo != roformData.AcceptanceAuthorityTo)
                            {
                                ModelState.Remove("formGroupAHdr.AcceptanceAuthorityRemarks");
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                                {
                                    ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                                }
                            }


                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART4_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                            ModelState.Remove("formGroupAHdr.PART2_3B");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            //ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            var part5_5_Weightage = frm["formGroupAHdr.PART5_5_Weightage"].ToString();
                            var part4_6_Weightage = frm["formGroupAHdr.PART4_6_Weightage"].ToString();

                            if (part5_5_Weightage == "NaN")
                                part5_5_Weightage = "";
                            if (part4_6_Weightage == "NaN")
                                part4_6_Weightage = "";

                            roformData.formGroupAHdr.PART5_5_Weightage = (part5_5_Weightage == "" || part5_5_Weightage == null) ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            roformData.formGroupAHdr.PART4_6_Weightage = (part4_6_Weightage == "" || part4_6_Weightage == null) ? 0 : Convert.ToDecimal(part4_6_Weightage);

                            //roformData.formGroupAHdr.PART5_5_Weightage = part5_5_Weightage == "" ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            //roformData.formGroupAHdr.PART4_6_Weightage = part4_6_Weightage == "" ? 0 : Convert.ToDecimal(part4_6_Weightage);

                            //roformData.formGroupAHdr.PART5_5_Weightage = Convert.ToDecimal(part5_5_Weightage);
                            //roformData.formGroupAHdr.PART4_6_Weightage = Convert.ToDecimal(part4_6_Weightage);
                            //  roformData.submittedBy = Model.SubmittedBy.ReviewingOfficer;
                            //  roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                        }
                        else if (roformData.AcceptanceAuthorityTo == roformData.loggedInEmpID)
                        {
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");
                            ModelState.Remove("formGroupAHdr.PART2_3B");
                            var part5_5_Weightage = frm["formGroupAHdr.PART5_5_Weightage"].ToString();
                            var part4_6_Weightage = frm["formGroupAHdr.PART4_6_Weightage"].ToString();
                            if (part5_5_Weightage == "NaN")
                                part5_5_Weightage = "";
                            if (part4_6_Weightage == "NaN")
                                part4_6_Weightage = "";

                            roformData.formGroupAHdr.PART5_5_Weightage = (part5_5_Weightage == "" || part5_5_Weightage == null) ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            roformData.formGroupAHdr.PART4_6_Weightage = (part4_6_Weightage == "" || part4_6_Weightage == null) ? 0 : Convert.ToDecimal(part4_6_Weightage);
                            if (roformData.ReviewingTo != roformData.AcceptanceAuthorityTo)
                            {
                                ModelState.Remove("formGroupAHdr.AcceptanceAuthorityRemarks");
                            }
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                            {
                                ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                            }
                            //roformData.formGroupAHdr.PART5_5_Weightage = Convert.ToDecimal(part5_5_Weightage);
                            //roformData.formGroupAHdr.PART4_6_Weightage = Convert.ToDecimal(part4_6_Weightage);
                            //  roformData.submittedBy = Model.SubmittedBy.AcceptanceAuthority;
                            //  roformData.formGroupAHdr.FormState = (byte)(ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        }


                        if (roformData.ReviewingTo == roformData.AcceptanceAuthorityTo && roformData.loggedInEmpID == roformData.ReviewingTo)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }

                        if (roformData.ReportingTo == roformData.ReviewingTo && roformData.ReviewingTo == roformData.AcceptanceAuthorityTo && roformData.loggedInEmpID == roformData.ReportingTo)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            }
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }

                        if (roformData.ApprovalHierarchy == 1 && roformData.ReviewingTo == roformData.loggedInEmpID)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }
                        else if (roformData.ApprovalHierarchy == 2 && roformData.ReviewingTo == roformData.ReportingTo)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            }
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }

                        if (roformData.FormID == 0)
                            roformData.FormID = (int)userDetail.AppraisalFormID;
                        if (!appraisalService.AppraisalDataExists(roformData.EmployeeID, roformData.FormID, roformData.formGroupAHdr.ReportingYr))
                        {
                            roformData.formGroupAHdr.CreatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                            roformData.formGroupAHdr.CreatedOn = DateTime.Now;
                            if (!roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROFrom", "Please select date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupA", roformData);
                            }
                            else if (!roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROTo", "Please select date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupA", roformData);
                            }
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_3B) || string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_1))
                            {

                                ModelState.AddModelError("gg", ".");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupA", roformData);
                            }
                            else
                            {

                                Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                {
                                    SenderID = userDetail.EmployeeID,
                                    ReceiverID = formNewAttributes.ReciverID,
                                    SenderDepartmentID = userDetail.DepartmentID,
                                    SenderDesignationID = userDetail.DesignationID,
                                    CreatedBy = userDetail.UserID,
                                    EmployeeID = roformData.EmployeeID,
                                    Scomments = $"Form A, Appraisal Form Submited By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                    ProcessID = (int)WorkFlowProcess.Appraisal,
                                    StatusID = (int)roformData.formGroupAHdr.FormState
                                };
                                roformData._ProcessWorkFlow = workFlow;
                                roformData.formGroupAHdr.DesignationID = (int)userDetail.DesignationID;
                                roformData.formGroupAHdr.DepartmentID = (int)userDetail.DepartmentID;
                                flag = appraisalService.InsertFormAData(roformData);
                                var redirect = ButtonType == "Save" ? 0 : 1;
                                return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = "Form submitted successfully.", redirect = redirect }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            int errorCount = 0;
                            if (roformData.EmployeeID == roformData.loggedInEmpID)
                            {

                                if (!roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue)
                                {
                                    ModelState.AddModelError("WorkedPeriodUnderROFrom", "Please select date.");
                                    errorCount++;
                                    roformData.isValid = false;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupA", roformData);
                                }
                                if (!roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                                {
                                    ModelState.AddModelError("WorkedPeriodUnderROTo", "Please select date.");
                                    errorCount++;
                                    roformData.isValid = false;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupA", roformData);
                                }
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_3B) || string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_1))
                                {

                                    ModelState.AddModelError("gg", ".");
                                    errorCount++;
                                    roformData.isValid = false;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupA", roformData);
                                }
                                if (roformData.formGroupADetail1List != null && roformData.formGroupADetail1List.Count > 0)
                                {
                                    for (int i = 0; i < roformData.formGroupADetail1List.Count; i++)
                                    {
                                        if (string.IsNullOrEmpty(roformData.formGroupADetail1List[i].Achievements))
                                        {
                                            ModelState.AddModelError($"formGroupADetail1List[{i}].Achievements", "Please enter achievements.");
                                            errorCount++;
                                        }
                                    }
                                }
                                //else
                                //{
                                if (errorCount == 0)
                                {
                                    roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                                    roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        // ReceiverID = GetWorkFlowReceiverID(roformData),
                                        ReceiverID = formNewAttributes.ReciverID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form A, Appraisal Form Submited By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.Appraisal,
                                        StatusID = (int)roformData.formGroupAHdr.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;
                                    flag = appraisalService.UpdateFormAData(roformData, "RO");

                                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                    var redirect = ButtonType == "Save" ? 0 : 1;
                                    return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg, redirect = redirect }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var formGroup1Details = roformData.formGroupADetail1List;
                                    roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    TempData.Keep("appraisal");
                                    roformData.formGroupADetail1List = formGroup1Details;
                                    roformData.isValid = errorCount == 0 ? true : false;
                                    return PartialView("_FormGroupA", roformData);
                                }
                                //}
                            }
                            else
                            {
                                //var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                                appraisalService.SetSectionValue(roformData);
                                RemoveModelError(roformData);
                                if (ModelState.IsValid)
                                {
                                    roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                                    roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        //  ReceiverID = GetWorkFlowReceiverID(roformData),
                                        ReceiverID = formNewAttributes.ReciverID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form A, Appraisal Form Updated By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.Appraisal,
                                        StatusID = (int)roformData.formGroupAHdr.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;
                                    ///====  Adding auto push workflow data ==== 
                                    roformData.FormState = roformData.formGroupAHdr.FormState;
                                    roformData.FormGroupID = roformData.formGroupAHdr.FormGroupID;
                                    AddAutoPushWorkFlow(roformData);
                                    ///=====   end  ============================

                                    flag = appraisalService.UpdateFormAData(roformData, "RO");
                                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                    var redirect = ButtonType == "Save" ? 0 : 2;
                                    return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg, redirect = redirect }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var PART5_5_Weightage = roformData.formGroupAHdr.PART5_5_Weightage;
                                    var PART5_2 = roformData.formGroupAHdr.PART5_2;
                                    roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    TempData.Keep("appraisal");
                                    roformData.formGroupAHdr.PART5_5_Weightage = PART5_5_Weightage;
                                    roformData.formGroupAHdr.PART5_2 = PART5_2;
                                    var trainingData = (Model.AppraisalForm)TempData["frmGroupTrainingData"];
                                    if (trainingData != null)
                                    {
                                        if (trainingData.formGroupATrainingDtls != null && trainingData.formGroupATrainingDtls.Count > 0)
                                            roformData.formGroupATrainingDtls = trainingData.formGroupATrainingDtls;
                                    }
                                    roformData.isValid = false;
                                    return PartialView("_FormGroupA", roformData);
                                }
                            }
                        }
                    }
                    // return PartialView("_FormGroupA", roformData);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region Form Group B
        public ActionResult _GetFormGroupB(Model.AppraisalForm appraisal)
        {
            log.Info($"AppraisalController/_GetFormGroupB");
            try
            {
                var reportingYr = appraisal.ReportingYr;// string.IsNullOrEmpty(appraisal.ReportingYr) ? DateTime.Now.GetFinancialYr() : appraisal.ReportingYr;
                //appraisal.loggedInEmpID = userDetail.EmployeeID;
                if (!appraisal.isAdmin)
                    appraisal.loggedInEmpID = userDetail.EmployeeID;
                else
                {
                    if (appraisal.AcceptanceAuthorityTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.AcceptanceAuthorityTo.Value;
                    else if (appraisal.ReviewingTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.ReviewingTo.Value;
                    else
                        appraisal.loggedInEmpID = appraisal.ReportingTo.Value;
                }

                appraisal.formGroupAHdr = appraisalService.GetFormGroupHdrDetail("FormGroupBHdr", null, appraisal.EmployeeID, reportingYr, null);
                appraisal.FormID = appraisal.formGroupAHdr.FormID;
                appraisal.EmployeeID = appraisal.formGroupAHdr.EmployeeID;
                appraisal.formGroupAHdr.ReportingYr = (appraisal.formGroupAHdr.ReportingYr != null && appraisal.formGroupAHdr.ReportingYr.Length > 0) ? appraisal.formGroupAHdr.ReportingYr : reportingYr;
                appraisal.formGroupAHdr.Part4_1_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_1_Grade;
                appraisal.formGroupAHdr.Part4_2_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_2_Grade;
                appraisal.formGroupAHdr.Part4_3_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_3_Grade;
                appraisal.formGroupAHdr.FormPart4Integrity = (Model.FormPart4Integrity)appraisal.formGroupAHdr.PART4_4_Grade;
                appraisal.formGroupADetail1List = appraisalService.GetFormGroupDetail1("FormGroupBHdr", "FormGroupBDetail1", appraisal.EmployeeID, appraisal.formGroupAHdr.FormGroupID).ToList();
                appraisal.formGroupATrainingDtls = appraisalService.GetFormTrainingDetail("FormGroupBHdr", "FormGroupTrainingDtls", appraisal.EmployeeID, appraisal.formGroupAHdr.FormGroupID, appraisal.FormID, appraisal.formGroupAHdr.ReportingYr).ToList();

                var currentYr = int.Parse(appraisal.formGroupAHdr.ReportingYr.Substring(0, 4));
                var prHeader = prService.GetPropertyReturn(currentYr, appraisal.EmployeeID);

                if (prHeader != null)
                {
                    appraisal.AnnualPropertyReturnSubmitted = prHeader.StatusID == 1 ? true : false;
                    appraisal.AnnualPropertySubmittedOn = prHeader.StatusID == 1 ? prHeader.UpdatedOn : null;
                }


                if (appraisal.formGroupATrainingDtls.Count > 0)
                {
                    var sno = 1;
                    appraisal.formGroupATrainingDtls.ForEach(x =>
                    {
                        x.sno = sno++;
                        x.FormTraining = (Model.FormPart4Training)x.TrainingID;
                    });

                }

                if (appraisal.formGroupADetail1List.Count > 0)
                {
                    var sno = 1;
                    appraisal.formGroupADetail1List.ForEach(x =>
                    {
                        x.sno = sno++;
                    });
                }
                appraisal.formGroupADetail2List = appraisalService.GetFormGroupDetail2("FormGroupBHdr", "FormGroupBDetail2", (int)userDetail.EmployeeID, appraisal.formGroupAHdr.FormGroupID).ToList();
                TempData["appraisal"] = appraisal;
                TempData.Keep("appraisal");
                appraisal.isValid = true;
                return PartialView("_FormGroupB", appraisal);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult _PostROFormBData(Model.AppraisalForm roformData, string ButtonType, FormCollection frm)
        {
            log.Info($"AppraisalController/_PostROFormBData");
            bool flag;
            try
            {
                if (ButtonType == "Add Row")
                {
                    if (roformData.formGroupADetail1List == null)
                        roformData.formGroupADetail1List = new List<Model.FormGroupDetail1>() {
                            new Model.FormGroupDetail1() { sno = 1, } };
                    else
                    {
                        if (roformData.formGroupADetail1List.Count == 1)
                            roformData.formGroupADetail1List.FirstOrDefault().sno = 1;
                        else
                        {
                            var s_no = 1;
                            roformData.formGroupADetail1List.ForEach(x =>
                            {
                                x.sno = s_no++;
                            });
                        }
                        if (roformData.formGroupADetail1List.Count < 10)

                            roformData.formGroupADetail1List.Add(new Model.FormGroupDetail1()
                            {
                                sno = roformData.formGroupADetail1List.Count + 1
                            });
                    }
                    TempData["frmGroupPart2Data"] = roformData;
                    return Json(new { part = 1, htmlData = ConvertViewToString("_FormGroupPart2GridView", roformData) }, JsonRequestBehavior.AllowGet);

                }
                else if (ButtonType == "Add New Training Row")
                {
                    if (roformData.formGroupATrainingDtls == null)
                        roformData.formGroupATrainingDtls = new List<Model.FormTrainingDtls>()
                        { new Model.FormTrainingDtls() { sno = 1  } };
                    else
                    {
                        if (roformData.formGroupATrainingDtls.Count == 1)
                            roformData.formGroupATrainingDtls.FirstOrDefault().sno = 1;

                        if (roformData.formGroupATrainingDtls.Count < 3)
                            roformData.formGroupATrainingDtls.Add(new Model.FormTrainingDtls()
                            {
                                sno = roformData.formGroupATrainingDtls.Count + 1
                            });
                    }
                    TempData["frmGroupTrainingData"] = roformData;
                    return Json(new { part = 2, htmlData = ConvertViewToString("_FormGroupTrainingGridView", roformData) }, JsonRequestBehavior.AllowGet);

                }
                else
                {

                    if (!roformData.ReportingTo.HasValue || roformData.ReportingTo.Value == 0)
                    {
                        var errormsg = "You can not apply for appraisal right now because either your Reporting or Reviewing Manager is not set.";
                        return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupB", roformData) }, JsonRequestBehavior.AllowGet);
                    }

                    if (ButtonType == "Save")
                    {
                        var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                        roformData.submittedBy = formNewAttributes.SubmittedBy;
                        roformData.formGroupAHdr.FormState = formNewAttributes.FormState;

                        if (!appraisalService.AppraisalDataExists(roformData.EmployeeID, roformData.FormID, roformData.formGroupAHdr.ReportingYr))
                        {
                            roformData.formGroupAHdr.CreatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                            roformData.formGroupAHdr.CreatedOn = DateTime.Now;
                            roformData.formGroupAHdr.DesignationID = (int)userDetail.DesignationID;
                            roformData.formGroupAHdr.DepartmentID = (int)userDetail.DepartmentID;
                            flag = appraisalService.InsertFormBData(roformData);
                            var redirect = ButtonType == "Save" ? 0 : 1;
                            return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = "Form Saved successfully.", redirect = redirect }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                            flag = appraisalService.UpdateFormBData(roformData);
                            var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                            var redirect = ButtonType == "Save" ? 0 : 1;
                            return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg, redirect = redirect }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                        var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                        roformData.submittedBy = formNewAttributes.SubmittedBy;
                        roformData.formGroupAHdr.FormState = formNewAttributes.FormState;

                        if (roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue && roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                        {
                            if (roformData.formGroupAHdr.WorkedPeriodUnderROTo.Value < roformData.formGroupAHdr.WorkedPeriodUnderROFrom.Value)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROFrom", "From date should be always less than To date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupB", roformData);
                            }
                        }

                        //if (!appraisalService.IsTrainingSubmitted(roformData.EmployeeID, roformData.formGroupAHdr.ReportingYr))
                        //{
                        //    var errormsg = "You can not apply for appraisal right now, please first apply for skill assessment.";
                        //    return Json(new { isTraining = 1, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupB", roformData) }, JsonRequestBehavior.AllowGet);
                        //}
                        //if (roformData.EmployeeID == roformData.loggedInEmpID)
                        //{
                        //    roformData.submittedBy = Model.SubmittedBy.Employee;
                        //    roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByEmployee : (int)Model.AppraisalFormState.SubmitedByEmployee);

                        //}
                        // else
                        if (roformData.ReportingTo == roformData.loggedInEmpID)
                        {
                            // roformData.submittedBy = Model.SubmittedBy.ReportingOfficer;
                            //  roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReporting : (int)Model.AppraisalFormState.SubmitedByReporting);

                            if (roformData.ReportingTo != roformData.ReviewingTo)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_2");
                                ModelState.Remove("formGroupAHdr.PART5_3");
                                ModelState.Remove("formGroupAHdr.PART5_4");
                                ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                                ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                                ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                                ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                                ModelState.Remove("formGroupAHdr.Remarks");
                                ModelState.Remove("formGroupADetail2List[0].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[1].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[2].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[3].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[4].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[5].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[6].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[7].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[8].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[9].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[10].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[11].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[12].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[13].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[14].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[15].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[16].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[17].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[18].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[19].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[20].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[21].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupAHdr.PART5_1");
                            }
                            else
                            {
                            }
                            ModelState.Remove("formGroupAHdr.PART5_2");
                            ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            if (roformData.ReportingTo == roformData.ReviewingTo && roformData.ReviewingTo == roformData.AcceptanceAuthorityTo)
                            { }
                            else
                                ModelState.Remove("formGroupAHdr.Remarks");
                        }

                        else if (roformData.ReviewingTo == roformData.loggedInEmpID)
                        {
                            var part5_5_Weightage = frm["formGroupAHdr.PART5_5_Weightage"].ToString();
                            var part4_6_Weightage = frm["formGroupAHdr.PART4_6_Weightage"].ToString();

                            if (part5_5_Weightage == "NaN")
                                part5_5_Weightage = "";
                            if (part4_6_Weightage == "NaN")
                                part4_6_Weightage = "";
                            roformData.formGroupAHdr.PART5_5_Weightage = (part5_5_Weightage == "" || part5_5_Weightage == null) ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            roformData.formGroupAHdr.PART4_6_Weightage = (part4_6_Weightage == "" || part4_6_Weightage == null) ? 0 : Convert.ToDecimal(part4_6_Weightage);



                            //  roformData.formGroupAHdr.PART5_5_Weightage = part5_5_Weightage == "" ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            //   roformData.formGroupAHdr.PART4_6_Weightage = part4_6_Weightage == "" ? 0 : Convert.ToDecimal(part4_6_Weightage);

                            //roformData.formGroupAHdr.PART5_5_Weightage = Convert.ToDecimal(part5_5_Weightage);
                            //roformData.formGroupAHdr.PART4_6_Weightage = Convert.ToDecimal(part4_6_Weightage);
                            ModelState.Remove("formGroupAHdr.PART2_3B");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART4_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.Remarks");

                            ModelState.Remove("formGroupAHdr.PART5_4");

                            //  roformData.submittedBy = Model.SubmittedBy.ReviewingOfficer;
                            //  roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                        }
                        else if (roformData.AcceptanceAuthorityTo == roformData.loggedInEmpID)
                        {
                            var part5_5_Weightage = frm["formGroupAHdr.PART5_5_Weightage"].ToString();
                            var part4_6_Weightage = frm["formGroupAHdr.PART4_6_Weightage"].ToString();

                            if (part5_5_Weightage == "NaN")
                                part5_5_Weightage = "";
                            if (part4_6_Weightage == "NaN")
                                part4_6_Weightage = "";
                            roformData.formGroupAHdr.PART5_5_Weightage = (part5_5_Weightage == "" || part5_5_Weightage == null) ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            roformData.formGroupAHdr.PART4_6_Weightage = (part4_6_Weightage == "" || part4_6_Weightage == null) ? 0 : Convert.ToDecimal(part4_6_Weightage);

                            //roformData.formGroupAHdr.PART5_5_Weightage = part5_5_Weightage == "" ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            //roformData.formGroupAHdr.PART4_6_Weightage = part4_6_Weightage == "" ? 0 : Convert.ToDecimal(part4_6_Weightage);

                            //roformData.formGroupAHdr.PART5_5_Weightage = Convert.ToDecimal(part5_5_Weightage);
                            //roformData.formGroupAHdr.PART4_6_Weightage = Convert.ToDecimal(part4_6_Weightage);
                            ModelState.Remove("formGroupAHdr.PART2_3B");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");
                            if (roformData.formGroupAHdr.Remarks == null || roformData.formGroupAHdr.Remarks == "")
                            {
                                ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                            }

                            //  roformData.submittedBy = Model.SubmittedBy.AcceptanceAuthority;
                            //  roformData.formGroupAHdr.FormState = (byte)(ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        }

                        if (roformData.ReviewingTo == roformData.AcceptanceAuthorityTo && roformData.loggedInEmpID == roformData.ReviewingTo)
                        {
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                            {
                                ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                            }

                            if (roformData.formGroupAHdr.PART5_2 == true)
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }

                        if (roformData.ReportingTo == roformData.ReviewingTo && roformData.ReviewingTo == roformData.AcceptanceAuthorityTo && roformData.loggedInEmpID == roformData.ReportingTo)
                        {
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                            {
                                ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                            }

                            if (roformData.formGroupAHdr.PART5_2 == true)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_3");
                                //if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_4))
                                //    ModelState.AddModelError("formGroupAHdr.PART5_4", "Please enter remark.");
                            }
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                                //if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_4))
                                //    ModelState.AddModelError("formGroupAHdr.PART5_4", "Please enter remark.");
                            }
                        }

                        if (roformData.ApprovalHierarchy == 1 && roformData.ReviewingTo == roformData.loggedInEmpID)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }
                        else if (roformData.ApprovalHierarchy == 2 && roformData.ReviewingTo == roformData.ReportingTo)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_3");
                                //if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_4))
                                //    ModelState.AddModelError("formGroupAHdr.PART5_4", "Please enter remark.");
                            }
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                                //if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_4))
                                //    ModelState.AddModelError("formGroupAHdr.PART5_4", "Please enter remark.");
                            }
                        }


                        if (roformData.FormID == 0)
                            roformData.FormID = (int)userDetail.AppraisalFormID;

                        if (!appraisalService.AppraisalDataExists(roformData.EmployeeID, roformData.FormID, roformData.formGroupAHdr.ReportingYr))
                        {
                            roformData.formGroupAHdr.CreatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                            roformData.formGroupAHdr.CreatedOn = DateTime.Now;
                            if (roformData.formGroupAHdr.WorkedPeriodUnderROFrom == default(DateTime))
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROFrom", "Please select date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupB", roformData);
                            }
                            else if (roformData.formGroupAHdr.WorkedPeriodUnderROTo == default(DateTime))
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROTo", "Please select date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupB", roformData);
                            }
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_3B) || string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_1))
                            {

                                ModelState.AddModelError("gg", ".");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupB", roformData);
                            }
                            else
                            {
                                Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                {
                                    SenderID = userDetail.EmployeeID,
                                    ReceiverID = GetWorkFlowReceiverID(roformData),
                                    SenderDepartmentID = userDetail.DepartmentID,
                                    SenderDesignationID = userDetail.DesignationID,
                                    CreatedBy = userDetail.UserID,
                                    EmployeeID = roformData.EmployeeID,
                                    Scomments = $"Form B, Appraisal Form Submited By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                    ProcessID = (int)WorkFlowProcess.Appraisal,
                                    StatusID = (int)roformData.formGroupAHdr.FormState
                                };
                                roformData._ProcessWorkFlow = workFlow;

                                roformData.formGroupAHdr.DesignationID = (int)userDetail.DesignationID;
                                roformData.formGroupAHdr.DepartmentID = (int)userDetail.DepartmentID;
                                flag = appraisalService.InsertFormBData(roformData);
                                var redirect = ButtonType == "Save" ? 0 : 1;
                                return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = "Form submitted successfully.", redirect = redirect }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (roformData.EmployeeID == roformData.loggedInEmpID)
                            {
                                int errorCount = 0;
                                if (roformData.formGroupAHdr.WorkedPeriodUnderROFrom == default(DateTime) || roformData.formGroupAHdr.WorkedPeriodUnderROFrom == null)
                                {
                                    ModelState.AddModelError("WorkedPeriodUnderROFrom", "Please select date.");
                                    errorCount++;
                                    roformData.isValid = false;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupB", roformData);
                                }
                                if (roformData.formGroupAHdr.WorkedPeriodUnderROTo == default(DateTime) || roformData.formGroupAHdr.WorkedPeriodUnderROTo == null)
                                {
                                    ModelState.AddModelError("WorkedPeriodUnderROTo", "Please select date.");
                                    errorCount++;
                                    roformData.isValid = false;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupB", roformData);
                                }
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_3B) || string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_1))
                                {

                                    ModelState.AddModelError("gg", ".");
                                    errorCount++;
                                    roformData.isValid = false;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupB", roformData);
                                }
                                //else
                                //{
                                if (errorCount == 0)
                                {
                                    roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                                    roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        //ReceiverID = GetWorkFlowReceiverID(roformData),
                                        ReceiverID = formNewAttributes.ReciverID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form B, Appraisal Form Submited By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.Appraisal,
                                        StatusID = (int)roformData.formGroupAHdr.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;
                                    flag = appraisalService.UpdateFormBData(roformData);
                                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                    var redirect = ButtonType == "Save" ? 0 : 1;
                                    return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg, redirect = redirect }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var formGroup1Details = roformData.formGroupADetail1List;
                                    roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    TempData.Keep("appraisal");
                                    roformData.formGroupADetail1List = formGroup1Details;
                                    roformData.isValid = errorCount == 0 ? true : false;
                                    return PartialView("_FormGroupB", roformData);
                                }
                                //}
                            }
                            else
                            {
                                //var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                                appraisalService.SetSectionValue(roformData);
                                RemoveModelError(roformData);
                                if (roformData.loggedInEmpID != roformData.AcceptanceAuthorityTo)
                                {
                                    ModelState.Remove("formGroupAHdr.Remarks");
                                }

                                if (ModelState.IsValid)
                                {
                                    roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                                    roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        //ReceiverID = GetWorkFlowReceiverID(roformData),
                                        ReceiverID = formNewAttributes.ReciverID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form B, Appraisal Form Updated By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.Appraisal,
                                        StatusID = (int)roformData.formGroupAHdr.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;

                                    ///====  Adding auto push workflow data ====
                                    roformData.FormState = roformData.formGroupAHdr.FormState;
                                    roformData.FormGroupID = roformData.formGroupAHdr.FormGroupID;
                                    AddAutoPushWorkFlow(roformData);
                                    ///=====   end  ============================

                                    flag = appraisalService.UpdateFormBData(roformData);
                                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                    var redirect = ButtonType == "Save" ? 0 : 2;
                                    return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg, redirect = redirect }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var PART5_5_Weightage = roformData.formGroupAHdr.PART5_5_Weightage;
                                    var PART5_2 = roformData.formGroupAHdr.PART5_2;
                                    roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    TempData.Keep("appraisal");
                                    roformData.formGroupAHdr.PART5_5_Weightage = PART5_5_Weightage;
                                    roformData.formGroupAHdr.PART5_2 = PART5_2;

                                    var trainingData = (Model.AppraisalForm)TempData["frmGroupTrainingData"];
                                    if (trainingData != null)
                                    {
                                        if (trainingData.formGroupATrainingDtls != null && trainingData.formGroupATrainingDtls.Count > 0)
                                            roformData.formGroupATrainingDtls = trainingData.formGroupATrainingDtls;
                                    }
                                    roformData.isValid = false;
                                    return PartialView("_FormGroupB", roformData);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region Form Group C

        public ActionResult _GetFormGroupC(Model.AppraisalForm appraisal)
        {
            log.Info($"AppraisalController/_GetFormGroupC");
            try
            {
                var reportingYr = appraisal.ReportingYr;// string.IsNullOrEmpty(appraisal.ReportingYr) ? DateTime.Now.GetFinancialYr() : appraisal.ReportingYr;
                //appraisal.loggedInEmpID = userDetail.EmployeeID;
                if (!appraisal.isAdmin)
                    appraisal.loggedInEmpID = userDetail.EmployeeID;
                else
                {
                    if (appraisal.AcceptanceAuthorityTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.AcceptanceAuthorityTo.Value;
                    else if (appraisal.ReviewingTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.ReviewingTo.Value;
                    else
                        appraisal.loggedInEmpID = appraisal.ReportingTo.Value;
                }
                appraisal.formGroupAHdr = appraisalService.GetFormGroupHdrDetail("FormGroupCHdr", null, appraisal.EmployeeID, reportingYr, null);
                appraisal.FormID = appraisal.formGroupAHdr.FormID;
                appraisal.EmployeeID = appraisal.formGroupAHdr.EmployeeID;
                appraisal.formGroupAHdr.ReportingYr = (appraisal.formGroupAHdr.ReportingYr != null && appraisal.formGroupAHdr.ReportingYr.Length > 0) ? appraisal.formGroupAHdr.ReportingYr : reportingYr;
                appraisal.formGroupAHdr.Part4_1_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_1_Grade;
                appraisal.formGroupAHdr.Part4_2_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_2_Grade;
                appraisal.formGroupAHdr.Part4_3_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_3_Grade;
                appraisal.formGroupAHdr.FormPart4Integrity = (Model.FormPart4Integrity)appraisal.formGroupAHdr.PART4_4_Grade;
                appraisal.formGroupADetail1List = appraisalService.GetFormGroupDetail1("FormGroupCHdr", "FormGroupCDetail1", appraisal.EmployeeID, appraisal.formGroupAHdr.FormGroupID).ToList();
                appraisal.formGroupATrainingDtls = appraisalService.GetFormTrainingDetail("FormGroupCHdr", "FormGroupTrainingDtls", appraisal.EmployeeID, appraisal.formGroupAHdr.FormGroupID, appraisal.FormID, appraisal.formGroupAHdr.ReportingYr).ToList();
                if (appraisal.formGroupATrainingDtls.Count > 0)
                {
                    var sno = 1;
                    appraisal.formGroupATrainingDtls.ForEach(x =>
                    {
                        x.sno = sno++;
                        x.FormTraining = (Model.FormPart4Training)x.TrainingID;
                    });
                }

                if (appraisal.formGroupADetail1List.Count > 0)
                {
                    var sno = 1;
                    appraisal.formGroupADetail1List.ForEach(x =>
                    {
                        x.sno = sno++;
                    });
                }

                var currentYr = int.Parse(appraisal.formGroupAHdr.ReportingYr.Substring(0, 4));
                var prHeader = prService.GetPropertyReturn(currentYr, appraisal.EmployeeID);

                if (prHeader != null)
                {
                    appraisal.AnnualPropertyReturnSubmitted = prHeader.StatusID == 1 ? true : false;
                    appraisal.AnnualPropertySubmittedOn = prHeader.StatusID == 1 ? prHeader.UpdatedOn : null;
                }

                appraisal.formGroupADetail2List = appraisalService.GetFormGroupDetail2("FormGroupCHdr", "FormGroupCDetail2", (int)userDetail.EmployeeID, appraisal.formGroupAHdr.FormGroupID).ToList();
                TempData["appraisal"] = appraisal;
                TempData.Keep("appraisal");
                appraisal.isValid = true;
                return PartialView("_FormGroupC", appraisal);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostROFormCData(Model.AppraisalForm roformData, string ButtonType, FormCollection frm)
        {
            log.Info($"AppraisalController/_PostROFormData");
            bool flag;
            try
            {

                if (ButtonType == "Add Row")
                {
                    if (roformData.formGroupADetail1List == null)
                        roformData.formGroupADetail1List = new List<Model.FormGroupDetail1>() {
                            new Model.FormGroupDetail1() { sno = 1, } };
                    else
                    {
                        if (roformData.formGroupADetail1List.Count == 1)
                            roformData.formGroupADetail1List.FirstOrDefault().sno = 1;
                        else
                        {
                            var s_no = 1;
                            roformData.formGroupADetail1List.ForEach(x =>
                            {
                                x.sno = s_no++;
                            });

                        }
                        if (roformData.formGroupADetail1List.Count < 10)
                            roformData.formGroupADetail1List.Add(new Model.FormGroupDetail1()
                            {
                                sno = roformData.formGroupADetail1List.Count + 1
                            });
                    }
                    TempData["frmGroupPart2Data"] = roformData;
                    return Json(new { part = 1, htmlData = ConvertViewToString("_FormGroupPart2GridView", roformData) }, JsonRequestBehavior.AllowGet);

                }
                else if (ButtonType == "Add New Training Row")
                {
                    if (roformData.formGroupATrainingDtls == null)
                        roformData.formGroupATrainingDtls = new List<Model.FormTrainingDtls>()
                        { new Model.FormTrainingDtls() { sno = 1  } };
                    else
                    {
                        if (roformData.formGroupATrainingDtls.Count == 1)
                            roformData.formGroupATrainingDtls.FirstOrDefault().sno = 1;

                        if (roformData.formGroupATrainingDtls.Count < 3)
                            roformData.formGroupATrainingDtls.Add(new Model.FormTrainingDtls()
                            {
                                sno = roformData.formGroupATrainingDtls.Count + 1
                            });
                    }
                    TempData["frmGroupTrainingData"] = roformData;
                    return Json(new { part = 2, htmlData = ConvertViewToString("_FormGroupTrainingGridView", roformData) }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    if (!roformData.ReportingTo.HasValue || roformData.ReportingTo.Value == 0)
                    {
                        var errormsg = "You can not apply for appraisal right now because either your Reporting or Reviewing Manager is not set.";
                        return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupC", roformData) }, JsonRequestBehavior.AllowGet);
                    }

                    if (ButtonType == "Save")
                    {
                        var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                        roformData.submittedBy = formNewAttributes.SubmittedBy;
                        roformData.formGroupAHdr.FormState = formNewAttributes.FormState;

                        if (!appraisalService.AppraisalDataExists(roformData.EmployeeID, roformData.FormID, roformData.formGroupAHdr.ReportingYr))
                        {
                            roformData.formGroupAHdr.CreatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                            roformData.formGroupAHdr.CreatedOn = DateTime.Now;
                            roformData.formGroupAHdr.DesignationID = (int)userDetail.DesignationID;
                            roformData.formGroupAHdr.DepartmentID = (int)userDetail.DepartmentID;
                            flag = appraisalService.InsertFormCData(roformData);
                            var redirect = ButtonType == "Save" ? 0 : 1;
                            return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = "Form Saved successfully.", redirect = redirect }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                            flag = appraisalService.UpdateFormCData(roformData);
                            var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                            var redirect = ButtonType == "Save" ? 0 : 2;
                            return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg, redirect = redirect }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                        if (roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue && roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                        {
                            if (roformData.formGroupAHdr.WorkedPeriodUnderROTo.Value < roformData.formGroupAHdr.WorkedPeriodUnderROFrom.Value)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROFrom", "From date should be always less than To date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupC", roformData);
                            }
                        }

                        //if (!appraisalService.IsTrainingSubmitted(roformData.EmployeeID, roformData.formGroupAHdr.ReportingYr))
                        //{
                        //    var errormsg = "You can not apply for appraisal right now, please first apply for skill assessment.";
                        //    return Json(new { isTraining = 1, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupC", roformData) }, JsonRequestBehavior.AllowGet);
                        //}

                        //if (roformData.EmployeeID == roformData.loggedInEmpID)
                        //{
                        //    roformData.submittedBy = Model.SubmittedBy.Employee;
                        //    roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByEmployee : (int)Model.AppraisalFormState.SubmitedByEmployee);

                        //}
                        //  else 
                        if (roformData.ReportingTo == roformData.loggedInEmpID)
                        {

                            //roformData.submittedBy = Model.SubmittedBy.ReportingOfficer;
                            //   roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReporting : (int)Model.AppraisalFormState.SubmitedByReporting);

                            if (roformData.ReportingTo != roformData.ReviewingTo)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_1");
                                ModelState.Remove("formGroupADetail2List[0].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[1].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[2].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[3].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[4].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[5].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[6].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[7].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[8].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[9].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[10].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[11].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[12].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[13].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[14].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[15].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[16].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[17].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[18].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[19].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[20].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[21].ReviewingAuthorityWeightage");
                            }
                            else
                            {
                            }
                            ModelState.Remove("formGroupAHdr.PART5_2");
                            ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");


                            if (roformData.ReportingTo == roformData.ReviewingTo && roformData.ReviewingTo == roformData.AcceptanceAuthorityTo)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                                {
                                    ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                                }
                            }
                            else
                                ModelState.Remove("formGroupAHdr.Remarks");

                        }

                        else if (roformData.ReviewingTo == roformData.loggedInEmpID)
                        {
                            var part5_5_Weightage = frm["formGroupAHdr.PART5_5_Weightage"].ToString();
                            var part4_6_Weightage = frm["formGroupAHdr.PART4_6_Weightage"].ToString();

                            if (part5_5_Weightage == "NaN")
                                part5_5_Weightage = "";
                            if (part4_6_Weightage == "NaN")
                                part4_6_Weightage = "";
                            roformData.formGroupAHdr.PART5_5_Weightage = (part5_5_Weightage == "" || part5_5_Weightage == null) ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            roformData.formGroupAHdr.PART4_6_Weightage = (part4_6_Weightage == "" || part4_6_Weightage == null) ? 0 : Convert.ToDecimal(part4_6_Weightage);

                            //roformData.formGroupAHdr.PART5_5_Weightage = part5_5_Weightage == "" ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            //roformData.formGroupAHdr.PART4_6_Weightage = part4_6_Weightage == "" ? 0 : Convert.ToDecimal(part4_6_Weightage);

                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART4_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART2_3B");
                            //ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");

                            if (roformData.ReviewingTo != roformData.AcceptanceAuthorityTo)
                            {
                                ModelState.Remove("formGroupAHdr.AcceptanceAuthorityRemarks");
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                                {
                                    ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                                }
                            }

                            //  roformData.submittedBy = Model.SubmittedBy.ReviewingOfficer;
                            //  roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                        }
                        else if (roformData.AcceptanceAuthorityTo == roformData.loggedInEmpID)
                        {
                            var part5_5_Weightage = frm["formGroupAHdr.PART5_5_Weightage"].ToString();
                            var part4_6_Weightage = frm["formGroupAHdr.PART4_6_Weightage"].ToString();

                            //roformData.formGroupAHdr.PART5_5_Weightage = part5_5_Weightage == "" ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            //roformData.formGroupAHdr.PART4_6_Weightage = part4_6_Weightage == "" ? 0 : Convert.ToDecimal(part4_6_Weightage);

                            if (part5_5_Weightage == "NaN")
                                part5_5_Weightage = "";
                            if (part4_6_Weightage == "NaN")
                                part4_6_Weightage = "";

                            roformData.formGroupAHdr.PART5_5_Weightage = (part5_5_Weightage == "" || part5_5_Weightage == null) ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            roformData.formGroupAHdr.PART4_6_Weightage = (part4_6_Weightage == "" || part4_6_Weightage == null) ? 0 : Convert.ToDecimal(part4_6_Weightage);

                            //roformData.formGroupAHdr.PART5_5_Weightage = Convert.ToDecimal(part5_5_Weightage);
                            //roformData.formGroupAHdr.PART4_6_Weightage = Convert.ToDecimal(part4_6_Weightage);
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");
                            ModelState.Remove("formGroupAHdr.PART2_3B");

                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                            {
                                ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                            }
                            //  roformData.submittedBy = Model.SubmittedBy.AcceptanceAuthority;
                            // roformData.formGroupAHdr.FormState = (byte)(ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        }

                        if (roformData.ReviewingTo == roformData.AcceptanceAuthorityTo && roformData.loggedInEmpID == roformData.ReviewingTo)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }

                        if (roformData.ReportingTo == roformData.ReviewingTo && roformData.ReviewingTo == roformData.AcceptanceAuthorityTo && roformData.loggedInEmpID == roformData.ReportingTo)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            }
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }

                        if (roformData.ApprovalHierarchy == 1 && roformData.ReviewingTo == roformData.loggedInEmpID)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }
                        else if (roformData.ApprovalHierarchy == 2 && roformData.ReviewingTo == roformData.ReportingTo)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            }
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }


                        if (roformData.FormID == 0)
                            roformData.FormID = (int)userDetail.AppraisalFormID;

                        if (!appraisalService.AppraisalDataExists(roformData.EmployeeID, roformData.FormID, roformData.formGroupAHdr.ReportingYr))
                        {
                            roformData.formGroupAHdr.CreatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                            roformData.formGroupAHdr.CreatedOn = DateTime.Now;
                            if (!roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROFrom", "Please select date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupC", roformData);
                            }
                            else if (!roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROTo", "Please select date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupC", roformData);
                            }
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_3B) || string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_1))
                            {

                                ModelState.AddModelError("gg", ".");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupC", roformData);
                            }
                            else
                            {
                                var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                                roformData.submittedBy = formNewAttributes.SubmittedBy;
                                roformData.formGroupAHdr.FormState = formNewAttributes.FormState;
                                Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                {
                                    SenderID = userDetail.EmployeeID,
                                    //ReceiverID = GetWorkFlowReceiverID(roformData),
                                    ReceiverID = formNewAttributes.ReciverID,
                                    SenderDepartmentID = userDetail.DepartmentID,
                                    SenderDesignationID = userDetail.DesignationID,
                                    CreatedBy = userDetail.UserID,
                                    EmployeeID = roformData.EmployeeID,
                                    Scomments = $"Form C, Appraisal Form Submited By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                    ProcessID = (int)WorkFlowProcess.Appraisal,
                                    StatusID = (int)roformData.formGroupAHdr.FormState
                                };
                                roformData._ProcessWorkFlow = workFlow;

                                roformData.formGroupAHdr.DesignationID = (int)userDetail.DesignationID;
                                roformData.formGroupAHdr.DepartmentID = (int)userDetail.DepartmentID;
                                flag = appraisalService.InsertFormCData(roformData);
                                var redirect = ButtonType == "Save" ? 0 : 1;
                                return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = "Form submitted successfully.",redirect=redirect }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (roformData.EmployeeID == roformData.loggedInEmpID)
                            {
                                int errorCount = 0;
                                if (!roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue)
                                {
                                    ModelState.AddModelError("WorkedPeriodUnderROFrom", "Please select date.");
                                    errorCount++;
                                    roformData.isValid = false;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupC", roformData);
                                }
                                if (!roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                                {
                                    ModelState.AddModelError("WorkedPeriodUnderROTo", "Please select date.");
                                    errorCount++;
                                    roformData.isValid = false;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupC", roformData);
                                }
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_3B) || string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_1))
                                {
                                    ModelState.AddModelError("gg", ".");
                                    errorCount++;
                                    roformData.isValid = false;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupC", roformData);
                                }
                                //else
                                //{
                                if (errorCount == 0)
                                {
                                    var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                                    roformData.submittedBy = formNewAttributes.SubmittedBy;
                                    roformData.formGroupAHdr.FormState = formNewAttributes.FormState;
                                    roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                                    roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        //ReceiverID = GetWorkFlowReceiverID(roformData),
                                        ReceiverID = formNewAttributes.ReciverID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form C, Appraisal Form Submited By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.Appraisal,
                                        StatusID = (int)roformData.formGroupAHdr.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;

                                    flag = appraisalService.UpdateFormCData(roformData);
                                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                    var redirect = ButtonType == "Save" ? 0 : 1;
                                    return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg ,redirect=redirect}, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var formGroup1Details = roformData.formGroupADetail1List;
                                    roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    TempData.Keep("appraisal");
                                    roformData.formGroupADetail1List = formGroup1Details;
                                    roformData.isValid = errorCount == 0 ? true : false;
                                    return PartialView("_FormGroupC", roformData);
                                }
                                //}
                            }
                            else
                            {
                                //var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                                appraisalService.SetSectionValue(roformData);
                                RemoveModelError(roformData);

                                if (ModelState.IsValid)
                                {
                                    var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                                    roformData.submittedBy = formNewAttributes.SubmittedBy;
                                    roformData.formGroupAHdr.FormState = formNewAttributes.FormState;
                                    roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                                    roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        //ReceiverID = GetWorkFlowReceiverID(roformData),
                                        ReceiverID = formNewAttributes.ReciverID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form C, Appraisal Form Updated By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.Appraisal,
                                        StatusID = (int)roformData.formGroupAHdr.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;

                                    ///====  Adding auto push workflow data ==== 
                                    roformData.FormState = roformData.formGroupAHdr.FormState;
                                    roformData.FormGroupID = roformData.formGroupAHdr.FormGroupID;
                                    AddAutoPushWorkFlow(roformData);
                                    ///=====   end  ============================

                                    flag = appraisalService.UpdateFormCData(roformData);
                                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                    var redirect = ButtonType == "Save" ? 0 : 2;
                                    return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var PART5_5_Weightage = roformData.formGroupAHdr.PART5_5_Weightage;
                                    var PART5_2 = roformData.formGroupAHdr.PART5_2;
                                    roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    TempData.Keep("appraisal");
                                    roformData.formGroupAHdr.PART5_5_Weightage = PART5_5_Weightage;
                                    roformData.formGroupAHdr.PART5_2 = PART5_2;
                                    var trainingData = (Model.AppraisalForm)TempData["frmGroupTrainingData"];
                                    if (trainingData != null)
                                    {
                                        if (trainingData.formGroupATrainingDtls != null && trainingData.formGroupATrainingDtls.Count > 0)
                                            roformData.formGroupATrainingDtls = trainingData.formGroupATrainingDtls;
                                    }
                                    roformData.isValid = false;
                                    return PartialView("_FormGroupC", roformData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region FORM GROUP D

        public ActionResult _GetFormGroupD(Model.AppraisalForm appraisal)
        {
            log.Info($"AppraisalController/_GetFormGroupD");
            try
            {
                var reportingYr = appraisal.ReportingYr;// string.IsNullOrEmpty(appraisal.ReportingYr) ? DateTime.Now.GetFinancialYr() : appraisal.ReportingYr;
                //appraisal.loggedInEmpID = userDetail.EmployeeID;
                if (!appraisal.isAdmin)
                    appraisal.loggedInEmpID = userDetail.EmployeeID;
                else
                {
                    if (appraisal.AcceptanceAuthorityTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.AcceptanceAuthorityTo.Value;
                    else if (appraisal.ReviewingTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.ReviewingTo.Value;
                    else
                        appraisal.loggedInEmpID = appraisal.ReportingTo.Value;
                }

                appraisal.formGroupDHdr = appraisalService.GetFormGroupDHdrDetail("FormGroupDHdr", null, appraisal.EmployeeID, reportingYr, null);
                appraisal.FormID = appraisal?.formGroupDHdr?.FormID > 0 ? appraisal.formGroupDHdr.FormID : appraisal.FormID;
                appraisal.EmployeeID = appraisal.formGroupDHdr?.EmployeeID > 0 ? appraisal.formGroupDHdr.EmployeeID : appraisal.EmployeeID;


                appraisal.formGroupDHdr.Part4_1_Gr = (Model.FormGrade)(appraisal.formGroupDHdr.PART4_1_Grade ?? 0);
                //  appraisal.formGroupDHdr.Part4_2_Gr = (Model.FormGrade)appraisal.formGroupDHdr.Part4_2_;
                appraisal.formGroupDHdr.Part4_3_Gr = (Model.FormGrade)(appraisal.formGroupDHdr.PART4_3_Grade ?? 0);

                appraisal.formGroupDHdr.FormPart4Integrity = (Model.FormPart4Integrity)(appraisal.formGroupDHdr.PART4_4_Grade ?? 0);

                appraisal.formGroupATrainingDtls = appraisalService.GetFormTrainingDetail("FormGroupDHdr", "FormGroupTrainingDtls", appraisal.EmployeeID, appraisal.formGroupDHdr.FormGroupID, appraisal.FormID, appraisal.formGroupDHdr.ReportingYr).ToList();

                if (appraisal.formGroupDHdr != null)

                    appraisal.formGroupDHdr.ReportingYr =
                     appraisal.formGroupDHdr != null && !string.IsNullOrEmpty(appraisal.formGroupDHdr.ReportingYr) ? appraisal.formGroupDHdr.ReportingYr : reportingYr;

                else
                    appraisal.formGroupDHdr = new Model.FormGroupDHdr() { ReportingYr = reportingYr };


                var currentYr = int.Parse(appraisal.formGroupDHdr.ReportingYr.Substring(0, 4));
                var prHeader = prService.GetPropertyReturn(currentYr, appraisal.EmployeeID);

                if (prHeader != null)
                {
                    appraisal.AnnualPropertyReturnSubmitted = prHeader.StatusID == 1 ? true : false;
                    appraisal.AnnualPropertySubmittedOn = prHeader.StatusID == 1 ? prHeader.UpdatedOn : null;
                }


                if (appraisal.formGroupATrainingDtls.Count > 0)
                {
                    var sno = 1;
                    appraisal.formGroupATrainingDtls.ForEach(x =>
                    {
                        x.sno = sno++;
                        x.FormTraining = (Model.FormPart4Training)x.TrainingID;
                    });
                }
                TempData["appraisal"] = appraisal;
                TempData.Keep("appraisal");
                appraisal.isValid = true;
                return PartialView("_FormGroupD", appraisal);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostFormDData(Model.AppraisalForm formData, string ButtonType)
        {
            log.Info($"AppraisalController/_PostFormDData");
            try
            {
                bool flag;

                if (ButtonType == "Add New Training Row")
                {
                    if (formData.formGroupATrainingDtls == null)
                        formData.formGroupATrainingDtls = new List<Model.FormTrainingDtls>()
                        { new Model.FormTrainingDtls() { sno = 1  } };
                    else
                    {
                        if (formData.formGroupATrainingDtls.Count == 1)
                            formData.formGroupATrainingDtls.FirstOrDefault().sno = 1;

                        if (formData.formGroupATrainingDtls.Count < 3)
                            formData.formGroupATrainingDtls.Add(new Model.FormTrainingDtls()
                            {
                                sno = formData.formGroupATrainingDtls.Count + 1
                            });
                    }
                    TempData["frmGroupTrainingData"] = formData;
                    return Json(new { part = 2, htmlData = ConvertViewToString("_FormGroupTrainingGridView", formData) }, JsonRequestBehavior.AllowGet);

                }

                else
                {
                    if (!formData.ReportingTo.HasValue || formData.ReportingTo.Value == 0)
                    {
                        var errormsg = "You can not apply for appraisal right now because either your Reporting or Reviewing Manager is not set.";
                        return Json(new { formState = formData.formGroupDHdr.FormState, part = 0, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupD", formData) }, JsonRequestBehavior.AllowGet);
                    }

                    if (ButtonType == "Save")
                    {
                        var formNewAttributes = GetFormAttributes(formData, ButtonType);
                        formData.submittedBy = formNewAttributes.SubmittedBy;
                        formData.formGroupDHdr.FormState = formNewAttributes.FormState;

                        if (formData.formGroupDHdr.FormGroupID == 0)
                        {
                            formData.formGroupDHdr.CreatedOn = DateTime.Now;
                            formData.formGroupDHdr.CreatedBy = userDetail.UserID;
                            formData.formGroupDHdr.FormID = formData.FormID;
                            formData.formGroupDHdr.DesignationID = (int)userDetail.DesignationID;
                            formData.formGroupDHdr.DepartmentID = userDetail.DepartmentID;

                            flag = appraisalService.InsertFormDData(formData);
                            var redirect = ButtonType == "Save" ? 0 : 1;
                            return Json(new { msgType = "success", msg = "Form saved successfully." ,redirect=redirect}, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            formData.formGroupDHdr.UpdatedOn = DateTime.Now;
                            formData.formGroupDHdr.UpdatedBy = userDetail.UserID;
                            formData.formGroupDHdr.FormID = formData.FormID;
                            ///====  Adding auto push workflow data ==== 
                            formData.FormState = formData.formGroupDHdr.FormState.Value;
                            formData.FormGroupID = formData.formGroupDHdr.FormGroupID;
                            AddAutoPushWorkFlow(formData);
                            ///=====   end  ============================

                            flag = appraisalService.UpdateFormDData(formData);
                            var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                            var redirect = ButtonType == "Save" ? 0 : 1;
                            return Json(new { msgType = "success", msg = msg,redirect=redirect }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //if (!appraisalService.IsTrainingSubmitted(formData.EmployeeID, formData.formGroupDHdr.ReportingYr))
                        //{
                        //    var errormsg = "You can not apply for appraisal right now, please first apply for skill assessment.";
                        //    return Json(new { isTraining = 1, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupD", formData) }, JsonRequestBehavior.AllowGet);
                        //}

                        if (formData.EmployeeID == formData.loggedInEmpID)
                        {
                            ModelState.Remove("formGroupDHdr.PART4_6_Remark");
                            // formData.submittedBy = Model.SubmittedBy.Employee;
                            // formData.formGroupDHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByEmployee : (int)Model.AppraisalFormState.SubmitedByEmployee);
                            ModelState.Remove("formGroupDHdr.ReportingOfficeComment");
                            ModelState.Remove("formGroupDHdr.ReviewingOfficerComment");
                            ModelState.Remove("formGroupDHdr.AcceptanceAuthorityComment");

                            ModelState.Remove("formGroupDHdr.Part3_2_A_1");
                            ModelState.Remove("formGroupDHdr.Part3_2_A_2");
                            ModelState.Remove("formGroupDHdr.Part3_2_A_3");
                            ModelState.Remove("formGroupDHdr.Part3_2_A_4");
                            ModelState.Remove("formGroupDHdr.Part3_2_B_1");
                            ModelState.Remove("formGroupDHdr.Part3_2_B_2");
                            ModelState.Remove("formGroupDHdr.Part3_2_B_3");
                            ModelState.Remove("formGroupDHdr.Part3_2_B_4");
                            ModelState.Remove("formGroupDHdr.Part3_2_B_5");
                            ModelState.Remove("formGroupDHdr.Part3_2_B_6");
                            ModelState.Remove("formGroupDHdr.Part3_2_B_7");
                            ModelState.Remove("formGroupDHdr.Part3_2_B_8");

                            ModelState.Remove("formGroupDHdr.Part3_2_C_1");
                            ModelState.Remove("formGroupDHdr.Part3_2_C_2");
                            ModelState.Remove("formGroupDHdr.Part3_2_C_3");
                            ModelState.Remove("formGroupDHdr.Part3_2_C_4");
                            ModelState.Remove("formGroupDHdr.Part3_2_C_5");
                            ModelState.Remove("formGroupDHdr.Part3_2_C_6");
                            ModelState.Remove("formGroupDHdr.Part4_1");
                            ModelState.Remove("formGroupDHdr.Part4_3");
                            ModelState.Remove("formGroupDHdr.Part4_4");
                            ModelState.Remove("formGroupDHdr.Part4_6_Remark");

                            ModelState.Remove("formGroupDHdr.Part5_1");
                            ModelState.Remove("formGroupDHdr.Part5_3");
                            ModelState.Remove("formGroupDHdr.AcceptanceAuthorityRemarks");

                        }
                        else if (formData.ReportingTo == formData.loggedInEmpID)
                        {
                            //  formData.submittedBy = Model.SubmittedBy.ReportingOfficer;
                            // formData.formGroupDHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReporting : (int)Model.AppraisalFormState.SubmitedByReporting);
                            ModelState.Remove("formGroupDHdr.ReviewingOfficerComment");
                            ModelState.Remove("formGroupDHdr.AcceptanceAuthorityComment");
                            ModelState.Remove("formGroupDHdr.AcceptanceAuthorityRemarks");
                            ModelState.Remove("formGroupDHdr.PART4_6_Remark");
                            ModelState.Remove("formGroupDHdr.Part5_3");
                            if (formData.ReportingTo != formData.ReviewingTo)
                            {
                                ModelState.Remove("formGroupDHdr.PART5_1");
                            }
                        }
                        else if (formData.ReviewingTo == formData.loggedInEmpID)
                        {
                            ModelState.Remove("formGroupDHdr.PART4_6_Remark");
                            ModelState.Remove("formGroupDHdr.ReportingOfficeComment");
                            //  formData.submittedBy = Model.SubmittedBy.ReviewingOfficer;
                            //   formData.formGroupDHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                            ModelState.Remove("formGroupDHdr.AcceptanceAuthorityComment");
                            if (formData.ReviewingTo != formData.AcceptanceAuthorityTo)
                            {
                                ModelState.Remove("formGroupDHdr.AcceptanceAuthorityRemarks");
                            }
                        }
                        else if (formData.AcceptanceAuthorityTo == formData.loggedInEmpID)
                        {
                            ModelState.Remove("formGroupDHdr.PART4_6_Remark");
                            ModelState.Remove("formGroupDHdr.ReportingOfficeComment");
                            ModelState.Remove("formGroupDHdr.ReviewingOfficerComment");
                            ModelState.Remove("formGroupDHdr.Part5_3");
                            //  formData.formGroupDHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                            //  formData.submittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        }

                        if (formData.ReviewingTo == formData.AcceptanceAuthorityTo && formData.loggedInEmpID == formData.ReviewingTo)
                        {
                            if (formData.formGroupDHdr.Part5_2 == true)
                                ModelState.Remove("formGroupDHdr.Part5_3");
                            else if (formData.formGroupDHdr.Part5_2 == false)
                            {
                                if (string.IsNullOrEmpty(formData.formGroupDHdr.Part5_3))
                                    ModelState.AddModelError("formGroupDHdr.Part5_3", "Please enter remark.");
                            }
                        }

                        if (formData.ReportingTo == formData.ReviewingTo && formData.ReviewingTo == formData.AcceptanceAuthorityTo && formData.loggedInEmpID == formData.ReportingTo)
                        {
                            if (formData.formGroupDHdr.Part5_2 == true)
                            {
                                ModelState.Remove("formGroupDHdr.Part5_3");
                            }
                            else if (formData.formGroupDHdr.Part5_2 == false)
                            {
                                if (string.IsNullOrEmpty(formData.formGroupDHdr.Part5_3))
                                    ModelState.AddModelError("formGroupAHdr.Part5_3", "Please enter remark.");
                            }

                            if (string.IsNullOrEmpty(formData.formGroupDHdr.AcceptanceAuthorityRemarks))
                            {
                                ModelState.AddModelError("formGroupDHdr.AcceptanceAuthorityRemarks", "Please enter remark.");
                            }
                        }

                        if (formData.ApprovalHierarchy == 1 && formData.ReviewingTo == formData.loggedInEmpID)
                        {
                            if (formData.formGroupDHdr.Part5_2 == true)
                                ModelState.Remove("formGroupDHdr.Part5_3");
                            else if (formData.formGroupDHdr.Part5_2 == false)
                            {
                                if (string.IsNullOrEmpty(formData.formGroupDHdr.Part5_3))
                                    ModelState.AddModelError("formGroupAHdr.Part5_3", "Please enter remark.");
                            }
                        }
                        else if (formData.ApprovalHierarchy == 2 && formData.ReviewingTo == formData.ReportingTo)
                        {
                            if (formData.formGroupDHdr.Part5_2 == true)
                            {
                                ModelState.Remove("formGroupDHdr.Part5_3");
                            }
                            else if (formData.formGroupDHdr.Part5_2 == false)
                            {
                                if (string.IsNullOrEmpty(formData.formGroupDHdr.Part5_3))
                                    ModelState.AddModelError("formGroupDHdr.Part5_3", "Please enter remark.");
                            }
                        }


                        formData.formGroupDHdr.CreatedBy = userDetail.UserID;
                        formData.formGroupDHdr.EmployeeID = formData.formGroupDHdr.EmployeeID > 0 ? formData.formGroupDHdr.EmployeeID : (int)userDetail.EmployeeID;

                        if (formData.FormID == 0)
                            formData.FormID = (int)userDetail.AppraisalFormID;

                        //if (formData.formGroupDHdr.WorkedPeriodUnderROFrom.HasValue && formData.formGroupDHdr.WorkedPeriodUnderROTo.HasValue)
                        //{
                        //    if (formData.formGroupDHdr.WorkedPeriodUnderROTo.Value < formData.formGroupDHdr.WorkedPeriodUnderROFrom.Value)
                        //    {
                        //        ModelState.AddModelError("InValidFromDate", "From date should be always less than To date.");
                        //        formData = (Model.AppraisalForm)TempData["appraisal"];
                        //        TempData.Keep("appraisal");
                        //        return PartialView("_FormGroupD", formData);
                        //    }
                        //}

                        if (!formData.formGroupDHdr.WorkedPeriodUnderROFrom.HasValue)
                        {
                            ModelState.AddModelError("InValidFromDate", "Please select date.");
                            formData = (Model.AppraisalForm)TempData["appraisal"];
                            TempData.Keep("appraisal");
                            formData.isValid = false;
                            //return PartialView("_FormGroupD", formData);
                        }
                        else if (!formData.formGroupDHdr.WorkedPeriodUnderROFrom.HasValue)
                        {
                            ModelState.AddModelError("InValidFromDate", "Please select date.");
                            formData = (Model.AppraisalForm)TempData["appraisal"];
                            TempData.Keep("appraisal");
                            formData.isValid = false;
                            //return PartialView("_FormGroupD", formData);
                        }

                        if (formData.formGroupDHdr.WorkedPeriodUnderROFrom.HasValue && formData.formGroupDHdr.WorkedPeriodUnderROTo.HasValue)
                        {
                            if (formData.formGroupDHdr.WorkedPeriodUnderROTo.Value < formData.formGroupDHdr.WorkedPeriodUnderROFrom.Value)
                            {
                                ModelState.AddModelError("InValidFromDate", "From date should be always less than To date.");
                                formData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                formData.isValid = false;
                                //return PartialView("_FormGroupD", formData);
                            }
                        }

                        //var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                        appraisalService.SetSectionValue(formData);
                        RemoveModelError(formData);

                        if (ModelState.IsValid)
                        {
                            var formNewAttributes = GetFormAttributes(formData, ButtonType);
                            formData.submittedBy = formNewAttributes.SubmittedBy;
                            formData.formGroupDHdr.FormState = formNewAttributes.FormState;

                            if (formData.formGroupDHdr.FormGroupID == 0)
                            {
                                formData.formGroupDHdr.CreatedOn = DateTime.Now;
                                formData.formGroupDHdr.CreatedBy = userDetail.UserID;
                                formData.formGroupDHdr.FormID = formData.FormID;
                                formData.formGroupDHdr.DesignationID = (int)userDetail.DesignationID;
                                formData.formGroupDHdr.DepartmentID = userDetail.DepartmentID;

                                Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                {
                                    SenderID = userDetail.EmployeeID,
                                    //ReceiverID = GetWorkFlowReceiverID(formData),
                                    ReceiverID = formNewAttributes.ReciverID,
                                    SenderDepartmentID = userDetail.DepartmentID,
                                    SenderDesignationID = userDetail.DesignationID,
                                    CreatedBy = userDetail.UserID,
                                    EmployeeID = formData.EmployeeID,
                                    Scomments = $"Form D, Appraisal Form Submited By : {(Model.SubmittedBy)(int)formData.submittedBy}",
                                    ProcessID = (int)WorkFlowProcess.Appraisal,
                                    StatusID = (int)formData.formGroupDHdr.FormState
                                };
                                formData._ProcessWorkFlow = workFlow;

                                flag = appraisalService.InsertFormDData(formData);
                                var redirect = ButtonType == "Save" ? 0 : 1;
                                return Json(new { msgType = "success", msg = "Form submitted successfully.",redirect=redirect }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                formData.formGroupDHdr.UpdatedOn = DateTime.Now;
                                formData.formGroupDHdr.UpdatedBy = userDetail.UserID;
                                formData.formGroupDHdr.FormID = formData.FormID;

                                Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                {
                                    SenderID = userDetail.EmployeeID,
                                    //ReceiverID = GetWorkFlowReceiverID(formData),
                                    ReceiverID = formNewAttributes.ReciverID,
                                    SenderDepartmentID = userDetail.DepartmentID,
                                    SenderDesignationID = userDetail.DesignationID,
                                    CreatedBy = userDetail.UserID,
                                    EmployeeID = formData.EmployeeID,
                                    Scomments = $"Form D, Appraisal Form Submited By : {(Model.SubmittedBy)(int)formData.submittedBy}",
                                    ProcessID = (int)WorkFlowProcess.Appraisal,
                                    StatusID = (int)formData.formGroupDHdr.FormState
                                };
                                formData._ProcessWorkFlow = workFlow;

                                ///====  Adding auto push workflow data ==== 
                                formData.FormState = formData.formGroupDHdr.FormState.Value;
                                formData.FormGroupID = formData.formGroupDHdr.FormGroupID;
                                AddAutoPushWorkFlow(formData);
                                ///=====   end  ============================

                                flag = appraisalService.UpdateFormDData(formData);
                                var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                var redirect = ButtonType == "Save" ? 0 : 2;
                                return Json(new { msgType = "success", msg = msg,redirect=redirect }, JsonRequestBehavior.AllowGet);
                            }
                            // return PartialView("_FormGroupH", formData);
                        }
                        else
                        {
                            var PART5_2 = formData.formGroupDHdr.Part5_2;
                            var acceptRemark = formData.formGroupDHdr.AcceptanceAuthorityRemarks;
                            var PART5_3 = formData.formGroupDHdr.Part5_3;
                            var PART5_1 = formData.formGroupDHdr.Part5_1;
                            var PART5_4 = formData.formGroupDHdr.Part5_4;
                            formData = (Model.AppraisalForm)TempData["appraisal"];
                            TempData.Keep("appraisal");
                            formData.formGroupDHdr.Part5_2 = PART5_2;
                            formData.formGroupDHdr.AcceptanceAuthorityRemarks = acceptRemark;
                            formData.formGroupDHdr.Part5_3 = PART5_3;
                            formData.formGroupDHdr.Part5_1 = PART5_1;
                            formData.formGroupDHdr.Part5_4 = PART5_4;
                            var trainingData = (Model.AppraisalForm)TempData["frmGroupTrainingData"];
                            if (trainingData != null)
                            {
                                if (trainingData.formGroupATrainingDtls != null && trainingData.formGroupATrainingDtls.Count > 0)
                                    formData.formGroupATrainingDtls = trainingData.formGroupATrainingDtls;
                            }
                            formData.isValid = false;
                            return PartialView("_FormGroupD", formData);
                        }


                    }
                }
                // return PartialView("_FormGroupD", formData);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion 

        #region Form Group E

        public ActionResult _GetFormGroupE(Model.AppraisalForm appraisal)
        {
            log.Info($"AppraisalController/_GetFormGroupE");
            try
            {
                var reportingYr = appraisal.ReportingYr;//  string.IsNullOrEmpty(appraisal.ReportingYr) ? DateTime.Now.GetFinancialYr() : appraisal.ReportingYr;
                //appraisal.loggedInEmpID = userDetail.EmployeeID;
                if (!appraisal.isAdmin)
                    appraisal.loggedInEmpID = userDetail.EmployeeID;
                else
                {
                    if (appraisal.AcceptanceAuthorityTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.AcceptanceAuthorityTo.Value;
                    else if (appraisal.ReviewingTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.ReviewingTo.Value;
                    else
                        appraisal.loggedInEmpID = appraisal.ReportingTo.Value;
                }
                appraisal.formGroupAHdr = appraisalService.GetFormGroupHdrDetail("FormGroupEHdr", null, appraisal.EmployeeID, reportingYr, null);
                appraisal.FormID = appraisal.formGroupAHdr.FormID;
                appraisal.EmployeeID = appraisal.formGroupAHdr.EmployeeID;

                appraisal.formGroupAHdr.ReportingYr = (appraisal.formGroupAHdr.ReportingYr != null && appraisal.formGroupAHdr.ReportingYr.Length > 0) ? appraisal.formGroupAHdr.ReportingYr : reportingYr;

                var currentYr = int.Parse(appraisal.formGroupAHdr.ReportingYr.Substring(0, 4));
                var prHeader = prService.GetPropertyReturn(currentYr, appraisal.EmployeeID);

                if (prHeader != null)
                {
                    appraisal.AnnualPropertyReturnSubmitted = prHeader.StatusID == 1 ? true : false;
                    appraisal.AnnualPropertySubmittedOn = prHeader.StatusID == 1 ? prHeader.UpdatedOn : null;
                }


                appraisal.formGroupAHdr.Part4_1_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_1_Grade;
                appraisal.formGroupAHdr.Part4_2_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_2_Grade;
                appraisal.formGroupAHdr.Part4_3_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_3_Grade;
                appraisal.formGroupAHdr.FormPart4Integrity = (Model.FormPart4Integrity)appraisal.formGroupAHdr.PART4_4_Grade;
                appraisal.formGroupADetail1List = appraisalService.GetFormGroupDetail1("FormGroupEHdr", "FormGroupEDetail1", appraisal.EmployeeID, appraisal.formGroupAHdr.FormGroupID).ToList();
                appraisal.formGroupATrainingDtls = appraisalService.GetFormTrainingDetail("FormGroupEHdr", "FormGroupTrainingDtls", appraisal.EmployeeID, appraisal.formGroupAHdr.FormGroupID, appraisal.FormID, appraisal.formGroupAHdr.ReportingYr).ToList();
                if (appraisal.formGroupATrainingDtls.Count > 0)
                {
                    var sno = 1;
                    appraisal.formGroupATrainingDtls.ForEach(x =>
                    {
                        x.sno = sno++;
                        x.FormTraining = (Model.FormPart4Training)x.TrainingID;
                    });
                }

                if (appraisal.formGroupADetail1List.Count > 0)
                {
                    var sno = 1;
                    appraisal.formGroupADetail1List.ForEach(x =>
                    {
                        x.sno = sno++;
                    });
                }
                appraisal.formGroupADetail2List = appraisalService.GetFormGroupDetail2("FormGroupEHdr", "FormGroupEDetail2", (int)userDetail.EmployeeID, appraisal.formGroupAHdr.FormGroupID).ToList();
                TempData["appraisal"] = appraisal;
                TempData.Keep("appraisal");
                appraisal.isValid = true;
                return PartialView("_FormGroupE", appraisal);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostROFormEData(Model.AppraisalForm roformData, string ButtonType, FormCollection frm)
        {
            log.Info($"AppraisalController/_PostROFormEData");
            bool flag;
            try
            {
                if (ButtonType == "Add Row")
                {
                    if (roformData.formGroupADetail1List == null)
                        roformData.formGroupADetail1List = new List<Model.FormGroupDetail1>() {
                            new Model.FormGroupDetail1() { sno = 1 } };
                    else
                    {
                        if (roformData.formGroupADetail1List.Count == 1)
                            roformData.formGroupADetail1List.FirstOrDefault().sno = 1;
                        else
                        {
                            var s_no = 1;
                            roformData.formGroupADetail1List.ForEach(x => { x.sno = s_no; s_no++; });
                        }
                        if (roformData.formGroupADetail1List.Count < 10)
                            roformData.formGroupADetail1List.Add(new Model.FormGroupDetail1()
                            {
                                sno = roformData.formGroupADetail1List.Count + 1
                            });
                    }
                    TempData["frmGroupPart2Data"] = roformData;
                    return Json(new { part = 1, htmlData = ConvertViewToString("_FormGroupPart2GridView", roformData) }, JsonRequestBehavior.AllowGet);

                }
                else if (ButtonType == "Add New Training Row")
                {
                    if (roformData.formGroupATrainingDtls == null)
                        roformData.formGroupATrainingDtls = new List<Model.FormTrainingDtls>()
                        { new Model.FormTrainingDtls() { sno = 1  } };
                    else
                    {
                        if (roformData.formGroupATrainingDtls.Count == 1)
                            roformData.formGroupATrainingDtls.FirstOrDefault().sno = 1;

                        if (roformData.formGroupATrainingDtls.Count < 3)
                            roformData.formGroupATrainingDtls.Add(new Model.FormTrainingDtls()
                            {
                                sno = roformData.formGroupATrainingDtls.Count + 1
                            });
                    }
                    TempData["frmGroupTrainingData"] = roformData;
                    return Json(new { part = 2, htmlData = ConvertViewToString("_FormGroupTrainingGridView", roformData) }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    if (!roformData.ReportingTo.HasValue || roformData.ReportingTo.Value == 0)
                    {
                        var errormsg = "You can not apply for appraisal right now because either your Reporting or Reviewing Manager is not set.";
                        return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupE", roformData) }, JsonRequestBehavior.AllowGet);
                    }

                    if (ButtonType == "Save")
                    {
                        var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                        roformData.submittedBy = formNewAttributes.SubmittedBy;
                        roformData.formGroupAHdr.FormState = formNewAttributes.FormState;

                        if (!appraisalService.AppraisalDataExists(roformData.EmployeeID, roformData.FormID, roformData.formGroupAHdr.ReportingYr))
                        {
                            roformData.formGroupAHdr.CreatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                            roformData.formGroupAHdr.CreatedOn = DateTime.Now;
                            roformData.formGroupAHdr.DesignationID = (int)userDetail.DesignationID;
                            roformData.formGroupAHdr.DepartmentID = (int)userDetail.DepartmentID;
                            flag = appraisalService.InsertFormEData(roformData);
                            return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = "Form Saved successfully." }, JsonRequestBehavior.AllowGet);


                        }
                        else
                        {

                            roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                            flag = appraisalService.UpdateFormEData(roformData);
                            var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                            return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //if (!appraisalService.IsTrainingSubmitted(roformData.EmployeeID, roformData.formGroupAHdr.ReportingYr))
                        //{
                        //    var errormsg = "You can not apply for appraisal right now, please first apply for skill assessment.";
                        //    return Json(new { isTraining = 1, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupE", roformData) }, JsonRequestBehavior.AllowGet);
                        //}
                        if (roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue && roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                        {
                            if (roformData.formGroupAHdr.WorkedPeriodUnderROTo.Value < roformData.formGroupAHdr.WorkedPeriodUnderROFrom.Value)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROFrom", "From date should be always less than To date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupE", roformData);
                            }
                        }
                        if (roformData.EmployeeID == roformData.loggedInEmpID)
                        {
                            roformData.submittedBy = Model.SubmittedBy.Employee;
                            //  roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByEmployee : (int)Model.AppraisalFormState.SubmitedByEmployee);

                        }
                        //   else 
                        if (roformData.ReportingTo == roformData.loggedInEmpID)
                        {
                            //  roformData.submittedBy = Model.SubmittedBy.ReportingOfficer;
                            //   roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReporting : (int)Model.AppraisalFormState.SubmitedByReporting);
                            ModelState.Remove("formGroupAHdr.PART2_3B");
                            if (roformData.ReportingTo != roformData.ReviewingTo)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_1");
                                ModelState.Remove("formGroupADetail2List[0].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[1].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[2].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[3].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[4].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[5].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[6].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[7].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[8].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[9].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[10].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[11].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[12].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[13].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[14].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[15].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[16].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[17].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[18].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[19].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[20].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[21].ReviewingAuthorityWeightage");
                            }
                            else
                            {
                            }

                            ModelState.Remove("formGroupAHdr.PART5_2");
                            ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            if (roformData.ReportingTo == roformData.ReviewingTo && roformData.ReviewingTo == roformData.AcceptanceAuthorityTo)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                                {
                                    ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                                }
                            }
                            else
                                ModelState.Remove("formGroupAHdr.Remarks");
                        }

                        else if (roformData.ReviewingTo == roformData.loggedInEmpID)
                        {
                            var part5_5_Weightage = frm["formGroupAHdr.PART5_5_Weightage"].ToString();
                            var part4_6_Weightage = frm["formGroupAHdr.PART4_6_Weightage"].ToString();
                            if (part5_5_Weightage == "NaN")
                                part5_5_Weightage = "";
                            if (part4_6_Weightage == "NaN")
                                part4_6_Weightage = "";
                            roformData.formGroupAHdr.PART5_5_Weightage = (part5_5_Weightage == "" || part5_5_Weightage == null) ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            roformData.formGroupAHdr.PART4_6_Weightage = (part4_6_Weightage == "" || part4_6_Weightage == null) ? 0 : Convert.ToDecimal(part4_6_Weightage);

                            //roformData.formGroupAHdr.PART5_5_Weightage =
                            //    part5_5_Weightage != "" ? Convert.ToDecimal(part5_5_Weightage) : 0;
                            //roformData.formGroupAHdr.PART4_6_Weightage =
                            // part4_6_Weightage != "" ? Convert.ToDecimal(part4_6_Weightage) : 0;


                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART2_3B");
                            ModelState.Remove("formGroupAHdr.PART4_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");

                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");

                          
                            ModelState.Remove("formGroupAHdr.PART5_4");

                            //ModelState.Remove("formGroupADetail2List[5].ReviewingAuthorityWeightage");
                            if (roformData.ReviewingTo != roformData.AcceptanceAuthorityTo)
                            {
                                ModelState.Remove("formGroupAHdr.AcceptanceAuthorityRemarks");
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                                {
                                    ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                                }
                            }

                            roformData.submittedBy = Model.SubmittedBy.ReviewingOfficer;
                            // roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                        }

                        else if (roformData.AcceptanceAuthorityTo == roformData.loggedInEmpID)
                        {
                            var part5_5_Weightage = frm["formGroupAHdr.PART5_5_Weightage"].ToString();
                            var part4_6_Weightage = frm["formGroupAHdr.PART4_6_Weightage"].ToString();

                            if (part5_5_Weightage == "NaN")
                                part5_5_Weightage = "";
                            if (part4_6_Weightage == "NaN")
                                part4_6_Weightage = "";

                            roformData.formGroupAHdr.PART5_5_Weightage = (part5_5_Weightage == "" || part5_5_Weightage == null) ? 0 : Convert.ToDecimal(part5_5_Weightage);
                            roformData.formGroupAHdr.PART4_6_Weightage = (part4_6_Weightage == "" || part4_6_Weightage == null) ? 0 : Convert.ToDecimal(part4_6_Weightage);

                            //roformData.formGroupAHdr.PART5_5_Weightage = Convert.ToDecimal(part5_5_Weightage);
                            //roformData.formGroupAHdr.PART4_6_Weightage = Convert.ToDecimal(part4_6_Weightage);
                            ModelState.Remove("formGroupAHdr.PART2_3B");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                            {
                                ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                            }
                            roformData.submittedBy = Model.SubmittedBy.AcceptanceAuthority;

                            // roformData.formGroupAHdr.FormState = (byte)(ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        }

                        if (roformData.ReviewingTo == roformData.AcceptanceAuthorityTo && roformData.loggedInEmpID == roformData.ReviewingTo)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }

                        if (roformData.ReportingTo == roformData.ReviewingTo && roformData.ReviewingTo == roformData.AcceptanceAuthorityTo && roformData.loggedInEmpID == roformData.ReportingTo)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            }
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }

                        if (roformData.ApprovalHierarchy == 1 && roformData.ReviewingTo == roformData.loggedInEmpID)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }
                        else if (roformData.ApprovalHierarchy == 2 && roformData.ReviewingTo == roformData.ReportingTo)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            }
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }


                        if (roformData.FormID == 0)
                            roformData.FormID = (int)userDetail.AppraisalFormID;

                        if (!appraisalService.AppraisalDataExists(roformData.EmployeeID, roformData.FormID, roformData.formGroupAHdr.ReportingYr))
                        {
                            roformData.formGroupAHdr.CreatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                            roformData.formGroupAHdr.CreatedOn = DateTime.Now;
                            if (!roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROFrom", "Please select date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                roformData.isValid = false;
                                TempData.Keep("appraisal");
                                return PartialView("_FormGroupE", roformData);
                            }
                            else if (!roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROTo", "Please select date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupE", roformData);
                            }
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_3B) || string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_1))
                            {

                                ModelState.AddModelError("gg", ".");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupE", roformData);
                            }
                            else
                            {
                                var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                                roformData.submittedBy = formNewAttributes.SubmittedBy;
                                roformData.formGroupAHdr.FormState = formNewAttributes.FormState;
                                Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                {
                                    SenderID = userDetail.EmployeeID,
                                    ReceiverID = formNewAttributes.ReciverID,
                                    SenderDepartmentID = userDetail.DepartmentID,
                                    SenderDesignationID = userDetail.DesignationID,
                                    CreatedBy = userDetail.UserID,
                                    EmployeeID = roformData.EmployeeID,
                                    Scomments = $"Form E, Appraisal Form Submited By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                    ProcessID = (int)WorkFlowProcess.Appraisal,
                                    StatusID = (int)roformData.formGroupAHdr.FormState
                                };
                                roformData._ProcessWorkFlow = workFlow;

                                roformData.formGroupAHdr.DesignationID = (int)userDetail.DesignationID;
                                roformData.formGroupAHdr.DepartmentID = (int)userDetail.DepartmentID;
                                flag = appraisalService.InsertFormEData(roformData);

                                return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = "Form submitted successfully." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (roformData.EmployeeID == roformData.loggedInEmpID)
                            {
                                int errorCount = 0;
                                if (!roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue)
                                {
                                    ModelState.AddModelError("WorkedPeriodUnderROFrom", "Please select date.");
                                    errorCount++;
                                }
                                if (!roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                                {
                                    ModelState.AddModelError("WorkedPeriodUnderROTo", "Please select date.");
                                    errorCount++;
                                }
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_3B) || string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_1))
                                {
                                    ModelState.AddModelError("gg", ".");
                                    errorCount++;
                                }
                                //else
                                //{
                                if (errorCount == 0)
                                {
                                    var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                                    roformData.submittedBy = formNewAttributes.SubmittedBy;
                                    roformData.formGroupAHdr.FormState = formNewAttributes.FormState;
                                    roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                                    roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        // ReceiverID = GetWorkFlowReceiverID(roformData),
                                        ReceiverID = formNewAttributes.ReciverID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form E, Appraisal Form Submited By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.Appraisal,
                                        StatusID = (int)roformData.formGroupAHdr.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;

                                    flag = appraisalService.UpdateFormEData(roformData);
                                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                    return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var formGroup1Details = roformData.formGroupADetail1List;
                                    roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    TempData.Keep("appraisal");
                                    roformData.formGroupADetail1List = formGroup1Details;
                                    roformData.isValid = errorCount == 0 ? true : false;
                                    return PartialView("_FormGroupE", roformData);
                                }
                                //}
                            }
                            else
                            {
                                // var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                                appraisalService.SetSectionValue(roformData);
                                RemoveModelError(roformData);
                                if (ModelState.IsValid)
                                {
                                    var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                                    roformData.submittedBy = formNewAttributes.SubmittedBy;
                                    roformData.formGroupAHdr.FormState = formNewAttributes.FormState;
                                    roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                                    roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        //  ReceiverID = GetWorkFlowReceiverID(roformData),
                                        ReceiverID = formNewAttributes.ReciverID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form E, Appraisal Form Updated By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.Appraisal,
                                        StatusID = (int)roformData.formGroupAHdr.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;

                                    ///====  Adding auto push workflow data ==== 
                                    roformData.FormState = roformData.formGroupAHdr.FormState;
                                    roformData.FormGroupID = roformData.formGroupAHdr.FormGroupID;
                                    AddAutoPushWorkFlow(roformData);
                                    ///=====   end  ============================

                                    flag = appraisalService.UpdateFormEData(roformData);
                                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                    return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var PART5_5_Weightage = roformData.formGroupAHdr.PART5_5_Weightage;
                                    var PART5_2 = roformData.formGroupAHdr.PART5_2;
                                    roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    TempData.Keep("appraisal");
                                    roformData.formGroupAHdr.PART5_5_Weightage = PART5_5_Weightage;
                                    roformData.formGroupAHdr.PART5_2 = PART5_2;
                                    var trainingData = (Model.AppraisalForm)TempData["frmGroupTrainingData"];
                                    if (trainingData != null)
                                    {
                                        if (trainingData.formGroupATrainingDtls != null && trainingData.formGroupATrainingDtls.Count > 0)
                                            roformData.formGroupATrainingDtls = trainingData.formGroupATrainingDtls;
                                    }
                                    roformData.isValid = false;
                                    return PartialView("_FormGroupE", roformData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region FORM GROUP F

        public ActionResult _GetFormGroupF(Model.AppraisalForm appraisal)
        {
            log.Info($"AppraisalController/_GetFormGroupF");
            try
            {
                var reportingYr = appraisal.ReportingYr;//  string.IsNullOrEmpty(appraisal.ReportingYr) ? DateTime.Now.GetFinancialYr() : appraisal.ReportingYr;
                //appraisal.loggedInEmpID = userDetail.EmployeeID;
                if (!appraisal.isAdmin)
                    appraisal.loggedInEmpID = userDetail.EmployeeID;
                else
                {
                    if (appraisal.AcceptanceAuthorityTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.AcceptanceAuthorityTo.Value;
                    else if (appraisal.ReviewingTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.ReviewingTo.Value;
                    else
                        appraisal.loggedInEmpID = appraisal.ReportingTo.Value;
                }

                appraisal.formGroupAHdr = appraisalService.GetFormGroupFHdrDetail("FormGroupFHdr", null, appraisal.EmployeeID, reportingYr, null);
                appraisal.FormID = appraisal.formGroupAHdr.FormID;
                appraisal.EmployeeID = appraisal.formGroupAHdr.EmployeeID;




                appraisal.formGroupAHdr.ReportingYr = (appraisal.formGroupAHdr.ReportingYr != null && appraisal.formGroupAHdr.ReportingYr.Length > 0) ? appraisal.formGroupAHdr.ReportingYr : reportingYr;

                var currentYr = int.Parse(appraisal.formGroupAHdr.ReportingYr.Substring(0, 4));
                var prHeader = prService.GetPropertyReturn(currentYr, appraisal.formGroupAHdr.EmployeeID);

                if (prHeader != null)
                {
                    appraisal.AnnualPropertyReturnSubmitted = prHeader.StatusID == 1 ? true : false;
                    appraisal.AnnualPropertySubmittedOn = prHeader.StatusID == 1 ? prHeader.UpdatedOn : null;
                }
                appraisal.formGroupAHdr.Part4_1_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_1_Grade;
                appraisal.formGroupAHdr.Part4_2_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_2_Grade;
                appraisal.formGroupAHdr.Part4_3_Gr = (Model.FormGrade)appraisal.formGroupAHdr.PART4_3_Grade;
                appraisal.formGroupAHdr.FormPart4Integrity = (Model.FormPart4Integrity)appraisal.formGroupAHdr.PART4_4_Grade;
                appraisal.formGroupADetail1List = appraisalService.GetFormGroupDetail1("FormGroupFHdr", "FormGroupFDetail1", appraisal.EmployeeID, appraisal.formGroupAHdr.FormGroupID).ToList();
                appraisal.formGroupATrainingDtls = appraisalService.GetFormTrainingDetail("FormGroupFHdr", "FormGroupTrainingDtls", appraisal.EmployeeID, appraisal.formGroupAHdr.FormGroupID, appraisal.FormID, appraisal.formGroupAHdr.ReportingYr).ToList();

                if (appraisal.formGroupATrainingDtls.Count > 0)
                {
                    var sno = 1;
                    appraisal.formGroupATrainingDtls.ForEach(x =>
                    {
                        x.sno = sno++;
                        x.FormTraining = (Model.FormPart4Training)x.TrainingID;
                    });
                }
                appraisal.formGroupADetail2List = appraisalService.GetFormGroupDetail2("FormGroupFHdr", "FormGroupFDetail2", (int)userDetail.EmployeeID, appraisal.formGroupAHdr.FormGroupID).ToList();
                TempData["appraisal"] = appraisal;
                TempData.Keep("appraisal");
                appraisal.isValid = true;
                return PartialView("_FormGroupF", appraisal);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostROFormFData(Model.AppraisalForm roformData, string ButtonType)
        {
            log.Info($"AppraisalController/_PostROFormFData");
            bool flag;
            try
            {
                if (ButtonType == "Add Row")
                {
                    if (roformData.formGroupADetail1List == null)
                        roformData.formGroupADetail1List = new List<Model.FormGroupDetail1>() {
                            new Model.FormGroupDetail1() { sno = 1 } };
                    else
                    {
                        if (roformData.formGroupADetail1List.Count == 1)
                            roformData.formGroupADetail1List.FirstOrDefault().sno = 1;
                        else
                        {
                            var s_no = 1;
                            roformData.formGroupADetail1List.ForEach(x =>
                            {
                                x.sno = s_no++;
                            });

                        }
                        if (roformData.formGroupADetail1List.Count < 10)
                            roformData.formGroupADetail1List.Add(new Model.FormGroupDetail1()
                            { sno = roformData.formGroupADetail1List.Count + 1 });
                        roformData.formGroupADetail1List.OrderBy(x => x.sno);
                    }
                    TempData["frmGroupPart2Data"] = roformData;
                    return Json(new { part = 1, htmlData = ConvertViewToString("_FormGroupPart2GridView", roformData) }, JsonRequestBehavior.AllowGet);

                }
                else if (ButtonType == "Add New Training Row")
                {
                    if (roformData.formGroupATrainingDtls == null)
                        roformData.formGroupATrainingDtls = new List<Model.FormTrainingDtls>()
                        { new Model.FormTrainingDtls() { sno = 1  } };
                    else
                    {
                        if (roformData.formGroupATrainingDtls.Count == 1)
                            roformData.formGroupATrainingDtls.FirstOrDefault().sno = 1;

                        if (roformData.formGroupATrainingDtls.Count < 3)
                            roformData.formGroupATrainingDtls.Add(new Model.FormTrainingDtls()
                            {
                                sno = roformData.formGroupATrainingDtls.Count + 1
                            });
                    }
                    TempData["frmGroupTrainingData"] = roformData;
                    return Json(new { part = 2, htmlData = ConvertViewToString("_FormGroupTrainingGridView", roformData) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (!roformData.ReportingTo.HasValue || roformData.ReportingTo.Value == 0)
                    {
                        var errormsg = "You can not apply for appraisal right now because either your Reporting or Reviewing Manager is not set.";
                        return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupF", roformData) }, JsonRequestBehavior.AllowGet);
                    }

                    if (ButtonType == "Save")
                    {
                        var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                        roformData.submittedBy = formNewAttributes.SubmittedBy;
                        roformData.formGroupAHdr.FormState = formNewAttributes.FormState;

                        if (!appraisalService.AppraisalDataExists(roformData.EmployeeID, roformData.FormID, roformData.formGroupAHdr.ReportingYr))
                        {
                            roformData.formGroupAHdr.CreatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                            roformData.formGroupAHdr.CreatedOn = DateTime.Now;
                            roformData.formGroupAHdr.DesignationID = (int)userDetail.DesignationID;
                            roformData.formGroupAHdr.DepartmentID = (int)userDetail.DepartmentID;
                            flag = appraisalService.InsertFormFData(roformData);
                            return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = "Form Saved successfully." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                            flag = appraisalService.UpdateFormFData(roformData);
                            var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                            return Json(new { formState = roformData.formGroupAHdr.FormState, part = 0, msgType = "success", msg = msg }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue && roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                        {
                            if (roformData.formGroupAHdr.WorkedPeriodUnderROTo.Value < roformData.formGroupAHdr.WorkedPeriodUnderROFrom.Value)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROFrom", "From date should be always less than To date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupF", roformData);
                            }
                        }

                        //if (!appraisalService.IsTrainingSubmitted(roformData.EmployeeID, roformData.formGroupAHdr.ReportingYr))
                        //{
                        //    var errormsg = "You can not apply for appraisal right now, please first apply for skill assessment.";
                        //    return Json(new { isTraining = 1, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupF", roformData) }, JsonRequestBehavior.AllowGet);
                        //}

                        var formNewAttributes = GetFormAttributes(roformData, ButtonType);
                        roformData.submittedBy = formNewAttributes.SubmittedBy;
                        roformData.formGroupAHdr.FormState = formNewAttributes.FormState;
                        //  if (roformData.EmployeeID == roformData.loggedInEmpID)
                        //  {
                        //  roformData.submittedBy = Model.SubmittedBy.Employee;m
                        //   roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByEmployee : (int)Model.AppraisalFormState.SubmitedByEmployee);
                        //  }
                        //    else 
                        if (roformData.ReportingTo == roformData.loggedInEmpID)
                        {
                            // roformData.submittedBy = Model.SubmittedBy.ReportingOfficer;
                            //    roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReporting : (int)Model.AppraisalFormState.SubmitedByReporting);
                            ModelState.Remove("formGroupAHdr.PART2_3B");
                            if (roformData.ReportingTo != roformData.ReviewingTo)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_1");
                                ModelState.Remove("formGroupADetail2List[0].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[1].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[2].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[3].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[4].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[5].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[6].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[7].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[8].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[9].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[10].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[11].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[12].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[13].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[14].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[15].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[16].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[17].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[18].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[19].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[20].ReviewingAuthorityWeightage");
                                ModelState.Remove("formGroupADetail2List[21].ReviewingAuthorityWeightage");
                            }

                            ModelState.Remove("formGroupAHdr.PART5_2");
                            ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            if (roformData.ReportingTo == roformData.ReviewingTo && roformData.ReviewingTo == roformData.AcceptanceAuthorityTo)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                                {
                                    ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                                }
                            }
                        }

                        else if (roformData.ReviewingTo == roformData.loggedInEmpID)
                        {
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART2_3B");
                            ModelState.Remove("formGroupAHdr.PART4_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");

                            //ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");

                            // roformData.submittedBy = Model.SubmittedBy.ReviewingOfficer;

                            // roformData.formGroupAHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                        }
                        else if (roformData.AcceptanceAuthorityTo == roformData.loggedInEmpID)
                        {
                            ModelState.Remove("formGroupAHdr.PART2_3B");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_3");
                            ModelState.Remove("formGroupAHdr.PART5_4");
                            ModelState.Remove("formGroupAHdr.PART4_1_Remark");
                            ModelState.Remove("formGroupAHdr.PART4_3_Remark");
                            ModelState.Remove("formGroupAHdr.PART4_4_Remark");
                            ModelState.Remove("formGroupAHdr.PART4_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART4_6_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                            ModelState.Remove("formGroupAHdr.PART5_5_Remark");
                            ModelState.Remove("formGroupAHdr.PART4_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_6_Remark");
                            ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                            {
                                ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                            }

                            roformData.submittedBy = Model.SubmittedBy.AcceptanceAuthority;
                            // roformData.formGroupAHdr.FormState = (byte)(ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        }

                        if (roformData.ReviewingTo == roformData.AcceptanceAuthorityTo && roformData.loggedInEmpID == roformData.ReviewingTo)
                        {
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                            {
                                ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                            }

                            if (roformData.formGroupAHdr.PART5_2 == true)
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }

                        if (roformData.ReportingTo == roformData.ReviewingTo && roformData.ReviewingTo == roformData.AcceptanceAuthorityTo && roformData.loggedInEmpID == roformData.ReportingTo)
                        {
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.Remarks))
                            {
                                ModelState.AddModelError("formGroupAHdr.Remarks", "Please enter remark.");
                            }

                            if (roformData.formGroupAHdr.PART5_2 == true)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            }
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }

                        if (roformData.ApprovalHierarchy == 1 && roformData.ReviewingTo == roformData.loggedInEmpID)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }
                        else if (roformData.ApprovalHierarchy == 2 && roformData.ReviewingTo == roformData.ReportingTo)
                        {
                            if (roformData.formGroupAHdr.PART5_2 == true)
                            {
                                ModelState.Remove("formGroupAHdr.PART5_3");
                            }
                            else if (roformData.formGroupAHdr.PART5_2 == false)
                            {
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART5_3))
                                    ModelState.AddModelError("formGroupAHdr.PART5_3", "Please enter remark.");
                            }
                        }


                        if (!appraisalService.AppraisalDataExists(roformData.EmployeeID, roformData.FormID, roformData.formGroupAHdr.ReportingYr))
                        {
                            roformData.FormID = (int)userDetail.AppraisalFormID;
                            roformData.formGroupAHdr.CreatedBy = userDetail.UserID;
                            roformData.formGroupAHdr.EmployeeID = roformData.EmployeeID > 0 ? roformData.EmployeeID : (int)userDetail.EmployeeID;
                            roformData.formGroupAHdr.CreatedOn = DateTime.Now;
                            roformData.formGroupAHdr.FormID = (int)userDetail.AppraisalFormID;

                            if (!roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROFrom", "Please select date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupF", roformData);
                            }
                            else if (!roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                            {
                                ModelState.AddModelError("WorkedPeriodUnderROTo", "Please select date.");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupF", roformData);
                            }
                            if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_3B) || string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_1))
                            {

                                ModelState.AddModelError("gg", ".");
                                roformData = (Model.AppraisalForm)TempData["appraisal"];
                                TempData.Keep("appraisal");
                                roformData.isValid = false;
                                return PartialView("_FormGroupF", roformData);
                            }
                            else
                            {
                                Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                {
                                    SenderID = userDetail.EmployeeID,
                                    ReceiverID = formNewAttributes.ReciverID,
                                    SenderDepartmentID = userDetail.DepartmentID,
                                    SenderDesignationID = userDetail.DesignationID,
                                    CreatedBy = userDetail.UserID,
                                    EmployeeID = roformData.EmployeeID,
                                    Scomments = $"Form F, Appraisal Form Submited By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                    ProcessID = (int)WorkFlowProcess.Appraisal,
                                    StatusID = (int)roformData.formGroupAHdr.FormState
                                };
                                roformData._ProcessWorkFlow = workFlow;
                                roformData.formGroupAHdr.DesignationID = (int)userDetail.DesignationID;
                                roformData.formGroupAHdr.DepartmentID = (int)userDetail.DepartmentID;

                                flag = appraisalService.InsertFormFData(roformData);
                                var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                return Json(new { part = 0, msgType = "success", msg = msg }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (roformData.EmployeeID == roformData.loggedInEmpID)
                            {
                                int errorCount = 0;
                                if (!roformData.formGroupAHdr.WorkedPeriodUnderROFrom.HasValue)
                                {
                                    ModelState.AddModelError("WorkedPeriodUnderROFrom", "Please select date.");
                                    errorCount++;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupF", roformData);
                                }
                                if (!roformData.formGroupAHdr.WorkedPeriodUnderROTo.HasValue)
                                {
                                    ModelState.AddModelError("WorkedPeriodUnderROTo", "Please select date.");
                                    errorCount++;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupF", roformData);
                                }
                                if (string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_3B) || string.IsNullOrEmpty(roformData.formGroupAHdr.PART2_1))
                                {

                                    ModelState.AddModelError("gg", ".");
                                    errorCount++;
                                    //roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    //TempData.Keep("appraisal");
                                    //return PartialView("_FormGroupF", roformData);
                                }
                                //else
                                //{
                                if (errorCount == 0)
                                {
                                    roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                                    roformData.formGroupAHdr.UpdatedOn = DateTime.Now;

                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        // ReceiverID = GetWorkFlowReceiverID(roformData),
                                        ReceiverID = formNewAttributes.ReciverID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form F, Appraisal Form Updated By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.Appraisal,
                                        StatusID = (int)roformData.formGroupAHdr.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;

                                    flag = appraisalService.UpdateFormFData(roformData);
                                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                    return Json(new { part = 0, msgType = "success", formState = roformData.formGroupAHdr.FormState, msg = msg }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var formGroup1Details = roformData.formGroupADetail1List;
                                    roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    TempData.Keep("appraisal");
                                    roformData.formGroupADetail1List = formGroup1Details;
                                    roformData.isValid = errorCount == 0 ? true : false;
                                    return PartialView("_FormGroupF", roformData);
                                }
                                //}
                            }
                            else
                            {
                                //var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                                appraisalService.SetSectionValue(roformData);
                                RemoveModelError(roformData);
                                if (ModelState.IsValid)
                                {
                                    roformData.formGroupAHdr.UpdatedBy = userDetail.UserID;
                                    roformData.formGroupAHdr.UpdatedOn = DateTime.Now;
                                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                                    {
                                        SenderID = userDetail.EmployeeID,
                                        //  ReceiverID = GetWorkFlowReceiverID(roformData),
                                        ReceiverID = formNewAttributes.ReciverID,
                                        SenderDepartmentID = userDetail.DepartmentID,
                                        SenderDesignationID = userDetail.DesignationID,
                                        CreatedBy = userDetail.UserID,
                                        EmployeeID = roformData.EmployeeID,
                                        Scomments = $"Form F, Appraisal Form Updated By : {(Model.SubmittedBy)(int)roformData.submittedBy}",
                                        ProcessID = (int)WorkFlowProcess.Appraisal,
                                        StatusID = (int)roformData.formGroupAHdr.FormState
                                    };
                                    roformData._ProcessWorkFlow = workFlow;

                                    ///====  Adding auto push workflow data ==== 
                                    roformData.FormState = roformData.formGroupAHdr.FormState;
                                    roformData.FormGroupID = roformData.formGroupAHdr.FormGroupID;
                                    AddAutoPushWorkFlow(roformData);
                                    ///=====   end  ============================

                                    flag = appraisalService.UpdateFormFData(roformData);
                                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                                    return Json(new { part = 0, formState = roformData.formGroupAHdr.FormState, msgType = "success", msg = msg }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    var PART4_6_Weightage = roformData.formGroupAHdr.PART4_6_Weightage;
                                    var PART5_6_Weightage = roformData.formGroupAHdr.PART5_6_Weightage;
                                    var PART5_2 = roformData.formGroupAHdr.PART5_2;
                                    roformData = (Model.AppraisalForm)TempData["appraisal"];
                                    TempData.Keep("appraisal");
                                    roformData.formGroupAHdr.PART5_6_Weightage = PART5_6_Weightage;
                                    roformData.formGroupAHdr.PART4_6_Weightage = PART4_6_Weightage;
                                    roformData.formGroupAHdr.PART5_2 = PART5_2;
                                    var trainingData = (Model.AppraisalForm)TempData["frmGroupTrainingData"];
                                    if (trainingData != null)
                                    {
                                        if (trainingData.formGroupATrainingDtls != null && trainingData.formGroupATrainingDtls.Count > 0)
                                            roformData.formGroupATrainingDtls = trainingData.formGroupATrainingDtls;
                                    }
                                    roformData.isValid = false;
                                    return PartialView("_FormGroupF", roformData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region FORM GROUP G

        public ActionResult _GetFormGroupG(Model.AppraisalForm appraisal)
        {
            log.Info($"AppraisalController/_GetFormGroupG");
            try
            {
                var reportingYr = appraisal.ReportingYr;//  string.IsNullOrEmpty(appraisal.ReportingYr) ? DateTime.Now.GetFinancialYr() : appraisal.ReportingYr;
                //appraisal.loggedInEmpID = userDetail.EmployeeID;
                if (!appraisal.isAdmin)
                    appraisal.loggedInEmpID = userDetail.EmployeeID;
                else
                {
                    if (appraisal.AcceptanceAuthorityTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.AcceptanceAuthorityTo.Value;
                    else if (appraisal.ReviewingTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.ReviewingTo.Value;
                    else
                        appraisal.loggedInEmpID = appraisal.ReportingTo.Value;
                }

                appraisal.formGroupGHdr = appraisalService.GetFormGroupGHdrDetail("FormGroupGHdr", null, appraisal.EmployeeID, reportingYr, null);
                appraisal.FormID = appraisal?.formGroupGHdr?.FormID > 0 ? appraisal.formGroupGHdr.FormID : appraisal.FormID;
                appraisal.EmployeeID = appraisal.formGroupGHdr?.EmployeeID > 0 ? appraisal.formGroupGHdr.EmployeeID : appraisal.EmployeeID;

                if (appraisal.formGroupGHdr != null)
                    appraisal.formGroupGHdr.ReportingYr =
                     appraisal.formGroupGHdr != null && !string.IsNullOrEmpty(appraisal.formGroupGHdr.ReportingYr) ? appraisal.formGroupGHdr.ReportingYr : reportingYr;

                else
                    appraisal.formGroupGHdr = new Model.FormGroupGHdr() { ReportingYr = reportingYr };

                appraisal.isValid = true;
                return PartialView("_FormGroupG", appraisal);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult _PostFormGData(Model.AppraisalForm formData, string ButtonType)
        {
            log.Info($"AppraisalController/_PostFormGData");
            try
            {
                bool flag;
                formData.FormState = formData.formGroupGHdr.FormState;
                var formNewAttributes = GetFormAttributes(formData, ButtonType);
                formData.submittedBy = formNewAttributes.SubmittedBy;
                formData.formGroupGHdr.FormState = formNewAttributes.FormState;

                if (!formData.ReportingTo.HasValue || formData.ReportingTo.Value == 0)
                {
                    var errormsg = "You can not apply for appraisal right now because either your Reporting or Reviewing Manager is not set.";
                    return Json(new { formState = formData.formGroupGHdr.FormState, part = 0, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupG", formData) }, JsonRequestBehavior.AllowGet);
                }

                if (ButtonType == "Save")
                {
                    if (!appraisalService.AppraisalDataExists(formData.EmployeeID, formData.FormID, formData.formGroupGHdr.ReportingYr))
                    {
                        formData.formGroupGHdr.CreatedOn = DateTime.Now;
                        formData.formGroupGHdr.CreatedBy = userDetail.UserID;
                        formData.formGroupGHdr.FormID = formData.FormID;
                        formData.formGroupGHdr.DesignationID = userDetail.DesignationID;
                        formData.formGroupGHdr.DepartmentID = userDetail.DepartmentID;
                        flag = appraisalService.InsertFormGData(formData);

                        return Json(new { msgType = "success", formState = formData.formGroupGHdr.FormState, msg = "Form saved successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        formData.formGroupGHdr.UpdatedOn = DateTime.Now;
                        formData.formGroupGHdr.UpdatedBy = userDetail.UserID;
                        formData.formGroupGHdr.FormID = formData.FormID;

                        ///====  Adding auto push workflow data ==== 
                        formData.FormState = formData.formGroupGHdr.FormState;
                        formData.FormGroupID = formData.formGroupGHdr.FormGroupID;
                        AddAutoPushWorkFlow(formData);
                        ///=====   end  ============================

                        flag = appraisalService.UpdateFormGData(formData);
                        var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                        return Json(new { msgType = "success", formState = formData.formGroupGHdr.FormState, msg = msg }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //if (!appraisalService.IsTrainingSubmitted(formData.EmployeeID, formData.formGroupGHdr.ReportingYr))
                    //{
                    //    var errormsg = "You can not apply for appraisal right now, please first apply for skill assessment.";
                    //    return Json(new { isTraining = 1, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupG", formData) }, JsonRequestBehavior.AllowGet);
                    //}
                    Model.ProcessWorkFlow workFlow = null;

                    if (formData.EmployeeID == formData.loggedInEmpID)
                    {
                        //  formData.submittedBy = Model.SubmittedBy.Employee;
                        //  formData.formGroupGHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByEmployee : (int)Model.AppraisalFormState.SubmitedByEmployee);
                        ModelState.Remove("formGroupGHdr.ReportingOfficeComment");
                        ModelState.Remove("formGroupGHdr.ReviewingOfficerComment");
                        ModelState.Remove("formGroupGHdr.AcceptanceAuthorityComment");
                    }
                    else if (formData.ReportingTo == formData.loggedInEmpID)
                    {
                        // formData.submittedBy = Model.SubmittedBy.ReportingOfficer;
                        // formData.formGroupGHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReporting : (int)Model.AppraisalFormState.SubmitedByReporting);
                        if (formData.ReviewingTo != formData.ReportingTo)
                        {
                            ModelState.Remove("formGroupGHdr.AcceptanceAuthorityComment");
                            ModelState.Remove("formGroupGHdr.ReviewingOfficerComment");
                        }
                        ModelState.Remove("formGroupGHdr.AcceptanceAuthorityComment");
                    }
                    else if (formData.ReviewingTo == formData.loggedInEmpID)
                    {
                        // formData.submittedBy = Model.SubmittedBy.ReviewingOfficer;
                        //  formData.formGroupGHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                        ModelState.Remove("formGroupGHdr.ReportingOfficeComment");
                        if (formData.ReviewingTo != formData.AcceptanceAuthorityTo)
                        {
                            ModelState.Remove("formGroupGHdr.AcceptanceAuthorityComment");
                        }
                    }
                    else if (formData.AcceptanceAuthorityTo == formData.loggedInEmpID)
                    {
                        ModelState.Remove("formGroupGHdr.ReportingOfficeComment");
                        //  ModelState.Remove("formGroupGHdr.ReviewingOfficerComment");
                        //  formData.submittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        //  formData.formGroupGHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                    }

                    if (formData.ReviewingTo == formData.AcceptanceAuthorityTo && formData.loggedInEmpID == formData.ReviewingTo)
                    {
                        if (string.IsNullOrEmpty(formData.formGroupGHdr.AcceptanceAuthorityComment))
                        {
                            ModelState.AddModelError("formGroupGHdr.AcceptanceAuthorityComment", "The Acceptance Authority Officer Comment field is required.");
                        }
                    }

                    if (formData.ReportingTo == formData.ReviewingTo && formData.ReviewingTo == formData.AcceptanceAuthorityTo && formData.loggedInEmpID == formData.ReportingTo)
                    {
                        if (string.IsNullOrEmpty(formData.formGroupGHdr.AcceptanceAuthorityComment))
                        {
                            ModelState.AddModelError("formGroupGHdr.AcceptanceAuthorityComment", "The Acceptance Authority Officer Comment field is required.");
                        }
                    }

                    formData.formGroupGHdr.CreatedBy = userDetail.UserID;
                    formData.formGroupGHdr.EmployeeID = formData.formGroupGHdr.EmployeeID > 0 ? formData.formGroupGHdr.EmployeeID : (int)userDetail.EmployeeID;

                    if (formData.FormID == 0)
                        formData.FormID = (int)userDetail.AppraisalFormID;

                    if (formData.formGroupGHdr.WorkedPeriodUnderROFrom.HasValue && formData.formGroupGHdr.WorkedPeriodUnderROTo.HasValue)
                    {
                        if (formData.formGroupGHdr.WorkedPeriodUnderROTo.Value < formData.formGroupGHdr.WorkedPeriodUnderROFrom.Value)
                        {
                            ModelState.AddModelError("InValidFromDate", "From date should be always less than To date.");
                            formData.isValid = false;
                            return PartialView("_FormGroupG", formData);
                        }
                    }

                    if (!formData.formGroupGHdr.WorkedPeriodUnderROFrom.HasValue)
                    {
                        ModelState.AddModelError("InValidFromDate", "Please select date.");

                        //return PartialView("_FormGroupG", formData);
                    }
                    else if (!formData.formGroupGHdr.WorkedPeriodUnderROTo.HasValue)
                    {
                        ModelState.AddModelError("InValidFromDate", "Please select date.");

                        //return PartialView("_FormGroupG", formData);
                    }

                    appraisalService.SetSectionValue(formData);
                    RemoveModelError(formData);
                    if (ModelState.IsValid)
                    {
                        if (formData.formGroupGHdr.WorkedPeriodUnderROFrom.HasValue && formData.formGroupGHdr.WorkedPeriodUnderROTo.HasValue)
                        {
                            if (formData.formGroupGHdr.WorkedPeriodUnderROTo.Value < formData.formGroupGHdr.WorkedPeriodUnderROFrom.Value)
                            {
                                ModelState.AddModelError("InValidFromDate", "From date should be always less than To date.");
                                formData.isValid = false;
                                return PartialView("_FormGroupG", formData);
                            }
                        }
                        if (!appraisalService.AppraisalDataExists(formData.EmployeeID, formData.FormID, formData.formGroupGHdr.ReportingYr))
                        {
                            formData.formGroupGHdr.CreatedOn = DateTime.Now;
                            formData.formGroupGHdr.CreatedBy = userDetail.UserID;
                            formData.formGroupGHdr.FormID = formData.FormID;
                            formData.formGroupGHdr.DesignationID = userDetail.DesignationID;
                            formData.formGroupGHdr.DepartmentID = userDetail.DepartmentID;

                            workFlow = new Model.ProcessWorkFlow()
                            {
                                SenderID = userDetail.EmployeeID,
                                ReceiverID = formNewAttributes.ReciverID,
                                SenderDepartmentID = userDetail.DepartmentID,
                                SenderDesignationID = userDetail.DesignationID,
                                CreatedBy = userDetail.UserID,
                                EmployeeID = formData.EmployeeID,
                                Scomments = $"Form G , Action performed by : {(Model.SubmittedBy)(int)formData.submittedBy}.",
                                ProcessID = (int)WorkFlowProcess.Appraisal,
                                StatusID = formData.formGroupGHdr.FormState
                            };
                            formData._ProcessWorkFlow = workFlow;
                            flag = appraisalService.InsertFormGData(formData);

                            return Json(new { msgType = "success", formState = formData.formGroupGHdr.FormState, msg = "Form submitted successfully." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            workFlow = new Model.ProcessWorkFlow()
                            {
                                SenderID = userDetail.EmployeeID,
                                ReceiverID = formNewAttributes.ReciverID,
                                SenderDepartmentID = userDetail.DepartmentID,
                                SenderDesignationID = userDetail.DesignationID,
                                CreatedBy = userDetail.UserID,
                                EmployeeID = formData.EmployeeID,
                                ProcessID = (int)WorkFlowProcess.Appraisal,
                                StatusID = formData.formGroupGHdr.FormState,
                                Scomments = $"Form G, Appraisal Form Updated By : {(Model.SubmittedBy)(int)formData.submittedBy}"
                            };
                            formData._ProcessWorkFlow = workFlow;
                            formData.formGroupGHdr.UpdatedOn = DateTime.Now;
                            formData.formGroupGHdr.UpdatedBy = userDetail.UserID;
                            formData.formGroupGHdr.FormID = formData.FormID;

                            ///====  Adding auto push workflow data ==== 
                            formData.FormState = formData.formGroupGHdr.FormState;
                            formData.FormGroupID = formData.formGroupGHdr.FormGroupID;
                            AddAutoPushWorkFlow(formData);
                            ///=====   end  ============================

                            flag = appraisalService.UpdateFormGData(formData);
                            var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                            return Json(new { msgType = "success", formState = formData.formGroupGHdr.FormState, msg = msg }, JsonRequestBehavior.AllowGet);
                        }
                        // return PartialView("_FormGroupH", formData);
                    }
                    else
                        formData.formGroupGHdr.FormState = formData.FormState;
                    formData.isValid = false;
                    return PartialView("_FormGroupG", formData);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region FORM GROUP H

        public ActionResult _GetFormGroupH(Model.AppraisalForm appraisal)
        {
            log.Info($"AppraisalController/_GetFormGroupH");
            try
            {
                var reportingYr = appraisal.ReportingYr;//  string.IsNullOrEmpty(appraisal.ReportingYr) ? DateTime.Now.GetFinancialYr() : appraisal.ReportingYr;
                if (!appraisal.isAdmin)
                    appraisal.loggedInEmpID = userDetail.EmployeeID;
                else
                {
                    if (appraisal.AcceptanceAuthorityTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.AcceptanceAuthorityTo.Value;
                    else if (appraisal.ReviewingTo.HasValue)
                        appraisal.loggedInEmpID = appraisal.ReviewingTo.Value;
                    else
                        appraisal.loggedInEmpID = appraisal.ReportingTo.Value;
                }
                //appraisal.loggedInEmpID = userDetail.EmployeeID;
                appraisal.formGroupHHdr = appraisalService.GetFormGroupHHdrDetail("FormGroupHHdr", null, appraisal.EmployeeID, reportingYr, null);

                appraisal.FormID = appraisal?.formGroupHHdr?.FormID > 0 ? appraisal.formGroupHHdr.FormID : appraisal.FormID;
                appraisal.EmployeeID = appraisal.formGroupHHdr?.EmployeeID > 0 ? appraisal.formGroupHHdr.EmployeeID : appraisal.EmployeeID;

                if (appraisal.formGroupHHdr != null)
                    appraisal.formGroupHHdr.ReportingYr =
                     appraisal.formGroupHHdr != null && !string.IsNullOrEmpty(appraisal.formGroupHHdr.ReportingYr) ? appraisal.formGroupHHdr.ReportingYr : reportingYr;

                else
                    appraisal.formGroupHHdr = new Model.FormGroupHHdr() { ReportingYr = reportingYr };

                appraisal.isValid = true;
                return PartialView("_FormGroupH", appraisal);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostFormHData(Model.AppraisalForm formData, string ButtonType)
        {
            log.Info($"AppraisalController/_PostFormHData");
            try
            {
                bool flag;
                if (!formData.ReportingTo.HasValue || formData.ReportingTo.Value == 0)
                {
                    var errormsg = "You can not apply for appraisal right now because either your Reporting or Reviewing Manager is not set.";
                    return Json(new { formState = formData.formGroupHHdr.FormState, part = 0, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupH", formData) }, JsonRequestBehavior.AllowGet);
                }

                if (ButtonType == "Save")
                {
                    var formNewAttributes = GetFormAttributes(formData, ButtonType);
                    formData.submittedBy = formNewAttributes.SubmittedBy;
                    formData.formGroupHHdr.FormState = formNewAttributes.FormState;

                    if (!appraisalService.AppraisalDataExists(formData.EmployeeID, formData.FormID, formData.formGroupHHdr.ReportingYr))
                    {
                        formData.formGroupHHdr.CreatedOn = DateTime.Now;
                        formData.formGroupHHdr.CreatedBy = userDetail.UserID;
                        formData.formGroupHHdr.FormID = formData.FormID;
                        formData.formGroupHHdr.DepartmentID = userDetail.DepartmentID;
                        formData.formGroupHHdr.DesignationID = userDetail.DesignationID;
                        flag = appraisalService.InsertFormHData(formData);

                        return Json(new { msgType = "success", formState = formData.formGroupHHdr.FormState, msg = "Form saved successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        formData.formGroupHHdr.UpdatedOn = DateTime.Now;
                        formData.formGroupHHdr.UpdatedBy = userDetail.UserID;
                        formData.formGroupHHdr.FormID = formData.FormID;

                        ///====  Adding auto push workflow data ==== 
                        formData.FormState = formData.formGroupHHdr.FormState;
                        formData.FormGroupID = formData.formGroupHHdr.FormGroupID;
                        AddAutoPushWorkFlow(formData);
                        ///=====   end  ============================

                        flag = appraisalService.UpdateFormHData(formData);
                        var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                        return Json(new { msgType = "success", formState = formData.formGroupHHdr.FormState, msg = msg }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //if (!appraisalService.IsTrainingSubmitted(formData.EmployeeID, formData.formGroupHHdr.ReportingYr))
                    //{
                    //    var errormsg = "You can not apply for appraisal right now, please first apply for skill assessment.";
                    //    return Json(new { isTraining = 1, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_FormGroupH", formData) }, JsonRequestBehavior.AllowGet);
                    //}
                    Model.ProcessWorkFlow workFlow = null;
                    if (formData.EmployeeID == formData.loggedInEmpID)
                    {
                        // formData.submittedBy = Model.SubmittedBy.Employee;
                        //  formData.formGroupHHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByEmployee : (int)Model.AppraisalFormState.SubmitedByEmployee);
                        ModelState.Remove("formGroupHHdr.ReportingOfficerRemark");
                        ModelState.Remove("formGroupHHdr.ReviewingOfficerRemark");
                        ModelState.Remove("formGroupHHdr.AcceptanceAuthorityRemark");
                    }
                    else if (formData.ReportingTo == formData.loggedInEmpID)
                    {
                        // formData.submittedBy = Model.SubmittedBy.ReportingOfficer;
                        //  formData.formGroupHHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReporting : (int)Model.AppraisalFormState.SubmitedByReporting);

                        ModelState.Remove("formGroupHHdr.AcceptanceAuthorityRemark");
                        if (formData.ReviewingTo != formData.ReportingTo)
                        {
                            ModelState.Remove("formGroupHHdr.AcceptanceAuthorityComment");
                            ModelState.Remove("formGroupHHdr.ReviewingOfficerRemark");
                        }
                    }

                    else if (formData.ReviewingTo == formData.loggedInEmpID)
                    {

                        ModelState.Remove("formGroupHHdr.ReportingOfficerRemark");
                        // ModelState.Remove("formGroupHHdr.AcceptanceAuthorityComment");
                        //  formData.submittedBy = Model.SubmittedBy.ReviewingOfficer;
                        //  formData.formGroupHHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                        if (formData.ReviewingTo != formData.AcceptanceAuthorityTo)
                        {
                            ModelState.Remove("formGroupHHdr.AcceptanceAuthorityRemark");
                        }

                    }
                    else if (formData.AcceptanceAuthorityTo == formData.loggedInEmpID)
                    {
                        ModelState.Remove("formGroupHHdr.ReportingOfficerRemark");
                        ModelState.Remove("formGroupHHdr.ReviewingOfficerRemark");
                        //  formData.submittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        //  formData.formGroupHHdr.FormState = (ButtonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                    }

                    if (formData.ReviewingTo == formData.AcceptanceAuthorityTo && formData.loggedInEmpID == formData.ReviewingTo)
                    {
                        if (string.IsNullOrEmpty(formData.formGroupHHdr.AcceptanceAuthorityRemark))
                        {
                            ModelState.AddModelError("formGroupHHdr.AcceptanceAuthorityRemark", "The Acceptance Authority Officer Remark field is required.");
                        }
                    }

                    if (formData.ReportingTo == formData.ReviewingTo && formData.ReviewingTo == formData.AcceptanceAuthorityTo && formData.loggedInEmpID == formData.ReportingTo)
                    {
                        if (string.IsNullOrEmpty(formData.formGroupHHdr.AcceptanceAuthorityRemark))
                        {
                            ModelState.AddModelError("formGroupHHdr.AcceptanceAuthorityRemark", "The Acceptance Authority Officer Remark field is required.");
                        }
                    }

                    formData.formGroupHHdr.CreatedBy = userDetail.UserID;
                    formData.formGroupHHdr.EmployeeID = formData.formGroupHHdr.EmployeeID > 0 ? formData.formGroupHHdr.EmployeeID : (int)userDetail.EmployeeID;

                    if (formData.FormID == 0)
                        formData.FormID = (int)userDetail.AppraisalFormID;

                    if (formData.formGroupHHdr.WorkedPeriodUnderROFrom.HasValue && formData.formGroupHHdr.WorkedPeriodUnderROTo.HasValue)
                    {
                        if (formData.formGroupHHdr.WorkedPeriodUnderROTo.Value < formData.formGroupHHdr.WorkedPeriodUnderROFrom.Value)
                        {
                            ModelState.AddModelError("InValidFromDate", "From date should be always less than To date.");
                            formData.isValid = false;
                            return PartialView("_FormGroupH", formData);
                        }
                    }
                    if (!formData.formGroupHHdr.WorkedPeriodUnderROFrom.HasValue)
                    {
                        ModelState.AddModelError("InValidFromDate", "Please select date.");
                        //return PartialView("_FormGroupH", formData);
                    }
                    else if (!formData.formGroupHHdr.WorkedPeriodUnderROTo.HasValue)
                    {
                        ModelState.AddModelError("InValidFromDate", "Please select date.");
                        //return PartialView("_FormGroupH", formData);
                    }
                    //var FF = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                    appraisalService.SetSectionValue(formData);
                    RemoveModelError(formData);
                    if (ModelState.IsValid)
                    {
                        if (formData.formGroupHHdr.WorkedPeriodUnderROFrom.HasValue && formData.formGroupHHdr.WorkedPeriodUnderROTo.HasValue)
                        {
                            if (formData.formGroupHHdr.WorkedPeriodUnderROTo.Value < formData.formGroupHHdr.WorkedPeriodUnderROFrom.Value)
                            {
                                ModelState.AddModelError("InValidFromDate", "From date should be always less than To date.");
                                formData.isValid = false;
                                return PartialView("_FormGroupH", formData);
                            }
                        }
                        var formNewAttributes = GetFormAttributes(formData, ButtonType);
                        formData.submittedBy = formNewAttributes.SubmittedBy;
                        formData.formGroupHHdr.FormState = formNewAttributes.FormState;

                        if (!appraisalService.AppraisalDataExists(formData.EmployeeID, formData.FormID, formData.formGroupHHdr.ReportingYr))
                        {
                            formData.formGroupHHdr.CreatedOn = DateTime.Now;
                            formData.formGroupHHdr.CreatedBy = userDetail.UserID;
                            formData.formGroupHHdr.FormID = formData.FormID;
                            formData.formGroupHHdr.DepartmentID = userDetail.DepartmentID;
                            formData.formGroupHHdr.DesignationID = userDetail.DesignationID;
                            workFlow = new Model.ProcessWorkFlow()
                            {
                                SenderID = userDetail.EmployeeID,
                                ReceiverID = formNewAttributes.ReciverID,
                                SenderDepartmentID = userDetail.DepartmentID,
                                SenderDesignationID = userDetail.DesignationID,
                                CreatedBy = userDetail.UserID,
                                EmployeeID = formData.EmployeeID,
                                Scomments = $"Form H , Action performed by : {(Model.SubmittedBy)(int)formData.submittedBy}.",
                                ProcessID = (int)WorkFlowProcess.Appraisal,
                                StatusID = (int)formData.formGroupHHdr.FormState
                            };
                            formData._ProcessWorkFlow = workFlow;

                            flag = appraisalService.InsertFormHData(formData);

                            return Json(new { msgType = "success", formState = formData.formGroupHHdr.FormState, msg = "Form submitted successfully." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            workFlow = new Model.ProcessWorkFlow()
                            {
                                SenderID = userDetail.EmployeeID,
                                ReceiverID = formNewAttributes.ReciverID,
                                SenderDepartmentID = userDetail.DepartmentID,
                                SenderDesignationID = userDetail.DesignationID,
                                CreatedBy = userDetail.UserID,
                                EmployeeID = formData.EmployeeID,
                                ProcessID = (int)WorkFlowProcess.Appraisal,
                                StatusID = (int)formData.formGroupHHdr.FormState,
                                Scomments = $"Form H, Appraisal Form Updated By : {(Model.SubmittedBy)(int)formData.submittedBy}"
                            };

                            formData._ProcessWorkFlow = workFlow;
                            formData.formGroupHHdr.UpdatedOn = DateTime.Now;
                            formData.formGroupHHdr.UpdatedBy = userDetail.UserID;
                            formData.formGroupHHdr.FormID = formData.FormID;

                            ///====  Adding auto push workflow data ==== 
                            formData.FormState = formData.formGroupHHdr.FormState;
                            formData.FormGroupID = formData.formGroupHHdr.FormGroupID;
                            AddAutoPushWorkFlow(formData);
                            ///=====   end  ============================

                            flag = appraisalService.UpdateFormHData(formData);
                            var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                            return Json(new { msgType = "success", formState = formData.formGroupHHdr.FormState, msg = msg }, JsonRequestBehavior.AllowGet);
                        }
                        // return PartialView("_FormGroupH", formData);
                    }
                    else
                        formData.formGroupGHdr.FormState = formData.FormState;
                    formData.isValid = false;
                    return PartialView("_FormGroupH", formData);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        [HttpPost]
        public ActionResult _FormGroupPart2FormData(List<Model.FormGroupDetail1> formData)
        {
            formData.Add(new Nafed.MicroPay.Model.FormGroupDetail1());
            return PartialView("_FormGroupPart2GridView", formData);
        }

        [HttpGet]
        public ActionResult _GetFormGroupPart2FormData(List<Model.FormGroupDetail1> formData)
        {
            log.Info($"AppraisalController/_GetFormGroupA");
            try
            {
                return PartialView("_FormGroupPart2GridView", formData);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _RemoveTargetRow(int sNo)
        {
            var modelData = (Model.AppraisalForm)TempData["frmGroupPart2Data"];
            if (modelData != null)
            {
                var deletedRow = modelData.formGroupADetail1List.Where(x => x.sno == sNo).FirstOrDefault();
                modelData.formGroupADetail1List.Remove(deletedRow);
                TempData["frmGroupPart2Data"] = modelData;
                return PartialView("_FormGroupPart2GridView", modelData);
            }
            return Content("");

        }

        [HttpGet]
        public ActionResult _RemoveTrainingRow(int sNo)
        {
            var modelData = (Model.AppraisalForm)TempData["frmGroupTrainingData"];
            if (modelData != null)
            {
                var deletedRow = modelData.formGroupATrainingDtls.Where(x => x.sno == sNo).FirstOrDefault();
                modelData.formGroupATrainingDtls.Remove(deletedRow);
                TempData["frmGroupTrainingData"] = modelData;
                return PartialView("_FormGroupTrainingGridView", modelData);
            }
            return Content("");

        }

        [NonAction]
        private Model.FormRulesAttributes GetFormAttributes(Model.AppraisalForm appForm, string buttonType)
        {
            log.Info($"AppraisalController/GetFormAttributes");
            try
            {
                var reportingYr = appForm.ReportingYr; // DateTime.Now.GetFinancialYr();
                Model.FormRulesAttributes formRules = new Model.FormRulesAttributes();
                formRules = appraisalService.GetFormRulesAttributes((AppraisalForm)appForm.FormID, appForm.EmployeeID, reportingYr);
                if (appForm.ApprovalHierarchy == 1)
                {
                    if (appForm.loggedInEmpID == appForm.ReportingTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByReporting : (int)Model.AppraisalFormState.SubmitedByReporting);
                        formRules.SubmittedBy = Model.SubmittedBy.ReportingOfficer;
                        formRules.SenderID = appForm.ReportingTo;
                        formRules.ReciverID = appForm.ReviewingTo;
                    }
                    else if (appForm.loggedInEmpID == appForm.ReviewingTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                        formRules.SubmittedBy = Model.SubmittedBy.ReviewingOfficer;
                        formRules.SenderID = appForm.ReviewingTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
                    }
                    else if (appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = appForm.AcceptanceAuthorityTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
                    }
                    else
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByEmployee : (int)Model.AppraisalFormState.SubmitedByEmployee);
                        formRules.SubmittedBy = Model.SubmittedBy.Employee;
                        formRules.SenderID = appForm.EmployeeID;
                        formRules.ReciverID = appForm.ReportingTo;
                    }

                }
                else if (appForm.ApprovalHierarchy == 2)
                {
                    if (appForm.loggedInEmpID == appForm.ReviewingTo && appForm.AcceptanceAuthorityTo != null)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByReviewer : (int)Model.AppraisalFormState.SubmitedByReviewer);
                        formRules.SubmittedBy = Model.SubmittedBy.ReviewingOfficer;
                        formRules.SenderID = appForm.ReviewingTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
                    }
                    else if (appForm.loggedInEmpID == appForm.ReviewingTo && appForm.AcceptanceAuthorityTo == null)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = appForm.AcceptanceAuthorityTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
                    }
                    else if (appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = appForm.AcceptanceAuthorityTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
                    }
                }
                else if (appForm.ApprovalHierarchy == 2.1 || appForm.ApprovalHierarchy == 3)
                {
                    if (appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByAcceptanceAuth : (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth);
                        formRules.SubmittedBy = Model.SubmittedBy.AcceptanceAuthority;
                        formRules.SenderID = appForm.AcceptanceAuthorityTo;
                        formRules.ReciverID = appForm.AcceptanceAuthorityTo;
                    }
                    else if (appForm.loggedInEmpID == appForm.EmployeeID)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.AppraisalFormState.SavedByEmployee : (int)Model.AppraisalFormState.SubmitedByEmployee);
                        formRules.SubmittedBy = Model.SubmittedBy.Employee;
                        formRules.SenderID = appForm.EmployeeID;
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

        //public ActionResult SkillSetDefine()
        //{
        //    var reportingYr = DateTime.Now.GetFinancialYr();
        //    if (!appraisalService.IsTrainingSubmitted((int)userDetail.EmployeeID, reportingYr))
        //    {
        //        return Json(false);
        //    }
        //    else
        //    {
        //        return Json(true); ;
        //    }
        //}

        public ActionResult ViewAPAR(int? appraisalFormID, int? reportingTo, int? reviewingTo, int? acceptanceAuthority, int? empID, string ryr)
        {
            log.Info($"AppraisalController/AppraisalForm");
            try
            {

                //if (!appraisalService.IsTrainingSubmitted((int)empID, reportingYr))
                //{
                //    //   var errormsg = "You can not apply for appraisal right now, please first apply for skill assessment.";
                //    return Json(new { type = 1, msgType = "error" }, JsonRequestBehavior.AllowGet);
                //}

                if (appraisalFormID.HasValue)
                {
                    var reportingYr = ryr;
                    Model.AppraisalForm appForm = new Model.AppraisalForm();
                    appForm.FormID = appraisalFormID.Value;
                    appForm.ReportingTo = reportingTo;
                    appForm.ReviewingTo = reviewingTo;
                    appForm.EmployeeID = (int)empID;
                    appForm.AcceptanceAuthorityTo = acceptanceAuthority;
                    appForm.ReportingYr = reportingYr;
                    appForm.isAdmin = true;
                    //appForm.loggedInEmpID = acceptanceAuthority;

                    if (acceptanceAuthority.HasValue)
                        appForm.loggedInEmpID = acceptanceAuthority;
                    else if (reviewingTo.HasValue)
                        appForm.loggedInEmpID = reviewingTo;
                    else
                        appForm.loggedInEmpID = reportingTo;


                    appForm.frmAttributes = appraisalService.GetFormRulesAttributes((AppraisalForm)appraisalFormID.Value, appForm.EmployeeID, reportingYr);

                    if (appForm.frmAttributes != null)
                    {
                        if ((appForm.ReportingTo == appForm.ReviewingTo) && (appForm.ReviewingTo == appForm.AcceptanceAuthorityTo))
                            appForm.ApprovalHierarchy = 3;
                        else if (((appForm.ReportingTo != appForm.ReviewingTo) && (appForm.ReviewingTo == appForm.AcceptanceAuthorityTo))
                            && appForm.loggedInEmpID == appForm.ReviewingTo)
                            appForm.ApprovalHierarchy = 2.1;
                        else if (((appForm.ReportingTo == appForm.ReviewingTo) && (appForm.ReviewingTo != appForm.AcceptanceAuthorityTo))
                            && (appForm.loggedInEmpID == appForm.ReportingTo || appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo))
                            appForm.ApprovalHierarchy = 2.0;
                        else
                            appForm.ApprovalHierarchy = 1;

                        if (appForm.frmAttributes.FormState == 1)
                        {
                            if (appForm.ApprovalHierarchy == 1 && appForm.loggedInEmpID == appForm.ReportingTo && DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate && DateTime.Now.Date < appForm.frmAttributes.ReportingSubmissionDate)
                                appForm.frmAttributes.EmployeeSection = true;
                            else if (appForm.ApprovalHierarchy == 1 && appForm.loggedInEmpID == appForm.ReviewingTo &&
                                (DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate && DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate) && DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate)
                            {
                                appForm.frmAttributes.EmployeeSection = true;
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 1 && appForm.loggedInEmpID == appForm.AcceptanceAuthorityTo &&
                                (DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate && DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate && DateTime.Now.Date > appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.EmployeeSection = true;
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 2.0 && appForm.loggedInEmpID == reviewingTo && (DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate && DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.EmployeeSection = true;
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.ReviewerButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 2.1 && appForm.loggedInEmpID == acceptanceAuthority &&
                                (DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate && DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.EmployeeSection = true;
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 3 && appForm.loggedInEmpID == acceptanceAuthority && (DateTime.Now.Date > appForm.frmAttributes.EmployeeSubmissionDate && DateTime.Now.Date < appForm.frmAttributes.AcceptanceSubmissionDate))
                            {
                                appForm.frmAttributes.EmployeeSection = true;
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }

                        }
                        else if (appForm.frmAttributes.FormState == 2 || appForm.frmAttributes.FormState == 3)
                        {
                            if (appForm.ApprovalHierarchy == 2.0 && appForm.loggedInEmpID == reviewingTo && (DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.ReviewerButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 2.0 && appForm.loggedInEmpID == acceptanceAuthority && (DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 2.1 && appForm.loggedInEmpID == acceptanceAuthority && (DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate))
                            {
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 3 && appForm.loggedInEmpID == acceptanceAuthority && DateTime.Now.Date > appForm.frmAttributes.ReportingSubmissionDate)
                            {
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.AcceptanceButton = true;
                            }
                        }
                        else if (appForm.frmAttributes.FormState == 4 || appForm.frmAttributes.FormState == 5)
                        {
                            if (appForm.ApprovalHierarchy == 1 && appForm.loggedInEmpID == reviewingTo && (DateTime.Now.Date < appForm.frmAttributes.ReviewerSubmissionDate))
                            {
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.ReviewerButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 2.0 && appForm.loggedInEmpID == reviewingTo && (DateTime.Now.Date < appForm.frmAttributes.ReportingSubmissionDate))
                            {
                                appForm.frmAttributes.ReportingSection = true;
                                appForm.frmAttributes.ReviewerSection = true;
                                appForm.frmAttributes.ReviewerButton = true;
                            }
                            else if (appForm.ApprovalHierarchy == 2.1 && appForm.loggedInEmpID == acceptanceAuthority && (DateTime.Now.Date > appForm.frmAttributes.ReviewerSubmissionDate && DateTime.Now.Date < appForm.frmAttributes.AcceptanceSubmissionDate))
                            {
                                appForm.frmAttributes.AcceptanceButton = true;
                            }
                        }
                    }
                    if (appraisalFormID.Value == (int)AppraisalForm.FormA)
                        ViewBag.Title = "A.P.A.R - Form A";
                    else if (appraisalFormID.Value == (int)AppraisalForm.FormB)
                        ViewBag.Title = "A.P.A.R - Form B";
                    else if (appraisalFormID.Value == (int)AppraisalForm.FormC)
                        ViewBag.Title = "A.P.A.R - Form C";
                    else if (appraisalFormID.Value == (int)AppraisalForm.FormD)
                        ViewBag.Title = "A.P.A.R - Form D";
                    else if (appraisalFormID.Value == (int)AppraisalForm.FormE)
                        ViewBag.Title = "A.P.A.R - Form E";

                    else if (appraisalFormID.Value == (int)AppraisalForm.FormF)
                        ViewBag.Title = "A.P.A.R - Form F";

                    else if (appraisalFormID.Value == (int)AppraisalForm.FormG)
                        ViewBag.Title = "A.P.A.R - Form G";
                    else if (appraisalFormID.Value == (int)AppraisalForm.FormH)
                        ViewBag.Title = "A.P.A.R - Form H";
                    else
                        return Content("");

                    return View("AppraisalFormContainer", appForm);
                }
                else
                    return Content("");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void RemoveModelError(Model.AppraisalForm roformData)
        {
            if ((roformData.FormID != (int)AppraisalForm.FormD) && (roformData.FormID != (int)AppraisalForm.FormG) && (roformData.FormID != (int)AppraisalForm.FormH))
            {
                if (roformData.loggedInEmpID == roformData.ReportingTo)
                {
                    if (!roformData.empSection)
                    {
                        ModelState.Remove("formGroupAHdr.PART2_1");
                    }
                }
                if (roformData.loggedInEmpID == roformData.ReviewingTo)
                {
                    if (!roformData.empSection)
                    {
                        ModelState.Remove("formGroupAHdr.PART2_1");
                    }
                    if (!roformData.reportingSection)
                    {
                        ModelState.Remove("formGroupAHdr.PART4_6_Weightage");
                        ModelState.Remove("formGroupAHdr.PART4_5_Remark");
                        ModelState.Remove("formGroupAHdr.PART4_4_Remark");
                        ModelState.Remove("formGroupAHdr.PART4_3_Remark");
                        ModelState.Remove("formGroupAHdr.PART4_1_Remark");
                        ModelState.Remove("formGroupADetail2List[0].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[1].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[2].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[3].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[4].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[5].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[6].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[7].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[8].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[9].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[10].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[11].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[12].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[13].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[14].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[15].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[16].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[17].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[18].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[19].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[20].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[21].ReportingAuthorityWeightage");
                    }
                }
                if (roformData.loggedInEmpID == roformData.AcceptanceAuthorityTo)
                {
                    if (!roformData.empSection)
                    {
                        ModelState.Remove("formGroupAHdr.PART2_1");
                    }
                    if (!roformData.reportingSection || roformData.ApprovalHierarchy == 2)
                    {
                        ModelState.Remove("formGroupAHdr.PART4_6_Weightage");
                        ModelState.Remove("formGroupAHdr.PART4_5_Remark");
                        ModelState.Remove("formGroupAHdr.PART4_4_Remark");
                        ModelState.Remove("formGroupAHdr.PART4_3_Remark");
                        ModelState.Remove("formGroupAHdr.PART4_1_Remark");
                        ModelState.Remove("formGroupADetail2List[0].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[1].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[2].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[3].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[4].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[5].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[6].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[7].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[8].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[9].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[10].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[11].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[12].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[13].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[14].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[15].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[16].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[17].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[18].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[19].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[20].ReportingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[21].ReportingAuthorityWeightage");
                    }
                    if (!roformData.reviewingSection || roformData.ApprovalHierarchy == 2)
                    {
                        ModelState.Remove("formGroupAHdr.PART5_5_Weightage");
                        ModelState.Remove("formGroupAHdr.PART5_1");
                        ModelState.Remove("formGroupADetail2List[0].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[1].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[2].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[3].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[4].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[5].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[6].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[7].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[8].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[9].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[10].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[11].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[12].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[13].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[14].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[15].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[16].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[17].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[18].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[19].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[20].ReviewingAuthorityWeightage");
                        ModelState.Remove("formGroupADetail2List[21].ReviewingAuthorityWeightage");
                    }
                }
            }
            else if (roformData.FormID == (int)AppraisalForm.FormD)
            {
                if (roformData.loggedInEmpID == roformData.ReviewingTo)
                {
                    if (!roformData.empSection)
                    {
                        ModelState.Remove("InValidFromDate");
                        ModelState.Remove("formGroupDHdr.Part2_1");
                    }
                    if (!roformData.reportingSection)
                    {
                        ModelState.Remove("formGroupDHdr.Part3_2_A_1");
                        ModelState.Remove("formGroupDHdr.Part3_2_A_2");
                        ModelState.Remove("formGroupDHdr.Part3_2_A_3");
                        ModelState.Remove("formGroupDHdr.Part3_2_A_4");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_1");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_2");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_3");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_4");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_5");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_6");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_7");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_8");

                        ModelState.Remove("formGroupDHdr.Part3_2_C_1");
                        ModelState.Remove("formGroupDHdr.Part3_2_C_2");
                        ModelState.Remove("formGroupDHdr.Part3_2_C_3");
                        ModelState.Remove("formGroupDHdr.Part3_2_C_4");
                        ModelState.Remove("formGroupDHdr.Part3_2_C_5");
                        ModelState.Remove("formGroupDHdr.Part3_2_C_6");
                        ModelState.Remove("formGroupDHdr.Part4_1");
                        ModelState.Remove("formGroupDHdr.Part4_3");
                        ModelState.Remove("formGroupDHdr.Part4_4");
                    }
                }

                if (roformData.loggedInEmpID == roformData.AcceptanceAuthorityTo)
                {
                    if (!roformData.empSection)
                    {
                        ModelState.Remove("InValidFromDate");
                        ModelState.Remove("formGroupDHdr.Part2_1");
                    }
                    if (!roformData.reportingSection)
                    {
                        ModelState.Remove("formGroupDHdr.Part3_2_A_1");
                        ModelState.Remove("formGroupDHdr.Part3_2_A_2");
                        ModelState.Remove("formGroupDHdr.Part3_2_A_3");
                        ModelState.Remove("formGroupDHdr.Part3_2_A_4");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_1");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_2");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_3");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_4");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_5");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_6");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_7");
                        ModelState.Remove("formGroupDHdr.Part3_2_B_8");

                        ModelState.Remove("formGroupDHdr.Part3_2_C_1");
                        ModelState.Remove("formGroupDHdr.Part3_2_C_2");
                        ModelState.Remove("formGroupDHdr.Part3_2_C_3");
                        ModelState.Remove("formGroupDHdr.Part3_2_C_4");
                        ModelState.Remove("formGroupDHdr.Part3_2_C_5");
                        ModelState.Remove("formGroupDHdr.Part3_2_C_6");
                        ModelState.Remove("formGroupDHdr.Part4_1");
                        ModelState.Remove("formGroupDHdr.Part4_3");
                        ModelState.Remove("formGroupDHdr.Part4_4");
                    }
                    if (!roformData.reviewingSection)
                    {
                        ModelState.Remove("formGroupDHdr.Part5_1");
                    }
                }
            }
            else if (roformData.FormID == (int)AppraisalForm.FormG)
            {
                if (roformData.loggedInEmpID == roformData.ReviewingTo)
                {
                    if (!roformData.reportingSection)
                    {
                        ModelState.Remove("InValidFromDate");
                        ModelState.Remove("formGroupGHdr.ReportingOfficeComment");
                    }
                }
                if (roformData.loggedInEmpID == roformData.AcceptanceAuthorityTo)
                {
                    if (!roformData.reportingSection)
                    {
                        ModelState.Remove("InValidFromDate");
                        ModelState.Remove("formGroupGHdr.ReportingOfficeComment");
                    }
                    if (!roformData.reviewingSection)
                    {
                        ModelState.Remove("formGroupGHdr.ReviewingOfficerComment");
                    }
                }
            }
            else if (roformData.FormID == (int)AppraisalForm.FormH)
            {
                if (roformData.loggedInEmpID == roformData.ReviewingTo)
                {
                    if (!roformData.reportingSection)
                    {
                        ModelState.Remove("InValidFromDate");
                        ModelState.Remove("formGroupHHdr.ReportingOfficerRemark");
                    }
                }
                if (roformData.loggedInEmpID == roformData.AcceptanceAuthorityTo)
                {
                    if (!roformData.reportingSection)
                    {
                        ModelState.Remove("InValidFromDate");
                        ModelState.Remove("formGroupHHdr.ReportingOfficerRemark");
                    }
                    if (!roformData.reviewingSection)
                    {
                        ModelState.Remove("formGroupHHdr.ReviewingOfficerRemark");
                    }
                }
            }
        }

        private void AddAutoPushWorkFlow(Model.AppraisalForm appForm)
        {
            log.Info("$Appraisal/AddAutoPushWorkFlow");
            try
            {
                var formSubmissionDates = appraisalService.GetAppraisalForms().FirstOrDefault(x => x.FormID == appForm.FormID);

                List<Model.ProcessWorkFlow> objProcessWrkFlow = new List<Model.ProcessWorkFlow>();

                if (appForm.ApprovalHierarchy == 1 && !appForm.reportingSection && appForm.FormState == (int)Model.AppraisalFormState.SubmitedByReviewer)
                {
                    objProcessWrkFlow.Add(
                        new Model.ProcessWorkFlow
                        {
                            Readflag = 0,
                            EmployeeID = appForm.EmployeeID,
                            CreatedBy = userDetail.UserID,
                            CreatedOn = DateTime.Now,
                            ProcessID = (int)WorkFlowProcess.Appraisal,
                            ReferenceID = appForm.FormGroupID,
                            StatusID = (int)Model.AppraisalFormState.SubmitedByReporting,
                            SenderID = appForm.ReportingTo,
                            ReceiverID = appForm.ReviewingTo,
                            Senddate = formSubmissionDates.ReportingSubmissionDueDate.Value.AddDays(1),
                            Scomments = $"{((AppraisalForm)appForm.FormID).ToString()}, Appraisal Form Submitted By : Auto push "
                        });
                }
                else if (appForm.FormState == (int)Model.AppraisalFormState.SubmitedByAcceptanceAuth)
                {
                    if (appForm.ApprovalHierarchy == 1)
                    {
                        if (!appForm.reportingSection)
                        {
                            objProcessWrkFlow.Add(
                                new Model.ProcessWorkFlow
                                {
                                    Readflag = 0,
                                    EmployeeID = appForm.EmployeeID,
                                    CreatedBy = userDetail.UserID,
                                    CreatedOn = DateTime.Now,
                                    ProcessID = (int)WorkFlowProcess.Appraisal,
                                    ReferenceID = appForm.FormGroupID,
                                    StatusID = (int)Model.AppraisalFormState.SubmitedByReporting,
                                    SenderID = appForm.ReportingTo,
                                    ReceiverID = appForm.ReviewingTo,
                                    Senddate = formSubmissionDates.ReportingSubmissionDueDate.Value.AddDays(1),
                                    Scomments = $"{((AppraisalForm)appForm.FormID).ToString()}, Appraisal Form Submitted By : Auto push "
                                });
                        }
                        if (!appForm.reviewingSection)
                        {
                            objProcessWrkFlow.Add(
                               new Model.ProcessWorkFlow
                               {
                                   Readflag = 0,
                                   EmployeeID = appForm.EmployeeID,
                                   CreatedBy = userDetail.UserID,
                                   CreatedOn = DateTime.Now,
                                   ProcessID = (int)WorkFlowProcess.Appraisal,
                                   ReferenceID = appForm.FormGroupID,
                                   StatusID = (int)Model.AppraisalFormState.SubmitedByReviewer,
                                   SenderID = appForm.ReviewingTo,
                                   ReceiverID = appForm.AcceptanceAuthorityTo,
                                   Senddate = formSubmissionDates.ReviewerSubmissionDueDate.Value.AddDays(1),
                                   Scomments = $"{((AppraisalForm)appForm.FormID).ToString()}, Appraisal Form Submitted By : Auto push "
                               });
                        }
                    }
                    else if (appForm.ApprovalHierarchy == 2.0)
                    {
                        if (!appForm.reviewingSection && !appForm.reportingSection)
                        {
                            objProcessWrkFlow.Add(
                                  new Model.ProcessWorkFlow
                                  {
                                      Readflag = 0,
                                      EmployeeID = appForm.EmployeeID,
                                      CreatedBy = userDetail.UserID,
                                      CreatedOn = DateTime.Now,
                                      ProcessID = (int)WorkFlowProcess.Appraisal,
                                      ReferenceID = appForm.FormGroupID,
                                      StatusID = (int)Model.AppraisalFormState.SubmitedByReviewer,
                                      SenderID = appForm.ReviewingTo,
                                      ReceiverID = appForm.AcceptanceAuthorityTo,
                                      Senddate = formSubmissionDates.ReviewerSubmissionDueDate.Value.AddDays(1),
                                      Scomments = $"{((AppraisalForm)appForm.FormID).ToString()}, Appraisal Form Submitted By : Auto push "
                                  });
                        }
                    }
                    else if (appForm.ApprovalHierarchy == 2.1)
                    {
                        if (!appForm.reportingSection)
                        {
                            objProcessWrkFlow.Add(
                                new Model.ProcessWorkFlow
                                {
                                    Readflag = 0,
                                    EmployeeID = appForm.EmployeeID,
                                    CreatedBy = userDetail.UserID,
                                    CreatedOn = DateTime.Now,
                                    ProcessID = (int)WorkFlowProcess.Appraisal,
                                    ReferenceID = appForm.FormGroupID,
                                    StatusID = (int)Model.AppraisalFormState.SubmitedByReporting,
                                    SenderID = appForm.ReportingTo,
                                    ReceiverID = appForm.ReviewingTo,
                                    Senddate = formSubmissionDates.ReportingSubmissionDueDate.Value.AddDays(1),
                                    Scomments = $"{((AppraisalForm)appForm.FormID).ToString()}, Appraisal Form Submitted By : Auto push "
                                });
                        }
                    }
                }
                appForm._AutoPushWorkFlow = objProcessWrkFlow;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult ARARTrainings()
        {
            log.Info("AppraisalController/ARARTrainings");
            try
            {
                Model.CommonFilter cFilter = new Model.CommonFilter();
                ViewBag.reportingYrs = ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                return View("APARTraining", cFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _GetAPAREmpTraining(Model.CommonFilter cfilter, string ButtonType)
        {
            log.Info("AppraisalController/_GetAPAREmpTraining");
            try
            {
                if (ButtonType == "Search")
                {
                    if (string.IsNullOrEmpty(cfilter.ReportingYear))
                    {
                        ModelState.AddModelError("ReportingYear", "Please select Reporting Year");
                        ViewBag.reportingYrs = ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                        return Json(new { success = false, htmlData = ConvertViewToString("_APARTrainingFilters", cfilter) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        List<Model.FormTrainingDtls> objTraining = new List<Model.FormTrainingDtls>();
                        objTraining = appraisalService.GetAPAREmployeeTrainings(null, cfilter.ReportingYear);
                        TempData["tempEmpTraining"] = objTraining;
                        return Json(new { success = true, htmlData = ConvertViewToString("_APARTrainingGridView", objTraining) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (ButtonType == "Export")
                {
                    string tFilter = string.Empty;
                    List<Model.FormTrainingDtls> empList = (List<Model.FormTrainingDtls>)TempData["tempEmpTraining"];
                    if (empList != null)
                    {
                        DataTable exportData = new DataTable();
                        string fileName = string.Empty, msg = string.Empty;
                        string fullPath = Server.MapPath("~/FileDownload/");
                        fileName = "Training Need From APAR Report" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;

                        DataColumn dtc0 = new DataColumn("S.No.");
                        DataColumn dtc1 = new DataColumn("Reporting Year");
                        DataColumn dtc2 = new DataColumn("Reporting Manager");
                        DataColumn dtc3 = new DataColumn("Name");
                        DataColumn dtc4 = new DataColumn("Branch");
                        DataColumn dtc5 = new DataColumn("Designation");
                        DataColumn dtc6 = new DataColumn("Training");
                        DataColumn dtc7 = new DataColumn("Comments");
                        exportData.Columns.Add(dtc0);
                        exportData.Columns.Add(dtc1);
                        exportData.Columns.Add(dtc2);
                        exportData.Columns.Add(dtc3);
                        exportData.Columns.Add(dtc4);
                        exportData.Columns.Add(dtc5);
                        exportData.Columns.Add(dtc6);
                        exportData.Columns.Add(dtc7);
                        for (int i = 0; i < empList.Count; i++)
                        {
                            DataRow row = exportData.NewRow();
                            row["S.No."] = i + 1;
                            row["Reporting Year"] = empList[i].ReportingYr;
                            row["Reporting Manager"] = empList[i].ReportingTo;
                            row["Name"] = empList[i].Name;
                            row["Branch"] = empList[i].BranchName;
                            row["Designation"] = empList[i].DesignationName;
                            row["Training"] = empList[i].FormTraining;
                            row["Comments"] = empList[i].Remark;
                            exportData.Rows.Add(row);
                        }
                        DataSet ds = new DataSet();
                        ds.Tables.Add(exportData);
                        appraisalService.ExportEmpAPARTraining(ds, fullPath, fileName);

                        fullPath = $"{fullPath}{fileName}";
                        return JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");

                    }
                    return new EmptyResult();
                }
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

    }
}