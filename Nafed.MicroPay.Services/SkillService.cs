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
    public class SkillService : BaseService, ISkillService
    {
        private readonly IGenericRepository genericRepo;
        public SkillService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.SkillType> GetSkillTypeList(bool isDeleted)
        {
            log.Info($"SkillService/GetSkillTypeList");
            try
            {
                var result = genericRepo.Get<DTOModel.SkillType>(x => (isDeleted == true ? x.IsDeleted == isDeleted : 1 > 0));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SkillType, Model.SkillType>()
                    .ForMember(c => c.SkillTypeID, c => c.MapFrom(s => s.SkillTypeID))
                    .ForMember(c => c.SkillType1, c => c.MapFrom(s => s.SkillType1))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listSkillType = Mapper.Map<List<Model.SkillType>>(result);
                return listSkillType.OrderBy(x => x.IsDeleted).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SkillTypeExists(int skillTypeID, string skillType)
        {
            log.Info($"SkillService/SkillTypeExists");
            try
            {
                return genericRepo.Exists<DTOModel.SkillType>(x => x.SkillTypeID != skillTypeID && x.SkillType1 == skillType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertSkillType(Model.SkillType createSkillType)
        {
            log.Info($"SkillService/InsertSkillType");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SkillType, DTOModel.SkillType>()
                    .ForMember(c => c.SkillType1, c => c.MapFrom(m => m.SkillType1))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoSkillType = Mapper.Map<DTOModel.SkillType>(createSkillType);
                genericRepo.Insert<DTOModel.SkillType>(dtoSkillType);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.SkillType GetSkillTypeID(int skillTypeID)
        {
            log.Info($"SkillService/GetSkillTypeID/ {skillTypeID}");
            try
            {
                var cadreObj = genericRepo.GetByID<DTOModel.SkillType>(skillTypeID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.SkillType, Model.SkillType>()
                     .ForMember(c => c.SkillTypeID, c => c.MapFrom(m => m.SkillTypeID))
                     .ForMember(c => c.SkillType1, c => c.MapFrom(m => m.SkillType1))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.SkillType, Model.SkillType>(cadreObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateSkillType(Model.SkillType editSkillType)
        {
            log.Info($"SkillService/UpdateSkillType");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.SkillType>(editSkillType.SkillTypeID);
                dtoObj.SkillType1 = editSkillType.SkillType1;
                dtoObj.IsDeleted = editSkillType.IsDeleted;
                dtoObj.UpdatedBy = editSkillType.UpdatedBy;
                dtoObj.UpdatedOn = editSkillType.UpdatedOn;
                genericRepo.Update<DTOModel.SkillType>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool Delete(int skillTypeID)
        {
            log.Info($"SkillService/Delete/{skillTypeID}");
            bool flag = false;
            try
            {
                var dtoSkillType = genericRepo.GetByID<DTOModel.SkillType>(skillTypeID);
                dtoSkillType.IsDeleted = true;
                genericRepo.Update<DTOModel.SkillType>(dtoSkillType);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public List<Model.Skill> GetSkillList(bool isDeleted)
        {
            log.Info($"SkillService/GetSkillList");
            try
            {
                var result = genericRepo.Get<DTOModel.Skill>(x => (isDeleted == true ? x.IsDeleted == isDeleted : 1 > 0));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Skill, Model.Skill>()
                    .ForMember(c => c.SkillId, c => c.MapFrom(s => s.SkillId))
                    .ForMember(c => c.Skill1, c => c.MapFrom(s => s.Skill1))
                    .ForMember(c => c.SkillTypeID, c => c.MapFrom(s => s.SkillTypeID))
                    .ForMember(c => c.SkillType, c => c.MapFrom(s => s.SkillType.SkillType1))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listSkillType = Mapper.Map<List<Model.Skill>>(result);
                return listSkillType.OrderBy(x => x.IsDeleted).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SkillExists(int skillID, string skill)
        {
            log.Info($"SkillService/SkillExists");
            try
            {
                return genericRepo.Exists<DTOModel.Skill>(x => x.SkillId != skillID && x.Skill1 == skill);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertSkill(Model.Skill createSkill)
        {
            log.Info($"SkillService/InsertSkill");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Skill, DTOModel.Skill>()
                    .ForMember(c => c.Skill1, c => c.MapFrom(m => m.Skill1))
                    .ForMember(c => c.SkillTypeID, c => c.MapFrom(m => m.SkillTypeID))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoSkill = Mapper.Map<DTOModel.Skill>(createSkill);
                genericRepo.Insert<DTOModel.Skill>(dtoSkill);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.Skill GetSkillByID(int skillID)
        {
            log.Info($"SkillService/GetSkillByID/ {skillID}");
            try
            {
                var skillObj = genericRepo.GetByID<DTOModel.Skill>(skillID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Skill, Model.Skill>()
                     .ForMember(c => c.SkillId, c => c.MapFrom(m => m.SkillId))
                     .ForMember(c => c.SkillTypeID, c => c.MapFrom(m => m.SkillTypeID))
                     .ForMember(c => c.Skill1, c => c.MapFrom(m => m.Skill1))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Skill, Model.Skill>(skillObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateSkill(Model.Skill editSkill)
        {
            log.Info($"SkillService/UpdateSkill");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Skill>(editSkill.SkillId);
                dtoObj.SkillTypeID = editSkill.SkillTypeID;
                dtoObj.Skill1 = editSkill.Skill1;
                dtoObj.IsDeleted = editSkill.IsDeleted;
                dtoObj.UpdatedBy = editSkill.UpdatedBy;
                dtoObj.UpdatedOn = editSkill.UpdatedOn;
                genericRepo.Update<DTOModel.Skill>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool DeleteSkill(int skillID)
        {
            log.Info($"SkillService/DeleteSkill/{skillID}");
            bool flag = false;
            try
            {
                var dtoSkill = genericRepo.GetByID<DTOModel.Skill>(skillID);
                dtoSkill.IsDeleted = true;
                genericRepo.Update<DTOModel.Skill>(dtoSkill);
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
