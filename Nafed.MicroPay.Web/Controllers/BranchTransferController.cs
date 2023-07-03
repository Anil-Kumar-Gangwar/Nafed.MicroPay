using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using System;

namespace MicroPay.Web.Controllers
{
    public class BranchTransferController : BaseController
    {
        // GET: BranchTransfer
        private readonly IBranchService branchService;
        private readonly IDropdownBindService dropDownService;

        public BranchTransferController(IBranchService branchService, IDropdownBindService dropDownService)
        {
            this.branchService = branchService;
            this.dropDownService = dropDownService;
        }
        public ActionResult Index(int? employeeID)
        {
            log.Info($"BranchTransferController/Index");
            BranchTransferViewModel branchTransferVM = new BranchTransferViewModel();
            var res = branchService.GetBranchTransferDtls(employeeID);
            branchTransferVM.branchTransfer = res;
            return View(branchTransferVM);
        }

        [HttpGet]
        public ActionResult ChangeBranch(int? employeeID, string employeeCode, int? transID)
        {
            log.Info($"BranchTransferController/ChangeBranch/{employeeID}/{transID}");
            ModelState.Clear();
            BranchManagerDetails branchDetail = new BranchManagerDetails();
            branchDetail.EmployeeID = employeeID;
            branchDetail.EmployeeCode = employeeCode;
            if (employeeID.HasValue && transID.HasValue)
            {
                branchDetail = branchService.GetBranchTransferForm(employeeID.Value, transID);
                branchDetail.FormActionType = "Update";
                // branchDetail.wayOfPostingEnum = (WayOfPosting)(promotionData.WayOfPostingID ?? 0);
                branchDetail.Id = transID.Value;
                branchDetail.branchList = dropDownService.ddlBranchList();
            }
            else
                // branchDetail.CadreID = currentCadreID;
                branchDetail.branchList = dropDownService.ddlBranchList();
            return PartialView("_BranchTransferForm", branchDetail);
        }

        [HttpPost]
        public ActionResult ChangeBranch(BranchManagerDetails branchDtl)
        {
            log.Info($"BranchTransferController/ChangeBranch/");
            branchDtl.branchList = dropDownService.ddlBranchList();
            if (ModelState.IsValid)
            {
                if (branchDtl.CreatedBy == 0)
                {
                    branchDtl.CreatedBy = userDetail.UserID;
                    branchDtl.CreatedOn = DateTime.Now;
                    branchDtl.FormActionType = "Create";
                }
                else
                {
                    branchDtl.UpdatedBy = userDetail.UserID;
                    branchDtl.UpdatedOn = DateTime.Now;
                    branchDtl.FormActionType = "Update";
                }
                if (branchDtl.DateTo.HasValue && branchDtl.DateFrom.Value > branchDtl.DateTo.Value)
                {
                    ModelState.AddModelError("ToDateRangeValidation", "To Date cannot less than From Date.");
                    return PartialView("_BranchTransferForm", branchDtl);
                }
                 branchDtl = branchService.ChangeBranch(branchDtl);

                if (branchDtl.Saved)
                {
                    if (branchDtl.FormActionType == "Create")
                        TempData["message"] = "Successfully Saved";
                    else
                        TempData["message"] = "Successfully updated.";
                    return Json(new { saved = branchDtl.Saved }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                branchDtl.branchList = dropDownService.ddlBranchList();
            }
            return PartialView("_BranchTransferForm", branchDtl);
        }

        public ActionResult Delete(int employeeID, int transID)
        {
            log.Info($"BranchTransferController/Delete/{employeeID}/{transID}");

            try
            {
                var flag = branchService.DeleteBranchTransEntry(transID);
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