using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports
{
    public class DesignationHistoryReportController : BaseController
    {
        // GET: DesignationHistoryReport
        private readonly IDropdownBindService ddlService;

        public DesignationHistoryReportController(IDropdownBindService ddlService)
        {
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            log.Info($"DesignationHistoryReport/Index");

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
            log.Info($"EmployeeTransferReport/GetReportFilters");

            try
            {
                BranchTransferViewModel filters = new BranchTransferViewModel();
                filters.branchList = ddlService.ddlBranchList();
                filters.employeeList = new List<SelectListModel>();
                return PartialView("_ReportFilter", filters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ShowReport(DependentViewModel filters, FormCollection frm)
        {
            log.Info($"DesignationHistoryReport/ShowReport");
            ModelState.Remove("EmployeeID");
            if (ModelState.IsValid)
            {
                var empCode = frm.Get("employeeCode");
                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();

                //  parameterList.Add(new ReportParameter { name = "branchID", value = filters.BranchID });
                parameterList.Add(new ReportParameter { name = "empcode", value = empCode });
                reportModel.reportParameters = parameterList;
                reportModel.rptName = "DesgHistoryReport.rpt";
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
            log.Info($"DesignationHistoryReport/GetEmployeeByBranchID/{branchID}");
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