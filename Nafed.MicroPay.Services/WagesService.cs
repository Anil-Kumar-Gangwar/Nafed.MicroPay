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
    public class WagesService : BaseService, IWagesService
    {
        private readonly IGenericRepository genericRepo;
        public WagesService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }       

        public List<Wages> GetWagesList()
        {
            log.Info($"WagesService/GetWagesList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblMstDailyWageRate>(x => (bool)!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstDailyWageRate, Model.Wages>()
                    .ForMember(c => c.RDate, c => c.MapFrom(s => s.RDate))
                    .ForMember(c => c.RatePerHour, c => c.MapFrom(s => s.RatePerHour))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listWages = Mapper.Map<List<Model.Wages>>(result);
                return listWages.OrderBy(x => x.RDate).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool WagesExists(DateTime effectiveDate, decimal ratePerHour)
        {
            log.Info($"WagesService/WagesExists/{effectiveDate}/{ratePerHour}");
            return genericRepo.Exists<DTOModel.tblMstDailyWageRate>(x => x.RDate == effectiveDate);
        }

        public bool UpdateWages(Wages updateWages)
        {
            log.Info($"WagesService/UpdateWages");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Wages, DTOModel.tblMstDailyWageRate>()
                   .ForMember(c => c.RDate, c => c.MapFrom(s => s.RDate))
                .ForMember(c => c.RatePerHour, c => c.MapFrom(s => s.RatePerHour))
                .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoWages = Mapper.Map<DTOModel.tblMstDailyWageRate>(updateWages);

                genericRepo.Update<DTOModel.tblMstDailyWageRate>(dtoWages);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertWages(Wages createWages)
        {
            log.Info($"WagesService/InsertBank");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Wages, DTOModel.tblMstDailyWageRate>()
                   .ForMember(c => c.RDate, c => c.MapFrom(s => s.RDate))
                .ForMember(c => c.RatePerHour, c => c.MapFrom(s => s.RatePerHour))
                .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoWages = Mapper.Map<DTOModel.tblMstDailyWageRate>(createWages);
                genericRepo.Insert<DTOModel.tblMstDailyWageRate>(dtoWages);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Wages GetWages(DateTime effectiveDate, decimal ratePerHour)
        {
            log.Info($"WagesService/GetWages/{effectiveDate}/{ratePerHour}");
            try
            {
                var WagesObj = genericRepo.Get<DTOModel.tblMstDailyWageRate>(x => x.RDate == effectiveDate && x.RatePerHour == ratePerHour).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstDailyWageRate, Model.Wages>()
                   .ForMember(c => c.RDate, c => c.MapFrom(s => s.RDate))
                   .ForMember(c => c.RatePerHour, c => c.MapFrom(s => s.RatePerHour))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                  .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblMstDailyWageRate, Model.Wages>(WagesObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(DateTime effectiveDate, decimal ratePerHour)
        {
            log.Info($"WagesService/Delete/{effectiveDate}/{ratePerHour}");
            bool flag = false;
            try
            {
                DTOModel.tblMstDailyWageRate dtoWages = new DTOModel.tblMstDailyWageRate();
                dtoWages = genericRepo.Get<DTOModel.tblMstDailyWageRate>(x => x.RDate == effectiveDate && x.RatePerHour == ratePerHour).FirstOrDefault();
                dtoWages.IsDeleted = true;
                genericRepo.Update<DTOModel.tblMstDailyWageRate>(dtoWages);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
    }
}
