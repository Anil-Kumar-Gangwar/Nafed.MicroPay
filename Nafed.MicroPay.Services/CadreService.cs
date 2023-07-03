using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public class CadreService : BaseService, ICadreService
    {

        private readonly IGenericRepository genericRepo;
        public CadreService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public List<Model.Cadre> GetCadreList()
        {
            log.Info($"CadreService/GetCadreList");
            try
            {
                var result = genericRepo.Get<DTOModel.Cadre>(x=>!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Cadre, Model.Cadre>()
                    .ForMember(c => c.CadreID, c => c.MapFrom(s => s.CadreID))
                    .ForMember(c => c.CadreCode, c => c.MapFrom(s => s.CadreCode))
                    .ForMember(c => c.CadreName, c => c.MapFrom(s => s.CadreName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listCadre = Mapper.Map<List<Model.Cadre>>(result);
                return listCadre.OrderBy(x => x.CadreName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool CadreNameExists(int? cadreID, string cadreName)
        {
            log.Info($"CadreService/CadreNameExists");
            return genericRepo.Exists<DTOModel.Cadre>(x => x.CadreID != cadreID && x.CadreName == cadreName && !x.IsDeleted);
        }
        public bool CadreCodeExists(int? cadreID, string cadreCode)
        {
            log.Info($"CadreService/CadreCodeExists");
            return genericRepo.Exists<DTOModel.Cadre>(x => x.CadreID != cadreID && x.CadreCode == cadreCode && !x.IsDeleted);
        }
        public bool UpdateCadre(Model.Cadre editCadreItem)
        {
            log.Info($"CadreService/UpdateCadre");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Cadre>(editCadreItem.CadreID);
                dtoObj.CadreCode = editCadreItem.CadreCode;
                dtoObj.CadreName = editCadreItem.CadreName;
                dtoObj.UpdatedBy = editCadreItem.UpdatedBy;
                dtoObj.UpdatedOn = editCadreItem.UpdatedOn;
                genericRepo.Update<DTOModel.Cadre>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertCadre(Model.Cadre createCadre)
        {
            log.Info($"CadreService/InsertCadre");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Cadre, DTOModel.Cadre>()
                    .ForMember(c => c.CadreCode, c => c.MapFrom(m => m.CadreCode))
                    .ForMember(c => c.CadreName, c => c.MapFrom(m => m.CadreName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m =>m.IsDeleted))
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoCadre = Mapper.Map<DTOModel.Cadre>(createCadre);
                genericRepo.Insert<DTOModel.Cadre>(dtoCadre);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.Cadre GetCadreByID(int cadreID)
        {
            log.Info($"CadreService/GetCadreByID/ {cadreID}");
            try
            {
                var cadreObj = genericRepo.GetByID<DTOModel.Cadre>(cadreID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Cadre, Model.Cadre>()

                     .ForMember(c => c.CadreCode, c => c.MapFrom(m => m.CadreCode))
                     .ForMember(c => c.CadreName, c => c.MapFrom(m => m.CadreName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Cadre, Model.Cadre>(cadreObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(int cadreID)
        {
            log.Info($"CadreService/Delete/{cadreID}");
            bool flag = false;
            try
            {
                DTOModel.Cadre dtoCadre = new DTOModel.Cadre();
                dtoCadre = genericRepo.GetByID<DTOModel.Cadre>(cadreID);
                dtoCadre.IsDeleted = true;
                genericRepo.Update<DTOModel.Cadre>(dtoCadre);
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
