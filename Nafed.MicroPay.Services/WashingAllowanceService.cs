using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public class WashingAllowanceService : BaseService, IWashingAllowanceService
    {
        private readonly IGenericRepository genericRepo;
        public WashingAllowanceService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }       

        public List<WashingAllowance> GetWashingAllowanceList()
        {
            log.Info($"WashingAllowanceService/GetWashingAllowanceList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblWashingAllowance>(x => (bool)!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblWashingAllowance, Model.WashingAllowance>()
                    .ForMember(c => c.EffectiveDate, c => c.MapFrom(s => s.EffectiveDate))
                    .ForMember(c => c.Rate, c => c.MapFrom(s => s.Rate))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listWashingAllowance = Mapper.Map<List<Model.WashingAllowance>>(result);
                return listWashingAllowance.OrderBy(x => x.EffectiveDate).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool WashingAllowanceExists(DateTime effectiveDate, decimal rate)
        {
            log.Info($"WashingAllowanceService/WashingAllowanceExists/{effectiveDate}/{rate}");
            return genericRepo.Exists<DTOModel.tblWashingAllowance>(x => x.EffectiveDate == effectiveDate);
        }

        public bool UpdateWashingAllowance(WashingAllowance updateWashingAllowance)
        {
            log.Info($"WashingAllowanceService/UpdateWashingAllowance");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.WashingAllowance, DTOModel.tblWashingAllowance>()
                   .ForMember(c => c.EffectiveDate, c => c.MapFrom(s => s.EffectiveDate))
                .ForMember(c => c.Rate, c => c.MapFrom(s => s.Rate))
                .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoWashingAllowance = Mapper.Map<DTOModel.tblWashingAllowance>(updateWashingAllowance);

                genericRepo.Update<DTOModel.tblWashingAllowance>(dtoWashingAllowance);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertWashingAllowance(WashingAllowance createWashingAllowance)
        {
            log.Info($"WashingAllowanceService/InsertWashingAllowance");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.WashingAllowance, DTOModel.tblWashingAllowance>()
                   .ForMember(c => c.EffectiveDate, c => c.MapFrom(s => s.EffectiveDate))
                .ForMember(c => c.Rate, c => c.MapFrom(s => s.Rate))
                .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoWashingAllowance = Mapper.Map<DTOModel.tblWashingAllowance>(createWashingAllowance);
                genericRepo.Insert<DTOModel.tblWashingAllowance>(dtoWashingAllowance);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public WashingAllowance GetWashingAllowance(DateTime effectiveDate, decimal rate)
        {
            log.Info($"WashingAllowanceService/GetWashingAllowance/{effectiveDate}/{rate}");
            try
            {
                var WagesObj = genericRepo.Get<DTOModel.tblWashingAllowance>(x => x.EffectiveDate == effectiveDate && x.Rate == rate).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblWashingAllowance, Model.WashingAllowance>()
                   .ForMember(c => c.EffectiveDate, c => c.MapFrom(s => s.EffectiveDate))
                   .ForMember(c => c.Rate, c => c.MapFrom(s => s.Rate))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                  .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblWashingAllowance, Model.WashingAllowance>(WagesObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(DateTime effectiveDate, decimal rate)
        {
            log.Info($"WashingAllowanceService/Delete/{effectiveDate}/{rate}");
            bool flag = false;
            try
            {
                DTOModel.tblWashingAllowance dtoWashingAllowance = new DTOModel.tblWashingAllowance();
                dtoWashingAllowance = genericRepo.Get<DTOModel.tblWashingAllowance>(x => x.EffectiveDate == effectiveDate && x.Rate == rate).FirstOrDefault();
                dtoWashingAllowance.IsDeleted = true;
                genericRepo.Update<DTOModel.tblWashingAllowance>(dtoWashingAllowance);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public WashingAllowance GetLatestWashingAllowance()
        {
            log.Info($"WashingAllowanceService/GetLatestWashingAllowance");

            WashingAllowance latestWashAllowance = new WashingAllowance();
            try
            {
                var latestRate = genericRepo.Get<DTOModel.tblWashingAllowance>(x => !x.IsDeleted).OrderByDescending(y => y.EffectiveDate).FirstOrDefault();

                Mapper.Initialize(cfg => {

                    cfg.CreateMap<DTOModel.tblWashingAllowance, WashingAllowance>();
                });
                latestWashAllowance = Mapper.Map<WashingAllowance>(latestRate);
                return latestWashAllowance;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
