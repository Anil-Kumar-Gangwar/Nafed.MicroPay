using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Controllers.Reports
{
    public class RegBranchMgrReportController : BaseController
    {
        // GET: RegBranchMgrReport
        public ActionResult Index()
        {
            log.Info($"RegBranchMgrReportController/Index");
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
            log.Info($"RegBranchMgrReportController/GetReportFilters");
            
            try
            {
                BranchMgrReportFilter reportFilter = new BranchMgrReportFilter();
                var comboItems = new List<SelectListModel>();

                comboItems.Add(new SelectListModel {  id =1 , value= "ZONAL COORDINATER" });
                comboItems.Add(new SelectListModel { id = 2, value = "BRANCH MANAGER" });
                comboItems.Add(new SelectListModel { id = 3, value = @"ZONAL COORD. & BM'S" });
                reportFilter.filterList = comboItems;

                return PartialView("_ReportFilter", reportFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ShowReport(BranchMgrReportFilter filters)
        {
            log.Info($"RegBranchMgrReport/ShowReport");

            var comboItems = new List<SelectListModel>();
            comboItems.Add(new SelectListModel { id = 1, value = "ZONAL COORDINATER" });
            comboItems.Add(new SelectListModel { id = 2, value = "BRANCH MANAGER" });
            comboItems.Add(new SelectListModel { id = 3, value = @"ZONAL COORD. & BM'S" });
            filters.filterList = comboItems;

            if (!filters.selectedID.HasValue)
            {
                ModelState.AddModelError("SelectItemRequired", "Please Select one from list.");
                return PartialView("_ReportFilter", filters);
            }

            if (ModelState.IsValid)
            {
                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();
                string parm = string.Empty, parmextra = string.Empty, type = string.Empty;
                if (filters.selectedID==1)
                {
                    parm = "1"; parmextra = ""; type = "ZONAL COORDINATER";
                }
                else if (filters.selectedID == 2)
                {
                    parm = ""; parmextra = "1"; type = "BRANCH MANAGER";
                }
                else if(filters.selectedID == 3)
                {
                    parm = "1"; parmextra = "1"; type = "ZONAL COORD. & BM'S";
                }
                parameterList.Add(new ReportParameter() { name = "typeRM", value = parm });
                parameterList.Add(new ReportParameter() { name = "typeBM", value = parmextra });
                parameterList.Add(new ReportParameter() { name = "type", value = type });
                reportModel.reportParameters = parameterList;
                reportModel.rptName = "BranchRegManagerReport.rpt";
                Session["ReportModel"] = reportModel;
                return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return PartialView("_ReportFilter", filters);
            }
        }
    }
}