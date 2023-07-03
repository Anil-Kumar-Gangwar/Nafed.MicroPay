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
    public class BankRatesService : BaseService, IBankRatesService
    {
        private readonly IGenericRepository genericRepo;
        public BankRatesService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public BankRates GetBankRatesbyDate(DateTime bankdate)
        {
            log.Info($"BankRatesService/GetBankRatesbyDate/{bankdate}");
            try
            {
                var bankRateObj = genericRepo.GetByID<DTOModel.tblmstbankrate>(bankdate);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblmstbankrate, Model.BankRates>()
                     .ForMember(c => c.EffectiveDate, c => c.MapFrom(m => m.EffectiveDate))
                    .ForMember(c => c.BankLoanRate, c => c.MapFrom(m => m.BankLoanRate))
                     .ForMember(c => c.BankAccuralRate, c => c.MapFrom(m => m.BankAccuralRate))
                    .ForMember(c => c.PFLoanRate, c => c.MapFrom(m => m.PFLoanRate))
                     .ForMember(c => c.PFLoanAccuralRate, c => c.MapFrom(m => m.PFLoanAccuralRate))
                    .ForMember(c => c.TDSRebetRate, c => c.MapFrom(m => m.TDSRebetRate))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                 .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblmstbankrate, Model.BankRates>(bankRateObj);
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<BankRates> GetBankRatesList()
        {
            log.Info($"BankRatesService/GetBankRatesList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblmstbankrate>(em=> !em.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblmstbankrate, Model.BankRates>()
                     .ForMember(c => c.EffectiveDate, c => c.MapFrom(m => m.EffectiveDate))
                    .ForMember(c => c.BankLoanRate, c => c.MapFrom(m => m.BankLoanRate))
                     .ForMember(c => c.BankAccuralRate, c => c.MapFrom(m => m.BankAccuralRate))
                    .ForMember(c => c.PFLoanRate, c => c.MapFrom(m => m.PFLoanRate))
                     .ForMember(c => c.PFLoanAccuralRate, c => c.MapFrom(m => m.PFLoanAccuralRate))
                    .ForMember(c => c.TDSRebetRate, c => c.MapFrom(m => m.TDSRebetRate))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listBranch = Mapper.Map<List<Model.BankRates>>(result);
                return listBranch.OrderBy(x => x.EffectiveDate).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool InsertBankRates(BankRates createBankRates)
        {
            log.Info($"BankRatesService/InsertBank");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                { 
                    cfg.CreateMap<Model.BankRates, DTOModel.tblmstbankrate>()
                     .ForMember(c => c.EffectiveDate, c => c.MapFrom(m => m.EffectiveDate))
                    .ForMember(c => c.BankLoanRate, c => c.MapFrom(m => m.BankLoanRate))
                     .ForMember(c => c.BankAccuralRate, c => c.MapFrom(m => m.BankAccuralRate))
                    .ForMember(c => c.PFLoanRate, c => c.MapFrom(m => m.PFLoanRate))
                     .ForMember(c => c.PFLoanAccuralRate, c => c.MapFrom(m => m.PFLoanAccuralRate))
                    .ForMember(c => c.TDSRebetRate, c => c.MapFrom(m => m.TDSRebetRate))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoBankRates = Mapper.Map<DTOModel.tblmstbankrate>(createBankRates);
                genericRepo.Insert<DTOModel.tblmstbankrate>(dtoBankRates);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        public bool UpdateBankRates(BankRates editBankRates)
        {
            log.Info($"BankRatesService/UpdateBank");
            bool flag = false;
            try
            {
              //  var dtoObj = genericRepo.GetByID<DTOModel.tblmstbankrate>(editBankRates.EffectiveDate);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap< Model.BankRates, DTOModel.tblmstbankrate>()
                     .ForMember(c => c.EffectiveDate, c => c.MapFrom(m => m.EffectiveDate))
                    .ForMember(c => c.BankLoanRate, c => c.MapFrom(m => m.BankLoanRate))
                     .ForMember(c => c.BankAccuralRate, c => c.MapFrom(m => m.BankAccuralRate))
                    .ForMember(c => c.PFLoanRate, c => c.MapFrom(m => m.PFLoanRate))
                     .ForMember(c => c.PFLoanAccuralRate, c => c.MapFrom(m => m.PFLoanAccuralRate))
                    .ForMember(c => c.TDSRebetRate, c => c.MapFrom(m => m.TDSRebetRate))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(m => m.UpdatedOn))
                   .ForMember(c => c.UpdatedBy, c => c.MapFrom(m => m.UpdatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoBankRates = Mapper.Map<DTOModel.tblmstbankrate>(editBankRates);
                genericRepo.Update<DTOModel.tblmstbankrate>(dtoBankRates);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }
        public bool BankDateExist(Nullable<System.DateTime> bankdate)
        {
            try
            {
                log.Info($"BankRatesService/BankRatesExist/{bankdate}");
                return genericRepo.Exists<DTOModel.tblmstbankrate>(x => x.EffectiveDate == bankdate);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool Delete(DateTime bankdate)
        {
            log.Info($"BankRatesService/Delete/{bankdate}");
            bool flag = false;
            try
            {
                DTOModel.tblmstbankrate dtoBankRates = new DTOModel.tblmstbankrate();
                dtoBankRates = genericRepo.GetByID<DTOModel.tblmstbankrate>(bankdate);
                dtoBankRates.IsDeleted = true;
                genericRepo.Update<DTOModel.tblmstbankrate>(dtoBankRates);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public BankRates GeLatestBankRate()
        {
            log.Info($"BankRatesService/GeLatestBankRate");
            try
            {
                BankRates bankRate = new BankRates();

                var dtoBankRate = genericRepo.Get<DTOModel.tblmstbankrate>(x => !x.IsDeleted).OrderByDescending(y => y.EffectiveDate).FirstOrDefault();
                Mapper.Initialize(cfg => {
                    cfg.CreateMap<DTOModel.tblmstbankrate, BankRates>();
                });
                bankRate = Mapper.Map<BankRates>(dtoBankRate);
                return bankRate;
            }
            catch (Exception)
            {


                throw;
            }
        }
    }
}
