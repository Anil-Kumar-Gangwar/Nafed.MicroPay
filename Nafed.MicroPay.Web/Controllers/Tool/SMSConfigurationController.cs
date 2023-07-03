using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers.Tool
{
    public class SMSConfigurationController : BaseController
    {
        // GET: SMSConfiguration
        private readonly ISMSConfigurationService smsConfigService;
        public SMSConfigurationController(ISMSConfigurationService smsConfigService)
        {
            this.smsConfigService = smsConfigService;
        }
        public ActionResult Index()
        {
            log.Info($"SMSConfigurationController/Index");
            SMSConfiguration smsConfig = new SMSConfiguration();
            try
            {
                smsConfig = (smsConfigService.GetSMSConfiguration()).FirstOrDefault();
               // smsConfig.CreatedBy = 3;
            }
            catch (Exception ex)
            {
                log.Error($"Message-{ ex.Message}, StackTrace-{ ex.StackTrace}, DatetimeStamp-{ DateTime.Now}");
                throw ex;
            }
            return View("Index", smsConfig);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Exclude = "SMSConfigurationID")]  SMSConfiguration config)
        {
            int smsConfigurationID = Request.Form["SMSConfigurationID"] != "" ? Convert.ToInt32(Request.Form["SMSConfigurationID"]) : 0;

            if (smsConfigurationID == 0)
            {
                log.Info($"SMSConfigurationController/Create");
               // config.SenderID = 1;
                config.CreatedBy = 3;
                config.CreatedOn = System.DateTime.Now;

                ModelState.Remove("CreatedBy");
                ModelState.Remove("CreatedOn");
                if (ModelState.IsValid)
                {
                    bool res = smsConfigService.CreateSMSConfiguration(config);
                    if (res)
                        return RedirectToAction("Index");
                }
                else
                {
                    return View("Index");
                }
            }
            else
            {
                log.Info($"SMSConfigurationController/Update");
                config.SMSConfigurationID = smsConfigurationID;
                config.UpdatedBy = 3;
                config.UpdatedOn = System.DateTime.Now;
                bool res = smsConfigService.UpdateSMSConfiguration(config);
                if (res)
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}