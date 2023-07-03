using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
namespace Nafed.MicroPay.Services
{
    public class LoanApplicationService : BaseService, ILoanApplicationService
    {
        private readonly ILoanApplicationRepository loanApplicationRepo;
        private readonly IGenericRepository genericRepo;
        public LoanApplicationService(ILoanApplicationRepository loanApplicationRepo, IGenericRepository genericRepo)
        {
            this.loanApplicationRepo = loanApplicationRepo;
            this.genericRepo = genericRepo;
        }
        public LoanApplication GetEmployeeDtl(int EmpID)
        {
            log.Info($"LoanApplicationService/GetEmployeeDtl/EmpID={EmpID}");
            try
            {
                var geTEmployeeDtl = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == EmpID && !x.IsDeleted).FirstOrDefault();
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.tblMstEmployee, Model.LoanApplication>()
                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Dateofjoining, o => o.MapFrom(s => s.Pr_Loc_DOJ))
                .ForMember(d => d.PFNo, o => o.MapFrom(s => s.PFNO))
                .ForAllOtherMembers(d => d.Ignore())
               );

                var dtoLoanAllication = Mapper.Map<Model.LoanApplication>(geTEmployeeDtl);
                if (dtoLoanAllication != null)
                    return dtoLoanAllication;
                else
                    return new LoanApplication();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<LoanApplication> GetLoanApplicationList(int EmpID, int? LoanApp)
        {
            log.Info($"LoanApplicationService/GetLoanApplicationList/EmpID={EmpID}/LoanApplicationNo={LoanApp}");
            try
            {
                var getLoanApplication = loanApplicationRepo.GetLoanApplicationList(EmpID, LoanApp);
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetLoanApplicationList_Result, Model.LoanApplication>()

               );

                var dtoLoanAllication = Mapper.Map<List<Model.LoanApplication>>(getLoanApplication);
                if (dtoLoanAllication != null && dtoLoanAllication.Count > 0)
                    return dtoLoanAllication;
                else
                    return new List<LoanApplication>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<LoanApplication> GetLoanApplicationDetails(int? EmpID, int? statusID, DateTime? fromDate, DateTime? toDate)
        {
            log.Info($"LoanApplicationService/GetLoanApplicationDetails/EmpID={EmpID}/statusID={statusID}");
            try
            {
                var getLoanApplication = loanApplicationRepo.GetLoanApplicationDetails(EmpID, statusID,fromDate,toDate);
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetLoanApplicationDetails_Result, Model.LoanApplication>()
                .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.ApplicationStatusID, o => o.MapFrom(s => s.ApplicationStatusID))
                .ForMember(d => d.Remarks, o => o.MapFrom(s => s.Remark))
               );

                var dtoLoanAllication = Mapper.Map<List<Model.LoanApplication>>(getLoanApplication);
                if (dtoLoanAllication != null && dtoLoanAllication.Count > 0)
                    return dtoLoanAllication;
                else
                    return new List<LoanApplication>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
      
        public bool InsertUpdateLoanApplication(LoanApplication loanApp)
        {
            log.Info($"InsertUpdateLoanApplication/InsertLoanApplication");
            try
            {
                if (loanApp.ApplicationID > 0)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<Model.LoanApplication, DTOModel.LoanApplication>()
                    .ForMember(d => d.ApplicationID, o => o.MapFrom(s => s.ApplicationID))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.LoanAmount, o => o.MapFrom(s => s.LoanAmount))
                    .ForMember(d => d.LoanReason, o => o.MapFrom(s => s.LoanReason))
                    .ForMember(d => d.Ceremony, o => o.MapFrom(s => s.Ceremony))
                    .ForMember(d => d.CeremonyDate, o => o.MapFrom(s => s.CeremonyDate))
                    .ForMember(d => d.CeremonyPlace, o => o.MapFrom(s => s.CeremonyPlace))
                    .ForMember(d => d.DeathPlace, o => o.MapFrom(s => s.DeathPlace))
                    .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.UpdateBy, o => o.MapFrom(s => s.UpdateBy))
                    .ForMember(d => d.UpdateOn, o => o.MapFrom(s => s.UpdateOn))
                    .ForAllOtherMembers(d => d.Ignore())
                );

