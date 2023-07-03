using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.ImportExport;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Nafed.MicroPay.Common.DataValidation;

namespace Nafed.MicroPay.Services
{
    public class ImportManualDataService : BaseService, IImportManualDataService
    {
        private readonly IExcelService excelService;
        private readonly IArrearRepository arrearRepo;
        public ImportManualDataService(IExcelService excelService, IArrearRepository arrearRepo)
        {
            this.excelService = excelService;
            this.arrearRepo = arrearRepo;
        }

        public List<Model.ArrearManualData> ReadAttendanceImportExcelData(string fileFullPath, bool isHeader, out string message, out int error, out int warning)
        {
            log.Info($"ImportManualDataService/ReadAttendanceImportExcelData/{0}");
            message = string.Empty;
            error = 0; warning = 0;
            bool isValidHeaderList;
            List<string> missingHeaders;

            List<Model.ArrearManualData> objModel = new List<ArrearManualData>();

            var headerList = new List<string> { "EmpCode", "Month",  "Year", "AdditionalOverTime", "ChildrenEducationAllowance", "DriverAllowance","IncomeTaxDeduction","LeaveWithoutPay","LifeInsurance", "MedicalReimbursement", "MiscellaneousDeduction1","MiscellaneousDeduction2","MiscellaneousDeduction3","MiscellaneousIncome1","MiscellaneousIncome2","MiscellaneousIncome3","NafedBazaarDeduction","NewsPaper","OverTime","PersonalPay","PetrolCharges","ProfessionalTax","SpecialAllowance", "SpecialPay", "SundryAdvance", "Telephone","TransportAllowance", "WashingAllowance"};
            DataTable dtExcel = ReadAndValidateHeader(fileFullPath, headerList, isHeader, out isValidHeaderList, out missingHeaders);

            if (isValidHeaderList)
            {
                DeleteTempFile(fileFullPath);


                DataTable dataWithHeader = excelService.GetDataWithHeader(dtExcel);
                if (dataWithHeader.Rows.Count > 0)
                {
                    DataTable dtEmpCodes = new DataTable();
                    DataTable dtEmpCodeFromDB = new DataTable();
                    ManualDataImportColValues colWithDistinctValues = GetDistinctValuesOfImportData(dataWithHeader);

                    if (colWithDistinctValues.empCode != null && colWithDistinctValues.empCode.Count() > 0)
                    {
                        var empCodes = (from x in colWithDistinctValues.empCode select new { ItemCode = x }).ToList();
                        dtEmpCodes = Common.ExtensionMethods.ToDataTable(empCodes);
                    }
                    dtEmpCodeFromDB = arrearRepo.GetImportFieldValues(dtEmpCodes);
                    MergeAttendanceSheetDataWithDBValues(dataWithHeader, dtEmpCodeFromDB);
                    ValidateAttendanceImportData(dataWithHeader, out error, out warning);
                    ///====  Merge Imported Data with Database data value

                    ///
                    ///====  Validate Imported data with database values 


                    objModel = (from DataRow row in dataWithHeader.Rows
                                select new ArrearManualData
                                {
                                    Empcode = row["EmpCode"].ToString(),
                                    Empname = row["Empname"].ToString(),
                                    Month = int.Parse(row["Month"].ToString()),
                                    Year = int.Parse(row["Year"].ToString()),
                                    AdditionalOverTime = Convert.ToDecimal(row["AdditionalOverTime"].ToString()==""?0: row["AdditionalOverTime"]),
                                    ChildrenEducationAllowance = Convert.ToDecimal(row["ChildrenEducationAllowance"].ToString()== "" ? 0 : row["ChildrenEducationAllowance"]),
                                    DriverAllowance = Convert.ToDecimal(row["DriverAllowance"].ToString() == "" ? 0 : row["DriverAllowance"]),
                                    IncomeTaxDeduction = Convert.ToDecimal(row["IncomeTaxDeduction"].ToString() == "" ? 0 : row["IncomeTaxDeduction"]),
                                    LeaveWithoutPay = Convert.ToDecimal(row["LeaveWithoutPay"].ToString() == "" ? 0 : row["LeaveWithoutPay"]),
                                    LifeInsurance = Convert.ToDecimal(row["LifeInsurance"].ToString() == "" ? 0 : row["LifeInsurance"]),
                                    MedicalReimbursement = Convert.ToDecimal(row["MedicalReimbursement"].ToString() == "" ? 0 : row["MedicalReimbursement"]),
                                    MiscellaneousDeduction1 = Convert.ToDecimal(row["MiscellaneousDeduction1"].ToString() == "" ? 0 : row["MiscellaneousDeduction1"]),
                                    MiscellaneousDeduction2 = Convert.ToDecimal(row["MiscellaneousDeduction2"].ToString() == "" ? 0 : row["MiscellaneousDeduction2"]),
                                    MiscellaneousDeduction3 = Convert.ToDecimal(row["MiscellaneousDeduction3"].ToString() == "" ? 0 : row["MiscellaneousDeduction3"]),
                                    MiscellaneousIncome1 = Convert.ToDecimal(row["MiscellaneousIncome1"].ToString() == "" ? 0 : row["MiscellaneousIncome1"]),
                                    MiscellaneousIncome2 = Convert.ToDecimal(row["MiscellaneousIncome2"].ToString() == "" ? 0 : row["MiscellaneousIncome2"]),
                                    MiscellaneousIncome3 = Convert.ToDecimal(row["MiscellaneousIncome3"].ToString() == "" ? 0 : row["MiscellaneousIncome3"]),
                                    NafedBazaarDeduction = Convert.ToDecimal(row["NafedBazaarDeduction"].ToString() == "" ? 0 : row["NafedBazaarDeduction"]),
                                    NewsPaper = Convert.ToDecimal(row["NewsPaper"].ToString() == "" ? 0 : row["NewsPaper"]),
                                    OverTime = Convert.ToDecimal(row["OverTime"].ToString() == "" ? 0 : row["OverTime"]),
                                    PersonalPay = Convert.ToDecimal(row["PersonalPay"].ToString() == "" ? 0 : row["PersonalPay"]),
                                    PetrolCharges = Convert.ToDecimal(row["PetrolCharges"].ToString() == "" ? 0 : row["PetrolCharges"]),
                                    ProfessionalTax = Convert.ToDecimal(row["ProfessionalTax"].ToString() == "" ? 0 : row["ProfessionalTax"]),
                                    SpecialAllowance = Convert.ToDecimal(row["SpecialAllowance"].ToString() == "" ? 0 : row["SpecialAllowance"]),
                                    SpecialPay = Convert.ToDecimal(row["SpecialPay"].ToString() == "" ? 0 : row["SpecialPay"]),
                                    SundryAdvance = Convert.ToDecimal(row["SundryAdvance"].ToString() == "" ? 0 : row["SundryAdvance"]),
                                    Telephone = Convert.ToDecimal(row["Telephone"].ToString() == "" ? 0 : row["Telephone"]),
                                    TransportAllowance = Convert.ToDecimal(row["TransportAllowance"].ToString() == "" ? 0 : row["TransportAllowance"]),
                                    WashingAllowance = Convert.ToDecimal(row["WashingAllowance"].ToString() == "" ? 0 : row["WashingAllowance"]),
                                    EmployeeId = int.Parse(row["employeeID"].ToString()),
                                    error = row["error"].ToString(),
                                    warning = row["warning"].ToString()
                                }).ToList();    //.OrderBy(x => x.storeNumber).ThenBy(x => x.itemCode)




                    message = "success";
                    return objModel;
                }
            }
            return objModel;
        }

