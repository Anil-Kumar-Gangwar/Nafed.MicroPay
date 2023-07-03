using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Common;
namespace MicroPay.Web.Controllers.Appraisal
{
    public class APARDatesController : BaseController
    {
        private readonly IAppraisalFormDueDateService aparduedateServ;
        public APARDatesController(IAppraisalFormDueDateService aparduedateServ)
        {
            this.aparduedateServ = aparduedateServ;

        }
        // GET: APARDates
        public ActionResult Index()
        {
            log.Info($"APARDatesController/Index");
            return View();
        }

        public ActionResult _GetAPARDatesGridView()
        {
            log.Info("APARDatesController/_GetAPARDatesGridView");
            try
            {
                List<AppraisalFormDueDate> objAPARDatesList = new List<AppraisalFormDueDate>();
                objAPARDatesList = aparduedateServ.GetAppraisalFormDueDate(null);
                return PartialView("_APARDatesList", objAPARDatesList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult Edit(string reportingYr)
        {
            log.Info("APARDatesController/Edit");
            try
            {
                AppraisalFormDueDate objAPARDates = new AppraisalFormDueDate();
                objAPARDates = aparduedateServ.GetAppraisalFormDueDate(reportingYr).First();
                return View(objAPARDates);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AppraisalFormDueDate updateAPARDates)
        {
            log.Info("APARDatesController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    //  updateAPARDates.ReportingYear = DateTime.Now.Date.GetFinancialYr();
                    aparduedateServ.UpdateAppraisalForm(updateAPARDates);
                    TempData["Message"] = "APAR Due Dates updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(updateAPARDates);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateAPARDates);
        }
    }
}