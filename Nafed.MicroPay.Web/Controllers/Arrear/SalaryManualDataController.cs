using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.Arrear;
using Model = Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers.Arrear
{
  
    public class SalaryManualDataController : BaseController
    {
        private readonly IArrearService arrearService;
        private readonly IDropdownBindService ddlService;
        public SalaryManualDataController(IArrearService arrearService, IDropdownBindService ddlService)
        {
            this.arrearService = arrearService;
            this.ddlService = ddlService;
        }

        public ActionResult Index()
        {
            log.Info($"SalaryManualDataController/Index");
            SalaryManualDataViewModel salarymanualdataVM = new SalaryManualDataViewModel();
            salarymanualdataVM.userRights = userAccessRight;
            return View(salarymanualdataVM);
        }

        public ActionResult SalaryManualDataGridView(FormCollection formCollection)
        {
            log.Info($"SalaryManualDataController/SalaryManualDataGridView");
            try
            {
                BindDropdowns();
                List<Model.SelectListModel> selectType = new List<Model.SelectListModel>();
                ViewBag.EmployeeDetails = new SelectList(selectType, "id", "value");
                SalaryManualDataViewModel  salarymanualdataVM = new SalaryManualDataViewModel();
                return PartialView("_SearchSalaryManualData", salarymanualdataVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult GetSalaryManualDataGridView(Model.SalaryManualData salmanualdata, FormCollection frm)
        {
            log.Info($"SalaryManualDataController/GetSalaryManualDataGridView");
            try
            {
               
                SalaryManualDataViewModel salManualDataVM = new SalaryManualDataViewModel();
                salManualDataVM.userRights = userAccessRight;
                salManualDataVM.SalaryManualDataList = arrearService.GetSalarymanualData(salmanualdata.BranchID, salmanualdata.EmployeeId, salmanualdata.DesignationID, salmanualdata.SalMonth,salmanualdata.SalYear);
                return PartialView("_SalaryManualDataGridView", salManualDataVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            var ddlBranchList = ddlService.ddlBranchList();
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            var ddlEmployeeList = ddlService.GetAllEmployee();
            Model.SelectListModel selectEmployee = new Model.SelectListModel();
            selectEmployee.id = 0;
            selectEmployee.value = "Select";
            ddlEmployeeList.Insert(0, selectEmployee);
            ViewBag.EmployeeDetails = new SelectList(ddlEmployeeList, "id", "value");

            var ddlDesignationList = ddlService.ddlDesignationList();
            Model.SelectListModel selectDesignation = new Model.SelectListModel();
            selectDesignation.id = 0;
            selectDesignation.value = "Select";
            ddlDesignationList.Insert(0, selectBranch);
            ViewBag.Designation = new SelectList(ddlDesignationList, "id", "value");

            //List<Model.SelectListModel> selectArreartype = new List<Model.SelectListModel>();
            //selectArreartype.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            //selectArreartype.Add(new Model.SelectListModel() { value = "DA Arrear", id = 1 });
            //selectArreartype.Add(new Model.SelectListModel() { value = "Basic Arrear", id = 2 });
            //ViewBag.ArrearType = new SelectList(selectArreartype, "id", "value");


        }
        public JsonResult GetBranchEmployee(int branchID)
        {
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
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("SalaryManualDataController/Create");
            try
            {
                BindDropdowns();
                Model.SalaryManualData objSalaryManualData = new Model.SalaryManualData();
                return View(objSalaryManualData);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.SalaryManualData createSalarymanualdata)
        {
            log.Info("SalaryManualDataController/Create");
            try
            {
                BindDropdowns();
                if (createSalarymanualdata.BranchID == 0)
                {
                    ModelState.AddModelError("BranchID", "Please select branch");
                    return View(createSalarymanualdata);
                }
             
                if (createSalarymanualdata.EmployeeId == 0)
                {
                    ModelState.AddModelError("EmployeeId", "Please select employee");
                    return View(createSalarymanualdata);
                }
                if (createSalarymanualdata.SalMonth == 0)
                {
                    ModelState.AddModelError("SalMonth", "Please select month");
                    return View(createSalarymanualdata);
                }
                if (createSalarymanualdata.SalYear == 0)
                {
                    ModelState.AddModelError("SalYear", "Please select year");
                    return View(createSalarymanualdata);
                }
                if (createSalarymanualdata.DesignationID == 0)
                {
                    ModelState.AddModelError("DesignationID", "Please select designation");
                    return View(createSalarymanualdata);
                }
              
                if (createSalarymanualdata.ArrearType == 0)
                {
                    ModelState.AddModelError("ArrearType", "Please select arreartype");
                    return View(createSalarymanualdata);
                }
                

                if (ModelState.IsValid)
                {
                    bool chkdata = false;
                    chkdata = arrearService.salaryDataExists(createSalarymanualdata.BranchID, createSalarymanualdata.EmployeeId, createSalarymanualdata.SalMonth, createSalarymanualdata.SalYear, createSalarymanualdata.ArrearType);
                    if (chkdata)
                    {
                        TempData["Error"] = "Data already exist for this period.";
                        return RedirectToAction("Create");
                    }

                    createSalarymanualdata.CreatedOn = DateTime.Now;
                    createSalarymanualdata.CreatedBy = userDetail.UserID;
                    createSalarymanualdata.SeqNo = 1;
                    createSalarymanualdata.DateofGenerateSalary = DateTime.Now;
                    createSalarymanualdata.RecordType = "S";
                    int ID = arrearService.InsertSalarymanualdata(createSalarymanualdata);
                    if (ID > 0)
                    {
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
            return View(createSalarymanualdata);
        }


        [HttpGet]
        public ActionResult Edit(int ID)
        {
            log.Info("SalaryManualDataController/Edit");
            try
            {
                BindDropdowns();
                Model.SalaryManualData objSalaryManualData = new Model.SalaryManualData();
                objSalaryManualData = arrearService.GetSalaryManualbyID(ID);
                //BindDropdowns();
                return View(objSalaryManualData);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.SalaryManualData editsalarymanualdata)
        {
            log.Info("SalaryManualDataController/Edit");
            try
            {
                BindDropdowns();
                if (ModelState.IsValid)
                {
                    editsalarymanualdata.UpdatedBy = userDetail.UserID;
                    editsalarymanualdata.UpdatedOn = DateTime.Now;
                    int PFOPBalanceID = arrearService.Updatesalarymanualdata(editsalarymanualdata);
                    TempData["Message"] = "Succesfully Updated";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editsalarymanualdata);
        }
        public ActionResult Delete(int ID)
        {
            log.Info("SalaryManualDataController/Delete");
            try
            {
                arrearService.DeleteArrearmanualdata(ID);
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