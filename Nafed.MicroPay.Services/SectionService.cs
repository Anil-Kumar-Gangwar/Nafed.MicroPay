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
    public class SectionService : BaseService, ISectionService
    { 
        private readonly IGenericRepository genericRepo;

        public SectionService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }


        public  bool  Delete(int SectionID)
        {
            log.Info($"SectionService/Delete/{SectionID}");
            bool flag = false;
            try
            {
                DTOModel.Section dtoSection = new DTOModel.Section();
                dtoSection = genericRepo.GetByID<DTOModel.Section>(SectionID);
                dtoSection.IsDeleted = true;
                genericRepo.Update<DTOModel.Section>(dtoSection);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.Section GetSectionByID(int SectionID)
        {
            log.Info($"SectionService/GetSectionByID/ {SectionID}");
            try
            {
                var SectionObj = genericRepo.GetByID<DTOModel.Section>(SectionID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Section, Model.Section>()

                     .ForMember(c => c.SectionCode, c => c.MapFrom(m => m.SectionCode))
                     .ForMember(c => c.SectionName, c => c.MapFrom(m => m.SectionName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Section, Model.Section>(SectionObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public  List<Model.Section>  GetSectionList()
        {
            log.Info($"SectionService/GetSectionList");
            try
            {
                var result = genericRepo.Get<DTOModel.Section>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Section, Model.Section>()
                    .ForMember(c => c.SectionID, c => c.MapFrom(s => s.SectionID))
                    .ForMember(c => c.SectionCode, c => c.MapFrom(s => s.SectionCode))
                    .ForMember(c => c.SectionName, c => c.MapFrom(s => s.SectionName))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listSection = Mapper.Map<List<Model.Section>>(result);
                return listSection.OrderBy(x => x.SectionName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool  InsertSection(Section createSection)
        {
            log.Info($"SectionService/InsertSection");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Section, DTOModel.Section>()
                    .ForMember(c => c.SectionCode, c => c.MapFrom(m => m.SectionCode))
                    .ForMember(c => c.SectionName, c => c.MapFrom(m => m.SectionName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoSection = Mapper.Map<DTOModel.Section>(createSection);
                genericRepo.Insert<DTOModel.Section>(dtoSection);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool  SectionCodeExists(int? SectionID, string SectionCode)
        {
            log.Info($"SectionService/SectionCodeExists");
            return genericRepo.Exists<DTOModel.Section>(x => x.SectionID != SectionID && x.SectionCode == SectionCode && !x.IsDeleted);
        }

        public bool  SectionNameExists(int? SectionID, string SectionName)
        {

            log.Info($"SectionService/SectionNameExists");
            return genericRepo.Exists<DTOModel.Section>(x => x.SectionID != SectionID && x.SectionName == SectionName && !x.IsDeleted);
        }

        public bool  UpdateSection(Section editSectionItem)
        {
            log.Info($"SectionService/UpdateSection");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Section>(editSectionItem.SectionID);
                dtoObj.SectionCode = editSectionItem.SectionCode;
                dtoObj.SectionName = editSectionItem.SectionName;
                dtoObj.UpdatedOn = editSectionItem.UpdatedOn;
                dtoObj.UpdatedBy = editSectionItem.UpdatedBy;
                genericRepo.Update<DTOModel.Section>(dtoObj);
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
