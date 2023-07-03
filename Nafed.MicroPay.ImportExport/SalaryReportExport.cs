using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Nafed.MicroPay.ImportExport
{
    public class SalaryReportExport : BaseExcel
    {
        private SalaryReportExport()
        {

        }
        public static string MonthlyBranchWiseExportToExcel(int salYear, int salMonth, IEnumerable<string> headers, DataTable rowData, int earningCols, int deductionCols, string sSheetName, string sFullPath)
        {
            log.Info("SalaryReportExport/MonthWiseExportToExcel");

            try
            {
                var rowCount = rowData.Rows.Count;
                rowData.Rows[rowCount - 1][1] = "Total";

                int columnCount = headers.ToList().Count;
                XSSFWorkbook workbook = CreateWookBook();

                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);

                    XSSFFont topHdrFont = CreateFont(workbook);

                    topHdrFont.FontHeightInPoints = 18;
                    topHdrFont.FontName = "Calibri";
                    topHdrFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    #region  ----- Top Header -=============

                    XSSFFont titleFont = CreateFont(workbook);
                    titleFont.FontHeightInPoints = 14;
                    titleFont.FontName = "Calibri";
                    titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(titleFont);
                    titlecell.CellStyle.WrapText = true;
                    titlerow.Height = 1200;
                    titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                    titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 33));

                    #endregion

                    #region ----== Report Caption ===================

                    XSSFFont hdrCapFont = CreateFont(workbook);
                    hdrCapFont.FontHeightInPoints = 12;
                    hdrCapFont.FontName = "Calibri";
                    hdrCapFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow hdrCaption = CreateRow(sheet, 1);
                    ICell hdrCapCell = hdrCaption.CreateCell(0);
                    hdrCapCell.CellStyle = workbook.CreateCellStyle();
                    hdrCapCell.CellStyle.SetFont(hdrCapFont);
                    hdrCapCell.CellStyle.WrapText = true;

                    //  hdrCaption.Height = 1200;
                    hdrCapCell.CellStyle.Alignment = HorizontalAlignment.Center;
                    hdrCapCell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    hdrCapCell.SetCellValue($"Monthly Pay Summary for the month {new DateTime(salYear, salMonth, 1).ToString("MMM")} ,{salYear}");

                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 33));

                    #endregion

                    #region   ======== ////==== Report Header Label Value  ==============

                    XSSFFont rLabel1Font = CreateFont(workbook);
                    rLabel1Font.FontHeightInPoints = 11;
                    rLabel1Font.FontName = "Calibri";
                    rLabel1Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow reportLabel1 = CreateRow(sheet, 2);
                    ICell rLabel1Cell = reportLabel1.CreateCell(0);
                    rLabel1Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel1Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel1Cell.CellStyle.WrapText = true;
                    //  hdrCaption.Height = 1200;

                    rLabel1Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel1Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    rLabel1Cell.SetCellValue($"Print Date : {DateTime.Now.ToString("dd-MM-yyyy")}");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 33));

                    XSSFRow reportLabel2 = CreateRow(sheet, 3);
                    ICell rLabel2Cell = reportLabel2.CreateCell(0);
                    rLabel2Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel2Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel2Cell.CellStyle.WrapText = true;

                    //  hdrCaption.Height = 1200;
                    rLabel2Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel2Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                    if (rowCount == 2)
                        rLabel2Cell.SetCellValue($"Branch : {rowData.Rows[0]["BranchName"].ToString()}");
                    else
                        rLabel2Cell.SetCellValue("Branch : All Branches (Except HO)");

                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 33));

                    #endregion

                    XSSFRow dataRow;
                    XSSFRow headerRow = CreateRow(sheet, 5);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);

                    XSSFRow tbCaption = CreateRow(sheet, 4);

                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 2));

                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 3, (3 + (earningCols - 1))));
                    // sheet.AddMergedRegion(new CellRangeAddress(2, 2, 3 + earningCols, 3 + earningCols));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 3 + earningCols + 1, 3 + earningCols + deductionCols));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 3 + earningCols + deductionCols + 1, 3 + earningCols + deductionCols + 2));

                    SetBordersToMergedCells(workbook, sheet);

                    var firstCellOfFourthRow = tbCaption.CreateCell(0);
                    firstCellOfFourthRow.CellStyle = workbook.CreateCellStyle();
                    firstCellOfFourthRow.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    firstCellOfFourthRow.CellStyle.FillPattern = FillPattern.SolidForeground;

                    var lastCellOfFouthRow = tbCaption.CreateCell(3 + earningCols + deductionCols + 1);
                    lastCellOfFouthRow.CellStyle = workbook.CreateCellStyle();
                    lastCellOfFouthRow.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    lastCellOfFouthRow.CellStyle.FillPattern = FillPattern.SolidForeground;


                    var earingHeaderCells = tbCaption.CreateCell(3);
                    earingHeaderCells.SetCellValue("TOTAL EARNINGS");

                    earingHeaderCells.CellStyle = workbook.CreateCellStyle();
                    earingHeaderCells.CellStyle.SetFont(hFont);
                    earingHeaderCells.CellStyle.Alignment = HorizontalAlignment.Center;
                    earingHeaderCells.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    earingHeaderCells.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    earingHeaderCells.CellStyle.FillPattern = FillPattern.SolidForeground;
                    earingHeaderCells.CellStyle.BorderBottom = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderTop = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderLeft = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderRight = BorderStyle.Thin;

                    var deductionHeaderCells = tbCaption.CreateCell(3 + earningCols + 1);
                    deductionHeaderCells.SetCellValue("TOTAL DEDUCTIONS");

                    deductionHeaderCells.CellStyle = workbook.CreateCellStyle();
                    deductionHeaderCells.CellStyle.SetFont(hFont);
                    deductionHeaderCells.CellStyle.Alignment = HorizontalAlignment.Center;
                    deductionHeaderCells.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    deductionHeaderCells.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    deductionHeaderCells.CellStyle.FillPattern = FillPattern.SolidForeground;
                    deductionHeaderCells.CellStyle.BorderBottom = BorderStyle.Thin;
                    deductionHeaderCells.CellStyle.BorderTop = BorderStyle.Thin;
                    deductionHeaderCells.CellStyle.BorderLeft = BorderStyle.Thin;
                    deductionHeaderCells.CellStyle.BorderRight = BorderStyle.Thin;

                    var centerBlackCell = tbCaption.CreateCell(3 + earningCols);
                    centerBlackCell.CellStyle = workbook.CreateCellStyle();
                    centerBlackCell.CellStyle.SetFont(hFont);

                    centerBlackCell.CellStyle.BorderBottom = BorderStyle.Thin;
                    centerBlackCell.CellStyle.BorderTop = BorderStyle.Thin;
                    centerBlackCell.CellStyle.BorderLeft = BorderStyle.Thin;
                    centerBlackCell.CellStyle.BorderRight = BorderStyle.Thin;

                    centerBlackCell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    centerBlackCell.CellStyle.FillPattern = FillPattern.SolidForeground;

                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.WrapText = true;
                        //   cell.CellStyle.ShrinkToFit = true;
                        if (hdr.Equals("BranchName", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Branch Name");
                        else if (hdr.Equals("SNo", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("S.No.");
                        else if (hdr.Equals("NoOfEmployee", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("No of Employee");
                        else
                            cell.SetCellValue(hdr);

                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;

                        cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                        cell.CellStyle.FillPattern = FillPattern.SolidForeground;

                        if (hdr == "BranchName")
                            sheet.SetColumnWidth(hdrColIndex, 6000);

                        else if (hdr == "NoOfEmployee")
                            sheet.SetColumnWidth(hdrColIndex, 2500);

                        else if (hdr == "Basic")
                            sheet.SetColumnWidth(hdrColIndex, 3500);

                        else if (hdrColIndex > 3)
                            sheet.SetColumnWidth(hdrColIndex, 3500);

                        headerRow.Height = 900;
                        hdrColIndex++;
                    }

                    var execlRow = sheet.GetRow(5);
                    ICell totalE_Cell = execlRow.GetCell(3 + earningCols);
                    ICell totalD_Cell = execlRow.GetCell(3 + earningCols + deductionCols + 1);
                    totalE_Cell.SetCellValue("TOTAL");
                    totalD_Cell.SetCellValue("TOTAL");

                    for (int jj = 0; jj < columnCount; jj++)
                    {
                        int rowIndex = 6;

                        if (jj == 0)
                            dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                        else
                            dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                        var cell = dataRow.CreateCell(jj);

                        if (jj > 0)
                        {
                            int numVal;
                            if (int.TryParse(jj.ToString(), out numVal))
                                cell.SetCellValue(numVal);
                            else
                                cell.SetCellValue(jj);
                        }
                        else
                            cell.SetCellValue("");

                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;

                        dataRow.Height = 300;
                    }

                    foreach (DataColumn column in rowData.Columns)
                    {
                        int rowIndex = 7;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            if (!row[1].ToString().Trim().Equals("Total"))
                            {
                                if (!row[column].ToString().Trim().Equals(""))
                                {
                                    ICell Cell = dataRow.CreateCell(column.Ordinal);
                                    Cell.CellStyle = workbook.CreateCellStyle();
                                    Cell.CellStyle.BorderBottom = BorderStyle.Thin;

                                    if (column.Ordinal != 1)
                                        Cell.CellStyle.Alignment = HorizontalAlignment.Right;

                                    if (column.Ordinal == 0)
                                        Cell.CellStyle.Alignment = HorizontalAlignment.Center;

                                    if (column.Ordinal > 2)
                                    {
                                        Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                        Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

                                        if (column.Ordinal == earningCols + 3)  ///=== Add Sum Formula for total of earning heads ===
                                        {
                                            var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(3));
                                            var fCellRefAsString = fromCellRef.FormatAsString();

                                            var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(column.Ordinal - 1));
                                            var toCellRefAsString = toCellRef.FormatAsString();

                                            hFont.FontHeightInPoints = 11;
                                            hFont.FontName = "Calibri";
                                            hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                            Cell.CellStyle.SetFont(hFont);
                                            Cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");
                                        }
                                        else if (column.Ordinal == 3 + earningCols + deductionCols + 1) ///=== Add Sum Formula for total of deduction heads ===
                                        {
                                            var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(3 + earningCols + 1));
                                            var fCellRefAsString = fromCellRef.FormatAsString();

                                            var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(column.Ordinal - 1));
                                            var toCellRefAsString = toCellRef.FormatAsString();

                                            hFont.FontHeightInPoints = 11;
                                            hFont.FontName = "Calibri";
                                            hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                            Cell.CellStyle.SetFont(hFont);
                                            Cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");
                                        }
                                        else if (column.Ordinal == rowData.Columns.Count - 1) ///=== Formula to get Net Salary 
                                        {
                                            var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(earningCols + 3));
                                            var fCellRefAsString = fromCellRef.FormatAsString();

                                            var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(3 + earningCols + deductionCols + 1));
                                            var toCellRefAsString = toCellRef.FormatAsString();

                                            hFont.FontHeightInPoints = 11;
                                            hFont.FontName = "Calibri";
                                            hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                            Cell.CellStyle.SetFont(hFont);
                                            Cell.SetCellFormula($"{fCellRefAsString}-{toCellRefAsString}");
                                        }
                                    }
                                    else if (column.Ordinal == 2)
                                    {
                                        int num;
                                        bool f = int.TryParse(row[column].ToString(), out num);
                                        Cell.SetCellValue(num);
                                    }
                                    else
                                        Cell.SetCellValue(row[column].ToString());
                                }
                            }
                            else
                            {
                                if (column.Ordinal > 0)
                                {
                                    var cell = dataRow.CreateCell(column.Ordinal);

                                    if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(decimal)))
                                        cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                    else if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(int)))
                                        cell.SetCellValue((int)row[column]);
                                    else
                                    {
                                        int numVal;
                                        if (int.TryParse(row[column].ToString(), out numVal))
                                            cell.SetCellValue(numVal);
                                        else if (!row[column].ToString().Equals("total", StringComparison.OrdinalIgnoreCase))
                                            cell.SetCellValue(row[column].ToString());
                                    }

                                    cell.CellStyle = workbook.CreateCellStyle();
                                    cell.CellStyle.SetFont(hFont);

                                    if (column.Ordinal > 2)
                                    {
                                        #region  ====  Data Formating & Set Cell Formula ============

                                        var fromCellRef = new CellReference(sheet.GetRow(7).GetCell(column.Ordinal));
                                        var fCellRefAsString = fromCellRef.FormatAsString();

                                        var toRowNo = sheet.LastRowNum - 1;
                                        var toCellRef = new CellReference(sheet.GetRow(toRowNo).GetCell(column.Ordinal));
                                        var toCellRefAsString = toCellRef.FormatAsString();
                                        cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");
                                        cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

                                        #endregion
                                    }
                                    else if (column.Ordinal == 2)
                                    {
                                        var fromCellRef = new CellReference(sheet.GetRow(7).GetCell(column.Ordinal));
                                        var fCellRefAsString = fromCellRef.FormatAsString();

                                        var toRowNo = sheet.LastRowNum - 1;
                                        var toCellRef = new CellReference(sheet.GetRow(toRowNo).GetCell(column.Ordinal));
                                        var toCellRefAsString = toCellRef.FormatAsString();
                                        cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");
                                        //  cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00");
                                        cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
                                    }
                                    if (column.Ordinal != 1)
                                        cell.CellStyle.Alignment = HorizontalAlignment.Right;
                                    else if (column.Ordinal == 2)
                                        cell.CellStyle.Alignment = HorizontalAlignment.Center;

                                    cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                    cell.CellStyle.BorderTop = BorderStyle.Thin;
                                    cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                    cell.CellStyle.BorderRight = BorderStyle.Thin;

                                    cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                                    cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                                }
                                else
                                {
                                    var cell = dataRow.CreateCell(column.Ordinal);
                                    cell.CellStyle = workbook.CreateCellStyle();
                                    cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                    cell.CellStyle.BorderTop = BorderStyle.Thin;
                                    cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                    cell.CellStyle.BorderRight = BorderStyle.Thin;
                                    cell.CellStyle.SetFont(hFont);
                                    cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                                    cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                                    cell.SetCellValue("Total");
                                }
                                dataRow.Height = 300;
                            }
                            rowIndex++;
                        }
                        //  if(column.Ordinal ==1)  sheet.AutoSizeColumn(column.Ordinal);
                    }

                    workbook.Write(file);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return "fail - " + ex.Message;
            }
        }

        public static string MonthlyEmployeeWiseExportToExcel(IEnumerable<string> headers, DataTable table, DataTable insuranceGroupDt, DataTable dtTotal, int earningCols, int deductionCols, string sSheetName, string sFullPath)
        {
            log.Info($"SalaryReportExport/MonthlyEmployeeWiseExportToExcel/");

            try
            {
                int columnCount = headers.ToList().Count;
                int rowFromIndex = 7;
                XSSFWorkbook workbook = CreateWookBook();

                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);

                    XSSFFont topHdrFont = CreateFont(workbook);

                    topHdrFont.FontHeightInPoints = 18;
                    topHdrFont.FontName = "Calibri";
                    topHdrFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    #region  ----- Top Header -=============

                    XSSFFont titleFont = CreateFont(workbook);
                    titleFont.FontHeightInPoints = 14;
                    titleFont.FontName = "Calibri";
                    titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);

                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(titleFont);

                    titlecell.CellStyle.WrapText = true;
                    titlerow.Height = 1200;

                    titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                    titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 35));

                    #endregion

                    #region ----== Report Caption ===================

                    XSSFFont hdrCapFont = CreateFont(workbook);
                    hdrCapFont.FontHeightInPoints = 12;
                    hdrCapFont.FontName = "Calibri";
                    hdrCapFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow hdrCaption = CreateRow(sheet, 1);
                    ICell hdrCapCell = hdrCaption.CreateCell(0);

                    hdrCapCell.CellStyle = workbook.CreateCellStyle();
                    hdrCapCell.CellStyle.SetFont(hdrCapFont);
                    hdrCapCell.CellStyle.WrapText = true;

                    //  hdrCaption.Height = 1200;
                    hdrCapCell.CellStyle.Alignment = HorizontalAlignment.Center;
                    hdrCapCell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    hdrCapCell.SetCellValue($"Employee wise report for the month {dtTotal.Rows[0]["month"].ToString()} ,{dtTotal.Rows[0]["year"].ToString()}");

                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 35));

                    #endregion

                    #region   ======== ////==== Report Header Label Value  ==============

                    XSSFFont rLabel1Font = CreateFont(workbook);
                    rLabel1Font.FontHeightInPoints = 11;
                    rLabel1Font.FontName = "Calibri";
                    rLabel1Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow reportLabel1 = CreateRow(sheet, 2);
                    ICell rLabel1Cell = reportLabel1.CreateCell(0);
                    rLabel1Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel1Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel1Cell.CellStyle.WrapText = true;
                    //  hdrCaption.Height = 1200;

                    rLabel1Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel1Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    rLabel1Cell.SetCellValue($"Print Date : {DateTime.Now.ToString("dd-MM-yyyy")}");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 35));

                    XSSFRow reportLabel2 = CreateRow(sheet, 3);
                    ICell rLabel2Cell = reportLabel2.CreateCell(0);
                    rLabel2Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel2Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel2Cell.CellStyle.WrapText = true;

                    //  hdrCaption.Height = 1200;
                    rLabel2Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel2Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                    //  if (rowCount == 2)
                    rLabel2Cell.SetCellValue($"Branch : {dtTotal.Rows[0]["BranchName"].ToString()}");
                    //else
                    //    rLabel2Cell.SetCellValue("Branch : All Branches (Except HO)");

                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 35));

                    #endregion

                    XSSFRow dataRow;
                    XSSFRow headerRow = CreateRow(sheet, 5);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);

                    XSSFRow tbCaption = CreateRow(sheet, 4);

                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 4));

                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 5, (5 + (earningCols - 1))));
                    // sheet.AddMergedRegion(new CellRangeAddress(2, 2, 3 + earningCols, 3 + earningCols));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 5 + earningCols + 1, 5 + earningCols + deductionCols));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 5 + earningCols + deductionCols + 1, 5 + earningCols + deductionCols + 2));

                    SetBordersToMergedCells(workbook, sheet);

                    var firstCellOfFourthRow = tbCaption.CreateCell(0);
                    firstCellOfFourthRow.CellStyle = workbook.CreateCellStyle();
                    firstCellOfFourthRow.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    firstCellOfFourthRow.CellStyle.FillPattern = FillPattern.SolidForeground;

                    var lastCellOfFouthRow = tbCaption.CreateCell(5 + earningCols + deductionCols + 1);
                    lastCellOfFouthRow.CellStyle = workbook.CreateCellStyle();
                    lastCellOfFouthRow.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    lastCellOfFouthRow.CellStyle.FillPattern = FillPattern.SolidForeground;


                    var earingHeaderCells = tbCaption.CreateCell(5);
                    earingHeaderCells.SetCellValue("TOTAL EARNINGS");

                    earingHeaderCells.CellStyle = workbook.CreateCellStyle();
                    earingHeaderCells.CellStyle.SetFont(hFont);
                    earingHeaderCells.CellStyle.Alignment = HorizontalAlignment.Center;
                    earingHeaderCells.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    earingHeaderCells.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    earingHeaderCells.CellStyle.FillPattern = FillPattern.SolidForeground;
                    earingHeaderCells.CellStyle.BorderBottom = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderTop = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderLeft = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderRight = BorderStyle.Thin;

                    var deductionHeaderCells = tbCaption.CreateCell(5 + earningCols + 1);
                    deductionHeaderCells.SetCellValue("TOTAL DEDUCTIONS");

                    deductionHeaderCells.CellStyle = workbook.CreateCellStyle();
                    deductionHeaderCells.CellStyle.SetFont(hFont);
                    deductionHeaderCells.CellStyle.Alignment = HorizontalAlignment.Center;
                    deductionHeaderCells.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    deductionHeaderCells.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    deductionHeaderCells.CellStyle.FillPattern = FillPattern.SolidForeground;
                    deductionHeaderCells.CellStyle.BorderBottom = BorderStyle.Thin;
                    deductionHeaderCells.CellStyle.BorderTop = BorderStyle.Thin;
                    deductionHeaderCells.CellStyle.BorderLeft = BorderStyle.Thin;
                    deductionHeaderCells.CellStyle.BorderRight = BorderStyle.Thin;

                    var centerBlackCell = tbCaption.CreateCell(5 + earningCols);
                    centerBlackCell.CellStyle = workbook.CreateCellStyle();
                    centerBlackCell.CellStyle.SetFont(hFont);

                    centerBlackCell.CellStyle.BorderBottom = BorderStyle.Thin;
                    centerBlackCell.CellStyle.BorderTop = BorderStyle.Thin;
                    centerBlackCell.CellStyle.BorderLeft = BorderStyle.Thin;
                    centerBlackCell.CellStyle.BorderRight = BorderStyle.Thin;

                    centerBlackCell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    centerBlackCell.CellStyle.FillPattern = FillPattern.SolidForeground;

                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        var cell = headerRow.CreateCell(hdrColIndex);

                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.WrapText = true;

                        if (hdr.Equals("BranchName", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Branch Name");

                        else if (hdr.Equals("SNo", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("S.No.");

                        else if (hdr.Equals("Employee", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Employee Name");

                        else if (hdr.Equals("EmployeeCode", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Employee Code");

                        else
                            cell.SetCellValue(hdr);

                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;

                        cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                        cell.CellStyle.FillPattern = FillPattern.SolidForeground;

                        headerRow.Height = 900;

                        if (hdr == "Employee")
                            sheet.SetColumnWidth(hdrColIndex, 9000);

                        else if (hdr == "BranchName")
                            sheet.SetColumnWidth(hdrColIndex, 6000);

                        else if (hdr == "EmployeeCode")
                            sheet.SetColumnWidth(hdrColIndex, 2500);

                        else if (hdr.Equals("designation", StringComparison.OrdinalIgnoreCase))
                            sheet.SetColumnWidth(hdrColIndex, 9700);

                        else if (hdrColIndex > 4)
                            sheet.SetColumnWidth(hdrColIndex, 3500);

                        hdrColIndex++;
                    }

                    //var execlRow = sheet.GetRow(5);                   
                    //ICell totalE_Cell = execlRow.GetCell(5 + earningCols);
                    //ICell totalD_Cell = execlRow.GetCell(5 + earningCols + deductionCols + 1);
                    //totalE_Cell.SetCellValue("TOTAL");
                    //totalD_Cell.SetCellValue("TOTAL");

                    for (int jj = 0; jj < columnCount; jj++)
                    {
                        int rowIndex = 6;

                        if (jj == 0)
                            dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                        else
                            dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                        var cell = dataRow.CreateCell(jj);

                        if (jj > 0)
                        {
                            int numVal;
                            if (int.TryParse((jj).ToString(), out numVal))
                                cell.SetCellValue(numVal);
                            else
                                cell.SetCellValue(jj);
                        }
                        else
                            cell.SetCellValue("");

                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;

                        dataRow.Height = 300;
                    }

                    foreach (DataColumn column in table.Columns)
                    {
                        int rowIndex = 7;

                        foreach (DataRow row in table.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            if (int.Parse(row["SNo"].ToString()) == 1)
                                rowFromIndex = rowIndex;

                            if (int.Parse(row["SNO"].ToString()) > 0 && table.Rows.IndexOf(row) < table.Rows.Count)
                            {
                                ICell Cell = dataRow.CreateCell(column.Ordinal);
                                Cell.CellStyle.FillBackgroundColor = HSSFColor.Red.Index;
                                Cell.CellStyle = workbook.CreateCellStyle();

                                Cell.CellStyle.BorderBottom = BorderStyle.Thin;

                                if (column.Ordinal == 0 || column.Ordinal == 2)
                                {
                                    Cell.SetCellValue(row[column].ToString());
                                    Cell.CellStyle.Alignment = HorizontalAlignment.Center;
                                }
                                else if (column.Ordinal > 4)
                                {
                                    Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                    Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                                    Cell.CellStyle.Alignment = HorizontalAlignment.Right;

                                    ///===== t_0_0_0_0______

                                    if (column.Ordinal == earningCols + 5)  ///=== Add Sum Formula for total of earning heads ===
                                    {
                                        var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(5));
                                        var fCellRefAsString = fromCellRef.FormatAsString();

                                        var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(column.Ordinal - 1));
                                        var toCellRefAsString = toCellRef.FormatAsString();

                                        hFont.FontHeightInPoints = 11;
                                        hFont.FontName = "Calibri";
                                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                        Cell.CellStyle.SetFont(hFont);
                                        Cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");
                                    }
                                    else if (column.Ordinal == 5 + earningCols + deductionCols + 1) ///=== Add Sum Formula for total of deduction heads ===
                                    {
                                        var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(5 + earningCols + 1));
                                        var fCellRefAsString = fromCellRef.FormatAsString();

                                        var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(column.Ordinal - 1));
                                        var toCellRefAsString = toCellRef.FormatAsString();

                                        hFont.FontHeightInPoints = 11;
                                        hFont.FontName = "Calibri";
                                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                        Cell.CellStyle.SetFont(hFont);
                                        Cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");
                                    }
                                    else if (column.Ordinal == table.Columns.Count - 1) ///=== Formula to get Net Salary 
                                    {
                                        var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(earningCols + 5));
                                        var fCellRefAsString = fromCellRef.FormatAsString();

                                        var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(5 + earningCols + deductionCols + 1));
                                        var toCellRefAsString = toCellRef.FormatAsString();

                                        hFont.FontHeightInPoints = 11;
                                        hFont.FontName = "Calibri";
                                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                        Cell.CellStyle.SetFont(hFont);
                                        Cell.SetCellFormula($"{fCellRefAsString}-{toCellRefAsString}");
                                    }

                                    ////====m_0_00_000

                                }
                                else
                                    Cell.SetCellValue(row[column].ToString());
                            }
                            else
                            {
                                var cell = dataRow.CreateCell(column.Ordinal);
                                cell.CellStyle = workbook.CreateCellStyle();

                                if (column.Ordinal > 0)
                                {
                                    if (column.Ordinal > 4)
                                    {
                                        cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                        cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                                        cell.CellStyle.Alignment = HorizontalAlignment.Right;

                                        //if (column.Ordinal == (earningCols + 5))
                                        //{
                                        var fromCellRef = new CellReference(sheet.GetRow(rowFromIndex).GetCell(column.Ordinal));
                                        var fCellRefAsString = fromCellRef.FormatAsString();

                                        var toCellRef = new CellReference(sheet.GetRow(rowIndex - 1).GetCell(column.Ordinal));
                                        var toCellRefAsString = toCellRef.FormatAsString();

                                        hFont.FontHeightInPoints = 11;
                                        hFont.FontName = "Calibri";
                                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                        cell.CellStyle.SetFont(hFont);
                                        cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");
                                        // }
                                    }
                                }
                                else
                                {
                                    cell.SetCellValue($"Total ");
                                    cell.CellStyle.Alignment = HorizontalAlignment.Left;
                                }
                                cell.CellStyle.SetFont(hFont);

                                cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                cell.CellStyle.BorderTop = BorderStyle.Thin;
                                cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                cell.CellStyle.BorderRight = BorderStyle.Thin;

                                cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                                cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                                dataRow.Height = 300;
                            }
                            rowIndex++;
                        }
                        // sheet.AutoSizeColumn(column.Ordinal);
                    }

                    #region  =====  Insurance Group Table & Summary Table.. ======

                    dataRow = (XSSFRow)sheet.CreateRow(sheet.LastRowNum + 2);

                    ICell tableCaption = dataRow.CreateCell(1, NPOI.SS.UserModel.CellType.String);
                    tableCaption.SetCellValue("INSURANCE AMOUNT BREAK-UP");

                    ICell summaryLabelCell = dataRow.CreateCell(4, NPOI.SS.UserModel.CellType.String);
                    summaryLabelCell.SetCellValue($"CONTROL INFORMATION FOR BRANCHWISE PAY SUMMARY PERIOD -{dtTotal.Rows[0]["month"]}, {dtTotal.Rows[0]["year"]}");

                    for (int i = 0; i < insuranceGroupDt.Rows.Count; i++)
                    {
                        dataRow = (XSSFRow)sheet.CreateRow(sheet.LastRowNum + 1);

                        var cellGroup = dataRow.CreateCell(1, NPOI.SS.UserModel.CellType.String);
                        cellGroup.SetCellValue($"GROUP {insuranceGroupDt.Rows[i][0].ToString()}");

                        var cellRate = dataRow.CreateCell(2, NPOI.SS.UserModel.CellType.String);
                        cellRate.SetCellValue($"(@ ₹ {Convert.ToDecimal(insuranceGroupDt.Rows[i][1]).ToString("0.##")})");
                        cellRate.CellStyle = workbook.CreateCellStyle();

                        cellRate.CellStyle.Alignment = HorizontalAlignment.Right;
                        cellRate.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                        if (dtTotal.Rows.Count > 0)
                        {
                            var cellSummaryRowKey = dataRow.CreateCell(4, NPOI.SS.UserModel.CellType.String);

                            if (i == 0)
                                cellSummaryRowKey.SetCellValue($"TOTAL NUMBER OF RECORDS =");
                            else if (i == 1)
                                cellSummaryRowKey.SetCellValue($"TOTAL EARNINGS =");
                            else if (i == 2)
                                cellSummaryRowKey.SetCellValue($"TOTAL DEDUCTIONS =");
                            else if (i == 3)
                                cellSummaryRowKey.SetCellValue($"TOTAL NET PAY =");

                            var cellSummaryRowValue = dataRow.CreateCell(5, NPOI.SS.UserModel.CellType.Numeric);
                            if (i == 0)
                                cellSummaryRowValue.SetCellValue(Convert.ToDouble(dtTotal.Rows[0]["NoOfRecords"]));
                            else if (i == 1)
                                cellSummaryRowValue.SetCellValue(Convert.ToDouble(dtTotal.Rows[0]["Total Earning"]));
                            else if (i == 2)
                                cellSummaryRowValue.SetCellValue(Convert.ToDouble(dtTotal.Rows[0]["Total Deduction"]));
                            else if (i == 3)
                                cellSummaryRowValue.SetCellValue(Convert.ToDouble(dtTotal.Rows[0]["Net Salary"]));

                            cellSummaryRowValue.CellStyle.Alignment = HorizontalAlignment.Right;
                        }
                    }

                    #endregion

                    workbook.Write(file);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return "fail - " + ex.Message;
            }
        }

        public static bool ExportSalaryReportEmployeeWise(DataSet dsSource, string sFullPath, int earningCols, int deductionCols, string fileName)
        {
            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    foreach (DataTable table in dsSource.Tables)
                    {
                        if (table.TableName == "Table")
                        {
                            XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(fileName);
                            XSSFFont topHdrFont = CreateFont(workbook);

                            topHdrFont.FontHeightInPoints = 18;
                            topHdrFont.FontName = "Calibri";
                            topHdrFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                            #region  ----- Top Header -=============

                            XSSFFont titleFont = CreateFont(workbook);
                            titleFont.FontHeightInPoints = 14;
                            titleFont.FontName = "Calibri";
                            titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                            XSSFRow titlerow = CreateRow(sheet, 0);
                            ICell titlecell = titlerow.CreateCell(0);

                            titlecell.CellStyle = workbook.CreateCellStyle();
                            titlecell.CellStyle.SetFont(titleFont);

                            titlecell.CellStyle.WrapText = true;
                            titlerow.Height = 1200;

                            titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                            titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");

                            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 33));

                            #endregion


                            #region ----== Report Caption ===================

                            XSSFFont hdrCapFont = CreateFont(workbook);
                            hdrCapFont.FontHeightInPoints = 12;
                            hdrCapFont.FontName = "Calibri";
                            hdrCapFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                            XSSFRow hdrCaption = CreateRow(sheet, 1);
                            ICell hdrCapCell = hdrCaption.CreateCell(0);
                            hdrCapCell.CellStyle = workbook.CreateCellStyle();
                            hdrCapCell.CellStyle.SetFont(hdrCapFont);
                            hdrCapCell.CellStyle.WrapText = true;

                            //  hdrCaption.Height = 1200;
                            hdrCapCell.CellStyle.Alignment = HorizontalAlignment.Center;
                            hdrCapCell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            hdrCapCell.SetCellValue($"Employee Wise Annual Report");

                            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 33));

                            #endregion

                            #region   ======== ////==== Report Header Label Value  ==============

                            XSSFFont rLabel1Font = CreateFont(workbook);
                            rLabel1Font.FontHeightInPoints = 11;
                            rLabel1Font.FontName = "Calibri";
                            rLabel1Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                            XSSFRow reportLabel1 = CreateRow(sheet, 2);
                            ICell rLabel1Cell = reportLabel1.CreateCell(0);
                            rLabel1Cell.CellStyle = workbook.CreateCellStyle();
                            rLabel1Cell.CellStyle.SetFont(rLabel1Font);
                            rLabel1Cell.CellStyle.WrapText = true;
                            //  hdrCaption.Height = 1200;

                            rLabel1Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                            rLabel1Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rLabel1Cell.SetCellValue($"Print Date : {DateTime.Now.ToString("dd-MM-yyyy")}");
                            sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 33));

                            XSSFRow reportLabel2 = CreateRow(sheet, 3);
                            ICell rLabel2Cell = reportLabel2.CreateCell(0);
                            rLabel2Cell.CellStyle = workbook.CreateCellStyle();
                            rLabel2Cell.CellStyle.SetFont(rLabel1Font);
                            rLabel2Cell.CellStyle.WrapText = true;

                            rLabel2Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                            rLabel2Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                            rLabel2Cell.SetCellValue($"Branch : {dsSource.Tables[0].Rows[0][1].ToString()}");

                            sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 33));

                            #endregion

                            SetBordersToMergedCells(workbook, sheet);

                            XSSFRow dataRow;
                            XSSFRow headerRow = CreateRow(sheet, 4);
                            XSSFRow serialNoRow = CreateRow(sheet, 5);

                            XSSFFont hFont = CreateFont(workbook);
                            hFont.FontHeightInPoints = 11;
                            hFont.FontName = "Calibri";
                            hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                            //int j = 1;
                            foreach (DataColumn column in table.Columns)
                            {
                                int rowIndex = 6;
                                headerRow.Height = 500;
                                if (column.ColumnName.ToLower() != "employeeid"
                                    && column.ColumnName.ToLower() != "employeecode"
                                    && column.ColumnName.ToLower() != "employeename")
                                {
                                    XSSFCell cell = (XSSFCell)headerRow.CreateCell(column.Ordinal);
                                    cell.SetCellValue(column.ColumnName);
                                    cell.CellStyle = workbook.CreateCellStyle();
                                    cell.CellStyle.Alignment = HorizontalAlignment.Center;
                                    cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;


                                    cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                                    cell.CellStyle.FillPattern = FillPattern.SolidForeground;

                                    cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                    cell.CellStyle.BorderTop = BorderStyle.Thin;
                                    cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                    cell.CellStyle.BorderRight = BorderStyle.Thin;

                                    cell.CellStyle.SetFont(hFont);
                                    cell.CellStyle.WrapText = true;
                                    var colType = column.DataType.Name;

                                    ICellStyle cellStyleDouble = workbook.CreateCellStyle();
                                    cellStyleDouble.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                                    cellStyleDouble.BorderBottom = BorderStyle.Thin;

                                    ICellStyle totalCellStyleDoubleColor = workbook.CreateCellStyle();
                                    totalCellStyleDoubleColor.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                                    totalCellStyleDoubleColor.SetFont(hFont);

                                    totalCellStyleDoubleColor.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index;
                                    totalCellStyleDoubleColor.FillPattern = FillPattern.SolidForeground;

                                    totalCellStyleDoubleColor.BorderBottom = BorderStyle.Thin;

                                    ICellStyle totalCellStyleColor = workbook.CreateCellStyle();
                                    totalCellStyleColor.SetFont(hFont);

                                    totalCellStyleColor.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index;
                                    totalCellStyleColor.FillPattern = FillPattern.SolidForeground;
                                    totalCellStyleColor.BorderBottom = BorderStyle.Thin;

                                    ICellStyle employeeDetails = workbook.CreateCellStyle();
                                    employeeDetails.BorderBottom = BorderStyle.Thin;
                                    employeeDetails.SetFont(hFont);

                                    #region ====== Serial Number row ===================

                                    //if (column.ColumnName.ToLower() != "sno")
                                    //{
                                    XSSFCell sRcell = (XSSFCell)serialNoRow.CreateCell(column.Ordinal);
                                    sRcell.SetCellValue(column.Ordinal + 1);
                                    sRcell.CellStyle = workbook.CreateCellStyle();
                                    sRcell.CellStyle.Alignment = HorizontalAlignment.Center;
                                    sRcell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                    sRcell.CellStyle.BorderBottom = BorderStyle.Thin;
                                    sRcell.CellStyle.BorderTop = BorderStyle.Thin;
                                    sRcell.CellStyle.BorderLeft = BorderStyle.Thin;
                                    sRcell.CellStyle.BorderRight = BorderStyle.Thin;

                                    //  }
                                    #endregion


                                    var oldEmployee = "";
                                    decimal totalDecimal = 0;
                                    for (int i = 0; i < table.Rows.Count; i++)
                                    {
                                        if (!column.ToString().ToLower().Trim().Equals("employeeid") && !column.ToString().ToLower().Trim().Equals("employeecode") && !column.ToString().ToLower().Trim().Equals("employeename"))
                                        {
                                            if (oldEmployee == table.Rows[i]["EmployeeID"].ToString())
                                            {
                                                if (column.Ordinal == 0)
                                                    dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                                else
                                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                                                if (colType == "String")
                                                {
                                                    var cell_0 = dataRow.CreateCell(column.Ordinal);
                                                    cell_0.CellStyle = workbook.CreateCellStyle();
                                                    cell_0.CellStyle.BorderBottom = BorderStyle.Thin;
                                                    cell_0.SetCellValue(table.Rows[i][column].ToString());
                                                }

                                                else if (colType == "Int32" || colType == "Single")
                                                {
                                                    //var cell_0 = dataRow.CreateCell(column.Ordinal);
                                                    //cell_0.CellStyle = workbook.CreateCellStyle();
                                                    //cell_0.CellStyle.BorderBottom = BorderStyle.Thin;
                                                    //cell_0.SetCellValue(table.Rows[i][column].ToString());

                                                    dataRow.CreateCell(column.Ordinal).SetCellValue(Convert.ToInt32(table.Rows[i][column]));
                                                }

                                                else if (colType == "Decimal")
                                                {
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = cellStyleDouble;
                                                    cell1.SetCellValue(Convert.ToDouble(table.Rows[i][column]));
                                                    totalDecimal = totalDecimal + Convert.ToDecimal(table.Rows[i][column]);
                                                }
                                                else
                                                {
                                                    var cell_0 = dataRow.CreateCell(column.Ordinal);
                                                    cell_0.CellStyle = workbook.CreateCellStyle();
                                                    cell_0.CellStyle.BorderBottom = BorderStyle.Thin;
                                                    cell_0.SetCellValue(table.Rows[i][column].ToString());

                                                }


                                                rowIndex++;

                                                if (rowIndex > 5 && (i == table.Rows.Count - 1) && column.Ordinal == 0)
                                                {
                                                    dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal);
                                                    cell1.CellStyle = totalCellStyleColor;
                                                    cell1.SetCellValue("Total :");
                                                    rowIndex++;
                                                }
                                                else if (rowIndex > 5 && (i == table.Rows.Count - 1) && (column.ToString() == "Branch" || column.ToString() == "Designation"))
                                                {
                                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal);
                                                    cell1.CellStyle = totalCellStyleColor;
                                                    cell1.SetCellValue("");
                                                    rowIndex++;
                                                }
                                                else if (rowIndex > 5 && (i == table.Rows.Count - 1) && (column.ToString() != "Branch" || column.ToString() != "Designation"))
                                                {
                                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = totalCellStyleDoubleColor;
                                                    cell1.SetCellValue(Convert.ToDouble(totalDecimal));
                                                    totalDecimal = 0;
                                                    rowIndex++;
                                                }
                                            }
                                            else if (column.Ordinal == 0)
                                            {
                                                if (rowIndex > 6)
                                                {
                                                    dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal);
                                                    cell1.CellStyle = totalCellStyleColor;
                                                    cell1.SetCellValue("Total :");
                                                    rowIndex++;
                                                }
                                                oldEmployee = table.Rows[i]["EmployeeID"].ToString();

                                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                                //dataRow.CreateCell(column.Ordinal).SetCellValue("EMPLOYEE CODE :");

                                                ICell cell2 = dataRow.CreateCell(column.Ordinal);
                                                cell2.CellStyle = employeeDetails;
                                                cell2.SetCellValue("EMPLOYEE CODE :");


                                                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, column.Ordinal + 1, 33));

                                                ICell cell_EmpCode = dataRow.CreateCell(column.Ordinal + 1);
                                                cell_EmpCode.CellStyle = workbook.CreateCellStyle();
                                                cell_EmpCode.CellStyle.BorderBottom = BorderStyle.Thin;
                                                cell_EmpCode.SetCellValue(table.Rows[i]["EmployeeCode"].ToString());

                                                rowIndex++;
                                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);

                                                ICell cell3 = dataRow.CreateCell(column.Ordinal);
                                                cell3.CellStyle = employeeDetails;
                                                cell3.SetCellValue("NAME OF EMPLOYEE :");

                                                //dataRow.CreateCell(column.Ordinal).SetCellValue("NAME OF EMPLOYEE :");

                                                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, column.Ordinal + 1, 33));

                                                CellRangeAddress cellAddress = new CellRangeAddress(rowIndex, rowIndex, column.Ordinal + 1, 33);


                                                var cell_EmpName = dataRow.CreateCell(column.Ordinal + 1);
                                                cell_EmpName.CellStyle = workbook.CreateCellStyle();
                                                cell_EmpName.CellStyle.BorderBottom = BorderStyle.Thin;
                                                cell_EmpName.SetCellValue(table.Rows[i]["EmployeeName"].ToString());

                                                rowIndex++;
                                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);

                                                if (colType == "String")
                                                {
                                                    var cell_0 = dataRow.CreateCell(column.Ordinal);
                                                    cell_0.CellStyle = workbook.CreateCellStyle();
                                                    cell_0.CellStyle.BorderBottom = BorderStyle.Thin;
                                                    cell_0.SetCellValue(table.Rows[i][column].ToString());
                                                }

                                                else if (colType == "Int32" || colType == "Single")
                                                    dataRow.CreateCell(column.Ordinal).SetCellValue(Convert.ToInt32(table.Rows[i][column]));
                                                else if (colType == "Decimal")
                                                {
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = cellStyleDouble;
                                                    cell1.SetCellValue(Convert.ToDouble(table.Rows[i][column]));
                                                    totalDecimal = totalDecimal + Convert.ToDecimal(table.Rows[i][column]);
                                                }
                                                else
                                                    dataRow.CreateCell(column.Ordinal).SetCellValue(table.Rows[i][column].ToString());

                                                //dataRow.CreateCell(column.Ordinal).SetCellValue(table.Rows[i][column].ToString());
                                                rowIndex++;
                                                if (i == (table.Rows.Count - 1) && rowIndex > 5)
                                                {
                                                    dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal);
                                                    cell1.CellStyle = totalCellStyleColor;
                                                    cell1.SetCellValue("Total :");
                                                }
                                            }
                                            else
                                            {
                                                if (rowIndex > 6 && (column.ToString() == "Branch" || column.ToString() == "Designation"))
                                                {
                                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal);
                                                    cell1.CellStyle = totalCellStyleColor;
                                                    cell1.CellStyle.BorderTop = BorderStyle.Thin;
                                                    cell1.SetCellValue("");
                                                    rowIndex++;
                                                }
                                                else if (rowIndex > 6 && (column.ToString() != "Branch" || column.ToString() != "Designation"))
                                                {
                                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = totalCellStyleDoubleColor;
                                                    cell1.SetCellValue(Convert.ToDouble(totalDecimal));
                                                    totalDecimal = 0;
                                                    rowIndex++;
                                                }
                                                oldEmployee = table.Rows[i]["EmployeeID"].ToString();
                                                rowIndex++;
                                                rowIndex++;
                                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                                                if (colType == "String")
                                                {
                                                    var cell_0 = dataRow.CreateCell(column.Ordinal);
                                                    cell_0.CellStyle.BorderBottom = BorderStyle.Thin;
                                                    cell_0.CellStyle.BorderTop = BorderStyle.Thin;
                                                    cell_0.SetCellValue(table.Rows[i][column].ToString());
                                                }

                                                else if (colType == "Int32" || colType == "Single")
                                                    dataRow.CreateCell(column.Ordinal).SetCellValue(Convert.ToInt32(table.Rows[i][column]));

                                                else if (colType == "Decimal")
                                                {
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = cellStyleDouble;
                                                    cell1.SetCellValue(Convert.ToDouble(table.Rows[i][column]));
                                                    totalDecimal = totalDecimal + Convert.ToDecimal(table.Rows[i][column]);
                                                }
                                                else
                                                {
                                                    var cell_00 = dataRow.CreateCell(column.Ordinal);
                                                    cell_00.CellStyle = workbook.CreateCellStyle();
                                                    cell_00.CellStyle.BorderBottom = BorderStyle.Thin;
                                                    cell_00.CellStyle.BorderTop = BorderStyle.Thin;
                                                    cell_00.SetCellValue(table.Rows[i][column].ToString());

                                                }
                                                rowIndex++;

                                                if (i == (table.Rows.Count - 1) && rowIndex > 5 && (column.ToString() == "Branch" || column.ToString() == "Designation"))
                                                {
                                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal);
                                                    cell1.CellStyle = totalCellStyleColor;
                                                    cell1.SetCellValue("");
                                                    //rowIndex++;
                                                }
                                                else if (i == (table.Rows.Count - 1) && rowIndex > 6 && (column.ToString() != "Branch" || column.ToString() != "Designation"))
                                                {
                                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = totalCellStyleDoubleColor;
                                                    cell1.SetCellValue(Convert.ToDouble(totalDecimal));

                                                    workbook.CreateCellStyle();
                                                    sRcell.CellStyle.Alignment = HorizontalAlignment.Center;
                                                    sRcell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                                    sRcell.CellStyle.BorderBottom = BorderStyle.Thin;

                                                    totalDecimal = 0;
                                                }
                                            }
                                        }
                                    }
                                    sheet.AutoSizeColumn(column.Ordinal);
                                }
                            }
                            // SetBordersToMergedCells(workbook, sheet);
                        }

                    }

                    workbook.Write(file);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return false;
            }
        }

        public static bool ExportSalaryReportBranchWiseAnnual(DataSet dsSource, string sFullPath, int earningCols, int deductionCols, string sheetName)
        {
            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    foreach (DataTable table in dsSource.Tables)
                    {
                        if (table.TableName == "Table")
                        {
                            XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sheetName);
                            XSSFFont topHdrFont = CreateFont(workbook);

                            topHdrFont.FontHeightInPoints = 18;
                            topHdrFont.FontName = "Calibri";
                            topHdrFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                            #region  ----- Top Header -=============

                            XSSFFont titleFont = CreateFont(workbook);
                            titleFont.FontHeightInPoints = 14;
                            titleFont.FontName = "Calibri";
                            titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                            XSSFRow titlerow = CreateRow(sheet, 0);
                            ICell titlecell = titlerow.CreateCell(0);

                            titlecell.CellStyle = workbook.CreateCellStyle();
                            titlecell.CellStyle.SetFont(titleFont);

                            titlecell.CellStyle.WrapText = true;
                            titlerow.Height = 1200;

                            titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                            titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");

                            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, (earningCols + deductionCols + 3)));

                            #endregion

                            #region ----====== Report Caption ===================

                            XSSFFont hdrCapFont = CreateFont(workbook);
                            hdrCapFont.FontHeightInPoints = 12;
                            hdrCapFont.FontName = "Calibri";
                            hdrCapFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                            XSSFRow hdrCaption = CreateRow(sheet, 1);
                            ICell hdrCapCell = hdrCaption.CreateCell(0);
                            hdrCapCell.CellStyle = workbook.CreateCellStyle();
                            hdrCapCell.CellStyle.SetFont(hdrCapFont);
                            hdrCapCell.CellStyle.WrapText = true;

                            //  hdrCaption.Height = 1200;
                            hdrCapCell.CellStyle.Alignment = HorizontalAlignment.Center;
                            hdrCapCell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            hdrCapCell.SetCellValue($"Branch Wise Annual Report");

                            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, (earningCols + deductionCols + 3)));

                            #endregion

                            #region   ======== ////==== Report Header Label Value  ==============

                            XSSFFont rLabel1Font = CreateFont(workbook);
                            rLabel1Font.FontHeightInPoints = 11;
                            rLabel1Font.FontName = "Calibri";
                            rLabel1Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                            XSSFRow reportLabel1 = CreateRow(sheet, 2);
                            ICell rLabel1Cell = reportLabel1.CreateCell(0);
                            rLabel1Cell.CellStyle = workbook.CreateCellStyle();
                            rLabel1Cell.CellStyle.SetFont(rLabel1Font);
                            rLabel1Cell.CellStyle.WrapText = true;
                            //  hdrCaption.Height = 1200;

                            rLabel1Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                            rLabel1Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rLabel1Cell.SetCellValue($"Print Date : {DateTime.Now.ToString("dd-MM-yyyy")}");
                            sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, (earningCols + deductionCols + 3)));

                            XSSFRow reportLabel2 = CreateRow(sheet, 3);
                            ICell rLabel2Cell = reportLabel2.CreateCell(0);
                            rLabel2Cell.CellStyle = workbook.CreateCellStyle();
                            rLabel2Cell.CellStyle.SetFont(rLabel1Font);
                            rLabel2Cell.CellStyle.WrapText = true;

                            rLabel2Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                            rLabel2Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                            //  rLabel2Cell.SetCellValue($"Branch : {dsSource.Tables[0].Rows[0][1].ToString()}");

                            sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, (earningCols + deductionCols + 3)));

                            #endregion

                            SetBordersToMergedCells(workbook, sheet);


                            XSSFRow dataRow;
                            XSSFRow headerRow = CreateRow(sheet, 4);
                            XSSFRow serialNoRow = CreateRow(sheet, 5);

                            XSSFFont hFont = CreateFont(workbook);
                            hFont.FontHeightInPoints = 11;
                            hFont.FontName = "Calibri";
                            hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                            foreach (DataColumn column in table.Columns)
                            {
                                int rowIndex = 6;
                                headerRow.Height = 500;
                                if (column.ColumnName.ToLower() != "branchcode" && column.ColumnName.ToLower() != "branch")
                                {
                                    XSSFCell cell = (XSSFCell)headerRow.CreateCell(column.Ordinal);
                                    cell.SetCellValue(column.ColumnName);
                                    cell.CellStyle = workbook.CreateCellStyle();
                                    cell.CellStyle.Alignment = HorizontalAlignment.Center;
                                    cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                    cell.CellStyle.BorderBottom = BorderStyle.Thin;

                                    cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                                    cell.CellStyle.FillPattern = FillPattern.SolidForeground;

                                    //if (column.Ordinal < 1)
                                    //{
                                    //    cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                                    //    cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                                    //}
                                    //else if (column.Ordinal > 0 && column.Ordinal <= (earningCols + 1))
                                    //{
                                    //    cell.CellStyle.FillForegroundColor = IndexedColors.LightBlue.Index;
                                    //    cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                                    //}
                                    //else if (column.Ordinal > (earningCols + 1) && column.Ordinal <= (deductionCols + earningCols + 2))
                                    //{
                                    //    cell.CellStyle.FillForegroundColor = IndexedColors.DarkYellow.Index;
                                    //    cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                                    //}
                                    //else if (column.ColumnName.ToLower().Equals("net salary"))
                                    //{
                                    //    cell.CellStyle.FillForegroundColor = IndexedColors.Green.Index;
                                    //    cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                                    //}


                                    cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                    cell.CellStyle.BorderTop = BorderStyle.Thin;
                                    cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                    cell.CellStyle.BorderRight = BorderStyle.Thin;


                                    cell.CellStyle.SetFont(hFont);
                                    cell.CellStyle.WrapText = true;
                                    var colType = column.DataType.Name;
                                    ICellStyle cellStyleDouble = workbook.CreateCellStyle();

                                    cellStyleDouble.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

                                    ICellStyle totalCellStyleDoubleColor = workbook.CreateCellStyle();
                                    totalCellStyleDoubleColor.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                                    totalCellStyleDoubleColor.SetFont(hFont);
                                    totalCellStyleDoubleColor.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index;
                                    totalCellStyleDoubleColor.FillPattern = FillPattern.SolidForeground;
                                    totalCellStyleDoubleColor.BorderBottom = BorderStyle.Thin;

                                    ICellStyle totalCellStyleColor = workbook.CreateCellStyle();
                                    totalCellStyleColor.SetFont(hFont);
                                    totalCellStyleColor.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index;
                                    totalCellStyleColor.FillPattern = FillPattern.SolidForeground;
                                    totalCellStyleColor.BorderBottom = BorderStyle.Thin;

                                    ICellStyle branchDetails = workbook.CreateCellStyle();

                                    branchDetails.BorderBottom = BorderStyle.Thin;
                                    branchDetails.SetFont(hFont);


                                    #region ====== Serial Number row ===================

                                    //if (column.ColumnName.ToLower() != "month")
                                    //{
                                    XSSFCell sRcell = (XSSFCell)serialNoRow.CreateCell(column.Ordinal);
                                    sRcell.SetCellValue(column.Ordinal + 1);
                                    sRcell.CellStyle = workbook.CreateCellStyle();
                                    sRcell.CellStyle.Alignment = HorizontalAlignment.Center;
                                    sRcell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                    sRcell.CellStyle.BorderBottom = BorderStyle.Thin;
                                    sRcell.CellStyle.BorderTop = BorderStyle.Thin;
                                    sRcell.CellStyle.BorderLeft = BorderStyle.Thin;
                                    sRcell.CellStyle.BorderRight = BorderStyle.Thin;
                                    //  }
                                    #endregion

                                    var oldBranch = "";
                                    decimal totalDecimal = 0;
                                    for (int i = 0; i < table.Rows.Count; i++)
                                    {
                                        if (!column.ToString().ToLower().Trim().Equals("branchcode") && !column.ToString().ToLower().Trim().Equals("branch"))
                                        {
                                            if (oldBranch == table.Rows[i]["BranchCode"].ToString())
                                            {
                                                if (column.Ordinal == 0)
                                                    dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                                else
                                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                                                if (colType == "String")
                                                {
                                                    var cell_0 = dataRow.CreateCell(column.Ordinal);
                                                    cell_0.CellStyle = workbook.CreateCellStyle();
                                                    cell_0.CellStyle.BorderBottom = BorderStyle.Thin;
                                                    cell_0.SetCellValue(table.Rows[i][column].ToString());
                                                    //  dataRow.CreateCell(column.Ordinal).SetCellValue(table.Rows[i][column].ToString());
                                                }
                                                else if (colType == "Int32" || colType == "Single")
                                                    dataRow.CreateCell(column.Ordinal).SetCellValue(Convert.ToInt32(table.Rows[i][column]));
                                                else if (colType == "Decimal")
                                                {
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = cellStyleDouble;
                                                    cell1.SetCellValue(Convert.ToDouble(table.Rows[i][column]));
                                                    totalDecimal = totalDecimal + Convert.ToDecimal(table.Rows[i][column]);
                                                }
                                                else
                                                {
                                                    ICell cell_1 = dataRow.CreateCell(column.Ordinal);
                                                    cell_1.SetCellValue(table.Rows[i][column].ToString());
                                                    cell_1.CellStyle = workbook.CreateCellStyle();
                                                    cell_1.CellStyle.BorderBottom = BorderStyle.Thin;

                                                }

                                                rowIndex++;

                                                if (rowIndex > 5 && (i == table.Rows.Count - 1) && column.Ordinal == 0)
                                                {
                                                    dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal);
                                                    cell1.CellStyle = totalCellStyleColor;
                                                    cell1.SetCellValue("Total :");
                                                    rowIndex++;
                                                }
                                                else if (rowIndex > 5 && (i == table.Rows.Count - 1) && (column.ToString() != "Branch" || column.ToString() != "BranchCode"))
                                                {
                                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = totalCellStyleDoubleColor;
                                                    cell1.SetCellValue(Convert.ToDouble(totalDecimal));
                                                    totalDecimal = 0;
                                                    rowIndex++;
                                                }
                                            }
                                            else if (column.Ordinal == 0)
                                            {
                                                if (rowIndex > 6)
                                                {
                                                    dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal);
                                                    cell1.CellStyle = totalCellStyleColor;
                                                    cell1.SetCellValue("Total :");
                                                    rowIndex++;
                                                }
                                                oldBranch = table.Rows[i]["BranchCode"].ToString();

                                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                                ICell cell2 = dataRow.CreateCell(column.Ordinal);
                                                cell2.CellStyle = branchDetails;
                                                cell2.SetCellValue("BRANCH CODE :");

                                                //dataRow.CreateCell(column.Ordinal).SetCellValue("BRANCH CODE :");
                                                dataRow.CreateCell((column.Ordinal) + 1).SetCellValue(table.Rows[i]["BranchCode"].ToString());
                                                rowIndex++;

                                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                                ICell cell3 = dataRow.CreateCell(column.Ordinal);
                                                cell3.CellStyle = branchDetails;
                                                cell3.SetCellValue("NAME OF BRANCH :");

                                                //dataRow.CreateCell(column.Ordinal).SetCellValue("NAME OF BRANCH :");
                                                dataRow.CreateCell((column.Ordinal) + 1).SetCellValue(table.Rows[i]["Branch"].ToString());
                                                rowIndex++;
                                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);

                                                if (colType == "String")
                                                {
                                                    //  dataRow.CreateCell(column.Ordinal).SetCellValue(table.Rows[i][column].ToString());
                                                    var cell_0 = dataRow.CreateCell(column.Ordinal);
                                                    cell_0.CellStyle = workbook.CreateCellStyle();
                                                    cell_0.CellStyle.BorderBottom = BorderStyle.Thin;
                                                    cell_0.SetCellValue(table.Rows[i][column].ToString());
                                                }

                                                else if (colType == "Int32" || colType == "Single")
                                                    dataRow.CreateCell(column.Ordinal).SetCellValue(Convert.ToInt32(table.Rows[i][column]));
                                                else if (colType == "Decimal")
                                                {
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = cellStyleDouble;
                                                    cell1.SetCellValue(Convert.ToDouble(table.Rows[i][column]));
                                                    totalDecimal = totalDecimal + Convert.ToDecimal(table.Rows[i][column]);
                                                }
                                                else
                                                    dataRow.CreateCell(column.Ordinal).SetCellValue(table.Rows[i][column].ToString());

                                                rowIndex++;
                                                if (i == (table.Rows.Count - 1) && rowIndex > 5)
                                                {
                                                    dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal);
                                                    cell1.CellStyle = totalCellStyleColor;
                                                    cell1.SetCellValue("Total :");
                                                }
                                            }
                                            else
                                            {
                                                if (rowIndex > 6 && (column.ToString() != "Branch" || column.ToString() != "BranchCode"))
                                                {
                                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = totalCellStyleDoubleColor;
                                                    cell1.SetCellValue(Convert.ToDouble(totalDecimal));
                                                    totalDecimal = 0;
                                                    rowIndex++;
                                                }

                                                oldBranch = table.Rows[i]["BranchCode"].ToString();

                                                rowIndex++;
                                                rowIndex++;

                                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                                                if (colType == "String")
                                                    dataRow.CreateCell(column.Ordinal).SetCellValue(table.Rows[i][column].ToString());
                                                else if (colType == "Int32" || colType == "Single")
                                                    dataRow.CreateCell(column.Ordinal).SetCellValue(Convert.ToInt32(table.Rows[i][column]));
                                                else if (colType == "Decimal")
                                                {
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = cellStyleDouble;
                                                    cell1.SetCellValue(Convert.ToDouble(table.Rows[i][column]));
                                                    totalDecimal = totalDecimal + Convert.ToDecimal(table.Rows[i][column]);
                                                }
                                                else
                                                    dataRow.CreateCell(column.Ordinal).SetCellValue(table.Rows[i][column].ToString());

                                                rowIndex++;

                                                if (i == (table.Rows.Count - 1) && rowIndex > 6 && (column.ToString() != "Branch" || column.ToString() != "BranchCode"))
                                                {
                                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                                                    ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                                    cell1.CellStyle = totalCellStyleDoubleColor;
                                                    cell1.SetCellValue(Convert.ToDouble(totalDecimal));
                                                    totalDecimal = 0;
                                                }
                                            }
                                        }
                                    }
                                    sheet.AutoSizeColumn(column.Ordinal);
                                }
                            }
                        }
                    }
                    workbook.Write(file);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return false;
            }
        }

        public static void SetBordersToMergedCells(XSSFWorkbook workBook, XSSFSheet sheet)
        {
            for (int i = 0; i < sheet.NumMergedRegions; i++)
            {
                CellRangeAddress mergedRegions = sheet.GetMergedRegion(i);
                RegionUtil.SetBorderTop(1, mergedRegions, sheet, workBook);
                RegionUtil.SetBorderTop(1, mergedRegions, sheet, workBook);
                RegionUtil.SetBorderLeft(1, mergedRegions, sheet, workBook);
                RegionUtil.SetBorderRight(1, mergedRegions, sheet, workBook);
                RegionUtil.SetBorderBottom(1, mergedRegions, sheet, workBook);
            }
        }

        public static string MonthlyEmployeeWisePaySlip(string branchName, int salYear, int salMonth,
            IEnumerable<string> headers, DataTable rowData, int earningCols, int deductionCols,
            string sSheetName, string sFullPath)
        {
            log.Info($"SalaryReportExport/MonthlyEmployeeWisePaySlip/branchName:{branchName}&salYear:{salYear}&salMonth:{salMonth}");

            try
            {
                var rowCount = rowData.Rows.Count;
                rowData.Rows[rowCount - 1][1] = "Total";

                int columnCount = headers.ToList().Count;
                XSSFWorkbook workbook = CreateWookBook();

                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);

                    XSSFFont topHdrFont = CreateFont(workbook);

                    topHdrFont.FontHeightInPoints = 18;
                    topHdrFont.FontName = "Calibri";
                    topHdrFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    #region  ----- Top Header -=============

                    XSSFFont titleFont = CreateFont(workbook);
                    titleFont.FontHeightInPoints = 14;
                    titleFont.FontName = "Calibri";
                    titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(titleFont);
                    titlecell.CellStyle.WrapText = true;
                    titlerow.Height = 1200;
                    titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                    titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 34));

                    #endregion

                    #region ----== Report Caption ===================

                    XSSFFont hdrCapFont = CreateFont(workbook);
                    hdrCapFont.FontHeightInPoints = 12;
                    hdrCapFont.FontName = "Calibri";
                    hdrCapFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow hdrCaption = CreateRow(sheet, 1);
                    ICell hdrCapCell = hdrCaption.CreateCell(0);
                    hdrCapCell.CellStyle = workbook.CreateCellStyle();
                    hdrCapCell.CellStyle.SetFont(hdrCapFont);
                    hdrCapCell.CellStyle.WrapText = true;

                    //  hdrCaption.Height = 1200;
                    hdrCapCell.CellStyle.Alignment = HorizontalAlignment.Center;
                    hdrCapCell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    hdrCapCell.SetCellValue($"Monthly Payslip for the month {new DateTime(salYear, salMonth, 1).ToString("MMM")} ,{salYear}");

                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 34));

                    #endregion

                    #region   ======== ////==== Report Header Label Value  ==============

                    XSSFFont rLabel1Font = CreateFont(workbook);
                    rLabel1Font.FontHeightInPoints = 11;
                    rLabel1Font.FontName = "Calibri";
                    rLabel1Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow reportLabel1 = CreateRow(sheet, 2);
                    ICell rLabel1Cell = reportLabel1.CreateCell(0);
                    rLabel1Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel1Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel1Cell.CellStyle.WrapText = true;
                    //  hdrCaption.Height = 1200;

                    rLabel1Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel1Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    rLabel1Cell.SetCellValue($"Print Date : {DateTime.Now.ToString("dd-MM-yyyy")}");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 34));

                    XSSFRow reportLabel2 = CreateRow(sheet, 3);
                    ICell rLabel2Cell = reportLabel2.CreateCell(0);
                    rLabel2Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel2Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel2Cell.CellStyle.WrapText = true;

                    //  hdrCaption.Height = 1200;
                    rLabel2Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel2Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    rLabel2Cell.SetCellValue($"Branch : {branchName}");
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 34));

                    //XSSFRow rHeader1 = CreateRow(sheet, 2);
                    //var cellRH1_0 = rHeader1.CreateCell(0);
                    //cellRH1_0.SetCellValue("Period");

                    //var cellRH1_1 = rHeader1.CreateCell(1);
                    //cellRH1_1.SetCellValue($"{new DateTime(salYear, salMonth, 1).ToString("MMM")} ,{salYear}");

                    //XSSFRow rHeader2 = CreateRow(sheet, 3);
                    //var cellRH2_0 = rHeader2.CreateCell(0);
                    //cellRH2_0.SetCellValue("Branch :");

                    //var cellRH2_1 = rHeader2.CreateCell(1);
                    //cellRH2_1.SetCellValue(branchName);

                    #endregion

                    XSSFRow dataRow;
                    XSSFRow headerRow = CreateRow(sheet, 5);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);
                    XSSFRow tbCaption = CreateRow(sheet, 4);

                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 4, (4 + (earningCols - 1))));

                    // sheet.AddMergedRegion(new CellRangeAddress(2, 2, 3 + earningCols, 3 + earningCols));

                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 4 + earningCols + 1, 4 + earningCols + deductionCols));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 4 + earningCols + deductionCols + 1, 4 + earningCols + deductionCols + 2));

                    SetBordersToMergedCells(workbook, sheet);

                    var firstCellOfFourthRow = tbCaption.CreateCell(0);
                    firstCellOfFourthRow.CellStyle = workbook.CreateCellStyle();
                    firstCellOfFourthRow.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    firstCellOfFourthRow.CellStyle.FillPattern = FillPattern.SolidForeground;

                    var lastCellOfFouthRow = tbCaption.CreateCell(4 + earningCols + deductionCols + 1);
                    lastCellOfFouthRow.CellStyle = workbook.CreateCellStyle();
                    lastCellOfFouthRow.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    lastCellOfFouthRow.CellStyle.FillPattern = FillPattern.SolidForeground;

                    var earingHeaderCells = tbCaption.CreateCell(4);
                    earingHeaderCells.SetCellValue("TOTAL EARNINGS");

                    earingHeaderCells.CellStyle = workbook.CreateCellStyle();
                    earingHeaderCells.CellStyle.SetFont(hFont);
                    earingHeaderCells.CellStyle.Alignment = HorizontalAlignment.Center;

                    earingHeaderCells.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    earingHeaderCells.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;

                    earingHeaderCells.CellStyle.FillPattern = FillPattern.SolidForeground;

                    earingHeaderCells.CellStyle.BorderBottom = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderTop = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderLeft = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderRight = BorderStyle.Thin;

                    var deductionHeaderCells = tbCaption.CreateCell(4 + earningCols + 1);
                    deductionHeaderCells.SetCellValue("TOTAL DEDUCTIONS");

                    deductionHeaderCells.CellStyle = workbook.CreateCellStyle();
                    deductionHeaderCells.CellStyle.SetFont(hFont);
                    deductionHeaderCells.CellStyle.Alignment = HorizontalAlignment.Center;
                    deductionHeaderCells.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                    deductionHeaderCells.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    deductionHeaderCells.CellStyle.FillPattern = FillPattern.SolidForeground;
                    deductionHeaderCells.CellStyle.BorderBottom = BorderStyle.Thin;

                    deductionHeaderCells.CellStyle.BorderTop = BorderStyle.Thin;
                    deductionHeaderCells.CellStyle.BorderLeft = BorderStyle.Thin;
                    deductionHeaderCells.CellStyle.BorderRight = BorderStyle.Thin;

                    var centerBlackCell = tbCaption.CreateCell(4 + earningCols);
                    centerBlackCell.CellStyle = workbook.CreateCellStyle();
                    centerBlackCell.CellStyle.SetFont(hFont);

                    centerBlackCell.CellStyle.BorderBottom = BorderStyle.Thin;
                    centerBlackCell.CellStyle.BorderTop = BorderStyle.Thin;

                    centerBlackCell.CellStyle.BorderLeft = BorderStyle.Thin;
                    centerBlackCell.CellStyle.BorderRight = BorderStyle.Thin;

                    centerBlackCell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    centerBlackCell.CellStyle.FillPattern = FillPattern.SolidForeground;

                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.CellStyle = workbook.CreateCellStyle();

                        //  sheet.SetColumnWidth(hdrColIndex, 15);
                        cell.CellStyle.WrapText = true;

                        if (hdr.Equals("BranchName", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Branch Name");

                        //else if (hdr.Equals("SNo", StringComparison.OrdinalIgnoreCase))
                        //    cell.SetCellValue("S.No.");
                        else if (hdr.Equals("EmployeeCode", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Employee Code");

                        else if (hdr.Equals("Name", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Employee Name");
                        else
                            cell.SetCellValue(hdr);

                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.BorderTop = BorderStyle.Thin;

                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;

                        cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                        cell.CellStyle.FillPattern = FillPattern.SolidForeground;

                        headerRow.Height = 900;
                        if (hdr == "Name")
                            sheet.SetColumnWidth(hdrColIndex, 9000);

                        else if (hdr == "BranchName")
                            sheet.SetColumnWidth(hdrColIndex, 6000);

                        else if (hdr == "EmployeeCode")
                            sheet.SetColumnWidth(hdrColIndex, 2500);

                        else if (hdrColIndex > 3)
                            sheet.SetColumnWidth(hdrColIndex, 3500);
                        // sheet.AutoSizeColumn(hdrColIndex);

                        hdrColIndex++;
                    }

                    var execlRow = sheet.GetRow(5);
                    ICell totalE_Cell = execlRow.GetCell(5 + earningCols - 1);
                    ICell totalD_Cell = execlRow.GetCell(5 + earningCols + deductionCols);

                    totalE_Cell.SetCellValue("TOTAL");
                    totalD_Cell.SetCellValue("TOTAL");

                    for (int jj = 0; jj < columnCount; jj++)
                    {
                        int rowIndex = 6;

                        if (jj == 0)
                            dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                        else
                            dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                        var cell = dataRow.CreateCell(jj);

                        if (jj > 0)
                        {
                            int numVal;
                            if (int.TryParse(jj.ToString(), out numVal))
                                cell.SetCellValue(numVal);
                            else
                                cell.SetCellValue(jj);
                        }
                        else
                            cell.SetCellValue("");

                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;

                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;
                        dataRow.Height = 300;
                    }
                    foreach (DataColumn column in rowData.Columns)
                    {
                        int rowIndex = 7;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            if (!row[1].ToString().Trim().Equals("Total"))
                            {
                                if (!row[column].ToString().Trim().Equals(""))
                                {
                                    ICell Cell = dataRow.CreateCell(column.Ordinal);
                                    Cell.CellStyle = workbook.CreateCellStyle();

                                    Cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                    //Cell.CellStyle.BorderTop = BorderStyle.Thin;
                                    //Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                    //Cell.CellStyle.BorderRight = BorderStyle.Thin;

                                    if (column.Ordinal == 0 || column.Ordinal == 2)
                                        Cell.CellStyle.Alignment = HorizontalAlignment.Center;

                                    if (column.Ordinal > 3)
                                    {
                                        Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                        Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

                                        //  setDataFormat(HSSFDataFormat.getBuiltinFormat("#,##0.00"));

                                        if (column.Ordinal == earningCols + 4)  ///=== Add Sum Formula for total of earning heads ===
                                        {
                                            var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(4));
                                            var fCellRefAsString = fromCellRef.FormatAsString();

                                            var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(column.Ordinal - 1));
                                            var toCellRefAsString = toCellRef.FormatAsString();

                                            hFont.FontHeightInPoints = 11;
                                            hFont.FontName = "Calibri";
                                            hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                            Cell.CellStyle.SetFont(hFont);
                                            Cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");

                                        }
                                        else if (column.Ordinal == 4 + earningCols + deductionCols + 1) ///=== Add Sum Formula for total of deduction heads ===
                                        {
                                            var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(4 + earningCols + 1));
                                            var fCellRefAsString = fromCellRef.FormatAsString();

                                            var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(column.Ordinal - 1));
                                            var toCellRefAsString = toCellRef.FormatAsString();

                                            hFont.FontHeightInPoints = 11;
                                            hFont.FontName = "Calibri";
                                            hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                            Cell.CellStyle.SetFont(hFont);
                                            Cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");
                                        }
                                        else if (column.Ordinal == rowData.Columns.Count - 1) ///=== Formula to get Net Salary 
                                        {
                                            var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(earningCols + 4));
                                            var fCellRefAsString = fromCellRef.FormatAsString();

                                            var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(4 + earningCols + deductionCols + 1));
                                            var toCellRefAsString = toCellRef.FormatAsString();

                                            hFont.FontHeightInPoints = 11;
                                            hFont.FontName = "Calibri";
                                            hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                            Cell.CellStyle.SetFont(hFont);
                                            Cell.SetCellFormula($"{fCellRefAsString}-{toCellRefAsString}");
                                        }
                                    }
                                    else
                                        Cell.SetCellValue(row[column].ToString());
                                }
                            }
                            else
                            {
                                if (column.Ordinal > 0)
                                {
                                    var cell = dataRow.CreateCell(column.Ordinal);

                                    if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(decimal)))
                                        cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                    else if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(int)))
                                        cell.SetCellValue((int)row[column]);
                                    else
                                    {
                                        int numVal;
                                        if (int.TryParse(row[column].ToString(), out numVal))
                                            cell.SetCellValue(numVal);
                                        else if (!row[column].ToString().Equals("total", StringComparison.OrdinalIgnoreCase))
                                            cell.SetCellValue(row[column].ToString());
                                    }

                                    cell.CellStyle = workbook.CreateCellStyle();
                                    cell.CellStyle.SetFont(hFont);

                                    if (column.Ordinal > 3)
                                    {
                                        #region  =========  Data Formating & Set Cell Formula ============

                                        var fromCellRef = new CellReference(sheet.GetRow(7).GetCell(column.Ordinal));
                                        var fCellRefAsString = fromCellRef.FormatAsString();

                                        var toRowNo = sheet.LastRowNum - 1;
                                        var toCellRef = new CellReference(sheet.GetRow(toRowNo).GetCell(column.Ordinal));
                                        var toCellRefAsString = toCellRef.FormatAsString();

                                        cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

                                        //  cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00");
                                        cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");

                                        #endregion
                                    }

                                    if (column.Ordinal != 1)
                                        cell.CellStyle.Alignment = HorizontalAlignment.Right;

                                    cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                    cell.CellStyle.BorderTop = BorderStyle.Thin;
                                    cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                    cell.CellStyle.BorderRight = BorderStyle.Thin;

                                    cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                                    cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                                }
                                else
                                {
                                    var cell = dataRow.CreateCell(column.Ordinal);
                                    cell.CellStyle = workbook.CreateCellStyle();
                                    cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                    cell.CellStyle.BorderTop = BorderStyle.Thin;
                                    cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                    cell.CellStyle.BorderRight = BorderStyle.Thin;
                                    cell.CellStyle.SetFont(hFont);
                                    cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                                    cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                                    cell.SetCellValue("Total");
                                }
                                dataRow.Height = 300;
                            }
                            rowIndex++;
                        }
                        // if(column.Ordinal==1 || column.Ordinal==3) sheet.AutoSizeColumn(column.Ordinal);
                    }

                    workbook.Write(file);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return "fail - " + ex.Message;
            }
        }

        public static string BranchEmployeeWisePaySlip(string branchName, int salYear, int salMonth,
            IEnumerable<string> header1, IEnumerable<string> header2, DataTable eHeadsDT,
            DataTable dHeadsDT, int earningCols, int deductionCols, string sheetName, string fullPath)
        {
            log.Info($"SalaryReportExport/BranchEmployeeWisePaySlip/branchName:{branchName}&salYear:{salYear}&salMonth:{salMonth}");

            try
            {
                int noOfCols_in_header1 = header1.ToList().Count;
                int noOfCols_in_header2 = header2.ToList().Count;

                var dHeadsDTStartIndex = 0;

                XSSFWorkbook workbook = CreateWookBook();

                using (var file = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sheetName);
                    XSSFFont topHdrFont = CreateFont(workbook);

                    topHdrFont.FontHeightInPoints = 18;
                    topHdrFont.FontName = "Calibri";
                    topHdrFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    #region  ---================ Top Header ========================

                    XSSFFont titleFont = CreateFont(workbook);
                    titleFont.FontHeightInPoints = 14;
                    titleFont.FontName = "Calibri";
                    titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(titleFont);
                    titlecell.CellStyle.WrapText = true;

                    titlerow.Height = 1200;
                    titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                    titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 23));

                    #endregion

                    #region ----============ Report Caption =====================

                    XSSFFont hdrCapFont = CreateFont(workbook);
                    hdrCapFont.FontHeightInPoints = 12;
                    hdrCapFont.FontName = "Calibri";
                    hdrCapFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow hdrCaption = CreateRow(sheet, 1);
                    ICell hdrCapCell = hdrCaption.CreateCell(0);
                    hdrCapCell.CellStyle = workbook.CreateCellStyle();
                    hdrCapCell.CellStyle.SetFont(hdrCapFont);
                    hdrCapCell.CellStyle.WrapText = true;

                    //  hdrCaption.Height = 1200;
                    hdrCapCell.CellStyle.Alignment = HorizontalAlignment.Center;
                    hdrCapCell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    hdrCapCell.SetCellValue($"Monthly Payslip for the month {new DateTime(salYear, salMonth, 1).ToString("MMM")} ,{salYear}");

                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 23));

                    #endregion

                    #region   ======== ////==== Report Header Label Value  ==============

                    XSSFFont rLabel1Font = CreateFont(workbook);
                    rLabel1Font.FontHeightInPoints = 11;
                    rLabel1Font.FontName = "Calibri";
                    rLabel1Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow reportLabel1 = CreateRow(sheet, 2);
                    ICell rLabel1Cell = reportLabel1.CreateCell(0);
                    rLabel1Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel1Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel1Cell.CellStyle.WrapText = true;
                    //  hdrCaption.Height = 1200;

                    rLabel1Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel1Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    rLabel1Cell.SetCellValue($"Print Date : {DateTime.Now.ToString("dd-MM-yyyy")}");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 23));

                    XSSFRow reportLabel2 = CreateRow(sheet, 3);
                    ICell rLabel2Cell = reportLabel2.CreateCell(0);
                    rLabel2Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel2Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel2Cell.CellStyle.WrapText = true;

                    //  hdrCaption.Height = 1200;
                    rLabel2Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel2Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    rLabel2Cell.SetCellValue($"Branch : {branchName}");
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 23));

                    #endregion


                    #region  ======== Earning Heads  Section ================
                    XSSFRow dataRow;
                    XSSFRow headerRow = CreateRow(sheet, 5);

                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);
                    XSSFRow tbCaption = CreateRow(sheet, 4);

                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 4, (4 + (earningCols - 1))));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 4 + earningCols, 4 + earningCols));

                    SetBordersToMergedCells(workbook, sheet);

                    var firstCellOfFourthRow = tbCaption.CreateCell(0);
                    firstCellOfFourthRow.CellStyle = workbook.CreateCellStyle();
                    firstCellOfFourthRow.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    firstCellOfFourthRow.CellStyle.FillPattern = FillPattern.SolidForeground;

                    var lastCellOfFouthRow = tbCaption.CreateCell(4 + earningCols);
                    lastCellOfFouthRow.CellStyle = workbook.CreateCellStyle();
                    lastCellOfFouthRow.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    lastCellOfFouthRow.CellStyle.FillPattern = FillPattern.SolidForeground;

                    var earingHeaderCells = tbCaption.CreateCell(4);
                    earingHeaderCells.SetCellValue("TOTAL EARNINGS");

                    earingHeaderCells.CellStyle = workbook.CreateCellStyle();
                    earingHeaderCells.CellStyle.SetFont(hFont);
                    earingHeaderCells.CellStyle.Alignment = HorizontalAlignment.Center;

                    earingHeaderCells.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    earingHeaderCells.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    earingHeaderCells.CellStyle.FillPattern = FillPattern.SolidForeground;

                    earingHeaderCells.CellStyle.BorderBottom = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderTop = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderLeft = BorderStyle.Thin;
                    earingHeaderCells.CellStyle.BorderRight = BorderStyle.Thin;

                    int hdrColIndex = 0;
                    foreach (var hdr in header1)
                    {
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.CellStyle = workbook.CreateCellStyle();

                        //  sheet.SetColumnWidth(hdrColIndex, 15);
                        cell.CellStyle.WrapText = true;

                        if (hdr.Equals("BranchName", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Branch Name");

                        else if (hdr.Equals("SNo", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("S.No.");
                        else if (hdr.Equals("EmployeeCode", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Employee Code");

                        else if (hdr.Equals("Name", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Employee Name");
                        else
                            cell.SetCellValue(hdr);

                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.BorderTop = BorderStyle.Thin;

                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;

                        cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                        cell.CellStyle.FillPattern = FillPattern.SolidForeground;

                        headerRow.Height = 900;
                        if (hdr == "Name")
                            sheet.SetColumnWidth(hdrColIndex, 9000);

                        else if (hdr == "BranchName")
                            sheet.SetColumnWidth(hdrColIndex, 6000);

                        else if (hdr == "EmployeeCode")
                            sheet.SetColumnWidth(hdrColIndex, 2500);

                        else if (hdrColIndex > 3)
                            sheet.SetColumnWidth(hdrColIndex, 3500);
                        // sheet.AutoSizeColumn(hdrColIndex);

                        hdrColIndex++;
                    }

                    for (int jj = 0; jj < noOfCols_in_header1; jj++)
                    {
                        int rowIndex = 6;

                        if (jj == 0)
                            dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                        else
                            dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                        var cell = dataRow.CreateCell(jj);

                        if (jj > 0)
                        {
                            int numVal;
                            if (int.TryParse(jj.ToString(), out numVal))
                                cell.SetCellValue(numVal);
                            else
                                cell.SetCellValue(jj);
                        }
                        else
                            cell.SetCellValue("");

                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;

                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;
                        dataRow.Height = 300;
                    }

                    foreach (DataColumn column in eHeadsDT.Columns)
                    {
                        int rowIndex = 7;
                        foreach (DataRow row in eHeadsDT.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            if (!row[column].ToString().Trim().Equals(""))
                            {
                                ICell Cell = dataRow.CreateCell(column.Ordinal);
                                Cell.CellStyle = workbook.CreateCellStyle();

                                Cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                //Cell.CellStyle.BorderTop = BorderStyle.Thin;
                                //Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                //Cell.CellStyle.BorderRight = BorderStyle.Thin;

                                if (column.Ordinal == 0 || column.Ordinal == 2)
                                    Cell.CellStyle.Alignment = HorizontalAlignment.Center;

                                if (column.Ordinal > 3)
                                {
                                    Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                    Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

                                    if (column.Ordinal == earningCols + 4)  ///=== Add Sum Formula for total of earning heads ===
                                    {
                                        var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(4));
                                        var fCellRefAsString = fromCellRef.FormatAsString();

                                        var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(column.Ordinal - 1));
                                        var toCellRefAsString = toCellRef.FormatAsString();

                                        hFont.FontHeightInPoints = 11;
                                        hFont.FontName = "Calibri";
                                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                        Cell.CellStyle.SetFont(hFont);
                                        Cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");
                                    }
                                }
                                else
                                    Cell.SetCellValue(row[column].ToString());
                            }
                            rowIndex++;
                        }
                        #region  ==== Earning Total Row ==============
                        if (column.Ordinal == 0)
                            dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                        else
                            dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                        if (column.Ordinal > 0)
                        {
                            var cell = dataRow.CreateCell(column.Ordinal);

                            cell.CellStyle = workbook.CreateCellStyle();
                            cell.CellStyle.SetFont(hFont);

                            if (column.Ordinal > 3)
                            {
                                #region  =========  Data Formating & Set Cell Formula ============

                                var fromCellRef = new CellReference(sheet.GetRow(7).GetCell(column.Ordinal));
                                var fCellRefAsString = fromCellRef.FormatAsString();

                                var toRowNo = sheet.LastRowNum - 1;
                                var toCellRef = new CellReference(sheet.GetRow(toRowNo).GetCell(column.Ordinal));
                                var toCellRefAsString = toCellRef.FormatAsString();

                                cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                                cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");

                                #endregion
                            }

                            if (column.Ordinal != 1)
                                cell.CellStyle.Alignment = HorizontalAlignment.Right;

                            cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            cell.CellStyle.BorderTop = BorderStyle.Thin;
                            cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            cell.CellStyle.BorderRight = BorderStyle.Thin;

                            cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                            cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                        }
                        else
                        {
                            var cell = dataRow.CreateCell(column.Ordinal);
                            cell.CellStyle = workbook.CreateCellStyle();
                            cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            cell.CellStyle.BorderTop = BorderStyle.Thin;
                            cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            cell.CellStyle.BorderRight = BorderStyle.Thin;
                            cell.CellStyle.SetFont(hFont);
                            cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                            cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                            cell.SetCellValue("Total");
                        }
                        dataRow.Height = 300;

                        #endregion
                    }

                    #endregion

                    #region ======= Deduction Section ==============

                    int lastRowIndex = sheet.LastRowNum + 1;

                    XSSFRow tbCaption2 = CreateRow(sheet, lastRowIndex);

                    sheet.AddMergedRegion(new CellRangeAddress(lastRowIndex, lastRowIndex, 0, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(lastRowIndex, lastRowIndex, 4, (4 + (deductionCols - 1))));
                    sheet.AddMergedRegion(new CellRangeAddress(lastRowIndex, lastRowIndex, 4 + deductionCols, 4 + deductionCols + 1));

                    SetBordersToMergedCells(workbook, sheet);

                    //====  merge start & end cells =======

                    var tbCaption2_start = tbCaption2.CreateCell(0);
                    tbCaption2_start.CellStyle = workbook.CreateCellStyle();
                    tbCaption2_start.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    tbCaption2_start.CellStyle.FillPattern = FillPattern.SolidForeground;

                    var tbCaption2_end = tbCaption2.CreateCell(4 + deductionCols);
                    tbCaption2_end.CellStyle = workbook.CreateCellStyle();
                    tbCaption2_end.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    tbCaption2_end.CellStyle.FillPattern = FillPattern.SolidForeground;

                    var deductionHeaderCells = tbCaption2.CreateCell(4);
                    deductionHeaderCells.SetCellValue("TOTAL DEDUCTIONS");

                    deductionHeaderCells.CellStyle = workbook.CreateCellStyle();
                    deductionHeaderCells.CellStyle.SetFont(hFont);
                    deductionHeaderCells.CellStyle.Alignment = HorizontalAlignment.Center;
                    deductionHeaderCells.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                    deductionHeaderCells.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    deductionHeaderCells.CellStyle.FillPattern = FillPattern.SolidForeground;
                    deductionHeaderCells.CellStyle.BorderBottom = BorderStyle.Thin;

                    deductionHeaderCells.CellStyle.BorderTop = BorderStyle.Thin;
                    deductionHeaderCells.CellStyle.BorderLeft = BorderStyle.Thin;
                    deductionHeaderCells.CellStyle.BorderRight = BorderStyle.Thin;

                    XSSFRow headerRow2 = CreateRow(sheet, sheet.LastRowNum + 1);

                    int table2HdrColIndex = 0;

                    foreach (var hdr2 in header2)
                    {
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        var cell = headerRow2.CreateCell(table2HdrColIndex);
                        cell.CellStyle = workbook.CreateCellStyle();

                        cell.CellStyle.WrapText = true;

                        if (hdr2.Equals("BranchName", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Branch Name");

                        else if (hdr2.Equals("SNo", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("S.No.");
                        else if (hdr2.Equals("EmployeeCode", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Employee Code");

                        else if (hdr2.Equals("Name", StringComparison.OrdinalIgnoreCase))
                            cell.SetCellValue("Employee Name");
                        else
                            cell.SetCellValue(hdr2);

                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.BorderTop = BorderStyle.Thin;

                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;

                        cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                        cell.CellStyle.FillPattern = FillPattern.SolidForeground;

                        headerRow2.Height = 900;

                        if (hdr2 == "Name")
                            sheet.SetColumnWidth(table2HdrColIndex, 9000);

                        else if (hdr2 == "BranchName")
                            sheet.SetColumnWidth(table2HdrColIndex, 6000);

                        else if (hdr2 == "EmployeeCode")
                            sheet.SetColumnWidth(table2HdrColIndex, 2500);

                        else if (table2HdrColIndex > 3)
                            sheet.SetColumnWidth(table2HdrColIndex, 3500);

                        table2HdrColIndex++;
                    }

                    var nRowIndex = sheet.LastRowNum + 1;
                    for (int jj = 0; jj < noOfCols_in_header2; jj++)
                    {
                        if (jj == 0)
                            dataRow = (XSSFRow)sheet.CreateRow(nRowIndex);
                        else
                            dataRow = (XSSFRow)sheet.GetRow(nRowIndex);

                        var cell = dataRow.CreateCell(jj);

                        if (jj > 0)
                        {
                            int numVal;
                            if (int.TryParse(jj.ToString(), out numVal))
                                cell.SetCellValue(numVal);
                            else
                                cell.SetCellValue(jj);
                        }
                        else
                            cell.SetCellValue("");

                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;

                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;
                        dataRow.Height = 300;
                    }

                    foreach (DataColumn column in dHeadsDT.Columns)
                    {
                        int rowIndex = nRowIndex + 1, tot_earningStartIndex = 7;
                        if (column.Ordinal == 0)
                            dHeadsDTStartIndex = rowIndex;

                        foreach (DataRow row in dHeadsDT.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            if (!row[column].ToString().Trim().Equals(""))
                            {
                                ICell Cell = dataRow.CreateCell(column.Ordinal);
                                Cell.CellStyle = workbook.CreateCellStyle();

                                Cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                //Cell.CellStyle.BorderTop = BorderStyle.Thin;
                                //Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                //Cell.CellStyle.BorderRight = BorderStyle.Thin;

                                if (column.Ordinal == 0 || column.Ordinal == 2)
                                    Cell.CellStyle.Alignment = HorizontalAlignment.Center;

                                if (column.Ordinal > 3)
                                {
                                    Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                    Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

                                    if (column.Ordinal == deductionCols + 4)  ///=== Add Sum Formula for total of earning heads ===
                                    {
                                        var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(4));
                                        var fCellRefAsString = fromCellRef.FormatAsString();

                                        var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(column.Ordinal - 1));
                                        var toCellRefAsString = toCellRef.FormatAsString();

                                        hFont.FontHeightInPoints = 11;
                                        hFont.FontName = "Calibri";
                                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                        Cell.CellStyle.SetFont(hFont);
                                        Cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");
                                    }
                                    else if (column.Ordinal == dHeadsDT.Columns.Count - 1)
                                    {
                                        var fromCellRef = new CellReference(sheet.GetRow(tot_earningStartIndex).GetCell(earningCols + 4));
                                        var fCellRefAsString = fromCellRef.FormatAsString();

                                        var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(column.Ordinal));
                                        var toCellRefAsString = toCellRef.FormatAsString();

                                        hFont.FontHeightInPoints = 11;
                                        hFont.FontName = "Calibri";
                                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                        Cell.CellStyle.SetFont(hFont);
                                        Cell.SetCellFormula($"");
                                    }
                                }
                                else
                                    Cell.SetCellValue(row[column].ToString());
                            }
                            rowIndex++; tot_earningStartIndex++;
                        }

                        #region  ===== = Total Row ============

                        if (column.Ordinal == 0)
                            dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                        else
                            dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                        if (column.Ordinal > 0)
                        {
                            var cell = dataRow.CreateCell(column.Ordinal);

                            cell.CellStyle = workbook.CreateCellStyle();
                            cell.CellStyle.SetFont(hFont);

                            if (column.Ordinal > 3)
                            {
                                #region  =========  Data Formating & Set Cell Formula ============

                                var fromCellRef = new CellReference(sheet.GetRow(dHeadsDTStartIndex).GetCell(column.Ordinal));
                                var fCellRefAsString = fromCellRef.FormatAsString();

                                var toRowNo = sheet.LastRowNum - 1;
                                var toCellRef = new CellReference(sheet.GetRow(toRowNo).GetCell(column.Ordinal));
                                var toCellRefAsString = toCellRef.FormatAsString();

                                cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                                cell.SetCellFormula($"SUM({fCellRefAsString}:{toCellRefAsString})");

                                #endregion
                            }

                            if (column.Ordinal != 1)
                                cell.CellStyle.Alignment = HorizontalAlignment.Right;

                            cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            cell.CellStyle.BorderTop = BorderStyle.Thin;
                            cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            cell.CellStyle.BorderRight = BorderStyle.Thin;

                            cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                            cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                        }
                        else
                        {
                            var cell = dataRow.CreateCell(column.Ordinal);
                            cell.CellStyle = workbook.CreateCellStyle();
                            cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            cell.CellStyle.BorderTop = BorderStyle.Thin;
                            cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            cell.CellStyle.BorderRight = BorderStyle.Thin;
                            cell.CellStyle.SetFont(hFont);
                            cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                            cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                            cell.SetCellValue("Total");
                        }
                        dataRow.Height = 300;

                        #endregion
                    }

                    #endregion

                    workbook.Write(file);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return "fail - " + ex.Message;
            }
        }

        private static void AddBorderToCell(ICellStyle currentCellStyle)
        {
            currentCellStyle.BorderBottom = BorderStyle.Thin;
            currentCellStyle.BorderTop = BorderStyle.Thin;
            currentCellStyle.BorderLeft = BorderStyle.Thin;
            currentCellStyle.BorderRight = BorderStyle.Thin;
        }

        public static string ExportEmpDtlWithICICIBank(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, int salMonth)
        {
            log.Info("SalaryReportExport/ExportEmpDtlWithICICIBank");

            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    var total = rowData.Rows[0]["total"].ToString();
                    string month;
                    switch (salMonth)
                    {
                        case 1:

                            month = "JAN.1";
                            break;
                        case 2:
                            month = "FEB.2";
                            break;
                        case 3:
                            month = "MAR.3";
                            break;
                        case 4:
                            month = "APR.4";
                            break;
                        case 5:
                            month = "MAY.5";
                            break;
                        case 6:
                            month = "JUN.6";
                            break;
                        case 7:
                            month = "JUL.7";
                            break;
                        case 8:
                            month = "AUG.8";
                            break;
                        case 9:
                            month = "SEP.9";
                            break;
                        case 10:
                            month = "OCT.10";
                            break;
                        case 11:
                            month = "NOV.11";
                            break;
                        case 12:
                            month = "DEC.12";
                            break;
                        default:
                            month = "";
                            break;
                    }

                    rowData.Columns.Remove("total");

                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);
                    XSSFRow dataRow;

                    XSSFFont scFont = CreateFont(workbook);
                    scFont.FontHeightInPoints = 11;
                    scFont.FontName = "Calibri";
                    scFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;


                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(scFont);
                    titlecell.CellStyle.WrapText = false;
                    titlecell.SetCellValue("SALARY UPLOAD PREPRARATION FOR THE MONTH OF______________OF_______________");
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 8));

                    XSSFRow headerRow = CreateRow(sheet, 1);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);
                    hFont.FontHeightInPoints = 11;
                    hFont.FontName = "Calibri";
                    hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {
                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.SetCellValue(hdr);
                        cell.CellStyle = workbook.CreateCellStyle();

                        cell.CellStyle.SetFont(hFont);
                        //   cell.CellStyle.Alignment = HorizontalAlignment.Left;
                        if (hdr == "PARTICULARS" || hdr == "EMPLOYEE NAME")
                        {
                            sheet.SetColumnWidth(hdrColIndex, 10000);
                        }
                        else
                            sheet.AutoSizeColumn(hdrColIndex);
                        //  cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        // headerRow.Height = 500;
                        //if (hdr == "")
                        //    sheet.SetColumnWidth(hdrColIndex, 35);
                        //else

                        hdrColIndex++;
                    }

                    XSSFRow thirdrow = CreateRow(sheet, 2);
                    ICell secondRScell = thirdrow.CreateCell(0);
                    secondRScell.CellStyle = workbook.CreateCellStyle();
                    secondRScell.CellStyle.SetFont(hFont);
                    secondRScell.SetCellValue("GRAND TOTAL-------------------------------->");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 2));

                    ICell thrdrow4cell = thirdrow.CreateCell(3);
                    thrdrow4cell.CellStyle = workbook.CreateCellStyle();
                    thrdrow4cell.CellStyle.Alignment = HorizontalAlignment.Right;
                    thrdrow4cell.SetCellValue(total);


                    foreach (DataColumn column in rowData.Columns)
                    {

                        int rowIndex = 3;
                        //  var oldvalue = string.Empty;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            ICell Cell = dataRow.CreateCell(column.Ordinal);
                            Cell.CellStyle = workbook.CreateCellStyle();
                            if (column.ColumnName == "PARTICULARS")
                            {

                                Cell.SetCellValue("SALARY FOR THE MONTH " + month);
                            }
                            else
                            {
                                if (column.ColumnName == "AMOUNT")
                                    Cell.CellStyle.Alignment = HorizontalAlignment.Right;

                                Cell.SetCellValue(row[column].ToString());
                            }
                            rowIndex++;
                        }
                    }

                    workbook.Write(file);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return "fail - " + ex.Message;
            }
        }


        public static string ExportEmpDtlWithPSBank(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, int salMonth)
        {
            log.Info("SalaryReportExport/ExportEmpDtlWithPSBank");

            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    var total = rowData.Rows[0]["total"].ToString();
                    string month;
                    switch (salMonth)
                    {
                        case 1:

                            month = "JAN.1";
                            break;
                        case 2:
                            month = "FEB.2";
                            break;
                        case 3:
                            month = "MAR.3";
                            break;
                        case 4:
                            month = "APR.4";
                            break;
                        case 5:
                            month = "MAY.5";
                            break;
                        case 6:
                            month = "JUN.6";
                            break;
                        case 7:
                            month = "JUL.7";
                            break;
                        case 8:
                            month = "AUG.8";
                            break;
                        case 9:
                            month = "SEP.9";
                            break;
                        case 10:
                            month = "OCT.10";
                            break;
                        case 11:
                            month = "NOV.11";
                            break;
                        case 12:
                            month = "DEC.12";
                            break;
                        default:
                            month = "";
                            break;
                    }

                    rowData.Columns.Remove("total");

                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);
                    XSSFRow dataRow;

                    XSSFFont scFont = CreateFont(workbook);
                    scFont.FontHeightInPoints = 11;
                    scFont.FontName = "Calibri";
                    scFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;


                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(scFont);
                    titlecell.CellStyle.WrapText = false;
                    titlecell.SetCellValue("SALARY UPLOAD PREPRARATION FOR THE MONTH OF______________OF_______________");
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 8));

                    XSSFRow headerRow = CreateRow(sheet, 1);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);
                    hFont.FontHeightInPoints = 11;
                    hFont.FontName = "Calibri";
                    hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {
                        var cell = headerRow.CreateCell(hdrColIndex);
                        if (hdr == "Column1" || hdr == "Column2")
                            cell.SetCellValue("");
                        else
                            cell.SetCellValue(hdr);
                        cell.CellStyle = workbook.CreateCellStyle();

                        cell.CellStyle.SetFont(hFont);
                        //   cell.CellStyle.Alignment = HorizontalAlignment.Left;
                        if (hdr == "REMARKS" || hdr == "EMPLOYEE NAME")
                        {
                            sheet.SetColumnWidth(hdrColIndex, 10000);
                        }
                        else
                            sheet.AutoSizeColumn(hdrColIndex);
                        //  cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        // headerRow.Height = 500;
                        //if (hdr == "")
                        //    sheet.SetColumnWidth(hdrColIndex, 35);
                        //else

                        hdrColIndex++;
                    }


                    foreach (DataColumn column in rowData.Columns)
                    {

                        int rowIndex = 3;
                        //  var oldvalue = string.Empty;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            ICell Cell = dataRow.CreateCell(column.Ordinal);
                            Cell.CellStyle = workbook.CreateCellStyle();
                            if (column.ColumnName == "REMARKS")
                            {
                                Cell.SetCellValue("SALARY FOR THE MONTH " + month);
                            }
                            else
                            {
                                if (column.ColumnName == "AMOUNT")
                                    Cell.CellStyle.Alignment = HorizontalAlignment.Right;

                                Cell.SetCellValue(row[column].ToString());
                            }
                            rowIndex++;
                        }
                    }
                    XSSFRow lastrow = CreateRow(sheet, rowData.Rows.Count + 3);
                    ICell lastR1cell = lastrow.CreateCell(0);
                    lastR1cell.CellStyle = workbook.CreateCellStyle();
                    lastR1cell.SetCellValue("");

                    ICell lastR2cell = lastrow.CreateCell(1);
                    lastR2cell.CellStyle = workbook.CreateCellStyle();
                    lastR2cell.SetCellValue("06121600075001");

                    ICell lastR3cell = lastrow.CreateCell(2);
                    lastR3cell.CellStyle = workbook.CreateCellStyle();
                    lastR3cell.SetCellValue("");

                    ICell lastR4cell = lastrow.CreateCell(3);
                    lastR4cell.CellStyle = workbook.CreateCellStyle();
                    lastR4cell.SetCellValue("");

                    ICell lastR5cell = lastrow.CreateCell(4);
                    lastR5cell.CellStyle = workbook.CreateCellStyle();
                    lastR5cell.SetCellValue("D");

                    ICell lastR6cell = lastrow.CreateCell(5);
                    lastR6cell.CellStyle = workbook.CreateCellStyle();
                    lastR6cell.CellStyle.Alignment = HorizontalAlignment.Right;
                    lastR6cell.SetCellValue(total);

                    ICell lastR7cell = lastrow.CreateCell(6);
                    lastR7cell.CellStyle = workbook.CreateCellStyle();
                    lastR7cell.SetCellValue("SALARY FOR THE MONTH " + month);

                    workbook.Write(file);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return "fail - " + ex.Message;
            }
        }

    }


}

