using Model = MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
namespace MicroPay.Web.Controllers.Approval
{
    public class ApprovalRequestController : BaseController
    {
        private readonly IEmployeeAttendancedetailsService empAttendanceService;
        private readonly IMarkAttendance markAttendance;
        private readonly IStaffLeaveDetailsService staffLeaveDetailsService;
        private readonly ILeaveService leaveService;
        private readonly IAppraisalFormService appraisalService;
        private readonly IDropdownBindService ddlServices;
        private readonly IConfirmationFormService confirmationService;
        private readonly ILTCService ltcService;
        private readonly ILoanApplicationService loanApplicationServ;
        private readonly IOTAService otaService;
        private readonly IEPFNominationService epfService;
        private readonly IConveyanceBillService conveyanceService;
        private readonly INRPFLoanService nrpfloanService;
        private readonly IEmployeePFOrganisationService pforgService;
        private readonly ITrainingService trainingService;
        private readonly ISeparationService sepService;
        // GET: Approval
        public ApprovalRequestController(IEmployeeAttendancedetailsService empAttendanceService,
            IMarkAttendance markAttendance, IStaffLeaveDetailsService staffLeaveDetailsService,
            ILeaveService leaveService, IAppraisalFormService appraisalService, IDropdownBindService ddlServices,
            IConfirmationFormService confirmationService, ILTCService ltcService, ILoanApplicationService loanApplicationServ,
            IOTAService otaService, IEPFNominationService epfService, IConveyanceBillService conveyanceService, INRPFLoanService nrpfloanService, IEmployeePFOrganisationService pforgService,
            ITrainingService trainingService, ISeparationService sepService)
        {
            this.empAttendanceService = empAttendanceService;
            this.markAttendance = markAttendance;
            this.staffLeaveDetailsService = staffLeaveDetailsService;
            this.leaveService = leaveService;
            this.appraisalService = appraisalService;
            this.ddlServices = ddlServices;
            this.confirmationService = confirmationService;
            this.ltcService = ltcService;
            this.loanApplicationServ = loanApplicationServ;
            this.otaService = otaService;
            this.epfService = epfService;
            this.conveyanceService = conveyanceService;
            this.nrpfloanService = nrpfloanService;
            this.pforgService = pforgService;
            this.trainingService = trainingService;
            this.sepService = sepService;
        }

        public ActionResult Index()
        {
            log.Info("ApprovalRequestController/Index");
            return View();
        }

