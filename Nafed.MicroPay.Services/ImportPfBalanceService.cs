using Nafed.MicroPay.ImportExport;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nafed.MicroPay.Common.DataValidation;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Dynamic;

namespace Nafed.MicroPay.Services
{
    public class ImportPfBalanceService : BaseService, IImportPfBalanceService
    {
        private readonly IExcelService excelService;
        private readonly IPFAccountBalanceRepository pfBalRepo;
        public ImportPfBalanceService(IExcelService excelService, IPFAccountBalanceRepository pfBalRepo)
        {
            this.pfBalRepo = pfBalRepo;
            this.excelService = excelService;
        }
        public IEnumerable<dynamic> ReadImportMonthlyExcelData(string fileFullPath, bool isHeader, out string message, out int error, out int warning, out List<Model.ImportPfBalanceData> DATA, out List<string> missingHeaders, out List<string> columnName)
        {
            log.Info($"ImportPfBalanceService/ReadImportMonthlyExcelData/{0}");
            List<dynamic> objModel = new List<dynamic>();
            try
            {
                message = string.Empty;
                error = 0; warning = 0;
                bool isValidHeaderList;
                DATA = new List<Model.ImportPfBalanceData>();
                missingHeaders = new List<string>();
                columnName = new List<string>();
                DataTable dtExcelResult = new DataTable();
                Import ob = new Import();
                dtExcelResult = ob.ReadExcelWithNoHeader(fileFullPath);

                var headerList = new List<string>();

                string[] header =
                    {
                     "Branch Name", "Employee Code",
                     "Employee Name", "PF No","Month","Year", "Employee PF Contribution",
                     "VPF", "Employer PF Contribution","Pension",
                     "Withdrawl To Employee Contribution","Withdrawl To Employer Contribution",
                     "Addition To Employee Contribution", "Addition To Employer Contribution"
                     ,"Interest Withdrawl To Employee Contribution",
                     "Interest Withdrawl To Employer Contribution"
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

                        Model.PfBalanceColValues colWithDistinctValues = GetDistinctValuesOfImportData(dataWithHeader);

                        if (colWithDistinctValues.empCode != null && colWithDistinctValues.empCode.Count() > 0)
                        {
                            var empCodes = (from x in colWithDistinctValues.empCode select new { ItemCode = x }).ToList();
                            dtEmpCodes = Common.ExtensionMethods.ToDataTable(empCodes);
                        }
                        else
                            dtEmpCodes.Columns.Add("ItemCode");

                        if (colWithDistinctValues.PfNo != null && colWithDistinctValues.PfNo.Count() > 0)
                        {
                            var pfNo = (from x in colWithDistinctValues.PfNo select new { ItemCode = (x.GetType() == typeof(int) ? x : "0") }).ToList();
                            dtPfNo = Common.ExtensionMethods.ToDataTable(pfNo);
                        }
                        else
                            dtPfNo.Columns.Add("ItemCode");

                        if (colWithDistinctValues.BranchName?.Count > 0)
                        {
                            var bNames = (from x in colWithDistinctValues.BranchName select new { ItemCode = x }).ToList();
                            dtBranch = Common.ExtensionMethods.ToDataTable(bNames);
                        }
                        else
                            dtBranch.Columns.Add("ItemCode");

                        DataSet dtEmpPfNoFromDB = new DataSet();
                        try
                        {


                            dtEmpPfNoFromDB = pfBalRepo.GetImportFieldValues(dtEmpCodes, dtPfNo, dtBranch);
                            MergeMonthlySheetDataWithDBValues(dataWithHeader, dtEmpPfNoFromDB.Tables[0], dtEmpPfNoFromDB.Tables[1], dtEmpPfNoFromDB.Tables[2]);
                        }
                        catch (Exception ex)
                        {
                            dataWithHeader.Columns.Add("EmployeeId", typeof(int));
                            dataWithHeader.Columns.Add("BranchID", typeof(int));
                            dataWithHeader.Columns.Add("DB_PFNo", typeof(string));
                            dataWithHeader.Columns.Add("error", typeof(string));
                            dataWithHeader.Columns.Add("warning", typeof(string));
                            dataWithHeader.Columns.Add("rowCount", typeof(int));

                            log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                        }

                        ValidatePfBalanceImportSheetData(dataWithHeader, out error, out warning);

                        columnName = colum.Columns.Cast<DataColumn>()
                              .Select(x => x.ColumnName).ToList();
                        columnName.Add("EmployeeId");
                        columnName.Add("BranchID");
                        columnName.Add("DB_PFNo");
                        columnName.Add("error");
                        columnName.Add("warning");

                        for (int i = 0; i < dataWithHeader.Rows.Count; i++)
                        {
                            Model.ImportPfBalanceData MI = new Model.ImportPfBalanceData();
                            for (int j = 0; j < dataWithHeader.Columns.Count; j++)
                            {
                                string sColName = dataWithHeader.Columns[j].ColumnName.ToString().Trim();

                                if (sColName == "#")
                                    MI.Sno = int.Parse(dataWithHeader.Rows[i][sColName].ToString().Trim());
                                else if (sColName == "BranchName")
                                    MI.BranchName = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "EmployeeCode")
                                    MI.EmployeeCode = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "EmployeeName")
                                    MI.EmployeeName = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "PFNo")
                                    MI.PFNo = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "Month")
                                    MI.Month = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "Year")
                                    MI.Year = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "EmployeePFContribution")
                                    MI.EmployeePFContribution = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "VPF")
                                    MI.VPF = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "EmployerPFContribution")
                                    MI.EmployerPFContribution = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "Pension")
                                    MI.Pension = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "WithdrawlToEmployeeContribution")
                                    MI.WithdrawlToEmployeeContribution = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "WithdrawlToEmployerContribution")
                                    MI.WithdrawlToEmployerContribution = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "AdditionToEmployeeContribution")
                                    MI.AdditionToEmployeeContribution = dataWithHeader.Rows[i][sColName].ToString().Trim();

                                else if (sColName == "AdditionToEmployerContribution")
                                    MI.AdditionToEmployerContribution = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "InterestWithdrawlToEmployeeContribution")
                                    MI.InterestWithdrawlToEmployeeContribution = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else if (sColName == "InterestWithdrawlToEmployerContribution")
                                    MI.InterestWithdrawlToEmployerContribution = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                else
                                {
                                    if (sColName != null && sColName != "rowCount")
                                        AddCellValue(MI, sColName, dataWithHeader.Rows[i][j].ToString().Trim());
                                }
                            }
                            var dynamic = RemoveProperty(MI);
                            DATA.Add(MI);
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


        private Model.PfBalanceColValues GetDistinctValuesOfImportData(DataTable dtImportData)
        {
            log.Info($"ImportPfBalanceService/GetDistinctValuesOfImportData");
            try
            {
                Model.PfBalanceColValues colWithDistinctValues = new Model.PfBalanceColValues();
                colWithDistinctValues.empCode = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "EmployeeCode").Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                colWithDistinctValues.BranchName = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "BranchName").Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                colWithDistinctValues.PfNo = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "PFNo").Distinct(StringComparer.OrdinalIgnoreCase).ToList();

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
                excelData.Columns.Add("DB_PFNo", typeof(string));
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

                    string sPFNo = row["PFNo"].ToString();
                    DataRow[] arrPFNo = dtPFNo.AsEnumerable().Where(x => x.Field<string>("PFNo").Equals(sPFNo, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (arrPFNo.Length > 0)
                    {
                        row["DB_PFNo"] = arrPFNo[0]["PFNO1"].ToString().Equals("") ? "" : arrPFNo[0]["PFNO1"].ToString();

                    }
                    else
                        row["DB_PFNo"] = "";


                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        void ValidatePfBalanceImportSheetData(DataTable dtSource, out int iError, out int iWarning)
        {
            iError = 0; iWarning = 0;

            log.Info($"ImportPfBalanceService/ValidatePfBalanceImportSheetData");
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
                            || sColName.ToLower().Equals("rowcount") || sColName.ToLower().Equals("warning")
                            || sColName.ToLower().Equals("DB_PFNo"))
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
                            #region Validate Branch Name


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

                        //else if (sColName.Equals("PFNo", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    if (IsBlank(sColVal))
                        //    {
                        //        sError = sError + "B|";
                        //        iError++;
                        //    }
                        //    else
                        //    {
                        //        int dbPFo = 0;
                        //        bool validPFNo = int.TryParse(dtSource.Rows[i]["DB_PFNo"].ToString(), out dbPFo);
                        //        if (!validPFNo)
                        //        {
                        //            sError = sError + "NF|";
                        //            iError++;
                        //        }
                        //        else
                        //            sError = sError + "|";
                        //    }
                        //}
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
                        else if (sColName.Equals("EmployeePFContribution", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!(IsNumeric(sColVal) && sColVal.NoOfDecimalPlaces() <= 2))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                            {
                                iError++;
                                sError = sError + "B|";
                            }
                        }
                        else if (sColName.Equals("VPF", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!(IsNumeric(sColVal) && sColVal.NoOfDecimalPlaces() <= 2))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                            {
                                iError++;
                                sError = sError + "B|";
                            }
                        }
                        else if (sColName.Equals("EmployerPFContribution", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!(IsNumeric(sColVal) && sColVal.NoOfDecimalPlaces() <= 2))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                            {
                                iError++;
                                sError = sError + "B|";
                            }
                        }
                        else if (sColName.Equals("Pension", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!(IsNumeric(sColVal) && sColVal.NoOfDecimalPlaces() <= 2))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                            {
                                iError++;
                                sError = sError + "B|";
                            }
                        }
                        else if (sColName.Equals("WithdrawlToEmployeeContribution", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!(IsNumeric(sColVal) && sColVal.NoOfDecimalPlaces() <= 2))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                            {
                                iError++;
                                sError = sError + "B|";
                            }
                        }
                        else if (sColName.Equals("WithdrawlToEmployerContribution", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!(IsNumeric(sColVal) && sColVal.NoOfDecimalPlaces() <= 2))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                            {
                                iError++;
                                sError = sError + "B|";
                            }
                        }
                        else if (sColName.Equals("AdditionToEmployeeContribution", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!(IsNumeric(sColVal) && sColVal.NoOfDecimalPlaces() <= 2))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                            {
                                iError++;
                                sError = sError + "B|";
                            }
                        }
                        else if (sColName.Equals("AdditionToEmployerContribution", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!(IsNumeric(sColVal) && sColVal.NoOfDecimalPlaces() <= 2))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                            {
                                iError++;
                                sError = sError + "B|";
                            }
                        }
                        else if (sColName.Equals("InterestWithdrawlToEmployeeContribution", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!(IsNumeric(sColVal) && sColVal.NoOfDecimalPlaces() <= 2))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                            {
                                iError++;
                                sError = sError + "B|";
                            }

                        }
                        else if (sColName.Equals("InterestWithdrawlToEmployerContribution", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (!(IsNumeric(sColVal) && sColVal.NoOfDecimalPlaces() <= 2))
                                {
                                    sError = sError + "OTDP|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            else
                            {
                                iError++;
                                sError = sError + "B|";
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

        public dynamic RemoveProperty(Model.ImportPfBalanceData model)
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
        public Model.ImportPfBalanceData AddCellValue(Model.ImportPfBalanceData input, string keyName, string cellValue)
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
    }
}
