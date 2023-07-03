using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.ImportExport.Interfaces;
using Nafed.MicroPay.ImportExport.Model;
using NPOI;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.DDF;
using NPOI.Util.Collections;
using System.IO;
using System.Collections;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Dynamic;

namespace Nafed.MicroPay.ImportExport
{
     public class Import : Base, IImport
    {
        public List<ExcelSheetList> GetWorksheetNames(string sFileFullPath)
        {
            string sFileType = System.IO.Path.GetExtension(sFileFullPath).ToString().ToLower();
            using (var fs = new FileStream(sFileFullPath, FileMode.Open, FileAccess.Read))
            {
                int iNoOfSheets = 0;
                List<ExcelSheetList> sheetList = new List<ExcelSheetList>();
                if (sFileType.Trim() == ".xls")
                {
                    HSSFWorkbook wb;
                    wb = new HSSFWorkbook(fs);
                    iNoOfSheets = wb.NumberOfSheets;
                    for (int i = 0; i < iNoOfSheets; i++)
                    {
                        sheetList.Add(new ExcelSheetList { sheetIndex = i, sheetName = wb.GetSheetAt(i).SheetName });
                    }
                }

                else if (sFileType.Trim() == ".xlsx")
                {
                    XSSFWorkbook wb;
                    wb = new XSSFWorkbook(fs);
                    iNoOfSheets = wb.NumberOfSheets;
                    for (int i = 0; i < iNoOfSheets; i++)
                    {
                        sheetList.Add(new ExcelSheetList { sheetIndex = i, sheetName = wb.GetSheetAt(i).SheetName });
                    }
                }
                return sheetList;
            }
        }

        public DataTable ReadExcelWithNoHeader(string sFileFullPath, int iSheetIndex = 0, string sSheetName = "")
        {
            string sFileType = Path.GetExtension(sFileFullPath).ToString().ToLower();
            DataTable data = new DataTable();
            using (var fs = new FileStream(sFileFullPath, FileMode.Open, FileAccess.Read))
            {
                IRow headerRow = null;
                int cellCount = 0;
                IEnumerator rows = null;
                NPOI.SS.UserModel.IFormulaEvaluator evaluator = null;
                if (sFileType.Trim() == ".xls")
                {
                    HSSFWorkbook wb;
                    HSSFSheet sh;
                    wb = new HSSFWorkbook(fs);
                    evaluator = wb.GetCreationHelper().CreateFormulaEvaluator();
                    if (!sSheetName.Equals(string.Empty))
                    {
                        sh = (HSSFSheet)wb.GetSheet(sSheetName);
                    }
                    else
                    {
                        sh = (HSSFSheet)wb.GetSheet(wb.GetSheetAt(iSheetIndex).SheetName);
                    }
                    if (sh == null)
                    {
                        sh = (HSSFSheet)wb.GetSheet(wb.GetSheetAt(0).SheetName);
                    }
                    headerRow = sh.GetRow(0);
                    cellCount = headerRow.LastCellNum;
                    rows = sh.GetRowEnumerator();
                }
                else if (sFileType.Trim() == ".xlsx")
                {
                    XSSFWorkbook wb;
                    XSSFSheet sh;
                    wb = new XSSFWorkbook(fs);
                    evaluator = wb.GetCreationHelper().CreateFormulaEvaluator();
                    if (!sSheetName.Equals(string.Empty))
                    {
                        sh = (XSSFSheet)wb.GetSheet(sSheetName);
                    }
                    else
                    {
                        sh = (XSSFSheet)wb.GetSheet(wb.GetSheetAt(iSheetIndex).SheetName);
                    }
                    if (sh == null)
                    {
                        sh = (XSSFSheet)wb.GetSheet(wb.GetSheetAt(0).SheetName);
                    }
                    //------------------------
                    headerRow = sh.GetRow(0);
                    cellCount = headerRow.LastCellNum;
                    rows = sh.GetRowEnumerator();
                }

                for (int j = headerRow.FirstCellNum; j < cellCount; j++)
                {
                    ICell cell = headerRow.GetCell(j);
                    if (cell != null)
                    {
                        string sHeadValue = string.Empty;
                        if (cell.CellType == CellType.Formula)
                        {
                            sHeadValue = evaluator.EvaluateInCell(cell).ToString();
                        }
                        else
                        {
                            sHeadValue = cell.ToString();
                        }
                    }
                    DataColumn column = new DataColumn(j.ToString());
                    data.Columns.Add(column);
                }
                while (rows.MoveNext())
                {
                    IRow row = null;
                    if (sFileType.Trim() == ".xls")
                    {
                        row = (HSSFRow)rows.Current;
                    }
                    else if (sFileType.Trim() == ".xlsx")
                    {
                        row = (XSSFRow)rows.Current;
                    }

                    DataRow dataRow = data.NewRow();
                    for (int i = 0; i < cellCount; i++)
                    {
                        ICell cell = row.GetCell(i);
                        if (cell != null)
                        {
                            string sCellVal = string.Empty;
                            sCellVal = getCellValue(cell);
                            dataRow[i] = sCellVal;
                        }
                    }
                    data.Rows.Add(dataRow);
                }
            }
            return data;
        }

