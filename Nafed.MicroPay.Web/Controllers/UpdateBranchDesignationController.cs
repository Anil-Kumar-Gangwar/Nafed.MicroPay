using System.Linq;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.Data;

namespace MicroPay.Web.Controllers
{
    public class UpdateBranchDesignationController : BaseController
    {
        private readonly IUpdateBasicService updatebasicService;
        private readonly IDropdownBindService ddlService;

        public UpdateBranchDesignationController(IUpdateBasicService updatebasicService, IDropdownBindService ddlService)
        {
            this.updatebasicService = updatebasicService;
            this.ddlService = ddlService;
        }

        public ActionResult Index()
        {
            log.Info($"UpdateBranchDesignationController/Index");
            return View(userAccessRight);
        }

        public ActionResult UpdateBranchDesigGridView(FormCollection formCollection)
        {
            log.Info($"CadreController/UpdateBranchDesigGridView");
            try
            {
                UpdateBasicViewModel updatebasicVM = new UpdateBasicViewModel();
                List<Model.UpdateBasic> objUpdateBasicList = new List<Model.UpdateBasic>();
                updatebasicVM.listUpdateBasic = updatebasicService.GetBranchDesignationList();
                updatebasicVM.userRights = userAccessRight;
                return PartialView("_UpdateBranchDesigView", updatebasicVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public JsonResult GetCurrentBranchDesignation(string EmployeeCode)
        {
            try
            {
                var CurrentDesignationBranch = updatebasicService.GetCurrentDesignationBranch(EmployeeCode);
                Model.SelectListModel selectEmployeebasic = new Model.SelectListModel();
                return Json(new { EmployeeDesignationBranch = CurrentDesignationBranch }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int EmployeeId)
        {
            log.Info("UpdateBranchDesignationController/Edit");
            try
            {
                BindDropdowns();
                Model.UpdateBasic objUpdateBranchDesignation = new Model.UpdateBasic();
                objUpdateBranchDesignation = updatebasicService.GetBranchDesignation(EmployeeId);
                return View(objUpdateBranchDesignation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.UpdateBasic editUpdateBranchDesignation)
        {
            log.Info("UpdateBranchDesignationController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    BindDropdowns();
                    editUpdateBranchDesignation.EmployeeId = editUpdateBranchDesignation.EmployeeId;
                    editUpdateBranchDesignation.NewDesg = editUpdateBranchDesignation.NewDesg;
                    editUpdateBranchDesignation.NewBranch = editUpdateBranchDesignation.NewBranch;
                    editUpdateBranchDesignation.UpdatedBy = userDetail.UserID;
                    editUpdateBranchDesignation.UpdatedOn = System.DateTime.Now;
                    if (editUpdateBranchDesignation.NewDesg < 1)
                    {
                        TempData["Error"] = "Please enter New Designation.";
                        return View(editUpdateBranchDesignation);
                    }
                   
                        updatebasicService.UpdateBranchDesignation(editUpdateBranchDesignation);
                        TempData["Message"] = "Succesfully Updated";
                        return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editUpdateBranchDesignation);

        }

        private void BindDropdowns()
        {
            var ddlDesignationList = ddlService.ddlDesignationList();
            ddlDesignationList.OrderBy(x => x.value);
            Model.SelectListModel selectDesignation = new Model.SelectListModel();
            selectDesignation.id = 0;
            selectDesignation.value = "Select";
            ddlDesignationList.Insert(0, selectDesignation);
            ViewBag.Designation = new SelectList(ddlDesignationList, "id", "value");


            var ddlBranchList = ddlService.ddlBranchList();
            ddlBranchList.OrderBy(x => x.value);
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");
            

        }
    }
}