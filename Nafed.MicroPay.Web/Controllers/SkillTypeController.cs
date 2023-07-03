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
    public class SkillTypeController : BaseController
    {
        private readonly ISkillService skillService;
        public SkillTypeController(ISkillService skillService)
        {
            this.skillService = skillService;
        }
        public ActionResult Index()
        {
            log.Info($"SkillTypeController/Index");
            SkillTypeViewModel skillTypeVM = new SkillTypeViewModel();
            return View(skillTypeVM);
        }

        public ActionResult SkillTypeGridView(SkillTypeViewModel STVM)
        {
            log.Info($"SkillTypeController/SkillTypeGridView");
            try
            {
                SkillTypeViewModel skillTypeVM = new SkillTypeViewModel();
                skillTypeVM.listSkillType = skillService.GetSkillTypeList(STVM.isDeleted);
                skillTypeVM.userRights = userAccessRight;
                return PartialView("_SkillTypeGridView", skillTypeVM);
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
            log.Info("SkillTypeController/Create");
            try
            {
                Model.SkillType objSkillType = new Model.SkillType();
                return View(objSkillType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.SkillType createSkillType)
        {
            log.Info("SkillTypeController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createSkillType.SkillType1 = createSkillType.SkillType1.Trim();
                    if (skillService.SkillTypeExists(createSkillType.SkillTypeID, createSkillType.SkillType1)) 
                    {
                        ModelState.AddModelError("SkillTypeAlreadyExist", "Skill Type Already Exist");
                    }
                    else
                    {
                        createSkillType.CreatedBy = userDetail.UserID;
                        createSkillType.CreatedOn = DateTime.Now;
                        skillService.InsertSkillType(createSkillType);
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
            return View(createSkillType);
        }

        [HttpGet]
        public ActionResult Edit(int skillTypeID)
        {
            log.Info("SkillTypeController/Edit");
            try
            {
                Model.SkillType objSkillType = new Model.SkillType();
                objSkillType = skillService.GetSkillTypeID(skillTypeID);
                return View(objSkillType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.SkillType editSkillType)
        {
            log.Info("SkillTypeController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editSkillType.SkillType1 = editSkillType.SkillType1.Trim();
                    editSkillType.UpdatedBy = userDetail.UserID;
                    editSkillType.UpdatedOn = System.DateTime.Now;
                    if (skillService.SkillTypeExists(editSkillType.SkillTypeID, editSkillType.SkillType1))
                    {
                        ModelState.AddModelError("SkillTypeAlreadyExist", "Skill Type Already Exist");
                    }
                    else
                    {
                        skillService.UpdateSkillType(editSkillType);
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
            return View(editSkillType);

        }

        public ActionResult Delete(int skillTypeID)
        {
            log.Info("SkillTypeController/Delete");
            try
            {
                skillService.Delete(skillTypeID);
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