using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services.Salary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers.Salary
{
    public class ArrearApprovalRequestController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IAdjustOldLoanService adjustOldLoanService;
        private readonly IPayrollApprovalSettingService pAppSettingService;
        private readonly ISalaryReportService salReportService;
        private readonly IBranchService branchService;

        // GET: ArrearApprovalRequest

        public ArrearApprovalRequestController(IDropdownBindService ddlService,
            IAdjustOldLoanService adjustOldLoanService,
            IPayrollApprovalSettingService pAppSettingService,
            ISalaryReportService salReportService,
            IBranchService branchService)
        {
            this.ddlService = ddlService;
            this.adjustOldLoanService = adjustOldLoanService;
            this.pAppSettingService = pAppSettingService;
            this.salReportService = salReportService;
            this.branchService = branchService;
        }

        public ActionResult Index()
        {
            log.Info($"ArrearApprovalRequest/Index");
            return View();
        }
        public ActionResult GetPeriodDetails(string details, string flag)
        {
            log.Info($"ArrearApprovalRequest/GetPeriodDetails");
            try
            {
                if (flag == "AD")
                {
                    var periodDetails = (List<ArrerPeriodDetails>)TempData["DAArrerDetails"];
                    TempData.Keep("DAArrerDetails");
                    var result = periodDetails.Where(x =>
                    x.details.ToLower().Trim() == details.ToLower().Trim()).
                        Select(y => new { m = (y.fromperiod + " To " + y.toperiod), fromperiod = y.fromperiod, toperiod = y.toperiod }).FirstOrDefault();
                    return Json(new { DAArrerPeriods = result.m, fromPeriod = result.fromperiod, toPeriod = result.toperiod }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var periodDetails = (List<ArrerPeriodDetails>)TempData["PayArrerDetails"];
                    TempData.Keep("PayArrerDetails");
                    var result = periodDetails.Where(x => x.details.ToLower().Trim() == details.ToLower().Trim()).
                  Select(y => new { m = (y.fromperiod + " To " + y.toperiod), fromperiod = y.fromperiod, toperiod = y.toperiod }).FirstOrDefault();
                    return Json(new { DAArrerPeriods = result.m, fromPeriod = result.fromperiod, toPeriod = result.toperiod }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult _DAArrearForm()
        {
            log.Info($"ArrearApprovalRequest/_DAArreraForm");
            ArrearApprovalRequest arrearApproval = new ArrearApprovalRequest();
            try
            {
                arrearApproval.Employees = ddlService.GetAllEmployee();
                arrearApproval.Branchs = ddlService.ddlBranchList();
                var result = adjustOldLoanService.GetArrearPeriodsDetails("AD");
                TempData["DAArrerDetails"] = result;
                arrearApproval.listArrerPeriod = result.Select(x => new SelectListModel { id = 0, value = x.details }).ToList();
                arrearApproval.EmployeeTypes = ddlService.ddlEmployeeTypeList();
                arrearApproval.enumEmpDACategory = EnumEmpDACategory.AllEmployees;
                arrearApproval.enumDABranch = EnumDABranch.BranchesExcecptHO;
                arrearApproval.AllEmployees = true;
                arrearApproval.BranchesExcecptHO = true;
                return PartialView("_DAArrear", arrearApproval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _DAArrearForm(ArrearApprovalRequest request)
        {
            log.Info($"ArrearApprovalRequest/_DAArrearForm");

            try
            {
                bool flag = false; bool requestExist = false;
                request.Employees = ddlService.GetAllEmployee();
                request.Branchs = ddlService.ddlBranchList();
                request.EmployeeTypes = ddlService.ddlEmployeeTypeList();

                var result = adjustOldLoanService.GetArrearPeriodsDetails("AD");
                TempData["DAArrerDetails"] = result;
                request.listArrerPeriod = result.Select(x => new SelectListModel { id = 0, value = x.details }).ToList();
                ModelState.Remove("selectedPayEmpTypeID");
                ModelState.Remove("selectedEmployeeTypeID");
                if (request.enumEmpDACategory == EnumEmpDACategory.SingleEmployee)
                {
                    ModelState.Remove("selectedDAEmpTypeID");
                    if (!request.selectedDAEmpID.HasValue)
                        ModelState.AddModelError("EmployeeIDRequired", "Please Select Employee.");
                }
                if (request.enumDABranch == EnumDABranch.SingleBranch)
                {
                    if (!request.selectedDABranchID.HasValue)
                        ModelState.AddModelError("BranchIDRequired", "Please Select Branch.");
                }
                if (request.ArrerPeriodDetailsDA == "Select")
                    ModelState.AddModelError("DAArrerPeriodRequired", "Please Select Arrer Period ");

                if (ModelState.IsValid)
                {
                    var yy = result.Where(x =>
                  x.details.ToLower().Trim() == request.ArrerPeriodDetailsDA.ToLower().Trim()).
                  Select(y => new { fromperiod = y.fromperiod, toperiod = y.toperiod }).FirstOrDefault();


                    var month = request.ArrerPeriodDetailsDA.Split('-')[0];
                    var year = request.ArrerPeriodDetailsDA.Split('-')[1];
                    var monthNumber = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;


                    #region === Get From Period ===========

                    var f_month = yy.fromperiod.Split(',')[0];
                    var f_year = yy.fromperiod.Split(',')[1];
                    var f_monthNumber = DateTime.ParseExact(f_month, "MMMM", CultureInfo.CurrentCulture).Month;
                    var fromPeriod = $"{f_year}{f_monthNumber.ToString("00")}";
                    request.PeriodForm = int.Parse(fromPeriod);
                    #endregion


                    #region === Get To Period ===========

                    var t_month = yy.toperiod.Split(',')[0];
                    var t_year = yy.toperiod.Split(',')[1];
                    var t_monthNumber = DateTime.ParseExact(t_month, "MMMM", CultureInfo.CurrentCulture).Month;
                    var toPeriod = $"{t_year}{t_monthNumber.ToString("00")}";
                    request.PeriodTo = int.Parse(toPeriod);
                    #endregion

                    request.salMonth = monthNumber;
                    request.salYear = Convert.ToInt32(year);
                    request.ProcessID = (int)WorkFlowProcess.DAArrearGenerate;
                    request.CreatedBy = userDetail.UserID;
                    request.CreatedOn = DateTime.Now;
                    flag = pAppSettingService.SendDAArrearApprovalRequest(request, out requestExist);
                    request.IsUpdateDone = flag;
                    request.requestExist = requestExist;
                    return PartialView("_DAArrear", request);
                }
                else
                {
                    return PartialView("_DAArrear", request);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _PayArrearForm()
        {
            log.Info($"ArrearApprovalRequest/_PayArrearForm");
            ArrearApprovalRequest arrearApproval = new ArrearApprovalRequest();
            try
            {
                arrearApproval.Employees = ddlService.GetAllEmployee();
                arrearApproval.Branchs = ddlService.ddlBranchList();
                var result = adjustOldLoanService.GetArrearPeriodsDetails("AB");
                TempData["PayArrerDetails"] = result;
                arrearApproval.listArrerPeriod = result.Select(x => new SelectListModel { id = 0, value = x.details }).ToList();
                arrearApproval.EmployeeTypes = ddlService.ddlEmployeeTypeList();
                arrearApproval.enumEmpPayCategory = EnumEmpPayCategory.AllEmployees;
                arrearApproval.enumPayBranch = EnumPayBranch.BranchesExcecptHO;
                arrearApproval.AllEmployees = true;
                arrearApproval.BranchesExcecptHO = true;
                return PartialView("_PayArrear", arrearApproval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PayArrearForm(ArrearApprovalRequest request)
        {
            log.Info($"ArrearApprovalRequest/_PayArrearForm");
            try
            {
                bool flag = false; bool requestExist = false;
                request.Employees = ddlService.GetAllEmployee();
                request.Branchs = ddlService.ddlBranchList();
                request.EmployeeTypes = ddlService.ddlEmployeeTypeList();

                var result = adjustOldLoanService.GetArrearPeriodsDetails("AB");
                TempData["PayArrerDetails"] = result;
                request.listArrerPeriod = result.Select(x => new SelectListModel { id = 0, value = x.details }).ToList();

                ModelState.Remove("selectedDAEmpTypeID");
                ModelState.Remove("selectedEmployeeTypeID");
                if (request.enumEmpPayCategory == EnumEmpPayCategory.SingleEmployee)
                {
                    ModelState.Remove("selectedPayEmpTypeID");
                    if (!request.selectedPayEmpID.HasValue)
                        ModelState.AddModelError("EmployeeIDRequired", "Please Select Employee.");
                }
                if (request.enumPayBranch == EnumPayBranch.SingleBranch)
                {
                    if (!request.selectedPayBranchID.HasValue)
                        ModelState.AddModelError("BranchIDRequired", "Please Select Branch.");
                }

                if (request.ArrerPeriodDetailsPay == "Select")
                    ModelState.AddModelError("PayArrerPeriodRequired", "Please Select Arrer Period ");

                if (ModelState.IsValid)
                {

                    var yy = result.Where(x =>
                    x.details.ToLower().Trim() == request.ArrerPeriodDetailsPay.ToLower().Trim()).
                    Select(y => new { fromperiod = y.fromperiod, toperiod = y.toperiod }).FirstOrDefault();

                    var month = request.ArrerPeriodDetailsPay.Split('-')[0];
                    var year = request.ArrerPeriodDetailsPay.Split('-')[1];
                    var monthNumber = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;


                    #region === Get From Period ===========

                    var f_month = yy.fromperiod.Split(',')[0];
                    var f_year = yy.fromperiod.Split(',')[1];
                    var f_monthNumber = DateTime.ParseExact(f_month, "MMMM", CultureInfo.CurrentCulture).Month;
                    var fromPeriod = $"{f_year}{f_monthNumber.ToString("00")}";
                    request.PeriodForm = int.Parse(fromPeriod);
                    #endregion


                    #region === Get To Period ===========

                    var t_month = yy.toperiod.Split(',')[0];
                    var t_year = yy.toperiod.Split(',')[1];
                    var t_monthNumber = DateTime.ParseExact(t_month, "MMMM", CultureInfo.CurrentCulture).Month;
                    var toPeriod = $"{t_year}{t_monthNumber.ToString("00")}";
                    request.PeriodTo = int.Parse(toPeriod);
                    #endregion


                    request.salMonth = monthNumber;
                    request.salYear = Convert.ToInt32(year);
                    request.ProcessID = (int)WorkFlowProcess.PayArrearGenerate;
                    request.CreatedBy = userDetail.UserID;
                    request.CreatedOn = DateTime.Now;

                    flag = pAppSettingService.SendPayArrearApprovalRequest(request, out requestExist);

                    request.IsUpdateDone = flag;
                    request.requestExist = requestExist;
                    return PartialView("_PayArrear", request);
                }
                else
                {
                    return PartialView("_PayArrear", request);
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