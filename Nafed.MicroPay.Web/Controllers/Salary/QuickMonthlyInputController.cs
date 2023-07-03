using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.Salary;
using Newtonsoft.Json;

namespace MicroPay.Web.Controllers.Salary
{
    public class QuickMonthlyInputController : BaseController
    {
        private readonly ISalaryHeadRuleService salaryService;
        private readonly IDropdownBindService dropdownBindService;
       // private readonly ITrainingService trainingService;
        public QuickMonthlyInputController(ISalaryHeadRuleService salaryService, IDropdownBindService dropdownBindService, ITrainingService trainingService)
        {
            this.salaryService = salaryService;
            this.dropdownBindService = dropdownBindService;
           // this.trainingService = trainingService;
        }
        // GET: QuickMonthlyInput
        public ActionResult Index()
        {
            log.Info("QuickMonthlyInput/Index");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _Create()
        {
            log.Info("QuickMonthlyInputController/_Create");
            try
            {
                Model.SalaryMonthlyInput objSalaryMonthlyInput = new Model.SalaryMonthlyInput();
                //objSalaryMonthlyInput.RegularEmployeeList = 
                //    trainingService.GetTrainingParticipantsDetailsList(null, null, null).Select(x => new Nafed.MicroPay.Model.SelectListModel { id = x.EmployeeID, value = x.EmployeeCode + "-" + x.EmployeeName }).ToList();
                objSalaryMonthlyInput.EmployeeTypeList = dropdownBindService.ddlEmployeeTypeList();
                objSalaryMonthlyInput.MonthlyInputHead = new List<Model.SalaryHeadField>()
                {
                    new Model.SalaryHeadField { FieldName = "0", FieldDesc = "Select" }
                };

                objSalaryMonthlyInput.RegularEmployeeList = new List<Model.SelectListModel>()
                {
                    new Model.SelectListModel { id=0, value="Select"  }
                };

                //var currentYear = DateTime.Now.Year;
                //var currentMonth = DateTime.Now.Month;

                //int PreMonth;
                //int PreYear;

                //if (currentMonth == 1)
                //{
                //    PreMonth = 12;
                //    PreYear = currentYear - 1;
                //}
                //else
                //{
                //    PreMonth = currentMonth - 1;
                //    PreYear = currentYear;
                //}

                //bool flag = salaryService.CheckMonthlyInputDetails(currentYear, currentMonth, PreYear, PreMonth);
                return PartialView("_Create", objSalaryMonthlyInput);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult GetBranchDetails(string SalaryHead, int EmployeeId, int Month, int Year)
        {
            log.Info("QuickMonthlyInputController/GetBranchDetails");
            try
            {
                Model.SalaryMonthlyBranchAndAmount objSalaryMonthlyAmount = new Model.SalaryMonthlyBranchAndAmount();
                //var currentYear = DateTime.Now.Year;
                //var currentMonth = DateTime.Now.Month;
                var branch = salaryService.GetBranchDetails(SalaryHead, EmployeeId, Month, Year);
                var result = JsonConvert.SerializeObject(branch, Formatting.Indented,
                           new JsonSerializerSettings
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
                return Json(new { saved = true, msgType = "success", msg = "Saved Sucessfully", htmlData = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.SalaryMonthlyInput salaryMonthlyInput)
        {
            log.Info("QuickMonthlyInputController/Create");
            bool flag = false;
            try
            {
                ModelState.Remove("OldAmount");
                if (salaryMonthlyInput.MonthlyInputHeadId == "0")
                    ModelState.AddModelError("monthlyInputRequired", "Monthly Head Required");
                if (ModelState.IsValid)
                {
                    var currMonth = DateTime.Now.Month;
                    var currYear = DateTime.Now.Year;
                    var cmbCurrent = Convert.ToString(currYear) + (Convert.ToString(currMonth).Length == 1 ? "0" + Convert.ToString(currMonth) : Convert.ToString(currMonth));
                    var selectedMonth = salaryMonthlyInput.monthId;
                    var selectedYear = salaryMonthlyInput.yearId;
                    var cmbSelected = Convert.ToString(selectedYear) + (Convert.ToString(selectedMonth).Length == 1 ? "0" + Convert.ToString(selectedMonth) : Convert.ToString(selectedMonth));
                    int PreMonth, PreYear;
                    if (selectedMonth == 1)
                    {
                        PreMonth = 12;
                        PreYear = selectedYear - 1;
                    }
                    else
                    {
                        PreMonth = selectedMonth - 1;
                        PreYear = selectedYear;
                    }
                    if (Convert.ToInt32(cmbSelected) <= Convert.ToInt32(cmbCurrent))
                        flag = salaryService.CheckMonthlyInputDetails(selectedYear, selectedMonth, PreYear, PreMonth);
                    if (Convert.ToInt32(cmbSelected) < Convert.ToInt32(cmbCurrent))
                    {
                        flag = salaryService.GetFinancialMonthlyDetails(selectedMonth, selectedYear, salaryMonthlyInput.EmployeeId);
                        if (flag)
                        {
                            //salaryMonthlyInput.RegularEmployeeList = trainingService.GetTrainingParticipantsDetailsList(null, null, null).Select(x => new Nafed.MicroPay.Model.SelectListModel { id = x.EmployeeID, value = x.EmployeeCode + "-" + x.EmployeeName }).ToList();
                            salaryMonthlyInput.RegularEmployeeList = dropdownBindService.GetEmployeeDetailsByEmployeeType(null, salaryMonthlyInput.EmployeeTypeId);
                            salaryMonthlyInput.MonthlyInputHead = new List<Model.SalaryHeadField>()
                            {
                                 new Model.SalaryHeadField { FieldName = "0", FieldDesc = "Select" }
                             };
                            salaryMonthlyInput.EmployeeTypeList = dropdownBindService.ddlEmployeeTypeList();
                            return Json(new { msgType = "error", htmlData = ConvertViewToString("_Create", salaryMonthlyInput), msg = "Can not update head value because salary for this month already published." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var message = "";
                            flag = salaryService.UpdateSalaryMonthlyInput(salaryMonthlyInput);
                            //TempData["Message"] = "Successfully Updated";
                            if (flag)
                            {
                                message = "Successfully Updated";
                                return Json(new { msgType = "success", msg = message }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                message = "Updation is not possible because  selected employee for selected month and year not exists";
                                return Json(new { msgType = "warning", msg = message }, JsonRequestBehavior.AllowGet);
                            }

                        }
                    }
                    else if (Convert.ToInt32(cmbSelected) > Convert.ToInt32(cmbCurrent))
                    {
                        //salaryMonthlyInput.RegularEmployeeList = trainingService.GetTrainingParticipantsDetailsList(null, null, null).Select(x => new Nafed.MicroPay.Model.SelectListModel { id = x.EmployeeID, value = x.EmployeeCode + "-" + x.EmployeeName }).ToList();
                        salaryMonthlyInput.RegularEmployeeList = dropdownBindService.GetEmployeeDetailsByEmployeeType(null, salaryMonthlyInput.EmployeeTypeId);
                        salaryMonthlyInput.MonthlyInputHead = new List<Model.SalaryHeadField>()
                            {
                                 new Model.SalaryHeadField { FieldName = "0", FieldDesc = "Select" }
                             };
                        salaryMonthlyInput.EmployeeTypeList = dropdownBindService.ddlEmployeeTypeList();
                        return Json(new { msgType = "error", htmlData = ConvertViewToString("_Create", salaryMonthlyInput), msg = "Can not update head value because future month update is not possible." }, JsonRequestBehavior.AllowGet);
                    }
                    else if ((Convert.ToInt32(cmbSelected) == Convert.ToInt32(cmbCurrent)) && salaryMonthlyInput.MonthlyInputHeadId != "E_14")
                    {
                        flag = salaryService.UpdateSalaryMonthlyInput(salaryMonthlyInput);
                        //TempData["Message"] = "Successfully Updated";
                        return Json(new { msgType = "success", msg = "Successfully Updated" }, JsonRequestBehavior.AllowGet);
                    }
                    else if ((Convert.ToInt32(cmbSelected) == Convert.ToInt32(cmbCurrent)) && salaryMonthlyInput.MonthlyInputHeadId == "E_14")
                    {
                        string currentMonth = "";
                        var fromYear = Convert.ToString(DateTime.Now.Year) + "04";
                        if (DateTime.Now.Month.ToString().Length == 1)
                            currentMonth = "0" + Convert.ToString(DateTime.Now.Month);
                        var toYear = Convert.ToString(DateTime.Now.Year + 1) + currentMonth;

                        var result = salaryService.GetMedicalReimbursement(salaryMonthlyInput.EmployeeId, fromYear, toYear);
                        if (result < 8000)
                        {
                            flag = salaryService.UpdateSalaryMonthlyInput(salaryMonthlyInput);
                            //TempData["Message"] = "Successfully Updated.";
                            return Json(new { msgType = "success", msg = "Successfully Created" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            //salaryMonthlyInput.RegularEmployeeList = trainingService.GetTrainingParticipantsDetailsList(null, null, null).Select(x => new Nafed.MicroPay.Model.SelectListModel { id = x.EmployeeID, value = x.EmployeeCode + "-" + x.EmployeeName }).ToList();
                            salaryMonthlyInput.RegularEmployeeList = dropdownBindService.GetEmployeeDetailsByEmployeeType(null, salaryMonthlyInput.EmployeeTypeId);
                            salaryMonthlyInput.MonthlyInputHead = new List<Model.SalaryHeadField>()
                            {
                                 new Model.SalaryHeadField { FieldName = "0", FieldDesc = "Select" }
                             };
                            salaryMonthlyInput.EmployeeTypeList = dropdownBindService.ddlEmployeeTypeList();
                            return Json(new { msgType = "error", htmlData = ConvertViewToString("_Create", salaryMonthlyInput) }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //salaryMonthlyInput.RegularEmployeeList = trainingService.GetTrainingParticipantsDetailsList(null, null, null).Select(x => new Nafed.MicroPay.Model.SelectListModel { id = x.EmployeeID, value = x.EmployeeCode + "-" + x.EmployeeName }).ToList();
                        salaryMonthlyInput.RegularEmployeeList = dropdownBindService.GetEmployeeDetailsByEmployeeType(null, salaryMonthlyInput.EmployeeTypeId);
                        salaryMonthlyInput.MonthlyInputHead = new List<Model.SalaryHeadField>()
                            {
                                 new Model.SalaryHeadField { FieldName = "0", FieldDesc = "Select" }
                             };
                        salaryMonthlyInput.EmployeeTypeList = dropdownBindService.ddlEmployeeTypeList();
                        return Json(new { msgType = "error", htmlData = ConvertViewToString("_Create", salaryMonthlyInput) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //salaryMonthlyInput.RegularEmployeeList = trainingService.GetTrainingParticipantsDetailsList(null, null, null).Select(x => new Nafed.MicroPay.Model.SelectListModel { id = x.EmployeeID, value = x.EmployeeCode + "-" + x.EmployeeName }).ToList();
                    salaryMonthlyInput.RegularEmployeeList = dropdownBindService.GetEmployeeDetailsByEmployeeType(null, salaryMonthlyInput.EmployeeTypeId);
                    salaryMonthlyInput.MonthlyInputHead = new List<Model.SalaryHeadField>()
                            {
                                 new Model.SalaryHeadField { FieldName = "0", FieldDesc = "Select" }
                             };
                    salaryMonthlyInput.EmployeeTypeList = dropdownBindService.ddlEmployeeTypeList();
                    return Json(new { msgType = "error", htmlData = ConvertViewToString("_Create", salaryMonthlyInput) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult GetSalaryHeadFields(int employeeTypeId)
        {
            log.Info("QuickMonthlyInputController/GetBranchDetails");
            try
            {
                var heads = dropdownBindService.GetSalaryHead(employeeTypeId);
                if (heads.Count == 0)
                {
                    heads = new List<Model.SalaryHeadField>()
                {
                    new Model.SalaryHeadField { FieldName = "0", FieldDesc = "Select" }
                };
                }
                return Json(new { saved = true, msgType = "success", msg = "Saved Sucessfully", htmlData = heads }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult GetEmployeeByEmployeeType(int employeeTypeId)
        {
            log.Info("QuickMonthlyInputController/GetEmployeeByEmployeeType");
            try
            {
                var emp = dropdownBindService.GetEmployeeDetailsByEmployeeType(null, employeeTypeId);
                emp.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });
                return Json(new { htmlData = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

      
        public ActionResult GetEmployeeList(int employeeTypeID, int salMonth, int salYear)
        {
            log.Info($"QuickMonthlyInput/GetEmployeeList/employeeTypeID:{employeeTypeID},salMonth:{salMonth},salYear:{salYear}");

            try
            {
                var empList = salaryService.GetEmployeeList(employeeTypeID, salMonth, salYear);
                empList.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });
               // if (empList?.Count > 0)
               // {
                    return Json(new { empList = empList }, JsonRequestBehavior.AllowGet);
               // }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }
    }
}