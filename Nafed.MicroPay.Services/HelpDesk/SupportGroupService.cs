using System;
using System.Collections.Generic;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Services.HelpDesk
{
    public class SupportGroupService : BaseService, ISupportGroupService
    {
        private readonly IGenericRepository genericRepo;
        public SupportGroupService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.SupportGroup> GetSupportGroupList()
        {
            log.Info($"SupportGroupService/GetSupportGroupList");
            try
            {
                var result = genericRepo.Get<DTOModel.Support_group>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Support_group, Model.SupportGroup>();              
                });
                var listSupportGroup = Mapper.Map<List<Model.SupportGroup>>(result);
                return listSupportGroup;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SupportGroupExists(string sgroupName, string description)
        {
            return genericRepo.Exists<DTOModel.Support_group>(x => x.name.Equals(sgroupName) && x.IsDeleted == false);
        }

        public int InsertSupportGroup(SupportGroup supportGroup)
        {
            log.Info($"SupportGroupService/InsertSupportGroup");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SupportGroup, DTOModel.Support_group>();
                });
                var dtoSupportGroup = Mapper.Map<DTOModel.Support_group>(supportGroup);
                genericRepo.Insert<DTOModel.Support_group>(dtoSupportGroup);
                return dtoSupportGroup.ID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.SupportGroup GetSupportGroupByID(int id)
        {
            log.Info($"SupportGroupService/InsertSupportGroup/{id}");
            try
            {
                var supportGroupObj = genericRepo.GetByID<DTOModel.Support_group>(id);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Support_group, Model.SupportGroup>();
                });
                var obj = Mapper.Map<Model.SupportGroup>(supportGroupObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateSupportGroup(Model.SupportGroup supportGroup)
        {
            log.Info($"SupportGroupService/UpdateSupportGroup");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Support_group>(supportGroup.ID);
                if (dtoObj != null)
                {
                    dtoObj.name = supportGroup.name;
                    dtoObj.description = supportGroup.description;
                    dtoObj.UpdatedBy = supportGroup.UpdatedBy;
                    dtoObj.UpdatedOn = supportGroup.UpdatedOn;
                    genericRepo.Update<DTOModel.Support_group>(dtoObj);
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

        public bool Delete(int supportGroupID)
        {
            log.Info($"SupportGroupService/Delete/{supportGroupID}");
            bool flag = false;
            try
            {

                var dtosupportGroup = genericRepo.GetByID<DTOModel.Support_group>(supportGroupID);
                if (dtosupportGroup != null)
                {
                    dtosupportGroup.IsDeleted = true;
                    genericRepo.Update(dtosupportGroup);
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
