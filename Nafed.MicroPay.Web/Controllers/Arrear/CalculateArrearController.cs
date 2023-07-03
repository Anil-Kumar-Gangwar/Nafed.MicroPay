using AutoMapper;
using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services.Arrear;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Model = Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.Salary;
using System.Web;
using System.IO;
using static Nafed.MicroPay.Common.FileHelper;

namespace MicroPay.Web.Controllers.Arrear
{
    public class CalculateArrearController : BaseController
    {
        // GET: CalculateArrear
        private readonly IDropdownBindService ddlService;
        private readonly IArrearService arrearService;
        private readonly ISalaryHeadRuleService salHeadRuleService;
        public CalculateArrearController(IDropdownBindService ddlService, IArrearService arrearService, ISalaryHeadRuleService salHeadRuleService)
        {
            this.ddlService = ddlService;
            this.arrearService = arrearService;
            this.salHeadRuleService = salHeadRuleService;
        }
        public ActionResult Index()
        {
            log.Info($"CalculateArrearController/Index");
            BindDropdowns();

            //Formula Editor

            SalaryHead salaryHead = new SalaryHead();

            if (!string.IsNullOrEmpty("E_01"))
            {
                salaryHead = salHeadRuleService.GetSalaryHeadFormulaRule("E_01");
            }

            if (salaryHead != null)
            {
                ///  if (salaryHead.FormulaText)

                if (salaryHead.FormulaColumn)
                    salaryHead.formulaType = FormulaType.FormulaColumn;
                if (salaryHead.FixedValueFormula)
                    salaryHead.formulaType = FormulaType.FixedValueFormula;
                if (salaryHead.Slab)
                    salaryHead.formulaType = FormulaType.Slab;
                if (salaryHead.MonthlyInput)
                    salaryHead.formulaType = FormulaType.MonthlyInput;
            }
            else
                salaryHead = new SalaryHead();
            //


            return View(salaryHead);
        }

        [HttpGet]
        public ActionResult _SalaryHeadFormulaForm(string fieldName)
        {
            log.Info($"SalaryHeadRulesController/GetSalaryHeadFormulaForm");

            SalaryHead salaryHead = new SalaryHead();

            TempData["BaseFormulaData"] = null;
            if (!string.IsNullOrEmpty("E_01"))
            {
                salaryHead = salHeadRuleService.GetSalaryHeadFormulaRule("E_01");
            }

            if (salaryHead != null)
            {
                if (salaryHead.FormulaColumn)
                    salaryHead.formulaType = FormulaType.FormulaColumn;
                if (salaryHead.FixedValueFormula)
                    salaryHead.formulaType = FormulaType.FixedValueFormula;
                if (salaryHead.Slab)
                    salaryHead.formulaType = FormulaType.Slab;
                if (salaryHead.MonthlyInput)
                    salaryHead.formulaType = FormulaType.MonthlyInput;
            }
            else
                salaryHead = new SalaryHead();

            return View(salaryHead);
        }

        private dynamic SalaryHeadFields(string fieldName)
        {
            SalaryHead salaryHead = new SalaryHead();
            IEnumerable<SalaryHeadField> salaryHeadFields = Enumerable.Empty<SalaryHeadField>();
            salaryHeadFields = salHeadRuleService.GetSalaryHeadFields();
            var headFields = salaryHeadFields.Where(x => !x.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase) && x.FieldDesc != "").Select(x => new { x.FieldName, x.FieldDesc }).ToList<dynamic>();
            return headFields;

        }

