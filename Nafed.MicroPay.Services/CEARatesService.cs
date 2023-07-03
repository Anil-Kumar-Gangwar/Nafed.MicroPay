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
    public class CEARatesService : BaseService, ICEARatesService
    {
        private readonly IGenericRepository genericRepo;
        public CEARatesService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }      
      
        public List<CEARates> GetCEARatesList()
        {
            log.Info($"CEARatesService/GetCEARatesList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblCEARate>(x => (bool)!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblCEARate, Model.CEARates>()
                    .ForMember(c => c.EffectiveDate, c => c.MapFrom(s => s.EffectiveDate))
                    .ForMember(c => c.MaxNoofChildren, c => c.MapFrom(s => s.MaxNoofChildren))
                    .ForMember(c => c.MaxAgeofEachChild, c => c.MapFrom(s => s.MaxAgeofEachChild))
                    .ForMember(c => c.MaxAmtperChild, c => c.MapFrom(s => s.MaxAmtperChild))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listWages = Mapper.Map<List<Model.CEARates>>(result);
                return listWages.OrderBy(x => x.EffectiveDate).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool CEARatesExists(DateTime effectiveDate)
        {
            log.Info($"CEARatesService/CEARatesExists/{effectiveDate}");
            return genericRepo.Exists<DTOModel.tblCEARate>(x => x.EffectiveDate == effectiveDate);
        }

        public bool UpdateCEARates(CEARates updateCEARates)
        {
            log.Info($"CEARatesService/UpdateCEARates");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.CEARates, DTOModel.tblCEARate>()
                  .ForMember(c => c.EffectiveDate, c => c.MapFrom(s => s.EffectiveDate))
                    .ForMember(c => c.MaxNoofChildren, c => c.MapFrom(s => s.MaxNoofChildren))
                    .ForMember(c => c.MaxAgeofEachChild, c => c.MapFrom(s => s.MaxAgeofEachChild))
                    .ForMember(c => c.MaxAmtperChild, c => c.MapFrom(s => s.MaxAmtperChild))
                .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoCEA = Mapper.Map<DTOModel.tblCEARate>(updateCEARates);

                genericRepo.Update<DTOModel.tblCEARate>(dtoCEA);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertCEARates(CEARates createCEARates)
        {
            log.Info($"CEARatesService/InsertCEARates");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.CEARates, DTOModel.tblCEARate>()
                    .ForMember(c => c.EffectiveDate, c => c.MapFrom(s => s.EffectiveDate))
                    .ForMember(c => c.MaxNoofChildren, c => c.MapFrom(s => s.MaxNoofChildren))
                    .ForMember(c => c.MaxAgeofEachChild, c => c.MapFrom(s => s.MaxAgeofEachChild))
                    .ForMember(c => c.MaxAmtperChild, c => c.MapFrom(s => s.MaxAmtperChild))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoCEA = Mapper.Map<DTOModel.tblCEARate>(createCEARates);
                genericRepo.Insert<DTOModel.tblCEARate>(dtoCEA);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public CEARates GetCEARates(DateTime effectiveDate)
        {
            log.Info($"CEARatesService/GetCEARates/{effectiveDate}");
            try
            {
                var CEAObj = genericRepo.Get<DTOModel.tblCEARate>(x => x.EffectiveDate == effectiveDate).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblCEARate, Model.CEARates>()
                   .ForMember(c => c.EffectiveDate, c => c.MapFrom(s => s.EffectiveDate))
                   .ForMember(c => c.MaxNoofChildren, c => c.MapFrom(s => s.MaxNoofChildren))
                   .ForMember(c => c.MaxAgeofEachChild, c => c.MapFrom(s => s.MaxAgeofEachChild))
                   .ForMember(c => c.MaxAmtperChild, c => c.MapFrom(s => s.MaxAmtperChild))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblCEARate, Model.CEARates>(CEAObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(DateTime effectiveDate)
        {
            log.Info($"CEARatesService/Delete/{effectiveDate}");
            bool flag = false;
            try
            {
                DTOModel.tblCEARate dtoCEA = new DTOModel.tblCEARate();
                dtoCEA = genericRepo.Get<DTOModel.tblCEARate>(x => x.EffectiveDate == effectiveDate).FirstOrDefault();
                dtoCEA.IsDeleted = true;
                genericRepo.Update<DTOModel.tblCEARate>(dtoCEA);
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
