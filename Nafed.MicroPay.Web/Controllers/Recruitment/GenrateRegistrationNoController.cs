using System.Web.Mvc;
using Model = Nafed.MicroPay.Model;
using System;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.Recruitment;
using Nafed.MicroPay.Services.IServices;
using CaptchaMvc.HtmlHelpers;

namespace MicroPay.Web.Controllers.Recruitment
{
    [EncryptedActionParameter]
    public class GenrateRegistrationNoController : Controller
    {
        private readonly IUserService userservice;
        private readonly IRecruitmentService recruitmentService;

        public GenrateRegistrationNoController(IUserService userservice, IRecruitmentService recruitmentService)
        {
            this.userservice = userservice;
            this.recruitmentService = recruitmentService;
        }

        public ActionResult Index(int requirementId)
        {
            try
            {
                Model.CandidateRegistration candidateRegistration = new Model.CandidateRegistration();
                candidateRegistration.RequirementID = requirementId;
                return View(candidateRegistration);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GenrateRegistrationNumber(Model.CandidateRegistration candidateRegistration, string ButtonType, int otpType, FormCollection frm)
        {
            try
            {
                ModelState.Remove("SelectedLocationID");
                ModelState.Remove("Husband_FatherName");
                ModelState.Remove("PmtAdd");
                ModelState.Remove("PmtStreet");
                ModelState.Remove("PmtCity");
                ModelState.Remove("PmtPin");
                ModelState.Remove("DeclarationName");
                ModelState.Remove("DeclarationPlace");
                ModelState.Remove("DeclarationDate");
                ModelState.Remove("Declaration");
                ModelState.Remove("AnnualGrossSalary");
                ModelState.Remove("DOB");
                ModelState.Remove("FeesApplicable");
                ModelState.Remove("FormStatus");
                ModelState.Remove("MimimumExpenrienceNo");


                if (ModelState.IsValid)
                {
                    if (!this.IsCaptchaValid("Captcha is not valid"))
                    {
                        ModelState.AddModelError("CaptchaInputText", "Captcha is not valid");
                        return PartialView("_RegistrationOTP", candidateRegistration);
                    }

                    if (ButtonType == "Generate OTP" || ButtonType == "Resend Otp")
                    {
                        bool isMobileNoAlreadyRegistered = recruitmentService.VerifyCandidationRegisterInfo(candidateRegistration.RequirementID, candidateRegistration.MobileNo.Trim(), 2);
                        bool isValidEmailIDAlreadyRegistered = recruitmentService.VerifyCandidationRegisterInfo(candidateRegistration.RequirementID, candidateRegistration.PersonalEmailID.Trim().ToLower(), 1);

                        if (isMobileNoAlreadyRegistered || isValidEmailIDAlreadyRegistered)
                        {
                            if (isMobileNoAlreadyRegistered)
                                ModelState.AddModelError("InValidMobileNo", "This mobile number is already registered for this post.");
                            if (isValidEmailIDAlreadyRegistered)
                                ModelState.AddModelError("InValidEmailID", "This email id is already registered for this post.");

                            return PartialView("_RegistrationOTP", candidateRegistration);
                        }

                        if (otpType == 2)
                        {
                            if (!string.IsNullOrEmpty(candidateRegistration.MobileNo))
                            {
                                int otp = userservice.GenerateOTPNo(candidateRegistration.AlternateEmailId);
                                Session["OTP"] = otp;
                                string mobileNo = candidateRegistration.MobileNo;
                                Model.UserDetail fdetail = new Model.UserDetail()
                                {
                                    MobileNo = mobileNo
                                };
                                ViewBag.OTPSent = userservice.SendOTP(fdetail, otp.ToString(), otpType, 2);
                                if (ViewBag.OTPSent)
                                    ViewBag.Message = "OTP has been sent on your mobile No.";

                                ViewBag.IsVerified = false;
                                return PartialView("_RegistrationOTP", candidateRegistration);
                            }
                        }
                        else if (otpType == 1)
                        {
                            if (!string.IsNullOrEmpty(candidateRegistration.PersonalEmailID))
                            {
                                int otp = userservice.GenerateOTPNo(candidateRegistration.AlternateEmailId);
                                Session["OTP"] = otp;
                                string emailid = candidateRegistration.PersonalEmailID;
                                Model.UserDetail fdetail = new Model.UserDetail()
                                {
                                    EmailID = emailid
                                };
                                ViewBag.OTPSent = userservice.SendOTP(fdetail, otp.ToString(), otpType, 2);
                                if (ViewBag.OTPSent)
                                    ViewBag.Message = "OTP has ben sent on your personal emailID.";

                                ViewBag.IsVerified = false;
                                return PartialView("_RegistrationOTP", candidateRegistration);
                            }
                        }
                    }
                    if (ButtonType == "Verify OTP")
                    {
                        if (Session["OTP"] != null)
                        {
                            var otpval = Convert.ToString(Session["OTP"]);
                            var opt = frm["textotp"];
                            if (opt != "" && otpval == opt)
                            {
                                ViewBag.Message = "Otp has been verified.";
                                ViewBag.VerifyOTP = true;
                                ViewBag.IsVerified = true;
                            }
                            else if (opt != "" && otpval != opt)
                            {
                                ViewBag.Message = "Incorrct OTP.";
                                ViewBag.IsVerified = false;
                            }
                            else
                            {
                                ViewBag.Message = "Please enter OTP.";
                                ViewBag.IsVerified = false;
                            }
                            return PartialView("_RegistrationOTP", candidateRegistration);
                        }
                    }
                    if (ButtonType == "Submit")
                    {
                        var requirementDetails = recruitmentService.GetRequirementByID(candidateRegistration.RequirementID);
                        candidateRegistration.DesignationID = requirementDetails.DesinationID;
                        candidateRegistration.PostName = requirementDetails.Post;
                        var jj = requirementDetails.JLocTypeId;
                        candidateRegistration.JobLocTypeID = jj;
                        candidateRegistration.MimimumExpenrienceNo = requirementDetails.MimimumExpenrienceNo;
                        TempData["CandidateRegistration"] = candidateRegistration;
                        //return RedirectToAction("Index", "CandidateRegistration");
                        return JavaScript("window.location = '" + Url.Action("Index", "CandidateRegistration") + "'");
                    }
                }
                return PartialView("_RegistrationOTP", candidateRegistration);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}