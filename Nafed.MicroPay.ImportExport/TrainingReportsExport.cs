using System;
using System.IO;
using System.Data;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using NPOI.SS.Util;
using NPOI.HSSF.UserModel;

namespace Nafed.MicroPay.ImportExport
{
    public class TrainingReportsExport : BaseExcel
    {
        private TrainingReportsExport()
        {
        }

        public static string ExportToExcel(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, string tFilter)
        {
            log.Info("TrainingReportsExport/ExportToExcel");

            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);
                    XSSFRow dataRow;

                    XSSFFont scFont = CreateFont(workbook);
                    scFont.FontHeightInPoints = 12;
                    scFont.FontName = "Calibri";
                    scFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFFont titleFont = CreateFont(workbook);
                    titleFont.FontHeightInPoints = 14;
                    titleFont.FontName = "Calibri";
                    titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(titleFont);
                    titlecell.CellStyle.WrapText = true;
                    titlerow.Height = 1000;
                    titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                    titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");
                    if (string.IsNullOrEmpty(tFilter))
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 11));
                    else
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));

                    XSSFRow firstRow = CreateRow(sheet, 1);
                    ICell firstCell = firstRow.CreateCell(0);
                    firstCell.CellStyle = workbook.CreateCellStyle();
                    firstCell.SetCellValue("");
                    firstCell.CellStyle.SetFont(scFont);
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 7));

                    firstCell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstCell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstCell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstCell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR2cell = firstRow.CreateCell(1);
                    firstR2cell.CellStyle = workbook.CreateCellStyle();
                    firstR2cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR3cell = firstRow.CreateCell(2);
                    firstR3cell.CellStyle = workbook.CreateCellStyle();
                    firstR3cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR4cell = firstRow.CreateCell(3);
                    firstR4cell.CellStyle = workbook.CreateCellStyle();
                    firstR4cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR5cell = firstRow.CreateCell(4);
                    firstR5cell.CellStyle = workbook.CreateCellStyle();
                    firstR5cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR6cell = firstRow.CreateCell(5);
                    firstR6cell.CellStyle = workbook.CreateCellStyle();
                    firstR6cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR7cell = firstRow.CreateCell(6);
                    firstR7cell.CellStyle = workbook.CreateCellStyle();
                    firstR7cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR8cell = firstRow.CreateCell(7);
                    firstR8cell.CellStyle = workbook.CreateCellStyle();
                    firstR8cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell printDate = firstRow.CreateCell(8);
                    printDate.CellStyle = workbook.CreateCellStyle();
                    printDate.SetCellValue("Print Date:  " + DateTime.Now.Date.ToString("dd-MMM-yyyy"));
                    printDate.CellStyle.SetFont(scFont);
                    printDate.CellStyle.BorderTop = BorderStyle.Thin;
                    printDate.CellStyle.BorderLeft = BorderStyle.Thin;
                    printDate.CellStyle.BorderRight = BorderStyle.Thin;
                    printDate.CellStyle.BorderBottom = BorderStyle.Thin;

                    if (string.IsNullOrEmpty(tFilter))
                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, 8, 11));
                    else
                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, 8, 10));



                    ICell firstR9cell = firstRow.CreateCell(9);
                    firstR9cell.CellStyle = workbook.CreateCellStyle();
                    firstR9cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR10cell = firstRow.CreateCell(10);
                    firstR10cell.CellStyle = workbook.CreateCellStyle();
                    firstR10cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    if (string.IsNullOrEmpty(tFilter))
                    {
                        ICell firstR11cell = firstRow.CreateCell(11);
                        firstR11cell.CellStyle = workbook.CreateCellStyle();
                        firstR11cell.CellStyle.BorderTop = BorderStyle.Thin;
                        firstR11cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        firstR11cell.CellStyle.BorderRight = BorderStyle.Thin;
                        firstR11cell.CellStyle.BorderBottom = BorderStyle.Thin;
                    }
                    XSSFRow secondrow = CreateRow(sheet, 2);
                    ICell secondcell = secondrow.CreateCell(0);
                    secondcell.CellStyle = workbook.CreateCellStyle();
                    secondcell.SetCellValue("");

                    if (string.IsNullOrEmpty(tFilter))
                        sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 11));
                    else
                        sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));



                    XSSFRow headerRow = CreateRow(sheet, 3);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);
                    hFont.FontHeightInPoints = 12;
                    hFont.FontName = "Calibri";
                    hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {
                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.SetCellValue(hdr);
                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        headerRow.Height = 500;
                        if (hdr == "Venue")
                            sheet.SetColumnWidth(hdrColIndex, 10000);
                        else
                            sheet.AutoSizeColumn(hdrColIndex);
                        hdrColIndex++;
                    }

                    foreach (DataColumn column in rowData.Columns)
                    {

                        int rowIndex = 4;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            ICell Cell = dataRow.CreateCell(column.Ordinal);
                            Cell.CellStyle = workbook.CreateCellStyle();
                            Cell.CellStyle.BorderTop = BorderStyle.Thin;
                            Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            Cell.CellStyle.BorderRight = BorderStyle.Thin;
                            Cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            Cell.SetCellValue(row[column].ToString());
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

        public static string ExportTrainingRating(dynamic _training,
          string venueAddress,
          IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath)
        {
            log.Info("TrainingReportsExport/ExportTrainingRating");

            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);


                    #region  ============= Report Header Section ==============


                    XSSFRow rHeader1 = CreateRow(sheet, 0);
                    var cellRH1_0 = rHeader1.CreateCell(0);
                    cellRH1_0.SetCellValue("Training Title");

                    var cellRH1_1 = rHeader1.CreateCell(1);
                    cellRH1_1.SetCellValue(_training.TrainingTitle);

                    var cellLabel_TType = rHeader1.CreateCell(3);
                    cellLabel_TType.SetCellValue("Training Type");

                    var cellValue_TType = rHeader1.CreateCell(4);
                    cellValue_TType.SetCellValue(_training.enumTrainingList.ToString());


                    XSSFRow rHeader2 = CreateRow(sheet, 1);
                    var cellRH2_0 = rHeader2.CreateCell(0);
                    cellRH2_0.SetCellValue("Training For");

                    var cellRH2_1 = rHeader2.CreateCell(1);
                    cellRH2_1.SetCellValue(_training.TrainingTypeName);

                    var cellLabel_TAType = rHeader2.CreateCell(3);
                    cellLabel_TAType.SetCellValue("Address Type");

                    var cellValue_TAType = rHeader2.CreateCell(4);
                    cellValue_TAType.SetCellValue(_training.enumResidentialNonResidential.ToString());


                    XSSFRow rHeader3 = CreateRow(sheet, 2);
                    var cellRH3_0 = rHeader3.CreateCell(0);
                    cellRH3_0.SetCellValue("Training Start Date");

                    var cellRH3_1 = rHeader3.CreateCell(1);
                    cellRH3_1.SetCellValue(_training.StartDate.ToString("dd-MM-yyyy"));

                    var cellRH3_3 = rHeader3.CreateCell(3);
                    cellRH3_3.SetCellValue("Training End Date");

                    var cellRH3_4 = rHeader3.CreateCell(4);
                    cellRH3_4.SetCellValue(_training.EndDate.ToString("dd-MM-yyyy"));

                    XSSFRow rHeader4 = CreateRow(sheet, 3);
                    var cellRH4_0 = rHeader4.CreateCell(0);
                    cellRH4_0.SetCellValue("Training Slot Type");

                    var cellRH4_1 = rHeader4.CreateCell(1);
                    cellRH4_1.SetCellValue(_training.enumTimeSlotType.ToString());


                    var cellRH4_3 = rHeader4.CreateCell(3);
                    cellRH4_3.SetCellValue("Start Time");

                    var cellRH4_4 = rHeader4.CreateCell(4);
                    cellRH4_4.SetCellValue(_training.StartDateFromTime.ToString());


                    var cellRH4_6 = rHeader4.CreateCell(6);
                    cellRH4_6.SetCellValue("End Time");

                    var cellRH4_7 = rHeader4.CreateCell(7);
                    cellRH4_7.SetCellValue(_training.StartDateToTime.ToString());


                    XSSFRow rHeader5 = CreateRow(sheet, 4);
                    var cellRH5_0 = rHeader5.CreateCell(0);
                    cellRH5_0.SetCellValue("Venue");

                    var cellRH5_1 = rHeader5.CreateCell(1);
                    cellRH5_1.SetCellValue(venueAddress);

                    #endregion


                    XSSFRow dataRow;
                    XSSFRow headerRow = CreateRow(sheet, 5);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);
                    hFont.FontHeightInPoints = 11;
                    hFont.FontName = "Calibri";
                    hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    int hdrColIndex = 0;

                    foreach (var hdr in headers)
                    {
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.SetCellValue(hdr);
                        cell.CellStyle = workbook.CreateCellStyle();

                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        headerRow.Height = 500;
                        hdrColIndex++;
                    }

                    foreach (DataColumn column in rowData.Columns)
                    {
                        int rowIndex = 6;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            var Cell = dataRow.CreateCell(column.Ordinal);
                            Cell.CellStyle = workbook.CreateCellStyle();

                            if (column.Ordinal > 2)
                            {
                                Cell.SetCellType(CellType.Numeric);

                                //var newDataFormat = workbook.CreateDataFormat();
                                //Cell.CellStyle.DataFormat = newDataFormat.GetFormat("0");
                                Cell.CellStyle.Alignment = HorizontalAlignment.Right;
                                Cell.SetCellValue(Convert.ToInt16(row[column].ToString()));
                            }
                            else
                                Cell.SetCellValue(row[column].ToString());
                            rowIndex++;
                        }
                    }

                    var lastRowIndex = sheet.LastRowNum;

                    foreach (DataColumn col in rowData.Columns)
                    {
                        if (col.Ordinal == 0)
                            dataRow = (XSSFRow)sheet.CreateRow(lastRowIndex + 1);
                        else
                            dataRow = (XSSFRow)sheet.GetRow(lastRowIndex + 1);

                        var cell = dataRow.CreateCell(col.Ordinal);
                        cell.SetCellValue("Average Rating");
                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                        cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                        cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;

                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;


                        if (col.Ordinal > 2)
                        {
                            ///==== Add average formula ===========
                            var fromCellRef = new CellReference(sheet.GetRow(6).GetCell(col.Ordinal));
                            var fCellRefAsString = fromCellRef.FormatAsString();

                            var toCellRef = new CellReference(sheet.GetRow(lastRowIndex).GetCell(col.Ordinal));
                            var toCellRefAsString = toCellRef.FormatAsString();

                            hFont.FontHeightInPoints = 11;
                            hFont.FontName = "Calibri";
                            hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                            cell.CellStyle.SetFont(hFont);
                            cell.SetCellFormula($"AVERAGE({fCellRefAsString}:{toCellRefAsString})");
                            cell.CellStyle.Alignment = HorizontalAlignment.Right;
                        }
                    }
                    sheet.AddMergedRegion(new CellRangeAddress(lastRowIndex + 1, lastRowIndex + 1, 0, 2));

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
        public static string ExportTrainingDesignationWise(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, string tFilter)
        {
            log.Info("TrainingReportsExport/ExportTrainingDesignationWise");

            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);
                    XSSFRow dataRow;

                    XSSFFont scFont = CreateFont(workbook);
                    scFont.FontHeightInPoints = 12;
                    scFont.FontName = "Calibri";
                    scFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFFont titleFont = CreateFont(workbook);
                    titleFont.FontHeightInPoints = 14;
                    titleFont.FontName = "Calibri";
                    titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(titleFont);
                    titlecell.CellStyle.WrapText = true;
                    titlerow.Height = 1000;
                    titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                    titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));


                    XSSFRow firstRow = CreateRow(sheet, 1);
                    ICell firstCell = firstRow.CreateCell(0);
                    firstCell.CellStyle = workbook.CreateCellStyle();
                    firstCell.SetCellValue("Designation:  " + tFilter);
                    firstCell.CellStyle.SetFont(scFont);

                    firstCell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstCell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstCell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstCell.CellStyle.BorderBottom = BorderStyle.Thin;
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 7));

                    ICell firstR2cell = firstRow.CreateCell(1);
                    firstR2cell.CellStyle = workbook.CreateCellStyle();
                    firstR2cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR3cell = firstRow.CreateCell(2);
                    firstR3cell.CellStyle = workbook.CreateCellStyle();
                    firstR3cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR4cell = firstRow.CreateCell(3);
                    firstR4cell.CellStyle = workbook.CreateCellStyle();
                    firstR4cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR5cell = firstRow.CreateCell(4);
                    firstR5cell.CellStyle = workbook.CreateCellStyle();
                    firstR5cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR6cell = firstRow.CreateCell(5);
                    firstR6cell.CellStyle = workbook.CreateCellStyle();
                    firstR6cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR7cell = firstRow.CreateCell(6);
                    firstR7cell.CellStyle = workbook.CreateCellStyle();
                    firstR7cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR8cell = firstRow.CreateCell(7);
                    firstR8cell.CellStyle = workbook.CreateCellStyle();
                    firstR8cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell printDate = firstRow.CreateCell(8);
                    printDate.CellStyle = workbook.CreateCellStyle();
                    printDate.SetCellValue("Print Date:  " + DateTime.Now.Date.ToString("dd-MMM-yyyy"));
                    printDate.CellStyle.SetFont(scFont);
                    printDate.CellStyle.BorderTop = BorderStyle.Thin;
                    printDate.CellStyle.BorderLeft = BorderStyle.Thin;
                    printDate.CellStyle.BorderRight = BorderStyle.Thin;
                    printDate.CellStyle.BorderBottom = BorderStyle.Thin;
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 8, 10));

                    ICell firstR9cell = firstRow.CreateCell(9);
                    firstR9cell.CellStyle = workbook.CreateCellStyle();
                    firstR9cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR10cell = firstRow.CreateCell(10);
                    firstR10cell.CellStyle = workbook.CreateCellStyle();
                    firstR10cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    XSSFRow secondrow = CreateRow(sheet, 2);
                    ICell secondRScell = secondrow.CreateCell(0);
                    secondRScell.CellStyle = workbook.CreateCellStyle();
                    secondRScell.SetCellValue("");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));

                    XSSFRow headerRow = CreateRow(sheet, 3);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);
                    hFont.FontHeightInPoints = 12;
                    hFont.FontName = "Calibri";
                    hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {
                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.SetCellValue(hdr);
                        cell.CellStyle = workbook.CreateCellStyle();

                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;

                        headerRow.Height = 500;
                        if (hdr == "Venue" || hdr == "Name")
                            sheet.SetColumnWidth(hdrColIndex, 10000);
                        else
                            sheet.AutoSizeColumn(hdrColIndex);
                        hdrColIndex++;
                    }

                    foreach (DataColumn column in rowData.Columns)
                    {

                        int rowIndex = 4;
                        var oldvalue = string.Empty;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            ICell Cell = dataRow.CreateCell(column.Ordinal);
                            Cell.CellStyle = workbook.CreateCellStyle();
                            Cell.CellStyle.BorderTop = BorderStyle.Thin;
                            Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            Cell.CellStyle.BorderRight = BorderStyle.Thin;
                            Cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            if (column.ColumnName == "Name")
                            {
                                if (oldvalue == row[column].ToString())
                                    Cell.SetCellValue("-");
                                else
                                    Cell.SetCellValue(row[column].ToString());
                            }
                            else
                                Cell.SetCellValue(row[column].ToString());
                            //if (column.ColumnName == "Venue")
                            //{                             
                            Cell.CellStyle.WrapText = true;
                            // }
                            oldvalue = row[column].ToString();
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

        public static string ExportTrainingInternalExternalWise(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, string tFilter)
        {
            log.Info("TrainingReportsExport/ExportTrainingInternalExternalWise");

            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);
                    XSSFRow dataRow;

                    XSSFFont scFont = CreateFont(workbook);
                    scFont.FontHeightInPoints = 12;
                    scFont.FontName = "Calibri";
                    scFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFFont titleFont = CreateFont(workbook);
                    titleFont.FontHeightInPoints = 14;
                    titleFont.FontName = "Calibri";
                    titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(titleFont);
                    titlecell.CellStyle.WrapText = true;
                    titlerow.Height = 1000;
                    titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                    titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));

                    XSSFRow firstRow = CreateRow(sheet, 1);
                    ICell firstCell = firstRow.CreateCell(0);
                    firstCell.CellStyle = workbook.CreateCellStyle();
                    firstCell.SetCellValue("Trainer Type:  " + tFilter);
                    firstCell.CellStyle.SetFont(scFont);
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 7));

                    firstCell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstCell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstCell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstCell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR2cell = firstRow.CreateCell(1);
                    firstR2cell.CellStyle = workbook.CreateCellStyle();
                    firstR2cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR3cell = firstRow.CreateCell(2);
                    firstR3cell.CellStyle = workbook.CreateCellStyle();
                    firstR3cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR4cell = firstRow.CreateCell(3);
                    firstR4cell.CellStyle = workbook.CreateCellStyle();
                    firstR4cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR5cell = firstRow.CreateCell(4);
                    firstR5cell.CellStyle = workbook.CreateCellStyle();
                    firstR5cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR6cell = firstRow.CreateCell(5);
                    firstR6cell.CellStyle = workbook.CreateCellStyle();
                    firstR6cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR7cell = firstRow.CreateCell(6);
                    firstR7cell.CellStyle = workbook.CreateCellStyle();
                    firstR7cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR8cell = firstRow.CreateCell(7);
                    firstR8cell.CellStyle = workbook.CreateCellStyle();
                    firstR8cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell printDate = firstRow.CreateCell(8);
                    printDate.CellStyle = workbook.CreateCellStyle();
                    printDate.SetCellValue("Print Date:  " + DateTime.Now.Date.ToString("dd-MMM-yyyy"));
                    printDate.CellStyle.SetFont(scFont);
                    printDate.CellStyle.BorderTop = BorderStyle.Thin;
                    printDate.CellStyle.BorderLeft = BorderStyle.Thin;
                    printDate.CellStyle.BorderRight = BorderStyle.Thin;
                    printDate.CellStyle.BorderBottom = BorderStyle.Thin;
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 8, 10));

                    ICell firstR9cell = firstRow.CreateCell(9);
                    firstR9cell.CellStyle = workbook.CreateCellStyle();
                    firstR9cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR10cell = firstRow.CreateCell(10);
                    firstR10cell.CellStyle = workbook.CreateCellStyle();
                    firstR10cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    XSSFRow secondrow = CreateRow(sheet, 2);
                    ICell secondcell = secondrow.CreateCell(0);
                    secondcell.CellStyle = workbook.CreateCellStyle();
                    secondcell.SetCellValue("");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));

                    XSSFRow headerRow = CreateRow(sheet, 3);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);
                    hFont.FontHeightInPoints = 12;
                    hFont.FontName = "Calibri";
                    hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {

                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.SetCellValue(hdr);
                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        headerRow.Height = 500;
                        if (hdr == "Venue" || hdr == "Name")
                            sheet.SetColumnWidth(hdrColIndex, 10000);
                        else
                            sheet.AutoSizeColumn(hdrColIndex);
                        hdrColIndex++;
                    }

                    foreach (DataColumn column in rowData.Columns)
                    {

                        int rowIndex = 4;
                        var oldvalue = string.Empty;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            ICell Cell = dataRow.CreateCell(column.Ordinal);
                            Cell.CellStyle = workbook.CreateCellStyle();
                            Cell.CellStyle.BorderTop = BorderStyle.Thin;
                            Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            Cell.CellStyle.BorderRight = BorderStyle.Thin;
                            Cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            if (column.ColumnName == "Name")
                            {
                                if (oldvalue == row[column].ToString())
                                    Cell.SetCellValue("-");
                                else
                                    Cell.SetCellValue(row[column].ToString());
                            }
                            else
                                Cell.SetCellValue(row[column].ToString());

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

        public static string ExportTrainingTypeWise(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, string tFilter)
        {
            log.Info("TrainingReportsExport/ExportTrainingTypeWise");

            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);
                    XSSFRow dataRow;

                    XSSFFont scFont = CreateFont(workbook);
                    scFont.FontHeightInPoints = 12;
                    scFont.FontName = "Calibri";
                    scFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFFont titleFont = CreateFont(workbook);
                    titleFont.FontHeightInPoints = 14;
                    titleFont.FontName = "Calibri";
                    titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(titleFont);
                    titlecell.CellStyle.WrapText = true;
                    titlerow.Height = 1000;
                    titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                    titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));


                    XSSFRow firstRow = CreateRow(sheet, 1);
                    ICell firstCell = firstRow.CreateCell(0);
                    firstCell.CellStyle = workbook.CreateCellStyle();
                    firstCell.SetCellValue("Training Type:  " + tFilter);
                    firstCell.CellStyle.SetFont(scFont);
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 7));

                    firstCell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstCell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstCell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstCell.CellStyle.BorderBottom = BorderStyle.Thin;


                    ICell firstR2cell = firstRow.CreateCell(1);
                    firstR2cell.CellStyle = workbook.CreateCellStyle();
                    firstR2cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR3cell = firstRow.CreateCell(2);
                    firstR3cell.CellStyle = workbook.CreateCellStyle();
                    firstR3cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR4cell = firstRow.CreateCell(3);
                    firstR4cell.CellStyle = workbook.CreateCellStyle();
                    firstR4cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR5cell = firstRow.CreateCell(4);
                    firstR5cell.CellStyle = workbook.CreateCellStyle();
                    firstR5cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR6cell = firstRow.CreateCell(5);
                    firstR6cell.CellStyle = workbook.CreateCellStyle();
                    firstR6cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR7cell = firstRow.CreateCell(6);
                    firstR7cell.CellStyle = workbook.CreateCellStyle();
                    firstR7cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR8cell = firstRow.CreateCell(7);
                    firstR8cell.CellStyle = workbook.CreateCellStyle();
                    firstR8cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell printDate = firstRow.CreateCell(8);
                    printDate.CellStyle = workbook.CreateCellStyle();
                    printDate.SetCellValue("Print Date:  " + DateTime.Now.Date.ToString("dd-MMM-yyyy"));
                    printDate.CellStyle.SetFont(scFont);
                    printDate.CellStyle.BorderTop = BorderStyle.Thin;
                    printDate.CellStyle.BorderLeft = BorderStyle.Thin;
                    printDate.CellStyle.BorderRight = BorderStyle.Thin;
                    printDate.CellStyle.BorderBottom = BorderStyle.Thin;
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 8, 10));

                    ICell firstR9cell = firstRow.CreateCell(9);
                    firstR9cell.CellStyle = workbook.CreateCellStyle();
                    firstR9cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR10cell = firstRow.CreateCell(10);
                    firstR10cell.CellStyle = workbook.CreateCellStyle();
                    firstR10cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    XSSFRow secondrow = CreateRow(sheet, 2);
                    ICell secondcell = secondrow.CreateCell(0);
                    secondcell.CellStyle = workbook.CreateCellStyle();
                    secondcell.SetCellValue("");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));

                    XSSFRow headerRow = CreateRow(sheet, 3);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);
                    hFont.FontHeightInPoints = 12;
                    hFont.FontName = "Calibri";
                    hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {

                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.SetCellValue(hdr);
                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        headerRow.Height = 500;
                        if (hdr == "Venue" || hdr == "Name")
                            sheet.SetColumnWidth(hdrColIndex, 10000);
                        else
                            sheet.AutoSizeColumn(hdrColIndex);
                        hdrColIndex++;
                    }

                    foreach (DataColumn column in rowData.Columns)
                    {
                        var oldvalue = string.Empty;
                        int rowIndex = 4;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            ICell Cell = dataRow.CreateCell(column.Ordinal);
                            Cell.CellStyle = workbook.CreateCellStyle();
                            Cell.CellStyle.BorderTop = BorderStyle.Thin;
                            Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            Cell.CellStyle.BorderRight = BorderStyle.Thin;
                            Cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            if (column.ColumnName == "Name")
                            {
                                if (oldvalue == row[column].ToString())
                                    Cell.SetCellValue("-");
                                else
                                    Cell.SetCellValue(row[column].ToString());
                            }
                            else
                                Cell.SetCellValue(row[column].ToString());

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

        public static string ExportTrainingProviderWise(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, string tFilter)
        {
            log.Info("TrainingReportsExport/ExportTrainingProviderWise");

            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);
                    XSSFRow dataRow;

                    XSSFFont scFont = CreateFont(workbook);
                    scFont.FontHeightInPoints = 12;
                    scFont.FontName = "Calibri";
                    scFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFFont titleFont = CreateFont(workbook);
                    titleFont.FontHeightInPoints = 14;
                    titleFont.FontName = "Calibri";
                    titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(titleFont);
                    titlecell.CellStyle.WrapText = true;
                    titlerow.Height = 1000;
                    titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                    titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));


                    XSSFRow firstRow = CreateRow(sheet, 1);
                    ICell firstCell = firstRow.CreateCell(0);
                    firstCell.CellStyle = workbook.CreateCellStyle();
                    firstCell.SetCellValue("Training Provider:  " + tFilter);
                    firstCell.CellStyle.SetFont(scFont);
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 7));

                    firstCell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstCell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstCell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstCell.CellStyle.BorderBottom = BorderStyle.Thin;


                    ICell firstR2cell = firstRow.CreateCell(1);
                    firstR2cell.CellStyle = workbook.CreateCellStyle();
                    firstR2cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR2cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR3cell = firstRow.CreateCell(2);
                    firstR3cell.CellStyle = workbook.CreateCellStyle();
                    firstR3cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR3cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR4cell = firstRow.CreateCell(3);
                    firstR4cell.CellStyle = workbook.CreateCellStyle();
                    firstR4cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR4cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR5cell = firstRow.CreateCell(4);
                    firstR5cell.CellStyle = workbook.CreateCellStyle();
                    firstR5cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR5cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR6cell = firstRow.CreateCell(5);
                    firstR6cell.CellStyle = workbook.CreateCellStyle();
                    firstR6cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR7cell = firstRow.CreateCell(6);
                    firstR7cell.CellStyle = workbook.CreateCellStyle();
                    firstR7cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR7cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR8cell = firstRow.CreateCell(7);
                    firstR8cell.CellStyle = workbook.CreateCellStyle();
                    firstR8cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR8cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell printDate = firstRow.CreateCell(8);
                    printDate.CellStyle = workbook.CreateCellStyle();
                    printDate.SetCellValue("Print Date:  " + DateTime.Now.Date.ToString("dd-MMM-yyyy"));
                    printDate.CellStyle.SetFont(scFont);
                    printDate.CellStyle.BorderTop = BorderStyle.Thin;
                    printDate.CellStyle.BorderLeft = BorderStyle.Thin;
                    printDate.CellStyle.BorderRight = BorderStyle.Thin;
                    printDate.CellStyle.BorderBottom = BorderStyle.Thin;
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 8, 10));

                    ICell firstR9cell = firstRow.CreateCell(9);
                    firstR9cell.CellStyle = workbook.CreateCellStyle();
                    firstR9cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR10cell = firstRow.CreateCell(10);
                    firstR10cell.CellStyle = workbook.CreateCellStyle();
                    firstR10cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR10cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    XSSFRow secondrow = CreateRow(sheet, 2);
                    ICell secondcell = secondrow.CreateCell(0);
                    secondcell.CellStyle = workbook.CreateCellStyle();
                    secondcell.SetCellValue("");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));

                    XSSFRow headerRow = CreateRow(sheet, 3);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);
                    hFont.FontHeightInPoints = 12;
                    hFont.FontName = "Calibri";
                    hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {
                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.SetCellValue(hdr);
                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        headerRow.Height = 500;
                        if (hdr == "Venue" || hdr == "Name")
                            sheet.SetColumnWidth(hdrColIndex, 10000);
                        else
                            sheet.AutoSizeColumn(hdrColIndex);
                        hdrColIndex++;
                    }

                    foreach (DataColumn column in rowData.Columns)
                    {
                        var oldvalue = string.Empty;
                        int rowIndex = 4;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            ICell Cell = dataRow.CreateCell(column.Ordinal);
                            Cell.CellStyle = workbook.CreateCellStyle();
                            Cell.CellStyle.BorderTop = BorderStyle.Thin;
                            Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            Cell.CellStyle.BorderRight = BorderStyle.Thin;
                            Cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            if (column.ColumnName == "Name")
                            {
                                if (oldvalue == row[column].ToString())
                                    Cell.SetCellValue("-");
                                else
                                    Cell.SetCellValue(row[column].ToString());
                            }
                            else
                                Cell.SetCellValue(row[column].ToString());
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
    }
}
