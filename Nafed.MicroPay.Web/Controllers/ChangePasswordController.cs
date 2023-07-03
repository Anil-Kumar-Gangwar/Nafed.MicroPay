using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services;

namespace MicroPay.Web.Controllers
{
    public class ChangePasswordController : BaseController
    {
        // GET: ChangePassword
        private readonly IUserService userservice;
        public ChangePasswordController(IUserService userservice)
        {
            this.userservice = userservice;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            ChangePassword changePassword = new ChangePassword();
            try
            {
                changePassword.UserID = userDetail.UserID;
                return PartialView("_ChangePassword", changePassword);
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return PartialView("_ChangePassword", changePassword);
            }
        }


        [HttpPost]
        public ActionResult ChangePassword(ChangePassword changePassword)
        {
            try
            {
                changePassword.UserID = userDetail.UserID;
                ModelState.Remove("UserID");

                if (string.IsNullOrEmpty(changePassword.hdOldPassword))
                    ModelState.AddModelError("OldPassword", "Please enter old password.");
                if (string.IsNullOrEmpty(changePassword.hdPassword))
                    ModelState.AddModelError("Password", "Please enter password.");
                if (string.IsNullOrEmpty(changePassword.hdCPassword))
                    ModelState.AddModelError("CPassword", "Please enter confirm password.");

                if (!string.IsNullOrEmpty(changePassword.hdOldPassword) && !string.IsNullOrEmpty(changePassword.hdPassword) && !string.IsNullOrEmpty(changePassword.hdCPassword))
                {
                    changePassword.OldPassword = Password.DecryptStringAES(changePassword.hdOldPassword);
                    changePassword.Password = Password.DecryptStringAES(changePassword.hdPassword);
                    changePassword.CPassword = Password.DecryptStringAES(changePassword.hdCPassword);
                }

                if (ModelState.IsValid)
                {
                    // changePassword.OldPassword = Password.CreatePasswordHash(changePassword.OldPassword.Trim(), Password.CreateSalt(Password.PASSWORD_SALT));
                    if (!Password.CheckPasswordAgainstPolicy(userDetail.UserName, changePassword.Password))
                    {
                        ViewBag.error = "Password should contain atleast one upper case letter, one lower case letter, one digit, one special character, 8-50 characters long and should not contain username";
                        return PartialView("_ChangePassword", changePassword);
                    }
                    if (userservice.PasswordExists(changePassword))
                    {
                        changePassword.Password = Password.CreatePasswordHash(changePassword.Password.Trim(), Password.CreateSalt(Password.PASSWORD_SALT));
                        if (userservice.ChangePassword(changePassword))
                        {
                            var message = "";
                            if (!String.IsNullOrEmpty(userDetail.EmailID))
                            {
                                message = "<div><p>Your password has been changed.<br/> Warning: Notify your administrator. If you did not request that your password be reset</p></div>.<div> <p>Regards, <br/> ENAFED </p> </div>";
                                userservice.GetEmailConfiguration(userDetail.EmailID, message, "Change Password Confirmation Mail");
                            }
                            else if (!String.IsNullOrEmpty(userDetail.MobileNo))
                            {
                                message = "Your password has been changed. Warning: Notify your administrator. If you did not request that your password be reset.";
                                userservice.SendSMS(userDetail.MobileNo, message);
                            }
                            TempData["Message"] = "Your Password has been changed.";
                            return JavaScript("window.location = '" + Url.Action("Index", "Login") + "'");

                        }
                        else
                        {
                            return PartialView("_ChangePassword", changePassword);
                        }
                    }
                    else
                    {
                        ViewBag.error = "Old Password not matched, Please provide correct password.";
                        return PartialView("_ChangePassword", changePassword);
                    }
                }
                else { return PartialView("_ChangePassword", changePassword); }
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return PartialView("_ChangePassword", changePassword);
            }

        }
    }
}