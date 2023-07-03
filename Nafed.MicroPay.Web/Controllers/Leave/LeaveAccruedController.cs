using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.Data;

namespace MicroPay.Web.Controllers.Leave
{
    public class LeaveAccruedController : BaseController
    {
        // GET: LeaveAccrued
        private readonly ILeaveService leaveService;

        public LeaveAccruedController(ILeaveService leaveService)
        {
            this.leaveService = leaveService;
        }

        public ActionResult Index()
        {
            log.Info($"LeaveAccruedController/Index");
            return View(userAccessRight);
        }

        public ActionResult LeaveAccrueddetailsGridView(Model.LeaveAccruedDetails leaveaccrued)
        {
            log.Info($"LeaveAccruedController/LeaveAccrueddetailsGridView");
            try
            {
                List<Model.LeaveAccruedDetails> leaveaccruedlist = new List<Model.LeaveAccruedDetails>();
                
               Creditleaves(leaveaccrued);
               leaveaccruedlist = leaveService.GetEmployeeLeaveAccruedList(leaveaccrued);
                return PartialView("_LeaveAccruedDetailsGridView", leaveaccruedlist);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult AddLeaveAccrueddetails(FormCollection formCollection)
        {
            log.Info($"LeaveAccruedController/AddLeaveAccrueddetails");
            try
            {
                BindDropdowns();
                LeaveAccruedViewModel leaveaccruedVM = new LeaveAccruedViewModel();
                List<Model.LeaveAccruedDetails> selectType = new List<Model.LeaveAccruedDetails>();
                ViewBag.Month = new SelectList(selectType, "id", "value");
                return PartialView("_AddLeaveAccrueddetails", leaveaccruedVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {

            #region LeaveType

            var ddlLeaveTypeList = leaveService.GetFillDropdown("L", 0);
            Model.LeaveAccruedDetails selectLeavetype = new Model.LeaveAccruedDetails();
            selectLeavetype.id = 0;
            selectLeavetype.value = "Select";
            ddlLeaveTypeList.Insert(0, selectLeavetype);
            ViewBag.leaveType = new SelectList(ddlLeaveTypeList, "id", "value");

            #endregion


        }

        public JsonResult GetleavetypeMonth(int leavecategoryId)
        {
            try
            {
                var ddlMonthList = leaveService.GetFillDropdown("M", leavecategoryId);
                Model.LeaveAccruedDetails selectmonth = new Model.LeaveAccruedDetails();
                selectmonth.id = 0;
                selectmonth.value = "Select";
                ddlMonthList.Insert(0, selectmonth);
                var month = new SelectList(ddlMonthList, "id", "value");
                return Json(new { employees = month }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpPost]
        public void Creditleaves(Model.LeaveAccruedDetails leaveacureddetails)
        {
            //string s = "";
            log.Info($"LeaveAccrued/Creditleaves");
            int creditleave = 0;
            try
            {
                if (leaveacureddetails.Month == 0)
                    ModelState.AddModelError("TitleRequired", "Select Month");
                if (leaveacureddetails.leavecategoryId == 0)
                    ModelState.AddModelError("TitleRequired", "Select Year");
                if (ModelState.IsValid)
                {
                    #region ELAccrued

                    if (leaveacureddetails.leavecategoryId == 4)
                    {
                        List<Model.LeaveAccruedDetails> leavevalidatemonth = new List<Model.LeaveAccruedDetails>();
                        leavevalidatemonth = leaveService.GetLeavevalidatemonth(leaveacureddetails);
                        DataTable DT = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(leavevalidatemonth);
                        if (DT.Rows.Count == 0)
                        {
                            creditleave = leaveService.ELAccumulation(leaveacureddetails, userDetail.UserID);
                            if (creditleave == 1)
                            {
                                TempData["Message"] = "Leave credited successfully";
                            }
                        }
                        else
                        {
                            TempData["Message"] = "This year's Earn Leaves Accrual is already processed for the  selected month.";
                        }
                    }
                    #endregion

                    #region CLAccrued

                    else if(leaveacureddetails.leavecategoryId == 2)
                    {
                        List<Model.LeaveAccruedDetails> leavevalidatemonth = new List<Model.LeaveAccruedDetails>();
                        leavevalidatemonth = leaveService.GetLeavevalidatemonth(leaveacureddetails);
                        DataTable DT = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(leavevalidatemonth);
                        if (DT.Rows.Count == 0)
                        {
                            creditleave = leaveService.CLAccumulation(leaveacureddetails, userDetail.UserID);
                            if (creditleave == 1)
                            {
                                TempData["Message"] = "Leave credited  successfully";
                            }
                        }
                        else
                        {
                            TempData["Message"] = "This year's Earn Leaves Accrual is already processed for the  selected month.";
                        }
                    }
                    #endregion

                    #region MLAccrued
                    else if (leaveacureddetails.leavecategoryId == 8)
                    {
                        List<Model.LeaveAccruedDetails> leavevalidatemonth = new List<Model.LeaveAccruedDetails>();
                        leavevalidatemonth = leaveService.GetLeavevalidatemonth(leaveacureddetails);
                        DataTable DT = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(leavevalidatemonth);
                        if (DT.Rows.Count == 0)
                        {
                            creditleave = leaveService.PuttingMedicalLeave(leaveacureddetails, userDetail.UserID);
                            if (creditleave == 1)
                            {
                                TempData["Message"] = "Leave credited successfully";
                            }
                        }
                        else
                        {
                            TempData["Message"] = "This year's Medical Leaves Accrual is already processed.";
                        }
                    }
                    #endregion
                }
                else
                {
                    TempData["Message"] = "Please select Leave Type and Month to credit leaves.";
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

    
    }
}