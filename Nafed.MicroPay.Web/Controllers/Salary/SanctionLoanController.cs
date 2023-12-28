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

namespace MicroPay.Web.Controllers.Salary
{
    public class SanctionLoanController : BaseController
    {
        private readonly ISalaryHeadRuleService salaryService;
        private readonly IDropdownBindService dropdownBindService;

        public SanctionLoanController(ISalaryHeadRuleService salaryService, IDropdownBindService dropdownBindService)
        {
            this.salaryService = salaryService;
            this.dropdownBindService = dropdownBindService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SanctionDetailsGridView()
        {
            log.Info($"SanctionLoanController/SanctionDetailsGridView");
            try
            {
                Model.SanctionLoan sanctionLoan = new Model.SanctionLoan();
                sanctionLoan.SanctionLoanList = salaryService.GetSanctionLoanList();
                return PartialView("_SanctionDetailsGridView", sanctionLoan);
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
            log.Info("SanctionLoanController/Create");
            try
            {
                Model.SanctionLoan objSanctionLoan = new Model.SanctionLoan();
                objSanctionLoan.EmployeeList = dropdownBindService.GetAllEmployee();
                objSanctionLoan.EmployeeList.Add(new Model.SelectListModel { id = 0, value = "Select" });
                objSanctionLoan.LoanTypeList = dropdownBindService.GetLoanType();
                return View(objSanctionLoan);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult GetEmployeeByPFNumber(int PFNumber)
        {
            log.Info($"SanctionLoanController/GetEmployeeByPFNumber{PFNumber}");
            try
            {
                var employeeDetail = dropdownBindService.GetEmployeeByPFNumber(PFNumber);
                return Json(new { htmlData = employeeDetail }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetLoanTypeDetails(int LoanTypeID)
        {
            log.Info($"SanctionLoanController/GetLoanTypeDetails{LoanTypeID}");
            try
            {
                var loanTypeDetail = salaryService.GetLoanTypeDetails(LoanTypeID);
                return Json(new { htmlData = loanTypeDetail }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetLoanNumberDetails(int LoanTypeID, string EmpCode)
        {
            log.Info($"SanctionLoanController/GetLoanNumberDetails/{LoanTypeID}/{EmpCode}");
            try
            {
                var loanTypeDetail = salaryService.GetLoanNumberDetails(LoanTypeID, EmpCode);
                return Json(new { htmlData = loanTypeDetail }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.SanctionLoan sanctionLoan)
        {
            log.Info("SanctionLoanController/Create");
            try
            {
                if (sanctionLoan.DateRcptApp < sanctionLoan.DateofApp)
                    ModelState.AddModelError("DateRcptAppValidation", "Date of reciept application must be after than date of Application");
                if (sanctionLoan.ReqAmt > sanctionLoan.MaxLoanAmt)
                    ModelState.AddModelError("ReqAmtValidation", "You are requesting more amount than the maximum limit of this loan");
                if (sanctionLoan.DateofSanc < sanctionLoan.DateRcptApp)
                    ModelState.AddModelError("DateofSancValidation", "Date of Sanction of Loan must be after than Date of Reciept Application");
               // if (sanctionLoan.DateofSanc < DateTime.Now.AddDays(-90))
                 //   ModelState.AddModelError("DateofSancValidation", "You cannot enter date less than three month back date of current date!");
                if (sanctionLoan.SancAmt > sanctionLoan.ReqAmt)
                    ModelState.AddModelError("SancAmtValidation", "Sanction Amount is more than Requested amount");
                if (sanctionLoan.SancAmt > sanctionLoan.MaxLoanAmt)
                    ModelState.AddModelError("SancAmtValidation", "Sanction amount should not greater than Maximum Amount For this Loan");
                if (sanctionLoan.DateAvailLoan < sanctionLoan.DateofSanc)
                    ModelState.AddModelError("DateAvailLoanValidation", "Date of availing the loan should be greater than date of Sanction of Loan");
                if (sanctionLoan.OriginalPinstAmt == 0)
                    ModelState.AddModelError("OriginalPInstAmtValidation", "Original Principal Installment Amount must be greater than zero.");



                if (ModelState.IsValid)
                {
                    var branchDetails = dropdownBindService.GetBranchByEmployeeId(sanctionLoan.EmployeeId.Value);
                    sanctionLoan.BranchId = branchDetails.id;
                    sanctionLoan.Branchcode = branchDetails.value;
                    sanctionLoan.IsNewLoanAfterDevelop = true;
                    sanctionLoan.Status = false;
                    sanctionLoan.CreatedBy = userDetail.UserID;
                    sanctionLoan.CreatedOn = DateTime.Now;
                    var flag = salaryService.SaveSanctionLoan(sanctionLoan);
                    TempData["Message"] = "Successfully Created";
                    return RedirectToAction("Index");
                }
                else
                {
                    sanctionLoan.IsModelValid = false;
                    sanctionLoan.EmployeeList = dropdownBindService.GetAllEmployee();
                    sanctionLoan.EmployeeList.Add(new Model.SelectListModel { id = 0, value = "Select" });
                    sanctionLoan.LoanTypeList = dropdownBindService.GetLoanType();
                    sanctionLoan.LoanTypeId = sanctionLoan.AssignLoanTypeId;
                    return View(sanctionLoan);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult Edit(string priorityNo, int mstLoanID)
        {
            log.Info("SanctionLoanController/Edit");
            try
            {
                Model.SanctionLoan objSanctionLoan = new Model.SanctionLoan();
                objSanctionLoan = salaryService.GetSanctionLoanByID(priorityNo, mstLoanID);
                objSanctionLoan.EmployeeList = dropdownBindService.GetAllEmployee();
                objSanctionLoan.EmployeeList.Add(new Model.SelectListModel { id = 0, value = "Select" });
                objSanctionLoan.LoanTypeList = dropdownBindService.GetLoanType();
                return View(objSanctionLoan);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.SanctionLoan sanctionLoan)
        {
            log.Info("SanctionLoanController/Edit");
            try
            {
                ModelState.Remove("DateofApp");
                ModelState.Remove("DateRcptApp");
                ModelState.Remove("ReqAmt");
                if (ModelState.IsValid)
                {
                    sanctionLoan.UpdatedBy = userDetail.UserID;
                    sanctionLoan.UpdatedOn = DateTime.Now;
                    salaryService.UpdateSanctionLoan(sanctionLoan);
                    TempData["Message"] = "Successfully Updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    sanctionLoan.IsModelValid = false;
                    sanctionLoan.EmployeeList = dropdownBindService.GetAllEmployee();
                    sanctionLoan.EmployeeList.Add(new Model.SelectListModel { id = 0, value = "Select" });
                    sanctionLoan.LoanTypeList = dropdownBindService.GetLoanType();
                    sanctionLoan.LoanTypeId = sanctionLoan.AssignLoanTypeId;
                    return View(sanctionLoan);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            //return View(sanctionLoan);
        }

        public ActionResult DeleteSanction(string priorityNo,  int mstLoanID)
        {
            log.Info("DeleteSanction");
            try
            {

               var flag = salaryService.DeleteSanction(priorityNo, mstLoanID,userDetail.UserID);
                if(flag)
                TempData["Message"] = "Successfully Deleted";
                else
                    TempData["Error"] = "Deletion failed!";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

        public ActionResult CalculateLoanInterest(string dateOfSanction, decimal sanctionAmnt, decimal rOI)
        {
            log.Info($"SanctionLoanController/CalculateLoanInterest/{dateOfSanction}/{sanctionAmnt}/{rOI}");
            try
            {
                var sanctionDate = Convert.ToDateTime(dateOfSanction);
                int mon = Convert.ToDateTime(dateOfSanction).Month;
                int yr = Convert.ToDateTime(dateOfSanction).Year;
                decimal balanceamt = sanctionAmnt;
                int totalMonth = 25;
                decimal actualinterest;
                decimal[] calarray = new decimal[25];
                string[] monarray = new string[25];
                decimal[] balanceamtarray = new decimal[25];
                int daysinmonth = 0;
                for (int i = 0; i < totalMonth - 1; i++)
                {
                    daysinmonth = DateTime.DaysInMonth(yr, mon);
                    if ((sanctionDate.Month == sanctionDate.AddMonths(25).Month) && (sanctionDate.Year == sanctionDate.AddYears(2).Year))
                    {

                    }
                    else
                    {
                        if (i == 0)
                        {
                            daysinmonth = (daysinmonth - sanctionDate.Day) + 1;
                            actualinterest = 0;
                            actualinterest = (sanctionAmnt * rOI) * daysinmonth / 36500;
                            monarray[i] = (i + 1).ToString() + " Month";
                            calarray[i] = actualinterest;
                            balanceamtarray[i] = sanctionAmnt;
                        }
                        else if (i == totalMonth - 1)
                        {
                            daysinmonth = DateTime.DaysInMonth(sanctionDate.AddMonths(25).Year, sanctionDate.AddMonths(25).Month);
                            actualinterest = 0;
                            actualinterest = (balanceamt * rOI) * daysinmonth / 36500;
                            monarray[i] = (i + 1).ToString() + " Month";
                            calarray[i] = actualinterest;
                            balanceamtarray[i] = balanceamt;
                        }
                        else
                        {
                            actualinterest = 0;
                            actualinterest = (balanceamt * rOI) * daysinmonth / 36500;
                            calarray[i] = actualinterest;
                            monarray[i] = (i + 1).ToString() + " Month";
                            balanceamtarray[i] = balanceamt;
                            balanceamt = balanceamt - (sanctionAmnt / 24);
                        }
                    }

                    if (mon == 12)
                    {
                        yr = yr + 1;
                        mon = 1;
                    }
                    else
                    {
                        mon = mon + 1;
                    }
                }

                decimal totalactual = 0;
                for (int j = 0; j < calarray.Length - 1; j++)
                {
                    totalactual = totalactual + calarray[j];
                }
                return Json(new { CalculateLoanInterest = totalactual }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}