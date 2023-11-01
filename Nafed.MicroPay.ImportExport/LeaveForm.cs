using System;
using System.IO;
using System.Data;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using NPOI.SS.Util;

namespace Nafed.MicroPay.ImportExport
{
    public class LeaveForm : BaseExcel
    {
        private LeaveForm()
        {
        }

        public static string ExportToExcel(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, string Name, string designationname, string employeeCode)
        {
            log.Info("LeaveForm/ExportToExcel");

            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);
                    XSSFRow dataRow;
                    XSSFRow headerRow = CreateRow(sheet, 0);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);
                    //  var font = workbook.CreateFont();


                    hFont.FontHeightInPoints = 11;
                    hFont.FontName = "Calibri";
                    hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    //hFont.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;


                    ICell Cell2 = headerRow.CreateCell(0);
                    Cell2.SetCellValue("Employee Code : ");
                    Cell2 = headerRow.CreateCell(1);
                    Cell2.SetCellValue(employeeCode);
                    Cell2.CellStyle = workbook.CreateCellStyle();
                    Cell2.CellStyle.SetFont(hFont);

                    headerRow = CreateRow(sheet, 1);
                    ICell Cell3 = headerRow.CreateCell(0);
                    Cell3.SetCellValue("Employee Name : ");
                    Cell3 = headerRow.CreateCell(1);
                    Cell3.SetCellValue(Name);
                    Cell3.CellStyle = workbook.CreateCellStyle();
                    Cell3.CellStyle.SetFont(hFont);

                    headerRow = CreateRow(sheet, 2);
                    ICell Cell4 = headerRow.CreateCell(0);
                    Cell4.SetCellValue("Designation : ");
                    Cell4 = headerRow.CreateCell(1);
                    Cell4.SetCellValue(designationname);
                    Cell4.CellStyle = workbook.CreateCellStyle();
                    Cell4.CellStyle.SetFont(hFont);

                    headerRow = CreateRow(sheet, 3);

                    int hdrColIndex = 0;

