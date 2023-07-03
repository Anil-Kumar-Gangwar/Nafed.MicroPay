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
    public class CEARatesController : BaseController
    {
        private readonly ICEARatesService ceaRatesService;
        public CEARatesController(ICEARatesService ceaRatesService)
        {
            this.ceaRatesService = ceaRatesService;
        }
        // GET: CEARates
        
        public ActionResult Index()
        {
            log.Info($"CEARatesController/Index");
            return View(userAccessRight);
        }

        public ActionResult CEARatesGridView()
        {
            log.Info($"CEARatesController/FDAGridView");
            try
            {
                CEARatesViewModel WCEARatesVM = new CEARatesViewModel();
                WCEARatesVM.CEARatesList = ceaRatesService.GetCEARatesList();
                WCEARatesVM.userRights = userAccessRight;
                return PartialView("_CEARatesGridView", WCEARatesVM);
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
            log.Info("CEARatesController/Create");
            try
            {
                CEARates objCEARates = new CEARates();
                objCEARates.EffectiveDate = DateTime.Now;
                return View(objCEARates);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(CEARates createCEARates)
        {
            log.Info("CEARatesController/Create");
            try
            {

                if (ModelState.IsValid)
                {

                    createCEARates.CreatedBy = userDetail.UserID;
                    createCEARates.CreatedOn = DateTime.Now;
                    createCEARates.IsDeleted = false;
                    if (ceaRatesService.CEARatesExists(createCEARates.EffectiveDate))
                    {
                        ModelState.AddModelError("CEARatesAlreadyExist", "Effective Date Already Exist");
                        return View(createCEARates);
                    }
                    else
                    {
                        ceaRatesService.InsertCEARates(createCEARates);
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
            return View(createCEARates);
        }

        [HttpGet]
        public ActionResult Edit(DateTime effectiveDate)
        {
            log.Info("CEARatesController/Edit");
            try
            {
                CEARates objCEARates = new CEARates();
              //  objCEARates.EffectiveDate = DateTime.Now;
                objCEARates = ceaRatesService.GetCEARates(effectiveDate);
                return View(objCEARates);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(CEARates updateCEARates)
        {
            log.Info("CEARatesController/Edit");
            try
            {

                var rr =Convert.ToDateTime(Request.Form["effDate"]);
                if (ModelState.IsValid)
                {
                    updateCEARates.UpdatedBy = userDetail.UserID;
                    updateCEARates.UpdatedOn = DateTime.Now;
                    updateCEARates.IsDeleted = false;
                    updateCEARates.EffectiveDate = rr;
                    ceaRatesService.UpdateCEARates(updateCEARates);
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateCEARates);

        }

        public ActionResult Delete(DateTime effectiveDate)
        {
            log.Info("CEARatesController/Delete");
            try
            {
                ceaRatesService.Delete(effectiveDate);
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