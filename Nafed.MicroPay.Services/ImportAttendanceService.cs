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
    public class ImportAttendanceService : BaseService, IImportAttendanceService
    {
        private readonly IExcelService excelService;
        private readonly IAttendanceImportRepository attendanceRepo;
        public ImportAttendanceService(IExcelService excelService, IAttendanceImportRepository attendanceRepo)
        {
            this.excelService = excelService;
            this.attendanceRepo = attendanceRepo;
        }
        public List<Model.EmpAttendanceForm> ReadAttendanceImportExcelData(string fileFullPath, bool isHeader, out string message, out int error, out int warning)
        {
            log.Info($"ImportAttendanceService/ReadAttendanceImportExcelData/{0}");
            message = string.Empty;
            error = 0; warning = 0;
            bool isValidHeaderList;
            List<string> missingHeaders;

            List<Model.EmpAttendanceForm> objModel = new List<EmpAttendanceForm>();

            var headerList = new List<string> { "EmpCode", "InDate", /*"OutDate"*/ "InTime", "OutTime", "Remarks" };
            DataTable dtExcel = ReadAndValidateHeader(fileFullPath, headerList, isHeader, out isValidHeaderList, out missingHeaders);

            if (isValidHeaderList)
            {
                DeleteTempFile(fileFullPath);


                DataTable dataWithHeader = excelService.GetDataWithHeader(dtExcel);
                if (dataWithHeader.Rows.Count > 0)
                {
                    DataTable dtEmpCodes = new DataTable();
                    DataTable dtEmpCodeFromDB = new DataTable();
                    AttendanceImportColValues colWithDistinctValues = GetDistinctValuesOfImportData(dataWithHeader);

                    if (colWithDistinctValues.empCode != null && colWithDistinctValues.empCode.Count() > 0)
                    {
                        var empCodes = (from x in colWithDistinctValues.empCode select new { ItemCode = x }).ToList();
                        dtEmpCodes = Common.ExtensionMethods.ToDataTable(empCodes);
                    }
                    dtEmpCodeFromDB = attendanceRepo.GetImportFieldValues(dtEmpCodes);
                    MergeAttendanceSheetDataWithDBValues(dataWithHeader, dtEmpCodeFromDB);
                    ValidateAttendanceImportData(dataWithHeader, out error, out warning);
                    ///====  Merge Imported Data with Database data value

                    ///
                    ///====  Validate Imported data with database values 


                    objModel = (from DataRow row in dataWithHeader.Rows
                                select new EmpAttendanceForm
                                {
                                    EmpCode = row["EmpCode"].ToString(),
                                    EmployeeName = row["EmployeeName"].ToString(),
                                    InDate = row["InDate"].ToString(),
                                    //  OutDate =Convert.ToDateTime( row["OutDate"].ToString()),
                                    InTime = row["InTime"].ToString(),
                                    OutTime = row["OutTime"].ToString(),
                                    EmployeeID = int.Parse(row["employeeID"].ToString()),
                                    Remarks = row["Remarks"].ToString(),
                                    error = row["error"].ToString(),
                                    warning = row["warning"].ToString()
                                }).ToList();    //.OrderBy(x => x.storeNumber).ThenBy(x => x.itemCode)




                    message = "success";
                    return objModel;
                }
            }
            return objModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sFilePath">File Path </param>
        /// <param name="isHeader"></param>
        /// <param name="headerList"></param>
        /// <param name="isValid"></param>
        /// <param name="sMissingHeader"></param>
        /// <returns></returns>
        /// 
        private DataTable ReadAndValidateHeader(string sFilePath, List<string> headerList, bool IsHeader, out bool isValid, out List<string> sMissingHeader)
        {

            log.Info($"ImportAttendanceService/ReadAndValidateHeader/{0}");

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
                        isValid = ValidateHeader(strList, headerList, out sMissingHeader,out inValidColumn);
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
            log.Info($"ImportAttendanceService/RemoveBlankRowAndColumns/{0}");
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

        private AttendanceImportColValues GetDistinctValuesOfImportData(DataTable dtImportData)
        {
            log.Info($"ImportAttendanceService/AttendanceImportColValues");
            try
            {
                AttendanceImportColValues colWithDistinctValues = new AttendanceImportColValues();
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

            log.Info($"ImportAttendanceService/ValidateAttendanceImportData");
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

       

        public int ImportAttendanceDetails(int userID, int userType, List<EmpAttendanceForm> attendanceDetails)
        {
            log.Info($"ImportAttendanceService/ImportAttendanceDetails");
            try
            {

                var dtoList = attendanceDetails.Select(x => new { EmployeeId= x.EmployeeID, ProxydateIn= x.InDate, InTime= x.InTime, OutTime= x.OutTime, Remarks=x.Remarks }).ToList();
                DataTable attendanceDT = new DataTable();
                attendanceDT = Common.ExtensionMethods.ToDataTable(dtoList);
                int res = attendanceRepo.ImportAttendanceData(userID, attendanceDT);
                return res;
            }
            catch 
            {
               // log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ;
            }
        }
    }
}
