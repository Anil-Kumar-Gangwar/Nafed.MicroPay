using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.IO;


namespace MicroPay.Web.Controllers.Leave
{
    public class LeaveCreditRuleController : BaseController
    {
        private readonly ILeaveService leaveCreditService;
        private readonly IDropdownBindService ddlService;

        public LeaveCreditRuleController(ILeaveService leaveCreditService, IDropdownBindService ddlService)
        {
            this.leaveCreditService = leaveCreditService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            log.Info($"LeaveCreditRuleController/Index");
            return View(userAccessRight);
        }

        public void BindDropdowns()
        {
            var ddlFinanicalYear = ddlService.ddlCalendarYearList();
            Model.SelectListModel selectFinancial = new Model.SelectListModel();
            selectFinancial.id = 0;
            selectFinancial.value = "Select";
            ddlFinanicalYear.Insert(0, selectFinancial);
            ViewBag.FinancialYear = new SelectList(ddlFinanicalYear, "id", "value");

            var ddlLeaveCategory = ddlService.ddlLeaveCategoryList();
            Model.SelectListModel selectCategory = new Model.SelectListModel();
            selectCategory.id = 0;
            selectCategory.value = "Select";
            ddlLeaveCategory.Insert(0, selectCategory);
            ViewBag.LeaveCategory = new SelectList(ddlLeaveCategory, "id", "value");

            List<Model.SelectListModel> selectType = new List<Model.SelectListModel>();
            selectType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            selectType.Add(new Model.SelectListModel() { value = "Jan", id = 1 });
            selectType.Add(new Model.SelectListModel() { value = "Feb", id = 2 });
            selectType.Add(new Model.SelectListModel() { value = "Mar", id = 3 });
            selectType.Add(new Model.SelectListModel() { value = "Apr", id = 4 });
            selectType.Add(new Model.SelectListModel() { value = "May", id = 5 });
            selectType.Add(new Model.SelectListModel() { value = "Jun", id = 6 });
            selectType.Add(new Model.SelectListModel() { value = "Jul", id = 7 });
            selectType.Add(new Model.SelectListModel() { value = "Aug", id = 8 });
            selectType.Add(new Model.SelectListModel() { value = "Sep", id = 9 });
            selectType.Add(new Model.SelectListModel() { value = "Oct", id = 10 });
            selectType.Add(new Model.SelectListModel() { value = "Nov", id = 11 });
            selectType.Add(new Model.SelectListModel() { value = "Dec", id = 12 });
            ViewBag.FMonth = new SelectList(selectType, "id", "value");
            ViewBag.TMonth = new SelectList(selectType, "id", "value");


            var employeetype = ddlService.ddlEmployeeTypeList();
            Model.SelectListModel select = new Model.SelectListModel();
            select.id = 0;
            select.value = "Select";
            employeetype.Insert(0, select);
            ViewBag.employeetype = new SelectList(employeetype, "id", "value");
        }

