using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using System;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers.Leave
{
    public class LeaveAdjustmentController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IDropdownBindService ddlService;

        public LeaveAdjustmentController(IEmployeeService employeeService, IDropdownBindService ddlService)
        {
            this.employeeService = employeeService;
            this.ddlService = ddlService;
        }

        public ActionResult Index()
        {
            log.Info($"LeaveAdjustmentController/Index");
            EmployeeViewModel empVM = new EmployeeViewModel();
            empVM.userRights = userAccessRight;
            empVM.employeeType = ddlService.ddlEmployeeTypeList();
            empVM.designation = ddlService.ddlDesignationList();
            return View(empVM);
        }

        [HttpGet]
        public ActionResult _GetEmployeeGridView(EmployeeViewModel empVM)
        {
            EmployeeViewModel employeeVM = new EmployeeViewModel();
            var emp = employeeService.GetEmployeeList(empVM.EmployeeName, empVM.EmployeeCode, empVM.DesignationID, empVM.EmployeeTypeID);
            employeeVM.Employee = emp;
            employeeVM.userRights = userAccessRight;
            return PartialView("_LeaveAdjustmentGridView", employeeVM);
        }


        [HttpGet]
        public ActionResult Create()
        {
            log.Info("LeaveAdjustmentController/Create");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult Edit(int employeeID)
        {
            log.Info($"LeaveAdjustmentController/Edit/{employeeID}");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}