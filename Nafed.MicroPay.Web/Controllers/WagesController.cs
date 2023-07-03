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
    public class WagesController : BaseController
    {
        private readonly IWagesService wagesService;
        public WagesController(IWagesService wagesService)
        {
            this.wagesService = wagesService;
        }
        // GET: Wages      
        public ActionResult Index()
        {
            log.Info($"WagesController/Index");
            return View(userAccessRight);
        }

        public ActionResult WagesGridView()
        {
            log.Info($"WagesController/FDAGridView");
            try
            {
                WagesViewModel WagesVM = new WagesViewModel();
                WagesVM.WagesList = wagesService.GetWagesList();
                WagesVM.userRights = userAccessRight;
                return PartialView("_WagesGridView", WagesVM);
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
            log.Info("WagesController/Create");
            try
            {
                Wages objWages = new Wages();
                objWages.RDate = DateTime.Now;
                return View(objWages);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Wages createWages)
        {
            log.Info("WagesController/Create");
            try
            {

                if (ModelState.IsValid)
                {

                    createWages.CreatedBy = userDetail.UserID;
                    createWages.CreatedOn = DateTime.Now;
                    createWages.IsDeleted = false;
                    if (wagesService.WagesExists(createWages.RDate, (decimal)createWages.RatePerHour))
                    {
                        ModelState.AddModelError("WagesAlreadyExist", "Effective Date Already Exist");
                        return View(createWages);
                    }
                    else
                    {
                        wagesService.InsertWages(createWages);
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
            return View(createWages);
        }

        [HttpGet]
        public ActionResult Edit(DateTime effectiveDate, decimal ratePerHour)
        {
            log.Info("WagesController/Edit");
            try
            {
                Wages objWages = new Wages();
                objWages.RDate = DateTime.Now;
                objWages = wagesService.GetWages(effectiveDate, ratePerHour);
                return View(objWages);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Wages updateWages)
        {
            log.Info("WagesController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    updateWages.UpdatedBy = userDetail.UserID;
                    updateWages.UpdatedOn = DateTime.Now;
                    updateWages.IsDeleted = false;
                    wagesService.UpdateWages(updateWages);
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateWages);

        }

        public ActionResult Delete(DateTime effectiveDate, decimal ratePerHour)
        {
            log.Info("WagesController/Delete");
            try
            {
                wagesService.Delete(effectiveDate, ratePerHour);
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