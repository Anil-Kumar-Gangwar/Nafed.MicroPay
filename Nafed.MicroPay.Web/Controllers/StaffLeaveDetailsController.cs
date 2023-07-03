using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.IO;
using Nafed.MicroPay.Common;
using CrystalDecisions.CrystalReports.Engine;

namespace MicroPay.Web.Controllers
{
    public class StaffLeaveDetailsController : BaseController
    {
        private readonly IStaffLeaveDetailsService staffLeaveDetailsService;
        private readonly IDropdownBindService ddlService;
        private readonly ILeaveService leaveService;
        public StaffLeaveDetailsController(IStaffLeaveDetailsService staffLeaveDetailsService, IDropdownBindService ddlService, ILeaveService leaveService)
        {
            this.staffLeaveDetailsService = staffLeaveDetailsService;
            this.leaveService = leaveService;
            this.ddlService = ddlService;
        }
        // GET: StaffLeaveDetails
        public ActionResult Index()
        {
            log.Info($"StaffLeaveDetailsController/Index");
            return View(userAccessRight);
        }

        public ActionResult StaffLeaveDetailsGridView(Model.EmployeeLeave empLeave)
        {

            log.Info($"StaffLeaveDetailsController/StaffLeaveDetailsGridView");
            try
            {
                StaffLeaveDetailsViewModel staffLeaveVM = new StaffLeaveDetailsViewModel();
                //  List<Model.EmployeeLeave> objGrade = new List<Model.EmployeeLeave>();

                staffLeaveVM.userRights = userAccessRight;

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);


                empLeave.DateFrom = empLeave.DateFrom.HasValue ? empLeave.DateFrom : startDate.Date;
                empLeave.DateTo = empLeave.DateTo.HasValue ? empLeave.DateTo : endDate.Date;
                staffLeaveVM.GetStaffLeaveDetailsList = staffLeaveDetailsService.GetEmployeeLeaveDetailsList(empLeave, Convert.ToInt32(userDetail.ReportingTo), Convert.ToInt32(userDetail.ReviwerTo), Convert.ToInt32(userDetail.EmployeeID));
                return PartialView("_StaffLeaveDetailsGridView", staffLeaveVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult SearchStaffleaveDetails(FormCollection formCollection)
        {
            log.Info($"StaffLeaveDetailsController/SearchStaffleaveDetails");
            try
            {
                BindDropdowns();

                StaffLeaveDetailsViewModel employeeVM = new StaffLeaveDetailsViewModel();
                var userTypeID = userDetail.UserTypeID;
                List<Model.SelectListModel> selectType = new List<Model.SelectListModel>();
                return PartialView("_SearchStaffleaveDetails", employeeVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult StaffLeaveDetailsUpdate(Model.EmployeeLeave empLeave)
        {
            log.Info($"StaffLeaveDetailsController/StaffLeaveDetailsUpdate");
            try
            {
                bool updatestatus = false;
                StaffLeaveDetailsViewModel staffLeaveVM = new StaffLeaveDetailsViewModel();
                updatestatus = leaveService.UpdateLeaveStatus(empLeave, userDetail.UserID);
                if (updatestatus == true)
                {
                    TempData["Message"] = "Status updated successfully";
                }
                staffLeaveVM.GetStaffLeaveDetailsList = staffLeaveDetailsService.GetEmployeeLeaveDetailsList(empLeave, Convert.ToInt32(userDetail.ReportingTo), Convert.ToInt32(userDetail.ReviwerTo), Convert.ToInt32(userDetail.EmployeeID));
                return PartialView("_StaffLeaveDetailsGridView", staffLeaveVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {

            List<SelectListItem> selectStatus = new List<SelectListItem>();

            selectStatus.Add(new SelectListItem
            { Text = "All", Value = "0" });
            selectStatus.Add(new SelectListItem
            { Text = "Pending", Value = "1" });

            selectStatus.Add(new SelectListItem
            { Text = "Approved", Value = "8" });


            selectStatus.Add(new SelectListItem
            { Text = "Rejected", Value = "2" });

            selectStatus.Add(new SelectListItem
            { Text = "In Progress", Value = "3" });

            selectStatus.Add(new SelectListItem
            { Text = "Withdrawl", Value = "6" });

            ViewBag.LeaveStatus = selectStatus;


            //var ddlStatus = ddlService.ddlstatus();
            //Model.SelectListModel selectStatus = new Model.SelectListModel();
            //selectStatus.id = 0;
            //selectStatus.value = "Select";
            //ddlStatus.Insert(0, selectStatus);
            //ViewBag.LeaveStatus = new SelectList(ddlStatus, "id", "value");

            var ddlBranchList = ddlService.ddlBranchList();
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");


            var employeedetails = ddlService.employeeByBranchID(0);
            Model.SelectListModel selectemployeeDetails = new Model.SelectListModel();
            selectemployeeDetails.id = 0;
            selectemployeeDetails.value = "Select";
            employeedetails.Insert(0, selectemployeeDetails);
            var emp = new SelectList(employeedetails, "id", "value");
            ViewBag.Employee = emp;

        }

        public ActionResult DownloadDocument(string FileID)
        {
            try
            {
                string fullPath = Server.MapPath("~/Document/");
                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath + FileID);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileID);
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                throw ex;
            }
        }

        public JsonResult GetBranchEmployee(int branchID)
        {
            log.Info($"StaffLeaveDetailsController/GetBranchEmployee");
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
                throw ex;
            }
        }

        public ActionResult DownloadReport(int lid, int empid)
        {
            log.Info("StaffLeaveDetailsController/DownloadReport");
            try
            {
                if (lid > 0)
                {
                    var dtset = "";
                    dtset = ConfigManager.Value("odbcServer");

                    string rptName = string.Empty;
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();
                    parameterList.Add(new ReportParameter { name = "leaveID", value = lid });
                    parameterList.Add(new ReportParameter { name = "employeeID", value = empid });
                    reportModel.reportParameters = parameterList;
                    if (dtset == "odbcNAFEDHR")
                        rptName = "ELForm.rpt";
                    else
                        rptName = "ELFormTest.rpt";
                    reportModel.rptName = rptName;
                    ReportDocument objReport = new ReportDocument();
                    var rptModel = reportModel;
                    string path = Path.Combine(Server.MapPath(rptModel.ReportFilePath), rptModel.rptName);
                    objReport.Load(path);

                
                    if(dtset== "odbcNAFEDHR")
                    objReport.SetDatabaseLogon(ConfigManager.Value("odbcUser"), ConfigManager.Value("odbcPassword"), @"odbcNAFEDHR", ConfigManager.Value("odbcDatabase"));
                    else
                        objReport.SetDatabaseLogon(ConfigManager.Value("odbcUser"), ConfigManager.Value("odbcPassword"), @"odbcNAFEDHRTEST", ConfigManager.Value("odbcDatabase"));
                    objReport.Refresh();
                    if (rptModel.reportParameters?.Count > 0)
                    {
                        foreach (var item in rptModel.reportParameters)
                            objReport.SetParameterValue($"@{item.name}", item.value);
                    }
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();


                    objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;

                    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

                    Stream stream = objReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "Leave Application.pdf");

                }
                else
                {
                    return new EmptyResult();
                }
                // else
                //  {
                // return View("SalaryReport");
                //  }
                // }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
