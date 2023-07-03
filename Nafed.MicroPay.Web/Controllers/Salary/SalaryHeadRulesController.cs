using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.Salary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using AutoMapper;

namespace MicroPay.Web.Controllers.Salary
{
    public class SalaryHeadRulesController : BaseController
    {
        // GET: SalaryHeadRules
        private readonly ISalaryHeadRuleService salHeadRuleService;
        private readonly IDropdownBindService dropdownBindService;
        public SalaryHeadRulesController(ISalaryHeadRuleService salHeadRuleService, IDropdownBindService dropdownBindService)
        {
            this.salHeadRuleService = salHeadRuleService;
            this.dropdownBindService = dropdownBindService;
        }

        public ActionResult Index()
        {
            log.Info($"SalaryHeadRulesController/Index");

            SalaryHeadViewModel salHeadVM = new SalaryHeadViewModel();
            IEnumerable<SalaryHeadField> salaryHeadFields = Enumerable.Empty<SalaryHeadField>();
            try
            {
                salaryHeadFields = salHeadRuleService.GetSalaryHeadFields();
                salHeadVM.fieldList = salaryHeadFields.Where(x => x.FieldName != "E_Basic"); //=== Exculded Basic Salary Field from Head Field list===
                salHeadVM.employeeType = dropdownBindService.ddlEmployeeTypeList();
            }

            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return View(salHeadVM);
        }

