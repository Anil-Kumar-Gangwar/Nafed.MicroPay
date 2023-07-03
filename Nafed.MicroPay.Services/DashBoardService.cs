using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using System.IO;

namespace Nafed.MicroPay.Services
{
    public class DashBoardService : BaseService, IDashBoardService
    {
        private readonly IDashBoardRepository dashBordService;
        private readonly IGenericRepository genericRepo;

        public DashBoardService(IDashBoardRepository dashBordService, IGenericRepository genericRepo)
        {
            this.dashBordService = dashBordService;
            this.genericRepo = genericRepo;
        }
        public IEnumerable<Dashboard> GetEmployeeDetailForDashBoard(int? empCode)
        {
            log.Info($"DashBoardService/GetEmployeeDetailForDashBoard/{empCode}");
            try
            {
                var employeeDetail = dashBordService.GetEmployeeDetailForDashBoard(empCode);
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetEmployeeDetailDashBoard_Result, Model.Dashboard>()
                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchName))
                .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.DesignationName))
                .ForMember(d => d.RM, o => o.MapFrom(s => s.RM))
                .ForMember(d => d.EXPERIENCE, o => o.MapFrom(s => s.EXPERIENCE))
                .ForMember(d => d.LeaveAppliedForMonth, o => o.MapFrom(s => s.LeaveAppliedForMonth))
                .ForMember(d => d.CLAvailed, o => o.MapFrom(s => s.CLAvailed))
                .ForMember(d => d.CLBalance, o => o.MapFrom(s => s.CLBalance))
                .ForMember(d => d.ELAvailed, o => o.MapFrom(s => s.ELAvailed))
                .ForMember(d => d.ELBalance, o => o.MapFrom(s => s.ELBalance))
                 .ForMember(d => d.Medical_Extra, o => o.MapFrom(s => s.Medical_Extra))
                .ForAllOtherMembers(d => d.Ignore())
                );
                var empDetail = Mapper.Map<List<Model.Dashboard>>(employeeDetail);
                return empDetail;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public IEnumerable<EmployeeDobDoj> GetEmployeeDOBWorkAnniversary(int? branchID, DateTime? todayDate)
        {
            log.Info($"DashBoardService/GetEmployeeDOBWorkAnniversary/{branchID}");
            try
            {
                var empDOBDOJDetail = dashBordService.GetEmployeeDOBWorkAnniversary(branchID, todayDate).ToList();
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetEmployeeDOBDOJ_Result, Model.EmployeeDobDoj>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.DOB, o => o.MapFrom(s => s.DOB))
                .ForMember(d => d.EmpProfilePhotoUNCPath, o => o.MapFrom(s => s.ImageName))
                .ForMember(d => d.AGE_NOW, o => o.MapFrom(s => s.AGE_NOW))
                .ForMember(d => d.AGE_ONE_WEEK_FROM_NOW, o => o.MapFrom(s => s.AGE_ONE_WEEK_FROM_NOW))
                .ForMember(d => d.EventType, o => o.MapFrom(s => s.EventType))
                .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.DepartmentName))
                // .ForAllOtherMembers(d => d.Ignore())
                );
                var empDetail = Mapper.Map<List<Model.EmployeeDobDoj>>(empDOBDOJDetail);
                return empDetail;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public IEnumerable<DashboardDocuments> GetDashboardDocumentList()
        {
            log.Info($"DashBoardService/GetDashboardDocumentList");
            try
            {
                var getdashBoard = genericRepo.Get<DTOModel.DashboardDocument>(x => !x.IsDeleted);
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.DashboardDocument, Model.DashboardDocuments>()
               .ForMember(d => d.DocID, o => o.MapFrom(s => s.DocID))
               .ForMember(d => d.DocTypeID, o => o.MapFrom(s => s.DocTypeID))
               .ForMember(d => d.DocumentName, o => o.MapFrom(s => s.DocumentName))
               .ForMember(d => d.DocumentPathName, o => o.MapFrom(s => s.DocumentPathName))
               .ForMember(d => d.DocumentDesc, o => o.MapFrom(s => s.DocumentDesc))
               .ForMember(d => d.DocumentTypeName, o => o.MapFrom(s => s.DashboardDocumentHdr.DocumentType))
               .ForAllOtherMembers(d => d.Ignore())
               );
                var dashBoardList = Mapper.Map<List<Model.DashboardDocuments>>(getdashBoard);
                return dashBoardList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> GetDocumentType()
        {
            log.Info($"DashBoardService/GetDocumentType");
            try
            {
                var GetDocumentType = genericRepo.Get<DTOModel.DashboardDocumentHdr>();
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.DashboardDocumentHdr, Model.SelectListModel>()
               .ForMember(d => d.id, o => o.MapFrom(s => s.DocTypeID))
               .ForMember(d => d.value, o => o.MapFrom(s => s.DocumentType))
               .ForAllOtherMembers(d => d.Ignore())
               );
                var documentTypeList = Mapper.Map<List<Model.SelectListModel>>(GetDocumentType);
                return documentTypeList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SaveDocument(DashboardDocuments document)
        {
            log.Info($"DashBoardService/SaveDocument");
            try
            {
                Mapper.Initialize(cfg => cfg.CreateMap<DashboardDocuments, DTOModel.DashboardDocument>()
               .ForMember(d => d.DocTypeID, o => o.MapFrom(s => s.DocTypeID))
               .ForMember(d => d.DocumentName, o => o.MapFrom(s => s.DocumentName))
               .ForMember(d => d.DocumentDesc, o => o.MapFrom(s => s.DocumentDesc))
               .ForMember(d => d.DocumentPathName, o => o.MapFrom(s => s.DocumentPathName))
               .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
               .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
               .ForMember(d => d.IsDeleted, o => o.UseValue(false))
               .ForAllOtherMembers(d => d.Ignore())
               );
                var documentData = Mapper.Map<DTOModel.DashboardDocument>(document);
                genericRepo.Insert(documentData);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool DeleteDocument(int documentID)
        {
            log.Info($"DashBoardService/DeleteDocument/{documentID}");
            try
            {
                var getDocument = genericRepo.GetByID<DTOModel.DashboardDocument>(documentID);
                if (getDocument != null)
                {
                    getDocument.IsDeleted = true;
                    genericRepo.Update(getDocument);
                    return true;
                }
                return false;
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public UserDetail GetUserInfo(string userName)
        {
            log.Info($"DashBoardService/GetUserInfo/{userName}");

            UserDetail userDetail = new UserDetail();
            try
            {
                var userDTO = genericRepo.Get<DTOModel.User>(x => !x.IsDeleted && x.UserName == userName)
                    .FirstOrDefault();

                if (userDTO != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.User, Model.UserDetail>()
                   .ForMember(d => d.UserID, o => o.MapFrom(source => source.UserID))
                   .ForMember(d => d.UserName, o => o.MapFrom(source => source.UserName))
                   .ForMember(d => d.Password, o => o.MapFrom(source => source.Password))
                   .ForMember(d => d.DepartmentID, o => o.MapFrom(source => source.DepartmentID))
                   .ForMember(d => d.UserTypeID, o => o.MapFrom(source => source.UserTypeID))
                   .ForMember(d => d.MobileNo, o => o.MapFrom(source => source.MobileNo))
                   .ForMember(d => d.EmployeeTypeId, o => o.MapFrom(source => source.tblMstEmployee.EmployeeTypeID))
                   .ForMember(d => d.EmailID, o => o.MapFrom(source => source.tblMstEmployee.OfficialEmail))
                   .ForMember(d => d.FullName, o => o.MapFrom(source => source.tblMstEmployee.Name))
                   .ForMember(d => d.EmployeeID, o => o.MapFrom(source => source.EmployeeID))
                   .ForMember(d => d.GenderID, o => o.MapFrom(source => source.tblMstEmployee.GenderID))
                   .ForMember(d => d.BranchID, o => o.MapFrom(source => source.tblMstEmployee.BranchID))
                   .ForMember(d => d.EmployeeCode, o => o.MapFrom(source => source.tblMstEmployee.EmployeeCode))
                   .ForMember(d => d.DesignationID, o => o.MapFrom(source => (int?)source.tblMstEmployee.DesignationID))
                   .ForMember(d => d.EmpProfilePhotoUNCPath, o => o.MapFrom(source => source.ImageName))
                   .ForAllOtherMembers(d => d.Ignore());
                    });
                    var dd = Mapper.Map<DTOModel.User, Model.UserDetail>(userDTO, userDetail);
                    if (userDetail.UserTypeID != (int)Common.UserType.Admin && userDetail.UserTypeID != (int)Common.UserType.SuperUser)
                    {
                        var getmaintenance = genericRepo.Get<DTOModel.EmailConfiguration>()
                            .Select(x => new
                            {
                                IsMaintenance = x.IsMaintenance,
                                MaintenanceDate = x.MaintenanceDateTime
                            }).FirstOrDefault();

                        if (getmaintenance != null)
                        {
                            userDetail.IsMaintenance = getmaintenance.IsMaintenance;
                            userDetail.MaintenanceDateTime = getmaintenance.MaintenanceDate;
                        }

                    }
                    userDetail.DepartmentName = genericRepo.GetByID<DTOModel.Department>(userDTO.DepartmentID).DepartmentName;
                    userDetail.Location = genericRepo.GetByID<DTOModel.Branch>(userDetail.BranchID).BranchName;
                    userDetail.AppraisalFormID = (genericRepo.Get<DTOModel.DesignationAppraisalForm>(x => x.DesignationID == (int)userDTO.tblMstEmployee.DesignationID
                    && !x.IsDeleted).FirstOrDefault())?.AppraisalFormID ?? null;

                    #region  Get User Profile Picture

                    userDetail.EmpProfilePhotoUNCPath = System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                  Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + userDetail.EmpProfilePhotoUNCPath)) ?
                  Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + userDetail.EmpProfilePhotoUNCPath) :
                  Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");

                    #endregion

                }
                return userDetail;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Dashboard GetDashBoardForRetiredEmployee(int employeeID)
        {
            log.Info($"DashBoardService/GetDashBoardForRetiredEmployee/employeeID:{employeeID}");
            try
            {
                var dtoEmployeeDtl = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.Dashboard>()
                    .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
                    .ForMember(d => d.ReportingTo, o => o.MapFrom(s => s.ReportingTo))
                     .ForMember(d => d.DOLeaveOrg, o => o.MapFrom(s => s.DOLeaveOrg))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var dashboard = Mapper.Map<Model.Dashboard>(dtoEmployeeDtl);

                if (dashboard.ReportingTo.HasValue)
                    dashboard.RM = genericRepo.GetByID<DTOModel.tblMstEmployee>(dashboard.ReportingTo.Value).Name;
                return dashboard;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
