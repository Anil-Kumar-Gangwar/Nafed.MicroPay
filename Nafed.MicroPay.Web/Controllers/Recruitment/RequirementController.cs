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
using Nafed.MicroPay.Services.Recruitment;
using AutoMapper;
using System.Data;
using System.Web.Security.AntiXss;
using Microsoft.Security.Application;

namespace MicroPay.Web.Controllers.Recruitment
{
    public class RequirementController : BaseController
    {
        private readonly IRecruitmentService recruitmentService;
        private readonly IDropdownBindService ddlService;

        public RequirementController(IRecruitmentService recruitmentService, IDropdownBindService ddlService)
        {
            this.recruitmentService = recruitmentService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            log.Info($"RequirementController/Index");
            RequirementViewModel requirementVM = new RequirementViewModel();
            requirementVM.designationList = ddlService.ddlDesignationList();
            requirementVM.userRights = userAccessRight;
            return View(requirementVM);
        }

        public ActionResult _GetRequirementGridView(RequirementViewModel requirementVM)
        {
            log.Info($"RequirementController/_GetRequirementGridView");
            try
            {
                RequirementViewModel requirVM = new RequirementViewModel();
                requirementVM.designationList = ddlService.ddlDesignationList();
                if (requirementVM.ToDate.HasValue && requirementVM.FromDate == null)
                {
                    //ModelState.AddModelError("FromDateValidation", "Please select publish from date.");
                    //return PartialView("_RequirementGridView", requirementVM);
                    return Json(new { htmlData = "", msgType = "warning", msg = "Please select publish from date." }, JsonRequestBehavior.AllowGet);
                }
                if (requirementVM.ToDate.HasValue && requirementVM.FromDate.Value > requirementVM.ToDate.Value)
                {
                    return Json(new { htmlData = "", msgType = "warning", msg = "To Date cannot less than From Date" }, JsonRequestBehavior.AllowGet);
                    //ModelState.AddModelError("ToDateRangeValidation", "To Date cannot less than From Date.");
                    //return PartialView("_RequirementGridView", requirementVM);
                }
                requirVM.RequirementList = recruitmentService.GetRequirementDetails(requirementVM.DesignationId, requirementVM.FromDate, requirementVM.ToDate);
                requirVM.userRights = userAccessRight;
                return Json(new { msgType = "sucess", htmlData = ConvertViewToString("_RequirementGridView", requirVM) }, JsonRequestBehavior.AllowGet);
                //return PartialView("_RequirementGridView", requirVM);
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
            log.Info("RequirementController/Create");
            try
            {
                BindDropdowns();
                Model.Requirement objRequirement = new Model.Requirement();
                CheckBoxListViewModel chklistVM = new CheckBoxListViewModel();
                var qualiList = QualificationList();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                chklistVM.AvailableFields = Mapper.Map<List<CheckBox>>(qualiList.ToList());
                objRequirement.QualificationCheckboxList = chklistVM;
                return View(objRequirement);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.SelectListModel> QualificationList()
        {
            try
            {
                List<Model.SelectListModel> qualificationList = new List<Model.SelectListModel>();
                qualificationList.Add(new Model.SelectListModel { id = 1, value = "10th" });
                qualificationList.Add(new Model.SelectListModel { id = 2, value = "12th" });
                qualificationList.Add(new Model.SelectListModel { id = 3, value = "Graduation" });
                qualificationList.Add(new Model.SelectListModel { id = 4, value = "Post Graduation" });
                qualificationList.Add(new Model.SelectListModel { id = 5, value = "Diploma" });
                qualificationList.Add(new Model.SelectListModel { id = 6, value = "Professional" });
                return qualificationList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Model.Requirement requirement, CheckBoxListViewModel chklistVM, string ButtonType)
        {
            log.Info("RequirementController/Create");
            try
            {
                if (ButtonType == "Create")
                {
                    BindDropdowns();
                    if (chklistVM.PostedFields != null)
                        requirement.jobLocations = chklistVM.PostedFields.fieldIds;
                    if (chklistVM.PostedFields1 != null)
                        requirement.JobQualification = chklistVM.PostedFields1.fieldIds;

                    List<Model.SelectListModel> result = new List<Model.SelectListModel>();
                    List<int> postedList = new List<int>();
                    if (chklistVM.PostedFields != null)
                        postedList = chklistVM.PostedFields.fieldIds.ToList();
                    var getFieldsValue = recruitmentService.GetFilterFields((int)requirement.requirementOptionType);

                    result = getFieldsValue.Where(p => postedList.Any(x => x == p.id)).ToList();

                    if (requirement.JobExamCenterDetails != null && ((int)requirement.requirementOptionType == 1 || (int)requirement.requirementOptionType == 2))
                        requirement.LocationList = result;
                    else if ((int)requirement.requirementOptionType == 3)
                    {
                        result.Add(new Model.SelectListModel { id = 3, value = "Anywhere in India" });
                        requirement.LocationList = result;
                    }

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    chklistVM.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValue.ToList());
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<int, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s));
                        // .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    chklistVM.SelectedFields = Mapper.Map<List<CheckBox>>(requirement.jobLocations);
                    requirement.CheckboxList = chklistVM;

                    CheckBoxListViewModel chklistVM1 = new CheckBoxListViewModel();

                    var qualiList = QualificationList();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    chklistVM1.AvailableFields = Mapper.Map<List<CheckBox>>(qualiList);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<int, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s));
                        // .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    chklistVM1.SelectedFields = Mapper.Map<List<CheckBox>>(requirement.JobQualification);
                    requirement.QualificationCheckboxList = chklistVM1;

                    if (requirement.PublishDate.HasValue && requirement.PublishDate.Value > requirement.LastDateofApplicationReceived.Value)
                        ModelState.AddModelError("LastDateofApplicationReceivedRangeValidation", "Last Date of Application Received cannot less than Publish Date.");
                    if (requirement.LastDateofApplicationReceived.HasValue && requirement.LastDateofApplicationReceived.Value > requirement.WrittenExamDate)
                        ModelState.AddModelError("WrittenExamDateRangeValidation", "Written Exam Date cannot less than Last Date of Application Received.");
                    if (requirement.GDExamDate.HasValue && requirement.LastDateofApplicationReceived.HasValue && requirement.LastDateofApplicationReceived.Value > requirement.GDExamDate.Value)
                        ModelState.AddModelError("GDExamDateRangeValidation", "GD Exam Date cannot less than Last Date of Application Received.");
                    if (requirement.enumEmployementType == 0)
                        ModelState.AddModelError("EmployementTypeValidation", "Employement type is required.");
                    if ((int)requirement.requirementOptionType < 3 && (int)requirement.requirementOptionType > 0 && chklistVM.PostedFields == null)
                        ModelState.AddModelError("RequirementOptionType", "Please select one of them.");
                    if ((int)requirement.requirementOptionType <= 0)
                        ModelState.AddModelError("LocationType", "Please select one of them.");
                    if (chklistVM.PostedFields1 == null)
                        ModelState.AddModelError("QualificationType", "Please select one of them.");
                    if (requirement.MinimumAgeLimits > requirement.MaximumAgeLimit)
                        ModelState.AddModelError("MinimumAgeValidation", "Minimum age should not greater then maximum age");
                    if (ModelState.IsValid)
                    {
                        requirement.Post = requirement.Post.Trim();
                        requirement.CreatedBy = userDetail.UserID;
                        requirement.CreatedOn = System.DateTime.Now;
                        requirement.PayScale = requirement.PayScale.Trim();
                        requirement.MinimumQualification = requirement.MinimumQualification.Trim();
                        requirement.MinimumExperinceDetail = requirement.MinimumExperinceDetail.Trim();
                        requirement.DesiredKeySkills = requirement.DesiredKeySkills != null ? requirement.DesiredKeySkills.Trim() : null;
                        requirement.OptionalKeySkills = requirement.OptionalKeySkills != null ? requirement.OptionalKeySkills.Trim() : null;
                        requirement.Essential_Duties_and_Responsibilities = requirement.Essential_Duties_and_Responsibilities.Trim();
                        requirement.MaximumAgeLimitDesc = requirement.MaximumAgeLimitDesc.Trim();
                        requirement.MethodofRecruitment = requirement.MethodofRecruitment.Trim();
                        requirement.INSTRUCTIONS = requirement.INSTRUCTIONS.Trim();
                        requirement.BranchID = requirement.BranchID == 0 ? null : requirement.BranchID;
                        requirement.Employementtype = (int?)requirement.enumEmployementType;
                        requirement.JLocTypeId = (int)requirement.requirementOptionType;


                        if (recruitmentService.RequirementAlreadyExists(requirement.RequirementID, requirement.DesinationID, requirement.PublishDate))
                        {
                            ModelState.AddModelError("RequirementAlreadyExist", "Requirement for this designation already open.");
                            //return View(requirement);
                            requirement.ModelIsValid = 2;
                            return PartialView("_CreateRequirement", requirement);
                        }
                        else
                        {
                            requirement.ModelIsValid = 1;

                            if (ValidateAntiXSS(requirement))
                            {
                                ModelState.AddModelError("", "Updation failed, A potentially dangerous request value was detected.");
                                requirement.ModelIsValid = 2;
                                return PartialView("_CreateRequirement", requirement);
                            }


                            recruitmentService.InsertRequirememnt(requirement);
                            TempData["Message"] = "Successfully Created";
                            return Json(new
                            {
                                part = 6,
                                msgType = "success",
                                msg = "Sucessfully Created"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //requirement.JLocTypeId = (int)requirement.requirementOptionType;
                        //List<int> postedList = new List<int>();
                        //if (chklistVM.PostedFields != null)
                        //    postedList = chklistVM.PostedFields.fieldIds.ToList();
                        //var jobFilter = recruitmentService.GetFilterFields(requirement.JLocTypeId);
                        //var result = jobFilter.Where(p => postedList.Any(x => x == p.id)).ToList();
                        requirement.ModelIsValid = 2;
                        return PartialView("_CreateRequirement", requirement);
                    }
                }
                else if (ButtonType == "Add Examination Center")
                {
                    requirement.JLocTypeId = (int)requirement.requirementOptionType;
                    List<Model.SelectListModel> result = new List<Model.SelectListModel>();
                    List<int> postedList = new List<int>();
                    if (requirement.JLocTypeId == 1 || requirement.JLocTypeId == 2)
                    {
                        if (chklistVM.PostedFields != null)
                            postedList = chklistVM.PostedFields.fieldIds.ToList();
                        var jobFilter = recruitmentService.GetFilterFields(requirement.JLocTypeId);
                        result = jobFilter.Where(p => postedList.Any(x => x == p.id)).ToList();
                        requirement.LocationList = result;
                    }
                    else if (requirement.JLocTypeId == 3)
                    {
                        result.Add(new Model.SelectListModel { id = 3, value = "Anywhere in India" });
                        requirement.LocationList = result;
                    }

                    if (requirement.JobExamCenterDetails == null)
                        requirement.JobExamCenterDetails = new List<Model.JobExamCenterDetails>() {
                           new Model.JobExamCenterDetails() { sno = 1/*,LocationList= result*/} };
                    else
                    {
                        if (requirement.JobExamCenterDetails.Count == 1)
                        {
                            requirement.JobExamCenterDetails.FirstOrDefault().sno = 1;
                            //requirement.JobExamCenterDetails.FirstOrDefault().LocationList = result;
                        }
                        else
                        {
                            var s_no = 1;
                            requirement.JobExamCenterDetails.ForEach(x =>
                            {
                                x.sno = s_no++;
                                //x.LocationList = result;
                            });

                        }
                        if (requirement.JLocTypeId == 3 && requirement.JobExamCenterDetails.Count < 1)
                            requirement.JobExamCenterDetails.Add(new Model.JobExamCenterDetails()
                            {
                                sno = requirement.JobExamCenterDetails.Count + 1,
                                //LocationList = result
                            });
                        else if (requirement.JLocTypeId == 1 || requirement.JLocTypeId == 2)
                            requirement.JobExamCenterDetails.Add(new Model.JobExamCenterDetails()
                            {
                                sno = requirement.JobExamCenterDetails.Count + 1,
                                //LocationList = result
                            });
                    }
                    TempData["RequirementExamCenterDetails"] = requirement;
                    return Json(new { part = 1, htmlData = ConvertViewToString("_ExamCenterDetails", requirement) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return PartialView("_CreateRequirement", requirement);
        }

        [HttpGet]
        public ActionResult Edit(int requirementId)
        {
            log.Info("RequirementController/Edit");
            try
            {
                BindDropdowns();
                Model.Requirement objRequirement = new Model.Requirement();
                CheckBoxListViewModel chklistVM = new CheckBoxListViewModel();
                objRequirement = recruitmentService.GetRequirementByID(requirementId);
                //objRequirement.CreatedBy = 1;
                if (objRequirement.JobExamCenterDetails.Count > 0)
                {
                    if (objRequirement.JLocTypeId == 1 || objRequirement.JLocTypeId == 2)
                    {
                        var selectedExamCenter = recruitmentService.GetJobLocation(requirementId);
                        objRequirement.LocationList = selectedExamCenter;
                    }
                    else if (objRequirement.JLocTypeId == 3)
                    {
                        List<Model.SelectListModel> result = new List<Model.SelectListModel>();
                        result.Add(new Model.SelectListModel { id = 3, value = "Anywhere in India" });
                        objRequirement.LocationList = result;
                    }
                    int sno = 1;
                    objRequirement.JobExamCenterDetails.ForEach(x =>
                    {
                        x.sno = sno++;
                    });
                }
                if (objRequirement.JLocTypeId == 1)
                    objRequirement.requirementOptionType = Model.RequirementOptionType.SpecificBranch;
                if (objRequirement.JLocTypeId == 2)
                    objRequirement.requirementOptionType = Model.RequirementOptionType.SpecificZone;
                if (objRequirement.JLocTypeId == 3)
                    objRequirement.requirementOptionType = Model.RequirementOptionType.Anywhereinindia;

                objRequirement.enumEmployementType = (Model.EmployementType)(objRequirement.Employementtype);


                var qualiList = QualificationList();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                chklistVM.AvailableFields = Mapper.Map<List<CheckBox>>(qualiList.ToList());
                var selectedFields = recruitmentService.GetJobQualification(requirementId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    /*.ForMember(d => d.Name, o => o.MapFrom(s => s.value))*/;
                });
                chklistVM.SelectedFields = Mapper.Map<List<CheckBox>>(selectedFields);
                objRequirement.QualificationCheckboxList = chklistVM;

                objRequirement.editFlag = 1;
                TempData["RequirementExamCenterDetails"] = objRequirement;
                return View(objRequirement);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Model.Requirement requirement, CheckBoxListViewModel chklistVM, string ButtonType)
        {
            log.Info("RequirementController/Edit");
            try
            {
                if (ButtonType == "Update")
                {
                    BindDropdowns();
                    if (chklistVM.PostedFields != null)
                        requirement.jobLocations = chklistVM.PostedFields.fieldIds;
                    if (chklistVM.PostedFields1 != null)
                        requirement.JobQualification = chklistVM.PostedFields1.fieldIds;

                    List<Model.SelectListModel> result = new List<Model.SelectListModel>();
                    List<int> postedList = new List<int>();
                    if (chklistVM.PostedFields != null)
                        postedList = chklistVM.PostedFields.fieldIds.ToList();

                    var getFieldsValue = recruitmentService.GetFilterFields((int)requirement.requirementOptionType);

                    result = getFieldsValue.Where(p => postedList.Any(x => x == p.id)).ToList();

                    if (requirement.JobExamCenterDetails != null && ((int)requirement.requirementOptionType == 1 || (int)requirement.requirementOptionType == 2))
                        requirement.LocationList = result;
                    else if ((int)requirement.requirementOptionType == 3)
                    {
                        result.Add(new Model.SelectListModel { id = 3, value = "Anywhere in India" });
                        requirement.LocationList = result;
                    }

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    chklistVM.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValue.ToList());
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<int, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s));
                        // .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    chklistVM.SelectedFields = Mapper.Map<List<CheckBox>>(requirement.jobLocations);
                    requirement.CheckboxList = chklistVM;

                    CheckBoxListViewModel chklistVM1 = new CheckBoxListViewModel();

                    var qualiList = QualificationList();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    chklistVM1.AvailableFields = Mapper.Map<List<CheckBox>>(qualiList);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<int, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s));
                        // .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    chklistVM1.SelectedFields = Mapper.Map<List<CheckBox>>(requirement.JobQualification);
                    requirement.QualificationCheckboxList = chklistVM1;

                    if (requirement.PublishDate.HasValue && requirement.LastDateofApplicationReceived.Value < requirement.PublishDate.Value)
                        ModelState.AddModelError("LastDateofApplicationReceivedRangeValidation", "Last Date of Application Received cannot less than Publish Date.");
                    if (requirement.LastDateofApplicationReceived.HasValue && requirement.LastDateofApplicationReceived.Value > requirement.WrittenExamDate)
                        ModelState.AddModelError("WrittenExamDateRangeValidation", "Written Exam Date cannot less than Last Date of Application Received.");
                    if (requirement.GDExamDate.HasValue && requirement.LastDateofApplicationReceived.HasValue && requirement.LastDateofApplicationReceived.Value > requirement.GDExamDate.Value)
                        ModelState.AddModelError("GDExamDateRangeValidation", "GD Exam Date cannot less than Last Date of Application Received.");
                    if (requirement.enumEmployementType == 0)
                        ModelState.AddModelError("EmployementTypeValidation", "Employement type is required.");
                    if ((int)requirement.requirementOptionType < 3 && (int)requirement.requirementOptionType > 0 && chklistVM.PostedFields == null)
                        ModelState.AddModelError("RequirementOptionType", "Please select one of them.");
                    if ((int)requirement.requirementOptionType <= 0)
                        ModelState.AddModelError("LocationType", "Please select one of them.");
                    if (chklistVM.PostedFields1 == null)
                        ModelState.AddModelError("QualificationType", "Please select one of them.");
                    if (requirement.MinimumAgeLimits > requirement.MaximumAgeLimit)
                        ModelState.AddModelError("MinimumAgeValidation", "Minimum age should not greater then maximum age");
                    if (ModelState.IsValid)
                    {
                        requirement.Post = requirement.Post.Trim();
                        requirement.UpdatedBy = userDetail.UserID;
                        requirement.UpdatedOn = System.DateTime.Now;
                        requirement.PayScale = requirement.PayScale.Trim();
                        requirement.MinimumQualification = requirement.MinimumQualification.Trim();
                        requirement.MinimumExperinceDetail = requirement.MinimumExperinceDetail.Trim();
                        requirement.DesiredKeySkills = requirement.DesiredKeySkills != null ? requirement.DesiredKeySkills.Trim() : null;
                        requirement.OptionalKeySkills = requirement.OptionalKeySkills != null ? requirement.OptionalKeySkills.Trim() : null;
                        requirement.Essential_Duties_and_Responsibilities = requirement.Essential_Duties_and_Responsibilities.Trim();
                        requirement.MaximumAgeLimitDesc = requirement.MaximumAgeLimitDesc.Trim();
                        requirement.MethodofRecruitment = requirement.MethodofRecruitment.Trim();
                        requirement.INSTRUCTIONS = requirement.INSTRUCTIONS.Trim();
                        requirement.BranchID = requirement.BranchID == 0 ? null : requirement.BranchID;
                        requirement.Employementtype = (int?)requirement.enumEmployementType;
                        //requirement.jobLocations = chklistVM.PostedFields.fieldIds;
                        requirement.JLocTypeId = (int)requirement.requirementOptionType;

                        if (recruitmentService.RequirementAlreadyExists(requirement.RequirementID, requirement.DesinationID, requirement.PublishDate))
                        {
                            ModelState.AddModelError("RequirementAlreadyExist", "Requirement for this designation already open.");
                            //return View(requirement);
                            requirement.ModelIsValid = 2;
                            return PartialView("_EditRequirement", requirement);
                        }
                        else
                        {
                            requirement.ModelIsValid = 1;                          

                            if (ValidateAntiXSS(requirement))
                            {
                                ModelState.AddModelError("", "Updation failed, A potentially dangerous request value was detected.");
                                requirement.ModelIsValid = 2;
                                requirement.editFlag = 0;
                                return PartialView("_EditRequirement", requirement);
                            }
                            recruitmentService.UpdateRequirememnt(requirement);
                            TempData["Message"] = "Sucessfully Update";
                            return Json(new
                            {
                                part = 6,
                                msgType = "success",
                                msg = "Sucessfully Update",
                                id = 5
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        requirement.ModelIsValid = 2;
                        requirement.editFlag = 0;
                        return PartialView("_EditRequirement", requirement);
                    }
                }
                else if (ButtonType == "Add Examination Center")
                {
                    requirement.JLocTypeId = (int)requirement.requirementOptionType;
                    List<Model.SelectListModel> result = new List<Model.SelectListModel>();
                    List<int> postedList = new List<int>();
                    if (requirement.JLocTypeId == 1 || requirement.JLocTypeId == 2)
                    {
                        if (chklistVM.PostedFields != null)
                            postedList = chklistVM.PostedFields.fieldIds.ToList();
                        var jobFilter = recruitmentService.GetFilterFields(requirement.JLocTypeId);
                        result = jobFilter.Where(p => postedList.Any(x => x == p.id)).ToList();
                        requirement.LocationList = result;
                    }
                    else if (requirement.JLocTypeId == 3)
                    {
                        result.Add(new Model.SelectListModel { id = 3, value = "Anywhere in India" });
                        requirement.LocationList = result;
                    }

                    if (requirement.JobExamCenterDetails == null)
                        requirement.JobExamCenterDetails = new List<Model.JobExamCenterDetails>() {
                           new Model.JobExamCenterDetails() { sno = 1/*,LocationList= result*/} };
                    else
                    {
                        if (requirement.JobExamCenterDetails.Count == 1)
                        {
                            requirement.JobExamCenterDetails.FirstOrDefault().sno = 1;
                            //requirement.JobExamCenterDetails.FirstOrDefault().LocationList = result;
                        }
                        else
                        {
                            var s_no = 1;
                            requirement.JobExamCenterDetails.ForEach(x =>
                            {
                                x.sno = s_no++;
                                //x.LocationList = result;
                            });
                        }
                        if (requirement.JLocTypeId == 3 && requirement.JobExamCenterDetails.Count < 1)
                            requirement.JobExamCenterDetails.Add(new Model.JobExamCenterDetails()
                            {
                                sno = requirement.JobExamCenterDetails.Count + 1,
                                //LocationList = result
                            });
                        else if (requirement.JLocTypeId == 1 || requirement.JLocTypeId == 2)
                            requirement.JobExamCenterDetails.Add(new Model.JobExamCenterDetails()
                            {
                                sno = requirement.JobExamCenterDetails.Count + 1,
                                //LocationList = result
                            });
                    }
                    TempData["RequirementExamCenterDetails"] = requirement;
                    return Json(new { part = 1, htmlData = ConvertViewToString("_ExamCenterDetails", requirement) }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return PartialView("_EditRequirement", requirement);
        }

        public ActionResult Delete(int requirementID)
        {
            log.Info("RequirementController/Delete");
            try
            {
                recruitmentService.Delete(requirementID);
                TempData["Message"] = "successfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

        private void BindDropdowns()
        {
            var ddlDesignationList = ddlService.ddlDesignationList();
            ddlDesignationList.OrderBy(x => x.value);
            Model.SelectListModel selectDesignation = new Model.SelectListModel();
            selectDesignation.id = 0;
            selectDesignation.value = "Select";
            ddlDesignationList.Insert(0, selectDesignation);
            ViewBag.DesignationList = new SelectList(ddlDesignationList, "id", "value");

            var ddlBranchList = ddlService.ddlBranchList();
            ddlBranchList.OrderBy(x => x.value);
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.BranchList = new SelectList(ddlBranchList, "id", "value");
        }

        public ActionResult _FillFilterFieldList(int fieldID, int? jLocTypeId, int? requirementId)
        {
            log.Info($"RequirementController/_FillFilterFieldList/{fieldID}");

            IEnumerable<Model.SelectListModel> fields = Enumerable.Empty<Model.SelectListModel>();
            var model = new CheckBoxListViewModel();
            try
            {
                if (fieldID < 3)
                {
                    // var selectedCheckboxes = new List<CheckBox>();
                    var getFieldsValue = recruitmentService.GetFilterFields(fieldID);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    model.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValue);
                    //model.SelectedFields = selectedCheckboxes;
                    //return PartialView("_ReqFilterFieldList", model);

                    if (jLocTypeId.HasValue && requirementId.HasValue)
                    {
                        var jobRequirementLoc = recruitmentService.GetLocRequirement(jLocTypeId.Value, requirementId.Value);
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.SelectListModel, CheckBox>()
                            .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                            .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                        });
                        model.SelectedFields = Mapper.Map<List<CheckBox>>(jobRequirementLoc);
                    }
                    return PartialView("_ReqFilterFieldList", model);
                }
                else
                {
                    return Content("");
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            //return PartialView("_FilterFieldList", model);
        }

        [HttpGet]
        public ActionResult _RemoveExamCenterRow(int sNo)
        {
            var modelData = (Model.Requirement)TempData["RequirementExamCenterDetails"];
            if (modelData != null)
            {
                var deletedRow = modelData.JobExamCenterDetails.Where(x => x.sno == sNo).FirstOrDefault();
                modelData.JobExamCenterDetails.Remove(deletedRow);
                TempData["RequirementExamCenterDetails"] = modelData;
                return PartialView("_ExamCenterDetails", modelData);
            }
            return Content("");
        }

        [HttpPost]
        public ActionResult _FillExamCenterDetails(int requirementId, int jLocTypeId)
        {
            log.Info($"RequirementController/_FillExamCenterDetails/{requirementId}");
            try
            {
                Model.Requirement requirement = new Model.Requirement();
                var examCenterDetails = recruitmentService.GetExamCenterDetails(requirementId);
                if (examCenterDetails != null)
                {
                    if (jLocTypeId == 1 || jLocTypeId == 2)
                    {
                        var selectedExamCenter = recruitmentService.GetJobLocation(requirementId);
                        requirement.LocationList = selectedExamCenter;
                    }
                    else
                    {
                        List<Model.SelectListModel> result = new List<Model.SelectListModel>();
                        result.Add(new Model.SelectListModel { id = 3, value = "Anywhere in India" });
                        requirement.LocationList = result;
                    }
                    requirement.JobExamCenterDetails = examCenterDetails;
                    TempData["RequirementExamCenterDetails"] = requirement;
                    return Json(new { part = 1, htmlData = ConvertViewToString("_ExamCenterDetails", requirement) }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { part = 0, htmlData = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult AppliedCandidatesList(int requirementId)
        {
            log.Info($"RequirementController/AppliedCandidatesList/requirementId={requirementId}");
            try
            {
                RequirementViewModel requirVM = new RequirementViewModel();
                requirVM.appliedCandidateList = recruitmentService.AppliedCandidatesList(requirementId);
                var postName = requirVM.appliedCandidateList.FirstOrDefault().Position;
                requirVM.PostName = postName;
                var CandidateDT = Common.ExtensionMethods.ToDataTable(requirVM.appliedCandidateList);
                TempData["appliedCandidateList"] = CandidateDT;
                return PartialView("_AppliedCandidateList", requirVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _ExportTemplate(string postName)
        {
            DataTable exportData = new DataTable();

            if (TempData["appliedCandidateList"] != null)
            {
                string fileName = string.Empty, msg = string.Empty;
                string fullPath = Server.MapPath("~/FileDownload/");

                List<Model.AppliedCandidateDetail> appliedCandidateList = new List<Model.AppliedCandidateDetail>();
                fileName = "Recruitment Report-" + postName + '.' + Common.FileExtension.xlsx;
                //Common.ExtensionMethods.SetUniqueFileName("Recruitment Report-", Common.FileExtension.xlsx);
                TempData.Keep("appliedCandidateList");
                var appliedcanDT = (DataTable)TempData["appliedCandidateList"];
                if (appliedcanDT.Columns.Contains("TotalExperience"))
                {
                    foreach (DataRow dr in appliedcanDT.Rows)
                    {
                        dr["RelevantExperience"] = "TotalExp. " + dr["TotalExperience"].ToString() + Environment.NewLine + "RelevantExp. " + dr["RelevantExperience"].ToString() + Environment.NewLine + "GovtExp. " + dr["GovtExperience"].ToString() + Environment.NewLine + "GovtReleExp. " + dr["GovtReleExp"].ToString();//assign value of column 4 to column1                     
                    }
                    appliedcanDT.Columns.Remove("TotalExperience");
                    appliedcanDT.Columns.Remove("GovtExperience");
                    appliedcanDT.Columns.Remove("GovtReleExp");
                }
                var res = recruitmentService.ExportAppCandidateList(appliedcanDT, fullPath, fileName);
                fullPath = $"{fullPath}{fileName}";
                return JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");

            }
            return Content("Refreshed Data");
        }
    }
}