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
    public class LeaveTypeController : BaseController
    {
        private readonly ILeaveService leaveRuleService;

        public LeaveTypeController(ILeaveService leaveRuleService)
        {
            this.leaveRuleService = leaveRuleService;
        }
    
        public ActionResult Index()
        {
            log.Info($"LeaveTypeController/Index");
            return View(userAccessRight);
        }

        public ActionResult LeaveTypeGridView(FormCollection formCollection)
        {
            log.Info($"LeaveTypeController/LeaveTypeGridView");
            try
            {
                LeaveTypeViewModel leaveTypeVM = new LeaveTypeViewModel();
                leaveTypeVM.leaveTypeList = leaveRuleService.GetLeaveTypeList();
                leaveTypeVM.userRights = userAccessRight;
                return PartialView("_LeaveTypeGridView", leaveTypeVM);
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
            log.Info("LeaveTypeController/Create");
            try
            {
                Model.LeaveType objLeaveType = new Model.LeaveType();
                return View(objLeaveType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.LeaveType createLeaveType)
        {
            log.Info("LeaveTypeController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    int exist = 0;

                    createLeaveType.LeaveCode = createLeaveType.LeaveCode.Trim();
                    createLeaveType.LeaveDesc = createLeaveType.LeaveDesc.Trim();
                     leaveRuleService.LeaveTypeExists(createLeaveType, out exist);
                    if (exist == 1)  
                        ModelState.AddModelError("LeaveCodeAlreadyExists", "Leave Code Already Exist");
                    if (exist == 2)
                        ModelState.AddModelError("LeaveDescAlreadyExists", "Leave Desc Already Exist");
                    if(exist == 3)
                    {
                        ModelState.AddModelError("LeaveCodeAlreadyExists", "Leave Code Already Exist");
                        ModelState.AddModelError("LeaveDescAlreadyExists", "Leave Desc Already Exist");
                    }
                    if(exist == 0)
                    {
                        createLeaveType.CreatedBy = userDetail.UserID;
                        createLeaveType.CreatedOn = DateTime.Now;
                        int leaveTypeID = leaveRuleService.InsertLeaveTypeDetails(createLeaveType);
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
            return View(createLeaveType);
        }

        [HttpGet]
        public ActionResult Edit(int leaveTypeID)
        {
            log.Info("LeaveTypeController/Edit");
            try
            {
                Model.LeaveType objLeaveRule = new Model.LeaveType();
                objLeaveRule = leaveRuleService.GetLeaveTypeByID(leaveTypeID);
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
        public ActionResult Edit(Model.LeaveType updateLeaveType)
        {
            log.Info("LeaveTypeController/Edit");
            try
            {
                
                if (ModelState.IsValid)
                {
                    int exist = 0;

                    updateLeaveType.LeaveCode = updateLeaveType.LeaveCode.Trim();
                    updateLeaveType.LeaveDesc = updateLeaveType.LeaveDesc.Trim();
                    leaveRuleService.LeaveTypeExists(updateLeaveType, out exist);
                    if (exist == 1)
                        ModelState.AddModelError("LeaveCodeAlreadyExists", "Leave Code Already Exist");
                    if (exist == 2)
                        ModelState.AddModelError("LeaveDescAlreadyExists", "Leave Desc Already Exist");
                    if (exist == 3)
                    {
                        ModelState.AddModelError("LeaveCodeAlreadyExists", "Leave Code Already Exist");
                        ModelState.AddModelError("LeaveDescAlreadyExists", "Leave Desc Already Exist");
                    }
                    if (exist == 0) 
                    {
                        updateLeaveType.UpdatedBy = userDetail.UserID;
                        leaveRuleService.UpdateLeaveTypeDetails(updateLeaveType);
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
            return View(updateLeaveType);

        }

        public ActionResult Delete(int leaveTypeID)
        {
            log.Info("LeaveTypeController/Delete");
            try
            {
                leaveRuleService.DeleteLeaveType(leaveTypeID);
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