using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
  public class MotherTongueService: BaseService, IMotherTongueService
    {
        private readonly IGenericRepository genericRepo;

        public MotherTongueService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.MotherTongueModel> GetMotherTongueList()
        {
            log.Info($"GetMotherTongueList");
            try
            {
                var result = genericRepo.Get<DTOModel.MotherTongue>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.MotherTongue, Model.MotherTongueModel>()
                    .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.Name, c => c.MapFrom(s => s.Name))
                    .ForMember(c => c.ShortName, c => c.MapFrom(s => s.ShortName))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listMotherTongue = Mapper.Map<List<Model.MotherTongueModel>>(result);
                return listMotherTongue.OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool MotherTongueExists(int? iD, string value)
        {
            return genericRepo.Exists<DTOModel.MotherTongue>(x => x.ID != iD && x.Name == value);
        }

        public int InsertMotherTongueDetails(Model.MotherTongueModel createMotherTongueDetails)
        {
            log.Info($"InsertMotherTongueDetails");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.MotherTongueModel, DTOModel.MotherTongue>()
                    .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.Name, c => c.MapFrom(s => s.Name))
                    .ForMember(c => c.ShortName, c => c.MapFrom(s => s.ShortName))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoMotherTongue = Mapper.Map<DTOModel.MotherTongue>(createMotherTongueDetails);
                genericRepo.Insert<DTOModel.MotherTongue>(dtoMotherTongue);
                return dtoMotherTongue.ID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.MotherTongueModel GetmotherTongueDetailsByID(int motherTongueId)
        {
            log.Info($"GetmotherTongueDetailsByID {motherTongueId}");
            try
            {
                var motherTongueObj = genericRepo.GetByID<DTOModel.MotherTongue>(motherTongueId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.MotherTongue, Model.MotherTongueModel>()
                    .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.Name, c => c.MapFrom(s => s.Name))
                    .ForMember(c => c.ShortName, c => c.MapFrom(s => s.ShortName))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.MotherTongue, Model.MotherTongueModel>(motherTongueObj);
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateMotherTongueDetails(Model.MotherTongueModel createMotherTongue)
        {
            log.Info($"UpdateMotherTongueDetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.MotherTongue>(createMotherTongue.ID);
                dtoObj.Name = createMotherTongue.Name;
                dtoObj.ShortName = createMotherTongue.ShortName;
                genericRepo.Update<DTOModel.MotherTongue>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool Delete(int motherTongueId)
        {
            log.Info($"DeleteByID {motherTongueId}");
            bool flag = false;
            try
            {
                DTOModel.MotherTongue dtoMotherTongue = new DTOModel.MotherTongue();
                dtoMotherTongue = genericRepo.GetByID<DTOModel.MotherTongue>(motherTongueId);
                dtoMotherTongue.IsDeleted = true;
                genericRepo.Update<DTOModel.MotherTongue>(dtoMotherTongue);
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
