using CrystalDecisions.CrystalReports.Engine;
using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Confirmation
{
    public class EmployeeConfirmationController : BaseController
    {
        private readonly IDropdownBindService ddlServices;
        private readonly IConfirmationFormService confirmationService;
        private readonly IDesignationService designationService;
        public EmployeeConfirmationController(IDropdownBindService ddlServices, IConfirmationFormService confirmationService, IDesignationService designationService)
        {
            this.confirmationService = confirmationService;
            this.ddlServices = ddlServices;
            this.designationService = designationService;
        }
        // GET: EmployeeConfirmation
        public ActionResult Index()
        {
            return View();
        }

        #region 
        [HttpGet]
        public ActionResult _EmployeeConfirmationFilters()
        {
            log.Info($"EmployeeConfirmation/_EmployeeConfirmationFilters");
            try
            {
                EmployeeConfirmationFormFilters filters = new EmployeeConfirmationFormFilters();

                filters.FormTypeList = new List<SelectListModel>()
                {
                    new SelectListModel { id = 6, value = "Appointment" },
                    new SelectListModel { id = 7, value = "Promotion" }
                };
                return PartialView("_EmployeeConfirmationFilters", filters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostEmployeeConfirmation(EmployeeConfirmationFormFilters filters)
        {
            log.Info($"EmployeeConfirmation/_PostEmployeeConfirmation");
            try
            {

                var confirmationForms = confirmationService.GetEmployeeConfirmationDetails(filters);
                filters.employeeConfirmationViewDetails = confirmationForms;
                return PartialView("_EmployeeConfirmationGridView", filters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult UpdateEmployeeConfirmationStatus(int formHdrID, int headerId, int formTypeId, int employeeID, int processID, int? filterFormTypeID = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            log.Info($"EmployeeConfirmation/UpdateEmployeeConfirmationStatus");
            try
            {
                EmployeeConfirmationFormFilters filters = new EmployeeConfirmationFormFilters();
                if (!designationService.CheckConfrmChildHasRecord(employeeID, processID))
                {
                    ModelState.AddModelError("NoRecordInConfChild", "Approval process is not set for this employee, you can set approval process from 'Manage -> Employee -> Confirmation Approval' menu.");
                    filters.FormTypeId = filterFormTypeID;
                    filters.FromDate = fromDate;
                    filters.ToDate = toDate;
                    filters.employeeConfirmationViewDetails = confirmationService.GetEmployeeConfirmationDetails(filters);
                    return PartialView("_EmployeeConfirmationGridView", filters);
                }

                var flag = confirmationService.UpdateEmployeeConfirmationStatus(formHdrID, headerId, formTypeId, employeeID, processID);
               
                filters.FormTypeId = filterFormTypeID;
                filters.FromDate = fromDate;
                filters.ToDate = toDate;
                var confirmationForms = confirmationService.GetEmployeeConfirmationDetails(filters);
                filters.employeeConfirmationViewDetails = confirmationForms;
                return PartialView("_EmployeeConfirmationGridView", filters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult GetEmployeeRemarks(int formHdrID, int headerId, int formTypeId, int employeeID, int pid, DateTime? duedt)
        {
            log.Info($"EmployeeConfirmation/GetEmployeeRemarks");
            try
            {
                ConfirmationPRSectionRemarks confirmationPRRemark = new ConfirmationPRSectionRemarks();
                confirmationPRRemark.FormHdID = formHdrID;
                confirmationPRRemark.FormHeaderID = headerId;
                confirmationPRRemark.FormTypeID = formTypeId;
                confirmationPRRemark.EmployeeID = employeeID;
                confirmationPRRemark.ProcessID = pid;
                confirmationPRRemark.DueDate = duedt.HasValue ? duedt.Value : DateTime.Now;
                var confRemrkList = confirmationService.GetConfirmationDetails(formHdrID, formTypeId, employeeID, headerId);
                confirmationPRRemark.confHdrList = confRemrkList.confHdrList;
                if (confirmationPRRemark.confHdrList != null && confirmationPRRemark.confHdrList.Count > 0)
                {
                    confirmationPRRemark.confHdrList.ForEach(x => { x.FormTypeID = confirmationPRRemark.FormTypeID; });
                }
                confirmationPRRemark.PersonalSectionRemark = confRemrkList.PersonalSectionRemark;
                confirmationPRRemark.PRSubmit = confRemrkList.PRSubmit;
                confirmationPRRemark.FileNo = confRemrkList.FileNo;
                confirmationPRRemark.GMEmailID = confRemrkList.GMEmailID;
                confirmationPRRemark.GeneralManager = confRemrkList.GeneralManager;
                confirmationPRRemark.GMDesignation = confRemrkList.GMDesignation;
                confirmationPRRemark.DVHEmployeeCode = confRemrkList.DVHEmployeeCode;
                confirmationPRRemark.EmailID1 = confRemrkList.EmailID1;
                confirmationPRRemark.EmailID2 = confRemrkList.EmailID2;
                confirmationPRRemark.EmailID3 = confRemrkList.EmailID3;
                confirmationPRRemark.EmailID4 = confRemrkList.EmailID4;
                confirmationPRRemark.EmailID5 = confRemrkList.EmailID5;
                confirmationPRRemark.StatusID = confRemrkList.StatusID;
                return PartialView("_PRSectionRemark", confirmationPRRemark);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult PostPersonalSectionRemarks(ConfirmationPRSectionRemarks confirmationPRRemark, string ButtonType)
        {
            log.Info($"EmployeeConfirmation/PostPersonalSectionRemarks");
            try
            {
                //var error = ModelState.Where(x => x.Value.Errors.Count > 1).ToList();
                if (string.IsNullOrEmpty(confirmationPRRemark.DVHEmployeeCode))
                    ModelState.AddModelError("DVHEmployeeCode", "Please enter Divisional Head Employee Code.");

                if (ModelState.IsValid)
                {
                    if (ButtonType == "Confirmed")
                    {
                        confirmationPRRemark.StatusID = (int)ConfirmationFormState.Confirmed;
                    }
                    else if (ButtonType == "Extend")
                        confirmationPRRemark.StatusID = (int)ConfirmationFormState.Rejected;
                    else if (ButtonType == "Publish")
                        confirmationPRRemark.PRSubmit = true;

                    confirmationPRRemark.UpdatedBy = userDetail.UserID;

                    var result = confirmationService.PostPersonalSectionRemarks(confirmationPRRemark);
                    if (result)
                    {
                        if (confirmationPRRemark.PRSubmit)
                        {
                            var confDtlforReport = confirmationService.GetConfirmationDtlForOrdrRpt(confirmationPRRemark.EmployeeID, confirmationPRRemark.FormHdID, confirmationPRRemark.FormHeaderID);
                            confDtlforReport.StatusID = confirmationPRRemark.StatusID;
                            confDtlforReport.EmployeeID = confirmationPRRemark.EmployeeID;
                            confDtlforReport.ProcessID = confirmationPRRemark.ProcessID;
                            var orderReportPath = DownloadAndMail_Order(confDtlforReport);
                            bool issendmail = confirmationService.SendMailForOrderReport(confDtlforReport, Server.MapPath(orderReportPath));
                        }
                        EmployeeConfirmationFormFilters filters = new EmployeeConfirmationFormFilters();
                        var confirmationForms = confirmationService.GetEmployeeConfirmationDetails(filters);
                        filters.employeeConfirmationViewDetails = confirmationForms;
                        return Json(new { msgType = "success", msg = "Remark saved successfully.", htmlData = ConvertViewToString("_EmployeeConfirmationGridView", filters) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { msgType = "error", msg = "Something is wrong remark not saved", htmlData = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { msgType = "required", htmlData = ConvertViewToString("_PRSectionRemark", confirmationPRRemark) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        public ActionResult DownloadReport(string fno, int empID, int formHdID, int formHeaderID, int statusID)
        {
            log.Info("EmployeeConfirmation/DownloadReport");
            try
            {
                if (!string.IsNullOrEmpty(fno))
                {
                    var cfnObj = confirmationService.GetConfirmationDtlForOrdrRpt(empID, formHeaderID, formHdID);
                    cfnObj.StatusID = statusID;
                    cfnObj.EmployeeID = empID;

                    string rptName = string.Empty;
                    BaseReportModel reportModel = new BaseReportModel();
                    List<ReportParameter> parameterList = new List<ReportParameter>();

                    parameterList.Add(new ReportParameter { name = "fileno", value = cfnObj.FileNo ?? " " });
                    parameterList.Add(new ReportParameter { name = "empName", value = cfnObj.Employee });
                    parameterList.Add(new ReportParameter { name = "designation", value = cfnObj.Designation });
                    parameterList.Add(new ReportParameter { name = "branch", value = cfnObj.Branch ?? " " });
                    parameterList.Add(new ReportParameter { name = "department", value = cfnObj.Department ?? " " });
                    parameterList.Add(new ReportParameter { name = "dueDate", value = cfnObj.DueDate });
                    parameterList.Add(new ReportParameter { name = "GM", value = cfnObj.GeneralManager ?? " " });
                    parameterList.Add(new ReportParameter { name = "GMDes", value = cfnObj.GMDesignation ?? " " });
                    parameterList.Add(new ReportParameter { name = "email", value = cfnObj.GMEmailID ?? " " });
                    parameterList.Add(new ReportParameter { name = "titleID", value = cfnObj.TitleID });
                    parameterList.Add(new ReportParameter { name = "genderID", value = cfnObj.GenderID });
                    parameterList.Add(new ReportParameter { name = "orderDate", value = cfnObj.OrderDate });
                    parameterList.Add(new ReportParameter { name = "payscale", value = cfnObj.PayScale ?? " " });
                    reportModel.reportParameters = parameterList;

                    if (cfnObj.StatusID == 8)
                        rptName = "ConfirmationConfirmed.rpt";
                    else if (cfnObj.StatusID == 9)
                        rptName = "ConfirmationExtend.rpt";

                    reportModel.rptName = rptName;
                    //  reportModel.reportType = 2;

                    ReportDocument objReport = new ReportDocument();
                    var rptModel = reportModel;
                    string path = Path.Combine(Server.MapPath(rptModel.ReportFilePath), rptModel.rptName);
                    objReport.Load(path);
                    //  objReport.SetDatabaseLogon(ConfigManager.Value("odbcUser"), ConfigManager.Value("odbcPassword"), @"odbcNAFEDHR", ConfigManager.Value("odbcDatabase"));
                    objReport.Refresh();
                    if (rptModel.reportParameters?.Count > 0)
                    {
                        foreach (var item in rptModel.reportParameters)
                            objReport.SetParameterValue($"{item.name}", item.value);
                    }
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();


                    objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;

                    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

                    Stream stream = objReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "Confirmation Letter.pdf");

                }
                else
                {
                    return new EmptyResult();
                }
                // else
                //  {
                // return View("SalaryReport");
                //  }
                // }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public string DownloadAndMail_Order(ConfirmationPRSectionRemarks cfnObj)
        {
            BaseReportModel reportModel = new BaseReportModel();
            List<ReportParameter> parameterList = new List<ReportParameter>();
            string rptName = string.Empty;
            parameterList.Add(new ReportParameter { name = "fileno", value = cfnObj.FileNo });
            parameterList.Add(new ReportParameter { name = "empName", value = cfnObj.Employee });
            parameterList.Add(new ReportParameter { name = "designation", value = cfnObj.Designation ?? " " });
            parameterList.Add(new ReportParameter { name = "branch", value = cfnObj.Branch ?? " " });
            parameterList.Add(new ReportParameter { name = "department", value = cfnObj.Department ?? " " });
            parameterList.Add(new ReportParameter { name = "dueDate", value = cfnObj.DueDate });
            parameterList.Add(new ReportParameter { name = "GM", value = cfnObj.GeneralManager ?? " " });
            parameterList.Add(new ReportParameter { name = "GMDes", value = cfnObj.GMDesignation ?? " " });
            parameterList.Add(new ReportParameter { name = "email", value = cfnObj.GMEmailID ?? " " });
            parameterList.Add(new ReportParameter { name = "titleID", value = cfnObj.TitleID });
            parameterList.Add(new ReportParameter { name = "genderID", value = cfnObj.GenderID });
            parameterList.Add(new ReportParameter { name = "orderDate", value = cfnObj.OrderDate });
            parameterList.Add(new ReportParameter { name = "payscale", value = cfnObj.PayScale ?? " " });
            reportModel.reportParameters = parameterList;

            if (cfnObj.StatusID == 8)
                rptName = "ConfirmationConfirmed.rpt";
            else if (cfnObj.StatusID == 9)
                rptName = "ConfirmationExtend.rpt";

            reportModel.rptName = rptName;
            //  reportModel.reportType = 2;

            ReportDocument objReport = new ReportDocument();
            var rptModel = reportModel;
            string path = Path.Combine(Server.MapPath(rptModel.ReportFilePath), rptModel.rptName);
            objReport.Load(path);
            //  objReport.SetDatabaseLogon(ConfigManager.Value("odbcUser"), ConfigManager.Value("odbcPassword"), @"odbcNAFEDHR", ConfigManager.Value("odbcDatabase"));
            objReport.Refresh();
            if (rptModel.reportParameters?.Count > 0)
            {
                foreach (var item in rptModel.reportParameters)
                    objReport.SetParameterValue($"{item.name}", item.value);
            }
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;

            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

            Stream stream = objReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Path.Combine(Server.MapPath(rptModel.AdmitCardCPath), $"Confirmation Letter.pdf"));
            return (ConfigManager.Value("AdmitCardUNCPath") + $"Confirmation Letter.pdf");
        }

        public ActionResult GetGMNameDesignation(string empCode)
        {
            log.Info($"EmployeeConfirmationController/GetGMNameDesignation/empCode={empCode}");
            try
            {
                var getData = confirmationService.GetEmployeeNameDesignation(empCode);
                if (getData != null)
                    return Json(new { msg = "success", htmlData = getData }, JsonRequestBehavior.AllowGet);

                else
                    return Json(new { msg = "error" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
