using System;
using System.Linq;
using AutoMapper;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.API.IAPIServices;



namespace Nafed.MicroPay.Services.API
{
    public class LoginService : BaseService, ILoginService
    {
        private ILoginRepository loginRepo;
        private IGenericRepository genericRepo;

        public LoginService()
        {
            loginRepo = new LoginRepository();
            genericRepo = new GenericRepository();
        }

        public bool ValidateUser(ValidateLogin userCredential, out string authenticationMessage, out UserDetail userDetail)
        {
            log.Info("LoginService/ValidateUser");
            userDetail = new UserDetail();
            authenticationMessage = string.Empty;
            string sUserName = userCredential.userName;
            string sPassword = userCredential.password;
            string dbPassword = string.Empty;

            bool isAuthenticated = false;
            bool isExists = false;

            try
            {
                isExists = loginRepo.VerifyUser(sUserName, out dbPassword);

                if (isExists)
                {
                    bool isMatch = Password.VerifyPassword(sPassword, dbPassword, Password.PASSWORD_SALT);
                    if (isMatch)
                    {
                        var userDTO = loginRepo.GetUserData(sUserName);

                        if (userDTO != null)
                        {
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<Data.Models.User, Model.UserDetail>()
                           .ForMember(d => d.FullName, o => o.MapFrom(source => source.FullName))
                           .ForMember(d => d.UserName, o => o.MapFrom(source => source.UserName))
                           .ForMember(d => d.MobileNo, o => o.MapFrom(source => source.MobileNo))
                            .ForMember(d => d.UserID, o => o.MapFrom(source => source.UserID))
                            .ForMember(d=>d.EmployeeID,o=>o.MapFrom(s=>s.EmployeeID))
                           .ForAllOtherMembers(d => d.Ignore());
                            });

                            var userDetailModel = Mapper.Map<Data.Models.User, Model.UserDetail>(userDTO, userDetail);

                            var emp = genericRepo.Get<Data.Models.tblMstEmployee>(x => x.EmployeeId == userDetailModel.EmployeeID);
                            userDetailModel.EmployeeID = emp.FirstOrDefault().EmployeeId;
                            userDetailModel.Designation = emp.FirstOrDefault().Designation.DesignationName;
                            userDetailModel.Location = emp.FirstOrDefault().Branch.BranchName;
                            userDetailModel.BranchID = emp.FirstOrDefault().BranchID;
                          
                            //   int? R_OfficerID = emp.FirstOrDefault().ReportingTo.HasValue ? emp.FirstOrDefault().ReportingTo : null;

                            //if (R_OfficerID.HasValue)
                            //{
                            //    var Report_Officer = genericRepo.GetByID<Data.Models.tblMstEmployee>(R_OfficerID);
                            //    if (Report_Officer != null)
                            //        userDetailModel.ReportingOfficer = Report_Officer.Name;
                            //}
                            //else
                            //    userDetailModel.ReportingOfficer = "";

                            userDetailModel.PresentAddress = (string.IsNullOrEmpty(Convert.ToString(emp.FirstOrDefault().PAdd)) ? "" : emp.FirstOrDefault().PAdd.ToString()) + ' ' + (string.IsNullOrEmpty(Convert.ToString(emp.FirstOrDefault().PStreet)) ? "" : emp.FirstOrDefault().PStreet.ToString()) + ' ' + (string.IsNullOrEmpty(Convert.ToString(emp.FirstOrDefault().PCity)) ? "" : emp.FirstOrDefault().PCity.ToString()) + ' ' + (string.IsNullOrEmpty(Convert.ToString(emp.FirstOrDefault().PPin)) ? "" : emp.FirstOrDefault().PPin.ToString());
                            userDetailModel.DepartmentName = !emp.FirstOrDefault().DepartmentID.HasValue ? "" :
emp.FirstOrDefault().Department.DepartmentName;

                            authenticationMessage = "User Validated";
                            isAuthenticated = true;
                        }
                        else
                        {
                            isAuthenticated = false;
                            authenticationMessage = "Invalid Credentials";
                        }
                    }
                    else
                    {
                        isAuthenticated = false;
                        authenticationMessage = "Invalid Password";
                    }
                }
                else
                {
                    isAuthenticated = false;
                    authenticationMessage = "Invalid Credentials";
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return isAuthenticated;
        }
        public UserAccessRight GetUserAccessRight(int userID, int menuID)
        {
            log.Info($"LoginService/ValidateUserAccess/{userID}/{menuID}");
            UserAccessRight userAccess = new UserAccessRight();
            return userAccess;
        }
    }
}
