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
    public class LoanTypeService : BaseService, ILoanTypeService
    {
        private readonly IGenericRepository genericRepo;
        public LoanTypeService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public bool LoanTypeExists(string loanType)
        {
            log.Info($"LoanTypeService/LoanTypeExists/{loanType}");
            return genericRepo.Exists<DTOModel.tblMstLoanType>(x => x.LoanType.ToLower()== loanType.ToLower());

        }

        public List<Model.tblMstLoanType> GetLoanTypeList()
        {
            log.Info($"LoanTypeService/GetLoanTypeList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblMstLoanType>(x=>!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstLoanType, Model.tblMstLoanType>();
                });
                var listLoanType = Mapper.Map<List<Model.tblMstLoanType>>(result);
                return listLoanType.OrderBy(x => x.LoanType).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertLoanType(tblMstLoanType createLoanType)
        {
            log.Info($"LoanTypeService/InsertLoabType");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.tblMstLoanType, DTOModel.tblMstLoanType>();
                });
                var dtoLoanType = Mapper.Map<DTOModel.tblMstLoanType>(createLoanType);
                genericRepo.Insert<DTOModel.tblMstLoanType>(dtoLoanType);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public tblMstLoanType GetLoanTypeDtls(int loanTypeID)
        {
            log.Info($"LoanTypeService/{loanTypeID}");
            try
            {
                var dtoLoanType = genericRepo.Get<DTOModel.tblMstLoanType>(x => x.LoanTypeId == loanTypeID).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblMstLoanType, Model.tblMstLoanType>();
                
                });
                var obj = Mapper.Map<DTOModel.tblMstLoanType, Model.tblMstLoanType>(dtoLoanType);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateLoanType(tblMstLoanType editLoanType)
        {
            log.Info($"LoanTypeService/UpdateLoanType");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.tblMstLoanType, DTOModel.tblMstLoanType>();
                });
                var dtoLoanType = Mapper.Map<DTOModel.tblMstLoanType>(editLoanType);
                genericRepo.Update<DTOModel.tblMstLoanType>(dtoLoanType);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool Delete(int loanTypeID)
        {
            log.Info($"LoanTypeService/Delete/{loanTypeID}");
            bool flag = false;
            try
            {
                DTOModel.tblMstLoanType dtoLoanType = new DTOModel.tblMstLoanType();
                dtoLoanType = genericRepo.Get<DTOModel.tblMstLoanType>(x =>x.LoanTypeId== loanTypeID).FirstOrDefault();
                dtoLoanType.IsDeleted = true;
                genericRepo.Update<DTOModel.tblMstLoanType>(dtoLoanType);
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