        public ActionResult LeaveCreditRuleGridView(FormCollection formCollection)
        {
            log.Info($"LeaveCreditRuleController/LeaveCreditRuleGridView");
            try
            {
                LeaveCreditRuleViewModel leaveCreditRuleVM = new LeaveCreditRuleViewModel();
                leaveCreditRuleVM.leaveCreditRuleList = leaveCreditService.GetLeaveCreditList();
                leaveCreditRuleVM.userRights = userAccessRight;
                return PartialView("_LeaveCreditRuleGridView", leaveCreditRuleVM);
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
            log.Info("LeaveCreditRuleController/Create");
            try
            {
                BindDropdowns();
                Model.LeaveCreditRule objLeaveCreditRule = new Model.LeaveCreditRule();
                return View(objLeaveCreditRule);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.LeaveCreditRule leaveCreditRule)
        {
            log.Info("LeaveCreditRuleController/Create");
            try
            {
                BindDropdowns();
                if (leaveCreditRule.C_YearID == 0)
                    ModelState.AddModelError("FYearRequired", "Select calendar year");
                if (leaveCreditRule.LeaveCategoryID == 0)
                    ModelState.AddModelError("LeaveCategoryAlreadyExists", "Select leave category");
                if (leaveCreditRule.FromMonth == 0)
                    ModelState.AddModelError("FromMonthRequired", "Select from month");
                if (leaveCreditRule.ToMonth == 0)
                    ModelState.AddModelError("ToMonthRequired", "Select from month");
                if (ModelState.IsValid)
                {
                    int exist = 0;
                    leaveCreditService.LeaveCreditRuleExists(leaveCreditRule, out exist);
                    if (exist == 1)
                        ModelState.AddModelError("FromMonthRequired", "From month for this calendar year and leave category already Exist");
                    if (exist == 2)
                        ModelState.AddModelError("ToMonthRequired", "To month for this calendar year and leave category already Exist");
                    if (exist == 3)
                    {
                        ModelState.AddModelError("FromMonthRequired", "From month for this calendar year and leave category already Exist");
                        ModelState.AddModelError("ToMonthRequired", "To month for this calendar year and leave category already Exist");
                    }
                    if (exist == 0) 
                    {
                        leaveCreditRule.CreatedBy = userDetail.UserID;
                        leaveCreditRule.CreatedOn = DateTime.Now;
                        int leaveCreditRuleID = leaveCreditService.InsertLeaveCreditRule(leaveCreditRule);
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
            return View(leaveCreditRule);
        }

        [HttpGet]
        public ActionResult Edit(int leaveCreditRuleID)
        {
            log.Info("LeaveCreditRuleController/Edit");
            try
            {
                BindDropdowns();
                Model.LeaveCreditRule objLeaveCreditRule = new Model.LeaveCreditRule();
                objLeaveCreditRule = leaveCreditService.GetLeaveCreditRuleByID(leaveCreditRuleID);
                return View(objLeaveCreditRule);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Model.LeaveCreditRule updateLeaveCreditRule)
        {
            log.Info("LeaveCreditRuleController/Edit");
            try
            {
                BindDropdowns();
                if (updateLeaveCreditRule.C_YearID == 0)
                    ModelState.AddModelError("FYearRequired", "Select calender year");
                if (updateLeaveCreditRule.LeaveCategoryID == 0)
                    ModelState.AddModelError("LeaveCategoryAlreadyExists", "Select leave category");
                if (updateLeaveCreditRule.FromMonth == 0)
                    ModelState.AddModelError("FromMonthRequired", "Select from month");
                if (updateLeaveCreditRule.ToMonth == 0)
                    ModelState.AddModelError("ToMonthRequired", "Select from month");

                if (ModelState.IsValid)
                {
                    int exist = 0;
                    leaveCreditService.LeaveCreditRuleExists(updateLeaveCreditRule, out exist);
                    if (exist == 1)
                        ModelState.AddModelError("FromMonthRequired", "From month for this calendar year and leave category already Exist");
                    if (exist == 2)
                        ModelState.AddModelError("ToMonthRequired", "To month for this calendar year and leave category already Exist");
                    if (exist == 3)
                    {
                        ModelState.AddModelError("FromMonthRequired", "From month for this calendar year and leave category already Exist");
                        ModelState.AddModelError("ToMonthRequired", "To month for this calendar year and leave category already Exist");
                    }
                    if (exist == 0) 
                    {
                        updateLeaveCreditRule.UpdatedBy = userDetail.UserID;
                        leaveCreditService.UpdateLeaveCreditRuleDetails(updateLeaveCreditRule);
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
            return View(updateLeaveCreditRule);

        }

        public ActionResult Delete(int leaveCreditRuleID)
        {
            log.Info("LeaveRulesController/Delete");
            try
            {
                leaveCreditService.DeleteLeaveCreditRule(leaveCreditRuleID);
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