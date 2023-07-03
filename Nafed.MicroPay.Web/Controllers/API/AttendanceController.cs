using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Model.API;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace MicroPay.Web.Controllers.API
{
    public class AttendanceController : BaseController
    {
        private readonly IMarkAttendance markAttendance;
        private IGenericRepository genericRepo;
        private IEmployeeAttendancedetailsService empAttendanceService;
        private IEmployeeAttendanceRepository empAttendanceRepo;
        private readonly IDropdownBindService ddlService;
        private readonly Nafed.MicroPay.Services.IServices.ILeaveService leaveService;

        public AttendanceController()
        {
            genericRepo = new GenericRepository();
            empAttendanceRepo = new EmployeeAttendanceRepository();
            markAttendance = new MarkAttendanceService(genericRepo, empAttendanceRepo);
            empAttendanceService = new EmployeeAttendancedetailsService(genericRepo, empAttendanceRepo);
            ddlService = new DropdownBindService(genericRepo);
            leaveService = new Nafed.MicroPay.Services.LeaveService(new GenericRepository(), new LeaveRepository());

        }
        /// <summary>
        ///  To insert Attendance detail of employee
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage InsertAttendance(FormDataCollection formData)
        {
            log.Info("AttendanceController/InsertAttendance");
            try
            {
                if (ModelState.IsValid)
                {
                    EmpAttendance attendance = new EmpAttendance()
                    {
                        EmployeeId = formData.Get("EmployeeId") == "" ? 0 : Convert.ToInt32(formData.Get("EmployeeId")),
                        ProxydateIn = Convert.ToDateTime(formData.Get("ProxydateIn")),
                        InTime = formData.Get("InTime"),
                        OutTime = formData.Get("OutTime"),
                        ProxyOutDate = Convert.ToDateTime(formData.Get("ProxyOutDate")),
                        Mode = "M",
                        TypeID = Convert.ToInt32(formData.Get("TypeID")),
                        MarkedBy = Convert.ToInt32(formData.Get("EmployeeId")),
                        CreatedBy = Convert.ToInt32(formData.Get("EmployeeId")),
                        Remarks = formData.Get("Remarks"),
                        CreatedOn = DateTime.Now,
                        Attendancestatus = 1,
                        BranchID= formData.Get("BranchID") == "" ? 0 : Convert.ToInt32(formData.Get("BranchID"))

                    };

                    if (markAttendance.AttendanceExists(attendance.BranchID, attendance.EmployeeId, attendance.ProxydateIn))
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Attendance Already Exist." });
                    }
                    else
                    {
                        int mainMarkAttendanceID = markAttendance.InsertTabletMarkAttendanceDetails(attendance);
                        if (mainMarkAttendanceID > 0)
                        {
                            attendance.EmpAttendanceID = mainMarkAttendanceID;                           
                            int markAttendanceID = markAttendance.InsertMarkAttendanceDetails(attendance);
                            if (markAttendanceID > 0)
                            {                              
                                var employeeAttendance = empAttendanceService.GetAttendanceDetails(attendance);
                                var markedAttendanceCount = 0;
                                var employeeAttendanceManagerWise = empAttendanceService.GetEmployeeAttendanceByManagerID(attendance, out markedAttendanceCount);
                                var appliedLeaveCount = 0;
                                var leaveApplied = leaveService.GetLeaveApplied((int)attendance.EmployeeId, out appliedLeaveCount); // total applied leave of its juniors

                                var response = Request.CreateResponse(HttpStatusCode.OK);
                                var attendanceJSON = JsonConvert.SerializeObject(new
                                {
                                    EmployeeAttendance = employeeAttendance,
                                    TotalAttendanceMarked = markedAttendanceCount,
                                    TotalAppliedLeave = appliedLeaveCount,
                                    message = "Inserted Successfully"
                                });
                                response.Content = new StringContent(attendanceJSON, System.Text.Encoding.UTF8, "application/json");
                                return response;

                                //  return Request.CreateResponse(HttpStatusCode.OK, new { message = "Inserted Successfully." });
                            }
                        }
                        else

                            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { errormessage = "error." });
                    }

                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { errormessage = "error." });
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { errormessage = ex.StackTrace });
            }
        }

        [HttpPost]
        public HttpResponseMessage GetAttendanceDetails(FormDataCollection formData)
        {
            log.Info("AttendanceController/MarkAttendance");
            try
            {
                MarkAttendance attendanceDetail = new MarkAttendance();
                attendanceDetail.EmployeeId = formData.Get("EmployeeId") == "" ? 0 : Convert.ToInt32(formData.Get("EmployeeId"));
                attendanceDetail.ProxydateIn = formData.Get("ProxydateIn") == "" ? DateTime.Now : Convert.ToDateTime(formData.Get("ProxydateIn"));
                attendanceDetail.ProxyOutDate = formData.Get("ProxyOutDate") == "" ? DateTime.Now : Convert.ToDateTime(formData.Get("ProxyOutDate"));
                if (ModelState.IsValid)
                {
                    EmpAttendance attendance = new EmpAttendance()
                    {
                        EmployeeId = attendanceDetail.EmployeeId,
                        ProxydateIn = attendanceDetail.ProxydateIn,
                        ProxyOutDate = attendanceDetail.ProxyOutDate
                    };
                    var employeeAttendance = empAttendanceService.GetAttendanceDetails(attendance);
                    var markedAttendanceCount = 0;
                    var employeeAttendanceManagerWise = empAttendanceService.GetEmployeeAttendanceByManagerID(attendance, out markedAttendanceCount);
                    var appliedLeaveCount = 0;
                    var leaveApplied = leaveService.GetLeaveApplied((int)attendanceDetail.EmployeeId, out appliedLeaveCount); // total applied leave of its juniors
                    var ddlAttendanceType = ddlService.ddlAttendanceTypeList();
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    var attendanceJSON = JsonConvert.SerializeObject(new
                    {
                        EmployeeAttendance = employeeAttendance,
                        AttendanceType = ddlAttendanceType,
                        TotalAttendanceMarked = markedAttendanceCount,
                        TotalAppliedLeave = appliedLeaveCount
                    });
                    response.Content = new StringContent(attendanceJSON, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { errormessage = ex.StackTrace });
            }
        }

        [HttpPost]
        public HttpResponseMessage GetEmployeeAttendanceByManagerID(FormDataCollection formData)
        {
            log.Info("AttendanceController/GetEmployeeAttendanceByManagerID");
            try
            {
                MarkAttendance attendanceDetail = new MarkAttendance();
                attendanceDetail.EmployeeId = formData.Get("EmployeeId") == "" ? 0 : Convert.ToInt32(formData.Get("EmployeeId"));
                attendanceDetail.ProxydateIn = formData.Get("ProxydateIn") == "" ? DateTime.Now : Convert.ToDateTime(formData.Get("ProxydateIn"));
                attendanceDetail.ProxyOutDate = formData.Get("ProxyOutDate") == "" ? DateTime.Now : Convert.ToDateTime(formData.Get("ProxyOutDate"));
                if (ModelState.IsValid)
                {
                    EmpAttendance attendance = new EmpAttendance()
                    {
                        EmployeeId = attendanceDetail.EmployeeId,
                        ProxydateIn = attendanceDetail.ProxydateIn,
                        ProxyOutDate = attendanceDetail.ProxyOutDate
                    };
                    var appliedAttendanceCount = 0;
                    var employeeAttendance = empAttendanceService.GetEmployeeAttendanceByManagerID(attendance, out appliedAttendanceCount);
                    var ddlAttendanceType = ddlService.ddlAttendanceTypeList();
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    var attendanceJSON = JsonConvert.SerializeObject(new
                    {
                        EmployeeAttendance = employeeAttendance,
                        TotalAttendanceMarked = appliedAttendanceCount,
                        AttendanceType = ddlAttendanceType
                    });
                    response.Content = new StringContent(attendanceJSON, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { errormessage = ex.StackTrace });
            }
        }

        [HttpPost]
        public HttpResponseMessage ApproveRejectAttendance(FormDataCollection formData)
        {
            log.Info("AttendanceController/ApproveRejectAttendance");
            try
            {
                if (ModelState.IsValid)
                {

                    EmpAttendance attendance = new EmpAttendance();
                    attendance.EmpAttendanceID = formData.Get("EmpAttendanceID") == "" ? 0 : Convert.ToInt32(formData.Get("EmpAttendanceID"));
                    attendance.EmployeeId = formData.Get("EmployeeId") == "" ? 0 : Convert.ToInt32(formData.Get("EmployeeId"));
                    attendance.Attendancestatus =Convert.ToInt32(formData.Get("Attendancestatus"));
                    attendance.RejectionRemarks = formData.Get("RejectionRemarks");
                    attendance.UpdatedBy = Convert.ToInt32(formData.Get("EmployeeId"));
                    attendance.UpdatedOn = DateTime.Now;


                    int attendanceStatus = markAttendance.ApproveRejectAttendance(attendance);
                    if (attendanceStatus > 0)
                        return Request.CreateResponse(HttpStatusCode.OK, new { message = "Updated Successfully." });
                    else
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { errormessage = "error." });
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { errormessage = "error." });
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { errormessage = ex.StackTrace });
            }
        }
    }
}
