using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using MicroPay.Web.Models;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using Common = Nafed.MicroPay.Common;
using static Nafed.MicroPay.Common.DocumentUploadFilePath;
using static Nafed.MicroPay.Common.FileHelper;
using System.IO;
using Nafed.MicroPay.Services.Increment;

namespace MicroPay.Web.Controllers.Increment
{
    public class StopIncrementController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IIncrement incrementService;
        public StopIncrementController(IDropdownBindService ddlService, IIncrement incrementService)
        {
            this.ddlService = ddlService;
            this.incrementService = incrementService;
        }
        public ActionResult Index()
        {
            log.Info($"StopIncrementController/Index");
            try
            {
                ProjectedIncrementViewModel projectedVM = new ProjectedIncrementViewModel();
                projectedVM.userRights = userAccessRight;

                var flgOpenFromIncrement = TempData["flgOpenFromIncrement"] == null ? false : (bool)TempData["flgOpenFromIncrement"];
                TempData.Keep("flgOpenFromIncrement");
                var gintIncrementMonth = TempData["gintIncrementMonth"] == null ? 0 : (int?)TempData["gintIncrementMonth"];
                TempData.Keep("gintIncrementMonth");
                List<int?> monthArray = new List<int?>();
                if (gintIncrementMonth == 1)
                {
                    projectedVM.January = true;
                    monthArray.Add(gintIncrementMonth);
                }
                else if (gintIncrementMonth == 7)
                {
                    projectedVM.July = true;
                    monthArray.Add(7);
                }
                int?[] incrmentMonth = monthArray.ToArray();
                bool validateincrement = flgOpenFromIncrement;
                projectedVM.OnlyStopped = flgOpenFromIncrement;

                if (incrmentMonth.Count() > 0)
                {
                    var stopIncrementDetails = incrementService.GetStopIncrementDetails(incrmentMonth, validateincrement);
                    projectedVM.stopIncrementDetails = stopIncrementDetails;
                }
                return View(projectedVM);
            }
            catch(Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                      throw ex;
            }
        }

        //public ActionResult GetStopIncrementDetails()
        //{
        //    log.Info($"StopIncrementController/GetStopIncrementDetails");
        //    try
        //    {
        //        ProjectedIncrementViewModel projectedVM = new ProjectedIncrementViewModel();
        //        projectedVM.userRights = userAccessRight;

        //        var flgOpenFromIncrement = TempData["flgOpenFromIncrement"] == null ? false : (bool)TempData["flgOpenFromIncrement"];
        //        TempData.Keep("flgOpenFromIncrement");
        //        var gintIncrementMonth = TempData["gintIncrementMonth"] == null ? 0 : (int?)TempData["gintIncrementMonth"];
        //        TempData.Keep("gintIncrementMonth");
        //        List<int?> monthArray = new List<int?>();
        //        if (gintIncrementMonth == 1) {
        //            projectedVM.January = true;
        //            monthArray.Add(gintIncrementMonth);
        //        }
        //        else if (gintIncrementMonth == 7) {
        //            projectedVM.July = true;
        //            monthArray.Add(7);
        //        }
        //        int?[] incrmentMonth = monthArray.ToArray();
        //        bool validateincrement = flgOpenFromIncrement;
        //        projectedVM.OnlyStopped = flgOpenFromIncrement;

        //        if (incrmentMonth.Count() > 0)
        //        {
        //            var stopIncrementDetails = incrementService.GetStopIncrementDetails(incrmentMonth, validateincrement);
        //            projectedVM.stopIncrementDetails = stopIncrementDetails;
        //        }
        //        return PartialView("_StopIncrementDetails", projectedVM);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //}

