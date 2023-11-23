using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Common;

using Nafed.MicroPay.Model;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services.Arrear;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Threading.Tasks;
using System.Text;
using AutoMapper;

namespace MicroPay.Web.Controllers.Arrear
{
    public class DAArrearReportsController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IArrearService ArrearService;
        private IGenericRepository genricRepository;
        public DAArrearReportsController(IDropdownBindService ddlService, IArrearService ArrearService, IGenericRepository genricRepository)
        {
            this.ddlService = ddlService;
            this.ArrearService = ArrearService;
            this.genricRepository = genricRepository;
        }
        public ActionResult Index()
        {
            log.Info($"DAArrearReportsController/Index");
            try
            {
                return View(userAccessRight);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetDAArrearReportFilters()
        {
            log.Info($"DAArrearReportsController/GetDAArrearReportFilters");
            try
            {
                BindDropdowns();
                ArrearFilters arrearFilters = new ArrearFilters();
                var result = ArrearService.GetArrearPeriodsDetailsforPay("AD");
                TempData["DAArrerDetails"] = result;
                arrearFilters.listArrerPeriod = result.Select(x => new SelectListModel { id = 0, value = x.DOG }).ToList();
                arrearFilters.branchList = ddlService.ddlBranchList();
                return PartialView("_ReportFilter", arrearFilters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public JsonResult GetArrearPeriodsDetails(int? branchId)
        {
            var result = ArrearService.GetArrearPeriodsDetailsforPay("AD", branchId);
            TempData["DAArrerDetails"] = result;
            var selectLists = result.Select(x => new SelectListModel { id = 0, value = x.DOG }).ToList();
           
            var selectListModel = new SelectListModel();
            selectListModel.id = 0;
            selectListModel.value = "SELECT";
            selectLists.Insert(0,selectListModel);
            var arrearPeriods = new SelectList(selectLists, "id", "value");
            return Json(new { arrearPeriods = arrearPeriods }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBranchEmployee(int branchID)
        {
            log.Info($"DAArrearReportsController/GetBranchEmployee");
            try
            {
                var employeedetails = ddlService.employeeByBranchID(branchID);
                SelectListModel selectemployeeDetails = new SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "All";
                employeedetails.Insert(0, selectemployeeDetails);
                var emp = new SelectList(employeedetails, "id", "value");
                return Json(new { employees = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }



        public void BindDropdowns()
        {
            ArrearFilters arrearFilters = new ArrearFilters();
            List<SelectListModel> reporttype = new List<SelectListModel>();
            reporttype.Add(new SelectListModel
            { value = "Select", id = 0 });
            reporttype.Add(new SelectListModel
            { value = "Arrear Detail", id = 1 });
            reporttype.Add(new SelectListModel
            { value = "Arrear Summary", id = 2 });
            reporttype.Add(new SelectListModel
            { value = "Bank Advice", id = 3 });
            reporttype.Add(new SelectListModel
            { value = "PF Statement", id = 4 });
            ViewBag.ReportType = new SelectList(reporttype, "id", "value");

            List<SelectListItem> selectEmployee = new List<SelectListItem>();
            selectEmployee.Add(new SelectListItem
            { Text = "select", Value = "0" });
            ViewBag.Employee = selectEmployee;

            var ddlBanks = ddlService.ddlBanks();
            ddlBanks.OrderBy(x => x.value);
            ViewBag.Banks = ddlBanks;



        }

        [HttpPost]
        public ActionResult DAArrearReportDetails(ArrearFilters ARVM, string ButtonType)
        {
            log.Info($"DAArrearReportsController/ArrearReportDetails");
            try
            {
                ModelState.Remove("genrateYearID");
                if (ButtonType == "Send for Approval")
                {
                    if (ARVM.ReportTypeID == 0)
                        ModelState.AddModelError("ReportTypeID", "Please Select Report Type.");

                    if (ARVM.ArrerPeriodDetailsDA == "" || ARVM.ArrerPeriodDetailsDA.ToUpper() == "SELECT") ModelState.AddModelError("ArrerPeriodDetailsDA", "Please Select Generate Date.");

                    if (ModelState.IsValid)
                    {
                        var periodDetails = (List<ArrerPeriodDetailsForPAYDA>)TempData["DAArrerDetails"];
                        TempData.Keep("DAArrerDetails");
                        string Period = periodDetails.Where(x => x.DOG.ToLower().Trim() == ARVM.ArrerPeriodDetailsDA.ToLower().Trim()).
                            Select(y => (y.fromperiod1)).FirstOrDefault();

                        string month = Period.Substring(0, 2);
                        string year = Period.Substring(2, 4);
                        string newPeriod = year + month;
                        string BranchCode = "Except-HO";
                        if (ARVM.branchID.HasValue)
                            BranchCode = "99";

                        if (genricRepository.Exists<Nafed.MicroPay.Data.Models.PayrollApprovalRequest>(x => x.Period == newPeriod
                  && x.ProcessID == (int)WorkFlowProcess.DAArrearGenerate && x.BranchCode == BranchCode && (x.Status != 2 && x.Status != 4)))
                        {
                            TempData["Error"] = $"Approval request already exists.";
                            //return PartialView("_ReportFilter", ARVM);
                            return RedirectToAction(nameof(GetDAArrearReportFilters));
                        }

                        var reportingPersonEmails = genricRepository.Get<Nafed.MicroPay.Data.Models.PayrollApprovalSetting>(x =>
                    x.ProcessID == (int)WorkFlowProcess.DAArrearGenerate && x.ToDate == null).FirstOrDefault();
                        var status = (byte)(reportingPersonEmails != null && reportingPersonEmails.Reporting2.HasValue ? 1 : 3);

                        Nafed.MicroPay.Data.Models.PayrollApprovalRequest request = new Nafed.MicroPay.Data.Models.PayrollApprovalRequest();

                        request.ProcessID = (int)WorkFlowProcess.DAArrearGenerate;
                        request.CreatedOn = DateTime.Now;
                        request.CreatedBy = userDetail.UserID;
                        request.EmployeeTypeID = userDetail.EmployeeTypeId;
                        if (!ARVM.branchID.HasValue)
                            request.BranchCode = "Except-HO";
                        else
                        {
                            request.BranchCode = "99";
                            request.BranchID = 44;
                        }

                        request.Status = status;
                        request.Period = newPeriod;
                        genricRepository.Insert<Nafed.MicroPay.Data.Models.PayrollApprovalRequest>(request);
                        if (request.ApprovalRequestID > 0)
                        {
                            PayrollApprovalRequest request2 = new PayrollApprovalRequest();
                            string fperiod = periodDetails.Where(x => x.DOG.ToLower().Trim() == ARVM.ArrerPeriodDetailsDA.ToLower().Trim()).
                           Select(y => (y.fromperiod1)).FirstOrDefault();

                            string tperiod = periodDetails.Where(x => x.DOG.ToLower().Trim() == ARVM.ArrerPeriodDetailsDA.ToLower().Trim()).
                           Select(y => (y.toperiod1)).FirstOrDefault();

                            request2.Reporting1 = reportingPersonEmails.Reporting1;
                            request2.Reporting2 = reportingPersonEmails.Reporting2;
                            request2.Reporting3 = reportingPersonEmails.Reporting3;
                            request2.Period = fperiod + '-' + tperiod;
                            request2.Status = (byte)(reportingPersonEmails != null && reportingPersonEmails.Reporting2.HasValue ? (int)ApprovalStatus.RequestedByReporting1 : (int)ApprovalStatus.ApprovedByReporting2);
                            var employee = genricRepository.GetByID<Nafed.MicroPay.Data.Models.tblMstEmployee>((reportingPersonEmails.Reporting2.HasValue ? request2.Reporting2 : request2.Reporting3));
                            request2.emailTo = employee.OfficialEmail;

                            if (!request.BranchID.HasValue)
                                request2.BranchName = request.BranchCode == "Except-HO" ? "All Branches (Except HO)" : "All Branches";

                            Task t1 = Task.Run(() => SendDAApprovalEmail(request2));


                            TempData["Message"] = $"Approval request sent successfully.";
                        }
                    }
                    else
                    {
                        TempData["Error"] = $"Please select all required field.";
                    }

                    return RedirectToAction(nameof(GetDAArrearReportFilters));
                    //return PartialView("_ReportFilter", ARVM);

                }
                else
                if (ModelState.IsValid)
                {
                    string branchName = "";
                    if (ARVM.branchID != null)
                    {
                        if (ARVM.branchID == 0)
                            branchName = "All Branch Except Head Office";
                        else
                            branchName = genricRepository.GetByID<Nafed.MicroPay.Data.Models.Branch>(ARVM.branchID).BranchName;
                    }

                    #region togetFrom and To period
                    if (TempData["DAArrerDetails"] == null)
                    {
                        ArrearFilters arrearFilters = new ArrearFilters();
                        var result = ArrearService.GetArrearPeriodsDetailsforPay("AD");
                        TempData["DAArrerDetails"] = result;
                    }

                    var periodDetails = (List<ArrerPeriodDetailsForPAYDA>)TempData["DAArrerDetails"];
                    TempData.Keep("DAArrerDetails");
                    string fromPeriod = "";
                    string toPeriod = "";
                    if (ARVM.ArrerPeriodDetailsDA == null)
                    {
                        fromPeriod = periodDetails.Where(x => x.fromperiod1.Trim() == ARVM.fromPeriod.Trim()).
                                                Select(y => (y.fromperiod1)).FirstOrDefault();
                        toPeriod = periodDetails.Where(x => x.fromperiod1.Trim() == ARVM.fromPeriod.Trim()).
                           Select(y => (y.toperiod1)).FirstOrDefault();
                        ARVM.employeeID = ARVM.employeeID == null ? 0 : ARVM.employeeID.Value;
                        if (ARVM.branchID.HasValue)
                        {
                            if (ARVM.branchID.Value == 0)
                                ARVM.branchID = null;
                        }
                    }
                    else
                    {
                        fromPeriod = periodDetails.Where(x => x.DOG.ToLower().Trim() == ARVM.ArrerPeriodDetailsDA.ToLower().Trim()).
                                                Select(y => (y.fromperiod1)).FirstOrDefault();
                        toPeriod = periodDetails.Where(x => x.DOG.ToLower().Trim() == ARVM.ArrerPeriodDetailsDA.ToLower().Trim()).
                           Select(y => (y.toperiod1)).FirstOrDefault();
                    }

                    string fromYear = fromPeriod.Substring(2, 4) + fromPeriod.Substring(0, 2);
                    string toYear = toPeriod.Substring(2, 4) + toPeriod.Substring(0, 2);
                    #endregion

                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();
                    //string fromYear = ARVM.fromdate.Year.ToString() + (ARVM.fromdate.Month.ToString().Length == 1 ? "0" + ARVM.fromdate.Month.ToString() : ARVM.fromdate.Month.ToString());
                    //string toYear = ARVM.todate.Year.ToString() + (ARVM.todate.Month.ToString().Length == 1 ? "0" + ARVM.todate.Month.ToString() : ARVM.todate.Month.ToString());
                    if (ARVM.ReportTypeID == 1)
                    {
                        parameterList.Add(new ReportParameter() { name = "FROMPERIOD", value = fromYear });
                        parameterList.Add(new ReportParameter() { name = "TOPERIOD", value = toYear });
                        parameterList.Add(new ReportParameter() { name = "ArrearType", value = "D" });
                        parameterList.Add(new ReportParameter() { name = "branchId", value = ARVM.branchID == null ? 0 : ARVM.branchID });
                        parameterList.Add(new ReportParameter() { name = "EmpId", value = ARVM.employeeID == null ? 0 : ARVM.employeeID });
                        //parameterList.Add(new ReportParameter() { name = "branchCode", value = ARVM.branchID });
                        //parameterList.Add(new ReportParameter() { name = "EmpCode", value = ARVM.employeeID == 0 ? null : ARVM.employeeID });
                        reportModel.reportParameters = parameterList;
                        //   reportModel.rptName = "daareear.rpt";
                        reportModel.rptName = "rpDAArrear.rpt";
                        reportModel.reportType = 2;
                        Session["ReportModel"] = reportModel;
                    }
                    else if (ARVM.ReportTypeID == 2)
                    {


                        parameterList.Add(new ReportParameter() { name = "FROMPERIOD", value = fromYear });
                        parameterList.Add(new ReportParameter() { name = "TOPERIOD", value = toYear });
                        parameterList.Add(new ReportParameter() { name = "EMPCOUNT", value = 126 });
                        parameterList.Add(new ReportParameter() { name = "ArrearType", value = "D" });
                        parameterList.Add(new ReportParameter() { name = "branchcode", value = ARVM.branchID == null ? 0 : ARVM.branchID });
                        parameterList.Add(new ReportParameter() { name = "ordno", value = ARVM.OrderNo });
                        parameterList.Add(new ReportParameter() { name = "orddate", value = ARVM.OrderDate.ToString("dd/MM/yyyy") });
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = "DAARREARSUMMARYNEW.rpt";
                        reportModel.reportType = 2;
                        Session["ReportModel"] = reportModel;

                    }
                    else if (ARVM.ReportTypeID == 3)
                    {
                        string bankName = "";
                        string BankCode = ARVM.BankCode == null ? "" : ARVM.BankCode;

                        if (BankCode != "")
                        {
                            bankName = genricRepository.GetByID<Nafed.MicroPay.Data.Models.tblMstBank>(BankCode).BankName;
                        }

                        parameterList.Add(new ReportParameter() { name = "FROMPERIOD", value = fromYear });
                        parameterList.Add(new ReportParameter() { name = "TOPERIOD", value = toYear });
                        parameterList.Add(new ReportParameter() { name = "BANKCODE", value = BankCode });
                        parameterList.Add(new ReportParameter() { name = "BRANCH", value = ARVM.branchID == null ? 0 : ARVM.branchID });
                        parameterList.Add(new ReportParameter() { name = "ArrearType", value = "D" });
                        parameterList.Add(new ReportParameter() { name = "branchname", value = branchName });
                        parameterList.Add(new ReportParameter() { name = "BANKNAME", value = bankName });
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = "BankAdvice.rpt";
                        reportModel.reportType = 2;
                        Session["ReportModel"] = reportModel;

                    }
                    else if (ARVM.ReportTypeID == 4)
                    {


                        parameterList.Add(new ReportParameter() { name = "FROMPERIOD", value = fromYear });
                        parameterList.Add(new ReportParameter() { name = "TOPERIOD", value = toYear });
                        parameterList.Add(new ReportParameter() { name = "BRANCH", value = ARVM.branchID == null ? 0 : ARVM.branchID });
                        parameterList.Add(new ReportParameter() { name = "ArrearType", value = "D" });
                        parameterList.Add(new ReportParameter() { name = "branchname", value = branchName });
                        reportModel.reportParameters = parameterList;
                        reportModel.rptName = "rptPFStatement.rpt";
                        reportModel.reportType = 2;
                        Session["ReportModel"] = reportModel;

                    }
                    return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<SelectListItem> selectEmployee = new List<SelectListItem>();
                    selectEmployee.Add(new SelectListItem
                    { Text = "select", Value = "0" });
                    ViewBag.Employee = selectEmployee;
                    ArrearViewModel AVM = new ArrearViewModel();
                    //int year = DateTime.Now.Year;
                    AVM.branchList = ddlService.ddlBranchList();

                    //SelectListModel selectBranch = new SelectListModel();
                    //selectBranch.id = 0;
                    //selectBranch.value = "All Branch Except HO";
                    //AVM.branchList.Insert(1, selectBranch);
                    ARVM.listArrerPeriod = new List<SelectListModel>();
                    return PartialView("_ReportFilter", ARVM);
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
            log.Info($"DAArrearReportsController/GetPeriodDetails");
            try
            {
                var periodDetails = (List<ArrerPeriodDetailsForPAYDA>)TempData["DAArrerDetails"];
                TempData.Keep("DAArrerDetails");
                var result = periodDetails.Where(x => x.DOG.ToLower().Trim() == details.ToLower().Trim()).
                 Select(y => (y.fromperiod + ":" + y.toperiod)).FirstOrDefault();

                var result2 = periodDetails.Where(x => x.DOG.ToLower().Trim() == details.ToLower().Trim()).
                Select(y => (y.orderNo + ":" + y.orderDate)).FirstOrDefault();

                return Json(new { DAArrerPeriods = result, DAArrerOrderNo= result2 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void SendDAApprovalEmail(PayrollApprovalRequest request)
        {
            log.Info($"DAArreaerReport/SendDAApprovalEmail");

            try
            {
                EmailMessage emailMessage = new EmailMessage();
                StringBuilder mailBody = new StringBuilder();

                var emailsetting = genricRepository.Get<Nafed.MicroPay.Data.Models.EmailConfiguration>().FirstOrDefault();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Nafed.MicroPay.Data.Models.EmailConfiguration, EmailMessage>()
                    .ForMember(d => d.From, o => o.MapFrom(s => $"NAFED HRMS <{s.ToEmail}>"))
                    .ForMember(d => d.To, o => o.UseValue(request.emailTo))
                    .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                    .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                    .ForMember(d => d.SmtpClientHost, o => o.MapFrom(s => s.Server))
                    .ForMember(d => d.SmtpPort, o => o.MapFrom(s => s.Port))
                    .ForMember(d => d.enableSSL, o => o.MapFrom(s => s.SSLStatus))
                    .ForMember(d => d.HTMLView, o => o.UseValue(true))
                    .ForMember(d => d.FriendlyName, o => o.UseValue("NAFED"));
                });

                emailMessage = Mapper.Map<EmailMessage>(emailsetting);

                if (request.Status == (int)ApprovalStatus.RequestedByReporting1)
                {
                    emailMessage.Subject = $"REVIEW & APPROVAL OF DA ARREAR BILL";
                    mailBody.Clear();
                    mailBody.AppendFormat("<div>Dear Sir/Ma'am,</div> <br>");
                    mailBody.AppendFormat($"<div>DA Arrear bill for the period of <b>{request.Period} <b> towards branch : <b> {request.BranchName} </b> has been prepared.the same may please be reviewed.<br>");

                    mailBody.AppendFormat($"<div>Regards, </div> <br>");
                    mailBody.AppendFormat($"<div>F & A Team,  </div> <br>");
                    mailBody.AppendFormat($"<div>Nafed  </div> <br> <br>");
                }

                emailMessage.Body = mailBody.ToString();
                EmailHelper.SendEmail(emailMessage);
            }
            catch (Exception)
            {
            }
        }
    }
}