        public DataTable ReadExcel(string sFileFullPath, bool IsHeader = true, int iSheetIndex = 0, string sSheetName = "")
        {
            ISheet sheet = null;
            IWorkbook workbook = null;
            DataTable data = new DataTable();
            int startRow = 0;
            string sFileType = Path.GetExtension(sFileFullPath).ToString().ToLower();
            if (IsHeader)
            {
                using (var fs = new FileStream(sFileFullPath, FileMode.Open, FileAccess.Read))
                {
                    if (sFileType.Trim() == ".xlsx")
                        workbook = new XSSFWorkbook(fs);
                    else if (sFileType.Trim() == ".xls")
                        workbook = new HSSFWorkbook(fs);
                    if (!sSheetName.Equals(string.Empty))
                    {
                        sheet = workbook.GetSheet(sSheetName);
                    }
                    else
                    {
                        sheet = workbook.GetSheet(workbook.GetSheetAt(iSheetIndex).SheetName);
                    }
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheet(workbook.GetSheetAt(0).SheetName);
                    }
                    if (sheet != null)
                    {
                        IRow firstRow = sheet.GetRow(0);
                        int cellCount = firstRow.LastCellNum;


                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = getCellValue(cell);
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;

                        int rowCount = sheet.LastRowNum;
                        for (int i = startRow; i <= rowCount; ++i)
                        {
                            IRow row = sheet.GetRow(i);
                            if (row == null) continue;
                            if (row.FirstCellNum >= 0)
                            {
                                DataRow dataRow = data.NewRow();
                                for (int j = row.FirstCellNum; j < cellCount; ++j)
                                {
                                    if (row.GetCell(j) != null)
                                        dataRow[j] = row.GetCell(j).ToString();
                                }
                                data.Rows.Add(dataRow);
                            }
                        }
                    }
                }
            }
            else
            {
                data = ReadExcelWithNoHeader(sFileFullPath, iSheetIndex, sSheetName);
            }
            return data;
        }

