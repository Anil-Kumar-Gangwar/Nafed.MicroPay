using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services.Salary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Salary
{
    public class PublishSalaryController : BaseController
    {
        // GET: PublishSalary
        private readonly IDropdownBindService ddlService;
        private readonly IAdjustOldLoanService adjustOldLoanService;
        private readonly IPayrollApprovalSettingService pAppSettingService;
        private readonly ISalaryReportService salReportService;
        private readonly IBranchService branchService;

        public PublishSalaryController(IDropdownBindService ddlService,
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
            log.Info($"PublishSalary/Index");
            return View();
        }

        [HttpGet]
        public ActionResult _SalaryApprovalRequest()
        {
            log.Info($"PublishSalary/_SalaryApprovalRequest");
            try
            {
                var loggedInEmployeeID = userDetail.EmployeeID.Value;
                var approvalSetting = pAppSettingService.GetApprovalSetting();

                List<PayrollApprovalRequest> approvalRequests = new List<PayrollApprovalRequest>();

                if (!approvalSetting.Any(y => y.ProcessID == (int)WorkFlowProcess.SalaryGenerate
                && (y.Reporting2 == loggedInEmployeeID || y.Reporting3 == loggedInEmployeeID)))
                {
                    return Content(@"<div class='col-lg-12 col-md-12 col-sm-12 col-xs-12 font-weight-bold text-center alert alert-warning'>
                           You are not authorized to access this page.
                      </div>");
                }
                approvalRequests = pAppSettingService.GetSalaryApprovalRequests(WorkFlowProcess.SalaryGenerate, loggedInEmployeeID);

                var reportingList = approvalSetting.FirstOrDefault(x => x.ProcessID == (int)WorkFlowProcess.SalaryGenerate);
                if (reportingList != null)
                {
                    approvalRequests.ForEach(x =>
                    {
                        x.Reporting1 = reportingList.Reporting1;
                        x.Reporting2 = reportingList.Reporting2;
                        x.Reporting3 = reportingList.Reporting3;
                        x.BranchName = !x.BranchID.HasValue ? x.BranchCode : x.BranchName;
                    });
                }
                return PartialView("_PublishSalary", approvalRequests);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult _DAArrearApprovalRequest()
        {
            log.Info($"PublishSalary/_DAArrearApprovalRequest");
            try
            {
                var loggedInEmployeeID = userDetail.EmployeeID.Value;
                var approvalSetting = pAppSettingService.GetApprovalSetting();

                List<PayrollApprovalRequest> approvalRequests = new List<PayrollApprovalRequest>();

                if (!approvalSetting.Any(y => y.ProcessID == (int)WorkFlowProcess.DAArrearGenerate
                && (y.Reporting2 == loggedInEmployeeID || y.Reporting3 == loggedInEmployeeID)))
                {
                    return Content(@"<div class='col-lg-12  col-md-12 col-sm-12 col-xs-12 font-weight-bold text-center alert alert-warning'>
                           You are not authorized to access this page.
                      </div>");
                }
                approvalRequests = pAppSettingService.GetDAArrearApprovalRequest(WorkFlowProcess.DAArrearGenerate, loggedInEmployeeID);

                var reportingList = approvalSetting.FirstOrDefault(x => x.ProcessID == (int)WorkFlowProcess.DAArrearGenerate);
                if (reportingList != null)
                {
                    approvalRequests.ForEach(x =>
                    {
                        x.Reporting1 = reportingList.Reporting1;
                        x.Reporting2 = reportingList.Reporting2;
                        x.Reporting3 = reportingList.Reporting3;
                        x.BranchName = !x.BranchID.HasValue ? x.BranchCode : x.BranchName;
                    });
                }
                return PartialView("_PublishDAArrear", approvalRequests);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult _PayArrearApprovalRequest()
        {
            log.Info($"PublishSalary/_PayArrearApprovalRequest");
            try
            {
                var loggedInEmployeeID = userDetail.EmployeeID.Value;
                var approvalSetting = pAppSettingService.GetApprovalSetting();

                List<PayrollApprovalRequest> approvalRequests = new List<PayrollApprovalRequest>();

                if (!approvalSetting.Any(y => y.ProcessID == (int)WorkFlowProcess.PayArrearGenerate
                && (y.Reporting2 == loggedInEmployeeID || y.Reporting3 == loggedInEmployeeID)))
                {
                    return Content(@"<div class='col-lg-12 col-md-12 col-sm-12 col-xs-12 font-weight-bold text-center alert alert-warning'>
                           You are not authorized to access this page.
                      </div>");
                }
                approvalRequests = pAppSettingService.GetDAArrearApprovalRequest(WorkFlowProcess.PayArrearGenerate, loggedInEmployeeID);

                var reportingList = approvalSetting.FirstOrDefault(x => x.ProcessID == (int)WorkFlowProcess.PayArrearGenerate);
                if (reportingList != null)
                {
                    approvalRequests.ForEach(x =>
                    {
                        x.Reporting1 = reportingList.Reporting1;
                        x.Reporting2 = reportingList.Reporting2;
                        x.Reporting3 = reportingList.Reporting3;
                        x.BranchName = !x.BranchID.HasValue ? x.BranchCode : x.BranchName;
                    });
                }
                return PartialView("_PublishPayArrear", approvalRequests);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult _PublishSalary(PublishSalaryFilters publishSalary, string ButtonType)
        {
            log.Info($"PublishSalary/_PublishSalary");
            try
            {
                bool flag = false;
                publishSalary.Employees = ddlService.GetAllEmployee();
                publishSalary.Branchs = ddlService.ddlBranchList();
                publishSalary.EmployeeTypes = ddlService.ddlEmployeeTypeList();
                ModelState.Remove("selectedDAEmpTypeID");
                ModelState.Remove("selectedPayEmpTypeID");
                if (publishSalary.enumEmpCategory == EnumEmpCategory.SingleEmployee)
                {
                    ModelState.Remove("selectedEmployeeTypeID");
                    if (!publishSalary.selectedEmployeeID.HasValue)
                        ModelState.AddModelError("EmployeeIDRequired", "Please Select Employee.");
                }
                if (publishSalary.enumBranch == EnumBranch.SingleBranch)
                {
                    if (!publishSalary.selectedBranchID.HasValue)
                        ModelState.AddModelError("BranchIDRequired", "Please Select Branch.");
                }

                if (ModelState.IsValid)
                {
                    flag = adjustOldLoanService.UpdateSalaryPublishField(publishSalary, ButtonType);
                    publishSalary.IsUpdateDone = flag;
                    return PartialView("_PublishSalary", publishSalary);
                }
                else
                {
                    return PartialView("_PublishSalary", publishSalary);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _PublishDAArrerForm()
        {
            log.Info($"PublishSalary/_PublishDAArrerForm");
            PublishSalaryFilters publishSalary = new PublishSalaryFilters();
            try
            {
                publishSalary.Employees = ddlService.GetAllEmployee();
                publishSalary.Branchs = ddlService.ddlBranchList();
                var result = adjustOldLoanService.GetArrearPeriodsDetails("AD");
                TempData["DAArrerDetails"] = result;
                publishSalary.listArrerPeriod = result.Select(x => new SelectListModel { id = 0, value = x.details }).ToList();
                publishSalary.EmployeeTypes = ddlService.ddlEmployeeTypeList();
                publishSalary.enumEmpDACategory = EnumEmpDACategory.AllEmployees;
                publishSalary.enumDABranch = EnumDABranch.BranchesExcecptHO;
                publishSalary.AllEmployees = true;
                publishSalary.BranchesExcecptHO = true;
                return PartialView("_PublishDAArrer", publishSalary);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult _PublishDAArrer(PublishSalaryFilters publishSalary, string ButtonType)
        {
            log.Info($"PublishSalary/_PublishDAArrer");
            try
            {
                bool flag = false;
                publishSalary.Employees = ddlService.GetAllEmployee();
                publishSalary.Branchs = ddlService.ddlBranchList();
                publishSalary.EmployeeTypes = ddlService.ddlEmployeeTypeList();

                var result = adjustOldLoanService.GetArrearPeriodsDetails("AD");
                TempData["DAArrerDetails"] = result;
                publishSalary.listArrerPeriod = result.Select(x => new SelectListModel { id = 0, value = x.details }).ToList();
                ModelState.Remove("selectedPayEmpTypeID");
                ModelState.Remove("selectedEmployeeTypeID");
                if (publishSalary.enumEmpDACategory == EnumEmpDACategory.SingleEmployee)
                {
                    ModelState.Remove("selectedDAEmpTypeID");
                    if (!publishSalary.selectedDAEmpID.HasValue)
                        ModelState.AddModelError("EmployeeIDRequired", "Please Select Employee.");
                }
                if (publishSalary.enumDABranch == EnumDABranch.SingleBranch)
                {
                    if (!publishSalary.selectedDABranchID.HasValue)
                        ModelState.AddModelError("BranchIDRequired", "Please Select Branch.");
                }
                if (publishSalary.ArrerPeriodDetailsDA == "Select")
                    ModelState.AddModelError("DAArrerPeriodRequired", "Please Select Arrer Period ");


                if (ModelState.IsValid)
                {
                    var month = publishSalary.ArrerPeriodDetailsDA.Split('-')[0];
                    var year = publishSalary.ArrerPeriodDetailsDA.Split('-')[1];
                    var monthNumber = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;
                    publishSalary.salMonth = monthNumber;
                    publishSalary.salYear = Convert.ToInt32(year);
                    flag = adjustOldLoanService.UpdatePublishDAArrer(publishSalary, ButtonType);
                    publishSalary.IsUpdateDone = flag;
                    return PartialView("_PublishDAArrer", publishSalary);
                }
                else
                {
                    return PartialView("_PublishDAArrer", publishSalary);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _PublishPayArrerForm()
        {
            log.Info($"PublishSalary/_PublishPayArrerForm");
            PublishSalaryFilters publishSalary = new PublishSalaryFilters();
            try
            {
                publishSalary.Employees = ddlService.GetAllEmployee();
                publishSalary.Branchs = ddlService.ddlBranchList();
                var result = adjustOldLoanService.GetArrearPeriodsDetails("AB");
                TempData["PayArrerDetails"] = result;
                publishSalary.listArrerPeriod = result.Select(x => new SelectListModel { id = 0, value = x.details }).ToList();
                publishSalary.EmployeeTypes = ddlService.ddlEmployeeTypeList();
                publishSalary.enumEmpPayCategory = EnumEmpPayCategory.AllEmployees;
                publishSalary.enumPayBranch = EnumPayBranch.BranchesExcecptHO;
                publishSalary.AllEmployees = true;
                publishSalary.BranchesExcecptHO = true;
                return PartialView("_PublishPayArrer", publishSalary);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PublishPayArrer(PublishSalaryFilters publishSalary, string ButtonType)
        {
            log.Info($"PublishSalary/_PublishPayArrer");
            try
            {
                bool flag = false;
                publishSalary.Employees = ddlService.GetAllEmployee();
                publishSalary.Branchs = ddlService.ddlBranchList();
                publishSalary.EmployeeTypes = ddlService.ddlEmployeeTypeList();

                var result = adjustOldLoanService.GetArrearPeriodsDetails("AB");
                TempData["PayArrerDetails"] = result;
                publishSalary.listArrerPeriod = result.Select(x => new SelectListModel { id = 0, value = x.details }).ToList();

                ModelState.Remove("selectedDAEmpTypeID");
                ModelState.Remove("selectedEmployeeTypeID");
                if (publishSalary.enumEmpPayCategory == EnumEmpPayCategory.SingleEmployee)
                {
                    ModelState.Remove("selectedPayEmpTypeID");
                    if (!publishSalary.selectedPayEmpID.HasValue)
                        ModelState.AddModelError("EmployeeIDRequired", "Please Select Employee.");
                }
                if (publishSalary.enumPayBranch == EnumPayBranch.SingleBranch)
                {
                    if (!publishSalary.selectedPayBranchID.HasValue)
                        ModelState.AddModelError("BranchIDRequired", "Please Select Branch.");
                }

                if (publishSalary.ArrerPeriodDetailsPay == "Select")
                    ModelState.AddModelError("PayArrerPeriodRequired", "Please Select Arrer Period ");

                if (ModelState.IsValid)
                {
                    var month = publishSalary.ArrerPeriodDetailsPay.Split('-')[0];
                    var year = publishSalary.ArrerPeriodDetailsPay.Split('-')[1];
                    var monthNumber = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;
                    publishSalary.salMonth = monthNumber;
                    publishSalary.salYear = Convert.ToInt32(year);
                    flag = adjustOldLoanService.UpdatePublishPayArrer(publishSalary, ButtonType);
                    publishSalary.IsUpdateDone = flag;
                    return PartialView("_PublishPayArrer", publishSalary);
                }
                else
                {
                    return PartialView("_PublishPayArrer", publishSalary);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetPeriodDetails(string details, string flag)
        {
            log.Info($"PublishSalary/GetPeriodDetails");
            try
            {
                if (flag == "AD")
                {
                    var periodDetails = (List<ArrerPeriodDetails>)TempData["DAArrerDetails"];
                    TempData.Keep("DAArrerDetails");
                    var result = periodDetails.Where(x => x.details.ToLower().Trim() == details.ToLower().Trim()).
                        Select(y => (y.fromperiod + " To " + y.toperiod)).FirstOrDefault();
                    return Json(new { DAArrerPeriods = result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var periodDetails = (List<ArrerPeriodDetails>)TempData["PayArrerDetails"];
                    TempData.Keep("PayArrerDetails");
                    var result = periodDetails.Where(x => x.details.ToLower().Trim() == details.ToLower().Trim()).
                        Select(y => (y.fromperiod + " To " + y.toperiod)).FirstOrDefault();
                    return Json(new { DAArrerPeriods = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostRequestRow(PayrollApprovalRequest request, FormCollection frm)
        {
            log.Info($"PublishSalary/_PostRequestRow");
            try
            {

                var hiddenFieldKeyName = "hdnAction_" + request.sno.ToString();

                var actionType = frm.Get(hiddenFieldKeyName);
                var loggedInEmpID = userDetail.EmployeeID;

                request.UpdatedBy = userDetail.UserID;
                request.UpdatedOn = DateTime.Now;

                if (actionType == "Approve")
                {
                    if (loggedInEmpID == request.Reporting2 && request.Reporting2 != request.Reporting3)
                        request.Status = (int)ApprovalStatus.ApprovedByReporting2;

                    else if (loggedInEmpID == request.Reporting2 && loggedInEmpID==request.Reporting3)
                        request.Status = (int)ApprovalStatus.ApprovedByReporting3;

                    else if (loggedInEmpID == request.Reporting3)
                    {
                        request.Status = (int)ApprovalStatus.ApprovedByReporting3;
                    }
                    else if (loggedInEmpID == request.Reporting2 && loggedInEmpID == request.Reporting3)
                    {

                    }
                    var result = pAppSettingService.SubmitApprovalRequest(request);

                    if (request.Status == (int)ApprovalStatus.ApprovedByReporting3)
                        return Json(new { msgType = "success", msg = "Salary Published Successfully." }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { msgType = "success", msg = "Request approved Successfully." }, JsonRequestBehavior.AllowGet);

                }
                else if (actionType == "Reject")
                {
                    if (loggedInEmpID == request.Reporting2)
                        request.Status = (int)ApprovalStatus.RejectedByReporting2;

                    else if (loggedInEmpID == request.Reporting3)
                    {
                        request.Status = (int)ApprovalStatus.RejectedByReporting3;
                    }
                    else if (loggedInEmpID == request.Reporting2 && loggedInEmpID == request.Reporting3)
                    {

                    }
                    var result = pAppSettingService.SubmitApprovalRequest(request);
                    return Json(new { msgType = "success", msg = "Request rejected Successfully." }, JsonRequestBehavior.AllowGet);
                }
                return PartialView("_ApprovalRequestRow", request);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult _PostDAArrerRequestRow(PayrollApprovalRequest request, FormCollection frm)
        {
            log.Info($"PublishSalary/_PostDAArrerRequestRow");
            try
            {
                var hiddenFieldKeyName = "hdnAction_DAArrear_" + request.sno.ToString();

                var actionType = frm.Get(hiddenFieldKeyName);
                var loggedInEmpID = userDetail.EmployeeID;

                request.UpdatedBy = userDetail.UserID;
                request.UpdatedOn = DateTime.Now;

                if (actionType == "Approve")
                {
                    if (loggedInEmpID == request.Reporting2)
                        request.Status = (int)ApprovalStatus.ApprovedByReporting2;

                    else if (loggedInEmpID == request.Reporting3)
                    {
                        request.Status = (int)ApprovalStatus.ApprovedByReporting3;
                    }
                    else if (loggedInEmpID == request.Reporting2 && loggedInEmpID == request.Reporting3)
                    {

                    }
                    var result = pAppSettingService.SubmitDAApprovalRequest(request);

                    if (request.Status == (int)ApprovalStatus.ApprovedByReporting3)
                        return Json(new { msgType = "success", msg = "DA Arrear Published Successfully." }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { msgType = "success", msg = "Request approved Successfully." }, JsonRequestBehavior.AllowGet);

                }
                else if (actionType == "Reject")
                {
                    if (loggedInEmpID == request.Reporting2)
                        request.Status = (int)ApprovalStatus.RejectedByReporting2;

                    else if (loggedInEmpID == request.Reporting3)
                    {
                        request.Status = (int)ApprovalStatus.RejectedByReporting3;
                    }
                    else if (loggedInEmpID == request.Reporting2 && loggedInEmpID == request.Reporting3)
                    {

                    }
                    var result = pAppSettingService.SubmitDAApprovalRequest(request);

                    return Json(new { msgType = "success", msg = "Request rejected Successfully." }, JsonRequestBehavior.AllowGet);
                }
                return PartialView("_DAArrerApprovalRequestRow", request);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }


        [HttpPost]
        public ActionResult _PostPayArrearRequestRow(PayrollApprovalRequest request, FormCollection frm)
        {
            log.Info($"PublishSalary/_PostPayArrearRequestRow");
            try
            {
                var hiddenFieldKeyName = "hdnAction_PayArrear_" + request.sno.ToString();

                var actionType = frm.Get(hiddenFieldKeyName);
                var loggedInEmpID = userDetail.EmployeeID;

                request.UpdatedBy = userDetail.UserID;
                request.UpdatedOn = DateTime.Now;

                if (actionType == "Approve")
                {
                    if (loggedInEmpID == request.Reporting2)
                        request.Status = (int)ApprovalStatus.ApprovedByReporting2;

                    else if (loggedInEmpID == request.Reporting3)
                    {
                        request.Status = (int)ApprovalStatus.ApprovedByReporting3;
                    }
                    else if (loggedInEmpID == request.Reporting2 && loggedInEmpID == request.Reporting3)
                    {

                    }
                    var result = pAppSettingService.SubmitPayArrearApprovalRequest(request);

                    if (request.Status == (int)ApprovalStatus.ApprovedByReporting3)
                        return Json(new { msgType = "success", msg = "Pay Arrear Published Successfully." }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { msgType = "success", msg = "Request approved Successfully." }, JsonRequestBehavior.AllowGet);

                }
                else if (actionType == "Reject")
                {
                    if (loggedInEmpID == request.Reporting2)
                        request.Status = (int)ApprovalStatus.RejectedByReporting2;

                    else if (loggedInEmpID == request.Reporting3)
                    {
                        request.Status = (int)ApprovalStatus.RejectedByReporting3;
                    }
                    else if (loggedInEmpID == request.Reporting2 && loggedInEmpID == request.Reporting3)
                    {

                    }
                    var result = pAppSettingService.SubmitPayArrearApprovalRequest(request);

                    return Json(new { msgType = "success", msg = "Request rejected Successfully." }, JsonRequestBehavior.AllowGet);
                }
                return PartialView("_DAArrerApprovalRequestRow", request);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region   ===/// Show PaySummary & Pay Slip Reports ==========

        public ActionResult _GetSalaryReport(string type, int? branchID, int empTypeID, int salMonth, int salYear)
        {
            log.Info($"PublishSalary/_GetSalaryReport/type:{type}&empTypeID:{empTypeID}&salMonth:{salMonth}&salYear:{salYear}");
            try
            {
                string branchName = string.Empty, rptName = string.Empty;
                var branchList = ddlService.ddlBranchList(null, null);

                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();
                reportModel.reportType = 2;

                if (type.Equals("summary", StringComparison.OrdinalIgnoreCase))
                {
                    if (branchID.HasValue)
                    {
                        branchName = branchList.Where(x => x.id == branchID).FirstOrDefault().value;
                        parameterList.Add(new ReportParameter { name = "BranchID", value = branchID.Value });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = salMonth });
                        parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(salYear) });
                        parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = empTypeID });
                        parameterList.Add(new ReportParameter { name = "Emps", value = 0 });
                        parameterList.Add(new ReportParameter { name = "Br", value = branchName });
                        parameterList.Add(new ReportParameter { name = "flag", value = " " });

                        rptName = "PaySummary.rpt";
                    }
                    else
                    {
                        parameterList.Add(new ReportParameter { name = "BranchID", value = 0 });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = salMonth });
                        parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(salYear) });
                        parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = empTypeID });
                        parameterList.Add(new ReportParameter { name = "Emps", value = 0 });
                        parameterList.Add(new ReportParameter { name = "Br", value = " " });
                        parameterList.Add(new ReportParameter { name = "flag", value = "P" });
                        rptName = "PaySummary.rpt";
                    }
                }
                else if (type.Equals("detail", StringComparison.OrdinalIgnoreCase))
                {
                    rptName = "PaySlipBR.rpt";

                    parameterList.Add(new ReportParameter { name = "SalMonth", value = salMonth });
                    parameterList.Add(new ReportParameter { name = "SalYear", value = Convert.ToInt16(salYear) });
                    parameterList.Add(new ReportParameter { name = "EmployeeTypeID", value = empTypeID });
                    parameterList.Add(new ReportParameter { name = "EMPID", value = 0 });

                    if (branchID.HasValue)
                    {
                        parameterList.Add(new ReportParameter { name = "BranchID", value = branchID.Value });

                        if (branchID.Value == 44)
                            rptName = "PaySlipHO.rpt";
                    }
                    else
                        parameterList.Add(new ReportParameter { name = "BranchID", value = 0 });
                }

                reportModel.reportParameters = parameterList;
                reportModel.rptName = rptName;
                Session["ReportModel"] = reportModel;
                return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion


        #region ===== Export Monthly Pay Summary & Pay Slip..  ================

        public ActionResult _ExportMonthlyPaySummary(int? branchID, int empTypeID, int salMonth, int salYear)
        {
            log.Info($"PublishSalary/_ExportMonthlyPaySummary/empTypeID:{empTypeID}&salMonth:{salMonth}&salYear:{salYear}");

            string fullPath = Server.MapPath("~/FileDownload/");
            string fileName = string.Empty, msg = string.Empty, result = string.Empty;

            SalaryReportFilter reportFilter = new SalaryReportFilter();

            fileName = ExtensionMethods.SetUniqueFileName($"Monthly Pay Summary-", FileExtension.xlsx);
            reportFilter.fileName = fileName;
            reportFilter.salMonth = (byte)salMonth;
            reportFilter.salYear = (short)salYear;
            reportFilter.filePath = fullPath;
            reportFilter.employeeTypeID = empTypeID;
            reportFilter.branchID = branchID;

            result = salReportService.GetMonthlyBranchWiseReport(reportFilter);

            if (result == "notfound")
                ModelState.AddModelError("ReportTypeRequired", "No Record Found.");
            else
                return Json(new
                {
                    fileName = fileName,
                    fullPath = fullPath + fileName,
                    message = "success"
                }, JsonRequestBehavior.AllowGet);

            return Content("");
        }


        public ActionResult _ExportMonthlyPaySlip(int? branchID, int empTypeID, int salMonth, int salYear)
        {
            log.Info($"PublishSalary/_ExportMonthlyPaySlip/empTypeID:{empTypeID}&salMonth:{salMonth}&salYear:{salYear}");

            string fullPath = Server.MapPath("~/FileDownload/");
            string fileName = string.Empty, msg = string.Empty, result = string.Empty;

            SalaryReportFilter reportFilter = new SalaryReportFilter();

            fileName = ExtensionMethods.SetUniqueFileName($"Monthly Pay Slip-", FileExtension.xlsx);
            reportFilter.fileName = fileName;
            reportFilter.salMonth = (byte)salMonth;
            reportFilter.salYear = (short)salYear;
            reportFilter.filePath = fullPath;
            reportFilter.employeeTypeID = empTypeID;
            reportFilter.branchID = branchID;

            if (branchID.HasValue)
                reportFilter.BranchName = branchService.GetBranchByID(branchID.Value).BranchName;

            else
                reportFilter.BranchName = "All Branches (Except HO)";

              result = salReportService.GetMonthlyEmployeeWisePaySlip(reportFilter);
           // result = salReportService.GetBranchEmployeeWisePaySlip(reportFilter);
            
            if (result == "notfound")
                ModelState.AddModelError("ReportTypeRequired", "No Record Found.");
            else
                return Json(new
                {
                    fileName = fileName,
                    fullPath = fullPath + fileName,
                    message = "success"
                }, JsonRequestBehavior.AllowGet);

            return Content("");
        }
        #endregion


        #region ======= Export DA Arrear Report ============

        [HttpGet]
        public ActionResult _GetDAArrearReport(int periodFrom, int periodTo, int? branchID, int? employeeID, int empTypeID)
        {
            log.Info($"PublishSalary/_GetDAArrearReport/periodFrom:{periodFrom}&periodTo:{periodTo}&branchID:{branchID}&employeeID:{employeeID}&empTypeID:{empTypeID}");
            try
            {
                ArrearReportFilter rFilter = new ArrearReportFilter();
                string fullPath = Server.MapPath("~/FileDownload/");
                string fileName = string.Empty, msg = string.Empty, result = string.Empty;

                fileName = ExtensionMethods.SetUniqueFileName($"DA Arrear Report-", FileExtension.xlsx);
                rFilter.fileName = fileName; rFilter.arrearType = 'D';
                rFilter.filePath = fullPath;
                rFilter.employeeTypeID = empTypeID;
                rFilter.branchID = branchID??0;
                rFilter.pFrom = periodFrom;
                rFilter.pTo = periodTo;
                rFilter.employeeID = employeeID??0;

                result = pAppSettingService.GetArrearReport(rFilter);

                if (result == "notfound")
                    ModelState.AddModelError("ReportTypeRequired", "No Record Found.");
                else
                    return Json(new
                    {
                        fileName = fileName,
                        fullPath = fullPath + fileName,
                        message = "success"
                    }, JsonRequestBehavior.AllowGet);

                return Content("");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion


        #region ======= Export PAy Arrear Report ===========

        [HttpGet]
        public ActionResult _GetPayArrearReport(int periodFrom, int periodTo, int? branchID, int? employeeID, int empTypeID)
        {
            log.Info($"PublishSalary/_GetPayArrearReport/periodFrom:{periodFrom}&periodTo:{periodTo}&branchID:{branchID}&employeeID:{employeeID}&empTypeID:{empTypeID}");
            try
            {
                ArrearReportFilter rFilter = new ArrearReportFilter();
                string fullPath = Server.MapPath("~/FileDownload/");
                string fileName = string.Empty, msg = string.Empty, result = string.Empty;

                fileName = ExtensionMethods.SetUniqueFileName($"Pay Arrear Report-", FileExtension.xlsx);
                rFilter.fileName = fileName; rFilter.arrearType = 'B';
                rFilter.filePath = fullPath;
                rFilter.employeeTypeID = empTypeID;
                rFilter.branchID = branchID ?? 0;
                rFilter.pFrom = periodFrom;
                rFilter.pTo = periodTo;
                rFilter.employeeID = employeeID ?? 0;
                result = pAppSettingService.GetArrearReport(rFilter);
                if (result == "notfound")
                    ModelState.AddModelError("ReportTypeRequired", "No Record Found.");
                else
                    return Json(new
                    {
                        fileName = fileName,
                        fullPath = fullPath + fileName,
                        message = "success"
                    }, JsonRequestBehavior.AllowGet);

                return Content("");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        #endregion
    }
}