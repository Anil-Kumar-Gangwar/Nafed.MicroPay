using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using System.Data;

namespace MicroPay.Web.Controllers
{
    public class ExgratiaListController : BaseController
    {
        private readonly IExgratiaService exgratiaService;

        public ExgratiaListController(IExgratiaService exgratiaService)
        {
            this.exgratiaService = exgratiaService;
        }
        public ActionResult Index()
        {
            log.Info($"ExgratiaListController/Index");
            try
            {
                return View(userAccessRight);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetExgratiaList()
        {
            log.Info($"ExgratiaListController/GetExgratiaList");
            try
            {
                ExgratiaViewModel EXVM = new ExgratiaViewModel();
                var reportingYr = ExtensionMethods.GetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
                EXVM.yearList = reportingYr;
                return PartialView("_ExgratiaList", EXVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GenerateExgratiaList(ExgratiaViewModel EXGR)
        {
            log.Info($"ExgratiaListController/GetExgratiaList");
            try
            {
                int exgratialist = 0;
                if (ModelState.IsValid)
                {
                    var getexgratiaList = exgratiaService.checkexistExgratiaList(EXGR.selectYearID);
                    DataTable dt = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(getexgratiaList);
                    if (dt.Rows.Count > 0)
                    {
                        return Json(new { formAction = "AlreadyExgratia" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                         exgratialist = exgratiaService.GenerateList(EXGR.selectYearID, Convert.ToInt32(EXGR.selectYearID.Substring(0,4)));
                    }
                    if(exgratialist>0)
                    {
                        return Json(new { formAction = "GenerateExgratia" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { formAction = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {

                    return PartialView("_ExgratiaList", EXGR);
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