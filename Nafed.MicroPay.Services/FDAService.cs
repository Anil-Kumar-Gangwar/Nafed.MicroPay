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
    public class FDAService : BaseService, IFDAService
    {
        private readonly IGenericRepository genericRepo;
        public FDAService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<FDA> GetFDAList()
        {

            log.Info($"FDAService/GetFDAList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblmstFDA>(x => (bool)!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblmstFDA, Model.FDA>()
                    .ForMember(c => c.upperlimit, c => c.MapFrom(s => s.upperlimit))
                    .ForMember(c => c.val, c => c.MapFrom(s => s.val))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listFDA = Mapper.Map<List<Model.FDA>>(result);
                return listFDA.OrderBy(x => x.upperlimit).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool FDAExists(decimal upperlimit, decimal percentage)
        {
            log.Info($"FDAService/FDAExists/{upperlimit}/{percentage}");
            return genericRepo.Exists<DTOModel.tblmstFDA>(x => x.upperlimit == upperlimit && x.val == percentage);
        }

        public bool UpdateFDA(FDA updateFDA)
        {
            log.Info($"FDAService/UpdateFDA");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FDA, DTOModel.tblmstFDA>()
                   .ForMember(c => c.upperlimit, c => c.MapFrom(s => s.upperlimit))
                .ForMember(c => c.val, c => c.MapFrom(s => s.val))
                .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoFDA = Mapper.Map<DTOModel.tblmstFDA>(updateFDA);

                genericRepo.Update<DTOModel.tblmstFDA>(dtoFDA);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertFDA(FDA createFDA)
        {
            log.Info($"FDAService/InsertBank");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FDA, DTOModel.tblmstFDA>()
                   .ForMember(c => c.upperlimit, c => c.MapFrom(s => s.upperlimit))
                .ForMember(c => c.val, c => c.MapFrom(s => s.val))
                .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoFDA = Mapper.Map<DTOModel.tblmstFDA>(createFDA);
                genericRepo.Insert<DTOModel.tblmstFDA>(dtoFDA);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public FDA GetFDA(decimal upperlimit, decimal percentage)
        {
            log.Info($"FDAService/GetBankbyCode/{upperlimit}/{percentage}");
            try
            {
                var FDAObj = genericRepo.Get<DTOModel.tblmstFDA>(x => x.upperlimit == upperlimit && x.val == percentage).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblmstFDA, Model.FDA>()
                   .ForMember(c => c.upperlimit, c => c.MapFrom(s => s.upperlimit))
                   .ForMember(c => c.val, c => c.MapFrom(s => s.val))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                  .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblmstFDA, Model.FDA>(FDAObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(decimal upperlimit, decimal percentage)
        {
            log.Info($"FDAService/Delete/{upperlimit}/{percentage}");
            bool flag = false;
            try
            {
                DTOModel.tblmstFDA dtoFDA = new DTOModel.tblmstFDA();
                dtoFDA = genericRepo.Get<DTOModel.tblmstFDA>(x => x.upperlimit == upperlimit && x.val == percentage).FirstOrDefault();
                dtoFDA.IsDeleted = true;
                genericRepo.Update<DTOModel.tblmstFDA>(dtoFDA);
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
