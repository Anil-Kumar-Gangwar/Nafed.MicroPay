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
    public class CadreController : BaseController
    {
        private readonly ICadreService cadreService;
        public CadreController(ICadreService cadreService)
        {
            this.cadreService = cadreService;
        }
        public ActionResult Index()
        {
            log.Info($"CadreController/Index");
            return View(userAccessRight);
        }
        public ActionResult CadreGridView(FormCollection formCollection)
        {
            log.Info($"CadreController/CadreGridView");
            try
            {
                CadreViewModel cadreVM = new CadreViewModel(); 
                List<Model.Cadre> objCadreList = new List<Model.Cadre>();
                cadreVM.listCadre = cadreService.GetCadreList();
                cadreVM.userRights = userAccessRight;
                return PartialView("_CadreGridView", cadreVM);
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
            log.Info("CadreController/Create");
            try
            {
                Model.Cadre objCadre = new Model.Cadre();
                return View(objCadre);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.Cadre createCadre)
        {
            log.Info("CadreController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createCadre.CadreName = createCadre.CadreName.Trim();
                    createCadre.CadreCode = createCadre.CadreCode.Trim();
                    createCadre.CreatedBy = userDetail.UserID;
                    createCadre.CreatedOn = System.DateTime.Now;

                    if (cadreService.CadreNameExists(createCadre.CadreID, createCadre.CadreName))
                    {
                        ModelState.AddModelError("CadreNameAlreadyExist", "Cadre Name Already Exist");
                        return View(createCadre);
                    }
                    else if (cadreService.CadreCodeExists(createCadre.CadreID, createCadre.CadreCode))
                    {
                        ModelState.AddModelError("CadreCodeAlreadyExist", "Cadre Code Already Exist");
                        return View(createCadre);
                    }
                    else
                    {
                        cadreService.InsertCadre(createCadre);
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
            return View(createCadre);
        }

        [HttpGet]
        public ActionResult Edit(int cadreID)
        {
            log.Info("CadreItemController/Edit");
            try
            {
                Model.Cadre objCadre = new Model.Cadre();
                objCadre = cadreService.GetCadreByID(cadreID);
                return View(objCadre);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Cadre editCadre)
        {
            log.Info("CadreItemController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editCadre.CadreName = editCadre.CadreName.Trim();
                    editCadre.CadreCode = editCadre.CadreCode.Trim();
                    editCadre.UpdatedBy = userDetail.UserID;
                    editCadre.UpdatedOn = System.DateTime.Now;
                    if (cadreService.CadreNameExists(editCadre.CadreID, editCadre.CadreName))
                    {
                        ModelState.AddModelError("CadreNameAlreadyExist", "Cadre Name Already Exist");
                        return View(editCadre);
                    }
                    else if (cadreService.CadreCodeExists(editCadre.CadreID, editCadre.CadreCode))
                    {
                        ModelState.AddModelError("CadreCodeAlreadyExist", "Cadre Code Already Exist");
                        return View(editCadre);
                    }
                    else
                    {
                        cadreService.UpdateCadre(editCadre);
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
            return View(editCadre);

        }


        public ActionResult Delete(int cadreID)
        {
            log.Info("Delete");
            try
            {
                cadreService.Delete(cadreID);
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