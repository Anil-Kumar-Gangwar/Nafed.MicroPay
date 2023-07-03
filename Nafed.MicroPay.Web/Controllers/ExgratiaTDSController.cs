using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using System.Web.Mvc;
using Model = Nafed.MicroPay.Model;
using System;
using System.Linq;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers
{
    public class ExgratiaTDSController : BaseController
    {
        private readonly IExgratiaService exgratiaService;

        public ExgratiaTDSController(IExgratiaService exgratiaService)
        {
            this.exgratiaService = exgratiaService;
        }
        public ActionResult Index()
        {
            log.Info($"ExgratiaTDSController/Index");
            try
            {
                ExgratiaViewModel ExVM = new ExgratiaViewModel();
             
                //ExVM.listExgratia = exgratiaService.GetExgratiaList("",0);
                return View(ExVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult TDSExgratia(int ? ExgratiaID)
        {
            log.Info($"ExgratiaTDSController/TDSExgratia");
            try
            {
                //ModelState.Clear();
                BindDropdowns();
                Model.ExgratiaList ExgrList = new Model.ExgratiaList();
                ExgrList.ExgratiaID = ExgratiaID;
                if (ExgratiaID != 0)
                {
                    BindDropdowns();
                    ExgrList = exgratiaService.GetExgratiaByID(ExgratiaID);
                    ExgrList.FormActionType = "Update";
                }
                else
                    BindDropdowns();
                return PartialView("_ExgratiaTDSForm", ExgrList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult TDSExgratia(Model.ExgratiaList exgratia)
        {
            log.Info($"ExgratiaTDSController/TDSExgratia");
            int exgratiaTDS = 0;
            if (exgratia.FinancialYear == null)
            {
                ModelState.AddModelError("FinancialYear", "Please select year");
                return View(exgratia);
            }
            if (exgratia.EmployeeID == 0)
            {
                ModelState.AddModelError("FinancialYear", "Please select employee");
                return View(exgratia);
            }

            if (ModelState.IsValid)
            {
                exgratia.Incometax = exgratia.Other;
                exgratia.Other = exgratia.Incometax;
                exgratia.FinancialYear = exgratia.FinancialYear;
                exgratia.OtherDeduction = exgratia.OtherDeduction;
                exgratia.UpdatedBy = userDetail.UserID;
                exgratia.UpdatedOn = System.DateTime.Now;
                exgratiaTDS = exgratiaService.updateExgratiaTDS(exgratia);
                exgratia.Saved = true;

                TempData["message"] = "Successfully updated.";
                return Json(new { saved = exgratia.Saved }, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_ExgratiaTDSForm", exgratia);
        }
        public void BindDropdowns()
        {
            Model.ExgratiaList EXVM = new Model.ExgratiaList();
            var reportingYr = ExtensionMethods.GetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
            ViewBag.ddlYear = new SelectList(reportingYr, "Value", "Text");

            var ddlEmployeeList = exgratiaService.GetAllEmployee();
            ddlEmployeeList.OrderBy(x => x.value);
            Model.SelectListModel employee = new Model.SelectListModel();
            employee.id = 0;
            employee.value = "Select";
            ddlEmployeeList.Insert(0, employee);
            ViewBag.employee = new SelectList(ddlEmployeeList, "id", "value");
        }

    }
}