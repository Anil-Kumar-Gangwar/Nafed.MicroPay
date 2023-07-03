using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;
using dtoModel = Nafed.MicroPay.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace Nafed.MicroPay.Services
{
    public class LoginService : BaseService, ILoginService
    {
        private ILoginRepository loginRepository;
        private IGenericRepository genricRepository;
        public LoginService(ILoginRepository loginRepository, IGenericRepository genricRepository)
        {
            this.loginRepository = loginRepository;
            this.genricRepository = genricRepository;
        }

        public bool ValidateUser(ValidateLogin userCredential, out string sAuthenticationMessage, out UserDetail userDetail)
        {
            log.Info("LoginService/ValidateUser");
            userDetail = new UserDetail();
            sAuthenticationMessage = string.Empty;
            string sUserName = userCredential.userName;
            string sPassword = userCredential.password;
            string dbPassword = string.Empty;
            bool isAuthenticated = false;
            bool isExists = false;

            try
            {
                isExists = loginRepository.VerifyUser(sUserName, out dbPassword);

                if (isExists)
                {
                    bool isMatch = Password.VerifyPassword(sPassword, dbPassword, Password.PASSWORD_SALT);
                    if (isMatch)
                    {
                        var userDTO = loginRepository.GetUserData(sUserName);
                        if (userDTO != null)
                        {
                            var LockDate = userDTO.LockDate ?? DateTime.Now;
                            if (userDTO.WrongAttemp == 0 && LockDate.AddHours(24) > DateTime.Now && (userDTO.UserTypeID != (int)Common.UserType.Admin && userDTO.UserTypeID != (int)Common.UserType.SuperUser))
                            {
                                userDetail.ErrorMessage = userDTO.LockDate.Value.AddDays(1).ToString("dd-MM-yyyy hh:mm tt");
                                return false;
                            }

                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<Data.Models.User, Model.UserDetail>()
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
                            .ForMember(d => d.DoLeaveOrg, o => o.MapFrom(s => s.tblMstEmployee.DOLeaveOrg))
                           .ForAllOtherMembers(d => d.Ignore());
                            });
                            var dd = Mapper.Map<Data.Models.User, Model.UserDetail>(userDTO, userDetail);
                            if (userDetail.UserTypeID != (int)Common.UserType.Admin && userDetail.UserTypeID != (int)Common.UserType.SuperUser)
                            {
                                var getmaintenance = genricRepository.Get<dtoModel.EmailConfiguration>().Select(x => new
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
                            userDetail.DepartmentName = genricRepository.GetByID<dtoModel.Department>(userDTO.DepartmentID).DepartmentName;
                            userDetail.Location = genricRepository.GetByID<dtoModel.Branch>(userDetail.BranchID).BranchName;
                            userDetail.AppraisalFormID = (genricRepository.Get<dtoModel.DesignationAppraisalForm>(x => x.DesignationID == (int)userDTO.tblMstEmployee.DesignationID
                            && !x.IsDeleted).FirstOrDefault())?.AppraisalFormID ?? null;
                            sAuthenticationMessage = "User Validated";
                            isAuthenticated = true;

                            #region  Get User Profile Picture

                            //      userDetail.EmpProfilePhotoUNCPath = System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                            //Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + userDetail.EmpProfilePhotoUNCPath)) ?
                            //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + userDetail.EmpProfilePhotoUNCPath) :
                            //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");

                            userDetail.EmpProfilePhotoUNCPath =
                                System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                      Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + userDetail.EmpProfilePhotoUNCPath)) ?
                       Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + userDetail.EmpProfilePhotoUNCPath : Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + "DefaultUser.png";
                            #endregion

                            userDTO.LockDate = null;
                            userDTO.WrongAttemp = null;
                            genricRepository.Update(userDTO);
                        }
                        else
                        {
                            isAuthenticated = false;
                            sAuthenticationMessage = "Invalid Credentials";
                        }
                    }
                    else
                    {
                        var uInnerData = genricRepository.GetIQueryable<dtoModel.User>(u => u.IsDeleted == false && u.IsActive == true && u.UserName == sUserName).FirstOrDefault();
                        if (uInnerData != null)
                        {
                            if (uInnerData.WrongAttemp == 0 && uInnerData.LockDate.HasValue)
                            {
                                //userDetail.ErrorMessage = uInnerData.LockDate.Value.AddDays(1).ToString("dd-MM-yyyy hh:mm tt");
                                userDetail.WrongAttemp = uInnerData.WrongAttemp;

                                return false;
                            }
                            switch (uInnerData.WrongAttemp)
                            {
                                case 0:
                                    userDetail.WrongAttemp = 0;
                                    break;
                                case 2:
                                    userDetail.WrongAttemp = 1;
                                    break;
                                case 1:
                                    userDetail.WrongAttemp = 0;
                                    break;
                                default:
                                    userDetail.WrongAttemp = 2;
                                    break;
                            }
                            if (userDetail.WrongAttemp.HasValue)
                            {
                                uInnerData.WrongAttemp = userDetail.WrongAttemp;
                                //if ((uInnerData.UserTypeID != (int)Common.UserType.Admin && uInnerData.UserTypeID != (int)Common.UserType.SuperUser))
                                uInnerData.LockDate = userDetail.WrongAttemp == 0 ? DateTime.Now : uInnerData.LockDate;

                                genricRepository.Update(uInnerData);
                            }
                        }
                        else { userDetail.ErrorMessage = "notexists"; sAuthenticationMessage = "Invalid Credentials"; }
                        isAuthenticated = false;
                        //isAuthenticated = false;
                        //sAuthenticationMessage = "Invalid Password";
                    }
                }
                else
                {
                    userDetail.ErrorMessage = "notexists";
                    sAuthenticationMessage = "Invalid Credentials";
                    isAuthenticated = false;

                }
            }
            catch (System.Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return isAuthenticated;
        }


        public UserAccessRight GetUserAccessRight(int userID, int menuID, int userTypeID)
        {
            log.Info($"LoginService/ValidateUserAccess/{userID}/{menuID}");
            UserAccessRight userAccess = new UserAccessRight();

            try
            {
                if (userTypeID != (int)Common.UserType.Admin && userTypeID != (int)Common.UserType.SuperUser)
                {
                    var userDTO = genricRepository.Get<dtoModel.UserRight>(x => x.MenuID == menuID && x.UserID == userID);
                    if (userDTO.Count() > 0)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Data.Models.UserRight, Model.UserAccessRight>()
                            .ForMember(d => d.DepartmentID, o => o.MapFrom(source => source.DepartmentID))
                            .ForMember(d => d.UserID, o => o.MapFrom(source => source.UserID))
                            .ForMember(d => d.UserID, o => o.MapFrom(source => source.UserID))
                            .ForMember(d => d.MenuID, o => o.MapFrom(source => source.MenuID))
                            .ForMember(d => d.ShowMenu, o => o.MapFrom(source => source.ShowMenu))
                            .ForMember(d => d.Create, o => o.MapFrom(source => source.CreateRight))
                            .ForMember(d => d.Edit, o => o.MapFrom(source => source.EditRight))
                            .ForMember(d => d.View, o => o.MapFrom(source => source.ViewRight))
                            .ForMember(d => d.Delete, o => o.MapFrom(source => source.DeleteRight))
                            .ForMember(d => d.HelpText, o => o.MapFrom(source => source.Menu.Help))
                            .ForAllOtherMembers(d => d.Ignore());
                        });
                        userAccess = Mapper.Map<Data.Models.UserRight, Model.UserAccessRight>(userDTO.Single());
                    }
                }
                else
                {
                    var selectedMenuItemHelpText = genricRepository.GetByID<dtoModel.Menu>(menuID).Help;
                    return new UserAccessRight
                    {
                        Create = true,
                        Delete = true,
                        Edit = true,
                        View = true,
                        MenuID = menuID,
                        UserID = userID,
                        ShowMenu = true,
                        HelpText = selectedMenuItemHelpText
                    };

                }
                //  return new UserAccessRight { Create = true, Delete = true, Edit = true, View = true, MenuID = menuID, UserID = userID, ShowMenu = true };

            }
            catch (System.Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return userAccess;
        }

        public bool InsertLoginDetials(UserLoginDetails userLoginDetails)
        {
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.UserLoginDetails, dtoModel.UserLoginDetails>()
                    .ForMember(c => c.UserName, c => c.MapFrom(m => m.UserName))
                    .ForMember(c => c.SessionId, c => c.MapFrom(m => m.SessionId))
                    .ForMember(c => c.LoginTime, c => c.MapFrom(m => m.LoginTime))
                    .ForMember(c => c.LogOutTime, c => c.MapFrom(m => m.LogOutTime))
                   .ForMember(c => c.Remarks, c => c.MapFrom(m => m.Remarks))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoUserDetails = Mapper.Map<dtoModel.UserLoginDetails>(userLoginDetails);
                genricRepository.Insert<dtoModel.UserLoginDetails>(dtoUserDetails);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool InsertLogOutDetials(UserLoginDetails userLoginDetails)
        {
            bool flag = false;
            try
            {
                var result = genricRepository.Get<dtoModel.UserLoginDetails>(x => x.SessionId == userLoginDetails.SessionId && x.UserName == userLoginDetails.UserName).FirstOrDefault();
                if (result != null)
                {
                    result.LogOutTime = DateTime.Now;
                    genricRepository.Update<dtoModel.UserLoginDetails>(result);
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

       
    }
}
