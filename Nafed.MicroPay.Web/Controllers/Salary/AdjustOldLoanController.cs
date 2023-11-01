using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using MicroPay.Web.Models;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using Nafed.MicroPay.Services.Increment;
using System.Data;
using Nafed.MicroPay.Services.Salary;

namespace MicroPay.Web.Controllers.Salary
{
    public class AdjustOldLoanController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IAdjustOldLoanService adjustOldLoanService;
        public AdjustOldLoanController(IDropdownBindService ddlService, IAdjustOldLoanService adjustOldLoanService)
        {
            this.ddlService = ddlService;
            this.adjustOldLoanService = adjustOldLoanService;
        }
        public ActionResult Index()
        {
            log.Info("AdjustOldLoanController/Index");
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

        public ActionResult GetEmployeeByLoanType(int loanTypeId, bool oldLoanEmployee)
        {
            log.Info("AdjustOldLoanController/GetEmployeeByLoanType");
            try
            {
                var result = adjustOldLoanService.GetEmployeeByLoanType(loanTypeId, oldLoanEmployee);
                TempData["EmployeeByLoanType"] = result;
                return Json(new { employees = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _AdjustLoanContent()
        {
            log.Info("AdjustOldLoanController/_AdjustLoanContent");
            try
            {
                Model.SanctionLoan sanctionLoan = new Model.SanctionLoan();
                sanctionLoan.EmployeeList = new List<Model.SelectListModel>();
                sanctionLoan.LoanTypeList = ddlService.GetLoanType();
                return PartialView("_AdjustOldLoanDetails", sanctionLoan);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetAdjustLoanDetails(int loanTypeId, int employeeId, bool showOld)
        {
            log.Info("AdjustOldLoanController/GetAdjustLoanDetails");
            try
            {
                Model.SanctionLoan sanctionLoan = new Model.SanctionLoan();
                List<Model.SanctionLoan> listSanctionLoan = new List<Model.SanctionLoan>();
                sanctionLoan.OldLoanEmployee = showOld;
                if (ModelState.IsValid)
                {
                    var loanAdjustResult = adjustOldLoanService.GetAdjustLoanOldDetails(employeeId, loanTypeId, showOld, out listSanctionLoan);

                    if (loanAdjustResult != null)
                    {
                        loanAdjustResult.RemainingPInstNo1 = loanAdjustResult.RemainingPInstNo;
                        sanctionLoan = loanAdjustResult;
                    }
                    sanctionLoan.LoanTypeList = ddlService.GetLoanType();
                    sanctionLoan.SanctionLoanList = listSanctionLoan;
                    if (TempData["EmployeeByLoanType"] != null)
                    {
                        sanctionLoan.EmployeeList = (List<Model.SelectListModel>)TempData["EmployeeByLoanType"];
                        TempData.Keep("EmployeeByLoanType");
                    }

                    return Json(new { msgType = "success", htmlData = ConvertViewToString("_AdjustOldLoanDetails", sanctionLoan) }, JsonRequestBehavior.AllowGet);
                    //return PartialView("_AdjustOldLoanDetails", sanctionLoan);
                }
                else
                {
                    if (TempData["EmployeeByLoanType"] != null)
                    {
                        sanctionLoan.EmployeeList = (List<Model.SelectListModel>)TempData["EmployeeByLoanType"];
                        TempData.Keep("EmployeeByLoanType");
                    }
                    else
                        sanctionLoan.EmployeeList = new List<Model.SelectListModel>();
                    sanctionLoan.LoanTypeList = ddlService.GetLoanType();
                    return PartialView("_AdjustOldLoanDetails", sanctionLoan);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult SaveAdjustOldLoanDetails(Model.SanctionLoan sanctionLoan, string buttonType)
        {
            log.Info("AdjustOldLoanController/SaveAdjustOldLoanDetails");
            try
            {
                bool flag = false;
                if (TempData["EmployeeByLoanType"] != null)
                {
                    sanctionLoan.EmployeeList = (List<Model.SelectListModel>)TempData["EmployeeByLoanType"];
                    TempData.Keep("EmployeeByLoanType");
                }
                else
                    sanctionLoan.EmployeeList = new List<Model.SelectListModel>();
                sanctionLoan.LoanTypeList = ddlService.GetLoanType();
                sanctionLoan.RemainingPInstNo1 = sanctionLoan.RemainingPInstNo;
                if (buttonType == "Save")
                {
                    int RemainingInst = 1;
                    bool status = false;
                    int remain = 0;
                    int last = 0;

                    sanctionLoan.CreatedBy = userDetail.UserID;
                    sanctionLoan.CreatedOn = DateTime.Now;
                    if (sanctionLoan.sanctionLoanRadio == Model.SanctionLoanRadio.TotalRefund || sanctionLoan.sanctionLoanRadio == Model.SanctionLoanRadio.LastInstallAdjust || sanctionLoan.sanctionLoanRadio == Model.SanctionLoanRadio.PartialRefund)
                    {
                        sanctionLoan.BalancePAmt = sanctionLoan.BalancePAmt == null ? 0 : sanctionLoan.BalancePAmt;
                        sanctionLoan.BalanceIAmt = sanctionLoan.BalanceIAmt == null ? 0 : sanctionLoan.BalanceIAmt;
                        if (Convert.ToDouble(sanctionLoan.BalancePAmt.Value) == 0.00)
                            ModelState.AddModelError("IInstallmentAmount", "Please insert Valid Value.");
                        if (sanctionLoan.LoanTypeId == 1 && Convert.ToDouble(sanctionLoan.BalanceIAmt.Value) == 0.00)
                            ModelState.AddModelError("InterestAmount", "Please insert Valid Value.");
                        if (sanctionLoan.RefundDate == null && !(sanctionLoan.sanctionLoanRadio == Model.SanctionLoanRadio.LastInstallAdjust))
                            ModelState.AddModelError("RefundDate", "Please insert Valid Value.");
                    }
                    if (sanctionLoan.sanctionLoanRadio == Model.SanctionLoanRadio.TotalRefund || sanctionLoan.sanctionLoanRadio == Model.SanctionLoanRadio.PartialRefund)
                    {
                        if (sanctionLoan.totlRefundMonthId == 0)
                            ModelState.AddModelError("RefundMonth", "Please select month");
                        if (sanctionLoan.totlRefundYearId == 0)
                            ModelState.AddModelError("RefundYear", "Please select year");
                    }
                    ModelState.Remove("DateofApp");
                    ModelState.Remove("DateRcptApp");
                    ModelState.Remove("ReqAmt");
                    ModelState.Remove("AssignLoanTypeId");                  
                    if (ModelState.IsValid)
                    {
                        if (sanctionLoan.RemainingPInstNo != null)
                        {
                            if (RemainingInst > 0)
                            {
                                remain = (RemainingInst - (sanctionLoan.RemainingPInstNo == null ? 0 : sanctionLoan.RemainingPInstNo.Value));
                                last = ((sanctionLoan.LastPaidPInstNo == null ? 0 : sanctionLoan.LastPaidPInstNo.Value) - remain);
                                if (sanctionLoan.RemainingPInstNo.Value > 0)
                                    status = false;
                                else
                                    status = true;
                            }
                            else if (RemainingInst == 0)
                            {
                                last = ((sanctionLoan.LastPaidIInstNo == null ? 0 : sanctionLoan.LastPaidIInstNo.Value) + 1);
                                if ((sanctionLoan.RemainingIInstNo == null ? 0 : sanctionLoan.RemainingIInstNo.Value) > 0)
                                    status = false;
                                else
                                    status = true;
                            }
                        }
                        sanctionLoan.Status = status;
                        if (sanctionLoan.sanctionLoanRadio == Model.SanctionLoanRadio.TotalRefund)
                        {
                            flag = adjustOldLoanService.UpdateTotalRefundDetails(sanctionLoan);
                            return Json(new { msgType = "success", msg = "Saved successfully", htmlData = ConvertViewToString("_AdjustOldLoanDetails", sanctionLoan) }, JsonRequestBehavior.AllowGet);
                        }
                        else if (sanctionLoan.sanctionLoanRadio == Model.SanctionLoanRadio.LastInstallAdjust)
                        {
                            flag = adjustOldLoanService.UpdateLastInstallAdjust(sanctionLoan);
                            return Json(new { msgType = "success", msg = "Saved successfully", htmlData = ConvertViewToString("_AdjustOldLoanDetails", sanctionLoan) }, JsonRequestBehavior.AllowGet);
                        }
                        else if (sanctionLoan.sanctionLoanRadio == Model.SanctionLoanRadio.InstallmentAdjestment)
                        {
                            flag = adjustOldLoanService.UpdateInstallmentAdjestment(sanctionLoan);
                            return Json(new { msgType = "success", msg = "Saved successfully", htmlData = ConvertViewToString("_AdjustOldLoanDetails", sanctionLoan) }, JsonRequestBehavior.AllowGet);
                        }
                        else if (sanctionLoan.sanctionLoanRadio == Model.SanctionLoanRadio.LoanFinish)
                        {
                            flag = adjustOldLoanService.UpdateLoanFinish(sanctionLoan);
                            return Json(new { msgType = "success", msg = "Saved successfully", htmlData = ConvertViewToString("_AdjustOldLoanDetails", sanctionLoan) }, JsonRequestBehavior.AllowGet);
                        }
                        else if (sanctionLoan.sanctionLoanRadio == Model.SanctionLoanRadio.PartialRefund)
                        {
                            flag = adjustOldLoanService.UpdatePartialRefund(sanctionLoan);
                            return Json(new { msgType = "success", msg = "Saved successfully", htmlData = ConvertViewToString("_AdjustOldLoanDetails", sanctionLoan) }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { msgType = "confirm", msg = "If you want to make new Installments(For Interest Installments) then press 'OK'.If you want to change the Remaining Installments only then press 'Cancel'", htmlData = ConvertViewToString("_AdjustOldLoanDetails", sanctionLoan) }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { msgType = "error", htmlData = ConvertViewToString("_AdjustOldLoanDetails", sanctionLoan) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    flag = adjustOldLoanService.UpdateAllLastInstallment(sanctionLoan.LoanTypeId);
                    return Json(new { msgType = "success", msg = "Last Installment Successfully updated.", htmlData = ConvertViewToString("_AdjustOldLoanDetails", sanctionLoan) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult UpdateLoanpriority(string priority, decimal remainingInstallments, decimal installmentAmt, decimal intInstallment, decimal intInstallmentAmount, bool status, bool flag)
        {
            log.Info("AdjustOldLoanController/UpdateLoanpriority");
            try
            {
                bool update = adjustOldLoanService.UpdateLoanpriority(priority, remainingInstallments, installmentAmt, intInstallment, intInstallmentAmount, status, flag);
                return Json(new { msgType = "success", msg = "Saved successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}