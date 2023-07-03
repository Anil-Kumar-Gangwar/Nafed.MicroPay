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
    public class HillCompensationController : BaseController
    {
        private readonly IHillCompensationService hillCompServiceService;
        private readonly IDropdownBindService dropdownBindService;
        public HillCompensationController(IHillCompensationService hillCompServiceService, IDropdownBindService dropdownBindService)
        {
            this.hillCompServiceService = hillCompServiceService;
            this.dropdownBindService = dropdownBindService;

        }
        // GET: HillCompensation      

        public ActionResult Index()
        {
            log.Info($"HillCompensationController/Index");
            return View(userAccessRight);

        }
        public ActionResult HillCompensationGridView()
        {
            log.Info($"HillCompensationController/HillCompensationGridView");
            try
            {
                HillCompensationViewModel hillCompVM = new HillCompensationViewModel();
                hillCompVM.HillCompensationList = hillCompServiceService.GetHillCompensationList();
                hillCompVM.userRights = userAccessRight;
                return PartialView("_HillCompensationGridView", hillCompVM);
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
            log.Info("HillCompensationController/Create");
            try
            {
                List<SelectListModel> branchList = new List<SelectListModel>();
                branchList = dropdownBindService.ddlBranchList();
                HillCompensation objhillComp = new HillCompensation();
                objhillComp.BranchList = branchList;
                return View(objhillComp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(HillCompensation createHillCompensation)
        {
            log.Info("HillCompensationController/Create");
            try
            {
                if (ModelState.IsValid)
                {

                    createHillCompensation.CreatedBy = userDetail.UserID;
                    createHillCompensation.CreatedOn = DateTime.Now;
                    createHillCompensation.IsDeleted = false;
                    if (hillCompServiceService.HillCompensationExists(createHillCompensation.BranchCode, createHillCompensation.UpperLimit))
                    {
                        List<SelectListModel> branchList = new List<SelectListModel>();
                        branchList = dropdownBindService.ddlBranchList();
                        createHillCompensation.BranchList = branchList;
                        ModelState.AddModelError("HillCompensationAlreadyExist", "Hill Compensation Already Exist");
                        return View(createHillCompensation);
                    }
                    else
                    {
                        hillCompServiceService.InsertHillCompensation(createHillCompensation);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    List<SelectListModel> branchList = new List<SelectListModel>();
                    branchList = dropdownBindService.ddlBranchList();
                    createHillCompensation.BranchList = branchList;
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createHillCompensation);
        }

        [HttpGet]
        public ActionResult Edit(string branchCode, decimal upperLimit)
        {
            log.Info("HillCompensationController/Edit");
            try
            {              
                HillCompensation objhillComp = new HillCompensation();
                objhillComp = hillCompServiceService.GetHillCompensationbyBranchAmount(branchCode, upperLimit);
                List<SelectListModel> branchList = new List<SelectListModel>();
                branchList = dropdownBindService.ddlBranchList();
                objhillComp.BranchList = branchList;
                return View(objhillComp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(HillCompensation editHillCompensation)
        {
            log.Info("HillCompensationController/Edit");
            try
            {
                if (ModelState.IsValid)
                {

                    editHillCompensation.UpdatedBy = userDetail.UserID;
                    editHillCompensation.UpdatedOn = DateTime.Now.Date;
                    editHillCompensation.IsDeleted = false;
                    hillCompServiceService.UpdateHillCompensation(editHillCompensation);
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");              
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editHillCompensation);

        }

        public ActionResult Delete(string branchCode, decimal upperLimit)
        {
            log.Info("HillCompensationController/Delete");
            try
            {
                hillCompServiceService.Delete(branchCode,upperLimit);
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