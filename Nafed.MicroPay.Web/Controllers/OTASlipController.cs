using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers
{
    public class OTASlipController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IOTAService otaService;
        public OTASlipController(IDropdownBindService ddlService, IOTAService otaService)
        {
            this.ddlService = ddlService;
            this.otaService = otaService;
        }
        // GET: OTASlip
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create(int ? empID=null, int ?appNo=null)
        {
            log.Info($"OTASlipController/Create");
            try
            {
                OTASlip otaSlip = new OTASlip();
                var employeeID = empID == null ? (int)userDetail.EmployeeID : empID;
                var AppNo = appNo == null ? 0 : appNo;
                    otaSlip = otaService.GetOTASlip(employeeID.Value, AppNo.Value);
                var approvalSettings = GetEmpProcessApprovalSetting(employeeID.Value, WorkFlowProcess.AttendanceApproval);
                //   otaSlip.approvalSetting = approvalSettings ?? new EmployeeProcessApproval();
                if (approvalSettings != null)
                {
                    otaSlip.ReportingTo = approvalSettings.ReportingTo;
                    otaSlip.ReviewingTo = approvalSettings.ReviewingTo;
                }
                otaSlip.loggedInEmpID = (int)userDetail.EmployeeID;
                otaSlip.EmployeeID = (int)employeeID;
                otaSlip.EmployeeName = otaSlip.EmployeeName == null ? userDetail.FullName : otaSlip.EmployeeName;
                return View("OTASlipForm", otaSlip);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(OTASlip otaSlip, string ButtonType)
        {
            log.Info($"OTASlipController/Create");
            try
            {

                if (otaSlip.loggedInEmpID == otaSlip.EmployeeID || ButtonType == "Reject")
                {
                    ModelState.Remove("IndicatedDate");
                    ModelState.Remove("AttendedDate");
                    ModelState.Remove("AttendedFromTime");
                    ModelState.Remove("AttendedToTime");
                }
                if (otaSlip.ApplicationID == 0)
                {
                    if (ModelState.IsValid)
                    {
                        if (otaSlip.HolidayFromTime > otaSlip.HolidayToTime)
                        {
                            ModelState.AddModelError("HolidayValidation", "From time can not be greater than To time.");
                            return View("OTASlipForm", otaSlip);
                        }
                        if (otaService.OTASlipExists(otaSlip.EmployeeID, otaSlip.HolidayDate.Value))
                        {
                            ModelState.AddModelError("HolidayDate", "Data already exists for this date.");
                            return View("OTASlipForm", otaSlip);
                        }
                        otaSlip.EmployeeName = userDetail.FullName;
                        // otaSlip.EmployeeList = ddlService.GetAllEmployee();
                        otaSlip.EmployeeID = (int)userDetail.EmployeeID;
                        otaSlip.loggedInEmpID = (int)userDetail.EmployeeID;
                        otaSlip.CreatedBy = userDetail.UserID;
                        otaSlip.CreatedOn = DateTime.Now;
                        otaSlip.StatusID = (int)EmpAttendanceStatus.Pending;
                        otaSlip.receiverID = otaSlip.ReportingTo;
                        ProcessWorkFlow workFlow = new ProcessWorkFlow()
                        {
                            SenderID = userDetail.EmployeeID,
                            ReceiverID = otaSlip.receiverID,
                            SenderDepartmentID = userDetail.DepartmentID,
                            SenderDesignationID = userDetail.DesignationID,
                            CreatedBy = userDetail.UserID,
                            EmployeeID = otaSlip.EmployeeID,
                            Scomments = "OTA Application submited Employee.",
                            ProcessID = (int)WorkFlowProcess.AttendanceApproval,
                            StatusID = otaSlip.StatusID
                        };
                        otaSlip._ProcessWorkFlow = workFlow;
                        var result = otaService.InsertOTASlip(otaSlip);
                        if (result)
                        {
                            TempData["Message"] = "Application Submitted successfully.";
                            return RedirectToAction("Index");
                        }
                        else
                            return View("OTASlipForm", otaSlip);
                    }
                    else
                    {
                        return View("OTASlipForm", otaSlip);
                    }
                }
                else if (otaSlip.ApplicationID > 0)
                {
                    if (ModelState.IsValid)
                    {
                        if (otaSlip.HolidayFromTime > otaSlip.HolidayToTime)
                        {
                            ModelState.AddModelError("HolidayValidation", "From time can not be greater than To time.");
                            return View("OTASlipForm", otaSlip);
                        }
                        if (otaSlip.AttendedFromTime > otaSlip.AttendedToTime)
                        {
                            ModelState.AddModelError("AttendedValidation", "From time can not be greater than To time.");
                            return View("OTASlipForm", otaSlip);
                        }
                        var rep_rev_Remark = "";
                        if (otaSlip.loggedInEmpID == otaSlip.ReportingTo)
                        {
                            if (ButtonType == "Accept")
                            {
                                if (otaSlip.ReportingTo == otaSlip.ReviewingTo)
                                {
                                    otaSlip.StatusID = (int)EmpAttendanceStatus.Approved;
                                    rep_rev_Remark = string.IsNullOrEmpty(otaSlip.ReportingRemark) ? otaSlip.ReviewingRemark : otaSlip.ReportingRemark;
                                }
                                else
                                {
                                    if (otaSlip.ReviewingTo.HasValue)
                                    {
                                        otaSlip.StatusID = (int)EmpAttendanceStatus.InProcess;
                                        otaSlip.receiverID = otaSlip.ReviewingTo;
                                    }
                                    else
                                    {
                                        otaSlip.StatusID = (int)EmpAttendanceStatus.Approved;
                                        otaSlip.receiverID = otaSlip.ReportingTo;
                                    }
                                    rep_rev_Remark = otaSlip.ReportingRemark;
                                }
                            }
                            else if (ButtonType == "Reject")
                            {
                                if (otaSlip.ReportingTo == otaSlip.ReviewingTo)
                                {
                                    otaSlip.StatusID = (int)EmpAttendanceStatus.RejectedByAcceptanceAuthority;
                                    rep_rev_Remark = string.IsNullOrEmpty(otaSlip.ReportingRemark) ? otaSlip.ReviewingRemark : otaSlip.ReportingRemark;
                                }
                                else
                                {
                                    if (otaSlip.ReviewingTo.HasValue)
                                    {
                                        otaSlip.StatusID = (int)EmpAttendanceStatus.RejectedByReportingOfficer;
                                        otaSlip.receiverID = otaSlip.ReviewingTo;
                                    }
                                    else
                                    {
                                        otaSlip.StatusID = (int)EmpAttendanceStatus.RejectedByAcceptanceAuthority;
                                        otaSlip.receiverID = otaSlip.ReportingTo;
                                    }
                                    rep_rev_Remark = otaSlip.ReportingRemark;
                                }
                            }
                        }
                        else if (otaSlip.loggedInEmpID == otaSlip.ReviewingTo)
                        {
                            if (ButtonType == "Accept")
                            {
                                otaSlip.StatusID = (int)EmpAttendanceStatus.Approved;
                                otaSlip.receiverID = otaSlip.ReviewingTo;
                                rep_rev_Remark = otaSlip.ReviewingRemark;

                            }
                            else if (ButtonType == "Reject")
                            {
                                otaSlip.StatusID = (int)EmpAttendanceStatus.RejectedByAcceptanceAuthority;
                                otaSlip.receiverID = otaSlip.ReportingTo;
                                rep_rev_Remark = otaSlip.ReviewingRemark;
                            }
                        }
                        ProcessWorkFlow workFlow = new ProcessWorkFlow()
                        {
                            SenderID = userDetail.EmployeeID,
                            ReceiverID = otaSlip.receiverID,
                            SenderDepartmentID = userDetail.DepartmentID,
                            SenderDesignationID = userDetail.DesignationID,
                            CreatedBy = userDetail.UserID,
                            EmployeeID = otaSlip.EmployeeID,
                            Scomments = rep_rev_Remark,
                            ProcessID = (int)WorkFlowProcess.AttendanceApproval,
                            StatusID = otaSlip.StatusID
                        };
                        otaSlip._ProcessWorkFlow = workFlow;

                        otaSlip.EmployeeName = userDetail.FullName;
                        otaSlip.loggedInEmpID = (int)userDetail.EmployeeID;
                        otaSlip.UpdateBy = userDetail.UserID;
                        otaSlip.UpdateOn = DateTime.Now;

                        var result = otaService.UpdateOTASlip(otaSlip);
                        if (result)
                        {

                            if (otaSlip.StatusID == (int)EmpAttendanceStatus.Pending)
                            {
                                TempData["Message"] = "Application Submitted successfully.";
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["Message"] = ButtonType == "Reject" ? "Application Rejected Successfully." : "Application Accepted Successfully";                                
                                return RedirectToAction("Index", "ApprovalRequest");
                            }
                        }
                        else
                            return View("OTASlipForm", otaSlip);
                    }
                    else
                        return View("OTASlipForm", otaSlip);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return View("OTASlipForm", otaSlip);
        }

        public ActionResult GetOTASlipList()
        {
            log.Info($"OTASlipController/GetOTASlipList");
            try
            {
                List<OTASlip> otaSlip = new List<OTASlip>();
                otaSlip = otaService.GetOTASlipList((int)userDetail.EmployeeID);
                return PartialView("_OTASlipGridview", otaSlip);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }        
    }
}