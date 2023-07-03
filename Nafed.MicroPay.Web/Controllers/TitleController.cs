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
    public class TitleController : BaseController
    {
        private readonly ITitleService titleService;
        public TitleController(ITitleService titleService)
        {
            this.titleService = titleService;
        }
        // GET: Title
        public ActionResult Index()
        {
            log.Info($"TitleController/Index");
            return View(userAccessRight);
        }
        public ActionResult TitleGridView(FormCollection formCollection)
        {
            log.Info($"TitleController/TitleGridView");
            try
            {
                TitleViewModel titleVM = new TitleViewModel();
                List<Model.Title> objTitle = new List<Model.Title>();
                titleVM.listTitle = titleService.GetTitleList();
                titleVM.userRights = userAccessRight;
                return PartialView("_TitleGridView", titleVM);
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
            log.Info("TitleController/Create");
            try
            {
                Model.Title objTitle = new Model.Title();
                return View(objTitle);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.Title createTitle)
        {
            log.Info("TitleController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createTitle.TitleID = createTitle.TitleID;
                    createTitle.TitleName = createTitle.TitleName.Trim();
                    createTitle.CreatedBy = userDetail.UserID;
                    createTitle.CreatedOn = DateTime.Now;
                    if (titleService.TitleNameExists(createTitle.TitleID, createTitle.TitleName))
                    {
                        ModelState.AddModelError("GradeNameAlreadyExist", "Grade Name Already Exist");
                        return View(createTitle);
                    }
                    //else if (gradeService.(createCategory.CategoryID, createCategory.CategoryCode))
                    //{
                    //    ModelState.AddModelError("CategoryCodeAlreadyExist", "Category Code Already Exist");
                    //    return View(createCategory);
                    //}
                    else
                    {
                        titleService.InsertTitle(createTitle);
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
            return View(createTitle);
        }

        [HttpGet]
        public ActionResult Edit(int titleID)
        {
            log.Info("TitleController/Edit");
            try
            {
                Model.Title objTitle = new Model.Title();
                objTitle = titleService.GetTitleByID(titleID);
                return View(objTitle);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Title editTitle)
        {
            log.Info("TitleController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editTitle.TitleName = editTitle.TitleName.Trim();
                    // editCategory.CategoryCode = editCategory.CategoryCode.Trim();
                    if (titleService.TitleNameExists(editTitle.TitleID, editTitle.TitleName))
                    {
                        ModelState.AddModelError("TitleNameAlreadyExist", "Title Name Already Exist");
                        return View(editTitle);
                    }
                    //else if (categoryService.CategoryCodeExists(editCategory.CategoryID, editCategory.CategoryCode))
                    //{
                    //    ModelState.AddModelError("CategoryCodeAlreadyExist", "Category Code Already Exist");
                    //    return View(editCategory);
                    //}
                    else
                    {
                        titleService.UpdateTitle(editTitle);
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
            return View(editTitle);

        }


        public ActionResult Delete(int titleID)
        {
            log.Info("Delete");
            try
            {
                titleService.Delete(titleID);
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