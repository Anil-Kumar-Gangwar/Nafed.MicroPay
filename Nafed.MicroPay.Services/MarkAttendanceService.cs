using AutoMapper;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using static Nafed.MicroPay.ImportExport.AttendanceForm;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Common;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.ImportExport.Interfaces;
using static Nafed.MicroPay.ImportExport.Export;
namespace Nafed.MicroPay.Services
{
    public class MarkAttendanceService : BaseService, IMarkAttendance
    {
        private readonly IGenericRepository genericRepo;
        private readonly IEmployeeAttendanceRepository empAttendanceRepo;
        public MarkAttendanceService(IGenericRepository genericRepo, IEmployeeAttendanceRepository empAttendanceRepo)
        {
            this.empAttendanceRepo = empAttendanceRepo;
            this.genericRepo = genericRepo;
        }
        public int InsertTabletMarkAttendanceDetails(EmpAttendance attendanceDetails, int userTypeID)
        {
            //var 
            //if (userDetail.UserID==1)
            log.Info($"MarkAttendanceService/InsertTabletMarkAttendanceDetails{userTypeID}");
            try
            {
                var fromDate = attendanceDetails.ProxydateIn;
                var toDate = attendanceDetails.ProxyOutDate;
                var diffDays = ((toDate - fromDate).TotalDays) + 1;

                for (int i = 0; i < diffDays; i++)
                {
                    //if (fromDate.DayOfWeek == DayOfWeek.Sunday)
                    //    return -1; // -1 is to raise custom error
                    fromDate = fromDate.AddDays(1);
                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmpAttendance, DTOModel.EmpAttendanceHdr>()
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                    .ForMember(c => c.InTime, c => c.MapFrom(s => s.InTime))
                    .ForMember(c => c.OutTime, c => c.MapFrom(s => s.OutTime))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                    .ForMember(c => c.Mode, c => c.MapFrom(s => s.Mode))
                    .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxyOutDate))
                    .ForMember(c => c.MarkedBy, c => c.MapFrom(s => s.MarkedBy))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    // .ForMember(c => c.Attendancestatus, c => c.UseValue(userTypeID!=3 ? 1 : 2))
                    .ForMember(d => d.Attendancestatus, o => o.UseValue((userTypeID == 1 || userTypeID == 2) ? (int)EmpAttendanceStatus.Approved : (int)EmpAttendanceStatus.Pending))
                    .ForMember(c => c.ReportingOfficer, c => c.MapFrom(s => s.Reportingofficer))
                    .ForMember(c => c.ReviewerTo, c => c.MapFrom(s => s.ReviewerTo))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoAttendance = Mapper.Map<DTOModel.EmpAttendanceHdr>(attendanceDetails);
                genericRepo.Insert<DTOModel.EmpAttendanceHdr>(dtoAttendance);
                attendanceDetails._ProcessWorkFlow.ReferenceID = dtoAttendance.EmpAttendanceID;
                AddProcessWorkFlow(attendanceDetails._ProcessWorkFlow);
                return dtoAttendance.EmpAttendanceID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public string GetAttendanceForm(int branchID, string fileName, string sFullPath, int? employeeType, DateTime date)
        {
            log.Info($"MarkAttendanceService/GetAttendanceForm/{branchID}");

            string result = string.Empty;
            if (Directory.Exists(sFullPath))
            {
                sFullPath = $"{sFullPath}{fileName}";
                var empDTO = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.BranchID == branchID && !x.IsDeleted);
                if (employeeType.HasValue)
                    empDTO = empDTO.Where(x => x.EmployeeTypeID == employeeType);

                DataTable dtAttendanceForm = new DataTable();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.EmpAttendanceForm>()
                    .ForMember(d => d.EmpCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.InDate, o => o.UseValue(date))
                    // .ForMember(d => d.OutDate, o => o.UseValue(DateTime.Now.Date))
                    .ForMember(d => d.InTime, o => o.UseValue(""))
                    .ForMember(d => d.OutTime, o => o.UseValue(""))
                    .ForMember(d => d.Remarks, o => o.UseValue(""))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var empList = Mapper.Map<List<Model.EmpAttendanceForm>>(empDTO);

                var sno = 1;
                empList.ForEach(x =>
                {
                    x.Sno = sno;
                    sno++;
                });

                if (empList.Count > 0)
                {
                    var formData = empList.Select(x => new
                    {
                        x.Sno,
                        x.EmpCode,
                        x.EmployeeName,
                        x.InDate,
                        x.InTime,
                        x.OutTime,
                        x.Remarks
                    }).ToList();

                    dtAttendanceForm = Common.ExtensionMethods.ToDataTable(formData);

                    //   dtAttendanceForm.Columns.Remove("EmployeeID");
                    //  dtAttendanceForm.Columns.Remove("error");
                    //  dtAttendanceForm.Columns.Remove("warning");
                }

                if (dtAttendanceForm != null && dtAttendanceForm.Rows.Count > 0)  //====== export attendance form if there is data ========= 
                {
                    dtAttendanceForm.Columns[0].ColumnName = "#";

                    IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                    exportHdr = dtAttendanceForm.Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();

                    result = ExportToExcel(exportHdr, dtAttendanceForm, "AttendanceForm", sFullPath);
                }
                else
                    result = "norec";
            }
            return result;
        }

