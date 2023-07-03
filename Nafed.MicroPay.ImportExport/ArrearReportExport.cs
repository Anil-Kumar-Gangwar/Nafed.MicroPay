using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;

namespace Nafed.MicroPay.ImportExport
{
    public class ArrearReportExport : BaseExcel
    {
        ArrearReportExport()
        {

        }

        public static string DAArrearExportToExcel(IEnumerable<string> headers,
            DataTable reportData, int fromPeriod, int toPeriod, string sSheetName, string sFullPath)
        {
            log.Info("ArrearReportExport/DAArrearExportToExcel");

            try
            {
                int f_monthNumber = (fromPeriod % 100); //1-12   
                int t_monthNumber = (toPeriod % 100);

                string f_monthName = new DateTimeFormatInfo().GetMonthName(f_monthNumber);
                string t_monthName = new DateTimeFormatInfo().GetMonthName(t_monthNumber);

                var f_Period = $"{f_monthName.Substring(0, 3) }-{fromPeriod.ToString().Substring(0, 4)}";
                var t_Period = $"{t_monthName.Substring(0, 3)}-{toPeriod.ToString().Substring(0, 4)}";

                XSSFWorkbook workbook = CreateWookBook();

                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);
                    XSSFFont hFont = CreateFont(workbook);

                    hFont.FontHeightInPoints = 20;
                    hFont.FontName = "Calibri";
                    hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    #region   ======Report Headering   === === ///============

                    XSSFRow rHeader1 = CreateRow(sheet, 0);

                    var cellRH1_0 = rHeader1.CreateCell(0);
                    cellRH1_0.SetCellValue("NATIONAL AGRICULTURAL COOPERATIVE MARKETING FEDERATION OF INDIA LTD.");

                    cellRH1_0.CellStyle = workbook.CreateCellStyle();
                    cellRH1_0.CellStyle.Alignment = HorizontalAlignment.Center;
                    cellRH1_0.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    cellRH1_0.CellStyle.SetFont(hFont);

                    XSSFRow rHeader2 = CreateRow(sheet, 1);

                    var cellRH2_0 = rHeader2.CreateCell(0);
                    cellRH2_0.SetCellValue($"D.A. ARREAR FOR THE PERIOD FROM {f_Period} TO {t_Period}");

                    cellRH2_0.CellStyle = workbook.CreateCellStyle();
                    cellRH2_0.CellStyle.Alignment = HorizontalAlignment.Center;
                    cellRH2_0.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    cellRH2_0.CellStyle.SetFont(hFont);

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 15));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 15));
                    //     SetBordersToMergedCells(workbook, sheet);

                    #endregion

                    //  XSSFRow dataRow;
                    XSSFRow headerRow = CreateRow(sheet, 3);
                    // Writing Header Row
                    XSSFRow dataRow;
                    int hdrColIndex = 0;

                    foreach (var hdr in headers)
                    {
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.CellStyle = workbook.CreateCellStyle();
                        //     cell.CellStyle.WrapText = true;

                        cell.SetCellValue(hdr.ToUpper());

                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;

                        cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                        cell.CellStyle.FillPattern = FillPattern.SolidForeground;

                        headerRow.Height = 500;
                        hdrColIndex++;
                    }

                    foreach (DataColumn column in reportData.Columns)
                    {
                        int rowIndex = 4;

                        foreach (DataRow row in reportData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            ICell Cell = dataRow.CreateCell(column.Ordinal);
                            Cell.CellStyle = workbook.CreateCellStyle();
                           

                            if (column.Ordinal > 4 && column.Ordinal < reportData.Columns.Count - 1)
                            {
                                Cell.CellStyle.Alignment = HorizontalAlignment.Right;
                                Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                            }
                            else if(column.Ordinal== reportData.Columns.Count - 1)
                            {
                                var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(column.Ordinal-2));
                                var fCellRefAsString = fromCellRef.FormatAsString();

                                var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(column.Ordinal - 1));
                                var toCellRefAsString = toCellRef.FormatAsString();
                                Cell.CellStyle.Alignment = HorizontalAlignment.Right;
                                Cell.SetCellFormula($"{fCellRefAsString}-{toCellRefAsString}");
                            }
                            else
                                Cell.SetCellValue(row[column].ToString());
                            Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00");
                            rowIndex++;
                        }
                        //   sheet.AutoSizeColumn(column.Ordinal);
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


        public static string PayArrearExportToExcel(IEnumerable<string> headers,
            DataTable reportData, int fromPeriod, int toPeriod, string sSheetName, string sFullPath)
        {
            log.Info("ArrearReportExport/PayArrearExportToExcel");
            try
            {
                int f_monthNumber = (fromPeriod % 100); //1-12   
                int t_monthNumber = (toPeriod % 100);

                string f_monthName = new DateTimeFormatInfo().GetMonthName(f_monthNumber);
                string t_monthName = new DateTimeFormatInfo().GetMonthName(t_monthNumber);

                var f_Period = $"{f_monthName.Substring(0, 3) }-{fromPeriod.ToString().Substring(0, 4)}";
                var t_Period = $"{t_monthName.Substring(0, 3)}-{toPeriod.ToString().Substring(0, 4)}";

                XSSFWorkbook workbook = CreateWookBook();

                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);
                    XSSFFont hFont = CreateFont(workbook);

                    hFont.FontHeightInPoints = 20;
                    hFont.FontName = "Calibri";
                    hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    #region   ======Report Headering   === === ///============

                    XSSFRow rHeader1 = CreateRow(sheet, 0);

                    var cellRH1_0 = rHeader1.CreateCell(0);
                    cellRH1_0.SetCellValue("NATIONAL AGRICULTURAL COOPERATIVE MARKETING FEDERATION OF INDIA LTD.");

                    cellRH1_0.CellStyle = workbook.CreateCellStyle();
                    cellRH1_0.CellStyle.Alignment = HorizontalAlignment.Center;
                    cellRH1_0.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    cellRH1_0.CellStyle.SetFont(hFont);

                    XSSFRow rHeader2 = CreateRow(sheet, 1);

                    var cellRH2_0 = rHeader2.CreateCell(0);
                    cellRH2_0.SetCellValue($"Pay ARREAR FOR THE PERIOD FROM {f_Period} TO {t_Period}");

                    cellRH2_0.CellStyle = workbook.CreateCellStyle();
                    cellRH2_0.CellStyle.Alignment = HorizontalAlignment.Center;
                    cellRH2_0.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    cellRH2_0.CellStyle.SetFont(hFont);

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 15));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 15));
                    //     SetBordersToMergedCells(workbook, sheet);

                    #endregion

                    //  XSSFRow dataRow;
                    XSSFRow headerRow = CreateRow(sheet, 3);
                    // Writing Header Row
                    XSSFRow dataRow;
                    int hdrColIndex = 0;

                    foreach (var hdr in headers)
                    {
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.CellStyle = workbook.CreateCellStyle();
                        //     cell.CellStyle.WrapText = true;

                        cell.SetCellValue(hdr.ToUpper());

                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;

                        cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                        cell.CellStyle.FillPattern = FillPattern.SolidForeground;

                        headerRow.Height = 500;
                        hdrColIndex++;
                    }

                    //foreach (DataColumn column in reportData.Columns)
                    //{
                    //    int rowIndex = 4;

                    //    foreach (DataRow row in reportData.Rows)
                    //    {
                    //        if (column.Ordinal == 0)
                    //            dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                    //        else
                    //            dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                    //        ICell Cell = dataRow.CreateCell(column.Ordinal);
                    //        Cell.CellStyle = workbook.CreateCellStyle();

                    //        if (column.Ordinal > 4)
                    //        {
                    //            Cell.CellStyle.Alignment = HorizontalAlignment.Right;
                    //            Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                    //            Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00");
                    //        }
                    //        else
                    //            Cell.SetCellValue(row[column].ToString());
                    //        rowIndex++;
                    //    }
                    //    //   sheet.AutoSizeColumn(column.Ordinal);
                    //}

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
        public static string PayArrearExportToExcel(IEnumerable<string> headers, string sSheetName, string sFullPath)
        {
            log.Info("ArrearReportExport/PayArrearExportToExcel");
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return "fail - " + ex.Message;
            }
        }
    }
}
