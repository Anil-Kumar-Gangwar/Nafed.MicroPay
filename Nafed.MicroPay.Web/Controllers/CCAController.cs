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
    public class CCAController : BaseController
    {
        private readonly ICCAService ccaService;
        public CCAController(ICCAService ccaService)
        {
            this.ccaService = ccaService;
        }
        // GET: CCA

        public ActionResult Index()
        {
            log.Info($"CCAController/Index");
            return View(userAccessRight);
        }

        public ActionResult CCAGridView()
        {
            log.Info($"CCAController/FDAGridView");
            try
            {
                CCAViewModel CCAVM = new CCAViewModel();
                CCAVM.CCAList = ccaService.GetCCAList();
                CCAVM.userRights = userAccessRight;
                return PartialView("_CCAGridView", CCAVM);
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
            log.Info("CCAController/Create");
            try
            {
                CCA objCCA = new CCA();               
                return View(objCCA);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(CCA createCCA)
        {
            log.Info("CCAController/Create");
            try
            {

                if (ModelState.IsValid)
                {

                    createCCA.CreatedBy = userDetail.UserID;
                    createCCA.CreatedOn = DateTime.Now;
                    createCCA.IsDeleted = false;
                    if (ccaService.CCAExists(createCCA.UpperLimit))
                    {
                        ModelState.AddModelError("CCAAlreadyExist", "Effective Date Already Exist");
                        return View(createCCA);
                    }
                    else
                    {
                        ccaService.InsertCCA(createCCA);
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
            return View(createCCA);
        }

        [HttpGet]
        public ActionResult Edit(decimal upperLimit)
        {
            log.Info("CCAController/Edit");
            try
            {
                CCA objCCA = new CCA();
                //  objCCA.EffectiveDate = DateTime.Now;
                objCCA = ccaService.GetCCA(upperLimit);
                return View(objCCA);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(CCA updateCCA)
        {
            log.Info("CCAController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    updateCCA.UpdatedBy = userDetail.UserID;
                    updateCCA.UpdatedOn = DateTime.Now;
                    updateCCA.IsDeleted = false;
                    ccaService.UpdateCCA(updateCCA);
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateCCA);

        }

        public ActionResult Delete(decimal upperLimit)
        {
            log.Info("CCAController/Delete");
            try
            {
                ccaService.Delete(upperLimit);
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