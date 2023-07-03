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
using System.Data;

namespace Nafed.MicroPay.Services
{
    public class UpdateBasicService : BaseService, IUpdateBasicService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IUpdateBasicRepository UpdateBasicRepo;
        public UpdateBasicService(IGenericRepository genericRepo, IUpdateBasicRepository UpdateBasicRepo)
        {
            this.genericRepo = genericRepo;
            this.UpdateBasicRepo = UpdateBasicRepo;
        }

        #region Update Basic

        public List<Model.UpdateBasic> GetUpdateBasicList()
        {
            log.Info($"UpdateBasicService/GetUpdateBasicList");
            try
            {
                var result = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.DELETEDEMPLOYEE == false); 
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TblMstEmployeeSalary, Model.UpdateBasic > ()
                     .ForMember(c => c.EmployeeSalaryID, c => c.MapFrom(s => s.EmpSalaryID))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeID))
                    .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.EmployeeCode))
                    .ForMember(c => c.EmployeeName, c => c.MapFrom(s => s.tblMstEmployee.Name))
                     .ForMember(c => c.ExistingBasic, c => c.MapFrom(s => s.E_Basic))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listCadre = Mapper.Map<List<Model.UpdateBasic>>(result);
                return listCadre.OrderBy(x => x.EmployeeCode).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Model.UpdateBasic GetBasic(int EmployeeId)
        {
            log.Info($"UpdateBasicService/GetBasic/ {EmployeeId}");
            try
            {
                var cadreObj = genericRepo.GetByID<DTOModel.TblMstEmployeeSalary>(EmployeeId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.TblMstEmployeeSalary, Model.UpdateBasic>()

                     .ForMember(c => c.EmployeeCode, c => c.MapFrom(m => m.EmployeeCode))
                      .ForMember(c => c.EmployeeName, c => c.MapFrom(m => m.tblMstEmployee.Name))
                     .ForMember(c => c.ExistingBasic, c => c.MapFrom(m => m.E_Basic))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.TblMstEmployeeSalary, Model.UpdateBasic>(cadreObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateBasic(Model.UpdateBasic editUpdatebasic)
        {
            log.Info($"UpdateBasicService/UpdateBasic");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.TblMstEmployeeSalary>(editUpdatebasic.EmployeeSalaryID);
                dtoObj.E_Basic = (decimal)editUpdatebasic.NewBasic;
                dtoObj.UpdatedBy = editUpdatebasic.UpdatedBy;
                dtoObj.UpdatedOn = editUpdatebasic.UpdatedOn;
                genericRepo.Update<DTOModel.TblMstEmployeeSalary>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public int ValidateNewBasicAmount(int EmployeeSalaryID, double newBasic)
        {
            int ValidateNewBasicAmount;
            try
            {
                ValidateNewBasicAmount = UpdateBasicRepo.ValidateNewBasicAmount(EmployeeSalaryID, newBasic);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return ValidateNewBasicAmount;
        }

        #endregion

        #region Update Branch/Designation

        public List<Model.UpdateBasic> GetBranchDesignationList()
        {
            log.Info($"UpdateBasicService/GetUpdateBranchDesignationList");
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.UpdateBasic>()
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.EmployeeCode))
                    .ForMember(c => c.EmployeeName, c => c.MapFrom(s => s.Name))
                     .ForMember(c => c.ExistingBranch, c => c.MapFrom(s => s.Branch.BranchName))
                     .ForMember(c => c.ExistingDesg, c => c.MapFrom(s => s.Designation.DesignationName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listCadre = Mapper.Map<List<Model.UpdateBasic>>(result);
                return listCadre.OrderBy(x => x.EmployeeCode).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public Model.UpdateBasic GetBranchDesignation(int EmployeeId)
        {
            log.Info($"UpdateBasicService/GetBranchDesignation/ {EmployeeId}");
            try
            {
                var BrDesgObj = genericRepo.GetByID<DTOModel.tblMstEmployee>(EmployeeId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblMstEmployee, Model.UpdateBasic>()
                     .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeId))
                     .ForMember(c => c.EmployeeCode, c => c.MapFrom(m => m.EmployeeCode))
                     .ForMember(c => c.EmployeeName, c => c.MapFrom(m => m.Name))
                     .ForMember(c => c.ExistingBranch, c => c.MapFrom(s => s.Branch.BranchName))
                     .ForMember(c => c.ExistingDesg, c => c.MapFrom(s => s.Designation.DesignationName))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblMstEmployee, Model.UpdateBasic>(BrDesgObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<Model.UpdateBasic> GetCurrentDesignationBranch(string EmployeeCode)
        {
            log.Info($"UpdateBasicService/GetCurrentDesignationBranch");
            try
            {
                var getCurrentDesig = genericRepo.Get<DTOModel.tblMstEmployee>(em => em.EmployeeCode == EmployeeCode && !em.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.UpdateBasic>()
                     .ForMember(d => d.ExistingDesg, o => o.MapFrom(s => s.Designation.DesignationName))
                      .ForMember(d => d.ExistingBranch, o => o.MapFrom(s => s.Branch.BranchName))
                     .ForAllOtherMembers(d => d.Ignore());
                });
                return Mapper.Map<List<Model.UpdateBasic>>(getCurrentDesig);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool UpdateBranchDesignation(Model.UpdateBasic editUpdateBranchDesignation)
        {
            log.Info($"UpdateBasicService/UpdateBranchDesignation");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.tblMstEmployee>(editUpdateBranchDesignation.EmployeeId);

                dtoObj.DesignationID = editUpdateBranchDesignation.NewDesg > 0 ? editUpdateBranchDesignation.NewDesg : dtoObj.DesignationID;
                dtoObj.BranchID = editUpdateBranchDesignation.NewBranch > 0 ? editUpdateBranchDesignation.NewBranch : dtoObj.BranchID;
                dtoObj.UpdatedBy = editUpdateBranchDesignation.UpdatedBy;
                dtoObj.UpdatedOn = editUpdateBranchDesignation.UpdatedOn;
                genericRepo.Update<DTOModel.tblMstEmployee>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        #endregion
    }
}
