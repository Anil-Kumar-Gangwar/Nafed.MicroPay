using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services.Budget;
namespace MicroPay.Web.Controllers.Reports.StaffBudget
{
    public class StaffBudgetReportController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IBudgetService budgetService;
        private readonly IDesignationService designationService;

        public StaffBudgetReportController(IDropdownBindService ddlService, IBudgetService budgetService, IDesignationService designationService)
        {
            this.ddlService = ddlService;
            this.budgetService = budgetService;
            this.designationService = designationService;
        }
        public ActionResult Index()
        {
            log.Info($"StaffBudgetReportController/Index");
            try
            {
                return View(userAccessRight);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetStaffBudgetReportFilters()
        {
            log.Info($"StaffBudgetReportController/GetStaffBudgetReportFilters");
            try
            {
                StaffBudgetViewModel SBVM = new StaffBudgetViewModel();
                var reportingYr = ExtensionMethods.GetBudgetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x.Key, Value = x.Value }).OrderByDescending(x => x.Value).ToList();
                SBVM.yearList = reportingYr;
                SBVM.designationList = ddlService.ddlDesignationList();
                return PartialView("_ReportFilter", SBVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult StaffBudgetReportDetails(StaffBudgetViewModel SBVM)
        {
            log.Info($"StaffBudgetReportController/StaffBudgetReportDetails");
            try
            {
                ModelState.Remove("genrateYearID");
                if (ModelState.IsValid)
                {
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();

                //    SBVM.selectYearID = SBVM.selectYearID == null ? Convert.ToString(DateTime.Now.Year - 1) + "-" + Convert.ToString(DateTime.Now.Year).Substring(2, 2) : SBVM.selectYearID;
                

                    parameterList.Add(new ReportParameter() { name = "desig", value = SBVM.designationID });
                    parameterList.Add(new ReportParameter() { name = "Year", value = SBVM.selectYearID });
                    parameterList.Add(new ReportParameter() { name = "Year1", value = SBVM.selectYearID });
                    reportModel.reportParameters = parameterList;
                    reportModel.rptName = "StaffBudget.rpt";
                    Session["ReportModel"] = reportModel;
                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var reportingYr = ExtensionMethods.GetBudgetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x.Key, Value = x.Value }).OrderByDescending(x => x.Value).ToList();
                    SBVM.yearList = reportingYr;
                    SBVM.designationList = ddlService.ddlDesignationList();
                    return PartialView("_ReportFilter", SBVM);
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