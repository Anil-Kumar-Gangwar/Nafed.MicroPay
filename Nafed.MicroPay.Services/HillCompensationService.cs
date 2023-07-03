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
    public class HillCompensationService : BaseService, IHillCompensationService
    {
        private readonly IGenericRepository genericRepo;
        public HillCompensationService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public bool HillCompensationExists(string branchCode, decimal upperLimit)
        {
            log.Info($"HillCompensationService/OTAExists/{branchCode}/{upperLimit}");
            return genericRepo.Exists<DTOModel.tblMstHillComp>(x => x.BranchCode == branchCode && x.UpperLimit==upperLimit && (bool)!x.IsDeleted);

        }
        public HillCompensation GetHillCompensationbyBranchAmount(string branchCode, decimal upperLimit)
        {
            log.Info($"HillCompensationService/GetHillCompensationbyBranchAmount/{branchCode}/{upperLimit}");
            try
            {
                var oTAObj = genericRepo.Get<DTOModel.tblMstHillComp>(x=>x.BranchCode== branchCode && x.UpperLimit==upperLimit).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstHillComp, Model.HillCompensation>()
                     .ForMember(c => c.BranchCode, c => c.MapFrom(m => m.BranchCode))
                      .ForMember(c => c.BranchID, c => c.MapFrom(m => m.Branch.BranchCode))
                    .ForMember(c => c.UpperLimit, c => c.MapFrom(m => m.UpperLimit))
                 .ForMember(c => c.Amount, c => c.MapFrom(m => m.Amount))                    
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblMstHillComp, Model.HillCompensation>(oTAObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<HillCompensation> GetHillCompensationList()
        {
            log.Info($"HillCompensationService/GetHillCompensationList");
            try

            {
                var result = genericRepo.Get<DTOModel.tblMstHillComp>(x => (bool)!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstHillComp, Model.HillCompensation>()
                       .ForMember(c => c.BranchID, c => c.MapFrom(m => m.Branch.BranchID))
                        .ForMember(c => c.BranchCode, c => c.MapFrom(m => m.BranchCode))
                         .ForMember(c => c.Branch, c => c.MapFrom(m => m.Branch.BranchName))
                      .ForMember(c => c.UpperLimit, c => c.MapFrom(m => m.UpperLimit))
                   .ForMember(c => c.Amount, c => c.MapFrom(m => m.Amount))
                       .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var listHillCompensation = Mapper.Map<List<Model.HillCompensation>>(result);
                return listHillCompensation.OrderBy(x => x.BranchCode).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool InsertHillCompensation(HillCompensation createHillCompensation)
        {
            log.Info($"HillCompensationService/InsertHillCompensation");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap< Model.HillCompensation, DTOModel.tblMstHillComp>()
                     .ForMember(c => c.BranchID, c => c.MapFrom(m => m.BranchID))
                      .ForMember(c => c.BranchCode, c => c.MapFrom(m =>m.BranchCode))
                     .ForMember(c => c.UpperLimit, c => c.MapFrom(m => m.UpperLimit))
                  .ForMember(c => c.Amount, c => c.MapFrom(m => m.Amount))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))     
                   .ForAllOtherMembers(c => c.Ignore());
                });                
                var dtoHillCompensation = Mapper.Map<DTOModel.tblMstHillComp>(createHillCompensation);
                var getBranchCode = genericRepo.GetByID<DTOModel.Branch>(dtoHillCompensation.BranchID).BranchCode;
                dtoHillCompensation.BranchCode = getBranchCode;
                genericRepo.Insert<DTOModel.tblMstHillComp>(dtoHillCompensation);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        public bool UpdateHillCompensation(HillCompensation editHillCompensation)
        {
            log.Info($"HillCompensationService/UpdateOTA");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.HillCompensation, DTOModel.tblMstHillComp>()
                      .ForMember(c => c.BranchID, c => c.MapFrom(m => m.BranchID))
                      .ForMember(c => c.BranchCode, c => c.MapFrom(m => m.BranchCode))
                     .ForMember(c => c.UpperLimit, c => c.MapFrom(m => m.UpperLimit))
                  .ForMember(c => c.Amount, c => c.MapFrom(m => m.Amount))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(m => m.UpdatedOn))
                   .ForMember(c => c.UpdatedBy, c => c.MapFrom(m => m.UpdatedBy))
                   .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoHillCompensation = Mapper.Map<DTOModel.tblMstHillComp>(editHillCompensation);
                genericRepo.Update<DTOModel.tblMstHillComp>(dtoHillCompensation);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }
        public bool Delete(string branchCode, decimal upperLimit)
        {
            log.Info($"HillCompensationService/Delete/{branchCode}");
            bool flag = false;
            try
            {
                DTOModel.tblMstHillComp dtoHillCompensation = new DTOModel.tblMstHillComp();
                dtoHillCompensation = genericRepo.Get<DTOModel.tblMstHillComp>(x=>x.BranchCode== branchCode && x.UpperLimit==upperLimit).FirstOrDefault();
                dtoHillCompensation.IsDeleted = true;
                genericRepo.Update<DTOModel.tblMstHillComp>(dtoHillCompensation);
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
