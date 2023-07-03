using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using CaptchaMvc.HtmlHelpers;
using Nafed.MicroPay.Services;
using System.Web;
using System.Security.Cryptography;
using System.Web.SessionState;
using System.Reflection;

namespace MicroPay.Web.Controllers
{
    public class LoginController : Controller
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
 (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        //    protected static readonly log4net.ILog loginlog =
        //log4net.LogManager.GetLogger("LoginAppender");

        private readonly ILoginService loginService;
        private readonly IMenuService menuService;

        public LoginController(ILoginService loginService, IMenuService menuService)
        {
            this.loginService = loginService;
            this.menuService = menuService;
        }
        public ActionResult Index()
        {
            log.Info("LoginController/Index");            

            Model.ValidateLogin objValidateLogin = new Model.ValidateLogin();
            ViewBag.Title = "Login";
          
            return View(objValidateLogin);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Model.ValidateLogin validateLogin)
        {            
            log.Info("LoginController/Index");

            if (string.IsNullOrEmpty(validateLogin.hduserName))
                ModelState.AddModelError("UserName", "Please enter user name.");
            if (string.IsNullOrEmpty(validateLogin.hdpassword))
                ModelState.AddModelError("Password", "Please enter password.");

            if (ModelState.IsValid)
            {
                if (!this.IsCaptchaValid("Captcha is not valid"))
                {
                    ViewBag.LoginMessage = "Captcha is not valid";
                    return View(validateLogin);
                }

                validateLogin.userName = Password.DecryptStringAES(validateLogin.hduserName);
                validateLogin.password = Password.DecryptStringAES(validateLogin.hdpassword).Replace(validateLogin.hdCp, "");

                bool isAuthenticated = false;
                string sAuthenticationMessage = string.Empty;
                ViewBag.LoginMessage = string.Empty;

                Model.UserDetail uDetail = new Model.UserDetail();
                isAuthenticated = loginService.ValidateUser(validateLogin, out sAuthenticationMessage, out uDetail);

                if (isAuthenticated)
                {
                    if ((uDetail.UserTypeID != (int)Nafed.MicroPay.Common.UserType.Admin || uDetail.UserTypeID != (int)Nafed.MicroPay.Common.UserType.SuperUser)
                        && uDetail.IsMaintenance && uDetail.MaintenanceDateTime > System.DateTime.Now)
                    {
                        return RedirectToAction("Maintenance");
                    }
                    else
                    {
                        Session["User"] = uDetail;
                        TempData["UserID"] = uDetail.UserID;
                        TempData["DepartmentID"] = uDetail.DepartmentID;

                        // createa a new GUID and save into the session
                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;
                       
                        // now create a new cookie with this guid value
                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                        // After Authentication change session id
                        string SessionId = RegenerateSessionId();
                        Session["ASP.NET_SessionId"] = SessionId;
                        //===== Insert user login details==============

                        Model.UserLoginDetails userLoginDetails = new Nafed.MicroPay.Model.UserLoginDetails();
                        userLoginDetails.UserName = uDetail.UserName;
                        userLoginDetails.LoginTime = DateTime.Now;
                        userLoginDetails.SessionId = SessionId;
                        loginService.InsertLoginDetials(userLoginDetails);
                        //===== Insert user login details==============



                        if (!uDetail.DoLeaveOrg.HasValue)
                        {
                            //===== Get user menu rights==============
                            uDetail.userMenuRights = menuService.GetUserMenuRights(uDetail.UserID, uDetail.DepartmentID, uDetail.UserTypeID);
                            //====== End=============================

                            return RedirectToAction("Index", "Home");
                        }
                        else
                            return RedirectToAction("Index", "RetiredEmpHomePage");


                    }
                }
                else
                {
                    if (uDetail.ErrorMessage == "notexists")
                    {
                        ViewBag.LoginMessage = "Invalid Credentials.";
                        TempData["LoginMessage"] = "Invalid Credentials.";
                    }
                    else if (uDetail.WrongAttemp.HasValue)
                    {
                        if (uDetail.WrongAttemp.Value == 0)
                        {
                            ViewBag.LoginMessage = "Your account is temporary locked, please contact your administrator.";
                            TempData["LoginMessage"] = "Your account is temporary locked, please contact your administrator.";
                        }
                        else
                        {
                            ViewBag.LoginMessage = $"Invalid Credentials, {uDetail.WrongAttemp.Value} attempt(s) left.";
                            TempData["LoginMessage"] = $"{uDetail.WrongAttemp.Value} attemp(s) left.";
                        }
                    }
                    else
                    {
                        ViewBag.LoginMessage = $"Your account is temporary locked, please try after {uDetail.ErrorMessage}";
                        TempData["LoginMessage"] = $"Your account is temporary locked, please try after {uDetail.ErrorMessage}";
                    }
                    return View(validateLogin);
                    //return RedirectToAction("Index", "Login");
                }
            }
            else
                return View(validateLogin);
        }

        public ActionResult Maintenance()
        {
            return PartialView("_Offline");
        }
        #region Notification
        public async Task FirebaseTopicNotification()
        {
            try
            {
                string SERVER_API_KEY = "AAAAMBRPDG4:APA91bHUZeRpsFxapBel9NGfaiN-1WdqKKGopUTMqRbkI8gJbjq4u025dY0RsyHLQLYXQmtsDNELjra_oJV7MxDMVMFuUL7F7aaCRDFY--AsvUnEma9bP_2_Tlgo2Vh0OH1Cl8kKZ5al";
                var SENDER_ID = "206499155054";
                //var value = message;
                List<string> topic = new List<string>();
                topic.Add("1412");
                Dictionary<string, string> data = new Dictionary<string, string>();
                foreach (var item in topic)
                {
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    //serverKey - Key from Firebase cloud messaging server  
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
                    //Sender Id - From firebase project setting  
                    tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
                    tRequest.ContentType = "application/json";
                    var payload = new
                    {
                        to = "/topics/" + Convert.ToString(item),
                        priority = "high",
                        content_available = true,
                        notification = new
                        {
                            body = "Login Alert!",
                            title = "Nafed Notification",
                            badge = 1
                        },
                        data = data

                    };

                    string postbody = JsonConvert.SerializeObject(payload).ToString();
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                    tRequest.ContentLength = byteArray.Length;
                    tRequest.ContentType = "application/json";
                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = await tRequest.GetResponseAsync())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        String sResponseFromServer = tReader.ReadToEnd();
                                        //result.Response = sResponseFromServer;
                                    }
                            }
                        }
                    }
                    //return sResponseFromServer;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion


        private string RegenerateSessionId()
        {
            System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
            string oldId = manager.GetSessionID(System.Web.HttpContext.Current);
            string newId = manager.CreateSessionID(System.Web.HttpContext.Current);
            bool isAdd = false, isRedir = false;
            manager.SaveSessionID(System.Web.HttpContext.Current, newId, out isRedir, out isAdd);
            HttpApplication ctx = (HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            HttpModuleCollection mods = ctx.Modules;
            System.Web.SessionState.SessionStateModule ssm = (SessionStateModule)mods.Get("Session");
            System.Reflection.FieldInfo[] fields = ssm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            SessionStateStoreProviderBase store = null;
            System.Reflection.FieldInfo rqIdField = null, rqLockIdField = null, rqStateNotFoundField = null;
            foreach (System.Reflection.FieldInfo field in fields)
            {
                if (field.Name.Equals("_store")) store = (SessionStateStoreProviderBase)field.GetValue(ssm);
                if (field.Name.Equals("_rqId")) rqIdField = field;
                if (field.Name.Equals("_rqLockId")) rqLockIdField = field;
                if (field.Name.Equals("_rqSessionStateNotFound")) rqStateNotFoundField = field;
            }
            object lockId = rqLockIdField.GetValue(ssm);
            if ((lockId != null) && (oldId != null)) store.ReleaseItemExclusive(System.Web.HttpContext.Current, oldId, lockId);
            rqStateNotFoundField.SetValue(ssm, true);
            rqIdField.SetValue(ssm, newId);
            return newId;
        }

       

    }
}