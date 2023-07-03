using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Nafed.MicroPay.ImportExport
{
    public class AchievementCertificationReportExport : BaseExcel
    {
        private AchievementCertificationReportExport()
        {

        }
        public static string EmpAchievementExportToExcel(IEnumerable<string> headers,
            string reportFilterLabel,
            DataTable dtNoOfEmployees, DataTable rowData, string sSheetName, string sFullPath)
        {
            log.Info("AchievementCertificationReportExport/EmpAchievementExportToExcel");

            try
            {
                XSSFWorkbook workbook = CreateWookBook();

                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);

                    XSSFFont topHdrFont = CreateFont(workbook);
                    topHdrFont.FontHeightInPoints = 14;
                    topHdrFont.FontName = "Calibri";
                    topHdrFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;


                    #region  ----- Top Header -=============

                    XSSFFont titleFont = CreateFont(workbook);
                    titleFont.FontHeightInPoints = 13;
                    titleFont.FontName = "Calibri";
                    titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(titleFont);
                    titlecell.CellStyle.WrapText = true;
                    titlerow.Height = 1200;
                    titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                    titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 3));

                    #endregion

                    #region ----== Report Caption ===================

                    XSSFFont hdrCapFont = CreateFont(workbook);
                    hdrCapFont.FontHeightInPoints = 12;
                    hdrCapFont.FontName = "Calibri";
                    hdrCapFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow hdrCaption = CreateRow(sheet, 1);
                    ICell hdrCapCell = hdrCaption.CreateCell(0);
                    hdrCapCell.CellStyle = workbook.CreateCellStyle();
                    hdrCapCell.CellStyle.SetFont(hdrCapFont);
                    hdrCapCell.CellStyle.WrapText = true;

                    //  hdrCaption.Height = 1200;
                    hdrCapCell.CellStyle.Alignment = HorizontalAlignment.Center;
                    hdrCapCell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    hdrCapCell.SetCellValue($"Employee Achievement Report");

                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 3));

                    #endregion

                    #region   ======== ////==== Report Header Label Value  ==============

                    XSSFFont rLabel1Font = CreateFont(workbook);
                    rLabel1Font.FontHeightInPoints = 11;
                    rLabel1Font.FontName = "Calibri";
                    rLabel1Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow reportLabel1 = CreateRow(sheet, 2);
                    ICell rLabel1Cell = reportLabel1.CreateCell(0);
                    rLabel1Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel1Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel1Cell.CellStyle.WrapText = true;
                    //  hdrCaption.Height = 1200;

                    rLabel1Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel1Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    rLabel1Cell.SetCellValue($"Print Date : {DateTime.Now.ToString("dd-MM-yyyy")}");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 3));

                    XSSFRow reportLabel2 = CreateRow(sheet, 3);
                    ICell rLabel2Cell = reportLabel2.CreateCell(0);
                    rLabel2Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel2Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel2Cell.CellStyle.WrapText = true;

                    rLabel2Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel2Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                    rLabel2Cell.SetCellValue($"Date Range : {reportFilterLabel}");

                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 3));

                    #endregion

                    XSSFRow dataRow;

                    XSSFFont hFont = CreateFont(workbook);

                    for (int i = 0; i < dtNoOfEmployees.Rows.Count; i++)
                    {
                        var employeeID = Convert.ToInt32(dtNoOfEmployees.Rows[i]["EmployeeID"].ToString());

                        #region ======  Write Employee Header  ==========

                        var rowNo = sheet.LastRowNum + 2;
                        XSSFRow empHdrRow = CreateRow(sheet, rowNo);

                        XSSFFont empHdrFont = CreateFont(workbook);
                        empHdrFont.FontHeightInPoints = 10;
                        empHdrFont.FontName = "Calibri";
                        empHdrFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        ICell empHdrCell0 = empHdrRow.CreateCell(0);
                        empHdrCell0.CellStyle = workbook.CreateCellStyle();
                        empHdrCell0.CellStyle.SetFont(empHdrFont);
                        empHdrCell0.CellStyle.WrapText = true;
                        //  hdrCaption.Height = 1200;

                        empHdrCell0.CellStyle.Alignment = HorizontalAlignment.Left;
                        empHdrCell0.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        empHdrCell0.SetCellValue($"Employee");
                        empHdrCell0.CellStyle.BorderBottom = BorderStyle.Thin;
                        empHdrCell0.CellStyle.BorderTop = BorderStyle.Thin;


                        ICell empHdrCell1 = empHdrRow.CreateCell(1);
                        empHdrCell1.CellStyle = workbook.CreateCellStyle();

                        empHdrCell1.CellStyle.WrapText = true;


                        empHdrCell1.CellStyle.Alignment = HorizontalAlignment.Left;
                        empHdrCell1.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        empHdrCell1.SetCellValue($"{dtNoOfEmployees.Rows[i]["Employee"].ToString()}");

                        sheet.AddMergedRegion(new CellRangeAddress(rowNo, rowNo, 1, 3));
                        #endregion

                        #region  ===== Write table  Header ==============
                        XSSFRow headerRow = CreateRow(sheet, sheet.LastRowNum + 1);
                        int hdrColIndex = 0;
                        foreach (var hdr in headers)
                        {
                            hFont.FontHeightInPoints = 11;
                            hFont.FontName = "Calibri";
                            hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                            var cell = headerRow.CreateCell(hdrColIndex);
                            cell.CellStyle = workbook.CreateCellStyle();
                            cell.CellStyle.WrapText = true;

                            cell.SetCellValue(hdr);

                            cell.CellStyle.SetFont(hFont);

                            if (hdrColIndex > 0)
                                cell.CellStyle.Alignment = HorizontalAlignment.Center;
                            else if (hdrColIndex == 0)
                                cell.CellStyle.Alignment = HorizontalAlignment.Right;

                            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                            cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            cell.CellStyle.BorderTop = BorderStyle.Thin;
                            cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            cell.CellStyle.BorderRight = BorderStyle.Thin;

                            cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                            cell.CellStyle.FillPattern = FillPattern.SolidForeground;

                            sheet.AutoSizeColumn(hdrColIndex);

                            headerRow.Height = 500;
                            hdrColIndex++;
                        }
                        #endregion

                        #region == Write table Body ============ =====

                        var tblFiltered = rowData.AsEnumerable()
                              .Where(row => row.Field<int>("EmployeeID") == employeeID)
                              .OrderByDescending(row => row.Field<DateTime>("Date"))
                              .CopyToDataTable();

                        var tableRowNo = 1;
                        foreach (DataRow row in tblFiltered.Rows)
                        {
                            var tableRowIndex = sheet.LastRowNum + 1;
                            for (int jj = 0; jj < 4; jj++)
                            {
                                if (jj == 0)
                                    dataRow = (XSSFRow)sheet.CreateRow(tableRowIndex);
                                else
                                    dataRow = (XSSFRow)sheet.GetRow(tableRowIndex);

                                ICell Cell = dataRow.CreateCell(jj);
                                Cell.CellStyle = workbook.CreateCellStyle();


                                if (jj == 0)
                                    Cell.CellStyle.Alignment = HorizontalAlignment.Right;

                                else if(jj==1)
                                    Cell.CellStyle.Alignment = HorizontalAlignment.Center;

                                if (jj == 0)
                                    Cell.SetCellValue(tableRowNo);
                                else if (jj == 1)
                                {
                                    Cell.SetCellValue(Convert.ToDateTime(row["Date"].ToString()));
                                    Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy");
                                }

                                else if (jj == 2)
                                    Cell.SetCellValue(row["Achievement Name"].ToString());
                                else if (jj == 3)
                                    Cell.SetCellValue("");

                                Cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                Cell.CellStyle.BorderTop = BorderStyle.Thin;
                                Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                Cell.CellStyle.BorderRight = BorderStyle.Thin;
                            }
                            tableRowNo++;
                        }
                        #endregion
                    }

                    SetBordersToMergedCells(workbook, sheet);
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



        public static string EmpCertificationExportToExcel(IEnumerable<string> headers,
          string reportFilterLabel,
          DataTable dtNoOfEmployees, DataTable rowData, string sSheetName, string sFullPath)
        {
            log.Info("AchievementCertificationReportExport/EmpCertificationExportToExcel/");

            try
            {
                XSSFWorkbook workbook = CreateWookBook();

                using (var file = new FileStream(sFullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet(sSheetName);

                    XSSFFont topHdrFont = CreateFont(workbook);
                    topHdrFont.FontHeightInPoints = 14;
                    topHdrFont.FontName = "Calibri";
                    topHdrFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;


                    #region  ----- Top Header -=============

                    XSSFFont titleFont = CreateFont(workbook);
                    titleFont.FontHeightInPoints = 13;
                    titleFont.FontName = "Calibri";
                    titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow titlerow = CreateRow(sheet, 0);
                    ICell titlecell = titlerow.CreateCell(0);
                    titlecell.CellStyle = workbook.CreateCellStyle();
                    titlecell.CellStyle.SetFont(titleFont);
                    titlecell.CellStyle.WrapText = true;
                    titlerow.Height = 1200;
                    titlecell.CellStyle.Alignment = HorizontalAlignment.Center;
                    titlecell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    titlecell.SetCellValue("National Agricultural Cooperative Marketing Federation of India Ltd.\n Nafed House, Sidhartha Enclave, Ashram Chowk, Ring Road, \nNew Delhi - 110 014");

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 3));

                    #endregion

                    #region ----== Report Caption ===================

                    XSSFFont hdrCapFont = CreateFont(workbook);
                    hdrCapFont.FontHeightInPoints = 12;
                    hdrCapFont.FontName = "Calibri";
                    hdrCapFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow hdrCaption = CreateRow(sheet, 1);
                    ICell hdrCapCell = hdrCaption.CreateCell(0);
                    hdrCapCell.CellStyle = workbook.CreateCellStyle();
                    hdrCapCell.CellStyle.SetFont(hdrCapFont);
                    hdrCapCell.CellStyle.WrapText = true;

                    //  hdrCaption.Height = 1200;
                    hdrCapCell.CellStyle.Alignment = HorizontalAlignment.Center;
                    hdrCapCell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    hdrCapCell.SetCellValue($"Employee Certification Report");

                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 3));

                    #endregion

                    #region   ======== ////==== Report Header Label Value  ==============

                    XSSFFont rLabel1Font = CreateFont(workbook);
                    rLabel1Font.FontHeightInPoints = 11;
                    rLabel1Font.FontName = "Calibri";
                    rLabel1Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                    XSSFRow reportLabel1 = CreateRow(sheet, 2);
                    ICell rLabel1Cell = reportLabel1.CreateCell(0);
                    rLabel1Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel1Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel1Cell.CellStyle.WrapText = true;
                    //  hdrCaption.Height = 1200;

                    rLabel1Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel1Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                    rLabel1Cell.SetCellValue($"Print Date : {DateTime.Now.ToString("dd-MM-yyyy")}");
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 3));

                    XSSFRow reportLabel2 = CreateRow(sheet, 3);
                    ICell rLabel2Cell = reportLabel2.CreateCell(0);
                    rLabel2Cell.CellStyle = workbook.CreateCellStyle();
                    rLabel2Cell.CellStyle.SetFont(rLabel1Font);
                    rLabel2Cell.CellStyle.WrapText = true;

                    rLabel2Cell.CellStyle.Alignment = HorizontalAlignment.Left;
                    rLabel2Cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                    rLabel2Cell.SetCellValue($"Date Range : {reportFilterLabel}");

                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 3));

                    #endregion

                    XSSFRow dataRow;

                    XSSFFont hFont = CreateFont(workbook);

                    for (int i = 0; i < dtNoOfEmployees.Rows.Count; i++)
                    {
                        var employeeID = Convert.ToInt32(dtNoOfEmployees.Rows[i]["EmployeeID"].ToString());

                        #region ======  Write Employee Header  ==========

                        var rowNo = sheet.LastRowNum + 2;
                        XSSFRow empHdrRow = CreateRow(sheet, rowNo);

                        XSSFFont empHdrFont = CreateFont(workbook);
                        empHdrFont.FontHeightInPoints = 10;
                        empHdrFont.FontName = "Calibri";
                        empHdrFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        ICell empHdrCell0 = empHdrRow.CreateCell(0);
                        empHdrCell0.CellStyle = workbook.CreateCellStyle();
                        empHdrCell0.CellStyle.SetFont(empHdrFont);
                        empHdrCell0.CellStyle.WrapText = true;
                        //  hdrCaption.Height = 1200;

                        empHdrCell0.CellStyle.Alignment = HorizontalAlignment.Left;
                        empHdrCell0.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        empHdrCell0.SetCellValue($"Employee");
                        empHdrCell0.CellStyle.BorderBottom = BorderStyle.Thin;
                        empHdrCell0.CellStyle.BorderTop = BorderStyle.Thin;


                        ICell empHdrCell1 = empHdrRow.CreateCell(1);
                        empHdrCell1.CellStyle = workbook.CreateCellStyle();

                        empHdrCell1.CellStyle.WrapText = true;


                        empHdrCell1.CellStyle.Alignment = HorizontalAlignment.Left;
                        empHdrCell1.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        empHdrCell1.SetCellValue($"{dtNoOfEmployees.Rows[i]["Employee"].ToString()}");

                        sheet.AddMergedRegion(new CellRangeAddress(rowNo, rowNo, 1, 3));
                        #endregion

                        #region  ===== Write table  Header ==============
                        XSSFRow headerRow = CreateRow(sheet, sheet.LastRowNum + 1);
                        int hdrColIndex = 0;
                        foreach (var hdr in headers)
                        {
                            hFont.FontHeightInPoints = 11;
                            hFont.FontName = "Calibri";
                            hFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                            var cell = headerRow.CreateCell(hdrColIndex);
                            cell.CellStyle = workbook.CreateCellStyle();
                            cell.CellStyle.WrapText = true;

                            cell.SetCellValue(hdr);

                            cell.CellStyle.SetFont(hFont);

                            if (hdrColIndex > 0)
                                cell.CellStyle.Alignment = HorizontalAlignment.Center;
                            else if (hdrColIndex == 0)
                                cell.CellStyle.Alignment = HorizontalAlignment.Right;

                            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                            cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            cell.CellStyle.BorderTop = BorderStyle.Thin;
                            cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            cell.CellStyle.BorderRight = BorderStyle.Thin;

                            cell.CellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                            cell.CellStyle.FillPattern = FillPattern.SolidForeground;

                            sheet.AutoSizeColumn(hdrColIndex);

                            headerRow.Height = 500;
                            hdrColIndex++;
                        }
                        #endregion

                        #region == Write table Body ============ =====

                        var tblFiltered = rowData.AsEnumerable()
                              .Where(row => row.Field<int>("EmployeeID") == employeeID)
                              .OrderByDescending(row => row.Field<DateTime>("Date"))
                              .CopyToDataTable();

                        var tableRowNo = 1;
                        foreach (DataRow row in tblFiltered.Rows)
                        {
                            var tableRowIndex = sheet.LastRowNum + 1;
                            for (int jj = 0; jj < 4; jj++)
                            {
                                if (jj == 0)
                                    dataRow = (XSSFRow)sheet.CreateRow(tableRowIndex);
                                else
                                    dataRow = (XSSFRow)sheet.GetRow(tableRowIndex);

                                ICell Cell = dataRow.CreateCell(jj);
                                Cell.CellStyle = workbook.CreateCellStyle();


                                if (jj == 0)
                                    Cell.CellStyle.Alignment = HorizontalAlignment.Right;
                                else if(jj==1)
                                    Cell.CellStyle.Alignment = HorizontalAlignment.Center;

                                if (jj == 0)
                                    Cell.SetCellValue(tableRowNo);
                                else if (jj == 1)
                                {
                                    Cell.SetCellValue(Convert.ToDateTime(row["Date"].ToString()));
                                    Cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy");
                                }

                                else if (jj == 2)
                                    Cell.SetCellValue(row["Certification Name"].ToString());
                                else if (jj == 3)
                                    Cell.SetCellValue("");

                                Cell.CellStyle.BorderBottom = BorderStyle.Thin;
                                Cell.CellStyle.BorderTop = BorderStyle.Thin;
                                Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                                Cell.CellStyle.BorderRight = BorderStyle.Thin;
                            }
                            tableRowNo++;
                        }
                        #endregion
                    }

                    SetBordersToMergedCells(workbook, sheet);
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

    }
}