        [HttpGet]
        public ActionResult _FormulaEditor(string fieldName, int? branchHeadFormulaID = null)
        {
            log.Info($"SalaryHeadRulesController/_FormulaEditor");

            SalaryHeadFormulaEditorVM salHeadFormulaEditor = new SalaryHeadFormulaEditorVM();
            salHeadFormulaEditor.headFields = SalaryHeadFields(fieldName);
            salHeadFormulaEditor.fieldName = fieldName;
            salHeadFormulaEditor.BranchFormulaID = branchHeadFormulaID;
            if (!branchHeadFormulaID.HasValue)
            {
                if (TempData["BaseFormulaData"] != null)
                {
                    salHeadFormulaEditor = (SalaryHeadFormulaEditorVM)TempData["BaseFormulaData"];

                    if (salHeadFormulaEditor.fieldName == fieldName)
                    {
                        List<dynamic> fieldList = new List<dynamic>();
                        foreach (var item in salHeadFormulaEditor.selectedFields)
                        {
                            var selectedValue = item.Trim().Replace(" ", string.Empty);
                            var preffixOperator = selectedValue.Substring(0, 1);
                            var operandFieldName = selectedValue.Substring(1, selectedValue.Length - 1);
                            var operandFieldDesc = preffixOperator + salHeadFormulaEditor.headFields.Where(x => x.FieldName == operandFieldName).FirstOrDefault().FieldDesc;

                            fieldList.Add(new
                            {
                                FieldName = item,
                                FieldDesc = operandFieldDesc
                            });
                        }
                        salHeadFormulaEditor.fieldList = fieldList;
                    }
                    else
                    {
                        salHeadFormulaEditor.headFields = SalaryHeadFields(fieldName);
                        salHeadFormulaEditor.Percentage = null;
                    }
                }
                else
                    salHeadFormulaEditor = GetFormulaEditorFormData(salHeadFormulaEditor);

            }
            else
            {
                if (TempData["ChildFormulaData"] != null)
                {
                    salHeadFormulaEditor = (SalaryHeadFormulaEditorVM)TempData["ChildFormulaData"];

                    if (salHeadFormulaEditor.fieldName == fieldName)
                    {
                        List<dynamic> fieldList = new List<dynamic>();
                        foreach (var item in salHeadFormulaEditor.selectedFields)
                        {
                            var selectedValue = item.Trim().Replace(" ", string.Empty);
                            var preffixOperator = selectedValue.Substring(0, 1);
                            var operandFieldName = selectedValue.Substring(1, selectedValue.Length - 1);
                            var operandFieldDesc = preffixOperator + salHeadFormulaEditor.headFields.Where(x => x.FieldName == operandFieldName).FirstOrDefault().FieldDesc;

                            fieldList.Add(new
                            {
                                FieldName = item,
                                FieldDesc = operandFieldDesc
                            });
                        }
                        salHeadFormulaEditor.fieldList = fieldList;
                    }
                    else
                    {
                        salHeadFormulaEditor.headFields = SalaryHeadFields(fieldName);
                        salHeadFormulaEditor.Percentage = null;
                    }
                }
                else
                    salHeadFormulaEditor = GetFormulaEditorFormData(salHeadFormulaEditor);
            }

            return PartialView("_FormulaEditor", salHeadFormulaEditor);
        }

