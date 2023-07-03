using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using System;

namespace MicroPay.Web.Controllers
{
    public class DesignationAssignmentController : BaseController
    {
        // GET: DesignationAssignment
        private readonly IEmployeeService employeeService;
        private readonly IDropdownBindService dropDownService;
        private readonly IDesignationService designationService;
        public DesignationAssignmentController(IDropdownBindService dropDownService, IDesignationService designationService, IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
            this.dropDownService = dropDownService;
            this.designationService = designationService;
        }
        public ActionResult Index(int? employeeID)
        {
            log.Info($"DesignationAssignmentController/Index");
            DesignationAssignmentVM desiAssignmentVM = new DesignationAssignmentVM();
            var res = employeeService.GetDesignationAssignation(employeeID);
            desiAssignmentVM.designationAssignment = res;
            return View(desiAssignmentVM);
        }

        [HttpGet]
        public ActionResult ChangeDesignation(int? employeeID, string employeeCode, int? currentCadreID, int? transID)
        {
            log.Info($"DesignationAssignmentController/ChangeDesignation/{employeeID}/{transID}");
            ModelState.Clear();
            Promotion promotionData = new Promotion();
            promotionData.EmployeeID = employeeID;
            promotionData.EmployeeCode = employeeCode;
            if (employeeID.HasValue && transID.HasValue)
            {
                promotionData = employeeService.GetPromotionForm(employeeID.Value, transID);
                promotionData.FormActionType = "Update";
                promotionData.wayOfPostingEnum = (WayOfPosting)(promotionData.WayOfPostingID ?? 0);
                promotionData.designationList = currentCadreID.HasValue ? designationService.GetDesignationByCadre(currentCadreID.Value) : new System.Collections.Generic.List<SelectListModel>();
            }
            else
                promotionData.CadreID = currentCadreID;
            promotionData.cadreList = dropDownService.ddlCadreList();
            return PartialView("_PromotionForm", promotionData);
        }

        [HttpPost]
        public ActionResult ChangeDesignation(Promotion frmData)
        {
            log.Info($"DesignationAssignmentController/ChangeDesignation/");
            // var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
            frmData.cadreList = dropDownService.ddlCadreList();
            // var cadr = Request.Form["CadreID"];

            if (!frmData.CadreID.HasValue)
                frmData.CadreID = Convert.ToInt32(Request.Form["CadreID"].ToString());

            frmData.designationList = designationService.GetDesignationByCadre(frmData.CadreID.Value);

            if (ModelState.IsValid)
            {
                if (frmData.CreatedBy == 0)
                {
                    frmData.CreatedBy = userDetail.UserID;
                    frmData.CreatedOn = DateTime.Now;
                    frmData.FormActionType = "Create";

                    //if (!frmData.OrderOfPromotion.HasValue)
                    //{
                    //    ModelState.AddModelError("OrderOfPromotion", "Please enter Order Date of Promotion.");
                    //    return PartialView("_PromotionForm", frmData);
                    //}

                    if (!designationService.CheckConfrmChildHasRecord((int)frmData.EmployeeID, (int)Nafed.MicroPay.Common.WorkFlowProcess.PromotionConfirmation))
                    {
                        ModelState.AddModelError("NoRecordInConfChild", "Approval process not set for this employee for Promotion, you can set approval process from 'Manage -> Employee -> Confirmation Approval' menu.");
                        return PartialView("_PromotionForm", frmData);
                    }

                    //var promotionYear = frmData.FromDate.Value.Year;
                    //var year = Convert.ToString(promotionYear) + "-" + Convert.ToString(Convert.ToString(promotionYear + 1).Substring(2, 2));
                    //if (!employeeService.IsStaffBudgetAvailable(frmData.DesignationID.Value, year, "P"))
                    //{
                    //    TempData["message"] = "There is no any vacant post for the selected designation for this year.";                      
                    //    return PartialView("_PromotionForm", frmData);
                    //}
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
                    return PartialView("_PromotionForm", frmData);
                }
                if (frmData.ConfirmationDate.HasValue && frmData.FromDate.Value > frmData.ConfirmationDate.Value)
                {
                    ModelState.AddModelError("ConfirmationDateRangeValidation", "Confirmation Date cannot less than From Date.");
                    return PartialView("_PromotionForm", frmData);
                }
                frmData.WayOfPostingID = (int)frmData.wayOfPostingEnum;
                // frmData.New = (int)frmData.cota;
                frmData = designationService.ChangeDesignation(frmData);

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
                frmData.cadreList = dropDownService.ddlCadreList();
                frmData.designationList = dropDownService.ddlDesignationList();
            }
            return PartialView("_PromotionForm", frmData);
        }

        public JsonResult GetDesignationByCadre(int cadreID)
        {
            log.Info($"DesignationAssignmentController/GetDesignationByCadre/{cadreID}");
            try
            {
                var designations = designationService.GetDesignationByCadre(cadreID);
                designations.Insert(0, new SelectListModel { id = 0, value = "Select" });
                return Json(new { designationList = new SelectList(designations, "id", "value") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetSeniorityCode(int designationID)
        {
            log.Info($"DesignationAssignmentController/GetSeniorityCode/{designationID}");
            try
            {
                var sen_code = employeeService.GetSeniorityCode(designationID);
                return Json(new { sen_code = sen_code }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult Delete(int employeeID, int transID)
        {
            log.Info($"DesignationAssignmentController/Delete/{employeeID}/{transID}");

            try
            {
                var flag = designationService.DeletePromotionTransEntry(transID);
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