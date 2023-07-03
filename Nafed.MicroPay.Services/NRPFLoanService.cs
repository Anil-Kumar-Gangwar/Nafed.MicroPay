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
using System.Data;

namespace Nafed.MicroPay.Services
{
    public class NRPFLoanService : BaseService, INRPFLoanService
    {

        private readonly IGenericRepository genericRepo;

        public NRPFLoanService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public List<NonRefundablePFLoan> checkexistdata(int NRPFLoanID)
        {
            try
            {
                var dto = genericRepo.Get<DTOModel.NonRefundablePFLoan>(x => x.NRPFLoanID == NRPFLoanID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.NonRefundablePFLoan, Model.NonRefundablePFLoan>()
                     .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                   .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                      .ForMember(c => c.Amount_of_Advanced, c => c.MapFrom(s => s.Amount_of_Advanced))
                       .ForMember(c => c.Purpose_of_Advanced, c => c.MapFrom(s => s.Purpose_of_Advanced))
                       .ForMember(c => c.Date_of_Sanction, c => c.MapFrom(s => s.Date_of_Sanction))
                       .ForMember(c => c.LocationOfDwellingSite, c => c.MapFrom(s => s.LocationOfDwellingSite))

                       .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                        .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                        .ForMember(c => c.HBName, c => c.MapFrom(s => s.tblMstEmployee.HBName))
                        .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                        .ForMember(c => c.DepartmentName, c => c.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                        .ForMember(c => c.NamePresentOwner, c => c.MapFrom(s => s.NamePresentOwner))
                        .ForMember(c => c.AddressPresentOwner, c => c.MapFrom(s => s.AddressPresentOwner))
                        .ForMember(c => c.PresentStateofDwelling, c => c.MapFrom(s => s.PresentStateofDwelling))
                        .ForMember(c => c.DesiredModeofRemittance, c => c.MapFrom(s => s.DesiredModeofRemittance))
                         .ForMember(c => c.ListofDocuments, c => c.MapFrom(s => s.ListofDocuments))
                        .ForMember(c => c.Branchname, c => c.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var list = Mapper.Map<List<Model.NonRefundablePFLoan>>(dto);
                return list.ToList();
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.NonRefundablePFLoan> GetnonrefPFloanList(int EmployeeID)
        {
            log.Info($"GetnonrefPFloanList");
            try
            {
                List<NonRefundablePFLoan> DT = new List<NonRefundablePFLoan>();
                var dtoNRPFLoan = genericRepo.Get<DTOModel.NonRefundablePFLoan>(x => (EmployeeID != 0 ? x.EmployeeId == EmployeeID : (1 > 0))).ToList();

                var dtoHDR = genericRepo.Get<DTOModel.NRPFLoanHDR>(x => (bool)!x.IsDeleted &&
             (EmployeeID != 0 ? x.EmployeeId == EmployeeID : (1 > 0)));


                DT = (from left in dtoHDR
                      join right in dtoNRPFLoan on left.NRPFLoanID equals right.NRPFLoanID into joinedList
                      from sub in joinedList.DefaultIfEmpty()
                      select new NonRefundablePFLoan()
                      {
                          NRPFLoanID = left.NRPFLoanID,
                          EmployeeId = left.EmployeeId,
                          Employeecode = left.tblMstEmployee.EmployeeCode,
                          Employeename = left.tblMstEmployee.Name,
                          Branchname = left.tblMstEmployee.Branch.BranchName,
                          DesignationName = left.tblMstEmployee.Designation.DesignationName,
                          DepartmentName = left.tblMstEmployee.Department.DepartmentName,
                          Requestdate = left.Requestdate,
                          StatusID = left.StatusID,
                          BankAcNo = left.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == EmployeeID).BankAcNo,
                          AccountNo = left.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == EmployeeID).BankAcNo,
                          BasicPay =Convert.ToDecimal (left.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == EmployeeID).E_Basic==null?0: left.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == EmployeeID).E_Basic),
                          DA = Convert.ToDecimal(left.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == EmployeeID).E_01 == null ? 0 : left.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == EmployeeID).E_01),

                          Amount_of_Advanced = sub == null ? 0 : sub.Amount_of_Advanced
                      }).ToList();

                return DT;
               // var result = genericRepo.Get<DTOModel.NRPFLoanHDR>(x => (bool)!x.IsDeleted &&
               //(EmployeeID != 0 ? x.EmployeeId == EmployeeID : (1 > 0))
               //);
               // Mapper.Initialize(cfg =>
               // {
               //     cfg.CreateMap<DTOModel.NRPFLoanHDR, Model.NonRefundablePFLoan>()
               //     .ForMember(c => c.NRPFLoanID, c => c.MapFrom(s => s.NRPFLoanID))
               //     .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
               //     .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
               //     .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
               //     .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
               //      .ForMember(c => c.DepartmentName, c => c.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
               //    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
               //     .ForMember(d => d.BankAcNo, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == EmployeeID).BankAcNo))
               //     .ForMember(d => d.AccountNo, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == EmployeeID).BankAcNo))
               //      .ForMember(d => d.BasicPay, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == EmployeeID).E_Basic))
               //        .ForMember(d => d.DA, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == EmployeeID).D_01))
               //    .ForAllOtherMembers(c => c.Ignore());
               // });
               // /*var listPR = Mapper.Map<DTOModel.EmpPFOrgHDR, Model.EmployeePFORG>(result);*/
               // var listPR = Mapper.Map<List<Model.NonRefundablePFLoan>>(result);
               // return listPR.OrderBy(x => x.NRPFLoanID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public NonRefundablePFLoan GetNRPFLoanDetails(int ID, int NRPFLoanID, int statusID,int employeeID)
        {
            log.Info($"NRPFLoanService/GetNRPFLoanDetails/{ID}");
            try
            {
                var obj = (dynamic)null;
                var empPFObj = (dynamic)null;

                if (ID != 0)
                {
                    empPFObj = genericRepo.GetByID<DTOModel.NonRefundablePFLoan>(ID);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Data.Models.NonRefundablePFLoan, Model.NonRefundablePFLoan>()
                         .ForMember(c => c.NRPFLoanID, c => c.MapFrom(m => m.NRPFLoanID))
                        .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeId))
                        .ForMember(c => c.Amount_of_Advanced, c => c.MapFrom(m => m.Amount_of_Advanced))
                   
                        .ForMember(c => c.Purpose_of_Advanced, c => c.MapFrom(m => m.Purpose_of_Advanced))
                        .ForMember(c => c.Date_of_Sanction, c => c.MapFrom(m => m.Date_of_Sanction))
                        .ForMember(c => c.LocationOfDwellingSite, c => c.MapFrom(m => m.LocationOfDwellingSite))
                         .ForMember(c => c.NamePresentOwner, c => c.MapFrom(m => m.NamePresentOwner))
                        .ForMember(c => c.AddressPresentOwner, c => c.MapFrom(m => m.AddressPresentOwner))
                        .ForMember(c => c.PresentStateofDwelling, c => c.MapFrom(m => m.PresentStateofDwelling))
                        .ForMember(c => c.DesiredModeofRemittance, c => c.MapFrom(m => m.DesiredModeofRemittance))
                        .ForMember(c => c.ListofDocuments, c => c.MapFrom(m => m.ListofDocuments))
                        .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                        .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))