        [HttpPost]
        public ActionResult _PostFormulaEditor(SalaryHeadFormulaEditorVM salHeadFormulaVM)
        {
            salHeadFormulaVM.headFields = SalaryHeadFields(salHeadFormulaVM.fieldName);

            var formulaExpression = string.Empty;
            var formulaDisplayExpression = string.Empty;

            if (salHeadFormulaVM.selectedFields == null)
            {
                ModelState.AddModelError("SalHeadFieldRequired", "Head Field is required.");
                return PartialView("_FormulaEditor", salHeadFormulaVM);
            }

            var selectedFields = salHeadFormulaVM.selectedFields.ToList<string>();

            foreach (var item in selectedFields)
            {
                var selectedValue = item.Trim().Replace(" ", string.Empty);
                var operandFieldName = selectedValue.Substring(1, selectedValue.Length - 1);

                var operandFieldDesc = salHeadFormulaVM.headFields.Where(x => x.FieldName == operandFieldName).FirstOrDefault().FieldDesc;
                var preffixOperator = item.Substring(0, 1);

                if (selectedFields.IndexOf(item) == 0)
                {
                    formulaExpression += "|" + preffixOperator + operandFieldName + "|";
                    formulaDisplayExpression = "|" + operandFieldDesc + "|";
                }
                else
                {
                    formulaExpression += preffixOperator + "|" + operandFieldName + "|";
                    formulaDisplayExpression += preffixOperator + "|" + operandFieldDesc + "|";
                }
            }

            if (ModelState.IsValid)
            {
                if (!salHeadFormulaVM.BranchFormulaID.HasValue)
                    TempData["BaseFormulaData"] = salHeadFormulaVM;
                else
                    TempData["ChildFormulaData"] = salHeadFormulaVM;

                return Json(new
                {
                    branchFormulaID = salHeadFormulaVM.BranchFormulaID,
                    percentageVal = salHeadFormulaVM.Percentage,
                    formulaExpression = formulaExpression,
                    formulaDisplayExpression = formulaDisplayExpression
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return PartialView("_FormulaEditor", salHeadFormulaVM);
        }

        public SalaryHeadFormulaEditorVM GetFormulaEditorFormData(SalaryHeadFormulaEditorVM frmEditorModel)
        {
            string lookUpHeadName = string.Empty, lookUpHead = string.Empty;

            if (frmEditorModel.BranchFormulaID.HasValue && frmEditorModel.BranchFormulaID.Value > 0)
            {
                var branchHeadFieldRule = salHeadRuleService.GetBranchHeadFieldRule(frmEditorModel.BranchFormulaID.Value);
                lookUpHeadName = branchHeadFieldRule.LookUpHeadName;
                lookUpHead = branchHeadFieldRule.LookUpHead;
            }
            else
            {
                var headRules = salHeadRuleService.GetSalaryHeadFormulaRule(frmEditorModel.fieldName);
                lookUpHeadName = headRules?.LookUpHeadName ?? string.Empty;
                lookUpHead = headRules?.LookUpHead ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(lookUpHeadName))
            {
                decimal per;
                bool isValid = decimal.TryParse((lookUpHeadName.Split(new char[] { '%' }))[0].Trim(), out per);
                frmEditorModel.Percentage = isValid ? (decimal?)per : null;


                var salaryFields = (lookUpHeadName.Split(new char[] { '(' }))[1].Trim(')');
                var selectedFields = salaryFields.Split(new char[] { '|' }).ToList<string>();

                selectedFields.RemoveAt(0);
                selectedFields.RemoveAt(selectedFields.Count - 1);

                var firstItem = selectedFields.First().ToString();
                selectedFields.RemoveAt(0);
                var odds = selectedFields.Where((item, index) => index % 2 != 0).ToList();
                var evens = selectedFields.Where((item, index) => index % 2 == 0).ToList();


                List<dynamic> fieldList = new List<dynamic>();

                #region firstItem

                bool hasStartingOperator = true;

                var firstItemOperator = firstItem.Substring(0, 1);
                if (firstItemOperator != "+" && firstItemOperator != "-")
                {
                    hasStartingOperator = false;
                    firstItem = "+" + firstItem;
                }
                var firstItemFieldName = firstItem.Substring(1, firstItem.Length - 1);
                fieldList.Add(new
                {

                    FieldName = firstItem,
                    FieldDesc = (hasStartingOperator ? firstItemOperator : "+") + frmEditorModel.headFields.Where(x => x.FieldName == firstItemFieldName).FirstOrDefault().FieldDesc

                });
                #endregion

                for (int i = 0; i < odds.Count(); i++)
                {
                    var operandFieldName = evens[i];
                    var operandFieldDesc = operandFieldName + frmEditorModel.headFields.Where(x => x.FieldName == odds[i]).FirstOrDefault().FieldDesc;

                    fieldList.Add(new
                    {
                        FieldName = operandFieldName + odds[i],
                        FieldDesc = operandFieldDesc
                    });
                }

                frmEditorModel.fieldList = fieldList;
            }
            return frmEditorModel;
        }

        public ActionResult _FillFilterEmployeeList(int branchID, DateTime fromdate, DateTime todate)

        {
            log.Info($"AdvanceSearchController/_FillFilterEmployeeList/{branchID}");

            IEnumerable<SelectListModel> fields = Enumerable.Empty<SelectListModel>();
            var model = new CheckBoxListViewModel();
            try
            {
                string fromYear = fromdate.Year.ToString() + (fromdate.Month.ToString().Length == 1 ? "0" + fromdate.Month.ToString() : fromdate.Month.ToString()) + (fromdate.Day.ToString().Length == 1 ? "0" + fromdate.Day.ToString() : fromdate.Day.ToString());
                string toYear = todate.Year.ToString() + (todate.Month.ToString().Length == 1 ? "0" + todate.Month.ToString() : todate.Month.ToString()) + (todate.Day.ToString().Length == 1 ? "0" + todate.Day.ToString() : todate.Day.ToString());
                var selectedCheckboxes = new List<CheckBox>();
                var getFieldsValue = arrearService.getemployeeByBranchID(branchID, fromYear, toYear, "0");

                List<SelectListModel> distinctFieldsValue = getFieldsValue
                    .GroupBy(m => new { m.id, m.value })
                    .Select(group => group.First())
                    .ToList();

                Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                model.AvailableFields = Mapper.Map<List<CheckBox>>(distinctFieldsValue);
                model.SelectedFields = selectedCheckboxes;
                return PartialView("_FilterEmployeeList", model);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            // return PartialView("_FilterFieldList", model);
        }

        [HttpPost]
        public ActionResult PostFilterFieldValues(HttpPostedFileBase file, CheckBoxListViewModel chklistVM, AdvanceSearchDateFilter dateFilter, FormCollection frm, string ButtonType, SalaryHead obj)
        {
            log.Info($"CalculateArrearController/PostFilterFieldValues");
            try
            {
                string fileName = string.Empty, msg = string.Empty, fullPath = string.Empty;
                var ArrearType = "";
                int arrPeriods = 0;
                DateTime fromdate = Convert.ToDateTime(frm.Get("FromDate"));
                DateTime todate = Convert.ToDateTime(frm.Get("ToDate"));
                int VPFrate = Convert.ToInt32(frm.Get("VPFrate"));
                var branchID = int.Parse(frm.Get("ddlbranch"));
                int arrearType = int.Parse(frm.Get("arrearType"));
                if (ButtonType == "Select")
                {
                    if (chklistVM.PostedFields?.fieldIds.Count() > 0)
                    {
                        return Json(new { formAction = ButtonType, selectedFields = chklistVM.PostedFields.fieldIds }, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (ButtonType == "Export Template")
                {
                    string path = string.Empty, fileextension = string.Empty;
                    log.Info("ImportEmpSalaryManualDataController/_ExportTemplate");
                    if (fromdate != null && todate != null)
                    {

                        string fromDate = fromdate.Year.ToString() + (fromdate.Month.ToString().Length == 1 ? "0" + fromdate.Month.ToString() : fromdate.Month.ToString()) + (fromdate.Day.ToString().Length == 1 ? "0" + fromdate.Day.ToString() : fromdate.Day.ToString());
                        string toDate = todate.Year.ToString() + (todate.Month.ToString().Length == 1 ? "0" + todate.Month.ToString() : todate.Month.ToString()) + (todate.Day.ToString().Length == 1 ? "0" + todate.Day.ToString() : todate.Day.ToString());

                        fullPath = Server.MapPath("~/FileDownload/");
                        fileName = ExtensionMethods.SetUniqueFileName("PayArrearDataSheet-", FileExtension.xlsx);
                        Response.ContentType = "application/vnd.ms-excel";

                        string employeecode = string.Empty;
                        if (Convert.ToInt32(chklistVM.PostedFields?.fieldIds.Count()) > 0)
                        {
                            //getselectedEmployee
                            employeecode = arrearService.getSelectedEmployeecode(Convert.ToInt32(chklistVM.PostedFields?.fieldIds.Count()), chklistVM.PostedFields.fieldIds);
                            var res = arrearService.GetPayArrearentryform(0, fromDate, toDate, fileName, fullPath, employeecode);
                            path = Path.Combine(Server.MapPath("~/FileDownload/"), fileName);
                            if (res == "norec")
                                return Json(new { fileName = "", message = "error" });
                        }
                        else
                        {
                            var res = arrearService.GetPayArrearentryform(0, fromDate, toDate, fileName, fullPath, "0");
                            path = Path.Combine(Server.MapPath("~/FileDownload/"), fileName);
                            if (res == "norec")
                                return Json(new { fileName = "", message = "error" });
                        }

                    }
                    return Json(new { fileName = fileName, fullPath = fullPath + fileName, message = "success" });
                }
                else if (ButtonType == "Calculate Arrear")
                {
                    //SalaryHead salaryHead = new SalaryHead();
                    //salaryHead = salHeadRuleService.GetSalaryHeadFormulaRule("E_01");
                    string fromYear = fromdate.Year.ToString() + (fromdate.Month.ToString().Length == 1 ? "0" + fromdate.Month.ToString() : fromdate.Month.ToString());
                    string toYear = todate.Year.ToString() + (todate.Month.ToString().Length == 1 ? "0" + todate.Month.ToString() : todate.Month.ToString());
                    var searchResult = arrearService.GetMultipleTableDataResult(fromYear, toYear);
                    arrPeriods = ((fromdate.Year - todate.Year) * 12) + (todate.Month - fromdate.Month) + 1;
                    if (arrearType == -1)
                    {
                        if (chklistVM.PostedFields?.fieldIds.Count() > 0)
                        {
                            if (branchID >= 0)
                            {
                                string formula = obj.LookUpHeadName;
                                //string formula = salaryHead.LookUpHeadName;
                                ArrearType = "DA";
                                int calarrear = arrearService.CalculateDAArrear(Convert.ToInt32(chklistVM.PostedFields?.fieldIds.Count()), chklistVM.PostedFields.fieldIds, ArrearType, arrPeriods, searchResult, branchID, fromdate, todate, VPFrate, formula);
                            }
                        }
                    }
                    else if (arrearType == 2)
                    {
                        var importDataList = (List<ArrearManualData>)TempData["manualData"];
                        DataTable DT = arrearService.ImportPayArrearDataDetails(importDataList);
                        ArrearType = "Basic";
                        int calarrear = arrearService.CalculatePayArrear(Convert.ToInt32(DT.Rows.Count), DT, ArrearType, arrPeriods, searchResult, branchID, fromdate, todate, VPFrate);
                    }
                }

                return Json(new { formAction = ButtonType }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public PartialViewResult ImportManualDataGridView()
        {
            log.Info($"CalculateArrearController/ImportManualDataGridView");
            try
            {
                AttendanceImportViewModel attendanceImportVM = new AttendanceImportViewModel();
                List<EmpAttendanceForm> empAttandanceForm = new List<EmpAttendanceForm>();
                attendanceImportVM.ErrorMsgCollection = GetErrorMessage();
                if (TempData["attendanceData"] != null)
                {
                    attendanceImportVM.attendanceData = (List<EmpAttendanceForm>)TempData["attendanceData"];
                    TempData.Keep("attendanceData");
                    //attendanceImportVM.NoOfErrors = TempData["error"] != null ? (int)TempData["error"] : 0;
                }
                else
                    attendanceImportVM.attendanceData = empAttandanceForm;
                attendanceImportVM.NoOfErrors = TempData["error"] != null ? (int)TempData["error"] : 0;

                return PartialView("_ImportAttendanceGridView", attendanceImportVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            var ddlBranchList = ddlService.ddlBranchList();
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            Model.SelectListModel selectBranch1 = new Model.SelectListModel();
            selectBranch.id = -1;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);

            selectBranch1.id = 0;
            selectBranch1.value = "All Branch Except HO";
            ddlBranchList.Insert(1, selectBranch1);

            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

        }

        [HttpPost]
        public ActionResult uploadFile(FormCollection frm)
        {

            log.Info($"check file exists");
            // check if the user selected a file to upload
            if (Request.Files.Count > 0)
            {
                int error = 0, warning = 0, duplicateEntries;
                string filePath = string.Empty;
                string msg = string.Empty;
                try
                {
                    HttpFileCollectionBase files = Request.Files;

                    HttpPostedFileBase file = files[0];
                    string fileName = file.FileName;
                    log.Info($"entry point");
                    if (file != null)
                    {
                        string fileExtension = Path.GetExtension(file.FileName);
                        fileName = Path.GetFileName(file.FileName);
                        var contentType = GetFileContentType(file.InputStream);
                        var dicValue = GetDictionaryValueByKeyName(".xlsx");
                        if (dicValue == contentType && (fileExtension == ".xlsx" || fileExtension == ".xlx"))
                        {
                            log.Info($"before file uplaod");
                            #region Upload File At temp location===
                            fileName = ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(file.FileName),
                                   Path.GetExtension(file.FileName));

                            filePath = DocumentUploadFilePath.MonthlyImportFilePath;
                            string uploadedFilePath = Server.MapPath(filePath);
                            string sPhysicalPath = Path.Combine(uploadedFilePath, fileName);
                            log.Info($"after file uplaod/{sPhysicalPath}");
                            file.SaveAs(sPhysicalPath);
                            log.Info($"after 1 file uplaod/{sPhysicalPath}");
                            #endregion

                            var objManualData = arrearService.ReadArrearImportExcelData(sPhysicalPath, false, out msg, out error, out warning);

                            duplicateEntries = objManualData.GroupBy(x => new { x.EmployeeId }).Sum(g => g.Count() - 1);
                            var getDuplicateRows = ExtensionMethods.FindDuplicates(objManualData, x => new { x.EmployeeId });
                            objManualData.Join(getDuplicateRows, (x) => new { x.EmployeeId }, (y) => new { y.EmployeeId }, (x, y) =>
                            {
                                x.isDuplicatedRow = (((x.EmployeeId == y.EmployeeId)) ? true : false);
                                return x;
                            }).ToList();

                            if (error == 0 && duplicateEntries > 0)
                                error = duplicateEntries;
                            else
                                error += duplicateEntries;

                            TempData["manualData"] = objManualData;
                            TempData["error"] = error;
                            TempData.Keep("manualData");
                            TempData.Keep("error");

                            return Json("Sheet validated successfully, Press calculate arrear button to generate arrear.");
                        }
                        else
                        {
                            TempData["error"] = -2;
                            return Json("inValidFileFormat");
                        }

                    }
                }

                catch (Exception e)
                {
                    return Json("error" + e.Message);
                }
            }

            return Json("no files were selected !");
        }
    }
}