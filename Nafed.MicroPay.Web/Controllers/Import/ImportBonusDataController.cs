using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Common;
using System.IO;
using static Nafed.MicroPay.Common.FileHelper;
using Newtonsoft.Json;
using System.Data;
using MicroPay.Web.Models;



namespace MicroPay.Web.Controllers.Import
{
    public class ImportBonusDataController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IImportBonusService importBonusService;
        public ImportBonusDataController(IDropdownBindService ddlService, IImportBonusService importBonusService)
        {
            this.ddlService = ddlService;
            this.importBonusService = importBonusService;
        }
        // GET: ImportBonusData
        public ActionResult Index()
        {
            log.Info($"ImportBonusData/Index/");
            return View();
        }

        [HttpGet]
        public ActionResult _GetFilter()
        {
            log.Info($"ImportBonusData/_GetFilter/");
            try
            {
                ImportBonus objFilter = new ImportBonus();
                objFilter.branches = ddlService.ddlBranchList(null, null);

                return PartialView("_Filter", objFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _ExportTemplate(ImportBonus model)
        {
            log.Info($"ImportBonusData/_ExportTemplate/");
            try
            {
                if (model.SalMonth == 0)
                    ModelState.AddModelError("MonthRequired", "Please Select Month");
                if (model.SalYear == 0)
                    ModelState.AddModelError("YearRequired", "Please Select Year");
                model.branches = ddlService.ddlBranchList(null, null);
                if (ModelState.IsValid)
                {
                    string fileName = string.Empty, msg = string.Empty;
                    string fullPath = Server.MapPath("~/FileDownload/");
                    fileName = ExtensionMethods.SetUniqueFileName("Bonus-", FileExtension.xlsx);

                    var res = importBonusService.GetBonusEntryForm(model.BranchID ?? 0, model.SalMonth, model.SalYear, fileName, fullPath);

                    if (res == "norec")
                        ModelState.AddModelError("OtherValidation", "No Record Found.");
                    else

                        return Json(new
                        {
                            fileName = fileName,
                            fullPath = fullPath + fileName,
                            message = "success",
                            htmlData = ConvertViewToString("_Filter", model)
                        });
                }

                return PartialView("_Filter", model);
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
            log.Info($"ImportBonusData/_Import/");

            try
            {
                string fileName = string.Empty; string filePath = string.Empty;
                int error = 0, warning = 0, duplicateEntries = 0;
                List<ImportBounsData> data = new List<ImportBounsData>();
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

                        var objData = importBonusService.
                            ReadImportMonthlyExcelData(sPhysicalPath, false, out msg, out error,
                            out warning, out data, out missingHeaders, out columnName);

                        duplicateEntries = objData.GroupBy(x => new { x.EmployeeId, x.Month, x.Year, x.BranchID }).Sum(g => g.Count() - 1);
                        var getDuplicateRows = ExtensionMethods.FindDuplicates(objData.ToList(), x => new { x.EmployeeId, x.Month, x.Year, x.BranchID });
                        objData.Join(getDuplicateRows, (x) => new { x.EmployeeId, x.Month, x.Year, x.BranchID }, (y) => new { y.EmployeeId, y.Month, y.Year, y.BranchID }, (x, y) =>
                        {
                            x.isDuplicatedRow = (((x.EmployeeId == y.EmployeeId)
                            && (x.Month == y.Month)
                            && (x.Year == y.Year) && (x.BranchID == y.BranchID)) ? true : false);
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

                            dataTable.Columns["EmployeeCode"].SetOrdinal(0);
                            dataTable.Columns["EmployeeName"].SetOrdinal(1);
                            dataTable.Columns["BranchName"].SetOrdinal(2);
                            dataTable.Columns["DesignationName"].SetOrdinal(3);

                            #endregion

                        }
                        TempData["columns"] = columnName;
                        TempData["InputData"] = dataTable;
                        TempData["ImportBonusData"] = data;
                        TempData["error"] = error;
                        TempData["msg"] = msg;
                        TempData["missingHeaders"] = missingHeaders;
                        TempData.Keep("InputData");
                        TempData.Keep("ImportBonusData");
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
                    // return PartialView("_Import");
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
            log.Info($"ImportBonusData/ImportedDataAsGridFormat");
            try
            {
                ImportBonusVM importBonusVM = new ImportBonusVM();
                List<ImportBounsData> fileData = new List<ImportBounsData>();
                importBonusVM.ErrorMsgCollection = GetErrorMessage();
                if (TempData["InputData"] != null)
                {
                    importBonusVM.importData = (List<ImportBounsData>)TempData["ImportBonusData"];
                    importBonusVM.inputData = (DataTable)TempData["InputData"];

                    TempData.Keep("InputData");
                    TempData.Keep("ImportBonusData");
                }
                else
                    importBonusVM.importData = fileData;

                importBonusVM.columnName = TempData["columns"] != null ? (List<string>)TempData["columns"] : new List<string>();

                importBonusVM.NoOfErrors = TempData["error"] != null ? (int)TempData["error"] : 0;
                importBonusVM.Message = TempData["msg"] != null ? Convert.ToString(TempData["msg"]) : "";
                if (importBonusVM.Message != "Headers")
                {
                    if (importBonusVM.NoOfErrors > 0)
                        importBonusVM.Message = string.Format("{0} error(s) found ,Please check the sheet and reupload.", importBonusVM.NoOfErrors.ToString());
                    else
                        importBonusVM.Message = "Sheet validated successfully, Click on Update button to update your data.";
                }
                else
                {
                    var list = (List<string>)TempData["missingHeaders"];
                    var missingheader = String.Join(",", list);
                    if (list.Count > 1)
                        importBonusVM.Message = string.Format("{0} header(s) are missing", missingheader);
                    else
                        importBonusVM.Message = string.Format("{0} header(s) is missing", missingheader);
                }
                return PartialView("_ImportDataGridView", importBonusVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult SubmitFileData()
        {
            log.Info("ImportBonusData/SubmitFileData/");
            try
            {
                var importDataList = (List<ImportBounsData>)TempData["ImportBonusData"];
                TempData.Keep("ImportBonusData");
                var result = importBonusService.ImportBonusManualData(userDetail.UserID, importDataList);
                return Json(new { errorCode = 0, msg = "Bonus Manual data has been imported successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                return Json(new { errorCode = -1, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}