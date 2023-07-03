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
    public class StaffVacancyController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IBudgetService budgetService;
        private readonly IDesignationService designationService;

        public StaffVacancyController(IDropdownBindService ddlService, IBudgetService budgetService, IDesignationService designationService)
        {
            this.ddlService = ddlService;
            this.budgetService = budgetService;
            this.designationService = designationService;
        }

        public ActionResult Index()
        {
            log.Info($"StaffVacancyController/Index");
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

        public ActionResult GetStaffVacancyReportFilters()
        {
            log.Info($"StaffVacancyController/GetStaffVacancyReportFilters");
            try
            {
                BindDropdowns();
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

        public void BindDropdowns()
        {
            List<Model.SelectListModel> staffMonth = new List<Model.SelectListModel>();
            staffMonth.Add(new Model.SelectListModel
            { value = "Select", id = 0 });
            staffMonth.Add(new Model.SelectListModel
            { value = "January", id = 1 });
            staffMonth.Add(new Model.SelectListModel
            { value = "July", id = 7 });
            ViewBag.StaffMonth = new SelectList(staffMonth, "id", "value");
        }

        [HttpPost]
        public ActionResult StaffVacancyReport(StaffBudgetViewModel SBVM)
        {
            log.Info($"StaffVacancyController/StaffVacancyReport");
            try
            {
                ModelState.Remove("genrateYearID");
                if (ModelState.IsValid)
                {
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();
                    if (!SBVM.periodWise)
                    {
                        parameterList.Add(new ReportParameter() { name = "Year", value = SBVM.selectYearID });
                        parameterList.Add(new ReportParameter() { name = "desig", value = SBVM.designationID });
                        reportModel.reportParameters = parameterList;
                      //  reportModel.dsUserName = "sa";
                        //reportModel.dsPassword = "sa@123";
                        reportModel.rptName = "StaffVacancy.rpt";
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                    }
                   
                    {
                        if (SBVM.monthID == 0)
                        {
                            ModelState.AddModelError("monthRequired", "Please Select Month");
                            BindDropdowns();
                            var reportingYr = ExtensionMethods.GetBudgetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x.Key, Value = x.Value }).OrderByDescending(x => x.Value).ToList();
                            SBVM.yearList = reportingYr;
                            SBVM.designationList = ddlService.ddlDesignationList();
                            return PartialView("_ReportFilter", SBVM);
                        }
                        else
                        {
                            ModelState.Remove("monthRequired");
                            parameterList.Add(new ReportParameter() { name = "Year", value = SBVM.selectYearID });
                            parameterList.Add(new ReportParameter() { name = "month", value = SBVM.monthID });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "PeriodWiseStaffVacancy.rpt";
                            Session["ReportModel"] = reportModel;
                            return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    BindDropdowns();
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