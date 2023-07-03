using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Nafed.MicroPay.Common;

namespace Nafed.MicroPay.Services
{
    public class DropdownBindService : BaseService, IDropdownBindService
    {
        private readonly IGenericRepository genericRepo;
        public DropdownBindService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;

        }

        public List<SelectListModel> ddlSkillType()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.SkillType>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SkillType, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.SkillTypeID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.SkillType1));
                });
                var ddlSkillTypeList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlSkillTypeList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlSkill(int? skillTypeID)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Skill>(x => x.SkillTypeID == skillTypeID && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Skill, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.SkillId))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.Skill1));
                });
                var ddlSkillList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlSkillList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Model.SelectListModel> ddlMenuList()
        {
            log.Info($"DropdownBindService/GetMenuList");

            try
            {
                var result = genericRepo.Get<DTOModel.Menu>(x => x.IsActive == true);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Menu, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.MenuID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.MenuName));
                });
                return Mapper.Map<List<Model.SelectListModel>>(result);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public List<SelectListModel> ddlDepartmentList()
        {
            log.Info($"DropdownBindService/ddlDepartmentList");
            try
            {
                var result = genericRepo.Get<DTOModel.Department>(x => x.DepartmentName != null && x.IsDeleted == false && x.IsActive == true);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Department, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.DepartmentID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.DepartmentName));
                });
                var ddlDepartmentList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlDepartmentList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> ddlUserTypeList()
        {
            log.Info($"DropdownBindService/ddlUserTypeList");
            try
            {
                var result = genericRepo.Get<DTOModel.UserType>(x => x.UserTypeName != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.UserType, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.UserTypeID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.UserTypeName));
                });
                var ddlUserType = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlUserType.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> ddlCategoryList()
        {
            log.Info($"DropdownBindService/ddlCategoryList");
            try
            {
                var result = genericRepo.Get<DTOModel.Category>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Category, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.CategoryID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.CategoryName));
                });
                var ddlCategoryList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlCategoryList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> ddlCadreList()
        {
            log.Info($"DropdownBindService/ddlCadreList");
            try
            {
                var result = genericRepo.Get<DTOModel.Cadre>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Cadre, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.CadreID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.CadreName));
                });
                var ddlCadreList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlCadreList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> ddlCityList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.City>(x => x.CityName != null);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.City, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.CityID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.CityName));
                });
                var ddlCityList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlCityList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SelectListModel> ddlGradeList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Grade>(x => x.GradeName != null);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Grade, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.GradeID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.GradeName));
                });
                return Mapper.Map<List<Model.SelectListModel>>(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SelectListModel> ddlTitleList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Title>(x => x.TitleName != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Title, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.TitleID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.TitleName));
                });
                var ddlTitleList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlTitleList.OrderBy(x => x.value).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlEmployeeTypeList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.EmployeeType>(x => x.EmployeeTypeName != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeType, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeTypeID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeTypeName));
                });
                var ddlEmployeeTypeList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlEmployeeTypeList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlGenderList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Gender>(x => x.Name != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Gender, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.GenderID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.Name));
                });
                var ddlGenderList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlGenderList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlDesignationList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Designation>(x => x.DesignationName != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Designation, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.DesignationID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.DesignationName));
                });
                var ddlDesignationList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlDesignationList.OrderBy(x => x.value).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlCadreCodeList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Cadre>(x => x.CadreCode != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Cadre, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.CadreID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.CadreCode));
                });
                var ddlCadreCodeList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlCadreCodeList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlBranchList(int? branchID = null, int? userTypeID = null)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Branch>(x => x.BranchName != null && x.IsDeleted != true &&
               userTypeID.HasValue ? ((userTypeID == (int)UserType.SuperUser || userTypeID == (int)UserType.Admin) ? 1 > 0 : x.BranchID == branchID) : (x.IsDeleted != true));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Branch, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.BranchID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.BranchName));
                });
                var ddlBranchList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlBranchList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlSectionList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Section>(x => x.SectionName != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Section, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.SectionID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.SectionName));
                });
                var ddlSectionList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlSectionList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlReligionList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Religion>(x => x.ReligionName != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Religion, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.ReligionID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.ReligionName));
                });
                var ddlReligionList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlReligionList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SelectListModel> ddlMotherTongueList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.MotherTongue>(x => x.Name != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.MotherTongue, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.ID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.Name));
                });
                var ddlMotherTongueList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlMotherTongueList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlMaritalStsList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.MaritalStatu>(x => x.MaritalStatusName != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.MaritalStatu, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.MaritalStatusID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.MaritalStatusName));
                });
                var ddlMaritalStsList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlMaritalStsList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlBloodGroupList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.BloodGroup>(x => x.BloodGroupName != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.BloodGroup, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.BloodGroupID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.BloodGroupName));
                });
                var ddlBloodGroupList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlBloodGroupList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SelectListModel> ddlRelationList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Relation>(x => x.RelationName != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Relation, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.RelationID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.RelationName));
                });
                var ddlRelationList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlRelationList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SelectListModel> ddlEmployeeCategoryList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.EmployeeCategory>(x => x.EmplCatName != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeCategory, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(s => s.EmplCatID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.EmplCatName));
                });
                var ddlEmployeeCategoryList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlEmployeeCategoryList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlAcedamicAndProfDtls(int typeID)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.AcadmicProfessionalDetail>(x => x.Value != null && x.TypeID == typeID && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AcadmicProfessionalDetail, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.ID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.Value));
                });
                var ddlAcedamicAndProfDtls = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlAcedamicAndProfDtls.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlCalendarYearList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.CalendarYear>(x => x.C_YearName != null && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.CalendarYear, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.C_YeraID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.C_YearName));
                });
                var ddlCalendarYearList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlCalendarYearList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SelectListModel> ddlLeaveCategoryList(int? genderID = null, string employeeLevel = null, int? emptypeID = null)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.LeaveCategory>(x => !x.IsDeleted
                && (genderID.HasValue ? (x.GenderID == genderID || x.GenderID == null) : (1 > 0))
                 && (emptypeID.HasValue ? (x.EmployeeTypeID == emptypeID || x.EmployeeTypeID == null) : (1 > 0)));

                if (string.IsNullOrEmpty(employeeLevel))
                {
                    // result = result.Where(x =>  (x.AllowLevelUpto >= employeeLevel || x.AllowLevelUpto==null));
                    result = result.Where(x => (x.AllowLevelUpto == null));
                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.LeaveCategory, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.LeaveCategoryID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.LeaveCategoryName));
                });
                var ddlLeaveCategoryList = Mapper.Map<List<Model.SelectListModel>>(result);

                return ddlLeaveCategoryList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> employeeByBranchID(int branchID, int? employeeID = null, int? userTypeID = null)
        {
            try
            {
                //var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && x.BranchID == branchID &&
                //(userTypeID == (int)UserType.SuperUser || userTypeID == (int)UserType.Admin) || userTypeID == null ? 1 > 0 : x.EmployeeId == employeeID);

                // All Branch Except Head Office(44)
                if (branchID == 0)
                {
                    var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.IsDeleted == false && x.BranchID != 44 && x.DOLeaveOrg == null
                                    &&
                                    (employeeID.HasValue ? (userTypeID == (int)UserType.SuperUser || userTypeID == (int)UserType.Admin) || userTypeID == null ? 1 > 0 : x.EmployeeId == employeeID : (1 > 0)

                                    ));
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                        .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeCode + "-" + s.Name));
                    });
                    var employeeByBranchID = Mapper.Map<List<Model.SelectListModel>>(result);
                    return employeeByBranchID.OrderBy(x => x.value).ToList();
                }
                else
                {
                    var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.IsDeleted == false && x.BranchID == branchID && x.DOLeaveOrg == null
                &&
                (employeeID.HasValue ? (userTypeID == (int)UserType.SuperUser || userTypeID == (int)UserType.Admin) || userTypeID == null ? 1 > 0 : x.EmployeeId == employeeID : (1 > 0)

                ));
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                        .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeCode + "-" + s.Name));
                    });
                    var employeeByBranchID = Mapper.Map<List<Model.SelectListModel>>(result);
                    return employeeByBranchID.OrderBy(x => x.value).ToList();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public List<SelectListModel> employeeByBranchID(int branchID, int? employeeID = null, int? userTypeID = null)
        //{
        //    try
        //    {
        //        var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && x.BranchID == branchID &&
        //        (userTypeID == (int)UserType.SuperUser || userTypeID == (int)UserType.Admin) ? 1 > 0 : x.EmployeeId == employeeID);
        //        Mapper.Initialize(cfg =>
        //        {
        //            cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
        //            .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
        //            .ForMember(d => d.value, o => o.MapFrom(s => s.Name));
        //        });
        //        return Mapper.Map<List<Model.SelectListModel>>(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public List<SelectListModel> ddlAttendanceTypeList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.AttendanceType>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AttendanceType, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.TypeID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.Name));
                });
                var ddlAttendanceTypeList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlAttendanceTypeList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> employeeReportingByID(int? EmpID)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && x.EmployeeId == EmpID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeCode + "-" + s.Name));
                });
                var employeeReporting = Mapper.Map<List<Model.SelectListModel>>(result);
                return employeeReporting.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> employeeLeavePurposeByID(int LeaveCatID)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.LeavePurpose>(x => !x.IsDeleted && x.LeaveCategoryID == LeaveCatID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.LeavePurpose, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.LeavePurposeID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.LeavePurposeName));
                });
                var employeeLeavePurpose = Mapper.Map<List<Model.SelectListModel>>(result);
                return employeeLeavePurpose.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlstatus()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.LeaveStatu>(x => x.StatusDesc != null);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.LeaveStatu, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.StatusID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.StatusDesc));
                });
                var ddlstatus = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlstatus.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<dynamic> ddlBanks()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstBank>(x => !x.IsDeleted);
                return result.Select(x => new { x.BankCode, x.BankName }).ToList<dynamic>();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<SelectListModel> GetBank()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstBank>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstBank, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.BankCode))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.BankName));
                });
                var ddlstatus = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlstatus.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetStateName(int stateID)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.State>(x => x.StateID == stateID);
                if (result != null)
                    return result.FirstOrDefault().StateName;
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<SelectListModel> GetAllEmployee(int? employeeID = null)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && x.DOLeaveOrg == null && (employeeID.HasValue ? x.EmployeeId == employeeID : (1 > 0)));
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

        public List<SelectListModel> ddlLeaveType()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.LeaveCategory>(x => x.IsDeleted == false);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.LeaveCategory, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.LeaveCategoryID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.LeaveCategoryName));
                });
                var ddlLeaveType = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlLeaveType.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlAppraisalForm()
        {
            log.Info($"DropdownBindService/ddlAppraisalForm");
            try
            {
                var result = genericRepo.Get<DTOModel.AppraisalForm>(x => x.IsDeleted == false);
                var ReportingYr = result.Select(x => x.ReportingYr).LastOrDefault();
                result = result.Where(x => x.ReportingYr == ReportingYr);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AppraisalForm, Model.SelectListModel>()
                    .ForMember(d => d.id, o => o.MapFrom(s => s.FormID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.FormName));
                });
                var ddlforms = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlforms.OrderBy(x => x.value).Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> GetAllEmployeeByProcessID(int employeeID, WorkFlowProcess wrkProcess)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.EmployeeProcessApproval>(x => x.ProcessID == (int)wrkProcess && x.ToDate == null && (x.ReportingTo == employeeID || x.ReviewingTo == employeeID || x.AcceptanceAuthority == employeeID));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeProcessApproval, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode + "-" + s.tblMstEmployee.Name));
                });
                var employeedetail = Mapper.Map<List<Model.SelectListModel>>(result);
                return employeedetail.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlFirstBranchList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Branch>(x => x.BranchName != null);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Branch, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.BranchID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.BranchName));
                });
                var ddlBranchList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlBranchList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlFirstDesignationList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Designation>(x => x.DesignationName != null);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Designation, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.DesignationID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.DesignationName));
                });
                var ddlDesignationList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlDesignationList.OrderBy(x => x.value).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlStateList()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.State>(x => x.StateName != null);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.State, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.StateID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.StateName));
                });
                var ddlStateList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlStateList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> GetLoanType()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstLoanType>();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstLoanType, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.LoanTypeId))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.LoanDesc));
                });
                var ddlLoanTypeList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlLoanTypeList.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SelectListModel GetEmployeeByPFNumber(int PFNumber)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DOLeaveOrg == null && !x.IsDeleted && x.PFNO != null);
                result = result.Where(x => x.PFNO == PFNumber);
                if (result.Count() > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                        .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeCode + "-" + s.Name));
                    });
                    var ddlEmployeeDetails = Mapper.Map<Model.SelectListModel>(result.FirstOrDefault());
                    return ddlEmployeeDetails;
                }
                else
                    return new SelectListModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SelectListModel GetBranchByEmployeeId(int employeeId)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DOLeaveOrg == null && !x.IsDeleted && x.EmployeeId == employeeId);
                if (result.Count() > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                        .ForMember(c => c.id, c => c.MapFrom(m => m.BranchID))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.Branch.BranchCode));
                    });
                    var ddlEmployeeDetails = Mapper.Map<Model.SelectListModel>(result.FirstOrDefault());
                    return ddlEmployeeDetails;
                }
                else
                    return new SelectListModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalaryHeadField> GetSalaryHeadForIndividualHead(int employeeTypeId)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.SalaryHead>(x => !x.IsDeleted && x.ActiveField == true && x.EmployeeTypeID == employeeTypeId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SalaryHead, Model.SalaryHeadField>()
                    .ForMember(c => c.FieldName, c => c.MapFrom(m => m.FieldName))
                    .ForMember(d => d.FieldDesc, o => o.MapFrom(s => s.FieldDesc));
                });
                var ddlSalaryHeadList = Mapper.Map<List<Model.SalaryHeadField>>(result);
                return ddlSalaryHeadList.OrderBy(x => x.FieldName).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalaryHeadField> GetSalaryHead(int employeeTypeId)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.SalaryHead>(x => !x.IsDeleted && x.MonthlyInput == true && x.ActiveField == true && x.EmployeeTypeID == employeeTypeId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SalaryHead, Model.SalaryHeadField>()
                    .ForMember(c => c.FieldName, c => c.MapFrom(m => m.FieldName))
                    .ForMember(d => d.FieldDesc, o => o.MapFrom(s => s.FieldDesc));
                });
                var ddlSalaryHeadList = Mapper.Map<List<Model.SalaryHeadField>>(result);
                return ddlSalaryHeadList.OrderBy(x => x.FieldName).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> GetEmployeeDetailsByEmployeeType(int? branchId, int? employeeTypeId)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DOLeaveOrg == null && !x.IsDeleted && (branchId.HasValue ? x.BranchID == branchId : 1 > 0) && (employeeTypeId.HasValue ? x.EmployeeTypeID == employeeTypeId : 1 > 0));
                if (result.Count() > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                        .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeCode + "-" + s.Name));
                    });
                    var ddlEmployeeDetails = Mapper.Map<List<Model.SelectListModel>>(result);
                    return ddlEmployeeDetails;
                }
                else
                    return new List<SelectListModel>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> GetEmployeeByDepartmentDesignationID(int? branchID, int? departmentID, int? designationID)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DOLeaveOrg == null && !x.IsDeleted && (branchID.HasValue ? x.BranchID == branchID : 1 > 0) && (departmentID.HasValue ? x.DepartmentID == departmentID : 1 > 0) &&
                (designationID.HasValue ? x.DesignationID == designationID : 1 > 0));
                if (result.Count() > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                        .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeCode + "-" + s.Name));
                    });
                    var ddlEmployeeDetails = Mapper.Map<List<Model.SelectListModel>>(result);
                    return ddlEmployeeDetails;
                }
                else
                    return new List<SelectListModel>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<SelectListModel> GetFileTrackingType()
        {
            log.Info($"DropdownBindService/ddlCategoryList");
            try
            {
                var getdata = genericRepo.Get<DTOModel.FileTrackingType>(x => !x.IsDeleted);
                if (getdata != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.FileTrackingType, SelectListModel>()
                      .ForMember(c => c.id, c => c.MapFrom(m => m.ID))
                      .ForMember(d => d.value, o => o.MapFrom(s => s.FileType));
                    });
                    var ddlFileType = Mapper.Map<List<Model.SelectListModel>>(getdata);
                    return ddlFileType;
                }
                else
                    return new List<SelectListModel>();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public string GetBranchCode(int branchId)
        {
            try
            {
                var branchCode = genericRepo.Get<DTOModel.Branch>(x => !x.IsDeleted && x.BranchID == branchId).FirstOrDefault().BranchCode;
                return branchCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> GetStaffBudget()
        {
            log.Info($"DropdownBindService/GetStaffBudget");
            try
            {
                var result = genericRepo.Get<DTOModel.tblStaffBudget>().Where(x => x.Year != null && x.Year != "");
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblStaffBudget, Model.SelectListModel>()
                    .ForMember(d => d.value, o => o.MapFrom(s => s.Year));
                });
                var ddlStaffBudgetList = Mapper.Map<List<Model.SelectListModel>>(result);
                var sno = 1;
                ddlStaffBudgetList.ForEach(x =>
                {
                    x.id = sno;
                    sno++;
                });
                return ddlStaffBudgetList.Distinct().OrderByDescending(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> GetAssetType()
        {
            log.Info($"DropdownBindService/GetAssetType");
            try
            {
                var result = genericRepo.Get<DTOModel.AssetType>().Where(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AssetType, Model.SelectListModel>()
                    .ForMember(d => d.id, o => o.MapFrom(s => s.AssetTypeID))
                     .ForMember(d => d.value, o => o.MapFrom(s => s.AssetTypeName));
                });
                var ddlAssetTypeList = Mapper.Map<List<Model.SelectListModel>>(result);
                if (ddlAssetTypeList != null)
                {
                    return ddlAssetTypeList.OrderBy(x => x.value).ToList();
                }
                else
                {
                    return new List<SelectListModel>();
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<SelectListModel> GetManufacturer()
        {
            log.Info($"DropdownBindService/GetManufacturer");
            try
            {
                var result = genericRepo.Get<DTOModel.Manufacturer>().Where(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Manufacturer, Model.SelectListModel>()
                    .ForMember(d => d.id, o => o.MapFrom(s => s.ManufacturerID))
                     .ForMember(d => d.value, o => o.MapFrom(s => s.ManufacturerName));
                });
                var ddlManufacturerList = Mapper.Map<List<Model.SelectListModel>>(result);
                if (ddlManufacturerList != null)
                {
                    return ddlManufacturerList.OrderBy(x => x.value).ToList();
                }
                else
                {
                    return new List<SelectListModel>();
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> GetAssetName(int statusID, int assetTypeID)
        {
            log.Info($"DropdownBindService/GetAssetName");
            try
            {
                var result = genericRepo.Get<DTOModel.InventoryManagement>().Where(x => !x.IsDeleted
                  && (statusID > 0 ? x.StatusID == statusID : 1 > 0)
                 && (assetTypeID > 0 ? x.AssetTypeID == assetTypeID : 1 > 0));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.InventoryManagement, Model.SelectListModel>()
                    .ForMember(d => d.id, o => o.MapFrom(s => s.IMID))
                     .ForMember(d => d.value, o => o.MapFrom(s => s.AssetName));
                });
                var ddlAssetNameList = Mapper.Map<List<Model.SelectListModel>>(result);
                if (ddlAssetNameList != null)
                {
                    return ddlAssetNameList.OrderBy(x => x.value).ToList();
                }
                else
                {
                    return new List<SelectListModel>();
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> GetTicketType()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.ticket_type>(x => x.IsDeleted == false);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ticket_type, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.ID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.code));
                });
                var ddlTicketType = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlTicketType.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> GetTicketPriority()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.ticket_priority>(x => x.IsDeleted == false);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.ticket_priority, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.ID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.description));
                });
                var ddlTicketPriority = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlTicketPriority.OrderBy(x => x.id).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> GetFinanceYear()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.AppraisalForm>().Select(x => x.ReportingYr).Distinct().ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<string, SelectListModel>()
                    .ForMember(d => d.value, o => o.MapFrom(s => s))
                    .ForMember(d => d.id, o => o.UseValue(0));

                });
                var ddlFinanceYr = Mapper.Map<List<SelectListModel>>(result);
                var sno = 1;
                ddlFinanceYr.ForEach(x =>
                {
                    x.id = sno;
                    sno++;
                });
                ddlFinanceYr.OrderByDescending(x => x.value);
                return ddlFinanceYr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> GetEmployeeByManager(int employeeID)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.EmployeeProcessApproval>(x => (x.ReportingTo == employeeID) && x.ProcessID == 5 && x.ToDate == null).Select(S => S.EmployeeID).ToList();
                result.Add(employeeID);
                var empList = genericRepo.Get<DTOModel.tblMstEmployee>(x => result.Contains(x.EmployeeId)).ToList();
                if (result.Count() > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                        .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeCode + "-" + s.Name));
                    });
                    var ddlEmployeeDetails = Mapper.Map<List<Model.SelectListModel>>(empList);

                    return ddlEmployeeDetails;
                }
                else
                    return new List<SelectListModel>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> GetNegativeSalEmployee(int salYear, int salMonth)
        {
            try
            {
                var getEmp = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x => x.SalYear == salYear && x.SalMonth == salMonth && x.chkNegative == true).Select(s => s.EmployeeCode);
                if (getEmp.Count() > 0)
                {
                    var empCodes = genericRepo.Get<DTOModel.tblMstEmployee>(x => getEmp.Any(r => r == x.EmployeeCode)).Select(m => new SelectListModel() { id = m.EmployeeId, value = m.EmployeeCode + "-" + m.Name }).ToList();

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<SelectListModel, Model.SelectListModel>()
                        .ForMember(c => c.id, c => c.MapFrom(m => m.id))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.value));
                    });

                    var ddlEmployee = Mapper.Map<List<Model.SelectListModel>>(empCodes);
                    return ddlEmployee;
                }

                else
                    return new List<SelectListModel>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlDesignationListForTicket()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Designation>(x => x.DesignationName != null && !x.IsDeleted && x.tblMstEmployees.Any(a => a.DesignationID == x.DesignationID && a.DOLeaveOrg == null));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Designation, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.DesignationID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.DesignationName));
                });
                var ddlDesignationList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlDesignationList.OrderBy(x => x.value).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlDepartmentHavingEmployee()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Department>(x => x.DepartmentName != null && !x.IsDeleted && x.tblMstEmployees.Any(a => a.DepartmentID == x.DepartmentID && a.DOLeaveOrg == null));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Department, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.DepartmentID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.DepartmentName));
                });
                var ddlDepartmentList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlDepartmentList.OrderBy(x => x.value).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SelectListModel> GetEmployee()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>();
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
        public List<SelectListModel> GetCurrExEmployeeDetailsByEmployeeType(int? branchId, int? employeeTypeId)
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => (branchId.HasValue ? x.BranchID == branchId : 1 > 0) && (employeeTypeId.HasValue ? x.EmployeeTypeID == employeeTypeId : 1 > 0));
                if (result.Count() > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                        .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeCode + "-" + s.Name));
                    });
                    var ddlEmployeeDetails = Mapper.Map<List<Model.SelectListModel>>(result);
                    return ddlEmployeeDetails;
                }
                else
                    return new List<SelectListModel>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> ddlDepartmentHavingTicket()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.Department>(x => x.DepartmentName != null && !x.IsDeleted && x.TicketWorkFlows.Any(a => a.DepartmentID == x.DepartmentID));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Department, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.DepartmentID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.DepartmentName));
                });
                var ddlDepartmentList = Mapper.Map<List<Model.SelectListModel>>(result);
                return ddlDepartmentList.OrderBy(x => x.value).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