        public StringBuilder ReadExcelInSB(string sFileFullPath, int iSheetIndex = 0, string sSheetName = "")
        {
            ISheet sheet = null;
            IWorkbook workbook = null;
            DataTable data = new DataTable();
            StringBuilder sb = new StringBuilder();
            string sFileType = Path.GetExtension(sFileFullPath).ToString().ToLower();
            using (var fs = new FileStream(sFileFullPath, FileMode.Open, FileAccess.Read))
            {
                if (sFileType.Trim() == ".xlsx")
                    workbook = new XSSFWorkbook(fs);
                else if (sFileType.Trim() == ".xls")
                    workbook = new HSSFWorkbook(fs);
                if (!sSheetName.Equals(string.Empty))
                {
                    sheet = workbook.GetSheet(sSheetName);
                }
                else
                {
                    sheet = workbook.GetSheet(workbook.GetSheetAt(iSheetIndex).SheetName);
                }
                if (sheet == null)
                {
                    sheet = workbook.GetSheet(workbook.GetSheetAt(0).SheetName);
                }
                if (sheet != null)
                {
                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    int cellCount = headerRow.LastCellNum;
                    sb.Append("<table class='table'><tr>");
                    for (int j = 0; j < cellCount; j++)
                    {
                        ICell cell = headerRow.GetCell(j);
                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                        sb.Append("<th>" + cell.ToString() + "</th>");
                    }
                    sb.Append("</tr>");
                    sb.AppendLine("<tr>");
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        }
                        sb.AppendLine("</tr>");
                    }
                    sb.Append("</table>");
                }
                return sb;
            }
        }
        public List<ExpandoObject> ReadExcelDynamic(string sFileFullPath, int iSheetIndex = 0, string sSheetName = "")
        {
            string sFileType = Path.GetExtension(sFileFullPath).ToString().ToLower();
            List<ExpandoObject> DynData = new List<ExpandoObject>();
            ISheet sheet = null;
            IWorkbook workbook = null;
            using (var fs = new FileStream(sFileFullPath, FileMode.Open, FileAccess.Read))
            {
                if (sFileType.Trim() == ".xlsx")
                    workbook = new XSSFWorkbook(fs);
                else if (sFileType.Trim() == ".xls")
                    workbook = new HSSFWorkbook(fs);
                if (!sSheetName.Equals(string.Empty))
                {
                    sheet = workbook.GetSheet(sSheetName);
                }
                else
                {
                    sheet = workbook.GetSheet(workbook.GetSheetAt(iSheetIndex).SheetName);
                }
                if (sheet == null)
                {
                    sheet = workbook.GetSheet(workbook.GetSheetAt(0).SheetName);
                }
                if (sheet != null)
                {
                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    int cellCount = headerRow.LastCellNum;
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        dynamic DynRow = new ExpandoObject();
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                ((IDictionary<String, Object>)DynRow).Add(new KeyValuePair<String, Object>(headerRow.GetCell(j).ToString(), getCellValue(row.GetCell(j))));
                        }
                        DynData.Add(DynRow);
                    }
                }
                return DynData;
            }
        }

        public DataTable GetDataWithHeader(DataTable output)
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
                            col.ColumnName = rowFirst[col].ToString().Trim().Replace(" ", "");
                        }
                        else
                        {
                            col.ColumnName = rowFirst[col].ToString().Trim().Replace(" ", "");
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

        public string getCellValue(ICell cell)
        {
            string value = string.Empty;
            if (cell.CellType == CellType.Formula)
            {
                switch (cell.CachedFormulaResultType)
                {
                    case CellType.String:
                        value = cell.StringCellValue;
                        break;
                    case CellType.Numeric:
                        value = cell.NumericCellValue.ToString();
                        break;
                    case CellType.Boolean:
                        value = cell.BooleanCellValue.ToString();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                var r = cell.CellType;
                value = cell.ToString();
            }
            return value;
        }
        public DataTable ReadAndValidateHeader(string filePath, List<string> headerList, bool isHeader, out bool isValid, out List<string> lstMissingHeader)
        {
            isValid = false; lstMissingHeader = new List<string>();
            List<string> inValidColumn = new List<string>();
            try
            {
                using (DataTable dtExcel = ReadExcel(filePath, isHeader))
                {
                   
                    Common.ExtensionMethods.DeleteFile(filePath);
                    int rowCount = dtExcel.Rows.Count;
                    if (rowCount > 0)
                    {
                        List<string> strList = dtExcel.Rows[0].ItemArray.Select(o => o == null ? String.Empty : o.ToString()).ToList();
                        isValid = Common.DataValidation.ValidateHeader(strList, headerList, out lstMissingHeader, out inValidColumn);
                    }
                    return dtExcel;
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
