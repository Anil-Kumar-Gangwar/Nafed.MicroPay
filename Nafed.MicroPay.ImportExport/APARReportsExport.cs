using System;
using System.IO;
using System.Data;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using NPOI.SS.Util;

namespace Nafed.MicroPay.ImportExport
{
    public class APARReportsExport : BaseExcel
    {
        private APARReportsExport()
        {
        }

        public static string ExportToExcel(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath)
        {
            log.Info("APARReportsExport/ExportToExcel");

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

                    int hdrColIndex = 0;

                    foreach (var hdr in headers)
                    {
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                        //hFont.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
                       var cell = headerRow.CreateCell(hdrColIndex);
                            cell.SetCellValue(hdr);
                            cell.CellStyle = workbook.CreateCellStyle();
                            
                            cell.CellStyle.SetFont(hFont);
                           // cell.CellStyle.ShrinkToFit = true;
                            cell.CellStyle.Alignment = HorizontalAlignment.Center;
                            
                            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            //cell.CellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                           // cell.CellStyle.BorderTop = BorderStyle.Thin;
                           // cell.CellStyle.BorderLeft = BorderStyle.Thin;
                           // cell.CellStyle.BorderRight = BorderStyle.Thin;
                           // cell.CellStyle.BorderBottom = BorderStyle.Thin;
                           // cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                           
                          headerRow.Height = 500;
                            hdrColIndex++;
                        }                    
                  

                    foreach (DataColumn column in rowData.Columns)
                    {
                        int rowIndex = 1;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                            ICell Cell = dataRow.CreateCell(column.Ordinal);
                            Cell.CellStyle = workbook.CreateCellStyle();
                           

                            if ( column.ColumnName.ToLower() == "apar status")
                            {
                                switch (row[column].ToString())
                                {
                                    case "1":
                                        Cell.SetCellValue("Save");
                                        break;
                                    case "2":
                                        Cell.SetCellValue("Submitted");
                                        break;
                                    case "3":
                                        Cell.SetCellValue("Saved by Reporting Officer");
                                        break;
                                    case "4":
                                        Cell.SetCellValue("Reviewed by Reporting Officer");
                                        break;
                                    case "5":
                                        Cell.SetCellValue("Saved by Reviewer Officer");
                                        break;
                                    case "6":
                                        Cell.SetCellValue("Reviewed by Reviewer Officer");
                                        break;
                                    case "7":
                                        Cell.SetCellValue("Saved by Acceptance Authority");
                                        break;
                                    case "8":
                                        Cell.SetCellValue("Approved");
                                        break;
                                    default:
                                        break;
                                }
                                
                            }
                            else
                            {
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


        public static string ExportApprovalListToExcel(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath,string tFilter)
        {
            log.Info("APARReportsExport/ExportApprovalListToExcel");

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
                    firstCell.SetCellValue(tFilter);
                    firstCell.CellStyle.SetFont(scFont);                

                    firstCell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstCell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstCell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstCell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR2cell = firstRow.CreateCell(1);
                    firstR2cell.CellStyle = workbook.CreateCellStyle();
                    firstR2cell.CellStyle.BorderTop = BorderStyle.Thin;
                   
                    firstR2cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR3cell = firstRow.CreateCell(2);
                    firstR3cell.CellStyle = workbook.CreateCellStyle();
                    firstR3cell.CellStyle.BorderTop = BorderStyle.Thin;
                 
                    firstR3cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR4cell = firstRow.CreateCell(3);
                    firstR4cell.CellStyle = workbook.CreateCellStyle();
                    firstR4cell.CellStyle.BorderTop = BorderStyle.Thin;
                 
                    firstR4cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR5cell = firstRow.CreateCell(4);
                    firstR5cell.CellStyle = workbook.CreateCellStyle();
                    firstR5cell.CellStyle.BorderTop = BorderStyle.Thin;
               
                    firstR5cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR6cell = firstRow.CreateCell(5);
                    firstR6cell.CellStyle = workbook.CreateCellStyle();                  
                    firstR6cell.CellStyle.BorderTop = BorderStyle.Thin;                  
                    firstR6cell.CellStyle.BorderBottom = BorderStyle.Thin;
                    firstR6cell.CellStyle.BorderRight = BorderStyle.Thin;
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 6));

                    ICell firstR7cell = firstRow.CreateCell(6);
                    firstR7cell.CellStyle = workbook.CreateCellStyle();
                    firstR7cell.CellStyle.BorderTop = BorderStyle.Thin;

                    firstR7cell.CellStyle.BorderBottom = BorderStyle.Thin;
                    firstR7cell.SetCellValue("Print Date:  " + DateTime.Now.Date.ToString("dd-MMM-yyyy"));
                    firstR7cell.CellStyle.SetFont(scFont);

                    ICell firstR8cell = firstRow.CreateCell(7);
                    firstR8cell.CellStyle = workbook.CreateCellStyle();
                  
                    firstR8cell.CellStyle.BorderTop = BorderStyle.Thin;

                    firstR8cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR9cell = firstRow.CreateCell(8);
                    firstR9cell.CellStyle = workbook.CreateCellStyle();
                    firstR9cell.CellStyle.BorderTop = BorderStyle.Thin;

                    firstR9cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR10cell = firstRow.CreateCell(9);
                    firstR10cell.CellStyle = workbook.CreateCellStyle();
                    firstR10cell.CellStyle.BorderTop = BorderStyle.Thin;  
                    firstR10cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR11cell = firstRow.CreateCell(10);
                    firstR11cell.CellStyle = workbook.CreateCellStyle();
                    firstR11cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR11cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR11cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR11cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 7, 10));

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
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                        //hFont.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.SetCellValue(hdr);
                        cell.CellStyle = workbook.CreateCellStyle();

                        cell.CellStyle.SetFont(hFont);
                        // cell.CellStyle.ShrinkToFit = true;
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;

                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        cell.CellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;

                        headerRow.Height = 500;
                         if (hdr == "S.No.")
                            sheet.AutoSizeColumn(hdrColIndex);
                        else
                            sheet.SetColumnWidth(hdrColIndex, 7000);

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

                            // Cell.CellStyle.BorderTop = BorderStyle.Thin;
                            //   Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            Cell.CellStyle.BorderRight = BorderStyle.Thin;
                            Cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            if (column.ColumnName.ToLower() == "apar status")
                            {
                                switch (row[column].ToString())
                                {
                                    case "1":
                                        Cell.SetCellValue("Save");
                                        break;
                                    case "2":
                                        Cell.SetCellValue("Submitted");
                                        break;
                                    case "3":
                                        Cell.SetCellValue("Saved by Reporting Officer");
                                        break;
                                    case "4":
                                        Cell.SetCellValue("Reviewed by Reporting Officer");
                                        break;
                                    case "5":
                                        Cell.SetCellValue("Saved by Reviewer Officer");
                                        break;
                                    case "6":
                                        Cell.SetCellValue("Reviewed by Reviewer Officer");
                                        break;
                                    case "7":
                                        Cell.SetCellValue("Saved by Acceptance Authority");
                                        break;
                                    case "8":
                                        Cell.SetCellValue("Approved");
                                        break;
                                    default:
                                        break;
                                }

                            }
                            else
                            {
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
    }
}
