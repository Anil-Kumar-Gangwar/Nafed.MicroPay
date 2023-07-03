using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.Data;
using Nafed.MicroPay.Common;
using System.Linq;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace MicroPay.Web.Controllers
{
    public class MyAttendanceDetailController : BaseController
    {
        private readonly IEmployeeAttendancedetailsService employeeAttendance;
        private readonly IDropdownBindService ddlService;
        private readonly IMarkAttendance markAttendance;

        public MyAttendanceDetailController(IEmployeeAttendancedetailsService employeeAttendance, IDropdownBindService ddlService, IMarkAttendance markAttendance)
        {
            this.employeeAttendance = employeeAttendance;
            this.ddlService = ddlService;
            this.markAttendance = markAttendance;
        }
        // GET: MyAttendanceDetail
        public ActionResult Index()
        {
            log.Info($"MyAttendanceDetailController/Index");
            EmployeeAttendancedetailsViewModel employeeAttendanceVM = new Models.EmployeeAttendancedetailsViewModel();
            employeeAttendanceVM.userRights = userAccessRight;
            return View(employeeAttendanceVM);
        }

        [HttpGet]
        public ActionResult EmployeeAttendancedetailsGridView(EmployeeAttendancedetailsViewModel empattendance, FormCollection frm, string sourceController,string ButtonType)
        {

            log.Info($"MyAttendanceDetailController/EmployeeAttendancedetailsGridView");
            try
            {
                List<Model.MyAttendanceDetails> attendanceList = new List<Model.MyAttendanceDetails>();
                attendanceList = employeeAttendance.GetMyAttendanceList((int)empattendance.BranchID, (int)empattendance.EmployeeID, empattendance.SelectedMonth.ToString(), empattendance.SelectYear.ToString(), (int)UserType.Employee, Convert.ToInt16(userDetail.EmployeeID));
                if (string.IsNullOrEmpty(sourceController))
                {
                    DataTable DT = ExtensionMethods.ToDataTable(attendanceList);
                    if (DT.Columns.Contains("EmployeeId"))
                        DT.Columns.Remove("EmployeeId");
                    if (DT.Columns.Contains("Editable"))
                        DT.Columns.Remove("Editable");
                    if (DT.Columns.Contains("EmpAttendanceID"))
                        DT.Columns.Remove("EmpAttendanceID");
                    DataSet ds = new DataSet();
                    ds.Tables.Add(DT);
                    TempData["ExportData"] = ds;
                }
                else
                ViewBag.sourceController = "employeeattendancedetails";
                return PartialView("_EmployeeAttendanceGridViewDetail", attendanceList);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult ExportExcel(EmployeeAttendancedetailsViewModel obj)
        {
            //// ReportDocument rd = new ReportDocument();
            //Reports.AttendanceReport rpts = new Reports.AttendanceReport();
            ////  rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AttendanceReport.rpt"));           
            //rpts.Load();
            //rpts.SetDataSource(FF);

            return Content("");
        }

        public ActionResult SearchEmployeeAttendancedetails(FormCollection formCollection)
        {
            log.Info($"MyAttendanceDetailController/SearchEmployeeAttendancedetails");
            try
            {
                var yearDDl = Enumerable.Range(2006, DateTime.Now.Year - 2006 + 1).
                            Select(i => new Model.SelectListModel { id = i, value = i.ToString() }).OrderByDescending(x => x.value).ToList();
                var monthDDL = Enumerable.Range(1, 12).
                                        Select(i => new Model.SelectListModel
                                        {
                                            id = i,
                                            value = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(i)
                                        }).ToList();

                monthDDL.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });


                EmployeeAttendancedetailsViewModel employeeAttendanceVM = new EmployeeAttendancedetailsViewModel();
                employeeAttendanceVM.BranchID = userDetail.BranchID;
                employeeAttendanceVM.EmployeeID = userDetail.EmployeeID;
                employeeAttendanceVM.BranchName = userDetail.Location;
                employeeAttendanceVM.EmployeeName = userDetail.FullName;
                employeeAttendanceVM.ddlYear = yearDDl;
                employeeAttendanceVM.ddlMonth = monthDDL;



                return PartialView("_SearchEmployeeAttendancedetails", employeeAttendanceVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            var ddlBranchList = ddlService.ddlBranchList();

            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

        }

        public JsonResult GetBranchEmployee(int branchID)
        {
            try
            {
                var employeedetails = ddlService.employeeByBranchID(branchID);
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

        public ActionResult ExportSheet(FormCollection frm)
        {
            try
            {
                log.Info("MyAttendanceDetailController/ExportSheet");
                string fileName = string.Empty, msg = string.Empty;
                string fullPath = Server.MapPath("~/FileDownload/");
                fileName = ExtensionMethods.SetUniqueFileName($"Attendance Details-{userDetail.EmployeeCode}-{userDetail.FullName}-", FileExtension.xlsx);

                var data = (DataSet)TempData["ExportData"];
                if (data != null && data.Tables[0].Rows.Count > 0)
                {
                    var res = employeeAttendance.ExportAttendance(data, fullPath, fileName);
                    return Json(new { fileName = fileName, fullPath = fullPath + fileName, message = "success" });
                }
                else
                {
                    return Json(new { msgType = "error", message = "No record(s) found to export." });
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _GetApprovalPopup(int empID, int empAttendanceID)
        {
            Model.EmpAttendance empAttendance = new Model.EmpAttendance();
            empAttendance.EmpAttendanceID = empAttendanceID;
            empAttendance.EmployeeId = empID;
            return PartialView("_ApprovalPopup", empAttendance);
        }

        [HttpPost]
        public ActionResult _PostAttendance(Model.EmpAttendance attendance,FormCollection frm)
        {
            log.Info("MyAttendanceDetailController/_PostAttendance");
            try
            {
                var approved = frm.Get("rbapproved");
                if (!string.IsNullOrEmpty(approved))
                {
                    if (ModelState.IsValid)
                    {

                        attendance.Attendancestatus = approved == "True" ? (int)EmpAttendanceStatus.Approved
                            : (int)EmpAttendanceStatus.RejectedByAcceptanceAuthority;
                        attendance.AcceptanceAuthRemark = attendance.Remarks;                     
                        string msg = "";
                        int res = markAttendance.ApproveRejectAttendance(attendance);
                        if (res > 0)
                        {
                            msg = approved == "True" ? "Attendance Approved." : "Attendance Rejected.";                           
                        }
                        return Json(new
                        {
                            status = res > 0 ? true : false,
                            type = "success",
                            msg = msg


                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                return PartialView("_AttendanceGridView", attendance);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                throw ex;
            }
        }
    }
}