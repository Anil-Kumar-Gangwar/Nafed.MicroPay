using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Model;
using CrystalDecisions.CrystalReports.Engine;
using Nafed.MicroPay.Common;
using System.IO;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.Salary;
namespace MicroPay.Web.Controllers.Reports
{
    public class MasterReportsController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly ISalaryReportService salService;
        public MasterReportsController(IDropdownBindService ddlService, ISalaryReportService salService)
        {
            this.ddlService = ddlService;
            this.salService = salService;
        }
        // GET: Reports
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult _ReportFilter()
        {
            log.Info($"MasterReportsController/_ReportFilter");
            try
            {
                MasterReports reportFilter = new MasterReports();

                ViewBag.ddlBranch = ddlService.ddlBranchList();
                ViewBag.ddlMonth = Enumerable.Range(1, 12).Select(em => new SelectListModel
                {
                    id = em,
                    value = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(em)
                }).ToList();

                ViewBag.ddlYear = Enumerable.Range(2006, (DateTime.Now.Year - 2006) + 1).Select(em => new SelectListModel
                {
                    id = em,
                    value = em.ToString()
                }).OrderByDescending(or => or.id).ToList();
                ViewBag.ddlEmployeeType = ddlService.ddlEmployeeTypeList();
                var ddlBank = ddlService.ddlBanks();
                ViewBag.Banks = ddlBank;
                reportFilter.EmployeeTypeID = 5;// for regular employee
                reportFilter.Month = DateTime.Now.Month;
                reportFilter.BankCode = "IC";
                return PartialView("_ReportFilter", reportFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult PrintReports(MasterReports reportFilter)
        {
            log.Info($"MasterReportsController/PrintReports");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();
                string rptName = string.Empty; string bankCode = string.Empty;
                string branchName = string.Empty;
                string fileName = string.Empty, msg = string.Empty;
                string fullPath = Server.MapPath("~/FileDownload/");
                bool flag;
                reportModel.reportType = 2;
                switch (reportFilter.RadioReportType)
                {
                    case ReportTypes.EmployeewiseBankAcnt:

                        if (reportFilter.BankCode == "IC")
                        {
                            fileName = "ICICI Bank Report" + '.' + FileExtension.xlsx;

                            flag = salService.ExportEmpDtlWithBankAccount(reportFilter, fullPath, fileName);
                            if (flag)
                            {
                                fullPath = $"{fullPath}{fileName}";
                                return Json(new { result = true, type = 1, fileName = fileName, fullPath = fullPath }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { result = false, type = 0, msg = "No records found." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (reportFilter.BankCode == "PS")
                        {
                            fileName = "Punjab & Sind Bank" + '.' + FileExtension.xlsx;
                            flag = salService.ExportEmpDtlWithBankAccount(reportFilter, fullPath, fileName);
                            if (flag)
                            {
                                fullPath = $"{fullPath}{fileName}";
                                return Json(new { result = true, type = 1, fileName = fileName, fullPath = fullPath }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { result = false, type = 0, msg = "No records found." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = reportFilter.AllBranch == true ? 0 : reportFilter.BranchID });
                            parameterList.Add(new ReportParameter { name = "SalMonth", value = reportFilter.Month });
                            parameterList.Add(new ReportParameter { name = "SalYear", value = reportFilter.Year });
                            parameterList.Add(new ReportParameter { name = "Bankcode", value = reportFilter.BankCode });
                            parameterList.Add(new ReportParameter { name = "EmpType", value = reportFilter.EmployeeTypeID });
                            rptName = "EmpWthbankaccount.rpt";

                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = rptName;
                            Session["ReportModel"] = reportModel;
                            return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                        }

                    case ReportTypes.BankAcnt:
                        parameterList.Add(new ReportParameter { name = "BranchID", value = reportFilter.AllBranch==true?0:reportFilter.BranchID});
                        parameterList.Add(new ReportParameter { name = "SalMonth", value = reportFilter.Month });
                        parameterList.Add(new ReportParameter { name = "SalYear", value = reportFilter.Year });
                        parameterList.Add(new ReportParameter { name = "EmpType", value = reportFilter.EmployeeTypeID });
                        rptName = "EmpWthoutbankaccount.rpt";

                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);

                    case ReportTypes.CCA:

                        rptName = "CCA.rpt";
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                    //  break;
                    case ReportTypes.SpeciaPay:
                        parameterList.Add(new ReportParameter { name = "BranchID", value = reportFilter.AllBranch == true ? 0 : reportFilter.BranchID });
                        parameterList.Add(new ReportParameter { name = "fmonth", value = reportFilter.Month });
                        parameterList.Add(new ReportParameter { name = "fyear", value = reportFilter.Year });
                        rptName = "RptSP.rpt";
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                    // break;
                    case ReportTypes.IncometaxDedc:

                        parameterList.Add(new ReportParameter { name = "salmonth", value = reportFilter.Month });
                        parameterList.Add(new ReportParameter { name = "salyear", value = reportFilter.Year });
                        rptName = "IncometaxName.rpt";
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                    case ReportTypes.Classificationofcitiesgrade:
                        rptName = "ClassificationofcitiesGrade.rpt";
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);

                    case ReportTypes.Locationwisettl:

                        parameterList.Add(new ReportParameter { name = "EMPTYPE", value = reportFilter.EmployeeTypeID });
                        rptName = "TOTEMPLOYEE.rpt";
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);

                    case ReportTypes.Locationwiselstttlemployess:
                        if (reportFilter.AllBranch)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = reportFilter.AllBranch == true ? 0 : reportFilter.BranchID });
                            parameterList.Add(new ReportParameter { name = "salmonth", value = reportFilter.Month });
                            parameterList.Add(new ReportParameter { name = "salyear", value = reportFilter.Year });
                            parameterList.Add(new ReportParameter { name = "EMPTYPE", value = reportFilter.EmployeeTypeID });
                            rptName = "EMPLIST.rpt";
                        }
                        else
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = reportFilter.AllBranch == true ? 0 : reportFilter.BranchID });
                            parameterList.Add(new ReportParameter { name = "salmonth", value = reportFilter.Month });
                            parameterList.Add(new ReportParameter { name = "salyear", value = reportFilter.Year });
                            parameterList.Add(new ReportParameter { name = "EMPTYPE", value = reportFilter.EmployeeTypeID });
                            rptName = "EMPLIST_HO.rpt";
                        }
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);


