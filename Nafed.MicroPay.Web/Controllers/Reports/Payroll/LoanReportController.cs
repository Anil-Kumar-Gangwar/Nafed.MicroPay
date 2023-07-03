
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;

namespace MicroPay.Web.Controllers.Reports.Payroll
{
    public class LoanReportController : BaseController
    {
        private readonly IDropdownBindService dropDownService;
        private readonly IBranchService branchService; 

        public LoanReportController(IDropdownBindService dropDownService, IBranchService branchService)
        {
            this.branchService = branchService;
            this.dropDownService = dropDownService;
        }
        // GET: LoanReport
        public ActionResult Index()
        {
            LoanReportFilterVM loanReportVM = new LoanReportFilterVM();
            return View(loanReportVM);
        }

        [HttpGet]
        public ActionResult _LoanReportFilters()
        {
            log.Info($"LoanReportController/_LoanReportFilters");

            try
            {
                LoanReportFilterVM loanReportVM = new LoanReportFilterVM();
                loanReportVM.branchList = dropDownService.ddlBranchList();
                loanReportVM.employeeTypeList = dropDownService.ddlEmployeeTypeList();
                loanReportVM.AllBranch = false;

                loanReportVM.employeeTypeID = 5;
                loanReportVM.yearID = DateTime.Now.Year;
                loanReportVM.monthID = DateTime.Now.Month;
                return PartialView("_ReportFilters", loanReportVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult _LoanReportFilters(LoanReportFilterVM loanRptVM)
        {
            if (!loanRptVM.AllBranch && !loanRptVM.branchID.HasValue)
                ModelState.AddModelError("BranchIDRequired", "Please Select Branch.");
            else
                ModelState.Remove("BranchIDRequired");

            if (ModelState.IsValid)
            {
                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();
                reportModel.reportType = 2;

                var month = loanRptVM.monthID.ToString("00");
                var year = loanRptVM.yearID.ToString();
                string branchCode = string.Empty, rptName = string.Empty;

                if (!loanRptVM.AllBranch)
                    branchCode = branchService.GetBranchByID(loanRptVM.branchID.Value).BranchCode;

                if (loanRptVM.employeeTypeID == 5)
                {
                    if (loanRptVM.loanReportOptions == LoanReportOptions.LoanFinish)
                    {
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        parameterList.Add(new ReportParameter { name = "SalYear", value = year });
                        parameterList.Add(new ReportParameter { name = "month", value = month });

                        rptName = "Loanfinish.rpt";
                        parameterList.Add(new ReportParameter { name = "chk", value = "L" });
                        if (!loanRptVM.AllBranch)
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                        else
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = null });
                    }
                    else if (loanRptVM.loanReportOptions == LoanReportOptions.FestivalLoan)
                    {
                        parameterList.Add(new ReportParameter { name = "SalYear", value = loanRptVM.yearID });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        if (!loanRptVM.AllBranch)
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                        else
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = null });
                        rptName = "FestivalLoan.rpt";
                    }

                    else if(loanRptVM.loanReportOptions==LoanReportOptions.ScooterLoan)
                    {
                        parameterList.Add(new ReportParameter { name = "SalYear", value = loanRptVM.yearID });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        if (!loanRptVM.AllBranch)
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                        else
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = null });
                        rptName = "ScooterLoan.rpt";

                    }
                    else if (loanRptVM.loanReportOptions == LoanReportOptions.CarLoan)
                    {
                        parameterList.Add(new ReportParameter { name = "SalYear", value = loanRptVM.yearID });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        if (!loanRptVM.AllBranch)
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                        else
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = null });
                        rptName = "CarLoan.rpt";

                    }
                    else if (loanRptVM.loanReportOptions == LoanReportOptions.TCSRecoveryLoan)
                    {
                        parameterList.Add(new ReportParameter { name = "SalYear", value = loanRptVM.yearID });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        if (!loanRptVM.AllBranch)
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                        else
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = null });
                        rptName = "TCSRecovery.rpt";
                    }
                    else if (loanRptVM.loanReportOptions == LoanReportOptions.LoanSanction)
                    {
                        parameterList.Add(new ReportParameter { name = "SalYear", value = loanRptVM.yearID });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        parameterList.Add(new ReportParameter { name = "CHK", value = "L" });

                        if (!loanRptVM.AllBranch)
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                        else
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = null });

                        rptName = "LoanSanction.rpt";
                    }
                    else if (loanRptVM.loanReportOptions == LoanReportOptions.HouseBuildingLoan)
                    {
                        parameterList.Add(new ReportParameter { name = "SalYear", value = loanRptVM.yearID });
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = month });
                        if (!loanRptVM.AllBranch)
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = branchCode });
                        else
                            parameterList.Add(new ReportParameter { name = "BranchCode", value = null });
                        rptName = "HouseBuildingAdvance.rpt";
                    }
                    else if(loanRptVM.loanReportOptions== LoanReportOptions.IncomeTax)
                    {

                    }
                }

                reportModel.reportParameters = parameterList;
                reportModel.rptName = rptName;
                Session["ReportModel"] = reportModel;
                return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                loanRptVM.branchList = dropDownService.ddlBranchList();
                loanRptVM.employeeTypeList = dropDownService.ddlEmployeeTypeList();
            }
            return PartialView("_ReportFilters", loanRptVM);
        }
    }
}