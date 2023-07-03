using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers
{
    public class SkillController : BaseController
    {
        private readonly ISkillService skillService;
        private readonly IDropdownBindService dropDownService;
        public SkillController(ISkillService skillService, IDropdownBindService dropDownService)
        {
            this.skillService = skillService;
            this.dropDownService = dropDownService;
        }
        public ActionResult Index()
        {
            log.Info($"SkillController/Index");
            SkillViewModel skillVM = new SkillViewModel();
            return View(skillVM);
        }

        public ActionResult SkillGridView(SkillViewModel SVM)
        {
            log.Info($"SkillController/SkillGridView");
            try
            {
                SkillViewModel skillVM = new SkillViewModel();
                skillVM.listSkill = skillService.GetSkillList(SVM.isDeleted);
                skillVM.userRights = userAccessRight;
                return PartialView("_SkillGridView", skillVM);
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
            log.Info("SkillController/Create");
            try
            {
                BindDropdown();
                Model.Skill objSkill = new Model.Skill();
                return View(objSkill);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdown()
        {
            var skillTypeList = dropDownService.ddlSkillType();
            Model.SelectListModel selectSkillType = new Model.SelectListModel();
            selectSkillType.id = 0;
            selectSkillType.value = "Select";
            skillTypeList.Insert(0, selectSkillType);
            ViewBag.SkillTypeList = new SelectList(skillTypeList, "id", "value");
        }

        [HttpPost]
        public ActionResult Create(Model.Skill createSkill)
        {
            log.Info("SkillController/Create");
            try
            {
                BindDropdown();
                if (ModelState.IsValid)
                {
                    createSkill.Skill1 = createSkill.Skill1.Trim();
                    if (skillService.SkillExists(createSkill.SkillId, createSkill.Skill1))
                    {
                        ModelState.AddModelError("SkillAlreadyExist", "Skill Already Exist");
                    }
                    else
                    {
                        createSkill.CreatedBy = userDetail.UserID;
                        createSkill.CreatedOn = DateTime.Now;
                        skillService.InsertSkill(createSkill);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createSkill);
        }

        [HttpGet]
        public ActionResult Edit(int skillID)
        {
            log.Info("SkillController/Edit");
            try
            {
                BindDropdown();
                Model.Skill objSkill = new Model.Skill();
                objSkill = skillService.GetSkillByID(skillID);
                return View(objSkill);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Skill editSkill)
        {
            log.Info("SkillController/Edit");
            try
            {
                BindDropdown();
                if (ModelState.IsValid)
                {
                    editSkill.Skill1 = editSkill.Skill1.Trim();
                    editSkill.UpdatedBy = userDetail.UserID;
                    editSkill.UpdatedOn = System.DateTime.Now;
                    if (skillService.SkillExists(editSkill.SkillId, editSkill.Skill1))
                    {
                        ModelState.AddModelError("SkillAlreadyExist", "Skill Already Exist");
                    }
                    else
                    {
                        skillService.UpdateSkill(editSkill);
                        TempData["Message"] = "Succesfully Updated";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editSkill);
        }

        public ActionResult Delete(int skillID)
        {
            log.Info("SkillController/Delete");
            try
            {
                skillService.DeleteSkill(skillID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }
    }
}