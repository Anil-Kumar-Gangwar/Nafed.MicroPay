using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.Linq;
using System.Data;

namespace MicroPay.Web.Controllers
{
    public class Form11ViewController : BaseController
    {
        private readonly IEmployeePFOrganisationService EmployeePFOrganisationService;
        private readonly IDropdownBindService ddlService;

        public Form11ViewController(IEmployeePFOrganisationService EmployeePFOrganisationService, IDropdownBindService ddlService)
        {
            this.EmployeePFOrganisationService = EmployeePFOrganisationService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            BindDropdowns();
            EmpPFOrgViewModel EMPPFVM = new EmpPFOrgViewModel();
            EMPPFVM.userRights = userAccessRight;
            return View(EMPPFVM);
        }
        [HttpGet]
        public ActionResult _Form11View(EmpPFOrgViewModel EMPPFVM)
        {
            log.Info($"Form11ViewController/_Form11View");
            try
            {
                BindDropdowns();
                EmpPFOrgViewModel Form11VM = new EmpPFOrgViewModel();
                Form11VM.EmpPFOrgList = EmployeePFOrganisationService.GetEmpPFHList(EMPPFVM.EmployeeId == null ? 0 : (int)EMPPFVM.EmployeeId);
                Form11VM.userRights = userAccessRight;
                return PartialView("_Form11View", Form11VM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
           
            var ddlEmployee = ddlService.GetAllEmployee();
            Model.SelectListModel selectEmployee = new Model.SelectListModel();          
            ViewBag.Employee = new SelectList(ddlEmployee, "id", "value");
          
        }
        [HttpGet]
        public ActionResult _Edit(int EmpPFID, int statusID)
        {
            log.Info($"Form11ViewController/_Edit");
            try
            {
                Model.EmployeePFORG EMPPFHVM = new Model.EmployeePFORG();
                var getexgratiaList = EmployeePFOrganisationService.checkexistdata(EmpPFID);
                DataTable dt = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(getexgratiaList);
                if (dt.Rows.Count > 0)
                {
                    EMPPFHVM = EmployeePFOrganisationService.GetEMPPFOrgDetails(Convert.ToInt32(dt.Rows[0]["ID"]), EmpPFID, statusID);
                }
                else
                {
                    EMPPFHVM = EmployeePFOrganisationService.GetEMPPFOrgDetails(0, EmpPFID, statusID);
                }
                
                return PartialView("_Edit", EMPPFHVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

    }
}