using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using System.Data;
using iTextSharp.text.pdf;
using System.IO;

namespace Nafed.MicroPay.ImportExport

{
    public class StockReportPDF:BasePDF
    {
      //  private PdfPageFormat pageFormat = null;
      
        public static BaseFont helvetica;
        public static BaseFont helveticaBold;
        public static float bodyFontSize = 7;
        public static int yAxis;
        public static float xAxis;

        public StockReportPDF(PdfPageFormat pageFormat) :base(pageFormat)
        {
          //  this.pageFormat = pageFormat;
        }

        private void WriteText(PdfContentByte cb, string text, float x, float y, float size, BaseFont bf)
        {
            cb.BeginText();
            cb.SetFontAndSize(bf, size);
            cb.SetTextMatrix(x, y);
            cb.SetRGBColorFill(0, 0, 0);
            cb.ShowText(text);
            cb.EndText();
        }

        public string CreatePDF(DataTable dataSource, string sFileName, string sDefaultOrderImage, string sWorkstreamXLogo, string sItemImageUNCPath)
        {
            log.Info("StockReportPDF/CreatePDF");
            //=====logic to generate pdf file for stock report ========

            string responseStatus = string.Empty;
            string DateCreated = pageFormat.createdOn.ToString("g");
            
            ///--- FileOperation.CreateDirectoryIfNotExists(ConfigManager.Value("ExportPath"));
            int iTotalRecord = dataSource.Rows.Count;
            int totalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(iTotalRecord) / Convert.ToDecimal(pageFormat.itemPerPage)));


