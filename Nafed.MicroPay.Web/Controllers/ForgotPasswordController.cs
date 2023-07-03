using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using MicroPay.Web.Attributes;
using CaptchaMvc.HtmlHelpers;

namespace MicroPay.Web.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly IUserService userservice;

        public ForgotPasswordController(IUserService userservice)
        {
            this.userservice = userservice;
        }
        // GET: ForgotPassword
        //[SessionTimeout]
        public ActionResult Index()
        {
            UserDetail fdetail = new UserDetail();
            fdetail.step = 1;
            return View(fdetail);
        }
        //[HttpPost]
        //public ActionResult Index(UserDetail fdetail)
        //{

        //    return View();


        //}
     
        [HttpPost]
       // [SessionTimeout]
        public ActionResult SendOTP(UserDetail fdetail, string inputOTP, int otpType,FormCollection frm)
        {
            try
            {
                var otpCode = frm.Get("OTPCode1");
                var oTPtimeStamp = frm.Get("OTPtimeStamp1");

                fdetail.OTPCode = Convert.ToInt32(otpCode);
                fdetail.OTPtimeStamp = Convert.ToDateTime(oTPtimeStamp);

                if (!this.IsCaptchaValid(""))
                {
                    ViewBag.error = "Captcha is not valid";
                    return PartialView("_OTP", fdetail);
                }

                if (fdetail.step == 1)
                {
                    if (otpType == 1)
                    {
                        if (userservice.UserEmailIDExists(fdetail.EmailID))
                        {
                            var userData = userservice.UserDetailForOTP(fdetail.EmailID, otpType);
                            fdetail.EmailID = userData.EmailID;
                            fdetail.MobileNo = userData.MobileNo;
                            fdetail.UserName = userData.UserName;
                            int otp = userservice.GenerateOTPNo(fdetail.EmailID);
                          //  Session["OTP"] = otp;
                            fdetail.OTPCode = otp;
                            fdetail.OTPtimeStamp= DateTime.Now.AddSeconds(300);
                           // Session["OTPtimeStamp"] = DateTime.Now.AddSeconds(300);
                            fdetail.step = 2;
                            ViewBag.OTPSent= userservice.SendOTP(fdetail, otp.ToString(), otpType,1);                           
                            return PartialView("_OTP", fdetail);
                        }
                        else
                        {
                            fdetail.step = 2;
                            fdetail.otpType = otpType;
                            ViewBag.error = "Please provide correct information.";
                            return PartialView("_OTP", fdetail);
                        }
                    }
                    else if (otpType == 2)
                    {
                        if (userservice.UserMobileNoExists(fdetail.MobileNo))
                        {
                            var userData = userservice.UserDetailForOTP(fdetail.MobileNo, otpType);
                            fdetail.EmailID = userData.EmailID;
                            fdetail.MobileNo = userData.MobileNo;
                            fdetail.UserName = userData.UserName;
                            int otp = userservice.GenerateOTPNo(fdetail.EmailID);
                            //Session["OTP"] = otp;
                            //Session["OTPtimeStamp"] = DateTime.Now.AddSeconds(300);
                            fdetail.OTPCode = otp;
                            fdetail.OTPtimeStamp = DateTime.Now.AddSeconds(300);
                            fdetail.step = 2;
                            ViewBag.OTPSent = userservice.SendOTP(fdetail, otp.ToString(), otpType,1);
                            return PartialView("_OTP", fdetail);
                        }
                        else
                        {
                            //ViewBag.error = "Mobile no is not linked with your NAFED HRMS account, Try to get One Time Password with email.";
                            fdetail.step = 2;
                            ViewBag.error = "Please provide correct information";
                            return PartialView("_OTP", fdetail);
                        }
                    }
                }
                else
                {
                    //if (DateTime.Parse(Session["OTPtimeStamp"].ToString()) <= DateTime.Now)
                    //{
                    if (fdetail.OTPtimeStamp <= DateTime.Now)
                    {
                        fdetail.step = 1;
                        ViewBag.error = "One Time password is expired, Try again. ";
                        return PartialView("_OTP", fdetail);
                    }
                    //else if (!string.IsNullOrEmpty(inputOTP) && Session["OTP"].ToString() == inputOTP)
                    //{
                    else if (!string.IsNullOrEmpty(inputOTP) && fdetail.OTPCode == Convert.ToInt32(inputOTP))
                    {
                        string targetDevice = "";
                        if (otpType == 1)
                        {
                            var userData = userservice.UserDetailForOTP(fdetail.EmailID, otpType);
                            fdetail.EmailID = userData.EmailID;
                            fdetail.MobileNo = userData.MobileNo;
                            fdetail.UserName = userData.UserName;
                            targetDevice = "Email";
                        }
                        else
                        {
                            var userData = userservice.UserDetailForOTP(fdetail.MobileNo, otpType);
                            fdetail.EmailID = userData.EmailID;
                            fdetail.MobileNo = userData.MobileNo;
                            fdetail.UserName = userData.UserName;
                            targetDevice = "Mobile";
                        }
                        userservice.ResetPassword(fdetail);
                        TempData["Message"] = $"Your new password has been sent to your {targetDevice}";
                        return JavaScript("window.location = '" + Url.Action("Index", "Login") + "'");
                    }
                    else
                    {
                        fdetail.step = 2;
                        ViewBag.error = "One Time Password did not match, Try again.";
                        return PartialView("_OTP", fdetail);
                    }
                }
                return PartialView("_OTP", fdetail);
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return PartialView("_OTP", fdetail);
            }
        }

        [HttpGet]
        public ActionResult SendOTP()
        {
            return PartialView("_OTP");
        }
    }
}