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

namespace Nafed.MicroPay.Services
{
    public class PayrollApprovalSettingService : BaseService, IPayrollApprovalSettingService, IPayrollApprovalEmail
    {
        private readonly IGenericRepository genericRepo;
        private readonly ISalaryRepository salaryRepo;
        private readonly IArrearRepository arrearRepo;
        public PayrollApprovalSettingService(IGenericRepository genericRepo,
            ISalaryRepository salaryRepo, IArrearRepository arrearRepo)
        {
            this.genericRepo = genericRepo;
            this.salaryRepo = salaryRepo;
            this.arrearRepo = arrearRepo;
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
                                           Reporting2 = sub == null ? 0 : sub.Reporting2.HasValue? sub.Reporting2.Value:0,
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

        public bool SubmitApprovalRequest(PayrollApprovalRequest request)
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


    }
}
