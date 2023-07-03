using AutoMapper;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Services
{
    public class InsuranceService : BaseService, IInsuranceService
    {
        private readonly IGenericRepository genericRepo;
        public InsuranceService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.Insurance> GetInsuranceList(int? employeeID, int? branchID = null,bool? dependentMedical=null)
        {
            log.Info($"InsuranceService/GetInsuranceList");
            try
            {
                var result = genericRepo.Get<DTOModel.Insurance>(x => (branchID.HasValue ? x.tblMstEmployee.BranchID == branchID.Value : (1 > 0)) && (employeeID.HasValue ? x.EmployeeId == employeeID : (1 > 0)) && (dependentMedical.HasValue?x.DependentMedicalPolicy==dependentMedical:(1>0)) && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Insurance, Model.Insurance>()
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode + " - " + s.tblMstEmployee.Name));
                });
                var listOfInsurance = Mapper.Map<List<Model.Insurance>>(result);
                return listOfInsurance.ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public bool InsertInsuranceDetails(Model.Insurance Insurance)
        {
            log.Info($"InsuranceService/InsertInsuranceDetails");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Insurance, DTOModel.Insurance>();
                });
                var dtoInsurance = Mapper.Map<DTOModel.Insurance>(Insurance);
                genericRepo.Insert<DTOModel.Insurance>(dtoInsurance);
                if (dtoInsurance.InsuranceId > 0)
                {
                    #region Child Table
                    if (Insurance.InsuranceDependenceList != null && Insurance.InsuranceDependenceList.Count() > 0)
                    {
                        Mapper.Initialize(cfg =>
                        cfg.CreateMap<Model.InsuranceDependent, DTOModel.InsuranceDependent>()
                        .ForMember(d => d.InsuranceId, o => o.UseValue(dtoInsurance.InsuranceId))
                         .ForMember(d => d.CreatedBy, o => o.UseValue(dtoInsurance.CreatedBy))
                          .ForMember(d => d.CreatedOn, o => o.UseValue(dtoInsurance.CreatedOn))
                        );
                        var dtoInsDependentlist = Mapper.Map<List<DTOModel.InsuranceDependent>>(Insurance.InsuranceDependenceList);
                        genericRepo.AddMultipleEntity(dtoInsDependentlist);
                    }
                    #endregion
                }

                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        public List<Model.InsuranceDependent> GetDependentList(int employeeId)
        {
            log.Info($"InsuranceService/GetDependentList/{employeeId}");
            try
            {
                var dependentList = genericRepo.Get<DTOModel.EmployeeDependent>(x => x.EmployeeId == employeeId && !x.IsDeleted).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeDependent, Model.InsuranceDependent>()
                    .ForMember(d => d.DependentId, o => o.MapFrom(s => s.EmpDependentID))
                    .ForMember(d => d.DependentName, o => o.MapFrom(s => s.DependentName))
                     .ForMember(d => d.Relation, o => o.MapFrom(s => s.Relation.RelationName))
                     .ForMember(d => d.Gender, o => o.MapFrom(s => s.Gender.Name))
                      .ForMember(d => d.Age, o => o.MapFrom(s => s.DOB.Value.CalculateAge()))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var dtoDependentList = Mapper.Map<List<Model.InsuranceDependent>>(dependentList);
                return dtoDependentList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public decimal? GetFmilyAssuredByDesignationId(int employeeId, out string designation)
        {
            var empDesgId = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == employeeId).FirstOrDefault().DesignationID;
            var getFamilyAssured = genericRepo.Get<DTOModel.Designation>(x => x.DesignationID == empDesgId).FirstOrDefault();
            if (getFamilyAssured != null)
            {
                designation = getFamilyAssured.DesignationName;
                return getFamilyAssured.FamilyAssured;
            }
            else { designation = string.Empty; return null; }

        }
        public Model.Insurance GetInsuranceByID(int employeeid, int insuranceId)
        {
            log.Info($"InsuranceService/GetInsuranceByID/{insuranceId}");
            try
            {
                var InsuranceObj = genericRepo.Get<DTOModel.Insurance>(x => x.EmployeeId == employeeid && x.InsuranceId == insuranceId).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Insurance, Model.Insurance>()
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode + " - " + s.tblMstEmployee.Name));

                });
                var obj = Mapper.Map<DTOModel.Insurance, Model.Insurance>(InsuranceObj);

                var InsuranceDep = genericRepo.Get<DTOModel.InsuranceDependent>(x => x.InsuranceId == insuranceId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.InsuranceDependent, Model.InsuranceDependent>();
                });
                var dtoInsuranceDep = Mapper.Map<List<Model.InsuranceDependent>>(InsuranceDep);

                var dtoDependentList = GetDependentList(employeeid);
                foreach (var item in dtoDependentList)
                {
                    item.PolicyJoinDate = dtoInsuranceDep.Where(x => x.DependentId == item.DependentId).FirstOrDefault() != null ? dtoInsuranceDep.Where(x => x.DependentId == item.DependentId).FirstOrDefault().PolicyJoinDate : null;
                    item.PolicyExpDate = dtoInsuranceDep.Where(x => x.DependentId == item.DependentId).FirstOrDefault() != null ? dtoInsuranceDep.Where(x => x.DependentId == item.DependentId).FirstOrDefault().PolicyExpDate : null;
                    item.Reason = dtoInsuranceDep.Where(x => x.DependentId == item.DependentId).FirstOrDefault() != null ? dtoInsuranceDep.Where(x => x.DependentId == item.DependentId).FirstOrDefault().Reason : null;
                    item.IsApplicable = dtoInsuranceDep.Where(x => x.DependentId == item.DependentId).FirstOrDefault() != null ? true : false;
                    item.InsuranceDepId = dtoInsuranceDep.Where(x => x.DependentId == item.DependentId).FirstOrDefault() != null ? dtoInsuranceDep.Where(x => x.DependentId == item.DependentId).FirstOrDefault().InsuranceDepId : 0;
                }
                //dtoDependentList.ForEach(c =>
                //{
                //    var depId = c.DependentId;
                //    c.PolicyJoinDate = dtoInsuranceDep.Where(x => x.DependentId == depId).FirstOrDefault().PolicyJoinDate;
                //    c.PolicyExpDate = dtoInsuranceDep.Where(x => x.DependentId == depId).FirstOrDefault().PolicyExpDate;
                //    c.Reason = dtoInsuranceDep.Where(x => x.DependentId == depId).FirstOrDefault().Reason;
                //});
                obj.InsuranceDependenceList = dtoDependentList;
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateInsuranceDetails(Model.Insurance editInsurance)
        {
            log.Info($"InsuranceService/UpdateInsuranceDetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.Get<DTOModel.Insurance>(x => x.EmployeeId == editInsurance.EmployeeId && x.InsuranceId == editInsurance.InsuranceId).FirstOrDefault();

                dtoObj.PolicyAvail = editInsurance.PolicyAvail;
                dtoObj.PolicyJoinDate = (DateTime)editInsurance.PolicyJoinDate;
                dtoObj.UpdatedBy = editInsurance.UpdatedBy;
                dtoObj.Updatedon = editInsurance.Updatedon;
                dtoObj.FamilyAssuredSum = editInsurance.FamilyAssuredSum;
                dtoObj.PolicyExpDate = (DateTime)editInsurance.PolicyExpDate;
                dtoObj.DependentMedicalPolicy = editInsurance.DependentMedicalPolicy;
                genericRepo.Update<DTOModel.Insurance>(dtoObj);

                if (dtoObj.InsuranceId > 0)
                {

                    #region Child Table
                    if (editInsurance.InsuranceDependenceList != null && editInsurance.InsuranceDependenceList.Count() > 0)
                    {
                        Mapper.Initialize(cfg =>
                        cfg.CreateMap<Model.InsuranceDependent, DTOModel.InsuranceDependent>()
                        .ForMember(d => d.InsuranceId, o => o.UseValue(dtoObj.InsuranceId))
                         .ForMember(d => d.CreatedBy, o => o.UseValue(editInsurance.UpdatedBy))
                          .ForMember(d => d.CreatedOn, o => o.UseValue(editInsurance.Updatedon))
                        );
                        var dtoInsDependentlist = Mapper.Map<List<DTOModel.InsuranceDependent>>(editInsurance.InsuranceDependenceList);

                        var deleteDependent = genericRepo.Get<DTOModel.InsuranceDependent>(x => x.InsuranceId == editInsurance.InsuranceId).ToList();
                        genericRepo.DeleteAll(deleteDependent);
                        genericRepo.AddMultipleEntity(dtoInsDependentlist);
                    }
                    #endregion
                }

                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool DeleteInsuranceDetails(int empInsuranceID)
        {
            log.Info($"InsuranceService/Delete/{empInsuranceID}");
            bool flag = false;
            try
            {
                DTOModel.Insurance dtoInsurance = new DTOModel.Insurance();
                dtoInsurance = genericRepo.GetByID<DTOModel.Insurance>(empInsuranceID);
                dtoInsurance.IsDeleted = true;
                genericRepo.Update<DTOModel.Insurance>(dtoInsurance);
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
