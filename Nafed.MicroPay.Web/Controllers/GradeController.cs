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
    public class GradeController : BaseController
    {
        private readonly IGradeService gradeService;
        public GradeController(IGradeService gradeService)
        {
            this.gradeService = gradeService;
        }
        // GET: Category
        public ActionResult Index()
        {
            log.Info($"GradeController/Index");
            return View(userAccessRight);
        }
        public ActionResult GradeGridView(FormCollection formCollection)
        {
            log.Info($"GradeController/GradeGridView");
            try
            {
                GradeViewModel gradeVM = new GradeViewModel();
                List<Model.Grade> objGrade = new List<Model.Grade>();
                gradeVM.listGrade = gradeService.GetGradeList();
                gradeVM.userRights = userAccessRight;
                return PartialView("_GradeGridView", gradeVM);
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
            log.Info("GradeController/Create");
            try
            {
                Model.Grade objGrade = new Model.Grade();
                return View(objGrade);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.Grade createGrade)
        {
            log.Info("GradeController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createGrade.GradeID = createGrade.GradeID;
                    createGrade.GradeName = createGrade.GradeName.Trim();
                    createGrade.CreatedBy = userDetail.UserID;
                    createGrade.CreatedOn = DateTime.Now;
                    if (gradeService.GradeNameExists(createGrade.GradeID, createGrade.GradeName))
                    {
                        ModelState.AddModelError("GradeNameAlreadyExist", "Grade Name Already Exist");
                        return View(createGrade);
                    }
                    //else if (gradeService.(createCategory.CategoryID, createCategory.CategoryCode))
                    //{
                    //    ModelState.AddModelError("CategoryCodeAlreadyExist", "Category Code Already Exist");
                    //    return View(createCategory);
                    //}
                    else
                    {
                        gradeService.InsertGrade(createGrade);
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
            return View(createGrade);
        }

        [HttpGet]
        public ActionResult Edit(int gradeID)
        {
            log.Info("GradeController/Edit");
            try
            {
                Model.Grade objGrade = new Model.Grade();
                objGrade = gradeService.GetGradeByID(gradeID);
                return View(objGrade);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Grade editGrade)
        {
            log.Info("GradeController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editGrade.GradeName = editGrade.GradeName.Trim();
                   // editCategory.CategoryCode = editCategory.CategoryCode.Trim();
                    if (gradeService.GradeNameExists(editGrade.GradeID, editGrade.GradeName))
                    {
                        ModelState.AddModelError("GradeNameAlreadyExist", "Grade Name Already Exist");
                        return View(editGrade);
                    }
                    //else if (categoryService.CategoryCodeExists(editCategory.CategoryID, editCategory.CategoryCode))
                    //{
                    //    ModelState.AddModelError("CategoryCodeAlreadyExist", "Category Code Already Exist");
                    //    return View(editCategory);
                    //}
                    else
                    {
                        gradeService.UpdateGade(editGrade);
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
            return View(editGrade);

        }


        public ActionResult Delete(int gradeID)
        {
            log.Info("Delete");
            try
            {
                gradeService.Delete(gradeID);
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