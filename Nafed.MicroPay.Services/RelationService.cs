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
    public class RelationService : BaseService, IRelationService
    {
        private readonly IGenericRepository genericRepo;

        public RelationService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.Relation> GetRelationList()
        {
            log.Info($"RelationService/GetRelationList");
            try
            {
                var result = genericRepo.Get<DTOModel.Relation>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Relation, Model.Relation>()
                    .ForMember(c => c.RelationID, c => c.MapFrom(s => s.RelationID))
                    .ForMember(c => c.RelationCode, c => c.MapFrom(s => s.RelationCode))
                    .ForMember(c => c.RelationName, c => c.MapFrom(s => s.RelationName))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listRelation = Mapper.Map<List<Model.Relation>>(result);
                return listRelation.OrderBy(x => x.RelationName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool RelationNameExists(int? RelationID, string RelationName)
        {
            log.Info($"RelationService/RelationNameExists");
            return genericRepo.Exists<DTOModel.Relation>(x => x.RelationID != RelationID && x.RelationName == RelationName && !x.IsDeleted);
        }

        public bool RelationCodeExists(int? RelationID, string RelationCode)
        {
            log.Info($"RelationService/RelationCodeExists");
            return genericRepo.Exists<DTOModel.Relation>(x => x.RelationID != RelationID && x.RelationCode == RelationCode && !x.IsDeleted);
        }


        public bool Delete(int RelationID)
        {
            log.Info($"RelationService/Delete/{RelationID}");
            bool flag = false;
            try
            {
                DTOModel.Relation dtoRelation = new DTOModel.Relation();
                dtoRelation = genericRepo.GetByID<DTOModel.Relation>(RelationID);
                dtoRelation.IsDeleted = true;
                genericRepo.Update<DTOModel.Relation>(dtoRelation);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
   

        public Model.Relation GetRelationByID(int RelationID)
        {
            log.Info($"RelationService/GetRelationByID/ {RelationID}");
            try
            {
                var RelationObj = genericRepo.GetByID<DTOModel.Relation>(RelationID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Relation, Model.Relation>()

                     .ForMember(c => c.RelationCode, c => c.MapFrom(m => m.RelationCode))
                     .ForMember(c => c.RelationName, c => c.MapFrom(m => m.RelationName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Relation, Model.Relation>(RelationObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        
         
        public bool  InsertRelation(Model.Relation createRelation)
        {
            log.Info($"RelationService/InsertRelation");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Relation, DTOModel.Relation>()
                    .ForMember(c => c.RelationCode, c => c.MapFrom(m => m.RelationCode))
                    .ForMember(c => c.RelationName, c => c.MapFrom(m => m.RelationName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoRelation = Mapper.Map<DTOModel.Relation>(createRelation);
                genericRepo.Insert<DTOModel.Relation>(dtoRelation);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

       
       
        public bool UpdateRelation(Model.Relation editRelationItem)
        {
            log.Info($"RelationService/UpdateRelation");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Relation>(editRelationItem.RelationID);
                dtoObj.RelationCode = editRelationItem.RelationCode;
                dtoObj.RelationName = editRelationItem.RelationName;
                dtoObj.UpdatedOn = editRelationItem.UpdatedOn;
                dtoObj.UpdatedBy = editRelationItem.UpdatedBy;
                genericRepo.Update<DTOModel.Relation>(dtoObj);
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