                    case ReportTypes.locationwisenetPay:
                        int pmon;
                        int pyear;
                        if (reportFilter.Month == 1)
                        {
                            pmon = 12;
                            pyear = reportFilter.Year.Value - 1;
                        }
                        else
                        {
                            pmon = reportFilter.Month.Value;
                            pmon = pmon - 1;
                            pyear = reportFilter.Year.Value;
                        }
                        parameterList.Add(new ReportParameter { name = "prevmon", value = pmon });
                        parameterList.Add(new ReportParameter { name = "curmon", value = reportFilter.Month });
                        parameterList.Add(new ReportParameter { name = "prevyr", value = pyear });
                        parameterList.Add(new ReportParameter { name = "curyr", value = reportFilter.Year });
                        rptName = "NetPayDiffrencial.rpt";
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);

                    case ReportTypes.EmployeelstDOB:
                        if (reportFilter.AllBranch)
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = reportFilter.AllBranch == true ? 0 : reportFilter.BranchID });
                            parameterList.Add(new ReportParameter { name = "fmonth", value = reportFilter.Month });
                            parameterList.Add(new ReportParameter { name = "fyear", value = reportFilter.Year });
                        }
                        else
                        {
                            parameterList.Add(new ReportParameter { name = "BranchID", value = reportFilter.AllBranch == true ? 0 : reportFilter.BranchID });
                            parameterList.Add(new ReportParameter { name = "fmonth", value = reportFilter.Month });
                            parameterList.Add(new ReportParameter { name = "fyear", value = reportFilter.Year });
                        }
                        rptName = "EMPLISTDOB.rpt";
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);

                    case ReportTypes.EmployeelstAdmngft:
                        if (reportFilter.AllBranch)
                        {
                            parameterList.Add(new ReportParameter { name = "salmonth", value = reportFilter.Month });
                            parameterList.Add(new ReportParameter { name = "salyear", value = reportFilter.Year });
                            parameterList.Add(new ReportParameter { name = "EMPTYPEID", value = reportFilter.EmployeeTypeID });
                            parameterList.Add(new ReportParameter { name = "count", value = 1 });
                        }
                        else
                        {
                            parameterList.Add(new ReportParameter { name = "salmonth", value = reportFilter.Month });
                            parameterList.Add(new ReportParameter { name = "salyear", value = reportFilter.Year });
                            parameterList.Add(new ReportParameter { name = "EMPTYPEID", value = reportFilter.EmployeeTypeID });
                            parameterList.Add(new ReportParameter { name = "count", value = 0 });
                        }
                        rptName = "rptEmplistAdmn.rpt";
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = rptName;
                        Session["ReportModel"] = reportModel;
                        return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                }


                return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult PrintData()
        {
            BaseReportModel reportModel = new BaseReportModel();
            reportModel.rptName = "DesgnationMasterReport.rpt";
            List<ReportParameter> parameterList = new List<ReportParameter>();
            parameterList.Add(new ReportParameter { name = "designationID", value = null });
            reportModel.reportParameters = parameterList;
            Session["ReportModel"] = reportModel;
            return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult GetFilter()
        {
            log.Info("TrainingController/GetFilter");
            try
            {
                CommonFilter cFilter = new CommonFilter();
                return PartialView("_Filter", cFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }


    }
}