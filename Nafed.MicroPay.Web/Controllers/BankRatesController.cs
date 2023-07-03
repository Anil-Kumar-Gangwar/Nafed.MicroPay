using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers
{
    public class BankRatesController : BaseController
    {
        private readonly IBankRatesService bankRateService;
        public BankRatesController(IBankRatesService bankRateService)
        {
            this.bankRateService = bankRateService;
        }
        // GET: BankRates
        public ActionResult Index()
        {
            log.Info($"BankRatesController/Index");
            return View(userAccessRight);
        }
        public ActionResult BankRatesGridView()
        {
            log.Info($"BankRatesController/BankRatesGridView");
            try
            {
                BankRatesViewModel BankRateVM = new BankRatesViewModel();
                List<BankRates> objBank = new List<BankRates>();
                BankRateVM.BankRateList = bankRateService.GetBankRatesList();
                BankRateVM.userRights = userAccessRight;
                return PartialView("_BankRatesGridView", BankRateVM);
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
            log.Info("BankRatesController/Create");
            try
            {
                BankRates objBankRates = new BankRates();
                objBankRates.EffectiveDate = DateTime.Now;
                return View(objBankRates);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(BankRates createBankRates)
        {
            log.Info("BankRatesController/Create");
            try
            {

                if (ModelState.IsValid)
                {
                    createBankRates.EffectiveDate = createBankRates.EffectiveDate;                   
                   createBankRates.CreatedBy = userDetail.UserID;
                    createBankRates.CreatedOn = DateTime.Now;
                   createBankRates.IsDeleted = false;
                    if (bankRateService.BankDateExist(createBankRates.EffectiveDate))
                    {
                        ModelState.AddModelError("BankDateAlreadyExist", "Effective Date Already Exist");
                        return View(createBankRates);
                    }                    
                    else
                    {
                        bankRateService.InsertBankRates(createBankRates);
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
            return View(createBankRates);
        }

        [HttpGet]
        public ActionResult Edit(DateTime bankDate)
        {
            log.Info("BankRatesController/Edit");
            try
            {
                BankRates objBank = new BankRates();
                objBank = bankRateService.GetBankRatesbyDate(bankDate);
                return View(objBank);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(BankRates editBankRates)
        {
            log.Info("BankRatesController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editBankRates.EffectiveDate = editBankRates.EffectiveDate;
                    editBankRates.UpdatedBy = userDetail.UserID;
                    editBankRates.UpdatedOn = DateTime.Now;
                    editBankRates.IsDeleted = false;                   
                    bankRateService.UpdateBankRates(editBankRates);
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");
                    //}
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editBankRates);

        }

        public ActionResult Delete(DateTime bankDate)
        {
            log.Info("BankRatesController/Delete");
            try
            {
                bankRateService.Delete(bankDate);
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