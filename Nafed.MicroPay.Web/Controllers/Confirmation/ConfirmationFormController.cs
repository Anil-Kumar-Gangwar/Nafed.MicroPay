using System;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Model = Nafed.MicroPay.Model;
using System.Linq;

namespace MicroPay.Web.Controllers.ConfirmationForm
{
    public class ConfirmationFormController : BaseController
    {
        private readonly IConfirmationFormService confirmationService;

        public ConfirmationFormController(IConfirmationFormService confirmationService)
        {
            this.confirmationService = confirmationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyConfirmationForms()
        {
            log.Info($"ConfirmationForm/MyConfirmationForms");
            try
            {
                ConfirmationFormViewModel conifrmFormVM = new ConfirmationFormViewModel();
                var frmList = confirmationService.GetConfirmationForms(userDetail.EmployeeID);
                conifrmFormVM.formList = frmList;
                return PartialView("_MyConfirmationFormList", conifrmFormVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int formTypeID, int? reportingTo, int? reviewingTo, int? processTypeID, int? empID, int formHdrID, int apid, int chdrid)
        {
            log.Info($"ConfirmationFormController/Edit");
            try
            {
                Model.ConfirmationFormHdr frmHdrDetail = new Model.ConfirmationFormHdr();
                frmHdrDetail.ReportingTo = reportingTo;
                frmHdrDetail.ReviewingTo = reviewingTo;
                frmHdrDetail.FormTypeID = formTypeID;
                frmHdrDetail.EmployeeID = empID.Value;
                frmHdrDetail.loggedInEmpID = userDetail.EmployeeID;
                frmHdrDetail.ProcessID = processTypeID.Value;
                frmHdrDetail.FormHdrID = formHdrID;
                frmHdrDetail.EmpProcessAppID = apid;
                frmHdrDetail.FormABHeaderID = chdrid;
                var reportingDetails = confirmationService.GetRTRVDetails(frmHdrDetail.ReportingTo);
                frmHdrDetail.ReportingName = reportingDetails.Name;
                frmHdrDetail.ReportingDesignation = reportingDetails.Designation;

                if (frmHdrDetail.ReviewingTo != null)
                {
                    var reviewingDetails = confirmationService.GetRTRVDetails(frmHdrDetail.ReviewingTo);
                    frmHdrDetail.ReviewingName = reviewingDetails.Name;
                    frmHdrDetail.ReviewingDesignation = reviewingDetails.Designation;
                }
                frmHdrDetail.confirmationFormRulesAttributes = confirmationService.GetConfirmationFormRulesAttributes(formTypeID, frmHdrDetail.EmployeeID, frmHdrDetail.ProcessID, frmHdrDetail.EmpProcessAppID, frmHdrDetail.FormABHeaderID);

                if (frmHdrDetail.confirmationFormRulesAttributes != null)
                {
                    if ((frmHdrDetail.ReportingTo == frmHdrDetail.ReviewingTo)
                       && (frmHdrDetail.loggedInEmpID == frmHdrDetail.ReportingTo || frmHdrDetail.loggedInEmpID == frmHdrDetail.ReviewingTo))
                        frmHdrDetail.ApprovalHierarchy = 2;
                    else
                        frmHdrDetail.ApprovalHierarchy = 1;

                    if (frmHdrDetail.confirmationFormRulesAttributes.FormState == 1)
                    {
                        if (frmHdrDetail.ApprovalHierarchy == 1 && frmHdrDetail.loggedInEmpID == frmHdrDetail.ReportingTo /*&& DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReportingSubmissionDate*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingButton = true;
                        }
                        else if (frmHdrDetail.ApprovalHierarchy == 1 && frmHdrDetail.loggedInEmpID == frmHdrDetail.ReviewingTo
                            /*&&(DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReviewerSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                        }
                        else if (frmHdrDetail.ApprovalHierarchy == 2 && frmHdrDetail.loggedInEmpID == frmHdrDetail.ReviewingTo /*&& (DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReviewerSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingButton = true;
                        }
                    }
                    else if (frmHdrDetail.confirmationFormRulesAttributes.FormState == 3)
                    {
                        if (frmHdrDetail.ApprovalHierarchy == 2 && frmHdrDetail.loggedInEmpID == reviewingTo
                            /*&& (DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReviewerSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                        }
                        else if (frmHdrDetail.ApprovalHierarchy == 1 && frmHdrDetail.loggedInEmpID == reviewingTo
                            /*&& (DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReviewerSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = false;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                        }
                    }
                    else if (frmHdrDetail.confirmationFormRulesAttributes.FormState == 4 || frmHdrDetail.confirmationFormRulesAttributes.FormState == 5 || frmHdrDetail.confirmationFormRulesAttributes.FormState == 6 || frmHdrDetail.confirmationFormRulesAttributes.FormState == 7)
                    {
                        if (frmHdrDetail.ApprovalHierarchy == 1 && frmHdrDetail.loggedInEmpID == reviewingTo
                            /*&& (DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReviewerSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                        }
                        else if (frmHdrDetail.ApprovalHierarchy == 2 && frmHdrDetail.loggedInEmpID == reviewingTo
                            /*&& (DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReportingSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                        }
                    }
                    else if (frmHdrDetail.confirmationFormRulesAttributes.FormState == 0 && frmHdrDetail.ReviewingTo.HasValue)
                    {
                        frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                        frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                    }
                    else if (frmHdrDetail.confirmationFormRulesAttributes.FormState == 2 && frmHdrDetail.loggedInEmpID == frmHdrDetail.ReviewingTo)
                    {
                        frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                        frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                    }
                }

                if (formTypeID == 1)
                    ViewBag.Title = "Confirmation - Form A";
                else if (formTypeID == 2)
                    ViewBag.Title = "Confirmation - Form B";
                return PartialView("ConfirmationFormContainer", frmHdrDetail);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _ConfirmationFormA(Model.ConfirmationFormHdr confirmation)
        {
            log.Info($"ConfirmationFormController/_GetFormGroupA");
            try
            {
                confirmation.loggedInEmpID = userDetail.EmployeeID;
                var confirmationFormList = confirmationService.GetEmployeeConfirmationList(confirmation.FormTypeID.Value, confirmation.ProcessID, confirmation.EmployeeID, confirmation.EmpProcessAppID, confirmation.FormABHeaderID);
                confirmation.EmployeeID = confirmationFormList.EmployeeId;
                confirmation.ProcessID = confirmationFormList.ProcessId;
                confirmationFormList.point8_1_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_1;
                confirmationFormList.point8_2_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_2;
                confirmationFormList.point8_3_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_3;
                confirmationFormList.point8_4_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_4;
                confirmationFormList.point8_5_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_5;
                confirmationFormList.point8_6_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_6;
                confirmationFormList.point8_7_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_7;
                confirmationFormList.point8_8_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_8;
                confirmationFormList.point8_9_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_9;
                confirmationFormList.point8_10_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_10;
                confirmationFormList.point8_11_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_11;
                confirmationFormList.point8_12_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_12;
                confirmationFormList.point8_13_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_13;
                confirmationFormList.RVpoint8_1_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_1;
                confirmationFormList.RVpoint8_2_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_2;
                confirmationFormList.RVpoint8_3_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_3;
                confirmationFormList.RVpoint8_4_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVRVPoint8_4;
                confirmationFormList.RVpoint8_5_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVRVPoint8_5;
                confirmationFormList.RVpoint8_6_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_6;
                confirmationFormList.RVpoint8_7_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_7;
                confirmationFormList.RVpoint8_8_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_8;
                confirmationFormList.RVpoint8_9_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_9;
                confirmationFormList.RVpoint8_10_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_10;
                confirmationFormList.RVpoint8_11_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_11;
                confirmationFormList.RVpoint8_12_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_12;
                confirmationFormList.RVpoint8_13_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_13;
                confirmation.confirmationFormDetail = confirmationFormList;
                var clarification = confirmationService.GetConfirmationClarification(confirmation.FormHdrID, confirmation.EmpProcessAppID, confirmation.FormTypeID.Value, confirmation.FormABHeaderID);
                confirmation.confirnationClarification = clarification;

                if (clarification?.Count > 0)
                    confirmation.confirmationFormDetail.ROClarificationRemark =
                        clarification.OrderByDescending(y => y.ClarificationID).First().ROClarification;

                TempData["ConfirmationFormA"] = confirmation;
                return PartialView("_ConfirmationFormA", confirmation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _ConfirmationFormB(Model.ConfirmationFormHdr confirmation)
        {
            log.Info($"ConfirmationFormController/_ConfirmationFormB");
            try
            {
                confirmation.loggedInEmpID = userDetail.EmployeeID;
                var confirmationFormList = confirmationService.GetEmployeeConfirmationList(confirmation.FormTypeID.Value, confirmation.ProcessID, confirmation.EmployeeID, confirmation.EmpProcessAppID, confirmation.FormABHeaderID);
                confirmation.EmployeeID = confirmationFormList.EmployeeId;
                confirmation.ProcessID = confirmationFormList.ProcessId;
                confirmationFormList.point8_1_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_1;
                confirmationFormList.point8_2_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_2;
                confirmationFormList.point8_3_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_3;
                confirmationFormList.point8_4_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_4;
                confirmationFormList.point8_5_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_5;
                confirmationFormList.point8_6_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_6;
                confirmationFormList.point8_7_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_7;
                confirmationFormList.point8_8_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_8;
                confirmationFormList.point8_9_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_9;
                confirmationFormList.point8_10_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_10;
                confirmationFormList.point8_11_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_11;
                confirmationFormList.point8_12_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_12;
                confirmationFormList.point8_13_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_13;

                confirmationFormList.RVpoint8_1_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_1;
                confirmationFormList.RVpoint8_2_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_2;
                confirmationFormList.RVpoint8_3_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_3;
                confirmationFormList.RVpoint8_4_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVRVPoint8_4;
                confirmationFormList.RVpoint8_5_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVRVPoint8_5;
                confirmationFormList.RVpoint8_6_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_6;
                confirmationFormList.RVpoint8_7_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_7;
                confirmationFormList.RVpoint8_8_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_8;
                confirmationFormList.RVpoint8_9_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_9;
                confirmationFormList.RVpoint8_10_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_10;
                confirmationFormList.RVpoint8_11_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_11;
                confirmationFormList.RVpoint8_12_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_12;
                confirmationFormList.RVpoint8_13_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_13;
                confirmation.confirmationFormDetail = confirmationFormList;
                var clarification = confirmationService.GetConfirmationClarification(confirmation.FormHdrID, confirmation.EmpProcessAppID, confirmation.FormTypeID.Value, confirmation.FormABHeaderID);
                confirmation.confirnationClarification = clarification;

                if (clarification?.Count > 0)
                    confirmation.confirmationFormDetail.ROClarificationRemark =
                        clarification.OrderByDescending(y => y.ClarificationID).First().ROClarification;

                TempData["ConfirmationFormB"] = confirmation;
                return PartialView("_ConfirmationFormB", confirmation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _GetConfirmationForm(int formTypeID, int? processTypeID, int? employeeID)
        {
            log.Info($"ConfirmationFormAController/_GetConfirmationForm");
            try
            {
                var confirmationFormList = confirmationService.GetEmployeeConfirmationList(formTypeID, processTypeID, employeeID, 0, 0);
                var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, (WorkFlowProcess)processTypeID);

                if (employeeID != approvalSettings.ReportingTo || employeeID != approvalSettings.ReviewingTo)
                    confirmationFormList.IsReviewingOfficer = false;
                confirmationFormList.FormTypeId = formTypeID;
                confirmationFormList.point8_1_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_1;
                confirmationFormList.point8_2_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_2;
                confirmationFormList.point8_3_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_3;
                confirmationFormList.point8_4_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_4;
                confirmationFormList.point8_5_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_5;
                confirmationFormList.point8_6_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_6;
                confirmationFormList.point8_7_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_7;
                confirmationFormList.point8_8_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_8;
                confirmationFormList.point8_9_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_9;
                confirmationFormList.point8_10_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_10;
                confirmationFormList.point8_11_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_11;
                confirmationFormList.point8_12_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_12;
                confirmationFormList.point8_13_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_13;
                confirmationFormList.RVpoint8_1_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_1;
                confirmationFormList.RVpoint8_2_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_2;
                confirmationFormList.RVpoint8_3_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_3;
                confirmationFormList.RVpoint8_4_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVRVPoint8_4;
                confirmationFormList.RVpoint8_5_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVRVPoint8_5;
                confirmationFormList.RVpoint8_6_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_6;
                confirmationFormList.RVpoint8_7_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_7;
                confirmationFormList.RVpoint8_8_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_8;
                confirmationFormList.RVpoint8_9_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_9;
                confirmationFormList.RVpoint8_10_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_10;
                confirmationFormList.RVpoint8_11_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_11;
                confirmationFormList.RVpoint8_12_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_12;
                confirmationFormList.RVpoint8_13_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_13;
                return PartialView("ConfirmationFormContainer", confirmationFormList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult PostConfirmationFormAData(Model.ConfirmationFormHdr confirmationFormHdr, string ButtonType)
        {
            log.Info($"ConfirmationFormAController/PostConfirmationFormAData");
            try
            {
                if (confirmationFormHdr.loggedInEmpID == confirmationFormHdr.ReportingTo && confirmationFormHdr.ApprovalHierarchy == 1)
                {
                    ModelState.Remove("confirmationFormDetail.ReviewingOfficerComment");

                    //=== added code to remove model validation for reviewing officer === 

                    ModelState.Remove("confirmationFormDetail.RVpoint8_1_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_2_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_3_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_4_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_5_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_6_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_7_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_8_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_9_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_10_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_11_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_12_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_13_Gr");



                }

                if (confirmationFormHdr.confirmationFormDetail.ConfirmationClarification
                    && confirmationFormHdr.loggedInEmpID == confirmationFormHdr.ReportingTo)
                {
                    if (string.IsNullOrEmpty(confirmationFormHdr.confirmationFormDetail.ROClarificationRemark))
                        ModelState.AddModelError("ROClarificationRequired", "Please Enter Clarification Remark");
                }
                var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();


                if (ButtonType.Equals("save", StringComparison.OrdinalIgnoreCase) || (confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption == Model.OptionType.Clarification && confirmationFormHdr.loggedInEmpID == confirmationFormHdr.ReviewingTo))
                {
                    ModelState.Remove("confirmationFormDetail.ReviewingOfficerComment");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_1_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_2_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_3_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_4_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_5_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_6_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_7_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_8_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_9_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_10_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_11_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_12_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_13_Gr");

                    ModelState.Remove("confirmationFormDetail.point8_1_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_2_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_3_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_4_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_5_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_6_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_7_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_8_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_9_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_10_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_11_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_12_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_13_Gr");


                }
                if (ModelState.IsValid)
                {
                    bool flag = false;


                    #region ==== Reviewing Officer  =============

                    if (confirmationFormHdr.confirmationFormDetail.RVpoint_9_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.RVPoint9 = true;
                    if (confirmationFormHdr.confirmationFormDetail.RVpoint_10_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.RVPoint10 = true;
                    if (confirmationFormHdr.confirmationFormDetail.RVpoint_11_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.RVPoint11 = true;
                    if (confirmationFormHdr.confirmationFormDetail.RVpoint_12_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.RVPoint12 = true;
                    #endregion
                    if (confirmationFormHdr.confirmationFormDetail.point_9_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.Point9 = true;
                    if (confirmationFormHdr.confirmationFormDetail.point_10_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.Point10 = true;
                    if (confirmationFormHdr.confirmationFormDetail.point_11_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.Point11 = true;
                    if (confirmationFormHdr.confirmationFormDetail.point_12_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.Point12 = true;
                    if (confirmationFormHdr.confirmationFormDetail.recommnded_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.ConfirmationRecommendedReporting = true;
                    if (confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.ConfirmationRecommended = true;
                    if (confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption == Model.OptionType.Clarification)
                        confirmationFormHdr.confirmationFormDetail.ConfirmationClarification = true;
                    else
                    {
                        confirmationFormHdr.confirmationFormDetail.ConfirmationClarification = false;
                        //  confirmationFormHdr.confirmationFormDetail.ReviewingOfficerComment = null;
                    }
                    confirmationFormHdr.confirmationFormDetail.Point8_1 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_1_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_2 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_2_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_3 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_3_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_4 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_4_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_5 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_5_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_6 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_6_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_7 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_7_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_8 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_8_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_9 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_9_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_10 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_10_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_11 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_11_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_12 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_12_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_13 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_13_Gr;
                    confirmationFormHdr.confirmationFormDetail.UpdatedBy = userDetail.UserID;
                    confirmationFormHdr.confirmationFormDetail.UpdatedOn = DateTime.Now;

                    confirmationFormHdr.confirmationFormDetail.RVPoint8_1 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_1_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_2 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_2_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_3 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_3_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVRVPoint8_4 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_4_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVRVPoint8_5 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_5_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_6 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_6_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_7 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_7_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_8 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_8_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_9 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_9_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_10 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_10_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_11 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_11_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_12 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_12_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_13 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_13_Gr;

                    var formNewAttributes = GetFormAttributes(confirmationFormHdr, ButtonType);
                    confirmationFormHdr.submittedBy = formNewAttributes.SubmittedBy;
                    confirmationFormHdr.confirmationFormDetail.FormState = formNewAttributes.FormState;

                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                    {
                        SenderID = userDetail.EmployeeID,
                        ReceiverID = formNewAttributes.ReciverID,
                        SenderDepartmentID = userDetail.DepartmentID,
                        SenderDesignationID = userDetail.DesignationID,
                        CreatedBy = userDetail.UserID,
                        EmployeeID = confirmationFormHdr.EmployeeID,
                        Scomments = $"Form A, Confirmation Form Submited By : {(Model.ConfirmationSubmittedBy)(int)confirmationFormHdr.submittedBy}",
                        ProcessID = confirmationFormHdr.ProcessID,
                        StatusID = (int)confirmationFormHdr.confirmationFormDetail.FormState
                    };
                    confirmationFormHdr._ProcessWorkFlow = workFlow;


                    if (confirmationFormHdr.confirmationFormDetail.ConfirmationClarification == true && ButtonType != "Save" && confirmationFormHdr.loggedInEmpID == confirmationFormHdr.ReviewingTo)
                    {
                        Model.ConfirmationClarification confirmationClarification = new Model.ConfirmationClarification()
                        {
                            FormHdrID = confirmationFormHdr.FormHdrID,
                            ClarificationRemarks = confirmationFormHdr.confirmationFormDetail.ReviewingOfficerComment,
                            CreatedBy = userDetail.UserID,
                            CreatedOn = DateTime.Now,
                            EmpProcessAppID = confirmationFormHdr.EmpProcessAppID,
                            FormABHeaderID = confirmationFormHdr.confirmationFormDetail.FormAHeaderId,
                            FormTypeID = (int)confirmationFormHdr.FormTypeID
                        };
                        confirmationFormHdr._ConfirmationClarification = confirmationClarification;
                    }

                    if (confirmationFormHdr.confirmationFormDetail.ConfirmationClarification
                        //&& ButtonType != "Save" 
                        && confirmationFormHdr.loggedInEmpID == confirmationFormHdr.ReportingTo)
                    {
                        Model.ConfirmationClarification confirmationClarification = new Model.ConfirmationClarification()
                        {
                            FormHdrID = confirmationFormHdr.FormHdrID,
                            ROClarification = confirmationFormHdr.confirmationFormDetail.ROClarificationRemark,
                            UpdatedBy = userDetail.UserID,
                            UpdatedOn = DateTime.Now,
                            //ClarificationRemarks = confirmationFormHdr.confirmationFormDetail.ReviewingOfficerComment,
                            //CreatedBy = userDetail.UserID,
                            //CreatedOn = DateTime.Now,
                            EmpProcessAppID = confirmationFormHdr.EmpProcessAppID,
                            FormABHeaderID = confirmationFormHdr.confirmationFormDetail.FormAHeaderId
                        };
                        confirmationFormHdr._ConfirmationClarification = confirmationClarification;
                    }
                    flag = confirmationService.UpdateFormAData(confirmationFormHdr);
                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                    return Json(new { formState = confirmationFormHdr.confirmationFormDetail.FormState, part = 0, msgType = "success", msg = msg }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var confirmation = (Model.ConfirmationFormHdr)TempData["ConfirmationFormA"];
                    TempData.Keep("ConfirmationFormA");
                    confirmation.confirmationFormDetail.recommnded_ReviewingOption = confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption;
                    if (confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption == Model.OptionType.Yes)
                    {
                        confirmation.confirmationFormDetail.ConfirmationRecommended = true;
                        confirmation.confirmationFormDetail.ConfirmationClarification = false;
                    }
                    if (confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption == Model.OptionType.No)
                    {
                        confirmation.confirmationFormDetail.ConfirmationRecommended = false;
                        confirmation.confirmationFormDetail.ConfirmationClarification = false;
                    }
                    return Json(new { part = 9, htmlData = ConvertViewToString("_ConfirmationFormA", confirmation), msgType = "validationFailed" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult PostConfirmationFormBData(Model.ConfirmationFormHdr confirmationFormHdr, string ButtonType)
        {
            log.Info($"ConfirmationFormAController/PostConfirmationFormBData");
            try
            {
                if (confirmationFormHdr.loggedInEmpID == confirmationFormHdr.ReportingTo && confirmationFormHdr.ApprovalHierarchy == 1)
                {
                    ModelState.Remove("confirmationFormDetail.ReviewingOfficerComment");

                    //=== added code to remove model validation for reviewing officer === 

                    ModelState.Remove("confirmationFormDetail.RVpoint8_1_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_2_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_3_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_4_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_5_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_6_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_7_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_8_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_9_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_10_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_11_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_12_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_13_Gr");



                }

                if (confirmationFormHdr.confirmationFormDetail.ConfirmationClarification &&
                    confirmationFormHdr.loggedInEmpID == confirmationFormHdr.ReportingTo)
                {
                    if (string.IsNullOrEmpty(confirmationFormHdr.confirmationFormDetail.ROClarificationRemark))
                        ModelState.AddModelError("ROClarificationRequired", "Please Enter Clarification Remark");
                }
                var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();

                if (ButtonType.Equals("save", StringComparison.OrdinalIgnoreCase) || (confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption == Model.OptionType.Clarification && confirmationFormHdr.loggedInEmpID == confirmationFormHdr.ReviewingTo))
                {
                    ModelState.Remove("confirmationFormDetail.ReviewingOfficerComment");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_1_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_2_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_3_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_4_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_5_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_6_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_7_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_8_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_9_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_10_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_11_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_12_Gr");
                    ModelState.Remove("confirmationFormDetail.RVpoint8_13_Gr");

                    ModelState.Remove("confirmationFormDetail.point8_1_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_2_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_3_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_4_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_5_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_6_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_7_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_8_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_9_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_10_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_11_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_12_Gr");
                    ModelState.Remove("confirmationFormDetail.point8_13_Gr");


                }
                if (ModelState.IsValid)
                {
                    bool flag = false;


                    #region ==== Reviewing Officer  =============

                    if (confirmationFormHdr.confirmationFormDetail.RVpoint_9_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.RVPoint9 = true;
                    if (confirmationFormHdr.confirmationFormDetail.RVpoint_10_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.RVPoint10 = true;
                    if (confirmationFormHdr.confirmationFormDetail.RVpoint_11_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.RVPoint11 = true;
                    if (confirmationFormHdr.confirmationFormDetail.RVpoint_12_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.RVPoint12 = true;
                    #endregion

                    if (confirmationFormHdr.confirmationFormDetail.point_9_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.Point9 = true;
                    if (confirmationFormHdr.confirmationFormDetail.point_10_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.Point10 = true;
                    if (confirmationFormHdr.confirmationFormDetail.point_11_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.Point11 = true;
                    if (confirmationFormHdr.confirmationFormDetail.point_12_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.Point12 = true;
                    if (confirmationFormHdr.confirmationFormDetail.recommnded_Option == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.ConfirmationRecommendedReporting = true;
                    if (confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption == Model.OptionType.Yes)
                        confirmationFormHdr.confirmationFormDetail.ConfirmationRecommended = true;
                    if (confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption == Model.OptionType.Clarification)
                        confirmationFormHdr.confirmationFormDetail.ConfirmationClarification = true;
                    else
                    {
                        confirmationFormHdr.confirmationFormDetail.ConfirmationClarification = false;
                        //  confirmationFormHdr.confirmationFormDetail.ReviewingOfficerComment = null;
                    }
                    confirmationFormHdr.confirmationFormDetail.Point8_1 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_1_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_2 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_2_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_3 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_3_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_4 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_4_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_5 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_5_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_6 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_6_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_7 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_7_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_8 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_8_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_9 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_9_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_10 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_10_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_11 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_11_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_12 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_12_Gr;
                    confirmationFormHdr.confirmationFormDetail.Point8_13 = (byte?)confirmationFormHdr.confirmationFormDetail.point8_13_Gr;

                    confirmationFormHdr.confirmationFormDetail.RVPoint8_1 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_1_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_2 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_2_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_3 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_3_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVRVPoint8_4 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_4_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVRVPoint8_5 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_5_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_6 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_6_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_7 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_7_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_8 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_8_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_9 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_9_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_10 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_10_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_11 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_11_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_12 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_12_Gr;
                    confirmationFormHdr.confirmationFormDetail.RVPoint8_13 = (byte?)confirmationFormHdr.confirmationFormDetail.RVpoint8_13_Gr;


                    confirmationFormHdr.confirmationFormDetail.UpdatedBy = userDetail.UserID;
                    confirmationFormHdr.confirmationFormDetail.UpdatedOn = DateTime.Now;

                    var formNewAttributes = GetFormAttributes(confirmationFormHdr, ButtonType);
                    confirmationFormHdr.submittedBy = formNewAttributes.SubmittedBy;
                    confirmationFormHdr.confirmationFormDetail.FormState = formNewAttributes.FormState;

                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                    {
                        SenderID = userDetail.EmployeeID,
                        ReceiverID = formNewAttributes.ReciverID,
                        SenderDepartmentID = userDetail.DepartmentID,
                        SenderDesignationID = userDetail.DesignationID,
                        CreatedBy = userDetail.UserID,
                        EmployeeID = confirmationFormHdr.EmployeeID,
                        Scomments = $"Form B, Confirmation Form Submited By : {(Model.ConfirmationSubmittedBy)(int)confirmationFormHdr.submittedBy}",
                        ProcessID = confirmationFormHdr.ProcessID,
                        StatusID = (int)confirmationFormHdr.confirmationFormDetail.FormState,
                        ReferenceID = confirmationFormHdr.EmpProcessAppID,
                        Forwarded_WorkflowID = confirmationFormHdr.FormHdrID
                    };
                    confirmationFormHdr._ProcessWorkFlow = workFlow;

                    if (confirmationFormHdr.confirmationFormDetail.ConfirmationClarification == true && ButtonType != "Save" && confirmationFormHdr.loggedInEmpID == confirmationFormHdr.ReviewingTo)
                    {
                        Model.ConfirmationClarification confirmationClarification = new Model.ConfirmationClarification()
                        {
                            FormHdrID = confirmationFormHdr.FormHdrID,
                            ClarificationRemarks = confirmationFormHdr.confirmationFormDetail.ReviewingOfficerComment,
                            CreatedBy = userDetail.UserID,
                            CreatedOn = DateTime.Now,
                            EmpProcessAppID = confirmationFormHdr.EmpProcessAppID,
                            FormABHeaderID = confirmationFormHdr.confirmationFormDetail.FormBHeaderId,
                            FormTypeID = (int)confirmationFormHdr.FormTypeID

                        };
                        confirmationFormHdr._ConfirmationClarification = confirmationClarification;
                    }

                    if (confirmationFormHdr.confirmationFormDetail.ConfirmationClarification
                        //&& ButtonType != "Save" 
                        && confirmationFormHdr.loggedInEmpID == confirmationFormHdr.ReportingTo)
                    {
                        Model.ConfirmationClarification confirmationClarification = new Model.ConfirmationClarification()
                        {
                            FormHdrID = confirmationFormHdr.FormHdrID,
                            ROClarification = confirmationFormHdr.confirmationFormDetail.ROClarificationRemark,
                            UpdatedBy = userDetail.UserID,
                            UpdatedOn = DateTime.Now,
                            //ClarificationRemarks = confirmationFormHdr.confirmationFormDetail.ReviewingOfficerComment,
                            //CreatedBy = userDetail.UserID,
                            //CreatedOn = DateTime.Now,
                            EmpProcessAppID = confirmationFormHdr.EmpProcessAppID,
                            FormABHeaderID = confirmationFormHdr.confirmationFormDetail.FormBHeaderId
                        };
                        confirmationFormHdr._ConfirmationClarification = confirmationClarification;
                    }
                    flag = confirmationService.UpdateFormBData(confirmationFormHdr);
                    var msg = ButtonType == "Save" ? "Form updated successfully." : "Form submitted successfully.";
                    return Json(new { formState = confirmationFormHdr.confirmationFormDetail.FormState, part = 0, msgType = "success", msg = msg }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var confirmation = (Model.ConfirmationFormHdr)TempData["ConfirmationFormB"];
                    TempData.Keep("ConfirmationFormB");
                    confirmation.confirmationFormDetail.recommnded_ReviewingOption = confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption;
                    if (confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption == Model.OptionType.Yes)
                    {
                        confirmation.confirmationFormDetail.ConfirmationRecommended = true;
                        confirmation.confirmationFormDetail.ConfirmationClarification = false;
                    }
                    if (confirmationFormHdr.confirmationFormDetail.recommnded_ReviewingOption == Model.OptionType.No)
                    {
                        confirmation.confirmationFormDetail.ConfirmationRecommended = false;
                        confirmation.confirmationFormDetail.ConfirmationClarification = false;
                    }

                    return Json(new { part = 9, htmlData = ConvertViewToString("_ConfirmationFormB", confirmation), msgType = "validationFailed" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [NonAction]
        private Model.ConfirmationFormRulesAttributes GetFormAttributes(Model.ConfirmationFormHdr appForm, string buttonType)
        {
            log.Info($"ConfirmationFormAController/GetFormAttributes");
            try
            {
                Model.ConfirmationFormRulesAttributes formRules = new Model.ConfirmationFormRulesAttributes();
                if (appForm.ApprovalHierarchy == 1)
                {
                    if (appForm.loggedInEmpID == appForm.ReportingTo && DateTime.Now.Date >= appForm.confirmationFormRulesAttributes.ReportingSubmissionDate)
                    {
                        formRules.FormState = (buttonType == "Save" ? (int)Model.ConfirmationFormState.SavedByReporting : appForm.confirmationFormDetail.ConfirmationRecommendedReporting == false ? (int)Model.ConfirmationFormState.RejectedByReporting : (int)Model.ConfirmationFormState.SubmitedByReporting);
                        formRules.SubmittedBy = Model.ConfirmationSubmittedBy.ReportingOfficer;
                        formRules.SenderID = appForm.ReportingTo;
                        formRules.ReciverID = appForm.ReviewingTo;
                    }
                    else if (appForm.loggedInEmpID == appForm.ReviewingTo && (DateTime.Now.Date >= appForm.confirmationFormRulesAttributes.ReviewerSubmissionDate))
                    {
                        //formRules.FormState = (buttonType == "Save" ? (int)Model.ConfirmationFormState.SavedByReviewer : appForm.confirmationFormDetail.ConfirmationRecommended == false ? (int)Model.ConfirmationFormState.RejectedByReviewer : appForm.confirmationFormDetail.ConfirmationClarification == false ? (int)Model.ConfirmationFormState.SubmitedByReviewer : (int)Model.ConfirmationFormState.SavedByReporting);

                        if (buttonType == "Save")
                            formRules.FormState = (int)Model.ConfirmationFormState.SavedByReviewer;
                        else if (buttonType != "Save" && appForm.confirmationFormDetail.ConfirmationRecommended == true)
                            formRules.FormState = (int)Model.ConfirmationFormState.SubmitedByReviewer;
                        else if (buttonType != "Save" && (appForm.confirmationFormDetail.ConfirmationRecommended == false && !appForm.confirmationFormDetail.ConfirmationClarification))
                            formRules.FormState = (int)Model.ConfirmationFormState.RejectedByReviewer;
                        else if (buttonType != "Save" && appForm.confirmationFormDetail.ConfirmationClarification == true)
                            formRules.FormState = (int)Model.ConfirmationFormState.SavedByReporting;

                        formRules.SubmittedBy = Model.ConfirmationSubmittedBy.ReviewingOfficer;
                        formRules.SenderID = appForm.ReviewingTo;
                        formRules.ReciverID = null;
                    }
                }
                else if (appForm.ApprovalHierarchy == 2)
                {
                    if (appForm.loggedInEmpID == appForm.ReviewingTo && DateTime.Now.Date >= appForm.confirmationFormRulesAttributes.ReviewerSubmissionDate)
                    {
                        //formRules.FormState = (buttonType == "Save" ? (int)Model.ConfirmationFormState.SavedByReviewer : appForm.confirmationFormDetail.ConfirmationRecommended == false && appForm.confirmationFormDetail.ConfirmationRecommended ? (int)Model.ConfirmationFormState.RejectedByReviewer : (int)Model.ConfirmationFormState.SubmitedByReviewer);

                        if (buttonType == "Save")
                            formRules.FormState = (int)Model.ConfirmationFormState.SavedByReviewer;
                        else if (buttonType != "Save" && appForm.confirmationFormDetail.ConfirmationRecommended == true)
                            formRules.FormState = (int)Model.ConfirmationFormState.SubmitedByReviewer;
                        else if (buttonType != "Save" && (appForm.confirmationFormDetail.ConfirmationRecommended == false && !appForm.confirmationFormDetail.ConfirmationClarification))
                            formRules.FormState = (int)Model.ConfirmationFormState.RejectedByReviewer;
                        else if (buttonType != "Save" && appForm.confirmationFormDetail.ConfirmationClarification == true)
                            formRules.FormState = (int)Model.ConfirmationFormState.SavedByReporting;

                        formRules.SubmittedBy = Model.ConfirmationSubmittedBy.ReviewingOfficer;
                        formRules.SenderID = appForm.ReviewingTo;
                        formRules.ReciverID = null;
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

        #region View Confirmation Form
        [HttpGet]
        public ActionResult View(int formTypeID, int? reportingTo, int? reviewingTo, int? processTypeID, int? empID, int formHdrID, int apid, int chdrid)
        {
            log.Info($"ConfirmationFormController/View");
            try
            {
                Model.ConfirmationFormHdr frmHdrDetail = new Model.ConfirmationFormHdr();
                frmHdrDetail.ReportingTo = reportingTo;
                frmHdrDetail.ReviewingTo = reviewingTo;
                frmHdrDetail.FormTypeID = formTypeID;
                frmHdrDetail.EmployeeID = empID.Value;
                frmHdrDetail.loggedInEmpID = userDetail.EmployeeID;
                frmHdrDetail.ProcessID = processTypeID.Value;
                frmHdrDetail.FormHdrID = formHdrID;
                frmHdrDetail.EmpProcessAppID = apid;
                frmHdrDetail.FormABHeaderID = chdrid;

                var reportingDetails = confirmationService.GetRTRVDetails(frmHdrDetail.ReportingTo);
                frmHdrDetail.ReportingName = reportingDetails.Name;
                frmHdrDetail.ReportingDesignation = reportingDetails.Designation;

                if (frmHdrDetail.ReviewingTo != null)
                {
                    var reviewingDetails = confirmationService.GetRTRVDetails(frmHdrDetail.ReviewingTo);
                    frmHdrDetail.ReviewingName = reviewingDetails.Name;
                    frmHdrDetail.ReviewingDesignation = reviewingDetails.Designation;
                }
                frmHdrDetail.confirmationFormRulesAttributes = confirmationService.GetConfirmationFormRulesAttributes(formTypeID, frmHdrDetail.EmployeeID, frmHdrDetail.ProcessID, frmHdrDetail.EmpProcessAppID, frmHdrDetail.FormABHeaderID);

                if (frmHdrDetail.confirmationFormRulesAttributes != null)
                {
                    if ((frmHdrDetail.ReportingTo == frmHdrDetail.ReviewingTo)
                       && (frmHdrDetail.loggedInEmpID == frmHdrDetail.ReportingTo || frmHdrDetail.loggedInEmpID == frmHdrDetail.ReviewingTo))
                        frmHdrDetail.ApprovalHierarchy = 2;
                    else
                        frmHdrDetail.ApprovalHierarchy = 1;

                    if (frmHdrDetail.confirmationFormRulesAttributes.FormState == 1)
                    {
                        if (frmHdrDetail.ApprovalHierarchy == 1 && frmHdrDetail.loggedInEmpID == frmHdrDetail.ReportingTo /*&& DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReportingSubmissionDate*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingButton = true;
                        }
                        else if (frmHdrDetail.ApprovalHierarchy == 1 && frmHdrDetail.loggedInEmpID == frmHdrDetail.ReviewingTo
                            /*&&(DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReviewerSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                        }
                        else if (frmHdrDetail.ApprovalHierarchy == 2 && frmHdrDetail.loggedInEmpID == frmHdrDetail.ReviewingTo /*&& (DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReviewerSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingButton = true;
                        }
                    }
                    else if (frmHdrDetail.confirmationFormRulesAttributes.FormState == 3)
                    {
                        if (frmHdrDetail.ApprovalHierarchy == 2 && frmHdrDetail.loggedInEmpID == reviewingTo
                            /*&& (DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReviewerSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                        }
                        else if (frmHdrDetail.ApprovalHierarchy == 1 && frmHdrDetail.loggedInEmpID == reviewingTo
                            /*&& (DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReviewerSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = false;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                        }
                    }
                    else if (frmHdrDetail.confirmationFormRulesAttributes.FormState == 4 || frmHdrDetail.confirmationFormRulesAttributes.FormState == 5 || frmHdrDetail.confirmationFormRulesAttributes.FormState == 6 || frmHdrDetail.confirmationFormRulesAttributes.FormState == 7)
                    {
                        if (frmHdrDetail.ApprovalHierarchy == 1 && frmHdrDetail.loggedInEmpID == reviewingTo
                            /*&& (DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReviewerSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                        }
                        else if (frmHdrDetail.ApprovalHierarchy == 2 && frmHdrDetail.loggedInEmpID == reviewingTo
                            /*&& (DateTime.Now.Date >= frmHdrDetail.confirmationFormRulesAttributes.ReportingSubmissionDate)*/)
                        {
                            frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                            frmHdrDetail.confirmationFormRulesAttributes.ReviewerButton = true;
                        }
                    }
                    else if (frmHdrDetail.confirmationFormRulesAttributes.FormState == 0 && frmHdrDetail.ReviewingTo.HasValue)
                    {
                        frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                        frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                    }
                    else if (frmHdrDetail.confirmationFormRulesAttributes.FormState == 2 && frmHdrDetail.loggedInEmpID == frmHdrDetail.ReviewingTo)
                    {
                        frmHdrDetail.confirmationFormRulesAttributes.ReportingSection = true;
                        frmHdrDetail.confirmationFormRulesAttributes.ReviewerSection = true;
                    }
                }

                if (formTypeID == 1)
                    ViewBag.Title = "Confirmation - Form A";
                else if (formTypeID == 2)
                    ViewBag.Title = "Confirmation - Form B";
                return PartialView("ViewConfirmationForm", frmHdrDetail);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult ViewConfirmationFormA(Model.ConfirmationFormHdr confirmation)
        {
            log.Info($"ConfirmationFormController/ViewConfirmationFormA");
            try
            {
                confirmation.loggedInEmpID = userDetail.EmployeeID;
                var confirmationFormList = confirmationService.GetEmployeeConfirmationList(confirmation.FormTypeID.Value, confirmation.ProcessID, confirmation.EmployeeID, confirmation.EmpProcessAppID, confirmation.FormABHeaderID);
                confirmation.EmployeeID = confirmationFormList.EmployeeId;
                confirmation.ProcessID = confirmationFormList.ProcessId;

                confirmationFormList.point8_1_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_1;
                confirmationFormList.point8_2_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_2;
                confirmationFormList.point8_3_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_3;
                confirmationFormList.point8_4_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_4;
                confirmationFormList.point8_5_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_5;

                confirmationFormList.point8_6_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_6;
                confirmationFormList.point8_7_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_7;
                confirmationFormList.point8_8_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_8;
                confirmationFormList.point8_9_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_9;
                confirmationFormList.point8_10_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_10;
                confirmationFormList.point8_11_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_11;
                confirmationFormList.point8_12_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_12;
                confirmationFormList.point8_13_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_13;

                confirmationFormList.RVpoint8_1_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_1;
                confirmationFormList.RVpoint8_2_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_2;
                confirmationFormList.RVpoint8_3_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_3;
                confirmationFormList.RVpoint8_4_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVRVPoint8_4;
                confirmationFormList.RVpoint8_5_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVRVPoint8_5;
                confirmationFormList.RVpoint8_6_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_6;
                confirmationFormList.RVpoint8_7_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_7;

                confirmationFormList.RVpoint8_8_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_8;
                confirmationFormList.RVpoint8_9_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_9;
                confirmationFormList.RVpoint8_10_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_10;
                confirmationFormList.RVpoint8_11_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_11;
                confirmationFormList.RVpoint8_12_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_12;
                confirmationFormList.RVpoint8_13_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_13;
                
                confirmation.confirmationFormDetail = confirmationFormList;
                var clarification = confirmationService.GetConfirmationClarification(confirmation.FormHdrID, confirmation.EmpProcessAppID, confirmation.FormTypeID.Value, confirmation.FormABHeaderID);
                confirmation.confirnationClarification = clarification;
                TempData["ConfirmationFormA"] = confirmation;
                return PartialView("_ViewConfirmationFormA", confirmation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult ViewConfirmationFormB(Model.ConfirmationFormHdr confirmation)
        {
            log.Info($"ConfirmationFormController/ViewConfirmationFormB");
            try
            {
                confirmation.loggedInEmpID = userDetail.EmployeeID;
                var confirmationFormList = confirmationService.GetEmployeeConfirmationList(confirmation.FormTypeID.Value, confirmation.ProcessID, confirmation.EmployeeID, confirmation.EmpProcessAppID, confirmation.FormABHeaderID);
                confirmation.EmployeeID = confirmationFormList.EmployeeId;
                confirmation.ProcessID = confirmationFormList.ProcessId;
                confirmationFormList.point8_1_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_1;
                confirmationFormList.point8_2_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_2;
                confirmationFormList.point8_3_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_3;
                confirmationFormList.point8_4_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_4;
                confirmationFormList.point8_5_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_5;
                confirmationFormList.point8_6_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_6;
                confirmationFormList.point8_7_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_7;
                confirmationFormList.point8_8_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_8;
                confirmationFormList.point8_9_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_9;
                confirmationFormList.point8_10_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_10;
                confirmationFormList.point8_11_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_11;
                confirmationFormList.point8_12_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_12;
                confirmationFormList.point8_13_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.Point8_13;
                confirmationFormList.RVpoint8_1_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_1;
                confirmationFormList.RVpoint8_2_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_2;
                confirmationFormList.RVpoint8_3_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_3;
                confirmationFormList.RVpoint8_4_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVRVPoint8_4;
                confirmationFormList.RVpoint8_5_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVRVPoint8_5;
                confirmationFormList.RVpoint8_6_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_6;
                confirmationFormList.RVpoint8_7_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_7;
                confirmationFormList.RVpoint8_8_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_8;
                confirmationFormList.RVpoint8_9_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_9;
                confirmationFormList.RVpoint8_10_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_10;
                confirmationFormList.RVpoint8_11_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_11;
                confirmationFormList.RVpoint8_12_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_12;
                confirmationFormList.RVpoint8_13_Gr = (Model.ConfirmationRatingGuide)confirmationFormList.RVPoint8_13;
                confirmation.confirmationFormDetail = confirmationFormList;
                var clarification = confirmationService.GetConfirmationClarification(confirmation.FormHdrID, confirmation.EmpProcessAppID, confirmation.FormTypeID.Value, confirmation.FormABHeaderID);
                confirmation.confirnationClarification = clarification;
                TempData["ConfirmationFormB"] = confirmation;
                return PartialView("_ViewConfirmationFormB", confirmation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion
    }
}
