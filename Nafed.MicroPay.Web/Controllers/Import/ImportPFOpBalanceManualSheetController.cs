using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Common;
using System.IO;
using static Nafed.MicroPay.Common.FileHelper;
using Newtonsoft.Json;
using System.Data;
using MicroPay.Web.Models;

namespace MicroPay.Web.Controllers.Import
{
    public class ImportPFOpBalanceManualSheetController : BaseController
    {
        // GET: ImportPFOpBalanceManualSheet
        private readonly IDropdownBindService ddlService;
        private readonly IPFAccountBalanceServices pfAccBalService;
        private readonly IImportPfBalanceService importPfBalSerivce;

        public ImportPFOpBalanceManualSheetController(IDropdownBindService ddlService, IPFAccountBalanceServices pfAccBalService, IImportPfBalanceService importPfBalSerivce)
        {
            this.pfAccBalService = pfAccBalService;
            this.ddlService = ddlService;
            this.importPfBalSerivce = importPfBalSerivce;
        }

        public ActionResult Index()
        {
            log.Info($"ImportPFOpBalanceManualSheet/Index/");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _GetFilter()
        {
            log.Info($"ImportPFOpBalanceManualSheet/_GetFilter/");
            try
            {
                EmpPFOpBalance pfBalance = new EmpPFOpBalance();
                pfBalance.branches = ddlService.ddlBranchList(null, null);

                return PartialView("_Filter", pfBalance);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _ExportTemplate(EmpPFOpBalance model)
        {
            log.Info($"ImportPFOpBalanceManualSheet/_ExportTemplate/");
            try
            {
                if (model.Salmonth == 0)
                    ModelState.AddModelError("MonthRequired", "Please Select Month");
                if (model.Salyear == 0)
                    ModelState.AddModelError("YearRequired", "Please Select Year");
                model.branches = ddlService.ddlBranchList(null, null);
                if (ModelState.IsValid)
                {
                    string fileName = string.Empty, msg = string.Empty;
                    string fullPath = Server.MapPath("~/FileDownload/");
                    fileName = ExtensionMethods.SetUniqueFileName("PF Account Balance-", FileExtension.xlsx);

                    var res = pfAccBalService.GetPfBalanceManualEntryForm(model.BranchID ?? 0, model.Salmonth, model.Salyear, fileName, fullPath);
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
            log.Info($"ImportPFOpBalanceManualSheet/_Import/");

            try
            {
                string fileName = string.Empty; string filePath = string.Empty;
                int error = 0, warning = 0, duplicateEntries = 0;
                List<ImportPfBalanceData> data = new List<ImportPfBalanceData>();
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

                        var objData = importPfBalSerivce.
                            ReadImportMonthlyExcelData(sPhysicalPath, false, out msg, out error,
                            out warning, out data, out missingHeaders, out columnName);


                        duplicateEntries = objData.GroupBy(x => new { x.EmployeeCode, x.Month, x.Year, x.BranchID }).Sum(g => g.Count() - 1);
                        var getDuplicateRows = ExtensionMethods.FindDuplicates(objData.ToList(), x => new { x.EmployeeCode, x.Month, x.Year, x.BranchID });
                        objData.Join(getDuplicateRows, (x) => new { x.EmployeeCode, x.Month, x.Year, x.BranchID }, (y) => new { y.EmployeeCode, y.Month, y.Year, y.BranchID }, (x, y) =>
                        {
                            x.isDuplicatedRow = (((x.EmployeeCode == y.EmployeeCode) && (x.Month == y.Month) && (x.Year == y.Year) && (x.BranchID == y.BranchID)) ? true : false);
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

                            dataTable.Columns["BranchName"].SetOrdinal(1);
                            dataTable.Columns["EmployeeCode"].SetOrdinal(2);
                            dataTable.Columns["EmployeeName"].SetOrdinal(3);

                            #endregion

                        }
                        TempData["columns"] = columnName;
                        TempData["InputData"] = dataTable;
                        TempData["ImportPfBalanceData"] = data;
                        TempData["error"] = error;
                        TempData["msg"] = msg;
                        TempData["missingHeaders"] = missingHeaders;
                        TempData.Keep("InputData");
                        TempData.Keep("ImportPfBalanceData");
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
            log.Info($"ImportPFOpBalanceManualSheet/ImportedDataAsGridFormat");
            try
            {
                ImportPfBalanceVM importPFVM = new ImportPfBalanceVM();
                List<ImportPfBalanceData> fileData = new List<ImportPfBalanceData>();
                importPFVM.ErrorMsgCollection = GetErrorMessage();
                if (TempData["InputData"] != null)
                {
                    importPFVM.importData = (List<ImportPfBalanceData>)TempData["ImportPfBalanceData"];
                    importPFVM.inputData = (DataTable)TempData["InputData"];

                    TempData.Keep("InputData");
                    TempData.Keep("ImportPfBalanceData");
                }
                else
                    importPFVM.importData = fileData;

                importPFVM.columnName = TempData["columns"] != null ? (List<string>)TempData["columns"] : new List<string>();

                importPFVM.NoOfErrors = TempData["error"] != null ? (int)TempData["error"] : 0;
                importPFVM.Message = TempData["msg"] != null ? Convert.ToString(TempData["msg"]) : "";
                if (importPFVM.Message != "Headers")
                {
                    if (importPFVM.NoOfErrors > 0)
                        importPFVM.Message = string.Format("{0} error(s) found ,Please check the sheet and reupload.", importPFVM.NoOfErrors.ToString());
                    else
                        importPFVM.Message = "Sheet validated successfully, Click on Update button to update your data.";
                }
                else
                {
                    var list = (List<string>)TempData["missingHeaders"];
                    var missingheader = String.Join(",", list);
                    if (list.Count > 1)
                        importPFVM.Message = string.Format("{0} header(s) are missing", missingheader);
                    else
                        importPFVM.Message = string.Format("{0} header(s) is missing", missingheader);
                }
                return PartialView("_ImportDataGridView", importPFVM);
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
            log.Info("ImportPFOpBalanceManualSheet/SubmitFileData");
            try
            {
                var importDataList = (List<ImportPfBalanceData>)TempData["ImportPfBalanceData"];
                TempData.Keep("ImportPfBalanceData");
                var result = pfAccBalService.ImportPfBalanceManualData(userDetail.UserID, importDataList);
                return Json(new { errorCode = 0, msg = "Pf Balance Manual data has been imported successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                return Json(new { errorCode = -1, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}