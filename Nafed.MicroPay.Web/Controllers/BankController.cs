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
    public class BankController : BaseController
    {
        private readonly IBankService bankService;
        public BankController(IBankService bankService)
        {
            this.bankService = bankService;
        }
        // GET: Bank
        public ActionResult Index()
        {
            log.Info($"BankController/Index");
            return View(userAccessRight);
        }
        public ActionResult BankGridView()
        {
            log.Info($"BankController/BankGridView");
            try
            {
                BankViewModel BankVM = new BankViewModel();
                List<Bank> objBank = new List<Bank>();
                BankVM.BankList = bankService.GetBankList();
                BankVM.userRights = userAccessRight;
                return PartialView("_BankGridView", BankVM);
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
            log.Info("BankController/Create");
            try
            {
                Bank objBank = new Bank();
                return View(objBank);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Bank createBank)
        {
            log.Info("BankController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createBank.BankCode = createBank.BankCode.Trim();
                    // createBank.BankCode = createBank.BankCode.Trim();
                    createBank.CreatedBy = userDetail.UserID;
                    createBank.CreatedOn = DateTime.Now;
                    createBank.IsDeleted = false;
                    if (bankService.BankNameExists(createBank.BankName))
                    {
                        ModelState.AddModelError("BankNameAlreadyExist", "Bank Name Already Exist");
                        return View(createBank);
                    }
                    else if (bankService.BankCodeExists(createBank.BankCode))
                    {
                        ModelState.AddModelError("BankCodeAlreadyExist", "Bank Code Already Exist");
                        return View(createBank);
                    }
                    else
                    {
                        bankService.InsertBank(createBank);
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
            return View(createBank);
        }

        [HttpGet]
        public ActionResult Edit(string bankCode)
        {
            log.Info("BankController/Edit");
            try
            {
                Bank objBank = new Bank();
                objBank = bankService.GetBankbyCode(bankCode);
                return View(objBank);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Bank editBank)
        {
            log.Info("BankController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editBank.BankCode = editBank.BankCode.Trim();
                    editBank.UpdatedBy = userDetail.UserID;
                    editBank.UpdatedOn = DateTime.Now;
                    editBank.IsDeleted = false;
                    //if (bankService.BankNameExists(editBank.BankName))
                    //{
                    //    ModelState.AddModelError("BankNameAlreadyExist", "Bank Name Already Exist");
                    //    return View(editBank);
                    //}
                    //else if (bankService.BankCodeExists(editBank.BankCode))
                    //{
                    //    ModelState.AddModelError("BankCodeAlreadyExist", "Bank Code Already Exist");
                    //    return View(editBank);
                    //}
                    //else
                    //{
                        bankService.UpdateBank(editBank);
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
            return View(editBank);

        }

        public ActionResult Delete(string bankCode)
        {
            log.Info("BankController/Delete");
            try
            {
                bankService.Delete(bankCode);
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