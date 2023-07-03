using AutoMapper;
using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers
{
    public class AdvanceSearchController : BaseController
    {
        // GET: AdvanceSearch
        private readonly IAdvanceSearchService advanceSerachService;
        public AdvanceSearchController(IAdvanceSearchService advanceSerachService)
        {
            this.advanceSerachService = advanceSerachService;
        }
        public ActionResult Index()
        {
            log.Info($"AdvanceSearchController/Index");

            return View();
        }

        public ActionResult _FillFilterFieldList(int fieldID, int selectedEmployeeType)
        {
            log.Info($"AdvanceSearchController/_FillFilterFieldList/{fieldID}");

            IEnumerable<SelectListModel> fields = Enumerable.Empty<SelectListModel>();
            var model = new CheckBoxListViewModel();
            try
            {
                if (fieldID < 8)
                {
                    var selectedCheckboxes = new List<CheckBox>();
                    var getFieldsValue = advanceSerachService.GetFilterFields((AdvanceSearchFilterFields)fieldID, selectedEmployeeType);

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<SelectListModel, CheckBox>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.id))
                        .ForMember(d => d.Name, o => o.MapFrom(s => s.value));
                    });
                    model.AvailableFields = Mapper.Map<List<CheckBox>>(getFieldsValue);
                    model.SelectedFields = selectedCheckboxes;
                    return PartialView("_FilterFieldList", model);
                }
                else
                {
                    AdvanceSearchDateFilter dateFilter = new AdvanceSearchDateFilter();
                    dateFilter.FilterID = fieldID; dateFilter.DateFrom = DateTime.Now;
                    dateFilter.DateTo = DateTime.Now;
                    return PartialView("_DateRangeFilter", dateFilter);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            // return PartialView("_FilterFieldList", model);
        }

        public ActionResult PostFilterFieldValues(CheckBoxListViewModel chklistVM, AdvanceSearchDateFilter dateFilter, FormCollection frm, string ButtonType)
        {
            log.Info($"AdvanceSearchController/PostFilterFieldValues");

            if (chklistVM.PostedFields?.fieldIds.Count() > 0)
            {
                var selectedFieldID = int.Parse(frm.Get("selectedFieldID"));
                var selectedEmployeeType = int.Parse(frm.Get("SelectedEmployeeType"));

                if (ButtonType == "Select")
                    return Json(new { formAction = ButtonType, selectedFields = chklistVM.PostedFields.fieldIds }, JsonRequestBehavior.AllowGet);

                else
                {
                    var searchedData = advanceSerachService.GetAdvanceSearchResult((AdvanceSearchFilterFields)selectedFieldID, selectedEmployeeType, chklistVM.PostedFields.fieldIds);
                    searchedData.SelectMoreFields = new SelectMoreFields();
                    searchedData.SelectMoreFields.PreviousFields = chklistVM.PostedFields.fieldIds;
                    searchedData.SelectMoreFields.FilterTypeID = selectedFieldID;
                    searchedData.SelectMoreFields.SelectedEmployeeType = selectedEmployeeType;
                    if (searchedData.SearchedResultDT != null)
                    {
                        TempData["SearchedResultDT"] = searchedData.SearchedResultDT;
                    }
                    return Json(new
                    {
                        formAction = ButtonType,
                        selectedFields = chklistVM.PostedFields.fieldIds,
                        htmlData = ConvertViewToString("_SearchResultWindow", searchedData)
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (dateFilter.FilterID > 7)
                {
                    if (ModelState.IsValid)
                    {
                        if ((dateFilter.DateFrom > dateFilter.DateTo) || (dateFilter.DateTo < dateFilter.DateFrom))
                        {
                            ModelState.AddModelError("DateRangeValidation", "From Date should be always less than To Date.");
                            return Json(new
                            {
                                formAction = ButtonType,
                                htmlData = ConvertViewToString("_DateRangeFilter", dateFilter),
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                        return PartialView("_DateRangeFilter", dateFilter);

                    if (ButtonType != "Select")
                    {
                        var selectedEmployeeType = int.Parse(frm.Get("SelectedEmployeeType"));
                        
                        Mapper.Initialize(cfg => {
                            cfg.CreateMap<AdvanceSearchDateFilter, SelectMoreFields>()
                            .ForMember(d => d.DateTo, o => o.MapFrom(s => s.DateTo))
                            .ForMember(d => d.DateFrom, o => o.MapFrom(s => s.DateFrom))
                            .ForMember(d => d.FilterTypeID, o => o.MapFrom(s => s.FilterID))
                            .ForMember(d => d.SelectedEmployeeType, o => o.UseValue(selectedEmployeeType))
                            .ForAllOtherMembers(d => d.Ignore());
                        });
                        var selectMoreFields = Mapper.Map<SelectMoreFields>(dateFilter);

                        var searchedData = advanceSerachService.GetAdvanceSearchResult((AdvanceSearchFilterFields)dateFilter.FilterID, selectedEmployeeType, null, selectMoreFields);

                        searchedData.SelectMoreFields = new SelectMoreFields();
                        searchedData.SelectMoreFields.FilterTypeID = dateFilter.FilterID;
                        searchedData.SelectMoreFields.SelectedEmployeeType = selectedEmployeeType;
                        searchedData.SelectMoreFields.DateFrom = dateFilter.DateFrom;
                        searchedData.SelectMoreFields.DateTo = dateFilter.DateTo;

                        if (searchedData.SearchedResultDT != null)
                        {
                            TempData["SearchedResultDT"] = searchedData.SearchedResultDT;
                        }
                        return Json(new
                        {
                            formAction = ButtonType,
                            partialView= "_SearchResultWindow",
                           // selectedFields = chklistVM.PostedFields.fieldIds,
                            htmlData = ConvertViewToString("_SearchResultWindow", searchedData)
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return Json(new { formAction = ButtonType }, JsonRequestBehavior.AllowGet);
              
                return Json(new { formAction = ButtonType }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDefaultSearchResult(int filterFieldID, int[] chkPostedFieldIDs)
        {
            log.Info($"AdvanceSearchController/GetDefaultSearchResult");
            try
            {
                AdvanceSearchResult searchResult = new AdvanceSearchResult();
                var searchedData = advanceSerachService.GetAdvanceSearchResult((AdvanceSearchFilterFields)filterFieldID, -1, chkPostedFieldIDs);
                return PartialView("_SearchResult", searchResult);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult ReSearchWithNewFields(SelectMoreFields moreFields, FormCollection frm, string Refilter)
        {
            log.Info($"AdvanceSearchController/ReSearchWithNewFields/{Refilter}");
            try
            {
                AdvanceSearchResult searchResult = new AdvanceSearchResult();

                if (Refilter == "Result")
                {
                    searchResult = advanceSerachService.GetAdvanceSearchResult((AdvanceSearchFilterFields)moreFields.FilterTypeID, moreFields.SelectedEmployeeType, moreFields.PreviousFields, moreFields);

                    if (searchResult.SearchedResultDT != null)
                    {
                        TempData["SearchedResultDT"] = searchResult.SearchedResultDT;
                    }
                    return PartialView("_SearchResultGridView", searchResult);
                }

                else
                {
                    DataSet exportData = new DataSet();

                    if (TempData["SearchedResultDT"] != null)
                    {
                        string fileName = string.Empty, msg = string.Empty;
                        string fullPath = Server.MapPath("~/FileDownload/");
                        fileName = ExtensionMethods.SetUniqueFileName("EmployeeSearchResult-", FileExtension.xlsx);
                        exportData = (DataSet)TempData["SearchedResultDT"];
                        var res = advanceSerachService.ExportAdvanceSearchResult(exportData, fullPath, fileName);
                        fullPath = $"{fullPath}{fileName}";
                        return JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");

                    }
                }
                return Content("Refreshed Data");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}