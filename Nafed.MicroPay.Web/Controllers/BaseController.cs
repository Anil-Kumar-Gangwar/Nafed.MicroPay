using MicroPay.Web.Attributes;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services;
using System.Collections.Generic;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using Nafed.MicroPay.Services.IServices;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using Microsoft.Security.Application;
using System;

namespace MicroPay.Web.Controllers
{
    [EncryptedActionParameter]
    [SessionTimeout]
    [CheckRight]
    [InsufficientLoggingMonitoring]
    public class BaseController : Controller
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
             (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //   protected static readonly log4net.ILog traininglog =
        //log4net.LogManager.GetLogger("RescheduleFileAppender");

        protected UserDetail userDetail
        {
            get
            {
                return (UserDetail)System.Web.HttpContext.Current.Session["User"];
            }
        }

        public Dictionary<string, string> GetErrorMessage() => ImportValidationMessage.ImportErrMsg();

        protected UserAccessRight userAccessRight
        {
            get
            {
                return (UserAccessRight)System.Web.HttpContext.Current.Session["UserAccess"];
            }
        }

        public ActionResult UnAuthorizeAccess()
        {

            log.Info($"Base/UnAuthorizeAccess");
            return View("UnAuthorizeAccess");
        }


        public FileResult GetImage(string imgPath)
        {
            log.Info($"Get Item image path {imgPath}");
            byte[] imageByteData = ImageService.GetImage(imgPath);
            return File(imageByteData, "image/jpg;base64");
        }


        [DeleteFileAttribute]
        public ActionResult DownloadAndDelete(string sFileName, string sFileFullPath)
        {
            log.Info($"Download and then delete file {sFileName} from path {sFileFullPath}.");
            if (TempData.ContainsKey("filePath"))
                TempData.Remove("filePath");
            TempData.Add("filePath", sFileFullPath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(sFileFullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, sFileName);
        }

        public ActionResult DownloadFile(string sFileName, string sFileFullPath)
        {
            log.Info($"Download file {sFileName} from path {sFileFullPath}.");
            //if (TempData.ContainsKey("filePath"))
            //    TempData.Remove("filePath");
            //TempData.Add("filePath", sFileFullPath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(sFileFullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, sFileName);
        }

        [NonAction]
        public string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

        [NonAction]

        protected EmployeeProcessApproval GetEmpProcessApprovalSetting(int empID, WorkFlowProcess wrkProcess)
        {
            BaseService ob = new BaseService();
            return ob.GetProcessApprovalSetting(empID, wrkProcess);
        }

        public RedirectResult ReportViewer()
        {
            return Redirect("~/ReportViewer.aspx");
        }

        public string GetSafeHtmlFragment(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(HttpUtility.HtmlEncode(value));
            stringBuilder.Replace("&lt;b&gt;", "<b>");
            stringBuilder.Replace("&lt;/b&gt;", "</b>");
            stringBuilder.Replace("&lt;u&gt;", "<u>");
            stringBuilder.Replace("&lt;/u&gt;", "</u>");
            return stringBuilder.ToString();

        }

        public static bool ValidateAntiXSS<T>(T obj)
        {
            var pattren = new StringBuilder();
            //Checks any js events i.e. onKeyUp(), onBlur(), alerts and custom js functions etc.             
            pattren.Append(@"((alert|on\w+|function\s+\w+)\s*\(\s*(['+\d\w](,?\s*['+\d\w]*)*)*\s*\))");

            //Checks any html tags i.e. <script, <embed, <object etc.
            pattren.Append(@"|(<(script|iframe|embed|frame|frameset|object|img|applet|body|html|style|layer|link|ilayer|meta|bgsound))");

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    string inputParameter = Convert.ToString(property.GetValue(obj, null));
                    if (Regex.IsMatch(HttpUtility.UrlDecode(inputParameter), pattren.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public T GetSafeHtml<T>(T obj)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    string val = Sanitizer.GetSafeHtmlFragment(Convert.ToString(property.GetValue(obj, null)));
                    property.SetValue(obj, Sanitizer.GetSafeHtmlFragment(Convert.ToString(property.GetValue(obj, null))), null);
                }
            }
            return obj;
        }        
    }
}