        public ActionResult StopIncrementDetails(ProjectedIncrementViewModel prVM)
        {
            log.Info($"StopIncrementController/StopIncrementDetails");
            try
            {
                ProjectedIncrementViewModel projectedVM = new ProjectedIncrementViewModel();
                projectedVM.userRights = userAccessRight;
                ModelState.Clear();
                List<int?> monthArray = new List<int?>();
                if (prVM.January)
                    monthArray.Add(1);
                else if (prVM.July)
                    monthArray.Add(7);
                else if(prVM.January && prVM.July)
                {
                    monthArray.Add(1);
                    monthArray.Add(7);
                }
                int?[] incrmentMonth = monthArray.ToArray();
                bool validateincrement = prVM.OnlyStopped;

                if (incrmentMonth.Count() > 0)
                {
                    var stopIncrementDetails = incrementService.GetStopIncrementDetails(incrmentMonth, validateincrement);
                    projectedVM.stopIncrementDetails = stopIncrementDetails;
                }
                return PartialView("_StopIncrementDetails", projectedVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
     
        public ActionResult UpdateStopIncrement(ProjectedIncrementViewModel pVM)
        {
            try
            {
                ProjectedIncrementViewModel projectedVM = new ProjectedIncrementViewModel();
                ModelState.Clear();
                if (pVM.stopIncrementDetails.Count > 0)
                {
                    var flag = incrementService.UpdateStopIncrementDetails(pVM.stopIncrementDetails);
                    if (flag)
                    {
                        List<int?> monthArray = new List<int?>();
                        if (pVM.January)
                            monthArray.Add(1);
                        else if (pVM.July)
                            monthArray.Add(7);
                        else if (pVM.January && pVM.July)
                        {
                            monthArray.Add(1);
                            monthArray.Add(7);
                        }
                        int?[] incrmentMonth = monthArray.ToArray();
                        var validateincrement = pVM.OnlyStopped;
                        var stopIncrementDetails = incrementService.GetStopIncrementDetails(incrmentMonth, validateincrement);
                        projectedVM.stopIncrementDetails = stopIncrementDetails;
                    }
                    else
                    {
                        projectedVM.stopIncrementDetails = pVM.stopIncrementDetails;
                        projectedVM.January = pVM.January;
                        projectedVM.July = pVM.July;
                        projectedVM.OnlyStopped = pVM.OnlyStopped;
                    }
                    ViewBag.Message = "Stop increment details updated successfully!";
                    return PartialView("_StopIncrementDetails", projectedVM);
                }
                else
                {
                    return PartialView("_StopIncrementDetails", pVM);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _GetStopIncrementDetails(ProjectedIncrementViewModel pIVM, string Validate)
        {
            try
            {
                ModelState.Clear();
                if (Validate.ToLower() == "search")
                {
                    ProjectedIncrementViewModel projectedVM = new ProjectedIncrementViewModel();
                    projectedVM.userRights = userAccessRight;

                    List<int?> monthArray = new List<int?>();
                    if (pIVM.January)
                        monthArray.Add(1);
                    else if (pIVM.July)
                        monthArray.Add(7);
                    else if (pIVM.January && pIVM.July)
                    {
                        monthArray.Add(1);
                        monthArray.Add(7);
                    }
                    int?[] incrmentMonth = monthArray.ToArray();
                    bool validateincrement = pIVM.OnlyStopped;
                    pIVM.EmployeeCode = pIVM.EmployeeCode != null ? pIVM.EmployeeCode.Trim() : pIVM.EmployeeCode;
                    pIVM.EmployeeName = pIVM.EmployeeName != null ? pIVM.EmployeeName.Trim() : pIVM.EmployeeName;
                    if (incrmentMonth.Count() > 0)
                    {
                        var stopIncrementDetails = incrementService.GetStopIncrementDetails(incrmentMonth, validateincrement);
                        stopIncrementDetails = stopIncrementDetails.Where(x => (pIVM.EmployeeCode != null ? (x.EmployeeCode == pIVM.EmployeeCode) : 1 > 0) && (pIVM.EmployeeName != null ? (x.EmployeeName == pIVM.EmployeeName) : 1 > 0)).ToList();
                        projectedVM.stopIncrementDetails = stopIncrementDetails;
                    }
                    return PartialView("_StopIncrementDetails", projectedVM);
                }
                else
                {
                    return UpdateStopIncrement(pIVM);
                }
            }
            catch(Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}