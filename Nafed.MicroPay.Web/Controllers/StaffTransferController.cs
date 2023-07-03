using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using System;
using System.Linq;

namespace MicroPay.Web.Controllers
{
    public class StaffTransferController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IDropdownBindService dropDownService;

        public StaffTransferController(IDropdownBindService dropDownService, IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
            this.dropDownService = dropDownService;
        }

        public ActionResult Index(int? employeeID)
        {
            log.Info($"StaffTransferController/Index");
            try
            {
                StaffTransferViewModel staffTransferVM = new StaffTransferViewModel();
                var res = employeeService.GetStaffTransfer(employeeID);
                staffTransferVM.staffTransfer = res;
                return View(staffTransferVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult ChangeBranch(int? employeeID, string employeeCode, int? transId)
        {
            log.Info($"StaffTransferController/ChangeBranch");
            try
            {
                ModelState.Clear();
                Transfer transferData = new Transfer();
                transferData.EmployeeId = employeeID;
                transferData.EmployeeCode = employeeCode;
                if (employeeID.HasValue && transId.HasValue)
                {
                    transferData = employeeService.GetStaffTransferForm(employeeID.Value, transId);
                    transferData.FormActionType = "Update";
                    transferData.branchList = dropDownService.ddlBranchList();
                }
                else
                    transferData.branchList = dropDownService.ddlBranchList();

                return PartialView("_StaffTransferForm", transferData);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ChangeBranch(Transfer frmData)
        {
            log.Info($"StaffTransferController/ChangeBranch/");

            frmData.branchList = dropDownService.ddlBranchList();

            if (ModelState.IsValid)
            {
                if (frmData.CreatedBy == 0)
                {
                    frmData.CreatedBy = userDetail.UserID;
                    frmData.CreatedOn = DateTime.Now;
                    frmData.FormActionType = "Create";

                }
                else
                {
                    frmData.UpdatedBy = userDetail.UserID;
                    frmData.UpdatedOn = DateTime.Now;
                    frmData.FormActionType = "Update";
                }
                if (frmData.ToDate.HasValue && frmData.FromDate.Value > frmData.ToDate.Value)
                {
                    ModelState.AddModelError("ToDateRangeValidation", "To Date cannot less than From Date.");
                    return PartialView("_StaffTransferForm", frmData);
                }
                // frmData.New = (int)frmData.cota;
                frmData = employeeService.ChangeStaffBranch(frmData);

                if (frmData.Saved)
                {
                    if (frmData.FormActionType == "Create")
                        TempData["message"] = "Successfully Saved";
                    else
                        TempData["message"] = "Successfully updated.";
                    return Json(new { saved = frmData.Saved }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                frmData.branchList = dropDownService.ddlBranchList();
            }
            return PartialView("_StaffTransferForm", frmData);
        }

        public ActionResult Delete(int employeeID, int transID)
        {
            log.Info($"DesignationAssignmentController/Delete/{transID}");
            try
            {
                var flag = employeeService.DeleteStaffTransferEntry(transID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return RedirectToAction("Index", new { @employeeID = employeeID });
        }
    }
}