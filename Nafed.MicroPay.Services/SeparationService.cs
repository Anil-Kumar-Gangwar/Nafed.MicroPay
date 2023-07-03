using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using static Nafed.MicroPay.Common.ExtensionMethods;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Services
{
    public class SeparationService : BaseService, ISeparationService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IEmployeeRepository empRepo;
        private readonly ISeparationRepository sepRepo;
        public SeparationService(IGenericRepository genericRepo, IEmployeeRepository empRepo, ISeparationRepository sepRepo)
        {
            this.genericRepo = genericRepo;
            this.empRepo = empRepo;
            this.sepRepo = sepRepo;
        }

        public List<SuperAnnuating> GetSuperAnnuating()
        {
            log.Info($"SeparationService/GetSuperAnnuating");

            try
            {
                var getEmpList = empRepo.GetSuperAnnuating();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetSuperAnnuating_Result, SuperAnnuating>()
                    .ForMember(d => d.StatusId, o => o.MapFrom(s => (s.StatusId == 7 && s.ClearanceDateUpto < DateTime.Now.Date) ? 8 : s.StatusId))
                    ;
                });
                return Mapper.Map<List<SuperAnnuating>>(getEmpList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool StartProcess(SuperAnnuating objSup)
        {
            log.Info($"SeparationService/StartProcess");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SuperAnnuating, DTOModel.Sepration>()
                    ;
                });
                var dtoSup = Mapper.Map<DTOModel.Sepration>(objSup);
                genericRepo.Insert(dtoSup);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool UploadDocument(SuperAnnuating objSup)
        {
            log.Info($"SeparationService/UploadDocument");
            try
            {
                var getEmpSep = genericRepo.Get<DTOModel.Sepration>(x => x.SeprationId == objSup.SeprationId && x.EmployeeId == objSup.EmployeeId).FirstOrDefault();
                if (getEmpSep != null)
                {
                    getEmpSep.StatusId = objSup.StatusId;
                    getEmpSep.DocumentName = objSup.DocumentName;
                    getEmpSep.UpdatedBy = objSup.UpdatedBy;
                    getEmpSep.UpdatedOn = objSup.UpdatedOn;
                    genericRepo.Update(getEmpSep);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool SendForClearance(EmployeeProcessApprovalVM empProcessApproval)
        {
            log.Info($"SeparationService/SendForClearance");
            bool flag = false;
            try
            {
                var getEmpSep = genericRepo.Get<DTOModel.Sepration>(x => x.SeprationId == empProcessApproval.empProcess.EmpProcessAppID && x.EmployeeId == empProcessApproval.empProcess.EmployeeID).FirstOrDefault();
                if (getEmpSep != null)
                {
                    getEmpSep.StatusId = empProcessApproval.ProcessId;
                    getEmpSep.ClearanceDateUpto = empProcessApproval.ClearanceDateUpto;
                    genericRepo.Update(getEmpSep);
                    flag = true;
                }
                if (flag)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.EmployeeProcessApproval, DTOModel.SeparationClearance>()

                        .ForMember(d => d.ReportingTo, o => o.MapFrom(s => s.ReportingTo))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.SeparationId, o => o.UseValue(empProcessApproval.empProcess.EmpProcessAppID))
                        .ForMember(d => d.ApprovalType, o => o.UseValue(empProcessApproval.ApprovalType))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    var dtoSepClearance = Mapper.Map<List<DTOModel.SeparationClearance>>(empProcessApproval.empProcessApp);
                    if (dtoSepClearance != null && dtoSepClearance.Count > 0)
                        genericRepo.AddMultipleEntity<DTOModel.SeparationClearance>(dtoSepClearance);

                    var empDetail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == empProcessApproval.empProcess.EmployeeID && !x.IsDeleted).Select(x => new
                    {
                        EmployeeCode = x.EmployeeCode,
                        Name = x.Name
                    }).FirstOrDefault();

                    for (int i = 0; i < dtoSepClearance.Count; i++)
                    {
                        //  SendMailMessageToReceiver(dtoSepClearance[i].ReportingTo, empDetail.EmployeeCode, empDetail.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }
            return flag;
        }

        public bool SendMailMessageToReceiver(int recieverID, string empCode, string empName)
        {
            log.Info($"ConfirmationFormService/SendMailMessageToReceiver/recieverID={recieverID}");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == recieverID && !x.IsDeleted).Select(x => new
                {
                    MobileNo = x.MobileNo,
                    OfficailEmail = x.OfficialEmail,
                    EmployeeCode = x.EmployeeCode
                }).FirstOrDefault();

                if (!string.IsNullOrEmpty(recieverMail.EmployeeCode))
                {
                    PushNotification notification = new PushNotification
                    {
                        UserName = recieverMail.EmployeeCode,
                        Title = "Clearance Intimation",
                        Message = $"Dear Sir/Madam, This is to intimate that you have received the Clearance Approval, for employee { empCode + "-" + empName} for further evaluation."

                    };
                    Task notif = Task.Run(() => FirebaseTopicNotification(notification));
                }

                if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                {
                    emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                 + "<p>This is to intimate that you have received the Clearance Approval for employee <b>" + empCode + "-" + empName + "</b> for further evaluation.</p>"
                 + "<p>Requesting your support for timely completion of the process by logging into: </p>"
                 + "<p>http://182.74.122.83/nafedhrms</p>"
                 + "<p>Please get in touch with Personnel Section in case of any disconnect/clarification required.</p> </div>");

                    emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");

                    Common.EmailMessage message = new Common.EmailMessage();
                    message.To = recieverMail.OfficailEmail;
                    message.Body = emailBody.ToString();
                    message.Subject = "NAFED-HRMS : Clearance Intimation";

                    Task t2 = Task.Run(() => SendEmail(message));
                }
                else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                {
                    emailBody.AppendFormat("Dear Sir/Madam,"
                  + "This is to intimate that you have received the Clearance Approval for " + empCode + "-" + empName + " for further evaluation."
                  + "Requesting your support for timely completion of the process by logging into:"
                  + "http://182.74.122.83/nafedhrms"
                  + "Please get in touch with Personnel Section in case of any disconnect/clarification required.");
                    emailBody.AppendFormat("Regards, ENAFED");

                    Task t1 = Task.Run(() => SendMessage(recieverMail.MobileNo, emailBody.ToString()));
                }
                flag = true;
            }
            catch
            {
                flag = true;
            }
            return flag;
        }

        void SendEmail(Common.EmailMessage message)
        {
            try
            {
                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                message.From = "NAFED HRMS <" + emailsetting.ToEmail + ">";
                message.UserName = emailsetting.UserName;
                message.Password = emailsetting.Password;
                message.SmtpClientHost = emailsetting.Server;
                message.SmtpPort = emailsetting.Port;
                message.enableSSL = emailsetting.SSLStatus;
                message.HTMLView = true;
                message.FriendlyName = "NAFED";
                Common.EmailHelper.SendEmail(message);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SendMessage(string mobileNo, string message)
        {
            try
            {
                SMSConfiguration smssetting = new SMSConfiguration();
                var smssettings = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SMSConfiguration, Model.SMSConfiguration>();

                });
                smssetting = Mapper.Map<Model.SMSConfiguration>(smssettings);

                string msgRecepient = mobileNo.Length == 10 ? "91" + mobileNo : mobileNo;
                Common.SMSParameter sms = new Common.SMSParameter();

                sms.MobileNo = msgRecepient;
                sms.Message = message;
                sms.URL = smssetting.SMSUrl;
                sms.User = smssetting.UserName;
                sms.Password = smssetting.Password;
                return Common.SMSHelper.SendSMS(sms);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return false;
            }
        }

        public List<SeparationClearance> GetClearanceApprovalStatus(int sepId, int aprType)
        {
            log.Info($"SeparationService/GetClearanceApprovalStatus");
            try
            {
                var getList = genericRepo.Get<DTOModel.SeparationClearance>(x => x.SeparationId == sepId && x.ApprovalType == aprType);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SeparationClearance, SeparationClearance>()
                    .ForMember(d => d.ReportingManager, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode + " - " + s.tblMstEmployee.Name))
                    .ForMember(d => d.ReportingDepartment, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Sepration.tblMstEmployee.EmployeeCode + " - " + s.Sepration.tblMstEmployee.Name))
                    .ForMember(d => d.DateofAction, o => o.MapFrom(s => s.UpdatedOn))
                     .ForMember(d => d.ClearanceDateUpto, o => o.MapFrom(s => s.Sepration.ClearanceDateUpto))
                     .ForMember(d => d.ApprovalType, o => o.MapFrom(s => s.ApprovalType));
                });
                return Mapper.Map<List<SeparationClearance>>(getList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SeparationClearance> GetApprovalClearanceList(int empId)
        {
            var getList = genericRepo.Get<DTOModel.SeparationClearance>(x => x.ReportingTo == empId && x.StatusId == null);
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.SeparationClearance, SeparationClearance>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Sepration.tblMstEmployee.EmployeeCode + " - " + s.Sepration.tblMstEmployee.Name))
                .ForMember(d => d.ApprovalType, o => o.MapFrom(s => s.ApprovalType));
            });
            var SepClearance = Mapper.Map<List<SeparationClearance>>(getList);

            var getResg = genericRepo.Get<DTOModel.Resignation>(x => x.tblMstEmployee.EmployeeProcessApprovals.Any(a => a.ProcessID == (int)Common.WorkFlowProcess.LeaveApproval && a.ToDate == null && a.ReportingTo == empId) && x.StatusId == 1);
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.Resignation, SeparationClearance>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode + " - " + s.tblMstEmployee.Name))
                .ForMember(d => d.ApprovalType, o => o.UseValue(3))
                .ForMember(d => d.ClearanceId, o => o.MapFrom(s => s.ResignationId))
            .ForAllOtherMembers(d => d.Ignore())
                ;
            });

            var resg = Mapper.Map<List<SeparationClearance>>(getResg);
            if (resg != null && resg.Count() > 0)
            {
                for (int i = 0; i < resg.Count; i++)
                {
                    SepClearance.Add(resg[i]);
                }
            }
            return SepClearance;
        }

        public bool ApproveRejectClearance(SeparationClearance objSep)
        {
            log.Info($"SeparationService/ApproveRejectClearance");
            try
            {
                if (objSep.ApprovalType == "3")
                {
                    var getResg = genericRepo.Get<DTOModel.Resignation>(x => x.ResignationId == objSep.ClearanceId).FirstOrDefault();
                    if (getResg != null)
                    {
                        getResg.StatusId = objSep.StatusId == true ? (int)Common.EmpLeaveStatus.InProcess : (int)Common.EmpLeaveStatus.RejectedByReportingOfficer;
                        if (objSep.StatusId == true)
                        {
                            SuperAnnuating objSup = new SuperAnnuating();
                            objSup.EmployeeId = getResg.EmployeeId;
                            objSup.SeprationType = 2; // for resignation
                            objSup.DateOfAction = getResg.ResignationDate;
                            objSup.NoticePeriod = getResg.NoticePeriod;
                            objSup.Reason = getResg.Reason;
                            objSup.OtherReason = getResg.OtherReason;
                            objSup.StatusId = 1;// initiate process
                            objSup.CreatedBy = getResg.CreatedBy;
                            objSup.CreatedOn = DateTime.Now;
                            StartProcess(objSup);
                        }
                        genericRepo.Update(getResg);
                    }
                    return true;
                }
                else
                {
                    var getData = genericRepo.Get<DTOModel.SeparationClearance>(x => x.ClearanceId == objSep.ClearanceId && x.ReportingTo == objSep.ReportingTo).FirstOrDefault();
                    if (getData != null)
                    {
                        var empId = getData.Sepration.EmployeeId;
                        // var sepId = getData.SeparationId;
                        getData.StatusId = objSep.StatusId;
                        getData.Remark = objSep.Remark;
                        getData.UpdatedOn = DateTime.Now;
                        genericRepo.Update(getData);
                        if (objSep.StatusId == false)
                        {
                            var sepStatusId = objSep.ApprovalType == "1" ? (int)SeprationStatus.ClearanceRejected : (int)SeprationStatus.DivisionalRejected;
                            sepRepo.AcceptRejectClearance(objSep.SeparationId, sepStatusId);
                        }
                        var aprType = Convert.ToInt16(objSep.ApprovalType);
                        var getClearance = genericRepo.Get<DTOModel.SeparationClearance>(x => x.SeparationId == objSep.SeparationId && x.ApprovalType == aprType);
                        if (getClearance != null && getClearance.Count() > 0)
                        {
                            if (!getClearance.Any(x => x.StatusId == null))
                            {
                                int count = 0;
                                foreach (var item in getClearance)
                                {
                                    if (item.StatusId == false)
                                        count++;
                                }
                                if (count == 0)
                                {
                                    var sepStatusId = objSep.ApprovalType == "1" ? (int)SeprationStatus.ClearanceApproved : (int)SeprationStatus.DivisionalApproved;
                                    sepRepo.AcceptRejectClearance(objSep.SeparationId, sepStatusId);
                                }
                            }
                        }

                        // SendMailMessageToPeronnel(empId);
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendMailMessageToPeronnel(int empId)
        {
            log.Info($"ConfirmationFormService/SendMailMessageToPeronnel");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                var gethrmail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DesignationID == 80 && x.DepartmentID == 36 && !x.IsDeleted && x.DOLeaveOrg == null).FirstOrDefault();
                if (gethrmail != null)
                {
                    var recieverMail = new
                    {
                        MobileNo = gethrmail.MobileNo,
                        OfficailEmail = gethrmail.OfficialEmail,
                        EmployeeCode = gethrmail.EmployeeCode
                    };

                    var empDtls = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == gethrmail.EmployeeId && !x.IsDeleted).Select(x => new
                    {
                        Name = x.EmployeeCode + " - " + x.Name
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(recieverMail.EmployeeCode))
                    {
                        PushNotification notification = new PushNotification
                        {
                            UserName = recieverMail.EmployeeCode,
                            Title = "Confirmation Approval",
                            Message = $"Dear Sir/Madam, This is to intimate that you have received the Clearance Approval Status, for employee { empDtls.Name} for further evaluation."
                        };
                        Task notif = Task.Run(() => FirebaseTopicNotification(notification));
                    }

                    if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                    {
                        emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                    + "<p>This is to intimate that you have received the Clearance Approval Status, for employee" + empDtls.Name + "</b> for further evaluation.</p>"
                    + "<p>Requesting your support for timely completion of the process by logging into: </p>"
                    + "<p>http://182.74.122.83/nafedhrms</p>");

                        emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");
                        Common.EmailMessage message = new Common.EmailMessage();
                        message.To = recieverMail.OfficailEmail;
                        message.Body = emailBody.ToString();
                        message.Subject = "NAFED-HRMS : Clearance Approval Status";
                        Task t2 = Task.Run(() => SendEmail(message));
                    }
                    flag = true;
                }
            }
            catch
            {
                flag = true;
            }
            return flag;
        }

        public bool UploadCircular(SuperAnnuating objSup)
        {
            log.Info($"SeparationService/UploadDocument");
            try
            {
                var getEmpSep = genericRepo.Get<DTOModel.Sepration>(x => x.SeprationId == objSup.SeprationId && x.EmployeeId == objSup.EmployeeId).FirstOrDefault();
                if (getEmpSep != null)
                {
                    getEmpSep.StatusId = objSup.StatusId;
                    getEmpSep.CircularDocument = objSup.CircularDocument;
                    getEmpSep.FileNo = objSup.FileNo;
                    getEmpSep.ReferenceNo = objSup.ReferenceNo;
                    getEmpSep.ApprovedDate = objSup.ApprovedDate;
                    getEmpSep.UpdatedBy = objSup.UpdatedBy;
                    getEmpSep.UpdatedOn = objSup.UpdatedOn;
                    genericRepo.Update(getEmpSep);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Resignation GetEmployeeDetail(int empId)
        {
            try
            {
                var getData = genericRepo.GetByID<DTOModel.tblMstEmployee>(empId);
                Mapper.Initialize(cfg =>
                cfg.CreateMap<DTOModel.tblMstEmployee, Resignation>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.EmployeeCode + " - " + s.Name))
                .ForMember(d => d.DOJ, o => o.MapFrom(s => s.DOJ))
                .ForMember(d => d.Designation, o => o.MapFrom(s => s.Designation.DesignationName))
                .ForMember(d => d.Branch, o => o.MapFrom(s => s.Branch.BranchName))
                .ForMember(d => d.Pr_Loc_DOJ, o => o.MapFrom(s => s.Pr_Loc_DOJ))
                .ForAllOtherMembers(d => d.Ignore())
                );

                return Mapper.Map<Resignation>(getData);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool Resignation(Resignation objResg)
        {
            log.Info($"SeparationService/Resignation");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Resignation, DTOModel.Resignation>()
                    .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.ResignationDate, o => o.MapFrom(s => s.ResignationDate))
                    .ForMember(d => d.NoticePeriod, o => o.MapFrom(s => s.NoticePeriod))
                    .ForMember(d => d.Reason, o => o.MapFrom(s => s.Reason))
                    .ForMember(d => d.OtherReason, o => o.MapFrom(s => s.OtherReason))
                    .ForMember(d => d.StatusId, o => o.MapFrom(s => s.StatusId))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                     .ForMember(d => d.DocName, o => o.MapFrom(s => s.DocName))
                    .ForAllOtherMembers(d => d.Ignore())
                    ;
                });
                var dtoResg = Mapper.Map<DTOModel.Resignation>(objResg);
                genericRepo.Insert(dtoResg);
                objResg._ProcessWorkFlow.ReferenceID = dtoResg.ResignationId;
                AddProcessWorkFlow(objResg._ProcessWorkFlow);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int? CheckForResignation(int empId)
        {
            log.Info($"SeparationService/CheckForResignation/empId={empId}");
            try
            {
                var getResg = genericRepo.Get<DTOModel.Resignation>(x => x.EmployeeId == empId).FirstOrDefault();
                if (getResg != null)
                {
                    return getResg.StatusId;
                }
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
