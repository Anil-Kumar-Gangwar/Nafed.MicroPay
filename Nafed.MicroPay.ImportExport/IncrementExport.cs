using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.ImportExport.Interfaces;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace Nafed.MicroPay.ImportExport
{
   public class IncrementExport: BaseExcel, IIncrementExport
    {
        public IncrementExport()
        {
        }
        public bool ExportToExcelIncrement(DataSet dsSource, string sFullPath, string fileName)
        {
            try
            {
                XSSFWorkbook workbook = CreateWookBook();
                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    foreach (DataTable table in dsSource.Tables)
                    {
                        XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(fileName);
                        XSSFRow dataRow;
                        //XSSFRow headerRow1 = CreateRow(sheet, 0);
                        //var incrementMonth = table.Rows[0]["INCREMENT MONTH"].ToString();
                        //var date = DateTime.Now.Date;
                        //ICell Cell1 = headerRow1.CreateCell(0);
                        //Cell1.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd., H.O.New Delhi");
                        //Cell1 = headerRow1.CreateCell(1);
                        //Cell1.SetCellValue("INCREMENT MONTH:-" + incrementMonth);
                        //Cell1 = headerRow1.CreateCell(2);
                        //Cell1.SetCellValue(" Report Date :" + date);
                        XSSFRow headerRow = CreateRow(sheet, 0);
                        XSSFFont hFont = CreateFont(workbook);
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                        //int j = 1;
                        foreach (DataColumn column in table.Columns)
                        {
                            int rowIndex = 1;
                            headerRow.Height = 500;
                            if (column.ColumnName != "Branch Name" && column.ColumnName != "Branch Code" && column.ColumnName != "INCREMENT MONTH")
                            {
                                XSSFCell cell = (XSSFCell)headerRow.CreateCell(column.Ordinal);
                                cell.SetCellValue(column.ColumnName);
                                cell.CellStyle = workbook.CreateCellStyle();
                                //hFont.IsBold = true;
                                cell.CellStyle.Alignment = HorizontalAlignment.Center;
                                cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                cell.CellStyle.SetFont(hFont);
                                cell.CellStyle.WrapText = true;
                                var oldbranch = "";

                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    if (/*!table.Rows[i][column].ToString().Trim().Equals("") &&*/ (!table.Rows[i][column].ToString().Trim().Equals("Branch Name") && !table.Rows[i][column].ToString().Trim().Equals("Branch Code") && !table.Rows[i][column].ToString().Trim().Equals("INCREMENT MONTH")))
                                    {
                                        if (oldbranch == table.Rows[i]["Branch Code"].ToString())
                                        {
                                            if (column.Ordinal == 0)
                                                dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                            else
                                                dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                                            dataRow.CreateCell(column.Ordinal).SetCellValue(table.Rows[i][column].ToString());
                                            rowIndex++;
                                        }
                                        else if (column.Ordinal == 0)
                                        {
                                            dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                            oldbranch = table.Rows[i]["Branch Code"].ToString();
                                            dataRow.CreateCell(column.Ordinal).SetCellValue("Branch Name :");
                                            dataRow.CreateCell((column.Ordinal) + 1).SetCellValue(table.Rows[i]["Branch Name"].ToString());
                                            rowIndex++;
                                            dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                            dataRow.CreateCell(column.Ordinal).SetCellValue(table.Rows[i][column].ToString());
                                            rowIndex++;
                                        }
                                        else
                                        {
                                            oldbranch = table.Rows[i]["Branch Code"].ToString();
                                            rowIndex++;
                                            dataRow = (XSSFRow)sheet.GetRow(rowIndex);
                                            dataRow.CreateCell(column.Ordinal).SetCellValue(table.Rows[i][column].ToString());
                                            rowIndex++;
                                        }
                                    }
                                }
                                //sheet.AutoSizeColumn(column.Ordinal);
                                if (column.ColumnName != "Pay Scale")
                                    sheet.AutoSizeColumn(column.Ordinal);
                                else
                                    sheet.SetColumnWidth(column.Ordinal, 30000);
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
    }
}
