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
using Nafed.MicroPay.Common;

namespace Nafed.MicroPay.Services
{
    public class EmployeePFOrganisationService : BaseService, IEmployeePFOrganisationService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IPFAccountBalanceRepository pfaRepo;
        public EmployeePFOrganisationService(IGenericRepository genericRepo, IPFAccountBalanceRepository pfaRepo)
        {
            this.genericRepo = genericRepo;
            this.pfaRepo = pfaRepo;
        }

        public List<Model.EmployeePFORG> GetEmpPFHList(int EmployeeID)
        {
            log.Info($"GetEmpPFHList");
            try
            {
                var result = genericRepo.Get<DTOModel.EmpPFOrgHDR>(x => (bool)!x.IsDeleted &&
               (EmployeeID != 0 ? x.EmployeeId == EmployeeID : (1 > 0))
               );
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmpPFOrgHDR, Model.EmployeePFORG>()
                    .ForMember(c => c.EmpPFID, c => c.MapFrom(s => s.EmpPFID))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                      .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                      .ForMember(c => c.Branchname, c => c.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                     .ForMember(c => c.DepartmentName, c => c.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                     .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                   //.ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == (int)Common.WorkFlowProcess.Form11 && y.ToDate == null && y.EmployeeID == s.EmployeeId)))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                /*var listPR = Mapper.Map<DTOModel.EmpPFOrgHDR, Model.EmployeePFORG>(result);*/
                var listPR = Mapper.Map<List<Model.EmployeePFORG>>(result);
                return listPR.OrderBy(x => x.EmpPFID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public EmployeePFORG GetEMPPFOrgDetails(int ID, int EmpPFID, int statusID)
        {
            log.Info($"BankService/GetEMPPFOrgDetails/{ID}");
            try
            {
                var obj = (dynamic)null;
                var empPFObj = (dynamic)null;

                if (ID != 0)
                {
                    empPFObj = genericRepo.GetByID<DTOModel.EmployeeProvidentFundOrganisation>(ID);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Data.Models.EmployeeProvidentFundOrganisation, Model.EmployeePFORG>()
                         .ForMember(c => c.EmpPFID, c => c.MapFrom(m => m.EmpPFID))
                        .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeId))
                        .ForMember(c => c.Employee_PF_Scheme_1952, c => c.MapFrom(m => m.Employee_PF_Scheme_1952))
                         .ForMember(c => c.Employee_PF_Scheme_1952_View, c => c.MapFrom(m => (m.Employee_PF_Scheme_1952 == 1 ? "Yes" : "No")))
                        .ForMember(c => c.Employee_Pension_Scheme_1995, c => c.MapFrom(m => m.Employee_Pension_Scheme_1995))
                         .ForMember(c => c.Employee_Pension_Scheme_1995_View, c => c.MapFrom(m => (m.Employee_Pension_Scheme_1995 == 1 ? "Yes" : "No")))
                        .ForMember(c => c.Universal_Acc_No, c => c.MapFrom(m => m.Universal_Acc_No))
                        .ForMember(c => c.Previous_PF_Acc_No, c => c.MapFrom(m => m.Previous_PF_Acc_No))
                        .ForMember(c => c.Dateof_Exit_Previos_Employment, c => c.MapFrom(m => m.Dateof_Exit_Previos_Employment))
                         .ForMember(c => c.Scheme_Certificate_No, c => c.MapFrom(m => m.Scheme_Certificate_No))
                        .ForMember(c => c.Pension_payment_Order_No, c => c.MapFrom(m => m.Pension_payment_Order_No))
                        .ForMember(c => c.International_Worker, c => c.MapFrom(m => m.International_Worker))
                        .ForMember(c => c.International_Worker_View, c => c.MapFrom(m => (m.International_Worker == 1 ? "Yes" : "No")))
                        .ForMember(c => c.State_Country_Origin, c => c.MapFrom(m => m.State_Country_Origin))
                        .ForMember(c => c.Passport_No, c => c.MapFrom(m => m.Passport_No))
                        .ForMember(c => c.Validity_Passport_from, c => c.MapFrom(m => m.Validity_Passport_from))
                        .ForMember(c => c.Validity_Passport_to, c => c.MapFrom(m => m.Validity_Passport_to))
                        .ForMember(c => c.BankAcNo, c => c.MapFrom(m => m.BankAcNo))
                        .ForMember(c => c.IFSCCode, c => c.MapFrom(m => m.IFSCCode))
                        .ForMember(c => c.AadhaarNo, c => c.MapFrom(m => m.AadhaarNo))
                        .ForMember(c => c.Permanent_AcNo, c => c.MapFrom(m => m.Permanent_AcNo))
                         .ForMember(c => c.PFNo, c => c.MapFrom(m => m.PFNo))
                        .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                        .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                        .ForMember(c => c.StatusID, c => c.MapFrom(s => statusID))
                        .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                        .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                        .ForMember(c => c.HBName, c => c.MapFrom(s => s.tblMstEmployee.HBName))
                        .ForMember(c => c.DOB, c => c.MapFrom(s => s.tblMstEmployee.DOB))
                         .ForMember(c => c.DOJ, c => c.MapFrom(s => s.tblMstEmployee.DOJ))
                        .ForMember(c => c.OfficialEmail, c => c.MapFrom(s => s.tblMstEmployee.OfficialEmail))
                        .ForMember(c => c.MobileNo, c => c.MapFrom(s => s.tblMstEmployee.MobileNo))
                        .ForMember(c => c.Gender, c => c.MapFrom(s => s.tblMstEmployee.Gender.Name))
                        .ForMember(c => c.MaritalSts, c => c.MapFrom(s => s.tblMstEmployee.MaritalStatu.MaritalStatusName))
                        .ForMember(c => c.Branchname, c => c.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                         .ForMember(c => c.PersonalEmailID, c => c.MapFrom(s => s.tblMstEmployee.PersonalEmail))
                        .ForMember(c => c.currentDate, c => c.MapFrom(s => (s.CurrentDate == null ? DateTime.Now : s.CurrentDate)))
                        //.ForMember(c => c.currentDate, c => c.MapFrom(s =>(s.EmpPFOrgHDR.CurrentDate == null ?  DateTime.Now : s.EmpPFOrgHDR.CurrentDate)))
                         .ForMember(c => c.currentDateAA, c => c.MapFrom(s => DateTime.Now))
                        .ForMember(c => c.AadhaarCardFilePath, c => c.MapFrom(s => s.AadhaarCardFilePath))
                       .ForMember(c => c.PanCardFilePath, c => c.MapFrom(s => s.PanCardFilePath))
                       .ForMember(c => c.PassportFilePath, c => c.MapFrom(s => s.PassportNoFilePath))
                       .ForMember(c => c.BankAccFilePath, c => c.MapFrom(s => s.BankAcFilePath))
                       .ForMember(c => c.EmployeeDeclaration, c => c.MapFrom(s => s.EmployeeDeclaration))
                        .ForAllOtherMembers(c => c.Ignore());
                    });
                    obj = Mapper.Map<DTOModel.EmployeeProvidentFundOrganisation, Model.EmployeePFORG>(empPFObj);
                }
                else
                {
                    empPFObj = genericRepo.GetByID<DTOModel.EmpPFOrgHDR>(EmpPFID);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Data.Models.EmpPFOrgHDR, Model.EmployeePFORG>()
                         .ForMember(c => c.EmpPFID, c => c.MapFrom(m => m.EmpPFID))
                         .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeId))
                        .ForMember(c => c.StatusID, c => c.MapFrom(s => statusID))
                        .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                        .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                        .ForMember(c => c.HBName, c => c.MapFrom(s => s.tblMstEmployee.HBName))
                        .ForMember(c => c.DOB, c => c.MapFrom(s => s.tblMstEmployee.DOB))
                          .ForMember(c => c.DOJ, c => c.MapFrom(s => s.tblMstEmployee.DOJ))
                        .ForMember(c => c.OfficialEmail, c => c.MapFrom(s => s.tblMstEmployee.OfficialEmail))
                        .ForMember(c => c.MobileNo, c => c.MapFrom(s => s.tblMstEmployee.MobileNo))
                        .ForMember(c => c.Gender, c => c.MapFrom(s => s.tblMstEmployee.Gender.Name))
                        .ForMember(c => c.MaritalSts, c => c.MapFrom(s => s.tblMstEmployee.MaritalStatu.MaritalStatusName))
                        .ForMember(c => c.Branchname, c => c.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                         .ForMember(c => c.PersonalEmailID, c => c.MapFrom(s => s.tblMstEmployee.PersonalEmail))
                         .ForMember(c => c.currentDate, c => c.MapFrom(s => DateTime.Now.ToShortDateString()))
                        .ForAllOtherMembers(c => c.Ignore());
                    });
                    obj = Mapper.Map<DTOModel.EmpPFOrgHDR, Model.EmployeePFORG>(empPFObj);
                }

                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<EmployeePFORG> checkexistdata(int EmpPFID)
        {
            try
            {
                var dto = genericRepo.Get<DTOModel.EmployeeProvidentFundOrganisation>(x => x.EmpPFID == EmpPFID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.EmployeeProvidentFundOrganisation, Model.EmployeePFORG>()
                     .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                   .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                      .ForMember(c => c.AadhaarCardFilePath, c => c.MapFrom(s => s.AadhaarCardFilePath))
                       .ForMember(c => c.PanCardFilePath, c => c.MapFrom(s => s.PanCardFilePath))
                       .ForMember(c => c.PassportFilePath, c => c.MapFrom(s => s.PassportNoFilePath))
                       .ForMember(c => c.BankAccFilePath, c => c.MapFrom(s => s.BankAcFilePath))

                       .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                        .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                        .ForMember(c => c.HBName, c => c.MapFrom(s => s.tblMstEmployee.HBName))
                        .ForMember(c => c.DOB, c => c.MapFrom(s => s.tblMstEmployee.DOB))
                        .ForMember(c => c.OfficialEmail, c => c.MapFrom(s => s.tblMstEmployee.OfficialEmail))
                        .ForMember(c => c.MobileNo, c => c.MapFrom(s => s.tblMstEmployee.MobileNo))
                        .ForMember(c => c.Gender, c => c.MapFrom(s => s.tblMstEmployee.Gender.Name))
                        .ForMember(c => c.MaritalSts, c => c.MapFrom(s => s.tblMstEmployee.MaritalStatu.MaritalStatusName))
                        .ForMember(c => c.Branchname, c => c.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                        .ForMember(c => c.EmployeeDeclaration, c => c.MapFrom(m => m.EmployeeDeclaration))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var list = Mapper.Map<List<Model.EmployeePFORG>>(dto);
                return list.ToList();
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool GetEmpdetails(int EmployeeID, out EmployeePFORG EMPPFdetail)
        {
            bool flag = false;
            log.Info($"GetEmpPFHList");
            try
            {
                EMPPFdetail = new EmployeePFORG();

                if (EmployeeID != 0)
                {

                    EMPPFdetail.Employeecode = genericRepo.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>(EmployeeID).EmployeeCode;
                    EMPPFdetail.AcceptanceAuthority = genericRepo.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>(EmployeeID).Name;
                    flag = true;
                }


            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool InsertEmployeePFDetails(EmployeePFORG createEMPPFOrg, ProcessWorkFlow workFlow)
        {
            log.Info($"EmployeePFOrganisationService/InsertEmployeePFDetails");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmployeePFORG, DTOModel.EmployeeProvidentFundOrganisation>()
                    .ForMember(c => c.EmpPFID, c => c.MapFrom(m => m.EmpPFID))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeId))
                    .ForMember(c => c.Employee_PF_Scheme_1952, c => c.MapFrom(m => m.Employee_PF_Scheme_1952))
                    .ForMember(c => c.Employee_Pension_Scheme_1995, c => c.MapFrom(m => m.Employee_Pension_Scheme_1995))
                    .ForMember(c => c.Universal_Acc_No, c => c.MapFrom(m => m.Universal_Acc_No))
                    .ForMember(c => c.Previous_PF_Acc_No, c => c.MapFrom(m => m.Previous_PF_Acc_No))
                    .ForMember(c => c.Dateof_Exit_Previos_Employment, c => c.MapFrom(m => m.Dateof_Exit_Previos_Employment))
                     .ForMember(c => c.Scheme_Certificate_No, c => c.MapFrom(m => m.Scheme_Certificate_No))
                    .ForMember(c => c.Pension_payment_Order_No, c => c.MapFrom(m => m.Pension_payment_Order_No))
                    .ForMember(c => c.International_Worker, c => c.MapFrom(m => m.International_Worker))
                    .ForMember(c => c.State_Country_Origin, c => c.MapFrom(m => m.State_Country_Origin))
                    .ForMember(c => c.Passport_No, c => c.MapFrom(m => m.Passport_No))
                    .ForMember(c => c.Validity_Passport_from, c => c.MapFrom(m => m.Validity_Passport_from))
                    .ForMember(c => c.Validity_Passport_to, c => c.MapFrom(m => m.Validity_Passport_to))
                    .ForMember(c => c.BankAcNo, c => c.MapFrom(m => m.BankAcNo))
                    .ForMember(c => c.IFSCCode, c => c.MapFrom(m => m.IFSCCode))
                    .ForMember(c => c.AadhaarNo, c => c.MapFrom(m => m.AadhaarNo))
                    .ForMember(c => c.BankAcFilePath, c => c.MapFrom(m => m.BankAccFilePath))
                    .ForMember(c => c.AadhaarCardFilePath, c => c.MapFrom(m => m.AadhaarCardFilePath))
                    .ForMember(c => c.PanCardFilePath, c => c.MapFrom(m => m.PanCardFilePath))
                    .ForMember(c => c.Permanent_AcNo, c => c.MapFrom(m => m.Permanent_AcNo))
                    .ForMember(c => c.CurrentDate, c => c.MapFrom(m => DateTime.Now))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                     .ForMember(c => c.EmployeeDeclaration, c => c.MapFrom(m => m.EmployeeDeclaration))
                    .ForAllOtherMembers(c => c.Ignore());

                });
                var dtoempPF = Mapper.Map<DTOModel.EmployeeProvidentFundOrganisation>(createEMPPFOrg);
                genericRepo.Insert<DTOModel.EmployeeProvidentFundOrganisation>(dtoempPF);

                if (dtoempPF.ID > 0)
                {
                    if (createEMPPFOrg.FormStatus == (short)form11FormState.SubmitedByEmployee)
                    {
                        workFlow.ReferenceID = dtoempPF.ID;
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

        public bool UpdateEmployeePFDetails(EmployeePFORG createEMPPFOrg)
        {
            log.Info($"EmployeePFOrganisationService/UpdateEmployeePFDetails");
            bool flag = false;
            try
            {
                var dto = genericRepo.Get<DTOModel.EmployeeProvidentFundOrganisation>(x => x.ID == createEMPPFOrg.ID).FirstOrDefault();
                dto.Employee_PF_Scheme_1952 = createEMPPFOrg.Employee_PF_Scheme_1952;
                dto.Employee_Pension_Scheme_1995 = createEMPPFOrg.Employee_Pension_Scheme_1995;
                dto.Universal_Acc_No = createEMPPFOrg.Universal_Acc_No;
                dto.Previous_PF_Acc_No = createEMPPFOrg.Previous_PF_Acc_No;
                dto.Dateof_Exit_Previos_Employment = createEMPPFOrg.Dateof_Exit_Previos_Employment;
                dto.Scheme_Certificate_No = createEMPPFOrg.Scheme_Certificate_No;
                dto.Pension_payment_Order_No = createEMPPFOrg.Pension_payment_Order_No;
                dto.International_Worker = createEMPPFOrg.International_Worker;
                dto.PFNo = createEMPPFOrg.PFNo;
                dto.UpdatedBy = createEMPPFOrg.UpdatedBy;
                dto.UpdatedOn = createEMPPFOrg.UpdatedOn;
                dto.EmployeeDeclaration = createEMPPFOrg.EmployeeDeclaration;
                genericRepo.Update<DTOModel.EmployeeProvidentFundOrganisation>(dto);

                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool UpdateEmployeePFStatus(EmployeePFORG createEMPPFOrg, ProcessWorkFlow workFlow)
        {
            log.Info($"EmployeePFOrganisationService/UpdateEmployeePFDetails");
            bool flag = false;
            try
            {
                var dto = genericRepo.Get<DTOModel.EmpPFOrgHDR>(x => x.EmpPFID == createEMPPFOrg.EmpPFID).FirstOrDefault();
                dto.StatusID = (int)createEMPPFOrg.StatusID;
                //dto.CurrentDate = DateTime.Now;
                dto.UpdatedBy = createEMPPFOrg.UpdatedBy;
                dto.UpdatedOn = createEMPPFOrg.UpdatedOn;
                genericRepo.Update<DTOModel.EmpPFOrgHDR>(dto);

                if (createEMPPFOrg.StatusID == 7)
                {
                    var dto1 = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == createEMPPFOrg.EmployeeId).FirstOrDefault();
                    dto1.PFNO = createEMPPFOrg.PFNo;
                    dto1.UpdatedBy = createEMPPFOrg.UpdatedBy;
                    dto1.UpdatedOn = createEMPPFOrg.UpdatedOn;
                    genericRepo.Update<DTOModel.tblMstEmployee>(dto1);
                }
                int res = 1;
                if (res > 0)
                {
                    if (createEMPPFOrg.FormStatus == (short)form11FormState.SubmitedByEmployee || createEMPPFOrg.FormStatus == (short)form11FormState.AcceptedByReporting ||
                        createEMPPFOrg.FormStatus == (short)form11FormState.AcceptedByReviewer || createEMPPFOrg.FormStatus == (short)form11FormState.SubmitedByAcceptanceAuth)
                    {
                        workFlow.ReferenceID = createEMPPFOrg.EmpPFID;
                        AddProcessWorkFlow(workFlow);
                        if (createEMPPFOrg.FormStatus == (short)form11FormState.AcceptedByReviewer || createEMPPFOrg.FormStatus == (short)form11FormState.SubmitedByAcceptanceAuth)
                        {
                            Task pfaTask = Task.Run(() => pfaRepo.UpdatePfNo((int)createEMPPFOrg.EmployeeId, createEMPPFOrg.PFNo));
                        }
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


        public IEnumerable<EmployeePFORG> GetForm11Hdr(Form11FormApprovalFilter filters)
        {
            log.Info($"EmployeePFOrganisationService/GetForm11Hdr/");
            try
            {
                IEnumerable<EmployeePFORG> formHdrs = Enumerable.Empty<EmployeePFORG>();

                List<DTOModel.EmpPFOrgHDR> dtoFormHdrs = new List<DTOModel.EmpPFOrgHDR>();

                var formsHdr = genericRepo.GetIQueryable<DTOModel.EmpPFOrgHDR>(x => x.tblMstEmployee.EmployeeProcessApprovals
                .Any(y => y.ProcessID == (int)Common.WorkFlowProcess.Form11
                && y.ToDate == null
                && (y.ReportingTo == filters.loggedInEmployeeID || y.ReviewingTo == filters.loggedInEmployeeID || y.AcceptanceAuthority == filters.loggedInEmployeeID))
                );

                if (filters != null)
                    dtoFormHdrs = formsHdr.Where(x =>
                    (filters.selectedEmployeeID == 0 ? (1 > 0) : x.EmployeeId == filters.selectedEmployeeID)).ToList();


                //  var dtoFormHdrs = appraisalRepo.GetAppraisalFormHdr();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmpPFOrgHDR, Model.EmployeePFORG>()
                   .ForMember(d => d.EmpPFID, o => o.MapFrom(s => s.EmpPFID))
                    .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.Employeecode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(d => d.Employeename, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == (int)Common.WorkFlowProcess.Form11 && y.ToDate == null && y.EmployeeID == s.EmployeeId)))
                    .ForMember(d => d.FormStatus, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.tblMstEmployee.DepartmentID))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.tblMstEmployee.DesignationID))
                    .ForAllOtherMembers(d => d.Ignore());

                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();

                });
                return Mapper.Map<List<Model.EmployeePFORG>>(dtoFormHdrs.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }



        public IEnumerable<UnAssignedPFRecord> GetUnAssignedPFRecords(int? branchID)
        {
            log.Info($"EmployeePFOrganisationService/GetUnAssignedPFRecords/branchID:{branchID}");

            IEnumerable<UnAssignedPFRecord> list = Enumerable.Empty<UnAssignedPFRecord>();
            try
            {
                var data = pfaRepo.GetUnAssignedPFRecords(branchID);
                list = Common.ExtensionMethods.ConvertToList<UnAssignedPFRecord>(data);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return list;
        }
        public bool UpdatePFNo(int employeeID, int pfNo)
        {           
            log.Info($"EmployeePFOrganisationService/UpdatePFNo/employeeID:{employeeID}&pfNo:{pfNo}");
            try
            {
                pfaRepo.UpdatePfNo(employeeID, pfNo);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            //  return flag;
        }

        public bool UpdateEmpAccountDetail(int eID, int? pfNo, string uan, string epfo, string pan, string aadhar, string ac, string bankCode, string ifscCode, int userID)
        {
            try
            {
                pfaRepo.UpdateEmpAccountDetail(eID, pfNo, uan, epfo, pan, aadhar, ac, bankCode, ifscCode, userID);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region Send Mail
        public bool SendMail(EmployeePFORG createEMPPFOrg)
        {
            log.Info($"EmployeePFOrganisationService/SendMail");
            bool flag = false;
            try
            {
                string ccmail = "";
                createEMPPFOrg.ReportingToEmail = genericRepo.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>(createEMPPFOrg.EmpProceeApproval.ReportingTo).OfficialEmail;
                createEMPPFOrg.ReviewingToEmail = genericRepo.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>(createEMPPFOrg.EmpProceeApproval.ReviewingTo).OfficialEmail;
                createEMPPFOrg.AcceptanceAuthorityEmail = genericRepo.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>(createEMPPFOrg.EmpProceeApproval.AcceptanceAuthority).OfficialEmail;

                if (createEMPPFOrg.ReportingToEmail != null)
                {
                    ccmail = createEMPPFOrg.ReportingToEmail;

                }
                if (createEMPPFOrg.ReviewingToEmail != null)
                {
                    ccmail = ccmail + ";" + createEMPPFOrg.ReviewingToEmail;

                }
                if (createEMPPFOrg.AcceptanceAuthorityEmail != null)
                {
                    ccmail = ccmail + ";" + createEMPPFOrg.AcceptanceAuthorityEmail;

                }

                StringBuilder emailBody = new StringBuilder("");
                if (!String.IsNullOrEmpty(createEMPPFOrg.Employeecode))
                {
                    emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:10pt;'> "
                         + "<p>Dear Sir / Madam</p>"
                     + "<p>With reference to verification and approval of personnel section, it is informed that PF No. DL/1507/" + createEMPPFOrg.PFNo + "</p>"
                   + "<p>has been allotted to Mr./Ms. " + createEMPPFOrg.Employeename + ", " + createEMPPFOrg.DesignationName + " of " + createEMPPFOrg.Branchname + "/ Head Office w.e.f.</p>"

                  + "<p>" + createEMPPFOrg.DOJ + "</p></div>");
                }
                if (!String.IsNullOrEmpty(createEMPPFOrg.PersonalEmailID))
                {
                    EmailConfiguration emailsetting = new EmailConfiguration();
                    var emailsettings = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.EmailConfiguration, Model.EmailConfiguration>();

                    });
                    emailsetting = Mapper.Map<Model.EmailConfiguration>(emailsettings);

                    EmailMessage message = new EmailMessage();
                    message.To = createEMPPFOrg.PersonalEmailID;
                    message.Body = emailBody.ToString();
                    message.CC = ccmail;
                    message.Subject = "NAFED - Generation of PF Number";
                    Task t1 = Task.Run(() => GetEmailConfiguration(message.To, message.Body, message.Subject, message.CC));
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

        private void GetEmailConfiguration(string ToEmailID, string mailbody, string subject, string CC)
        {
            try
            {
                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                EmailMessage message = new EmailMessage();
                message.To = ToEmailID;
                message.CC = CC;
                message.From = "NAFED HRMS <" + emailsetting.ToEmail + ">";
                message.Subject = subject;
                message.UserName = emailsetting.UserName;
                message.Password = emailsetting.Password;
                message.SmtpClientHost = emailsetting.Server;
                message.SmtpPort = emailsetting.Port;
                message.enableSSL = emailsetting.SSLStatus;
                message.Body = mailbody;
                message.HTMLView = true;
                message.FriendlyName = "NAFED";
                EmailHelper.SendEmail(message);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }



        #endregion

    }
}
