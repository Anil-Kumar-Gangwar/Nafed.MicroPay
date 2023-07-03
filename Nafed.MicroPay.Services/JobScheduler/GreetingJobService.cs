using AutoMapper;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Services.JobScheduler
{
    public class GreetingJobService : BaseService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IDashBoardRepository dashRepo;

        public GreetingJobService()
        {
            genericRepo = new GenericRepository();
            dashRepo = new DashBoardRepository();
        }

        public void ExecuteJob()
        {
            try
            {
                log.Info($"GreetingJobService/ExecuteJob : Executed at: {DateTime.Now}");

                var dtoRecords = dashRepo.TodayGreetingNotifications(DateTime.Now.Date.AddDays(-1));

                Mapper.Initialize(cfg =>
                      cfg.CreateMap<DTOModel.GetGreetingNotification_Result, Model.EmployeeDobDoj>()
                      .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                      .ForMember(d => d.EventType, o => o.MapFrom(s => s.EventType))
                  );

                var gRecords = Mapper.Map<List<Model.EmployeeDobDoj>>(dtoRecords);

                if (gRecords?.Count() > 0)
                {
                    var bdayGreetings = gRecords.Where(x => x.EventType.Equals("dob", StringComparison.OrdinalIgnoreCase));
                    var wrkAnnGreetings = gRecords.Where(x => x.EventType.Equals("doj", StringComparison.OrdinalIgnoreCase));

                    #region ==== Get Email & SMS Configuration Setting ============

                    EmailMessage emailMessage = new EmailMessage();
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

                    //========= End ==========================================


                    SMSParameter smsParameter = new SMSParameter();
                    var smssettings = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.SMSConfiguration, SMSParameter>()
                        .ForMember(d => d.URL, o => o.MapFrom(s => s.SMSUrl))
                        .ForMember(d => d.User, o => o.MapFrom(s => s.UserName))
                        .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                        ;
                    });
                    smsParameter = Mapper.Map<SMSParameter>(smssettings);

                    #endregion

                    if (bdayGreetings?.Count() > 0)
                    {
                        //  Task bdEmail_Task1 = Task.Run(() => 
                        BirthDayGreeting(bdayGreetings, emailMessage, smsParameter);
                    }
                    if (wrkAnnGreetings?.Count() > 0)
                    {
                        // Task WkEmail_Task2 = Task.Run(() => 
                        WrkAnniveryGreeting(wrkAnnGreetings, emailMessage, smsParameter);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-GreetingJobService/ExecuteJob-" + ex.InnerException + ", StackTrace-" + ex.StackTrace + ", DateTimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private void BirthDayGreeting(IEnumerable<Model.EmployeeDobDoj> rows, EmailMessage emailMessage, SMSParameter smsParameter)
        {
            log.Info($"GreetingJobService/BirthDayGreeting /");

            try
            {

             //   Parallel.ForEach(x=>x,)

                foreach (var item in rows)
                {
                    StringBuilder bodyText = new StringBuilder("");
                    if (!String.IsNullOrEmpty(item.OfficialEmail))
                    {
                        bodyText.AppendFormat($"<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                     + "<p>We feel proud to call you our employee. The world is your oyster, and the opportunities are endless. Here's to a great year filled with happiness, success and good health. Wishing you a happy birthday! </p>");

                        bodyText.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");

                        emailMessage.To = item.OfficialEmail;
                        emailMessage.Body = bodyText.ToString();
                        emailMessage.Subject = "Happy Birthday !";
                     //   Task bdEmail_Task1 = Task.Run(() => EmailHelper.SendEmail(emailMessage));

                        EmailHelper.SendEmail(emailMessage);
                    }
                    else if (!String.IsNullOrEmpty(item.MobileNo))
                    {
                        bodyText.AppendFormat("Dear Sir/Madam,"
                      + "We feel proud to call you our employee. The world is your oyster, and the opportunities are endless. Here's to a great year filled with happiness, success and good health. Wishing you a happy birthday!.");
                        bodyText.AppendFormat("Regards, ENAFED");

                        smsParameter.Message = bodyText.ToString();
                        smsParameter.MobileNo = item.MobileNo.Length == 10 ? "91" + item.MobileNo : item.MobileNo;
                      
                        //  Task bdSMS_Task1 = Task.Run(() => SMSHelper.SendSMS(smsParameter));

                        SMSHelper.SendSMS(smsParameter);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void WrkAnniveryGreeting(IEnumerable<Model.EmployeeDobDoj> rows, EmailMessage emailMessage, SMSParameter smsParameter)
        {
            log.Info($"GreetingJobService/WrkAnniveryGreeting/");
            try
            {
                foreach (var item in rows)
                {
                    StringBuilder bodyText = new StringBuilder("");
                    if (!String.IsNullOrEmpty(item.OfficialEmail))
                    {
                        bodyText.AppendFormat($"<div style='font-family:Tahoma;font-size:9pt;'> "
                       + "<p>Dear Sir/Madam,</p>"
                     + "<p>Your positive attitude towards work inspires everyone here to give their best. "
                     + "It’s your work anniversary today and we are thankful for having you with us. Happy Work Anniversary !</p>"
                     );

                        bodyText.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");

                        emailMessage.To = item.OfficialEmail;
                        emailMessage.Body = bodyText.ToString();
                        emailMessage.Subject = "Happy Work Anniversary !";
                        // Task Wk_t1 = Task.Run(() => EmailHelper.SendEmail(emailMessage));
                        EmailHelper.SendEmail(emailMessage);
                    }
                    else if (!String.IsNullOrEmpty(item.MobileNo))
                    {
                        bodyText.AppendFormat("Dear Sir/Madam,"
                      + "Your positive attitude towards work inspires everyone here to give their best. It’s your work anniversary today and we are thankful for having you with us. Happy Work Anniversary !");
                        bodyText.AppendFormat("Regards, ENAFED");

                        smsParameter.Message = bodyText.ToString();
                        smsParameter.MobileNo = item.MobileNo.Length == 10 ? "91" + item.MobileNo : item.MobileNo;
                        // Task Wk_t2 = Task.Run(() => SMSHelper.SendSMS(smsParameter));

                        SMSHelper.SendSMS(smsParameter);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
