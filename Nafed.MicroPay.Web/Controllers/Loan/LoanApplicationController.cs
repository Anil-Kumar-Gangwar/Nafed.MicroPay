using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers.Loan
{
    public class LoanApplicationController : BaseController
    {
        private readonly ILoanApplicationService loanApplicationServ;
        private readonly IDropdownBindService ddlService;
        public LoanApplicationController(ILoanApplicationService loanApplicationServ, IDropdownBindService ddlService)
        {
            this.loanApplicationServ = loanApplicationServ;
            this.ddlService = ddlService;
        }
        // GET: LoanApplication
        public ActionResult Index()
        {
            LoanApplication loanApp = new LoanApplication();
            return View(loanApp);
        }

        [HttpGet]
        public ActionResult Create()
        {
            LoanApplication loanApp = new LoanApplication();
            var getEmployeeDtls = loanApplicationServ.GetEmployeeDtl((int)userDetail.EmployeeID);
            // var getLoanApplication = loanApplicationServ.GetLoanApplicationList((int)userDetail.EmployeeID);
            loanApp = getEmployeeDtls;
            var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.LoanApplication);
            loanApp.approvalSetting = approvalSettings ?? new EmployeeProcessApproval();

            //loanApp.EmployeeName = getEmployeeDtls.EmployeeName;
            //loanApp.EmployeeCode = getEmployeeDtls.EmployeeCode;
            //loanApp.Dateofjoining = getEmployeeDtls.Dateofjoining;
            //loanApp.PFNo = getEmployeeDtls.PFNo;
            //loanApp.EmployeeID = getEmployeeDtls.EmployeeID;
            return View("ApplicationForm", loanApp);
        }

        [HttpPost]
        public ActionResult Create(LoanApplication loanApp)
        {
            log.Info($"LoanApplicationController/Create");
            try
            {
                if (loanApp.approvalSetting.ReportingTo == 0)
                {
                    TempData["Error"] = "You can not apply for Loan right now because your Reporting Manager is not set.";
                    return View("ApplicationForm", loanApp);
                }

                if (ModelState.IsValid)
                {
                    ProcessWorkFlow workFlow = null;

                    workFlow = new ProcessWorkFlow()
                    {
                        SenderID = userDetail.EmployeeID,
                        ReceiverID = loanApp.approvalSetting.ReportingTo,
                        SenderDepartmentID = userDetail.DepartmentID,
                        SenderDesignationID = userDetail.DesignationID,
                        CreatedBy = userDetail.UserID,
                        EmployeeID = loanApp.EmployeeID,
                        Scomments = loanApp.Remarks,
                        ProcessID = (int)WorkFlowProcess.LoanApplication,
                        StatusID = (int)LoanApplicationStatus.Pending
                    };
                    loanApp._ProcessWorkFlow = workFlow;

                    loanApp.CreatedBy = userDetail.UserID;
                    loanApp.CreatedOn = DateTime.Now;
                    loanApp.StatusID = (int)LoanApplicationStatus.Pending;
                    bool result = loanApplicationServ.InsertUpdateLoanApplication(loanApp);
                    if (result)
                    {
                        TempData["Message"] = "Loan Application Submitted Successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = "Problem while submitting Loan Application - Please contact your application administrator.";
                        return View("ApplicationForm", loanApp);
                    }
                }
                else
                    return View("ApplicationForm", loanApp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetLoanApplicationList()
        {
            log.Info($"LoanApplicationController/GetLoanApplicationList");
            try
            {
                List<LoanApplication> loanApp = new List<LoanApplication>();
                var getLoanApplication = loanApplicationServ.GetLoanApplicationList((int)userDetail.EmployeeID, null);
                loanApp = getLoanApplication;
                return PartialView("_LoanApplicationGridView", loanApp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult Edit(int EmpID, int? AppNo)
        {
            log.Info($"LoanApplicationController/Edit/EmpID={EmpID}/AppNo={AppNo}");
            try
            {
                LoanApplication loanApp = new LoanApplication();
                var getEmployeeDtls = loanApplicationServ.GetEmployeeDtl((int)userDetail.EmployeeID);
                var getLoanApplication = loanApplicationServ.GetLoanApplicationList(EmpID, AppNo).FirstOrDefault();
                loanApp = getLoanApplication;
                loanApp.EmployeeName = getEmployeeDtls.EmployeeName;
                loanApp.EmployeeCode = getEmployeeDtls.EmployeeCode;
                loanApp.Dateofjoining = getEmployeeDtls.Dateofjoining;
                loanApp.PFNo = getEmployeeDtls.PFNo;
                loanApp.EmployeeID = getEmployeeDtls.EmployeeID;
                return View("EditApplication", loanApp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(LoanApplication loanApp)
        {
            log.Info($"LoanApplicationController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    loanApp.UpdateBy = userDetail.UserID;
                    loanApp.UpdateOn = DateTime.Now;
                    loanApp.StatusID = (int)LoanApplicationStatus.Pending;
                    bool result = loanApplicationServ.InsertUpdateLoanApplication(loanApp);
                    if (result)
                    {
                        TempData["Message"] = "Loan Application Submitted Successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = "Problem while submitting Loan Application - Please contact your application administrator.";
                        return View("EditApplication", loanApp);
                    }
                }
                else
                    return View("EditApplication", loanApp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult LoanApplications()
        {
            log.Info("LoanApplicationController/LoanApplications");
            try
            {
                return View("FileContainer");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetLoanApplicationPendingList()
        {
            log.Info($"LoanApplicationController/GetLoanApplicationPendingList");
            try
            {
                List<LoanApplication> loanApp = new List<LoanApplication>();
                //  var getLoanApplication = loanApplicationServ.GetLoanApplicationDetails(null, (int)LoanApplicationStatus.Pending);
                //   loanApp = getLoanApplication;
                return PartialView("_LoanApplicationPendingList", loanApp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }


        [HttpGet]
        public ActionResult AcceptRejectApplication(int empID, int appNo, byte statusID)
        {
            log.Info($"LoanApplicationController/AcceptRejectApplication/empID={empID}/appNo={appNo}");
            try
            {
                LoanApplication loanApp = new LoanApplication();
                loanApp.EmployeeID = empID;
                loanApp.ApplicationID = appNo;
                loanApp.StatusID = statusID;
                return PartialView("_LoanAcceptRejectPopup", loanApp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        [HttpPost]
        public ActionResult AcceptRejectApplication(LoanApplication loanApp)
        {
            log.Info($"LoanApplicationController/AcceptRejectApplication");
            try
            {
                if (string.IsNullOrEmpty(loanApp.Remarks))
                {
                    ModelState.AddModelError("RemarkValidator", "Please Enter Remark.");
                    return PartialView("_LoanAcceptRejectPopup", loanApp);
                }
                else
                {
                    loanApp.CreatedBy = userDetail.UserID;
                    loanApp.CreatedOn = DateTime.Now;
                    bool result = loanApplicationServ.InsertLoanApplicationHistory(loanApp);
                    if (result)
                    {
                        var msg = loanApp.StatusID == 2 ? "Loan Application Accepted." : "Loan Application Rejected.";
                        return Json(new { status = 1, type = "success", msg = msg }, JsonRequestBehavior.AllowGet);
                    }
                }
                return PartialView("_LoanAcceptRejectPopup", loanApp);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public ActionResult GetLoanApplicationHistoryList()
        {
            log.Info($"LoanApplicationController/GetLoanApplicationHistoryList");
            try
            {
                List<LoanApplication> loanApp = new List<LoanApplication>();
                var getLoanApplication = loanApplicationServ.GetLoanApplicationDetails(null, null, null, null);
                loanApp = getLoanApplication;
                return PartialView("_LoanApplicationHistory", loanApp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult _LoanHistorySearch()
        {
            log.Info($"LoanApplicationController/_LoanHistorySearch");
            try
            {
                LoanApplicationFilter loanFilter = new LoanApplicationFilter();
                loanFilter.EmployeeList = ddlService.GetAllEmployee();
                return PartialView("_LoanApplicationFilters", loanFilter);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpPost]
        public ActionResult _LoanHistorySearch(LoanApplicationFilter loanFilter)
        {
            log.Info($"LoanApplicationController/_LoanHistorySearch");
            try
            {
                if ((loanFilter.FromDate.HasValue && !loanFilter.ToDate.HasValue) || (!loanFilter.FromDate.HasValue && loanFilter.ToDate.HasValue))
                {
                    ModelState.AddModelError("DateValidation", "Please select date.");
                    return PartialView("_LoanApplicationFilters", loanFilter);
                }

                List<LoanApplication> loanApp = new List<LoanApplication>();
                short? statusID = null;
                if (!loanFilter.ApplicationStatusID.HasValue)
                    statusID = null;
                else
                    statusID = (short)loanFilter.ApplicationStatusID;
                var getLoanApplication = loanApplicationServ.GetLoanApplicationDetails(loanFilter.EmployeeID, statusID, loanFilter.FromDate, loanFilter.ToDate);
                loanApp = getLoanApplication;

                loanFilter.EmployeeList = ddlService.GetAllEmployee();
                return PartialView("_LoanApplicationHistory", loanApp);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
    }
}