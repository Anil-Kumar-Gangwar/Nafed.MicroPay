using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public class SMSConfigurationService : BaseService, ISMSConfigurationService
    {
        private readonly IGenericRepository genericRepo;

        public SMSConfigurationService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public bool CreateSMSConfiguration(Model.SMSConfiguration smsConfig)
        {
            bool flag = false;
            log.Info($"SMSConfigurationService/CreateSMSConfiguration");

            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SMSConfiguration, DTOModel.SMSConfiguration>()
                     .ForMember(d => d.ApiKey, o => o.MapFrom(s => s.ApiKey))
                        .ForMember(d => d.Channel, o => o.MapFrom(s => s.Channel))
                        .ForMember(d => d.SMSUrl, o => o.MapFrom(s => s.SMSUrl))
                        .ForMember(d => d.SenderID, o => o.MapFrom(s => s.SenderID))
                        .ForMember(d => d.DCS, o => o.MapFrom(s => s.DCS))
                        .ForMember(d => d.FlashSMS, o => o.MapFrom(s => s.FlashSMS))
                        .ForMember(d => d.Route, o => o.MapFrom(s => s.Route))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                    .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                    .ForAllOtherMembers(d => d.Ignore());
                });

                var dtoSMSSetting = Mapper.Map<Data.Models.SMSConfiguration>(smsConfig);
                genericRepo.Insert<Data.Models.SMSConfiguration>(dtoSMSSetting);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
            return flag;
        }

        public List<Model.SMSConfiguration> GetSMSConfiguration()
        {
            log.Info($"SMSConfigurationService/GetSMSConfiguration");
            try
            {
                var result = genericRepo.Get<DTOModel.SMSConfiguration>(x=>!x.IsDeleted);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SMSConfiguration, Model.SMSConfiguration>()
                    .ForMember(d => d.SMSConfigurationID, o => o.MapFrom(s => s.SMSConfigurationID))
                    .ForMember(d => d.ApiKey, o => o.MapFrom(s => s.ApiKey))
                    .ForMember(d => d.Channel, o => o.MapFrom(s => s.Channel))
                    .ForMember(d => d.SMSUrl, o => o.MapFrom(s => s.SMSUrl))
                    .ForMember(d => d.SenderID, o => o.MapFrom(s => s.SenderID))
                    .ForMember(d => d.DCS, o => o.MapFrom(s => s.DCS))
                    .ForMember(d => d.FlashSMS, o => o.MapFrom(s => s.FlashSMS))
                    .ForMember(d => d.Route, o => o.MapFrom(s => s.Route))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))

                    .ForAllOtherMembers(d => d.Ignore())
                    ;
                });
                return Mapper.Map<List<Model.SMSConfiguration>>(result);

            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
        }

        public bool UpdateSMSConfiguration(Model.SMSConfiguration smsConfig)
        {
            log.Info($"SMSConfigurationService/UpdateSMSConfiguration");
            bool flag = false;
          
            try
            {

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SMSConfiguration, DTOModel.SMSConfiguration>()
                     .ForMember(d => d.SMSConfigurationID, o => o.MapFrom(s => s.SMSConfigurationID))
                     .ForMember(d => d.ApiKey, o => o.MapFrom(s => s.ApiKey))
                        .ForMember(d => d.Channel, o => o.MapFrom(s => s.Channel))
                        .ForMember(d => d.SMSUrl, o => o.MapFrom(s => s.SMSUrl))
                        .ForMember(d => d.SenderID, o => o.MapFrom(s => s.SenderID))
                        .ForMember(d => d.DCS, o => o.MapFrom(s => s.DCS))
                        .ForMember(d => d.FlashSMS, o => o.MapFrom(s => s.FlashSMS))
                        .ForMember(d => d.Route, o => o.MapFrom(s => s.Route))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                    .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                    .ForAllOtherMembers(d => d.Ignore());
                });

                var dtoEmailSetting = Mapper.Map<Data.Models.SMSConfiguration>(smsConfig);
                genericRepo.Update<Data.Models.SMSConfiguration>(dtoEmailSetting);
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
