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
using Nafed.MicroPay.Model.Employees;

namespace Nafed.MicroPay.Services
{
    public class NonRegularEmployeesService : BaseService, INonRegularEmployeesService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IEmployeeRepository empRepo;
        private readonly IExport exportExcel;
        public NonRegularEmployeesService(IGenericRepository genericRepo, IEmployeeRepository empRepo, IExport exportExcel)
        {
            this.genericRepo = genericRepo;
            this.empRepo = empRepo;
            this.exportExcel = exportExcel;
        }

        public bool EmployeeDetailsExists(int id, string value)
        {
            log.Info($"NonRegularEmployeesService/EmployeeDetailsExists/{id}/{value}");
            return genericRepo.Exists<DTOModel.tblMstEmployee>(x => x.EmployeeId != id && x.EmployeeCode == value);
        }
        DTOModel.GetLastEmployeeCode_Result GetLastestEmployeeCode(int empTypeID)
        {
            return empRepo.GetLastEmployeeCode(empTypeID);
        }
        public int InsertEmployeeDetails(Model.Employees.NonRegularEmployee employeeDetails)
        {
            log.Info($"NonRegularEmployeesService/InsertEmployeeDetailsDetails");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Employees.NonRegularEmployee, DTOModel.tblMstEmployee>()
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
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoemployeeDetails = Mapper.Map<DTOModel.tblMstEmployee>(employeeDetails);
                var data = GetLastestEmployeeCode(dtoemployeeDetails.EmployeeTypeID);

                #region Update Last Employee Code in MaxEmpCodeTypeWise table

                var empCode = string.Empty;              
                    if ((data.MaxEmployee + 1).ToString().Length == 1)
                        empCode = data.Prifix + "0000" + (data.MaxEmployee + 1);
                    else if ((data.MaxEmployee + 1).ToString().Length == 2)
                        empCode = data.Prifix + "000" + (data.MaxEmployee + 1);
                    else if ((data.MaxEmployee + 1).ToString().Length == 3)
                        empCode = data.Prifix + "00" + (data.MaxEmployee + 1);
                    else if ((data.MaxEmployee + 1).ToString().Length == 4)
                        empCode = data.Prifix + "0" + (data.MaxEmployee + 1);
               
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

                #region Add Employee Mode of Payment Detail in Employee Salary Table

                var empSalRow = new DTOModel.TblMstEmployeeSalary();
                empSalRow.EmployeeCode = employeeDetails.EmployeeCode;
                empSalRow.EmployeeID = dtoemployeeDetails.EmployeeId;
                empSalRow.BranchID = dtoemployeeDetails.BranchID;
                empSalRow.ModePay = ((Model.Employees.ModeOfPayment)employeeDetails.modOfPayment).GetDisplayName().Trim();
                var branchCode = genericRepo.GetByID<DTOModel.Branch>(empSalRow.BranchID).BranchCode;
                empSalRow.BranchCode = branchCode   /*dtoemployeeDetails.Branch.BranchCode*/;
                empSalRow.CreatedBy = employeeDetails.CreatedBy;
                empSalRow.CreatedOn = employeeDetails.CreatedOn;
                empSalRow.IsSalgenrated = false;
                empSalRow.E_Basic = employeeDetails.E_Basic;
                if (employeeDetails.modOfPayment == Model.Employees.ModeOfPayment.Bank)
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
        public List<Model.Employees.NonRegularEmployee> GetEmployeeList(string empName, string empCode, int? designationID, int? empTypeID)
        {
            log.Info($"NonRegularEmployeesService/GetEmployeeList");
            try
            {
                var result = empRepo.GetEmployeeList(empName, empCode, designationID, empTypeID);
                result = result.Where(x => !x.EmployeeTypeName.Equals("Regular")).ToList();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetEmployeeDetails_Result, Model.Employees.NonRegularEmployee>()
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
                var lstEmployee = Mapper.Map<List<Model.Employees.NonRegularEmployee>>(result);
                return lstEmployee;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool UpdateEmployeeGeneralDetails(Model.Employees.NonRegularEmployee employee)
        {
            log.Info($"NonRegularEmployeesService/UpdateEmployeeGeneralDetails");
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
                dtoEmp.DossierNo = employee.DossierNo;
                dtoEmp.SectionID = employee.SectionID;
                dtoEmp.DepartmentID = employee.DepartmentID;
                dtoEmp.UpdatedBy = employee.UpdatedBy;
                dtoEmp.UpdatedOn = employee.UpdatedOn;
                dtoEmp.IsJoinAfterNoon = employee.IsJoinAfterNoon;
                genericRepo.Update<DTOModel.tblMstEmployee>(dtoEmp);

                #region ==== Update Department ID in User Table
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
                    empSalRow.E_Basic = employee.E_Basic;
                    if (employee.modOfPayment == Model.Employees.ModeOfPayment.Bank)
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
                    #region Add Employee Mode of Payment Detail in Employee Salary Table

                    var newEmpSalRow = new DTOModel.TblMstEmployeeSalary();
                    newEmpSalRow.EmployeeCode = employee.EmployeeCode;
                    newEmpSalRow.EmployeeID = employee.EmployeeID;
                    newEmpSalRow.BranchID = employee.BranchID;
                    newEmpSalRow.ModePay = ((Model.Employees.ModeOfPayment)employee.modOfPayment).GetDisplayName().Trim();
                    var branchCode = genericRepo.GetByID<DTOModel.Branch>(newEmpSalRow.BranchID).BranchCode;
                    newEmpSalRow.BranchCode = branchCode   /*dtoemployeeDetails.Branch.BranchCode*/;
                    newEmpSalRow.CreatedBy = employee.UpdatedBy ?? 0;
                    newEmpSalRow.CreatedOn = employee.UpdatedOn.Value;
                    newEmpSalRow.IsSalgenrated = false;
                    newEmpSalRow.E_Basic = employee.E_Basic;
                    if (employee.modOfPayment == Model.Employees.ModeOfPayment.Bank)
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
        public bool DeleteEmployee(int employeeID)
        {
            log.Info($"NonRegularEmployeesService/DeleteEmployee/{employeeID}");
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
        public string GetEmployeeProfilePath(string empCode)
        {
            log.Info($"NonRegularEmployeesService/GetEmployeeProfilePath/{empCode}");
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
            log.Info($"NonRegularEmployeesService/GetEmployeeProfile/employeeID={employeeID}");
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
            log.Info($"NonRegularEmployeesService/CheckEmployeeCodeExistance/empCode={empCode}");
            return genericRepo.Exists<DTOModel.tblMstEmployee>(x => x.EmployeeCode == empCode && !x.IsDeleted);
        }
        public List<Model.Employees.NonRegularEmployee> GetEmployeeDetailsByCode(string empCode)
        {
            log.Info($"NonRegularEmployeesService/GetEmployeeDetailsByCode/empCode={empCode}");
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && x.EmployeeCode == empCode).Select(em => new Model.Employees.NonRegularEmployee()
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
        public bool UpdatetEmployeePersonalDetails(Model.Employees.NonRegularEmployee employeeDetails)
        {
            log.Info($"NonRegularEmployeesService/UpdatetEmployeePersonalDetails");
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
        public Model.Employees.NonRegularEmployee GetEmployeeByID(int employeeID)
        {
            log.Info($"NonRegularEmployeesService/GetEmployeeByID/{employeeID}");
            try
            {
                var dtoEmployee = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblMstEmployee, Model.Employees.NonRegularEmployee>()
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
                    //  .ForMember(c => c.EmpPayScale, c => c.MapFrom(s => s.PayScale))
                    .ForMember(c => c.IsJoinAfterNoon, c => c.MapFrom(s => s.IsJoinAfterNoon))
                     .ForMember(c => c.EmpCatID, c => c.MapFrom(s => s.EmpCatID))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var employee = Mapper.Map<DTOModel.tblMstEmployee, Model.Employees.NonRegularEmployee>(dtoEmployee);

                var salary = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeID == employee.EmployeeID).FirstOrDefault();
                if (salary != null)
                    employee.E_Basic = salary.E_Basic;
                //if (string.IsNullOrEmpty(employee.PayScale))
                //{
                //    var emp_payScale = empRepo.GetDesignationPayScaleList(dtoEmployee.DesignationID).FirstOrDefault();
                //    employee.PayScale = emp_payScale?.PayScale ?? null;
                //}
                return employee;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<Model.Employees.NonRegularEmployee> GetEmployeesByBranchID(int? branchID)
        {
            log.Info($"NonRegularEmployeesService/GetEmployeeList");
            try
            {

                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && (branchID == 0 ? (1 > 0) : x.BranchID == (int)branchID));

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.Employees.NonRegularEmployee>()
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
                var lstEmployee = Mapper.Map<List<Model.Employees.NonRegularEmployee>>(result);
                return lstEmployee.OrderBy(x => x.EmployeeCode).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
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
        public List<Model.EmployeeCertification> GetEmployeeCertification(int employeeID)
        {

            log.Info($"NonRegularEmployeesService/GetEmployeeCertification/employeeID:{employeeID}");

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
        public Model.EmployeeSalary GetEmployeeSalaryDtls(string empCode)
        {
            Model.EmployeeSalary objEmpSalary = new Model.EmployeeSalary();
            log.Info($"NonRegularEmployeesService/GetEmployeeSalaryDtls/{empCode}");
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
        public Model.Employees.NonRegularEmployee GetEmployeePaymentModeDtls(string empCode)
        {
            log.Info($"EmployeeService/GetEmployeePaymentModeDtls/{empCode}");
            Model.Employees.NonRegularEmployee employee = new Model.Employees.NonRegularEmployee();
            var empSalRow = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeCode == empCode).FirstOrDefault();
            if (empSalRow != null)
            {
                employee.BankAcNo = empSalRow.BankAcNo; employee.BankCode = empSalRow.BankCode;
                if (empSalRow.ModePay?.Trim() == "CASH")
                    employee.modOfPayment = Model.Employees.ModeOfPayment.Cash;
            }
            return employee;
        }

        public List<Model.Employees.NonRegularEmployeesExtension> GetEmployeeContractExtensionList(int employeeID)
        {
            log.Info($"NonRegularEmployeesService/GetEmployeeContractExtensionList/{employeeID}");
            try
            {
                var dtoEmpExtension = genericRepo.Get<DTOModel.NREmployeesContractExtention>(x => x.EmployeeId == employeeID && x.Deleted == false).OrderByDescending(o => o.Id);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.NREmployeesContractExtention, Model.Employees.NonRegularEmployeesExtension>()
                    .ForMember(c => c.Id, c => c.MapFrom(s => s.Id))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.EmplyeeName, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.ExtentionNoticeDate, c => c.MapFrom(s => s.ExtentionNoticeDate))
                    .ForMember(c => c.ExtentionFromDate, c => c.MapFrom(s => s.ExtentionFromDate))
                    .ForMember(c => c.ExtentionToDate, c => c.MapFrom(s => s.ExtentionToDate))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var extension = Mapper.Map<List<DTOModel.NREmployeesContractExtention>, List<Model.Employees.NonRegularEmployeesExtension>>(dtoEmpExtension.ToList());
                return extension;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Model.Employees.NonRegularEmployeesExtension GetEmployeeContractExtension(int employeeID, int id)
        {
            log.Info($"NonRegularEmployeesService/GetEmployeeContractExtension/{employeeID}");
            try
            {
                var dtoEmpExtension = genericRepo.Get<DTOModel.NREmployeesContractExtention>(x => x.EmployeeId == employeeID && x.Id == id && x.Deleted == false).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.NREmployeesContractExtention, Model.Employees.NonRegularEmployeesExtension>()
                    .ForMember(c => c.Id, c => c.MapFrom(s => s.Id))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.EmplyeeName, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.ExtentionNoticeDate, c => c.MapFrom(s => s.ExtentionNoticeDate))
                    .ForMember(c => c.ExtentionFromDate, c => c.MapFrom(s => s.ExtentionFromDate))
                    .ForMember(c => c.ExtentionToDate, c => c.MapFrom(s => s.ExtentionToDate))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var extension = Mapper.Map<DTOModel.NREmployeesContractExtention, Model.Employees.NonRegularEmployeesExtension>(dtoEmpExtension);
                return extension;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertUpdateContractExtension(NonRegularEmployeesExtension model)
        {
            log.Info($"NonRegularEmployeesService/InsertUpdateContractExtension");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Employees.NonRegularEmployeesExtension, DTOModel.NREmployeesContractExtention>()
                    .ForMember(c => c.Id, c => c.MapFrom(s => s.Id))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.ExtentionNoticeDate, c => c.MapFrom(s => s.ExtentionNoticeDate))
                    .ForMember(c => c.ExtentionFromDate, c => c.MapFrom(s => s.ExtentionFromDate))
                    .ForMember(c => c.ExtentionToDate, c => c.MapFrom(s => s.ExtentionToDate))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var extension = Mapper.Map<Model.Employees.NonRegularEmployeesExtension, DTOModel.NREmployeesContractExtention>(model);
                if (extension.Id == 0)
                {
                    genericRepo.Insert<DTOModel.NREmployeesContractExtention>(extension); // insert data in table
                }
                else
                {
                    var dtoObj = genericRepo.GetByID<DTOModel.NREmployeesContractExtention>(extension.Id);
                    dtoObj.ExtentionNoticeDate = extension.ExtentionNoticeDate;
                    dtoObj.ExtentionFromDate = extension.ExtentionFromDate;
                    dtoObj.ExtentionToDate = extension.ExtentionToDate;
                    dtoObj.UpdatedBy = extension.UpdatedBy;
                    dtoObj.UpdatedOn = extension.UpdatedOn;
                    genericRepo.Update<DTOModel.NREmployeesContractExtention>(dtoObj);
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool DeleteContractExtension(int id)
        {
            log.Info($"NonRegularEmployeesService/DeleteContractExtension{id}");
            try
            {
                var extension = genericRepo.GetByID<DTOModel.NREmployeesContractExtention>(id);
                extension.Deleted = true;
                genericRepo.Update<DTOModel.NREmployeesContractExtention>(extension);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}

