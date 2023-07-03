using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using Nafed.MicroPay.Common;

namespace Nafed.MicroPay.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IEmployeeRepository empRepo;
        public UserService(IGenericRepository genericRepo, IEmployeeRepository empRepo)
        {
            this.genericRepo = genericRepo;
            this.empRepo = empRepo;
        }
        public List<Model.User> GetUserList()
        {
            log.Info($"UserService/GetUserList");
            try
            {
                var result = genericRepo.Get<DTOModel.User>(x => x.IsDeleted == false && x.UserName.ToLower() != "admin");
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.User, Model.User>()
                    .ForMember(c => c.UserID, c => c.MapFrom(s => s.UserID))
                    .ForMember(c => c.FullName, c => c.MapFrom(s => s.FullName))
                    .ForMember(c => c.UserName, c => c.MapFrom(s => s.UserName))
                    .ForMember(c => c.Password, c => c.MapFrom(s => s.Password))
                    .ForMember(c => c.DepartmentID, c => c.MapFrom(s => s.DepartmentID))
                    .ForMember(c => c.UserTypeID, c => c.MapFrom(s => s.UserTypeID))
                    .ForMember(c => c.MobileNo, c => c.MapFrom(s => s.MobileNo))
                    .ForMember(c => c.EmailID, c => c.MapFrom(s => s.EmailID))
                    .ForMember(c => c.ImageName, c => c.MapFrom(s => s.ImageName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.IsActive, c => c.MapFrom(s => s.IsActive))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForMember(c => c.LockDate, c => c.MapFrom(s => s.LockDate))
                    .ForMember(c => c.WrongAttemp, c => c.MapFrom(s => s.WrongAttemp))

                    .ForMember(c => c.DepartmentName, c => c.MapFrom(s => s.Department.DepartmentName))
                     .ForMember(c => c.UserTypeName, c => c.MapFrom(s => s.UserType.UserTypeName))
                       .ForMember(c => c.DesignationID, c => c.MapFrom(s => s.tblMstEmployee.DesignationID))
                    .ForMember(c => c.DOB, c => c.MapFrom(s => s.tblMstEmployee.DOB))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listUser = Mapper.Map<List<Model.User>>(result);
                return listUser.OrderBy(x => x.UserName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UserNameExists(int? UserID, string UserName)
        {
            log.Info($"UserService/UserNameExists/{UserID}/{UserName}");
            return genericRepo.Exists<DTOModel.User>(x => x.UserID != UserID && x.UserName == UserName && x.IsDeleted == false);
        }

        public bool PasswordExists(Model.ChangePassword changePassword)
        {
            log.Info($"UserService/PasswordExists/");
            bool isMatched = false;
            var dbPassword = genericRepo.Get<DTOModel.User>(x => x.UserID == changePassword.UserID && x.IsDeleted == false).FirstOrDefault().Password;
            if (dbPassword != null)
            {
                isMatched = Password.VerifyPassword(changePassword.OldPassword, dbPassword, Password.PASSWORD_SALT);
            }
            return isMatched;

        }

        public bool ChangePassword(Model.ChangePassword updatePassword)
        {
            log.Info($"UserService/ChangePassword");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.User>(updatePassword.UserID);
                dtoObj.Password = updatePassword.Password;
                genericRepo.Update<DTOModel.User>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool UserEmailIDExists(string EmailID)
        {
            log.Info($"UserService/UserEmailIDExists/{EmailID}");
            try
            {
                if (genericRepo.Exists<DTOModel.tblMstEmployee>(x => x.OfficialEmail == EmailID && x.IsDeleted == false))
                {
                    return true;
                }
                else
                {

                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                return false;
            }

        }

        public bool UserMobileNoExists(string Mobile)
        {
            log.Info($"UserService/UserMobileNoExists/{Mobile}");
            try
            {
                if (genericRepo.Exists<DTOModel.tblMstEmployee>(x => x.MobileNo == Mobile && x.IsDeleted == false))
                {
                    return true;
                }
                else
                {

                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);

                return false;
            }

        }
        public Model.UserDetail UserDetailForOTP(string fieldName, int otpType)
        {
            log.Info($"UserService/UserDetailForOTP/{fieldName}/{otpType}");
            Model.UserDetail userDetail = new Model.UserDetail();

            try
            {
                var userData = genericRepo.Get<DTOModel.tblMstEmployee>(x => (otpType == 1 ? x.OfficialEmail == fieldName : x.MobileNo == fieldName) && !x.IsDeleted).Select(x => new
                {
                    MobileNo = x.MobileNo,
                    EmailID = x.OfficialEmail,
                    UserName = x.EmployeeCode
                }).FirstOrDefault();
                if (userData != null)
                {
                    userDetail.EmailID = userData.EmailID;
                    userDetail.MobileNo = userData.MobileNo;
                    userDetail.UserName = userData.UserName;
                }
                return userDetail;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return userDetail;
            }

        }
        public bool UpdateUser(Model.User editUserItem)
        {
            log.Info($"UserService/UpdateUser");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.User>(editUserItem.UserID);
                //dtoObj.UserID = editUserItem.UserID;
                //dtoObj.UserName = editUserItem.UserName;
                dtoObj.FullName = editUserItem.FullName;
                //  dtoObj.Password = editUserItem.Password;
                //dtoObj.DepartmentID = editUserItem.DepartmentID;
                dtoObj.UserTypeID = editUserItem.UserTypeID;
                dtoObj.MobileNo = editUserItem.MobileNo;
                dtoObj.EmailID = editUserItem.EmailID;
                dtoObj.ImageName = editUserItem.ImageName;
                //dtoObj.CreatedBy = editUserItem.CreatedBy;
                //dtoObj.CreatedOn = editUserItem.CreatedOn;
                dtoObj.UpdatedBy = editUserItem.UpdatedBy;
                dtoObj.UpdatedOn = editUserItem.UpdatedOn;
                dtoObj.IsActive = editUserItem.IsActive;
                //dtoObj.IsDeleted = editUserItem.IsDeleted;
                genericRepo.Update<DTOModel.User>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }
        public int GenerateOTPNo(string EmailID)
        {
            log.Info($"UserService/GenerateOTPNo/{EmailID}");
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        public bool ResetPassword(Model.UserDetail editUserItem)
        {
            log.Info($"UserService/ResetPassword");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.Get<DTOModel.User>(x => x.UserName == editUserItem.UserName).FirstOrDefault();
                var kk = ExtensionMethods.GeneratePassword();
                dtoObj.Password = kk;
                string originalpassword = dtoObj.Password;
                dtoObj.Password = Password.CreatePasswordHash(dtoObj.Password, Password.CreateSalt(Password.PASSWORD_SALT));
                genericRepo.Update<DTOModel.User>(dtoObj);
                var mailbody = genericRepo.Get<DTOModel.Mail_Process_Content>(x => x.MailProcessID == 2).FirstOrDefault();

                string cont = mailbody.Content;
                cont = cont.Replace("$newpassword", originalpassword);
                if (editUserItem.otpType == 1)
                {
                    cont = cont.Replace("\\n\\n", "</br>");
                    Task t1 = Task.Run(() => GetEmailConfiguration(editUserItem.EmailID, cont, "NAFED-HRMS : Password Recovery"));
                }
                else if (editUserItem.otpType == 2)
                {
                    cont = cont.Replace("\\n\\n", "");
                    Model.SMSConfiguration smssetting = new Model.SMSConfiguration();
                    var smssettings = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.SMSConfiguration, Model.SMSConfiguration>();

                    });
                    smssetting = Mapper.Map<Model.SMSConfiguration>(smssettings);

                    Task t1 = Task.Run(() => SendMessageOnMobile(editUserItem.MobileNo, cont, smssetting));
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

        public bool SendSMS(string MobileNo, string message)
        {
            log.Info($"EmployeeLeaveService/SendMail");
            try
            {
                Model.SMSConfiguration smssetting = new Model.SMSConfiguration();
                var smssettings = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SMSConfiguration, Model.SMSConfiguration>();

                });
                smssetting = Mapper.Map<Model.SMSConfiguration>(smssettings);
                return SendMessageOnMobile(MobileNo, message, smssetting);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }
        }


        public bool SendOTP(Model.UserDetail editUserItem, string OTP, int MessageType, int? processType = null)
        {
            log.Info($"UserService/SendOTP/{OTP}/{MessageType}");
            bool flag = false;
            try
            {
                // var dtoObj = genericRepo.GetByID<DTOModel.User>(editUserItem.UserID);
                if (MessageType == 1)
                {
                    var mailbody = genericRepo.Get<DTOModel.Mail_Process_Content>(x => x.MailProcessID == 1).FirstOrDefault();

                    if (processType.HasValue && processType.Value == 1)
                        mailbody.Content = mailbody.Content.Replace("{#var#}", "Password forget");
                    else if (processType.HasValue && processType.Value == 2)
                        mailbody.Content = mailbody.Content.Replace("{#var#}", "Registration");

                    string cont = mailbody.Content;
                    cont = cont.Replace("$OTP", OTP);

                    GetEmailConfiguration(editUserItem.EmailID, cont, "NAFED-HRMS : One Time Password");
                    flag = true;
                }
                else
                {
                    Model.SMSConfiguration smssetting = new Model.SMSConfiguration();
                    var smssettings = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.SMSConfiguration, Model.SMSConfiguration>();

                    });
                    smssetting = Mapper.Map<Model.SMSConfiguration>(smssettings);

                    var mailbody = genericRepo.Get<DTOModel.Mail_Process_Content>(x => x.MailProcessID == 1).FirstOrDefault();

                    if (processType.HasValue && processType.Value == 1)
                        mailbody.Content = mailbody.Content.Replace("{#var#}", "Password forget");
                    mailbody.Content.Replace("$OTP", OTP);
                    string cont = mailbody.Content;
                    cont = cont.Replace("$OTP", OTP);
                    // SendMessageOnMobile(editUserItem.MobileNo, cont);
                    flag = SendMessageOnMobile(editUserItem.MobileNo, cont, smssetting);
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public int InsertUser(Model.User createUser, string ext)
        {
            log.Info($"UserService/InsertUser/{ext}");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.User, DTOModel.User>()
                     .ForMember(c => c.UserID, c => c.MapFrom(s => s.UserID))
                     .ForMember(c => c.FullName, c => c.MapFrom(s => s.FullName))
                    .ForMember(c => c.UserName, c => c.MapFrom(s => s.UserName))
                    .ForMember(c => c.Password, c => c.MapFrom(s => s.Password))
                     .ForMember(c => c.DepartmentID, c => c.MapFrom(s => s.DepartmentID))
                    .ForMember(c => c.UserTypeID, c => c.MapFrom(s => s.UserTypeID))
                    .ForMember(c => c.MobileNo, c => c.MapFrom(s => s.MobileNo))
                     .ForMember(c => c.EmailID, c => c.MapFrom(s => s.EmailID))
                    .ForMember(c => c.ImageName, c => c.MapFrom(s => s.ImageName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                     .ForMember(c => c.IsActive, c => c.MapFrom(s => s.IsActive))
                      .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                      .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeID))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoUser = Mapper.Map<DTOModel.User>(createUser);
                genericRepo.Insert<DTOModel.User>(dtoUser);
                dtoUser.ImageName = dtoUser.UserID + ext;
                genericRepo.Update<DTOModel.User>(dtoUser);
                return dtoUser.UserID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            // return 0;
        }

        public Model.User GetUserByID(int userID)
        {
            log.Info($"UserService/GetUserByID {userID}");
            try
            {
                var UserObj = genericRepo.GetByID<DTOModel.User>(userID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.User, Model.User>()
                       .ForMember(c => c.UserID, c => c.MapFrom(s => s.UserID))
                       .ForMember(c => c.FullName, c => c.MapFrom(s => s.FullName))
                       .ForMember(c => c.UserName, c => c.MapFrom(s => s.UserName))
                       .ForMember(c => c.Password, c => c.MapFrom(s => s.Password))
                       .ForMember(c => c.DepartmentID, c => c.MapFrom(s => s.DepartmentID))
                       .ForMember(c => c.UserTypeID, c => c.MapFrom(s => s.UserTypeID))
                       .ForMember(c => c.MobileNo, c => c.MapFrom(s => s.MobileNo))
                       .ForMember(c => c.EmailID, c => c.MapFrom(s => s.EmailID))
                       .ForMember(c => c.ImageName, c => c.MapFrom(s => s.ImageName))
                       .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                       .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                       .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                       .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                       .ForMember(c => c.IsActive, c => c.MapFrom(s => s.IsActive))
                       .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                       .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeID))
                       .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.User, Model.User>(UserObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(int userID)
        {
            log.Info($"UserService/Delete/{userID}");
            bool flag = false;
            try
            {
                DTOModel.User dtoUser = new DTOModel.User();
                dtoUser = genericRepo.GetByID<DTOModel.User>(userID);
                dtoUser.IsDeleted = true;
                genericRepo.Update<DTOModel.User>(dtoUser);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public void GetEmailConfiguration(string ToEmailID, string mailbody, string subject)
        {
            log.Info($"UserService/GetEmailConfiguration/{ToEmailID}/{mailbody}/{subject}");
            try
            {
                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                EmailMessage message = new EmailMessage();
                message.To = ToEmailID;
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

        public bool SendMessageOnMobile(string mobileNo, string message, Model.SMSConfiguration smssetting)
        {
            try
            {
                string msgRecepient = mobileNo.Length == 10 ? "91" + mobileNo : mobileNo;
                SMSParameter sms = new SMSParameter();

                sms.MobileNo = msgRecepient;
                sms.Message = message;
                sms.URL = smssetting.SMSUrl;
                sms.User = smssetting.UserName;
                sms.Password = smssetting.Password;
                return SMSHelper.SendSMS(sms);
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return false;
            }

        }
        public bool GeneratePasswordforAllEmployees(List<Model.User> users)
        {
            bool flag = false;
            log.Info($"UserService/GeneratePasswordforAllEmployees");
            try
            {
                var selectedUsers = users.Select(x => new Model.SelectListModel { id = x.UserID, value = x.Password }).ToList();
                var selectedUsersDT = ExtensionMethods.ToDataTable(selectedUsers);

                int res = empRepo.GeneratePasswordforAllEmployees(selectedUsersDT);

                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public int? GetUserTypeByEmployeeID(int? employeeID)
        {
            log.Info($"UserService/GetUserTypeByEmployeeID/{employeeID}");
            try
            {
                var res = genericRepo.Get<DTOModel.User>(x => x.EmployeeID == employeeID).FirstOrDefault();
                if (res != null)
                    return res.UserTypeID;
                else
                    return (int?)5;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public List<Model.SelectListModel> GetUnMappedEmployees(int? branchID)
        {

            log.Info($"UserService/GetUnMappedEmployees/{branchID}");
            List<Model.SelectListModel> employeeList = new List<Model.SelectListModel>();
            try
            {
                var res = empRepo.GetUnMappedEmployees(branchID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.GetUnMappedEmployeeList_Result, Model.SelectListModel>()
                        .ForMember(d => d.id, o => o.MapFrom(s => s.EmployeeId))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.Name))
                        .ForAllOtherMembers(c => c.Ignore());
                });
                return Mapper.Map<List<DTOModel.GetUnMappedEmployeeList_Result>, List<Model.SelectListModel>>(res); ;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool LockUnlockUser(int userId, bool ulock)
        {
            try
            {
                var dData = genericRepo.GetByID<DTOModel.User>(userId);
                if (dData != null)
                {
                    if (ulock)
                    {
                        dData.WrongAttemp = 0;
                        dData.LockDate = DateTime.Now;
                    }
                    else
                    {
                        dData.WrongAttemp = null;
                        dData.LockDate = null;
                    }
                    genericRepo.Update(dData);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
