using System;
using System.IO;
using System.Data;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace Nafed.MicroPay.ImportExport
{
    public class RecruitmentReportExport : BaseExcel
    {
        private RecruitmentReportExport()
        {
        }

        public static string ExportToExcel(IEnumerable<string> headers, DataTable rowData, string sSheetName, string sFullPath)
        {
            log.Info("RecruitmentReportExport/ExportToExcel");

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
                        string newhdr = "";
                        switch (hdr)
                        {
                            case "ApplicantName":
                                newhdr = "Name of the Applicant";
                                break;
                            case "Gender":
                                newhdr = "Gender";
                                break;
                            case "FathersName":
                                newhdr = "Fathers Name";
                                break;
                            case "MobileNo":
                                newhdr = "Mobile No.";
                                break;
                            case "Email":
                                newhdr = "E-mail Id";
                                break;
                            case "Position":
                                newhdr = "Position Applied for";
                                break;
                            case "Zoneapplied":
                                newhdr = "Zone applied for (If applicable)";
                                break;
                            case "DOB":
                                newhdr = "DOB";
                                break;
                            case "Age":
                                newhdr = "Age as on date of Advertisement";
                                break;
                            case "Qualification":
                                newhdr = "Qualification";
                                break;
                            case "RelevantExperience":
                                newhdr = "Minimum experience (in YY:MM)";
                                break;
                            case "AnnualGrossSalary":
                                newhdr = "Pay Scale / Fixed Monthly Remuneration";
                                break;
                            case "Address":
                                newhdr = "Present Address";
                                break;
                            case "DateofRegistration":
                                newhdr = "Date of Registration by the Candidates";
                                break;
                            case "Dateofsubmission":
                                newhdr = "Date of submission of Application";
                                break;
                            default:
                                break;
                        }
                        if (hdr == "TotalExperience" || hdr == "GovtExperience" || hdr == "GovtReleExp")
                        { }
                        else
                        {
                            var cell = headerRow.CreateCell(hdrColIndex);
                            cell.SetCellValue(newhdr);
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
                            // Cell.CellStyle.ShrinkToFit = true;
                            // Cell.CellStyle.BorderTop = BorderStyle.Thin;
                            //  Cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            // Cell.CellStyle.BorderRight = BorderStyle.Thin;
                            //  Cell.CellStyle.BorderBottom = BorderStyle.Thin;

                            if (column.ColumnName.ToLower() == "dob" || column.ColumnName.ToLower() == "dateofsubmission" || column.ColumnName.ToLower() == "dateofregistration")
                            {
                                var iDate = Convert.ToDateTime(row[column]).ToString("dd/MMM/yyyy");
                                Cell.SetCellValue(iDate);
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
