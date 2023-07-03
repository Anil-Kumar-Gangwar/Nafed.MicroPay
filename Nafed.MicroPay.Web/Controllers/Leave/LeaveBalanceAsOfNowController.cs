using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.IO;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Data.Repositories;
using DataModel = Nafed.MicroPay.Data;
using Common = Nafed.MicroPay.Common;
using static Nafed.MicroPay.Common.DocumentUploadFilePath;
using System.Data;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers.Leave
{
    public class LeaveBalanceAsOfNowController : BaseController
    {
        private readonly ILeaveBalanceAsOfNowService leaveBalanceAsOfNowDetailsService;
        private readonly IDropdownBindService ddlService;
        private readonly IEmployeeLeaveService empLeaveService;
        private IGenericRepository genericRepo;
        public LeaveBalanceAsOfNowController(IDropdownBindService ddlService, ILeaveBalanceAsOfNowService leaveBalanceAsOfNowDetailsService, IEmployeeLeaveService empLeaveService)
        {
            this.ddlService = ddlService;
            this.leaveBalanceAsOfNowDetailsService = leaveBalanceAsOfNowDetailsService;
            this.empLeaveService = empLeaveService;
            genericRepo = new GenericRepository();
        }

        public ActionResult Index()
        {
            log.Info($"LeaveBalanceAsOfNowController/Index");
            return View(userAccessRight);
        }

        public ActionResult LeaveBalanceAsOfNowGridView(Model.LeaveBalanceAsOfNow empLeave)
        {
            log.Info($"LeaveBalanceAsOfNowController/LeaveBalanceAsOfNowGridView");
            try
            {
                LeaveBalanceAsOfNowViewModel LeaveBalanceAsOfNowVM = new LeaveBalanceAsOfNowViewModel();

                LeaveBalanceAsOfNowVM.userRights = userAccessRight;
                var year = DateTime.Now.Year;
                empLeave.EmployeeCode = empLeave.EmployeeCode != null ? empLeave.EmployeeCode.Split('-')[0] : null;

                // LeaveBalanceAsOfNowVM.LeaveBalanceAsOfNowDetailsList = leaveBalanceAsOfNowDetailsService.GetEmployeeLeaveBalanceAsOfNowDetails(empLeave.EmployeeCode, empLeave.Branch, year.ToString());        

                LeaveBalanceAsOfNowVM.LeaveBalanceAsOfNowDetailsList = leaveBalanceAsOfNowDetailsService.GetEmployeeLeaveBalanceAsOfNowDetails(empLeave.EmployeeCode, empLeave.Branch, empLeave.LeaveYear);

                DataTable dataTable = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(LeaveBalanceAsOfNowVM.LeaveBalanceAsOfNowDetailsList);
                DataSet ds = new DataSet();
                ds.Tables.Add(dataTable);
                TempData["ExportData"] = ds;

                return PartialView("_LeaveBalanceAsOfNowGridView", LeaveBalanceAsOfNowVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult SearchLeaveBalanceAsOfNowDetails(FormCollection formCollection)
        {
            log.Info($"LeaveBalanceAsOfNowController/SearchLeaveBalanceAsOfNowDetails");
            try
            {
                BindDropdowns();
                LeaveBalanceAsOfNowViewModel LeaveBalanceAsOfNowVM = new LeaveBalanceAsOfNowViewModel();
                return PartialView("_LeaveBalanceAsOfNowDetails", LeaveBalanceAsOfNowVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            var ddlBranchList = ddlService.ddlBranchList();
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            var ddlEmployee = ddlService.GetAllEmployee();
            Model.SelectListModel selectemployeeCode = new Model.SelectListModel();
            selectemployeeCode.id = 0;
            selectemployeeCode.value = "Select";
            ddlEmployee.Insert(0, selectemployeeCode);
            ViewBag.EmployeeCode = new SelectList(ddlEmployee, "id", "value");
        }


        public ActionResult LeaveBalanceExportSheet()
        {
            try
            {
                log.Info("LeaveBalanceAsOfNowController/LeaveBalanceExportSheet");
                string fileName = string.Empty, msg = string.Empty;
                string fullPath = Server.MapPath("~/FileDownload/");
                fileName = ExtensionMethods.SetUniqueFileName("LeaveBalanceAsOfNow-", FileExtension.xlsx);

                var data = (DataSet)TempData["ExportData"];
                var res = empLeaveService.ExportLeaveBalanceAsOfNow(data, fullPath, fileName);
                return Json(new { fileName = fileName, fullPath = fullPath + fileName, message = "success" });
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public JsonResult GetEmployeeBranchWise(int branchID)
        {
            var ddlEmployee = new List<Model.SelectListModel>();
            if (branchID != 0)
            {
                ddlEmployee = ddlService.employeeByBranchID(branchID);
                Model.SelectListModel selectemployeeCode = new Model.SelectListModel();
                selectemployeeCode.id = 0;
                selectemployeeCode.value = "Select";
                ddlEmployee.Insert(0, selectemployeeCode);
                
            }
            else
            {
                ddlEmployee = ddlService.GetAllEmployee();
                Model.SelectListModel selectemployeeCode = new Model.SelectListModel();
                selectemployeeCode.id = 0;
                selectemployeeCode.value = "Select";
                ddlEmployee.Insert(0, selectemployeeCode);
            }

            return Json(new { EmployeeList = ddlEmployee });
        }

        [HttpGet]
        public ActionResult UpdateLeaveDetails(string empCode, string year)
        {
            try
            {
                log.Info($"LeaveBalanceAsOfNowController/UpdateLeaveDetails/{empCode}/{year}");
                ModelState.Clear();
                LeaveBalanceAsOfNowViewModel LeaveBalanceAsOfNowVM = new LeaveBalanceAsOfNowViewModel();
                var getLeaveBalanceInfo = leaveBalanceAsOfNowDetailsService.GetEmployeeLeaveBalanceAsOfNowDetails(empCode, null, year).FirstOrDefault();
                return PartialView("_UpdateLeaveBalanceDetails", getLeaveBalanceInfo);
            }
            catch(Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UpdateLeavesBalance(Model.LeaveBalanceAsOfNow leaveBalanceAsOfNow)
        {
            try
            {
                log.Info($"LeaveBalanceAsOfNowController/UpdateLeavesBalance");
                ModelState.Clear();
                leaveBalanceAsOfNowDetailsService.UpdateLeavesBalance(leaveBalanceAsOfNow);
                return PartialView("_UpdateLeaveBalanceDetails", leaveBalanceAsOfNow);
            }
            catch(Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}