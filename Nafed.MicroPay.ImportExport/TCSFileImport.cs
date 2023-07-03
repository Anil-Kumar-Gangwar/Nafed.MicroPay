using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Collections;
using NPOI.XSSF.UserModel;
using System.Data;

namespace Nafed.MicroPay.ImportExport
{
    public class TCSFileImport : Base
    {

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

        public string getCellValue(ICell cell, int? timeFieldColIndex = null)
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
                if (cell.CellType == CellType.Numeric && cell.DateCellValue.Hour > 0)
                    value = cell.DateCellValue.TimeOfDay.ToString();

                else
                    value = cell.ToString();
            }
            return value;
        }
    }
}
