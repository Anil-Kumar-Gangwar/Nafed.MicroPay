using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers.Reports
{
    public class EmployeeDependentReportController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        public EmployeeDependentReportController(IDropdownBindService ddlService)
        {
            this.ddlService = ddlService;
        }
        // GET: EmployeeDependentReport
        public ActionResult Index()
        {
            log.Info($"EmployeeDependentReport/Index");
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

        public ActionResult GetReportFilters()
        {
            log.Info($"EmployeeDependentReport/GetReportFilters");

            try
            {
                DependentViewModel empDepFilters = new DependentViewModel();
                empDepFilters.branchList = ddlService.ddlBranchList();
                empDepFilters.employeeList = new List<SelectListModel>();
                return PartialView("_ReportFilter", empDepFilters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ShowReport(DependentViewModel filters,FormCollection frm)
        {
            log.Info($"EmployeeDependentReport/ShowReport");
            ModelState.Remove("BranchID");
            ModelState.Remove("EmployeeID");
            if (ModelState.IsValid)
            {
                var empCode = frm.Get("employeeCode");
                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();

                parameterList.Add(new ReportParameter { name = "branchID", value = filters.BranchID });
                parameterList.Add(new ReportParameter { name = "empcode", value = empCode });
                reportModel.reportParameters = parameterList;
                reportModel.rptName = "DependentReport.rpt";
                Session["ReportModel"] = reportModel;
                return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                filters.employeeList = new List<SelectListModel>();
                filters.branchList = ddlService.ddlBranchList();
                return PartialView("_ReportFilter", filters);
            }
        }

        public JsonResult GetEmployeeByBranchID(int branchID)
        {
            log.Info($"EmployeeDependentReport/GetEmployeeByBranchID/{branchID}");
            try
            {
                var employees = ddlService.employeeByBranchID(branchID);
                employees.Insert(0, new SelectListModel { id = 0, value = "Select" });
                return Json(new { employeeList = new SelectList(employees, "id", "value") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}