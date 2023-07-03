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
   public class AcadmicProfessionalDetailsService: BaseService, IAcadmicProfessionalDetails
    {
        private readonly IGenericRepository genericRepo;

        public AcadmicProfessionalDetailsService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.AcadmicProfessionalDetailsModel> GetAcadmicProfessionalList(int? typeID)
        {
            log.Info($"GetAcadmicProfessionalList");
            try
            {
                var result = genericRepo.Get<DTOModel.AcadmicProfessionalDetail>(x => !x.IsDeleted && (typeID.HasValue ? x.TypeID == typeID.Value : 1 > 0));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AcadmicProfessionalDetail, Model.AcadmicProfessionalDetailsModel>()
                    .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                    .ForMember(c => c.Value, c => c.MapFrom(s => s.Value))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listAcadmicProfessional = Mapper.Map<List<Model.AcadmicProfessionalDetailsModel>>(result);

                listAcadmicProfessional.ForEach(x =>
                {
                    x.Type = x.TypeID == 1 ? "Academic" : "Professional";
                });

                return listAcadmicProfessional.OrderBy(x => x.Value).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool AcadmicProfessionalDetailsExists(int? id, string value, int typeID)
        {
            return genericRepo.Exists<DTOModel.AcadmicProfessionalDetail>(x => x.ID != id && x.Value == value && x.TypeID == typeID && x.IsDeleted == false);
        }

        public int InsertAcadmicProfessionalDetails(Model.AcadmicProfessionalDetailsModel createAcadmicProfessionalDetails)
        {
            log.Info($"InsertAcadmicProfessionalDetails");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.AcadmicProfessionalDetailsModel, DTOModel.AcadmicProfessionalDetail>()
                    .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                    .ForMember(c => c.Value, c => c.MapFrom(s => s.Value))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoAcadmicProfessional = Mapper.Map<DTOModel.AcadmicProfessionalDetail>(createAcadmicProfessionalDetails);
                genericRepo.Insert<DTOModel.AcadmicProfessionalDetail>(dtoAcadmicProfessional);
                return dtoAcadmicProfessional.ID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            
        }

        public Model.AcadmicProfessionalDetailsModel GetAcadmicProfessionalDetailsByID(int acadmicProfessionalDetailsID)
        {
            log.Info($"GetAcadmicProfessionalDetailsByID {acadmicProfessionalDetailsID}");
            try
            {
                var acadmicProfessionalObj = genericRepo.GetByID<DTOModel.AcadmicProfessionalDetail>(acadmicProfessionalDetailsID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.AcadmicProfessionalDetail, Model.AcadmicProfessionalDetailsModel>()
                    .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                    .ForMember(c => c.Value, c => c.MapFrom(s => s.Value))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.AcadmicProfessionalDetail, Model.AcadmicProfessionalDetailsModel>(acadmicProfessionalObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateAcadmicProfessionalDetails(Model.AcadmicProfessionalDetailsModel editAcadmicProfessionalDetails)
        {
            log.Info($"UpdateAcadmicProfessionalDetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.AcadmicProfessionalDetail>(editAcadmicProfessionalDetails.ID);
                dtoObj.TypeID = editAcadmicProfessionalDetails.TypeID;
                dtoObj.Value = editAcadmicProfessionalDetails.Value;
                genericRepo.Update<DTOModel.AcadmicProfessionalDetail>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool Delete(int acadmicProfessionalDetailsId)
        {
            log.Info($"DeleteByID {acadmicProfessionalDetailsId}");
            bool flag = false;
            try
            {
                DTOModel.AcadmicProfessionalDetail dtoAcadmicProfessional = new DTOModel.AcadmicProfessionalDetail();
                dtoAcadmicProfessional = genericRepo.GetByID<DTOModel.AcadmicProfessionalDetail>(acadmicProfessionalDetailsId);

                dtoAcadmicProfessional.IsDeleted = true;

                genericRepo.Update<DTOModel.AcadmicProfessionalDetail>(dtoAcadmicProfessional);
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
