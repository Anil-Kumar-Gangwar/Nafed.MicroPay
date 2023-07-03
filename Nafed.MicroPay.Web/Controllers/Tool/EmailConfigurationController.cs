using System;
using System.Linq;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services;
using static Nafed.MicroPay.Common.SiteMaintenance;

namespace MicroPay.Web.Controllers.Tool
{
    public class EmailConfigurationController : BaseController
    {
        private readonly IEmailConfigurationService emailConfig;
        public EmailConfigurationController(IEmailConfigurationService emailConfig)
        {
            this.emailConfig = emailConfig;
        }

        public ActionResult Index()

        {
            log.Info($"Tool/EmailConfigurationController/Index");
            EmailConfiguration emailSetting = new EmailConfiguration();
            try
            {
                emailSetting = (emailConfig.GetEmailConfigList()).FirstOrDefault();
                if (emailSetting != null)
                {
                    if (emailSetting.IsMaintenance && emailSetting.MaintenanceDateTime.HasValue)
                    {
                        if (emailSetting.MaintenanceDateTime.Value < DateTime.Now)
                            IsSiteUnderMaintenace = false;
                        else
                            IsSiteUnderMaintenace = true;
                    }
                    else
                        IsSiteUnderMaintenace = false;
                }
                else
                    IsSiteUnderMaintenace = false;
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }

            return View("Index", emailSetting);
        }


        [HttpPost]

        public ActionResult Create([Bind(Exclude = "EmailConfigurationID")]  EmailConfiguration config)
        {
            log.Info($"EmailConfigurationController/Create");
            try
            {
                int emailConfigurationID = Request.Form["EmailConfigurationID"] != "" ? Convert.ToInt32(Request.Form["EmailConfigurationID"]) : 0;
                if (ModelState.IsValid)
                {
                    if (config.IsMaintenance && !config.MaintenanceDateTime.HasValue)
                    {
                        config.EmailConfigurationID = 1;
                        ModelState.AddModelError("validateDate", "Please enter date and time for maintenance");
                        return View("Index", config);
                    }

                    if (emailConfigurationID == 0)
                    {
                        config.EmailConfigurationID = 1;
                        config.CreatedBy = (int)userDetail.UserID;
                        config.CreatedOn = System.DateTime.Now;

                        bool res = emailConfig.CreateEmailConfiguration(config);
                        if (res)
                        {
                            IsSiteUnderMaintenace = config.IsMaintenance;
                            TempData["message"] = "Successfully Inserted";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        config.EmailConfigurationID = emailConfigurationID;
                        config.UpdatedBy = userDetail.UserID;
                        config.UpdatedOn = System.DateTime.Now;
                        bool res = emailConfig.UpdateEmailConfiguration(config);
                        if (res)
                        {
                            TempData["message"] = "Successfully Updated";
                            return RedirectToAction("Index");
                        }
                    }
                }
                return View("Index", config);
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
        }


    }
}