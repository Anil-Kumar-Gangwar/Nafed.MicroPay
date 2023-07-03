using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public class CCAService : BaseService, ICCAService
    {
        private readonly IGenericRepository genericRepo;
        public CCAService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }      
      
        public List<CCA> GetCCAList()
        {
            log.Info($"CCAService/GetCCAList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblCCAMaster>(x => (bool)!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblCCAMaster, Model.CCA>()
                    .ForMember(c => c.UpperLimit, c => c.MapFrom(s => s.UpperLimit))
                    .ForMember(c => c.AmtCityGradeA1, c => c.MapFrom(s => s.AmtCityGradeA1))
                    .ForMember(c => c.AmtCityGradeA, c => c.MapFrom(s => s.AmtCityGradeA))
                    .ForMember(c => c.AmtCityGradeB1, c => c.MapFrom(s => s.AmtCityGradeB1))
                    .ForMember(c => c.AmtCityGradeB2, c => c.MapFrom(s => s.AmtCityGradeB2))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listCCA = Mapper.Map<List<Model.CCA>>(result);
                return listCCA.OrderBy(x => x.UpperLimit).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool CCAExists(decimal upperLimit)
        {
            log.Info($"CCAService/CCAExists/{upperLimit}");
            return genericRepo.Exists<DTOModel.tblCCAMaster>(x => x.UpperLimit == upperLimit);
        }

        public bool UpdateCCA(CCA updateCCA)
        {
            log.Info($"CCAService/UpdateCCA");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.CCA, DTOModel.tblCCAMaster>()
                 .ForMember(c => c.UpperLimit, c => c.MapFrom(s => s.UpperLimit))
                    .ForMember(c => c.AmtCityGradeA1, c => c.MapFrom(s => s.AmtCityGradeA1))
                    .ForMember(c => c.AmtCityGradeA, c => c.MapFrom(s => s.AmtCityGradeA))
                    .ForMember(c => c.AmtCityGradeB1, c => c.MapFrom(s => s.AmtCityGradeB1))
                    .ForMember(c => c.AmtCityGradeB2, c => c.MapFrom(s => s.AmtCityGradeB2))
                .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoCCA = Mapper.Map<DTOModel.tblCCAMaster>(updateCCA);

                genericRepo.Update<DTOModel.tblCCAMaster>(dtoCCA);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertCCA(CCA createCCA)
        {
            log.Info($"CCAService/InsertCCA");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.CCA, DTOModel.tblCCAMaster>()
                    .ForMember(c => c.UpperLimit, c => c.MapFrom(s => s.UpperLimit))
                    .ForMember(c => c.AmtCityGradeA1, c => c.MapFrom(s => s.AmtCityGradeA1))
                    .ForMember(c => c.AmtCityGradeA, c => c.MapFrom(s => s.AmtCityGradeA))
                    .ForMember(c => c.AmtCityGradeB1, c => c.MapFrom(s => s.AmtCityGradeB1))
                    .ForMember(c => c.AmtCityGradeB2, c => c.MapFrom(s => s.AmtCityGradeB2))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoCCA = Mapper.Map<DTOModel.tblCCAMaster>(createCCA);
                genericRepo.Insert<DTOModel.tblCCAMaster>(dtoCCA);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public CCA GetCCA(decimal upperLimit)
        {
            log.Info($"CCAService/GetCCA/{upperLimit}");
            try
            {
                var CCAObj = genericRepo.Get<DTOModel.tblCCAMaster>(x => x.UpperLimit == upperLimit).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblCCAMaster, Model.CCA>()
                    .ForMember(c => c.UpperLimit, c => c.MapFrom(s => s.UpperLimit))
                    .ForMember(c => c.AmtCityGradeA1, c => c.MapFrom(s => s.AmtCityGradeA1))
                    .ForMember(c => c.AmtCityGradeA, c => c.MapFrom(s => s.AmtCityGradeA))
                    .ForMember(c => c.AmtCityGradeB1, c => c.MapFrom(s => s.AmtCityGradeB1))
                    .ForMember(c => c.AmtCityGradeB2, c => c.MapFrom(s => s.AmtCityGradeB2))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblCCAMaster, Model.CCA>(CCAObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(decimal upperLimit)
        {
            log.Info($"CCAService/Delete/{upperLimit}");
            bool flag = false;
            try
            {
                DTOModel.tblCCAMaster dtoCCA = new DTOModel.tblCCAMaster();
                dtoCCA = genericRepo.Get<DTOModel.tblCCAMaster>(x => x.UpperLimit == upperLimit).FirstOrDefault();
                dtoCCA.IsDeleted = true;
                genericRepo.Update<DTOModel.tblCCAMaster>(dtoCCA);
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
