using AutoMapper;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.ImportExport.Interfaces;
using Nafed.MicroPay.ImportExport;
using static Nafed.MicroPay.Common.DataValidation;
using System.Dynamic;

namespace Nafed.MicroPay.Services
{
    public class ImportMonthlyInputService : BaseService, IImportMonthlyInputService
    {
        private readonly IExcelService excelService;
        private readonly IGenericRepository genericRepo;
        private readonly IExport export;
        private readonly IDataImportRepository dataImportRepo;
        public ImportMonthlyInputService(IExcelService excelService, IGenericRepository genericRepo, IExport export, IDataImportRepository dataImportRepo)
        {
            this.excelService = excelService;
            this.genericRepo = genericRepo;
            this.export = export;
            this.dataImportRepo = dataImportRepo;
        }

        public string GetMonthlyInputForm(int branchID, int month, int year, string fileName, string sFullPath, int? employeeTypeId)
        {
            log.Info($"ImportMonthlyInputService/GetMonthlyInputForm/");
            try
            {
                string result = string.Empty;
                if (Directory.Exists(sFullPath))
                {
                    sFullPath = $"{sFullPath}{fileName}";
                    List<MonthlyInputExport> monthlyInputList = new List<MonthlyInputExport>();

                    DataSet dtMonthlyInput = new DataSet();
                    DataTable dt = new DataTable();
                    //if (branchID != 0)
                    //{
                    dt = dataImportRepo.GetMonthlyInputData(branchID, employeeTypeId, month, year);
                    monthlyInputList = Common.ExtensionMethods.ConvertToList<Model.MonthlyInputExport>(dt);
                    //}
                    //if (monthlyInputList.Count() <= 0)
                    //{
                    //    IEnumerable<DTOModel.tblMstEmployee> empDTO = null;
                    //    if (branchID != 0)
                    //    {
                    //        empDTO = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.BranchID == branchID && x.DOLeaveOrg != null && x.EmployeeTypeID == employeeTypeId);
                    //    }
                    //    Mapper.Initialize(cfg =>
                    //    {
                    //        cfg.CreateMap<DTOModel.tblMstEmployee, Model.ImportMonthlyInput>()
                    //        .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    //        .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name))
                    //        .ForMember(d => d.EmployeeType, o => o.MapFrom(s => s.EmployeeType.EmployeeTypeName))
                    //        .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.Branch.BranchCode))
                    //        .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                    //        .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.DepartmentName))
                    //        .ForMember(d => d.Month, o => o.UseValue(month))
                    //        .ForMember(d => d.Year, o => o.UseValue(year))
                    //        .ForMember(d => d.E_Basic, o => o.UseValue(0))
                    //         .ForMember(d => d.E_SP, o => o.UseValue(0))
                    //         .ForMember(d => d.E_FDA, o => o.UseValue(0))
                    //         .ForMember(d => d.E_01, o => o.UseValue(0))
                    //         .ForMember(d => d.E_02, o => o.UseValue(0))
                    //         .ForMember(d => d.E_03, o => o.UseValue(0))
                    //         .ForMember(d => d.E_04, o => o.UseValue(0))
                    //         .ForMember(d => d.E_05, o => o.UseValue(0))
                    //         .ForMember(d => d.E_06, o => o.UseValue(0))
                    //         .ForMember(d => d.E_07, o => o.UseValue(0))
                    //         .ForMember(d => d.E_08, o => o.UseValue(0))
                    //         .ForMember(d => d.E_09, o => o.UseValue(0))
                    //         .ForMember(d => d.E_10, o => o.UseValue(0))
                    //         .ForMember(d => d.E_11, o => o.UseValue(0))
                    //         .ForMember(d => d.E_12, o => o.UseValue(0))
                    //         .ForMember(d => d.E_13, o => o.UseValue(0))
                    //         .ForMember(d => d.E_14, o => o.UseValue(0))
                    //         .ForMember(d => d.E_15, o => o.UseValue(0))
                    //         .ForMember(d => d.E_16, o => o.UseValue(0))
                    //         .ForMember(d => d.E_17, o => o.UseValue(0))
                    //         .ForMember(d => d.E_18, o => o.UseValue(0))
                    //         .ForMember(d => d.E_19, o => o.UseValue(0))
                    //         .ForMember(d => d.E_20, o => o.UseValue(0))
                    //         .ForMember(d => d.E_21, o => o.UseValue(0))
                    //         .ForMember(d => d.E_22, o => o.UseValue(0))
                    //         .ForMember(d => d.E_23, o => o.UseValue(0))
                    //         .ForMember(d => d.E_24, o => o.UseValue(0))
                    //         .ForMember(d => d.E_25, o => o.UseValue(0))
                    //         .ForMember(d => d.E_26, o => o.UseValue(0))
                    //         .ForMember(d => d.E_27, o => o.UseValue(0))
                    //         .ForMember(d => d.E_28, o => o.UseValue(0))
                    //         .ForMember(d => d.E_29, o => o.UseValue(0))
                    //         .ForMember(d => d.E_30, o => o.UseValue(0))
                    //         .ForMember(d => d.D_PF, o => o.UseValue(0))
                    //         .ForMember(d => d.D_VPF, o => o.UseValue(0))
                    //         .ForMember(d => d.D_01, o => o.UseValue(0))
                    //         .ForMember(d => d.D_02, o => o.UseValue(0))
                    //         .ForMember(d => d.D_03, o => o.UseValue(0))
                    //         .ForMember(d => d.D_04, o => o.UseValue(0))
                    //         .ForMember(d => d.D_05, o => o.UseValue(0))
                    //         .ForMember(d => d.D_06, o => o.UseValue(0))
                    //         .ForMember(d => d.D_07, o => o.UseValue(0))
                    //         .ForMember(d => d.D_08, o => o.UseValue(0))
                    //         .ForMember(d => d.D_09, o => o.UseValue(0))
                    //         .ForMember(d => d.D_10, o => o.UseValue(0))
                    //         .ForMember(d => d.D_11, o => o.UseValue(0))
                    //         .ForMember(d => d.D_12, o => o.UseValue(0))
                    //         .ForMember(d => d.D_13, o => o.UseValue(0))
                    //         .ForMember(d => d.D_14, o => o.UseValue(0))
                    //         .ForMember(d => d.D_15, o => o.UseValue(0))
                    //         .ForMember(d => d.D_16, o => o.UseValue(0))
                    //         .ForMember(d => d.D_17, o => o.UseValue(0))
                    //         .ForMember(d => d.D_18, o => o.UseValue(0))
                    //         .ForMember(d => d.D_19, o => o.UseValue(0))
                    //         .ForMember(d => d.D_20, o => o.UseValue(0))
                    //         .ForMember(d => d.D_21, o => o.UseValue(0))
                    //         .ForMember(d => d.D_22, o => o.UseValue(0))
                    //         .ForMember(d => d.D_23, o => o.UseValue(0))
                    //         .ForMember(d => d.D_24, o => o.UseValue(0))
                    //         .ForMember(d => d.D_25, o => o.UseValue(0))
                    //         .ForMember(d => d.D_26, o => o.UseValue(0))
                    //         .ForMember(d => d.D_27, o => o.UseValue(0))
                    //         .ForMember(d => d.D_28, o => o.UseValue(0))
                    //         .ForMember(d => d.D_29, o => o.UseValue(0))
                    //         .ForMember(d => d.D_30, o => o.UseValue(0))
                    //         .ForMember(d => d.C_TotEarn, o => o.UseValue(0))
                    //         .ForMember(d => d.C_TotDedu, o => o.UseValue(0))
                    //         .ForMember(d => d.C_NetSal, o => o.UseValue(0))
                    //         .ForMember(d => d.C_Pension, o => o.UseValue(0))
                    //         .ForMember(d => d.C_GrossSalary, o => o.UseValue(0))
                    //         .ForMember(d => d.SalaryLock, o => o.UseValue(false))
                    //         .ForMember(d => d.LWP, o => o.UseValue(0))
                    //         .ForMember(d => d.OTHrs, o => o.UseValue(0))
                    //         .ForMember(d => d.AOTHrs, o => o.UseValue(0))
                    //         .ForMember(d => d.DeductPFLoan, o => o.UseValue(false))
                    //         .ForMember(d => d.DeductNB, o => o.UseValue(false))
                    //         .ForMember(d => d.DeductTCS, o => o.UseValue(false))
                    //         .ForMember(d => d.DeductHouseLoan, o => o.UseValue(false))
                    //         .ForMember(d => d.DeductFestivalLoan, o => o.UseValue(false))
                    //         .ForMember(d => d.DeductCarLoan, o => o.UseValue(false))
                    //         .ForMember(d => d.DeductScooterLoan, o => o.UseValue(false))
                    //         .ForMember(d => d.DeductSundryAdv, o => o.UseValue(false))
                    //         .ForMember(d => d.E_31, o => o.UseValue(0))
                    //         .ForMember(d => d.E_32, o => o.UseValue(0))
                    //        .ForAllOtherMembers(d => d.Ignore());
                    //    });
                    //    monthlyInputList = Mapper.Map<List<Model.ImportMonthlyInput>>(empDTO);
                    //}

                    var sno = 1;
                    monthlyInputList.ForEach(x =>
                    {
                        x.Sno = sno;
                        sno++;
                    });

                    if (monthlyInputList.Count > 0)
                    {
                        var formData = monthlyInputList;
                        var monthlyDataTable = Common.ExtensionMethods.ToDataTable(formData);
                        var salaryHead = genericRepo.Get<DTOModel.SalaryHead>(x => x.IsDeleted == false && x.ActiveField == true && x.MonthlyInput == true && x.EmployeeTypeID == employeeTypeId).Select(x => new { x.FieldName, x.FieldDesc }).ToList();

                        monthlyDataTable.Columns.Remove("error");
                        monthlyDataTable.Columns.Remove("warning");
                        monthlyDataTable.Columns.Remove("isDuplicatedRow");
                        monthlyDataTable.Columns.Remove("EmployeeId");
                        monthlyDataTable.Columns.Remove("BranchId");

                        string[] columnList = monthlyDataTable.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();

                        for (int k = 0; k < columnList.Count(); k++)
                        {
                            var col = columnList[k].ToLower();
                            if (col != "sno" && col != "employeecode" && col != "employeename" && col != "branchcode" && col != "branchname" && col != "departmentname" && col != "month" && col != "year" && col != "employeetype")
                            {
                                if (!salaryHead.Any(x => x.FieldName.ToLower().ToString() == col))
                                    monthlyDataTable.Columns.Remove(columnList[k]);
                            }
                        }

                        for (int i = 0; i < monthlyDataTable.Columns.Count; i++)
                        {
                            for (int j = 0; j < salaryHead.Count; j++)
                            {
                                if (monthlyDataTable.Columns[i].ColumnName.ToLower().ToString() == salaryHead[j].FieldName.ToLower().ToString())
                                {
                                    monthlyDataTable.Columns[i].ColumnName = salaryHead[j].FieldDesc;
                                }
                            }
                        }
                        dtMonthlyInput.Tables.Add(monthlyDataTable);


                        if (dtMonthlyInput != null && dtMonthlyInput.Tables[0].Rows.Count > 0)  //====== export Monthly Input if there is data ========= 
                        {
                            dtMonthlyInput.Tables[0].Columns[0].ColumnName = "#";
                            //bool res = export.ExportToExcel(dtMonthlyInput, sFullPath, "MonthlyInput");
                            bool res = export.ExportFormatedExcel(dtMonthlyInput, sFullPath, "MonthlyInput");
                        }
                    }
                    else
                        result = "Norec";
                }
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public IEnumerable<dynamic> ReadImportMonthlyExcelData(string fileFullPath, bool isHeader, out string message, out int error, out int warning, out List<Model.ImportMonthlyInput> importMonthlyInputSave, out int employeeTypeId, out List<string> missingHeaders, out List<string> columnName)
        {
            log.Info($"ImportMonthlyInputService/ReadImportMonthlyExcelData/{0}");
            List<dynamic> objModel = new List<dynamic>();
            try
            {
                message = string.Empty;
                error = 0; warning = 0; employeeTypeId = 0;
                bool isValidHeaderList;
                importMonthlyInputSave = new List<ImportMonthlyInput>();
                missingHeaders = new List<string>();
                columnName = new List<string>();
                DataTable dtExcelResult = new DataTable();
                Import ob = new Import();
                dtExcelResult = ob.ReadExcelWithNoHeader(fileFullPath);
                string type = Convert.ToString(dtExcelResult.Rows[1].ItemArray[3]).Trim();
                var typeid = genericRepo.Get<DTOModel.EmployeeType>(x => x.EmployeeTypeName.Trim() == type && !x.IsDeleted).FirstOrDefault().EmployeeTypeID;
                employeeTypeId = typeid;
                var headerList = genericRepo.Get<DTOModel.SalaryHead>(x => x.IsDeleted == false && x.ActiveField == true && x.MonthlyInput == true && x.EmployeeTypeID == typeid).Select(x => x.FieldDesc.Trim()).ToList();

                string[] header = { "EmployeeCode", "EmployeeName", "EmployeeType", "BranchCode", "BranchName", "DepartmentName", "Month", "Year" };

                headerList.AddRange(header);

                DataTable dtExcel = ReadAndValidateHeader(fileFullPath, headerList, isHeader, out isValidHeaderList, out missingHeaders);

                if (isValidHeaderList)
                {
                    DeleteTempFile(fileFullPath);
                    DataTable dt = dtExcel.Copy();
                    var colum = GetHeader(dt);

                    DataTable dataWithHeader = excelService.GetDataWithHeader(dtExcel);
                    if (dataWithHeader.Rows.Count > 0)
                    {
                        DataTable dtEmpCodes = new DataTable();
                        DataTable dtBranchCodes = new DataTable();
                        DataTable dtEmpCodeFromDB = new DataTable();
                        MonthlyInputColValues colWithDistinctValues = GetDistinctValuesOfImportData(dataWithHeader);

                        if (colWithDistinctValues.empCode != null && colWithDistinctValues.empCode.Count() > 0)
                        {
                            var empCodes = (from x in colWithDistinctValues.empCode select new { ItemCode = x }).ToList();
                            dtEmpCodes = Common.ExtensionMethods.ToDataTable(empCodes);
                        }
                        if (colWithDistinctValues.BranchCode != null && colWithDistinctValues.BranchCode.Count() > 0)
                        {
                            var branchCodes = (from x in colWithDistinctValues.BranchCode select new { ItemCode = x }).ToList();
                            dtBranchCodes = Common.ExtensionMethods.ToDataTable(branchCodes);
                        }

                        DataSet dtEmpBranchFromDB = new DataSet();

                        dtEmpBranchFromDB = dataImportRepo.GetImportFieldValues(dtEmpCodes, dtBranchCodes);
                        MergeMonthlySheetDataWithDBValues(dataWithHeader, dtEmpBranchFromDB.Tables[0], dtEmpBranchFromDB.Tables[1]);
                        ValidateMonthlyImportSheetData(dataWithHeader, typeid, out error, out warning);
                        ///====  Merge Imported Data with Database data value

                        ///
                        ///====  Validate Imported data with database values 
                        ///
                        var keyFieldVal = genericRepo.Get<DTOModel.SalaryHead>(x => x.IsDeleted == false && x.ActiveField == true && x.MonthlyInput == true && x.EmployeeTypeID == typeid).Select(x => new SalaryHead { FieldName = x.FieldName, FieldDesc = x.FieldDesc.Trim() }).ToList();


                        keyFieldVal.AddRange(new List<SalaryHead>() {
                            new SalaryHead() { FieldName = "EmployeeId", FieldDesc = "EmployeeId" },
                            new SalaryHead() { FieldName = "BranchId", FieldDesc = "BranchId" },
                            new SalaryHead() { FieldName = "DepartmentName", FieldDesc = "DepartmentName" },
                            new SalaryHead() { FieldName = "error", FieldDesc = "error" },
                            new SalaryHead() { FieldName = "warning", FieldDesc = "warning" }
                        });

                        columnName = colum.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName).ToList();
                        columnName.Add("EmployeeId");
                        columnName.Add("BranchId");
                        columnName.Add("error");
                        columnName.Add("warning");
                        //columnName.Add("rowCount");
                        //columnName.Remove("#");
                        //columnName.Remove("EmployeeId");
                        //columnName.Remove("BranchId");
                        //columnName.Remove("error");
                        //columnName.Remove("warning");
                        //columnName.Remove("rowCount");

                        for (int i = 0; i < dataWithHeader.Rows.Count; i++)
                        {
                            ImportMonthlyInput MI = new ImportMonthlyInput();
                            for (int j = 0; j < dataWithHeader.Columns.Count; j++)
                            {
                                string sColName = dataWithHeader.Columns[j].ColumnName.ToString().Trim();
                                string salaryHeadColumn = string.Empty;
                                if (j == 0)
                                    MI.Sno = int.Parse(dataWithHeader.Rows[i][j].ToString().Trim());
                                if (j == 1)
                                    MI.EmployeeCode = dataWithHeader.Rows[i][j].ToString().Trim();
                                if (j == 2)
                                    MI.EmployeeName = dataWithHeader.Rows[i][j].ToString().Trim();
                                if (j == 3)
                                    MI.EmployeeType = dataWithHeader.Rows[i][j].ToString().Trim();
                                if (j == 4)
                                    MI.BranchCode = dataWithHeader.Rows[i][j].ToString().Trim();
                                if (j == 5)
                                    MI.BranchName = dataWithHeader.Rows[i][j].ToString().Trim();
                                if (j == 6)
                                    MI.DepartmentName = dataWithHeader.Rows[i][j].ToString().Trim();
                                if (j == 7)
                                    MI.Month = dataWithHeader.Rows[i][j].ToString().Trim();
                                if (j == 8)
                                    MI.Year = dataWithHeader.Rows[i][j].ToString().Trim();
                                else if (j > 8)
                                {
                                    var key = keyFieldVal.Where(x => x.FieldDesc.Replace(" ", "") == sColName).FirstOrDefault()?.FieldName;
                                    if (key != null)
                                        AddCellValue(MI, key, dataWithHeader.Rows[i][j].ToString().Trim());
                                }
                            }
                            var dynamic = RemoveProperty(MI);
                            importMonthlyInputSave.Add(MI);
                            objModel.Add(dynamic);
                        }
                        message = "success";
                        return objModel;
                    }
                }
                else
                {
                    error = error + 1;
                    message = "Headers";
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return objModel;
        }

        public DataTable GetHeader(DataTable output)
        {
            string SelectorBlankRowfilter = string.Empty;
            if (output.Rows.Count >= 1)
            {
                DataRow rowFirst = output.Rows[0];
                for (int icol = output.Columns.Count - 1; icol >= 0; icol--)
                {
                    DataColumn col = output.Columns[icol];
                    if (rowFirst[col].ToString().Trim() == "")
                        output.Columns.Remove(col);
                    else
                    {
                        if (output.Columns.Contains(rowFirst[col].ToString().Trim()))
                        {
                            output.Columns.Remove(rowFirst[col].ToString().Trim());
                            col.ColumnName = rowFirst[col].ToString().Trim();
                        }
                        else
                        {
                            col.ColumnName = rowFirst[col].ToString().Trim();
                            SelectorBlankRowfilter = SelectorBlankRowfilter == "" ? "([" + col.ColumnName + "]='' OR [" + col.ColumnName + "] IS NULL)" : SelectorBlankRowfilter + " AND ([" + col.ColumnName + "]='' OR [" + col.ColumnName + "] IS NULL)";
                        }
                    }
                }
                output.Rows.Remove(rowFirst);
                DataRow[] rowsBlankToDelete = output.Select(SelectorBlankRowfilter);
                if (rowsBlankToDelete.Length > 0)
                {
                    foreach (DataRow rowDelete in rowsBlankToDelete)
                        output.Rows.Remove(rowDelete);
                }
            }
            return output;
        }

        public dynamic RemoveProperty(ImportMonthlyInput mInput)
        {
            try
            {
                var returnClass = new ExpandoObject() as IDictionary<string, object>;
                var pprop = mInput.GetType().GetProperties();
                foreach (var pr in pprop)
                {
                    var val = pr.GetValue(mInput);
                    if (val is string && string.IsNullOrWhiteSpace(val.ToString()))
                    {
                        returnClass.Add(pr.Name, val);
                    }
                    else if (val == null)
                    {
                    }
                    else
                    {
                        returnClass.Add(pr.Name, val);
                    }
                }
                return returnClass;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ImportMonthlyInput AddCellValue(ImportMonthlyInput mInput, string keyName, string cellValue)
        {
            try
            {
                var prop = mInput.GetType().GetProperty(keyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                prop.SetValue(mInput, cellValue, null);
                var returnClass = new ExpandoObject() as IDictionary<string, object>;
                returnClass.Add(keyName, cellValue);
                return mInput;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        private DataTable ReadAndValidateHeader(string sFilePath, List<string> headerList, bool IsHeader, out bool isValid, out List<string> sMissingHeader)
        {

            log.Info($"ImportMonthlyInputService/ReadAndValidateHeader/{0}");

            isValid = false; sMissingHeader = new List<string>();
            DataTable dtExcel = new DataTable();
            List<string> invalidColumn = new List<string>();
            try
            {
                Import ob = new Import();
                using (dtExcel = ob.ReadExcelWithNoHeader(sFilePath))
                {
                    RemoveBlankRowAndColumns(ref dtExcel, true);
                    int iExcelRowCount = dtExcel.Rows.Count;
                    if (iExcelRowCount > 0)
                    {
                        List<string> strList = dtExcel.Rows[0].ItemArray.Select(o => o == null ? String.Empty : o.ToString()).ToList();
                        isValid = ValidateHeader(strList, headerList, out sMissingHeader, out invalidColumn);
                        return dtExcel;
                    }
                    else
                        return dtExcel;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private void RemoveBlankRowAndColumns(ref DataTable dtExcel, bool bRemoveBlankCols, bool skipFirstRow = false, int rowsColStartIndex = 0)
        {
            log.Info($"ImportMonthlyInputService/RemoveBlankRowAndColumns/{0}");
            //Trimming cells data
            dtExcel.AsEnumerable().ToList().ForEach(row =>
            {
                var cellList = row.ItemArray.ToList();
                row.ItemArray = cellList.Select(x => x.ToString().Trim()).ToArray();
            });

            DataRow[] drExcel = dtExcel.Rows.Cast<DataRow>().Where(row => !row.ItemArray.Skip(rowsColStartIndex).All(field => field is System.DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).ToArray<DataRow>();
            if (drExcel.Length > 0)
            {
                dtExcel = dtExcel.Rows.Cast<DataRow>().Where(row => !row.ItemArray.Skip(rowsColStartIndex).All(field => field is System.DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).CopyToDataTable();
            }
            if (bRemoveBlankCols)
            {
                // Removing blank columns
                foreach (var column in dtExcel.Columns.Cast<DataColumn>().ToArray())
                {
                    if (column.Ordinal != 3 && column.Ordinal != 4)
                    {
                        if (skipFirstRow)
                        {
                            if (dtExcel.AsEnumerable().Skip(1).All(dr => dr[column].ToString().Equals("")))
                                dtExcel.Columns.Remove(column);
                        }
                        else
                        if (dtExcel.AsEnumerable().All(dr => dr[column].ToString().Equals("")))
                            dtExcel.Columns.Remove(column);
                    }
                }
            }
            dtExcel.AcceptChanges();
        }

        private MonthlyInputColValues GetDistinctValuesOfImportData(DataTable dtImportData)
        {
            log.Info($"ImportMonthlyInputService/AttendanceImportColValues");
            try
            {
                MonthlyInputColValues colWithDistinctValues = new MonthlyInputColValues();
                colWithDistinctValues.empCode = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "EmployeeCode").Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                colWithDistinctValues.BranchCode = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "BranchCode").Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                return colWithDistinctValues;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        void MergeMonthlySheetDataWithDBValues(DataTable excelData, DataTable dtEmpCodes, DataTable dtBranchCode)
        {
            try
            {
                excelData.Columns.Add("EmployeeId", typeof(int));
                excelData.Columns.Add("BranchId", typeof(int));
                excelData.Columns.Add("error", typeof(string));
                excelData.Columns.Add("warning", typeof(string));
                excelData.Columns.Add("rowCount", typeof(int));

                foreach (DataRow row in excelData.Rows)
                {
                    string sEmpCode = row["EmployeeCode"].ToString();
                    DataRow[] arrEmpCode = dtEmpCodes.AsEnumerable().Where(x => x.Field<string>("EmployeeCode").Equals(sEmpCode, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (arrEmpCode.Length > 0)
                    {
                        row["EmployeeId"] = arrEmpCode[0]["EmployeeID"].ToString().Equals("") ? -1 : int.Parse(arrEmpCode[0]["EmployeeID"].ToString());

                    }
                    else
                        row["EmployeeId"] = -1;


                    string sBranchCode = row["BranchCode"].ToString();
                    DataRow[] arrBranchCode = dtBranchCode.AsEnumerable().Where(x => x.Field<string>("BranchCode").Equals(sBranchCode, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (arrBranchCode.Length > 0)
                    {
                        row["BranchId"] = arrEmpCode[0]["BranchID"].ToString().Equals("") ? -1 : int.Parse(arrEmpCode[0]["BranchID"].ToString());

                    }
                    else
                        row["BranchId"] = -1;

                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        void ValidateMonthlyImportSheetData(DataTable dtSource, int employeeTypeId, out int iError, out int iWarning)
        {
            iError = 0; iWarning = 0;

            log.Info($"ImportMonthlyInputService/ValidateAttendanceImportData");
            try
            {
                iError = 0; iWarning = 0;
                var headerList = genericRepo.Get<DTOModel.SalaryHead>(x => x.IsDeleted == false && x.ActiveField == true && x.MonthlyInput == true && x.EmployeeTypeID == employeeTypeId).Select(x => x.FieldDesc.Trim()).ToList();
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    DataRow cDrow = dtSource.Rows[i];
                    string sError = string.Empty;
                    string sWarning = string.Empty;

                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        string sColName = dtSource.Columns[j].ColumnName.ToString().Trim()/*.ToLower()*/;
                        string salaryHeadColumn = string.Empty;
                        for (int k = 0; k < headerList.Count; k++)
                        {
                            if (sColName == headerList[k].Replace(" ", ""))
                            {
                                salaryHeadColumn = headerList[k].Replace(" ", "");
                                break;
                            }
                        }

                        if (
                            sColName.ToLower().Equals("EmployeeID")
                            || sColName.ToLower().Equals("error")
                            || sColName.ToLower().Equals("rowcount") || sColName.ToLower().Equals("warning")
                            || sColName.ToLower().Equals("BranchID"))
                        {
                            continue;
                        }
                        string sColVal = dtSource.Rows[i][j].ToString().Trim();

                        if (sColName.Equals("EmployeeCode")) // Empoyee Code (1)
                        {
                            #region Validate employee code
                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                int employeeID = int.Parse(dtSource.Rows[i]["EmployeeID"].ToString());
                                if (!(employeeID > 0))
                                {
                                    sError = sError + "NF|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            sWarning = sWarning + "|";
                            #endregion
                        }
                        else if (sColName.Equals("BranchCode")) // BranchCode (3)
                        {

                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                int branchID = int.Parse(dtSource.Rows[i]["BranchID"].ToString());
                                if (!(branchID > 0))
                                {
                                    sError = sError + "NF|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                        }
                        else if (sColName.Equals("Month")) // Month (5)
                        {
                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                if (!IsNumeric(sColVal))
                                {
                                    sError = sError + "PN|";
                                    iError++;
                                }
                                else
                                {
                                    if (!IsBlank(sColVal) && sColVal.Length > 2)
                                    {
                                        sError = sError + "MAXLEN|";
                                        iError++;
                                    }
                                    else
                                        sError = sError + "|";
                                }
                            }
                        }
                        else if (sColName.Equals("Year")) // Month (6)
                        {
                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                if (!IsNumeric(sColVal))
                                {
                                    sError = sError + "PN|";
                                    iError++;
                                }
                                else
                                {
                                    if ((!IsBlank(sColVal) && sColVal.Length > 4))
                                    {
                                        sError = sError + "MAXLEN|";
                                        iError++;
                                    }
                                    else
                                        sError = sError + "|";
                                }
                            }
                        }
                        else if (sColName.Equals(salaryHeadColumn)) // Basic (7)
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!IsNumeric(sColVal))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                                sError = sError + "|";
                        }
                        else if (sColName.Equals("DeductPFLoan")) // DeductPFLoan (81)
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!IsValidBoolean(sColVal))
                                {
                                    sError = sError + "BOA|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                                sError = sError + "|";
                        }
                        else if (sColName.Equals("DeductNB")) // DeductNB (82)
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!IsValidBoolean(sColVal))
                                {
                                    sError = sError + "BOA|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                                sError = sError + "|";
                        }
                        else if (sColName.Equals("DeductTCS")) // DeductTCS (83)
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!IsValidBoolean(sColVal))
                                {
                                    sError = sError + "BOA|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                                sError = sError + "|";
                        }
                        else if (sColName.Equals("DeductHouseLoan")) // DeductHouseLoan (84)
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!IsValidBoolean(sColVal))
                                {
                                    sError = sError + "BOA|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                                sError = sError + "|";
                        }
                        else if (sColName.Equals("DeductFestivalLoan")) // DeductFestivalLoan (85)
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!IsValidBoolean(sColVal))
                                {
                                    sError = sError + "BOA|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                                sError = sError + "|";
                        }
                        else if (sColName.Equals("DeductCarLoan")) // DeductCarLoan (86)
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!IsValidBoolean(sColVal))
                                {
                                    sError = sError + "BOA|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                                sError = sError + "|";
                        }
                        else if (sColName.Equals("DeductScooterLoan")) // DeductScooterLoan (87)
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!IsValidBoolean(sColVal))
                                {
                                    sError = sError + "BOA|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                                sError = sError + "|";
                        }
                        else if (sColName.Equals("DeductSundryAdv")) // DeductSundryAdv (88)
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!IsValidBoolean(sColVal))
                                {
                                    sError = sError + "BOA|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                                sError = sError + "|";
                        }
                        else if (sColName.Equals("PetrolCharges")) // Petrol Charges (89)
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!IsNumeric(sColVal))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                                sError = sError + "|";
                        }
                        else if (sColName.Equals("DriverAllowance")) // Driver Allowance (90)
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!IsNumeric(sColVal))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                                sError = sError + "|";
                        }
                        else
                        {
                            sError = sError + "|";
                            sWarning = sWarning + "|";
                        }
                    }
                    dtSource.Rows[i]["error"] = sError;
                    dtSource.Rows[i]["warning"] = sWarning;
                    dtSource.Rows[i]["rowCount"] = i + 1;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int ImportMonthlyInputDetails(int userID, List<Model.ImportMonthlyInput> monthlyInputDetails)
        {
            log.Info($"ImportMonthlyInputService/ImportMonthlyInputDetails");
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("BranchCode", typeof(string));
                table.Columns.Add("SalMonth", typeof(int));
                table.Columns.Add("SalYear", typeof(int));
                table.Columns.Add("EmployeeCode", typeof(string));
                table.Columns.Add("E_Basic", typeof(decimal));
                table.Columns.Add("E_SP", typeof(decimal));
                table.Columns.Add("E_FDA", typeof(decimal));
                table.Columns.Add("E_01", typeof(decimal));
                table.Columns.Add("E_02", typeof(decimal));
                table.Columns.Add("E_03", typeof(decimal));
                table.Columns.Add("E_04", typeof(decimal));
                table.Columns.Add("E_05", typeof(decimal));
                table.Columns.Add("E_06", typeof(decimal));
                table.Columns.Add("E_07", typeof(decimal));
                table.Columns.Add("E_08", typeof(decimal));
                table.Columns.Add("E_09", typeof(decimal));
                table.Columns.Add("E_10", typeof(decimal));
                table.Columns.Add("E_11", typeof(decimal));
                table.Columns.Add("E_12", typeof(decimal));
                table.Columns.Add("E_13", typeof(decimal));
                table.Columns.Add("E_14", typeof(decimal));
                table.Columns.Add("E_15", typeof(decimal));
                table.Columns.Add("E_16", typeof(decimal));
                table.Columns.Add("E_17", typeof(decimal));
                table.Columns.Add("E_18", typeof(decimal));
                table.Columns.Add("E_19", typeof(decimal));
                table.Columns.Add("E_20", typeof(decimal));
                table.Columns.Add("E_21", typeof(decimal));
                table.Columns.Add("E_22", typeof(decimal));
                table.Columns.Add("E_23", typeof(decimal));
                table.Columns.Add("E_24", typeof(decimal));
                table.Columns.Add("E_25", typeof(decimal));
                table.Columns.Add("E_26", typeof(decimal));
                table.Columns.Add("E_27", typeof(decimal));
                table.Columns.Add("E_28", typeof(decimal));
                table.Columns.Add("E_29", typeof(decimal));
                table.Columns.Add("E_30", typeof(decimal));
                table.Columns.Add("D_PF", typeof(decimal));
                table.Columns.Add("D_VPF", typeof(decimal));
                table.Columns.Add("D_01", typeof(decimal));
                table.Columns.Add("D_02", typeof(decimal));
                table.Columns.Add("D_03", typeof(decimal));
                table.Columns.Add("D_04", typeof(decimal));
                table.Columns.Add("D_05", typeof(decimal));
                table.Columns.Add("D_06", typeof(decimal));
                table.Columns.Add("D_07", typeof(decimal));
                table.Columns.Add("D_08", typeof(decimal));
                table.Columns.Add("D_09", typeof(decimal));
                table.Columns.Add("D_10", typeof(decimal));
                table.Columns.Add("D_11", typeof(decimal));
                table.Columns.Add("D_12", typeof(decimal));
                table.Columns.Add("D_13", typeof(decimal));
                table.Columns.Add("D_14", typeof(decimal));
                table.Columns.Add("D_15", typeof(decimal));
                table.Columns.Add("D_16", typeof(decimal));
                table.Columns.Add("D_17", typeof(decimal));
                table.Columns.Add("D_18", typeof(decimal));
                table.Columns.Add("D_19", typeof(decimal));
                table.Columns.Add("D_20", typeof(decimal));
                table.Columns.Add("D_21", typeof(decimal));
                table.Columns.Add("D_22", typeof(decimal));
                table.Columns.Add("D_23", typeof(decimal));
                table.Columns.Add("D_24", typeof(decimal));
                table.Columns.Add("D_25", typeof(decimal));
                table.Columns.Add("D_26", typeof(decimal));
                table.Columns.Add("D_27", typeof(decimal));
                table.Columns.Add("D_28", typeof(decimal));
                table.Columns.Add("D_29", typeof(decimal));
                table.Columns.Add("D_30", typeof(decimal));
                table.Columns.Add("C_TotEarn", typeof(decimal));
                table.Columns.Add("C_TotDedu", typeof(decimal));
                table.Columns.Add("C_NetSal", typeof(decimal));
                table.Columns.Add("C_Pension", typeof(decimal));
                table.Columns.Add("C_GrossSalary", typeof(decimal));
                table.Columns.Add("SalaryLock", typeof(bool));
                table.Columns.Add("LWP", typeof(decimal));
                table.Columns.Add("OTHrs", typeof(decimal));
                table.Columns.Add("AOTHrs", typeof(decimal));
                table.Columns.Add("DeductPFLoan", typeof(bool));
                table.Columns.Add("DeductNB", typeof(bool));
                table.Columns.Add("DeductTCS", typeof(bool));
                table.Columns.Add("DeductHouseLoan", typeof(bool));
                table.Columns.Add("DeductFestivalLoan", typeof(bool));
                table.Columns.Add("DeductCarLoan", typeof(bool));
                table.Columns.Add("DeductScooterLoan", typeof(bool));
                table.Columns.Add("DeductSundryAdv", typeof(bool));
                table.Columns.Add("E_31", typeof(decimal));
                table.Columns.Add("E_32", typeof(decimal));
                table.Columns.Add("CreatedOn", typeof(DateTime));
                table.Columns.Add("CreatedBy", typeof(int));
                table.Columns.Add("EmployeeId", typeof(int));
                table.Columns.Add("BranchId", typeof(int));

                foreach (var item in monthlyInputDetails)
                {
                    var row = table.NewRow();
                    row["BranchCode"] = Convert.ToString(item.BranchCode);
                    row["SalMonth"] = Convert.ToInt32(item.Month);
                    row["SalYear"] = Convert.ToInt32(item.Year);
                    row["EmployeeCode"] = Convert.ToString(item.EmployeeCode);
                    row["E_Basic"] = Convert.ToDecimal(item.E_Basic);
                    row["E_SP"] = Convert.ToDecimal(item.E_SP);
                    row["E_FDA"] = Convert.ToDecimal(item.E_FDA);
                    row["E_01"] = Convert.ToDecimal(item.E_01);
                    row["E_02"] = Convert.ToDecimal(item.E_02);
                    row["E_03"] = Convert.ToDecimal(item.E_03);
                    row["E_04"] = Convert.ToDecimal(item.E_04);
                    row["E_05"] = Convert.ToDecimal(item.E_05);
                    row["E_06"] = Convert.ToDecimal(item.E_06);
                    row["E_07"] = Convert.ToDecimal(item.E_07);
                    row["E_08"] = Convert.ToDecimal(item.E_08);
                    row["E_09"] = Convert.ToDecimal(item.E_09);
                    row["E_10"] = Convert.ToDecimal(item.E_10);
                    row["E_11"] = Convert.ToDecimal(item.E_11);
                    row["E_12"] = Convert.ToDecimal(item.E_12);
                    row["E_13"] = Convert.ToDecimal(item.E_13);
                    row["E_14"] = Convert.ToDecimal(item.E_14);
                    row["E_15"] = Convert.ToDecimal(item.E_15);
                    row["E_16"] = Convert.ToDecimal(item.E_16);
                    row["E_17"] = Convert.ToDecimal(item.E_17);
                    row["E_18"] = Convert.ToDecimal(item.E_18);
                    row["E_19"] = Convert.ToDecimal(item.E_19);
                    row["E_20"] = Convert.ToDecimal(item.E_20);
                    row["E_21"] = Convert.ToDecimal(item.E_21);
                    row["E_22"] = Convert.ToDecimal(item.E_22);
                    row["E_23"] = Convert.ToDecimal(item.E_23);
                    row["E_24"] = Convert.ToDecimal(item.E_24);
                    row["E_25"] = Convert.ToDecimal(item.E_25);
                    row["E_26"] = Convert.ToDecimal(item.E_26);
                    row["E_27"] = Convert.ToDecimal(item.E_27);
                    row["E_28"] = Convert.ToDecimal(item.E_28);
                    row["E_29"] = Convert.ToDecimal(item.E_29);
                    row["E_30"] = Convert.ToDecimal(item.E_30);
                    row["D_PF"] = Convert.ToDecimal(item.D_PF);
                    row["D_VPF"] = Convert.ToDecimal(item.D_VPF);
                    row["D_01"] = Convert.ToDecimal(item.D_01);
                    row["D_02"] = Convert.ToDecimal(item.D_02);
                    row["D_03"] = Convert.ToDecimal(item.D_03);
                    row["D_04"] = Convert.ToDecimal(item.D_04);
                    row["D_05"] = Convert.ToDecimal(item.D_05);
                    row["D_06"] = Convert.ToDecimal(item.D_06);
                    row["D_07"] = Convert.ToDecimal(item.D_07);
                    row["D_08"] = Convert.ToDecimal(item.D_08);
                    row["D_09"] = Convert.ToDecimal(item.D_09);
                    row["D_10"] = Convert.ToDecimal(item.D_10);
                    row["D_11"] = Convert.ToDecimal(item.D_11);
                    row["D_12"] = Convert.ToDecimal(item.D_12);
                    row["D_13"] = Convert.ToDecimal(item.D_13);
                    row["D_14"] = Convert.ToDecimal(item.D_14);
                    row["D_15"] = Convert.ToDecimal(item.D_15);
                    row["D_16"] = Convert.ToDecimal(item.D_16);
                    row["D_17"] = Convert.ToDecimal(item.D_17);
                    row["D_18"] = Convert.ToDecimal(item.D_18);
                    row["D_19"] = Convert.ToDecimal(item.D_19);
                    row["D_20"] = Convert.ToDecimal(item.D_20);
                    row["D_21"] = Convert.ToDecimal(item.D_21);
                    row["D_22"] = Convert.ToDecimal(item.D_22);
                    row["D_23"] = Convert.ToDecimal(item.D_23);
                    row["D_24"] = Convert.ToDecimal(item.D_24);
                    row["D_25"] = Convert.ToDecimal(item.D_25);
                    row["D_26"] = Convert.ToDecimal(item.D_26);
                    row["D_27"] = Convert.ToDecimal(item.D_27);
                    row["D_28"] = Convert.ToDecimal(item.D_28);
                    row["D_29"] = Convert.ToDecimal(item.D_29);
                    row["D_30"] = Convert.ToDecimal(item.D_30);
                    row["C_TotEarn"] = Convert.ToDecimal(item.C_TotEarn);
                    row["C_TotDedu"] = Convert.ToDecimal(item.C_TotDedu);
                    row["C_NetSal"] = Convert.ToDecimal(item.C_NetSal);
                    row["C_Pension"] = Convert.ToDecimal(item.C_Pension);
                    row["C_GrossSalary"] = Convert.ToDecimal(item.C_GrossSalary);
                    row["SalaryLock"] = Convert.ToBoolean(item.SalaryLock);
                    row["LWP"] = Convert.ToDecimal(item.LWP);
                    row["OTHrs"] = Convert.ToDecimal(item.OTHrs);
                    row["AOTHrs"] = Convert.ToDecimal(item.AOTHrs);
                    row["DeductPFLoan"] = Convert.ToBoolean(item.DeductPFLoan);
                    row["DeductNB"] = Convert.ToBoolean(item.DeductNB);
                    row["DeductTCS"] = Convert.ToBoolean(item.DeductTCS);
                    row["DeductHouseLoan"] = Convert.ToBoolean(item.DeductHouseLoan);
                    row["DeductCarLoan"] = Convert.ToBoolean(item.DeductCarLoan);
                    row["DeductSundryAdv"] = Convert.ToBoolean(item.DeductSundryAdv);
                    row["E_31"] = Convert.ToDecimal(item.E_31);
                    row["E_32"] = Convert.ToDecimal(item.E_32);
                    row["CreatedOn"] = DateTime.Now;
                    row["CreatedBy"] = userID;
                    row["EmployeeId"] = Convert.ToInt32(item.EmployeeId);
                    row["BranchId"] = Convert.ToInt32(item.BranchId);
                    table.Rows.Add(row);
                }
                int month = Convert.ToInt32(table.Rows[0]["SalMonth"]);
                int year = Convert.ToInt32(table.Rows[0]["SalYear"]);
                int branchId = Convert.ToInt32(table.Rows[0]["BranchId"]);
                int res = dataImportRepo.ImportMonthlyInputData(userID, table, month, year, branchId);
                return res;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SalaryHead> GetHeadsFields(int employeeTypeID)
        {
            log.Info($"ImportMonthlyInputService/GetHeadsFields");
            try
            {
                var result = genericRepo.Get<DTOModel.SalaryHead>(x => x.IsDeleted == false && x.ActiveField == true && x.MonthlyInput == true && x.EmployeeTypeID == employeeTypeID).Select(x => new SalaryHead { FieldName = x.FieldName, FieldDesc = x.FieldDesc.Trim() }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public DataTable GetMonthlyInputData(int branchID, int? employeeTypeId, int? month, int? year)
        {
            log.Info($"ImportMonthlyInputService/GetMonthlyInputData");
            try
            {
                var result = dataImportRepo.GetMonthlyInputData(branchID, employeeTypeId, month, year);
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
