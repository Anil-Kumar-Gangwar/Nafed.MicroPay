using AutoMapper;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;
using static Nafed.MicroPay.ImportExport.ArrearReportExport;
using System.Data;
using System.Globalization;
using static Nafed.MicroPay.ImportExport.SalaryReportExport;

namespace Nafed.MicroPay.Services
{
    public class PayrollApprovalSettingService : BaseService, IPayrollApprovalSettingService, IPayrollApprovalEmail
    {
        private readonly IGenericRepository genericRepo;
        private readonly ISalaryRepository salaryRepo;
        private readonly IArrearRepository arrearRepo;
        private readonly ISalaryReportRepository salReportRepo;
        public PayrollApprovalSettingService(IGenericRepository genericRepo,
            ISalaryRepository salaryRepo, IArrearRepository arrearRepo, ISalaryReportRepository salReportRepo)
        {
            this.genericRepo = genericRepo;
            this.salaryRepo = salaryRepo;
            this.arrearRepo = arrearRepo;
            this.salReportRepo = salReportRepo;
        }
        public List<PayrollApprovalSetting> GetApprovalSetting()
        {
            log.Info($"PayrollApprovalSettingService/GetApprovalSetting");
            try
            {
                List<PayrollApprovalSetting> approvalSetting = new List<PayrollApprovalSetting>();

                var processList = new[] { (int)WorkFlowProcess.SalaryGenerate, (int)WorkFlowProcess.DAArrearGenerate,
                    (int)WorkFlowProcess.PayArrearGenerate };

                if (genericRepo.Exists<DTOModel.PayrollApprovalSetting>(x => processList.Contains(x.ProcessID)))
                {
                    var dtoprocess = genericRepo.Get<DTOModel.Process>(x => processList.Contains(x.ProcessID)).ToList();

                    var dtoPayrollApprovalSeetting = genericRepo.Get<DTOModel.PayrollApprovalSetting>(x =>
                     x.ToDate == null && processList.Contains(x.ProcessID)).ToList();


                    approvalSetting = (from left in dtoprocess
                                       join right in dtoPayrollApprovalSeetting on left.ProcessID
                                       equals right.ProcessID into joinedList
                                       from sub in joinedList.DefaultIfEmpty()

                                       select new PayrollApprovalSetting()
                                       {
                                           ProcessName = left.ProcessName,
                                           OldReporting1 = sub == null ? 0 : sub.Reporting1,
                                           OldReporting2 = sub == null ? 0 : sub.Reporting2,
                                           OldReporting3 = sub == null ? 0 : sub.Reporting3,
                                           Reporting1 = sub == null ? 0 : sub.Reporting1,
                                           Reporting2 = sub == null ? 0 : sub.Reporting2.HasValue ? sub.Reporting2.Value : 0,
                                           Reporting3 = sub == null ? 0 : sub.Reporting3.HasValue ? sub.Reporting3.Value : 0,
                                           ProcessAppID = sub == null ? 0 : sub.ProcessAppID,
                                           CreatedBy = sub == null ? 0 : sub.CreatedBy,
                                           CreatedOn = sub == null ? DateTime.Now : sub.CreatedOn,
                                           ProcessID = sub == null ? left.ProcessID : sub.ProcessID,

                                       }).ToList();
                }
                else
                {
                    var res1 = genericRepo.Get<DTOModel.Process>(x => processList.Contains(x.ProcessID)).ToList();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.Process, Model.PayrollApprovalSetting>()
                        .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                        .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.ProcessName))
                        .ForAllOtherMembers(d => d.Ignore())
                        ;

                    });
                    approvalSetting = Mapper.Map<List<Model.PayrollApprovalSetting>>(res1);
                }
                return approvalSetting;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public bool InsertPayrollApprovalSetting(List<PayrollApprovalSetting> pApprovalSetting)
        {
            log.Info($"PayrollApprovalSettingService/InsertPayrollApprovalSetting");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<PayrollApprovalSetting, DTOModel.PayrollApprovalSetting>()
                    .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))


                    .ForMember(d => d.Reporting1, o => o.MapFrom(s => s.Reporting1))
                    .ForMember(d => d.Reporting2, o => o.MapFrom(s => s.Reporting2))
                    .ForMember(d => d.Reporting3, o => o.MapFrom(s => s.Reporting3))
                    .ForMember(d => d.FromDate, o => o.MapFrom(s => s.FromDate))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForAllOtherMembers(d => d.Ignore());
                });

                var dtopApprovalSetting = Mapper.Map<List<DTOModel.PayrollApprovalSetting>>(pApprovalSetting);
                if (dtopApprovalSetting != null)
                    genericRepo.AddMultipleEntity<DTOModel.PayrollApprovalSetting>(dtopApprovalSetting);


                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }
            return flag;
        }

        public bool UpdatePayrollApprovalSetting(List<PayrollApprovalSetting> pApprovalSetting)
        {
            log.Info($"PayrollApprovalSettingService/UpdatePayrollApprovalSetting");
            bool flag = false;
            try
            {
                for (int i = 0; i < pApprovalSetting.Count; i++)
                {
                    var dtoObj = genericRepo.GetByID<DTOModel.PayrollApprovalSetting>
                        (pApprovalSetting[i].ProcessAppID);
                    if (dtoObj != null)
                    {
                        dtoObj.ToDate = DateTime.Now.Date.AddDays(-1);
                        dtoObj.UpdatedBy = pApprovalSetting[i].UpdatedBy;
                        dtoObj.UpdatedOn = pApprovalSetting[i].UpdatedOn;
                        genericRepo.Update<DTOModel.PayrollApprovalSetting>(dtoObj);
                        flag = true;
                    }
                }
                flag = InsertPayrollApprovalSetting(pApprovalSetting);
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }
            return flag;
        }

        public List<PayrollApprovalRequest> GetSalaryApprovalRequests(WorkFlowProcess wrkProcessID, int reportingEmpID)
        {
            log.Info($"PayrollApprovalSettingService/GetSalaryApprovalRequests");
            try
            {
                List<DTOModel.PayrollApprovalRequest> dtoApprovalRequests = new List<DTOModel.PayrollApprovalRequest>();

                var approvalSetting = genericRepo.Get<DTOModel.PayrollApprovalSetting>(x =>
                 x.ProcessID == (int)wrkProcessID && x.ToDate == null).FirstOrDefault();


                if (reportingEmpID == approvalSetting.Reporting1)
                {
                    dtoApprovalRequests = genericRepo.Get<DTOModel.PayrollApprovalRequest>(x =>
                    x.ProcessID == (int)wrkProcessID && x.Status == 1).OrderByDescending(x => x.ApprovalRequestID).ToList();
                }

                else if (reportingEmpID == approvalSetting.Reporting2)
                {
                    dtoApprovalRequests = genericRepo.Get<DTOModel.PayrollApprovalRequest>(x =>
                    x.ProcessID == (int)wrkProcessID && x.Status <= 3).OrderByDescending(x => x.ApprovalRequestID).ToList();
                }

                else if (reportingEmpID == approvalSetting.Reporting3)
                {
                    dtoApprovalRequests = genericRepo.Get<DTOModel.PayrollApprovalRequest>(x =>
                   x.ProcessID == (int)wrkProcessID && x.Status >= 3).OrderByDescending(x => x.ApprovalRequestID).ToList();

                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.PayrollApprovalRequest, PayrollApprovalRequest>()
                   .ForMember(d => d.EmpployeeTypeName, o => o.MapFrom(s => s.EmployeeType.EmployeeTypeName))
                   .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName));
                });

                return Mapper.Map<List<PayrollApprovalRequest>>(dtoApprovalRequests);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<PayrollApprovalRequest> GetDAArrearApprovalRequest(WorkFlowProcess wrkProcessID, int reportingEmpID)
        {
            log.Info($"PayrollApprovalSettingService/GetDAArrearApprovalRequest");
            try
            {
                List<DTOModel.PayrollApprovalRequest> dtoApprovalRequests = new List<DTOModel.PayrollApprovalRequest>();

                var approvalSetting = genericRepo.Get<DTOModel.PayrollApprovalSetting>(x =>
                 x.ProcessID == (int)wrkProcessID && x.ToDate == null).FirstOrDefault();


                if (reportingEmpID == approvalSetting.Reporting1)
                {
                    dtoApprovalRequests = genericRepo.Get<DTOModel.PayrollApprovalRequest>(x =>
                    x.ProcessID == (int)wrkProcessID && x.Status == 1).ToList();
                }

                else if (reportingEmpID == approvalSetting.Reporting2)
                {
                    dtoApprovalRequests = genericRepo.Get<DTOModel.PayrollApprovalRequest>(x =>
                    x.ProcessID == (int)wrkProcessID && x.Status <= 3).ToList();
                }

                else if (reportingEmpID == approvalSetting.Reporting3)
                {
                    dtoApprovalRequests = genericRepo.Get<DTOModel.PayrollApprovalRequest>(x =>
                   x.ProcessID == (int)wrkProcessID && x.Status >= 3).ToList();

                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.PayrollApprovalRequest, PayrollApprovalRequest>()
                   .ForMember(d => d.EmpployeeTypeName, o => o.MapFrom(s => s.EmployeeType.EmployeeTypeName))
                   .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName));
                });

                return Mapper.Map<List<PayrollApprovalRequest>>(dtoApprovalRequests);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<PayrollApprovalRequest> GetPayArrearApprovalRequest(WorkFlowProcess wrkProcessID, int reportingEmpID)
        {
            log.Info($"PayrollApprovalSettingService/GetDAArrearApprovalRequest");
            try
            {
                List<DTOModel.PayrollApprovalRequest> dtoApprovalRequests = new List<DTOModel.PayrollApprovalRequest>();

                var approvalSetting = genericRepo.Get<DTOModel.PayrollApprovalSetting>(x =>
                 x.ProcessID == (int)wrkProcessID && x.ToDate == null).FirstOrDefault();


                if (reportingEmpID == approvalSetting.Reporting1)
                {
                    dtoApprovalRequests = genericRepo.Get<DTOModel.PayrollApprovalRequest>(x =>
                    x.ProcessID == (int)wrkProcessID && x.Status == 1).ToList();
                }

                else if (reportingEmpID == approvalSetting.Reporting2)
                {
                    dtoApprovalRequests = genericRepo.Get<DTOModel.PayrollApprovalRequest>(x =>
                    x.ProcessID == (int)wrkProcessID && x.Status <= 3).ToList();
                }

                else if (reportingEmpID == approvalSetting.Reporting3)
                {
                    dtoApprovalRequests = genericRepo.Get<DTOModel.PayrollApprovalRequest>(x =>
                   x.ProcessID == (int)wrkProcessID && x.Status >= 3).ToList();

                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.PayrollApprovalRequest, PayrollApprovalRequest>()
                   .ForMember(d => d.EmpployeeTypeName, o => o.MapFrom(s => s.EmployeeType.EmployeeTypeName))
                   .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName));
                });

                return Mapper.Map<List<PayrollApprovalRequest>>(dtoApprovalRequests);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SubmitApprovalRequest(PayrollApprovalRequest request, string filePath)
        {
            log.Info($"PayrollApprovalSettingService/SubmitApprovalRequest");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<PayrollApprovalRequest, DTOModel.PayrollApprovalRequest>();
                });

                var dtoApprovalRequest = Mapper.Map<DTOModel.PayrollApprovalRequest>(request);

                genericRepo.Update<DTOModel.PayrollApprovalRequest>(dtoApprovalRequest);

                if (request.Status == (int)ApprovalStatus.RejectedByReporting2 || request.Status == (int)ApprovalStatus.ApprovedByReporting2)
                {
                    //var reportingPersonEmails = genericRepo.Get<DTOModel.PayrollApprovalSetting>(x =>
                    // x.ProcessID == (int)WorkFlowProcess.SalaryGenerate).FirstOrDefault();

                    //request.Reporting1 = reportingPersonEmails.Reporting1;
                    //request.Reporting2 = reportingPersonEmails.Reporting2;
                    //request.Reporting3 = reportingPersonEmails.Reporting3;

                    var employee = genericRepo.GetByID<DTOModel.tblMstEmployee>(request.Reporting3);
                    request.emailTo = employee.OfficialEmail;

                    if (!request.BranchID.HasValue)
                        request.BranchName = request.BranchCode == "Except-HO" ? "All Branches (Except HO)" : "All";

                    Task t1 = Task.Run(() => SendApprovalEmail(request));
                }

                else if (request.Status == (int)ApprovalStatus.ApprovedByReporting3)
                {
                    /// === Set Publish =1 in tblFinalMonthlySalary table ========
                    /// 
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<PayrollApprovalRequest, DTOModel.PayrollApprovalRequest>();
                    });

                    var dtoPayrollApprovalRequest = Mapper.Map<DTOModel.PayrollApprovalRequest>(request);
                    flag = salaryRepo.PublishSalary(dtoPayrollApprovalRequest);
                    if (flag && !request.BranchID.HasValue)
                    {
                        var repo3 = genericRepo.GetByID<DTOModel.tblMstEmployee>(request.Reporting3).OfficialEmail;
                        var repo2 = genericRepo.GetByID<DTOModel.tblMstEmployee>(request.Reporting2).OfficialEmail;
                        var repo1 = genericRepo.GetByID<DTOModel.tblMstEmployee>(request.Reporting1).OfficialEmail;
                        List<int> logIds = null;
                        Task t2 = Task.Run(() => SendSalaryReportToBM(request, filePath, out logIds)).ContinueWith(t =>
                        SendFailedMailLogToReporting3(logIds, repo1, repo2, repo3)
                        );

                    }
                }
                else if (request.Status == (int)ApprovalStatus.RejectedByReporting3)
                {
                    /// === Set Publish =0 in tblFinalMonthlySalary table ========
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<PayrollApprovalRequest, DTOModel.PayrollApprovalRequest>();
                    });

                    var dtoPayrollApprovalRequest = Mapper.Map<DTOModel.PayrollApprovalRequest>(request);
                    flag = salaryRepo.UndoPublishSalary(dtoPayrollApprovalRequest);
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

        public bool SubmitDAApprovalRequest(PayrollApprovalRequest request)
        {
            log.Info($"PayrollApprovalSettingService/SubmitDAApprovalRequest");
            bool flag = false;

            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<PayrollApprovalRequest, DTOModel.PayrollApprovalRequest>();
                });

                var dtoApprovalRequest = Mapper.Map<DTOModel.PayrollApprovalRequest>(request);

                genericRepo.Update<DTOModel.PayrollApprovalRequest>(dtoApprovalRequest);

                if (request.Status == (int)ApprovalStatus.RejectedByReporting2 || request.Status == (int)ApprovalStatus.ApprovedByReporting2)
                {
                    var employee = genericRepo.GetByID<DTOModel.tblMstEmployee>(request.Reporting3);
                    request.emailTo = employee.OfficialEmail;

                    if (!request.BranchID.HasValue)
                        request.BranchName = request.BranchCode == "Except-HO" ? "All Branches (Except HO)" : "All";

                    Task t1 = Task.Run(() => SendApprovalEmail(request));
                }

                else if (request.Status == (int)ApprovalStatus.ApprovedByReporting3)
                {
                    /// === Set Publish =1 in tblFinalMonthlySalary table ========
                    /// 
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<PayrollApprovalRequest, DTOModel.PayrollApprovalRequest>();
                    });

                    var dtoPayrollApprovalRequest = Mapper.Map<DTOModel.PayrollApprovalRequest>(request);
                    flag = salaryRepo.PublishDAArrer(dtoPayrollApprovalRequest);

                }
                else if (request.Status == (int)ApprovalStatus.RejectedByReporting3)
                {
                    /// === Set Publish =0 in tblFinalMonthlySalary table ========
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<PayrollApprovalRequest, DTOModel.PayrollApprovalRequest>();
                    });
                    var dtoPayrollApprovalRequest = Mapper.Map<DTOModel.PayrollApprovalRequest>(request);
                    flag = salaryRepo.UndoPublishDAArrear(dtoPayrollApprovalRequest);
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

        public bool SubmitPayArrearApprovalRequest(PayrollApprovalRequest request)
        {
            log.Info($"PayrollApprovalSettingService/SubmitPayArrearApprovalRequest");

            bool flag = false;

            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<PayrollApprovalRequest, DTOModel.PayrollApprovalRequest>();
                });

                var dtoApprovalRequest = Mapper.Map<DTOModel.PayrollApprovalRequest>(request);

                genericRepo.Update<DTOModel.PayrollApprovalRequest>(dtoApprovalRequest);

                if (request.Status == (int)ApprovalStatus.RejectedByReporting2 || request.Status == (int)ApprovalStatus.ApprovedByReporting2)
                {
                    var employee = genericRepo.GetByID<DTOModel.tblMstEmployee>(request.Reporting3);
                    request.emailTo = employee.OfficialEmail;

                    if (!request.BranchID.HasValue)
                        request.BranchName = request.BranchCode == "Except-HO" ? "All Branches (Except HO)" : "All";

                    Task t1 = Task.Run(() => SendApprovalEmail(request));
                }

                else if (request.Status == (int)ApprovalStatus.ApprovedByReporting3)
                {
                    /// === Set Publish =1 in tblFinalMonthlySalary table ========
                    /// 
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<PayrollApprovalRequest, DTOModel.PayrollApprovalRequest>();
                    });

                    var dtoPayrollApprovalRequest = Mapper.Map<DTOModel.PayrollApprovalRequest>(request);
                    flag = salaryRepo.PublishPayArrear(dtoPayrollApprovalRequest);

                }
                else if (request.Status == (int)ApprovalStatus.RejectedByReporting3)
                {
                    /// === Set Publish =0 in tblFinalMonthlySalary table ========
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<PayrollApprovalRequest, DTOModel.PayrollApprovalRequest>();
                    });
                    var dtoPayrollApprovalRequest = Mapper.Map<DTOModel.PayrollApprovalRequest>(request);
                    flag = salaryRepo.UndoPublishPayArrear(dtoPayrollApprovalRequest);
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
        public void SendApprovalEmail(PayrollApprovalRequest request)
        {
            log.Info($"PayrollApprovalSettingService/SendApprovalEmail");

            try
            {
                EmailMessage emailMessage = new EmailMessage();
                StringBuilder mailBody = new StringBuilder();

                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmailConfiguration, EmailMessage>()
                    .ForMember(d => d.From, o => o.MapFrom(s => $"NAFED HRMS <{s.ToEmail}>"))
                    .ForMember(d => d.To, o => o.UseValue(request.emailTo))
                    .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                    .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                    .ForMember(d => d.SmtpClientHost, o => o.MapFrom(s => s.Server))
                    .ForMember(d => d.SmtpPort, o => o.MapFrom(s => s.Port))
                    .ForMember(d => d.enableSSL, o => o.MapFrom(s => s.SSLStatus))
                    .ForMember(d => d.HTMLView, o => o.UseValue(true))
                    .ForMember(d => d.FriendlyName, o => o.UseValue("NAFED"));
                });

                emailMessage = Mapper.Map<EmailMessage>(emailsetting);

                if (request.Status == (int)ApprovalStatus.ApprovedByReporting2)
                {
                    if (request.ProcessID == (int)WorkFlowProcess.SalaryGenerate)  //====  Publish Salary Tab 1
                    {
                        emailMessage.Subject = $"REVIEW & APPROVAL OF SALARY BILL";
                        mailBody.Clear();
                        mailBody.AppendFormat("<div>Dear Sir,</div> <br> ");
                        mailBody.AppendFormat($"<div>Salary bill for the month of <b>{request.periodInDateFormat.Value.ToString("MMM, yyyy")}</b> towards branch : <b> {request.BranchName} </b> has been reviewed. Kindly approve the same.<br> <br>");

                        mailBody.AppendFormat($"<div><b>Remark :</b> {request.Comments} <br> <br>");

                        mailBody.AppendFormat($"<div>Regards, </div> <br>");
                        mailBody.AppendFormat($"<div>F & A Team, </div> <br>");
                        mailBody.AppendFormat($"<div>Nafed  </div> <br> <br>");
                    }
                    else if (request.ProcessID == (int)WorkFlowProcess.DAArrearGenerate) //====  Publish DA Arrer Tab 2
                    {
                        emailMessage.Subject = $"REVIEW & APPROVAL OF DA ARREAR BILL";
                        mailBody.AppendFormat($"<div>DA Arrear bill for the month of <b>{request.periodInDateFormat.Value.ToString("MMM, yyyy")}</b> towards branch : <b> {request.BranchName} </b> has been reviewed. Kindly approve the same.<br> <br>");

                        mailBody.AppendFormat($"<div><b>Remark :</b> {request.Comments} <br> <br>");

                        mailBody.AppendFormat($"<div>Regards, </div> <br>");
                        mailBody.AppendFormat($"<div>F & A Team, </div> <br>");
                        mailBody.AppendFormat($"<div>Nafed  </div> <br> <br>");
                    }
                    else if (request.ProcessID == (int)WorkFlowProcess.PayArrearGenerate)
                    {
                        emailMessage.Subject = $"REVIEW & APPROVAL OF PAY ARREAR BILL";
                        mailBody.AppendFormat($"<div>PAY Arrear bill for the month of <b>{request.periodInDateFormat.Value.ToString("MMM, yyyy")}</b> towards branch : <b> {request.BranchName} </b> has been reviewed. Kindly approve the same.<br> <br>");

                        mailBody.AppendFormat($"<div><b>Remark :</b> {request.Comments} <br> <br>");

                        mailBody.AppendFormat($"<div>Regards, </div> <br>");
                        mailBody.AppendFormat($"<div>F & A Team, </div> <br>");
                        mailBody.AppendFormat($"<div>Nafed  </div> <br> <br>");
                    }
                }
                else if (request.Status == (int)ApprovalStatus.RejectedByReporting2)
                {
                    if (request.ProcessID == (int)WorkFlowProcess.SalaryGenerate)  //====  Publish Salary Tab 1 =
                    {
                        emailMessage.Subject = $"REVIEW & REJECTION OF SALARY BILL";
                        mailBody.Clear();
                        mailBody.AppendFormat("<div>Dear Sir,</div> <br>");
                        mailBody.AppendFormat($"<div>Salary bill for the month of <b> {request.periodInDateFormat.Value.ToString("MMM, yyyy")} </b> towards branch : <b>{request.BranchName} </b> has been rejected.Kindly reject the same.<br> <br>");

                        mailBody.AppendFormat($"<div><b>Remark :</b> {request.Comments} <br> <br>");

                        mailBody.AppendFormat($"<div>Regards, </div> <br>");
                        mailBody.AppendFormat($"<div>F & A Team,  </div> <br>");
                        mailBody.AppendFormat($"<div>Nafed  </div> <br> <br>");
                    }
                    else if (request.ProcessID == (int)WorkFlowProcess.DAArrearGenerate)  //====  Publish DA ARREAR Tab 2 =
                    {

                        emailMessage.Subject = $"REVIEW & REJECTION OF DA ARREAR BILL";
                        mailBody.Clear();
                        mailBody.AppendFormat("<div>Dear Sir,</div> <br>");
                        mailBody.AppendFormat($"<div>DA Arrear bill for the month of <b> {request.periodInDateFormat.Value.ToString("MMM, yyyy")} </b> towards branch : <b>{request.BranchName} </b> has been rejected.Kindly reject the same.<br> <br>");

                        mailBody.AppendFormat($"<div><b>Remark :</b> {request.Comments} <br> <br>");

                        mailBody.AppendFormat($"<div>Regards, </div> <br>");
                        mailBody.AppendFormat($"<div>F & A Team,  </div> <br>");
                        mailBody.AppendFormat($"<div>Nafed  </div> <br> <br>");
                    }
                    else if (request.ProcessID == (int)WorkFlowProcess.PayArrearGenerate)  //==== Publish PAY ARREAR tab 3 === 
                    {
                        emailMessage.Subject = $"REVIEW & REJECTION OF PAY ARREAR BILL";
                        mailBody.Clear();
                        mailBody.AppendFormat("<div>Dear Sir,</div> <br>");
                        mailBody.AppendFormat($"<div>PAY Arrear bill for the month of <b> {request.periodInDateFormat.Value.ToString("MMM, yyyy")} </b> towards branch : <b>{request.BranchName} </b> has been rejected.Kindly reject the same.<br> <br>");

                        mailBody.AppendFormat($"<div><b>Remark :</b> {request.Comments} <br> <br>");

                        mailBody.AppendFormat($"<div>Regards, </div> <br>");
                        mailBody.AppendFormat($"<div>F & A Team,  </div> <br>");
                        mailBody.AppendFormat($"<div>Nafed  </div> <br> <br>");
                    }
                }

                emailMessage.Body = mailBody.ToString();
                EmailHelper.SendEmail(emailMessage);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SendDAArrearApprovalRequest(ArrearApprovalRequest request, out bool requestExist)
        {
            log.Info($"PayrollApprovalSetting/SendDAArrearApprovalRequest");
            bool flag = false;
            try
            {
                requestExist = flag;
                string period = $"{request.salYear}{request.salMonth.ToString("00")}";


                string branchCode = string.Empty;

                if (request.enumDABranch == EnumDABranch.BranchesExcecptHO)
                    branchCode = "Except-HO";
                else if (request.enumDABranch == EnumDABranch.SingleBranch && request.selectedDABranchID.HasValue)
                    branchCode = genericRepo.Get<DTOModel.Branch>(
                        x => x.BranchID == request.selectedDABranchID.Value).FirstOrDefault().BranchCode;

                if (!genericRepo.Exists<DTOModel.PayrollApprovalRequest>(x =>
                     x.Period == period && x.ProcessID == request.ProcessID
                     && (request.selectedDABranchID.HasValue ? (x.BranchID == request.selectedDABranchID.Value) : (1 > 0))
                     && (request.selectedDAEmpTypeID.HasValue ? (x.EmployeeTypeID == request.selectedDAEmpTypeID.Value) : (1 > 0))
                     && (request.selectedDAEmpID.HasValue ? (x.EmployeeID == request.selectedDAEmpID.Value) : (1 > 0))
                ))
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<ArrearApprovalRequest, DTOModel.PayrollApprovalRequest>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.selectedDAEmpID))
                        .ForMember(d => d.BranchID, o => o.MapFrom(s => s.selectedDABranchID))
                        .ForMember(d => d.EmployeeTypeID, o => o.MapFrom(s => s.selectedDAEmpTypeID))
                        .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                        .ForMember(d => d.BranchCode, o => o.UseValue(branchCode))
                        .ForMember(d => d.Period, o => o.UseValue(period))
                        .ForMember(d => d.Status, o => o.UseValue(1));
                    });
                    var dtoArrearApprovalRequest = Mapper.Map<DTOModel.PayrollApprovalRequest>(request);
                    genericRepo.Insert<DTOModel.PayrollApprovalRequest>(dtoArrearApprovalRequest);
                    flag = true;
                }
                else
                    requestExist = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool SendPayArrearApprovalRequest(ArrearApprovalRequest request, out bool requestExist)
        {
            log.Info($"PayrollApprovalSetting/SendPayArrearApprovalRequest");
            bool flag = false;
            try
            {
                requestExist = flag;
                string period = $"{request.salYear}{request.salMonth.ToString("00")}";
                string branchCode = string.Empty;


                if (request.enumPayBranch == EnumPayBranch.BranchesExcecptHO)
                    branchCode = "Except-HO";
                else if (request.enumPayBranch == EnumPayBranch.SingleBranch && request.selectedDABranchID.HasValue)
                    branchCode = genericRepo.Get<DTOModel.Branch>(
                        x => x.BranchID == request.selectedDABranchID.Value).FirstOrDefault().BranchCode;


                if (!genericRepo.Exists<DTOModel.PayrollApprovalRequest>(x =>
                     x.Period == period && x.ProcessID == request.ProcessID
                     && (request.selectedPayBranchID.HasValue ? (x.BranchID == request.selectedPayBranchID.Value) : (1 > 0))
                     && (request.selectedPayEmpTypeID.HasValue ? (x.EmployeeTypeID == request.selectedPayEmpTypeID.Value) : (1 > 0))
                     && (request.selectedPayEmpID.HasValue ? (x.EmployeeID == request.selectedPayEmpID.Value) : (1 > 0))
                ))
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<ArrearApprovalRequest, DTOModel.PayrollApprovalRequest>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.selectedPayEmpID))
                        .ForMember(d => d.BranchID, o => o.MapFrom(s => s.selectedPayBranchID))
                        .ForMember(d => d.EmployeeTypeID, o => o.MapFrom(s => s.selectedPayEmpTypeID))
                        .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                        .ForMember(d => d.BranchCode, o => o.UseValue(branchCode))
                        .ForMember(d => d.Period, o => o.UseValue(period))
                        .ForMember(d => d.Status, o => o.UseValue(1));
                    });
                    var dtoArrearApprovalRequest = Mapper.Map<DTOModel.PayrollApprovalRequest>(request);
                    genericRepo.Insert<DTOModel.PayrollApprovalRequest>(dtoArrearApprovalRequest);
                    flag = true;
                }
                else
                    requestExist = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public string GetArrearReport(ArrearReportFilter rFilter)
        {
            log.Info($"PayrollApprovalSetting/GetDAArrearReport/");

            try
            {
                string result = string.Empty; string sFullPath = string.Empty;

                if (Directory.Exists(rFilter.filePath))
                {
                    sFullPath = $"{rFilter.filePath}{rFilter.fileName}";

                    if (rFilter.arrearType.Equals('D'))
                    {
                        DataTable reportData = arrearRepo.GetArrearReport(rFilter);

                        if (reportData?.Rows.Count > 0)
                        {
                            IEnumerable<string> exportHdr = Enumerable.Empty<string>();

                            exportHdr = reportData.Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();
                            exportHdr.ToList().Add("#");
                            var res = DAArrearExportToExcel(exportHdr, reportData, rFilter.pFrom, rFilter.pTo, "DA Arrear Report", sFullPath);
                        }
                        /// === DA Arrear Report....
                    }
                    else if (rFilter.arrearType.Equals('B'))
                    {
                        //==== Pay Arrear Report====

                        DataTable payArrearData = new DataTable();
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();

                        exportHdr = payArrearData.Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();
                        var res = PayArrearExportToExcel(exportHdr, payArrearData, rFilter.pFrom, rFilter.pTo, "Pay Arrear Report", sFullPath);

                    }
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void SendSalaryReportToBM(PayrollApprovalRequest request, string filePath, out List<int> logIds)
        {
            log.Info($"PayrollApprovalSettingService/SendSalaryReportToBM");
            logIds = new List<int>();
            MailFailedLog obj = new MailFailedLog();
            EmailMessage emailMessage = new EmailMessage();
            StringBuilder mailBody = new StringBuilder();
            var year = Convert.ToInt32(request.Period.Substring(0, 4));
            var month = Convert.ToInt32(request.Period.Substring(4, 2));
            SalaryReportFilter filter = new SalaryReportFilter()
            {
                salMonth = (byte)month,
                salYear = (short)year,
                employeeTypeID = request.EmployeeTypeID,
                filePath = filePath
            };
            var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.EmailConfiguration, EmailMessage>()
                .ForMember(d => d.From, o => o.MapFrom(s => $"NAFED HRMS <{s.ToEmail}>"))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                .ForMember(d => d.SmtpClientHost, o => o.MapFrom(s => s.Server))
                .ForMember(d => d.SmtpPort, o => o.MapFrom(s => s.Port))
                .ForMember(d => d.enableSSL, o => o.MapFrom(s => s.SSLStatus))
                .ForMember(d => d.HTMLView, o => o.UseValue(true))
                .ForMember(d => d.FriendlyName, o => o.UseValue("NAFED"));
            });
            emailMessage = Mapper.Map<EmailMessage>(emailsetting);

            var repo1 = genericRepo.GetByID<DTOModel.tblMstEmployee>(request.Reporting1).OfficialEmail;
            var repo2 = genericRepo.GetByID<DTOModel.tblMstEmployee>(request.Reporting2).OfficialEmail;
            var repo3 = genericRepo.GetByID<DTOModel.tblMstEmployee>(request.Reporting3).OfficialEmail;
            // Get All Branch mail for email sending 
        //    var employeeList = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.BranchID != 44 && x.IsBM == true && x.IsDeleted == false && x.DOLeaveOrg == null).Select(s => new { s.EmployeeId, s.OfficialEmail, s.BranchID, s.Branch.BranchName, s.EmployeeTypeID, s.Branch.EmailId }).ToArray();

            var branchList = genericRepo.Get<DTOModel.Branch>(x => x.IsDeleted == false && x.BranchID != 44);
            int cntr = 0;
            foreach (var branch in branchList)
            {
                try
                {
                    obj.WorkingArea = 1; /*(int)Common.WorkingArea.SalaryApproval*/;
                    obj.EmployeeId = 1;// employee.EmployeeId;
                    obj.BranchId = branch.BranchID;
                    obj.EmployeeTypeId = 5;
                    obj.SalMonth = (byte)month;
                    obj.SalYear = (short)year;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = (int)request.UpdatedBy;

                    if (!string.IsNullOrEmpty(branch.EmailId))
                    {
                        emailMessage.Subject = $"SALARY REPORT FOR THE {branch.BranchName.ToUpper()} BRANCH";
                        mailBody.Clear();
                        mailBody.AppendFormat("<div>Dear Sir/Madam,</div> <br> ");
                        mailBody.AppendFormat($"<div>Salary report for the month of <b>{request.periodInDateFormat.Value.ToString("MMM, yyyy")}</b> has been generated. Kindly check.<br> <br>");
                        mailBody.AppendFormat($"<div>Regards, </div> <br>");
                        mailBody.AppendFormat($"<div>F & A Team, </div> <br>");
                        mailBody.AppendFormat($"<div>Nafed</div> <br> <br>");
                        filter.branchID = branch.BranchID;
                        GenerateEmployeeMonthlySalaryReport(filter);
                        emailMessage.To = branch.EmailId;
                        emailMessage.Bcc = repo1 + "," + repo2 + "," + repo3;
                        emailMessage.Attachments = GetSalaryReportAttachment(filter.fileName);
                        emailMessage.Body = mailBody.ToString();
                        EmailHelper.SendEmail(emailMessage);
                        foreach (var attachment in emailMessage.Attachments)
                        {
                            attachment.Content.Dispose();
                        }
                    }
                    else
                    {
                        obj.Reason = $"Official mail Id is missing";
                        int id = InsertMailFailedLog(obj);
                        logIds.Add(id);
                        cntr++;

                    }
                }
                catch (Exception ex)
                {
                    obj.Reason = ex.Message;
                    int id = InsertMailFailedLog(obj);
                    logIds.Add(id);
                    log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                }
            }

        }
        public static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }
        private List<MailAttachment> GetSalaryReportAttachment(string fileName)
        {
            log.Info($"PayRollApprovalService/GetSalaryReportAttachment");

            List<MailAttachment> attachments = new List<MailAttachment>();
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileDownload/" + fileName);

                if (System.IO.File.Exists(fullPath))
                {
                    byte[] tmpBytes = ReadFile(fullPath);
                    MemoryStream ms = new MemoryStream();
                    using (MemoryStream tempStream = new MemoryStream())
                    {
                        ms.Write(tmpBytes, 0, tmpBytes.Length);
                    }
                    if ((ms != null) && (ms.Length != 0))
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        ms.Position = 0;
                    }
                    attachments.Add(new MailAttachment { Content = ms, FileName = fileName });

                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.InnerException + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return attachments;
        }


        public string GenerateEmployeeMonthlySalaryReport(SalaryReportFilter rFilter)
        {
            log.Info($"PayRollApprovalService/GenerateEmployeeMonthlySalaryReport");

            string result = string.Empty; string sFullPath = string.Empty;
            string selectedPeriod = $"{rFilter.salYear.ToString()}{rFilter.salMonth.ToString("00")}";

            if (genericRepo.Exists<DTOModel.salaryheadshistory>(x => x.EmployeeTypeID == rFilter.employeeTypeID.Value
             && x.Period == selectedPeriod))
            {
                if (Directory.Exists(rFilter.filePath))
                {
                    int nE_Cols = 0, nD_Cols = 0;

                    DataSet dsReportData = salReportRepo.GetMonthlyEmployeeWiseReport(rFilter.salMonth, rFilter.salYear,
                        rFilter.employeeTypeID, rFilter.branchID, out nE_Cols, out nD_Cols);

                    if (dsReportData != null && dsReportData.Tables[0].Rows.Count > 0)  //====== export report if there is data ========= 
                    {
                        DataTable dtBaseData = new DataTable();
                        dtBaseData = dsReportData.Tables[0].Clone();

                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dsReportData.Tables[0].Columns.Cast<DataColumn>()
                            .Where(x => (x.ColumnName.ToString() != "BranchCode"
                            )).Select(x => x.ColumnName).AsEnumerable<string>();

                        var distinctBrachCodes = (from row in dsReportData.Tables[0].AsEnumerable()
                                                  select row.Field<string>("BranchCode")).Distinct();

                        foreach (var item in distinctBrachCodes)
                        {
                            var drArray = (from myRow in dsReportData.Tables[0].AsEnumerable()
                                           where myRow.Field<string>("BranchCode") == item
                                           select myRow).ToArray<DataRow>();

                            var drTotArray = (from myRow in dsReportData.Tables[3].AsEnumerable()
                                              where myRow.Field<string>("BranchCode") == item
                                              select myRow).FirstOrDefault();

                            foreach (DataRow dr in drArray)
                                dtBaseData.ImportRow(dr);

                            drTotArray["SNO"] = -99;
                            dtBaseData.ImportRow(drTotArray);

                            dtBaseData.Columns.Remove("BranchCode");
                            DateTime date = new DateTime(rFilter.salYear, rFilter.salMonth, 1);
                            var filename = "SalaryReport_" + drTotArray.ItemArray[2].ToString() + "_" + date.ToString("MMMM") + rFilter.salYear.ToString() + ".xlsx";
                            sFullPath = $"{rFilter.filePath}{filename}";
                            result = MonthlyEmployeeWiseExportToExcel(exportHdr, dtBaseData, dsReportData.Tables[1], dsReportData.Tables[2], nE_Cols, nD_Cols, $"Employee Wise Monthly-{selectedPeriod}", sFullPath);
                            rFilter.fileName = filename;
                        }


                    }
                    else
                        result = "norec";
                }
            }
            else
                result = "notfound";
            return result;
        }

        public int InsertMailFailedLog(MailFailedLog model)
        {
            log.Info($"PayrollApprovalSettingService/InsertMailFailedLog");

            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<MailFailedLog, DTOModel.MailFailedLog>();
                });
                var dtoMailFailed = Mapper.Map<DTOModel.MailFailedLog>(model);
                if (dtoMailFailed != null)
                    genericRepo.Insert<DTOModel.MailFailedLog>(dtoMailFailed);

                return dtoMailFailed.Id;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool SendFailedMailLogToReporting3(List<int> logIds, string reporting1Mail, string reporting2Mail, string reporting3Mail)
        {
            log.Info($"PayrollApprovalSettingService/SendFailedMailLogToReporting3");

            try
            {
                if (logIds.Count > 0)
                {
                    List<dynamic> employeeDetail = new List<dynamic>();
                    var _logIds = logIds.ToArray();
                    var failedMails = genericRepo.Get<DTOModel.MailFailedLog>(x => _logIds.Contains(x.Id));

                    var period = new DateTime((int)failedMails.First().SalYear, (int)failedMails.First().SalMonth, 1);

                    foreach (var item in failedMails)
                    {
                        employeeDetail.Add(new
                        {
                            branchName = item.Branch.BranchName,
                            reason = item.Reason,
                        });
                    }

                    EmailMessage emailMessage = new EmailMessage();
                    StringBuilder mailBody = new StringBuilder();

                    var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.EmailConfiguration, EmailMessage>()
                        .ForMember(d => d.From, o => o.MapFrom(s => $"NAFED HRMS <{s.ToEmail}>"))
                        .ForMember(d => d.To, o => o.UseValue(reporting1Mail + "," + reporting2Mail + "," + reporting3Mail))
                        .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                        .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                        .ForMember(d => d.SmtpClientHost, o => o.MapFrom(s => s.Server))
                        .ForMember(d => d.SmtpPort, o => o.MapFrom(s => s.Port))
                        .ForMember(d => d.enableSSL, o => o.MapFrom(s => s.SSLStatus))
                        .ForMember(d => d.HTMLView, o => o.UseValue(true))
                        .ForMember(d => d.FriendlyName, o => o.UseValue("NAFED"));
                    });

                    emailMessage = Mapper.Map<EmailMessage>(emailsetting);
                    emailMessage.Subject = $"SALARY REPORT INTIMATION MAIL FAILED";
                    mailBody.Clear();
                    mailBody.AppendFormat("<div>Dear Sir/Madam,</div> <br> ");
                    mailBody.AppendFormat($"<div>Salary report intimation for the month of <b>{period.ToString("MMM, yyyy")}</b> has been failed in following branch(s). Kindly check the issue.<br> <br>");
                    foreach (var employee in employeeDetail)
                    {
                        mailBody.AppendFormat($"<div>Branch :<b> {employee.branchName}</b>, Reason :{employee.reason} <br>");
                    }
                    mailBody.AppendFormat($"<div>Regards, </div> <br>");
                    mailBody.AppendFormat($"<div>F & A Team, </div> <br>");
                    mailBody.AppendFormat($"<div>Nafed</div> <br> <br>");
                    emailMessage.Body = mailBody.ToString();
                    EmailHelper.SendEmail(emailMessage);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
