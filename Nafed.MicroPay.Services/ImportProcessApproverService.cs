using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using System.Data;
using Nafed.MicroPay.ImportExport;
using static Nafed.MicroPay.Common.DataValidation;
using System.Dynamic;

namespace Nafed.MicroPay.Services
{
    public class ImportProcessApproverService : BaseService, IImportProcessApproverService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IExcelService excelService;
        private readonly IProcessApproverRepository processAppRepo;
        public ImportProcessApproverService(IGenericRepository genericRepo,
            IExcelService excelService,
            IProcessApproverRepository processAppRepo)
        {
            this.genericRepo = genericRepo;
            this.excelService = excelService;
            this.processAppRepo = processAppRepo;
        }

        public List<Process> GetProcessList()
        {
            log.Info($"ImportProcessApproverList/GetProcessList/");

            try
            {
                var dtoProcesses = genericRepo.Get<DTOModel.Process>();
                Mapper.Initialize(cfg =>
                {

                    cfg.CreateMap<DTOModel.Process, Process>();
                });

                return Mapper.Map<List<Process>>(dtoProcesses);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public IEnumerable<dynamic> ReadImportMonthlyExcelData(string fileFullPath, bool isHeader,
      out string message, out int error, out int warning, out List<Model.ImportProcessApprovers> DATA,
      out List<string> missingHeaders, out List<string> columnName)
        {
            log.Info($"ImportProcessApproverList/ReadImportMonthlyExcelData/{0}");
            List<dynamic> objModel = new List<dynamic>();
            try
            {
                message = string.Empty;
                error = 0; warning = 0;
                bool isValidHeaderList;
                DATA = new List<Model.ImportProcessApprovers>();
                missingHeaders = new List<string>();
                columnName = new List<string>();
                DataTable dtExcelResult = new DataTable();
                Import ob = new Import();
                dtExcelResult = ob.ReadExcelWithNoHeader(fileFullPath);

                var headerList = new List<string>();

                string[] header =
                    {
                       "E.Code",
                       "ReportingTo","Reviewer","Acceptance Code"
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
                        DataTable dtReportingTo = new DataTable();
                        DataTable dtReviewerTo = new DataTable();
                        DataTable dtAcceptanceTo = new DataTable();

                        DataTable dtBranch = new DataTable();

                        Model.ApproverColValues colWithDistinctValues = GetDistinctValuesOfImportData(dataWithHeader);

                        if (colWithDistinctValues.ECode != null && colWithDistinctValues.ECode.Count() > 0)
                        {
                            var ECode = (from x in colWithDistinctValues.ECode select new { ItemCode = x }).ToList();
                            dtEmpCodes = Common.ExtensionMethods.ToDataTable(ECode);
                        }
                        else
                            dtEmpCodes.Columns.Add("ItemCode");


                        if (colWithDistinctValues.ReportingTo?.Count > 0)
                        {
                            var ReportingTo = (from x in colWithDistinctValues.ReportingTo select new { ItemCode = x }).ToList();
                            dtReportingTo = Common.ExtensionMethods.ToDataTable(ReportingTo);
                        }
                        else
                            dtReportingTo.Columns.Add("ItemCode");

                        if (colWithDistinctValues.ReviewerTo?.Count > 0)
                        {
                            var ReviewerTo = (from x in colWithDistinctValues.ReviewerTo select new { ItemCode = x }).ToList();
                            dtReviewerTo = Common.ExtensionMethods.ToDataTable(ReviewerTo);
                        }
                        else
                            dtReviewerTo.Columns.Add("ItemCode");

                        if (colWithDistinctValues.AcceptanceTo?.Count > 0)
                        {
                            var AcceptanceTo = (from x in colWithDistinctValues.AcceptanceTo select new { ItemCode = x }).ToList();
                            dtAcceptanceTo = Common.ExtensionMethods.ToDataTable(AcceptanceTo);
                        }
                        else
                            dtAcceptanceTo.Columns.Add("ItemCode");

                        DataSet dtImportFieldValueDB = new DataSet();
                        try
                        {
                            dtImportFieldValueDB = processAppRepo.GetImportFieldValues(dtEmpCodes, dtReportingTo, dtReviewerTo, dtAcceptanceTo);

                            MergeMonthlySheetDataWithDBValues(dataWithHeader, dtImportFieldValueDB.Tables[0], dtImportFieldValueDB.Tables[1], dtImportFieldValueDB.Tables[2], dtImportFieldValueDB.Tables[3]);
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

                        ValidateProcessApproverImportSheet(dataWithHeader, out error, out warning);

                        columnName = colum.Columns.Cast<DataColumn>()
                              .Select(x => x.ColumnName).ToList();
                      
                        columnName.Add("EmployeeId");
                        columnName.Add("error");
                        columnName.Add("warning");

                        for (int i = 0; i < dataWithHeader.Rows.Count; i++)
                        {
                            Model.ImportProcessApprovers MI = new Model.ImportProcessApprovers();
                            for (int j = 0; j < dataWithHeader.Columns.Count; j++)
                            {
                                try
                                {
                                    string sColName = dataWithHeader.Columns[j].ColumnName.ToString().Trim();

                                    if (sColName.Equals("E.Code", StringComparison.OrdinalIgnoreCase))
                                        MI.ECode = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if(sColName.Equals("ReportingTo",StringComparison.OrdinalIgnoreCase))
                                        MI.ReportingTo = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName.Equals("Reviewer", StringComparison.OrdinalIgnoreCase))
                                        MI.ReviewerTo = dataWithHeader.Rows[i][sColName].ToString().Trim();
                                    else if (sColName.Equals("AcceptanceCode", StringComparison.OrdinalIgnoreCase))
                                        MI.AcceptanceTo = dataWithHeader.Rows[i][sColName].ToString().Trim();


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
                    else
                    {
                        error = error + 1;
                        message = "Headers";
                    }
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

            log.Info($"ImportProcessApproverList/ReadAndValidateHeader/{0}");

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

        public dynamic RemoveProperty(Model.ImportProcessApprovers model)
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
        private Model.ApproverColValues GetDistinctValuesOfImportData(DataTable dtImportData)
        {
            log.Info($"ImportProcessApproverList/GetDistinctValuesOfImportData");
            try
            {
                Model.ApproverColValues colWithDistinctValues = new Model.ApproverColValues();

                colWithDistinctValues.ECode = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "E.Code").Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                colWithDistinctValues.ReportingTo = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "ReportingTo").Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                colWithDistinctValues.ReviewerTo = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "Reviewer").Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                colWithDistinctValues.AcceptanceTo = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "AcceptanceCode").Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                return colWithDistinctValues;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        void MergeMonthlySheetDataWithDBValues(DataTable excelData, DataTable dtEmpCodes, DataTable dtReportingTo, DataTable dtReviewerTo, DataTable dtAcceptanceTo)
        {
            try
            {
                excelData.Columns.Add("EmployeeId", typeof(int));
                excelData.Columns.Add("ReportingToID", typeof(int));
                excelData.Columns.Add("ReviewerToID", typeof(int));
                excelData.Columns.Add("AcceptanceToID", typeof(int));

                excelData.Columns.Add("error", typeof(string));
                excelData.Columns.Add("warning", typeof(string));
                excelData.Columns.Add("rowCount", typeof(int));

                foreach (DataRow row in excelData.Rows)
                {
                    string sEmpCode = row["E.Code"].ToString();

                    if (sEmpCode == "1321")
                    {
                       // var tt = 34;
                    }

                    if (sEmpCode.Length < 6)
                        sEmpCode = sEmpCode.PadLeft(4, '0');
                    DataRow[] arrEmpCode = dtEmpCodes.AsEnumerable().Where(x => x.Field<string>("EmployeeCode").Equals(sEmpCode, StringComparison.OrdinalIgnoreCase)).ToArray();

                    if (arrEmpCode.Length > 0)
                    {
                        row["EmployeeId"] = arrEmpCode[0]["EmployeeId"].ToString().Equals("") ? -1 : int.Parse(arrEmpCode[0]["EmployeeId"].ToString());
                    }
                    else
                        row["EmployeeId"] = -1;


                    string ReportingTo = row["ReportingTo"].ToString();
                    if (ReportingTo.Length < 6)
                        ReportingTo = ReportingTo.PadLeft(4, '0');

                    DataRow[] arrReporting = dtReportingTo.AsEnumerable().Where(x => x.Field<string>("EmployeeCode").Equals(ReportingTo, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (arrReporting.Length > 0)
                    {
                        row["ReportingToID"] = arrReporting[0]["EmployeeId"].ToString().Equals("") ? -1 : int.Parse(arrReporting[0]["EmployeeId"].ToString());
                    }
                    else
                        row["ReportingToID"] = -1;


                    string ReviewerTo = row["Reviewer"].ToString();
                    if (ReviewerTo.Length < 6)
                        ReviewerTo = ReviewerTo.PadLeft(4, '0');

                    DataRow[] arrReviewer = dtReviewerTo.AsEnumerable().Where(x => x.Field<string>("EmployeeCode").Equals(ReviewerTo, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (arrReviewer.Length > 0)
                    {
                        row["ReviewerToID"] = arrReviewer[0]["EmployeeId"].ToString().Equals("") ? -1 : int.Parse(arrReviewer[0]["EmployeeId"].ToString());
                    }
                    else
                        row["ReviewerToID"] = -1;

                    string AcceptanceTo = row["AcceptanceCode"].ToString();
                    if (AcceptanceTo.Length < 6)
                        AcceptanceTo = AcceptanceTo.PadLeft(4, '0');

                    DataRow[] arrAcceptance = dtAcceptanceTo.AsEnumerable().Where(x => x.Field<string>("EmployeeCode").Equals(AcceptanceTo, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (arrAcceptance.Length > 0)
                    {
                        row["AcceptanceToID"] = arrAcceptance[0]["EmployeeId"].ToString().Equals("") ? -1 : int.Parse(arrAcceptance[0]["EmployeeId"].ToString());
                    }
                    else
                        row["AcceptanceToID"] = -1;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        void ValidateProcessApproverImportSheet(DataTable dtSource, out int iError, out int iWarning)
        {
            iError = 0; iWarning = 0;

            log.Info($"ImportProcessApproverList/ValidateProcessApproverImportSheet");
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
                              sColName.ToLower().Equals("EmployeeId")
                             || sColName.ToLower().Equals("ReportingToID")
                             || sColName.ToLower().Equals("ReviewerToID")
                             || sColName.ToLower().Equals("AcceptanceToID")
                             || sColName.ToLower().Equals("error")
                             || sColName.ToLower().Equals("rowcount") || sColName.ToLower().Equals("warning"))
                        {
                            continue;
                        }
                        string sColVal = dtSource.Rows[i][j].ToString().Trim();

                      
                        if (sColName.Equals("E.Code")) // Empoyee Code (1)
                        {
                            #region Validate employee code

                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                int employeeID = int.Parse(dtSource.Rows[i]["EmployeeId"].ToString());
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

                        else if (sColName.Equals("ReportingTo")) // Reporting Code (1)
                        {
                            #region Validate employee code

                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                int reportingToID = int.Parse(dtSource.Rows[i]["ReportingToID"].ToString());
                                if (!(reportingToID > 0))
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
                        else if (sColName.Equals("Reviewer")) // Reviewer Code (1)
                        {
                            #region Validate Reviewer Code

                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                int reviewerToID = int.Parse(dtSource.Rows[i]["ReviewerToID"].ToString());

                                if (!(reviewerToID > 0))
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
                        else if (sColName.Equals("AcceptanceCode")) // Acceptance Code (1)
                        {
                            #region Validate Acceptance Code

                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                int acceptanceToID = int.Parse(dtSource.Rows[i]["AcceptanceToID"].ToString());
                                if (!(acceptanceToID > 0))
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

        public Model.ImportProcessApprovers AddCellValue(Model.ImportProcessApprovers input, string keyName, string cellValue)
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

        public int ImportProcessApproverData(int userID, int processID, List<ImportProcessApprovers> dataList)
        {
           
            log.Info($"ImportProcessApproverList/ImportProcessApproverData");
            try
            {
                DataTable table = new DataTable();

              
                table.Columns.Add("EmployeeId", typeof(int));
                table.Columns.Add("ReportingToID", typeof(int));
                table.Columns.Add("ReviewerToID", typeof(int));
                table.Columns.Add("AcceptanceToID", typeof(int));

                foreach (var item in dataList)
                {
                    var row = table.NewRow();
                    row["EmployeeId"] = Convert.ToInt32(item.EmployeeId);
                    row["ReportingToID"] = Convert.ToInt32(item.ReportingToID);
                    row["ReviewerToID"] = Convert.ToInt32(item.ReviewerToID);
                    row["AcceptanceToID"] = Convert.ToDecimal(item.AcceptanceToID);
                    table.Rows.Add(row);
                }
                 int res = processAppRepo.ImportProcessApproverData(userID, processID, table);
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