                    foreach (var hdr in headers)
                    {
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                        //hFont.Color = NPOI.HSSF.Util.HSSFColor.White.Index;

                        var cell = headerRow.CreateCell(hdrColIndex);
                        if (hdr.ToLower() == "createdon")
                        {
                            cell.SetCellValue("Applied On");
                        }
                        else if (hdr.ToLower() == "datefrom")
                        {
                            cell.SetCellValue("From Date");
                        }
                        else if (hdr.ToLower() == "dateto")
                        {
                            cell.SetCellValue("To Date");
                        }
                        else if (hdr.ToLower() == "statusname")
                        {
                            cell.SetCellValue("Status");
                        }
                        else if (hdr.ToLower() == "leavetype")
                        {
                            cell.SetCellValue("Leave Type");
                        }
                        else
                        {
                            cell.SetCellValue(hdr);

                        }
                        cell.CellStyle = workbook.CreateCellStyle();

                        cell.CellStyle.SetFont(hFont);
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        cell.CellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightYellow.Index;
                        cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                        headerRow.Height = 500;
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

                            //dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                            //dataRow.CreateCell(column.Ordinal).SetCellValue(row[column]);
                            ICell Cell = dataRow.CreateCell(column.Ordinal);
                            if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(decimal)))
                                Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                            else if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(int)))
                                Cell.SetCellValue((int)row[column]);
                            else
                            {

                                if (column.ColumnName.ToLower() == "createdon" || column.ColumnName.ToLower() == "datefrom" || column.ColumnName.ToLower() == "dateto")
                                {
                                    var iDate = Convert.ToDateTime(row[column]).ToString("dd/MMM/yyyy");
                                    Cell.SetCellValue(iDate);
                                }
                                else
                                {
                                    Cell.SetCellValue(row[column].ToString());
                                }
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

        public static string ExportToExcel(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, string tFilter)
        {
            log.Info("LeaveForm/ExportToExcel");

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
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 8));

                    XSSFRow firstRow = CreateRow(sheet, 1);
                    ICell firstCell = firstRow.CreateCell(0);
                    firstCell.CellStyle = workbook.CreateCellStyle();
                    firstCell.SetCellValue(tFilter);
                    firstCell.CellStyle.SetFont(scFont);
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 5));

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
                    firstR7cell.SetCellValue("Print Date:  " + DateTime.Now.Date.ToString("dd-MMM-yyyy"));
                    firstR7cell.CellStyle.SetFont(scFont);
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

                    ICell firstR9cell = firstRow.CreateCell(8);
                    firstR9cell.CellStyle = workbook.CreateCellStyle();
                    firstR9cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR9cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 6, 8));

                    XSSFRow secondrow = CreateRow(sheet, 2);
                    ICell secondcell = secondrow.CreateCell(0);
                    secondcell.CellStyle = workbook.CreateCellStyle();
                    secondcell.SetCellValue("");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 8));

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
                        if (hdr == "Name")
                        {
                            sheet.SetColumnWidth(hdrColIndex, 10000);
                            cell.CellStyle.WrapText = true;
                        }
                        else if (hdr == "S.No.")
                            sheet.AutoSizeColumn(hdrColIndex);
                        else
                            sheet.SetColumnWidth(hdrColIndex, 5000);

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
                            if (column.Ordinal > 2)
                            {
                                Cell.CellStyle.Alignment = HorizontalAlignment.Right;
                                if (row[column].ToString() != "")
                                    Cell.SetCellValue(Convert.ToDouble(row[column].ToString()));
                                Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

                                if (column.Ordinal == 6)  //---- Add Sum Formula for Gross Amount
                                {
                                    var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(4));
                                    var fCellRefAsString = fromCellRef.FormatAsString();
                                    var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(5));
                                    var toCellRefAsString = toCellRef.FormatAsString();
                                    Cell.SetCellFormula($"({fCellRefAsString}+{toCellRefAsString})");
                                }
                                if (column.Ordinal == 8)  //---- Add Sum Formula for Net Amount
                                {
                                    var fromCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(6));
                                    var fCellRefAsString = fromCellRef.FormatAsString();
                                    var toCellRef = new CellReference(sheet.GetRow(rowIndex).GetCell(7));
                                    var toCellRefAsString = toCellRef.FormatAsString();
                                    Cell.SetCellFormula($"({fCellRefAsString}-{toCellRefAsString})");
                                }
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

        public static string ExportToExcelNew(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, string tFilter)
        {
            log.Info("LeaveForm/ExportToExcel");

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
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110014");
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 14));

                    XSSFRow firstRow = CreateRow(sheet, 1);
                    ICell firstCell = firstRow.CreateCell(0);
                    firstCell.CellStyle = workbook.CreateCellStyle();
                    firstCell.SetCellValue(tFilter);
                    firstCell.CellStyle.SetFont(scFont);
                    firstCell.CellStyle.Alignment = HorizontalAlignment.Center;
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 14));

                    XSSFRow secondrow = CreateRow(sheet, 2);
                    ICell secondcell = secondrow.CreateCell(0);
                    secondcell.CellStyle = workbook.CreateCellStyle();
                    secondcell.SetCellValue("");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 14));

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
                        if(hdr.Equals("Basic") || hdr.Equals("TDS"))
                        sheet.SetColumnWidth(hdrColIndex, 5000);
                        else
                            sheet.AutoSizeColumn(hdrColIndex);
                        cell.CellStyle.WrapText = true;
                        hdrColIndex++;
                    }
                    int lastOrdinal = rowData.Columns.Count - 2; // 
                    int rowCount = 4 + rowData.Rows.Count;
                    double basic = 0, da = 0, gross = 0, dalatest = 0, grossleatest = 0, diff = 0, tds = 0, net = 0;
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
                            if (column.Ordinal > 3 && column.Ordinal < lastOrdinal)
                            {
                                Cell.CellStyle.Alignment = HorizontalAlignment.Right;
                                if (row[column].ToString() != "")
                                    Cell.SetCellValue(Convert.ToDouble(row[column].ToString()));
                                Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                                #region Sum of all amount columns
                                switch (column.Ordinal)
                                {
                                    case 8:
                                        {
                                            if (row[column].ToString() != "")
                                                basic += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                    case 9:
                                        {
                                            if (row[column].ToString() != "")
                                                da += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                    case 10:
                                        {
                                            if (row[column].ToString() != "")
                                                gross += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                    case 11:
                                        {
                                            if (row[column].ToString() != "")
                                                tds += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                    case 12:
                                        {
                                            if (row[column].ToString() != "")
                                                net += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                }
                                #endregion
                            }
                            else
                                Cell.SetCellValue(row[column].ToString());

                            rowIndex++;
                        }
                    }

                    XSSFRow totalRow = CreateRow(sheet, rowData.Rows.Count + 4);
                    ICell Celltotal = totalRow.CreateCell(2);
                    Celltotal.CellStyle = workbook.CreateCellStyle();
                    Celltotal.CellStyle.Alignment = HorizontalAlignment.Left;
                    Celltotal.SetCellValue("TOTAL");
                    Celltotal.CellStyle.SetFont(hFont);

                    ICell Cellbasic = totalRow.CreateCell(8);
                    Cellbasic.CellStyle = workbook.CreateCellStyle();
                    Cellbasic.CellStyle.Alignment = HorizontalAlignment.Right;
                    Cellbasic.SetCellValue(basic);
                    Cellbasic.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    Cellbasic.CellStyle.SetFont(hFont);

                    ICell Cellda = totalRow.CreateCell(9);
                    Cellda.CellStyle = workbook.CreateCellStyle();
                    Cellda.CellStyle.Alignment = HorizontalAlignment.Right;
                    Cellda.SetCellValue(da);
                    Cellda.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    Cellda.CellStyle.SetFont(hFont);                  

                    ICell Cellgross = totalRow.CreateCell(10);
                    Cellgross.CellStyle = workbook.CreateCellStyle();
                    Cellgross.CellStyle.Alignment = HorizontalAlignment.Right;
                    Cellgross.SetCellValue(gross);
                    Cellgross.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    Cellgross.CellStyle.SetFont(hFont);

                    ICell Celltds = totalRow.CreateCell(11);
                    Celltds.CellStyle = workbook.CreateCellStyle();
                    Celltds.CellStyle.Alignment = HorizontalAlignment.Right;
                    Celltds.SetCellValue(tds);
                    Celltds.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    Celltds.CellStyle.SetFont(hFont);

                    ICell Cellnet = totalRow.CreateCell(12);
                    Cellnet.CellStyle = workbook.CreateCellStyle();
                    Cellnet.CellStyle.Alignment = HorizontalAlignment.Right;
                    Cellnet.SetCellValue(net);
                    Cellnet.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    Cellnet.CellStyle.SetFont(hFont);
                
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


        public static string ExportToExcelDALeaveEncashment(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, string tFilter)
        {
            log.Info("LeaveForm/ExportToExcelDALeaveEncashmentLog");

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
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110014");
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 14));

                    XSSFRow firstRow = CreateRow(sheet, 1);
                    ICell firstCell = firstRow.CreateCell(0);
                    firstCell.CellStyle = workbook.CreateCellStyle();
                    firstCell.SetCellValue(tFilter);
                    firstCell.CellStyle.SetFont(scFont);
                    firstCell.CellStyle.Alignment = HorizontalAlignment.Center;
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 14));

                    XSSFRow secondrow = CreateRow(sheet, 2);
                    ICell secondcell = secondrow.CreateCell(0);
                    secondcell.CellStyle = workbook.CreateCellStyle();
                    secondcell.SetCellValue("");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 14));

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
                        if (hdr.Equals("Basic") || hdr.Equals("TDS"))
                            sheet.SetColumnWidth(hdrColIndex, 5000);
                        else
                            sheet.AutoSizeColumn(hdrColIndex);
                        cell.CellStyle.WrapText = true;
                        hdrColIndex++;
                    }
                    int lastOrdinal = rowData.Columns.Count - 2; // 
                    int rowCount = 4 + rowData.Rows.Count;
                    double basic = 0, da = 0, gross = 0, dalatest = 0, grossleatest = 0, diff = 0, tds = 0, net = 0;
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
                            if (column.Ordinal > 3 && column.Ordinal < lastOrdinal)
                            {
                                Cell.CellStyle.Alignment = HorizontalAlignment.Right;
                                if (row[column].ToString() != "")
                                    Cell.SetCellValue(Convert.ToDouble(row[column].ToString()));
                                Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                                #region Sum of all amount columns
                                switch (column.Ordinal)
                                {
                                    case 8:
                                        {
                                            if (row[column].ToString() != "")
                                                basic += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                    case 9:
                                        {
                                            if (row[column].ToString() != "")
                                                da += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                    case 10:
                                        {
                                            if (row[column].ToString() != "")
                                                gross += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                    case 11:
                                        {
                                            if (row[column].ToString() != "")
                                                dalatest += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                    case 12:
                                        {
                                            if (row[column].ToString() != "")
                                                grossleatest += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                    case 13:
                                        {
                                            if (row[column].ToString() != "")
                                                diff += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                    case 14:
                                        {
                                            if (row[column].ToString() != "")
                                                tds += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                    case 15:
                                        {
                                            if (row[column].ToString() != "")
                                                net += Convert.ToDouble(row[column].ToString());
                                            break;
                                        }
                                }
                                #endregion
                            }
                            else
                                Cell.SetCellValue(row[column].ToString());

                            rowIndex++;
                        }
                    }

                    XSSFRow totalRow = CreateRow(sheet, rowData.Rows.Count + 4);
                    ICell Celltotal = totalRow.CreateCell(2);
                    Celltotal.CellStyle = workbook.CreateCellStyle();
                    Celltotal.CellStyle.Alignment = HorizontalAlignment.Left;
                    Celltotal.SetCellValue("TOTAL");
                    Celltotal.CellStyle.SetFont(hFont);

                    ICell Cellbasic = totalRow.CreateCell(8);
                    Cellbasic.CellStyle = workbook.CreateCellStyle();
                    Cellbasic.CellStyle.Alignment = HorizontalAlignment.Right;
                    Cellbasic.SetCellValue(basic);
                    Cellbasic.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    Cellbasic.CellStyle.SetFont(hFont);

                    ICell Cellda = totalRow.CreateCell(9);
                    Cellda.CellStyle = workbook.CreateCellStyle();
                    Cellda.CellStyle.Alignment = HorizontalAlignment.Right;
                    Cellda.SetCellValue(da);
                    Cellda.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    Cellda.CellStyle.SetFont(hFont);

                    ICell Cellgross = totalRow.CreateCell(10);
                    Cellgross.CellStyle = workbook.CreateCellStyle();
                    Cellgross.CellStyle.Alignment = HorizontalAlignment.Right;
                    Cellgross.SetCellValue(gross);
                    Cellgross.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    Cellgross.CellStyle.SetFont(hFont);

                    ICell CelldaLatest = totalRow.CreateCell(11);
                    CelldaLatest.CellStyle = workbook.CreateCellStyle();
                    CelldaLatest.CellStyle.Alignment = HorizontalAlignment.Right;
                    CelldaLatest.SetCellValue(dalatest);
                    CelldaLatest.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    CelldaLatest.CellStyle.SetFont(hFont);

                    ICell CellgrossLatest = totalRow.CreateCell(12);
                    CellgrossLatest.CellStyle = workbook.CreateCellStyle();
                    CellgrossLatest.CellStyle.Alignment = HorizontalAlignment.Right;
                    CellgrossLatest.SetCellValue(grossleatest);
                    CellgrossLatest.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    CellgrossLatest.CellStyle.SetFont(hFont);


                    ICell Celldiff = totalRow.CreateCell(13);
                    Celldiff.CellStyle = workbook.CreateCellStyle();
                    Celldiff.CellStyle.Alignment = HorizontalAlignment.Right;
                    Celldiff.SetCellValue(diff);
                    Celldiff.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    Celldiff.CellStyle.SetFont(hFont);

                    ICell Celltds = totalRow.CreateCell(14);
                    Celltds.CellStyle = workbook.CreateCellStyle();
                    Celltds.CellStyle.Alignment = HorizontalAlignment.Right;
                    Celltds.SetCellValue(tds);
                    Celltds.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    Celltds.CellStyle.SetFont(hFont);

                    ICell Cellnet = totalRow.CreateCell(15);
                    Cellnet.CellStyle = workbook.CreateCellStyle();
                    Cellnet.CellStyle.Alignment = HorizontalAlignment.Right;
                    Cellnet.SetCellValue(net);
                    Cellnet.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
                    Cellnet.CellStyle.SetFont(hFont);
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
