using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Nafed.MicroPay.ImportExport
{
    public class SalaryConfigurationExport : BaseExcel
    {
        private SalaryConfigurationExport()
        {

        }

        public static string BranchSalaryConfiguration(IEnumerable<string> headers, DataTable table, string sSheetName, string sFullPath)
        {
            log.Info($"SalaryConfigurationExport/BranchSalaryConfiguration/");
            try
            {
                int columnCount = headers.ToList().Count;
                XSSFWorkbook workbook = CreateWookBook();

                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);

                    #region   ======Report Header Label Value  === === 

                    //XSSFRow rHeader1 = CreateRow(sheet, 0);

                    //var cellRH1_0 = rHeader1.CreateCell(0);
                    //cellRH1_0.SetCellValue("Branch Code:");

                    //var cellRH1_1 = rHeader1.CreateCell(1);
                    //cellRH1_1.SetCellValue(branchCode);
                    
                    //XSSFRow rHeader2 = CreateRow(sheet, 1);

                    //var cellRH2_0 = rHeader2.CreateCell(0);
                    //cellRH2_0.SetCellValue("Branch Name :");

                    //var cellRH2_1 = rHeader2.CreateCell(1);
                    //cellRH2_1.SetCellValue(branchName);

                    #endregion

                    XSSFRow dataRow;
                    XSSFRow headerRow = CreateRow(sheet, 1);
                    // Writing Header Row
                    XSSFFont hFont = CreateFont(workbook);

                    int hdrColIndex = 0;
                    foreach (var hdr in headers)
                    {
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        var cell = headerRow.CreateCell(hdrColIndex);
                        if (hdrColIndex == 1)
                            cell.SetCellType(CellType.String);
                            cell.SetCellValue(hdr);

                       
                        cell.CellStyle = workbook.CreateCellStyle();

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

                    for (int jj = 0; jj < columnCount; jj++)
                    {
                        int rowIndex = 2;

                        if (jj == 0)
                            dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                        else
                            dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                        var cell = dataRow.CreateCell(jj);

                            int numVal;
                            if (int.TryParse(jj.ToString(), out numVal))
                                cell.SetCellValue(numVal+1);
                            else
                                cell.SetCellValue((jj+1));
                       
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
                        int rowIndex = 3;
                        foreach (DataRow row in table.Rows)
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
                                    Cell.CellStyle.FillBackgroundColor = HSSFColor.Red.Index;
                                    if (table.Columns[column.Ordinal].DataType.Equals(typeof(decimal)))
                                        Cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                    else if (table.Columns[column.Ordinal].DataType.Equals(typeof(int)))
                                        Cell.SetCellValue((int)row[column]);
                                    else
                                    {
                                        int numVal;
                                        if (column.Ordinal != 1 && column.Ordinal != 3 && int.TryParse(row[column].ToString(), out numVal))
                                            Cell.SetCellValue(numVal);
                                        else if (column.Ordinal == 8) //===  E_Basic column
                                        {
                                          //  var f_val = row[column];
                                            Cell.SetCellValue(Convert.ToDouble(row[column].ToString()));
                                        }
                                        else if ((column.Ordinal > 7 && column.Ordinal <= 19) || column.Ordinal == 21)
                                        {
                                            Cell.SetCellValue(Common.ExtensionMethods.BoolToYesNo((bool)row[column]));
                                            Cell.CellStyle = workbook.CreateCellStyle();
                                            Cell.CellStyle.Alignment = HorizontalAlignment.Right;
                                        }
                                        else
                                            Cell.SetCellValue(row[column].ToString());
                                    }
                                }
                            }
                            else
                            {
                                if (column.Ordinal > 0)
                                {
                                    var cell = dataRow.CreateCell(column.Ordinal);

                                    if (table.Columns[column.Ordinal].DataType.Equals(typeof(decimal)))
                                        cell.SetCellValue(decimal.ToDouble((decimal)row[column]));
                                    else if (table.Columns[column.Ordinal].DataType.Equals(typeof(int)))
                                        cell.SetCellValue((int)row[column]);
                                    else
                                    {
                                        int numVal;
                                        if (int.TryParse(row[column].ToString(), out numVal))
                                            cell.SetCellValue(numVal);
                                        else
                                            cell.SetCellValue(row[column].ToString());
                                    }

                                    cell.CellStyle = workbook.CreateCellStyle();

                                    cell.CellStyle.SetFont(hFont);

                                    if (column.Ordinal != 1)
                                        cell.CellStyle.Alignment = HorizontalAlignment.Right;

                                    cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                    cell.CellStyle.BorderTop = BorderStyle.Thin;
                                    cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                    cell.CellStyle.BorderRight = BorderStyle.Thin;

                                    cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                                    cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                                }
                                dataRow.Height = 300;
                            }
                            rowIndex++;
                        }
                        sheet.AutoSizeColumn(column.Ordinal);
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
