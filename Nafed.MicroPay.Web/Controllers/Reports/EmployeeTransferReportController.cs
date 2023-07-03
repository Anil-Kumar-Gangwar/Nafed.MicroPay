using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using System.Data;

namespace MicroPay.Web.Controllers.Reports
{
    public class EmployeeTransferReportController : BaseController
    {
        // GET: EmployeeTransferReport
        private readonly IDropdownBindService ddlService;
        private readonly IEmployeeService empService;
        public EmployeeTransferReportController(IDropdownBindService ddlService, IEmployeeService empService)
        {
            this.ddlService = ddlService;
            this.empService = empService;
        }
        public ActionResult Index()
        {
            log.Info($"EmployeeTransferReport/Index");
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
        public ActionResult ShowReport(DependentViewModel filters, FormCollection frm, string eventType)
        {
            log.Info($"EmployeeTransferReport/ShowReport");
            if (eventType == "Download")
            {
                ModelState.Remove("EmployeeID");
                ModelState.Remove("BranchID");
            }
            else
                ModelState.Remove("EmployeeID");

            if (ModelState.IsValid)
            {
                if (eventType == "Download")
                {
                    string tFilter = string.Empty;
                    DataTable exportData = new DataTable();
                    var empCode = frm.Get("employeeCode");
                    int? branchID = null;
                    if (filters.BranchID > 0)
                    {
                        branchID = filters.BranchID;
                    }
                    else
                        branchID = null;

                   var  empList = empService.GetEmployeeTransferDetail(branchID, empCode);
                    if (empList != null)
                    {
                        string fileName = string.Empty, msg = string.Empty;
                        string fullPath = Server.MapPath("~/FileDownload/");
                        fileName = "Employee Transfer Report" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;

                        DataColumn dtc0 = new DataColumn("S.No.");
                        DataColumn dtc1 = new DataColumn("Name");
                        DataColumn dtc2 = new DataColumn("Branch Name");                      
                        DataColumn dtc3 = new DataColumn("From Date");
                        DataColumn dtc4 = new DataColumn("To Date");
                        exportData.Columns.Add(dtc0);                       
                        exportData.Columns.Add(dtc1);
                        exportData.Columns.Add(dtc2);
                        exportData.Columns.Add(dtc3);
                        exportData.Columns.Add(dtc4);
                        for (int i = 0; i < empList.Rows.Count; i++)
                        {
                            DataRow row = exportData.NewRow();
                            row["S.No."] = i + 1;
                            row["Name"] = empList.Rows[i]["employeecode"].ToString()+"-"+ empList.Rows[i]["name"].ToString();
                            row["Branch Name"] = empList.Rows[i]["branchname"].ToString();                          
                            row["From Date"] = empList.Rows[i]["fromdate"];
                            row["To Date"] =  empList.Rows[i]["todate"];
                            exportData.Rows.Add(row);
                        }
                        DataSet ds = new DataSet();
                        ds.Tables.Add(exportData);
                        empService.ExportEmployeeTransferDetail(ds, fullPath, fileName);

                        fullPath = $"{fullPath}{fileName}";
                        return  JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");

                    }
                    return Content("");
                }
                else
                {
                    var empCode = frm.Get("employeeCode");
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();

                    //  parameterList.Add(new ReportParameter { name = "branchID", value = filters.BranchID });
                    parameterList.Add(new ReportParameter { name = "empcode", value = empCode });
                    reportModel.reportParameters = parameterList;
                    reportModel.rptName = "TransferHistoryReport.rpt";
                    Session["ReportModel"] = reportModel;
                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                }
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
            log.Info($"EmployeeTransferReport/GetEmployeeByBranchID/{branchID}");
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