using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model = Nafed.MicroPay.Model;
using Nafed.MicroPay.Common;
using System.IO;
using System.Data;
using System.Text;
using static Nafed.MicroPay.Common.FileHelper;
namespace MicroPay.Web.Controllers.Approval
{
    public class AcarAparController : BaseController
    {
        private readonly IAppraisalFormService appraisalService;
        private readonly IDropdownBindService ddlServices;

        public AcarAparController(IAppraisalFormService appraisalService, IDropdownBindService ddlServices)
        {
            this.appraisalService = appraisalService;
            this.ddlServices = ddlServices;
        }
        // GET: AcarApar
        public ActionResult Index()
        {
            return View();
        }

        #region Admin Search
        [HttpGet]
        public ActionResult _AdminACARAPARFilters()
        {
            log.Info($"AcarAparController/_AdminACARAPARFilters");
            try
            {
                AppraisalFormApprovalFilter filters = new AppraisalFormApprovalFilter();
                filters.employees = ddlServices.GetAllEmployee();

                filters.reportingYrs = ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                filters.ACAR = false;
                return PartialView("_AcarAparFilters", filters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public ActionResult _GetAcarAparGrid()
        {
            log.Info($"AcarAparController/_GetAcarAparGrid");
            try
            {
                ApprovalRequestVM approval = new ApprovalRequestVM();
                return PartialView("_AcarAparGridView", approval);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostAcarAparFilters(AppraisalFormApprovalFilter filters, string ButtonType)
        {
            log.Info($"AcarAparController/_PostAcarAparFilters");
            try
            {

                if (ButtonType == "Export")
                {
                    DataTable exportData = new DataTable();

                    if (TempData["AcarAparList"] != null)
                    {

                        string fileName = string.Empty, msg = string.Empty;
                        string fullPath = Server.MapPath("~/FileDownload/");
                        fileName = "APAR Status Report" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;
                        TempData.Keep("AcarAparList");
                        var APARStatus = (DataTable)TempData["AcarAparList"];
                        DataColumn dtc0 = new DataColumn("Sno.");
                        DataColumn dtc1 = new DataColumn("Reporting Year");
                        DataColumn dtc2 = new DataColumn("Form Name");
                        DataColumn dtc3 = new DataColumn("Name");
                        DataColumn dtc4 = new DataColumn("Department");
                        DataColumn dtc5 = new DataColumn("Designation");
                        DataColumn dtc6 = new DataColumn("APAR Status");
                        exportData.Columns.Add(dtc0);
                        exportData.Columns.Add(dtc1);
                        exportData.Columns.Add(dtc2);
                        exportData.Columns.Add(dtc3);
                        exportData.Columns.Add(dtc4);
                        exportData.Columns.Add(dtc5);
                        exportData.Columns.Add(dtc6);
                        for (int i = 0; i < APARStatus.Rows.Count; i++)
                        {
                            DataRow row = exportData.NewRow();
                            row["Sno."] = i + 1; ;
                            row["Reporting Year"] = APARStatus.Rows[i]["ReportingYr"].ToString();
                            row["Form Name"] = APARStatus.Rows[i]["FormName"].ToString();
                            row["Name"] = APARStatus.Rows[i]["EmployeeCode"].ToString() + " - " + APARStatus.Rows[i]["EmpName"].ToString();
                            row["Department"] = APARStatus.Rows[i]["DepartmentName"].ToString();
                            row["Designation"] = APARStatus.Rows[i]["DesignationName"].ToString();
                            row["APAR Status"] = APARStatus.Rows[i]["StatusID"].ToString();
                            exportData.Rows.Add(row);
                        }

                        var res = appraisalService.ExportAPARStatusList(exportData, fullPath, fileName);
                        fullPath = $"{fullPath}{fileName}";
                        return JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");

                    }
                    return Content("Refreshed Data");
                }
                else
                {
                    if (filters.appraisalFormStatus == AppraisalFormState.SavedByEmployee)
                        filters.statusId = (int)AppraisalFormState.SavedByEmployee;
                    else if (filters.appraisalFormStatus == AppraisalFormState.SubmitedByEmployee)
                        filters.statusId = (int)AppraisalFormState.SubmitedByEmployee;
                    else if (filters.appraisalFormStatus == AppraisalFormState.SavedByReporting)
                        filters.statusId = (int)AppraisalFormState.SavedByReporting;
                    else if (filters.appraisalFormStatus == AppraisalFormState.SubmitedByReporting)
                        filters.statusId = (int)AppraisalFormState.SubmitedByReporting;
                    else if (filters.appraisalFormStatus == AppraisalFormState.SavedByReviewer)
                        filters.statusId = (int)AppraisalFormState.SavedByReviewer;
                    else if (filters.appraisalFormStatus == AppraisalFormState.SubmitedByReviewer)
                        filters.statusId = (int)AppraisalFormState.SubmitedByReviewer;
                    else if (filters.appraisalFormStatus == AppraisalFormState.SavedByAcceptanceAuth)
                        filters.statusId = (int)AppraisalFormState.SavedByAcceptanceAuth;
                    else if (filters.appraisalFormStatus == AppraisalFormState.SubmitedByAcceptanceAuth)
                        filters.statusId = (int)AppraisalFormState.SubmitedByAcceptanceAuth;
                    else
                        filters.statusId = null;

                    //  var appraisalForms = appraisalService.GetAcarAparDetails(filters, "SP_AcarAparDetails");
                    //var appraisalForms = appraisalService.GetAppraisalFormHdr(filters);

                    ApprovalRequestVM approval = new ApprovalRequestVM();
                    var appraisalForms = appraisalService.GetAppraisalFormHistory(filters);
                    //  approval.appraisalForms = appraisalForms.Where(x => x.StatusID != 1).ToList();
                    approval.appraisalForms = appraisalForms.ToList();
                    approval.FormAttributes = appraisalService.GetFormSubmittionDueDate(filters.selectedReportingYear);
                    var AcarAparList = ExtensionMethods.ToDataTable(approval.appraisalForms);
                    TempData["AcarAparList"] = AcarAparList;
                    return PartialView("_AcarAparGridView", approval);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        public ActionResult ACARIndex()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _AdminACARFilters()
        {
            log.Info($"AcarAparController/_AdminACARFilters");
            try
            {
                AppraisalFormApprovalFilter filters = new AppraisalFormApprovalFilter();
                filters.employees = ddlServices.GetAllEmployee();
                filters.reportingYrs = new List<SelectListModel>()
                {
                    new SelectListModel { id = 1, value = "2019-2020" },
                    new SelectListModel { id = 2, value = "2020-2021" }
                };
                filters.ACAR = true;

                return PartialView("_AcarAparFilters", filters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostAcarFilters(AppraisalFormApprovalFilter filters)
        {
            log.Info($"AcarAparController/_PostAcarFilters");
            try
            {
                if (filters.competencyFormState == CompetencyFormState.SavedByEmployee)
                    filters.statusId = (int)CompetencyFormState.SavedByEmployee;
                else if (filters.competencyFormState == CompetencyFormState.SubmitedByEmployee)
                    filters.statusId = (int)CompetencyFormState.SubmitedByEmployee;
                else if (filters.competencyFormState == CompetencyFormState.SavedByReporting)
                    filters.statusId = (int)CompetencyFormState.SavedByReporting;
                else if (filters.competencyFormState == CompetencyFormState.SubmitedByReporting)
                    filters.statusId = (int)CompetencyFormState.SubmitedByReporting;
                else
                    filters.statusId = null;

                var appraisalForms = appraisalService.GetAcarAparDetails(filters, "SP_ACARDetails");
                return PartialView("_AcarGridView", appraisalForms);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region Upload And Download And Get Documents
        [HttpGet]
        public ActionResult GetUploadDocuments(int appraisalFormID, int reportingTo, int? reviewingTo, int? acceptanceAuthority, int empID, string ryr)
        {
            log.Info($"AcarAparController/GetUploadDocuments");
            try
            {
                Model.AppraisalForm appForm = new Model.AppraisalForm();

                var reportingYr = ryr;
                appForm.FormID = appraisalFormID;
                appForm.ReportingTo = reportingTo;
                appForm.ReviewingTo = reviewingTo;
                appForm.EmployeeID = (int)empID;
                appForm.AcceptanceAuthorityTo = acceptanceAuthority;
                appForm.loggedInEmpID = userDetail.EmployeeID;
                appForm.ReportingYr = reportingYr;
                appForm.frmAttributes = appraisalService.GetFormRulesAttributes((Nafed.MicroPay.Common.AppraisalForm)appraisalFormID, empID, reportingYr);

                if (appForm.FormID == (int)(Nafed.MicroPay.Common.AppraisalForm.FormA) || appForm.FormID == (int)(Nafed.MicroPay.Common.AppraisalForm.FormB) || appForm.FormID == (int)(Nafed.MicroPay.Common.AppraisalForm.FormC) || appForm.FormID == (int)(Nafed.MicroPay.Common.AppraisalForm.FormE) || appForm.FormID == (int)(Nafed.MicroPay.Common.AppraisalForm.FormF))
                    appForm.UploadRemarksLength = "500";
                else if (appForm.FormID == (int)(Nafed.MicroPay.Common.AppraisalForm.FormD) || appForm.FormID == (int)(Nafed.MicroPay.Common.AppraisalForm.FormG) || appForm.FormID == (int)(Nafed.MicroPay.Common.AppraisalForm.FormH))
                    appForm.UploadRemarksLength = "200";

                return PartialView("_UploadDocument", appForm);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadDocuments(Model.AppraisalForm apprForm)
        {
            log.Info($"AcarAparController/UploadDocuments");
            try
            {
                var files = (List<string>)TempData["UploadedFiles"];
                #region Document Upload
                List<Model.APARReviewedSignedCopy> docList = new List<Model.APARReviewedSignedCopy>();
                foreach (var file in files)
                {
                    Model.APARReviewedSignedCopy aparSignedDocuments = new Model.APARReviewedSignedCopy();
                    aparSignedDocuments.FormID = apprForm.FormID;
                    aparSignedDocuments.FormGroupID = apprForm.frmAttributes.FormGroupID;
                    aparSignedDocuments.UploadedDocName = file;
                    aparSignedDocuments.UploadedBy = userDetail.UserID;
                    aparSignedDocuments.UploadedOn = DateTime.Now;
                    docList.Add(aparSignedDocuments);
                }
                apprForm.UploadedDocList = docList;
                #endregion
                var existingFileDetails = appraisalService.GetAPARDocumentsList(apprForm.FormID, apprForm.EmployeeID, apprForm.frmAttributes.FormGroupID);


                #region Process Work Flow
                if (existingFileDetails == null)
                {
                    Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                    {
                        SenderID = apprForm.AcceptanceAuthorityTo,
                        ReceiverID = apprForm.AcceptanceAuthorityTo,
                        CreatedBy = userDetail.UserID,
                        CreatedOn = DateTime.Now,
                        EmployeeID = apprForm.EmployeeID,
                        Scomments = $"{((Nafed.MicroPay.Common.AppraisalForm)apprForm.FormID).ToString()}, Appraisal Form Uploaded By : {(Model.SubmittedBy)4}",
                        ProcessID = (int)WorkFlowProcess.Appraisal,
                        StatusID = (int)AppraisalFormState.SubmitedByAcceptanceAuth,
                        ReferenceID = apprForm.frmAttributes.FormGroupID,
                        Senddate = DateTime.Now
                    };
                    apprForm._ProcessWorkFlow = workFlow;
                }
                #endregion
                bool flag = appraisalService.UpdateUploadDocumentsDetails(apprForm, existingFileDetails);
                return Json(new { part = 0, msgType = "success", msg = $"{((Nafed.MicroPay.Common.AppraisalForm)apprForm.FormID).ToString()}, Signed Copy Uploaded Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            HttpFileCollectionBase files = Request.Files;
            string fileName = string.Empty; string filePath = string.Empty;
            string fname = "";
            List<string> filename = new List<string>();
            string fullPath = "";
            TempData["UploadedFiles"] = null;

            var formId = Request.Form["formId"];
            var formGroupId = Request.Form["formGroupId"];
            var employeeId = Request.Form["employeeId"];


            #region Check Mime Type
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                var contentType = GetFileContentType(file.InputStream);

                if (!IsValidFileName(file.FileName))
                {
                    stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                    stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                    stringBuilder.Append($"I. a to z characters.");
                    stringBuilder.Append($"II. numbers(0 to 9).");
                    stringBuilder.Append($"III. - and _ with space.");
                }

                var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                if (dicValue != contentType)
                {
                    stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                    stringBuilder.Append("<br>");
                }
            }
            if (stringBuilder.ToString() != "")
            {
                return Json(stringBuilder.ToString());
            }
            #endregion

            #region Delete existing file
            var existingFileDetails = appraisalService.GetAPARDocumentsList(Convert.ToInt32(formId), Convert.ToInt32(employeeId), Convert.ToInt32(formGroupId));
            if (existingFileDetails != null)
            {
                var existingFileName = existingFileDetails.UploadedDocName;
                fullPath = Request.MapPath("~/Document/APARSignedCopy/" + existingFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            #endregion

            #region Save file on floder
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                fname = ExtensionMethods.SetUniqueFileName("APARSignedCopy-", Path.GetExtension(file.FileName));
                fileName = fname;
                filename.Add(fileName);
                fullPath = Request.MapPath("~/Document/APARSignedCopy/" + fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                fname = Path.Combine(Server.MapPath("~/Document/APARSignedCopy/"), fname);
                file.SaveAs(fname);
            }
            #endregion
            TempData["UploadedFiles"] = filename;
            return Json(files.Count + " Files Uploaded!");
        }

        [HttpGet]
        public ActionResult GetDownloadDocuments(int appraisalFormID, int empID, string ryr)
        {
            log.Info($"AcarAparController/GetDownloadDocuments");
            try
            {
                var reportingYr = ryr;
                int formGroupId = 0;
                var formDetails = appraisalService.GetFormRulesAttributes((Nafed.MicroPay.Common.AppraisalForm)appraisalFormID, empID, reportingYr);
                if (formDetails != null)
                    formGroupId = formDetails.FormGroupID;

                var result = appraisalService.GetAPARDocumentsList(appraisalFormID, empID, formGroupId);
                return PartialView("_DownloadDocument", result);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region APAR Approval Process List and Export

        [HttpGet]
        public ActionResult ProcessApproval()
        {
            log.Info($"AcarAparController/ProcessApproval");
            try
            {
                List<FormGroupAHdr> aprLst = new List<FormGroupAHdr>();
                return View(aprLst);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _ReportFilter()
        {
            var branchList = ddlServices.ddlBranchList();
            ViewBag.BranchList = new SelectList(branchList, "id", "value");

            var processList = new[] { (int)WorkFlowProcess.LeaveApproval, (int)WorkFlowProcess.AttendanceApproval, (int)WorkFlowProcess.Appraisal,
                    (int)WorkFlowProcess.AppointmentConfirmation ,(int)WorkFlowProcess.PromotionConfirmation,
                    (int)WorkFlowProcess.LTC, (int)WorkFlowProcess.Competency,(int)WorkFlowProcess.LoanApplication,(int)WorkFlowProcess.ConveyanceBill,
                    (int)WorkFlowProcess.EPFNomination,
                 (int)WorkFlowProcess.Form11,(int)WorkFlowProcess.NonRefundablePFLoan };

            var enumData = from WorkFlowProcess e in Enum.GetValues(typeof(WorkFlowProcess))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };
            enumData = enumData.Where(o => processList.Contains(o.ID)).OrderBy(l => l.Name);
            ViewBag.ProcessList = new SelectList(enumData, "ID", "Name");
            return PartialView("_RrtFilterPrcApproval");
        }

        [HttpGet]
        public ActionResult GetProcessApprovalDetailAndExport(CommonFilter filters, string ButtonType)
        {
            log.Info($"AcarAparController/GetProcessApprovalDetailAndExport");
            try
            {

                if (ButtonType == "Export")
                {
                    DataTable exportData = new DataTable();

                    if (TempData["TempApprovalList"] != null)
                    {

                        string fileName = string.Empty, msg = string.Empty;
                        string tFilter = string.Empty;
                        string fullPath = Server.MapPath("~/FileDownload/");

                        if (filters.ProcessID > 0)
                            tFilter = "Employee Process Approval Report For- " + (WorkFlowProcess)filters.ProcessID;


                        fileName = "Approval Process Report" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;
                        TempData.Keep("TempApprovalList");

                        var ApprovalList = (DataTable)TempData["TempApprovalList"];
                        DataColumn dtc0 = new DataColumn("S.No.");
                        DataColumn dtc1 = new DataColumn("Employee");
                        DataColumn dtc2 = new DataColumn("Designation");
                        DataColumn dtc3 = new DataColumn("Branch");
                        DataColumn dtc4 = new DataColumn("Reporting To");
                        DataColumn dtc5 = new DataColumn("Reviewer To");
                        DataColumn dtc6 = new DataColumn("Acceptance Authority");

                        DataColumn dtc7 = new DataColumn("Employee Code");
                        DataColumn dtc8 = new DataColumn("ReportingTo Employee Code");
                        DataColumn dtc9 = new DataColumn("Reviewer Employee Code");
                        DataColumn dtc10 = new DataColumn("Acceptance Code");

                        exportData.Columns.Add(dtc0);
                        exportData.Columns.Add(dtc7);
                        exportData.Columns.Add(dtc1);
                        exportData.Columns.Add(dtc2);
                        exportData.Columns.Add(dtc3);
                        exportData.Columns.Add(dtc8);
                        exportData.Columns.Add(dtc4);
                        exportData.Columns.Add(dtc9);
                        exportData.Columns.Add(dtc5);
                        exportData.Columns.Add(dtc10);
                        exportData.Columns.Add(dtc6);

                        for (int i = 0; i < ApprovalList.Rows.Count; i++)
                        {
                            DataRow row = exportData.NewRow();
                            row["S.No."] = i + 1;
                            row["Employee Code"] = ApprovalList.Rows[i]["EmployeeCode"].ToString();
                            row["Employee"] = ApprovalList.Rows[i]["EmployeeName"].ToString();
                            row["Designation"] = ApprovalList.Rows[i]["DesignationName"].ToString();
                            row["Branch"] = ApprovalList.Rows[i]["PlaceOfJoin"].ToString();
                            row[5] = ApprovalList.Rows[i]["RPTEmpCode"].ToString();
                            row["Reporting To"] = ApprovalList.Rows[i]["ReportingTo"].ToString();
                            row[7] = ApprovalList.Rows[i]["RVEmpCode"].ToString();
                            row["Reviewer To"] = ApprovalList.Rows[i]["ReviewerTo"].ToString();
                            row[9] = ApprovalList.Rows[i]["ACCEmpCode"].ToString();
                            row["Acceptance Authority"] = ApprovalList.Rows[i]["AcceptanceAuth"].ToString();
                            exportData.Rows.Add(row);
                        }

                        var res = appraisalService.ExportApprovalList(exportData, fullPath, fileName, tFilter);
                        fullPath = $"{fullPath}{fileName}";
                        return JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");

                    }
                    return Content("");
                }
                else
                {
                    var approvalList = appraisalService.GetProcessApprovalDetail(filters);

                    var ApprovalList = ExtensionMethods.ToDataTable(approvalList);
                    TempData["TempApprovalList"] = ApprovalList;
                    return PartialView("_EmpProcessApprovalDetail", approvalList);
                }
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