        private DataTable ReadAndValidateHeader(string sFilePath, List<string> headerList, bool IsHeader, out bool isValid, out List<string> sMissingHeader)
        {

            log.Info($"ImportManualDataService/ReadAndValidateHeader/{0}");

            isValid = false; sMissingHeader = new List<string>();
            DataTable dtExcel = new DataTable();
            List<string> inValidColumn = new List<string>();
            try
            {
                AttendanceImport ob = new AttendanceImport();
                using (dtExcel = ob.ReadExcelWithNoHeader(sFilePath))
                {
                    RemoveBlankRowAndColumns(ref dtExcel, true);
                    int iExcelRowCount = dtExcel.Rows.Count;
                    if (iExcelRowCount > 0)
                    {
                        List<string> strList = dtExcel.Rows[0].ItemArray.Select(o => o == null ? String.Empty : o.ToString()).ToList();
                        isValid = ValidateHeader(strList, headerList, out sMissingHeader, out inValidColumn);
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
            log.Info($"ImportManualDataService/RemoveBlankRowAndColumns/{0}");
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

        private ManualDataImportColValues GetDistinctValuesOfImportData(DataTable dtImportData)
        {
            log.Info($"ImportManualDataService/AttendanceImportColValues");
            try
            {
                ManualDataImportColValues colWithDistinctValues = new ManualDataImportColValues();
                colWithDistinctValues.empCode = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "EmpCode").Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                return colWithDistinctValues;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        void MergeAttendanceSheetDataWithDBValues(DataTable excelData, DataTable dtEmpCodes)
        {
            try
            {
                excelData.Columns.Add("employeeID", typeof(int));
                excelData.Columns.Add("error", typeof(string));
                excelData.Columns.Add("warning", typeof(string));
                excelData.Columns.Add("rowCount", typeof(int));

                foreach (DataRow row in excelData.Rows)
                {
                    string sEmpCode = row["EmpCode"].ToString();
                    DataRow[] arrIC = dtEmpCodes.AsEnumerable().Where(x => x.Field<string>("EmpCode").Equals(sEmpCode, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (arrIC.Length > 0)
                    {
                        row["employeeID"] = arrIC[0]["EmployeeID"].ToString().Equals("") ? -1 : int.Parse(arrIC[0]["EmployeeID"].ToString());

                    }
                    else
                        row["employeeID"] = -1;

                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        void ValidateAttendanceImportData(DataTable dtSource, out int iError, out int iWarning)
        {
            iError = 0; iWarning = 0;

            log.Info($"ImportManualDataService/ValidateAttendanceImportData");
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
                        string sColName = dtSource.Columns[j].ColumnName.ToString().Trim().ToLower();
                        if (
                            sColName.ToLower().Equals("employeeID")
                            || sColName.ToLower().Equals("error")
                            || sColName.ToLower().Equals("rowcount") || sColName.ToLower().Equals("warning"))
                        {
                            continue;
                        }
                        string sColVal = dtSource.Rows[i][j].ToString().Trim();

                        if (sColName.Equals("empcode"))
                        {
                            #region Validate employee code
                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                int employeeID = int.Parse(dtSource.Rows[i]["employeeID"].ToString());
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
                        else if (sColName.Equals("indate"))
                        {

                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                DateTime dtInputDate, currentDate = DateTime.Now.Date;
                                bool isValidDate = DateTime.TryParse(sColVal, out dtInputDate);

                                if (!isValidDate)
                                {
                                    sError = sError + "NDF|";
                                    iError++;
                                }
                                else
                                {
                                    if (dtInputDate > System.DateTime.Now.Date)
                                    {
                                        sError = sError + "FDNA|";
                                        iError++;
                                    }
                                    else if ((currentDate - dtInputDate).TotalDays > 1)
                                    {
                                        sError = sError + "OPD|";
                                        iError++;
                                    }
                                    else
                                    {
                                        if (dtInputDate.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            sError = sError + "WKND|";
                                            iError++;
                                        }
                                        else
                                            sError = sError + "|";
                                    }
                                    //else
                                    //{
                                    //    if (!sColVal.IsValidDateFormat())
                                    //    {
                                    //        sError = sError + "IVDF|";
                                    //        iError++;
                                    //    }
                                    //}
                                }
                            }
                        }
                        else if (sColName.Equals("intime"))
                        {
                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                if (!sColVal.IsValidTimeIn24Format())
                                {
                                    sError = sError + "NTF|";
                                    iError++;
                                }
                                else
                                {
                                    string outTimeValue = dtSource.Rows[i]["outtime"].ToString().Trim();
                                    if (!IsBlank(outTimeValue) && outTimeValue.IsValidTimeIn24Format())
                                    {
                                        TimeSpan inTime = TimeSpan.Parse(sColVal);
                                        TimeSpan outTime = TimeSpan.Parse(outTimeValue);

                                        if (inTime > outTime)
                                        {
                                            sError = sError + "ITLTT|";
                                            iError++;
                                        }
                                        else
                                            sError = sError + "|";
                                    }
                                    else
                                        sError = sError + "|";
                                }
                            }
                        }
                        else if (sColName.Equals("outtime"))
                        {
                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                if (!sColVal.IsValidTimeIn24Format())
                                {
                                    sError = sError + "NTF|";
                                    iError++;
                                }
                                else
                                {
                                    string inTimeValue = dtSource.Rows[i]["intime"].ToString().Trim();
                                    if (!IsBlank(inTimeValue) && inTimeValue.IsValidTimeIn24Format())
                                    {
                                        TimeSpan outime = TimeSpan.Parse(sColVal);
                                        TimeSpan inTime = TimeSpan.Parse(inTimeValue);

                                        if (outime < inTime)
                                        {
                                            sError = sError + "OTLTT|";
                                            iError++;
                                        }
                                        else
                                            sError = sError + "|";
                                    }
                                    else
                                        sError = sError + "|";
                                }
                            }
                        }
                        else if (sColName.Equals("remarks"))
                        {
                            if (!IsBlank(sColVal))
                            {
                                if (sColVal.Length > 7)
                                {
                                    sError = sError + "MAXLEN|";
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
    }
}