                    var dtoLoanAllication = Mapper.Map<DTOModel.LoanApplication>(loanApp);
                    if (dtoLoanAllication != null)
                    {
                        genericRepo.Update<DTOModel.LoanApplication>(dtoLoanAllication);
                        return true;
                    }
                    else
                        return false;
                }
                else
                {

                    Mapper.Initialize(cfg => cfg.CreateMap<Model.LoanApplication, DTOModel.LoanApplication>()
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.LoanAmount, o => o.MapFrom(s => s.LoanAmount))
                    .ForMember(d => d.LoanReason, o => o.MapFrom(s => s.LoanReason))
                    .ForMember(d => d.Ceremony, o => o.MapFrom(s => s.Ceremony))
                    .ForMember(d => d.CeremonyDate, o => o.MapFrom(s => s.CeremonyDate))
                    .ForMember(d => d.CeremonyPlace, o => o.MapFrom(s => s.CeremonyPlace))
                    .ForMember(d => d.DeathPlace, o => o.MapFrom(s => s.DeathPlace))
                    .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForAllOtherMembers(d => d.Ignore())
                );

                    var dtoLoanAllication = Mapper.Map<DTOModel.LoanApplication>(loanApp);
                    if (dtoLoanAllication != null)
                    {
                        genericRepo.Insert<DTOModel.LoanApplication>(dtoLoanAllication);
                        if (loanApp._ProcessWorkFlow.EmployeeID > 0)
                        {
                            loanApp._ProcessWorkFlow.ReferenceID = dtoLoanAllication.ApplicationID;
                            AddProcessWorkFlow(loanApp._ProcessWorkFlow);
                        }
                        return true;
                        
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool InsertLoanApplicationHistory(LoanApplication loanApp)
        {
            log.Info($"InsertUpdateLoanApplication/InsertLoanApplicationHistory");
            try
            {                
                    Mapper.Initialize(cfg => cfg.CreateMap<Model.LoanApplication, DTOModel.LoanApplicationHistory>()
                    .ForMember(d => d.LoanApplicationID, o => o.MapFrom(s => s.ApplicationID))
                    .ForMember(d => d.ApplicationStatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remarks))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))                   
                    .ForAllOtherMembers(d => d.Ignore())
                );

                    var dtoLoanAllication = Mapper.Map<DTOModel.LoanApplicationHistory>(loanApp);
                    if (dtoLoanAllication != null)
                    {
                        genericRepo.Insert<DTOModel.LoanApplicationHistory>(dtoLoanAllication);
                        if (dtoLoanAllication.LoanAppHistoryID > 0)
                        {
                            var getLoanApplication = genericRepo.Get<DTOModel.LoanApplication>(x => x.ApplicationID == loanApp.ApplicationID).FirstOrDefault();
                            if (getLoanApplication != null)
                            {
                                getLoanApplication.StatusID = loanApp.StatusID;
                                genericRepo.Update(getLoanApplication);

                            if (loanApp._ProcessWorkFlow.EmployeeID > 0)
                            {
                                loanApp._ProcessWorkFlow.ReferenceID = loanApp.ApplicationID;
                                AddProcessWorkFlow(loanApp._ProcessWorkFlow);
                            }
                            return true;
                            }
                            else
                                return false;
                        }
                        else
                            return false;
                       
                    }
                    else
                        return false;              
               
                
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<LoanApplication> GetLoanApplicationForApproval(int loggedInEmpID)
        {
            log.Info($"AppraisalFormService/GetLoanApplicationForApproval");
            try
            {

                var loanData = genericRepo.GetIQueryable<DTOModel.LoanApplication>
                    (x => x.tblMstEmployee.EmployeeProcessApprovals
                       .Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LoanApplication 
                       && y.ToDate == null && (y.ReportingTo == loggedInEmpID))
                );
             
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.LoanApplication, Model.LoanApplication>()
                     .ForMember(d => d.ApplicationID, o => o.MapFrom(s => s.ApplicationID))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.LoanAmount, o => o.MapFrom(s => s.LoanAmount))
                    .ForMember(d => d.LoanReason, o => o.MapFrom(s => s.LoanReason))
                    .ForMember(d => d.Ceremony, o => o.MapFrom(s => s.Ceremony))
                    .ForMember(d => d.CeremonyDate, o => o.MapFrom(s => s.CeremonyDate))
                    .ForMember(d => d.CeremonyPlace, o => o.MapFrom(s => s.CeremonyPlace))
                    .ForMember(d => d.DeathPlace, o => o.MapFrom(s => s.DeathPlace))
                    .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))                    
                    .ForMember(d => d.approvalSetting, o => o.MapFrom(s =>
                     s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y =>
                     y.ProcessID == (int)Common.WorkFlowProcess.LoanApplication && y.ToDate == null && y.EmployeeID == s.EmployeeID)))
                   
                    .ForAllOtherMembers(d => d.Ignore());

                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();

                });
                return Mapper.Map<List<Model.LoanApplication>>(loanData.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
