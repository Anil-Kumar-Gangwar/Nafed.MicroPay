using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.API;
using Nafed.MicroPay.Services.API.IAPIServices;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using static Nafed.MicroPay.Common.EncryptDecrypt;

namespace MicroPay.Web.Controllers.API
{
    public class AuthenticationController : BaseController
    {
        private readonly ILoginService loginService;
        public AuthenticationController()
        {
            loginService = new LoginService();
        }

        [System.Web.Mvc.HttpPost]
        public HttpResponseMessage ValidateUser(FormDataCollection formData)
        {
            log.Info($"/Api/AuthenticationController/ValidateUser : Called at :{DateTime.Now}");

            try
            {
                ValidateLogin validateLogin = new ValidateLogin()
                {
                    userName = formData.Get("userName"),
                    password = formData.Get("password")
                };

                string authenticationMessage, result = String.Empty, accessKey = string.Empty;
              
                UserDetail uDetail = new UserDetail();

                bool isAuthenticated = loginService.ValidateUser(validateLogin, out authenticationMessage, out uDetail);
                if (isAuthenticated)
                    accessKey = Encrypt(uDetail.UserName, cryptographyKey);

                var response = Request.CreateResponse(HttpStatusCode.OK);

                var responseJSON = JsonConvert.SerializeObject(new
                {
                    isAuthenticated = isAuthenticated,
                    accessKey= accessKey,
                    authenticationMessage = authenticationMessage,
                    UserDetail = uDetail
                });
                response.Content = new StringContent(responseJSON, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
                throw ex;
            }
        }

        [HttpPost]
        //     [Route("API/MarkAttendance/PostData")]
        public HttpResponseMessage PostData(FormDataCollection frmData)
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { content = "data fetched successfully." });
        }

    }
}