        public int InsertMarkAttendanceDetails(Model.EmpAttendance tabletProxyAttendanceDetails)
        {
            log.Info($"MarkAttendanceService/InsertMarkAttendanceDetails");
            try
            {
                var fromDate = tabletProxyAttendanceDetails.ProxydateIn;
                var toDate = tabletProxyAttendanceDetails.ProxyOutDate;
                var diffDays = ((toDate - fromDate).TotalDays) + 1;

                for (int i = 0; i < diffDays; i++)
                {
                    tabletProxyAttendanceDetails.ProxydateIn = fromDate.AddDays(i);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.EmpAttendance, DTOModel.EmpAttendanceDetail>()
                        .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                        .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                        .ForMember(c => c.InTime, c => c.MapFrom(s => s.InTime))
                        .ForMember(c => c.OutTime, c => c.MapFrom(s => s.OutTime))
                        .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                        .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                        .ForMember(c => c.Mode, c => c.MapFrom(s => s.Mode))
                        .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxydateIn))
                        .ForMember(c => c.MarkedBy, c => c.MapFrom(s => s.MarkedBy))
                        .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                        .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                        .ForMember(c => c.EmpAttendanceID, c => c.MapFrom(s => s.EmpAttendanceID))
                        .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                       .ForAllOtherMembers(c => c.Ignore());
                    });
                    var dtomarkProxyAttendance = Mapper.Map<DTOModel.EmpAttendanceDetail>(tabletProxyAttendanceDetails);
                    genericRepo.Insert<DTOModel.EmpAttendanceDetail>(dtomarkProxyAttendance);

