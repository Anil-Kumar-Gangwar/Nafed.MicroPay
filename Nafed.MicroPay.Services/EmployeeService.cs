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
using Nafed.MicroPay.ImportExport.Interfaces;
using static Nafed.MicroPay.ImportExport.AchievementCertificationReportExport;

namespace Nafed.MicroPay.Services
{
    public class EmployeeService : BaseService, IEmployeeService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IEmployeeRepository empRepo;
        private readonly IExport exportExcel;
        public EmployeeService(IGenericRepository genericRepo, IEmployeeRepository empRepo, IExport exportExcel)
        {
            this.genericRepo = genericRepo;
            this.empRepo = empRepo;
            this.exportExcel = exportExcel;
        }

        public bool EmployeeDetailsExists(int id, string value)
        {
            log.Info($"EmployeeService/EmployeeDetailsExists/{id}/{value}");
            return genericRepo.Exists<DTOModel.tblMstEmployee>(x => x.EmployeeId != id && x.EmployeeCode == value);
        }

        DTOModel.GetLastEmployeeCode_Result GetLastestEmployeeCode(int empTypeID)
        {
            return empRepo.GetLastEmployeeCode(empTypeID);
        }


        public int InsertEmployeeDetails(Model.Employee employeeDetails)
        {
            log.Info($"EmployeeService/InsertEmployeeDetailsDetails");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Employee, DTOModel.tblMstEmployee>()
                   .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.EmployeeCode))
                   .ForMember(c => c.TitleID, c => c.MapFrom(s => s.TitleID))
                   .ForMember(c => c.Name, c => c.MapFrom(s => s.Name))
                   .ForMember(c => c.EmployeeTypeID, c => c.MapFrom(s => s.EmployeeTypeID))
                   .ForMember(c => c.GenderID, c => c.MapFrom(s => s.GenderID))
                   .ForMember(c => c.DesignationID, c => c.MapFrom(s => s.DesignationID))
                   .ForMember(c => c.CadreID, c => c.MapFrom(s => s.CadreID))
                   .ForMember(c => c.IsCadreOfficer, c => c.MapFrom(s => s.IsCadreOfficer))
                   .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                   .ForMember(c => c.DepartmentID, c => c.MapFrom(s => s.DepartmentID))
                   .ForMember(c => c.SectionID, c => c.MapFrom(s => s.SectionID))
                   .ForMember(c => c.DOJ, c => c.MapFrom(s => s.DOJ))
                   .ForMember(c => c.S_DOJ, c => c.MapFrom(s => s.S_DOJ))
                   .ForMember(c => c.SL_No, c => c.MapFrom(s => s.SL_No))
                   .ForMember(c => c.Pr_Loc_DOJ, c => c.MapFrom(s => s.Pr_Loc_DOJ))
                   .ForMember(c => c.ConfirmationDate, c => c.MapFrom(s => s.ConfirmationDate))
                   .ForMember(c => c.Sen_Code, c => c.MapFrom(s => s.Sen_Code))
                   .ForMember(c => c.FileNo, c => c.MapFrom(s => s.FileNo))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.OfficialEmail, c => c.MapFrom(s => s.OfficialEmail))
                   .ForMember(c => c.MobileNo, c => c.MapFrom(s => s.MobileNo))
                   .ForMember(c => c.FirstBranchID, c => c.MapFrom(s => s.FirstBranch))
                   .ForMember(c => c.FirstDesgID, c => c.MapFrom(s => s.FirstDesg))
                   .ForMember(c => c.IsJoinAfterNoon, c => c.MapFrom(s => s.IsJoinAfterNoon))
                   //.ForMember(c => c.ReportingTo, c => c.MapFrom(s => s.ReportingToID))
                   //.ForMember(c => c.ReviewerTo, c => c.MapFrom(s => s.ReviewerTo))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoemployeeDetails = Mapper.Map<DTOModel.tblMstEmployee>(employeeDetails);
                var data = GetLastestEmployeeCode(dtoemployeeDetails.EmployeeTypeID);

                #region Update Last Employee Code in MaxEmpCodeTypeWise table

                var empCode = string.Empty;
                if (data.Prifix == "D" || data.Prifix == "B")
                {
                    if (data.MaxEmployee.ToString().Length == 1)
                        empCode = data.Prifix + "0000" + (data.MaxEmployee + 1);
                    else if (data.MaxEmployee.ToString().Length == 2)
                        empCode = data.Prifix + "000" + (data.MaxEmployee + 1);
                    else if (data.MaxEmployee.ToString().Length == 3)
                        empCode = data.Prifix + "00" + (data.MaxEmployee + 1);
                    else
                        empCode = data.Prifix + "0" + (data.MaxEmployee + 1);
                }
                else
                {
                    empCode = (data.MaxEmployee + 1).ToString();
                }

                dtoemployeeDetails.EmployeeCode = empCode;
                employeeDetails.EmployeeCode = empCode;

                genericRepo.Insert<DTOModel.tblMstEmployee>(dtoemployeeDetails); // insert data in table

                if (genericRepo.Exists<DTOModel.MaxEmpCodeTypeWise>(x => x.EmployeeTypeID == dtoemployeeDetails.EmployeeTypeID))
                {
                    var getLastEmployeeCode = genericRepo.Get<DTOModel.MaxEmpCodeTypeWise>(x => x.EmployeeTypeID == dtoemployeeDetails.EmployeeTypeID).FirstOrDefault();
                    if (getLastEmployeeCode != null)
                    {
                        getLastEmployeeCode.MaxEmployee = data.MaxEmployee + 1;
                        genericRepo.Update(getLastEmployeeCode);
                    }
                }
                #endregion

                #region Add Employee Mode of Payment Detail in Employee Salary Table === Dated On -17.Apr.2020 SG

                var empSalRow = new DTOModel.TblMstEmployeeSalary();
                empSalRow.EmployeeCode = employeeDetails.EmployeeCode;
                empSalRow.EmployeeID = dtoemployeeDetails.EmployeeId;
                empSalRow.BranchID = dtoemployeeDetails.BranchID;
                empSalRow.ModePay = ((ModeOfPayment)employeeDetails.modOfPayment).GetDisplayName().Trim();
                var branchCode = genericRepo.GetByID<DTOModel.Branch>(empSalRow.BranchID).BranchCode;
                empSalRow.BranchCode = branchCode   /*dtoemployeeDetails.Branch.BranchCode*/;
                empSalRow.CreatedBy = employeeDetails.CreatedBy;
                empSalRow.CreatedOn = employeeDetails.CreatedOn;
                empSalRow.IsSalgenrated = false;

                if (employeeDetails.modOfPayment == ModeOfPayment.Bank)
                {
                    empSalRow.BankAcNo = employeeDetails.BankAcNo;
                    empSalRow.BankCode = employeeDetails.BankCode;
                }

                genericRepo.Insert<DTOModel.TblMstEmployeeSalary>(empSalRow);

                #endregion

                #region Update Direct coata column in tblStaffBudget table for the current year.

                //var joinYear = dtoemployeeDetails.Pr_Loc_DOJ.Value.Year;
                //var year = Convert.ToString(joinYear) + "-" + Convert.ToString(Convert.ToString(joinYear + 1).Substring(2, 2));

                //var staffBudgetDetails = genericRepo.Get<DTOModel.tblStaffBudget>(x => x.Year == year && x.DesignationID == dtoemployeeDetails.DesignationID).FirstOrDefault();

                //if (staffBudgetDetails != null)
                //{
                //    staffBudgetDetails.FDirect = Convert.ToInt16(staffBudgetDetails.FDirect - 1);
                //    genericRepo.Update<DTOModel.tblStaffBudget>(staffBudgetDetails);
                //}
                #endregion
                return dtoemployeeDetails.EmployeeId;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateEmployeeOtherDetails(Model.Employee empOtherDetails)
        {
            log.Info($"EmployeeService/UpdateEmployeeOtherDetails");
            bool flag = false;
            try
            {
                ///-----------  Update VRS value in tblStaffBudget -------------------------
                var oldVrs = genericRepo.GetByID<DTOModel.tblMstEmployee>(empOtherDetails.EmployeeID).IsVRS;
                if (oldVrs == false && empOtherDetails.IsVRS == true && empOtherDetails.DOLeaveOrg != null)
                {
                    var leaveYear = empOtherDetails.DOLeaveOrg.Value.Year;
                    var year = Convert.ToString(leaveYear) + "-" + Convert.ToString(Convert.ToString(leaveYear + 1).Substring(2, 2));

                    //var staffBudgetDetails = genericRepo.Get<DTOModel.tblStaffBudget>(x => x.Year == year && x.DesignationID == empOtherDetails.DesignationID).FirstOrDefault();

                    //if (staffBudgetDetails != null)
                    //{
                    //    staffBudgetDetails.VRS = Convert.ToInt16(staffBudgetDetails.VRS + 1);
                    //    genericRepo.Update<DTOModel.tblStaffBudget>(staffBudgetDetails);
                    //}
                }
                else if (oldVrs == true && empOtherDetails.IsVRS == false && empOtherDetails.DOLeaveOrg != null)
                {
                    var leaveYear = empOtherDetails.DOLeaveOrg.Value.Year;
                    var year = Convert.ToString(leaveYear) + "-" + Convert.ToString(Convert.ToString(leaveYear + 1).Substring(2, 2));

                    //var staffBudgetDetails = genericRepo.Get<DTOModel.tblStaffBudget>(x => x.Year == year && x.DesignationID == empOtherDetails.DesignationID).FirstOrDefault();
                    //if (staffBudgetDetails != null)
                    //{

                    //    staffBudgetDetails.VRS = Convert.ToInt16(staffBudgetDetails.VRS - 1);
                    //    genericRepo.Update<DTOModel.tblStaffBudget>(staffBudgetDetails);
                    //}
                }
                //=============== End ============================

                var dtoObj = genericRepo.GetByID<DTOModel.tblMstEmployee>(empOtherDetails.EmployeeID);
                dtoObj.PFNO = empOtherDetails.PFNO;
                dtoObj.Folio_No = empOtherDetails.Folio_No;
                dtoObj.ACR_No = empOtherDetails.ACR_No;
                dtoObj.GraAssuranceNo = empOtherDetails.GraAssuranceNo;
                dtoObj.PeriodAppointment = empOtherDetails.PeriodAppointment;
                dtoObj.ExtnAppointment = empOtherDetails.ExtnAppointment;
                dtoObj.GISNominee = empOtherDetails.GISNominee;
                dtoObj.NomineeRelationID = empOtherDetails.NomineeRelationID == 0 ? null : (int?)empOtherDetails.NomineeRelationID;
                dtoObj.EmpCatID = empOtherDetails.EmpCatID == 0 ? null : empOtherDetails.EmpCatID;
                dtoObj.Dept_Enq = empOtherDetails.Dept_Enq;
                dtoObj.Cer_Given = empOtherDetails.Cer_Given;
                dtoObj.Inv_Issued = empOtherDetails.Inv_Issued;
                dtoObj.Books_LIB = empOtherDetails.Books_LIB;
                dtoObj.OtaCode = empOtherDetails.OtaCode;
                dtoObj.IsVRS = empOtherDetails.IsVRS;
                dtoObj.IsDismissal = empOtherDetails.IsDismissal;
                dtoObj.IsBM = empOtherDetails.IsBM;
                dtoObj.IsRM = empOtherDetails.IsRM;
                dtoObj.IsExpire = empOtherDetails.IsExpire;
                dtoObj.IsTermination = empOtherDetails.IsTermination;
                dtoObj.IsForceFully = empOtherDetails.IsForceFully;
                dtoObj.IsResigned = empOtherDetails.IsResigned;
                dtoObj.ReasonOfLeaving = empOtherDetails.ReasonOfLeaving;
                dtoObj.UpdatedBy = empOtherDetails.UpdatedBy;
                dtoObj.UpdatedOn = empOtherDetails.UpdatedOn;
                dtoObj.DOLeaveOrg = empOtherDetails.DOLeaveOrg;
                dtoObj.DOSupAnnuating = empOtherDetails.DOSupAnnuating;
                dtoObj.DOSupAnnuating1 = empOtherDetails.DOSupAnnuating1;
                dtoObj.UpdatedBy = empOtherDetails.UpdatedBy;
                dtoObj.UpdatedOn = empOtherDetails.UpdatedOn;
                genericRepo.Update<DTOModel.tblMstEmployee>(dtoObj);

                if (empOtherDetails.DOLeaveOrg != null)
                {
                    UpdateSeniority(dtoObj.EmployeeId, (int)dtoObj.DesignationID);
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
        public List<Model.Employee> GetEmployeeList(string empName, string empCode, int? designationID, int? empTypeID,string employeeType=null)
        {
            log.Info($"EmployeeService/GetEmployeeList");
            try
            {
                var result = empRepo.GetEmployeeList(empName, empCode, designationID, empTypeID);

                if(string.IsNullOrEmpty(employeeType))
                result = result.Where(x => x.EmployeeTypeName.Equals(employeeType)).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetEmployeeDetails_Result, Model.Employee>()
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.EmployeeTypeName, o => o.MapFrom(s => s.EmployeeTypeName))
                    .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchName))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.DesignationName))
                    .ForMember(d => d.EmpProfilePhotoUNCPath, o => o.MapFrom(s => s.ImageName))
                    .ForMember(d => d.UserTypeID, o => o.MapFrom(s => s.UserTypeID))
                    .ForMember(d => d.DOLeaveOrg, o => o.MapFrom(s => s.DOLeaveOrg))

                    .ForAllOtherMembers(d => d.Ignore());
                });
                var lstEmployee = Mapper.Map<List<Model.Employee>>(result);
                return lstEmployee;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateEmployeeGeneralDetails(Model.Employee employee)
        {
            log.Info($"EmployeeService/UpdateEmployeeGeneralDetails");
            bool flag = false;
            try
            {
                var dtoEmp = genericRepo.GetByID<DTOModel.tblMstEmployee>(employee.EmployeeID);
                dtoEmp.TitleID = employee.TitleID;
                dtoEmp.Name = employee.Name;
                dtoEmp.EmployeeTypeID = employee.EmployeeTypeID;
                dtoEmp.GenderID = employee.GenderID;
                dtoEmp.DesignationID = employee.DesignationID;
                dtoEmp.CadreID = employee.CadreID == 0 ? null : employee.CadreID;
                dtoEmp.IsCadreOfficer = employee.IsCadreOfficer;
                dtoEmp.BranchID = employee.BranchID;
                dtoEmp.DesignationID = employee.DesignationID;
                dtoEmp.DOJ = (DateTime)employee.DOJ;
                dtoEmp.S_DOJ = employee.S_DOJ;
                dtoEmp.ConfirmationDate = employee.ConfirmationDate;
                dtoEmp.Sen_Code = employee.Sen_Code;
                dtoEmp.FileNo = employee.FileNo;
                dtoEmp.CategoryID = employee.CategoryID == 0 ? null : employee.CategoryID;
                dtoEmp.FirstBranchID = employee.FirstBranch == 0 ? null : (int?)employee.FirstBranch;
                dtoEmp.FirstDesgID = employee.FirstDesg == 0 ? null : (int?)employee.FirstDesg;
                dtoEmp.MobileNo = employee.MobileNo;
                dtoEmp.OfficialEmail = employee.OfficialEmail;
                dtoEmp.IsBM = employee.IsBM;
                dtoEmp.IsRM = employee.IsRM;
                //dtoEmp.ReportingTo = employee.ReportingToID;
                //dtoEmp.ReviewerTo = employee.ReviewerTo;
                //dtoEmp.AcceptanceAuthority = employee.AcceptanceAuthority;
                dtoEmp.DossierNo = employee.DossierNo;
                dtoEmp.SectionID = employee.SectionID;
                dtoEmp.DepartmentID = employee.DepartmentID;
                dtoEmp.UpdatedBy = employee.UpdatedBy;
                dtoEmp.UpdatedOn = employee.UpdatedOn;
                dtoEmp.IsJoinAfterNoon = employee.IsJoinAfterNoon;
                genericRepo.Update<DTOModel.tblMstEmployee>(dtoEmp);

                #region ==== Update Department ID in User Table =====  Added on 21-may-2020  ====
                if (employee.DepartmentID > 0)
                {
                    var userInfo = genericRepo.Get<DTOModel.User>(x => x.UserName == employee.EmployeeCode
                    && x.EmployeeID == employee.EmployeeID && !x.IsDeleted).FirstOrDefault();
                    if (userInfo != null)
                    {
                        userInfo.DepartmentID = (int)employee.DepartmentID;
                        genericRepo.Update<DTOModel.User>(userInfo);
                    }
                }

                #endregion

                var empSalRow = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeCode == employee.EmployeeCode).FirstOrDefault();
                if (empSalRow != null)
                {
                    if (employee.modOfPayment == ModeOfPayment.Bank)
                    {
                        empSalRow.BankAcNo = employee.BankAcNo;
                        empSalRow.BankCode = employee.BankCode;
                    }
                    else
                    {
                        empSalRow.BankAcNo = null;
                        empSalRow.BankCode = null;
                    }
                    empSalRow.ModePay = employee.modOfPayment.GetDisplayName();
                    genericRepo.Update<DTOModel.TblMstEmployeeSalary>(empSalRow);
                }
                else
                {
                    #region Add Employee Mode of Payment Detail in Employee Salary Table === Dated On -17.Apr.2020 SG

                    var newEmpSalRow = new DTOModel.TblMstEmployeeSalary();
                    newEmpSalRow.EmployeeCode = employee.EmployeeCode;
                    newEmpSalRow.EmployeeID = employee.EmployeeID;
                    newEmpSalRow.BranchID = employee.BranchID;
                    newEmpSalRow.ModePay = ((ModeOfPayment)employee.modOfPayment).GetDisplayName().Trim();
                    var branchCode = genericRepo.GetByID<DTOModel.Branch>(newEmpSalRow.BranchID).BranchCode;
                    newEmpSalRow.BranchCode = branchCode   /*dtoemployeeDetails.Branch.BranchCode*/;
                    newEmpSalRow.CreatedBy = employee.UpdatedBy ?? 0;
                    newEmpSalRow.CreatedOn = employee.UpdatedOn.Value;
                    newEmpSalRow.IsSalgenrated = false;

                    if (employee.modOfPayment == ModeOfPayment.Bank)
                    {
                        newEmpSalRow.BankAcNo = employee.BankAcNo;
                        newEmpSalRow.BankCode = employee.BankCode;
                    }

                    genericRepo.Insert<DTOModel.TblMstEmployeeSalary>(newEmpSalRow);

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

        public bool UpdateEmployeePromotionalAndIncrementDtls(Model.Employee employee)
        {
            log.Info($"EmployeeService/UpdateEmployeeGeneralDetails");
            bool flag = false;
            try
            {
                var dtoEmp = genericRepo.GetByID<DTOModel.tblMstEmployee>(employee.EmployeeID);
                dtoEmp.IncrementMonth = employee.IncrementMonth;
                dtoEmp.IncrementDate = employee.IncrementDate;
                dtoEmp.ValidateIncrement = employee.ValidateIncrement;
                dtoEmp.Reason = employee.Reason;
                dtoEmp.PromotionDate = employee.PromotionDate;
                dtoEmp.PerfLCT = employee.PerfLCT;
                dtoEmp.orderofpromotion = employee.orderofpromotion;
                dtoEmp.S_DOJ = employee.S_DOJ;
                dtoEmp.UpdatedBy = employee.UpdatedBy;
                dtoEmp.UpdatedOn = employee.UpdatedOn;
                genericRepo.Update<DTOModel.tblMstEmployee>(dtoEmp);

                #region Insert / Update record in Employee Salary table , based on employeeCode

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Employee, Model.EmployeeSalary>()
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                    .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                    .ForMember(d => d.E_Basic, o => o.MapFrom(s => s.E_Basic))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                    .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var empSal = Mapper.Map<Model.Employee, Model.EmployeeSalary>(employee);
                InsertUpdateEmployeeSalaryInfo(empSal);


                #endregion

                #region Update DueDate in Confirmation table after order date assign
                if (employee.orderofpromotion.HasValue)
                {
                    var confFormA = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.EmployeeId == employee.EmployeeID && x.ProcessId == 6 && x.DueConfirmationDate == null).FirstOrDefault();
                    if (confFormA != null)
                    {
                        confFormA.DueConfirmationDate = employee.orderofpromotion.Value.AddYears(1);
                        genericRepo.Update(confFormA);
                    }

                    var confFormB = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.EmployeeId == employee.EmployeeID && x.ProcessId == 6 && x.DueConfirmationDate == null).FirstOrDefault();
                    if (confFormB != null)
                    {
                        confFormB.DueConfirmationDate = employee.orderofpromotion.Value.AddYears(1);
                        genericRepo.Update(confFormB);
                    }
                }
                #endregion
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateEmployeeProfessionalDtls(EmployeeQualification empProfessionalDtls, int employeeID)
        {
            log.Info($"EmployeeService/UpdateEmployeeProfessionalDtls");
            bool flag = false;
            try
            {
                var getacademicdetails = genericRepo.Get<DTOModel.tblEmployeeQualificationdetail>(x => x.EmployeeID == employeeID).ToList<DTOModel.tblEmployeeQualificationdetail>();
                if (getacademicdetails != null || getacademicdetails.Count > 0)
                    genericRepo.DeleteAll<DTOModel.tblEmployeeQualificationdetail>(getacademicdetails);

                #region insert into tblEmployeeQualificationdetail

                if (employeeID > 0)
                {
                    if (empProfessionalDtls.CheckBoxListAcademicvalue != null)
                    {
                        Mapper.Initialize(cfg =>
                       cfg.CreateMap<int, DTOModel.tblEmployeeQualificationdetail>()
                       .ForMember(d => d.EmployeeID, o => o.UseValue(employeeID))
                       .ForMember(d => d.TypeID, o => o.MapFrom(s => 1))
                       .ForMember(d => d.QID, o => o.MapFrom(s => s))
                       .ForMember(d => d.CreatedBy, o => o.MapFrom(s => 1))
                       .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                       .ForAllOtherMembers(d => d.Ignore())
                       );
                        var dtoempacademicdetails = Mapper.Map<List<DTOModel.tblEmployeeQualificationdetail>>(empProfessionalDtls.CheckBoxListAcademicvalue);
                        if (dtoempacademicdetails != null)
                            genericRepo.AddMultipleEntity<DTOModel.tblEmployeeQualificationdetail>(dtoempacademicdetails);


                    }
                    if (empProfessionalDtls.CheckBoxListProfessionalvalue != null)
                    {
                        Mapper.Initialize(cfg =>
                       cfg.CreateMap<int, DTOModel.tblEmployeeQualificationdetail>()
                       .ForMember(d => d.EmployeeID, o => o.UseValue(employeeID))
                       .ForMember(d => d.TypeID, o => o.MapFrom(s => 2))
                       .ForMember(d => d.QID, o => o.MapFrom(s => s))
                       .ForMember(d => d.CreatedBy, o => o.MapFrom(s => 1))
                       .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                       .ForAllOtherMembers(d => d.Ignore())
                       );
                        var dtoempprofessionaldetails = Mapper.Map<List<DTOModel.tblEmployeeQualificationdetail>>(empProfessionalDtls.CheckBoxListProfessionalvalue);
                        if (dtoempprofessionaldetails != null)
                            genericRepo.AddMultipleEntity<DTOModel.tblEmployeeQualificationdetail>(dtoempprofessionaldetails);
                    }

                }

                #endregion
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateEmployeePayScales(Model.EmployeePayScale empPayScale)
        {
            log.Info($"EmployeeService/UpdateEmployeePayScales");

            bool flag = false;

            string payScale = string.Empty;
            try
            {
                if (empPayScale.B1 > 0 || empPayScale.I1 > 0)
                    payScale = (empPayScale.B1.ToString() + "-" + empPayScale.I1.ToString());

                if (empPayScale.B2 > 0 || empPayScale.I2 > 0)
                    payScale += "-" + (empPayScale.B2.ToString() + "-" + empPayScale.I2.ToString());

                if (empPayScale.B3 > 0 || empPayScale.I3 > 0)
                    payScale += "-" + (empPayScale.B3.ToString() + "-" + empPayScale.I3.ToString());

                if (empPayScale.B4 > 0 || empPayScale.I4 > 0)
                    payScale += "-" + (empPayScale.B4.ToString() + "-" + empPayScale.I4.ToString());

                if (empPayScale.B5 > 0 || empPayScale.I5 > 0)
                    payScale += "-" + (empPayScale.B5.ToString() + "-" + empPayScale.I5.ToString());

                if (empPayScale.B6 > 0 || empPayScale.I6 > 0)
                    payScale += "-" + (empPayScale.B6.ToString() + "-" + empPayScale.I6.ToString());

                if (empPayScale.B7 > 0 || empPayScale.I7 > 0)
                    payScale += "-" + (empPayScale.B7.ToString() + "-" + empPayScale.I7.ToString());

                if (empPayScale.B8 > 0 || empPayScale.I8 > 0)
                    payScale += "-" + (empPayScale.B8.ToString() + "-" + empPayScale.I8.ToString());

                if (empPayScale.B9 > 0 || empPayScale.I9 > 0)
                    payScale += "-" + (empPayScale.B9.ToString() + "-" + empPayScale.I9.ToString());

                if (empPayScale.B10 > 0 || empPayScale.I10 > 0)
                    payScale += "-" + (empPayScale.B10.ToString() + "-" + empPayScale.I10.ToString());

                if (empPayScale.B11 > 0 || empPayScale.I11 > 0)
                    payScale += "-" + (empPayScale.B11.ToString() + "-" + empPayScale.I11.ToString());

                if (empPayScale.B12 > 0 || empPayScale.I12 > 0)
                    payScale += "-" + (empPayScale.B12.ToString() + "-" + empPayScale.I12.ToString());

                if (empPayScale.B13 > 0 || empPayScale.I13 > 0)
                    payScale += "-" + (empPayScale.B13.ToString() + "-" + empPayScale.I13.ToString());

                if (empPayScale.B14 > 0 || empPayScale.I14 > 0)
                    payScale += "-" + (empPayScale.B14.ToString() + "-" + empPayScale.I14.ToString());

                if (empPayScale.B15 > 0 || empPayScale.I15 > 0)
                    payScale += "-" + (empPayScale.B15.ToString() + "-" + empPayScale.I15.ToString());

                if (empPayScale.B16 > 0 || empPayScale.I16 > 0)
                    payScale += "-" + (empPayScale.B16.ToString() + "-" + empPayScale.I16.ToString());

                if (empPayScale.B17 > 0 || empPayScale.I17 > 0)
                    payScale += "-" + (empPayScale.B17.ToString() + "-" + empPayScale.I17.ToString());

                if (empPayScale.B18 > 0 || empPayScale.I18 > 0)
                    payScale += "-" + (empPayScale.B18.ToString() + "-" + empPayScale.I18.ToString());

                if (empPayScale.B19 > 0 || empPayScale.I19 > 0)
                    payScale += "-" + (empPayScale.B19.ToString() + "-" + empPayScale.I19.ToString());

                if (empPayScale.B20 > 0 || empPayScale.I20 > 0)
                    payScale += "-" + (empPayScale.B20.ToString() + "-" + empPayScale.I20.ToString());

                if (empPayScale.B21 > 0 || empPayScale.I21 > 0)
                    payScale += "-" + (empPayScale.B21.ToString() + "-" + empPayScale.I21.ToString());

                if (empPayScale.B22 > 0 || empPayScale.I22 > 0)
                    payScale += "-" + (empPayScale.B22.ToString() + "-" + empPayScale.I22.ToString());

                if (empPayScale.B23 > 0 || empPayScale.I23 > 0)
                    payScale += "-" + (empPayScale.B23.ToString() + "-" + empPayScale.I23.ToString());

                if (empPayScale.B24 > 0 || empPayScale.I24 > 0)
                    payScale += "-" + (empPayScale.B24.ToString() + "-" + empPayScale.I24.ToString());

                if (empPayScale.B25 > 0 || empPayScale.I25 > 0)
                    payScale += "-" + (empPayScale.B25.ToString() + "-" + empPayScale.I25.ToString());

                if (empPayScale.B26 > 0 || empPayScale.I26 > 0)
                    payScale += "-" + (empPayScale.B26.ToString() + "-" + empPayScale.I26.ToString());

                if (empPayScale.B27 > 0 || empPayScale.I27 > 0)
                    payScale += "-" + (empPayScale.B27.ToString() + "-" + empPayScale.I27.ToString());

                if (empPayScale.B28 > 0 || empPayScale.I28 > 0)
                    payScale += "-" + (empPayScale.B28.ToString() + "-" + empPayScale.I28.ToString());

                if (empPayScale.B29 > 0 || empPayScale.I29 > 0)
                    payScale += "-" + (empPayScale.B29.ToString() + "-" + empPayScale.I29.ToString());

                if (empPayScale.B30 > 0 || empPayScale.I30 > 0)
                    payScale += "-" + (empPayScale.B30.ToString() + "-" + empPayScale.I30.ToString());

                if (empPayScale.B31 > 0 || empPayScale.I31 > 0)
                    payScale += "-" + (empPayScale.B31.ToString() + "-" + empPayScale.I31.ToString());

                if (empPayScale.B32 > 0 || empPayScale.I32 > 0)
                    payScale += "-" + (empPayScale.B32.ToString() + "-" + empPayScale.I32.ToString());

                if (empPayScale.B33 > 0 || empPayScale.I33 > 0)
                    payScale += "-" + (empPayScale.B33.ToString() + "-" + empPayScale.I33.ToString());

                if (empPayScale.B34 > 0 || empPayScale.I34 > 0)
                    payScale += "-" + (empPayScale.B34.ToString() + "-" + empPayScale.I34.ToString());

                if (empPayScale.B35 > 0 || empPayScale.I35 > 0)
                    payScale += "-" + (empPayScale.B35.ToString() + "-" + empPayScale.I35.ToString());

                if (empPayScale.B36 > 0 || empPayScale.I36 > 0)
                    payScale += "-" + (empPayScale.B36.ToString() + "-" + empPayScale.I36.ToString());

                if (empPayScale.B37 > 0 || empPayScale.I37 > 0)
                    payScale += "-" + (empPayScale.B37.ToString() + "-" + empPayScale.I37.ToString());

                if (empPayScale.B38 > 0 || empPayScale.I38 > 0)
                    payScale += "-" + (empPayScale.B38.ToString() + "-" + empPayScale.I38.ToString());

                if (empPayScale.B39 > 0 || empPayScale.I39 > 0)
                    payScale += "-" + (empPayScale.B39.ToString() + "-" + empPayScale.I39.ToString());

                if (empPayScale.B40 > 0 || empPayScale.I40 > 0)
                    payScale += "-" + (empPayScale.B40.ToString() + "-" + empPayScale.I40.ToString());

                payScale = payScale.Trim(new char[] { '-' });

                var dtoEmp = genericRepo.GetByID<DTOModel.tblMstEmployee>(empPayScale.employeeID);
                dtoEmp.PayScale = payScale;
                genericRepo.Update<DTOModel.tblMstEmployee>(dtoEmp);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool DeleteEmployee(int employeeID)
        {
            log.Info($"EmployeeService/DeleteEmployee/{employeeID}");
            bool flag = false;
            try
            {
                DTOModel.tblMstEmployee dtoEmployee = new DTOModel.tblMstEmployee();
                dtoEmployee = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID);
                dtoEmployee.IsDeleted = true;
                genericRepo.Update<DTOModel.tblMstEmployee>(dtoEmployee);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        public bool DeleteEmployeeDeputationInfo(int employeeID, int empDeputationID)
        {
            log.Info($"EmployeeService/DeleteEmployee/{employeeID}");
            bool flag = false;
            try
            {
                DTOModel.EmployeeDeputation dtoEmpDeputation = new DTOModel.EmployeeDeputation();
                dtoEmpDeputation = genericRepo.Get<DTOModel.EmployeeDeputation>(x => x.EmpDeputationID == empDeputationID).FirstOrDefault();
                dtoEmpDeputation.IsDeleted = true;
                genericRepo.Update<DTOModel.EmployeeDeputation>(dtoEmpDeputation);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;

        }
        public string GetEmployeeProfilePath(string empCode)
        {
            log.Info($"EmployeeService/GetEmployeeProfilePath/{empCode}");
            if (genericRepo.Exists<DTOModel.User>(x => x.UserName == empCode))
            {
                var empProfilePic = genericRepo.Get<DTOModel.User>(x => x.UserName == empCode).First().ImageName;
                if (string.IsNullOrEmpty(empProfilePic))
                    return Path.Combine(DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");
                else
                {
                    var filePath =

                  System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + empProfilePic)) ?

                   Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + empProfilePic) :
                   Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");

                    return filePath;
                }
            }
            else
            {
                return Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");
            }
        }

        public Model.EmployeeProfile GetEmployeeProfile(int employeeID)
        {
            log.Info($"EmployeeService/GetEmployeeProfile/employeeID={employeeID}");
            try
            {
                var dtoEmployee = genericRepo.Get<Data.Models.tblMstEmployee>(x => x.EmployeeId == employeeID).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.EmployeeProfile>()
                   .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeId))
                   .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmployeeCode))
                   .ForMember(c => c.EmpName, c => c.MapFrom(s => s.Name))
                   .ForMember(c => c.HBName, c => c.MapFrom(s => s.HBName))
                   .ForMember(c => c.Title, c => c.MapFrom(s => s.Title.TitleName))
                   .ForMember(c => c.PresentCity, c => c.MapFrom(s => s.PCity))
                   .ForMember(c => c.AcademicQualification, c => c.MapFrom(s => s.AcadmicProfessionalDetail.Value))
                   .ForMember(c => c.ProfessionalQualification, c => c.MapFrom(s => s.AcadmicProfessionalDetail1.Value))
                   .ForMember(c => c.QAcademicID, c => c.MapFrom(s => s.QAcademicID))
                   .ForMember(c => c.QProfessionalID, c => c.MapFrom(s => s.QProfessionalID))
                   .ForMember(c => c.SpecialSkills, c => c.MapFrom(s => s.SpecialSkills))
                   .ForMember(c => c.Gender, c => c.MapFrom(s => s.Gender.Name))
                   .ForMember(c => c.DOB, c => c.MapFrom(s => s.DOB))
                   .ForMember(c => c.PermanentCity, c => c.MapFrom(s => s.PmtCity))
                   .ForMember(c => c.PresentAddress, c => c.MapFrom(s => s.PAdd))
                   .ForMember(c => c.PermanentAddress, c => c.MapFrom(s => s.PmtAdd))
                   .ForMember(c => c.PresentPin, c => c.MapFrom(s => s.PPin))
                   .ForMember(c => c.PermanentPin, c => c.MapFrom(s => s.PmtPin))
                   .ForMember(c => c.MobileNo, c => c.MapFrom(s => s.MobileNo))
                   .ForMember(c => c.EmailID, c => c.MapFrom(s => s.OfficialEmail))
                   //.ForMember(c => c.SupervisorName, c => c.MapFrom(s =>
                   //s.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == 3 && y.ReportingTo == y.tblMstEmployee2.EmployeeId).tblMstEmployee2.Name))
                   .ForMember(c => c.BloodGroupID, c => c.MapFrom(s => s.BGroupID))
                   .ForMember(c => c.BloodGroupName, c => c.MapFrom(s => s.BloodGroup.BloodGroupName))
                   .ForMember(c => c.PANNo, c => c.MapFrom(s => s.PANNo))
                   .ForMember(c => c.AadhaarNo, c => c.MapFrom(s => s.AadhaarNo))
                   .ForMember(c => c.Category, c => c.MapFrom(s => s.Category.CategoryName))
                   .ForMember(c => c.Section, c => c.MapFrom(s => s.Section.SectionName))
                   .ForMember(c => c.Branch, c => c.MapFrom(s => s.Branch.BranchName))
                   .ForMember(c => c.Department, c => c.MapFrom(s => s.Department.DepartmentName))
                   .ForMember(c => c.Designation, c => c.MapFrom(s => s.Designation.DesignationName))
                   .ForMember(c => c.DOJ, c => c.MapFrom(s => s.Pr_Loc_DOJ))
                   //.ForMember(c => c.ReviewerName, c => c.MapFrom(s => s.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == 3 && y.ReviewingTo == y.tblMstEmployee3.EmployeeId).tblMstEmployee3.Name))
                   //.ForMember(c => c.AcceptanceAuthorityName, c => c.MapFrom(s => s.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == 3 && y.AcceptanceAuthority == y.tblMstEmployee1.EmployeeId).tblMstEmployee1.Name))
                   .ForMember(c => c.FileNo, c => c.MapFrom(s => s.FileNo))
                   .ForMember(c => c.IDMark, c => c.MapFrom(s => s.ID_Mark))
                   .ForMember(c => c.Sen_Code, c => c.MapFrom(s => s.Sen_Code))
                   .ForMember(c => c.PayScale, c => c.MapFrom(s => s.PayScale))
                   .ForMember(c => c.PassportNo, c => c.MapFrom(s => s.PassPortNo))
                   .ForMember(c => c.Nominee, c => c.MapFrom(s => s.GISNominee))
                   .ForMember(c => c.Relation, c => c.MapFrom(s => s.Relation.RelationName))
                  .ForMember(c => c.IncrementMonth, c => c.MapFrom(s => s.IncrementMonth))
                  .ForMember(c => c.PFNo, c => c.MapFrom(s => s.PFNO))
                   .ForMember(c => c.ACRNo, c => c.MapFrom(s => s.ACR_No))
                  .ForMember(c => c.UANNo, c => c.MapFrom(s => s.PensionUAN))
                   .ForMember(c => c.AadhaarCardFilePath, c => c.MapFrom(s => s.AadhaarCardFilePath))
                   .ForMember(c => c.PanCardFilePath, c => c.MapFrom(s => s.PanCardFilePath))
                   .ForMember(c => c.MotherName, c => c.MapFrom(s => s.MotherName))
                   .ForMember(c => c.IFSCCode, c => c.MapFrom(s => s.IFSCCode))

                   .ForMember(c => c.PresentStateID, c => c.MapFrom(s => s.PState))
                   .ForMember(c => c.PresentState, c => c.MapFrom(s => s.State1.StateName))
                    .ForMember(c => c.PmtStateID, c => c.MapFrom(s => s.PmtState))
                   .ForMember(c => c.PermanentState, c => c.MapFrom(s => s.State.StateName))
                   .ForMember(c => c.PensionNumber, c => c.MapFrom(s => s.PensionNumber))
                   .ForMember(c => c.EPFOMemberID, c => c.MapFrom(s => s.EPFOMemberID))
                   //  .ForMember(d => d.EmpProfilePhotoUNCPath, o => o.MapFrom(s => s.User.ImageName))
                   .ForAllOtherMembers(c => c.Ignore());

                    cfg.CreateMap<DTOModel.tblEmployeeQualificationdetail, AcadmicProfessionalDetailsModel>()
                    .ForMember(d => d.ID, o => o.MapFrom(s => s.AcadmicProfessionalDetail.ID))
                    .ForMember(d => d.TypeID, o => o.MapFrom(s => s.AcadmicProfessionalDetail.TypeID))
                    .ForMember(d => d.Value, o => o.MapFrom(s => s.AcadmicProfessionalDetail.Value))
                    .ForAllOtherMembers(d => d.Ignore());                    
                });
                var employee = Mapper.Map<DTOModel.tblMstEmployee, Model.EmployeeProfile>(dtoEmployee);
                List<Model.EmployeeProfile> res = new List<Model.EmployeeProfile>();
                var QualificationList = Mapper.Map<List<AcadmicProfessionalDetailsModel>>(dtoEmployee.tblEmployeeQualificationdetails);
                res.Add(employee);
                res.ForEach(x =>
                {
                    x.HBName = x.HBName != null && x.HBName.Trim().ToLower() == "null" ? null : x.HBName;
                    x.PANNo = x.PANNo != null && x.PANNo.Trim().ToLower() == "null" ? null : x.PANNo;
                    x.PermanentAddress = x.PermanentAddress != null && x.PermanentAddress.Trim().ToLower() == "null" ? null : x.PermanentAddress;
                    x.PresentAddress = x.PresentAddress != null && x.PresentAddress.Trim().ToLower() == "null" ? null : x.PresentAddress;
                    x.PresentCity = x.PresentCity != null && x.PresentCity.Trim().ToLower() == "null" ? null : x.PresentCity;
                    x.PresentPin = x.PresentPin != null && x.PresentPin.Trim().ToLower() == "null" ? null : x.PresentPin;
                    x.PermanentCity = x.PermanentCity != null && x.PermanentCity.Trim().ToLower() == "null" ? null : x.PermanentCity;
                    x.PermanentPin = x.PermanentPin != null && x.PermanentPin.Trim().ToLower() == "null" ? null : x.PermanentPin;
                    x.IDMark = x.IDMark != null && x.IDMark.Trim().ToLower() == "null" ? null : x.IDMark;
                    x.PayScale = x.PayScale != null && x.PayScale.Trim().ToLower() == "null" ? null : x.PayScale;
                    x.PassportNo = x.PassportNo != null && x.PassportNo.Trim().ToLower() == "null" ? null : x.PassportNo;
                });

                employee = res.FirstOrDefault();

                if (string.IsNullOrEmpty(employee.PayScale))
                {
                    var emp_payScale = empRepo.GetDesignationPayScaleList(dtoEmployee.DesignationID).FirstOrDefault();
                    employee.PayScale = emp_payScale?.PayScale ?? null;
                }

                employee.EmpProfilePhotoUNCPath = genericRepo.Get<DTOModel.User>(x => x.EmployeeID == employeeID).FirstOrDefault().ImageName;

                var LeaveApproverMangers = GetProcessApprovalManagers(employeeID, WorkFlowProcess.LeaveApproval);

                employee.SupervisorName = LeaveApproverMangers.Any(x => x.id == 1) ? LeaveApproverMangers.FirstOrDefault(x => x.id == 1).value : string.Empty;
                employee.ReviewerName = LeaveApproverMangers.Any(x => x.id == 2) ? LeaveApproverMangers.FirstOrDefault(x => x.id == 2).value : string.Empty;
                employee.AcceptanceAuthorityName = LeaveApproverMangers.Any(x => x.id == 3) ? LeaveApproverMangers.FirstOrDefault(x => x.id == 3).value : string.Empty;
                employee.QualificationList = QualificationList;




                #region ====== Get Employee Achievement & Certification list...=============

                employee.achievements = GetEmployeeAchievement(employeeID);
                employee.certifications = GetEmployeeCertification(employeeID);

                #endregion


                #region Get Insurance Detail
                var getInsurance = GetInsuranceByID(employeeID);
                #endregion
                employee.EmployeeInsurance = getInsurance;
                return employee;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool CheckEmployeeCodeExistance(string empCode)
        {
            log.Info($"EmployeeService/CheckEmployeeCodeExistance/empCode={empCode}");
            return genericRepo.Exists<DTOModel.tblMstEmployee>(x => x.EmployeeCode == empCode && !x.IsDeleted);
        }

        public string GetSupervisiorName(int employeeID)
        {
            log.Info($"EmployeeService/GetSupervisiorName/employeeID={employeeID}");
            return genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID).Name;
        }

        public int InsertTabletMarkAttendanceDetails(Model.EmpAttendance tabletProxyAttendanceDetails)
        {
            log.Info($"InsertTabletMarkAttendanceDetails");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmpAttendance, DTOModel.EmpAttendanceHdr>()
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                    .ForMember(c => c.InTime, c => c.MapFrom(s => s.InTime))
                    .ForMember(c => c.OutTime, c => c.MapFrom(s => s.OutTime))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.TypeID, c => c.MapFrom(s => s.TypeID))
                    .ForMember(c => c.Mode, c => c.MapFrom(s => s.Mode))
                    .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxyOutDate))
                    .ForMember(c => c.MarkedBy, c => c.MapFrom(s => s.MarkedBy))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.Attendancestatus, c => c.MapFrom(s => s.Attendancestatus))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtotabletProxyAttendance = Mapper.Map<DTOModel.EmpAttendanceHdr>(tabletProxyAttendanceDetails);
                genericRepo.Insert<DTOModel.EmpAttendanceHdr>(dtotabletProxyAttendance);
                return dtotabletProxyAttendance.EmpAttendanceID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public IEnumerable<Model.EmployeeDeputation> GetEmployeeDeputationDtls(int employeeID, int? empDeputationID)
        {
            IEnumerable<Model.EmployeeDeputation> obj = Enumerable.Empty<Model.EmployeeDeputation>();
            log.Info($"GetEmployeeDeputationDtls");
            try
            {
                var dtoEmpDeputation = genericRepo.Get<Data.Models.EmployeeDeputation>(x => !x.IsDeleted && x.EmployeeID == employeeID && (empDeputationID.HasValue ? (x.EmpDeputationID == empDeputationID.Value) : 1 > 0));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeDeputation, Model.EmployeeDeputation>()
                    .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeID))
                    .ForMember(c => c.FromDate, c => c.MapFrom(s => s.FromDate))
                    .ForMember(c => c.ToDate, c => c.MapFrom(s => s.ToDate))
                     .ForMember(c => c.EmpDeputationID, c => c.MapFrom(s => s.EmpDeputationID))
                    .ForMember(c => c.OrganizationName, c => c.MapFrom(s => s.OrganizationName))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                return Mapper.Map<List<Model.EmployeeDeputation>>(dtoEmpDeputation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool AddAndUpdateEmpDeputationInfo(Model.EmployeeDeputation empDeputation)
        {
            log.Info($"AddAndUpdateEmpDeputationInfo");
            bool flag = false;
            try
            {
                var result = genericRepo.Exists<Data.Models.EmployeeDeputation>(x => x.EmployeeID == empDeputation.EmployeeID &&
             ((empDeputation.FromDate >= x.FromDate && empDeputation.FromDate <= x.ToDate) || (empDeputation.ToDate >= x.FromDate && empDeputation.ToDate <= x.ToDate)));
                if (result && empDeputation.EmpDeputationID == 0)
                {
                    return flag;
                }
                else
                {
                    if (empDeputation.EmpDeputationID > 0)
                    {
                        var update = empRepo.UpdateEmployeeDeputation(empDeputation.EmpDeputationID, empDeputation.FromDate, empDeputation.ToDate, empDeputation.OrganizationName, empDeputation.CreatedBy);
                        flag = true;
                    }
                    else
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.EmployeeDeputation, DTOModel.EmployeeDeputation>()
                            .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                            .ForMember(d => d.FromDate, o => o.MapFrom(s => s.FromDate))
                            .ForMember(d => d.ToDate, o => o.MapFrom(s => s.ToDate))
                            .ForMember(d => d.OrganizationName, o => o.MapFrom(s => s.OrganizationName))
                            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                            .ForAllOtherMembers(d => d.Ignore());
                        });

                        var dtoInsertModel = Mapper.Map<DTOModel.EmployeeDeputation>(empDeputation);
                        genericRepo.Insert<DTOModel.EmployeeDeputation>(dtoInsertModel);
                        flag = true;
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }
            return flag;
        }

        public List<Model.Employee> GetEmployeeDetailsByCode(string empCode)
        {
            log.Info($"EmployeeService/GetEmployeeDetailsByCode/empCode={empCode}");
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && x.EmployeeCode == empCode).Select(em => new Model.Employee()
                {
                    EmployeeID = em.EmployeeId,
                    EmployeeCode = em.EmployeeCode,
                    TitleName = em.Title.TitleName,
                    Name = em.Name,
                    HBName = em.HBName,
                    MotherName = em.MotherName,
                    EmployeeTypeName = em.EmployeeType.EmployeeTypeName,
                    BranchID = em.BranchID,
                    BranchName = em.Branch.BranchName,
                    DepartmentID = em.DepartmentID == null ? 0 : (int?)em.DepartmentID,
                    DepartmentName = em.DepartmentID == null ? string.Empty : em.Department.DepartmentName,
                    DesignationID = em.DesignationID,
                    DesignationName = em.Designation.DesignationName,
                    DOB = em.DOB,
                    DOJ = em.DOJ,
                    PlaceOfJoin = em.PlaceOfJoin,
                    Gender = em.Gender.Name,
                    CategoryID = em.CategoryID == null ? 0 : (int?)em.CategoryID,
                    CategoryName = em.CategoryID == null ? string.Empty : em.Category.CategoryName,
                    ReligionID = em.ReligionID == null ? 0 : (int?)em.ReligionID,
                    ReligionName = em.ReligionID == null ? string.Empty : em.Religion.ReligionName,
                    MTongueID = em.MTongueID == null ? 0 : (int?)em.MTongueID,
                    MotherTongueName = em.MTongueID == null ? string.Empty : em.MotherTongue.Name,
                    BGroupID = em.BGroupID == null ? 0 : (int?)em.BGroupID,
                    BloodGroupName = em.BGroupID == null ? string.Empty : em.BloodGroup.BloodGroupName,
                    MaritalStsID = em.MaritalStsID == null ? 0 : (int?)em.MaritalStsID,
                    MaritalStatusName = em.MaritalStsID == null ? string.Empty : em.MaritalStatu.MaritalStatusName,
                    EmpCatID = em.EmpCatID == null ? 0 : (int?)em.EmpCatID,
                    EmplCatName = em.EmpCatID == null ? string.Empty : em.EmployeeCategory.EmplCatName,
                    CadreID = em.CadreID == null ? 0 : (int?)em.CadreID,
                    CadreName = em.CadreID == null ? string.Empty : em.Cadre.CadreName,
                    CadreRank = em.CadreRank,
                    DivisionID = em.DivisionID == null ? 0 : (int?)em.DivisionID,
                    DivisionName = em.DivisionID == null ? string.Empty : em.Division.DivisionName,
                    SectionID = em.SectionID == null ? 0 : (int?)em.SectionID,
                    SectionName = em.SectionID == null ? string.Empty : em.Section.SectionName,
                    Pr_Loc_DOJ = em.Pr_Loc_DOJ,
                    S_DOJ = em.S_DOJ,
                    Pr_desg = em.Pr_desg,
                    Sen_Code = em.Sen_Code,
                    P_Scale = em.P_Scale,
                    ID_Mark = em.ID_Mark,
                    PAdd = em.PAdd,
                    PStreet = em.PStreet,
                    PCity = em.PCity,
                    PPin = em.PPin,
                    TelPh = em.TelPh,
                    PmtAdd = em.PmtAdd,
                    PmtCity = em.PmtCity,
                    PmtPin = em.PmtPin,
                    PmtStreet = em.PmtStreet,
                    FileNo = em.FileNo,
                    PFNO = em.PFNO,
                    Folio_No = em.Folio_No,
                    ACR_No = em.ACR_No,
                    SL_No = em.SL_No,
                    PANNo = em.PANNo,
                    ConfirmationDate = em.ConfirmationDate,
                    DOSupAnnuating = em.DOSupAnnuating,
                    PassPortNo = em.PassPortNo,
                    PPEDate = em.PPEDate,
                    PPIDate = em.PPIDate,
                    GISNominee = em.GISNominee,
                    NomineeRelationID = em.NomineeRelationID == null ? 0 : (int?)em.NomineeRelationID,
                    RelationName = em.NomineeRelationID == null ? string.Empty : em.Relation.RelationName,
                    IncrementDate = em.IncrementDate,
                    DateOfRetirement = em.DateOfRetirement,
                    GraAssuranceNo = em.GraAssuranceNo,
                    SpecialSkills = em.SpecialSkills,
                    Specialincr = em.Specialincr,
                    DOSpecialIncr = em.DOSpecialIncr,
                    PromotionDate = em.PromotionDate,
                    orderofpromotion = em.orderofpromotion,
                    PeriodAppointment = em.PeriodAppointment,
                    ExtnAppointment = em.ExtnAppointment,
                    PerfLCT = em.PerfLCT,
                    IncrementMonth = em.IncrementMonth,
                    ValidateIncrement = em.ValidateIncrement ?? false,
                    Reason = em.Reason,
                    Dept_Enq = em.Dept_Enq ?? false,
                    Cer_Given = em.Cer_Given ?? false,
                    Inv_Issued = em.Inv_Issued ?? false,
                    Books_LIB = em.Books_LIB ?? false,
                    DOLeaveOrg = em.DOLeaveOrg,
                    ReasonOfLeaving = em.ReasonOfLeaving,
                    CadOfEmp = em.CadOfEmp,
                    Conv = em.Conv,
                    HBA = em.HBA,
                    Zone = em.Zone,
                    IsForceFully = em.IsForceFully ?? false,
                    IsBM = em.IsBM ?? false,
                    IsCadreOfficer = em.IsCadreOfficer,
                    IsDismissal = em.IsDismissal ?? false,
                    IsExpire = em.IsExpire ?? false,
                    IsResigned = em.IsResigned ?? false,
                    IsRM = em.IsRM ?? false,
                    IsSalgenrated = em.IsSalgenrated,
                    IsTermination = em.IsTermination ?? false,
                    IsVRS = em.IsVRS ?? false,
                    PayScale = em.PayScale,
                    ByLCT = em.ByLCT,
                    //ReportingToID = em.ReportingTo,
                    CreatedOn = em.CreatedOn,
                    CreatedBy = em.CreatedBy ?? 0,
                    UpdatedOn = em.UpdatedOn ?? null,
                    UpdatedBy = em.UpdatedBy ?? 0,
                    OfficialEmail = em.OfficialEmail,
                    PersonalEmail = em.PersonalEmail,
                    VisaValidUpTo = em.VisaValidUpTo,
                    WorkfromHome = em.WorkfromHome,
                    BiometricID = em.BiometricID,
                    MobileNo = em.MobileNo,
                    AadhaarNo = em.AadhaarNo,
                    PanCardFilePath = em.PanCardFilePath,
                    AadhaarCardFilePath = em.AadhaarCardFilePath,
                    IFSCCode = em.IFSCCode

                }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool UpdatetEmployeePersonalDetails(Model.Employee employeeDetails)
        {
            log.Info($"EmployeeService/UpdatetEmployeePersonalDetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeDetails.EmployeeID);
                dtoObj.HBName = employeeDetails.HBName;
                dtoObj.DOB = employeeDetails.DOB;
                dtoObj.ReligionID = employeeDetails.ReligionID == 0 ? null : employeeDetails.ReligionID;
                dtoObj.MTongueID = employeeDetails.MTongueID == 0 ? null : employeeDetails.MTongueID;
                dtoObj.MaritalStsID = employeeDetails.MaritalStsID == 0 ? null : employeeDetails.MaritalStsID;
                dtoObj.BGroupID = employeeDetails.BGroupID == 0 ? null : employeeDetails.BGroupID;
                dtoObj.ID_Mark = employeeDetails.ID_Mark;
                dtoObj.PANNo = employeeDetails.PANNo;
                dtoObj.AadhaarNo = employeeDetails.AadhaarNo;
                dtoObj.PassPortNo = employeeDetails.PassPortNo;
                dtoObj.PPIDate = employeeDetails.PPIDate;
                dtoObj.PPEDate = employeeDetails.PPEDate;
                dtoObj.PAdd = employeeDetails.PAdd;
                dtoObj.PStreet = employeeDetails.PStreet;
                dtoObj.PCity = employeeDetails.PCity;
                dtoObj.PPin = employeeDetails.PPin;
                dtoObj.TelPh = employeeDetails.TelPh;
                dtoObj.PmtAdd = employeeDetails.PmtAdd;
                dtoObj.PmtStreet = employeeDetails.PmtStreet;
                dtoObj.PmtCity = employeeDetails.PmtCity;
                dtoObj.PmtPin = employeeDetails.PmtPin;
                dtoObj.AadhaarCardFilePath = employeeDetails.AadhaarCardFilePath;
                dtoObj.PanCardFilePath = employeeDetails.PanCardFilePath;
                dtoObj.MotherName = employeeDetails.MotherName;
                dtoObj.IFSCCode = employeeDetails.IFSCCode;
                dtoObj.UpdatedBy = employeeDetails.UpdatedBy;
                dtoObj.UpdatedOn = employeeDetails.UpdatedOn;
                dtoObj.PState = employeeDetails.PState == 0 ? null : employeeDetails.PState;
                dtoObj.PmtState = employeeDetails.PmtState == 0 ? null : employeeDetails.PmtState;
                dtoObj.PensionUAN = employeeDetails.PensionUAN;
                dtoObj.PensionNumber = employeeDetails.PensionNumber;
                dtoObj.EPFOMemberID = employeeDetails.EPFOMemberID;
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

        public Model.Employee GetEmployeeByID(int employeeID)
        {
            log.Info($"EmployeeService/GetEmployeeByID/{employeeID}");
            try
            {
                var dtoEmployee = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblMstEmployee, Model.Employee>()
                    .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeId))
                    //.ForMember(c => c.ReportingToID, c => c.MapFrom(s => s.ReportingTo))
                    //.ForMember(c => c.ReviewerTo, c => c.MapFrom(s => s.ReviewerTo))
                    .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.EmployeeCode))
                    .ForMember(c => c.TitleID, c => c.MapFrom(s => s.TitleID))
                    .ForMember(c => c.Name, c => c.MapFrom(s => s.Name))
                    .ForMember(c => c.EmployeeTypeID, c => c.MapFrom(s => s.EmployeeTypeID))
                    .ForMember(c => c.GenderID, c => c.MapFrom(s => s.GenderID))
                    .ForMember(c => c.DesignationID, c => c.MapFrom(s => s.DesignationID))
                    .ForMember(c => c.CadreID, c => c.MapFrom(s => s.CadreID))
                    .ForMember(c => c.IsCadreOfficer, c => c.MapFrom(s => s.IsCadreOfficer))
                    .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                    .ForMember(c => c.BranchCode, c => c.MapFrom(s => s.Branch.BranchCode))
                    .ForMember(c => c.DepartmentID, c => c.MapFrom(s => s.DepartmentID))
                    .ForMember(c => c.SectionID, c => c.MapFrom(s => s.SectionID))
                    .ForMember(c => c.FirstBranch, c => c.MapFrom(s => s.FirstBranchID))
                    .ForMember(c => c.FirstDesg, c => c.MapFrom(s => s.FirstDesgID))
                    .ForMember(c => c.DOJ, c => c.MapFrom(s => s.DOJ))
                    .ForMember(c => c.S_DOJ, c => c.MapFrom(s => s.S_DOJ))
                    .ForMember(c => c.Pr_Loc_DOJ, c => c.MapFrom(s => s.Pr_Loc_DOJ))
                    .ForMember(c => c.SL_No, c => c.MapFrom(s => s.SL_No))
                    .ForMember(c => c.ConfirmationDate, c => c.MapFrom(s => s.ConfirmationDate))
                    .ForMember(c => c.Sen_Code, c => c.MapFrom(s => s.Sen_Code))
                    .ForMember(c => c.FileNo, c => c.MapFrom(s => s.FileNo))
                    .ForMember(c => c.CategoryID, c => c.MapFrom(s => s.CategoryID))
                    .ForMember(c => c.DOB, c => c.MapFrom(s => s.DOB))
                    .ForMember(c => c.HBName, c => c.MapFrom(s => s.HBName))
                    .ForMember(c => c.ReligionID, c => c.MapFrom(s => s.ReligionID))
                    .ForMember(c => c.MTongueID, c => c.MapFrom(s => s.MTongueID))
                    .ForMember(c => c.MaritalStsID, c => c.MapFrom(s => s.MaritalStsID))
                    .ForMember(c => c.BGroupID, c => c.MapFrom(s => s.BGroupID))
                    .ForMember(c => c.ID_Mark, c => c.MapFrom(s => s.ID_Mark))
                    .ForMember(c => c.PANNo, c => c.MapFrom(s => s.PANNo))
                    .ForMember(c => c.PassPortNo, c => c.MapFrom(s => s.PassPortNo))
                    .ForMember(c => c.PPIDate, c => c.MapFrom(s => s.PPIDate))
                    .ForMember(c => c.PPEDate, c => c.MapFrom(s => s.PPEDate))
                    .ForMember(c => c.PAdd, c => c.MapFrom(s => s.PAdd))
                    .ForMember(c => c.PStreet, c => c.MapFrom(s => s.PStreet))
                    .ForMember(c => c.PCity, c => c.MapFrom(s => s.PCity))
                    .ForMember(c => c.PPin, c => c.MapFrom(s => s.PPin))
                    .ForMember(c => c.TelPh, c => c.MapFrom(s => s.TelPh))
                    .ForMember(c => c.PmtAdd, c => c.MapFrom(s => s.PmtAdd))
                    .ForMember(c => c.PmtStreet, c => c.MapFrom(s => s.PmtStreet))
                    .ForMember(c => c.PmtCity, c => c.MapFrom(s => s.PmtCity))
                    .ForMember(c => c.PmtPin, c => c.MapFrom(s => s.PmtPin))
                    .ForMember(c => c.PFNO, c => c.MapFrom(s => s.PFNO))
                    .ForMember(c => c.Folio_No, c => c.MapFrom(s => s.Folio_No))
                    .ForMember(c => c.ACR_No, c => c.MapFrom(s => s.ACR_No))
                    .ForMember(c => c.GraAssuranceNo, c => c.MapFrom(s => s.GraAssuranceNo))
                    .ForMember(c => c.PeriodAppointment, c => c.MapFrom(s => s.PeriodAppointment))
                    .ForMember(c => c.ExtnAppointment, c => c.MapFrom(s => s.ExtnAppointment))
                    .ForMember(c => c.GISNominee, c => c.MapFrom(s => s.GISNominee))
                    .ForMember(c => c.GISNominee, c => c.MapFrom(s => s.GISNominee))
                    .ForMember(c => c.NomineeRelationID, c => c.MapFrom(s => s.NomineeRelationID))
                    .ForMember(c => c.PassPortNo, c => c.MapFrom(s => s.PassPortNo))
                    .ForMember(c => c.ReasonOfLeaving, c => c.MapFrom(s => s.ReasonOfLeaving))
                    .ForMember(c => c.Dept_Enq, c => c.MapFrom(s => s.Dept_Enq))
                    .ForMember(c => c.Cer_Given, c => c.MapFrom(s => s.Cer_Given))
                    .ForMember(c => c.Inv_Issued, c => c.MapFrom(s => s.Inv_Issued))
                    .ForMember(c => c.Books_LIB, c => c.MapFrom(s => s.Books_LIB))
                    .ForMember(c => c.DOSupAnnuating, c => c.MapFrom(s => s.DOSupAnnuating))
                    .ForMember(c => c.DOLeaveOrg, c => c.MapFrom(s => s.DOLeaveOrg))
                    .ForMember(c => c.OtaCode, c => c.MapFrom(s => s.OtaCode))
                    .ForMember(c => c.IsVRS, c => c.MapFrom(s => s.IsVRS))
                    .ForMember(c => c.IsDismissal, c => c.MapFrom(s => s.IsDismissal))
                    .ForMember(c => c.IsTermination, c => c.MapFrom(s => s.IsTermination))
                    .ForMember(c => c.IsBM, c => c.MapFrom(s => s.IsBM))
                    .ForMember(c => c.IsRM, c => c.MapFrom(s => s.IsRM))
                    .ForMember(c => c.IsExpire, c => c.MapFrom(s => s.IsExpire))
                    .ForMember(c => c.IsForceFully, c => c.MapFrom(s => s.IsForceFully))
                    .ForMember(c => c.IsResigned, c => c.MapFrom(s => s.IsResigned))
                    .ForMember(c => c.CategoryID, c => c.MapFrom(s => s.CategoryID))
                    .ForMember(c => c.AadhaarNo, c => c.MapFrom(s => s.AadhaarNo))
                    .ForMember(c => c.OfficialEmail, c => c.MapFrom(s => s.OfficialEmail))
                    .ForMember(c => c.PersonalEmail, c => c.MapFrom(s => s.PersonalEmail))
                    .ForMember(c => c.MobileNo, c => c.MapFrom(s => s.MobileNo))
                    .ForMember(c => c.IncrementMonth, c => c.MapFrom(s => s.IncrementMonth))
                    .ForMember(c => c.IncrementDate, c => c.MapFrom(s => s.IncrementDate))
                    .ForMember(c => c.ValidateIncrement, c => c.MapFrom(s => s.ValidateIncrement))
                    .ForMember(c => c.Reason, c => c.MapFrom(s => s.Reason))
                    .ForMember(c => c.PromotionDate, c => c.MapFrom(s => s.PromotionDate))
                    .ForMember(c => c.PerfLCT, c => c.MapFrom(s => s.PerfLCT))
                    .ForMember(c => c.orderofpromotion, c => c.MapFrom(s => s.orderofpromotion))
                    .ForMember(c => c.QAcademicID, c => c.MapFrom(s => s.QAcademicID))
                    .ForMember(c => c.QProfessionalID, c => c.MapFrom(s => s.QProfessionalID))
                    .ForMember(c => c.SpecialSkills, c => c.MapFrom(s => s.SpecialSkills))
                    .ForMember(c => c.PanCardFilePath, c => c.MapFrom(s => s.PanCardFilePath))
                    .ForMember(c => c.AadhaarCardFilePath, c => c.MapFrom(s => s.AadhaarCardFilePath))
                    .ForMember(c => c.MotherName, c => c.MapFrom(s => s.MotherName))
                    .ForMember(c => c.IFSCCode, c => c.MapFrom(s => s.IFSCCode))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.BranchName, c => c.MapFrom(s => s.Branch.BranchName))
                    .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.Designation.DesignationName))
                    .ForMember(c => c.PState, c => c.MapFrom(s => s.PState))
                    .ForMember(c => c.PresentState, c => c.MapFrom(s => s.State1.StateName))
                    .ForMember(c => c.PmtState, c => c.MapFrom(s => s.PmtState))
                    .ForMember(c => c.PermenantState, c => c.MapFrom(s => s.State.StateName))
                    .ForMember(c => c.Gender, c => c.MapFrom(s => s.Gender.Name))
                    .ForMember(c => c.PensionNumber, c => c.MapFrom(s => s.PensionNumber))
                    .ForMember(c => c.EPFOMemberID, c => c.MapFrom(s => s.EPFOMemberID))
                    .ForMember(c => c.EmpPayScale, c => c.MapFrom(s => s.PayScale))
                    .ForMember(c => c.IsJoinAfterNoon, c => c.MapFrom(s => s.IsJoinAfterNoon))
                     .ForMember(c => c.EmpCatID, c => c.MapFrom(s => s.EmpCatID))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var employee = Mapper.Map<DTOModel.tblMstEmployee, Model.Employee>(dtoEmployee);

                //if(genericRepo.Exists<DTOModel.TblMstEmployeeSalary>(x=>x.EmployeeCode== employee.EmployeeCode))
                //  employee.E_Basic = genericRepo.GetByID<DTOModel.TblMstEmployeeSalary>(employee.EmployeeCode).E_Basic;
                if (string.IsNullOrEmpty(employee.PayScale))
                {
                    var emp_payScale = empRepo.GetDesignationPayScaleList(dtoEmployee.DesignationID).FirstOrDefault();
                    employee.PayScale = emp_payScale?.PayScale ?? null;
                }
                return employee;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.Employee> GetEmployeesByBranchID(int? branchID)
        {
            log.Info($"EmployeeService/GetEmployeeList");
            try
            {

                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && (branchID == 0 ? (1 > 0) : x.BranchID == (int)branchID));

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.Employee>()
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.EmployeeTypeName, o => o.MapFrom(s => s.EmployeeType.EmployeeTypeName))
                    .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
                    .ForMember(d => d.Sen_Code, o => o.MapFrom(s => s.Sen_Code))
                    .ForMember(d => d.E_Basic, o => o.MapFrom(s => s.TblMstEmployeeSalaries.FirstOrDefault().E_Basic))
                    //  .ForMember(d => d.EmpProfilePhotoUNCPath, o => o.MapFrom(s => s.))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var lstEmployee = Mapper.Map<List<Model.Employee>>(result);
                return lstEmployee.OrderBy(x => x.EmployeeCode).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region Employee Salary Related functions ======================

        public bool InsertUpdateEmployeeSalaryInfo(Model.EmployeeSalary empSalary)
        {
            log.Info($"EmployeeService/InsertUpdateEmployeeSalaryInfo");
            bool flag = false;
            try
            {
                if (!genericRepo.Exists<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeCode == empSalary.EmployeeCode))
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.EmployeeSalary, DTOModel.TblMstEmployeeSalary>()
                        .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                         .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                        .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                        .ForMember(d => d.E_Basic, o => o.MapFrom(s => s.E_Basic))
                        .ForMember(d => d.DELETEDEMPLOYEE, o => o.UseValue(false))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(System.DateTime.Now))
                        //.ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.DepartmentName))
                        //.ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
                        //.ForMember(d => d.EmpProfilePhotoUNCPath, o => o.MapFrom(s => s.))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    var insertedEmpSalaryData = Mapper.Map<DTOModel.TblMstEmployeeSalary>(empSalary);
                    genericRepo.Insert<DTOModel.TblMstEmployeeSalary>(insertedEmpSalaryData);
                    flag = true;
                }
                else
                {
                    //Mapper.Initialize(cfg =>
                    //{
                    //    cfg.CreateMap<Model.EmployeeSalary, DTOModel.TblMstEmployeeSalary>()
                    //    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    //    .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                    //    .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                    //    .ForMember(d => d.E_Basic, o => o.MapFrom(s => s.E_Basic))
                    //    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    //    .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                    //    .ForMember(d => d.UpdatedOn, o => o.UseValue(System.DateTime.Now))
                    //    //.ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.DepartmentName))
                    //    //.ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
                    //    //.ForMember(d => d.EmpProfilePhotoUNCPath, o => o.MapFrom(s => s.))
                    //    .ForAllOtherMembers(d => d.Ignore());
                    //});
                    //var updatedEmpSalaryData = Mapper.Map<DTOModel.TblMstEmployeeSalary>(empSalary);

                    var empSalaryDetails = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeCode == empSalary.EmployeeCode).FirstOrDefault();
                    empSalaryDetails.BranchID = empSalary.BranchID;
                    empSalaryDetails.BranchCode = empSalary.BranchCode;
                    empSalaryDetails.E_Basic = empSalary.E_Basic;
                    empSalaryDetails.UpdatedBy = empSalary.UpdatedBy;
                    empSalaryDetails.UpdatedOn = empSalary.UpdatedOn;
                    genericRepo.Update<DTOModel.TblMstEmployeeSalary>(empSalaryDetails);
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
        public Model.EmployeeSalary GetEmployeeSalaryDtls(string empCode)
        {
            Model.EmployeeSalary objEmpSalary = new Model.EmployeeSalary();
            log.Info($"EmployeeService/GetEmployeeSalaryDtls/{empCode}");
            try
            {
                var dtoEmpSalary = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeCode == empCode);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TblMstEmployeeSalary, Model.EmployeeSalary>()

                      .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.SuspensionPeriods, o => o.MapFrom(s => s.tblMstEmployee.EmployeeSuspensionPeriods.Where(x => x.EmployeeID == s.EmployeeID)))
                   .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                   .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                   .ForMember(d => d.HRA, o => o.MapFrom(s => s.HRA))
                   .ForMember(d => d.IsFlatDeduction, o => o.MapFrom(s => s.IsFlatDeduction))
                   .ForMember(d => d.None, o => o.MapFrom(s => s.None))
                   .ForMember(d => d.E_Basic, o => o.MapFrom(s => s.E_Basic))
                   .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                   .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                   .ForMember(d => d.CCA, o => o.MapFrom(s => s.CCA))
                   .ForMember(d => d.WASHING, o => o.MapFrom(s => s.WASHING))
                   .ForMember(d => d.UnionFee, o => o.MapFrom(s => s.UnionFee))
                   .ForMember(d => d.ProfTax, o => o.MapFrom(s => s.ProfTax))
                   .ForMember(d => d.SportClub, o => o.MapFrom(s => s.SportClub))
                   .ForMember(d => d.IsRateVPF, o => o.MapFrom(s => s.IsRateVPF))
                   .ForMember(d => d.OTACode, o => o.MapFrom(s => s.OTACode))
                   .ForMember(d => d.VPFValueRA, o => o.MapFrom(s => s.VPFValueRA))
                   .ForMember(d => d.IsSuspended, o => o.MapFrom(s => s.IsSuspended))
                   .ForMember(d => d.IsOTACode, o => o.MapFrom(s => s.IsOTACode))
                   .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                   .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                   .ForMember(d => d.D_VPF, o => o.MapFrom(s => s.D_VPF))
                   .ForMember(d => d.IsPensionDeducted, o => o.MapFrom(s => s.IsPensionDeducted))
                   .ForMember(d => d.BasicSalPercentageForSuspendedEmp, o => o.MapFrom(s => s.BasicSalPercentageForSuspendedEmp))
                   .ForMember(d => d.E_Basic_Pay, o => o.MapFrom(s => s.E_Basic_Pay))
                   .ForMember(d => d.BranchID_Pay, o => o.MapFrom(s => s.tblMstEmployee.BranchID_Pay))
                   .ForMember(d => d.DOJ, o => o.MapFrom(s => s.tblMstEmployee.DOJ))
                   .ForMember(d => d.DOLeaveOrg, o => o.MapFrom(s => s.tblMstEmployee.DOLeaveOrg))
                   .ForAllOtherMembers(d => d.Ignore());

                    cfg.CreateMap<DTOModel.EmployeeSuspensionPeriod, EmployeeSuspensionPeriod>()
                    ;

                });
                objEmpSalary = Mapper.Map<Model.EmployeeSalary>(dtoEmpSalary.FirstOrDefault());

                return objEmpSalary;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public bool InsertUpdateEmployeeSalary(Model.EmployeeSalary empSalary)
        {

            log.Info($"EmployeeService/InsertUpdateEmployeeSalary");
            bool flag = false;
            try
            {
                if (empSalary.EmployeeCode != null)
                {

                    var dtoEmpSalary = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeCode == empSalary.EmployeeCode).FirstOrDefault();
                    if (dtoEmpSalary != null)
                    {

                        dtoEmpSalary.HRA = empSalary.HRA;
                        dtoEmpSalary.IsFlatDeduction = empSalary.IsFlatDeduction;
                        dtoEmpSalary.None = empSalary.None;
                        dtoEmpSalary.E_Basic = empSalary.E_Basic;
                        dtoEmpSalary.BranchID = empSalary.BranchID;
                        dtoEmpSalary.BranchCode = empSalary.BranchCode;
                        dtoEmpSalary.CCA = empSalary.CCA;
                        dtoEmpSalary.WASHING = empSalary.WASHING;
                        dtoEmpSalary.UnionFee = empSalary.UnionFee;
                        dtoEmpSalary.ProfTax = empSalary.ProfTax;
                        dtoEmpSalary.SportClub = empSalary.SportClub;
                        dtoEmpSalary.IsRateVPF = empSalary.IsRateVPF;
                        dtoEmpSalary.OTACode = empSalary.OTACode;
                        dtoEmpSalary.IsOTACode = empSalary.IsOTACode;
                        dtoEmpSalary.D_VPF = empSalary.D_VPF;
                        dtoEmpSalary.VPFValueRA = empSalary.VPFValueRA;
                        dtoEmpSalary.IsSuspended = empSalary.IsSuspended;
                        dtoEmpSalary.BasicSalPercentageForSuspendedEmp = empSalary.BasicSalPercentageForSuspendedEmp;
                        dtoEmpSalary.E_02 = empSalary.E_02;
                        dtoEmpSalary.E_07 = empSalary.E_07;
                        dtoEmpSalary.E_05 = empSalary.E_05;
                        dtoEmpSalary.D_02 = empSalary.D_02;
                        dtoEmpSalary.D_06 = empSalary.D_06;
                        dtoEmpSalary.D_07 = empSalary.D_07;
                        dtoEmpSalary.D_14 = empSalary.D_14;
                        dtoEmpSalary.D_VPF = empSalary.D_VPF;
                        dtoEmpSalary.IsPensionDeducted = empSalary.IsPensionDeducted;
                        dtoEmpSalary.UpdatedBy = empSalary.UpdatedBy;
                        dtoEmpSalary.UpdatedOn = DateTime.Now;
                        dtoEmpSalary.E_Basic_Pay = (empSalary.E_Basic_Pay.HasValue && empSalary.E_Basic_Pay.Value == 0) ? null : empSalary.E_Basic_Pay;
                    }
                    /// ===-= Update Code =====
                    //Mapper.Initialize(cfg =>
                    //{
                    //    cfg.CreateMap<Model.EmployeeSalary, DTOModel.TblMstEmployeeSalary>()
                    //   .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    //   .ForMember(d => d.HRA, o => o.MapFrom(s => s.HRA))
                    //   .ForMember(d => d.IsFlatDeduction, o => o.MapFrom(s => s.IsFlatDeduction))
                    //   .ForMember(d => d.None, o => o.MapFrom(s => s.None))
                    //   .ForMember(d => d.E_Basic, o => o.MapFrom(s => s.E_Basic))
                    //   .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                    //    .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                    //   .ForMember(d => d.CCA, o => o.MapFrom(s => s.CCA))
                    //   .ForMember(d => d.WASHING, o => o.MapFrom(s => s.WASHING))
                    //   .ForMember(d => d.UnionFee, o => o.MapFrom(s => s.UnionFee))
                    //   .ForMember(d => d.ProfTax, o => o.MapFrom(s => s.ProfTax))
                    //   .ForMember(d => d.SportClub, o => o.MapFrom(s => s.SportClub))
                    //   .ForMember(d => d.IsRateVPF, o => o.MapFrom(s => s.IsRateVPF))
                    //   .ForMember(d => d.OTACode, o => o.MapFrom(s => s.OTACode))
                    //   .ForMember(d => d.VPFValueRA, o => o.MapFrom(s => s.VPFValueRA))
                    //   .ForMember(d => d.IsSuspended, o => o.MapFrom(s => s.IsSuspended))
                    //   .ForMember(d => d.IsOTACode, o => o.MapFrom(s => s.IsOTACode))
                    //   .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    //   .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    //   .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                    //   .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                    //   .ForMember(d => d.BasicSalPercentageForSuspendedEmp, o => o.MapFrom(s => s.BasicSalPercentageForSuspendedEmp))
                    //  .ForAllOtherMembers(d => d.Ignore());
                    //});
                    //var dtoEmpSalary = Mapper.Map<DTOModel.TblMstEmployeeSalary>(empSalary);

                    //genericRepo.Update<DTOModel.TblMstEmployeeSalary>(dtoEmpSalary);

                    genericRepo.Update<DTOModel.TblMstEmployeeSalary>(dtoEmpSalary);
                    #region --- Update BranchID_Pay column in tblmstEmployee table for Salary process
                    var getEmpDtl = genericRepo.GetIQueryable<DTOModel.tblMstEmployee>(x => x.EmployeeCode == empSalary.EmployeeCode).FirstOrDefault();
                    if (getEmpDtl != null)
                    {
                        getEmpDtl.BranchID_Pay = empSalary.BranchID_Pay;
                        genericRepo.Update(getEmpDtl);
                    }
                    #endregion

                    #region  ====  Add Employee Suspension Period Record  ==== (Added on - 27 -Aug-2020)
                    if (empSalary.SuspensionFromData.HasValue && empSalary.SuspensionToData.HasValue)
                    {
                        EmployeeSuspensionPeriod empSuspensionPeriod = new EmployeeSuspensionPeriod();
                        empSuspensionPeriod.EmployeeID = dtoEmpSalary.EmployeeID ?? 0;
                        empSuspensionPeriod.PeriodFrom = empSalary.SuspensionFromData.Value;
                        empSuspensionPeriod.PeriodTo = empSalary.SuspensionToData.Value;
                        empSuspensionPeriod.CreatedBy = empSalary.UpdatedBy.Value;
                        empSuspensionPeriod.CreatedOn = empSalary.UpdatedOn.Value;
                        empSuspensionPeriod.BasicSalaryPercentage = (double)empSalary.BasicSalPercentageForSuspendedEmp.Value;

                        ManageEmpSuspensionPeriod(empSuspensionPeriod);
                    }
                    #endregion

                    flag = true;
                }
                else
                {
                    /// ===-= Insert Code =====
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
        #endregion

        #region Employee Approval Process
        public List<EmployeeProcessApproval> GetEmpApprovalProcesses(int employeeID)
        {
            log.Info($"EmployeeService/GetEmpApprovalProcesses/employeeID={employeeID}");
            try
            {
                var processList = new[] { (int)WorkFlowProcess.LeaveApproval, (int)WorkFlowProcess.AttendanceApproval, (int)WorkFlowProcess.Appraisal,
                    (int)WorkFlowProcess.LTC, (int)WorkFlowProcess.Competency,(int)WorkFlowProcess.LoanApplication,(int)WorkFlowProcess.ConveyanceBill,
                    (int)WorkFlowProcess.EPFNomination,
                 (int)WorkFlowProcess.Form11,(int)WorkFlowProcess.NonRefundablePFLoan
                };
                List<EmployeeProcessApproval> empProcessApproval = new List<EmployeeProcessApproval>();
                if (genericRepo.Exists<DTOModel.Process>(x => x.EmployeeProcessApprovals.FirstOrDefault(y => y.EmployeeID == employeeID).EmployeeID > 0))
                {
                    var dtoEmpProcessModel = genericRepo.Get<DTOModel.EmployeeProcessApproval>(x => x.EmployeeID == employeeID && x.ToDate == null && processList.Contains(x.ProcessID)).ToList();
                    var dtoprocess = genericRepo.Get<DTOModel.Process>(x => processList.Contains(x.ProcessID)).ToList();

                    empProcessApproval = (from left in dtoprocess
                                          join right in dtoEmpProcessModel on left.ProcessID equals right.ProcessID into joinedList
                                          from sub in joinedList.DefaultIfEmpty()

                                          select new EmployeeProcessApproval()
                                          {
                                              ProcessName = left.ProcessName,
                                              OldReportingTo = sub == null ? 0 : sub.ReportingTo,
                                              OldReviewingTo = sub == null ? 0 : sub.ReviewingTo,
                                              OldAcceptanceAuthority = sub == null ? 0 : sub.AcceptanceAuthority,
                                              ReportingTo = sub == null ? 0 : sub.ReportingTo,
                                              ReviewingTo = sub == null ? 0 : sub.ReviewingTo,
                                              AcceptanceAuthority = sub == null ? 0 : sub.AcceptanceAuthority,
                                              EmpProcessAppID = sub == null ? 0 : sub.EmpProcessAppID,
                                              RoleID = sub == null ? 0 : sub.RoleID,
                                              CreatedBy = sub == null ? 0 : sub.CreatedBy,
                                              CreatedOn = sub == null ? DateTime.Now : sub.CreatedOn,
                                              ProcessID = sub == null ? left.ProcessID : sub.ProcessID,
                                              EmployeeID = sub == null ? employeeID : sub.EmployeeID


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
                        .ForAllOtherMembers(d => d.Ignore())
                        ;

                    });
                    empProcessApproval = Mapper.Map<List<Model.EmployeeProcessApproval>>(res1);
                }

                return empProcessApproval;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool InsertEmpProcessApproval(List<Model.EmployeeProcessApproval> empProcessApproval)
        {
            log.Info($"EmployeeService/InsertEmpProcessApproval");
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
                if (dtoProcessApproval != null)
                    genericRepo.AddMultipleEntity<DTOModel.EmployeeProcessApproval>(dtoProcessApproval);

                #region Confirmation Form Insert
                var insertDetail = empProcessApproval.Select(x => new { employeeID = x.EmployeeID, createdBy = x.CreatedBy }).FirstOrDefault();
                InsertEmployeeConfirmationDetails(insertDetail.employeeID, insertDetail.createdBy);
                #endregion
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }
            return flag;
        }

        public bool InsertEmployeeConfirmationDetails(int employeeID, int createdBy)
        {
            log.Info($"EmployeeService/InsertEmployeeConfirmationDetails");
            try
            {
                bool flag = false;
                var level = genericRepo.Get<DTOModel.Designation>(x => x.tblMstEmployees.Any(y => y.EmployeeId == employeeID)).FirstOrDefault().Level;
                var promotiomDetails = genericRepo.Get<DTOModel.tblpromotion>(x => x.EmployeeID == employeeID && !x.IsDeleted && x.EmployeeID != null).ToList();
                var EmpDetails = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == employeeID).FirstOrDefault();
                if (promotiomDetails.Count > 1 && promotiomDetails.Any(x => x.ToDate == null))
                {
                    if (level == "1" || level == "2" || level == "3" || level == "4" || level == "5" || level == "6" || level == "7")
                    {
                        if (!genericRepo.Exists<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID))
                        {
                            Model.ConfirmationFormHdr confirmationFormHdr = new ConfirmationFormHdr();
                            confirmationFormHdr.FormTypeID = 2;
                            confirmationFormHdr.ProcessID = 7;
                            confirmationFormHdr.StatusID = 0;
                            confirmationFormHdr.EmployeeID = employeeID;
                            confirmationFormHdr.CreatedBy = createdBy;
                            confirmationFormHdr.CreatedOn = DateTime.Now;

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

                            var formHdrID = dtoConfirmationFormHdr.FormHdrID;

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
                        }
                        else
                        {
                            var dtoObj = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID).ToList();
                            dtoObj.ForEach(x => { x.ProcessID = 7; x.FormTypeID = 2; });
                            if (dtoObj.Count > 0)
                            {
                                for (int i = 0; i < dtoObj.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoObj[i]);
                                }
                            }
                            var dtoObj1 = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.EmployeeId == employeeID).ToList();
                            if (dtoObj1.Count > 0)
                            {
                                dtoObj1.ForEach(x => { x.ProcessId = 7; });
                                for (int i = 0; i < dtoObj1.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormBHeader>(dtoObj1[i]);
                                }
                            }
                        }
                    }
                    else if (level == "8" || level == "9" || level == "10" || level == "11" || level == "12" || level == "13A")
                    {
                        if (!genericRepo.Exists<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID))
                        {
                            Model.ConfirmationFormHdr confirmationFormHdr = new ConfirmationFormHdr();
                            confirmationFormHdr.FormTypeID = 1;
                            confirmationFormHdr.ProcessID = 7;
                            confirmationFormHdr.StatusID = 0;
                            confirmationFormHdr.EmployeeID = employeeID;
                            confirmationFormHdr.CreatedBy = createdBy;
                            confirmationFormHdr.CreatedOn = DateTime.Now;

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

                            var formHdrID = dtoConfirmationFormHdr.FormHdrID;

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
                        }
                        else
                        {
                            var dtoObj = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID).ToList();
                            dtoObj.ForEach(x => { x.ProcessID = 7; x.FormTypeID = 1; });
                            if (dtoObj.Count > 0)
                            {
                                for (int i = 0; i < dtoObj.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoObj[i]);
                                }
                            }
                            var dtoObj1 = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.EmployeeId == employeeID).ToList();
                            if (dtoObj1.Count > 0)
                            {
                                dtoObj1.ForEach(x => { x.ProcessId = 7; });
                                for (int i = 0; i < dtoObj1.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormAHeader>(dtoObj1[i]);
                                }
                            }

                        }
                    }
                }
                else if (promotiomDetails.Count == 1 && promotiomDetails.Exists(x => x.ToDate == null))
                {
                    if (level == "1" || level == "2" || level == "3" || level == "4" || level == "5" || level == "6" || level == "7")
                    {
                        if (!genericRepo.Exists<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID))
                        {
                            Model.ConfirmationFormHdr confirmationFormHdr = new ConfirmationFormHdr();
                            confirmationFormHdr.FormTypeID = 2;
                            confirmationFormHdr.ProcessID = 6;
                            confirmationFormHdr.StatusID = 0;
                            confirmationFormHdr.EmployeeID = employeeID;
                            confirmationFormHdr.CreatedBy = createdBy;
                            confirmationFormHdr.CreatedOn = DateTime.Now;

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

                            var formHdrID = dtoConfirmationFormHdr.FormHdrID;

                            Model.ConfirmationFormAHdr confirmationForm = new ConfirmationFormAHdr();
                            confirmationForm.EmployeeId = employeeID;
                            confirmationForm.ProcessId = 6;
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
                        }
                        else
                        {
                            var dtoObj = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID).ToList();
                            dtoObj.ForEach(x => { x.ProcessID = 6; x.FormTypeID = 2; });
                            if (dtoObj.Count > 0)
                            {
                                for (int i = 0; i < dtoObj.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoObj[i]);
                                }
                            }
                            var dtoObj1 = genericRepo.Get<DTOModel.ConfirmationFormBHeader>(x => x.EmployeeId == employeeID).ToList();
                            if (dtoObj1.Count > 0)
                            {
                                dtoObj1.ForEach(x => { x.ProcessId = 6; });
                                for (int i = 0; i < dtoObj1.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormBHeader>(dtoObj1[i]);
                                }
                            }

                        }
                    }
                    else if (level == "8" || level == "9" || level == "10" || level == "11" || level == "12" || level == "13A")
                    {
                        if (!genericRepo.Exists<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID))
                        {
                            Model.ConfirmationFormHdr confirmationFormHdr = new ConfirmationFormHdr();
                            confirmationFormHdr.FormTypeID = 1;
                            confirmationFormHdr.ProcessID = 6;
                            confirmationFormHdr.StatusID = 0;
                            confirmationFormHdr.EmployeeID = employeeID;
                            confirmationFormHdr.CreatedBy = createdBy;
                            confirmationFormHdr.CreatedOn = DateTime.Now;

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

                            var formHdrID = dtoConfirmationFormHdr.FormHdrID;

                            Model.ConfirmationFormAHdr confirmationForm = new ConfirmationFormAHdr();
                            confirmationForm.EmployeeId = employeeID;
                            confirmationForm.ProcessId = 6;
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
                        }
                        else
                        {
                            var dtoObj = genericRepo.Get<DTOModel.ConfirmationFormHdr>(x => x.EmployeeID == employeeID).ToList();
                            dtoObj.ForEach(x => { x.ProcessID = 6; x.FormTypeID = 1; });
                            if (dtoObj.Count > 0)
                            {
                                for (int i = 0; i < dtoObj.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormHdr>(dtoObj[i]);
                                }
                            }
                            var dtoObj1 = genericRepo.Get<DTOModel.ConfirmationFormAHeader>(x => x.EmployeeId == employeeID).ToList();
                            if (dtoObj1.Count > 0)
                            {
                                dtoObj1.ForEach(x => { x.ProcessId = 6; });
                                for (int i = 0; i < dtoObj1.Count; i++)
                                {
                                    genericRepo.Update<DTOModel.ConfirmationFormAHeader>(dtoObj1[i]);
                                }
                            }
                        }
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.InnerException + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateEmpProcessApproval(List<Model.EmployeeProcessApproval> empProcessApproval)
        {
            log.Info($"EmployeeService/UpdateEmpProcessApproval");
            bool flag = false;
            try
            {
                for (int i = 0; i < empProcessApproval.Count; i++)
                {
                    var dtoObj = genericRepo.GetByID<DTOModel.EmployeeProcessApproval>(empProcessApproval[i].EmpProcessAppID);
                    if (dtoObj != null)
                    {
                        dtoObj.ToDate = DateTime.Now.Date.AddDays(-1);
                        dtoObj.UpdatedBy = empProcessApproval[i].UpdatedBy;
                        dtoObj.UpdatedOn = empProcessApproval[i].UpdatedOn;
                        genericRepo.Update<DTOModel.EmployeeProcessApproval>(dtoObj);
                        flag = true;
                    }
                }

                flag = InsertEmpProcessApproval(empProcessApproval);
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }
            return flag;
        }

        //public bool UpdatetEmployeeProfile(EmployeeProfile employeeDetails)
        //{
        //    log.Info($"EmployeeService/UpdatetEmployeeProfile");
        //    bool flag = false;
        //    try
        //    {
        //        var dtoObj = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeDetails.EmployeeID);
        //        dtoObj.PANNo = employeeDetails.PANNo;
        //        dtoObj.PensionUAN = employeeDetails.UANNo;
        //        dtoObj.AadhaarNo = employeeDetails.AadhaarNo;
        //        dtoObj.PAdd = employeeDetails.PresentAddress;
        //        dtoObj.PStreet = employeeDetails.PresentCity;
        //        dtoObj.PCity = employeeDetails.PresentCity;
        //        dtoObj.PPin = employeeDetails.PresentPin;
        //        dtoObj.PmtAdd = employeeDetails.PermanentAddress;
        //        dtoObj.PmtCity = employeeDetails.PermanentCity;
        //        dtoObj.PmtPin = employeeDetails.PermanentPin;
        //        dtoObj.AadhaarCardFilePath = employeeDetails.AadhaarCardFilePath;
        //        dtoObj.PanCardFilePath = employeeDetails.PanCardFilePath;
        //        genericRepo.Update<DTOModel.tblMstEmployee>(dtoObj);
        //        flag = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }

        //    return flag;
        //}
        #endregion

        #region Employee Designation Assignment 

        public DesignationAssignment GetDesignationAssignation(int? employeeID)
        {
            log.Info($"EmployeeService/DesignationAssignment/{employeeID}");
            try
            {
                DesignationAssignment dAssignment = new DesignationAssignment();
                var employeeInfo = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.DesignationAssignment>()
                    .ForMember(d => d.employeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.employeeName, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.employeeID, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.cadre, o => o.MapFrom(s => s.Cadre.CadreName))
                    .ForMember(d => d.designation, o => o.MapFrom(s => s.Designation.DesignationName))
                    .ForMember(d => d.Sen_Code, o => o.MapFrom(s => s.Sen_Code))
                    .ForMember(d => d.basicSalary, o => o.MapFrom(s => s.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == employeeID).E_Basic))
                    .ForMember(d => d.CurrentCadreID, o => o.MapFrom(s => s.CadreID))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                dAssignment = Mapper.Map<Model.DesignationAssignment>(employeeInfo);

                if (genericRepo.Exists<DTOModel.tblpromotion>(x => x.EmployeeID == employeeID))
                {
                    var promotionHistory = empRepo.GetPromotionList(employeeID, null);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.GetEmployeePromotions_Result, Model.Promotion>()
                        .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                        .ForMember(d => d.CadreID, o => o.MapFrom(s => s.CadreID))
                        .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                        .ForMember(d => d.TransID, o => o.MapFrom(s => s.TransId))
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.CadreCode, o => o.MapFrom(s => s.cadreCode))
                        .ForMember(d => d.DesgName, o => o.MapFrom(s => s.DesignationName))
                        .ForMember(d => d.FromDate, o => o.MapFrom(s => s.Date_Of_Joining))
                        .ForMember(d => d.ToDate, o => o.MapFrom(s => s.To_Date))
                        .ForMember(d => d.ConfirmationDate, o => o.MapFrom(s => s.ConfirmationDate))
                        .ForMember(d => d.E_Basic, o => o.MapFrom(s => s.Basic_Salary))
                        .ForAllOtherMembers(d => d.Ignore());

                    });
                    dAssignment.promotionList = Mapper.Map<List<Model.Promotion>>(promotionHistory);
                }
                else
                    dAssignment.promotionList = new List<Promotion>();

                return dAssignment;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public Promotion GetPromotionForm(int? employeeID, int? transID)
        {
            log.Info($"EmployeeService/GetPromotionForm/{employeeID}/{transID}");
            try
            {
                Promotion promotion = new Promotion();
                var result = empRepo.GetPromotionList(employeeID, transID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetEmployeePromotions_Result, Model.Promotion>()
                     .ForMember(d => d.CadreID, o => o.MapFrom(s => s.CadreID))
                     .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                     .ForMember(d => d.TransID, o => o.MapFrom(s => s.TransId))
                            .ForMember(d => d.CadreCode, o => o.MapFrom(s => s.cadreCode))
                            .ForMember(d => d.DesgName, o => o.MapFrom(s => s.DesignationName))
                            .ForMember(d => d.FromDate, o => o.MapFrom(s => s.Date_Of_Joining))
                            .ForMember(d => d.ToDate, o => o.MapFrom(s => s.To_Date))
                            .ForMember(d => d.ConfirmationDate, o => o.MapFrom(s => s.ConfirmationDate))
                            .ForMember(d => d.E_Basic, o => o.MapFrom(s => s.Basic_Salary))
                            .ForMember(d => d.WayOfPostingID, o => o.MapFrom(s => s.WayOfPostingID))
                            .ForMember(d => d.Confirmed, o => o.MapFrom(s => s.Confirmed))
                            .ForMember(d => d.SeniorityCode, o => o.MapFrom(s => s.SeniorityCode))
                            .ForMember(d => d.NewTS, o => o.MapFrom(s => s.TS))
                            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                            .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                            .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                            .ForMember(d => d.OrderOfPromotion, o => o.MapFrom(s => s.OrderOfPromotion))
                            .ForAllOtherMembers(d => d.Ignore());
                });
                return Mapper.Map<Model.Promotion>(result.FirstOrDefault());
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        public bool ChangeProfilePicture(int employeeID, string pictureName, string newMobileNo, string newEmailID)
        {
            log.Info($"EmployeeService/ChangeProfilePicture/{employeeID}/{pictureName}/{newMobileNo}/{newEmailID}");
            bool flag = false;
            try
            {

                var userProfile = genericRepo.Get<DTOModel.User>(x => x.EmployeeID == employeeID).FirstOrDefault();
                userProfile.ImageName = pictureName;
                genericRepo.Update<DTOModel.User>(userProfile);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;

        }

        public bool ChangeMobileNo(int employeeID, string newMobileNo)
        {
            log.Info($"EmployeeService/ChangeMobileNo/{employeeID}/{newMobileNo}");
            bool flag = false;
            try
            {
                var emp = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == employeeID).FirstOrDefault();
                emp.MobileNo = newMobileNo;
                genericRepo.Update<DTOModel.tblMstEmployee>(emp);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Designation GetDesignationDtls(int designationID)
        {
            log.Info($"EmployeeService/GetDesignationDtls/{designationID}");

            try
            {
                var dtoModel = genericRepo.GetByID<DTOModel.Designation>(designationID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Designation, Designation>()
                    .ForMember(d => d.CadreID, o => o.MapFrom(s => s.CadreID))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                    .ForMember(d => d.Level, o => o.MapFrom(s => s.Level))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                return Mapper.Map<Designation>(dtoModel);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public bool UpdatetEmployeeProfile(Model.EmployeeProfile empProfile)
        {
            log.Info($"EmployeeService/UpdatetEmployeePersonalDetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.tblMstEmployee>(empProfile.EmployeeID);
                if (dtoObj != null)
                {
                    dtoObj.HBName = empProfile.HBName;
                    dtoObj.MotherName = empProfile.MotherName;
                    dtoObj.BGroupID = empProfile.BloodGroupID == 0 ? null : empProfile.BloodGroupID;
                    dtoObj.ID_Mark = empProfile.IDMark;
                    dtoObj.PANNo = empProfile.PANNo;
                    dtoObj.AadhaarNo = empProfile.AadhaarNo;
                    dtoObj.PassPortNo = empProfile.PassportNo;
                    dtoObj.QAcademicID = empProfile.QAcademicID;
                    dtoObj.QProfessionalID = empProfile.QProfessionalID;
                    dtoObj.SpecialSkills = empProfile.SpecialSkills;
                    dtoObj.PAdd = empProfile.PresentAddress;
                    dtoObj.PCity = empProfile.PresentCity;
                    dtoObj.PPin = empProfile.PresentPin;
                    dtoObj.PmtAdd = empProfile.PermanentAddress;
                    dtoObj.PmtCity = empProfile.PermanentCity;
                    dtoObj.PmtPin = empProfile.PermanentPin;
                    dtoObj.MobileNo = empProfile.MobileNo;
                    dtoObj.OfficialEmail = empProfile.EmailID;
                    dtoObj.PFNO = empProfile.PFNo;
                    dtoObj.PState = empProfile.PresentStateID == 0 ? null : empProfile.PresentStateID;
                    dtoObj.PmtState = empProfile.PmtStateID == 0 ? null : empProfile.PmtStateID;
                    genericRepo.Update<DTOModel.tblMstEmployee>(dtoObj);
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public List<SubOrdinatesDetails> GetSubOrdinatesDetails(int? employeeID)
        {
            log.Info($"EmployeeService/GetSubOrdinatesDetails");
            try
            {
                var result = empRepo.GetSubOrdinatesDetails(employeeID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetSubOrdinatesDetails_Result, Model.SubOrdinatesDetails>()
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.DesignationName))
                    .ForMember(d => d.Appraisal, o => o.MapFrom(s => s.Appraisal))
                    .ForMember(d => d.Attendance_Approval, o => o.MapFrom(s => s.Attendance_Approval))
                    .ForMember(d => d.Leave_Approval, o => o.MapFrom(s => s.Leave_Approval))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var lstSubOrdinates = Mapper.Map<List<Model.SubOrdinatesDetails>>(result);
                return lstSubOrdinates.OrderBy(x => x.EmployeeCode).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int GetSeniorityCode(int designationID)
        {
            log.Info($"EmployeeService/GetSeniorityCode/{designationID}");

            int? sen_Code = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DesignationID == designationID).Max(y => y.Sen_Code);
            return sen_Code.HasValue ? sen_Code.Value + 1 : 1;
        }

        #region Staff Transfer
        public StaffTransfer GetStaffTransfer(int? employeeID)
        {
            log.Info($"EmployeeService/GetStaffTransfer/{employeeID}");
            try
            {
                StaffTransfer staffTransfer = new StaffTransfer();
                var employeeInfo = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.StaffTransfer>()
                    .ForMember(d => d.employeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.employeeName, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.employeeID, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.branchCode, o => o.MapFrom(s => s.Branch.BranchCode))
                    .ForMember(d => d.branchName, o => o.MapFrom(s => s.Branch.BranchName))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                staffTransfer = Mapper.Map<Model.StaffTransfer>(employeeInfo);

                if (genericRepo.Exists<DTOModel.tblmsttransfer>(x => x.EmployeeId == employeeID))
                {
                    var staffTransferHistory = genericRepo.Get<DTOModel.tblmsttransfer>(x => x.EmployeeId == employeeID && !x.IsDeleted);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.tblmsttransfer, Model.Transfer>()
                        .ForMember(d => d.TransId, o => o.MapFrom(s => s.TransId))
                        .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                        .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                        .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                        .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                        .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                        .ForMember(d => d.FromDate, o => o.MapFrom(s => s.FromDate))
                        .ForMember(d => d.ToDate, o => o.MapFrom(s => s.ToDate))
                        .ForAllOtherMembers(d => d.Ignore());

                    });
                    staffTransfer.transferList = Mapper.Map<List<Model.Transfer>>(staffTransferHistory.OrderByDescending(x => x.FromDate));
                }
                return staffTransfer;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Transfer GetStaffTransferForm(int? employeeID, int? transId)
        {
            log.Info($"EmployeeService/GetStaffTransferForm/{employeeID}");
            try
            {
                Transfer transfer = new Transfer();
                var staffTransferHistory = genericRepo.Get<DTOModel.tblmsttransfer>(x => x.EmployeeId == employeeID && !x.IsDeleted && x.TransId == transId);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblmsttransfer, Model.Transfer>()
                     .ForMember(d => d.TransId, o => o.MapFrom(s => s.TransId))
                     .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                     .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                     .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                     .ForMember(d => d.FromDate, o => o.MapFrom(s => s.FromDate))
                     .ForMember(d => d.ToDate, o => o.MapFrom(s => s.ToDate))
                     .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                     .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                     .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                     .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                     .ForAllOtherMembers(d => d.Ignore());
                });
                return Mapper.Map<Model.Transfer>(staffTransferHistory.OrderByDescending(x => x.FromDate).FirstOrDefault());
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Transfer ChangeStaffBranch(Transfer transfer)
        {
            log.Info($"EmployeeService/ChangeStaffBranch");
            try
            {
                if (transfer.FormActionType == "Create" && ValidateStaffTransferFormInputs(transfer.EmployeeCode))
                {
                    transfer.IsValidInputs = false;
                    transfer.ValidationMessage = "You can not process new transaction until the details of previous transaction is completed";
                }
                else
                {
                    var isWorkedAlready = false; DateTime? fromDate = null, toDate = null;
                    var staffTransEntities = genericRepo.GetIQueryable<DTOModel.tblmsttransfer>(x => x.EmployeeCode == transfer.EmployeeCode && !x.IsDeleted).ToList();

                    staffTransEntities.ForEach(x =>
                    {
                        isWorkedAlready = x.ToDate.HasValue && transfer.FromDate.Value < x.ToDate.Value ? true : isWorkedAlready;
                        fromDate = x.FromDate;
                        toDate = x.ToDate;
                    });

                    if (transfer.FormActionType == "Create" && isWorkedAlready)
                    {
                        transfer.IsValidInputs = false;
                        transfer.ValidationMessage = $"This employee is already working on Previous Branch at this time From {fromDate.Value.ToString("dd/MM/yyyy")} & To {toDate.Value.ToString("dd/MM/yyyy")},So please select from date greater than pevious to date";
                    }
                }

                if (transfer.IsValidInputs)
                {
                    if (transfer.FormActionType == "Create")
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Transfer, DTOModel.tblmsttransfer>()
                            .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                            .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                            .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                            .ForMember(d => d.FromDate, o => o.MapFrom(s => s.FromDate))
                            .ForMember(d => d.ToDate, o => o.MapFrom(s => s.ToDate))
                            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                            .ForMember(d => d.IsDeleted, o => o.UseValue(false))
                            .ForAllOtherMembers(d => d.Ignore());
                        });
                        var dtoTransfer = Mapper.Map<DTOModel.tblmsttransfer>(transfer);
                        int newtransID = genericRepo.Insert<DTOModel.tblmsttransfer>(dtoTransfer);


                        var prevBranch = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeCode == transfer.EmployeeCode).FirstOrDefault();
                        prevBranch.BranchID = transfer.BranchId;
                        genericRepo.Update<DTOModel.tblMstEmployee>(prevBranch);

                       var branch =  genericRepo.Get<DTOModel.Branch>(x => x.BranchID == transfer.BranchId && x.IsDeleted ==false).FirstOrDefault();
                        if (branch != null)
                        {
                            var emp = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeCode== transfer.EmployeeCode).FirstOrDefault();
                            if (emp != null)
                            {
                                emp.BranchID = transfer.BranchId;
                                emp.BranchCode = branch.BranchCode;
                                genericRepo.Update<DTOModel.TblMstEmployeeSalary>(emp);
                            }
                        }
                        transfer.Saved = true;
                    }
                    else
                    {
                        var prevTransRow = genericRepo.Get<DTOModel.tblmsttransfer>(x => x.TransId == transfer.TransId && x.EmployeeCode == transfer.EmployeeCode).FirstOrDefault();
                        prevTransRow.ToDate = transfer.ToDate;
                        prevTransRow.FromDate = transfer.FromDate;
                        prevTransRow.UpdatedBy = transfer.UpdatedBy;
                        prevTransRow.UpdatedOn = transfer.UpdatedOn;

                        // prevTransRow.
                        genericRepo.Update<DTOModel.tblmsttransfer>(prevTransRow);

                        var prevBranch = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeCode == transfer.EmployeeCode).FirstOrDefault();
                        prevBranch.BranchID = transfer.BranchId;
                        genericRepo.Update<DTOModel.tblMstEmployee>(prevBranch);

                        var branch = genericRepo.Get<DTOModel.Branch>(x => x.BranchID == transfer.BranchId && x.IsDeleted == false).FirstOrDefault();
                        if (branch != null)
                        {
                            var emp = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeCode == transfer.EmployeeCode).FirstOrDefault();
                            if (emp != null)
                            {
                                emp.BranchID = transfer.BranchId;
                                emp.BranchCode = branch.BranchCode;
                                genericRepo.Update<DTOModel.TblMstEmployeeSalary>(emp);
                            }
                        }
                        transfer.Saved = true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return transfer;
        }

        private bool ValidateStaffTransferFormInputs(string employeeCode)
        {
            log.Info($"ValidateStaffTransferFormInputs/ChangeBranch");

            var lastTrans = genericRepo.GetIQueryable<DTOModel.tblmsttransfer>
                (x => x.EmployeeCode == employeeCode && !x.IsDeleted).OrderByDescending(y => y.TransId).Take(1);
            return lastTrans.Any(z => z.ToDate == null);
        }

        public bool DeleteStaffTransferEntry(int transID)
        {
            log.Info($"EmployeeService/DeleteStaffTransferEntry/{transID}");
            bool flag = false;
            try
            {
                var transRow = genericRepo.GetByID<DTOModel.tblmsttransfer>(transID);
                transRow.IsDeleted = true;
                genericRepo.Update<DTOModel.tblmsttransfer>(transRow);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;

        }

        public DataTable GetEmployeeTransferDetail(int? branchID, string employeeCode)
        {
            log.Info($"EmployeeService/GetEmployeeTransferDetail/branch={branchID}/empCode={employeeCode}");
            try
            {
                var staffTransferHistory = empRepo.GetEmployeeTransferDetail(branchID, employeeCode);
                return staffTransferHistory;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool ExportEmployeeTransferDetail(DataSet dsSource, string sFullPath, string fileName)
        {
            try
            {
                var flag = false;
                sFullPath = $"{sFullPath}{fileName}";
                flag = exportExcel.ExportToExcel(dsSource, sFullPath, fileName);
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        public EmployeeQualification GetQualificationDetail(int employeeID)
        {
            log.Info($"TrainingService/GetQualificationDetail");
            try
            {
                EmployeeQualification empqual = new EmployeeQualification();

                var dtoEmployee = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID);

                if (dtoEmployee != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.tblMstEmployee, Employee>()

                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => employeeID))
                    ;
                        cfg.CreateMap<DTOModel.tblEmployeeQualificationdetail, EmployeeQualificationM>()
                        .ForMember(d => d.TypeID, o => o.MapFrom(s => s.TypeID))
                        .ForMember(d => d.QID, o => o.MapFrom(s => s.QID))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    var dtoqualList = Mapper.Map<Employee>(dtoEmployee);
                    var dtoqual = Mapper.Map<List<EmployeeQualificationM>>(dtoEmployee.tblEmployeeQualificationdetails);
                    empqual._Employee = dtoqualList;
                    empqual.EmployeeQualificationList = dtoqual;
                }
                return empqual;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Model.Employee GetEmployeePaymentModeDtls(string empCode)
        {

            log.Info($"EmployeeService/GetEmployeePaymentModeDtls/{empCode}");

            //====  Get Employee Mode Of PAyment Information======  Dated On- 17-Apr-2020 - SG

            Model.Employee employee = new Employee();

            var empSalRow = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeCode == empCode).FirstOrDefault();
            if (empSalRow != null)
            {
                employee.BankAcNo = empSalRow.BankAcNo; employee.BankCode = empSalRow.BankCode;
                if (empSalRow.ModePay?.Trim() == "CASH")
                    employee.modOfPayment = ModeOfPayment.Cash;
            }
            ///========End===========
            return employee;
        }

        public string ExportBrachWiseSalaryConfiguration(int branchID, string fileName, string filePath)
        {
            log.Info($"EmployeeService/ExportBrachWiseSalaryConfiguration/branchID:{branchID}");
            try
            {
                string result = string.Empty; string sFullPath = string.Empty;
                if (Directory.Exists(filePath))
                {
                    sFullPath = $"{filePath}{fileName}";
                    var bSalaryConfigurations = empRepo.GetBranchWiseSalaryConfig(branchID);

                    if (bSalaryConfigurations != null && bSalaryConfigurations.Rows.Count > 0)
                    {
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = bSalaryConfigurations.Columns.Cast<DataColumn>()
                            .Select(x => x.ColumnName).AsEnumerable<string>();

                        // var branchInfo = genericRepo.GetByID<DTOModel.Branch>(branchID);

                        result = BranchSalaryConfiguration(exportHdr,
                            bSalaryConfigurations, $"Branch Salary Configuration", sFullPath);

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        bool ManageEmpSuspensionPeriod(EmployeeSuspensionPeriod suspensionPeriod)
        {
            log.Info($"EmployeeService/ManageEmpSuspensionPeriod/");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<EmployeeSuspensionPeriod, DTOModel.EmployeeSuspensionPeriod>();
                });
                var dtoEmpSuspension = Mapper.Map<DTOModel.EmployeeSuspensionPeriod>(suspensionPeriod);
                genericRepo.Insert<DTOModel.EmployeeSuspensionPeriod>(dtoEmpSuspension);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public List<Model.EmployeeSuspensionPeriod> GetEmployeeSuspensionHistory(int employeeID)
        {
            log.Info($"EmployeeService/GetEmployeeSuspensionHistory/employeeID:{employeeID}");
            try
            {
                var dtoEmpSuspensionPeriods = genericRepo.Get<DTOModel.EmployeeSuspensionPeriod>(x => x.EmployeeID == employeeID);
                Mapper.Initialize(cfg =>
                {

                    cfg.CreateMap<DTOModel.EmployeeSuspensionPeriod, EmployeeSuspensionPeriod>();
                });
                return Mapper.Map<List<EmployeeSuspensionPeriod>>(dtoEmpSuspensionPeriods);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool DoesNewPeriodOverLapped(int employeeID, DateTime periodFrom, DateTime periodTo)
        {
            log.Info($"EmployeeService/DoesNewPeriodOverLapped/employeeID:{employeeID},periodFrom:{periodFrom},periodTo:{periodTo}");

            try
            {
                var dtoEmpSuspensionIntervals = genericRepo.Get<DTOModel.EmployeeSuspensionPeriod>(x =>
                    x.EmployeeID == employeeID).Select(x => new { x.PeriodFrom, x.PeriodTo }).ToList();
                return dtoEmpSuspensionIntervals.All(y => (periodFrom > y.PeriodTo || periodTo < y.PeriodFrom));

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        private bool UpdateSeniority(int employeeID, int desID)
        {
            log.Info($"EmployeeService/UpdateSeniority/employeeID={employeeID}/desID={desID}");

            try
            {
                var employeeSenList = empRepo.GetSeniorityList(employeeID, desID);

                for (int i = 0; i < employeeSenList.Count; i++)
                {
                    var getdata = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeSenList[i].EmployeeId);
                    if (getdata != null)
                    {
                        getdata.Sen_Code = getdata.Sen_Code - 1;
                        genericRepo.Update<DTOModel.tblMstEmployee>(getdata);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateEmployeeAcheivement(Model.EmployeeAchievement empAchievement, List<Model.EmpAchievementAndCertificationDocument> documents)
        {
            log.Info($"EmployeeService/UpdateEmployeeAcheivement");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmployeeAchievement, DTOModel.EmployeeAchievement>();
                });

                var dtoEmpAchievement = Mapper.Map<DTOModel.EmployeeAchievement>(empAchievement);
                genericRepo.Insert<DTOModel.EmployeeAchievement>(dtoEmpAchievement);

                int pkValue = dtoEmpAchievement.EmpAchievementID;

                if (pkValue > 0 && documents?.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {

                        cfg.CreateMap<Model.EmpAchievementAndCertificationDocument, DTOModel.EmpAchievementAndCertificationDocument>()
                        .ForMember(d => d.LinkedAchivementID, o => o.UseValue(pkValue));
                    });
                    var dtoEmpAchievementDoc = Mapper.Map<List<DTOModel.EmpAchievementAndCertificationDocument>>(documents);

                    genericRepo.AddMultipleEntity<DTOModel.EmpAchievementAndCertificationDocument>(dtoEmpAchievementDoc);

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

        public List<Model.EmployeeAchievement> GetEmployeeAchievement(int employeeID)
        {
            log.Info($"EmployeeService/GetEmployeeAchievement/employeeID:{employeeID}");

            try
            {
                var dtoEmpAchievement = genericRepo.Get<DTOModel.EmployeeAchievement>(x =>
                x.EmployeeID == employeeID && !x.IsDeleted).OrderByDescending(y => y.DateOfAchievement).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeAchievement, Model.EmployeeAchievement>()
                    .ForMember(d => d.DateOfAchievement, o => o.MapFrom(s => s.DateOfAchievement))
                    .ForMember(d => d.EmpAchievementID, o => o.MapFrom(s => s.EmpAchievementID))
                    .ForMember(d => d.AchievementName, o => o.MapFrom(s => s.AchievementName))
                    .ForMember(d => d.AchievementRemark, o => o.MapFrom(s => s.AchievementRemark))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.documents, o => o.MapFrom(s => s.EmpAchievementAndCertificationDocuments))
                    .ForAllOtherMembers(d => d.Ignore());

                    cfg.CreateMap<DTOModel.EmpAchievementAndCertificationDocument,
                        Model.EmpAchievementAndCertificationDocument>();
                });

                return Mapper.Map<List<Model.EmployeeAchievement>>(dtoEmpAchievement);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.EmployeeCertification> GetEmployeeCertification(int employeeID)
        {

            log.Info($"EmployeeService/GetEmployeeCertification/employeeID:{employeeID}");

            try
            {
                var dtoEmpCertification = genericRepo.Get<DTOModel.EmployeeCertification>(x =>
                x.EmployeeID == employeeID && !x.IsDeleted).OrderByDescending(y => y.DateOfIssue).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeCertification, Model.EmployeeCertification>()
                    .ForMember(d => d.DateOfIssue, o => o.MapFrom(s => s.DateOfIssue))
                    .ForMember(d => d.CertificationRemark, o => o.MapFrom(s => s.CertificationRemark))
                    .ForMember(d => d.EmpCertificateID, o => o.MapFrom(s => s.EmpCertificateID))
                    .ForMember(d => d.CertificationName, o => o.MapFrom(s => s.CertificationName))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                     .ForMember(d => d.documents, o => o.MapFrom(s => s.EmpAchievementAndCertificationDocuments))
                    .ForAllOtherMembers(d => d.Ignore());

                    cfg.CreateMap<DTOModel.EmpAchievementAndCertificationDocument,
                       Model.EmpAchievementAndCertificationDocument>();
                });

                return Mapper.Map<List<Model.EmployeeCertification>>(dtoEmpCertification);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateEmployeeCertification(Model.EmployeeCertification empCertification, List<Model.EmpAchievementAndCertificationDocument> documents)
        {
            log.Info($"EmployeeService/UpdateEmployeeCertification");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmployeeCertification, DTOModel.EmployeeCertification>();
                });

                var dtoEmpCertification = Mapper.Map<DTOModel.EmployeeCertification>(empCertification);
                genericRepo.Insert<DTOModel.EmployeeCertification>(dtoEmpCertification);

                int pkValue = dtoEmpCertification.EmpCertificateID;

                if (pkValue > 0 && documents?.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.EmpAchievementAndCertificationDocument, DTOModel.EmpAchievementAndCertificationDocument>()
                        .ForMember(d => d.LinkedCertificateID, o => o.UseValue(pkValue));
                    });
                    var dtoEmpAchievementDoc = Mapper.Map<List<DTOModel.EmpAchievementAndCertificationDocument>>(documents);

                    genericRepo.AddMultipleEntity<DTOModel.EmpAchievementAndCertificationDocument>(dtoEmpAchievementDoc);

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

        public bool DeleteEmpAchievement(int empAchievementID, int userID)
        {
            log.Info($"EmployeeService/DeleteEmpAchievement/empAchievementID:{empAchievementID}");
            try
            {
                var dtoEmpAchievement = genericRepo.GetByID<DTOModel.EmployeeAchievement>(empAchievementID);
                dtoEmpAchievement.IsDeleted = true;
                dtoEmpAchievement.UpdatedBy = userID;
                dtoEmpAchievement.UpdatedOn = DateTime.Now;

                genericRepo.Update<DTOModel.EmployeeAchievement>(dtoEmpAchievement);

                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool DeleteEmpCertificate(int empCertificateID, int userID)
        {
            log.Info($"EmployeeService/DeleteEmpCertificate/empCertificateID:{empCertificateID}");
            try
            {
                var dtoEmpCertificate = genericRepo.GetByID<DTOModel.EmployeeCertification>(empCertificateID);
                dtoEmpCertificate.IsDeleted = true;
                dtoEmpCertificate.UpdatedBy = userID;
                dtoEmpCertificate.UpdatedOn = DateTime.Now;

                genericRepo.Update<DTOModel.EmployeeCertification>(dtoEmpCertificate);

                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool IsStaffBudgetAvailable(int deignationID, string year, string quataType)
        {
            log.Info($"EmployeeService/IsStaffBudgetAvailable/{deignationID}/{year}");

            var getStaffBudget = genericRepo.Get<DTOModel.tblStaffBudget>(x => x.Year == year && x.DesignationID == deignationID).FirstOrDefault();
            if (getStaffBudget != null)
            {
                if (quataType == "D") // for direct Quata
                {
                    if (getStaffBudget.FDirect > 0)
                        return true;
                    else
                        return false;
                }
                else if (quataType == "P") // for promotion Quata
                {
                    if (getStaffBudget.FPromotion > 0)
                        return true;
                    else
                        return false;
                }
                return false;
            }
            else
                return false;
        }

        #region  ======== Employee Achievement & Certification Report Export To Excel (SG - 10 -Mar-2021) ============


        public string GetAchievementAndCertificationReport(
            byte category,
            DateTime fromDate, DateTime toDate, int? employeeID, string fileName, string filePath)
        {
            log.Info($"EmployeeService/GetAchievementAndCertificationReport/category:{category}");
            try
            {
                string result = string.Empty; string sFullPath = string.Empty, reportFilterLabel = string.Empty;

                if (Directory.Exists(filePath))
                {
                    sFullPath = $"{filePath}{fileName}";
                    reportFilterLabel = $"{fromDate.ToString("dd/MM/yyyy")}-{toDate.ToString("dd/MM/yyyy")}";

                    if (category == 1)  //========= refer to employee achivement 
                    {
                        var dtoEmpAchivements = genericRepo.Get<DTOModel.EmployeeAchievement>(
                            x => !x.IsDeleted
                            &&
                            (x.DateOfAchievement.Value >= fromDate && x.DateOfAchievement.Value <= toDate)
                            &&
                            (!employeeID.HasValue ? (1 > 0) : (x.EmployeeID == employeeID.Value)))
                            .OrderBy(y => y.tblMstEmployee.EmployeeCode);

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<DTOModel.EmployeeAchievement, EmployeeAchievement>()
                            .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                            .ForMember(d => d.AchievementName, o => o.MapFrom(s => s.AchievementName))
                            .ForMember(d => d.AchievementRemark, o => o.MapFrom(s => s.AchievementRemark))
                            .ForMember(d => d.DateOfAchievement, o => o.MapFrom(s => s.DateOfAchievement))
                            .ForMember(d => d.Employee, o => o.MapFrom(s => ($"{s.tblMstEmployee.EmployeeCode}-{s.tblMstEmployee.Name}")))
                            .ForAllOtherMembers(d => d.Ignore());
                        });

                        var empAchievements = Mapper.Map<List<EmployeeAchievement>>(dtoEmpAchivements);

                        if (empAchievements?.Count > 0)
                        {
                            var noOfEmployees = empAchievements.Select(y => new EmployeeAchievement
                            {
                                EmployeeID = y.EmployeeID,
                                Employee = y.Employee
                            })
                            .GroupBy(o => new { o.EmployeeID, o.Employee })
                                 .Select(o => o.FirstOrDefault()).ToList();

                            var noOfEmployeeDT = Common.ExtensionMethods.ToDataTable<EmployeeAchievement>(noOfEmployees);
                            var dtEmpAchievement = Common.ExtensionMethods.ToDataTable<Model.EmployeeAchievement>(empAchievements);

                            dtEmpAchievement.Columns["AchievementName"].ColumnName = "Achievement Name";
                            dtEmpAchievement.Columns["DateOfAchievement"].ColumnName = "Date";
                            dtEmpAchievement.Columns["AchievementRemark"].ColumnName = "Achievement Remark";

                            List<string> exportHdr = new List<string>();

                            exportHdr.Add("#");
                            exportHdr.Add("Date");
                            exportHdr.Add("Achievement Name");
                            exportHdr.Add("Achievement Remark");

                            result = EmpAchievementExportToExcel(exportHdr, reportFilterLabel, noOfEmployeeDT, dtEmpAchievement, $"Employee Achievement", sFullPath);
                        }
                        else
                            result = "norec";
                    }
                    else if (category == 2)
                    {
                        var dtoEmpCertification = genericRepo.Get<DTOModel.EmployeeCertification>(
                           x => !x.IsDeleted
                           &&
                           (x.DateOfIssue.Value >= fromDate && x.DateOfIssue.Value <= toDate)
                           &&
                           (!employeeID.HasValue ? (1 > 0) : (x.EmployeeID == employeeID.Value)))
                           .OrderBy(y => y.tblMstEmployee.EmployeeCode);

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<DTOModel.EmployeeCertification, EmployeeCertification>()
                            .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                            .ForMember(d => d.CertificationName, o => o.MapFrom(s => s.CertificationName))
                            .ForMember(d => d.CertificationRemark, o => o.MapFrom(s => s.CertificationRemark))
                            .ForMember(d => d.DateOfIssue, o => o.MapFrom(s => s.DateOfIssue))
                            .ForMember(d => d.Employee, o => o.MapFrom(s => ($"{s.tblMstEmployee.EmployeeCode}-{s.tblMstEmployee.Name}")))
                            .ForAllOtherMembers(d => d.Ignore());
                        });
                        var empCertifications = Mapper.Map<List<EmployeeCertification>>(dtoEmpCertification);

                        if (empCertifications?.Count > 0)
                        {
                            var noOfEmployees = empCertifications.Select(y => new EmployeeCertification
                            {
                                EmployeeID = y.EmployeeID,
                                Employee = y.Employee
                            })
                            .GroupBy(o => new { o.EmployeeID, o.Employee })
                                 .Select(o => o.FirstOrDefault()).ToList();

                            var noOfEmployeeDT = Common.ExtensionMethods.ToDataTable<EmployeeCertification>(noOfEmployees);
                            var dtEmpCertification = Common.ExtensionMethods.ToDataTable<Model.EmployeeCertification>(empCertifications);

                            dtEmpCertification.Columns["CertificationName"].ColumnName = "Certification Name";
                            dtEmpCertification.Columns["DateOfIssue"].ColumnName = "Date";
                            dtEmpCertification.Columns["CertificationRemark"].ColumnName = "Certification Remark";

                            List<string> exportHdr = new List<string>();

                            exportHdr.Add("#");
                            exportHdr.Add("Date");
                            exportHdr.Add("Certification Name");
                            exportHdr.Add("Certification Remark");

                            result = EmpCertificationExportToExcel(exportHdr, reportFilterLabel, noOfEmployeeDT, dtEmpCertification, $"Employee Certification", sFullPath);

                        }
                        else
                            result = "norec";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion


        public Model.Insurance GetInsuranceByID(int employeeid)
        {
            log.Info($"EmployeeService/GetInsuranceByID/{employeeid}");
            try
            {
                var InsuranceObj = genericRepo.Get<DTOModel.Insurance>(x => x.EmployeeId == employeeid).OrderByDescending(o=> o.InsuranceId).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Insurance, Model.Insurance>()
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode + " - " + s.tblMstEmployee.Name));

                });
                var obj = Mapper.Map<DTOModel.Insurance, Model.Insurance>(InsuranceObj);
                if (obj != null)
                {
                    var InsuranceDep = genericRepo.Get<DTOModel.InsuranceDependent>(x => x.InsuranceId == obj.InsuranceId);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.InsuranceDependent, Model.InsuranceDependent>();
                    });
                    var dtoInsuranceDep = Mapper.Map<List<Model.InsuranceDependent>>(InsuranceDep);

                    var dtoDependentList = GetDependentList(employeeid);
                    foreach (var item in dtoInsuranceDep)
                    {
                        item.DependentName = dtoDependentList.Where(x => x.DependentId == item.DependentId).FirstOrDefault() != null ? dtoDependentList.Where(x => x.DependentId == item.DependentId).FirstOrDefault().DependentName : null;
                        item.Relation = dtoDependentList.Where(x => x.DependentId == item.DependentId).FirstOrDefault() != null ? dtoDependentList.Where(x => x.DependentId == item.DependentId).FirstOrDefault().Relation : null;
                        item.Gender = dtoDependentList.Where(x => x.DependentId == item.DependentId).FirstOrDefault() != null ? dtoDependentList.Where(x => x.DependentId == item.DependentId).FirstOrDefault().Gender : null;
                        item.Age = dtoDependentList.Where(x => x.DependentId == item.DependentId).FirstOrDefault() != null ? dtoDependentList.Where(x => x.DependentId == item.DependentId).FirstOrDefault().Age : 0;

                    }
                    obj.InsuranceDependenceList = dtoInsuranceDep;
                }
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.InsuranceDependent> GetDependentList(int employeeId)
        {
            log.Info($"EmployeeService/GetDependentList/{employeeId}");
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


    }
}

