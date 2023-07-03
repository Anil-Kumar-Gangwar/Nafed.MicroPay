using System;
using System.Collections.Generic;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Services.HelpDesk
{
    public class SupportTeamService : BaseService, ISupportTeamService
    {
        private readonly IGenericRepository genericRepo;
        public SupportTeamService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.SupportTeam> GetSupportTeamList()
        {
            log.Info($"SupportTeamService/GetSupportTeamList");
            try
            {
                var result = genericRepo.Get<DTOModel.Support_team>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Support_team, Model.SupportTeam>();              
                });
                var listSupportTeam = Mapper.Map<List<Model.SupportTeam>>(result);
                return listSupportTeam;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SupportTeamExists(string sTeamName, string description)
        {
            return genericRepo.Exists<DTOModel.Support_team>(x => x.name.Equals(sTeamName) && x.description.Equals(description) && x.IsDeleted == false);
        }

        public int InsertSupportTeam(SupportTeam SupportTeam)
        {
            log.Info($"SupportTeamService/InsertSupportTeam");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SupportTeam, DTOModel.Support_team>();
                });
                var dtoSupportTeam = Mapper.Map<DTOModel.Support_team>(SupportTeam);
                genericRepo.Insert<DTOModel.Support_team>(dtoSupportTeam);
                return dtoSupportTeam.ID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.SupportTeam GetSupportTeamByID(int id)
        {
            log.Info($"SupportTeamService/InsertSupportTeam/{id}");
            try
            {
                var SupportTeamObj = genericRepo.GetByID<DTOModel.Support_team>(id);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Support_team, Model.SupportTeam>();
                });
                var obj = Mapper.Map<Model.SupportTeam>(SupportTeamObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateSupportTeam(Model.SupportTeam SupportTeam)
        {
            log.Info($"SupportTeamService/UpdateSupportTeam");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Support_team>(SupportTeam.ID);
                if (dtoObj != null)
                {
                    dtoObj.name = SupportTeam.name;
                    dtoObj.description = SupportTeam.description;
                    dtoObj.UpdatedBy = SupportTeam.UpdatedBy;
                    dtoObj.UpdatedOn = SupportTeam.UpdatedOn;
                    genericRepo.Update<DTOModel.Support_team>(dtoObj);
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

        public bool Delete(int SupportTeamID)
        {
            log.Info($"SupportTeamService/Delete/{SupportTeamID}");
            bool flag = false;
            try
            {

                var dtoSupportTeam = genericRepo.GetByID<DTOModel.Support_team>(SupportTeamID);
                if (dtoSupportTeam != null)
                {
                    dtoSupportTeam.IsDeleted = true;
                    genericRepo.Update(dtoSupportTeam);
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
