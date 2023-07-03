using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.Increment;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services.Budget;

namespace MicroPay.Web.Controllers.Reports.Increment
{
    public class IncrementReportController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IIncrement incrementService;

        public IncrementReportController(IDropdownBindService ddlService, IIncrement incrementService)
        {
            this.ddlService = ddlService;
            this.incrementService = incrementService;
        }

        public ActionResult Index()
        {
            log.Info($"IncrementReportController/Index");
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

        public ActionResult GetIncrementReportFilters()
        {
            log.Info($"IncrementReportController/GetIncrementReportFilters");
            try
            {
                ProjectedIncrementViewModel incrementVM = new ProjectedIncrementViewModel();
                incrementVM.branchList = ddlService.ddlBranchList();

                List<SelectListItem> incrementMonth = new List<SelectListItem>();
                incrementMonth.Add(new SelectListItem
                { Text = "Select", Value = "0" });
                incrementMonth.Add(new SelectListItem
                { Text = "January", Value = "1" });
                incrementMonth.Add(new SelectListItem
                { Text = "July", Value = "7" });
                incrementVM.incrementMonth = incrementMonth;
                BindDropdowns();
                return PartialView("_ReportFilter", incrementVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            List<SelectListItem> forEmployee = new List<SelectListItem>();
            forEmployee.Add(new SelectListItem
            { Text = "ALL", Value = "0" });
            forEmployee.Add(new SelectListItem
            { Text = "ABOVE G.M", Value = "1" });
            forEmployee.Add(new SelectListItem
            { Text = "BELOW G.M", Value = "2" });
            ViewBag.ForEmployee = forEmployee;
        }

        [HttpPost]
        public ActionResult IncrementReportDetails(ProjectedIncrementViewModel IVM, string eventType)
        {
            log.Info($"IncrementReportController/IncrementReportDetails");
            try
            {
                BindDropdowns();
                if (IVM.incrementMonthId == 0)
                    ModelState.AddModelError("MonthRequired", "Increment month required.");
                //if (IVM.forEmployeeId == 0)
                //    ModelState.AddModelError("ForEmployeeRequired", "Employee Required.");
                if (ModelState.IsValid)
                {
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();
                    int Rank1 = 0, Rank2 = 0;
                    
                    if (IVM.forEmployeeId == 1)
                    {
                        Rank1 = 1;
                        Rank2 = 5;
                    }
                    else if (IVM.forEmployeeId == 2)
                    {
                        Rank1 = 6;
                        Rank2 = incrementService.GetMaxRankDesignation();
                    }
                    else if(IVM.forEmployeeId == 0)
                    {
                        Rank1 = 1;
                        Rank2 = incrementService.GetMaxRankDesignation();
                    }
                    if (/*eventType.ToLower().Trim()*/eventType.Trim() == "2")
                    {
                        parameterList.Add(new ReportParameter() { name = "IncrementMonth", value = IVM.incrementMonthId });
                        parameterList.Add(new ReportParameter() { name = "RANK1", value = Rank1 });
                        parameterList.Add(new ReportParameter() { name = "RANK2", value = Rank2 });
                        parameterList.Add(new ReportParameter() { name = "Branch", value = IVM.BranchID == null ? 0 : IVM.BranchID });
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = "Projected_Increment.rpt";
                        Session["ReportModel"] = reportModel;
                    }
                    else if (/*eventType.ToLower().Trim()*/eventType.Trim() == "1") 
                    {
                        parameterList.Add(new ReportParameter() { name = "IncrementMonth", value = IVM.incrementMonthId });
                        parameterList.Add(new ReportParameter() { name = "RANK1", value = Rank1 });
                        parameterList.Add(new ReportParameter() { name = "RANK2", value = Rank2 });
                        parameterList.Add(new ReportParameter() { name = "Branch", value = IVM.BranchID == null ? 0 : IVM.BranchID });
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = "Increment.rpt";
                        Session["ReportModel"] = reportModel;
                    }
                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    IVM.branchList = ddlService.ddlBranchList();
                    List<SelectListItem> incrementMonth = new List<SelectListItem>();
                    incrementMonth.Add(new SelectListItem
                    { Text = "Select", Value = "0" });
                    incrementMonth.Add(new SelectListItem
                    { Text = "January", Value = "1" });
                    incrementMonth.Add(new SelectListItem
                    { Text = "July", Value = "7" });
                    IVM.incrementMonth = incrementMonth;
                    return PartialView("_ReportFilter", IVM);
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