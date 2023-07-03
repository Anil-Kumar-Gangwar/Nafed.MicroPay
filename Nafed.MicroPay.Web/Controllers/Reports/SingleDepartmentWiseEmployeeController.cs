using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers.Reports
{
    public class SingleDepartmentWiseEmployeeController : BaseController
    {
        public ActionResult Index()
        {
            log.Info($"SingleDepartmentWiseEmployeeController/Index");
            try
            {
                userAccessRight.ControllerName = "SingleDepartmentWiseEmployee";
                userAccessRight.PageTitle = "Single Department Wise Employee";
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
            log.Info($"SingleDepartmentWiseEmployeeController/SubmitForm");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                reportModel.rptName = "singledepartmentwise.rpt";
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