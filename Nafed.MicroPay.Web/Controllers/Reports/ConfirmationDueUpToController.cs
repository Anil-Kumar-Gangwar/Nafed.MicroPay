using MicroPay.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports
{
    public class ConfirmationDueUpToController : BaseController
    {
        public ActionResult Index()
        {
            log.Info($"ConfirmationDueUpToController/Index");
            try
            {
                EmployeeViewModel confirmationUpTo = new EmployeeViewModel();
                return View(confirmationUpTo);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _ReportFilter()
        {
            log.Info($"ConfirmationDueUpToController/_ReportFilter");
            try
            {
                EmployeeViewModel confirmationUpTo = new EmployeeViewModel();
                return PartialView("_ReportFilter", confirmationUpTo);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ShowReport(EmployeeViewModel confirmationUpTo)
        {
            log.Info($"ConfirmationDueUpToController/ShowReport");
            try
            {
                if (confirmationUpTo.ConfirmationDueDateUpTo == null)
                    ModelState.AddModelError("DueDateUpTo", "Please select upto date");
                if (ModelState.IsValid)
                {
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();
                    parameterList.Add(new ReportParameter() { name = "duedate", value = confirmationUpTo.ConfirmationDueDateUpTo });
                    reportModel.reportParameters = parameterList;
                    reportModel.rptName = "ConfirmationDueReport.rpt";
                    Session["ReportModel"] = reportModel;
                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return PartialView("_ReportFilter", confirmationUpTo);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}