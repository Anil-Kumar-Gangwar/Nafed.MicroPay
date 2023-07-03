using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
namespace Nafed.MicroPay.Services
{
    public class EPFNominationService : BaseService, IEPFNominationService
    {
        private readonly IEPFNominationRepository epfNomRepo;
        private readonly IGenericRepository genericRepo;
        public EPFNominationService(IEPFNominationRepository epfNomRepo, IGenericRepository genericRepo)
        {
            this.epfNomRepo = epfNomRepo;
            this.genericRepo = genericRepo;
        }
        public EPFNomination GetEPFNominationDetail(int epfNoID, int employeeID)
        {
            log.Info($"EPFNominationService/GetEPFNominationDetail/epfNoID={epfNoID}/employeeID={employeeID}");
            try
            {
                var getEPFDetail = epfNomRepo.GetEPFNominationDetail(epfNoID, employeeID);
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetEPFNominationDetail_Result, Model.EPFNomination>()
                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.DOB, o => o.MapFrom(s => s.DOB))
                .ForMember(d => d.PlaceOfJoin, o => o.MapFrom(s => s.PlaceOfJoin))
                .ForMember(d => d.DOJ, o => o.MapFrom(s => s.DOJ))
                .ForMember(d => d.ReportingName, o => o.MapFrom(s => s.ReportingTo))
                .ForMember(d => d.ReportingDesignation, o => o.MapFrom(s => s.ReportingDesignation))
                .ForMember(d => d.RTSenddate, o => o.MapFrom(s => s.RTSenddate))
                .ForMember(d => d.EmpSenddate, o => o.MapFrom(s => s.EmpSenddate))
                .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                .ForMember(d => d.EPFNoID, o => o.MapFrom(s => s.EPFNoID))
                .ForMember(d => d.HBName, o => o.MapFrom(s => s.HBName))
                .ForMember(d => d.Gender, o => o.MapFrom(s => s.Gender))
                .ForMember(d => d.MaritalStatusName, o => o.MapFrom(s => s.MaritalStatusName))
                .ForMember(d => d.PresentAddress, o => o.MapFrom(s => s.PresentAddress))
                .ForMember(d => d.PmtAddress, o => o.MapFrom(s => s.PmtAddress))
                .ForMember(d => d.RTPlaceOfJoin, o => o.MapFrom(s => s.RTPlaceOfJoin))

                .ForAllOtherMembers(d => d.Ignore())
                );
                var dtoEPF = Mapper.Map<Model.EPFNomination>(getEPFDetail);
                if (dtoEPF != null)
                    return dtoEPF;
                else
                    return new EPFNomination();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<EPFDetail> GetEPFNominee(int employeeID, int filterBy, int? epfNo)
        {
            log.Info($"EPFNominationService/GetEPFNominee/employeeID={employeeID}/filterBy={filterBy}");
            try
            {
                var getEPFDetail = epfNomRepo.GetEPFEPSNominee(employeeID, filterBy, epfNo);
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetEPFEPSNominee_Result, Model.EPFDetail>()
                .ForMember(d => d.EPFIDDetail, o => o.MapFrom(s => s.EPFIDDetail))
                .ForMember(d => d.EPFID, o => o.MapFrom(s => s.EPFNoID))
                .ForMember(d => d.NomineeID, o => o.MapFrom(s => s.NomineeID))
                .ForMember(d => d.GuardianName, o => o.MapFrom(s => s.GuardianName))
                .ForMember(d => d.GuardianRelationID, o => o.MapFrom(s => s.GuardianRelationID))
                .ForMember(d => d.GuardianAddress, o => o.MapFrom(s => s.GuardianAddress))
                .ForMember(d => d.RelationName, o => o.MapFrom(s => s.RelationName))
                .ForMember(d => d.DOB, o => o.MapFrom(s => s.DOB))
                .ForMember(d => d.PFDistribution, o => o.MapFrom(s => s.PFDistribution))
                .ForMember(d => d.EmpDependentID, o => o.MapFrom(s => s.EmpDependentID))
                .ForMember(d => d.DependentName, o => o.MapFrom(s => s.DependentName))
                .ForMember(d => d.Address, o => o.MapFrom(s => s.Address))
                .ForMember(d => d.IsMinor, o => o.MapFrom(s => s.IsMinor))
                .ForAllOtherMembers(d => d.Ignore())
                );
                var dtoEPF = Mapper.Map<List<Model.EPFDetail>>(getEPFDetail);
                if (dtoEPF != null)
                    return dtoEPF;
                else
                    return new List<EPFDetail>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<EPSDetail> GetEPSNominee(int employeeID, int filterBy, int? epfNo)
        {
            log.Info($"EPFNominationService/GetEPSNominee/employeeID={employeeID}/filterBy={filterBy}");
            try
            {
                var getEPFDetail = epfNomRepo.GetEPFEPSNominee(employeeID, filterBy, epfNo);
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetEPFEPSNominee_Result, Model.EPSDetail>()
                .ForMember(d => d.EPFIDDetail, o => o.MapFrom(s => s.EPFIDDetail))
                .ForMember(d => d.EPSID, o => o.MapFrom(s => s.EPFNoID))
                .ForMember(d => d.NomineeID, o => o.MapFrom(s => s.NomineeID))
                .ForMember(d => d.GuardianName, o => o.MapFrom(s => s.GuardianName))
                .ForMember(d => d.GuardianRelationID, o => o.MapFrom(s => s.GuardianRelationID))
                .ForMember(d => d.GuardianAddress, o => o.MapFrom(s => s.GuardianAddress))
                .ForMember(d => d.RelationName, o => o.MapFrom(s => s.RelationName))
                .ForMember(d => d.DOB, o => o.MapFrom(s => s.DOB))
                .ForMember(d => d.PFDistribution, o => o.MapFrom(s => s.PFDistribution))
                .ForMember(d => d.EmpDependentID, o => o.MapFrom(s => s.EmpDependentID))
                .ForMember(d => d.DependentName, o => o.MapFrom(s => s.DependentName))
                .ForMember(d => d.Address, o => o.MapFrom(s => s.Address))

                .ForAllOtherMembers(d => d.Ignore())
                );
                var dtoEPF = Mapper.Map<List<Model.EPSDetail>>(getEPFDetail);
                if (dtoEPF != null)
                    return dtoEPF;
                else
                    return new List<EPSDetail>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public int InsertEPFNominee(EPFNomination epfNom)
        {
            log.Info($"EPFNominationService/InsertEPFNominee");
            try
            {
                Mapper.Initialize(cfg =>
                cfg.CreateMap<EPFNomination, DTOModel.EPFNomination>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForAllOtherMembers(d => d.Ignore())
                );

                var dtoEPF = Mapper.Map<DTOModel.EPFNomination>(epfNom);
                var res = genericRepo.Insert(dtoEPF);
                if (dtoEPF.EPFNoID > 0)
                {
                    #region Child Table
                    if (epfNom.EPFDetailList != null && epfNom.EPFDetailList.Count > 0)
                    {
                        Mapper.Initialize(cfg =>
                        cfg.CreateMap<EPFDetail, DTOModel.EPFDetail>()
                        .ForMember(d => d.EPFNoID, o => o.UseValue(dtoEPF.EPFNoID))
                        .ForMember(d => d.NomineeID, o => o.MapFrom(s => s.EmpDependentID))
                        .ForMember(d => d.GuardianName, o => o.MapFrom(s => s.GuardianName))
                        .ForMember(d => d.GuardianRelationID, o => o.MapFrom(s => s.GuardianRelationID))
                        .ForMember(d => d.GuardianAddress, o => o.MapFrom(s => s.GuardianAddress))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoEPF.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(dtoEPF.CreatedOn))
                        .ForMember(d => d.IsMinor, o => o.MapFrom(s => s.IsMinor))
                        .ForAllOtherMembers(d => d.Ignore())
                        );
                        var dtoBehavioral = Mapper.Map<List<DTOModel.EPFDetail>>(epfNom.EPFDetailList);
                        genericRepo.AddMultipleEntity(dtoBehavioral);
                    }

                    #endregion

                    if (epfNom.EPFNoID == 0)
                    {
                        epfNom._ProcessWorkFlow.ReferenceID = dtoEPF.EPFNoID;
                        AddProcessWorkFlow(epfNom._ProcessWorkFlow);
                    }

                }
                return dtoEPF.EPFNoID;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public int UpdateEPFNominee(EPFNomination epfNom)
        {
            log.Info($"EPFNominationService/UpdateEPFNominee");
            try
            {
                var getdata = genericRepo.GetByID<DTOModel.EPFNomination>(epfNom.EPFNoID);
                if (getdata != null)
                {
                    getdata.StatusID = epfNom.StatusID;
                    getdata.UpdatedBy = epfNom.UpdatedBy;
                    getdata.UpdatedOn = epfNom.UpdatedOn;
                    genericRepo.Update(getdata);
                }
                if (epfNom.EPFNoID > 0)
                {
                    if (epfNom.loggedInEmpID == epfNom.EmployeeID)
                    {
                        #region Child Table
                        if (epfNom.EPFDetailList != null && epfNom.EPFDetailList.Count > 0)
                        {
                            var getDelNomList = genericRepo.Get<DTOModel.EPFDetail>(x => x.EPFNoID == epfNom.EPFNoID).ToList();

                            Mapper.Initialize(cfg =>
                            cfg.CreateMap<EPFDetail, DTOModel.EPFDetail>()
                            .ForMember(d => d.EPFNoID, o => o.UseValue(epfNom.EPFNoID))
                            .ForMember(d => d.NomineeID, o => o.MapFrom(s => s.EmpDependentID))
                            .ForMember(d => d.GuardianName, o => o.MapFrom(s => s.GuardianName))
                            .ForMember(d => d.GuardianRelationID, o => o.MapFrom(s => s.GuardianRelationID))
                            .ForMember(d => d.GuardianAddress, o => o.MapFrom(s => s.GuardianAddress))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(epfNom.UpdatedBy))
                            .ForMember(d => d.CreatedOn, o => o.UseValue(epfNom.UpdatedOn))
                            .ForMember(d => d.IsMinor, o => o.MapFrom(s => s.IsMinor))
                            .ForAllOtherMembers(d => d.Ignore())
                            );
                            var dtoNomList = Mapper.Map<List<DTOModel.EPFDetail>>(epfNom.EPFDetailList);
                            if (getDelNomList != null && getDelNomList.Count > 0)
                                genericRepo.DeleteAll<DTOModel.EPFDetail>(getDelNomList);
                            if (dtoNomList != null)
                                genericRepo.AddMultipleEntity<DTOModel.EPFDetail>(dtoNomList);
                        }
                        #endregion
                    }
                    else
                    {
                        epfNom._ProcessWorkFlow.ReferenceID = epfNom.EPFNoID;
                        AddProcessWorkFlow(epfNom._ProcessWorkFlow);
                        SendMailMessageToReceiver(epfNom.EmployeeCode, epfNom.EmployeeName);
                    }
                }
                return epfNom.EPFNoID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<EPFNomination> GetEPFNomineeList(int? employeeID, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var getEPFDetail = genericRepo.Get<DTOModel.EPFNomination>(x => employeeID.HasValue ? x.EmployeeID == employeeID : (1 > 0) && ((fromDate.HasValue && toDate.HasValue) ? (x.CreatedOn >= fromDate && x.CreatedOn <= toDate) : (1 > 0))).ToList();
            Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.EPFNomination, Model.EPFNomination>()
                     .ForMember(c => c.EmployeeCode, c => c.MapFrom(m => m.tblMstEmployee.EmployeeCode))
                     .ForMember(c => c.EmployeeName, c => c.MapFrom(m => m.tblMstEmployee.Name))
            );
            var dtoEPF = Mapper.Map<List<Model.EPFNomination>>(getEPFDetail);
            if (dtoEPF != null)
                return dtoEPF;
            else
                return new List<EPFNomination>();
        }
        public List<EPSDetail> GetMaleEmployeeEPSNominee(int employeeID)
        {
            log.Info($"EPFNominationService/GetMaleEmployeeEPSNominee/employeeID={employeeID}");
            try
            {
                var getEPFDetail = epfNomRepo.GetMaleEmployeeNominee(employeeID);
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetMaleEmployeeNominee_Result, Model.EPSDetail>()
                .ForMember(d => d.RelationName, o => o.MapFrom(s => s.RelationName))
                .ForMember(d => d.DOB, o => o.MapFrom(s => s.DOB))
                .ForMember(d => d.EmpDependentID, o => o.MapFrom(s => s.EmpDependentID))
                .ForMember(d => d.DependentName, o => o.MapFrom(s => s.DependentName))
                .ForMember(d => d.Address, o => o.MapFrom(s => s.Address))
                .ForAllOtherMembers(d => d.Ignore())
                );
                var dtoEPF = Mapper.Map<List<Model.EPSDetail>>(getEPFDetail);
                if (dtoEPF != null)
                    return dtoEPF;
                else
                    return new List<EPSDetail>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<EPFNomination> GetEPFApprovalList(CommonFilter filters)
        {
            log.Info($"EPFNominationService/GetEPFApprovalList");
            try
            {
                var getEPFList = genericRepo.GetIQueryable<DTOModel.EPFNomination>
                (x => x.tblMstEmployee.EmployeeProcessApprovals
                       .Any(y => y.ProcessID == (int)Common.WorkFlowProcess.EPFNomination
                       && y.ToDate == null && (y.ReportingTo == filters.loggedInEmployee))
                ).ToList();
                if (filters.FromDate.HasValue && filters.ToDate.HasValue)
                {
                    getEPFList = getEPFList.Where(x => x.CreatedOn.Date >= filters.FromDate.Value.Date && x.CreatedOn.Date <= filters.ToDate.Value.Date).ToList();
                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EPFNomination, Model.EPFNomination>()
                  .ForMember(c => c.EPFNoID, c => c.MapFrom(m => m.EPFNoID))

                     .ForMember(c => c.EmployeeID, c => c.MapFrom(m => m.EmployeeID))
                     .ForMember(c => c.StatusID, c => c.MapFrom(m => m.StatusID))
                     .ForMember(c => c.EmployeeCode, c => c.MapFrom(m => m.tblMstEmployee.EmployeeCode))
                     .ForMember(c => c.EmployeeName, c => c.MapFrom(m => m.tblMstEmployee.Name))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(d => d.approvalSettings, o => o.MapFrom(s =>
                     s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y =>
                     y.ProcessID == (int)Common.WorkFlowProcess.EPFNomination && y.ToDate == null && y.EmployeeID == s.EmployeeID)))
                    .ForAllOtherMembers(d => d.Ignore());

                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();

                });
                return Mapper.Map<List<EPFNomination>>(getEPFList.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        bool SendMailMessageToReceiver(string empCode, string empName)
        {
            log.Info($"EPFNominationService/SendMailMessageToReceiver/empCode={empCode}");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DesignationID == 59 && x.DepartmentID == 32 && !x.IsDeleted && x.DOLeaveOrg == null).Select(x => new
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
                        Title = "EPF Nomination Approval",
                        Message = $"Dear Sir/Madam, This is to intimate that you have received the EPF Nomination Form, for employee { empCode + "-" + empName } for further evaluation."

                    };
                    Task notif = Task.Run(() => FirebaseTopicNotification(notification));
                }

                if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                {
                    emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                 + "<p>This is to intimate that you have received the EPF Nomination Form of the employee <b>" + empCode + "-" + empName + "</b>.</p>"
                 + "<p>You can check the form by logging into: </p>"
                 + "<p>http://182.74.122.83/nafedhrms</p>"
                 + "<p>Please get in touch with Personnel Section in case of any information required. </p> </div>");

                    emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");

                    Common.EmailMessage message = new Common.EmailMessage();
                    message.To = recieverMail.OfficailEmail;
                    message.Body = emailBody.ToString();
                    message.Subject = "NAFED-HRMS : EPF Nomination Intimation";

                    Task t2 = Task.Run(() => SendEmail(message));
                }
                else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                {
                    emailBody.AppendFormat("Dear Sir/Madam,"
                       + "<p>This is to intimate that you have received the EPF Nomination Form of the employee <b>" + empCode + "-" + empName + "</b>.</p>"
      + "<p>You can check the form by logging into: </p>"
                  + "http://182.74.122.83/nafedhrms");
                    emailBody.AppendFormat("Regards, ENAFED");

                    Task t1 = Task.Run(() => SendMessage(recieverMail.MobileNo, emailBody.ToString()));
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
    }
}
