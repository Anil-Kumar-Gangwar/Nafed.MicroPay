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
    public class ManufacturerService: BaseService, IManufacturerService
    {
        private readonly IGenericRepository genericRepo;
        public ManufacturerService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.Manufacturer> GetManufacturerList(bool isDeleted)
        {
            log.Info($"ManufacturerService/GetManufacturerList");
            try
            {
                var result = genericRepo.Get<DTOModel.Manufacturer>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Manufacturer, Model.Manufacturer>()
                    .ForMember(c => c.ManufacturerID, c => c.MapFrom(s => s.ManufacturerID))
                    .ForMember(c => c.ManufacturerName, c => c.MapFrom(s => s.ManufacturerName))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listManufacturer = Mapper.Map<List<Model.Manufacturer>>(result);
                return listManufacturer.OrderBy(x => x.IsDeleted).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool ManufacturerExists(string Manufacturer)
        {
            log.Info($"ManufacturerService/ManufacturerExists");
            try
            {
                return genericRepo.Exists<DTOModel.Manufacturer>(x => x.ManufacturerName == Manufacturer && !x.IsDeleted);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertManufacturer(Model.Manufacturer createManufacturer)
        {
            log.Info($"ManufacturerService/InsertManufacturer");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Manufacturer, DTOModel.Manufacturer>()
                    .ForMember(c => c.ManufacturerName, c => c.MapFrom(m => m.ManufacturerName))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoManufacturer = Mapper.Map<DTOModel.Manufacturer>(createManufacturer);
                genericRepo.Insert<DTOModel.Manufacturer>(dtoManufacturer);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.Manufacturer GetManufacturerID(int ManufacturerID)
        {
            log.Info($"ManufacturerService/GetManufacturerID/ {ManufacturerID}");
            try
            {
                var cadreObj = genericRepo.GetByID<DTOModel.Manufacturer>(ManufacturerID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Manufacturer, Model.Manufacturer>()
                     .ForMember(c => c.ManufacturerID, c => c.MapFrom(m => m.ManufacturerID))
                     .ForMember(c => c.ManufacturerName, c => c.MapFrom(m => m.ManufacturerName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Manufacturer, Model.Manufacturer>(cadreObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateManufacturer(Model.Manufacturer editManufacturer)
        {
            log.Info($"ManufacturerService/UpdateManufacturer");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Manufacturer>(editManufacturer.ManufacturerID);
                if (dtoObj != null)
                {
                    dtoObj.ManufacturerName = editManufacturer.ManufacturerName;
                    dtoObj.UpdatedBy = editManufacturer.UpdatedBy;
                    dtoObj.UpdatedOn = editManufacturer.UpdatedOn;
                    genericRepo.Update<DTOModel.Manufacturer>(dtoObj);
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool Delete(int ManufacturerID)
        {
            log.Info($"ManufacturerService/Delete/{ManufacturerID}");
            bool flag = false;
            try
            {
                var dtoManufacturer = genericRepo.GetByID<DTOModel.Manufacturer>(ManufacturerID);
                if (dtoManufacturer != null)
                {                   
                    genericRepo.Delete<DTOModel.Manufacturer>(dtoManufacturer);
                    flag = true;
                }
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
