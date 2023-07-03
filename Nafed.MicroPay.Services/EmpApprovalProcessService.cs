using AutoMapper;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Common;
using System.Data;
using static Nafed.MicroPay.ImportExport.SalaryConfigurationExport;

namespace Nafed.MicroPay.Services
{
    public class EmpApprovalProcessService : BaseService, IEmpApprovalProcessService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IEmployeeRepository empRepo;
        public EmpApprovalProcessService(IGenericRepository genericRepo, IEmployeeRepository empRepo)
        {
            this.genericRepo = genericRepo;
            this.empRepo = empRepo;
        }

        #region Appointment Approval Process 
        public List<Model.Employee> GetEmployeeList(int? branchID, int? employeeID)
        {
            log.Info($"EmployeeService/GetEmployeeList/{branchID}/{employeeID}");
            try
            {
                var dtoEmployee = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DOLeaveOrg==null && (branchID.HasValue ? x.BranchID == branchID : (1 > 0)) && (employeeID.HasValue && employeeID >0 ? x.EmployeeId == employeeID : (1 > 0))).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblMstEmployee, Model.Employee>()
                     .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.EmployeeTypeName, o => o.MapFrom(s => s.EmployeeType.EmployeeTypeName))
                    .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var employee = Mapper.Map<List<Model.Employee>>(dtoEmployee);
                return employee;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<EmployeeProcessApproval> GetConfirmationApprovalProcesses(int employeeID, int[] processList)
        {
            log.Info($"EmployeeService/GetConfirmationApprovalProcesses/employeeID={employeeID}");
            try
            {
                List<EmployeeProcessApproval> empProcessApproval = new List<EmployeeProcessApproval>();
                if (genericRepo.Exists<DTOModel.Process>(x => x.EmployeeProcessApprovals.FirstOrDefault(y => y.EmployeeID == employeeID && y.ToDate == null && processList.Contains(y.ProcessID)).EmployeeID > 0))
                {
                    var dtoEmpProcessModel = genericRepo.Get<DTOModel.EmployeeProcessApproval>(x => x.EmployeeID == employeeID && x.ToDate == null && processList.Contains(x.ProcessID)).ToList();
                    var dtoprocess = genericRepo.Get<DTOModel.Process>(x => processList.Contains(x.ProcessID)).ToList();

                    empProcessApproval = (from c in dtoprocess
                                          join o in dtoEmpProcessModel on c.ProcessID equals o.ProcessID
                                          select new EmployeeProcessApproval()
                                          {
                                              MultiReporting = o.MultiReporting,
                                              ProcessName = c.ProcessName,
                                              ReportingTo = o.ReportingTo,
                                              ReviewingTo = o.ReviewingTo,
                                              AcceptanceAuthority = o.AcceptanceAuthority,
                                              OldReportingTo = o.ReportingTo,
                                              OldReviewingTo = o.ReviewingTo == null ? 0 : o.ReviewingTo,
                                              OldAcceptanceAuthority = o.AcceptanceAuthority == null ? 0 : o.AcceptanceAuthority,
                                              EmpProcessAppID = o.EmpProcessAppID,
                                              EmployeeID = o.EmployeeID == 0 ? employeeID : o.EmployeeID,
                                              ProcessID = c.ProcessID,
                                              ReportingToName = o.ReportingTo == 0 ? string.Empty : o.tblMstEmployee2.EmployeeCode + " - "+ o.tblMstEmployee2.Name,
                                              ReviewerName = o.ReviewingTo.HasValue ? o.tblMstEmployee3.EmployeeCode + " - " + o.tblMstEmployee3.Name : string.Empty,
                                              AcceptanceAuthorityName = o.AcceptanceAuthority.HasValue ? o.tblMstEmployee1.Name : string.Empty,
                                              CreatedBy = o.CreatedBy,
                                              CreatedOn = o.CreatedOn

                                          }).ToList();
                }
                else
                {
                    var res1 = genericRepo.Get<DTOModel.Process>(x => processList.Contains(x.ProcessID)).ToList();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.Process, Model.EmployeeProcessApproval>()
                        .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                        .ForMember(d => d.EmployeeID, o => o.UseValue(employeeID))
                        .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.ProcessName))
                        .ForMember(d => d.ProcessName, o => o.MapFrom(s => s.ProcessName))
                        .ForAllOtherMembers(d => d.Ignore())
                        ;

                    });
                    empProcessApproval = Mapper.Map<List<Model.EmployeeProcessApproval>>(res1);
                }

                return empProcessApproval;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                throw ex;
            }
        }

        public bool InsertMultiProcessApproval(List<Model.EmployeeProcessApproval> empProcessApproval)
        {
            log.Info($"EmployeeService/InsertMultiProcessApproval");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmployeeProcessApproval, DTOModel.EmployeeProcessApproval>()
                    .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.RoleID, o => o.MapFrom(s => s.RoleID))
                    .ForMember(d => d.ReportingTo, o => o.MapFrom(s => s.ReportingTo))
                    .ForMember(d => d.ReviewingTo, o => o.MapFrom(s => s.ReviewingTo))
                    .ForMember(d => d.AcceptanceAuthority, o => o.MapFrom(s => s.AcceptanceAuthority))
                    .ForMember(d => d.Fromdate, o => o.MapFrom(s => s.Fromdate))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.MultiReporting, o => o.MapFrom(s => s.MultiReporting))
                    .ForAllOtherMembers(d => d.Ignore());
                });

                var dtoProcessApproval = Mapper.Map<List<DTOModel.EmployeeProcessApproval>>(empProcessApproval);
                if (dtoProcessApproval != null && dtoProcessApproval.Count>0)
                    genericRepo.AddMultipleEntity<DTOModel.EmployeeProcessApproval>(dtoProcessApproval);

                #region Confirmation Form Insert
                int ? formHdrID = null;
                int formABHdrID = 0;
                int formTypeID = 0;
                var insertDetail = empProcessApproval.Select(x => new { employeeID = x.EmployeeID, createdBy = x.CreatedBy,processid=x.ProcessID }).FirstOrDefault();
                if(insertDetail !=null && insertDetail.processid !=7)
                    formHdrID = InsertEmployeeConfirmationDetails(insertDetail.employeeID, insertDetail.createdBy, insertDetail.processid,out formABHdrID,out formTypeID);
                #endregion

                #region Insert into Confirmation Status Table for MultiReporting System
                int? formAHdrid = null;
                int? formBHdrid = null;
                if (formTypeID == 1)
                    formAHdrid = formABHdrID;
                else if(formTypeID==2)
                    formBHdrid = formABHdrID;

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeProcessApproval, DTOModel.ConfirmationStatu>()
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.FormHdrID, o => o.UseValue(formHdrID))
                    .ForMember(d => d.StatusID, o => o.UseValue(0))
                    .ForMember(d => d.EmpProcessAppID, o => o.MapFrom(s => s.EmpProcessAppID))
                    .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessID))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))                   
                    .ForMember(d => d.FormAHeaderID, o => o.UseValue(formAHdrid))
                     .ForMember(d => d.FormBHeaderID, o => o.UseValue(formBHdrid))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var dtoConfStatus = Mapper.Map<List<DTOModel.ConfirmationStatu>>(dtoProcessApproval);
                if (dtoProcessApproval != null && dtoProcessApproval.Count>0)
                {
                    genericRepo.AddMultipleEntity<DTOModel.ConfirmationStatu>(dtoConfStatus);
                    flag = true;
                }
                #endregion                
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }
            return flag;
        }

        public bool UpdateMultiProcessApproval(List<Model.EmployeeProcessApproval> empProcessApproval)
        {
            log.Info($"EmployeeService/UpdateMultiProcessApproval");
            bool flag = false;
            try
            {
                for (int i = 0; i < empProcessApproval.Where(x => x.IsDeleted == true).ToList().Count; i++)
                {
                    var dtoObj = genericRepo.GetByID<DTOModel.EmployeeProcessApproval>(empProcessApproval[i].EmpProcessAppID);
                    if (dtoObj != null )
                    {
                        dtoObj.ToDate = DateTime.Now.Date.AddDays(-1);
                        dtoObj.UpdatedBy = empProcessApproval[i].UpdatedBy;
                        dtoObj.UpdatedOn = empProcessApproval[i].UpdatedOn;
                        genericRepo.Update<DTOModel.EmployeeProcessApproval>(dtoObj);

                        var dtoObjStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.EmpProcessAppID == dtoObj.EmpProcessAppID).FirstOrDefault();
                        if (dtoObjStatus != null)
                        {
                            genericRepo.Delete<DTOModel.ConfirmationStatu>(dtoObjStatus);
                            flag = true;
                        }
                    }
                }

                flag = InsertMultiProcessApproval(empProcessApproval.Where(x => x.IsDeleted == false).ToList());
                flag = true;
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }
            return flag;
        }

        public int InsertEmployeeConfirmationDetails(int employeeID, int createdBy,int processID, out int formABHdrID,out int formTypeID)
        {
            log.Info($"EmployeeService/InsertEmployeeConfirmationDetails");
            try
            {
                int formHdrID = 0;
                formABHdrID = 0;
                formTypeID = 0;
                //int formABHdrID = 0;
                var level = genericRepo.Get<DTOModel.Designation>(x => x.tblMstEmployees.Any(y => y.EmployeeId == employeeID)).FirstOrDefault().Level;
                var promotiomDetails = genericRepo.Get<DTOModel.tblpromotion>(x => x.EmployeeID == employeeID && !x.IsDeleted && x.EmployeeID != null).ToList();
                var EmpDetails = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == employeeID).FirstOrDefault();
                if (promotiomDetails.Count > 1 && promotiomDetails.Any(x => x.ToDate == null))
                {
                    if (level == "1" || level == "2" || level == "3" || level == "4" || level == "5" || level == "6" || level == "7")
                    {
                        if (!genericRepo.Exists<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID && x.ProcessID== processID && x.StatusID==0))
                        {
                            Model.ConfirmationFormHdr confirmationFormHdr = new ConfirmationFormHdr();
                            confirmationFormHdr.FormTypeID = 2;
                            confirmationFormHdr.ProcessID = 7;
                            confirmationFormHdr.StatusID = 0;
                            confirmationFormHdr.EmployeeID = employeeID;
                            confirmationFormHdr.CreatedBy = createdBy;
                            confirmationFormHdr.CreatedOn = DateTime.Now;
                            formTypeID =(int) confirmationFormHdr.FormTypeID;
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormHdr>()
                                .ForMember(d => d.FormTypeID, o => o.MapFrom(s => s.FormTypeID))
                                .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                                .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                                .ForAllOtherMembers(d => d.Ignore());
                            });

                            var dtoConfirmationFormHdr = Mapper.Map<DTOModel.ConfirmationFormHdr>(confirmationFormHdr);
                            genericRepo.Insert<DTOModel.ConfirmationFormHdr>(dtoConfirmationFormHdr);

                            formHdrID = dtoConfirmationFormHdr.FormHdrID;

                            Model.ConfirmationFormAHdr confirmationForm = new ConfirmationFormAHdr();
                            confirmationForm.EmployeeId = employeeID;
                            confirmationForm.ProcessId = 7;
                            confirmationForm.DesignationId = EmpDetails.DesignationID;
                            confirmationForm.BranchId = EmpDetails.BranchID;
                            confirmationForm.CertificatesReceived = true;
                            confirmationForm.PoliceVerification = true;
                            confirmationForm.CreatedBy = createdBy;
                            confirmationForm.CreatedOn = DateTime.Now;
                            confirmationForm.DueConfirmationDate = EmpDetails.Pr_Loc_DOJ.Value.AddYears(1);
                            confirmationForm.FormHdrID = formHdrID;

                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<Model.ConfirmationFormAHdr, DTOModel.ConfirmationFormBHeader>()
                                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                                .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                                .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                              .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                              .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                              .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                              .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
                              .ForMember(d => d.FormState, o => o.UseValue(0))
                             .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                             .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                             .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
                             .ForAllOtherMembers(d => d.Ignore());
                            });

                            var dtoConfirmationFormBHdr = Mapper.Map<DTOModel.ConfirmationFormBHeader>(confirmationForm);
                            genericRepo.Insert<DTOModel.ConfirmationFormBHeader>(dtoConfirmationFormBHdr);
                            formABHdrID = dtoConfirmationFormBHdr.FormBHeaderId;
                        }
                        else
                        {
                            formTypeID = 2;
                            var dtoObj = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID && x.ProcessID == processID && x.StatusID==0).ToList();
                            dtoObj.ForEach(x => { x.ProcessID = 7; x.FormTypeID = 2; });
                            if (dtoObj.Count > 0)
                            {
                         
                                for (int i = 0; i < dtoObj.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoObj[i]);
                                    formHdrID = dtoObj[i].FormHdrID;
                                }
                            }
                            var dtoObj1 = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.EmployeeId == employeeID && x.ProcessId == processID).ToList();
                            if (dtoObj1.Count > 0)
                            {
                                //formHdrID = dtoObj1.FirstOrDefault().FormHdrID;
                                dtoObj1.ForEach(x => { x.ProcessId = 7; });
                                for (int i = 0; i < dtoObj1.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormBHeader>(dtoObj1[i]);
                                    formABHdrID = dtoObj1[i].FormBHeaderId;
                                }
                            }
                        }
                    }
                    else if (level == "8" || level == "9" || level == "10" || level == "11" || level == "12" || level == "13A")
                    {
                        if (!genericRepo.Exists<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID && x.ProcessID == processID && x.StatusID==0))
                        {
                            Model.ConfirmationFormHdr confirmationFormHdr = new ConfirmationFormHdr();
                            confirmationFormHdr.FormTypeID = 1;
                            confirmationFormHdr.ProcessID = 7;
                            confirmationFormHdr.StatusID = 0;
                            confirmationFormHdr.EmployeeID = employeeID;
                            confirmationFormHdr.CreatedBy = createdBy;
                            confirmationFormHdr.CreatedOn = DateTime.Now;
                            formTypeID = (int)confirmationFormHdr.FormTypeID;
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormHdr>()
                                .ForMember(d => d.FormTypeID, o => o.MapFrom(s => s.FormTypeID))
                                .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                                .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                                .ForAllOtherMembers(d => d.Ignore());
                            });

                            var dtoConfirmationFormHdr = Mapper.Map<DTOModel.ConfirmationFormHdr>(confirmationFormHdr);
                            genericRepo.Insert<DTOModel.ConfirmationFormHdr>(dtoConfirmationFormHdr);

                            formHdrID = dtoConfirmationFormHdr.FormHdrID;

                            Model.ConfirmationFormAHdr confirmationForm = new ConfirmationFormAHdr();
                            confirmationForm.EmployeeId = employeeID;
                            confirmationForm.ProcessId = 7;
                            confirmationForm.DesignationId = EmpDetails.DesignationID;
                            confirmationForm.BranchId = EmpDetails.BranchID;
                            confirmationForm.CertificatesReceived = true;
                            confirmationForm.PoliceVerification = true;
                            confirmationForm.CreatedBy = createdBy;
                            confirmationForm.CreatedOn = DateTime.Now;
                            confirmationForm.DueConfirmationDate = EmpDetails.Pr_Loc_DOJ.Value.AddYears(1);
                            confirmationForm.FormHdrID = formHdrID;

                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<Model.ConfirmationFormAHdr, DTOModel.ConfirmationFormAHeader>()
                                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                                .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                                .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                                 .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                                 .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                                 .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                                 .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
                                 .ForMember(d => d.FormState, o => o.UseValue(0))
                                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                                .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
                                .ForAllOtherMembers(d => d.Ignore());
                            });

                            var dtoConfirmationFormAHdr = Mapper.Map<DTOModel.ConfirmationFormAHeader>(confirmationForm);
                            genericRepo.Insert<DTOModel.ConfirmationFormAHeader>(dtoConfirmationFormAHdr);
                            formABHdrID = dtoConfirmationFormAHdr.FormAHeaderId;
                        }
                        else
                        {
                            formTypeID = 1;
                            var dtoObj = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID && x.ProcessID == processID && x.StatusID == 0).ToList();
                            dtoObj.ForEach(x => { x.ProcessID = 7; x.FormTypeID = 1; });
                            if (dtoObj.Count > 0)
                            {
                                
                                for (int i = 0; i < dtoObj.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoObj[i]);
                                    formHdrID = dtoObj[i].FormHdrID;
                                }
                            }
                            var dtoObj1 = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.EmployeeId == employeeID && x.ProcessId == processID).ToList();
                            if (dtoObj1.Count > 0)
                            {
                                //formHdrID = dtoObj1.FirstOrDefault().FormHdrID;
                                dtoObj1.ForEach(x => { x.ProcessId = 7; });
                                for (int i = 0; i < dtoObj1.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormAHeader>(dtoObj1[i]);
                                    formABHdrID = dtoObj1[i].FormAHeaderId;
                                }
                            }

                        }
                    }
                }
                else if (promotiomDetails.Count == 1 && promotiomDetails.Exists(x => x.ToDate == null))
                {
                    if (level == "1" || level == "2" || level == "3" || level == "4" || level == "5" || level == "6" || level == "7")
                    {
                        if (!genericRepo.Exists<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID && x.ProcessID == processID && x.StatusID == 0))
                        {
                            Model.ConfirmationFormHdr confirmationFormHdr = new ConfirmationFormHdr();
                            confirmationFormHdr.FormTypeID = 2;
                            confirmationFormHdr.ProcessID = 6;
                            confirmationFormHdr.StatusID = 0;
                            confirmationFormHdr.EmployeeID = employeeID;
                            confirmationFormHdr.CreatedBy = createdBy;
                            confirmationFormHdr.CreatedOn = DateTime.Now;
                            formTypeID = (int)confirmationFormHdr.FormTypeID;
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormHdr>()
                                .ForMember(d => d.FormTypeID, o => o.MapFrom(s => s.FormTypeID))
                                .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                                .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                                .ForAllOtherMembers(d => d.Ignore());
                            });

                            var dtoConfirmationFormHdr = Mapper.Map<DTOModel.ConfirmationFormHdr>(confirmationFormHdr);
                            genericRepo.Insert<DTOModel.ConfirmationFormHdr>(dtoConfirmationFormHdr);

                            formHdrID = dtoConfirmationFormHdr.FormHdrID;

                            Model.ConfirmationFormAHdr confirmationForm = new ConfirmationFormAHdr();
                            confirmationForm.EmployeeId = employeeID;
                            confirmationForm.ProcessId = 6;
                            confirmationForm.DesignationId = EmpDetails.DesignationID;
                            confirmationForm.BranchId = EmpDetails.BranchID;
                            confirmationForm.CertificatesReceived = true;
                            confirmationForm.PoliceVerification = true;
                            confirmationForm.CreatedBy = createdBy;
                            confirmationForm.CreatedOn = DateTime.Now;
                            //confirmationForm.DueConfirmationDate = EmpDetails.Pr_Loc_DOJ.Value.AddYears(1);
                            confirmationForm.DueConfirmationDate = null;
                            confirmationForm.FormHdrID = formHdrID;

                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<Model.ConfirmationFormAHdr, DTOModel.ConfirmationFormBHeader>()
                                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                                .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                                .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                                 .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                                 .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                                 .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                                 .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
                                 .ForMember(d => d.FormState, o => o.UseValue(0))
                                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                                .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
                                .ForAllOtherMembers(d => d.Ignore());
                            });
                            var dtoConfirmationFormBHdr = Mapper.Map<DTOModel.ConfirmationFormBHeader>(confirmationForm);
                            genericRepo.Insert<DTOModel.ConfirmationFormBHeader>(dtoConfirmationFormBHdr);
                            formABHdrID = dtoConfirmationFormBHdr.FormBHeaderId;
                        }
                        else
                        {
                            formTypeID = 2;
                            var dtoObj = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID && x.ProcessID == processID && x.StatusID == 0).ToList();
                            dtoObj.ForEach(x => { x.ProcessID = 6; x.FormTypeID = 2; });
                            if (dtoObj.Count > 0)
                            {
                               
                                for (int i = 0; i < dtoObj.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoObj[i]);
                                    formHdrID = dtoObj[i].FormHdrID;
                                }
                            }
                            var dtoObj1 = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.EmployeeId == employeeID && x.ProcessId == processID).ToList();
                            if (dtoObj1.Count > 0)
                            {
                                dtoObj1.ForEach(x => { x.ProcessId = 6; });
                                for (int i = 0; i < dtoObj1.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormBHeader>(dtoObj1[i]);
                                   formABHdrID = dtoObj1[i].FormBHeaderId;
                                }
                            }
                        }
                    }
                    else if (level == "8" || level == "9" || level == "10" || level == "11" || level == "12" || level == "13A")
                    {
                        if (!genericRepo.Exists<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID && x.ProcessID == processID && x.StatusID == 0))
                        {
                            Model.ConfirmationFormHdr confirmationFormHdr = new ConfirmationFormHdr();
                            confirmationFormHdr.FormTypeID = 1;
                            confirmationFormHdr.ProcessID = 6;
                            confirmationFormHdr.StatusID = 0;
                            confirmationFormHdr.EmployeeID = employeeID;
                            confirmationFormHdr.CreatedBy = createdBy;
                            confirmationFormHdr.CreatedOn = DateTime.Now;
                            formTypeID = (int)confirmationFormHdr.FormTypeID;
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormHdr>()
                                .ForMember(d => d.FormTypeID, o => o.MapFrom(s => s.FormTypeID))
                                .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                                .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                                .ForAllOtherMembers(d => d.Ignore());
                            });

                            var dtoConfirmationFormHdr = Mapper.Map<DTOModel.ConfirmationFormHdr>(confirmationFormHdr);
                            genericRepo.Insert<DTOModel.ConfirmationFormHdr>(dtoConfirmationFormHdr);

                            formHdrID = dtoConfirmationFormHdr.FormHdrID;

                            Model.ConfirmationFormAHdr confirmationForm = new ConfirmationFormAHdr();
                            confirmationForm.EmployeeId = employeeID;
                            confirmationForm.ProcessId = 6;
                            confirmationForm.DesignationId = EmpDetails.DesignationID;
                            confirmationForm.BranchId = EmpDetails.BranchID;
                            confirmationForm.CertificatesReceived = true;
                            confirmationForm.PoliceVerification = true;
                            confirmationForm.CreatedBy = createdBy;
                            confirmationForm.CreatedOn = DateTime.Now;
                            // confirmationForm.DueConfirmationDate = EmpDetails.Pr_Loc_DOJ.Value.AddYears(1);
                            confirmationForm.DueConfirmationDate = null;
                            confirmationForm.FormHdrID = formHdrID;

                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<Model.ConfirmationFormAHdr, DTOModel.ConfirmationFormAHeader>()
                                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                                .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                                .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                                .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                                .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                                .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                                .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
                                .ForMember(d => d.FormState, o => o.UseValue(0))
                                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                                .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
                                .ForAllOtherMembers(d => d.Ignore());
                            });

                            var dtoConfirmationFormAHdr = Mapper.Map<DTOModel.ConfirmationFormAHeader>(confirmationForm);
                            genericRepo.Insert<DTOModel.ConfirmationFormAHeader>(dtoConfirmationFormAHdr);
                            formABHdrID = dtoConfirmationFormAHdr.FormAHeaderId;
                        }
                        else
                        {
                            formTypeID = 1;
                            var dtoObj = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID && x.ProcessID == processID && x.StatusID == 0).ToList();
                            dtoObj.ForEach(x => { x.ProcessID = 6; x.FormTypeID = 1; });
                            if (dtoObj.Count > 0)
                            {
                                
                                for (int i = 0; i < dtoObj.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoObj[i]);
                                    formHdrID = dtoObj[i].FormHdrID;
                                }
                            }
                            var dtoObj1 = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.EmployeeId == employeeID).ToList();
                            if (dtoObj1.Count > 0)
                            {
                                dtoObj1.ForEach(x => { x.ProcessId = 6; });
                                for (int i = 0; i < dtoObj1.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormAHeader>(dtoObj1[i]);
                                    formABHdrID = dtoObj1[i].FormAHeaderId;
                                }
                            }
                        }
                    }
                }
                return formHdrID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.InnerException + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool IsProcessStarted(int empProcessAppID, int empID)
        {
            log.Info($"EmployeeService/InsertEmployeeConfirmationDetails");
            try
            {
                return genericRepo.Exists<DTOModel.ConfirmationStatu>(x => x.EmpProcessAppID == empProcessAppID && x.EmployeeID == empID && x.StatusID > 0 && x.StatusID < 8);
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
