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
    public class PfBalanceManualEntryForm : BaseExcel
    {
        private PfBalanceManualEntryForm()
        {

        }
        public static string ExportToExcel(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath)
        {
            log.Info("PfBalanceManualEntryForm/ExportToExcel");

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

                        if (hdr == "#")
                            sheet.SetColumnWidth(hdrColIndex, 1500);
                        else if (hdr == "Employee Name")
                            sheet.SetColumnWidth(hdrColIndex, 9000);
                        else if (hdr == "Branch Name")
                            sheet.SetColumnWidth(hdrColIndex, 6000);
                        else if (hdr == "Employee Code")
                            sheet.SetColumnWidth(hdrColIndex, 2500);
                        else if (hdr == "Employee PF Contribution")
                            sheet.SetColumnWidth(hdrColIndex, 3200);
                        else if (hdr == "Employer PF Contribution")
                            sheet.SetColumnWidth(hdrColIndex, 3200);
                        else if (hdr == "Withdrawl To Employee Contribution")
                            sheet.SetColumnWidth(hdrColIndex, 3500);
                        else if (hdr == "Withdrawl To Employer Contribution")
                            sheet.SetColumnWidth(hdrColIndex, 3500);
                        else if(hdr== "Addition To Employee Contribution")
                            sheet.SetColumnWidth(hdrColIndex, 4100);
                        else if (hdr == "Addition To Employer Contribution")
                            sheet.SetColumnWidth(hdrColIndex, 4100);
                        else if(hdr== "Interest Withdrawl To Employee Contribution")
                            sheet.SetColumnWidth(hdrColIndex, 4300);
                        else if (hdr == "Interest Withdrawl To Employer Contribution")
                            sheet.SetColumnWidth(hdrColIndex, 4300);

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

                                    if (column.Ordinal==0)
                                        Cell.CellStyle.Alignment = HorizontalAlignment.Center;

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
                                    }
                                }
                            }
                            rowIndex++;
                        }
                        //  if (!IsImageCol) { sheet.AutoSizeColumn(column.Ordinal); }
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

    }
}
