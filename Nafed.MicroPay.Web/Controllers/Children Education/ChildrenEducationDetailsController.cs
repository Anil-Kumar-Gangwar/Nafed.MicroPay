using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Children_Education
{
    public class ChildrenEducationDetailsController : BaseController
    {
        private readonly IChildrenEducationService childrenEduService;
        private readonly IDropdownBindService ddlServices;

        public ChildrenEducationDetailsController(IChildrenEducationService childrenEduService, IDropdownBindService ddlServices)
        {
            this.childrenEduService = childrenEduService;
            this.ddlServices = ddlServices;
        }

        // GET: ChildrenEducationDetails
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetBranchEmployee(int branchID)
        {
            try
            {
                var employeedetails = ddlServices.employeeByBranchID(branchID);
                SelectListModel selectemployeeDetails = new SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "ALL";
                employeedetails.Insert(0, selectemployeeDetails);
                var emp = new SelectList(employeedetails, "id", "value");
                return Json(new { employees = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult _AdminChildrenEducationFilters()
        {
            log.Info($"ChildrenEducationDetailsController/_AdminChildrenEducationFilters");
            try
            {
                AppraisalFormApprovalFilter filters = new AppraisalFormApprovalFilter();
                //filters.employees = ddlServices.GetAllEmployee();  
                filters.ddlBranch = ddlServices.ddlBranchList();
                var reportingYrs = ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                filters.reportingYrs = reportingYrs;
                filters.selectedReportingYear = reportingYrs.First().value; // set default as current year
                return PartialView("_AdminChildrenEducationFilters", filters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostChildrenEducationFilters(AppraisalFormApprovalFilter filters)
        {
            log.Info($"ChildrenEducationDetailsController/_PostChildrenEducationFilters");
            try
            {
                var conveyanceForms = childrenEduService.GetChildrenEducationForAdmin(filters);
                TempData["conveyanceForms"] = conveyanceForms;
                TempData.Keep("conveyanceForms");
                return PartialView("_ChildrenEducationForAdmin", conveyanceForms);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult DownloadExcel()
        {
            log.Info($"ChildrenEducationDetailsController/DownloadExcel");

            DataTable exportData = new DataTable();

            string fileName = string.Empty, msg = string.Empty;
            string fullPath = Server.MapPath("~/FileDownload/");
            fileName = "Children Education Allowance Report" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;

            DataColumn dtc0 = new DataColumn("S.No.", typeof(string));
            DataColumn dtc1 = new DataColumn("Reporting Year", typeof(string));
            DataColumn dtc2 = new DataColumn("Employee Code",typeof(string));
            DataColumn dtc3 = new DataColumn("Employee Name", typeof(string));
            DataColumn dtc4 = new DataColumn("Branch Name", typeof(string));
            DataColumn dtc5 = new DataColumn("Designation", typeof(string));
            DataColumn dtc6 = new DataColumn("Receipt No.", typeof(string));
            DataColumn dtc7 = new DataColumn("Amount", typeof(decimal));
            exportData.Columns.Add(dtc0);
            exportData.Columns.Add(dtc1);
            exportData.Columns.Add(dtc2);
            exportData.Columns.Add(dtc3);
            exportData.Columns.Add(dtc4);
            exportData.Columns.Add(dtc5);
            exportData.Columns.Add(dtc6);
            exportData.Columns.Add(dtc7);
            var lstDetail = (List<ChildrenEducationHistoryModel>)TempData["conveyanceForms"];
            TempData["conveyanceForms"] = lstDetail;
            TempData.Keep("conveyanceForms");
            if (lstDetail != null && lstDetail.Count > 0)
            {
                for (int i = 0; i < lstDetail.Count; i++)
                {
                    DataRow row = exportData.NewRow();
                    row["S.No."] = i + 1;
                    row["Reporting Year"] = lstDetail[i].ReportingYear;
                    row["Employee Code"] = lstDetail[i].EmployeeCode;
                    row["Employee Name"] = lstDetail[i].EmployeeName;
                    row["Branch Name"] = lstDetail[i].Branch;
                    row["Designation"] = lstDetail[i].DesignationName;
                    row["Receipt No."] = lstDetail[i].ReceiptNo;
                    row["Amount"] = lstDetail[i].Amount;
                    exportData.Rows.Add(row);
                }
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(exportData);
            childrenEduService.DownloadExcel(ds, fullPath, fileName);
            return Json(new
            {
                fileName = fileName,
                fullPath = fullPath + fileName
            }, JsonRequestBehavior.AllowGet);

        }
    }
}