        [HttpGet]
        public ActionResult GetSalaryHeadFields()
        {
            log.Info($"SalaryHeadRulesController/Index");
            IEnumerable<SalaryHeadField> salaryHeadFields = Enumerable.Empty<SalaryHeadField>();
            try
            {
                salaryHeadFields = salHeadRuleService.GetSalaryHeadFields();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return PartialView("_SalaryHeadFields", salaryHeadFields);
        }

        [HttpGet]
        public ActionResult _SalaryHeadFormulaForm(string fieldName,int ? employeeTypeID)
        {
            log.Info($"SalaryHeadRulesController/GetSalaryHeadFormulaForm");

            SalaryHead salaryHead = new SalaryHead();
            salaryHead.EmployeeTypeID = employeeTypeID;

            TempData["BaseFormulaData"] = null;
            if (!string.IsNullOrEmpty(fieldName))
            {
                salaryHead = salHeadRuleService.GetSalaryHeadFormulaRule(fieldName, employeeTypeID);
            }

            if (salaryHead != null)
            {
                if (salaryHead.FormulaColumn)
                    salaryHead.formulaType = FormulaType.FormulaColumn;
                if (salaryHead.FixedValueFormula)
                    salaryHead.formulaType = FormulaType.FixedValueFormula;
                //if (salaryHead.Slab)
                //    salaryHead.formulaType = FormulaType.Slab;
                if (salaryHead.MonthlyInput)
                    salaryHead.formulaType = FormulaType.MonthlyInput;
            }
            else
                salaryHead = new SalaryHead();

            return PartialView("_SalaryHeadFormulaForm", salaryHead);
        }

        [HttpPost]
        public ActionResult _SalaryHeadFormulaForm(SalaryHead salHead)
        {
            log.Info($"SalaryHeadRulesController/GetSalaryHeadFormulaForm");
            try
            {
                if (salHead.formulaType == FormulaType.FormulaColumn)
                    salHead.FormulaColumn = true;
                //if (salHead.formulaType == FormulaType.Slab)
                //    salHead.Slab = true;
                if (salHead.formulaType == FormulaType.MonthlyInput)
                    salHead.MonthlyInput = true;
                if (salHead.formulaType == FormulaType.FixedValueFormula)
                    salHead.FixedValueFormula = true;

                if(salHead.formulaType==0)
                {
                    ModelState.AddModelError("formulaTypeRequired", "Please select formula type.");
                    return PartialView("_SalaryHeadFormulaForm", salHead);
                }

                if(salHead.LowerRange.HasValue && salHead.UpperRange.HasValue)
                {
                    if (salHead.LowerRange.Value > salHead.UpperRange.Value)
                    {
                        ModelState.AddModelError("RangeError", "Lower Range cannot be greater than Upper Range.");
                        return PartialView("_SalaryHeadFormulaForm", salHead);
                    }
                }

                if (salHead.CreatedOn == default(DateTime) && string.IsNullOrEmpty(salHead.ActionType))
                {
                    salHead.CreatedOn = DateTime.Now;
                    salHead.CreatedBy = userDetail.UserID;
                    salHead.ActionType = "Insert";
                }
                else
                {
                    salHead.UpdatedBy = userDetail.UserID;
                    salHead.UpdatedOn = DateTime.Now;
                    salHead.ActionType = "Update";
                }

                if (ModelState.IsValid)
                {
                    var result = salHeadRuleService.PostSalaryFormData(salHead);

                    if (result)
                    {
                        if (salHead.ActionType == "Insert")
                        {
                            return Json(new
                            {
                                actionType = salHead.ActionType,
                                fieldName = salHead.FieldName,
                                EmployeeTypeID = salHead.EmployeeTypeID,
                                type = "success",
                                msg = "Salary Head Rules created successfully. ",

                                //,
                                //createdBy = salHead.CreatedBy,
                                //createdOn = salHead.CreatedOn,
                                //AjaxReturn = ConvertViewToString("_SalaryHeadFormulaForm", salHead)
                            }, JsonRequestBehavior.AllowGet);
                        }
                        if (salHead.ActionType == "Update")
                        {
                            return Json(new
                            {
                                actionType = salHead.ActionType,
                                fieldName = salHead.FieldName,
                                EmployeeTypeID = salHead.EmployeeTypeID,
                                type = "success",
                                msg = "Salary Head Rules updated successfully. "
                                //,
                                //createdBy = salHead.CreatedBy,
                                //createdOn = salHead.CreatedOn,
                                //AjaxReturn = ConvertViewToString("_SalaryHeadFormulaForm", salHead)
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                return PartialView("_SalaryHeadFormulaForm", salHead);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _LocationDependentFormula(int ? employeeTypeID,string fieldName, FormulaType formulaType, string lookUpHead, string lookUpHeadName, decimal? fixedValue)
        {
            log.Info($"SalaryHeadRulesController/_LocationDepenedentFormulaPopup");
            try
            {
                TempData["ChildFormulaData"] = null;

                SalaryHead salaryHead = new SalaryHead();
                List<SelectListModel> branchList = new List<SelectListModel>();
                branchList = dropdownBindService.ddlBranchList();
                salaryHead.BranchList = branchList;
                salaryHead.EmployeeTypeID = employeeTypeID;
                salaryHead.formulaType = formulaType;
                salaryHead.FixedValue = fixedValue ?? 0;

                if (formulaType == FormulaType.FormulaColumn)
                    salaryHead.FormulaColumn = true;
                if (formulaType == FormulaType.FixedValueFormula)
                    salaryHead.FixedValueFormula = true;
                salaryHead.LookUpHead = lookUpHead;
                salaryHead.LookUpHeadName = lookUpHeadName;

                salaryHead.FieldName = fieldName;
                salaryHead.BranchFormulaID = 0;

                return PartialView("_LocationDependentFormulaPopup", salaryHead);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _LocationDependentFormula(SalaryHead salHead)
        {
            log.Info($"SalaryHeadRulesController/_LocationDependentFormula");
            try
            {
                var branchLookUpHeadName = Request.Form["BranchLookUpHeadName"];
                var branchLookUpHead= Request.Form["BranchLookUpHead"];

                ModelState.Remove("SeqNo");

                List<SelectListModel> branchList = new List<SelectListModel>();
                branchList = dropdownBindService.ddlBranchList();
                salHead.BranchList = branchList;

                if (!string.IsNullOrEmpty(branchLookUpHeadName))
                    salHead.LookUpHeadName = branchLookUpHeadName;

                if (!string.IsNullOrEmpty(branchLookUpHead))
                    salHead.LookUpHead = branchLookUpHead;

                if (salHead.formulaType == FormulaType.FormulaColumn)
                    salHead.FormulaColumn = true;
                if (salHead.formulaType == FormulaType.FixedValueFormula)
                    salHead.FixedValueFormula = true;

                if (salHead.SelectedBranchID == 0)
                {
                    ModelState.AddModelError("branchIDRequired", "Please select branch .");
                    return PartialView("_LocationFormulaEditor", salHead);
                }

                if (ModelState.IsValid)
                {
                    if (salHead.CreatedOn == default(DateTime) && string.IsNullOrEmpty(salHead.ActionType))
                    {
                        salHead.CreatedOn = DateTime.Now;
                        salHead.CreatedBy = userDetail.UserID;
                        salHead.ActionType = "Insert";

                        var isBranchHeadFieldRuleExists = salHeadRuleService.IsBranchHeadRuleExists(salHead.FieldName, salHead.SelectedBranchID,salHead.EmployeeTypeID);
                        if (isBranchHeadFieldRuleExists)
                        {
                            ModelState.AddModelError("branchHeadFieldRuleExists", "Branch Head field rule already exists.");
                            return PartialView("_LocationFormulaEditor", salHead);
                        }
                    }
                    else
                    {
                        salHead.UpdatedBy = userDetail.UserID;
                        salHead.UpdatedOn = DateTime.Now;
                        salHead.ActionType = "Update";
                    }

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<SalaryHead, BranchSalaryHeadRule>()
                        .ForMember(d=>d.BranchFormulaID,o=>o.MapFrom(s=>(int)s.BranchFormulaID))
                        .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
                        .ForMember(d => d.BranchID, o => o.MapFrom(s => s.SelectedBranchID))
                        .ForMember(d => d.FormulaColumn, o => o.MapFrom(s => s.FormulaColumn))
                        .ForMember(d => d.FixedValueFormula, o => o.MapFrom(s => s.FixedValueFormula))
                        .ForMember(d => d.LookUpHead, o => o.MapFrom(s => s.LookUpHead))
                        .ForMember(d => d.LookUpHeadName, o => o.MapFrom(s => s.LookUpHeadName))
                        .ForMember(d => d.FixedValue, o => o.MapFrom(s => s.FixedValue))
                        .ForMember(d => d.LowerRange, o => o.MapFrom(s => s.LowerRange))
                        .ForMember(d => d.UpperRange, o => o.MapFrom(s => s.UpperRange))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                        .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                        .ForMember(d => d.ActionType, o => o.MapFrom(s => s.ActionType))
                         .ForMember(d => d.EmployeeTypeID, o => o.MapFrom(s => s.EmployeeTypeID))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    var branchSalRule = Mapper.Map<BranchSalaryHeadRule>(salHead);

                    var result = salHeadRuleService.PostBranchHeadRule(branchSalRule);

                    if (result)
                    {
                        if (branchSalRule.ActionType == "Insert")
                        {
                            return Json(new
                            {
                                actionType = branchSalRule.ActionType,
                                fieldName = branchSalRule.FieldName,
                                type = "success",
                                msg = "Branch Head Rules created successfully. "
                            //   , formulaForm= /*PartialView("_LocationFormulaEditor", salHead)*/ ConvertViewToString("_LocationFormulaEditor", salHead)
                            }, JsonRequestBehavior.AllowGet);
                        }
                        if (branchSalRule.ActionType == "Update")
                        {
                            return Json(new
                            {
                                actionType = branchSalRule.ActionType,
                                fieldName = branchSalRule.FieldName,
                                type = "success",
                                msg = "Salary Head Rules updated successfully. "
                             // ,         formulaForm = /*PartialView("_LocationFormulaEditor", salHead)*/ ConvertViewToString("_LocationFormulaEditor", salHead)
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return PartialView("_LocationFormulaEditor", salHead);
                // return PartialView("_LocationFormulaEditor", salHead);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private dynamic SalaryHeadFields(string fieldName,int ? employeeTypeID=null)
        { 
            SalaryHead salaryHead = new SalaryHead();

            IEnumerable<SalaryHeadField> salaryHeadFields = Enumerable.Empty<SalaryHeadField>();
            salaryHeadFields = salHeadRuleService.GetSalaryHeadFields(employeeTypeID);

            var headFields = salaryHeadFields.Where(x => !x.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase) 
                  && x.FieldDesc != "").Select(x => new { x.FieldName, x.FieldDesc }).ToList<dynamic>();

            return headFields;
        }

        [HttpGet]
        public ActionResult _FormulaEditor(string fieldName, int? branchHeadFormulaID = null, int ? employeeTypeID=null)
        {
            log.Info($"SalaryHeadRulesController/_FormulaEditor");

            SalaryHeadFormulaEditorVM salHeadFormulaEditor = new SalaryHeadFormulaEditorVM();
            salHeadFormulaEditor.headFields = SalaryHeadFields(fieldName, employeeTypeID);
            salHeadFormulaEditor.fieldName = fieldName;
            salHeadFormulaEditor.EmployeeTypeID = employeeTypeID;
            salHeadFormulaEditor.BranchFormulaID = branchHeadFormulaID;
            salHeadFormulaEditor.EmployeeTypeID = employeeTypeID;

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
                        salHeadFormulaEditor.headFields = SalaryHeadFields(fieldName, employeeTypeID);
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
                        salHeadFormulaEditor.headFields = SalaryHeadFields(fieldName, employeeTypeID);
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
            salHeadFormulaVM.headFields = SalaryHeadFields(salHeadFormulaVM.fieldName,salHeadFormulaVM.EmployeeTypeID);

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

        [HttpGet]
        public ActionResult _GetBranchSalaryHeadRules(string fieldName, int ? employeeTypeID)
        {
            log.Info($"SalaryHeadRulesController/_GetBranchSalaryHeadRules");

            try
            {
                var bHeadRules = salHeadRuleService.GetBranchSalaryHeadRules(fieldName, employeeTypeID);
                return PartialView("_BranchSalaryHeadRules", bHeadRules);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult DeleteBranchHeadFieldRule(int branchFormulaID,string fieldName)
        {
            log.Info($"SalaryHeadRulesController/DeleteBranchHeadFieldRule/{branchFormulaID}");

            try
            {
                var res = salHeadRuleService.DeleteBranchHeadFieldRule(branchFormulaID, fieldName);

                if (res)
                    return Json(new
                    {
                        isDeleted = res,
                        fieldName = fieldName,
                        type = "success",
                        msg = "Branch Head Rules deleted successfully. ",
                    }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new
                    {
                        isDeleted = res,
                        fieldName = fieldName,
                        type = "fail",
                        msg = "Failed to delete this record. ",
                    }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult EditBranchHeadFieldRule(int branchFormulaID)
        {
            log.Info($"SalaryHeadRulesController/EditBranchHeadFieldRule/{branchFormulaID}");

            try
            {
                TempData["ChildFormulaData"] = null;
                var bHeadFieldRule = salHeadRuleService.GetBranchHeadFieldRule(branchFormulaID);

                if (bHeadFieldRule.FormulaColumn)
                    bHeadFieldRule.formulaType = FormulaType.FormulaColumn;
                if (bHeadFieldRule.FixedValueFormula)
                    bHeadFieldRule.formulaType = FormulaType.FixedValueFormula;

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<BranchSalaryHeadRule, SalaryHead>()
                     .ForMember(d => d.BranchFormulaID, o => o.MapFrom(s => (int?)s.BranchFormulaID))
                    .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
                    .ForMember(d => d.SelectedBranchID, o => o.MapFrom(s => s.BranchID))
                    .ForMember(d => d.FormulaColumn, o => o.MapFrom(s => s.FormulaColumn))
                    .ForMember(d => d.FixedValueFormula, o => o.MapFrom(s => s.FixedValueFormula))
                    .ForMember(d => d.LookUpHead, o => o.MapFrom(s => s.LookUpHead))
                    .ForMember(d => d.LookUpHeadName, o => o.MapFrom(s => s.LookUpHeadName))
                    .ForMember(d => d.formulaType, o => o.MapFrom(s => s.formulaType))
                    .ForMember(d => d.FixedValue, o => o.MapFrom(s => s.FixedValue))
                    .ForMember(d => d.LowerRange, o => o.MapFrom(s => s.LowerRange))
                    .ForMember(d => d.UpperRange, o => o.MapFrom(s => s.UpperRange))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                    .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                    .ForMember(d => d.ActionType, o => o.MapFrom(s => s.ActionType))
                    .ForAllOtherMembers(d => d.Ignore());
                });

                var salHeadModel = Mapper.Map<SalaryHead>(bHeadFieldRule);
                salHeadModel.BranchList = dropdownBindService.ddlBranchList();

                return PartialView("_LocationFormulaEditor", salHeadModel);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
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
            else {
                var headRules = salHeadRuleService.GetSalaryHeadFormulaRule(frmEditorModel.fieldName, frmEditorModel.EmployeeTypeID);
                lookUpHeadName = headRules?.LookUpHeadName ?? string.Empty;
                lookUpHead = headRules?.LookUpHead ?? string.Empty ;
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

    }
}