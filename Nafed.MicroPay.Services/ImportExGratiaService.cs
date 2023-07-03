using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System.IO;
using System.Data;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.ImportExport;
using System.Dynamic;
using static Nafed.MicroPay.Common.DataValidation;

namespace Nafed.MicroPay.Services
{
    public class ImportExGratiaService : BaseService, IImportExGratiaService
    {
        private readonly IExcelService excelService;
        private readonly IExgratiaRepository exgratiaRepo;
        public ImportExGratiaService(IExcelService excelService, IExgratiaRepository exgratiaRepo)
        {
            this.excelService = excelService;
            this.exgratiaRepo = exgratiaRepo;
        }

        public IEnumerable<dynamic> ReadImportMonthlyExcelData(string fileFullPath, bool isHeader,
            out string message, out int error, out int warning, out List<Model.ImportExgratiaData> DATA,
            out List<string> missingHeaders, out List<string> columnName)
        {
            log.Info($"ImportPfBalanceService/ReadImportMonthlyExcelData/{0}");
            List<dynamic> objModel = new List<dynamic>();
            try
            {
                message = string.Empty;
                error = 0; warning = 0;
                bool isValidHeaderList;
                DATA = new List<Model.ImportExgratiaData>();
                missingHeaders = new List<string>();
                columnName = new List<string>();
                DataTable dtExcelResult = new DataTable();
                Import ob = new Import();
                dtExcelResult = ob.ReadExcelWithNoHeader(fileFullPath);

                var headerList = new List<string>();

                string[] header =
                    {
                      "Employee Code",
                     "Employee Name","Branch Name","Designation", "Month","Year", "ExGratia","TDS","NetPay"
                };

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
                        DataTable dtPfNo = new DataTable();
                        DataTable dtBranch = new DataTable();

                        Model.ExgratiaColValues colWithDistinctValues = GetDistinctValuesOfImportData(dataWithHeader);

                        if (colWithDistinctValues.empCode != null && colWithDistinctValues.empCode.Count() > 0)
                        {
                            var empCodes = (from x in colWithDistinctValues.empCode select new { ItemCode = x }).ToList();
                            dtEmpCodes = Common.ExtensionMethods.ToDataTable(empCodes);
                        }
                        else
                            dtEmpCodes.Columns.Add("ItemCode");


                        if (colWithDistinctValues.BranchName?.Count > 0)
                        {
                            var bNames = (from x in colWithDistinctValues.BranchName select new { ItemCode = x }).ToList();
                            dtBranch = Common.ExtensionMethods.ToDataTable(bNames);
                        }
                        else
                            dtBranch.Columns.Add("ItemCode");

                        DataSet dtImportFieldValueDB = new DataSet();
                        try
                        {

                            dtImportFieldValueDB = exgratiaRepo.GetImportFieldValues(dtEmpCodes, dtBranch);
                            MergeMonthlySheetDataWithDBValues(dataWithHeader, dtImportFieldValueDB.Tables[0], dtImportFieldValueDB.Tables[1]);
                        }
                        catch (Exception ex)
                        {
                            dataWithHeader.Columns.Add("EmployeeId", typeof(int));
                            dataWithHeader.Columns.Add("BranchID", typeof(int));
                            dataWithHeader.Columns.Add("error", typeof(string));
                            dataWithHeader.Columns.Add("warning", typeof(string));
                            dataWithHeader.Columns.Add("rowCount", typeof(int));

                            log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                        }

                        ValidateExgratiaImportSheetData(dataWithHeader, out error, out warning);

                        columnName = colum.Columns.Cast<DataColumn>()
                              .Select(x => x.ColumnName).ToList();
                        columnName.Add("EmployeeId");
                        columnName.Add("BranchID");
                        columnName.Add("error");
                        columnName.Add("warning");

                        for (int i = 0; i < dataWithHeader.Rows.Count; i++)
                        {
                            Model.ImportExgratiaData MI = new Model.ImportExgratiaData();
                            for (int j = 0; j < dataWithHeader.Columns.Count; j++)
                            {
                                try
                                {
                                    string sColName = dataWithHeader.Columns[j].ColumnName.ToString().Trim();

                                    //if (sColName == "#")
                                    //    MI.Sno = int.Parse(dataWithHeader.Rows[i][sColName].ToString().Trim());
                                    if (sColName == "BranchName")
                                        MI.BranchName = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "Designation")
                                        MI.DesignationName = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "EmployeeCode")
                                        MI.EmployeeCode = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "EmployeeName")
                                        MI.EmployeeName = dataWithHeader.Rows[i][sColName].ToString().Trim();

                                    else if (sColName == "Month")
                                        MI.Month = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "Year")
                                        MI.Year = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "ExGratia")
                                        MI.ExGratia = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "TDS")
                                        MI.IncomeTaxDeduction = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    //else if (sColName == "NetPay")
                                    //    MI.NetPay = dataWithHeader.Rows[i][sColName].ToString().Trim();

