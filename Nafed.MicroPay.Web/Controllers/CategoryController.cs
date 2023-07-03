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
    public class CategoryController : BaseController
    {
        private readonly ICategoryService categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }
        // GET: Category
        public ActionResult Index()
        {
            log.Info($"CadreController/Index");
            return View(userAccessRight);
        }
        public ActionResult CategoryGridView(FormCollection formCollection)
        {
            log.Info($"CategoryController/CategoryGridView");
            try
            {
                CategoryViewModel categoryVM = new CategoryViewModel();
                List<Model.Category> objCategory = new List<Model.Category>();
                categoryVM.listCategory = categoryService.GetCategoryList();
                categoryVM.userRights = userAccessRight;
                return PartialView("_CategoryGridView", categoryVM);
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
            log.Info("CategoryController/Create");
            try
            {
                Model.Category objCategory = new Model.Category();
                return View(objCategory);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.Category createCategory)
        {
            log.Info("CategoryController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createCategory.CategoryName = createCategory.CategoryName.Trim();
                    createCategory.CategoryCode = createCategory.CategoryCode.Trim();
                    createCategory.CreatedBy = userDetail.UserID;
                    createCategory.CreatedOn = DateTime.Now;

                    if (categoryService.CategoryNameExists(createCategory.CategoryID, createCategory.CategoryName))
                    {
                        ModelState.AddModelError("CategoryNameAlreadyExist", "Category Name Already Exist");
                        return View(createCategory);
                    }
                    else if (categoryService.CategoryCodeExists(createCategory.CategoryID, createCategory.CategoryCode))
                    {
                        ModelState.AddModelError("CategoryCodeAlreadyExist", "Category Code Already Exist");
                        return View(createCategory);
                    }
                    else
                    {
                        categoryService.InsertCategory(createCategory);
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
            return View(createCategory);
        }

        [HttpGet]
        public ActionResult Edit(int categoryID)
        {
            log.Info("CategoryController/Edit");
            try
            {
                Model.Category objCategory = new Model.Category();
                objCategory = categoryService.GetCategoryByID(categoryID);
                return View(objCategory);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Category editCategory)
        {
            log.Info("CategoryController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editCategory.CategoryName = editCategory.CategoryName.Trim();
                    editCategory.CategoryCode = editCategory.CategoryCode.Trim();
                    if (categoryService.CategoryNameExists(editCategory.CategoryID, editCategory.CategoryName))
                    {
                        ModelState.AddModelError("CategoryNameAlreadyExist", "Category Name Already Exist");
                        return View(editCategory);
                    }
                    else if (categoryService.CategoryCodeExists(editCategory.CategoryID, editCategory.CategoryCode))
                    {
                        ModelState.AddModelError("CategoryCodeAlreadyExist", "Category Code Already Exist");
                        return View(editCategory);
                    }
                    else
                    {
                        categoryService.UpdateCategory(editCategory);
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
            return View(editCategory);

        }


        public ActionResult Delete(int categoryID)
        {
            log.Info("Delete");
            try
            {
                categoryService.Delete(categoryID);
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