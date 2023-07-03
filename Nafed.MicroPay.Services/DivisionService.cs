using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
namespace Nafed.MicroPay.Services
{
    public class DivisionService : BaseService, IDivisionService
    {
        private readonly IGenericRepository genericRepo;

        public DivisionService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public bool Delete(int DivisionID)
        {
            log.Info($"DivisionService/Delete/{DivisionID}");
            bool flag = false;
            try
            {
                DTOModel.Division dtoDivision = new DTOModel.Division();
                dtoDivision = genericRepo.GetByID<DTOModel.Division>(DivisionID);
                dtoDivision.IsDeleted = true;
                genericRepo.Update<DTOModel.Division>(dtoDivision);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool DivisionCodeExists(int? DivisionID, string DivisionCode)
        {
            log.Info($"DivisionService/DivisionCodeExists");
            return genericRepo.Exists<DTOModel.Division>(x => x.DivisionID != DivisionID && x.DivisionCode == DivisionCode && !x.IsDeleted);
        }

        public bool DivisionNameExists(int? DivisionID, string DivisionName)
        {
            log.Info($"DivisionServise/DivisionNameExists");
            return genericRepo.Exists<DTOModel.Division>(x => x.DivisionID != DivisionID && x.DivisionName == DivisionName && !x.IsDeleted);
        }

        public Model.Division GetDivisionByID(int DivisionID)
        {
            log.Info($"DivisionService/GetDivisionByID/ {DivisionID}");
            try
            {
                var DivisionObj = genericRepo.GetByID<DTOModel.Division>(DivisionID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Division, Model.Division>()

                     .ForMember(c => c.DivisionCode, c => c.MapFrom(m => m.DivisionCode))
                     .ForMember(c => c.DivisionName, c => c.MapFrom(m => m.DivisionName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Division, Model.Division>(DivisionObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.Division> GetDivisionList()
        {
            log.Info($"DivisionServise/GetDivisionList");
            try
            {
                var result = genericRepo.Get<DTOModel.Division>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {    
                    cfg.CreateMap<DTOModel.Division, Model.Division>()
                    .ForMember(c => c.DivisionID, c => c.MapFrom(s => s.DivisionID))
                    .ForMember(c => c.DivisionCode, c => c.MapFrom(s => s.DivisionCode))
                    .ForMember(c => c.DivisionName, c => c.MapFrom(s => s.DivisionName))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore()); 
                });
                var listDivision = Mapper.Map<List<Model.Division>>(result);
                return listDivision.OrderBy(x => x.DivisionName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex; 
            }
        }

        public bool InsertDivision(Division createDivision)
        {
            log.Info($"DivisionService/InsertDivision");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Division, DTOModel.Division>()
                    .ForMember(c => c.DivisionCode, c => c.MapFrom(m => m.DivisionCode))
                    .ForMember(c => c.DivisionName, c => c.MapFrom(m => m.DivisionName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoDivision = Mapper.Map<DTOModel.Division>(createDivision);
                genericRepo.Insert<DTOModel.Division>(dtoDivision);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateDivision(Division editDivisionItem)
        {
            log.Info($"DivisionService/UpdateDivision");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Division>(editDivisionItem.DivisionID);
                dtoObj.DivisionCode = editDivisionItem.DivisionCode;
                dtoObj.DivisionName = editDivisionItem.DivisionName;
                dtoObj.UpdatedOn = editDivisionItem.UpdatedOn;
                dtoObj.UpdatedBy = editDivisionItem.UpdatedBy;
                genericRepo.Update<DTOModel.Division>(dtoObj);
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
