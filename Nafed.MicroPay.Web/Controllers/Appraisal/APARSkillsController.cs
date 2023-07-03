using Nafed.MicroPay.Common;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using AutoMapper;

namespace MicroPay.Web.Controllers.Appraisal
{
    public class APARSkillsController : BaseController
    {
        private readonly IAppraisalFormService appraisalService;
        private readonly IDropdownBindService ddlService;
        public APARSkillsController(IAppraisalFormService appraisalService, IDropdownBindService ddlService)
        {
            this.appraisalService = appraisalService;
            this.ddlService = ddlService;
        }
        // GET: APARSkills
        public ActionResult Index()
        {
            log.Info($"APARSkillsController/Index");
            Models.APARSkillsViewModel empAPARVM = new Models.APARSkillsViewModel();
            var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.Appraisal);
            empAPARVM.approvalSetting = approvalSettings ?? new EmployeeProcessApproval();
            return View(empAPARVM);
        }

        public ActionResult MyAPARSkillList()
        {
            log.Info($"APARSkillsController/MyAPARSkillList");
            try
            {
                List<APARSkillSetFormHdr> empAPARVM = new List<APARSkillSetFormHdr>();
                //  Models.APARSkillsViewModel empAPARVM = new Models.APARSkillsViewModel();
                empAPARVM = appraisalService.GetEmployeeAPARSkillList((int)userDetail.EmployeeID);
                return PartialView("_MyAPARSkillList", empAPARVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult Edit(int? reportingTo, int? empID, int departmentID, int designationID, string ryr = null)
        {
            log.Info($"APARSkillsController/Edit{reportingTo}/{empID}");
            try
            {
                Models.APARSkillsViewModel aparForm = new Models.APARSkillsViewModel();

                #region Skill Set popup
                var selectedCheckboxesBehavioral = new List<CheckBox>();
                var getFieldsValueBehavioral = ddlService.ddlSkill(2);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aparForm.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                aparForm.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;

                //-----------------------------------
                var selectedCheckboxesFunctional = new List<CheckBox>();
                var getFieldsValueFunctional = ddlService.ddlSkill(3);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aparForm.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                aparForm.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;
                #endregion

                aparForm.ReportingTo = reportingTo;
                aparForm.EmployeeID = (int)empID;
                aparForm.LoggedInEmpID = userDetail.EmployeeID;
                var reportingYr = ryr ?? DateTime.Now.GetFinancialYr();
                aparForm.APARFormHdr = new APARSkillSetFormHdr();
                aparForm.APARFormHdr = appraisalService.GetFormAPARHdrDetail(null, aparForm.EmployeeID, reportingYr);
                aparForm.APARFormHdr.ReportingYr = aparForm.APARFormHdr.ReportingYr == null ? reportingYr : aparForm.APARFormHdr.ReportingYr;
                var aparFromDetail = appraisalService.GetAPARFormDetail(aparForm.EmployeeID, departmentID, designationID, reportingYr);
                aparForm.Part1BehavioralList = aparFromDetail.Where(x => (x.PartID == 1 || x.PartID == 0) && x.SkillTypeID == 2).ToList();
                aparForm.Part1FunctionalList = aparFromDetail.Where(x => (x.PartID == 1 || x.PartID == 0) && x.SkillTypeID == 3).ToList();

                //aparForm.Part2BehavioralList = aparFromDetail.Where(x => (x.PartID == 2 || x.PartID == 0) && x.SkillTypeID == 2).ToList();
                //if (aparForm.Part2BehavioralList.Count == 0)
                //{
                //    var lff = (aparFromDetail.Where(x => x.SkillTypeID == 2).ToList());
                //    aparForm.Part2BehavioralList = new List<APARSkillSetFormDetail>();
                //    lff.ForEach(x =>
                //    {
                //        aparForm.Part2BehavioralList.Add(new APARSkillSetFormDetail()
                //        {
                //            SkillID = x.SkillID,
                //            SkillTypeID = x.SkillTypeID,
                //            Skill = x.Skill,
                //            SkillSetID = x.SkillSetID,
                //            SkillRemark = x.SkillRemark
                //        });
                //    });
                //}
                //aparForm.Part2FunctionalList = aparFromDetail.Where(x => (x.PartID == 2 || x.PartID == 0) && x.SkillTypeID == 3).ToList();
                //if (aparForm.Part2FunctionalList.Count == 0)
                //{
                //    var lff = (aparFromDetail.Where(x => x.SkillTypeID == 3).ToList());
                //    aparForm.Part2FunctionalList = new List<APARSkillSetFormDetail>();
                //    lff.ForEach(x =>
                //    {
                //        aparForm.Part2FunctionalList.Add(new APARSkillSetFormDetail()
                //        {
                //            SkillID = x.SkillID,
                //            SkillTypeID = x.SkillTypeID,
                //            Skill = x.Skill,
                //            SkillSetID = x.SkillSetID,
                //            SkillRemark = x.SkillRemark
                //        });
                //    });
                //}

                TempData["APARFormDtls"] = aparForm;
                TempData.Keep("APARFormDtls");
                return View("APARFormContainer", aparForm);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult _PostAPARFormData(Models.APARSkillsViewModel aparForm, string ButtonType)
        {
            log.Info($"APARSkillsController/_PostAPARFormData");
            try
            {
                #region Skill Set popup
                var selectedCheckboxesBehavioral1 = new List<CheckBox>();
                var getFieldsValueBehavioral1 = ddlService.ddlSkill(2);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aparForm.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral1);
                aparForm.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral1;

                //-----------------------------------
                var selectedCheckboxesFunctional1 = new List<CheckBox>();
                var getFieldsValueFunctional1 = ddlService.ddlSkill(3);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aparForm.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional1);
                aparForm.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional1;
                #endregion
                if (ButtonType == "Add")
                {
                    int skillSetID = 0;
                    if (aparForm.Part1BehavioralList == null && aparForm.Part1FunctionalList == null)
                    {
                        APARSkills aprSkill = new APARSkills();
                        aprSkill.APARSkillSet = new APARSkillSet();
                        if (aparForm.CheckBoxListBehavioral.PostedFields != null)
                            aprSkill.CheckBoxListBehavioral = aparForm.CheckBoxListBehavioral.PostedFields.fieldIds;
                        if (aparForm.CheckBoxListFunctional.PostedFields != null)
                            aprSkill.CheckBoxListFunctional = aparForm.CheckBoxListFunctional.PostedFields.fieldIds;
                        aprSkill.APARSkillSet.EmployeeID = (int)userDetail.EmployeeID;
                        aprSkill.APARSkillSet.DepartmentID = userDetail.DepartmentID;
                        aprSkill.APARSkillSet.DesignationID = (int)userDetail.DesignationID;
                        aprSkill.APARSkillSet.CreatedBy = userDetail.UserID;
                        aprSkill.APARSkillSet.CreatedOn = DateTime.Now;
                        aprSkill.APARSkillSet.StatusID = 1;

                        skillSetID = appraisalService.InsertAPARSkill(aprSkill);
                    }
                    var FunctionalSkillSetID = 0;
                    var BehavioralSkillSetID = 0;
                    if (aparForm.CheckBoxListBehavioral.PostedFields != null)
                        aparForm.CheckBoxBehavioral = aparForm.CheckBoxListBehavioral.PostedFields.fieldIds;
                    if (aparForm.CheckBoxListFunctional.PostedFields != null)
                        aparForm.CheckBoxFunctional = aparForm.CheckBoxListFunctional.PostedFields.fieldIds;
                    if (skillSetID > 0)
                    {
                        FunctionalSkillSetID = skillSetID;
                        BehavioralSkillSetID = skillSetID;
                    }
                    else
                    {


                        if (aparForm.Part1FunctionalList != null)
                        {
                            FunctionalSkillSetID = aparForm.Part1FunctionalList.FirstOrDefault().SkillSetID;
                            BehavioralSkillSetID = aparForm.Part1FunctionalList.FirstOrDefault().SkillSetID;
                        }
                        else
                        {

                            FunctionalSkillSetID = aparForm.Part1BehavioralList.FirstOrDefault().SkillSetID;
                            BehavioralSkillSetID = aparForm.Part1BehavioralList.FirstOrDefault().SkillSetID;

                        }
                    }
                    Mapper.Initialize(cfg =>
                       cfg.CreateMap<int, APARSkillSetFormDetail>()
                       .ForMember(d => d.SkillTypeID, o => o.UseValue(2))
                       .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                       .ForMember(d => d.SkillSetID, o => o.UseValue(BehavioralSkillSetID))
                       .ForAllOtherMembers(d => d.Ignore())
                       );
                    var dtoBehavioral = Mapper.Map<List<APARSkillSetFormDetail>>(aparForm.CheckBoxBehavioral);

                    Mapper.Initialize(cfg =>
                       cfg.CreateMap<int, APARSkillSetFormDetail>()
                       .ForMember(d => d.SkillTypeID, o => o.UseValue(3))
                       .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                       .ForMember(d => d.SkillSetID, o => o.UseValue(FunctionalSkillSetID))
                       .ForAllOtherMembers(d => d.Ignore())
                       );
                    var dtoFunctional = Mapper.Map<List<APARSkillSetFormDetail>>(aparForm.CheckBoxFunctional);
                    var getFieldsValueBehavioral = ddlService.ddlSkill(2);
                    var getFieldsValueFunctional = ddlService.ddlSkill(3);
                    if (dtoBehavioral != null && dtoBehavioral.Count > 0)
                    {
                        if (aparForm.Part1BehavioralList == null)
                        {
                            aparForm.Part1BehavioralList = new List<APARSkillSetFormDetail>();
                            dtoBehavioral.ForEach(x =>
                            {
                                aparForm.Part1BehavioralList.Add(new APARSkillSetFormDetail()
                                {
                                    SkillID = x.SkillID,
                                    SkillTypeID = x.SkillTypeID,
                                    Skill = getFieldsValueBehavioral.Where(y => y.id == x.SkillID).FirstOrDefault().value,
                                    SkillSetID = x.SkillSetID
                                });
                            });
                        }
                        else
                        {
                            dtoBehavioral.ForEach(x =>
                        {
                            if (!aparForm.Part1BehavioralList.Any(em => em.SkillID == x.SkillID))
                                aparForm.Part1BehavioralList.Add(new APARSkillSetFormDetail()
                                {
                                    SkillID = x.SkillID,
                                    SkillTypeID = x.SkillTypeID,
                                    Skill = getFieldsValueBehavioral.Where(y => y.id == x.SkillID).FirstOrDefault().value,
                                    SkillSetID = x.SkillSetID

                                });
                        });
                        }
                    }

                    if (dtoFunctional != null && dtoFunctional.Count > 0)
                    {
                        if (aparForm.Part1FunctionalList == null)
                        {
                            aparForm.Part1FunctionalList = new List<APARSkillSetFormDetail>();
                            dtoFunctional.ForEach(x =>
                            {
                                aparForm.Part1FunctionalList.Add(new APARSkillSetFormDetail()
                                {
                                    SkillID = x.SkillID,
                                    SkillTypeID = x.SkillTypeID,
                                    Skill = getFieldsValueFunctional.Where(y => y.id == x.SkillID).FirstOrDefault().value,
                                    SkillSetID = x.SkillSetID

                                });
                            });
                        }
                        else
                        {
                            dtoFunctional.ForEach(x =>
                            {
                                if (!aparForm.Part1FunctionalList.Any(em => em.SkillID == x.SkillID))
                                    aparForm.Part1FunctionalList.Add(new APARSkillSetFormDetail()
                                    {
                                        SkillID = x.SkillID,
                                        SkillTypeID = x.SkillTypeID,
                                        Skill = getFieldsValueFunctional.Where(y => y.id == x.SkillID).FirstOrDefault().value,
                                        SkillSetID = x.SkillSetID
                                    });
                            });
                        }
                    }

                    #region Skill Set popup
                    var selectedCheckboxesBehavioral = new List<CheckBox>();


                    var dd = aparForm.Part1BehavioralList.Distinct().ToList();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    aparForm.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                    aparForm.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;

                    //-----------------------------------
                    var selectedCheckboxesFunctional = new List<CheckBox>();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    aparForm.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                    aparForm.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;
                    #endregion
                    TempData["APARFormDtls"] = aparForm;
                    TempData.Keep("APARFormDtls");
                    return Json(new { part = 5, htmlData = ConvertViewToString("_APARSkillForm", aparForm) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    APARForm AFrom = new APARForm();
                    AFrom.APARFormHdr = aparForm.APARFormHdr;
                    AFrom.Part1BehavioralList = aparForm.Part1BehavioralList;
                    AFrom.Part1FunctionalList = aparForm.Part1FunctionalList;
                    AFrom.Part2BehavioralList = aparForm.Part2BehavioralList;
                    AFrom.Part2FunctionalList = aparForm.Part2FunctionalList;
                    string msg = "";
                    int? receiverID = null;
                    if (!aparForm.APARFormHdr.WorkedPeriodUnderROFrom.HasValue)
                    {
                        ModelState.AddModelError("WorkedPeriodUnderROFrom", "Please select date.");
                        aparForm = (Models.APARSkillsViewModel)TempData["APARFormDtls"];
                        TempData.Keep("APARFormDtls");
                        return Json(new { part = 5, htmlData = ConvertViewToString("_APARSkillForm", aparForm) }, JsonRequestBehavior.AllowGet);
                    }
                    else if (!aparForm.APARFormHdr.WorkedPeriodUnderROTo.HasValue)
                    {
                        ModelState.AddModelError("WorkedPeriodUnderROTo", "Please select date.");
                        aparForm = (Models.APARSkillsViewModel)TempData["APARFormDtls"];
                        TempData.Keep("APARFormDtls");
                        return Json(new { part = 5, htmlData = ConvertViewToString("_APARSkillForm", aparForm) }, JsonRequestBehavior.AllowGet);

                    }
                    if (aparForm.APARFormHdr.WorkedPeriodUnderROFrom.HasValue && aparForm.APARFormHdr.WorkedPeriodUnderROTo.HasValue)
                    {
                        if (aparForm.APARFormHdr.WorkedPeriodUnderROTo.Value < aparForm.APARFormHdr.WorkedPeriodUnderROFrom.Value)
                        {
                            ModelState.AddModelError("WorkedPeriodUnderROFrom", "From date should be always less than To date.");
                            aparForm = (Models.APARSkillsViewModel)TempData["APARFormDtls"];
                            TempData.Keep("APARFormDtls");
                            return Json(new { part = 5, htmlData = ConvertViewToString("_APARSkillForm", aparForm) }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    bool isvalid = false;

                    if (!aparForm.ReportingTo.HasValue || aparForm.ReportingTo.Value == 0)
                    {
                        var errormsg = "You can not apply for A.P.A.R Skill right now because your Reporting Manager is not set.";
                        return Json(new { part = 0, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_APARSkillForm", aparForm) }, JsonRequestBehavior.AllowGet);
                    }

                    if (aparForm.Part1BehavioralList == null || aparForm.Part1FunctionalList == null)
                    {
                        var errormsg = "Please Add Skill Assessment.";
                        return Json(new { part = 0, msgType = "error", msg = errormsg, htmlData = ConvertViewToString("_APARSkillForm", aparForm) }, JsonRequestBehavior.AllowGet);
                    }

                    if (aparForm.EmployeeID == aparForm.LoggedInEmpID)
                    {
                        #region 
                        if (AFrom.Part1BehavioralList != null && AFrom.Part1BehavioralList.Count > 0)
                        {
                            for (int i = 0; i < AFrom.Part1BehavioralList.Count; i++)
                            {
                                if (!AFrom.Part1BehavioralList[i].IsDeleted)
                                {
                                    if (AFrom.Part1BehavioralList[i].enumAssessment == 0)
                                    {
                                        ModelState.AddModelError("Part1Behavioral_" + i, "Please select assessment");
                                        isvalid = true;
                                    }
                                }
                            }
                        }

                        if (AFrom.Part1FunctionalList != null && AFrom.Part1FunctionalList.Count > 0)
                        {
                            for (int i = 0; i < AFrom.Part1FunctionalList.Count; i++)
                            {
                                if (!AFrom.Part1FunctionalList[i].IsDeleted)
                                {
                                    if (AFrom.Part1FunctionalList[i].enumAssessment == 0)
                                    {
                                        ModelState.AddModelError("Part1Functional_" + i, "Please select assessment");
                                        isvalid = true;
                                    }
                                }
                            }
                        }

                        if (isvalid)
                        {
                            var ddd = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                            aparForm.APARFormHdr.StatusID = (int)aparForm.APARFormHdr.formState;
                            TempData["APARFormDtls"] = aparForm;
                            TempData.Keep("APARFormDtls");
                            return Json(new { part = 5, htmlData = ConvertViewToString("_APARSkillForm", aparForm) }, JsonRequestBehavior.AllowGet);
                        }
                        #endregion
                    }
                    else if (aparForm.ReportingTo == aparForm.LoggedInEmpID)
                    {
                        #region 
                        if (ButtonType != "Reject")
                        {
                            if (AFrom.Part1BehavioralList != null && AFrom.Part1BehavioralList.Count > 0)
                            {
                                for (int i = 0; i < AFrom.Part1BehavioralList.Count; i++)
                                {
                                    if (!AFrom.Part1BehavioralList[i].IsDeleted)
                                    {
                                        if (AFrom.Part1BehavioralList[i].enumAssessmentReporting == 0)
                                        {
                                            ModelState.AddModelError("ReportingPart1Behavioral_" + i, "Please select assessment");

                                            isvalid = true;
                                        }
                                    }
                                }
                            }


                            if (AFrom.Part1FunctionalList != null && AFrom.Part1FunctionalList.Count > 0)
                            {
                                for (int i = 0; i < AFrom.Part1FunctionalList.Count; i++)
                                {
                                    if (!AFrom.Part1FunctionalList[i].IsDeleted)
                                    {
                                        if (AFrom.Part1FunctionalList[i].enumAssessmentReporting == 0)
                                        {
                                            ModelState.AddModelError("ReportingPart1Functional_" + i, "Please select assessment");

                                            isvalid = true;
                                        }
                                    }
                                }
                            }

                            if (isvalid)
                            {
                                // aparForm.APARFormHdr.StatusID = (int)aparForm.APARFormHdr.formState;
                                TempData["APARFormDtls"] = aparForm;
                                TempData.Keep("APARFormDtls");
                                return Json(new { part = 5, htmlData = ConvertViewToString("_APARSkillForm", aparForm) }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        #endregion
                    }

                    if (ButtonType == "Save")
                    {
                        if (aparForm.LoggedInEmpID == aparForm.APARFormHdr.EmployeeID)
                        {
                            AFrom.APARFormHdr.StatusID = (int)AppraisalFormState.SavedByEmployee;

                        }
                        else if (aparForm.LoggedInEmpID == aparForm.ReportingTo)
                        {
                            AFrom.APARFormHdr.StatusID = (int)AppraisalFormState.SavedByReporting;
                            receiverID = aparForm.ReportingTo;
                        }
                    }
                    else if (ButtonType == "Submit")
                    {
                        if (aparForm.LoggedInEmpID == aparForm.APARFormHdr.EmployeeID)
                        {
                            AFrom.APARFormHdr.StatusID = (int)AppraisalFormState.SubmitedByEmployee;
                            receiverID = aparForm.ReportingTo;
                        }
                        else if (aparForm.LoggedInEmpID == aparForm.ReportingTo)
                        {
                            AFrom.APARFormHdr.StatusID = (int)AppraisalFormState.SubmitedByReporting;
                            receiverID = aparForm.ReportingTo;
                        }
                    }

                    else if (ButtonType == "Reject")
                    {
                        if (aparForm.LoggedInEmpID == aparForm.ReportingTo)
                        {
                            AFrom.APARFormHdr.StatusID = (int)AppraisalFormState.RejectedbyReporting;
                            receiverID = aparForm.APARFormHdr.EmployeeID;
                        }
                    }
                    int res = 0;
                    if (aparForm.APARFormHdr.APARHdrID == 0)
                    {

                        ProcessWorkFlow workFlow = new ProcessWorkFlow()
                        {
                            SenderID = userDetail.EmployeeID,
                            // ReceiverID = GetWorkFlowReceiverID(roformData),
                            ReceiverID = receiverID,
                            SenderDepartmentID = userDetail.DepartmentID,
                            SenderDesignationID = userDetail.DesignationID,
                            CreatedBy = userDetail.UserID,
                            EmployeeID = aparForm.APARFormHdr.EmployeeID,
                            Scomments = $"A.P.A.R. Skill Form",
                            ProcessID = (int)WorkFlowProcess.Appraisal,
                            StatusID = aparForm.APARFormHdr.StatusID
                        };
                        AFrom._ProcessWorkFlow = workFlow;

                        AFrom.APARFormHdr.CreatedBy = userDetail.UserID;
                        AFrom.APARFormHdr.CreatedOn = DateTime.Now;
                        AFrom.APARFormHdr.DepartmentID = userDetail.DepartmentID;
                        AFrom.APARFormHdr.DesignationID = (int)userDetail.DesignationID;
                        res = appraisalService.InsertAPARFormData(AFrom);
                        msg = ButtonType == "Save" ? "Form has been saved successfully." : "Form has been submitted successfully for further review.";
                    }
                    else
                    {
                        ProcessWorkFlow workFlow = new ProcessWorkFlow()
                        {
                            SenderID = userDetail.EmployeeID,
                            // ReceiverID = GetWorkFlowReceiverID(roformData),
                            ReceiverID = receiverID,
                            SenderDepartmentID = userDetail.DepartmentID,
                            SenderDesignationID = userDetail.DesignationID,
                            CreatedBy = userDetail.UserID,
                            EmployeeID = aparForm.APARFormHdr.EmployeeID,
                            Scomments = $"A.P.A.R. Skill Form",
                            ProcessID = (int)WorkFlowProcess.Appraisal,
                            StatusID = aparForm.APARFormHdr.StatusID
                        };
                        AFrom._ProcessWorkFlow = workFlow;

                        AFrom.APARFormHdr.UpdatedBy = userDetail.UserID;
                        AFrom.APARFormHdr.UpdatedOn = DateTime.Now;
                        res = appraisalService.UpdateAPARFormData(AFrom);
                        msg = ButtonType == "Save" ? "Form has been saved successfully." : ButtonType == "Submit" ? "Form has been submitted successfully for further review.": "Form has been reverted to employee successfully.";
                    }
                    if (res > 0)
                    {
                        TempData.Remove("APARFormDtls");
                        return Json(new { formState = aparForm.APARFormHdr.StatusID, part = 0, msgType = "success", msg = msg, button = ButtonType }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            aparForm = (Models.APARSkillsViewModel)TempData["APARFormDtls"];
            TempData.Keep("APARFormDtls");
            return PartialView("_APARSkillForm", aparForm);

        }

        public ActionResult ApprovalTabEdit(int? reportingTo, int? empID, int departmentID, int designationID, string ryr = null)
        {
            log.Info($"APARSkillsController/Edit{reportingTo}/{empID}");
            try
            {
                Models.APARSkillsViewModel aparForm = new Models.APARSkillsViewModel();

                #region Skill Set popup
                var selectedCheckboxesBehavioral = new List<CheckBox>();
                var getFieldsValueBehavioral = ddlService.ddlSkill(2);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aparForm.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                aparForm.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;

                //-----------------------------------
                var selectedCheckboxesFunctional = new List<CheckBox>();
                var getFieldsValueFunctional = ddlService.ddlSkill(3);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aparForm.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                aparForm.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;
                #endregion

                aparForm.ReportingTo = reportingTo;
                aparForm.EmployeeID = (int)empID;
                aparForm.LoggedInEmpID = reportingTo;
                var reportingYr = ryr ?? DateTime.Now.GetFinancialYr();
                aparForm.APARFormHdr = new APARSkillSetFormHdr();
                aparForm.APARFormHdr = appraisalService.GetFormAPARHdrDetail(null, aparForm.EmployeeID, reportingYr);
                aparForm.APARFormHdr.ReportingYr = aparForm.APARFormHdr.ReportingYr == null ? reportingYr : aparForm.APARFormHdr.ReportingYr;
                var aparFromDetail = appraisalService.GetAPARFormDetail(aparForm.EmployeeID, departmentID, designationID, reportingYr);
                aparForm.Part1BehavioralList = aparFromDetail.Where(x => (x.PartID == 1 || x.PartID == 0) && x.SkillTypeID == 2).ToList();
                aparForm.Part1FunctionalList = aparFromDetail.Where(x => (x.PartID == 1 || x.PartID == 0) && x.SkillTypeID == 3).ToList();

                aparForm.Part2BehavioralList = aparFromDetail.Where(x => (x.PartID == 2 || x.PartID == 0) && x.SkillTypeID == 2).ToList();
                if (aparForm.Part2BehavioralList.Count == 0)
                {
                    var lff = (aparFromDetail.Where(x => x.SkillTypeID == 2).ToList());
                    aparForm.Part2BehavioralList = new List<APARSkillSetFormDetail>();
                    lff.ForEach(x =>
                    {
                        aparForm.Part2BehavioralList.Add(new APARSkillSetFormDetail()
                        {
                            SkillID = x.SkillID,
                            SkillTypeID = x.SkillTypeID,
                            Skill = x.Skill,
                            SkillSetID = x.SkillSetID,
                            SkillRemark = x.SkillRemark
                        });
                    });
                }
                aparForm.Part2FunctionalList = aparFromDetail.Where(x => (x.PartID == 2 || x.PartID == 0) && x.SkillTypeID == 3).ToList();
                if (aparForm.Part2FunctionalList.Count == 0)
                {
                    var lff = (aparFromDetail.Where(x => x.SkillTypeID == 3).ToList());
                    aparForm.Part2FunctionalList = new List<APARSkillSetFormDetail>();
                    lff.ForEach(x =>
                    {
                        aparForm.Part2FunctionalList.Add(new APARSkillSetFormDetail()
                        {
                            SkillID = x.SkillID,
                            SkillTypeID = x.SkillTypeID,
                            Skill = x.Skill,
                            SkillSetID = x.SkillSetID,
                            SkillRemark = x.SkillRemark
                        });
                    });
                }

                TempData["APARFormDtls"] = aparForm;
                TempData.Keep("APARFormDtls");
                return View("APARFormContainer", aparForm);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult ViewACAR(int? reportingTo, int? empID, int departmentID, int designationID, string ryr = null)
        {
            log.Info($"APARSkillsController/ViewACAR{reportingTo}/{empID}");
            try
            {
                Models.APARSkillsViewModel aparForm = new Models.APARSkillsViewModel();

                #region Skill Set popup
                var selectedCheckboxesBehavioral = new List<CheckBox>();
                var getFieldsValueBehavioral = ddlService.ddlSkill(2);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aparForm.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                aparForm.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;

                //-----------------------------------
                var selectedCheckboxesFunctional = new List<CheckBox>();
                var getFieldsValueFunctional = ddlService.ddlSkill(3);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aparForm.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                aparForm.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;
                #endregion

                aparForm.ReportingTo = reportingTo;
                aparForm.EmployeeID = (int)empID;
                if (reportingTo.HasValue)
                    aparForm.LoggedInEmpID = reportingTo;
                aparForm.isAdmin = true;
                //aparForm.LoggedInEmpID = userDetail.EmployeeID;
                var reportingYr = ryr ?? DateTime.Now.GetFinancialYr();
                aparForm.APARFormHdr = new APARSkillSetFormHdr();
                aparForm.APARFormHdr = appraisalService.GetFormAPARHdrDetail(null, aparForm.EmployeeID, reportingYr);
                aparForm.APARFormHdr.ReportingYr = aparForm.APARFormHdr.ReportingYr == null ? reportingYr : aparForm.APARFormHdr.ReportingYr;
                var aparFromDetail = appraisalService.GetAPARFormDetail(aparForm.EmployeeID, departmentID, designationID, reportingYr);
                aparForm.Part1BehavioralList = aparFromDetail.Where(x => (x.PartID == 1 || x.PartID == 0) && x.SkillTypeID == 2).ToList();
                aparForm.Part1FunctionalList = aparFromDetail.Where(x => (x.PartID == 1 || x.PartID == 0) && x.SkillTypeID == 3).ToList();

                aparForm.Part2BehavioralList = aparFromDetail.Where(x => (x.PartID == 2 || x.PartID == 0) && x.SkillTypeID == 2).ToList();
                if (aparForm.Part2BehavioralList.Count == 0)
                {
                    var lff = (aparFromDetail.Where(x => x.SkillTypeID == 2).ToList());
                    aparForm.Part2BehavioralList = new List<APARSkillSetFormDetail>();
                    lff.ForEach(x =>
                    {
                        aparForm.Part2BehavioralList.Add(new APARSkillSetFormDetail()
                        {
                            SkillID = x.SkillID,
                            SkillTypeID = x.SkillTypeID,
                            Skill = x.Skill,
                            SkillSetID = x.SkillSetID,
                            SkillRemark = x.SkillRemark
                        });
                    });
                }
                aparForm.Part2FunctionalList = aparFromDetail.Where(x => (x.PartID == 2 || x.PartID == 0) && x.SkillTypeID == 3).ToList();
                if (aparForm.Part2FunctionalList.Count == 0)
                {
                    var lff = (aparFromDetail.Where(x => x.SkillTypeID == 3).ToList());
                    aparForm.Part2FunctionalList = new List<APARSkillSetFormDetail>();
                    lff.ForEach(x =>
                    {
                        aparForm.Part2FunctionalList.Add(new APARSkillSetFormDetail()
                        {
                            SkillID = x.SkillID,
                            SkillTypeID = x.SkillTypeID,
                            Skill = x.Skill,
                            SkillSetID = x.SkillSetID,
                            SkillRemark = x.SkillRemark
                        });
                    });
                }

                TempData["APARFormDtls"] = aparForm;
                TempData.Keep("APARFormDtls");
                return View("APARFormContainer", aparForm);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}