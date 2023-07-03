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
using NPOI.SS.Util;
namespace Nafed.MicroPay.ImportExport
{
    public class Export : BaseExcel, IExport
    {
        public Export()
        {
        }
        string IExport.ExportBlankTemplateWithDroDowns(DataTable sourceDataTable, string sSheetName, string sFullPath, bool bDropdown, DataSet dsDropDownValues)
        {
            throw new NotImplementedException();
        }

        public bool ExportToExcel(DataSet dsSource, string sFullPath, string fileName)
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
                        XSSFRow headerRow = CreateRow(sheet, 0);
                        XSSFFont hFont = CreateFont(workbook);
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                        foreach (DataColumn column in table.Columns)
                        {
                            int rowIndex = 1;
                            headerRow.Height = 500;
                            XSSFCell cell = (XSSFCell)headerRow.CreateCell(column.Ordinal);
                            cell.SetCellValue(column.ColumnName);
                            cell.CellStyle = workbook.CreateCellStyle();
                            //hFont.IsBold = true;
                            cell.CellStyle.Alignment = HorizontalAlignment.Center;
                            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            cell.CellStyle.SetFont(hFont);
                            foreach (DataRow row in table.Rows)
                            {
                                if (column.Ordinal == 0)
                                    dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                else
                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                                if (!row[column].ToString().Trim().Equals(""))
                                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                                rowIndex++;
                            }
                            sheet.AutoSizeColumn(column.Ordinal);
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

        byte[] IExport.ExportToExcel(DataTable sourceDataTable, string sSheetName)
        {
            throw new NotImplementedException();
        }

        string IExport.ExportToExcel(DataTable sourceDataTable, string sSheetName, string sFullPath, string sDefaultOrderImage, string sItemImageUNCPath, string sArchiveItemImageUNCPath, string[] nonExportColumn)
        {
            throw new NotImplementedException();
        }

        string IExport.ExportToExcelWithDroDowns(DataTable sourceDataTable, string sSheetName, string sFullPath, string sDefaultOrderImage, string sItemImageUNCPath, bool bDropdown, DataSet dtItemGroup)
        {
            throw new NotImplementedException();
        }

        string IExport.ExportToExcelWithError(DataTable sourceDataTable, string sSheetName, string sFullPath, string sDefaultOrderImage, string sItemImageUNCPath, List<string> lstSkipColumns)
        {
            throw new NotImplementedException();
        }

        string IExport.ExportToExcelWithErrorDynamic(DataTable sourceDataTable, string sSheetName, string sFullPath, string sDefaultOrderImage, string sItemImageUNCPath, List<string> lstSkipColumns)
        {
            throw new NotImplementedException();
        }

        byte[] IExport.ReadMSToExcel(MemoryStream ms, DataSet dsArtworkTemplate, List<string> ArryTemplateIDs)
        {
            throw new NotImplementedException();
        }

        MemoryStream IExport.WriteExcelInMS(DataTable sourceDataTable, string sSheetName)
        {
            throw new NotImplementedException();
        }

        public bool ExportFormatedExcel(DataSet dsSource, string sFullPath, string fileName)
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
                        XSSFRow headerRow = CreateRow(sheet, 0);
                        XSSFFont hFont = CreateFont(workbook);
                        hFont.FontHeightInPoints = 11;
                        hFont.FontName = "Calibri";
                        hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                        foreach (DataColumn column in table.Columns)
                        {
                            int rowIndex = 1;
                            headerRow.Height = 500;
                            XSSFCell cell = (XSSFCell)headerRow.CreateCell(column.Ordinal);
                            cell.SetCellValue(column.ColumnName);
                            cell.CellStyle = workbook.CreateCellStyle();
                            //hFont.IsBold = true;
                            cell.CellStyle.Alignment = HorizontalAlignment.Center;
                            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            cell.CellStyle.SetFont(hFont);
                            var colType = column.DataType.Name;
                            ICellStyle cellStyleDouble = workbook.CreateCellStyle();
                            cellStyleDouble.DataFormat = workbook.CreateDataFormat().GetFormat("0.000");

                            foreach (DataRow row in table.Rows)
                            {
                                if (column.Ordinal == 0)
                                    dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                                else
                                    dataRow = (XSSFRow)sheet.GetRow(rowIndex);

                                if (!row[column].ToString().Trim().Equals(""))
                                {
                                    if (colType == "String")
                                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                                    else if (colType == "Int32" || colType == "Single")
                                        dataRow.CreateCell(column.Ordinal).SetCellValue(Convert.ToInt32(row[column]));
                                    else if (colType == "Decimal")
                                    {
                                        ICell cell1 = dataRow.CreateCell(column.Ordinal, NPOI.SS.UserModel.CellType.Numeric);
                                        cell1.CellStyle = cellStyleDouble;
                                        cell1.SetCellValue(Convert.ToDouble(row[column]));
                                    }
                                    else
                                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                                }
                                rowIndex++;
                            }
                            sheet.AutoSizeColumn(column.Ordinal);
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


        public static string ExportToExcel(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, string tFilter)
        {          

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
                    //if (string.IsNullOrEmpty(tFilter))
                    //    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 8));
                    //else
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 13));

                    XSSFRow firstRow = CreateRow(sheet, 1);
                    ICell firstCell = firstRow.CreateCell(0);
                    firstCell.CellStyle = workbook.CreateCellStyle();
                    firstCell.SetCellValue(tFilter);
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
                   
                    ICell printDate = firstRow.CreateCell(8);
                    printDate.CellStyle = workbook.CreateCellStyle();
                    printDate.SetCellValue("Print Date:  " + DateTime.Now.Date.ToString("dd-MMM-yyyy"));
                    printDate.CellStyle.SetFont(scFont);
                    printDate.CellStyle.BorderTop = BorderStyle.Thin;
                    printDate.CellStyle.BorderLeft = BorderStyle.Thin;
                    printDate.CellStyle.BorderRight = BorderStyle.Thin;
                    printDate.CellStyle.BorderBottom = BorderStyle.Thin;                   

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

                    ICell firstR11cell = firstRow.CreateCell(11);
                    firstR11cell.CellStyle = workbook.CreateCellStyle();
                    firstR11cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR11cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR11cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR11cell.CellStyle.BorderBottom = BorderStyle.Thin;


                    ICell firstR12cell = firstRow.CreateCell(12);
                    firstR12cell.CellStyle = workbook.CreateCellStyle();
                    firstR12cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR12cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR12cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR12cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR13cell = firstRow.CreateCell(13);
                    firstR13cell.CellStyle = workbook.CreateCellStyle();
                    firstR13cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR13cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR13cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR13cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 8, 13));

