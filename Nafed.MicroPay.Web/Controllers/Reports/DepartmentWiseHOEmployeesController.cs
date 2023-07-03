using MicroPay.Web.Models;
using System;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Reports
{
    public class DepartmentWiseHOEmployeesController : BaseController
    {
        // GET: DepartmentWiseHOEmployees
        public ActionResult Index()
        {
            log.Info($"DepartmentWiseHOEmployees/Index");
            try
            {
                userAccessRight.PageTitle = "Employees Posted In Head Office Report";
                userAccessRight.ControllerName = "DepartmentWiseHOEmployees";
                return PartialView("_CommonReportViewer", userAccessRight);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult SubmitForm()
        {
            log.Info($"DepartmentWiseHOEmployees/SubmitForm");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                reportModel.rptName = "DepartmentWiseEmployee.rpt";
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