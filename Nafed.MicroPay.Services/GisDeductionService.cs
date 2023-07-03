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
    public class GisDeductionService : BaseService, IGisDeductionService
    {
        private readonly IGenericRepository genericRepo;
        public GisDeductionService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }      
      
        public List<GisDeduction> GetGisDeductionList()
        {
            log.Info($"GisDeductionService/GetGisDeductionList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblGisDeduction>(x => (bool)!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblGisDeduction, Model.GisDeduction>()
                    .ForMember(c => c.Category, c => c.MapFrom(s => s.Category))
                    .ForMember(c => c.Code, c => c.MapFrom(s => s.Code))
                    .ForMember(c => c.Rate, c => c.MapFrom(s => s.Rate))                  
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listGisDeduction = Mapper.Map<List<Model.GisDeduction>>(result);
                return listGisDeduction.OrderBy(x => x.Category).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool GisDeductionExists(string category, int code)
        {
            log.Info($"GisDeductionService/GisDeductionExists/{category}/{code}");
            return genericRepo.Exists<DTOModel.tblGisDeduction>(x => x.Category == category && x.Code==code);
        }

        public bool UpdateGisDeduction(GisDeduction updateGisDeduction)
        {
            log.Info($"GisDeductionService/UpdateGisDeduction");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                 cfg.CreateMap<Model.GisDeduction, DTOModel.tblGisDeduction>()
                .ForMember(c => c.Category, c => c.MapFrom(s => s.Category))
                .ForMember(c => c.Code, c => c.MapFrom(s => s.Code))
                .ForMember(c => c.Rate, c => c.MapFrom(s => s.Rate))
                .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoGisDeduction = Mapper.Map<DTOModel.tblGisDeduction>(updateGisDeduction);

                genericRepo.Update<DTOModel.tblGisDeduction>(dtoGisDeduction);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertGisDeduction(GisDeduction createGisDeduction)
        {
            log.Info($"GisDeductionService/InsertGisDeduction");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.GisDeduction, DTOModel.tblGisDeduction>()
                    .ForMember(c => c.Category, c => c.MapFrom(s => s.Category))
                    .ForMember(c => c.Code, c => c.MapFrom(s => s.Code))
                    .ForMember(c => c.Rate, c => c.MapFrom(s => s.Rate))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoGisDeduction = Mapper.Map<DTOModel.tblGisDeduction>(createGisDeduction);
                genericRepo.Insert<DTOModel.tblGisDeduction>(dtoGisDeduction);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public GisDeduction GetGisDeduction(string category, int code)
        {
            log.Info($"GisDeductionService/GetGisDeduction/{category}/{code}");
            try
            {
                var GisDeductionObj = genericRepo.Get<DTOModel.tblGisDeduction>(x => x.Category == category && x.Code== code).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblGisDeduction, Model.GisDeduction>()
                    .ForMember(c => c.Category, c => c.MapFrom(s => s.Category))
                    .ForMember(c => c.Code, c => c.MapFrom(s => s.Code))
                    .ForMember(c => c.Rate, c => c.MapFrom(s => s.Rate))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblGisDeduction, Model.GisDeduction>(GisDeductionObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(string category, int code)
        {
            log.Info($"GisDeductionService/Delete/{category}/{code}");
            bool flag = false;
            try
            {
                DTOModel.tblGisDeduction dtoGisDeduction = new DTOModel.tblGisDeduction();
                dtoGisDeduction = genericRepo.Get<DTOModel.tblGisDeduction>(x => x.Category == category && x.Code==code).FirstOrDefault();
                dtoGisDeduction.IsDeleted = true;
                genericRepo.Update<DTOModel.tblGisDeduction>(dtoGisDeduction);
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
