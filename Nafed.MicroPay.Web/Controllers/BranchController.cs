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

namespace MicroPay.Web.Controllers
{
    public class BranchController : BaseController
    {
        private readonly IBranchService branchService;
        private readonly IDropdownBindService ddlService;
        public BranchController(IBranchService branchService, IDropdownBindService ddlService)
        {
            this.branchService = branchService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BranchGridView(FormCollection formCollection)
        {
            log.Info($"BranchController/BranchGridView");
            try
            {
                BranchViewModel branchVM = new BranchViewModel();
                branchVM.branchList = branchService.GetBranchList();
                return PartialView("_BranchGridView", branchVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            var ddlCityList = ddlService.ddlCityList();
            Model.SelectListModel selectCity = new Model.SelectListModel();
            selectCity.id = 0;
            selectCity.value = "Select";
            ddlCityList.Insert(0, selectCity);
            ViewBag.CityList = new SelectList(ddlCityList, "id", "value");


            var ddlGradeList = ddlService.ddlGradeList();
            Model.SelectListModel selectGradeList = new Model.SelectListModel();
            selectGradeList.id = 0;
            selectGradeList.value = "Select";
            ddlGradeList.Insert(0, selectGradeList);
            ViewBag.GradeList = new SelectList(ddlGradeList, "id", "value");

        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("BranchController/Create");
            try
            {
                BindDropdowns();
                Model.Branch objBranch = new Model.Branch();
                return View(objBranch);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.Branch createBranch)
        {
            log.Info("BranchController/Create");
            try
            {
                BindDropdowns();
                if (branchService.BranchNameExists(createBranch.BranchName, branchId: null))
                    ModelState.AddModelError("BranchName", "Branch Name Already Exist");
                if (branchService.EmailidExists(createBranch.EmailId, branchId: null))
                    ModelState.AddModelError("EmailId", "Email Address Already Exist");

                if (ModelState.IsValid)
                {
                    createBranch.BranchCode = "123";
                    createBranch.CreatedOn = DateTime.Now;
                    createBranch.CreatedBy = userDetail.UserID;
                    createBranch.CityID = createBranch.CityID == 0 ? null : createBranch.CityID;
                    createBranch.GradeID = createBranch.GradeID == 0 ? null : createBranch.GradeID;
                    int branchID = branchService.InsertBranch(createBranch);
                    TempData["Message"] = "Successfully Created";
                    return RedirectToAction("Index");

                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createBranch);
        }

        [HttpGet]
        public ActionResult Edit(int branchID)
        {
            log.Info("BranchController/Edit");
            try
            {
                BindDropdowns();
                Model.Branch objBranch = new Model.Branch();
                objBranch = branchService.GetBranchByID(branchID);
                return View(objBranch);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Model.Branch editBranch)
        {
            log.Info("BranchController/Edit");
            try
            {
                BindDropdowns();
                if (branchService.BranchNameExists(editBranch.BranchName, branchId: editBranch.BranchID))
                    ModelState.AddModelError("BranchName", "Branch Name Already Exist");
                if (branchService.EmailidExists(editBranch.EmailId, branchId: editBranch.BranchID))
                    ModelState.AddModelError("EmailId", "Email Address Already Exist");
                if (ModelState.IsValid)
                {
                    editBranch.BranchCode = editBranch.BranchCode.Trim();
                    editBranch.UpdatedOn = DateTime.Now;
                    editBranch.UpdatedBy = userDetail.UserID;
                    branchService.UpdateBranch(editBranch);

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
            return View(editBranch);

        }

        public ActionResult Delete(int branchID)
        {
            log.Info("BranchController/Delete");
            try
            {
                branchService.Delete(branchID);
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