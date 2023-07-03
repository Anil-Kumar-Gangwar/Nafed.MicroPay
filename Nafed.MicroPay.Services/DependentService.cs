using AutoMapper;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Services
{
    public class DependentService : BaseService, IDependentService
    {
        private readonly IGenericRepository genericRepo;
        public DependentService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.EmployeeDependent> GetDependentList(int? employeeID, int? branchID = null)
        {
            log.Info($"DependentService/GetDependentList");
            try
            {
                var result = genericRepo.Get<DTOModel.EmployeeDependent>(x => !x.tblMstEmployee.DOLeaveOrg.HasValue && !x.IsDeleted &&
                (employeeID.HasValue ? x.EmployeeId == employeeID.Value : (1 > 0))
                && (branchID == 0 || branchID==null ? (1 > 0) : x.tblMstEmployee.BranchID == branchID)
                ).OrderBy(x => x.tblMstEmployee.BranchID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeDependent, Model.EmployeeDependent>()
                    .ForMember(c => c.EmpDependentID, c => c.MapFrom(s => s.EmpDependentID))
                    .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmpCode))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.DependentCode, c => c.MapFrom(s => s.DependentCode))
                    .ForMember(c => c.DependentName, c => c.MapFrom(s => s.DependentName))
                    .ForMember(c => c.GenderID, c => c.MapFrom(s => s.GenderID))
                    .ForMember(c => c.DOB, c => c.MapFrom(s => s.DOB))
                    .ForMember(c => c.RelationID, c => c.MapFrom(s => s.RelationID))
                    .ForMember(c => c.Handicapped, c => c.MapFrom(s => s.Handicapped))
                    .ForMember(c => c.Gender, c => c.MapFrom(s => s.GenderID != null ? s.Gender.Name.ToString() : null))
                    .ForMember(c => c.Relation, c => c.MapFrom(s => s.RelationID != null ? s.Relation.RelationName.ToString() : null))
                    .ForMember(c => c.PFNominee, c => c.MapFrom(s => s.PFNominee))
                    .ForMember(c => c.PFDistribution, c => c.MapFrom(s => s.PFDistribution))
                    .ForMember(c => c.Employee, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.EPSNominee, c => c.MapFrom(s => s.EPSNominee))
                    .ForMember(c => c.Address, c => c.MapFrom(s => s.Address))
                    .ForMember(c => c.Branch, c => c.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var listOfDependent = Mapper.Map<List<Model.EmployeeDependent>>(result);
                return listOfDependent.OrderBy(x => x.Branch).ToList();
                //return listOfDependent.ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertDependentDetails(Model.EmployeeDependent employeeDependent)
        {
            log.Info($"DependentService/InsertDependentDetails");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmployeeDependent, DTOModel.EmployeeDependent>()
                    .ForMember(c => c.EmpCode, c => c.MapFrom(m => m.EmpCode))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeId))
                    .ForMember(c => c.DependentCode, c => c.MapFrom(m => m.DependentCode))
                    .ForMember(c => c.DependentName, c => c.MapFrom(m => m.DependentName))
                    .ForMember(c => c.GenderID, c => c.MapFrom(m => m.GenderID))
                    .ForMember(c => c.DOB, c => c.MapFrom(m => m.DOB))
                    .ForMember(c => c.RelationID, c => c.MapFrom(m => m.RelationID))
                    .ForMember(c => c.Handicapped, c => c.MapFrom(m => m.Handicapped))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(c => c.PFNominee, c => c.MapFrom(m => m.PFNominee))
                    .ForMember(c => c.PFDistribution, c => c.MapFrom(m => m.PFDistribution))
                    .ForMember(c => c.EPSNominee, c => c.MapFrom(m => m.EPSNominee))
                    .ForMember(c => c.Address, c => c.MapFrom(s => s.Address))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoDependent = Mapper.Map<DTOModel.EmployeeDependent>(employeeDependent);
                genericRepo.Insert<DTOModel.EmployeeDependent>(dtoDependent);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.EmployeeDependent GetDependentByID(int empDependentID)
        {
            log.Info($"DependentService/GetDependentByID/{empDependentID}");
            try
            {
                var dependentObj = genericRepo.GetByID<DTOModel.EmployeeDependent>(empDependentID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeDependent, Model.EmployeeDependent>()
                    .ForMember(c => c.EmpDependentID, c => c.MapFrom(s => s.EmpDependentID))
                    .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmpCode))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.DependentCode, c => c.MapFrom(s => s.DependentCode))
                    .ForMember(c => c.DependentName, c => c.MapFrom(s => s.DependentName))
                    .ForMember(c => c.GenderID, c => c.MapFrom(s => s.GenderID))
                    .ForMember(c => c.DOB, c => c.MapFrom(s => s.DOB))
                    .ForMember(c => c.RelationID, c => c.MapFrom(s => s.RelationID))
                    .ForMember(c => c.Handicapped, c => c.MapFrom(s => s.Handicapped))
                    .ForMember(c => c.PFNominee, c => c.MapFrom(s => s.PFNominee))
                    .ForMember(c => c.PFDistribution, c => c.MapFrom(s => s.PFDistribution))
                    .ForMember(c => c.EPSNominee, c => c.MapFrom(s => s.EPSNominee))
                    .ForMember(c => c.Address, c => c.MapFrom(s => s.Address))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.EmployeeDependent, Model.EmployeeDependent>(dependentObj);
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateDependentDetails(Model.EmployeeDependent editDependent)
        {
            log.Info($"DependentService/UpdateDependentDetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.EmployeeDependent>(editDependent.EmpDependentID);
                dtoObj.EmpCode = editDependent.EmpCode;
                dtoObj.EmployeeId = editDependent.EmployeeId;
                dtoObj.DependentCode = editDependent.DependentCode;
                dtoObj.DependentName = editDependent.DependentName;
                dtoObj.GenderID = editDependent.GenderID;
                dtoObj.DOB = editDependent.DOB;
                dtoObj.RelationID = editDependent.RelationID;
                dtoObj.Handicapped = editDependent.Handicapped;
                dtoObj.UpdatedBy = editDependent.UpdatedBy;
                dtoObj.UpdatedOn = editDependent.UpdatedOn;
                dtoObj.PFNominee = editDependent.PFNominee;
                dtoObj.PFDistribution = editDependent.PFDistribution;
                dtoObj.EPSNominee = editDependent.EPSNominee;
                dtoObj.Address = editDependent.Address;
                genericRepo.Update<DTOModel.EmployeeDependent>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool DeleteDependentDetails(int empDependentID)
        {
            log.Info($"DependentService/Delete/{empDependentID}");
            bool flag = false;
            try
            {
                DTOModel.EmployeeDependent dtoDependent = new DTOModel.EmployeeDependent();
                dtoDependent = genericRepo.GetByID<DTOModel.EmployeeDependent>(empDependentID);
                dtoDependent.IsDeleted = true;
                genericRepo.Update<DTOModel.EmployeeDependent>(dtoDependent);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public int GetLastDependentCode(int employeeID)
        {
            var lastDependentCode = genericRepo.Get<DTOModel.EmployeeDependent>(x => x.EmployeeId == employeeID).FirstOrDefault();
            if (lastDependentCode != null)
                return lastDependentCode.DependentCode;
            else
                return 0;
        }

        public decimal? GetSumOfPfDistibution(int? employeeId, int? rowId)
        {
            if (rowId == null)
            {
                var sumOfPfDistibution = genericRepo.Get<DTOModel.EmployeeDependent>(x => x.EmployeeId == employeeId && !x.IsDeleted && x.PFDistribution != null).Sum(x => x.PFDistribution);
                if (sumOfPfDistibution != null)
                    return sumOfPfDistibution;
                else
                    return 0;
            }
            else
            {
                var sumOfPfDistibution = genericRepo.Get<DTOModel.EmployeeDependent>(x => x.EmployeeId == employeeId && x.EmpDependentID != rowId && !x.IsDeleted && x.PFDistribution != null).Sum(x => x.PFDistribution);
                if (sumOfPfDistibution != null)
                    return sumOfPfDistibution;
                else
                    return 0;
            }
        }
    }
}
