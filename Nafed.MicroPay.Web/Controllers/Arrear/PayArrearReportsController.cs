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
using System.Data;
using System.Threading.Tasks;
using System.Text;
using AutoMapper;

namespace MicroPay.Web.Controllers.Arrear
{
    public class PayArrearReportsController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IArrearService ArrearService;
        private IGenericRepository genricRepository;

        public PayArrearReportsController(IDropdownBindService ddlService, IArrearService ArrearService, IGenericRepository genricRepository)
        {
            this.ddlService = ddlService;
            this.ArrearService = ArrearService;
            this.genricRepository = genricRepository;
        }
        public ActionResult Index()
        {
            log.Info($"PayArrearReportsController/Index");
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
        public ActionResult GetPAYArrearReportFilters()
        {
            log.Info($"PayArrearReportsController/GetPAYArrearReportFilters");
            try
            {
                BindDropdowns();
                ArrearFilters arrearFilters = new ArrearFilters();
                var result = ArrearService.GetArrearPeriodsDetailsforPay("AB");
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

        public void BindDropdowns()
        {
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

        //public JsonResult Getdateofgenerate(DateTime fromdate,DateTime todate)
        //{
        //    log.Info($"PayArrearReportsController/Getdateofgenerate");
        //    try
        //    {
        //        int periodfrom =Convert.ToInt32( fromdate.Year + (fromdate.Month.ToString().Length == 1 ? "0" + fromdate.Month.ToString() : fromdate.Month.ToString()));
        //        int periodto = Convert.ToInt32(todate.Year + (todate.Month.ToString().Length == 1 ? "0" + todate.Month.ToString() : todate.Month.ToString()));

        //        var datedetails = ddlService.datedetails(periodfrom, periodto);
        //        return Json(new { generatePayArreardate = datedetails }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //}

        [HttpPost]
        public ActionResult PAYArrearReportDetails(ArrearFilters ARVM, string ButtonType)
        {
            log.Info($"PayArrearReportsController/PAYArrearReportDetails");
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
                  && x.ProcessID == (int)WorkFlowProcess.PayArrearGenerate && x.BranchCode == BranchCode && (x.Status != 2 && x.Status != 4)))
                        {
                            TempData["Error"] = $"Approval request already exists.";
                            //return PartialView("_ReportFilter", ARVM);
                            return RedirectToAction(nameof(GetPAYArrearReportFilters));
                        }

                        var reportingPersonEmails = genricRepository.Get<Nafed.MicroPay.Data.Models.PayrollApprovalSetting>(x =>
                    x.ProcessID == (int)WorkFlowProcess.PayArrearGenerate && x.ToDate == null).FirstOrDefault();
                        var status = (byte)(reportingPersonEmails != null && reportingPersonEmails.Reporting2.HasValue ? 1 : 3);

                        Nafed.MicroPay.Data.Models.PayrollApprovalRequest request = new Nafed.MicroPay.Data.Models.PayrollApprovalRequest();

                        request.ProcessID = (int)WorkFlowProcess.PayArrearGenerate;
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

                    return RedirectToAction(nameof(GetPAYArrearReportFilters));
                    //return PartialView("_ReportFilter", ARVM);

                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        bool flag = false;
                        string branchName = "", branchCode = "";
                        if (ARVM.branchID != null && ARVM.branchID != 0)
                        {
                            ArrearFilters uDetail = new ArrearFilters();
                            flag = ArrearService.getemployeebranchDetails(Convert.ToInt32(ARVM.branchID), out uDetail);

                            if (flag)
                            {
                                branchName = uDetail.BranchName;
                                branchCode = uDetail.BranchCode;
                            }
                        }

                        #region togetFrom and To period

                        if (TempData["DAArrerDetails"] == null)
                        {
                            ArrearFilters arrearFilters = new ArrearFilters();
                            var result = ArrearService.GetArrearPeriodsDetailsforPay("AB");
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

                            ARVM.ArrerPeriodDetailsDA = periodDetails.Where(x => x.fromperiod1.Trim() == ARVM.fromPeriod.Trim()).
                                                    Select(y => (y.DOG)).FirstOrDefault();

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

                        string mon = periodDetails.Where(x => x.DOG.ToLower().Trim() == ARVM.ArrerPeriodDetailsDA.ToLower().Trim()).
                            Select(y => (y.mon)).FirstOrDefault();
                        string DOG = ARVM.ArrerPeriodDetailsDA.Substring(ARVM.ArrerPeriodDetailsDA.IndexOf(',') + 2, 4) + mon + ARVM.ArrerPeriodDetailsDA.Substring(0, 2);
                        #endregion

                        BaseReportModel reportModel = new BaseReportModel();
                        List<ReportParameter> parameterList = new List<ReportParameter>();

                        if (ARVM.ReportTypeID == 1)
                        {
                            parameterList.Add(new ReportParameter() { name = "FROMPERIOD", value = fromYear });
                            parameterList.Add(new ReportParameter() { name = "TOPERIOD", value = toYear });
                            parameterList.Add(new ReportParameter() { name = "ArrearType", value = "B" });
                            parameterList.Add(new ReportParameter() { name = "branchId", value = ARVM.branchID == 0 ? null : ARVM.branchID });
                            parameterList.Add(new ReportParameter() { name = "EmpId", value = ARVM.employeeID==0?null:ARVM.employeeID });
                            //parameterList.Add(new ReportParameter() { name = "fdate", value = null });
                            //parameterList.Add(new ReportParameter() { name = "tdate", value = null });
                            //parameterList.Add(new ReportParameter() { name = "tax", value = null });
                            parameterList.Add(new ReportParameter() { name = "GenerateDate", value = DOG });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "rptPayArrear.rpt";
                            reportModel.reportType = 2;
                            Session["ReportModel"] = reportModel;
                        }
                        else if (ARVM.ReportTypeID == 2)
                        {

                            int getFieldsValue = ArrearService.getPayArrearEmployee(branchCode, Convert.ToInt32(fromYear), Convert.ToInt32(toYear), DOG);

                            parameterList.Add(new ReportParameter() { name = "FROMPERIOD", value = fromYear });
                            parameterList.Add(new ReportParameter() { name = "TOPERIOD", value = toYear });
                            parameterList.Add(new ReportParameter() { name = "BRANCH", value = ARVM.branchID == null ? 0 : ARVM.branchID });
                            //parameterList.Add(new ReportParameter() { name = "EMPCOUNT", value = 1 });
                            //parameterList.Add(new ReportParameter() { name = "ArrearType", value = "B" });
                            //parameterList.Add(new ReportParameter() { name = "branchcode", value = ARVM.branchID == null ? 0 : ARVM.branchID });
                            parameterList.Add(new ReportParameter() { name = "ordno", value = ARVM.OrderNo });
                            parameterList.Add(new ReportParameter() { name = "orddate", value = ARVM.OrderDate.ToString("dd/MM/yyyy") });
                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "PayArrearSummary.rpt";
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
                            int getFieldsValue = ArrearService.getPayArrearEmployee(branchCode, Convert.ToInt32(fromYear), Convert.ToInt32(toYear), DOG);
                            parameterList.Add(new ReportParameter() { name = "FROMPERIOD", value = fromYear });
                            parameterList.Add(new ReportParameter() { name = "TOPERIOD", value = toYear });
                            parameterList.Add(new ReportParameter() { name = "BRANCH", value = ARVM.branchID == null ? 0 : ARVM.branchID });
                            parameterList.Add(new ReportParameter() { name = "BANKCODE", value = BankCode });
                            parameterList.Add(new ReportParameter() { name = "branchname", value = branchName });
                            parameterList.Add(new ReportParameter() { name = "BANKNAME", value = bankName });

                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "BankAdvicePayArrearAll.rpt";
                            reportModel.reportType = 2;
                            Session["ReportModel"] = reportModel;

                        }
                        else if (ARVM.ReportTypeID == 4)
                        {
                            int getFieldsValue = ArrearService.getPayArrearEmployee(branchCode, Convert.ToInt32(fromYear), Convert.ToInt32(toYear), DOG);

                            parameterList.Add(new ReportParameter() { name = "FROMPERIOD", value = fromYear });
                            parameterList.Add(new ReportParameter() { name = "TOPERIOD", value = toYear });
                            parameterList.Add(new ReportParameter() { name = "BRANCH", value = ARVM.branchID});

                            reportModel.reportParameters = parameterList;
                            reportModel.rptName = "rptPFPAYArrear.rpt";
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
                        int year = DateTime.Now.Year;
                        AVM.branchList = ddlService.ddlBranchList();
                        return PartialView("_ReportFilter", ARVM);
                    }
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
                return Json(new { DAArrerPeriods = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void SendDAApprovalEmail(PayrollApprovalRequest request)
        {
            log.Info($"PayArreaerReport/SendPAYApprovalEmail");

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
                    emailMessage.Subject = $"REVIEW & APPROVAL OF PAY ARREAR BILL";
                    mailBody.Clear();
                    mailBody.AppendFormat("<div>Dear Sir/Ma'am,</div> <br>");
                    mailBody.AppendFormat($"<div>Pay Arrear bill for the period of <b>{request.Period} <b> towards branch : <b> {request.BranchName} </b> has been prepared.the same may please be reviewed.<br>");

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