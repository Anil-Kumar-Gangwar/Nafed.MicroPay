using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using MicroPay.Web.Models;
using AutoMapper;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Controllers.Appraisal
{

    public class APARSkillsSetController : BaseController
    {
        private readonly IAppraisalFormService appraisalService;
        private readonly IDropdownBindService ddlService;
        public APARSkillsSetController(IAppraisalFormService appraisalService, IDropdownBindService ddlService)
        {
            this.appraisalService = appraisalService;
            this.ddlService = ddlService;
        }
        // GET: APARSkills
        public ActionResult Index()
        {
            APARSkillSetViewModel aprVM = new Models.APARSkillSetViewModel();
            aprVM.UserRights = userAccessRight;
            return View(aprVM);
        }
        public ActionResult GetAPARSkillSetList()
        {
            log.Info($"APARSkillsSetController/GetAPARSkillDetail");
            try
            {
                APARSkillSetViewModel aparVM = new APARSkillSetViewModel();
                //if (userDetail.UserName.ToLower() != "admin")
                //{
                //    aparVM.APARSkillSetList = appraisalService.GetAPARSkillList(userDetail.DepartmentID, userDetail.DesignationID);
                //}
                //if (userDetail.UserName.ToLower() == "admin")
                //{
                aparVM.APARSkillSetList = appraisalService.GetAPARSkillList(null, null);
                //
                return PartialView("_APARSkillGridView", aparVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public ActionResult GetSkillsList(int SkillSetID, string departnemt, string designation)
        {
            log.Info($"APARSkillsSetController/GetSkillsList");
            try
            {
                APARSkillSetViewModel aparVM = new APARSkillSetViewModel();
                var arpSkillDetail = appraisalService.GetAPARSkillDetail(SkillSetID, null).ToList();
                aparVM.APARSkillSetDtlList = arpSkillDetail;
                aparVM.APARSkillSet = new APARSkillSet();
                aparVM.APARSkillSet.Department = departnemt;
                aparVM.APARSkillSet.Designation = designation;
                return PartialView("_APARSkillPopup", aparVM);
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
            log.Info($"APARSkillsSetController/Create");
            try
            {
                APARSkillSetViewModel aprVM = new Models.APARSkillSetViewModel();
                aprVM.DepartmentList = ddlService.ddlDepartmentList();
                aprVM.DesignationList = ddlService.ddlDesignationList();
                aprVM.APARSkillSet = new APARSkillSet();
                aprVM.APARSkillSet.DepartmentID = userDetail.DepartmentID;
                aprVM.APARSkillSet.DesignationID = (int)userDetail.DesignationID;
                var selectedCheckboxesBehavioral = new List<CheckBox>();
                var getFieldsValueBehavioral = ddlService.ddlSkill(2);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aprVM.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                aprVM.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;

                //-----------------------------------
                var selectedCheckboxesFunctional = new List<CheckBox>();
                var getFieldsValueFunctional = ddlService.ddlSkill(3);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aprVM.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                aprVM.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;

                return View(aprVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(APARSkillSetViewModel aprVM)
        {
            log.Info($"APARSkillsSetController/Create");
            try
            {
                APARSkills aprSkill = new APARSkills();
                aprSkill.APARSkillSet = new APARSkillSet();
                if (aprVM.CheckBoxListBehavioral.PostedFields != null)
                    aprSkill.CheckBoxListBehavioral = aprVM.CheckBoxListBehavioral.PostedFields.fieldIds;
                if (aprVM.CheckBoxListFunctional.PostedFields != null)
                    aprSkill.CheckBoxListFunctional = aprVM.CheckBoxListFunctional.PostedFields.fieldIds;
                aprSkill.APARSkillSet.EmployeeID = (int)userDetail.EmployeeID;
                aprSkill.APARSkillSet.DepartmentID = aprVM.APARSkillSet.DepartmentID;
                aprSkill.APARSkillSet.DesignationID = aprVM.APARSkillSet.DesignationID;
                aprSkill.APARSkillSet.CreatedBy = userDetail.UserID;
                aprSkill.APARSkillSet.CreatedOn = DateTime.Now;
                aprSkill.APARSkillSet.StatusID = 1;

                int res = appraisalService.InsertAPARSkill(aprSkill);
                if (res > 0)
                {
                    TempData["Message"] = "Saved Successfully.";
                    RedirectToAction("Index");
                }

                aprVM.DepartmentList = ddlService.ddlDepartmentList();
                aprVM.DesignationList = ddlService.ddlDesignationList();
                var selectedCheckboxesBehavioral = new List<CheckBox>();
                var getFieldsValueBehavioral = ddlService.ddlSkill(2);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aprVM.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                aprVM.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;

                //-----------------------------------
                var selectedCheckboxesFunctional = new List<CheckBox>();
                var getFieldsValueFunctional = ddlService.ddlSkill(3);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aprVM.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                aprVM.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;
                return View(aprVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int departmentID, int designationID, int skillSetID)
        {
            log.Info($"APARSkillsSetController/Edit{departmentID}/{designationID}/{skillSetID}");
            try
            {
                var arpSkillDetail = appraisalService.GetAPARSkillDetail(skillSetID, null);
                APARSkillSetViewModel aprVM = new Models.APARSkillSetViewModel();
                aprVM.APARSkillSet = new APARSkillSet();
                aprVM.DepartmentList = ddlService.ddlDepartmentList();
                aprVM.DesignationList = ddlService.ddlDesignationList();
                var selectedCheckboxesBehavioral = new List<CheckBox>();
                var getFieldsValueBehavioral = ddlService.ddlSkill(2);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aprVM.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);

                // selectedCheckboxesBehavioral = ff.Where(x => x.SkillTypeID == 2).ToList().Select(y => new CheckBox {Name=y.SkillID.ToString() });

                aprVM.CheckBoxListBehavioral.SelectedFields = arpSkillDetail.Where(x => x.SkillTypeID == 2).ToList().Select(y => new CheckBox { Id = y.SkillID });

                //-----------------------------------
                var selectedCheckboxesFunctional = new List<CheckBox>();
                var getFieldsValueFunctional = ddlService.ddlSkill(3);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aprVM.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                aprVM.CheckBoxListFunctional.SelectedFields = arpSkillDetail.Where(x => x.SkillTypeID == 3).ToList().Select(y => new CheckBox { Id = y.SkillID }); ;
                var getAPARSkillSet = arpSkillDetail.FirstOrDefault();
                aprVM.APARSkillSet.DepartmentID = getAPARSkillSet == null ? userDetail.DepartmentID : getAPARSkillSet.DepartmentID;
                aprVM.APARSkillSet.DesignationID = getAPARSkillSet == null ? (int)userDetail.DesignationID : getAPARSkillSet.DesignationID;
                aprVM.APARSkillSet.SkillSetID = getAPARSkillSet == null ? skillSetID : getAPARSkillSet.SkillSetID;

                return View(aprVM);
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(APARSkillSetViewModel aprVM)
        {
            log.Info($"APARSkillsSetController/Edit");
            try
            {
                APARSkills aprSkill = new APARSkills();
                aprSkill.APARSkillSet = new APARSkillSet();
                if (aprVM.CheckBoxListBehavioral.PostedFields != null)
                    aprSkill.CheckBoxListBehavioral = aprVM.CheckBoxListBehavioral.PostedFields.fieldIds;
                if (aprVM.CheckBoxListFunctional.PostedFields != null)
                    aprSkill.CheckBoxListFunctional = aprVM.CheckBoxListFunctional.PostedFields.fieldIds;
                aprSkill.APARSkillSet.EmployeeID = (int)userDetail.EmployeeID;
                aprSkill.APARSkillSet.DepartmentID = aprVM.APARSkillSet.DepartmentID;
                aprSkill.APARSkillSet.DesignationID = aprVM.APARSkillSet.DesignationID;
                aprSkill.APARSkillSet.SkillSetID = aprVM.APARSkillSet.SkillSetID;
                aprSkill.APARSkillSet.UpdatedBy = userDetail.UserID;
                aprSkill.APARSkillSet.UpdatedOn = DateTime.Now;

                int res = appraisalService.UpdateAPARSkill(aprSkill);
                if (res > 0)
                {
                    TempData["Message"] = "Updated Successfully.";
                    return RedirectToAction("Index");
                }

                aprVM.DepartmentList = ddlService.ddlDepartmentList();
                aprVM.DesignationList = ddlService.ddlDesignationList();
                var selectedCheckboxesBehavioral = new List<CheckBox>();
                var getFieldsValueBehavioral = ddlService.ddlSkill(2);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aprVM.CheckBoxListBehavioral.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueBehavioral);
                aprVM.CheckBoxListBehavioral.SelectedFields = selectedCheckboxesBehavioral;

                //-----------------------------------
                var selectedCheckboxesFunctional = new List<CheckBox>();
                var getFieldsValueFunctional = ddlService.ddlSkill(3);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SelectListModel, CheckBox>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                });
                aprVM.CheckBoxListFunctional.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValueFunctional);
                aprVM.CheckBoxListFunctional.SelectedFields = selectedCheckboxesFunctional;
                return View(aprVM);
            }


            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult Delete(int skillSetID)
        {
            log.Info($"APARSkillsSetController/Delete/{skillSetID}");
            try
            {
                var flag = false;
                flag = appraisalService.Delete(skillSetID, userDetail.UserID);
                if (flag)
                {
                    TempData["Message"] = "Deleted Successfully.";
                    return RedirectToAction("Index");
                }
                return View("Index");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult UpdateSkillRemarks()
        {
            log.Info($"APARSkillsSetController/UpdateSkillRemarks");
            try
            {
                APARSkillSetViewModel aparVM = new APARSkillSetViewModel();
                aparVM.DepartmentList = ddlService.ddlDepartmentList();
                return PartialView("_SetSkillRemark", aparVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }


        }


        public ActionResult GetSkillRemark(int departmentID)
        {
            log.Info($"APARSkillsSetController/GetSkillRemark");
            try
            {
                APARSkillSetViewModel aparVM = new APARSkillSetViewModel();
                aparVM.APARSkillSetDtlList = appraisalService.GetSkillDetails(departmentID);
                return Json(new { htmlData = ConvertViewToString("_RemarkGridView", aparVM) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UpdateSkillRemarks(APARSkillSetViewModel aparSkill)
        {
            log.Info($"APARSkillsSetController/UpdateSkillRemarks");
            try
            {              
                appraisalService.UpdateSkillRemarks(aparSkill.APARSkillSetDtlList);
                return Json(new { msgType="success",msg="Updated Successfully."}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }


        }

    }
}