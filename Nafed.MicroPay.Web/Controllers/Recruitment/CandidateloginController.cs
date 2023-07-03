using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using Model = Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.Recruitment;
using System;

namespace MicroPay.Web.Controllers.Recruitment
{
    [EncryptedActionParameter]
    public class CandidateLoginController : Controller
    {

        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private readonly IRecruitmentService RecruitmentService;
        // GET: Candidatelogin

        public CandidateLoginController(IRecruitmentService RecruitmentService)
        {
            this.RecruitmentService = RecruitmentService;
        }
        public ActionResult Index()
        {
            log.Info("CandidateloginController/Index");
            Model.CandidateLogin objCandidateLogin = new Model.CandidateLogin();
            
            Session["RegistrationID"] = null;

            // After Authentication change session id
            string SessionId = RegenerateSession.RegenerateSessionId();
            Session["ASP.NET_SessionId"] = SessionId;
            // createa a new GUID and save into the session
            string guid = Guid.NewGuid().ToString();
            Session["AuthToken"] = guid;

            // now create a new cookie with this guid value
            Response.Cookies.Add(new HttpCookie("AuthToken", guid));

            ViewBag.Title = "Login";
            return View(objCandidateLogin);
        }
        [HttpPost]
        public ActionResult Index(Model.CandidateLogin CandidateLogin)
        {
            log.Info("CandidateloginController/Index");
            if (ModelState.IsValid)
            {
                Model.CandidateRegistration candidateForm = new Model.CandidateRegistration();
                bool isAuthenticated = false;
                string sAuthenticationMessage = string.Empty;
                ViewBag.LoginMessage = string.Empty;
                if (!this.IsCaptchaValid("Captcha is not valid"))
                {
                    ViewBag.LoginMessage = "Captcha is not valid";
                    return View(CandidateLogin);
                }

                isAuthenticated = RecruitmentService.ValidateUser(CandidateLogin, out sAuthenticationMessage, out candidateForm);

                if (isAuthenticated)
                { // After Authentication change session id
                                       
                    string SessionId = RegenerateSession.RegenerateSessionId();
                    Session["ASP.NET_SessionId"] = SessionId;
                    // createa a new GUID and save into the session
                    string guid = Guid.NewGuid().ToString();
                    Session["AuthToken"] = guid;

                    // now create a new cookie with this guid value
                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                    Session["RegistrationID"] = candidateForm.RegistrationID;

                    if (candidateForm.FormStatus.HasValue && candidateForm.FormStatus.Value == 2)
                        return RedirectToAction("ViewApplicationForm", "CandidateRegistration");
                    else
                        return RedirectToAction("Index", "CandidateRegistration");
                }
                else
                {
                    ViewBag.LoginMessage = sAuthenticationMessage;
                    TempData["LoginMessage"] = sAuthenticationMessage;

                    return View(CandidateLogin);
                    //return RedirectToAction("Index", "Login");
                }
            }
            else
                return View(CandidateLogin);
        }
    }
}