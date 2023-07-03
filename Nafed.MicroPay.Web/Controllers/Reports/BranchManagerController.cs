using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;


namespace MicroPay.Web.Controllers.Reports
{
    public class BranchManagerController : BaseController
    {
        private readonly IDropdownBindService dropDownService;
        public BranchManagerController(IDropdownBindService dropDownService)
        {
            this.dropDownService = dropDownService;
        }

        public ActionResult Index()
        {
            log.Info($"BranchManagerController/Index");
            try
            {
                BranchMgrReportFilter branchMgrReportFilter = new BranchMgrReportFilter();
                return View(branchMgrReportFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _ReportFilter()
        {
            log.Info($"BranchManagerController/_ReportFilter");
            try
            {
                BranchMgrReportFilter BMVM = new BranchMgrReportFilter();
                BMVM.filterList = dropDownService.GetAllEmployee();
                return PartialView("_ReportFilter", BMVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult BranchManagerDetails(BranchMgrReportFilter BMVM)
        {
            log.Info($"BranchManagerController/BranchManagerDetails");
            try
            {
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();
                    parameterList.Add(new ReportParameter() { name = "EmployeeId", value = BMVM.selectedEmployee });
                    reportModel.reportParameters = parameterList;
                    reportModel.rptName = "BranchManagerReport.rpt";
                    Session["ReportModel"] = reportModel;
                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}