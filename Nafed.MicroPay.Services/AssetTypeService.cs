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
    public class AssetTypeService: BaseService, IAssetTypeService
    {
        private readonly IGenericRepository genericRepo;
        public AssetTypeService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.AssetType> GetAssetTypeList(bool isDeleted)
        {
            log.Info($"AssetTypeService/GetAssetTypeList");
            try
            {
                var result = genericRepo.Get<DTOModel.AssetType>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AssetType, Model.AssetType>()
                    .ForMember(c => c.AssetTypeID, c => c.MapFrom(s => s.AssetTypeID))
                    .ForMember(c => c.AssetTypeName, c => c.MapFrom(s => s.AssetTypeName))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listAssetType = Mapper.Map<List<Model.AssetType>>(result);
                return listAssetType.OrderBy(x => x.IsDeleted).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool AssetTypeExists(string AssetType)
        {
            log.Info($"AssetTypeService/AssetTypeExists");
            try
            {
                return genericRepo.Exists<DTOModel.AssetType>(x => x.AssetTypeName == AssetType && !x.IsDeleted);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertAssetType(Model.AssetType createAssetType)
        {
            log.Info($"AssetTypeService/InsertAssetType");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.AssetType, DTOModel.AssetType>()
                    .ForMember(c => c.AssetTypeName, c => c.MapFrom(m => m.AssetTypeName))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoAssetType = Mapper.Map<DTOModel.AssetType>(createAssetType);
                genericRepo.Insert<DTOModel.AssetType>(dtoAssetType);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.AssetType GetAssetTypeID(int AssetTypeID)
        {
            log.Info($"AssetTypeService/GetAssetTypeID/ {AssetTypeID}");
            try
            {
                var cadreObj = genericRepo.GetByID<DTOModel.AssetType>(AssetTypeID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.AssetType, Model.AssetType>()
                     .ForMember(c => c.AssetTypeID, c => c.MapFrom(m => m.AssetTypeID))
                     .ForMember(c => c.AssetTypeName, c => c.MapFrom(m => m.AssetTypeName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.AssetType, Model.AssetType>(cadreObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateAssetType(Model.AssetType editAssetType)
        {
            log.Info($"AssetTypeService/UpdateAssetType");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.AssetType>(editAssetType.AssetTypeID);
                if (dtoObj != null)
                {
                    dtoObj.AssetTypeName = editAssetType.AssetTypeName;
                    dtoObj.UpdatedBy = editAssetType.UpdatedBy;
                    dtoObj.UpdatedOn = editAssetType.UpdatedOn;
                    genericRepo.Update<DTOModel.AssetType>(dtoObj);
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

        public bool Delete(int AssetTypeID)
        {
            log.Info($"AssetTypeService/Delete/{AssetTypeID}");
            bool flag = false;
            try
            {
                var dtoAssetType = genericRepo.GetByID<DTOModel.AssetType>(AssetTypeID);
                if (dtoAssetType != null)
                {
                   // dtoAssetType.IsDeleted = true;
                    genericRepo.Delete<DTOModel.AssetType>(dtoAssetType);
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
