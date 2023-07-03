using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public class GradeService : BaseService, IGradeService
    {
        private readonly IGenericRepository genericRepo;

        public GradeService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public List<Grade> GetGradeList()
        {
            log.Info($"GradeService/GetGradeList");

            try
            {
                var result = genericRepo.Get<Data.Models.Grade>(x=>x.IsDeleted==false);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Grade, Model.Grade>();
                });
                var listGrade = Mapper.Map<List<Model.Grade>>(result);
                return listGrade.OrderBy(x => x.GradeName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
          
          
        }
        public bool GradeNameExists(int? gradeID, string gradeName)
        {
            return genericRepo.Exists<Data.Models.Grade>(x => x.GradeID != gradeID && x.GradeName == gradeName && x.IsDeleted == false);
        }
        public bool UpdateGade(Model.Grade editGradeEntity)
        {
            log.Info($"GradeService/UpdateGade");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<Data.Models.Grade>(editGradeEntity.GradeID);             
                dtoObj.GradeName = editGradeEntity.GradeName;
                genericRepo.Update<Data.Models.Grade>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertGrade(Model.Grade createGrade)
        {
            log.Info($"GradeService/InsertMenu");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Grade, Data.Models.Grade>()                    
                    .ForMember(c => c.GradeName, c => c.MapFrom(m => m.GradeName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoGrade = Mapper.Map<Data.Models.Grade>(createGrade);
                genericRepo.Insert<Data.Models.Grade>(dtoGrade);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.Grade GetGradeByID(int gradeID)
        {
            log.Info($"GradeService/GetGradeByID {gradeID}");
            try
            {
                var HolidayObj = genericRepo.GetByID<Data.Models.Grade>(gradeID);
                Mapper.Initialize(cfg =>
                {
                     cfg.CreateMap<Data.Models.Grade, Model.Grade>()
                    
                     .ForMember(c => c.GradeName, c => c.MapFrom(m => m.GradeName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<Data.Models.Grade, Model.Grade>(HolidayObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(int gradeID)
        {
            log.Info($"GradeService/Delete{gradeID}");
            bool flag = false;
            try
            {
                Data.Models.Grade dtoGrade = new Data.Models.Grade();
                dtoGrade = genericRepo.GetByID<Data.Models.Grade>(gradeID);
                dtoGrade.IsDeleted = true;
                genericRepo.Update<Data.Models.Grade>(dtoGrade);
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
