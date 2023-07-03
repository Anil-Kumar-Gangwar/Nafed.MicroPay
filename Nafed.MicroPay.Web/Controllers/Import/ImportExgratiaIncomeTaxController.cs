using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System.Web.Mvc;
using Nafed.MicroPay.Common;
using System.Web;
using static Nafed.MicroPay.Common.FileHelper;
using System;
using System.IO;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.Linq;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data;

using Nafed.MicroPay.Services.Salary;

namespace MicroPay.Web.Controllers.Import
{
    public class ImportExgratiaIncomeTaxController : BaseController
    {
        private readonly IExgratiaService exgratiaService;
        private readonly IDropdownBindService ddlService;
        private readonly IImportExGratiaService importExService;
        public ImportExgratiaIncomeTaxController(IExgratiaService exgratiaService, IDropdownBindService ddlService, IImportExGratiaService importExService)
        {
            this.exgratiaService = exgratiaService;
            this.ddlService = ddlService;
            this.importExService = importExService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _GetFilter()
        {
            try
            {
                ExgratiaIncomeTaxFilterModel EXVM = new ExgratiaIncomeTaxFilterModel();

                BindDropdowns();
                return PartialView("_Filter", EXVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _ExportTemplate(ExgratiaIncomeTaxFilterModel model)
        {
            log.Info($"ImportBonusData/_ExportTemplate/");
            try
            {
                BindDropdowns();

                if (ModelState.IsValid)
                {
                    string fileName = string.Empty, msg = string.Empty;
                    string fullPath = Server.MapPath("~/FileDownload/");
                    fileName = ExtensionMethods.SetUniqueFileName("Ex-gratia-", FileExtension.xlsx);
                    string fromYear = model.selectYearID.Substring(0, 4) + "04";
                    string toYear = model.selectYearID.Substring(5, 4) + "03";

                    var res = exgratiaService.GetExgratiaExportTemplate(fromYear, toYear, model.selectYearID, (int)model.branchID, model.EmpTypeID, fileName, fullPath);

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

        public ActionResult _ImportExgratiaIncomeTax()
        {
            return PartialView("_ImportExgratiaIncomeTax");
        }
        [HttpPost]
        public ActionResult _ImportExgratiaIncomeTax(HttpPostedFileBase file)
        {
            try
            {
                string fileName = string.Empty; string filePath = string.Empty;
                int error = 0, warning = 0, duplicateEntries = 0;
                List<ImportExgratiaIncomeTaxData> data = new List<ImportExgratiaIncomeTaxData>();
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

                        var objData = importExService.ReadImportExGratiaIncomeTaxExcelData(sPhysicalPath, false, out msg, out error,
                            out warning, out data, out missingHeaders, out columnName);

                        duplicateEntries = objData.GroupBy(x => new { x.EmpCode }).Sum(g => g.Count() - 1);

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
                        }
                        TempData["columns"] = columnName;
                        TempData["InputData"] = dataTable;
                        TempData["ImportExGratiaData"] = data;
                        TempData["error"] = error;
                        TempData["msg"] = msg;
                        TempData["missingHeaders"] = missingHeaders;
                        TempData.Keep("InputData");
                        TempData.Keep("ImportExGratiaData");
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
                    return Json("nofileFound");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public PartialViewResult _ImportExgratiaIncomeTaxGridView()
        {
            try
            {
                ImportExgratiaIncomeTaxVM IncomeTaxImportVM = new ImportExgratiaIncomeTaxVM();
                List<ImportExgratiaIncomeTaxData> fileData = new List<ImportExgratiaIncomeTaxData>();
                IncomeTaxImportVM.ErrorMsgCollection = GetErrorMessage();
                if (TempData["InputData"] != null)
                {
                    IncomeTaxImportVM.importData = (List<ImportExgratiaIncomeTaxData>)TempData["ImportExGratiaData"];
                    IncomeTaxImportVM.inputData = (DataTable)TempData["InputData"];

                    TempData.Keep("InputData");
                    TempData.Keep("ImportExGratiaData");
                }
                else
                    IncomeTaxImportVM.importData = fileData;
                IncomeTaxImportVM.columnName = TempData["columns"] != null ? (List<string>)TempData["columns"] : new List<string>();

                IncomeTaxImportVM.NoOfErrors = TempData["error"] != null ? (int)TempData["error"] : 0;
                IncomeTaxImportVM.Message = TempData["msg"] != null ? Convert.ToString(TempData["msg"]) : "";
                if (IncomeTaxImportVM.Message != "Headers")
                {
                    if (IncomeTaxImportVM.NoOfErrors > 0)
                        IncomeTaxImportVM.Message = string.Format("{0} error(s) found ,Please check the sheet and reupload.", IncomeTaxImportVM.NoOfErrors.ToString());
                    else
                        IncomeTaxImportVM.Message = "Sheet validated successfully, Click on Update button to update your data.";
                }
                else
                {
                    var list = (List<string>)TempData["missingHeaders"];
                    var missingheader = String.Join(",", list);
                    if (list.Count > 1)
                        IncomeTaxImportVM.Message = string.Format("{0} header(s) are missing", missingheader);
                    else
                        IncomeTaxImportVM.Message = string.Format("{0} header(s) is missing", missingheader);
                }
                return PartialView("_ImportExgratiaIncomeTaxGridView", IncomeTaxImportVM);
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
            try
            { 
                var importDataList = (List<ImportExgratiaIncomeTaxData>)TempData["ImportExGratiaData"];
                TempData.Keep("ImportExGratiaData");
                var result = importExService.ImportExGratiaIncomeTaxData(userDetail.UserID, importDataList);
                return Json(new { errorCode = 0, msg = "Ex-Gratia income tax data has been imported sucessfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);

                return Json(new { errorCode = -1, error = ex.Message }, JsonRequestBehavior.AllowGet);
                // throw;
            }

        }
        public void BindDropdowns()
        {

            var reportingYr = ExtensionMethods.GetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
            ViewBag.ddlYear = new SelectList(reportingYr, "Value", "Text");

            List<SelectListItem> selectEmployee = new List<SelectListItem>();
            selectEmployee.Add(new SelectListItem
            { Text = "Select", Value = "0" });
            ViewBag.Employee = selectEmployee;

            var ddlBranchList = ddlService.ddlBranchList();
            Nafed.MicroPay.Model.SelectListModel selectBranch = new Nafed.MicroPay.Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            var ddlEmployeeTypeList = ddlService.ddlEmployeeTypeList();
            ddlEmployeeTypeList.OrderBy(x => x.value);
            Nafed.MicroPay.Model.SelectListModel selectEmployeeType = new Nafed.MicroPay.Model.SelectListModel();
            selectEmployeeType.id = 0;
            selectEmployeeType.value = "Select";
            ddlEmployeeTypeList.Insert(0, selectEmployeeType);
            ViewBag.EmpType = new SelectList(ddlEmployeeTypeList, "id", "value");

        }

    }
}