using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using System.Data;

namespace MicroPay.Web.Controllers.Attendance
{
    public class MarkAttendanceController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IDropdownBindService ddlService;
        private readonly IMarkAttendance markAttendance;

        public MarkAttendanceController(IEmployeeService employeeService, IDropdownBindService ddlService,
            IMarkAttendance markAttendance)
        {
            this.employeeService = employeeService;
            this.ddlService = ddlService;
            this.markAttendance = markAttendance;
            //   this.empAttendanceRepo = empAttendanceRepo;
        }
        public ActionResult Index()
        {
            log.Info($"MarkAttendanceController/Index");
            ViewBag.EmployeeID = userDetail.EmployeeID;
            ViewBag.BranchID = userDetail.BranchID;
            var reportingTo = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.AttendanceApproval);
            if (reportingTo != null)
            {
                ViewBag.ReportingTo = "1";
            }
            else
            {
                ViewBag.ReportingTo = "0";
            }
            if (userDetail.UserTypeID != (int)UserType.Employee)
            {
                ViewBag.UserID = "1";
            }
            else
            {
                ViewBag.UserID = "2";

            }
            return View(userAccessRight);
        }

        public ActionResult AddMarkAttendance(FormCollection formCollection)
        {
            log.Info($"MarkAttendanceController/AddMarkAttendance");
            try
            {
                BindDropdowns();
                EmployeeViewModel employeeVM = new EmployeeViewModel();
                var userTypeID = userDetail.UserTypeID;
                List<Model.SelectListModel> selectType = new List<Model.SelectListModel>();
                selectType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
                ViewBag.EmployeeDetails = new SelectList(selectType, "id", "value");
                return PartialView("_AddMarkAttendance", employeeVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public PartialViewResult MarkAttendanceGridView(Model.EmpAttendance empAttendance)
        {
            log.Info($"MarkAttendanceController/MarkAttendanceGridView");
            try
            {
                MarkAttendanceViewModel markAttendanceVM = new MarkAttendanceViewModel();
                markAttendanceVM.TabletProxyList = markAttendance.AttendanceDetails(empAttendance);
                markAttendanceVM.userRights = userAccessRight;
                return PartialView("_MarkAttendanceGridView", markAttendanceVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            var ddlAttendanceType = ddlService.ddlAttendanceTypeList();
            Model.SelectListModel selectAttendance = new Model.SelectListModel();
            selectAttendance.id = 0;
            selectAttendance.value = "Select";
            ddlAttendanceType.Insert(0, selectAttendance);
            ViewBag.AttendanceType = new SelectList(ddlAttendanceType, "id", "value");

            var ddlBranchList = ddlService.ddlBranchList(userDetail.BranchID, userDetail.UserTypeID);

            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            var ddlTourBranchList = ddlService.ddlBranchList(userDetail.BranchID, null);

            Model.SelectListModel selectTourBranch = new Model.SelectListModel();
            selectTourBranch.id = 0;
            selectTourBranch.value = "Select";
            ddlTourBranchList.Insert(0, selectTourBranch);
            ViewBag.TourBranch = new SelectList(ddlTourBranchList, "id", "value");

            var ddlLocationList = ddlService.ddlBranchList(userDetail.BranchID, null);
            Model.SelectListModel selectLocation = new Model.SelectListModel();
            //selectLocation.id = 0;
            //selectLocation.value = "Select";
            //ddlLocationList.Insert(0, selectLocation);

            selectLocation.id = 9999;
            selectLocation.value = "Other Location";
            ddlLocationList.Insert(ddlLocationList.Count, selectLocation);
            ViewBag.Location = new SelectList(ddlLocationList, "id", "value");
        }


        public JsonResult InsertAttendance(Model.EmpAttendance empAttendance)
        {
            log.Info("MarkAttendanceController/InsertMarkAttendance");
            var result = new JsonResult();
            try
            {
                var empReportings = GetEmpProcessApprovalSetting((int)empAttendance.EmployeeId, Nafed.MicroPay.Common.WorkFlowProcess.AttendanceApproval);
                if (empReportings == null)
                {
                    return result = Json(new { insertEmployees = "-3", msg = "You can not mark attendance right now because either your Reporting Manager or Reviewing Manager is not set." }, JsonRequestBehavior.AllowGet);
                }
                if (ModelState.IsValid)
                {
                    empAttendance.CreatedBy = userDetail.UserID;
                    empAttendance.CreatedOn = DateTime.Now;
                    empAttendance.MarkedBy = userDetail.UserID;
                    empAttendance.Reportingofficer = userDetail.ReportingTo;
                    empAttendance.ReviewerTo = userDetail.ReviwerTo;
                    if (markAttendance.AttendanceExists(empAttendance.BranchID, empAttendance.EmployeeId, empAttendance.ProxydateIn, empAttendance.ProxyOutDate))
                    {
                        result = Json(new { insertEmployees = "-2" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                        {
                            SenderID = empAttendance.EmployeeId,
                            ReceiverID = empReportings.ReviewingTo ?? empReportings.ReportingTo,
                            SenderDepartmentID = userDetail.DepartmentID,
                            SenderDesignationID = userDetail.DesignationID,
                            CreatedBy = userDetail.UserID,
                            EmployeeID = empAttendance.EmployeeId,
                            Scomments = empAttendance.Remarks,
                            ProcessID = (int)WorkFlowProcess.AttendanceApproval,
                            StatusID = (int)EmpAttendanceStatus.Pending

                        };
                        empAttendance._ProcessWorkFlow = workFlow;

                        int mainMarkAttendanceID = markAttendance.InsertTabletMarkAttendanceDetails(empAttendance, userDetail.UserTypeID);
                        if (mainMarkAttendanceID > 0)
                        {
                            empAttendance.EmpAttendanceID = mainMarkAttendanceID;
                            int markAttendanceID = markAttendance.InsertMarkAttendanceDetails(empAttendance);
                            if (markAttendanceID > 0)
                            {
                                result = Json(new { insertEmployees = "Submit Successfully" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (mainMarkAttendanceID == -1)
                        {
                            result = Json(new { insertEmployees = "-1" }, JsonRequestBehavior.AllowGet);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return result;
        }

        public JsonResult GetBranchEmployee(int branchID)
        {
            try
            {

                var employeedetails = ddlService.employeeByBranchID(branchID, userDetail.EmployeeID, userDetail.UserTypeID);
                Model.SelectListModel selectemployeeDetails = new Model.SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "Select";
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

        public JsonResult GetTourBranchEmployee(int branchID)
        {
            try
            {

                var employeedetails = ddlService.employeeByBranchID(branchID, userDetail.EmployeeID, null);
                Model.SelectListModel selectemployeeDetails = new Model.SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "Select";
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

        public ActionResult EmployeeTour()
        {
            log.Info($"MarkAttendanceController/EmployeeTour");
            return View("TourIndex");
        }
        [HttpGet]
        public ActionResult AddTour(FormCollection formCollection)
        {
            log.Info($"MarkAttendanceController/AddTour");
            try
            {
                BindDropdowns();
                Model.EmpAttendance employeeVM = new Model.EmpAttendance();
                var userTypeID = userDetail.UserTypeID;
                List<Model.SelectListModel> selectType = new List<Model.SelectListModel>();
                selectType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
                ViewBag.EmployeeDetails = new SelectList(selectType, "id", "value");
                return PartialView("_AddTour", employeeVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult AddTour(Model.EmpAttendance empAttandance)
        {
            log.Info($"MarkAttendanceController/AddTour");
            try
            {
                empAttandance.Mode = "T";// for tour entry.
                empAttandance.CreatedBy = userDetail.UserID;
                empAttandance.CreatedOn = DateTime.Now;
                int res = markAttendance.InsertTourDetail(empAttandance);
                if (res > 0)
                    return Json(new { success = true, msg = "Record successfully added." }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetTourDetail(Model.EmpAttendance empAttendance, string ButtonType)
        {
            log.Info($"MarkAttendanceController/GetTourDetail");
            try
            {
                if (ButtonType == "Search")
                {
                    var tourList = markAttendance.GetTourDetails(empAttendance);
                    TempData["tempTourList"] = tourList;
                    return PartialView("_TourGridView", tourList);
                }
                else if (ButtonType == "Export")
                {
                    string tFilter = string.Empty;
                    List<Model.EmpAttendance> tourList = (List<Model.EmpAttendance>)TempData["tempTourList"];
                    if (tourList != null)
                    {
                        DataTable exportData = new DataTable();
                        string fileName = string.Empty, msg = string.Empty;
                        string fullPath = Server.MapPath("~/FileDownload/");
                        fileName = "Employee Tour Details Report" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;
                        if (empAttendance.BranchID == 0)
                            tFilter = "Branch - All";

                        else if (empAttendance.BranchID > 0)
                        {
                            var branchName = tourList[0].BranchName;
                            tFilter = $"Branch - {branchName}";
                        }
                        DataColumn dtc0 = new DataColumn("S.No.");
                        DataColumn dtc1 = new DataColumn("Name");
                        DataColumn dtc2 = new DataColumn("Designation");
                        DataColumn dtc3 = new DataColumn("Tour Type");
                        DataColumn dtc4 = new DataColumn("Tour Location");
                        DataColumn dtc5 = new DataColumn("Order Date");
                        DataColumn dtc51 = new DataColumn("From Date");
                        DataColumn dtc52 = new DataColumn("To Date");
                        DataColumn dtc53 = new DataColumn("Joining Date");
                        DataColumn dtc54 = new DataColumn("Releaving Date");
                        DataColumn dtc55 = new DataColumn("Joining Back Date");
                        DataColumn dtc56 = new DataColumn("Releaving Date From Tour Location");
                        DataColumn dtc6 = new DataColumn("Duration");
                        DataColumn dtc7 = new DataColumn("Remark");
                        exportData.Columns.Add(dtc0);
                        exportData.Columns.Add(dtc1);
                        exportData.Columns.Add(dtc2);
                        exportData.Columns.Add(dtc3);
                        exportData.Columns.Add(dtc4);
                        exportData.Columns.Add(dtc5);
                        exportData.Columns.Add(dtc51);
                        exportData.Columns.Add(dtc52);
                        exportData.Columns.Add(dtc53);
                        exportData.Columns.Add(dtc54);
                        exportData.Columns.Add(dtc55);
                        exportData.Columns.Add(dtc56);
                        exportData.Columns.Add(dtc6);
                        exportData.Columns.Add(dtc7);
                        for (int i = 0; i < tourList.Count; i++)
                        {
                            var place = string.Empty;

                            if (tourList[i].TypeID == (int)PlaceOfAttendance.Office)
                            {
                                place = "Office";
                            }
                            if (tourList[i].TypeID == (int)PlaceOfAttendance.ClientSite)
                            {
                                place = "Local Tour";
                            }
                            if (tourList[i].TypeID == (int)PlaceOfAttendance.WorkFromHome)
                            {
                                place = "Work From Home";
                            }
                            if (tourList[i].TypeID == (int)PlaceOfAttendance.Tour)
                            {
                                place = "Remote Tour";
                            }

                            DataRow row = exportData.NewRow();
                            row["S.No."] = i + 1;
                            row["Name"] = tourList[i].EmployeeName;
                            row["Designation"] = tourList[i].SenderDesignation;
                            row["Tour Type"] = place;
                            row["Tour Location"] = tourList[i].LocationName;
                            row["Order Date"] = tourList[i].OrderDate.ToString("dd-MMM-yyyy");
                            row["From Date"] = tourList[i].ProxydateIn.ToString("dd-MMM-yyyy");
                            row["To Date"] = tourList[i].ProxyOutDate.ToString("dd-MMM-yyyy");
                            row["Joining Date"] = tourList[i].JoiningDate.HasValue ? tourList[i].JoiningDate.Value.ToString("dd-MMM-yyyy") : "";
                            row["Releaving Date"] = tourList[i].ReleavingDate.HasValue ? tourList[i].ReleavingDate.Value.ToString("dd-MMM-yyyy") : "";
                            row["Joining Back Date"] = tourList[i].JoinBackDate.HasValue ? tourList[i].JoinBackDate.Value.ToString("dd-MMM-yyyy") : "";
                            row["Releaving Date From Tour Location"] = tourList[i].ReleavDateFromLoc.HasValue ? tourList[i].ReleavDateFromLoc.Value.ToString("dd-MMM-yyyy") : "";
                            row["Duration"] = ((tourList[i].ProxyOutDate - tourList[i].ProxydateIn).Days + 1) + " Day(s)";
                            row["Remark"] = tourList[i].Remarks;
                            exportData.Rows.Add(row);
                        }
                        markAttendance.ExportTourDetail(exportData, fullPath, fileName, tFilter);
                        fullPath = $"{fullPath}{fileName}";
                        return JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");

                    }
                }
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

    }
}