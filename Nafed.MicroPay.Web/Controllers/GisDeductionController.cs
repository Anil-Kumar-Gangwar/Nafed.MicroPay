using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace MicroPay.Web.Controllers
{
    public class GisDeductionController : BaseController
    {
        private readonly IGisDeductionService GisDeductionService;
        public GisDeductionController(IGisDeductionService GisDeductionService)
        {
            this.GisDeductionService = GisDeductionService;
        }

        // GET: GisDeduction
        public ActionResult Index()
        {
            log.Info($"GisDeductionController/Index");
            return View(userAccessRight);
        }

        public ActionResult GisDeductionGridView()
        {
            log.Info($"GisDeductionController/GisDeductionGridView");
            try
            {
                GisDeductionViewModel GisDeductionVM = new GisDeductionViewModel();
                GisDeductionVM.GisDeductionList = GisDeductionService.GetGisDeductionList();
                GisDeductionVM.userRights = userAccessRight;
                return PartialView("_GisDeductionGridView", GisDeductionVM);
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
            log.Info("GisDeductionController/Create");
            try
            {
                List<SelectListModel> categorylist = new List<SelectListModel>();
                categorylist.Add(new SelectListModel() { value = "A", id = 1 });
                categorylist.Add(new SelectListModel() { value = "B", id = 2 });
                categorylist.Add(new SelectListModel() { value = "C", id = 3 });

                List<SelectListModel> codelist = new List<SelectListModel>();
                codelist.Add(new SelectListModel() { value = "1", id = 1 });
                codelist.Add(new SelectListModel() { value = "2", id = 2 });
                codelist.Add(new SelectListModel() { value = "3", id = 3 });

                GisDeduction objGisDeduction = new GisDeduction();
                objGisDeduction.CategoryList = categorylist;
                objGisDeduction.CodeList = codelist;             
                return View(objGisDeduction);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(GisDeduction createGisDeduction)
        {
            log.Info("GisDeductionController/Create");
            try
            {

                if (ModelState.IsValid)
                {

                    createGisDeduction.CreatedBy = userDetail.UserID;
                    createGisDeduction.CreatedOn = DateTime.Now;
                    createGisDeduction.IsDeleted = false;
                    if (GisDeductionService.GisDeductionExists(createGisDeduction.Category, createGisDeduction.Code))
                    {
                        List<SelectListModel> categorylist = new List<SelectListModel>();
                        categorylist.Add(new SelectListModel() { value = "A", id = 1 });
                        categorylist.Add(new SelectListModel() { value = "B", id = 2 });
                        categorylist.Add(new SelectListModel() { value = "C", id = 3 });

                        List<SelectListModel> codelist = new List<SelectListModel>();
                        codelist.Add(new SelectListModel() { value = "1", id = 1 });
                        codelist.Add(new SelectListModel() { value = "2", id = 2 });
                        codelist.Add(new SelectListModel() { value = "3", id = 3 });

                        createGisDeduction.CategoryList = categorylist;
                        createGisDeduction.CodeList = codelist;

                        ModelState.AddModelError("GisDeductionAlreadyExist", "GIS Deduction Already Exist");
                        return View(createGisDeduction);
                    }
                    else
                    {
                        GisDeductionService.InsertGisDeduction(createGisDeduction);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                }
                else {
                    List<SelectListModel> categorylist = new List<SelectListModel>();
                    categorylist.Add(new SelectListModel() { value = "A", id = 1 });
                    categorylist.Add(new SelectListModel() { value = "B", id = 2 });
                    categorylist.Add(new SelectListModel() { value = "C", id = 3 });

                    List<SelectListModel> codelist = new List<SelectListModel>();
                    codelist.Add(new SelectListModel() { value = "1", id = 1 });
                    codelist.Add(new SelectListModel() { value = "2", id = 2 });
                    codelist.Add(new SelectListModel() { value = "3", id = 3 });

                    createGisDeduction.CategoryList = categorylist;
                    createGisDeduction.CodeList = codelist;
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createGisDeduction);
        }

        [HttpGet]
        public ActionResult Edit(string category, int code)
        {
            log.Info("GisDeductionController/Edit");
            try
            {
                GisDeduction objGisDeduction = new GisDeduction();
                List<SelectListModel> categorylist = new List<SelectListModel>();
                categorylist.Add(new SelectListModel() { value = "A", id = 1 });
                categorylist.Add(new SelectListModel() { value = "B", id = 2 });
                categorylist.Add(new SelectListModel() { value = "C", id = 3 });

                List<SelectListModel> codelist = new List<SelectListModel>();
                codelist.Add(new SelectListModel() { value = "1", id = 1 });
                codelist.Add(new SelectListModel() { value = "2", id = 2 });
                codelist.Add(new SelectListModel() { value = "3", id = 3 });                                
                objGisDeduction = GisDeductionService.GetGisDeduction(category, code);
                objGisDeduction.CategoryList = categorylist;
                objGisDeduction.CodeList = codelist;
                return View(objGisDeduction);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(GisDeduction updateGisDeduction)
        {
            log.Info("GisDeductionController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    updateGisDeduction.UpdatedBy = userDetail.UserID;
                    updateGisDeduction.UpdatedOn = DateTime.Now;
                    updateGisDeduction.IsDeleted = false;
                    GisDeductionService.UpdateGisDeduction(updateGisDeduction);
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    List<SelectListModel> categorylist = new List<SelectListModel>();
                    categorylist.Add(new SelectListModel() { value = "A", id = 1 });
                    categorylist.Add(new SelectListModel() { value = "B", id = 2 });
                    categorylist.Add(new SelectListModel() { value = "C", id = 3 });

                    List<SelectListModel> codelist = new List<SelectListModel>();
                    codelist.Add(new SelectListModel() { value = "1", id = 1 });
                    codelist.Add(new SelectListModel() { value = "2", id = 2 });
                    codelist.Add(new SelectListModel() { value = "3", id = 3 });

                    updateGisDeduction.CategoryList = categorylist;
                    updateGisDeduction.CodeList = codelist;
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateGisDeduction);

        }

        public ActionResult Delete(string category, int code)
        {
            log.Info("GisDeductionController/Delete");
            try
            {
                GisDeductionService.Delete(category, code);
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