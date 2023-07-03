using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Controllers.Reports.Payroll
{
    public class NonRegularEmpSalaryReportController : BaseController
    {
        // GET: NonRegularEmpSalaryReport
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _NonRegularEmpSalaryFilters()
        {
            log.Info($"NonRegularEmpSalaryReport/_NonRegularEmpSalaryFilters");
            try
            {
                return PartialView("_ReportFilters");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _NonRegularEmpSalaryReport(FormCollection frmcollection)
        {
            log.Info($"NonRegularEmpSalaryReport/_NonRegularEmpSalaryReport");
            try
            {
                var salmonth = Convert.ToByte(frmcollection["ddlMonth"]);
                var salYear = Convert.ToInt16(frmcollection["ddlYear"]);
                if (salmonth <= 0)
                    ModelState.AddModelError("monthRequired", "Please Select Month.");
                if (salYear <= 0)
                    ModelState.AddModelError("yearRequired", "Please Select Year.");
                if (ModelState.IsValid)
                {
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();

                    reportModel.reportType = 2;
                    parameterList.Add(new ReportParameter { name = "EMPID", value = userDetail.EmployeeID });
                    parameterList.Add(new ReportParameter { name = "SalMonth", value = salmonth });
                    parameterList.Add(new ReportParameter { name = "SalYear", value = salYear });
                    reportModel.reportParameters = parameterList;
                    reportModel.rptName = "NonRegularEmpPaySlip.rpt";
                    Session["ReportModel"] = reportModel;
                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return PartialView("_ReportFilters");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}