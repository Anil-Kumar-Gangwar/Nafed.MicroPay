using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers
{
    public class SectionController : BaseController
    {

        private readonly ISectionService sectionService;
        public SectionController(ISectionService sectionService)
        {
            this.sectionService = sectionService;
        }
        public ActionResult Index()
        {
            log.Info($"SectionController/Index");
            return View(userAccessRight);
        }

        public ActionResult SectionGridView(FormCollection formCollection)
        {
            log.Info($"SectionController/SectionGridView");
            try
            {
                SectionViewModel SecVM = new SectionViewModel();
                List<Model.Section> objSectionList = new List<Model.Section>();
                SecVM.listSection = sectionService.GetSectionList();
                SecVM.userRights = userAccessRight;
                return PartialView("_SectionGridView", SecVM);
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
            log.Info("SectionController/Create");
            try
            {
                Model.Section objSection = new Model.Section();
                return View(objSection);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult Create(Model.Section createSection)
        {
            log.Info("SectionController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createSection.SectionName = createSection.SectionName.Trim();
                    createSection.SectionCode = createSection.SectionCode.Trim();

                    createSection.CreatedBy = userDetail.UserID;
                    createSection.CreatedOn = System.DateTime.Now;

                    if (sectionService.SectionNameExists(createSection.SectionID, createSection.SectionName))
                    {
                        ModelState.AddModelError("SectionNameAlreadyExist", "Section Name Already Exist");
                        return View(createSection);
                    }
                    else if (sectionService.SectionCodeExists(createSection.SectionID, createSection.SectionCode))
                    {
                        ModelState.AddModelError("SectionCodeAlreadyExist", "Section Code Already Exist");
                        return View(createSection);
                    }
                    else
                    {
                        sectionService.InsertSection(createSection);
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
            return View(createSection);
        }

        [HttpGet]
        public ActionResult Edit(int sectionID)
        {
            log.Info("SectionItemController/Edit");
            try
            {
                Model.Section objSection = new Model.Section();
                objSection = sectionService.GetSectionByID(sectionID);
                return View(objSection);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Section editSection)
        {
            log.Info("SectionItemController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editSection.SectionName = editSection.SectionName.Trim();
                    editSection.SectionCode = editSection.SectionCode.Trim();

                    editSection.UpdatedBy = userDetail.UserID;
                    editSection.UpdatedOn = System.DateTime.Now;

                    if (sectionService.SectionNameExists(editSection.SectionID, editSection.SectionName))
                    {
                        ModelState.AddModelError("SectionNameAlreadyExist", "Section Name Already Exist");
                        return View(editSection);
                    }
                    else if (sectionService.SectionCodeExists(editSection.SectionID, editSection.SectionCode))
                    {
                        ModelState.AddModelError("SectionCodeAlreadyExist", "Section Code Already Exist");
                        return View(editSection);
                    }
                    else
                    {
                        sectionService.UpdateSection(editSection);
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
            return View(editSection);
        }

        public ActionResult Delete(int sectionID)
        {
            log.Info("Delete");
            try
            {
                sectionService.Delete(sectionID);
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