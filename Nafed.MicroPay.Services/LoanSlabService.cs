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
    public class LoanSlabService : BaseService, ILoanSlab
    {
        private readonly IGenericRepository genericRepo;
        public LoanSlabService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public bool LoabSlabExists(int? slabNo)
        {
            log.Info($"LoanSlabService/LoabSlabExists/{slabNo}");
            return genericRepo.Exists<DTOModel.tblmstslab>(x => x.SlabNo == slabNo && (bool)!x.IsDeleted);

        }
        public LoanSlab GetLoabSlabbyNo(int? slabNo)
        {
            log.Info($"LoanSlabService/GetLoabSlabbyNo/{slabNo}");
            try
            {
                var bankObj = genericRepo.Get<DTOModel.tblmstslab>(x=>x.SlabNo== slabNo).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblmstslab, Model.LoanSlab>()
                     .ForMember(c => c.EffectiveDate, c => c.MapFrom(m => m.EffectiveDate))
                    .ForMember(c => c.SlabNo, c => c.MapFrom(m => m.SlabNo))
                 .ForMember(c => c.AmountOfSlab, c => c.MapFrom(m => m.AmountOfSlab))
                    .ForMember(c => c.RateOfInterest, c => c.MapFrom(m => m.RateOfInterest))
                      .ForMember(c => c.LoanTypeId, c => c.MapFrom(m => m.LoanTypeId))
                    .ForMember(c => c.LoanDesc, c => c.MapFrom(m => m.LoanDesc))
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblmstslab, Model.LoanSlab>(bankObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<LoanSlab> GetLoabSlabList()
        {
            log.Info($"LoanSlabService/GetLoabSlabList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblmstslab>(x => (bool)!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblmstslab, Model.LoanSlab>()
                    .ForMember(c => c.EffectiveDate, c => c.MapFrom(m => m.EffectiveDate))
                    .ForMember(c => c.SlabNo, c => c.MapFrom(m => m.SlabNo))
                    .ForMember(c => c.AmountOfSlab, c => c.MapFrom(m => m.AmountOfSlab))
                    .ForMember(c => c.RateOfInterest, c => c.MapFrom(m => m.RateOfInterest))
                    .ForMember(c => c.LoanType, c => c.MapFrom(m => m.LoanType))
                   .ForMember(c => c.LoanTypeId, c => c.MapFrom(m => m.LoanTypeId))
                    .ForMember(c => c.LoanDesc, c => c.MapFrom(m => m.tblMstLoanType.LoanDesc))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var listLoanSlab = Mapper.Map<List<Model.LoanSlab>>(result);
                return listLoanSlab.OrderBy(x => x.SlabNo).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool InsertLoabSlab(LoanSlab createloanSlab)
        {
            log.Info($"LoanSlabService/InsertLoabSlab");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LoanSlab, DTOModel.tblmstslab>()
                   .ForMember(c => c.EffectiveDate, c => c.MapFrom(m => m.EffectiveDate))
                   .ForMember(c => c.SlabNo, c => c.MapFrom(m => m.SlabNo))
                   .ForMember(c => c.AmountOfSlab, c => c.MapFrom(m => m.AmountOfSlab))
                   .ForMember(c => c.RateOfInterest, c => c.MapFrom(m => m.RateOfInterest))                   
                   .ForMember(c => c.LoanDesc, c => c.MapFrom(m => m.LoanDesc))
                   .ForMember(c => c.LoanTypeId, c => c.MapFrom(m => m.LoanTypeId))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                   .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(m => m.UpdatedBy))
                   .ForMember(c => c.UpdatedOn, c => c.MapFrom(m => m.UpdatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoLoanSlab = Mapper.Map<DTOModel.tblmstslab>(createloanSlab);
                genericRepo.Insert<DTOModel.tblmstslab>(dtoLoanSlab);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        public bool UpdateLoabSlab(LoanSlab editloanSlab)
        {
            log.Info($"LoanSlabService/UpdateLoabSlab");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LoanSlab, DTOModel.tblmstslab>()
                   .ForMember(c => c.EffectiveDate, c => c.MapFrom(m => m.EffectiveDate))
                   .ForMember(c => c.SlabNo, c => c.MapFrom(m => m.SlabNo))
                   .ForMember(c => c.AmountOfSlab, c => c.MapFrom(m => m.AmountOfSlab))
                   .ForMember(c => c.RateOfInterest, c => c.MapFrom(m => m.RateOfInterest))
                  .ForMember(c => c.LoanTypeId, c => c.MapFrom(m => m.LoanTypeId))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(m => m.UpdatedOn))
                   .ForMember(c => c.UpdatedBy, c => c.MapFrom(m => m.UpdatedBy))
                   .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoLoanSlab = Mapper.Map<DTOModel.tblmstslab>(editloanSlab);
                genericRepo.Update<DTOModel.tblmstslab>(dtoLoanSlab);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }
        public bool Delete(int? slabNo,string loanType,DateTime effectiveDate)
        {
            log.Info($"LoanSlabService/Delete/{slabNo}");
            bool flag = false;
            try
            {
                DTOModel.tblmstslab dtoLoanSlab = new DTOModel.tblmstslab();
                dtoLoanSlab = genericRepo.Get<DTOModel.tblmstslab>(x=>x.SlabNo==slabNo && x.LoanType== loanType).FirstOrDefault();
                dtoLoanSlab.IsDeleted = true;
                genericRepo.Update<DTOModel.tblmstslab>(dtoLoanSlab);
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
