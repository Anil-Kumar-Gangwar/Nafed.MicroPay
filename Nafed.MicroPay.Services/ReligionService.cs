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
    public class ReligionService : BaseService, IReligionService
    {
        private readonly IGenericRepository genericRepo;
        public ReligionService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public bool Delete(int religionID)
        {
            log.Info($"ReligionService/Delete{religionID}");
            bool flag = false;
            try
            {
                Data.Models.Religion dtoReligion = new Data.Models.Religion();
                dtoReligion = genericRepo.GetByID<Data.Models.Religion>(religionID);
                dtoReligion.IsDeleted = true;
                genericRepo.Update<Data.Models.Religion>(dtoReligion);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public List<Model.Religion> GetReligion()
        {
            log.Info($"ReligionService/GetReligion");
            try
            {
                var result = genericRepo.Get<DTOModel.Religion>(x=>!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Religion, Model.Religion>()
                    .ForMember(c => c.ReligionID, c => c.MapFrom(s => s.ReligionID))
                    .ForMember(c => c.ReligionName, c => c.MapFrom(s => s.ReligionName))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listReligion = Mapper.Map<List<Model.Religion>>(result);
                return listReligion.OrderBy(x => x.ReligionName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Model.Religion GetReligionByID(int religionID)
        {
            log.Info($"ReligionService/GetReligionByID/{religionID}");
            try
            {
                var religionObj = genericRepo.GetByID<DTOModel.Religion>(religionID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Religion, Model.Religion>()
                     .ForMember(c => c.ReligionID, c => c.MapFrom(m => m.ReligionID))
                    .ForMember(c => c.ReligionName, c => c.MapFrom(m => m.ReligionName))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Religion, Model.Religion>(religionObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertReligion(Model.Religion createReligion)
        {
            log.Info($"ReligionService/InsertReligion");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                     cfg.CreateMap<Model.Religion, DTOModel.Religion>()
                    .ForMember(c => c.ReligionName, c => c.MapFrom(m => m.ReligionName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoReligion = Mapper.Map<DTOModel.Religion>(createReligion);
                genericRepo.Insert<DTOModel.Religion>(dtoReligion);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool ReligionNameExists(int? religionID, string religionName)
        {
            log.Info($"ReligionService/ReligionNameExists{religionID}/{religionName}");
            return genericRepo.Exists<DTOModel.Religion>(x => x.ReligionID != religionID && x.ReligionName == religionName && !x.IsDeleted);
        }

        public bool UpdateReligion(Model.Religion editReligion)
        {
            log.Info($"ReligionService/UpdateReligion");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Religion>(editReligion.ReligionID);
                dtoObj.ReligionName = editReligion.ReligionName;
                genericRepo.Update<DTOModel.Religion>(dtoObj);
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
