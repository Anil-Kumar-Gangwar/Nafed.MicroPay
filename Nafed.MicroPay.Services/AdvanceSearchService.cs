using System;
using System.Collections.Generic;
using System.Linq;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Services.IServices;
using System.Data;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.ImportExport.Interfaces;

namespace Nafed.MicroPay.Services
{
    public class AdvanceSearchService : BaseService, IAdvanceSearchService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IDropdownBindService dropdownService;
        private readonly IAdvanceSearchRepository advanceSearchRepo;
        private readonly IEmployeeRepository empRepo;
        private readonly IExport export;

        public AdvanceSearchService(IGenericRepository genericRepo, IDropdownBindService dropdownService,
            IAdvanceSearchRepository advanceSearchRepo, IExport export, IEmployeeRepository empRepo)
        {
            this.dropdownService = dropdownService;
            this.genericRepo = genericRepo;
            this.advanceSearchRepo = advanceSearchRepo;
            this.export = export;
            this.empRepo = empRepo;
            // var emp_payScale = empRepo.GetDesignationPayScaleList(dtoEmployee.DesignationID).FirstOrDefault();
        }
        public IEnumerable<SelectListModel> GetFilterFields(AdvanceSearchFilterFields filter, int selectedEmployeeType)
        {
            log.Info($"AdvanceSearchService/GetFilterFields");

            IEnumerable<SelectListModel> fields = Enumerable.Empty<SelectListModel>();

            try
            {
                if (filter == AdvanceSearchFilterFields.EmployeeName)
                {
                    if (selectedEmployeeType == -1)
                        fields = genericRepo.Get<DTOModel.tblMstEmployee>()
                            .Select(x => new SelectListModel { id = x.EmployeeId, value = x.Name }).ToList().OrderBy(x => x.value);

                    else if (selectedEmployeeType == 0)
                        fields = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DOLeaveOrg != null)
                           .Select(x => new SelectListModel { id = x.EmployeeId, value = x.Name }).ToList().OrderBy(x => x.value);
                    else if (selectedEmployeeType == 1)
                        fields = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && x.DOLeaveOrg == null)
                         .Select(x => new SelectListModel { id = x.EmployeeId, value = x.Name }).ToList().OrderBy(x => x.value);
                }
                else if (filter == AdvanceSearchFilterFields.EmployeeCode)
                {
                    if (selectedEmployeeType == -1)
                        fields = genericRepo.Get<DTOModel.tblMstEmployee>()
                               .Select(x => new SelectListModel { id = x.EmployeeId, value = x.EmployeeCode }).ToList().OrderBy(x => x.value);

                    else if (selectedEmployeeType == 0)

                        fields = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DOLeaveOrg != null)
                        .Select(x => new SelectListModel { id = x.EmployeeId, value = x.EmployeeCode }).ToList().OrderBy(x => x.value);

