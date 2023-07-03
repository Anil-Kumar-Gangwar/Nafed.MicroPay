using System;
using System.Linq;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using static Nafed.MicroPay.Common.ExtensionMethods;
using Nafed.MicroPay.Services.TDS;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers.TDS
{
    public class TDSTaxRuleSlabsController : BaseController
    {
        private readonly ITDSTaxRuleSlabService tdsTaxRuleService;

        public TDSTaxRuleSlabsController(ITDSTaxRuleSlabService tdsTaxRuleService)
        {
            this.tdsTaxRuleService = tdsTaxRuleService;
        }

        // GET: TDSTaxRuleSlabs
        public ActionResult Index()
        {
            log.Info($"TDSTaxRuleSlabsController/Index");

            TDSTaxRuleSlabsVM model = new TDSTaxRuleSlabsVM();
            model.userRights = userAccessRight;
            var financialYear = GetYearBetweenYearsList(2005, 2044);
            model.fYList = financialYear.Select(x => new SelectListModel { id = 0, value = x }).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult _GetTaxRuleSlabs(TDSTaxRuleSlabsVM filter)
        {
            log.Info($"TDSTaxRuleSlabs/_GetTaxRuleSlabs");
            try
            {
                TDSTaxRuleSlabsVM taxRuleVM = new TDSTaxRuleSlabsVM();
                var taxSlabList = tdsTaxRuleService.GetTaxRuleSlabs(filter.selectedFyear);
                taxRuleVM.fYrTdsSlabs = taxSlabList;
                taxRuleVM.userRights = userAccessRight;
                return PartialView("_FinancialTaxRuleSlabs", taxRuleVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult GetTaxRuleSlabForm(short? view, string fYear)
        {
            log.Info($"TDSTaxRuleSlabs/GetTaxRuleSlabForm/fYear:{fYear}");
            try
            {
                tblTDSTaxRulesSlab tdsTaxRuleSlab = new tblTDSTaxRulesSlab();

                if (!string.IsNullOrEmpty(fYear))
                {
                    tdsTaxRuleSlab = tdsTaxRuleService.GetTaxRuleSlabs(fYear).FirstOrDefault();
                }
                var financialYear = GetYearBetweenYearsList(2005, 2044);
                tdsTaxRuleSlab.fYList = financialYear.Select(x => new SelectListModel { id = 0, value = x }).ToList();

                return View("TDSTaxRuleSlabs", tdsTaxRuleSlab);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostTDSSlabForm(tblTDSTaxRulesSlab tdsSlabs)
        {
            log.Info($"TDSTaxRuleSlabs/_PostTDSSlabForm");

            try
            {
                if (tdsSlabs.CreatedBy == 0 && tdsTaxRuleService.IsTaxRuleSlabExists(tdsSlabs.FinancialYear))
                    ModelState.AddModelError("TdsRuleSlabExists", "Tax Rule Slabs already defined.");

                if (ModelState.IsValid)
                {
                    if (tdsSlabs.CreatedBy == 0)
                    {
                        tdsSlabs.CreatedBy = userDetail.UserID;
                        tdsSlabs.CreatedOn = DateTime.Now;
                    }
                    else
                    {
                        tdsSlabs.UpdatedBy = userDetail.UserID;
                        tdsSlabs.UpdatedOn = DateTime.Now;
                    }
                    var result = tdsTaxRuleService.SubmitTaxRuleSlabs(tdsSlabs);
                    if (result)
                        tdsSlabs.ResponseText = tdsSlabs.UpdatedBy.HasValue ? "Updated Successfully." : "Created Successfully";
                }

                var financialYear = GetYearBetweenYearsList(2005, 2044);
                tdsSlabs.fYList = financialYear.Select(x => new SelectListModel { id = 0, value = x }).ToList();

                return PartialView("_TDSSlabEntryForm", tdsSlabs);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}