        #region Attendance Approval
        [HttpGet]
        public ActionResult _GetAttendanceApprovalRequest()
        {
            log.Info("ApprovalRequestController/_GetAttendanceApprovalRequest");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                var fromDate = Request.Form["FromDate"];
                var toDate = Request.Form["ToDate"];
                EmpAttendance attendance = new EmpAttendance()
                {
                    ProxydateIn = fromDate == null ? DateTime.Now.Date.AddDays(-30) : Convert.ToDateTime(fromDate),
                    ProxyOutDate = toDate == null ? DateTime.Now.Date : Convert.ToDateTime(toDate)
                };
                attendance.EmployeeId = (int)userDetail.EmployeeID;
                if (ModelState.IsValid)
                {
                    var appliedAttendanceCount = 0;
                    approval.attendanceList = empAttendanceService.GetEmployeeAttendanceByManagerID(attendance, out appliedAttendanceCount);
                }
                return PartialView("_AttendanceGridView", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _GetAttendanceApprovalRequest(EmpAttendance attendance)
        {
            log.Info("ApprovalRequestController/_GetAttendanceApprovalRequest");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                var fromDate = Request.Form["FromDate"];
                var toDate = Request.Form["ToDate"];
                attendance.ProxydateIn = fromDate == null || fromDate == "" ? DateTime.Now.Date.AddDays(-30) : Convert.ToDateTime(fromDate);
                attendance.ProxyOutDate = toDate == null || toDate == "" ? DateTime.Now.Date : Convert.ToDateTime(toDate);
                attendance.EmployeeId = (int)userDetail.EmployeeID;
                if (ModelState.IsValid)
                {
                    var appliedAttendanceCount = 0;
                    approval.attendanceList = empAttendanceService.GetEmployeeAttendanceByManagerID(attendance, out appliedAttendanceCount);
                }
                return PartialView("_AttendanceGridView", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostAttendance(EmpAttendance attendance)
        {
            log.Info("ApprovalRequestController/_PostAttendance");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                ProcessWorkFlow workFlow = null;
                if (ModelState.IsValid)
                {
                    #region RecieverID and Remark

                    int? receiverID = 0;
                    var remarks = string.Empty;
                    if (attendance.ApproverType == (int)ApproverType.ReportingTo) // for reporting officer approve/ reject
                    {
                        receiverID = attendance.EmpAttendanceApprovalSettings.ReviewingTo ?? attendance.EmpAttendanceApprovalSettings.ReportingTo;
                        remarks = attendance.ReportingToRemark;
                    }
                    else if (attendance.ReviewerTo.HasValue && attendance.ApproverType == (int)ApproverType.ReviewerTo) // for reviewer approve/ reject
                    {
                        receiverID = attendance.EmpAttendanceApprovalSettings.AcceptanceAuthority ?? (int?)attendance.EmpAttendanceApprovalSettings.ReviewingTo;
                        remarks = attendance.ReviewerToRemark;
                    }
                    else if (!attendance.ReviewerTo.HasValue && attendance.ApproverType == (int)ApproverType.ReviewerTo) // for reviewer approve/ reject
                    {
                        receiverID = attendance.EmpAttendanceApprovalSettings.ReviewingTo ?? (int?)attendance.EmpAttendanceApprovalSettings.ReportingTo;
                        remarks = attendance.ReviewerToRemark;
                    }

                    else if (attendance.EmpAttendanceApprovalSettings.AcceptanceAuthority.HasValue && attendance.ApproverType == (int)ApproverType.AcceptanceAuthority)
                    {
                        receiverID = (int?)attendance.EmpAttendanceApprovalSettings.AcceptanceAuthority;
                        remarks = attendance.AcceptanceAuthRemark;
                    }
                    else if (!attendance.EmpAttendanceApprovalSettings.AcceptanceAuthority.HasValue && attendance.ApproverType == (int)ApproverType.AcceptanceAuthority)
                    {
                        receiverID = (int?)attendance.EmpAttendanceApprovalSettings.AcceptanceAuthority ?? (int?)attendance.EmpAttendanceApprovalSettings.ReviewingTo;
                        remarks = attendance.AcceptanceAuthRemark;
                    }
                    #endregion

                    attendance.Attendancestatus = (int)GetAttendanceStatusID(attendance);
                    workFlow = new ProcessWorkFlow()
                    {
                        SenderID = userDetail.EmployeeID,
                        ReceiverID = receiverID,
                        SenderDepartmentID = userDetail.DepartmentID,
                        SenderDesignationID = userDetail.DesignationID,
                        CreatedBy = userDetail.UserID,
                        EmployeeID = attendance.EmployeeId,
                        Scomments = remarks,
                        ProcessID = (int)WorkFlowProcess.AttendanceApproval,
                        StatusID = attendance.Attendancestatus
                    };
                    attendance._ProcessWorkFlow = workFlow;
                    string msg = "";
                    attendance.SenderName = userDetail.FullName;
                    attendance.SenderDesignation = userDetail.DepartmentName;
                    int res = markAttendance.ApproveRejectAttendance(attendance);

                    if (res > 0)
                    {
                        msg = attendance.ActionType == "accept" ? "Attendance Approved." : "Attendance Rejected.";
                        var fromDate = Request.Form["FromDate"];
                        var toDate = Request.Form["ToDate"];
                        attendance.ProxydateIn = Convert.ToDateTime(fromDate);
                        attendance.ProxyOutDate = Convert.ToDateTime(toDate);
                        attendance.EmployeeId = (int)userDetail.EmployeeID;
                        if (ModelState.IsValid)
                        {
                            var appliedAttendanceCount = 0;
                            approval.attendanceList = empAttendanceService.GetEmployeeAttendanceByManagerID(attendance, out appliedAttendanceCount);
                        }
                    }
                    return Json(new
                    {
                        status = res > 0 ? true : false,
                        type = "success",
                        msg = msg


                    }, JsonRequestBehavior.AllowGet);
                }
                return PartialView("_AttendanceGridView", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                throw ex;
            }
        }
        
        [HttpGet]
        public ActionResult _GetApprovalPopup(int empID, int empAttendanceID, int status, string reportingToRemark, string reviewerToRemark, string acceptanceAuthRemark, string actionType, int? approverType = null, int? reportingTo = null, int? reviewerTo = null, int? acceptanceAuthority = null)
        {
            log.Info("ApprovalRequestController/_GetApprovalPopup");
            try
            {
                EmpAttendance attendanceData = new EmpAttendance
                {
                    EmpAttendanceID = empAttendanceID,
                    EmployeeId = empID,
                    Attendancestatus = status,
                    ApproverType = approverType,
                    ActionType = actionType
                };
                attendanceData.EmpAttendanceApprovalSettings.ReportingTo = (int)reportingTo;
                attendanceData.EmpAttendanceApprovalSettings.ReviewingTo = reviewerTo;
                attendanceData.EmpAttendanceApprovalSettings.AcceptanceAuthority = acceptanceAuthority;
                return PartialView("_ApprovalPopup", attendanceData);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                throw ex;
            }
        }

        private EmpAttendanceStatus GetAttendanceStatusID(EmpAttendance empAttendance)
        {
            if (empAttendance.ActionType == "accept")
            {
                if (empAttendance.ApproverType == (int)ApproverType.ReportingTo)
                {
                    return EmpAttendanceStatus.InProcess;
                }
                else if (empAttendance.ApproverType == (int)ApproverType.ReviewerTo)
                {
                    return EmpAttendanceStatus.AcceptedByReviewerOfficer;
                }
                else if (empAttendance.ApproverType == (int)ApproverType.AcceptanceAuthority)
                {
                    return EmpAttendanceStatus.Approved;
                }
            }
            else if (empAttendance.ActionType == "reject")
            {
                if (empAttendance.ApproverType == (int)ApproverType.ReportingTo)
                {
                    return EmpAttendanceStatus.RejectedByReportingOfficer;
                }
                else if (empAttendance.ApproverType == (int)ApproverType.ReviewerTo)
                {
                    return EmpAttendanceStatus.RejectedByReviwerOfficer;
                }
                else if (empAttendance.ApproverType == (int)ApproverType.AcceptanceAuthority)
                {
                    return EmpAttendanceStatus.RejectedByAcceptanceAuthority;
                }
            }
            return (EmpAttendanceStatus)empAttendance.Attendancestatus;
        }

        #endregion

        #region Leave Approval

        [HttpGet]
        public ActionResult _GetLeaveApprovalRequest()
        {
            log.Info("ApprovalRequestController/_GetLeaveApprovalRequest");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                var fromDate = Request.Form["FromDate"];
                var toDate = Request.Form["ToDate"];
                var ProxydateIn = fromDate == null ? DateTime.Now.Date.AddDays(-30) : Convert.ToDateTime(fromDate);
                var ProxyOutDate = toDate == null ? DateTime.Now.Date : Convert.ToDateTime(toDate);
                //attendance.EmployeeId = (int)userDetail.EmployeeID;
                EmployeeLeave empLeave = new EmployeeLeave()
                {
                    DateFrom = ProxydateIn,
                    DateTo = ProxyOutDate
                };
                if (ModelState.IsValid)
                {
                    approval.leaveList = staffLeaveDetailsService.GetStaffLeaveDetailsList(empLeave, (int?)userDetail.ReportingTo, (int?)userDetail.ReviwerTo, (int)userDetail.EmployeeID);
                    //   approval.leaveList.OrderByDescending(x => x.CreatedOn).ThenBy(x => x.StatusID).ToList();

                }
                TempData["leaveList"] = approval.leaveList.Where(x => x.StatusID != 8);
                return PartialView("_LeaveGridView", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _GetLeaveApprovalRequest(EmpAttendance attendance)
        {
            log.Info("ApprovalRequestController/_GetLeaveApprovalRequest");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                var fromDate = Request.Form["FromDate"];
                var toDate = Request.Form["ToDate"];
                attendance.ProxydateIn = fromDate == null || fromDate == "" ? DateTime.Now.Date.AddDays(-30) : Convert.ToDateTime(fromDate);
                attendance.ProxyOutDate = toDate == null || toDate == "" ? DateTime.Now.Date : Convert.ToDateTime(toDate);
                // attendance.EmployeeId = (int)userDetail.EmployeeID;
                EmployeeLeave empLeave = new EmployeeLeave()
                {
                    DateFrom = attendance.ProxydateIn,
                    DateTo = attendance.ProxyOutDate
                };

                if (ModelState.IsValid)
                {
                    approval.leaveList = staffLeaveDetailsService.GetStaffLeaveDetailsList(empLeave, (int?)userDetail.ReportingTo, (int?)userDetail.ReviwerTo, (int)userDetail.EmployeeID);
                }
                TempData["leaveList"] = approval.leaveList;
                return PartialView("_LeaveGridView", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult _GetLeaveApprovalPopup(int empID, int empLeaveID, int status, string reportingToRemark, string reviewerToRemark, string actionType, int? approverType = null, int? reportingTo = null, int? reviewerTo = null, int? acceptanceAuthority = null, int? approvalRequiredUpto = null, string leaveCategoryName = null)
        {
            log.Info("ApprovalRequestController/_GetLeaveApprovalPopup");
            try
            {
                var leaveList = TempData["leaveList"] as IEnumerable<EmployeeLeave>;
                TempData.Keep("leaveList");

                var empLeave = leaveList.Where(x => x.LeaveID == empLeaveID).FirstOrDefault();
                EmployeeLeave leaveData = new EmployeeLeave
                {
                    LeaveID = empLeaveID,
                    EmployeeId = empID,
                    StatusID = status,
                    ApproverType = approverType,
                    ActionType = actionType,
                    ReportingTo = reportingTo,
                    ReviewerTo = reviewerTo,
                    AcceptanceAuthority = acceptanceAuthority,
                    ApprovalRequiredUpto = (int)approvalRequiredUpto,
                    ReporotingToRemark = reportingToRemark,
                    ReviewerToRemark = reviewerToRemark,
                    LeaveCategoryName = leaveCategoryName,
                    DateFrom = empLeave.DateFrom,
                    DateTo = empLeave.DateTo,
                    EmployeeName = empLeave.EmployeeName,
                    Unit = empLeave.Unit,
                    Reason = empLeave.Reason,
                    EmployeeCode = empLeave.EmployeeCode
                };
                return PartialView("_LeaveApprovalPopup", leaveData);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                throw ex;
            }
        }

        private EmpLeaveStatus GetStatusID(EmployeeLeave empLeave)
        {
            if (empLeave.ActionType == "accept")
            {
                if (empLeave.ApproverType == (int)ApproverType.ReportingTo)
                {
                    if (empLeave.ReportingTo.HasValue && empLeave.ApprovalRequiredUpto == 1)
                    {
                        return EmpLeaveStatus.Approved;
                    }
                    else if ((empLeave.ReportingTo.HasValue && empLeave.ReviewerTo.HasValue) && empLeave.ApprovalRequiredUpto == 2)
                    {
                        return EmpLeaveStatus.InProcess;
                    }
                    else if ((empLeave.ReportingTo.HasValue && !empLeave.ReviewerTo.HasValue) && empLeave.ApprovalRequiredUpto == 2)
                    {
                        return EmpLeaveStatus.Approved;
                    }
                    else if ((empLeave.ReportingTo.HasValue && empLeave.ReviewerTo.HasValue) && empLeave.ApprovalRequiredUpto == 3)
                    {
                        return EmpLeaveStatus.InProcess;
                    }
                    else if ((empLeave.ReportingTo.HasValue && !empLeave.ReviewerTo.HasValue) && empLeave.ApprovalRequiredUpto == 3)
                    {
                        return EmpLeaveStatus.Approved;
                    }
                }
                else if (empLeave.ApproverType == (int)ApproverType.ReviewerTo)
                {
                    if (empLeave.ReviewerTo.HasValue && empLeave.ApprovalRequiredUpto == 2)
                    {
                        return EmpLeaveStatus.Approved;
                    }
                    else if (empLeave.ApprovalRequiredUpto == 3 && empLeave.AcceptanceAuthority.HasValue)
                    {
                        return EmpLeaveStatus.AcceptedByReviewerOfficer;
                    }
                    else if (empLeave.ApprovalRequiredUpto == 3 && !empLeave.AcceptanceAuthority.HasValue)
                    {
                        return EmpLeaveStatus.Approved;
                    }
                }
                else if (empLeave.ApproverType == (int)ApproverType.AcceptanceAuthority)
                {
                    if (empLeave.AcceptanceAuthority.HasValue)
                    {
                        return EmpLeaveStatus.Approved;
                    }
                }
            }
            else if (empLeave.ActionType == "reject")
            {
                if (empLeave.ApproverType == (int)ApproverType.ReportingTo)
                {
                    if (empLeave.ReportingTo.HasValue && empLeave.ApprovalRequiredUpto == 1)
                    {
                        return EmpLeaveStatus.RejectedByAcceptanceAuthority;
                    }
                    else if ((empLeave.ReportingTo.HasValue && !empLeave.ReviewerTo.HasValue) && empLeave.ApprovalRequiredUpto == 2)
                    {
                        return EmpLeaveStatus.RejectedByAcceptanceAuthority;
                    }
                    else if ((empLeave.ReportingTo.HasValue && empLeave.ReviewerTo.HasValue) && empLeave.ApprovalRequiredUpto == 2)
                    {
                        return EmpLeaveStatus.RejectedByReportingOfficer;
                    }
                    else if ((empLeave.ReportingTo.HasValue && empLeave.ReviewerTo.HasValue) && empLeave.ApprovalRequiredUpto == 3)
                    {
                        return EmpLeaveStatus.RejectedByReportingOfficer;
                    }
                    else if ((empLeave.ReportingTo.HasValue && !empLeave.ReviewerTo.HasValue) && empLeave.ApprovalRequiredUpto == 3)
                    {
                        return EmpLeaveStatus.RejectedByAcceptanceAuthority;
                    }
                }
                else if (empLeave.ApproverType == (int)ApproverType.ReviewerTo)
                {
                    if ((empLeave.ReviewerTo.HasValue && !empLeave.AcceptanceAuthority.HasValue) && empLeave.ApprovalRequiredUpto == 3)
                    {
                        return EmpLeaveStatus.RejectedByAcceptanceAuthority;
                    }
                    else if (empLeave.ApprovalRequiredUpto == 3 && empLeave.AcceptanceAuthority.HasValue)
                    {
                        return EmpLeaveStatus.RejectedByReviwerOfficer;
                    }
                    else if (empLeave.ReviewerTo.HasValue && empLeave.ApprovalRequiredUpto == 2)
                    {
                        return EmpLeaveStatus.RejectedByAcceptanceAuthority;
                    }

                }
                else if (empLeave.ApproverType == (int)ApproverType.AcceptanceAuthority)
                {
                    if (empLeave.AcceptanceAuthority.HasValue)
                    {
                        return EmpLeaveStatus.RejectedByAcceptanceAuthority;
                    }
                }
            }
            return (EmpLeaveStatus)empLeave.StatusID;
        }

        [HttpPost]
        public ActionResult _PostLeave(EmployeeLeave empLeave)
        {
            log.Info("ApprovalRequestController/ApproveRejectAttendance");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                ModelState.Remove("DateFrom");
                ModelState.Remove("Reason");
                ModelState.Remove("DateTo");
                ProcessWorkFlow workFlow = null;
                if (ModelState.IsValid)
                {
                    #region RecieverID and Remark
                    int? receiverID = 0;
                    var remarks = string.Empty;
                    if (empLeave.ReportingTo.HasValue && empLeave.ApproverType == (int)ApproverType.ReportingTo) // for reporting officer approve/ reject
                    {
                        if (empLeave.ApprovalRequiredUpto == 1)
                        {
                            receiverID = empLeave.ReportingTo;
                            remarks = empLeave.ReporotingToRemark;
                        }
                        else if (empLeave.ApprovalRequiredUpto == 2)
                        {
                            receiverID = empLeave.ReviewerTo ?? empLeave.ReportingTo;
                            remarks = empLeave.ReporotingToRemark;
                        }
                        else if (empLeave.ApprovalRequiredUpto == 3)
                        {
                            receiverID = empLeave.ReviewerTo ?? empLeave.ReportingTo;
                            remarks = empLeave.ReporotingToRemark;
                        }
                    }
                    else if (empLeave.ReviewerTo.HasValue && empLeave.ApproverType == (int)ApproverType.ReviewerTo) // for reviewer approve/ reject
                    {
                        if (empLeave.ApprovalRequiredUpto == 2)
                        {
                            receiverID = empLeave.AcceptanceAuthority ?? (int?)empLeave.ReviewerTo;
                            remarks = empLeave.ReviewerToRemark;
                        }
                        else if (empLeave.ApprovalRequiredUpto == 3)
                        {
                            receiverID = empLeave.AcceptanceAuthority ?? (int?)empLeave.ReviewerTo;
                            remarks = empLeave.ReviewerToRemark;
                        }
                    }
                    else if (empLeave.AcceptanceAuthority.HasValue && empLeave.ApproverType == (int)ApproverType.AcceptanceAuthority)
                    {
                        receiverID = (int?)empLeave.AcceptanceAuthority;
                        remarks = empLeave.AcceptanceAuthorityRemark;
                    }
                    #endregion

                    workFlow = new ProcessWorkFlow()
                    {
                        SenderID = userDetail.EmployeeID,
                        ReceiverID = receiverID,
                        SenderDepartmentID = userDetail.DepartmentID,
                        SenderDesignationID = userDetail.DesignationID,
                        CreatedBy = userDetail.UserID,
                        EmployeeID = empLeave.EmployeeId,
                        Scomments = remarks,
                        ProcessID = (int)WorkFlowProcess.LeaveApproval,
                        StatusID = (int)GetStatusID(empLeave)
                    };

                    bool updatestatus = false;
                    string msg = "";
                    empLeave.StatusID = (int)GetStatusID(empLeave);
                    empLeave._ProcessWorkFlow = workFlow;
                    updatestatus = leaveService.UpdateLeaveStatus(empLeave, userDetail.UserID);
                    if (updatestatus)
                    {
                        updatestatus = leaveService.UpdateLeaveTransStatus(empLeave, userDetail.UserID);
                        msg = empLeave.ActionType == "accept" ? "Leave Approved." : "Leave Rejected.";
                        int mailSenderID = 0, mailRceiverID = 0;
                        if (empLeave.StatusID == (int)EmpLeaveStatus.Approved || empLeave.StatusID == (int)EmpLeaveStatus.RejectedByReportingOfficer || empLeave.StatusID == (int)EmpLeaveStatus.RejectedByReviwerOfficer || empLeave.StatusID == (int)EmpLeaveStatus.RejectedByAcceptanceAuthority)
                        {
                            mailSenderID = (int)workFlow.SenderID;
                            mailRceiverID = workFlow.EmployeeID;
                        }
                        else
                        {
                            mailSenderID = (int)workFlow.SenderID;
                            mailRceiverID = (int)workFlow.ReceiverID;
                        }
                        leaveService.SenderSendMail(mailSenderID, empLeave.LeaveCategoryName, empLeave, "Approval");

                        empLeave.ReportingOfficerName = userDetail.FullName;
                        empLeave.DesignationName = userDetail.DepartmentName;
                        leaveService.RecieverSendMail(mailRceiverID, empLeave.LeaveCategoryName, empLeave, "Approval");

                    }
                    return Json(new
                    {
                        status = updatestatus,
                        type = "success",
                        msg = msg


                    }, JsonRequestBehavior.AllowGet);
                }
                return PartialView("_LeaveGridView", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                throw ex;
            }
        }
        #endregion

        #region Appraisal Approval

        public ActionResult _AppraisalApprovalTab()
        {
            log.Info($"ApprovalRequestController/_AppraisalApprovalTab");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                //  var res = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.Appraisal);
                AppraisalFormApprovalFilter filters = new AppraisalFormApprovalFilter() { loggedInEmployeeID = (int)userDetail.EmployeeID };
                approval.AppraisalFilters = filters;
                filters.appraisalForms = ddlServices.ddlAppraisalForm();
                filters.employees = ddlServices.GetAllEmployeeByProcessID((int)userDetail.EmployeeID, WorkFlowProcess.Appraisal);
                int sno = 1;
                filters.reportingYrs = ExtensionMethods.GetFinancialYrList(2019, DateTime.Now.Year).Select(x => new SelectListModel
                { id = sno++, value = x }).OrderByDescending(x => x.id).ToList();

                //    = new List<SelectListModel>()
                //{
                //    new SelectListModel { id = 1, value = "2019-2020" },
                //    new SelectListModel { id = 2, value = "2020-2021" }}.OrderByDescending(x=>x.id).ToList();               
                //approval.FormAttributes = appraisalService.GetFormSubmittionDueDate(filters.selectedReportingYear);
                // var appraisalForms = appraisalService.GetAppraisalFormHdr(approval.AppraisalFilters);
                //  approval.appraisalForms = appraisalForms.ToList();
                //approval.appraisalForms = appraisalForms.Where(x => x.StatusID != (int)AppraisalFormState.SubmitedByAcceptanceAuth && x.StatusID != 1 && x.StatusID != 2).ToList();
                approval.AppraisalFilters = filters;

                return PartialView("_AppraisalFormTab", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult _PostAppraisalFormFilters(AppraisalFormApprovalFilter filters, FormCollection frm)
        {
            log.Info($"ApprovalRequestController/_PostAppraisalFormFilters");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                var appraisalForms = appraisalService.GetAppraisalFormHdr(filters);

                approval.appraisalForms = appraisalForms.ToList();

                approval.FormAttributes = appraisalService.GetFormSubmittionDueDate(filters.selectedReportingYear);
                return PartialView("_AppraisalFormGridView", approval);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region Skill Assessment Approval 

        public ActionResult _SkillAssessmentApprovalTab()
        {
            log.Info($"AppraisalController/_SkillAssessmentApprovalTab");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                //var getEmpProcessApprovalSetting = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.Appraisal);
                SkillAssessmentApprovalFilters filters = new SkillAssessmentApprovalFilters() { loggedInEmployeeID = (int)userDetail.EmployeeID };
                approval.skillAssessmentFilters = filters;
                filters.employees = ddlServices.GetAllEmployeeByProcessID((int)userDetail.EmployeeID, WorkFlowProcess.Appraisal);
                int sno = 1;
                filters.reportingYrs = filters.reportingYrs = ExtensionMethods.GetFinancialYrList(2019, DateTime.Now.Year).Select(x => new SelectListModel
                { id = sno++, value = x }).OrderByDescending(x => x.id).ToList();

                //= new List<SelectListModel>() {
                //new SelectListModel { id = 1, value = "2019-2020" },
                //new SelectListModel { id = 2, value = "2020-2021" } };
                approval.skillAssessmentFilters = filters;
                //  var appraisalForms = appraisalService.GetAppraisalFormHdr(approval.AppraisalFilters);
                var skillAssessmentForms = appraisalService.GetSkillAssessmentFormHdr(approval.skillAssessmentFilters);
                approval.skillAssessmentForms = skillAssessmentForms.ToList();
                return PartialView("_SkillAssessmentTab", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostSkillAssessmentFilters(SkillAssessmentApprovalFilters filters, FormCollection frm)
        {
            log.Info($"ApprovalRequestController/_PostAppraisalFormFilters");
            try
            {
                var reportingYr = frm["hdnyear"].ToString();
                filters.reportingYr = reportingYr;
                // filters.loggedInEmployeeID =(int) userDetail.EmployeeID;
                var skillAssessmentForms = appraisalService.GetSkillAssessmentFormHdr(filters);
                return PartialView("_SkillAssessmentGridView", skillAssessmentForms);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region Confirmation Approval
        public ActionResult _ConfirmationApprovalTab()
        {
            log.Info($"AppraisalController/_ConfirmationApprovalTab");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                //  var res = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.Appraisal);
                ConfirmationFormApprovalFilter filters = new ConfirmationFormApprovalFilter() { loggedInEmployeeID = (int)userDetail.EmployeeID };

                approval.ConfirmationFilters = filters;
                var confirmationForms = confirmationService.GetConfirmationFormHdr(approval.ConfirmationFilters);

                filters.confirmationForms = new List<SelectListModel>() {
                   new SelectListModel { id = 1, value = "Form A" } ,
                   new SelectListModel { id = 2, value = "Form B" }};

                filters.processList = new List<SelectListModel>() {
                   new SelectListModel { id = 6, value = "Appointment Confirmation" } ,
                   new SelectListModel { id = 7, value = "Promotional Confirmation" }};


                filters.employees = confirmationService.GetEmployeeFilter((int)userDetail.EmployeeID);

                approval.confirmationFormsHdr = confirmationForms.ToList();

                approval.ConfirmationFilters = filters;

                return PartialView("_ConfirmationFormTab", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _PostConfirmationFormFilters(ConfirmationFormApprovalFilter filters)
        {
            log.Info($"AppraisalController/_PostConfirmationFormFilters");
            try
            {
                if (filters.confirmationFormState == ConfirmationFormState.Pending)
                    filters.StatusId = (int)ConfirmationFormState.Pending;
                else if (filters.confirmationFormState == ConfirmationFormState.SavedByReporting)
                    filters.StatusId = (int)ConfirmationFormState.SavedByReporting;
                else if (filters.confirmationFormState == ConfirmationFormState.SubmitedByReporting)
                    filters.StatusId = (int)ConfirmationFormState.SubmitedByReporting;
                else if (filters.confirmationFormState == ConfirmationFormState.SavedByReviewer)
                    filters.StatusId = (int)ConfirmationFormState.SavedByReviewer;
                else if (filters.confirmationFormState == ConfirmationFormState.SubmitedByReviewer)
                    filters.StatusId = (int)ConfirmationFormState.SubmitedByReviewer;
                else if (filters.confirmationFormState == ConfirmationFormState.RejectedByReporting)
                    filters.StatusId = (int)ConfirmationFormState.RejectedByReporting;
                else if (filters.confirmationFormState == ConfirmationFormState.RejectedByReviewer)
                    filters.StatusId = (int)ConfirmationFormState.RejectedByReviewer;

                var confirmationForms = confirmationService.GetConfirmationFormHdr(filters);
                return PartialView("_ConfirmationFormGridView", confirmationForms);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region LCT Approval
        [HttpGet]
        public ActionResult _GetLTCApprovalRequest()
        {
            log.Info("ApprovalRequestController/_GetLTCApprovalRequest");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                var fromDate = Request.Form["FromDate"];
                var toDate = Request.Form["ToDate"];
                var FromDt = fromDate == null ? DateTime.Now.Date.AddDays(-30) : Convert.ToDateTime(fromDate);
                var ToDt = toDate == null ? DateTime.Now.Date : Convert.ToDateTime(toDate);

                approval.ltcFilter = new LTC()
                {
                    DateAvailLTC = FromDt,
                    DateofReturn = ToDt,
                    loggedInEmpID = (int)userDetail.EmployeeID
                };
                List<LTC> approvalLTC = new List<LTC>();
                approvalLTC = ltcService.GetLTCListForApproval(approval.ltcFilter).ToList();

                return PartialView("_LTCFormGridView", approvalLTC);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region Loan Application Approval
        [HttpGet]
        public ActionResult GetLoanApplicationPendingList()
        {
            log.Info($"ApprovalRequestController/GetLoanApplicationPendingList");
            try
            {
                List<LoanApplication> loanApp = new List<LoanApplication>();
                var getLoanApplication = loanApplicationServ.GetLoanApplicationForApproval((int)userDetail.EmployeeID);
                loanApp = getLoanApplication;
                return PartialView("_LoanApplicationPendingList", loanApp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult AcceptRejectApplication(int empID, int appNo, byte statusID, int reportingTo)
        {
            log.Info($"ApprovalRequestController/AcceptRejectApplication/empID={empID}/appNo={appNo}");
            try
            {
                LoanApplication loanApp = new LoanApplication();
                loanApp.EmployeeID = empID;
                loanApp.ApplicationID = appNo;
                loanApp.StatusID = statusID;
                loanApp.approvalSetting.ReportingTo = reportingTo;
                return PartialView("_LoanAcceptRejectPopup", loanApp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        [HttpPost]
        public ActionResult AcceptRejectApplication(LoanApplication loanApp)
        {
            log.Info($"ApprovalRequestController/AcceptRejectApplication");
            try
            {
                if (string.IsNullOrEmpty(loanApp.Remarks))
                {
                    ModelState.AddModelError("RemarkValidator", "Please Enter Remark.");
                    return PartialView("_LoanAcceptRejectPopup", loanApp);
                }
                else
                {
                    ProcessWorkFlow workFlow = null;

                    workFlow = new ProcessWorkFlow()
                    {
                        SenderID = userDetail.EmployeeID,
                        ReceiverID = loanApp.approvalSetting.ReportingTo,
                        SenderDepartmentID = userDetail.DepartmentID,
                        SenderDesignationID = userDetail.DesignationID,
                        CreatedBy = userDetail.UserID,
                        EmployeeID = loanApp.EmployeeID,
                        Scomments = loanApp.Remarks,
                        ProcessID = (int)WorkFlowProcess.LoanApplication,
                        StatusID = loanApp.StatusID
                    };
                    loanApp._ProcessWorkFlow = workFlow;

                    loanApp.CreatedBy = userDetail.UserID;
                    loanApp.CreatedOn = DateTime.Now;
                    bool result = loanApplicationServ.InsertLoanApplicationHistory(loanApp);
                    if (result)
                    {
                        var msg = loanApp.StatusID == 2 ? "Loan Application Accepted." : "Loan Application Rejected.";
                        return Json(new { status = 1, type = "success", msg = msg }, JsonRequestBehavior.AllowGet);
                    }
                }
                return PartialView("_LoanAcceptRejectPopup", loanApp);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        #endregion

        #region OTA Slip Approval 

        public ActionResult GetOTAPendingList()
        {
            log.Info($"AppraisalController/_OTAApprovalTab");
            try
            {
                var fromDate = Request.Form["FromDate"];
                var toDate = Request.Form["ToDate"];
                CommonFilter otaFilter = new CommonFilter()
                {
                    FromDate = fromDate == null ? DateTime.Now.Date.AddDays(-30) : Convert.ToDateTime(fromDate),
                    ToDate = toDate == null ? DateTime.Now.Date : Convert.ToDateTime(toDate),
                    loggedInEmployee = (int)userDetail.EmployeeID

                };
                List<OTASlip> otaSlip = new List<OTASlip>();
                otaSlip = otaService.GetOTASlipDetail(otaFilter);

                return PartialView("_OTASlipGridView", otaSlip);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _OTAApprovalTab(CommonFilter otaFilter)
        {
            log.Info($"ApprovalRequestController/_OTAApprovalTab");
            try
            {
                //var skillAssessmentForms = appraisalService.GetSkillAssessmentFormHdr(filters);
                //return PartialView("_SkillAssessmentGridView", skillAssessmentForms);
                return Content("");

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region EPF Application Approval 

        public ActionResult GetEPFPendingList()
        {
            log.Info($"AppraisalController/GetEPFPendingList");
            try
            {
                var fromDate = Request.Form["FromDate"];
                var toDate = Request.Form["ToDate"];
                CommonFilter otaFilter = new CommonFilter()
                {
                    FromDate = fromDate == null ? DateTime.Now.Date.AddDays(-30) : Convert.ToDateTime(fromDate),
                    ToDate = toDate == null ? DateTime.Now.Date : Convert.ToDateTime(toDate),
                    loggedInEmployee = (int)userDetail.EmployeeID

                };
                List<EPFNomination> epfNomList = new List<EPFNomination>();
                epfNomList = epfService.GetEPFApprovalList(otaFilter);

                return PartialView("_EPFGridView", epfNomList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region Conveyance Approval
        public ActionResult _ConveyanceApprovalTab()
        {
            log.Info("ApprovalRequestController/_ConveyanceApprovalTab");
            Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
            ConveyanceBillApprovalFilter filters = new ConveyanceBillApprovalFilter() { loggedInEmployeeID = (int)userDetail.EmployeeID };
            approval.conveyanceBillFilters = filters;
            var conveyanceForms = conveyanceService.GetConveyanceFormHdr(approval.conveyanceBillFilters);
            filters.employees = conveyanceService.GetEmployeeFilter((int)userDetail.EmployeeID);
            approval.conveyanceBillFormsHdr = conveyanceForms.ToList();
            approval.conveyanceBillFilters = filters;
            return PartialView("_ConveyanceFormTab", approval);
        }

        public ActionResult _PostConveyanceFormFilters(ConveyanceBillApprovalFilter filters)
        {
            log.Info($"AppraisalController/_PostConfirmationFormFilters");
            try
            {
                if (filters.conveyanceFormState == ConveyanceFormStatus.SubmitedByEmployee)
                    filters.StatusId = (int)ConveyanceFormStatus.SubmitedByEmployee;
                else if (filters.conveyanceFormState == ConveyanceFormStatus.SavedByEmployee)
                    filters.StatusId = (int)ConveyanceFormStatus.SavedByEmployee;
                else if (filters.conveyanceFormState == ConveyanceFormStatus.SavedBySectionalHead)
                    filters.StatusId = (int)ConveyanceFormStatus.SavedBySectionalHead;
                else if (filters.conveyanceFormState == ConveyanceFormStatus.SubmitedBySectionalHead)
                    filters.StatusId = (int)ConveyanceFormStatus.SubmitedBySectionalHead;
                else if (filters.conveyanceFormState == ConveyanceFormStatus.SavedByDivisionalHead)
                    filters.StatusId = (int)ConveyanceFormStatus.SavedByDivisionalHead;
                else if (filters.conveyanceFormState == ConveyanceFormStatus.SubmitedByDivisionalHead)
                    filters.StatusId = (int)ConveyanceFormStatus.SubmitedByDivisionalHead;

                var conveyanceForms = conveyanceService.GetConveyanceFormHdr(filters);
                return PartialView("_ConveyanceFormGridView", conveyanceForms.ToList());
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region NON REFUNDABLE PF LOAN

        public ActionResult _NRPFLoanTab()
        {
            log.Info($"ApprovalRequestController/_NRPFLoanTab");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                NRPFLoanFormApprovalFilter filters = new NRPFLoanFormApprovalFilter() { loggedInEmployeeID = (int)userDetail.EmployeeID };
                approval.NRPFloanFilters = filters;
                var Forms = nrpfloanService.GetNRPFLoanHdr(approval.NRPFloanFilters);
                filters.employees = ddlServices.GetAllEmployeeByProcessID((int)userDetail.EmployeeID, WorkFlowProcess.NonRefundablePFLoan);

                approval.NRPFloanHdr = Forms.ToList();
                approval.NRPFloanFilters = filters;
                return PartialView("_NRPFLoanTab", approval);


            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _PostNRPFLoanFilters(NRPFLoanFormApprovalFilter filters)
        {
            log.Info($"ApprovalRequestController/_PostNRPFLoanFilters");
            try
            {
                var forms = nrpfloanService.GetNRPFLoanHdr(filters);
                return PartialView("_NRPFLoanGridView", forms);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region Form11

        public ActionResult _Form11Tab()
        {
            log.Info($"ApprovalRequestController/_Form11Tab");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                Form11FormApprovalFilter filters = new Form11FormApprovalFilter() { loggedInEmployeeID = (int)userDetail.EmployeeID };
                approval.Form11Filters = filters;
                var Forms = pforgService.GetForm11Hdr(approval.Form11Filters);

                filters.employees = ddlServices.GetAllEmployeeByProcessID((int)userDetail.EmployeeID, WorkFlowProcess.Form11);


                approval.Form11Hdr = Forms.ToList();
                approval.Form11Filters = filters;

                return PartialView("_Form11Tab", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _PostForm11Filters(Form11FormApprovalFilter filters)
        {
            log.Info($"ApprovalRequestController/_PostForm11Filters");
            try
            {
                var forms = pforgService.GetForm11Hdr(filters);
                return PartialView("_Form11GridView", forms);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        [HttpGet]
        public ActionResult Viewhistory(int referenceID, int empID)
        {
            try
            {
                log.Info($"LeaveBalanceAsOfNowController/Viewhistory/{referenceID}/{empID}");
                ModelState.Clear();

                var processworkflow = appraisalService.GetAppraisalHistory(referenceID, empID, 5);
                return PartialView("_AppraisalHistoryPopup", processworkflow);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region Training Approval

        [HttpGet]
        public ActionResult _GetTrainingApprovalRequest()
        {
            log.Info("ApprovalRequestController/_GetTrainingApprovalRequest");
            try
            {
                var fromDate = Request.Form["FromDate"];
                var toDate = Request.Form["ToDate"];
                CommonFilter cFilter = new CommonFilter()
                {
                    FromDate = fromDate == null || fromDate == "" ? DateTime.Now.Date.AddDays(-90) : Convert.ToDateTime(fromDate),
                    ToDate = toDate == null || toDate == "" ? DateTime.Now.Date : Convert.ToDateTime(toDate),
                    loggedInEmployee = (int)userDetail.EmployeeID

                };
                Model.ApprovalRequestVM approvalVm = new Model.ApprovalRequestVM();
                approvalVm.participantsData = trainingService.GetTrainingNomineePending(cFilter);

                return PartialView("_TrainingParticipantGridView", approvalVm);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _GetTrainingApprovalRequest(CommonFilter cFilter)
        {
            log.Info("ApprovalRequestController/_GetLeaveApprovalRequest");
            try
            {
                Model.ApprovalRequestVM approvalVm = new Model.ApprovalRequestVM();

                cFilter.FromDate = cFilter.FromDate == null ? DateTime.Now.Date.AddDays(-90) : Convert.ToDateTime(cFilter.FromDate);
                cFilter.ToDate = cFilter.ToDate == null ? DateTime.Now.Date : Convert.ToDateTime(cFilter.ToDate);
                cFilter.loggedInEmployee = (int)userDetail.EmployeeID;
                approvalVm.participantsData = trainingService.GetTrainingNomineePending(cFilter);
                return PartialView("_TrainingParticipantGridView", approvalVm);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult _GetTrainingApprovalPopup(int employeeid, int participantID, string actionType, int? approverType = null, int? reportingTo = null, int? reviewerTo = null)
        {
            log.Info("ApprovalRequestController/_GetTrainingApprovalPopup");
            try
            {
                EmployeeLeave participantData = new EmployeeLeave
                {
                    EmployeeId = employeeid,
                    LeaveID = participantID,
                    ApproverType = approverType,
                    ActionType = actionType,
                    ReportingTo = reportingTo,
                    ReviewerTo = reviewerTo,
                };
                return PartialView("_TrainingPopup", participantData);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostTrainingApproval(EmployeeLeave tParticipantAppr)
        {
            log.Info("ApprovalRequestController/_PostTrainingApproval");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                ProcessWorkFlow workFlow = null;
                // if (ModelState.IsValid)
                //   {
                #region RecieverID and Remark

                int? receiverID = 0;
                var remarks = string.Empty;
                if (tParticipantAppr.ApproverType == (int)ApproverType.ReportingTo) // for reporting officer approve/ reject
                {
                    if (tParticipantAppr.ActionType == "accept")
                    {
                        receiverID = tParticipantAppr.ReviewerTo ?? tParticipantAppr.ReportingTo;
                        tParticipantAppr.StatusID = (int)EmpLeaveStatus.InProcess;
                    }
                    else if (tParticipantAppr.ActionType == "reject")
                    {
                        receiverID = (int)tParticipantAppr.EmployeeId;
                        tParticipantAppr.StatusID = (int)EmpLeaveStatus.RejectedByReportingOfficer;
                    }
                    remarks = tParticipantAppr.ReviewerToRemark;
                }
                else if (tParticipantAppr.ApproverType == (int)ApproverType.ReviewerTo) // for reviewer approve/ reject
                {
                    if (tParticipantAppr.ActionType == "accept")
                    {
                        receiverID = tParticipantAppr.EmployeeId;
                        tParticipantAppr.StatusID = (int)EmpLeaveStatus.Approved;
                    }
                    else if (tParticipantAppr.ActionType == "reject")
                    {
                        receiverID = tParticipantAppr.EmployeeId;
                        tParticipantAppr.StatusID = (int)EmpLeaveStatus.RejectedByAcceptanceAuthority;
                    }
                    remarks = tParticipantAppr.ReviewerToRemark;
                }
                #endregion

                workFlow = new ProcessWorkFlow()
                {
                    SenderID = userDetail.EmployeeID,
                    ReceiverID = receiverID,
                    SenderDepartmentID = userDetail.DepartmentID,
                    SenderDesignationID = userDetail.DesignationID,
                    CreatedBy = userDetail.UserID,
                    EmployeeID = tParticipantAppr.EmployeeId,
                    Scomments = remarks,
                    ProcessID = (int)WorkFlowProcess.LeaveApproval,
                    StatusID = tParticipantAppr.StatusID
                };
                tParticipantAppr._ProcessWorkFlow = workFlow;
                string msg = "";

                bool res = trainingService.NominationApproval(tParticipantAppr);

                if (res)
                {
                    msg = tParticipantAppr.ActionType == "accept" ? "Nomination Approved." : "Nomination Rejected.";

                }
                return Json(new
                {
                    status = res ? true : false,
                    type = "success",
                    msg = msg


                }, JsonRequestBehavior.AllowGet);
                // }
                //return PartialView("_AttendanceGridView", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                throw ex;
            }
        }
        #endregion

        #region Separation Approval
        [HttpGet]
        public ActionResult _GetClearnceApprovalRequest()
        {
            log.Info("ApprovalRequestController/_GetClearnceApprovalRequest");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                approval.ClearanceList = sepService.GetApprovalClearanceList((int)userDetail.EmployeeID);
                return PartialView("_ClearanceGridView", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        //[HttpPost]
        //public ActionResult _GetClearnceApprovalRequest(EmpAttendance attendance)
        //{
        //    log.Info("ApprovalRequestController/_GetClearnceApprovalRequest");
        //    try
        //    {
        //        Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
        //        var fromDate = Request.Form["FromDate"];
        //        var toDate = Request.Form["ToDate"];
        //        attendance.ProxydateIn = fromDate == null || fromDate == "" ? DateTime.Now.Date.AddDays(-30) : Convert.ToDateTime(fromDate);
        //        attendance.ProxyOutDate = toDate == null || toDate == "" ? DateTime.Now.Date : Convert.ToDateTime(toDate);
        //        attendance.EmployeeId = (int)userDetail.EmployeeID;
        //        if (ModelState.IsValid)
        //        {
        //            var appliedAttendanceCount = 0;
        //            approval.attendanceList = empAttendanceService.GetEmployeeAttendanceByManagerID(attendance, out appliedAttendanceCount);
        //        }
        //        return PartialView("_ClearanceGridView", approval);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //}

        [HttpPost]
        public ActionResult _PostClearance(SeparationClearance sepAppr)
        {
            log.Info("ApprovalRequestController/_PostClearance");
            try
            {
                Model.ApprovalRequestVM approval = new Model.ApprovalRequestVM();
                if (ModelState.IsValid)
                {
                    string msg = "";
                    bool res = sepService.ApproveRejectClearance(sepAppr);
                    if (res)
                    {
                        msg = sepAppr.StatusId == true ? "Approved." : "Rejected.";
                        approval.ClearanceList = sepService.GetApprovalClearanceList((int)userDetail.EmployeeID);

                        return Json(new
                        {
                            status = res,
                            type = "success",
                            msg = msg


                        }, JsonRequestBehavior.AllowGet);
                    }
                    return PartialView("_ClearanceGridView", approval);
                }
                return PartialView("_ClearanceGridView", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                throw ex;
            }
        }


        [HttpGet]
        public ActionResult _GetClearanceApprovalPopup(int sepId, int clearanceId, string actionType, int reportingTo,string aprType)
        {
            log.Info("ApprovalRequestController/_GetApprovalPopup");
            try
            {
                SeparationClearance sepApp = new SeparationClearance
                {
                    SeparationId = sepId,
                    ClearanceId = clearanceId,
                    ReportingTo = reportingTo,
                    StatusId = actionType == "accept" ? true : false,
                    ApprovalType=aprType
                };

                return PartialView("_SeparationApprovalPopup", sepApp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                throw ex;
            }
        }

        #endregion
    }
}