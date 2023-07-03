using AutoMapper;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Services.TDS
{
    public class TDSTaxRuleSlabService : BaseService, ITDSTaxRuleSlabService
    {
        private readonly IGenericRepository genericRepo;
        public TDSTaxRuleSlabService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public IEnumerable<tblTDSTaxRulesSlab> GetTaxRuleSlabs(string fYear)
        {
            log.Info($"TDSTaxRuleSlabService/GetTaxRuleSlabs/fYear={fYear}");

            IEnumerable<tblTDSTaxRulesSlab> taxRuleSlabs = Enumerable.Empty<tblTDSTaxRulesSlab>();

            try
            {
                var dtoTaxRuleSlabs = genericRepo.Get<DTOModel.tblTDSTaxRulesSlab>(x => x.FinancialYear == fYear);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblTDSTaxRulesSlab, tblTDSTaxRulesSlab>();
                });
                taxRuleSlabs = Mapper.Map<IEnumerable<tblTDSTaxRulesSlab>>(dtoTaxRuleSlabs);
                return taxRuleSlabs;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SubmitTaxRuleSlabs(tblTDSTaxRulesSlab tdsSlab)
        {
            log.Info($"TDSTaxRuleSlabService/SubmitTaxRuleSlabs/fYear={tdsSlab.FinancialYear}");

            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<tblTDSTaxRulesSlab, DTOModel.tblTDSTaxRulesSlab>();
                });
                var dtoTDSSlabs = Mapper.Map<DTOModel.tblTDSTaxRulesSlab>(tdsSlab);

                if (!tdsSlab.UpdatedBy.HasValue)
                    genericRepo.Insert<DTOModel.tblTDSTaxRulesSlab>(dtoTDSSlabs);
                else
                    genericRepo.Update<DTOModel.tblTDSTaxRulesSlab>(dtoTDSSlabs);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool IsTaxRuleSlabExists(string fYear)
        {
            log.Info($"TDSTaxRuleSlabService/IsTaxRuleSlabExists/fYear:{fYear}");
            return genericRepo.Exists<DTOModel.tblTDSTaxRulesSlab>(x => x.FinancialYear == fYear);
        }
    }
}
