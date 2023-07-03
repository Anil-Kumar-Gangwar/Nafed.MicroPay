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
    public class InsuranceController : BaseController
    {
        private readonly IInsuranceService InsuranceService;
        private readonly IDropdownBindService ddlService;
        public InsuranceController(IInsuranceService InsuranceService, IDropdownBindService ddlService)
        {
            this.InsuranceService = InsuranceService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            log.Info($"InsuranceController/Index");
            return View();
        }

        public ActionResult _Filters()
        {
            log.Info($"InsuranceController/InsuranceGridView");
            try
            {
                Model.CommonFilter cFilter = new Nafed.MicroPay.Model.CommonFilter();
                ViewBag.branchList = ddlService.ddlBranchList();
                ViewBag.employeeList = ddlService.GetAllEmployee();

              var StatusList = new List<Model.SelectListModel>()
                {
                    new Model.SelectListModel { id = 0, value = "No" },
                    new Model.SelectListModel { id = 1, value = "Yes" }
                };
                ViewBag.Status = StatusList;
                return PartialView("_Filter", cFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _PostInsuranceFilter(Model.CommonFilter cFilter)
        {
            log.Info($"InsuranceController/_PostInsuranceFilters");
            try
            {
                if (!cFilter.BranchID.HasValue)
                {
                    return Json(new { msgType = "error", msg = "Please select branch." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    bool? status = cFilter.StatusID.HasValue ? (cFilter.StatusID.Value == 0 ? false : true) : (bool?)null;
                    var listInsurance = InsuranceService.GetInsuranceList(cFilter.EmployeeID,cFilter.BranchID, status);
                    return PartialView("_InsuranceGridView", listInsurance);
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
            log.Info($"InsuranceController/InsuranceGridView");
            try
            {
                var employeedetails = ddlService.employeeByBranchID(branchID);              
                return Json(new { employees = employeedetails }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Create(int? empId)
        {
            log.Info("InsuranceController/Create");
            try
            {
             
                if (empId > 0)
                {
                    var ddlEmployeeList = ddlService.GetAllEmployee(empId.Value);                    
               
                    Model.Insurance objInsurance = new Model.Insurance();
                    var insDepList = InsuranceService.GetDependentList(empId.Value);
                    objInsurance.InsuranceDependenceList = insDepList;
                    objInsurance.EmployeeId= ddlEmployeeList.FirstOrDefault().id;
                    objInsurance.EmployeeName = ddlEmployeeList.FirstOrDefault().value;
                    objInsurance.PolicyAvail = true;
                    objInsurance.DependentMedicalPolicy = true;
                    string designationName = string.Empty;
                    objInsurance.FamilyAssuredSum = InsuranceService.GetFmilyAssuredByDesignationId(empId.Value,out designationName);
                    objInsurance.DesignationName = designationName;
                    return View(objInsurance);
                }
                else
                {
                    TempData["Error"] = "Please select employee to add insurance policy";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult Create(Model.Insurance Insurance)
        {
            log.Info("InsuranceController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    if (Insurance.PolicyAvail)
                    {
                        Insurance.CreatedBy = userDetail.UserID;
                        Insurance.CreatedOn = DateTime.Now;
                        if (Insurance.DependentMedicalPolicy)
                        {
                            var getCheckedDep = Insurance.InsuranceDependenceList.Where(x => x.IsApplicable == true && x.PolicyJoinDate != null).ToList();
                            Insurance.InsuranceDependenceList = getCheckedDep;
                        }
                        InsuranceService.InsertInsuranceDetails(Insurance);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = "You have not select the Member of Nafed Cashless Group Mediclaim policy option, plese select the option first.";
                        return View(Insurance);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(Insurance);
        }

        [HttpGet]
        public ActionResult Edit(int insuranceID, int employeeId)
        {
            log.Info("InsuranceController/Edit");
            try
            {
                Model.Insurance objInsurance = new Model.Insurance();
                objInsurance = InsuranceService.GetInsuranceByID(employeeId, insuranceID);                        
                return View(objInsurance);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Insurance editInsurance)
        {
            log.Info("InsuranceController/Edit");
            try
            {               
                if (ModelState.IsValid)
                {
                    if (editInsurance.InsuranceDependenceList != null)
                    {
                        var getCheckedDep = editInsurance.InsuranceDependenceList.Where(x => x.IsApplicable == true && x.PolicyJoinDate != null).ToList();
                        editInsurance.InsuranceDependenceList = getCheckedDep;
                    }
                    editInsurance.UpdatedBy = userDetail.UserID;
                    editInsurance.Updatedon = DateTime.Now;
                    InsuranceService.UpdateInsuranceDetails(editInsurance);
                    TempData["Message"] = "Successfully Updated";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editInsurance);
        }

        public ActionResult Delete(int empInsuranceID)
        {
            log.Info("Delete");
            try
            {
                InsuranceService.DeleteInsuranceDetails(empInsuranceID);
                TempData["Message"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetDependent(int employeeId)
        {
            log.Info("InsuranceController/GetDependent");
            try
            {
                var insDepList = InsuranceService.GetDependentList(employeeId);
                return Json(new { dependent = ConvertViewToString("_DependentList", insDepList) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}