                                    else
                                    {
                                        if (sColName != null && sColName != "rowCount")
                                            AddCellValue(MI, sColName, dataWithHeader.Rows[i][j].ToString().Trim());
                                    }

                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                                /////==== end =======
                            }
                            var dynamic = RemoveProperty(MI);
                            DATA.Add(MI);
                            objModel.Add(dynamic);
                        }
                        message = "success";
                        return objModel;
                        //}
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

        public IEnumerable<dynamic> ReadImportExGratiaIncomeTaxExcelData(string fileFullPath, bool isHeader,
            out string message, out int error, out int warning, out List<Model.ImportExgratiaIncomeTaxData> DATA,
            out List<string> missingHeaders, out List<string> columnName)
        {

            List<dynamic> objModel = new List<dynamic>();
            try
            {
                message = string.Empty;
                error = 0; warning = 0;
                bool isValidHeaderList;
                DATA = new List<Model.ImportExgratiaIncomeTaxData>();
                missingHeaders = new List<string>();
                columnName = new List<string>();
                DataTable dtExcelResult = new DataTable();
                Import ob = new Import();
                dtExcelResult = ob.ReadExcelWithNoHeader(fileFullPath);

                var headerList = new List<string>();

                string[] header =
                    {
                      "Emp Code",
                     "PFNO","Name","DOJ", "Designation","Financial Year", "Salary for Ex gratia","Amount of Ex gratia","Net Amount", "Income Tax"
                };

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
                        DataTable dtPfNo = new DataTable();
                        DataTable dtBranch = new DataTable();

                        Model.ExgratiaIncomeTaxColValues colWithDistinctValues = GetDistinctValuesOfImportDataExGratia(dataWithHeader);

                        if (colWithDistinctValues.empCode != null && colWithDistinctValues.empCode.Count() > 0)
                        {
                            var empCodes = (from x in colWithDistinctValues.empCode select new { ItemCode = x }).ToList();
                            dtEmpCodes = Common.ExtensionMethods.ToDataTable(empCodes);
                        }
                        else
                            dtEmpCodes.Columns.Add("ItemCode");

                        ValidateExgratiaIncomeTaxImportSheetData(dataWithHeader, out error, out warning);

                        columnName = colum.Columns.Cast<DataColumn>()
                              .Select(x => x.ColumnName).ToList();

                        columnName.Add("error");
                        columnName.Add("warning");

                        for (int i = 0; i < dataWithHeader.Rows.Count; i++)
                        {
                            Model.ImportExgratiaIncomeTaxData MI = new Model.ImportExgratiaIncomeTaxData();
                            for (int j = 0; j < dataWithHeader.Columns.Count; j++)
                            {
                                try
                                {
                                    string sColName = dataWithHeader.Columns[j].ColumnName.ToString().Trim();

                                    if (sColName == "EmpCode")
                                        MI.EmpCode = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "PFNO")
                                        MI.PFNO = dataWithHeader.Rows[i][sColName].ToString().Trim();

                                    else if (sColName == "Name")
                                        MI.Name = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "DOJ")
                                        MI.DOJ = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "Designation")
                                        MI.Designation = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "FinancialYear")
                                        MI.FinancialYear = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "SalaryforExgratia")
                                        MI.SalaryforExgratia = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "AmountofExgratia")
                                        MI.AmountofExgratia = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName == "NetAmount")
                                        MI.NetAmount = dataWithHeader.Rows[i][sColName].ToString().Trim();

                                    else
                                    {
                                        if (sColName != null && sColName != "rowCount")
                                            AddCellValueExGratia(MI, sColName, dataWithHeader.Rows[i][j].ToString().Trim());
                                    }

                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                                /////==== end =======
                            }
                            var dynamic = RemovePropertyExGratia(MI);
                            DATA.Add(MI);
                            objModel.Add(dynamic);
                        }
                        message = "success";
                        return objModel;
                        //}
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

        private DataTable ReadAndValidateHeader(string sFilePath, List<string> headerList, bool IsHeader, out bool isValid, out List<string> sMissingHeader)
        {

            log.Info($"ImportExGratiaService/ReadAndValidateHeader/{0}");

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
            log.Info($"ImportExGratiaService/RemoveBlankRowAndColumns/{0}");
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


        private Model.ExgratiaColValues GetDistinctValuesOfImportData(DataTable dtImportData)
        {
            log.Info($"ImportExGratiaService/GetDistinctValuesOfImportData");
            try
            {
                Model.ExgratiaColValues colWithDistinctValues = new Model.ExgratiaColValues();
                colWithDistinctValues.empCode = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "EmployeeCode").Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                colWithDistinctValues.BranchName = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "BranchName").Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                return colWithDistinctValues;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private Model.ExgratiaIncomeTaxColValues GetDistinctValuesOfImportDataExGratia(DataTable dtImportData)
        {
            log.Info($"ImportExGratiaService/GetDistinctValuesOfImportData");
            try
            {
                Model.ExgratiaIncomeTaxColValues colWithDistinctValues = new Model.ExgratiaIncomeTaxColValues();
                colWithDistinctValues.empCode = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "EmpCode").Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                return colWithDistinctValues;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        void MergeMonthlySheetDataWithDBValues(DataTable excelData, DataTable dtEmpCodes, DataTable dtPFNo, DataTable dtBranch)
        {
            try
            {
                excelData.Columns.Add("EmployeeId", typeof(int));
                excelData.Columns.Add("BranchID", typeof(int));
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

                    string BranchName = row["BranchName"].ToString();
                    DataRow[] arrBName = dtBranch.AsEnumerable().Where(x => x.Field<string>("BranchName").Equals(BranchName, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (arrBName.Length > 0)
                    {
                        row["BranchID"] = arrBName[0]["BranchID"].ToString().Equals("") ? -1 : int.Parse(arrBName[0]["BranchID"].ToString());
                    }
                    else
                        row["BranchID"] = -1;

                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        void ValidateExgratiaImportSheetData(DataTable dtSource, out int iError, out int iWarning)
        {
            iError = 0; iWarning = 0;

            log.Info($"ImportExGratiaService/ValidateExgratiaImportSheetData");
            try
            {
                iError = 0; iWarning = 0;
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    DataRow cDrow = dtSource.Rows[i];
                    string sError = string.Empty;
                    string sWarning = string.Empty;

                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        string sColName = dtSource.Columns[j].ColumnName.ToString().Trim()/*.ToLower()*/;

                        if (
                             //sColName.ToLower().Equals("#") ||
                             sColName.ToLower().Equals("EmployeeID")
                            || sColName.ToLower().Equals("error")
                            || sColName.ToLower().Equals("rowcount") || sColName.ToLower().Equals("warning"))
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
                        else if (sColName.Equals("BranchName", StringComparison.OrdinalIgnoreCase))
                        {
                            #region Validate Branch Name =========

                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                int BranchID = int.Parse(dtSource.Rows[i]["BranchID"].ToString());
                                if (!(BranchID > 0))
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

                        else if (sColName.Equals("ExGratia"))
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
                            }
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

        void ValidateExgratiaIncomeTaxImportSheetData(DataTable dtSource, out int iError, out int iWarning)
        {
            iError = 0; iWarning = 0;

            dtSource.Columns.Add("error", typeof(string));
            dtSource.Columns.Add("warning", typeof(string));
            dtSource.Columns.Add("rowCount", typeof(int));

            log.Info($"ImportExGratiaService/ValidateExgratiaIncomeTaxImportSheetData");
            try
            {
                iError = 0; iWarning = 0;
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    DataRow cDrow = dtSource.Rows[i];
                    string sError = string.Empty;
                    string sWarning = string.Empty;

                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        string sColName = dtSource.Columns[j].ColumnName.ToString().Trim()/*.ToLower()*/;
                        string sColVal = dtSource.Rows[i][j].ToString().Trim();

                        if (sColName.Equals("EmpCode")) // Empoyee Code (1)
                        {
                            #region Validate employee code
                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                int empCode = int.Parse(dtSource.Rows[i]["EmpCode"].ToString());
                                if (!(empCode > 0))
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
                        else if (sColName.Equals("Name", StringComparison.OrdinalIgnoreCase))
                        {
                            #region Validate Employee Name =========

                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                                sError = sError + "|";                          
                            #endregion
                        }
                        else if (sColName.Equals("FinancialYear", StringComparison.OrdinalIgnoreCase))
                        {
                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                                sError = sError + "|";
                        }
                        else if (sColName.Equals("SalaryforExgratia"))
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
                                    sError = sError + "|";
                            }
                        }

                        else if (sColName.Equals("AmountofExgratia"))
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
                                    sError = sError + "|";
                            }
                        }
                        else if (sColName.Equals("NetAmount"))
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
                                    sError = sError + "|";
                            }
                        }
                        else if (sColName.Equals("IncomeTax"))
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
                                    sError = sError + "|";
                            }
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

        public dynamic RemoveProperty(Model.ImportExgratiaData model)
        {
            try
            {
                var returnClass = new ExpandoObject() as IDictionary<string, object>;
                var pprop = model.GetType().GetProperties();
                foreach (var pr in pprop)
                {
                    var val = pr.GetValue(model);
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
        public Model.ImportExgratiaData AddCellValue(Model.ImportExgratiaData input, string keyName, string cellValue)
        {
            try
            {
                var prop = input.GetType().GetProperty(keyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                prop.SetValue(input, cellValue, null);
                var returnClass = new ExpandoObject() as IDictionary<string, object>;
                returnClass.Add(keyName, cellValue);
                return input;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public dynamic RemovePropertyExGratia(Model.ImportExgratiaIncomeTaxData model)
        {
            try
            {
                var returnClass = new ExpandoObject() as IDictionary<string, object>;
                var pprop = model.GetType().GetProperties();
                foreach (var pr in pprop)
                {
                    var val = pr.GetValue(model);
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
        public Model.ImportExgratiaIncomeTaxData AddCellValueExGratia(Model.ImportExgratiaIncomeTaxData input, string keyName, string cellValue)
        {
            try
            {
                var prop = input.GetType().GetProperty(keyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                prop.SetValue(input, cellValue, null);
                var returnClass = new ExpandoObject() as IDictionary<string, object>;
                returnClass.Add(keyName, cellValue);
                return input;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        void MergeMonthlySheetDataWithDBValues(DataTable excelData, DataTable dtEmpCodes, DataTable dtBranch)
        {
            try
            {
                excelData.Columns.Add("EmployeeId", typeof(int));
                excelData.Columns.Add("BranchID", typeof(int));
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

                    string BranchName = row["BranchName"].ToString();
                    DataRow[] arrBName = dtBranch.AsEnumerable().Where(x => x.Field<string>("BranchName").Equals(BranchName, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (arrBName.Length > 0)
                    {
                        row["BranchID"] = arrBName[0]["BranchID"].ToString().Equals("") ? -1 : int.Parse(arrBName[0]["BranchID"].ToString());
                    }
                    else
                        row["BranchID"] = -1;

                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int ImportExGratiaManualData(int userID, List<ImportExgratiaData> dataList)
        {
            log.Info($"ImportExGratiaService/ImportExGratiaManualData");

            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("BranchID", typeof(int));
                table.Columns.Add("EmployeeId", typeof(int));

                table.Columns.Add("Year", typeof(int));
                table.Columns.Add("Month", typeof(int));
                table.Columns.Add("ExGratia", typeof(decimal));
                table.Columns.Add("TDS", typeof(decimal));
                table.Columns.Add("NetPay", typeof(decimal));

                foreach (var item in dataList)
                {
                    var row = table.NewRow();
                    row["Year"] = Convert.ToInt32(item.Year);
                    row["Month"] = Convert.ToInt32(item.Month);
                    row["BranchID"] = Convert.ToInt32(item.BranchID);
                    row["ExGratia"] = Convert.ToDecimal(item.ExGratia);
                    row["TDS"] = Convert.ToDecimal(item.IncomeTaxDeduction);
                    row["NetPay"] = Convert.ToDecimal(item.NetPay);
                    row["EmployeeId"] = Convert.ToInt32(item.EmployeeId);
                    table.Rows.Add(row);
                }

                int month = Convert.ToInt32(table.Rows[0]["Month"]);
                int year = Convert.ToInt32(table.Rows[0]["Year"]);

                int res = exgratiaRepo.ImportExGratiaManualData(userID, table, month, year);
                return res;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int ImportExGratiaIncomeTaxData(int userID, List<ImportExgratiaIncomeTaxData> dataList)
        {
            log.Info($"ImportExGratiaService/ImportExGratiaIncomeTaxData");

            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("EmpCode", typeof(string));
                table.Columns.Add("PFNO", typeof(string));
                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("SalaryforExgratia", typeof(decimal));
                table.Columns.Add("AmountofExgratia", typeof(decimal));
                table.Columns.Add("NetAmount", typeof(decimal));
                table.Columns.Add("IncomeTax", typeof(decimal));

                foreach (var item in dataList)
                {
                    var row = table.NewRow();
                    row["EmpCode"] = Convert.ToString(item.EmpCode);
                    row["PFNO"] = Convert.ToString(item.PFNO);
                    row["Name"] = Convert.ToString(item.Name);                    
                    row["SalaryforExgratia"] = Convert.ToDecimal(item.SalaryforExgratia);
                    row["AmountofExgratia"] = Convert.ToDecimal(item.AmountofExgratia);
                    row["NetAmount"] = Convert.ToDecimal(item.NetAmount);
                    row["IncomeTax"] = Convert.ToDecimal(item.IncomeTax);
                    table.Rows.Add(row);
                }

                string financialYear = dataList.Select(x => x.FinancialYear).FirstOrDefault();

                int res = exgratiaRepo.ImportExGratiaIncomeTaxData(userID, table,financialYear);
                return res;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }




    }
}
