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
    public class LeavePurposeController : BaseController
    {
        private readonly ILeaveService leaveRuleService;
        private readonly IDropdownBindService ddlService;

        public LeavePurposeController(ILeaveService leaveRuleService, IDropdownBindService ddlService)
        {
            this.leaveRuleService = leaveRuleService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            log.Info($"LeavePurposeController/Index");
            return View(userAccessRight);
        }

        public void BindDropdowns()
        {
            var ddlLeaveCategory = ddlService.ddlLeaveCategoryList();
            Model.SelectListModel selectCategory = new Model.SelectListModel();
            selectCategory.id = 0;
            selectCategory.value = "Select";
            ddlLeaveCategory.Insert(0, selectCategory);
            ViewBag.LeaveCategory = new SelectList(ddlLeaveCategory, "id", "value");
        }


        public ActionResult LeavePurposeGridView(FormCollection formCollection)
        {
            log.Info($"LeavePurposeController/LeavePurposeGridView");
            try
            {
                LeavePurposeViewModel leavePurposeVM = new LeavePurposeViewModel();
                leavePurposeVM.leavePurposeList = leaveRuleService.GetleavePurposeList();
                leavePurposeVM.userRights = userAccessRight;
                return PartialView("_LeavePurposeGridView", leavePurposeVM);
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
            log.Info("LeavePurposeController/Create");
            try
            {
                BindDropdowns();
                Model.LeavePurpose objLeavePurpose = new Model.LeavePurpose();
                return View(objLeavePurpose);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.LeavePurpose createLeavePurpose)
        {
            log.Info("LeavePurposeController/Create");
            try
            {
                BindDropdowns();
                if (createLeavePurpose.LeaveCategoryID == 0)
                    ModelState.AddModelError("LeaveCategoryAlreadyExists", "Select Leave Category");
                if (ModelState.IsValid)
                {
                    createLeavePurpose.LeavePurposeName = createLeavePurpose.LeavePurposeName.Trim();
                    if (leaveRuleService.LeavePurposeExists(createLeavePurpose.LeavePurposeID, createLeavePurpose.LeavePurposeName))
                        ModelState.AddModelError("LeavePurposeAlreadyExists", "Leave Purpose Already Exist");
                  
                    else
                    {
                        createLeavePurpose.CreatedBy = userDetail.UserID;
                        createLeavePurpose.CreatedOn = DateTime.Now;
                        int leavePurposeID = leaveRuleService.InsertLeavePurposeDetails(createLeavePurpose);
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
            return View(createLeavePurpose);
        }

        [HttpGet]
        public ActionResult Edit(int leavePurposeID)
        {
            log.Info("LeavePurposeController/Edit");
            try
            {
                BindDropdowns();
                Model.LeavePurpose objLeavePurpose = new Model.LeavePurpose();
                objLeavePurpose = leaveRuleService.GetLeavePurposeByID(leavePurposeID);
                return View(objLeavePurpose);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Model.LeavePurpose updateLeavePurpose)
        {
            log.Info("LeavePurposeController/Edit");
            try
            {
                BindDropdowns();
                if (updateLeavePurpose.LeaveCategoryID == 0)
                    ModelState.AddModelError("LeaveCategoryAlreadyExists", "Select Leave Category");
                if (ModelState.IsValid)
                {
                    updateLeavePurpose.LeavePurposeName = updateLeavePurpose.LeavePurposeName.Trim();
                    if (leaveRuleService.LeavePurposeExists(updateLeavePurpose.LeavePurposeID, updateLeavePurpose.LeavePurposeName))
                        ModelState.AddModelError("LeavePurposeAlreadyExists", "Leave Purpose Already Exist");
                  
                    else
                    {
                        updateLeavePurpose.UpdatedBy = userDetail.UserID;
                        leaveRuleService.UpdateLeavePurposeDetails(updateLeavePurpose);
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
            return View(updateLeavePurpose);

        }


        public ActionResult Delete(int leavePurposeID)
        {
            log.Info("LeaveTypeController/Delete");
            try
            {
                leaveRuleService.DeleteLeavePurpose(leavePurposeID);
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