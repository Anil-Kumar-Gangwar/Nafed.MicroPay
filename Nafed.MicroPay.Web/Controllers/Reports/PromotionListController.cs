using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports
{
    public class PromotionListController : BaseController
    {
        // GET: PromotionList
        private readonly IDropdownBindService dropDownService;
        public PromotionListController(IDropdownBindService dropDownService)
        {
            this.dropDownService = dropDownService;
        }

        public ActionResult Index()
        {
            log.Info($"PromotionListController/Index");
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

        public ActionResult _ReportFilter()
        {
            log.Info($"PromotionListController/_ReportFilter");
            try
            {
                PromotionFilters promotionReportFilter = new PromotionFilters();
                promotionReportFilter.staffBudgetYearList = dropDownService.GetStaffBudget();

                promotionReportFilter.staffBudgetYearList = promotionReportFilter.staffBudgetYearList.Where(x => x.value != "").Select(x => x.value).Distinct().
                                  Select(i => new SelectListModel { value = i.ToString() }).OrderByDescending(x => x.value).ToList();

                promotionReportFilter.branchList = dropDownService.ddlBranchList();
                promotionReportFilter.monthList = new List<SelectListModel>()
                {
                    new SelectListModel { id = 0, value = "Select" },
                    new SelectListModel { id = 1, value = "JANUARY" },
                    new SelectListModel { id = 7, value = "JULY" }
                };
                return PartialView("_ReportFilter", promotionReportFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult PromotionListDetails(PromotionFilters promotionReportFilter)
        {
            log.Info($"PromotionListController/PromotionListDetails");
            try
            {
                if (ModelState.IsValid)
                {
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();
                    int branchId = 0, month = 0, year = 0;
                    string rptName = string.Empty, buggetYear = string.Empty;
                    string branchCode = string.Empty, FROMDATE = string.Empty, TODATE = string.Empty, PROYEAR = string.Empty;
                    branchId = (promotionReportFilter.branchId == null ? 0 : promotionReportFilter.branchId.Value);
                    month = (promotionReportFilter.monthId == null ? 0 : promotionReportFilter.monthId.Value);
                    year = (promotionReportFilter.yearId == null ? 0 : promotionReportFilter.yearId.Value);
                    buggetYear = promotionReportFilter.staffBudgetYearId;
                    if (branchId != 0)
                        branchCode = dropDownService.GetBranchCode(branchId);

                    if (promotionReportFilter.monthId == 1)
                    {
                        FROMDATE = "01/01/" + Convert.ToString(year);
                        TODATE = "30/06/" + Convert.ToString(year);
                        PROYEAR = buggetYear;
                    }
                    else
                    {
                        FROMDATE = "01/07/" + Convert.ToString(year);
                        TODATE = "31/12/" + Convert.ToString(year);
                        PROYEAR = buggetYear;
                    }

                    parameterList.Add(new ReportParameter { name = "Branch", value = branchCode });
                    parameterList.Add(new ReportParameter { name = "fromdate", value = FROMDATE });
                    parameterList.Add(new ReportParameter { name = "todate", value = TODATE });
                    parameterList.Add(new ReportParameter { name = "mon", value = (month == 1 ? "JANUARY" : "JULY") });
                    parameterList.Add(new ReportParameter { name = "fyear", value = year });
                    parameterList.Add(new ReportParameter { name = "PROYEAR", value = buggetYear });
                    rptName = "PromotionListrpt.rpt";
                    reportModel.reportParameters = parameterList;
                    reportModel.rptName = rptName;
                    Session["ReportModel"] = reportModel;
                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    promotionReportFilter.staffBudgetYearList = dropDownService.GetStaffBudget();

                    promotionReportFilter.staffBudgetYearList = promotionReportFilter.staffBudgetYearList.Where(x => x.value != "").Select(x => x.value).Distinct().
                                      Select(i => new SelectListModel { value = i.ToString() }).OrderByDescending(x => x.value).ToList();

                    promotionReportFilter.branchList = dropDownService.ddlBranchList();
                    promotionReportFilter.monthList = new List<SelectListModel>()
                {
                    new SelectListModel { id = 0, value = "Select" },
                    new SelectListModel { id = 1, value = "JANUARY" },
                    new SelectListModel { id = 7, value = "JULY" }
                };
                    return PartialView("_ReportFilter", promotionReportFilter);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}