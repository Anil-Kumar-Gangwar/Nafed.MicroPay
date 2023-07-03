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
    public class ImportMonthlyInputController : BaseController
    {
        private readonly IDropdownBindService dropdownService;
        private readonly IImportMonthlyInputService importMonthlyInput;
        private readonly ISalaryHeadRuleService salaryService;

        public ImportMonthlyInputController(IDropdownBindService dropdownService, IImportMonthlyInputService importMonthlyInput, ISalaryHeadRuleService salaryService)
        {
            this.dropdownService = dropdownService;
            this.importMonthlyInput = importMonthlyInput;
            this.salaryService = salaryService;
        }

        public ActionResult Index()
        {
            log.Info("ImportMonthlyInput/Index");
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

        private void LoadBranch()
        {
            log.Info("ImportMonthlyInputController/LoadBranch");

            var ddlBranchList = dropdownService.ddlBranchList().ToList();
            SelectListModel selectBranch = new SelectListModel();
            //selectBranch.id = 0;
            //selectBranch.value = "Select";
            //ddlBranchList.Insert(0, selectBranch);
            selectBranch.id = 0;
            selectBranch.value = "All Branches Except Head Office";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            var ddlEmployeeType = dropdownService.ddlEmployeeTypeList().ToList();
            SelectListModel employeeType = new SelectListModel();
            employeeType.id = 0;
            employeeType.value = "Select";
            ddlEmployeeType.Insert(0, employeeType);
            ViewBag.EmployeeType = new SelectList(ddlEmployeeType, "id", "value");
        }

        public ActionResult _ExportTemplate()
        {
            log.Info("ImportMonthlyInputController/_ExportTemplate");
            LoadBranch();
            return PartialView("_ExportTemplate");
        }

        public ActionResult _ImportMonthlyInput()
        {
            log.Info("ImportMonthlyInputController/_ImportAttendance");
            return PartialView("_ImportMonthlyInputData");
        }

        [HttpPost]
        public ActionResult _ExportTemplate(FormCollection frm)
        {
            log.Info("ImportMonthlyInputController/_ExportTemplate");
            var branchID = Request.Form["BranchID"] == "" ? "0" : Request.Form["BranchID"];
            var month = Convert.ToInt32(Request.Form["ddlMonth"]) == 0 ? null : (int?)Convert.ToInt32(Request.Form["ddlMonth"]);
            var year = Convert.ToInt32(Request.Form["ddlYear"]) == 0 ? null : (int?)Convert.ToInt32(Request.Form["ddlYear"]);
            var employeeTypeId = Convert.ToInt32(Request.Form["ddlEmployeeType"]) == 0 ? null : (int?)Convert.ToInt32(Request.Form["ddlEmployeeType"]);
            bool flag = false;
            if (employeeTypeId != null/* && branchID != "0"*/)
            {
                TempData["employeeTypeId"] = employeeTypeId.Value;
                TempData.Keep("employeeTypeId");
                string fileName = string.Empty, msg = string.Empty;
                string fullPath = Server.MapPath("~/FileDownload/");
                fileName = ExtensionMethods.SetUniqueFileName("MonthlyInput-", FileExtension.xlsx);
                int PreMonth, PreYear;
                var currMonth = DateTime.Now.Month;
                var currYear = DateTime.Now.Year;
                var cmbCurrent = Convert.ToString(currYear) + (Convert.ToString(currMonth).Length == 1 ? "0" + Convert.ToString(currMonth) : Convert.ToString(currMonth));

                if (month == 1)
                {
                    PreMonth = 12;
                    PreYear = year.Value - 1;
                }
                else
                {
                    PreMonth = month.Value - 1;
                    PreYear = year.Value;
                }
                var cmbSelected = Convert.ToString(year.Value) + (Convert.ToString(month.Value).Length == 1 ? "0" + Convert.ToString(month.Value) : Convert.ToString(month.Value));

                if (Convert.ToInt32(cmbSelected) <= Convert.ToInt32(cmbCurrent))
                    flag = salaryService.CheckMonthlyInputDetails(year.Value, month.Value, PreYear, PreMonth);
                else
                    return Json(new { fileName = fileName, fullPath = fullPath + fileName, message = "error", AlertMsg = "Selected period is from future date,please select current month period or previous month." });

                var res = importMonthlyInput.GetMonthlyInputForm(int.Parse(branchID), month ?? 0, year ?? 0, fileName, fullPath, employeeTypeId);
                return Json(new { fileName = fileName, fullPath = fullPath + fileName, message = "success", AlertMsg = res });
            }
            else
            {
                LoadBranch();
                ModelState.AddModelError("EmployeeTypeRequired", "Select Employee Type");
                ModelState.AddModelError("BranchRequired", "Select Branch");
                return PartialView("_ExportTemplate");
            }
        }

        [HttpPost]
        public ActionResult _ImportMonthlyInputData(HttpPostedFileBase file)
        {
            log.Info("ImportMonthlyInputController/_ImportMonthlyInputData");
            try
            {
                string fileName = string.Empty; string filePath = string.Empty;
                int error = 0, warning = 0, duplicateEntries = 0;
                List<ImportMonthlyInput> importMonthlyInputSave = new List<ImportMonthlyInput>();
                List<string> columnName = new List<string>();
                List<string> missingHeaders;
                string msg = string.Empty;
                int employeeTypeId = 0;              
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

                        var objMonthlyInput = importMonthlyInput.ReadImportMonthlyExcelData(sPhysicalPath, false, out msg, out error, out warning, out importMonthlyInputSave, out employeeTypeId, out missingHeaders, out columnName);

                        duplicateEntries = objMonthlyInput.GroupBy(x => new { x.EmployeeId, x.Month, x.Year }).Sum(g => g.Count() - 1);
                        var getDuplicateRows = ExtensionMethods.FindDuplicates(objMonthlyInput.ToList(), x => new { x.EmployeeId, x.Month, x.Year });
                        objMonthlyInput.Join(getDuplicateRows, (x) => new { x.EmployeeId, x.Month, x.Year }, (y) => new { y.EmployeeId, y.Month, y.Year }, (x, y) =>
                        {
                            x.isDuplicatedRow = (((x.EmployeeId == y.EmployeeId) && (x.Month == y.Month) && (x.Year == y.Year)) ? true : false);
                            return x;
                        }).ToList();

                        if (error == 0 && duplicateEntries > 0)
                            error = duplicateEntries;
                        else
                            error += duplicateEntries;

                        var json = JsonConvert.SerializeObject(objMonthlyInput);
                        DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

                        var salheadFields = importMonthlyInput.GetHeadsFields(employeeTypeId);
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            for (int j = 0; j < salheadFields.Count; j++)
                            {
                                if (dataTable.Columns[i].ColumnName.ToLower().ToString() == salheadFields[j].FieldName.ToLower().ToString())
                                {
                                    dataTable.Columns[i].ColumnName = salheadFields[j].FieldDesc;
                                }
                            }
                        }
                        if (msg != "Headers")
                        {
                            var count = columnName.Count;
                            columnName.Insert(count, "isDuplicatedRow");
                            int columnIndex = 0;
                            var newList = columnName.Select(s => s.Replace("#", "Sno")).ToList();
                            foreach (var col in newList)
                            {
                                dataTable.Columns[col].SetOrdinal(columnIndex);
                                columnIndex++;
                            }
                        }

                        TempData["MonthlyInputData"] = dataTable;
                        TempData["MonthlyInputSave"] = importMonthlyInputSave;
                        TempData["error"] = error;
                        TempData["msg"] = msg;
                        TempData["missingHeaders"] = missingHeaders;
                        TempData.Keep("MonthlyInputData");
                        TempData.Keep("MonthlyInputSave");
                        TempData.Keep("error");
                        TempData.Keep("msg");
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
        public PartialViewResult ImportMonthlyGridView()
        {
            log.Info($"ImportMonthlyInputController/ImportMonthlyGridView");
            try
            {
                ImportMonthlyInputViewModel monthlyImportVM = new ImportMonthlyInputViewModel();
                List<ImportMonthlyInput> monthlyInputForm = new List<ImportMonthlyInput>();
                monthlyImportVM.ErrorMsgCollection = GetErrorMessage();
                if (TempData["MonthlyInputData"] != null)
                {
                    //monthlyImportVM.monthlyInputData = (List<ImportMonthlyInput>)TempData["MonthlyInputData"];
                    monthlyImportVM.monthlyInput = (DataTable)TempData["MonthlyInputData"];
                    TempData.Keep("MonthlyInputData");
                }
                else
                    monthlyImportVM.monthlyInputData = monthlyInputForm;

                monthlyImportVM.NoOfErrors = TempData["error"] != null ? (int)TempData["error"] : 0;
                monthlyImportVM.Message = TempData["msg"] != null ? Convert.ToString(TempData["msg"]) : "";
                if (monthlyImportVM.Message != "Headers")
                {
                    if (monthlyImportVM.NoOfErrors > 0)
                        monthlyImportVM.Message = string.Format("{0} error(s) found ,Please check the sheet and reupload.", monthlyImportVM.NoOfErrors.ToString());
                    else
                        monthlyImportVM.Message = "Sheet validated successfully, Click on Update button to update your data.";
                }
                else
                {
                    var list = (List<string>)TempData["missingHeaders"];
                    var missingheader = String.Join(",", list);
                    if (list.Count > 1)
                        monthlyImportVM.Message = string.Format("{0} header(s) are missing", missingheader);
                    else
                        monthlyImportVM.Message = string.Format("{0} header(s) is missing", missingheader);
                }
                return PartialView("_ImportMonthlyGridView", monthlyImportVM);
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
            log.Info("ImportMonthlyInputController/SubmitFileData");
            try
            {
                //var importDataList = (List<ImportMonthlyInput>)TempData["MonthlyInputData"];
                var importDataList = (List<ImportMonthlyInput>)TempData["MonthlyInputSave"];
                TempData.Keep("MonthlyInputSave");
                var result = importMonthlyInput.ImportMonthlyInputDetails(userDetail.UserID, importDataList);
                return Json(new { errorCode = 0, msg = "Monthly input data has been imported sucessfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);

                return Json(new { errorCode = -1, error = ex.Message }, JsonRequestBehavior.AllowGet);
                // throw;
            }

        }
    }
}