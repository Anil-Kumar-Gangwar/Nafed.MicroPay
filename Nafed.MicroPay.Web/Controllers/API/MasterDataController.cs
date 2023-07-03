using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Model.API;
using AutoMapper;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Net.Http.Formatting;
using Nafed.MicroPay.ImportExport;

namespace MicroPay.Web.Controllers.API
{
    public class MasterDataController : BaseController
    {
        private IGenericRepository genericRepo;
        private IEmployeeRepository empRepo;
        private readonly IEmployeeService employeeService;
        private readonly ILeaveService leaveRuleService;
        private readonly ILeaveRepository leaveRepo;
        private readonly Nafed.MicroPay.ImportExport.Interfaces.IExport export;
        public MasterDataController()
        {
            genericRepo = new GenericRepository();
            empRepo = new EmployeeRepository();
            leaveRepo = new LeaveRepository();
            export = new Export();
            employeeService = new EmployeeService(genericRepo, empRepo,export);
            leaveRuleService = new LeaveService(genericRepo, leaveRepo);
        }
        public HttpResponseMessage GetEmployeeType()
        {
            log.Info($"MasterDataController/GetMasterData");

            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                string result = String.Empty;
                IEnumerable<EmployeeType> empType = null;
                var emplType = (genericRepo.Get<DTOModel.EmployeeType>(em => !em.IsDeleted)).Select(em => new EmployeeType()
                {
                    EmployeeTypeID = em.EmployeeTypeID,
                    EmployeeTypeName = em.EmployeeTypeName,
                    IsDeleted = em.IsDeleted,
                    EmployeeTypeCode = em.EmployeeTypeCode
                }).ToList();

                empType = emplType.ToList<EmployeeType>();
                AllMasterData obj = new AllMasterData() { EmployeeType = empType };
                result = js.Serialize(obj);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
                throw ex;
            }
        }
        public HttpResponseMessage GetEmployeeDetail()
        {
            log.Info($"MasterDataController/GetEmployeeDetail");
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                string result = String.Empty;
                IEnumerable<Employee> empDetails = null;
                var empData = employeeService.GetEmployeeList(null, null, null, null);
                empDetails = empData;
                AllMasterData obj = new AllMasterData() { EmployeeDetails = empDetails };
                result = js.Serialize(obj);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
                throw ex;
            }
        }

        [System.Web.Mvc.HttpPost]
        public HttpResponseMessage GetEmployeeByCode(FormDataCollection formData)
        {
            log.Info($"MasterDataController/GetEmployeeByCode");
            try
            {
                string EmployeeCode = formData.Get("EmployeeCode");
                string result = String.Empty;
                IEnumerable<Employee> empDetails = null;
                JavaScriptSerializer js = new JavaScriptSerializer();
                // this code is get all the details of perticular employee based on empID for api.
                if (!string.IsNullOrEmpty(EmployeeCode))
                {
                    var empData = employeeService.GetEmployeeDetailsByCode(EmployeeCode);
                    empDetails = empData;
                }
                // this code is to get all master data fro api.
                var title = genericRepo.Get<DTOModel.Title>(x => !x.IsDeleted).ToList();
                var empType = genericRepo.Get<DTOModel.EmployeeType>(x => !x.IsDeleted).ToList();
                var branch = genericRepo.Get<DTOModel.Branch>(x => !x.IsDeleted).ToList();
                var department = genericRepo.Get<DTOModel.Department>(x => !(bool)x.IsDeleted).ToList();
                var designation = genericRepo.Get<DTOModel.Designation>(x => !x.IsDeleted).ToList();
                var category = genericRepo.Get<DTOModel.Category>(x => !x.IsDeleted).ToList();
                var religion = genericRepo.Get<DTOModel.Religion>(x => !x.IsDeleted).ToList();
                var relation = genericRepo.Get<DTOModel.Relation>(x => !x.IsDeleted).ToList();
                var motherTongue = genericRepo.Get<DTOModel.MotherTongue>(x => !x.IsDeleted).ToList();
                var maritalStatus = genericRepo.Get<DTOModel.MaritalStatu>(x => !x.IsDeleted).ToList(); //lfet
                var cadre = genericRepo.Get<DTOModel.Cadre>(x => !(bool)x.IsDeleted).ToList();
                var division = genericRepo.Get<DTOModel.Division>(x => !x.IsDeleted).ToList();
                var section = genericRepo.Get<DTOModel.Section>(x => !x.IsDeleted).ToList();
                var bloodGroup = genericRepo.Get<DTOModel.BloodGroup>(x => !(bool)x.IsDeleted).ToList();
                var empCategory = genericRepo.Get<DTOModel.EmployeeCategory>(x => !x.IsDeleted).ToList();
                var gender = genericRepo.Get<DTOModel.Gender>(x => !x.IsDeleted).ToList();

                #region Title
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Title, Title>()

                     .ForMember(c => c.TitleID, c => c.MapFrom(m => m.TitleID))
                     .ForMember(c => c.TitleName, c => c.MapFrom(m => m.TitleName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objTitle = Mapper.Map<List<DTOModel.Title>, List<Title>>(title);
                #endregion
                #region EmployeeType
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeType, EmployeeType>()

                     .ForMember(c => c.EmployeeTypeCode, c => c.MapFrom(m => m.EmployeeTypeCode))
                     .ForMember(c => c.EmployeeTypeID, c => c.MapFrom(m => m.EmployeeTypeID))
                     .ForMember(c => c.EmployeeTypeName, c => c.MapFrom(m => m.EmployeeTypeName))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objempType = Mapper.Map<List<DTOModel.EmployeeType>, List<EmployeeType>>(empType);
                #endregion
                #region Branch
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Branch, Branch>()
                     .ForMember(c => c.BranchID, c => c.MapFrom(m => m.BranchID))
                     .ForMember(c => c.BranchCode, c => c.MapFrom(m => m.BranchCode))
                     .ForMember(c => c.BranchName, c => c.MapFrom(m => m.BranchName))
                     .ForMember(c => c.Address1, c => c.MapFrom(m => m.Address1))
                     .ForMember(c => c.Address2, c => c.MapFrom(m => m.Address2))
                     .ForMember(c => c.Address3, c => c.MapFrom(m => m.Address3))
                     .ForMember(c => c.Address1, c => c.MapFrom(m => m.Address1))
                     .ForMember(c => c.Pin, c => c.MapFrom(m => m.Pin))
                     .ForMember(c => c.CityID, c => c.MapFrom(m => m.CityID))
                     .ForMember(c => c.GradeID, c => c.MapFrom(m => m.GradeID))
                     .ForMember(c => c.Region, c => c.MapFrom(m => m.Region))
                     .ForMember(c => c.PhoneSTD, c => c.MapFrom(m => m.PhoneSTD))
                     .ForMember(c => c.PhoneNo, c => c.MapFrom(m => m.PhoneNo))
                     .ForMember(c => c.FaxNo, c => c.MapFrom(m => m.FaxNo))
                     .ForMember(c => c.FaxSTD, c => c.MapFrom(m => m.FaxSTD))
                     .ForMember(c => c.Remarks, c => c.MapFrom(m => m.Remarks))
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                     .ForMember(c => c.UpdatedBy, c => c.MapFrom(m => m.UpdatedBy))
                     .ForMember(c => c.UpdatedOn, c => c.MapFrom(m => m.UpdatedOn))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objbranch = Mapper.Map<List<DTOModel.Branch>, List<Branch>>(branch);
                #endregion
                #region Department
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Department, Department>()

                     .ForMember(c => c.DepartmentID, c => c.MapFrom(m => m.DepartmentID))
                     .ForMember(c => c.DepartmentCode, c => c.MapFrom(m => m.DepartmentCode))
                     .ForMember(c => c.DepartmentName, c => c.MapFrom(m => m.DepartmentName))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objdepartment = Mapper.Map<List<DTOModel.Department>, List<Department>>(department);
                #endregion
                #region Designation
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Designation, Designation>()
                     .ForMember(c => c.DesignationCode, c => c.MapFrom(s => s.DesignationCode))
                    .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.DesignationName))
                    .ForMember(c => c.Level, c => c.MapFrom(s => s.Level))
                    .ForMember(c => c.IsOfficer, c => c.MapFrom(s => s.IsOfficer))
                    .ForMember(c => c.Rank, c => c.MapFrom(s => s.Rank))
                    .ForMember(c => c.CadreID, c => c.MapFrom(s => s.CadreID))
                    .ForMember(c => c.DesignationID, c => c.MapFrom(s => s.DesignationID))
                    .ForMember(c => c.CadreID, c => c.MapFrom(s => s.CadreID))
                    .ForMember(c => c.LCT, c => c.MapFrom(s => s.LCT))
                    .ForMember(c => c.Promotion, c => c.MapFrom(s => s.Promotion))
                    .ForMember(c => c.Direct, c => c.MapFrom(s => s.Direct))
                    .ForMember(c => c.IsUpgradedDesignation, c => c.MapFrom(s => s.IsUpgradedDesignation))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objdesignation = Mapper.Map<List<DTOModel.Designation>, List<Designation>>(designation);
                #endregion
                #region Category
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Category, Category>()
                     .ForMember(c => c.CategoryID, c => c.MapFrom(m => m.CategoryID))
                     .ForMember(c => c.CategoryCode, c => c.MapFrom(m => m.CategoryCode))
                     .ForMember(c => c.CategoryName, c => c.MapFrom(m => m.CategoryName))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objcategory = Mapper.Map<List<DTOModel.Category>, List<Category>>(category);
                #endregion
                #region Religion
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Religion, Religion>()
                     .ForMember(c => c.ReligionID, c => c.MapFrom(m => m.ReligionID))
                     .ForMember(c => c.ReligionName, c => c.MapFrom(m => m.ReligionName))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objreligion = Mapper.Map<List<DTOModel.Religion>, List<Religion>>(religion);
                #endregion
                #region Relation
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Relation, Relation>()
                     .ForMember(c => c.RelationID, c => c.MapFrom(m => m.RelationID))
                     .ForMember(c => c.RelationCode, c => c.MapFrom(m => m.RelationCode))
                     .ForMember(c => c.RelationName, c => c.MapFrom(m => m.RelationName))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objrelation = Mapper.Map<List<DTOModel.Relation>, List<Relation>>(relation);
                #endregion
                #region MotherTongue
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.MotherTongue, MotherTongueModel>()

                     .ForMember(c => c.ID, c => c.MapFrom(m => m.ID))
                     .ForMember(c => c.ShortName, c => c.MapFrom(m => m.ShortName))
                     .ForMember(c => c.Name, c => c.MapFrom(m => m.Name))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objmotherTongue = Mapper.Map<List<DTOModel.MotherTongue>, List<MotherTongueModel>>(motherTongue);
                #endregion
                #region MaritalStatus
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.MaritalStatu, MaritalStatus>()

                     .ForMember(c => c.MaritalStatusID, c => c.MapFrom(m => m.MaritalStatusID))
                     .ForMember(c => c.MaritalStatusCode, c => c.MapFrom(m => m.MaritalStatusCode))
                     .ForMember(c => c.MaritalStatusName, c => c.MapFrom(m => m.MaritalStatusName))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objmaritalStatus = Mapper.Map<List<DTOModel.MaritalStatu>, List<MaritalStatus>>(maritalStatus);
                #endregion
                #region Cadre
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Cadre, Cadre>()
                     .ForMember(c => c.CadreID, c => c.MapFrom(m => m.CadreID))
                     .ForMember(c => c.CadreCode, c => c.MapFrom(m => m.CadreCode))
                     .ForMember(c => c.CadreName, c => c.MapFrom(m => m.CadreName))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objcadre = Mapper.Map<List<DTOModel.Cadre>, List<Cadre>>(cadre);
                #endregion
                #region Division
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Division, Division>()
                     .ForMember(c => c.DivisionID, c => c.MapFrom(m => m.DivisionID))
                     .ForMember(c => c.DivisionCode, c => c.MapFrom(m => m.DivisionCode))
                     .ForMember(c => c.DivisionName, c => c.MapFrom(m => m.DivisionName))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objdivision = Mapper.Map<List<DTOModel.Division>, List<Division>>(division);
                #endregion
                #region Section
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Section, Section>()
                     .ForMember(c => c.SectionID, c => c.MapFrom(m => m.SectionID))
                     .ForMember(c => c.SectionCode, c => c.MapFrom(m => m.SectionCode))
                     .ForMember(c => c.SectionName, c => c.MapFrom(m => m.SectionName))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objsection = Mapper.Map<List<DTOModel.Section>, List<Section>>(section);
                #endregion
                #region BloodGroup
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.BloodGroup, BloodGroup>()
                     .ForMember(c => c.BloodGroupID, c => c.MapFrom(m => m.BloodGroupID))
                     .ForMember(c => c.BloodGroupCode, c => c.MapFrom(m => m.BloodGroupCode))
                     .ForMember(c => c.BloodGroupName, c => c.MapFrom(m => m.BloodGroupName))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objbloodGroup = Mapper.Map<List<DTOModel.BloodGroup>, List<BloodGroup>>(bloodGroup);
                #endregion
                #region EmployeeCategory
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeCategory, EmployeeCategory>()
                     .ForMember(c => c.EmplCatID, c => c.MapFrom(m => m.EmplCatID))
                     .ForMember(c => c.EmplCatCode, c => c.MapFrom(m => m.EmplCatCode))
                     .ForMember(c => c.EmplCatName, c => c.MapFrom(m => m.EmplCatName))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objempCategory = Mapper.Map<List<DTOModel.EmployeeCategory>, List<EmployeeCategory>>(empCategory);
                #endregion
                #region Gender
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Gender, Gender>()
                     .ForMember(c => c.GenderID, c => c.MapFrom(m => m.GenderID))
                     .ForMember(c => c.Name, c => c.MapFrom(m => m.Name))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var objgender = Mapper.Map<List<DTOModel.Gender>, List<Gender>>(gender);
                #endregion
                #region EmployeeAttendance
                IEnumerable<EmpAttendance> empAttendance = null;
                var empAttendanceData = genericRepo.Get<DTOModel.EmpAttendanceHdr>(x => !x.IsDeleted).Select(em => new EmpAttendance
                {
                    EmpAttendanceID = em.EmpAttendanceID,
                    EmployeeId = em.EmployeeId,
                    ProxydateIn = em.ProxydateIn.Date,
                    ProxyOutDate = em.ProxyOutDate.Date,
                    InTime = em.InTime,
                    OutTime = em.OutTime,
                    Remarks = em.Remarks,
                    Attendancestatus = (int) em.Attendancestatus,
                    Reportingofficer = em.ReportingOfficer,
                    RejectionRemarks = em.ReportingToRemark,
                    TypeID = em.TypeID,
                    Mode = em.Mode,
                    Old_Intime = em.Old_Intime,
                    Old_outtime = em.Old_outtime,
                    IsDeleted = em.IsDeleted
                }).ToList();
                empAttendance = empAttendanceData;
                #endregion
                #region LeaveRule
                IEnumerable<LeaveRule> empLeaveRule = null;
                var empLeave = leaveRuleService.GetLeaveRuleList();
                empLeaveRule = empLeave;
                #endregion 
                AllMasterData allData = new AllMasterData
                {
                    EmployeeDetails = empDetails,
                    Title = objTitle,
                    EmployeeType = objempType,
                    Branch = objbranch,
                    Department = objdepartment,
                    Designation = objdesignation,
                    Category = objcategory,
                    Religion = objreligion,
                    Relation = objrelation,
                    MotherTongue = objmotherTongue,
                    MaritalStatus = objmaritalStatus,
                    Cadre = objcadre,
                    Division = objdivision,
                    Section = objsection,
                    BloodGroup = objbloodGroup,
                    EmployeeCategory = objempCategory,
                    Gender = objgender,
                    LeaveRule = empLeaveRule,
                    EmployeesAttendance = empAttendance
                };
                result = js.Serialize(allData);
                result = Regex.Replace(result, @"\""\\/Date\((-?\d+)\)\\/\""", "new Date($1)");
                // return Ok(result);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
                throw ex;
            }
        }
        public HttpResponseMessage GetEmployeeAttendance(int empID)
        {
            log.Info($"MasterDataController/GetEmployeeAttendance");
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                string result = String.Empty;
                IEnumerable<EmpAttendance> empAttendance = null;
                var empAttendanceData = genericRepo.Get<DTOModel.EmpAttendanceHdr>(x => !x.IsDeleted && x.EmployeeId == empID).Select(em => new EmpAttendance
                {
                    EmpAttendanceID = em.EmpAttendanceID,
                    EmployeeId = em.EmployeeId,
                    ProxydateIn = em.ProxydateIn.Date,
                    ProxyOutDate = em.ProxyOutDate.Date,
                    InTime = em.InTime,
                    OutTime = em.OutTime,
                    Remarks = em.Remarks,
                    Attendancestatus = (int)em.Attendancestatus,
                    Reportingofficer = em.ReportingOfficer,
                    RejectionRemarks = em.ReportingToRemark,
                    TypeID = em.TypeID,
                    Mode = em.Mode,
                    Old_Intime = em.Old_Intime,
                    Old_outtime = em.Old_outtime,
                    IsDeleted = em.IsDeleted
                }).ToList();
                empAttendance = empAttendanceData;
                AllMasterData obj = new AllMasterData() { EmployeesAttendance = empAttendance };
                result = js.Serialize(obj);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
                throw ex;
            }
        }
        public HttpResponseMessage GetLeaveRule()
        {
            log.Info($"MasterDataController/GetLeaveRule");
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                string result = String.Empty;
                IEnumerable<LeaveRule> empLeaveRule = null;
                var empLeave = leaveRuleService.GetLeaveRuleList();
                empLeaveRule = empLeave;
                AllMasterData obj = new AllMasterData() { LeaveRule = empLeave };
                result = js.Serialize(obj);
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
                throw ex;
            }
        }


    }
}