          //  if (!string.IsNullOrEmpty(ConfigManager.Value("ExportFilePath")))
         //   {
                string[] columnNames = dataSource.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                string[] columnNameExceptImageField = columnNames.Where(x => x != "photo").ToArray();

                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sWorkstreamXLogo);

                try
                {

                    Document doc = CreateDocument();                   
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(sFileName, FileMode.Create));
                    writer.CompressionLevel = PdfStream.BEST_COMPRESSION;

                    helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1257, BaseFont.EMBEDDED);
                    helveticaBold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1257, BaseFont.EMBEDDED);
                    doc.SetPageSize(PageSize.A4.Rotate());
                    
                    doc.Open();

                    PdfContentByte cb = writer.DirectContent;

                    for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
                    {
                        int y = 5;
                        yAxis = 550;
                        xAxis = 15.53f;
                        float xAxisH = 15.53f;
                        int exculdingItems = pageIndex * pageFormat.itemPerPage;

                        PrintLogo(cb, logoPath,xAxis, yAxis);

                        WriteText(cb, pageFormat.pageHeaderText, xAxisH + 310, yAxis + 10, 13, helveticaBold);
                        WriteText(cb, "Date Created:", xAxisH + 660, yAxis + 25, 8, helveticaBold);
                        WriteText(cb, DateCreated, xAxisH + 720, yAxis + 25, 8, helvetica);
                        WriteText(cb, "Page:", xAxisH + 660, yAxis + 10, 8, helveticaBold);
                        WriteText(cb, (pageIndex + 1) + " of " + totalPages, xAxisH + 720, yAxis + 10, 8, helvetica);
                        WriteText(cb, "Total Items:", xAxisH + 660, yAxis - 5, 8, helveticaBold);
                        WriteText(cb, dataSource.Rows.Count.ToString(), xAxisH + 720, yAxis - 5, 8, helvetica);

                        WriteDashLine(cb, 10);

                        DataTable dtOfCurrentPage = dataSource.Rows.Cast<System.Data.DataRow>().Skip(exculdingItems).Take(pageFormat.itemPerPage).CopyToDataTable();

                        int MItemsPerPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(dtOfCurrentPage.Rows.Count) / 2));
                        int Item = 0;
                     
                        foreach (DataRow itemRow in dtOfCurrentPage.Rows)
                        {
                            if (Item == 3)
                            {
                                xAxis = 15.53f;
                                //    xAxis += 1;
                                y += 265;
                            }
                            try
                            {
                                string itemImageName = itemRow["photo"].ToString();
                                string sImagePath = string.Empty;
                                byte[] itemImageArray = null;
                                if (itemImageName != "")
                                {
                                    sImagePath = sItemImageUNCPath + itemImageName;
                                    if (File.Exists(sImagePath))
                                    {
                                        itemImageArray = File.ReadAllBytes(sImagePath);
                                    }
                                    else
                                    {
                                        itemImageArray = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sDefaultOrderImage));
                                        //  itemImageArray = File.ReadAllBytes(ConfigManager.Value("defaultOrderImage"));
                                    }
                                }
                                else
                                {
                                    itemImageArray = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sDefaultOrderImage));
                                    // itemImageArray = File.ReadAllBytes(ConfigManager.Value("defaultOrderImage"));
                                }

                            Image itemimage = Image.GetInstance(itemImageArray);
                            itemimage.ScaleAbsoluteWidth(200);
                            itemimage.ScaleAbsoluteHeight(171);
                            itemimage.CompressionLevel = 50;
                            itemimage.SetAbsolutePosition(xAxis, yAxis - (y + 180));//  yAxis - (y + 230));
                            cb.AddImage(itemimage);
                        }
                            catch (Exception ex)
                            {
                                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                            }

                            int xPositionHeader = 1;
                            int countColumn = 0;
                            int lineNumber = 1;
                            int yPositionHeaderData = 193;
                            int xPositionData = 55;
                            bodyFontSize = 8;

                            foreach (string col in columnNameExceptImageField)
                            {                             
                                 WriteText(cb, col + ":", xAxis + xPositionHeader, yAxis - (y + yPositionHeaderData), bodyFontSize, helveticaBold);

                                 string columnDataTypeName = dtOfCurrentPage.Columns[col].DataType.Name.ToString();

                                 string cellNewValue = FormatPDFCellValues(columnDataTypeName, itemRow[col].ToString());

                                 WriteText(cb, cellNewValue, xAxis + xPositionData, yAxis - (y + yPositionHeaderData), bodyFontSize, helvetica);
                             
                                    xPositionHeader += 150;
                                    xPositionData += 150;
                                    countColumn += 1;
                                    /********** Reset x and yaxis (For next line) after printing 5 columns and break for next line **********/
                                    if (countColumn % 1 == 0)
                                    {
                                        yPositionHeaderData = yPositionHeaderData + 15;
                                        lineNumber += 1;
                                        xPositionHeader = 1;
                                        xPositionData = 55;
                                    }
                                    /***************************************************************************************/
                             //   }
                            }
                            xAxis += 290;                        
                            xAxis += 1;
                            Item++;
                        }                     
                        /**********************************************************************************************************************/
                        doc.NewPage();
                    }                  
                     doc.Close();
                    writer.Close();
                    responseStatus = "success";
                }
             
                catch (Exception ex)
                {
                    responseStatus = "fail";
                    log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                }

                return responseStatus;
            //}
            //else
            //{
            //    return responseStatus;
            //}           
        }
        public override Document CreateDocument()
        {
            return new Document
                (
                PageSize.A4,
                pageFormat.marginLeft,
                pageFormat.marginRight,
                pageFormat.marginTop,
                pageFormat.marginBottom
                );
        }
        public static void WriteDashLine(PdfContentByte cb, int yAxs)
        {
            cb.SetLineDash(new float[] { 3.0f, 3.0f }, 0);
            cb.SetLineWidth(0.7f);
            cb.SetGrayFill(1.0f);
            cb.MoveTo(15, yAxis - yAxs);
            cb.LineTo(821, yAxis - yAxs);
            cb.Stroke();
            cb.SetLineDash(0f);
        }
    }
}
