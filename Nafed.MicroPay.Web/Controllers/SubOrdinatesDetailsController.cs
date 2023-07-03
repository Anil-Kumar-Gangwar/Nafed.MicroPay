using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using MicroPay.Web.Models;
using System.Linq;
using System.Web;
using System.Collections.Generic;

namespace MicroPay.Web.Controllers
{
    public class SubOrdinatesDetailsController : BaseController
    {
        private readonly IEmployeeService employeeService;
        public SubOrdinatesDetailsController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }
        public ActionResult Index()
        {
            log.Info($"StaffBudgetReportController/Index");
            try
            {
                SubOrdinatesViewModel SOVM = new SubOrdinatesViewModel();
                int? employeeID = null;
                //if (userDetail.UserTypeID != 1 && userDetail.UserTypeID != 2)
                    employeeID = userDetail.EmployeeID;
                SOVM.subOrdinatesDetails = employeeService.GetSubOrdinatesDetails(employeeID);
                return View(SOVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}