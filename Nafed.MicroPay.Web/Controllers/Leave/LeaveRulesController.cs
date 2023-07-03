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
    public class LeaveRulesController : BaseController
    {
        private readonly ILeaveService leaveRuleService;
        private readonly IDropdownBindService ddlService;
        public LeaveRulesController(ILeaveService leaveRuleService, IDropdownBindService ddlService)
        {
            this.leaveRuleService = leaveRuleService;
            this.ddlService = ddlService;
        }

        public ActionResult Index()
        {
            log.Info($"LeaveRulesController/Index");
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
        }

        public ActionResult LeaveRulesGridView(FormCollection formCollection)
        {
            log.Info($"LeaveRulesController/LeaveRulesGridView");
            try
            {
                LeaveRuleViewModel leaveRuleVM = new LeaveRuleViewModel();
                leaveRuleVM.leaveRuleList = leaveRuleService.GetLeaveRuleList();
                leaveRuleVM.userRights = userAccessRight;
                return PartialView("_LeaveRuleGridView", leaveRuleVM);
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
            log.Info("LeaveRulesController/Create");
            try
            {
                BindDropdowns();
                Model.LeaveRule objLeaveRule = new Model.LeaveRule();
                return View(objLeaveRule);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.LeaveRule createLeaveRule)
        {
            log.Info("LeaveRulesController/Create");
            try
            {
                BindDropdowns();
                if (createLeaveRule.C_YearID == 0)
                    ModelState.AddModelError("FYearAlreadyExists", "Select Financial Year");
                if (createLeaveRule.LeaveCategoryID == 0)
                    ModelState.AddModelError("LeaveCategoryAlreadyExists", "Select Leave Category");
                if (ModelState.IsValid)
                {
                    createLeaveRule.C_YearID = createLeaveRule.C_YearID;
                    if (leaveRuleService.LeaveRuleExists(createLeaveRule.LeaveRuleID, createLeaveRule.LeaveCategoryID))
                        ModelState.AddModelError("LeaveCategoryAlreadyExists", "Leave category for this calendar already Exist");
                    else
                    {
                        createLeaveRule.CreatedBy = userDetail.UserID;
                        createLeaveRule.CreatedOn = DateTime.Now;
                        int leaveCategoryID = leaveRuleService.InsertLeaveRuleDetails(createLeaveRule);
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
            return View(createLeaveRule);
        }


        [HttpGet]
        public ActionResult Edit(int leaveRuleID)
        {
            log.Info("LeaveRulesController/Edit");
            try
            {
                BindDropdowns();
                Model.LeaveRule objLeaveRule = new Model.LeaveRule();
                objLeaveRule = leaveRuleService.GetLeaveRuleByID(leaveRuleID);
                return View(objLeaveRule);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Model.LeaveRule updateLeaveRule)
        {
            log.Info("LeaveRulesController/Edit");
            try
            {
                BindDropdowns();
                if (updateLeaveRule.C_YearID == 0)
                    ModelState.AddModelError("FYearAlreadyExists", "Select Financial Year");
                if (updateLeaveRule.LeaveCategoryID == 0)
                    ModelState.AddModelError("LeaveCategoryAlreadyExists", "Select Leave Category");
                if (ModelState.IsValid)
                {
                    updateLeaveRule.C_YearID = updateLeaveRule.C_YearID;
                    if (leaveRuleService.LeaveRuleExists(updateLeaveRule.LeaveRuleID, updateLeaveRule.LeaveCategoryID))
                        ModelState.AddModelError("LeaveCategoryAlreadyExists", "Leave category for this calendar already Exist");
                    else
                    {
                        updateLeaveRule.UpdateBy = userDetail.UserID;
                        leaveRuleService.UpdateLeaveRuleDetails(updateLeaveRule);
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
            return View(updateLeaveRule);

        }

        public ActionResult Delete(int leaveRuleID)
        {
            log.Info("LeaveRulesController/Delete");
            try
            {
                leaveRuleService.DeleteLeaveRule(leaveRuleID);
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