                    var empReportings = GetProcessApprovalSetting((int)dtomarkProxyAttendance.EmployeeId, WorkFlowProcess.AttendanceApproval);
                    if (empReportings.ReportingTo > 0)
                    {
                        SenderMailSMS(dtomarkProxyAttendance.EmployeeId, tabletProxyAttendanceDetails, "Attendance");
                        RecieverMailSMS(empReportings.ReportingTo, tabletProxyAttendanceDetails, "Attendance");
                    }
                }


                return 1;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<Model.EmpAttendance> AttendanceDetails(Model.EmpAttendance tabletProxyAttendanceDetails)
        {
            log.Info($"MarkAttendanceService/GetAttendanceForm/");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmpAttendance, DTOModel.EmpAttendanceHdr>()
                    .ForMember(c => c.EmpAttendanceID, c => c.MapFrom(s => s.EmpAttendanceID))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                    .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxyOutDate))
                    .ForMember(c => c.InTime, c => c.MapFrom(s => s.InTime))
                    .ForMember(c => c.OutTime, c => c.MapFrom(s => s.OutTime))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                    .ForMember(c => c.Attendancestatus, c => c.MapFrom(s => s.Attendancestatus))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoModel = Mapper.Map<Model.EmpAttendance, DTOModel.EmpAttendanceHdr>(tabletProxyAttendanceDetails);

                var listproxyAttendance = empAttendanceRepo.GetAttendanceDetails(dtoModel);

                var getList = listproxyAttendance.Select(x => new Model.EmpAttendance()
                {
                    EmployeeId = x.EmployeeId,
                    EmpAttendanceID = x.EmpAttendanceID,
                    ProxydateIn = x.ProxydateIn,
                    ProxyOutDate = x.ProxyOutDate,
                    InTime = x.InTime,
                    OutTime = x.OutTime,
                    Remarks = x.Remarks,
                    TypeID = x.TypeID,
                    Attendancestatus = (int)x.EmpAttendanceHdr.Attendancestatus

                });
                return getList.OrderByDescending(x => x.ProxydateIn).ToList();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int ApproveRejectAttendance(EmpAttendance attendanceDetail)
        {

            log.Info($"MarkAttendanceService/ApproveRejectAttendance");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmpAttendance, DTOModel.EmpAttendanceHdr>()
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.EmpAttendanceID, c => c.MapFrom(s => s.EmpAttendanceID))
                    .ForMember(c => c.Attendancestatus, c => c.MapFrom(s => s.Attendancestatus))
                    .ForMember(c => c.ReportingToRemark, c => c.MapFrom(s => s.ReportingToRemark))
                    .ForMember(c => c.ReviewerToRemark, c => c.MapFrom(s => s.ReviewerToRemark))
                     .ForMember(c => c.AcceptanceAuthRemark, c => c.MapFrom(s => s.AcceptanceAuthRemark))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoapproverejectAttendance = Mapper.Map<DTOModel.EmpAttendanceHdr>(attendanceDetail);
                empAttendanceRepo.ApproveRejectAttendance(dtoapproverejectAttendance);
                if (attendanceDetail._ProcessWorkFlow.EmployeeID > 0)
                {
                    attendanceDetail._ProcessWorkFlow.ReferenceID = dtoapproverejectAttendance.EmpAttendanceID;
                    AddProcessWorkFlow(attendanceDetail._ProcessWorkFlow);
                }
                if (attendanceDetail._ProcessWorkFlow.SenderID != null)
                    SenderMailSMS((int)attendanceDetail._ProcessWorkFlow.SenderID, attendanceDetail, "Approval");
                if (attendanceDetail._ProcessWorkFlow.ReceiverID != null)
                    RecieverMailSMS((int)attendanceDetail._ProcessWorkFlow.ReceiverID, attendanceDetail, "Approval");
                return dtoapproverejectAttendance.EmpAttendanceID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool AttendanceExists(int branchID, int empID, DateTime inDate, DateTime outDate)
        {

            log.Info($"MarkAttendanceService/AttendanceExists{branchID}/{empID}/{inDate}");
            try
            {
                //return genericRepo.Exists<DTOModel.EmpAttendanceHdr>(x => x.tblMstEmployee.BranchID == branchID && x.EmployeeId == empID
                //&& inDate <= x.ProxyOutDate &&   (x.Attendancestatus != 6 && x.Attendancestatus != 2 && x.Attendancestatus != 4 && x.Attendancestatus != 7) && x.TypeID != 5);
                return genericRepo.Exists<DTOModel.EmpAttendanceHdr>(x => x.tblMstEmployee.BranchID == branchID && x.EmployeeId == empID
               && ((inDate >= x.ProxydateIn && outDate <= x.ProxyOutDate) || (outDate >= x.ProxydateIn && outDate <= x.ProxyOutDate)) && (x.Attendancestatus != 6 && x.Attendancestatus != 2 && x.Attendancestatus != 4 && x.Attendancestatus != 7) && x.TypeID != 5);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        bool SenderMailSMS(int senderID, EmpAttendance empAttendance, string mailType)
        {
            log.Info($"MarkAttendanceService/SenderMailSMS");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                var senderMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == senderID && !x.IsDeleted).Select(x => new
                {
                    MobileNo = x.MobileNo,
                    OfficailEmail = x.OfficialEmail,
                    EmployeeName = x.Name,
                    EmployeeCode = x.EmployeeCode
                }).FirstOrDefault();

                if (mailType == "Attendance")
                {
                    if (!String.IsNullOrEmpty(senderMail.OfficailEmail))
                    {
                        emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear <b>" + senderMail.EmployeeName + "</b>,</p>"
                   + "<p>Your Attendance has been successfully marked.</p>"
                   + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'> <tr><td> Period </td> <td style='font-weight: bold'> {0} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {1} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{2}</td></tr>", empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString(), ((empAttendance.ProxyOutDate - empAttendance.ProxydateIn).TotalDays) + 1, empAttendance.Remarks);
                        emailBody.AppendFormat("</Table>");

                        EmailConfiguration emailsetting = new EmailConfiguration();
                        var emailsettings = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<DTOModel.EmailConfiguration, Model.EmailConfiguration>();

                        });
                        emailsetting = Mapper.Map<Model.EmailConfiguration>(emailsettings);

                        EmailMessage message = new EmailMessage();
                        message.To = senderMail.OfficailEmail;
                        message.Body = emailBody.ToString();
                        message.Subject = "NAFED-HRMS : Attendance Application";
                        Task t1 = Task.Run(() => SendMail(message, emailsetting));

                    }
                    else if (!String.IsNullOrEmpty(senderMail.MobileNo))
                    {
                        emailBody.AppendFormat("Dear Sir/Madam, Attendance has been successfully marked."
                            + ", Period - " + empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString() + "");

                        SMSConfiguration smssetting = new SMSConfiguration();
                        var smssettings = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<DTOModel.SMSConfiguration, Model.SMSConfiguration>();

                        });
                        smssetting = Mapper.Map<Model.SMSConfiguration>(smssettings);
                        Task t1 = Task.Run(() => SendMessage(senderMail.MobileNo, emailBody.ToString(), smssetting));
                    }
                }
                else if (mailType == "Approval")
                {
                    if (empAttendance.ActionType == "accept")
                    {
                        if (!String.IsNullOrEmpty(senderMail.OfficailEmail))
                        {
                            emailBody.AppendFormat("<div> <p><b><span style='font-family:Tahoma;font-size:12pt;color:green'>Approved</span></b> - "
                       + "" + senderMail.EmployeeName + " (" + senderMail.EmployeeCode + ")" + "</p>"
                       + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'> <tr><td> Period </td> <td style='font-weight: bold'> {0} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {1} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{2}</td></tr>", empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString(), ((empAttendance.ProxyOutDate - empAttendance.ProxydateIn).TotalDays) + 1, empAttendance.Remarks);
                            emailBody.AppendFormat("</Table>");
                            emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");
                        }
                        else if (!String.IsNullOrEmpty(senderMail.MobileNo))
                        {
                            emailBody.AppendFormat("Approved " + senderMail.EmployeeName + " - " + senderMail.EmployeeCode + ""
                                + ", Period - " + empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString() + "");
                        }
                    }
                    else if (empAttendance.ActionType == "reject")
                    {
                        if (!String.IsNullOrEmpty(senderMail.OfficailEmail))
                        {
                            emailBody.AppendFormat("<div> <p><b> <span style='font-family:Tahoma;font-size:12pt;color:red'>Rejected</span></b> - "
                       + "" + senderMail.EmployeeName + " - " + senderMail.EmployeeCode + "</p>"
                       + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'> <tr><td> Period </td> <td style='font-weight: bold'> {0} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {1} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{2}</td></tr>", empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString(), ((empAttendance.ProxyOutDate - empAttendance.ProxydateIn).TotalDays) + 1, empAttendance.Remarks);
                            emailBody.AppendFormat("</Table>");
                            emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");
                        }
                        else if (!String.IsNullOrEmpty(senderMail.MobileNo))
                        {
                            emailBody.AppendFormat("Rejected " + senderMail.EmployeeName + " - " + senderMail.EmployeeCode + ""
                                + ", Period - " + empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString() + "");
                        }
                    }
                }


                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        bool RecieverMailSMS(int recieverID, EmpAttendance empAttendance, string mailType)
        {
            log.Info($"MarkAttendanceService/RecieverMailSMS");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == recieverID && !x.IsDeleted).Select(x => new
                {
                    MobileNo = x.MobileNo,
                    OfficailEmail = x.OfficialEmail,
                    EmployeName = x.Name,
                    EmployeeCode = x.EmployeeCode
                }).FirstOrDefault();

                var employeeDetail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == empAttendance.EmployeeId && !x.IsDeleted).Select(x => new
                {
                    EmployeeName = x.Name,
                    EmployeeCode = x.EmployeeCode

                }).FirstOrDefault();
                if (mailType == "Attendance")
                {
                    if (!string.IsNullOrEmpty(recieverMail.EmployeeCode))
                    {
                        PushNotification notification = new PushNotification
                        {
                            UserName = recieverMail.EmployeeCode,
                            Title = "Attendance Approval",
                            Message = $"Dear Sir/Madam, This is to intimate that you have received the Attendance Approval, for employee {employeeDetail.EmployeeCode + " - " + employeeDetail.EmployeeName} with period { empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString()}  for further evaluation."

                        };
                        Task notif = Task.Run(() => FirebaseTopicNotification(notification));
                    }

                    if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                    {
                        emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
              + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'><tr><td> Period </td> <td style='font-weight: bold'> {1} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {2} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{3}</td></tr>", empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString(), ((empAttendance.ProxyOutDate - empAttendance.ProxydateIn).TotalDays) + 1, empAttendance.Remarks);
                        emailBody.AppendFormat("</Table>");
                        emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>For details please refer to my request on your Dashboard. Please login to  HRMS portal. </p>");
                        emailBody.AppendFormat("<div> <p>Regards, <br/> Name : " + employeeDetail.EmployeeName + "-" + employeeDetail.EmployeeCode + " </p> </div>");

                        EmailConfiguration emailsetting = new EmailConfiguration();
                        var emailsettings = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<DTOModel.EmailConfiguration, Model.EmailConfiguration>();

                        });
                        emailsetting = Mapper.Map<Model.EmailConfiguration>(emailsettings);

                        EmailMessage message = new EmailMessage();
                        message.To = recieverMail.OfficailEmail;
                        message.Body = emailBody.ToString();
                        message.Subject = "NAFED-HRMS : Attendance Application";
                        Task t2 = Task.Run(() => SendMail(message, emailsetting));

                    }
                    else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                    {
                        emailBody.AppendFormat("Dear Sir/Madam, For details please refer to my request on your Dashboard. Please login to  HRMS portal."
                            + ", Period - " + empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString() + "");

                        SMSConfiguration smssetting = new SMSConfiguration();
                        var smssettings = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<DTOModel.SMSConfiguration, Model.SMSConfiguration>();

                        });
                        smssetting = Mapper.Map<Model.SMSConfiguration>(smssettings);
                        Task t1 = Task.Run(() => SendMessage(recieverMail.MobileNo, emailBody.ToString(), smssetting));
                    }
                }
                else if (mailType == "Approval")
                {
                    if (empAttendance.ActionType == "accept")
                    {
                        if (empAttendance.Attendancestatus == (int)EmpLeaveStatus.Approved)
                        {
                            if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                            {
                                emailBody.AppendFormat("<div> <p><b><span style='font-family:Tahoma;font-size:12pt;color:green'>Approved</span></b></p>"
                      + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'> <tr><td> Period </td> <td style='font-weight: bold'> {0} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {1} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{2}</td></tr>", empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString(), ((empAttendance.ProxyOutDate - empAttendance.ProxydateIn).TotalDays) + 1, empAttendance.Remarks);
                                emailBody.AppendFormat("</Table>");
                                emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");

                            }
                            else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                            {
                                emailBody.AppendFormat("Dear Sir/Madam, Your Attendance Application has been Approved."
                                     + ", Period - " + empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString() + "");
                            }
                        }

                        else
                        {
                            if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                            {
                                emailBody.AppendFormat("<div> <p><b><span style='font-family:Tahoma;font-size:12pt;'> Approval Recommendation </span></b> </p>"
                                    + "<p>Dear Sir/Madam,</p>"
                      + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'><tr><td> Period </td> <td style='font-weight: bold'> {0} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {1} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{2}</td></tr>", empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString(), ((empAttendance.ProxyOutDate - empAttendance.ProxydateIn).TotalDays) + 1, empAttendance.Remarks);
                                emailBody.AppendFormat("</Table>");
                                emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>For details please login to HRMS portal. </p>");
                                emailBody.AppendFormat("<div> <p>Regards, <br/> Name : " + empAttendance.SenderName + " <br/> Post : " + empAttendance.SenderDesignation + " </p> </div>");

                            }
                            else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                            {
                                emailBody.AppendFormat("Approval Recommendation - Dear Sir/Madam, For details please login to HRMS portal."
                                    + " ." + ", Period - " + empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString() + "");
                            }
                        }
                    }
                    if (empAttendance.ActionType == "reject")
                    {
                        if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                        {
                            emailBody.AppendFormat("<div> <p><b><span style='font-family:Tahoma;font-size:12pt;color:red'> Rejected</span> </b> due to - " + empAttendance._ProcessWorkFlow.Scomments + " </p>"
                  + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'> <tr><td> Period </td> <td style='font-weight: bold'> {0} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {1} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{2}</td></tr>", empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString(), ((empAttendance.ProxyOutDate - empAttendance.ProxydateIn).TotalDays) + 1, empAttendance.Remarks);
                            emailBody.AppendFormat("</Table>");
                            emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");

                        }
                        else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                        {
                            emailBody.AppendFormat("Dear Sir/Madam, Your Attendance Application has been Rejected."
                                 + ", Period - " + empAttendance.ProxydateIn.Date.ToShortDateString() + " To " + empAttendance.ProxyOutDate.Date.ToShortDateString() + "");
                        }
                    }
                }
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

            }

            return flag;
        }

        private void SendMail(EmailMessage message, EmailConfiguration emailsetting)
        {
            try
            {
                message.From = "NAFED HRMS <" + emailsetting.ToEmail + ">";
                message.UserName = emailsetting.UserName;
                message.Password = emailsetting.Password;
                message.SmtpClientHost = emailsetting.Server;
                message.SmtpPort = emailsetting.Port;
                message.enableSSL = emailsetting.SSLStatus;
                message.HTMLView = true;
                message.FriendlyName = "NAFED";
                EmailHelper.SendEmail(message);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SendMessage(string mobileNo, string message, SMSConfiguration smssetting)
        {
            try
            {
                string msgRecepient = mobileNo.Length == 10 ? "91" + mobileNo : mobileNo;
                SMSParameter sms = new SMSParameter();

                sms.MobileNo = msgRecepient;
                sms.Message = message;
                sms.URL = smssetting.SMSUrl;
                sms.User = smssetting.UserName;
                sms.Password = smssetting.Password;
                return SMSHelper.SendSMS(sms);
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return false;
            }

        }

        public int InsertTourDetail(EmpAttendance attendanceDetails)
        {

            log.Info($"MarkAttendanceService/InsertTourDetail");
            try
            {
                var getEmpDesID = genericRepo.GetByID<DTOModel.tblMstEmployee>(attendanceDetails.EmployeeId).DesignationID;

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmpAttendance, DTOModel.EmpAttendanceHdr>()
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                    .ForMember(c => c.InTime, c => c.UseValue("00:00"))
                    .ForMember(c => c.OutTime, c => c.UseValue("00:00"))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                    .ForMember(c => c.Mode, c => c.MapFrom(s => s.Mode))
                    .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxyOutDate))
                    .ForMember(c => c.MarkedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.LocationID, c => c.MapFrom(s => s.LocationID == 9999 ? null : s.LocationID))
                    .ForMember(c => c.OtherLocation, c => c.MapFrom(s => s.LocationName))
                    .ForMember(c => c.DesignationID, c => c.UseValue(getEmpDesID))
                    .ForMember(d => d.Attendancestatus, o => o.UseValue((int)EmpAttendanceStatus.Approved))
                    .ForMember(c => c.OrderDate, c => c.MapFrom(s => s.OrderDate))
                    .ForMember(c => c.JoiningDate, c => c.MapFrom(s => s.JoiningDate))
                    .ForMember(c => c.ReleavingDate, c => c.MapFrom(s => s.ReleavingDate))
                    .ForMember(c => c.JoinBackDate, c => c.MapFrom(s => s.JoinBackDate))
                    .ForMember(c => c.ReleavDateFromLoc, c => c.MapFrom(s => s.ReleavDateFromLoc))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoAttendance = Mapper.Map<DTOModel.EmpAttendanceHdr>(attendanceDetails);
                genericRepo.Insert<DTOModel.EmpAttendanceHdr>(dtoAttendance);


                //Mapper.Initialize(cfg =>
                //{
                //    cfg.CreateMap<Model.EmpAttendance, DTOModel.EmpAttendanceDetail>()
                //    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                //    .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                //    .ForMember(c => c.InTime, c => c.UseValue("00:00"))
                //    .ForMember(c => c.OutTime, c => c.UseValue("00:00"))
                //    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                //    .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                //    .ForMember(c => c.Mode, c => c.MapFrom(s => s.Mode))
                //    .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxyOutDate))
                //    .ForMember(c => c.MarkedBy, c => c.MapFrom(s => s.CreatedBy))
                //    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                //    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                //    .ForMember(c => c.EmpAttendanceID, c => c.UseValue(dtoAttendance.EmpAttendanceID))
                //    .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                //   .ForAllOtherMembers(c => c.Ignore());
                //});
                List<EmpAttendance> tourList = new List<EmpAttendance>();
                var fromDate = attendanceDetails.ProxydateIn;
                var toDate = attendanceDetails.ProxyOutDate;
                var diffDays = ((toDate - fromDate).TotalDays) + 1;

                for (int i = 0; i < diffDays; i++)
                {
                    attendanceDetails.ProxydateIn = fromDate.AddDays(i);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.EmpAttendance, EmpAttendance>()
                       .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                       .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                       .ForMember(c => c.InTime, c => c.UseValue("00:00"))
                       .ForMember(c => c.OutTime, c => c.UseValue("00:00"))
                       .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                       .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                       .ForMember(c => c.Mode, c => c.MapFrom(s => s.Mode))
                       .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxydateIn))
                       .ForMember(c => c.MarkedBy, c => c.MapFrom(s => s.CreatedBy))
                       .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                       .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                       .ForMember(c => c.EmpAttendanceID, c => c.UseValue(dtoAttendance.EmpAttendanceID))
                       .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                          .ForAllOtherMembers(c => c.Ignore());
                    });
                    var tourDtls = Mapper.Map<EmpAttendance>(attendanceDetails);

                    tourList.Add(tourDtls);
                }


                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmpAttendance, DTOModel.EmpAttendanceDetail>()
                   .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                   .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                   .ForMember(c => c.InTime, c => c.MapFrom(s => s.InTime))
                   .ForMember(c => c.OutTime, c => c.MapFrom(s => s.OutTime))
                   .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                   .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                   .ForMember(c => c.Mode, c => c.MapFrom(s => s.Mode))
                   .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxyOutDate))
                   .ForMember(c => c.MarkedBy, c => c.MapFrom(s => s.CreatedBy))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.EmpAttendanceID, c => c.MapFrom(s => s.EmpAttendanceID))
                   .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                      .ForAllOtherMembers(c => c.Ignore());
                });
                var dtomarkProxyAttendance = Mapper.Map<List<DTOModel.EmpAttendanceDetail>>(tourList);
                genericRepo.AddMultiple(dtomarkProxyAttendance);


                return dtoAttendance.EmpAttendanceID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public List<Model.EmpAttendance> GetTourDetails(EmpAttendance empAttend)
        {
            log.Info($"MarkAttendanceService/GetTourDetails/");
            try
            {
                var getdata = genericRepo.Get<DTOModel.EmpAttendanceHdr>(x => x.Mode == "T" && (empAttend.BranchID == 0 ? (1 > 0) : x.EmpAttendanceDetails.Any(y => y.BranchID == empAttend.BranchID))
                  && (empAttend.EmployeeId == 0 ? (1 > 0) : x.EmployeeId == empAttend.EmployeeId) && (default(DateTime) == empAttend.ProxydateIn ? (1 > 0) :
            x.ProxydateIn >= empAttend.ProxydateIn) && (default(DateTime) == empAttend.ProxyOutDate ? (1 > 0) :
            x.ProxyOutDate <= empAttend.ProxyOutDate)).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmpAttendanceHdr, Model.EmpAttendance>()
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                    .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxyOutDate))
                    .ForMember(c => c.EmpAttendanceID, c => c.MapFrom(s => s.EmpAttendanceID))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                    .ForMember(c => c.EmployeeName, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode + " - " + s.tblMstEmployee.Name))
                    .ForMember(c => c.SenderDesignation, c => c.MapFrom(s => s.Designation.DesignationName))
                    .ForMember(c => c.Mode, c => c.MapFrom(s => s.Mode))
                    .ForMember(c => c.LocationName, c => c.MapFrom(s => s.LocationID == null ? s.OtherLocation : s.Branch.BranchName))
                    .ForMember(c => c.BranchName, c => c.MapFrom(s => s.Branch.BranchName))
                    .ForMember(c => c.OrderDate, c => c.MapFrom(s => s.OrderDate))
                    .ForMember(c => c.JoiningDate, c => c.MapFrom(s => s.JoiningDate))
                    .ForMember(c => c.ReleavingDate, c => c.MapFrom(s => s.ReleavingDate))
                    .ForMember(c => c.JoinBackDate, c => c.MapFrom(s => s.JoinBackDate))
                    .ForMember(c => c.ReleavDateFromLoc, c => c.MapFrom(s => s.ReleavDateFromLoc))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var getList = Mapper.Map<List<Model.EmpAttendance>>(getdata);
                return getList.OrderByDescending(x => x.ProxydateIn).ToList();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool ExportTourDetail(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter)
        {
            log.Info($"MarkAttendanceService/ExportTourDetail/{sFullPath}/{fileName}");
            try
            {
                var flag = false;
                if (dtTable != null)
                {
                    if (System.IO.Directory.Exists(sFullPath))
                    {
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dtTable.Columns.Cast<System.Data.DataColumn>()
                            .Select(x => x.ColumnName).AsEnumerable<string>();
                        sFullPath = $"{sFullPath}{fileName}";
                        ExportToExcel(exportHdr, dtTable, fileName, sFullPath, tFilter);
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }


}
