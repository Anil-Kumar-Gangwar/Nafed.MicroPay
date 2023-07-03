using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers
{
    public class EmployeeDependentController : BaseController
    {
        private readonly IDependentService dependentService;
        private readonly IDropdownBindService ddlService;
        public EmployeeDependentController(IDependentService dependentService, IDropdownBindService ddlService)
        {
            this.dependentService = dependentService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            log.Info($"DependentController/Index");
            DependentViewModel dependentVM = new DependentViewModel();
            dependentVM.userRights = userAccessRight;
            dependentVM.branchList = ddlService.ddlBranchList();
            dependentVM.employeeList = ddlService.employeeByBranchID(dependentVM.BranchID);
            dependentVM.EmployeeName = userDetail.FullName;
            dependentVM.EmployeeCode = userDetail.EmployeeCode;
            dependentVM.DepartmentName = userDetail.DepartmentName;
            dependentVM.EmployeeID = userDetail.EmployeeID;
            dependentVM.userTypeID = userDetail.UserTypeID;
            return View(dependentVM);
        }
        public ActionResult EmployeeIndex()
        {
            log.Info($"DependentController/Index");
            DependentViewModel dependentVM = new DependentViewModel();
            dependentVM.userRights = userAccessRight;
            dependentVM.branchList = ddlService.ddlBranchList();
            dependentVM.employeeList = ddlService.employeeByBranchID(dependentVM.BranchID);
            dependentVM.EmployeeName = userDetail.FullName;
            dependentVM.EmployeeCode = userDetail.EmployeeCode;
            dependentVM.DepartmentName = userDetail.DepartmentName;
            dependentVM.EmployeeID = userDetail.EmployeeID;
            dependentVM.userTypeID = userDetail.UserTypeID;
            return View(dependentVM);
        }

        public ActionResult _PostMyDependentFilters(DependentViewModel dependentVM)
        {
            log.Info($"DependentController/DependentGridView");
            try
            {

                dependentVM.EmployeeID = dependentVM.EmployeeID == 0 ? null : dependentVM.EmployeeID;
                dependentVM.listDependent = dependentService.GetDependentList(dependentVM.EmployeeID, dependentVM.BranchID);
                dependentVM.userRights = userAccessRight;
                dependentVM.userTypeID = userDetail.UserTypeID;
                return PartialView("_EmployeeDependentGridView", dependentVM);


            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _PostEmployeeDependentFilters(DependentViewModel dependentVM)
        {
            log.Info($"DependentController/DependentGridView");
            try
            {
                if (userDetail.UserTypeID == 1)
                {
                    dependentVM.EmployeeID = dependentVM.EmployeeID == 0 ? null : dependentVM.EmployeeID;
                    dependentVM.listDependent = dependentService.GetDependentList(dependentVM.EmployeeID, dependentVM.BranchID);
                    dependentVM.userRights = userAccessRight;
                    dependentVM.userTypeID = userDetail.UserTypeID;
                    return PartialView("_DependentGridView", dependentVM);
                }
                else if (userDetail.UserTypeID != 1)
                {
                    dependentVM.EmployeeID = dependentVM.EmployeeID == 0 ? null : dependentVM.EmployeeID;
                    dependentVM.listDependent = dependentService.GetDependentList(dependentVM.EmployeeID, dependentVM.BranchID);
                    dependentVM.userRights = userAccessRight;
                    dependentVM.userTypeID = userDetail.UserTypeID;
                    return PartialView("_DependentGridView", dependentVM);
                }
                else
                {
                    return Json(new { msgType = "error", msg = "Please select Branch." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public JsonResult GetBranchEmployee(int branchID)
        {
            log.Info($"DependentController/DependentGridView");
            try
            {
                var employeedetails = ddlService.employeeByBranchID(branchID);
                Model.SelectListModel selectemployeeDetails = new Model.SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "Select";
                employeedetails.Insert(0, selectemployeeDetails);
                var emp = new SelectList(employeedetails, "id", "value");
                return Json(new { employees = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult DependentGridView(FormCollection formCollection)
        {
            log.Info($"DependentController/DependentGridView");
            try
            {
                DependentViewModel dependentVM = new DependentViewModel();
                dependentVM.listDependent = dependentService.GetDependentList((int?)null);
                dependentVM.userRights = userAccessRight;
                return PartialView("_DependentGridView", dependentVM);
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
            log.Info("DependentController/Create");
            try
            {
                BindDropdowns();
                Model.EmployeeDependent objDependent = new Model.EmployeeDependent();
                var sumOfPfDistibution = dependentService.GetSumOfPfDistibution(userDetail.EmployeeID, null);
                if (Convert.ToInt32(sumOfPfDistibution) < 100)
                    objDependent.PFDistribution = (100 - Convert.ToInt32(sumOfPfDistibution));
                return View(objDependent);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private void BindDropdowns()
        {
            var ddlGenderList = ddlService.ddlGenderList();
            ddlGenderList.OrderBy(x => x.value);
            Model.SelectListModel selectGender = new Model.SelectListModel();
            selectGender.id = 0;
            selectGender.value = "Select";
            ddlGenderList.Insert(0, selectGender);
            ViewBag.Gender = new SelectList(ddlGenderList, "id", "value");

            var ddlRelationList = ddlService.ddlRelationList();
            ddlRelationList.OrderBy(x => x.value);
            Model.SelectListModel relation = new Model.SelectListModel();
            relation.id = 0;
            relation.value = "Select";
            ddlRelationList.Insert(0, relation);
            ViewBag.Relation = new SelectList(ddlRelationList, "id", "value");
           // var empID = userDetail.UserTypeID == 1 ? null : userDetail.EmployeeID == 977 ? null : userDetail.EmployeeID;
            var ddlEmployeeList = ddlService.GetAllEmployee(null);
            ddlEmployeeList.OrderBy(x => x.value);
            Model.SelectListModel employee = new Model.SelectListModel();
            employee.id = 0;
            employee.value = "Select";
            ddlEmployeeList.Insert(0, employee);
            ViewBag.Employee = new SelectList(ddlEmployeeList, "id", "value");

        }
        [HttpPost]
        public ActionResult Create(Model.EmployeeDependent employeeDependent)
        {
            log.Info("DependentController/Create");
            try
            {
                BindDropdowns();
                var sumOfPfDistibution = dependentService.GetSumOfPfDistibution(userDetail.EmployeeID, null);
                if (Convert.ToInt32(employeeDependent.PFDistribution) > 100)
                    ModelState.AddModelError("pfDistributionRequired", "PF distribution can't exceed 100%");
                else if (Convert.ToInt32(sumOfPfDistibution) + Convert.ToInt32(employeeDependent.PFDistribution) > 100)
                    ModelState.AddModelError("pfDistributionRequired", "PF distribution can't exceed 100%");
                if (ModelState.IsValid)
                {
                    //var lastDependentCode = dependentService.GetLastDependentCode(employeeDependent.EmployeeId);
                    //employeeDependent.DependentCode = lastDependentCode + 1;
                    employeeDependent.EmpCode = employeeDependent.EmpCode.Split('-')[0];
                    employeeDependent.CreatedBy = userDetail.UserID;
                    employeeDependent.CreatedOn = DateTime.Now;
                    dependentService.InsertDependentDetails(employeeDependent);
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
            return View(employeeDependent);
        }

        [HttpGet]
        public ActionResult Edit(int empDependentID)
        {
            log.Info("DependentController/Edit");
            try
            {
                BindDropdowns();
                Model.EmployeeDependent objEmployeeDependent = new Model.EmployeeDependent();
                var sumOfPfDistibution = dependentService.GetSumOfPfDistibution(userDetail.EmployeeID, null);
                objEmployeeDependent = dependentService.GetDependentByID(empDependentID);
                if (objEmployeeDependent.PFDistribution == null && Convert.ToInt32(sumOfPfDistibution) < 100)
                    objEmployeeDependent.PFDistribution = (100 - Convert.ToInt32(sumOfPfDistibution));
                return View(objEmployeeDependent);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.EmployeeDependent editDependent)
        {
            log.Info("DependentController/Edit");
            try
            {
                BindDropdowns();
                var sumOfPfDistibution = dependentService.GetSumOfPfDistibution(userDetail.EmployeeID, editDependent.EmpDependentID);
                if (Convert.ToInt32(editDependent.PFDistribution) > 100)
                    ModelState.AddModelError("pfDistributionRequired", "PF distribution can't exceed 100%");
                else if (Convert.ToInt32(sumOfPfDistibution) + Convert.ToInt32(editDependent.PFDistribution) > 100)
                    ModelState.AddModelError("pfDistributionRequired", "PF distribution can't exceed 100%");
                if (ModelState.IsValid)
                {
                    editDependent.UpdatedBy = userDetail.UserID;
                    editDependent.UpdatedOn = DateTime.Now;
                    dependentService.UpdateDependentDetails(editDependent);
                    TempData["Message"] = "Successfully Updated";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editDependent);
        }

        public ActionResult Delete(int empDependentID)
        {
            log.Info("Delete");
            try
            {
                dependentService.DeleteDependentDetails(empDependentID);
                TempData["Message"] = "Successfully Deleted";
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