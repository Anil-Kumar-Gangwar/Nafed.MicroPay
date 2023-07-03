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
    public class WashingAllowanceController : BaseController
    {
        // GET: WashingAllowance
        private readonly IWashingAllowanceService washingAllowanceService;
        public WashingAllowanceController(IWashingAllowanceService washingAllowanceService)
        {
            this.washingAllowanceService = washingAllowanceService;
        }          
        public ActionResult Index()
        {
            log.Info($"WashingAllowanceController/Index");
            return View(userAccessRight);
        }

        public ActionResult WashingAllowanceGridView()
        {
            log.Info($"WashingAllowanceController/FDAGridView");
            try
            {
                WashingAllowanceViewModel WashingAllowanceVM = new WashingAllowanceViewModel();
                WashingAllowanceVM.WashingAllowanceList = washingAllowanceService.GetWashingAllowanceList();
                WashingAllowanceVM.userRights = userAccessRight;
                return PartialView("_WashingAllowanceGridView", WashingAllowanceVM);
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
            log.Info("WashingAllowanceController/Create");
            try
            {
                WashingAllowance objWashingAllowance = new WashingAllowance();
                objWashingAllowance.EffectiveDate = DateTime.Now;
                return View(objWashingAllowance);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(WashingAllowance createWashingAllowance)
        {
            log.Info("WashingAllowanceController/Create");
            try
            {

                if (ModelState.IsValid)
                {

                    createWashingAllowance.CreatedBy = userDetail.UserID;
                    createWashingAllowance.CreatedOn = DateTime.Now;
                    createWashingAllowance.IsDeleted = false;
                    if (washingAllowanceService.WashingAllowanceExists((DateTime)createWashingAllowance.EffectiveDate, (decimal)createWashingAllowance.Rate))
                    {
                        ModelState.AddModelError("WashingAllowanceAlreadyExist", "Effective Date Already Exist");
                        return View(createWashingAllowance);
                    }
                    else
                    {
                        washingAllowanceService.InsertWashingAllowance(createWashingAllowance);
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
            return View(createWashingAllowance);
        }

        [HttpGet]
        public ActionResult Edit(decimal rate, DateTime? effectiveDate = null)
        {
            log.Info("WashingAllowanceController/Edit");
            try
            {
                WashingAllowance objWashingAllowance = new WashingAllowance();             
                objWashingAllowance = washingAllowanceService.GetWashingAllowance((DateTime)effectiveDate, rate);
                return View(objWashingAllowance);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(WashingAllowance updateWashingAllowance,FormCollection frm)
        {
            log.Info("WashingAllowanceController/Edit");
            try
            {
                ModelState.Remove("EffectiveDate");
                //var effectiveDate = Request.Form["effectiveDate1"];
                var effectiveDate= frm.Get("effectiveDate1");
                if (ModelState.IsValid)
                {
                    updateWashingAllowance.UpdatedBy = userDetail.UserID;
                    updateWashingAllowance.UpdatedOn = DateTime.Now;
                    updateWashingAllowance.IsDeleted = false;                  
                    updateWashingAllowance.EffectiveDate =Convert.ToDateTime(effectiveDate);  
                    washingAllowanceService.UpdateWashingAllowance(updateWashingAllowance);                   
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateWashingAllowance);

        }

        public ActionResult Delete(DateTime effectiveDate, decimal rate)
        {
            log.Info("WashingAllowanceController/Delete");
            try
            {
                washingAllowanceService.Delete(effectiveDate, rate);
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