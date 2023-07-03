using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers
{
    public class DesignationPayScaleController : BaseController
    {
        private readonly IDesignationService designationService;
        public DesignationPayScaleController(IDesignationService designationService)
        {
            this.designationService = designationService;
        }
        public ActionResult Index()
        {
            log.Info($"DesignationPayScaleController/Index");
            return View(userAccessRight);
        }

        public ActionResult DesignationPayScale(int Designationid)
        {
            log.Info($"DesignationPayScaleController/DesignationPayScale");
            try
            {
                //var designationId = Convert.ToInt32(Request.QueryString["Designationid"]);
                var getDesignationPayScale = designationService.GetDesignationPayScale(Designationid);
                return PartialView("_DesignationPayScale", getDesignationPayScale);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _EditPayScaleDetails(EmployeePayScale payScale)
        {
            log.Info($"DesignationPayScaleController/_EditPayScaleDetails");
            try
            {
                ModelState.Remove("DesignationName");
                if (ModelState.IsValid)
                {
                    payScale.UpdatedBy = userDetail.UserID;
                    payScale.UpdatedOn = DateTime.Now;
                    bool isUpdated = designationService.UpdateDesignationPayScales(payScale);
                }
                return PartialView("_DesignationPayScale", payScale);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult CalculateBasic(EmployeePayScale payScale)
        {
            log.Info($"DesignationPayScaleController/CalculateBasic");
            try
            {
                if (payScale.B1 != 0)
                {
                    payScale.level = payScale.level.Length < 2 ? "0" + payScale.level : payScale.level;
                    var updatedPayScale = designationService.CalculateBasicAndIncrement(payScale);
                    return PartialView("_DesignationPayScale", updatedPayScale);
                }
               else
                {
                    return PartialView("_DesignationPayScale", payScale);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}