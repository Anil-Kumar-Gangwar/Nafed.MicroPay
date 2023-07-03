using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.Budget;


namespace MicroPay.Web.Controllers.Budget
{
    public class ConfigureBudgetController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IBudgetService budgetService;

        public ConfigureBudgetController(IDropdownBindService ddlService, IBudgetService budgetService)
        {
            this.ddlService = ddlService;
            this.budgetService = budgetService;
        }
        public ActionResult Index()
        {
            log.Info($"ConfigureBudgetController/Index");
            return View(userAccessRight);
        }

        public ActionResult ConfigureBudgetDetails()
        {
            log.Info($"ConfigureBudgetController/ConfigureBudgetDetails");
            try
            {
                BindDropdowns();
                Model.PromotionCota promotionCota = new Model.PromotionCota();
                return PartialView("_ConfigureBudgetDetails", promotionCota);
            }
            catch(Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public void BindDropdowns()
        {
            var ddlDesignationList = ddlService.ddlDesignationList();
            Model.SelectListModel selectDesignation = new Model.SelectListModel();
            selectDesignation.id = 0;
            selectDesignation.value = "Select";
            ddlDesignationList.Insert(0, selectDesignation);
            ViewBag.DesignationList = new SelectList(ddlDesignationList, "id", "value");
        }

        [HttpPost]
        public ActionResult UpdateConfigureBudget(Model.PromotionCota promotionCota)
        {
            log.Info($"ConfigureBudgetController/UpdateConfigureBudget");
            try
            {
                BindDropdowns();
                if (ModelState.IsValid)
                {
                    promotionCota.UpdatedBy = userDetail.UserID;
                    promotionCota.Promotion = promotionCota.Promotion == null ? 0 : promotionCota.Promotion;
                    promotionCota.Direct = promotionCota.Direct == null ? 0 : promotionCota.Direct;
                    promotionCota.LCT = promotionCota.LCT == null ? 0 : promotionCota.LCT;
                    promotionCota.NPromotion = promotionCota.NPromotion == null ? 0 : promotionCota.NPromotion;
                    promotionCota.NDirect = promotionCota.NDirect == null ? 0 : promotionCota.NDirect;
                    promotionCota.NLCT = promotionCota.NLCT == null ? 0 : promotionCota.NLCT;
                    var promotionCotaDetails = budgetService.UpdatePromotionDetails(promotionCota);
                    if (promotionCotaDetails)
                        ViewBag.Message = "Updated Successfully";
                }
                return PartialView("_ConfigureBudgetDetails", promotionCota);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult GetConfigureBudgetDetails(int designationID)
        {
            log.Info($"ConfigureBudgetController/GetConfigureBudgetDetails");
            try
            {
                BindDropdowns();
                var promotionCota = budgetService.GetPromotionDetails(designationID);
                return PartialView("_ConfigureBudgetDetails", promotionCota);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}