using System;
using System.IO;
using System.Data;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace Nafed.MicroPay.ImportExport
{
   public  class AttendanceForm : BaseExcel
    {
        private AttendanceForm()
        {
        }

        public static string ExportToExcel(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath)
        {
            log.Info("AtthendanceForm/ExportToExcel");

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
                        //XSSFCell cell = (XSSFCell)headerRow.CreateCell(hdrColIndex);
                        //cell.SetCellValue(hdr);
                        //cell.CellStyle = workbook.CreateCellStyle();
                        //hFont.IsBold = true;
                        //cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        //cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        //cell.CellStyle.SetFont(hFont);
                        hdrColIndex++;
                    }

                   
                    foreach (DataColumn column in rowData.Columns)
                    {
                        int rowIndex = 1;
                        bool IsImageCol = column.ColumnName.ToLower().Equals("image") ? true : false;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                            if (IsImageCol)
                            {
                                dataRow.Height = 3800;
                                sheet.SetColumnWidth(column.Ordinal, 9000);
                              //  AddImage(workbook, sheet, rowIndex, column.Ordinal, sDefaultOrderImage, sItemImageUNCPath, row[column].ToString());
                            }
                            else
                            {
                                //dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                                if (!row[column].ToString().Trim().Equals(""))
                                {      //dataRow.CreateCell(column.Ordinal).SetCellValue(row[column]);
                                    ICell Cell = dataRow.CreateCell(column.Ordinal);
                                    if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(decimal)))
                                        Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                    else if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(int)))
                                        Cell.SetCellValue((int)row[column]);
                                    else
                                    {
                                        if (column.ColumnName.ToLower() != "empcode")
                                        {
                                            int numVal;
                                            if (int.TryParse(row[column].ToString(), out numVal))
                                                Cell.SetCellValue(numVal);
                                            else
                                                Cell.SetCellValue(row[column].ToString());
                                        }
                                        if (column.ColumnName.ToLower() == "indate")
                                        {
                                            var iDate =Convert.ToDateTime( row[column]).ToString("dd/MMM/yyyy");

                                          //  var fff = ((DateTime)row[column]).ToString("dd/MM/yyyy");
                                              Cell.SetCellValue(iDate);
                                        }
                                        else
                                        {
                                            Cell.SetCellValue(row[column].ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    ICell Cell = dataRow.CreateCell(column.Ordinal);
                                    if (column.ColumnName.ToLower() == "intime" || column.ColumnName.ToLower() == "outtime")
                                    {

                                       // Cell.SetCellType(CellType.String);
                                        Cell.SetCellValue(row[column].ToString());
                                    }
                                }
                            }
                            rowIndex++;
                        }
                        if (!IsImageCol) { sheet.AutoSizeColumn(column.Ordinal); }
                    }

                    #region Guideline sheet

                    XSSFSheet guidelineSheet = (XSSFSheet)workbook.CreateSheet("Guidelines");
                    XSSFRow Row = CreateRow(guidelineSheet, 0);
                    ICell Cell1 = Row.CreateCell(0);
                    Cell1.SetCellValue("1.");
                    Cell1 = Row.CreateCell(1);

                    Cell1.SetCellValue($"Indate should be in dd/MM/yyyy format.");

                    Row = CreateRow(guidelineSheet, 1);
                    Cell1 = Row.CreateCell(0);
                    Cell1.SetCellValue("2.");
                    Cell1 = Row.CreateCell(1);
                    Cell1.SetCellValue("InTime & OutTime should be in 24 hours format.");

                    Row = CreateRow(guidelineSheet, 2);
                    Cell1 = Row.CreateCell(0);
                    Cell1.SetCellValue("3.");
                    Cell1 = Row.CreateCell(1);
                    Cell1.SetCellValue("Do not change value in cell marked with gray color.");

                    Row = CreateRow(guidelineSheet, 3);
                    Cell1 = Row.CreateCell(0);
                    Cell1.SetCellValue("4.");
                    Cell1 = Row.CreateCell(1);
                    Cell1.SetCellValue("Remarks should not be more than 200 characters including white space.");

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

        public static string ExportToExcelManualData(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath)
        {
            log.Info("AtthendanceForm/ExportToExcel");

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
                    sheet.DefaultColumnWidth = 24;

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
                        int rowIndex = 1;
                        foreach (DataRow row in rowData.Rows)
                        {
                            if (column.Ordinal == 0)
                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                            else
                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);


                            if (!row[column].ToString().Trim().Equals(""))
                            {      //dataRow.CreateCell(column.Ordinal).SetCellValue(row[column]);
                                ICell Cell = dataRow.CreateCell(column.Ordinal);


                                if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(decimal)))
                                    Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                else if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(int)))

                                    Cell.SetCellValue((int)row[column]);
                                else
                                {
                                    if (column.ColumnName.ToLower() != "empcode")
                                    {
                                        int numVal;
                                        if (int.TryParse(row[column].ToString(), out numVal))
                                            Cell.SetCellValue(numVal);
                                        else
                                            Cell.SetCellValue(row[column].ToString());
                                    }
                                    else
                                    {
                                        Cell.SetCellValue(row[column].ToString());
                                    }
                                }
                            }
                            else
                            {
                                ICell Cell = dataRow.CreateCell(column.Ordinal);
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
