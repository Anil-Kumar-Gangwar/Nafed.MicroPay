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
    public class BankService : BaseService, IBankService
    {
        private readonly IGenericRepository genericRepo;
        public BankService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public bool BankNameExists(string bankName)
        {
            log.Info($"BankService/BankNameExists/{bankName}");
            return genericRepo.Exists<DTOModel.tblMstBank>(x => x.BankName == bankName);

        }
        public bool BankCodeExists(string bankCode)
        {
            log.Info($"BankService/BankNameExists/{bankCode}");
            return genericRepo.Exists<DTOModel.tblMstBank>(x => x.BankCode == bankCode);

        }
        public Bank GetBankbyCode(string bankCode)
        {
            log.Info($"BankService/GetBankbyCode/{bankCode}");
            try
            {
                var bankObj = genericRepo.GetByID<DTOModel.tblMstBank>(bankCode);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblMstBank, Model.Bank>()
                     .ForMember(c => c.BankCode, c => c.MapFrom(m => m.BankCode))
                    .ForMember(c => c.BankName, c => c.MapFrom(m => m.BankName))
                     .ForMember(c => c.Address1, c => c.MapFrom(s => s.Address1))
                    .ForMember(c => c.Address2, c => c.MapFrom(s => s.Address2))
                    .ForMember(c => c.Address3, c => c.MapFrom(s => s.Address3))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblMstBank, Model.Bank>(bankObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<Bank> GetBankList()
        {
            log.Info($"BankService/GetBankList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblMstBank>(x => (bool)!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstBank, Model.Bank>()
                    .ForMember(c => c.BankCode, c => c.MapFrom(s => s.BankCode))
                    .ForMember(c => c.BankName, c => c.MapFrom(s => s.BankName))
                    .ForMember(c => c.Branch, c => c.MapFrom(s => s.Branch))
                    .ForMember(c => c.Address1, c => c.MapFrom(s => s.Address1))
                    .ForMember(c => c.Address2, c => c.MapFrom(s => s.Address2))
                    .ForMember(c => c.Address3, c => c.MapFrom(s => s.Address3))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listBranch = Mapper.Map<List<Model.Bank>>(result);
                return listBranch.OrderBy(x => x.BankName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool InsertBank(Bank createBank)
        {
            log.Info($"BankService/InsertBank");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Bank, DTOModel.tblMstBank>()
                    .ForMember(c => c.BankCode, c => c.MapFrom(m => m.BankCode))
                    .ForMember(c => c.BankName, c => c.MapFrom(m => m.BankName))
                    .ForMember(c => c.Branch, c => c.MapFrom(m => m.Branch))
                    .ForMember(c => c.Address1, c => c.MapFrom(m => m.Address1))
                    .ForMember(c => c.Address1, c => c.MapFrom(m => m.Address1))
                    .ForMember(c => c.Address2, c => c.MapFrom(m => m.Address2))
                    .ForMember(c => c.Address3, c => c.MapFrom(m => m.Address3))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoBank = Mapper.Map<DTOModel.tblMstBank>(createBank);
                genericRepo.Insert<DTOModel.tblMstBank>(dtoBank);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        public bool UpdateBank(Bank bankObj)
        {
            log.Info($"BankService/UpdateBank");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Bank, DTOModel.tblMstBank>()
                    .ForMember(c => c.BankCode, c => c.MapFrom(m => m.BankCode))
                    .ForMember(c => c.BankName, c => c.MapFrom(m => m.BankName))
                    .ForMember(c => c.Branch, c => c.MapFrom(m => m.Branch))
                    .ForMember(c => c.Address1, c => c.MapFrom(m => m.Address1))
                    .ForMember(c => c.Address1, c => c.MapFrom(m => m.Address1))
                    .ForMember(c => c.Address2, c => c.MapFrom(m => m.Address2))
                    .ForMember(c => c.Address3, c => c.MapFrom(m => m.Address3))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(m => m.UpdatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(m => m.UpdatedOn))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoBank = Mapper.Map<DTOModel.tblMstBank>(bankObj);

                genericRepo.Update<DTOModel.tblMstBank>(dtoBank);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }
        public bool Delete(string bankCode)
        {
            log.Info($"BankService/Delete/{bankCode}");
            bool flag = false;
            try
            {
                DTOModel.tblMstBank dtoBank = new DTOModel.tblMstBank();
                dtoBank = genericRepo.GetByID<DTOModel.tblMstBank>(bankCode);
                dtoBank.IsDeleted = true;
                genericRepo.Update<DTOModel.tblMstBank>(dtoBank);
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
