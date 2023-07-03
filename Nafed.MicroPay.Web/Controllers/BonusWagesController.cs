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
    public class BonusWagesController : BaseController
    {
        private readonly IBonusWagesService bonuswagesService;

        public BonusWagesController(IBonusWagesService bonuswagesService)
        {
            this.bonuswagesService = bonuswagesService;
        }
        public ActionResult Index()
        {
            log.Info($"BonusWagesController/Index");
            return View(userAccessRight);
        }

        public ActionResult BonusWagesGridView(FormCollection formCollection)
        {
            log.Info($"BonusWagesController/BonusWagesGridView");
            try
            {
                BonusWagesViewModel bonuswagesVM = new BonusWagesViewModel();
                List<Model.BonusWages> objBWList = new List<Model.BonusWages>();
                bonuswagesVM.listBonusWages = bonuswagesService.GetBonusWagesList();
                bonuswagesVM.userRights = userAccessRight;
                return PartialView("_BonusWagesGridView", bonuswagesVM);
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
            log.Info("BonusWagesController/Create");
            try
            {
                Model.BonusWages objBonusWages = new Model.BonusWages();
                return View(objBonusWages);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult Create(Model.BonusWages createBonusWages)
        {
            log.Info("BonusWagesController/Create");
            try
            {
                
                if (createBonusWages.From_date == null)
                {
                    ModelState.AddModelError("From_date", "Please select from period.");
                }
                if (createBonusWages.To_date == null)
                {
                    ModelState.AddModelError("To_date", "Please select to period.");
                }
                if (createBonusWages.Restricted_Salary_as_Per_bonus == 0)
                {
                    ModelState.AddModelError("Restricted_Salary_as_Per_bonus", "Please select restricted salary.");
                }
                
                if (ModelState.IsValid)
                {
                    createBonusWages.From_date = createBonusWages.From_date;
                    createBonusWages.To_date = createBonusWages.To_date;
                    createBonusWages.Restricted_Salary_as_Per_bonus = createBonusWages.Restricted_Salary_as_Per_bonus;
                    createBonusWages.CreatedBy = userDetail.UserID;
                    createBonusWages.CreatedOn = System.DateTime.Now;
                    if (bonuswagesService.BonusWagesExists(createBonusWages.From_date, createBonusWages.To_date))
                    {
                        TempData["Error"] = "Data already exist for this period";
                        return View(createBonusWages);
                    }
                    else
                    {
                        bonuswagesService.InsertBonusWages(createBonusWages);
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
            return View(createBonusWages);
        }


        [HttpGet]
        public ActionResult Edit(int ID)
        {
            log.Info("BonusWagesController/Edit");
            try
            {
                Model.BonusWages objBW = new Model.BonusWages();
                objBW = bonuswagesService.GetBonusWagesByID(ID);
                return View(objBW);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.BonusWages editBW)
        {
            log.Info("BonusWagesController/Edit");
            try
            {
                ModelState.Remove("FinancialYear");
                ModelState.Remove("selectYearID");
                if (ModelState.IsValid)
                {
                    editBW.Restricted_Salary_as_Per_bonus = editBW.Restricted_Salary_as_Per_bonus;
                    editBW.UpdatedBy = userDetail.UserID;
                    editBW.UpdatedOn = System.DateTime.Now;
                   
                    bonuswagesService.UpdateBonusWages(editBW);
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editBW);

        }
    }
}