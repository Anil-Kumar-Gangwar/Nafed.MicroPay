using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Nafed.MicroPay.Common.ExtensionMethods;
using static Nafed.MicroPay.Common.FileHelper;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Controllers.Children_Education
{
    public class ChildrenEducationAllowanceController : BaseController
    {
        private readonly IDropdownBindService _ddlServices;
        private readonly IChildrenEducationService _childrenService;
        public ChildrenEducationAllowanceController(IDropdownBindService dropdownService, IChildrenEducationService childrenService)
        {
            _ddlServices = dropdownService;
            _childrenService = childrenService;
        }

        // GET: ChildrenEducationAllowance
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Filter()
        {
            log.Info("ChildrenEducationAllowanceController/_Filter");
            try
            {
                Model.CommonFilter cFilter = new Model.CommonFilter();
                cFilter.ddlBranch = _ddlServices.ddlBranchList();
                var reportingYrs = _ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                ViewBag.reportingYrs = reportingYrs;
                cFilter.ReportingYear = reportingYrs.First().value; // set default as current year
                return PartialView("_Filters", cFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult _List()
        {
            log.Info($"ChildrenEducationAllowanceController/_List");
            try
            {
                ChildrenEducationViewModel empChildrenEducationVM = new ChildrenEducationViewModel();
                var reportingYrs = _ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                Model.CommonFilter filter = new Nafed.MicroPay.Model.CommonFilter();
                filter.ReportingYear = reportingYrs.First().value;
                var childAllow = _childrenService.GetEmployeeChildrenEducationYearWise(filter);
                empChildrenEducationVM.childrenEducationList = childAllow;
                TempData["childAllow"] = childAllow;
                TempData.Keep("childAllow");
                return PartialView("_List", empChildrenEducationVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult _GetList(Model.CommonFilter filter)
        {
            log.Info($"ChildrenEducationAllowanceController/_List");
            try
            {
                ChildrenEducationViewModel empChildrenEducationVM = new ChildrenEducationViewModel();
                var childAllow = _childrenService.GetEmployeeChildrenEducationYearWise(filter);
                empChildrenEducationVM.childrenEducationList = childAllow;
                TempData["childAllow"] = childAllow;
                TempData.Keep("childAllow");
                return PartialView("_List", empChildrenEducationVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult View(int empID, int childrenEduHdrId, bool updateOrNot = true)
        {
            log.Info($"ChildrenEducationAllowance/View/{ empID}/{childrenEduHdrId}");
            try
            {
                Model.ChildrenEducationHdr childrenEduData = new Model.ChildrenEducationHdr();
                var reportingYr = DateTime.Now.GetFinancialYr();
                childrenEduData = _childrenService.GetChildrenEducation(empID, childrenEduHdrId);
                childrenEduData.ChildrenEducationDetailsList = _childrenService.GetChildrenEducationDetails(empID, childrenEduHdrId);
                childrenEduData.DependentList = _childrenService.GetDependentList(empID);
                childrenEduData.ChildrenEducationDocumentsList = _childrenService.GetChildrenEducationDocumentsList(empID, childrenEduHdrId);

                if (childrenEduData.ChildrenEducationDetailsList.Count > 0)
                {
                    var sno = 1;
                    childrenEduData.ChildrenEducationDetailsList.ForEach(x =>
                    {
                        x.sno = sno++;
                    });
                }

                childrenEduData.IsDependentMatch = true;
                return View("ChildrenEducationContainer", childrenEduData);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetChildrenEducationDocuments(int employeeId, int childrenEduHdrID)
        {
            log.Info($"ChildrenEducationAllowance/GetChildrenEducationDocuments");
            try
            {
                ChildrenEducationViewModel documentViewModel = new ChildrenEducationViewModel();
                documentViewModel.childrenEducationDocuments = _childrenService.GetChildrenEducationDocumentsList(employeeId, childrenEduHdrID);
                return PartialView("_ReceiptDocumentPopUp", documentViewModel);
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
            log.Info($"ChildrenEducationAllowance/DownloadExcel");

            DataTable exportData = new DataTable();

            string fileName = string.Empty, msg = string.Empty;
            string fullPath = Server.MapPath("~/FileDownload/");
            fileName = "Children Education Allowance Report" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;

            DataColumn dtc0 = new DataColumn("S.No.", typeof(string));
            DataColumn dtc1 = new DataColumn("Reporting Year", typeof(string));
            DataColumn dtc2 = new DataColumn("Employee Code", typeof(string));
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
            var lstDetail = (List<Model.ChildrenEducationHdr>)TempData["childAllow"];
            TempData["childAllow"] = lstDetail;
            TempData.Keep("childAllow");
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
            _childrenService.DownloadExcel(ds, fullPath, fileName);
            return Json(new
            {
                fileName = fileName,
                fullPath = fullPath + fileName
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetBranchEmployee(int branchID)
        {
            try
            {
                var employeedetails = _ddlServices.employeeByBranchID(branchID);
                Model.SelectListModel selectemployeeDetails = new Model.SelectListModel();
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
    }
}