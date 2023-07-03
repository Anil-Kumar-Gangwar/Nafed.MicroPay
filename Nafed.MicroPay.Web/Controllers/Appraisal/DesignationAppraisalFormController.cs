using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using MicroPay.Web.Models;


namespace MicroPay.Web.Controllers.Appraisal 
{
    public class DesignationAppraisalFormController : BaseController
    {

        private readonly IDropdownBindService dropdownService;
        private readonly IAppraisalFormService appraisalFormService;
        public DesignationAppraisalFormController(IDropdownBindService dropdownService, IAppraisalFormService appraisalFormService)
        {
            this.appraisalFormService = appraisalFormService;
            this.dropdownService = dropdownService;
        }
        // GET: DesignationAppraisalForm
        public ActionResult Index()
        {
            log.Info($"DesignationAppraisalFormController/Index");
            try
            {
                DesignationAppraisalFormVM dAppraisalFormVM = new DesignationAppraisalFormVM();
                //ViewBag.AppraisalForms = dropdownService.ddlAppraisalForms();
                dAppraisalFormVM.appraisalForms = appraisalFormService.GetAppraisalForms();
                dAppraisalFormVM.designations = dropdownService.ddlDesignationList();
                return View(dAppraisalFormVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult InsertUpdateDesignationAppraisalForm(DesignationAppraisalFormVM desiAppraisalVM)
        {
            log.Info($"DesignationAppraisalFormController/Index");

            var selectedDesignations = Request.Form["SelectedDesignation"];

            List<int> designations = new List<int>();

            if (!string.IsNullOrEmpty(selectedDesignations))
            {
                if (ModelState.IsValid)
                {
                    var designationIds = (selectedDesignations.Split(new char[] { ',' })).Select(int.Parse).ToList();

                    bool res = appraisalFormService.InsertUpdateDesignationApprasialForm((int)desiAppraisalVM.appraisalFormID, designationIds, userDetail.UserID);
                    if (res)
                        TempData["Message"] = " Record(s) updated successfully.";
                    else TempData["Error"] = " Record(s) updation failed.";
                }
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult LinkedDesignations(int formID)
        {
            log.Info($"DesignationAppraisalFormController/LinkedDesignations/{formID}");
            try
            {
                var selectedValues = appraisalFormService.LinkedDesignations(formID);
           
                return Json(new { selectedValues = selectedValues.Select(x=>new { _id=x }).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}