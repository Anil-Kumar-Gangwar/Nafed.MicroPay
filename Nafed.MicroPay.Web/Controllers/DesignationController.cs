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
    public class DesignationController : BaseController
    {
        private readonly IDesignationService designationService;
        private readonly IDropdownBindService dropdownBindService;
        public DesignationController(IDesignationService designationService, IDropdownBindService dropdownBindService)
        {
            this.designationService = designationService;
            this.dropdownBindService = dropdownBindService;
        }
        // GET: Designation
        public ActionResult Index()
        {
            log.Info($"DesignationController/Index");
            DesignationViewModel designationVM = new DesignationViewModel();
            designationVM.userRights = userAccessRight;
            designationVM.designationList = dropdownBindService.ddlDesignationList();
            designationVM.cadreList = dropdownBindService.ddlCadreList();
            return View(designationVM);
        }

        public ActionResult GetDesignationGridView(DesignationViewModel desigVM)
        {
            log.Info($"DesignationController/GetDesignationGridView");
            try
            {
                DesignationViewModel designationVM = new DesignationViewModel();
                designationVM.listDesignation = designationService.GetDesignationList(desigVM.designationID, desigVM.cadreID);
                designationVM.userRights = userAccessRight;
                return PartialView("_DesignationGridView", designationVM);
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
            log.Info("DesignationController/Create");
            try
            {
                GetCategory();
                GetCadre();
                Model.Designation objDesignation = new Model.Designation();
                return View(objDesignation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult Create(Model.Designation createDesignation)
        {
            log.Info("DesignationController/Create");
            try
            {
                GetCategory();
                GetCadre();

                if (ModelState.IsValid)
                {
                    createDesignation.DesignationName = createDesignation.DesignationName.Trim();
                    createDesignation.DesignationCode = "123";
                    createDesignation.CadreID = createDesignation.CadreID == 0 ? null : createDesignation.CadreID;
                    createDesignation.CateogryID = createDesignation.CateogryID == 0 ? null : createDesignation.CateogryID;
                    createDesignation.CreatedBy = userDetail.UserID;
                    createDesignation.CreatedOn = DateTime.Now;
                    if (designationService.DesignationNameExists(createDesignation.DesignationID, createDesignation.DesignationName))
                    {
                        ModelState.AddModelError("DesignationNameAlreadyExist", "Designation Name Already Exist");
                        return View(createDesignation);
                    }
                    //else if (designationService.DesignationCodeExists(createDesignation.DesignationID, createDesignation.DesignationName))
                    //{
                    //    ModelState.AddModelError("DesignationCodeAlreadyExist", "Designation Code Already Exist");
                    //    return View(createDesignation);
                    //}
                    else
                    {
                        designationService.InsertDesignation(createDesignation);
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
            return View(createDesignation);
        }

        public ActionResult Edit(int designationID)
        {
            log.Info("DesignationController/Edit");
            try
            {
                GetCategory();
                GetCadre();

                Model.Designation objDesignation = new Model.Designation();
                objDesignation.DesignationID = designationID;
                //objDesignation = designationService.GetDesignationByID(designationID);
                return View(objDesignation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Designation editDesignation)
        {
            log.Info("DesignationController/Edit");
            try
            {
                GetCategory();
                GetCadre();
                if (ModelState.IsValid)
                {
                    if (designationService.DesignationNameExists(editDesignation.DesignationID, editDesignation.DesignationName))
                    {
                        editDesignation.DesignationName = editDesignation.DesignationName.Trim();
                        editDesignation.DesignationCode = editDesignation.DesignationCode.Trim();
                        editDesignation = designationService.GetDesignationByID(editDesignation.DesignationID);
                        ModelState.AddModelError("DesignationNameAlreadyExist", "Designation Name Already Exist");
                        return View(editDesignation);
                    }
                    else
                    {
                        editDesignation.CadreID = editDesignation.CadreID == 0 ? null : editDesignation.CadreID;
                        editDesignation.CateogryID = editDesignation.CateogryID == 0 ? null : editDesignation.CateogryID;
                        designationService.UpdateDesignation(editDesignation);

                        //TempData["Message"] = "Succesfully Updated";
                        //return View("Edit", editDesignation);
                        //return RedirectToAction("Index");
                    }
                }
                return PartialView("_EditDesignation", editDesignation);
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
                //TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            //return View(editDesignation);
        }

        public ActionResult Delete(int designationID)
        {
            log.Info($"DesignationController/Delete/{designationID}");
            try
            {
                designationService.Delete(designationID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }


        private void GetCategory() {

            log.Info("DesignationController/GetCategory");

            var category = dropdownBindService.ddlEmployeeCategoryList();
            Model.SelectListModel select = new Model.SelectListModel();
            select.id = 0;
            select.value = "Select";
            category.Insert(0, select);
            ViewBag.Category = new SelectList(category, "id", "value");
        }

        private void GetCadre()
        {
            log.Info("DesignationController/GetCadre"); 
            var cadre = dropdownBindService.ddlCadreList();
            Model.SelectListModel select = new Model.SelectListModel();
            select.id = 0;
            select.value = "Select";
            cadre.Insert(0, select);
            ViewBag.Cadre = new SelectList(cadre, "id", "value");
        }

        public ActionResult DesignationDetailsByID(int designationID)
        {
            log.Info("DesignationController/DesignationDetailsByID");
            try
            {
                GetCategory();
                GetCadre();
                Model.Designation objDesignation = new Model.Designation();
                objDesignation = designationService.GetDesignationByID(designationID);
                return PartialView("_EditDesignation", objDesignation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}