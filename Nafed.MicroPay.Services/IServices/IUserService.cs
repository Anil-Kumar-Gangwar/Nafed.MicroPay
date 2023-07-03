using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IUserService
    {
        List<Model.User> GetUserList();
        bool UserNameExists(int? userID, string UserName);
        bool UserEmailIDExists(string EmailID);
        bool UserMobileNoExists(string Mobile);
        Model.UserDetail UserDetailForOTP(string fieldName, int otpType);
        int GenerateOTPNo(string EmailID);
        bool ResetPassword(Model.UserDetail editUser);
        bool SendOTP(Model.UserDetail editUser, string OTP, int MessageType,int ? processType=null);

        bool UpdateUser(Model.User editUser);
        int InsertUser(Model.User createUser, string ext);
        Model.User GetUserByID(int userID);
        bool PasswordExists(Model.ChangePassword changePassword);
        bool ChangePassword(Model.ChangePassword updatePassword);
        bool Delete(int userID);
        void GetEmailConfiguration(string ToEmailID, string mailbody, string subject);
        bool SendMessageOnMobile(string mobileno, string message, Model.SMSConfiguration smssetting);

        bool GeneratePasswordforAllEmployees(List<Model.User> users);

        int? GetUserTypeByEmployeeID(int? employeeID);
        bool SendSMS(string MobileNo, string message);
        List<Model.SelectListModel> GetUnMappedEmployees(int? branchID);
        bool LockUnlockUser(int userId, bool ulock);
    }
}
