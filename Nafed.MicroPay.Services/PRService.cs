using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services
{
    public class PRService : BaseService, IPRService
    {
        private readonly IGenericRepository genericRepo;
        public PRService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.PR> GetPRHList(int EmployeeID)
        {
            log.Info($"GetPRHList");
            try
            {
                var result = genericRepo.Get<DTOModel.tblPropertyReturnHDR>(x => (bool)!x.IsDeleted &&
               (EmployeeID != 0 ? x.EmployeeId == EmployeeID : (1 > 0))
               );

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblPropertyReturnHDR, Model.PR>()
                    .ForMember(c => c.PRID, c => c.MapFrom(s => s.PRID))
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                     .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                     .ForMember(c => c.Duedate, c => c.MapFrom(s => s.Duedate))
                     .ForMember(c => c.BranchName, c => c.MapFrom(s => s.Branch.BranchName))
                   .ForMember(c => c.DepartmentName, c => c.MapFrom(s => s.Department.DepartmentName))
                   .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.Designation.DesignationName))
                   .ForMember(c => c.NotApplicable, c => c.MapFrom(s => s.NotApplicable))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listPR = Mapper.Map<List<Model.PR>>(result);
                return listPR.OrderBy(x => x.PRID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<Model.PR> GetPRGVList(int EmployeeID, int? PRID, int? Year)
        {
            log.Info($"GetPRList");
            try
            {
                var result = genericRepo.Get<DTOModel.tblPropertyReturn>(x => (bool)!x.IsDeleted &&
               (EmployeeID != 0 ? x.EmployeeId == EmployeeID : (1 > 0)) &&
                (PRID != 0 ? x.PRID == PRID : (1 > 0))
                && (Year != 0 ? x.Year == Year : (1 > 0))
               );
                //var result = genericRepo.Get<DTOModel.tblPropertyReturnHDR>(x => (bool)!x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblPropertyReturn, Model.PR>()
                     .ForMember(c => c.Counter, c => c.MapFrom(s => s.Counter))
                    .ForMember(c => c.PRID, c => c.MapFrom(s => s.PRID))
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                     .ForMember(c => c.StatusID, c => c.MapFrom(s => s.tblPropertyReturnHDR.StatusID))
                    .ForMember(c => c.PropertySituated, c => c.MapFrom(s => s.PropertySituated))
                    .ForMember(c => c.PropertyType, c => c.MapFrom(s => s.PropertyType))
                    .ForMember(c => c.PropertyDetails, c => c.MapFrom(s => s.PropertyDetails))
                    .ForMember(c => c.PresentValue, c => c.MapFrom(s => s.PresentValue))
                    .ForMember(c => c.SelfProperty, c => c.MapFrom(s => s.SelfProperty))
                    .ForMember(c => c.PropertyOwner, c => c.MapFrom(s => s.PropertyOwner))
                    .ForMember(c => c.RelationID, c => c.MapFrom(s => s.RelationID))
                    .ForMember(c => c.AcquiredTypeID, c => c.MapFrom(s => s.AcquiredTypeID))
                    .ForMember(c => c.AcquisitionDate, c => c.MapFrom(s => s.AcquisitionDate))
                    .ForMember(c => c.AcquiredPerson, c => c.MapFrom(s => s.AcquiredPerson))
                    .ForMember(c => c.AcquiredDetails, c => c.MapFrom(s => s.AcquiredDetails))
                    .ForMember(c => c.PropertyIncome, c => c.MapFrom(s => s.PropertyIncome))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listPR = Mapper.Map<List<Model.PR>>(result);
                return listPR.OrderBy(x => x.PRID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> GetAllEmployee()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && x.DOLeaveOrg == null);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeCode + "-" + s.Name));
                });
                var employeedetail = Mapper.Map<List<Model.SelectListModel>>(result);
                return employeedetail.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Model.PR GetPRList(int CounterID)
        {
            log.Info($"GetPRList {CounterID}");
            try
            {
                var PRObj = genericRepo.GetByID<DTOModel.tblPropertyReturn>(CounterID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblPropertyReturn, Model.PR>()
                     .ForMember(c => c.PRID, c => c.MapFrom(s => s.PRID))
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.PropertySituated, c => c.MapFrom(s => s.PropertySituated))
                    .ForMember(c => c.PropertyType, c => c.MapFrom(s => s.PropertyType))
                    .ForMember(c => c.PropertyDetails, c => c.MapFrom(s => s.PropertyDetails))
                    .ForMember(c => c.PresentValue, c => c.MapFrom(s => s.PresentValue))
                    .ForMember(c => c.SelfProperty, c => c.MapFrom(s => s.SelfProperty))
                    .ForMember(c => c.PropertyOwner, c => c.MapFrom(s => s.PropertyOwner))
                    .ForMember(c => c.RelationID, c => c.MapFrom(s => s.RelationID))
                    .ForMember(c => c.AcquiredTypeID, c => c.MapFrom(s => s.AcquiredTypeID))
                    .ForMember(c => c.AcquisitionDate, c => c.MapFrom(s => s.AcquisitionDate))
                    .ForMember(c => c.AcquiredPerson, c => c.MapFrom(s => s.AcquiredPerson))
                    .ForMember(c => c.AcquiredDetails, c => c.MapFrom(s => s.AcquiredDetails))
                    .ForMember(c => c.PropertyIncome, c => c.MapFrom(s => s.PropertyIncome))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblPropertyReturn, Model.PR>(PRObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int InsertPRHeaders(Model.PR createPR)
        {
            log.Info($"InsertPRHeaders");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.PR, DTOModel.tblPropertyReturnHDR>()
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => 1))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoPR = Mapper.Map<DTOModel.tblPropertyReturnHDR>(createPR);
                genericRepo.Insert<DTOModel.tblPropertyReturnHDR>(dtoPR);
                return dtoPR.PRID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool InsertPR(Model.PR createPR, int PRID)
        {
            log.Info($"InsertPR");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.PR, DTOModel.tblPropertyReturn>()
                     .ForMember(c => c.PRID, c => c.MapFrom(s => PRID))
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.PropertySituated, c => c.MapFrom(s => s.PropertySituated))
                     .ForMember(c => c.PropertyType, c => c.MapFrom(s => s.PropertyType))
                    .ForMember(c => c.PropertyDetails, c => c.MapFrom(s => s.PropertyDetails))
                    .ForMember(c => c.PresentValue, c => c.MapFrom(s => s.PresentValue))
                    .ForMember(c => c.SelfProperty, c => c.MapFrom(s => s.SelfProperty))
                    .ForMember(c => c.PropertyOwner, c => c.MapFrom(s => s.PropertyOwner))
                     .ForMember(c => c.RelationID, c => c.MapFrom(s => s.RelationID))
                    .ForMember(c => c.AcquiredTypeID, c => c.MapFrom(s => s.AcquiredTypeID))
                    .ForMember(c => c.AcquisitionDate, c => c.MapFrom(s => s.AcquisitionDate))
                     .ForMember(c => c.AcquiredPerson, c => c.MapFrom(s => s.AcquiredPerson))
                    .ForMember(c => c.AcquiredDetails, c => c.MapFrom(s => s.AcquiredDetails))
                     .ForMember(c => c.PropertyIncome, c => c.MapFrom(s => s.PropertyIncome))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoPR = Mapper.Map<DTOModel.tblPropertyReturn>(createPR);
                genericRepo.Insert<DTOModel.tblPropertyReturn>(dtoPR);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdatePR(Model.PR editPR)
        {
            log.Info($"PRService/UpdatePR");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.tblPropertyReturn>(editPR.Counter);
                dtoObj.PropertySituated = editPR.PropertySituated;
                dtoObj.PropertyType = editPR.PropertyType;
                dtoObj.PropertyDetails = editPR.PropertyDetails;
                dtoObj.PresentValue = editPR.PresentValue;
                dtoObj.SelfProperty = editPR.SelfProperty;
                dtoObj.PropertyOwner = editPR.PropertyOwner;
                dtoObj.RelationID = editPR.RelationID;
                dtoObj.AcquiredTypeID = editPR.AcquiredTypeID;
                dtoObj.AcquisitionDate = editPR.AcquisitionDate;
                dtoObj.AcquiredPerson = editPR.AcquiredPerson;
                dtoObj.AcquiredDetails = editPR.AcquiredDetails;
                dtoObj.PropertyIncome = editPR.PropertyIncome;
                dtoObj.Remarks = editPR.Remarks;
                dtoObj.UpdatedBy = editPR.UpdatedBy;
                dtoObj.UpdatedOn = editPR.UpdatedOn;

                genericRepo.Update<DTOModel.tblPropertyReturn>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdatePRStatus(int PRID, bool isApplicable)
        {
            log.Info($"PRService/UpdatePRStatus");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.tblPropertyReturnHDR>(PRID);
                if (dtoObj != null)
                {
                    if (!dtoObj.E_Basic.HasValue)
                    {
                        dtoObj.E_Basic = (decimal)genericRepo.Get<Nafed.MicroPay.Data.Models.TblMstEmployeeSalary>(x => x.EmployeeID == dtoObj.EmployeeId).FirstOrDefault().E_Basic;
                    }
                }
                dtoObj.StatusID = "1";
                dtoObj.UpdatedBy = 1;
                dtoObj.NotApplicable = isApplicable;
                dtoObj.UpdatedOn = DateTime.Now;
                //   dtoObj.E_Basic=
                genericRepo.Update<DTOModel.tblPropertyReturnHDR>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool DeletePR(int Counter)
        {
            log.Info($"PRService/Delete/{Counter}");
            bool flag = false;
            try
            {
                DTOModel.tblPropertyReturn dtoPR = new DTOModel.tblPropertyReturn();
                dtoPR = genericRepo.GetByID<DTOModel.tblPropertyReturn>(Counter);
                dtoPR.IsDeleted = true;
                genericRepo.Update<DTOModel.tblPropertyReturn>(dtoPR);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public List<Model.PR> GetPRViewList(int? Year)
        {
            log.Info($"GetPRViewList");
            try
            {
                var result = genericRepo.Get<DTOModel.tblPropertyReturnHDR>(x => (bool)!x.IsDeleted &&
                // (empcode != "" ? x.tblMstEmployee.EmployeeCode == empcode : (1 > 0)) &&
                //(name !="" ? x.tblMstEmployee.Name == name : (1 > 0)) &&
                 (Year != 0 ? x.Year == Year : (1 > 0))
               );
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblPropertyReturnHDR, Model.PR>()
                    .ForMember(c => c.PRID, c => c.MapFrom(s => s.PRID))
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                     .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                   .ForMember(c => c.Duedate, c => c.MapFrom(s => s.Duedate))
                   .ForMember(c => c.BranchName, c => c.MapFrom(s => s.Branch.BranchName))
                   .ForMember(c => c.DepartmentName, c => c.MapFrom(s => s.Department.DepartmentName))
                   .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.Designation.DesignationName))
                   .ForMember(c => c.NotApplicable, c => c.MapFrom(s => s.NotApplicable))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listPR = Mapper.Map<List<Model.PR>>(result);
                return listPR.OrderBy(x => x.Employeecode).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool getemployeeallDetails(int designationID, int employeeID, out PR PRdetail, int? year = null)
        {
            bool flag = false;
            try
            {
                PRdetail = new PR();

                if (designationID == 0)
                {
                    designationID = genericRepo.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>(employeeID).DesignationID;

                }

                var submitdate = genericRepo.Get<DTOModel.tblPropertyReturnHDR>(x => x.EmployeeId == employeeID && x.Year == year.Value).FirstOrDefault();
                if (submitdate != null)
                {
                    PRdetail.UpdatedOn = submitdate.UpdatedOn;
                    PRdetail.E_Basic = submitdate.E_Basic ?? 0;
                }
                PRdetail.DesignationName = genericRepo.GetByID<Nafed.MicroPay.Data.Models.Designation>(designationID).DesignationName;
                //  var ebasic = genericRepo.Get<Nafed.MicroPay.Data.Models.tblFinalMonthlySalary>(x => x.EmployeeID == employeeID && x.SalYear==year).FirstOrDefault();

                PRdetail.Employeename = genericRepo.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>(employeeID).Name;
                int branchID = genericRepo.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>(employeeID).BranchID;
                PRdetail.BranchName = genericRepo.GetByID<Nafed.MicroPay.Data.Models.Branch>(branchID).BranchName;
                if (PRdetail.E_Basic == 0)
                {
                    PRdetail.E_Basic = (decimal)genericRepo.Get<Nafed.MicroPay.Data.Models.TblMstEmployeeSalary>(x => x.EmployeeID == employeeID).FirstOrDefault().E_Basic;
                }
                flag = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flag;
        }

        public Model.PR GetPropertyReturn(int year, int employeeID)
        {
            log.Info("$PRService/GetPropertyReturn");
            try
            {
                var prHdr = genericRepo.Get<DTOModel.tblPropertyReturnHDR>(x => (bool)!x.IsDeleted
                && x.Year == year && x.EmployeeId == employeeID).FirstOrDefault();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblPropertyReturnHDR, Model.PR>()
                    .ForMember(c => c.PRID, c => c.MapFrom(s => s.PRID))
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.Duedate, c => c.MapFrom(s => s.Duedate))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                return Mapper.Map<Model.PR>(prHdr);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