                    XSSFRow secondrow = CreateRow(sheet, 2);
                    ICell secondcell = secondrow.CreateCell(0);
                    secondcell.CellStyle = workbook.CreateCellStyle();
                    secondcell.SetCellValue("");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 13));

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
                        if (hdr == "Remark" || hdr == "Name")
                        {
                            sheet.SetColumnWidth(hdrColIndex, 10000);
                            cell.CellStyle.WrapText = true;
                        }
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

        public static string ECRExportToExcel(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath, string tFilter)
        {
            log.Info("Export/ECRExportToExcel");

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
                    //if (string.IsNullOrEmpty(tFilter))
                    //    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 8));
                    //else
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 28));

                    XSSFRow firstRow = CreateRow(sheet, 1);
                    ICell firstCell = firstRow.CreateCell(0);
                    firstCell.CellStyle = workbook.CreateCellStyle();
                    firstCell.SetCellValue(tFilter);
                    firstCell.CellStyle.SetFont(scFont);
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 20));

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
                    printDate.CellStyle.BorderTop = BorderStyle.Thin;
                    printDate.CellStyle.BorderLeft = BorderStyle.Thin;
                    printDate.CellStyle.BorderRight = BorderStyle.Thin;
                    printDate.CellStyle.BorderBottom = BorderStyle.Thin;

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

                    ICell firstR11cell = firstRow.CreateCell(11);
                    firstR11cell.CellStyle = workbook.CreateCellStyle();
                    firstR11cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR11cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR11cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR11cell.CellStyle.BorderBottom = BorderStyle.Thin;


                    ICell firstR12cell = firstRow.CreateCell(12);
                    firstR12cell.CellStyle = workbook.CreateCellStyle();
                    firstR12cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR12cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR12cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR12cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR13cell = firstRow.CreateCell(13);
                    firstR13cell.CellStyle = workbook.CreateCellStyle();
                    firstR13cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR13cell.CellStyle.BorderLeft = BorderStyle.Thin;
                    firstR13cell.CellStyle.BorderRight = BorderStyle.Thin;
                    firstR13cell.CellStyle.BorderBottom = BorderStyle.Thin;

                    ICell firstR14cell = firstRow.CreateCell(14);
                    firstR14cell.CellStyle = workbook.CreateCellStyle();
                    firstR14cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR14cell.CellStyle.BorderRight = BorderStyle.Thin;

                    ICell firstR15cell = firstRow.CreateCell(15);
                    firstR15cell.CellStyle = workbook.CreateCellStyle();
                    firstR15cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR15cell.CellStyle.BorderRight = BorderStyle.Thin;

                    ICell firstR16cell = firstRow.CreateCell(16);
                    firstR16cell.CellStyle = workbook.CreateCellStyle();
                    firstR16cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR16cell.CellStyle.BorderRight = BorderStyle.Thin;

                    ICell firstR17cell = firstRow.CreateCell(17);
                    firstR17cell.CellStyle = workbook.CreateCellStyle();
                    firstR17cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR17cell.CellStyle.BorderRight = BorderStyle.Thin;

                    ICell firstR18cell = firstRow.CreateCell(18);
                    firstR18cell.CellStyle = workbook.CreateCellStyle();
                    firstR18cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR18cell.CellStyle.BorderRight = BorderStyle.Thin;

                    ICell firstR19cell = firstRow.CreateCell(19);
                    firstR19cell.CellStyle = workbook.CreateCellStyle();
                    firstR19cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR19cell.CellStyle.BorderRight = BorderStyle.Thin;

                    ICell firstR20cell = firstRow.CreateCell(20);
                    firstR20cell.CellStyle = workbook.CreateCellStyle();
                    firstR20cell.CellStyle.BorderTop = BorderStyle.Thin;
                    firstR14cell.CellStyle.BorderRight = BorderStyle.Thin;

                    ICell firstR21cell = firstRow.CreateCell(21);
                    firstR21cell.CellStyle = workbook.CreateCellStyle();
                    firstR21cell.SetCellValue("Print Date:  " + DateTime.Now.Date.ToString("dd-MMM-yyyy"));
                    firstR21cell.CellStyle.SetFont(scFont);

                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 21, 28));

                    XSSFRow secondrow = CreateRow(sheet, 2);
                    ICell secondcell = secondrow.CreateCell(0);
                    secondcell.CellStyle = workbook.CreateCellStyle();
                    secondcell.SetCellValue("");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 28));

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
                        //if (hdr == "Remark" || hdr == "Name")
                        //{
                        //    sheet.SetColumnWidth(hdrColIndex, 10000);
                        //    cell.CellStyle.WrapText = true;
                        //}
                        //else
                            sheet.AutoSizeColumn(hdrColIndex);
                        hdrColIndex++;
                    }
                  var style=  workbook.CreateCellStyle();
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
                            Cell.CellStyle = style;
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

    }
}