                    else if (selectedEmployeeType == 1)
                        fields = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && x.DOLeaveOrg == null)
                         .Select(x => new SelectListModel { id = x.EmployeeId, value = x.EmployeeCode }).ToList().OrderBy(x => x.value);
                }

                else if (filter == AdvanceSearchFilterFields.Branch)
                {
                    fields = genericRepo.Get<DTOModel.Branch>(x => !x.IsDeleted).Select(x => new SelectListModel { id = x.BranchID, value = x.BranchName }).ToList().OrderBy(x => x.value);
                }
                else if (filter == AdvanceSearchFilterFields.Desigantion)
                {
                    fields = genericRepo.Get<DTOModel.Designation>(x => !x.IsDeleted).Select(x => new SelectListModel { id = x.DesignationID, value = x.DesignationName }).ToList().OrderBy(x => x.value);
                }
                else if (filter == AdvanceSearchFilterFields.Section)
                {
                    fields = genericRepo.Get<DTOModel.Section>(x => !x.IsDeleted).Select(x => new SelectListModel { id = x.SectionID, value = x.SectionName }).ToList().OrderBy(x => x.value);
                }
                else if (filter == AdvanceSearchFilterFields.Divison)
                {
                    fields = genericRepo.Get<DTOModel.Division>(x => !x.IsDeleted).Select(x => new SelectListModel { id = x.DivisionID, value = x.DivisionName }).ToList().OrderBy(x => x.value);
                }

                else if (filter == AdvanceSearchFilterFields.Cadre)
                {
                    fields = genericRepo.Get<DTOModel.Cadre>(x => !x.IsDeleted).Select(x => new SelectListModel { id = x.CadreID, value = x.CadreName }).ToList().OrderBy(x => x.value);
                }

                return fields;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AdvanceSearchResult GetAdvanceSearchResult(AdvanceSearchFilterFields filter, int selectedEmpType, int[] selectedIDs, SelectMoreFields moreFields = null)
        {
            log.Info($"AdvanceSearchService/GetFilterFields/{selectedEmpType}");
            try
            {
                var dataModal = new AdvanceSearchResult();
                DataTable dtFieldIds = new DataTable();
                DataTable columnName = new DataTable(), columnDisplayName = new DataTable(); DataTable payScaleDT = new DataTable();

                if (selectedIDs != null)
                {
                    var selectedFields = (from x in selectedIDs select new { SelectedID = x }).ToList();
                    dtFieldIds = Common.ExtensionMethods.ToDataTable(selectedFields);
                }
                else
                {
                    dtFieldIds = new DataTable();
                    dtFieldIds.Columns.Add("value", typeof(string));

                    DataRow dr = dtFieldIds.NewRow(); dr[0] = 1;
                    dtFieldIds.Rows.Add(dr);
                }
                payScaleDT = new DataTable();
                payScaleDT.Columns.Add("id", typeof(int));
                payScaleDT.Columns.Add("value", typeof(string));

                if (moreFields?.CheckedChkName?.Count() > 0)
                {
                    var columnList = moreFields.CheckedChkName.Select(x => new SelectListModel { id = 0, value = x }).ToList();
                    var columnDisplayNameList = moreFields.CheckedChkDisplayName.Select(x => new SelectListModel { id = 0, value = x }).ToList();

                    int col_idx = 1, col_display_idx = 1;
                    columnList.ForEach(x => { x.id = col_idx++; }); columnDisplayNameList.ForEach(x => { x.id = col_display_idx++; });

                    columnName = ExtensionMethods.ToDataTable(columnList);
                    columnDisplayName = ExtensionMethods.ToDataTable(columnDisplayNameList);
                }
                else {

                    columnName = new DataTable();
                    columnName.Columns.Add("id", typeof(int));
                    columnName.Columns.Add("value", typeof(string));

                    columnDisplayName = new DataTable();
                    columnDisplayName.Columns.Add("id", typeof(int));
                    columnDisplayName.Columns.Add("value", typeof(string));
                }

                #region Get Designation Pay Scales //==

                if (moreFields?.PayScale ?? false == true)
                {
                    var designationPayList = empRepo.GetDesignationPayScaleList(null);
                    var payScales = designationPayList.Select(x => new SelectListModel { id = x.DesignationID, value = x.PayScale }).ToList();
                    payScaleDT = ExtensionMethods.ToDataTable(payScales);
                }
                #endregion

                var result = advanceSearchRepo.GetAdvanceSearchResult((int)filter, selectedEmpType, dtFieldIds, payScaleDT, columnName, columnDisplayName, moreFields?.DateFrom ?? null, moreFields?.DateTo ?? null);
                if (result.Rows.Count > 0)
                {
                    dataModal.SearchedResultDT = new DataSet();
                    dataModal.SearchedResultDT.Tables.Add(result);
                    dataModal.GridView = new DynamicGrid();
                    dataModal.GridView.Columns = new List<string>();

                    for (int i = 5; i < result.Columns.Count; i++)
                    {
                        dataModal.GridView.Columns.Add(result.Columns[i].ToString().Replace("_", " "));
                    }
                    dataModal.GridView.Rows = new List<DynamicGridRow>();

                    List<DynamicGridRow> Rowlist = new List<DynamicGridRow>();
                    for (int i = 0; i < result.Rows.Count; i++)
                    {
                        DynamicGridRow GridRow = new DynamicGridRow();
                        List<string> value = new List<string>();

                        GridRow.BranchCode = result.Rows[i][0].ToString();
                        GridRow.EmployeeCode = result.Rows[i][1].ToString();
                        GridRow.EmployeeName = result.Rows[i][2].ToString();
                        GridRow.DesignationCode = result.Rows[i][3].ToString();
                        GridRow.EmployeeType = result.Rows[i][4].ToString();

                        for (int j = 5; j < result.Columns.Count; j++)
                        {
                            value.Add(result.Rows[i][j].ToString());
                        }
                        GridRow.Values = value;
                        Rowlist.Add(GridRow);
                    }
                    dataModal.GridView.Rows = Rowlist;
                }
                return dataModal;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Export

        public bool ExportAdvanceSearchResult(DataSet dsSource, string sFullPath, string fileName)
        {
            log.Info($"AdvanceSearchService/ExportAdvanceSearchResult/{sFullPath}/{fileName}");
            try
            {
                var flag = false;
                sFullPath = $"{sFullPath}{fileName}";
                flag = export.ExportToExcel(dsSource, sFullPath, fileName);
                return flag;
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
