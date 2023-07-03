using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Nafed.MicroPay.Common.FileHelper;

namespace MicroPay.Web.Controllers.Import
{
    public class ImportProcessApproverListController : BaseController
    {
        // GET: ImportProcessApproverList

        private readonly IImportProcessApproverService importProcessApprServices;

        public ImportProcessApproverListController(IImportProcessApproverService importProcessApprServices)
        {
            this.importProcessApprServices = importProcessApprServices;
        }
        public ActionResult Index()
        {
            log.Info($"ImportProcessApproverList/Index/");
            return View();
        }

        [HttpGet]
        public ActionResult _GetFilter()
        {
            log.Info($"ImportProcessApproverList/_GetFilter/");
            try
            {
                var processList = importProcessApprServices.GetProcessList();
                return PartialView("_Filter", processList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult PostFormData(FormCollection frm)
        {
            log.Info($"ImportProcessApproverList/PostFormData/");
            try
            {
                var processList = importProcessApprServices.GetProcessList();

                if (frm["ddlProcess"]?.Length == 0)
                    ModelState.AddModelError("ProcessRequired", "Please Select Process");

                var selectedValue = frm["ddlProcess"];

                if (ModelState.IsValid)
                {
                    return Json(new
                    {
                        message = "success",
                        selectedID = selectedValue,
                        htmlData = ConvertViewToString("_Filter", processList)
                    });
                }
                return PartialView("_Filter", processList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult _Import(HttpPostedFileBase file)
        {
            log.Info($"ImportProcessApproverList/_Import/");

            try
            {
                string fileName = string.Empty; string filePath = string.Empty;
                int error = 0, warning = 0, duplicateEntries = 0;
                List<ImportProcessApprovers> data = new List<ImportProcessApprovers>();
                List<string> columnName = new List<string>();
                List<string> missingHeaders;
                string msg = string.Empty;

                if (file != null)
                {
                    string fileExtension = Path.GetExtension(file.FileName);
                    fileName = Path.GetFileName(file.FileName);
                    var contentType = GetFileContentType(file.InputStream);
                    var dicValue = GetDictionaryValueByKeyName(".xlsx");
                    if (dicValue == contentType && (fileExtension == ".xlsx" || fileExtension == ".xlx"))
                    {
                        #region Upload File At temp location=== 

                        fileName = ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(file.FileName),
                               Path.GetExtension(file.FileName));

                        filePath = DocumentUploadFilePath.MonthlyImportFilePath;
                        string uploadedFilePath = Server.MapPath(filePath);
                        string sPhysicalPath = Path.Combine(uploadedFilePath, fileName);
                        file.SaveAs(sPhysicalPath);

                        #endregion

                        var objData = importProcessApprServices.
                            ReadImportMonthlyExcelData(sPhysicalPath, false, out msg, out error,
                            out warning, out data, out missingHeaders, out columnName);

                        duplicateEntries = objData.GroupBy(x => new { x.ECode, x.ReportingTo, x.ReviewerTo, x.AcceptanceTo }).Sum(g => g.Count() - 1);
                        var getDuplicateRows = ExtensionMethods.FindDuplicates(objData.ToList(),
                            x => new { x.ECode, x.ReportingTo, x.ReviewerTo, x.AcceptanceTo });
                        objData.Join(getDuplicateRows, (x) => new { x.ECode, x.ReportingTo, x.ReviewerTo, x.AcceptanceTo }, (y) => new { y.ECode, y.ReportingTo, y.ReviewerTo, y.AcceptanceTo }, (x, y) =>
                        {
                            x.isDuplicatedRow = (((x.ECode == y.ECode)
                            && (x.ReportingTo == y.ReportingTo)
                            && (x.ReviewerTo == y.ReviewerTo) && (x.AcceptanceTo == y.AcceptanceTo)) ? true : false);
                            return x;
                        }).ToList();

                        if (error == 0 && duplicateEntries > 0)
                            error = duplicateEntries;
                        else
                            error += duplicateEntries;

                        var json = JsonConvert.SerializeObject(objData);
                        DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

                        if (msg != "Headers")
                        {
                            var dataTableColumns = dataTable.Columns.Cast<DataColumn>()
                              .Select(x => x.ColumnName).ToList();

                            var count = dataTableColumns.Count;

                            int columnIndex = 0;

                            var newList = dataTableColumns.Select(s => s.Replace("#", "Sno")).ToList();
                            foreach (var col in newList)
                            {
                                dataTable.Columns[col].SetOrdinal(columnIndex);
                                columnIndex++;
                            }

                            #region  Set Column Ordinal No ======

                            dataTable.Columns["ECode"].SetOrdinal(0);
                            dataTable.Columns["ReportingTo"].SetOrdinal(1);
                            dataTable.Columns["ReviewerTo"].SetOrdinal(2);
                            dataTable.Columns["AcceptanceTo"].SetOrdinal(3);

                            #endregion

                        }
                        TempData["columns"] = columnName;
                        TempData["InputData"] = dataTable;
                        TempData["ImportProcessApproverData"] = data;
                        TempData["error"] = error;
                        TempData["msg"] = msg;
                        TempData["missingHeaders"] = missingHeaders;
                        TempData.Keep("InputData");
                        TempData.Keep("ImportProcessApproverData");
                        TempData.Keep("error");
                        TempData.Keep("msg");
                        TempData.Keep("columnName");
                        return Json("success");
                    }
                    else
                    {
                        TempData["error"] = -2;
                        return Json("inValidFileFormat");
                    }
                }
                else
                {
                    TempData["error"] = -1;
                    ModelState.AddModelError("FileRequired", "Please Select File.");
                    return Json("nofileFound");
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public PartialViewResult _ImportedDataAsGridFormat()
        {
            log.Info($"ImportProcessApproverList/ImportedDataAsGridFormat");
            try
            {
                ImportProcessApproverVM impProcessApproverVM = new ImportProcessApproverVM();
                List<ImportProcessApprovers> fileData = new List<ImportProcessApprovers>();
                impProcessApproverVM.ErrorMsgCollection = GetErrorMessage();
                if (TempData["InputData"] != null)
                {
                    impProcessApproverVM.importData = (List<ImportProcessApprovers>)TempData["ImportProcessApproverData"];
                    impProcessApproverVM.inputData = (DataTable)TempData["InputData"];

                    TempData.Keep("InputData");
                    TempData.Keep("ImportProcessApproverData");
                }
                else
                    impProcessApproverVM.importData = fileData;

                impProcessApproverVM.columnName = TempData["columns"] != null ? (List<string>)TempData["columns"] : new List<string>();

                impProcessApproverVM.NoOfErrors = TempData["error"] != null ? (int)TempData["error"] : 0;
                impProcessApproverVM.Message = TempData["msg"] != null ? Convert.ToString(TempData["msg"]) : "";
                if (impProcessApproverVM.Message != "Headers")
                {
                    if (impProcessApproverVM.NoOfErrors > 0)
                        impProcessApproverVM.Message = string.Format("{0} error(s) found ,Please check the sheet and reupload.", impProcessApproverVM.NoOfErrors.ToString());
                    else
                        impProcessApproverVM.Message = "Sheet validated successfully, Click on Update button to update your data.";
                }
                else
                {
                    var list = (List<string>)TempData["missingHeaders"];
                    var missingheader = String.Join(",", list);
                    if (list.Count > 1)
                        impProcessApproverVM.Message = string.Format("{0} header(s) are missing", missingheader);
                    else
                        impProcessApproverVM.Message = string.Format("{0} header(s) is missing", missingheader);
                }
                return PartialView("_ImportDataGridView", impProcessApproverVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        
        [HttpPost]
        public ActionResult SubmitFileData(int processID)
        {
            log.Info("ImportProcessApproverList/SubmitFileData/");
            try
            {
                var importDataList = (List<ImportProcessApprovers>)TempData["ImportProcessApproverData"];
                TempData.Keep("ImportProcessApproverData");
                var result= importProcessApprServices.ImportProcessApproverData(userDetail.UserID, processID,importDataList);
                return Json(new { errorCode = 0, msg = "Process Approver List has been imported successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                return Json(new { errorCode = -1, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}