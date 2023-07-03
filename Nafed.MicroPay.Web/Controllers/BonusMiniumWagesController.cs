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
   
    public class BonusMiniumWagesController : BaseController
    {
        private readonly IBonusWagesService bonuswagesService;
        private readonly IDropdownBindService ddlService;

        public BonusMiniumWagesController(IBonusWagesService bonuswagesService, IDropdownBindService ddlService)
        {
            this.bonuswagesService = bonuswagesService;
            this.ddlService = ddlService; 
        }

        public ActionResult Index()
        {
            log.Info($"BonusMiniumWagesController/Index");
            return View(userAccessRight);
        }

        public ActionResult BonusMinimumWagesGridView(FormCollection formCollection)
        {
            log.Info($"BonusMiniumWagesController/BonusMinimumWagesGridView");
            try
            {
                BonusWagesViewModel bonusminwagesVM = new BonusWagesViewModel();
                List<Model.BonusWages> objBWList = new List<Model.BonusWages>();
                bonusminwagesVM.listBonusWages = bonuswagesService.GetBonusMiimumWagesList();
                bonusminwagesVM.userRights = userAccessRight;
                return PartialView("_BonusMinumumWagesGridView", bonusminwagesVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            var ddldesignationList = ddlService.ddlDesignationList();
            Model.SelectListModel selectDesignation = new Model.SelectListModel();
            selectDesignation.id = 0;
            selectDesignation.value = "Select";
            ddldesignationList.Insert(0, selectDesignation);
            ViewBag.Designation = new SelectList(ddldesignationList, "id", "value");

        }

        [HttpGet]
        public ActionResult Edit(int ID)
        {
            log.Info("BonusMiniumWagesController/Edit");
            try
            {
                BindDropdowns();
                Model.BonusWages objBW = new Model.BonusWages();
                objBW = bonuswagesService.GetBonusminimumWagesByID(ID);
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
                ModelState.Remove("From_date");
                ModelState.Remove("To_date");
                ModelState.Remove("selectYearID");
                if (ModelState.IsValid)
                {
                    editBW.designationID = editBW.designationID;
                    editBW.Minimum_monthly_wages = editBW.Minimum_monthly_wages;
                    editBW.UpdatedBy = userDetail.UserID;
                    editBW.UpdatedOn = System.DateTime.Now;

                    bonuswagesService.UpdateBonusMinimumWages(editBW);
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