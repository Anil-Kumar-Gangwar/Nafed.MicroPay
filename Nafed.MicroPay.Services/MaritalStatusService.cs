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
    public class MaritalStatusService : BaseService, IMaritalStatusService
    {

        private readonly IGenericRepository genericRepo;

        public MaritalStatusService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.MaritalStatus> GetMaritalStatusList()
        {
            log.Info($"MaritalStatusService/GetMaritalStatusList");
            try
            {
                var result = genericRepo.Get<DTOModel.MaritalStatu>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.MaritalStatu, Model.MaritalStatus>()
                    .ForMember(c => c.MaritalStatusID, c => c.MapFrom(s => s.MaritalStatusID))
                    .ForMember(c => c.MaritalStatusCode, c => c.MapFrom(s => s.MaritalStatusCode))
                    .ForMember(c => c.MaritalStatusName, c => c.MapFrom(s => s.MaritalStatusName))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listMaritalStatus = Mapper.Map<List<Model.MaritalStatus>>(result);
                return listMaritalStatus.OrderBy(x => x.MaritalStatusName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool  Delete(int MaritalStatusID)
        {
            log.Info($"MaritalStatusService/Delete/{MaritalStatusID}");
            bool flag = false;
            try
            {
                DTOModel.MaritalStatu dtoMaritalStatus = new DTOModel.MaritalStatu();
                dtoMaritalStatus = genericRepo.GetByID<DTOModel.MaritalStatu>(MaritalStatusID);
                dtoMaritalStatus.IsDeleted = true;
                genericRepo.Update<DTOModel.MaritalStatu>(dtoMaritalStatus);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.MaritalStatus  GetMaritalStatusByID(int MaritalStatusID)
        {
            log.Info($"MaritalStatusService/GetMaritalStatusByID/ {MaritalStatusID}");
            try
            {
                var MaritalStatusObj = genericRepo.GetByID<DTOModel.MaritalStatu>(MaritalStatusID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.MaritalStatu, Model.MaritalStatus>()

                     .ForMember(c => c.MaritalStatusCode, c => c.MapFrom(m => m.MaritalStatusCode))
                     .ForMember(c => c.MaritalStatusName, c => c.MapFrom(m => m.MaritalStatusName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.MaritalStatu, Model.MaritalStatus>(MaritalStatusObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        

        public bool  InsertMaritalStatus(MaritalStatus createMaritalStatus)
        {
            log.Info($"MaritalStatusService/InsertMaritalStatus");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.MaritalStatus, DTOModel.MaritalStatu>()
                    .ForMember(c => c.MaritalStatusCode, c => c.MapFrom(m => m.MaritalStatusCode))
                    .ForMember(c => c.MaritalStatusName, c => c.MapFrom(m => m.MaritalStatusName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoMaritalStatus = Mapper.Map<DTOModel.MaritalStatu>(createMaritalStatus);
                genericRepo.Insert<DTOModel.MaritalStatu>(dtoMaritalStatus);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool  MaritalStatusCodeExists(int? MaritalStatusID, string MaritalStatusCode)
        {
            log.Info($"MaritalStatusService/MaritalStatusCodeExists");
            return genericRepo.Exists<DTOModel.MaritalStatu>(x => x.MaritalStatusID != MaritalStatusID && x.MaritalStatusCode == MaritalStatusCode && !x.IsDeleted);
        }

        public bool  MaritalStatusNameExists(int? MaritalStatusID, string MaritalStatusName)
        {

            log.Info($"MaritalStatusService/MaritalStatusNameExists");
            return genericRepo.Exists<DTOModel.MaritalStatu>(x => x.MaritalStatusID != MaritalStatusID && x.MaritalStatusName == MaritalStatusName && !x.IsDeleted);
        }

        public bool  UpdateMaritalStatus(MaritalStatus editMaritalStatusItem)
        {
            log.Info($"MaritalStatusService/UpdateMaritalStatus");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.MaritalStatu>(editMaritalStatusItem.MaritalStatusID);
                dtoObj.MaritalStatusCode = editMaritalStatusItem.MaritalStatusCode;
                dtoObj.MaritalStatusName = editMaritalStatusItem.MaritalStatusName;
                dtoObj.UpdatedOn = editMaritalStatusItem.UpdatedOn;
                dtoObj.UpdatedBy = editMaritalStatusItem.UpdatedBy;
                genericRepo.Update<DTOModel.MaritalStatu>(dtoObj);
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
