using System;
using System.Collections.Generic;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;


namespace Nafed.MicroPay.Services
{
    public class EmailConfigurationService : BaseService,IEmailConfigurationService
    {
        private readonly IGenericRepository genericRepo;

        public EmailConfigurationService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public List<EmailConfiguration> GetEmailConfigList()
        {
            log.Info($"EmailConfigurationService/GetEmailConfigList");

            try
            {
                var result = genericRepo.Get<DTOModel.EmailConfiguration>(x=>!x.IsDeleted);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmailConfiguration, Model.EmailConfiguration>()
                    .ForMember(d=>d.EmailConfigurationID,o=>o.MapFrom(s=>s.EmailConfigurationID))
                    .ForMember(d=>d.SSLStatus,o=>o.MapFrom(s=>s.SSLStatus))
                    .ForMember(d => d.ToEmail, o => o.MapFrom(s => s.ToEmail))
                    .ForMember(d => d.Bcc, o => o.MapFrom(s => s.Bcc))
                    .ForMember(d => d.CcEmail, o => o.MapFrom(s => s.CcEmail))
                    .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                    .ForMember(d => d.Port, o => o.MapFrom(s => s.Port))
                    .ForMember(d => d.Server, o => o.MapFrom(s => s.Server))
                    .ForMember(d=>d.Signature, o=>o.MapFrom(s=>s.Signature))
                    .ForMember(d=>d.CreatedBy,o=>o.MapFrom(s=>s.CreatedBy))
                    .ForMember(d =>d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.IsMaintenance, o => o.MapFrom(s => s.IsMaintenance))
                    .ForMember(d => d.MaintenanceDateTime, o => o.MapFrom(s => s.MaintenanceDateTime))
                    .ForAllOtherMembers(d=>d.Ignore())
                    ;
                });
                 return Mapper.Map<List<Model.EmailConfiguration>>(result);
               
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
        }

        public bool CreateEmailConfiguration(EmailConfiguration emailConfig)
        {
            bool flag = false;
            log.Info($"EmailConfigurationService/CreateEmailConfiguration");
            try
            {
           

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmailConfiguration, DTOModel.EmailConfiguration>()
                   // .ForMember(d => d.EmailConfigurationID, o => o.MapFrom(s => s.EmailConfigurationID))
                    .ForMember(d => d.SSLStatus, o => o.MapFrom(s => s.IsSSLEnable))
                    .ForMember(d => d.ToEmail, o => o.MapFrom(s => s.ToEmail))
                    .ForMember(d => d.UserName , o => o.MapFrom(s => s.UserName))
                    .ForMember(d => d.Bcc, o => o.MapFrom(s => s.Bcc))
                    .ForMember(d => d.CcEmail, o => o.MapFrom(s => s.CcEmail))
                    .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                    .ForMember(d => d.Port, o => o.MapFrom(s => s.Port))
                    .ForMember(d => d.Server, o => o.MapFrom(s => s.Server))
                    .ForMember(d => d.Signature, o => o.MapFrom(s => s.Signature))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                    .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                    .ForMember(d => d.IsMaintenance, o => o.MapFrom(s => s.IsMaintenance))
                    .ForMember(d => d.MaintenanceDateTime, o => o.MapFrom(s => s.MaintenanceDateTime))
                    .ForAllOtherMembers(d => d.Ignore())
                    ;
                });

                var dtoEmailSetting = Mapper.Map<Data.Models.EmailConfiguration>(emailConfig);
                 genericRepo.Insert<Data.Models.EmailConfiguration>(dtoEmailSetting);               
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
            return flag;
        }

       
        public bool UpdateEmailConfiguration(EmailConfiguration emailConfig)
        {
            bool flag = false;
            log.Info($"EmailConfigurationService/UpdateEmailConfiguration");
            try
            {
               
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmailConfiguration, DTOModel.EmailConfiguration>()
                    .ForMember(d => d.EmailConfigurationID, o => o.MapFrom(s => s.EmailConfigurationID))
                      .ForMember(d => d.SSLStatus, o => o.MapFrom(s => s.IsSSLEnable))
                      .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                    .ForMember(d => d.ToEmail, o => o.MapFrom(s => s.ToEmail))
                    .ForMember(d => d.Bcc, o => o.MapFrom(s => s.Bcc))
                    .ForMember(d => d.CcEmail, o => o.MapFrom(s => s.CcEmail))
                    .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                    .ForMember(d => d.Port, o => o.MapFrom(s => s.Port))
                    .ForMember(d => d.Server, o => o.MapFrom(s => s.Server))
                    .ForMember(d => d.Signature, o => o.MapFrom(s => s.Signature))
                   .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                    .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                    .ForMember(d => d.IsMaintenance, o => o.MapFrom(s => s.IsMaintenance))
                    .ForMember(d => d.MaintenanceDateTime, o => o.MapFrom(s => s.MaintenanceDateTime))
                    .ForAllOtherMembers(d => d.Ignore())
                    ;
                });

                var dtoEmailSetting = Mapper.Map<Data.Models.EmailConfiguration>(emailConfig);
                genericRepo.Update<Data.Models.EmailConfiguration>(dtoEmailSetting);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
            return flag;
        }
    }
}
