using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.ImportExport
{
    public class DATDSmanualEntryForm : BaseExcel
    {

        private DATDSmanualEntryForm()
        {

        }

        public static string ExportToExcel(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath)
        {
            log.Info("DATDSmanualEntryForm/ExportToExcel");

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

                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {

                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        var cell = headerRow.CreateCell(hdrColIndex);
                        cell.SetCellValue(hdr);
                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.WrapText = true;

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

                        if (hdr == "Employee Code")
                            sheet.SetColumnWidth(hdrColIndex, 2500);
                        else if (hdr == "Employee Name")
                            sheet.SetColumnWidth(hdrColIndex, 9000);
                        else if (hdr == "Branch Name")
                            sheet.SetColumnWidth(hdrColIndex, 6000);
                        else if (hdr == "Designation Name")
                            sheet.SetColumnWidth(hdrColIndex, 9000);
                        else if (hdr == "Month")
                            sheet.SetColumnWidth(hdrColIndex, 2000);
                        else if (hdr == "Year")
                            sheet.SetColumnWidth(hdrColIndex, 2000);
                        else if (hdr == "DA")
                            sheet.SetColumnWidth(hdrColIndex, 3500);
                        else if (hdr == "TDS")
                            sheet.SetColumnWidth(hdrColIndex, 3500);
                        else if (hdr == "NetPay")
                            sheet.SetColumnWidth(hdrColIndex, 3500);
                       

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
                            }
                            else
                            {
                                if (!row[column].ToString().Trim().Equals(""))
                                {
                                    ICell Cell = dataRow.CreateCell(column.Ordinal);
                                    Cell.CellStyle = workbook.CreateCellStyle();

                                    if (column.Ordinal == 0)
                                        Cell.CellStyle.Alignment = HorizontalAlignment.Center;

                                    if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(decimal)))
                                        Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                    else if (rowData.Columns[column.Ordinal].DataType.Equals(typeof(int)))
                                        Cell.SetCellValue((int)row[column]);
                                    else
                                    {
                                        if (column.ColumnName == "Employee Code")
                                        {
                                                Cell.SetCellValue(row[column].ToString());
                                        }
                                        else if (column.ColumnName.ToLower() != "emp code")
                                        {
                                            int numVal;
                                            if (int.TryParse(row[column].ToString(), out numVal))
                                                Cell.SetCellValue(numVal);
                                            else
                                                Cell.SetCellValue(row[column].ToString());
                                        }
                                    }
                                }
                            }
                            rowIndex++;
                        }
                        //  if (!IsImageCol) { sheet.AutoSizeColumn(column.Ordinal); }
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
