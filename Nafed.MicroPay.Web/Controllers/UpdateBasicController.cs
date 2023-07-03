using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers
{
    public class UpdateBasicController : BaseController
    {
        private readonly IUpdateBasicService updatebasicService;
        private readonly IDropdownBindService ddlService;
        public UpdateBasicController(IUpdateBasicService updatebasicService, IDropdownBindService ddlService)
        {
            this.updatebasicService = updatebasicService;
            this.ddlService = ddlService;
        }

        public ActionResult Index()
        {
            log.Info($"UpdateBasicController/Index");
            return View(userAccessRight);
        }

        public ActionResult UpdateBasicGridView(FormCollection formCollection)
        {
            log.Info($"CadreController/UpdateBasicGridView");
            try
            {
                UpdateBasicViewModel updatebasicVM = new UpdateBasicViewModel();
                List<Model.UpdateBasic> objUpdateBasicList = new List<Model.UpdateBasic>();
                updatebasicVM.listUpdateBasic = updatebasicService.GetUpdateBasicList();
                updatebasicVM.userRights = userAccessRight;
                return PartialView("_UpdateBasicView", updatebasicVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private void EmployeeCode()
        {
            log.Info("UpdateBasicController/EmployeeCode");
            var employeetype = ddlService.employeeByBranchID(44);
            Model.SelectListModel select = new Model.SelectListModel();
            select.id = 0;
            select.value = "Select";
            employeetype.Insert(0, select);
            ViewBag.employeetype = new SelectList(employeetype, "id", "value");

        }

        [HttpGet]
        public ActionResult Edit(int EmployeeSalaryID)
        {
            log.Info("UpdateBasicController/Edit");
            try
            {
                Model.UpdateBasic objUpdateBasic = new Model.UpdateBasic();
                objUpdateBasic = updatebasicService.GetBasic(EmployeeSalaryID);
                return View(objUpdateBasic);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.UpdateBasic editUpdateBasic)
        {
            log.Info("UpdateBasicController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editUpdateBasic.EmployeeId = editUpdateBasic.EmployeeId;
                    editUpdateBasic.NewBasic = editUpdateBasic.NewBasic;
                    editUpdateBasic.UpdatedBy = userDetail.UserID;
                    editUpdateBasic.UpdatedOn = System.DateTime.Now;
                    if(editUpdateBasic.NewBasic <1)
                    {
                        TempData["Error"] = "Please enter New Basic.";
                        return View(editUpdateBasic);
                    }
                    int chkvalidateBasic = updatebasicService.ValidateNewBasicAmount(editUpdateBasic.EmployeeSalaryID,(double)editUpdateBasic.NewBasic);
                    if (chkvalidateBasic==0)
                    {
                        TempData["Error"] = "New Basic Amount is not fall in Current Salary Slab of Employee!";
                        return View(editUpdateBasic);
                    }
                    else
                    {
                        updatebasicService.UpdateBasic(editUpdateBasic);
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
            return View(editUpdateBasic);

        }

    }
}