                        .ForMember(c => c.StatusID, c => c.MapFrom(s => statusID))
                        .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                        .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                        .ForMember(c => c.HBName, c => c.MapFrom(s => s.tblMstEmployee.HBName))
                        .ForMember(c => c.Branchname, c => c.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                        .ForMember(c => c.Paddress, c => c.MapFrom(s => s.tblMstEmployee.PAdd))
                        .ForMember(c => c.currentDate, c => c.MapFrom(s => (s.CurrentDate == null ? DateTime.Now : s.CurrentDate)))
                        .ForMember(d => d.BankAcNo, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == employeeID).BankAcNo))
                    .ForMember(d => d.AccountNo, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == employeeID).BankAcNo))
                     .ForMember(d => d.BasicPay, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == employeeID).E_Basic))
                       .ForMember(d => d.DA, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == employeeID).D_01))
                        .ForAllOtherMembers(c => c.Ignore());
                    });
                    obj = Mapper.Map<DTOModel.NonRefundablePFLoan, Model.NonRefundablePFLoan>(empPFObj);
                }
                else
                {
                    empPFObj = genericRepo.GetByID<DTOModel.NRPFLoanHDR>(NRPFLoanID);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Data.Models.NRPFLoanHDR, Model.NonRefundablePFLoan>()
                         .ForMember(c => c.NRPFLoanID, c => c.MapFrom(m => m.NRPFLoanID))
                         .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeId))
                        .ForMember(c => c.StatusID, c => c.MapFrom(s => statusID))
                        .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                        .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                        .ForMember(c => c.HBName, c => c.MapFrom(s => s.tblMstEmployee.HBName))
                        .ForMember(c => c.Branchname, c => c.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                         .ForMember(c => c.currentDate, c => c.MapFrom(s => DateTime.Now.ToShortDateString()))
                              .ForMember(c => c.Paddress, c => c.MapFrom(s => s.tblMstEmployee.PAdd))
                           .ForMember(d => d.BankAcNo, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == employeeID).BankAcNo))
                    .ForMember(d => d.AccountNo, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == employeeID).BankAcNo))
                     .ForMember(d => d.BasicPay, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == employeeID).E_Basic))
                       .ForMember(d => d.DA, o => o.MapFrom(s => s.tblMstEmployee.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == employeeID).D_01))
                        .ForAllOtherMembers(c => c.Ignore());
                    });
                    obj = Mapper.Map<DTOModel.NRPFLoanHDR, Model.NonRefundablePFLoan>(empPFObj);
                }

                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertNRPFLoanDetails(NonRefundablePFLoan createNRPFLoan, ProcessWorkFlow workFlow)
        {
            log.Info($"NRPFLoanService/InsertNRPFLoanDetails");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.NonRefundablePFLoan, DTOModel.NonRefundablePFLoan>()
                    .ForMember(c => c.NRPFLoanID, c => c.MapFrom(m => m.NRPFLoanID))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeId))
                    .ForMember(c => c.Amount_of_Advanced, c => c.MapFrom(m => m.Amount_of_Advanced  ))
                    .ForMember(c => c.Purpose_of_Advanced, c => c.MapFrom(m => m.Purpose_of_Advanced))
                    .ForMember(c => c.Date_of_Sanction, c => c.MapFrom(m => m.Date_of_Sanction))
                    .ForMember(c => c.LocationOfDwellingSite, c => c.MapFrom(m => m.LocationOfDwellingSite))
                    .ForMember(c => c.NamePresentOwner, c => c.MapFrom(m => m.NamePresentOwner))
                     .ForMember(c => c.AddressPresentOwner, c => c.MapFrom(m => m.AddressPresentOwner))
                    .ForMember(c => c.PresentStateofDwelling, c => c.MapFrom(m => m.PresentStateofDwelling))
                    .ForMember(c => c.DesiredModeofRemittance, c => c.MapFrom(m => m.DesiredModeofRemittance))
                    .ForMember(c => c.ListofDocuments, c => c.MapFrom(m => m.ListofDocuments))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(c => c.CurrentDate, c => c.MapFrom(m => DateTime.Now))
                    .ForAllOtherMembers(c => c.Ignore());

                });
                var dto = Mapper.Map<DTOModel.NonRefundablePFLoan>(createNRPFLoan);
                genericRepo.Insert<DTOModel.NonRefundablePFLoan>(dto);

                if (dto.ID > 0)
                {
                    if (createNRPFLoan.FormStatus == (short)NRPFLoanFormState.SubmitedByEmployee)
                    {
                        workFlow.ReferenceID = dto.ID;
                        AddProcessWorkFlow(workFlow);
                    }
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

        public bool UpdateNRPFLOanDetails(NonRefundablePFLoan createNRPFLoan)
        {
            log.Info($"EmployeePFOrganisationService/UpdateEmployeePFDetails");
            bool flag = false;
            try
            {
                var dto = genericRepo.Get<DTOModel.NonRefundablePFLoan>(x => x.ID == createNRPFLoan.ID).FirstOrDefault();
                dto.Amount_of_Advanced = createNRPFLoan.Amount_of_Advanced;
                dto.Purpose_of_Advanced = createNRPFLoan.Purpose_of_Advanced;
                dto.Date_of_Sanction = createNRPFLoan.Date_of_Sanction;
                dto.LocationOfDwellingSite = createNRPFLoan.LocationOfDwellingSite;
                dto.NamePresentOwner = createNRPFLoan.NamePresentOwner;
                dto.AddressPresentOwner = createNRPFLoan.AddressPresentOwner;
                dto.PresentStateofDwelling = createNRPFLoan.PresentStateofDwelling;
                dto.DesiredModeofRemittance = createNRPFLoan.DesiredModeofRemittance;
                dto.ListofDocuments = createNRPFLoan.ListofDocuments;
                dto.UpdatedBy = createNRPFLoan.UpdatedBy;
                dto.UpdatedOn = createNRPFLoan.UpdatedOn;
                genericRepo.Update<DTOModel.NonRefundablePFLoan>(dto);

                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool UpdateNRPFLoanStatus(NonRefundablePFLoan createNRPFLoan, ProcessWorkFlow workFlow)
        {
            log.Info($"NRPFLoanService/UpdateEmployeePFStatus");
            bool flag = false;
            try
            {
                var dto = genericRepo.Get<DTOModel.NRPFLoanHDR>(x => x.NRPFLoanID == createNRPFLoan.NRPFLoanID).FirstOrDefault();
                dto.StatusID = (int)createNRPFLoan.StatusID;
                dto.UpdatedBy = createNRPFLoan.UpdatedBy;
                dto.UpdatedOn = createNRPFLoan.UpdatedOn;
                genericRepo.Update<DTOModel.NRPFLoanHDR>(dto);

               
                int res = 1;
                if (res > 0)
                {
                    if (createNRPFLoan.FormStatus == (short)NRPFLoanFormState.SubmitedByEmployee || createNRPFLoan.FormStatus == (short)NRPFLoanFormState.AcceptedByReporting  )
                    {
                        workFlow.ReferenceID = createNRPFLoan.NRPFLoanID;
                        AddProcessWorkFlow(workFlow);
                    }
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

        public IEnumerable<NonRefundablePFLoan> GetNRPFLoanHdr(NRPFLoanFormApprovalFilter filters)
        {
            log.Info($"EmployeePFOrganisationService/GetNRPFLoanHdr/");
            try
            {
                IEnumerable<NonRefundablePFLoan> formHdrs = Enumerable.Empty<NonRefundablePFLoan>();

                List<DTOModel.NRPFLoanHDR> dtoFormHdrs = new List<DTOModel.NRPFLoanHDR>();

                var formsHdr = genericRepo.GetIQueryable<DTOModel.NRPFLoanHDR>(x => x.tblMstEmployee.EmployeeProcessApprovals
                .Any(y => y.ProcessID == (int)Common.WorkFlowProcess.NonRefundablePFLoan
                && y.ToDate == null
                && (y.ReportingTo == filters.loggedInEmployeeID || y.ReviewingTo == filters.loggedInEmployeeID || y.AcceptanceAuthority == filters.loggedInEmployeeID))
                );

                if (filters != null)
                    dtoFormHdrs = formsHdr.Where(x =>
                    (filters.selectedEmployeeID == 0 ? (1 > 0) : x.EmployeeId == filters.selectedEmployeeID)).ToList();


                //  var dtoFormHdrs = appraisalRepo.GetAppraisalFormHdr();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.NRPFLoanHDR, Model.NonRefundablePFLoan>()
                   .ForMember(d => d.NRPFLoanID, o => o.MapFrom(s => s.NRPFLoanID))
                    .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.Employeecode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(d => d.Employeename, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == (int)Common.WorkFlowProcess.NonRefundablePFLoan && y.ToDate == null && y.EmployeeID == s.EmployeeId)))
                    .ForMember(d => d.FormStatus, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.tblMstEmployee.DepartmentID))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.tblMstEmployee.DesignationID))
                    .ForAllOtherMembers(d => d.Ignore());

                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();

                });
                return Mapper.Map<List<Model.NonRefundablePFLoan>>(dtoFormHdrs.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertNRPFLoanDetailsHDR(int employeeID, NonRefundablePFLoan createNRPFLoan)
        {
            log.Info($"NRPFLoanService/InsertNRPFLoanDetailsHDR");
            bool flag = false;
            try
            {
                int branchID = genericRepo.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>(employeeID).BranchID;
                int departmentID =(int) genericRepo.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>(employeeID).DepartmentID;
                int designationID = genericRepo.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>(employeeID).DesignationID;
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.NonRefundablePFLoan, DTOModel.NRPFLoanHDR>()
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(m => employeeID))
                    .ForMember(c => c.BranchID, o => o.MapFrom(m=> branchID))
                    .ForMember(c => c.DepartmentID, o => o.MapFrom(m => departmentID))
                    .ForMember(c => c.DesignationID, o => o.MapFrom(m => designationID))
                    .ForMember(c => c.StatusID, o => o.MapFrom(m =>0))
                    .ForMember(c => c.Requestdate, o => o.MapFrom(m => DateTime.Now))
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());

                });
                var dto = Mapper.Map<DTOModel.NRPFLoanHDR>(createNRPFLoan);
                genericRepo.Insert<DTOModel.NRPFLoanHDR>(dto);
               
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
