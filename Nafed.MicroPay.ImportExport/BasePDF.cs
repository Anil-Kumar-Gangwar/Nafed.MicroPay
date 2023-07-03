using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Nafed.MicroPay.ImportExport
{
    public class BasePDF:Base
    {

        protected PdfPageFormat pageFormat = null;
        public BasePDF(PdfPageFormat pageFormat)
        {
            this.pageFormat = pageFormat;
        }
        public static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }


        public virtual Document CreateDocument()
        {
            return new Document
                (            
                );
        }

        public static void WriteText(PdfContentByte cb, string text, float x, float y, float size, BaseFont bf,BaseColor textColor=null)
        {
            cb.BeginText();
            cb.SetFontAndSize(bf, size);
            cb.SetTextMatrix(x, y);
            cb.SetColorFill(textColor != null? textColor: BaseColor.BLACK);
            cb.ShowText(text);
            cb.EndText();
        }

      


        public static void PrintLogo(PdfContentByte cb,string imagePath,float absoluteX,float absoluteY )
        {
          //  string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigManager.Value("companyLogo"));
            //   string imagePath =ConfigManager.Value("companyLogo");
            byte[] byteArray = File.ReadAllBytes(imagePath);
            Image imageLogo = Image.GetInstance(byteArray);
           
            // imageLogo.BackgroundColor =  new BaseColor(System.Drawing.ColorTranslator.FromHtml("#a7092c"));
            imageLogo.SetAbsolutePosition(absoluteX, absoluteY);
            cb.AddImage(imageLogo);
        }


        public static PdfPCell ImageCell(string path, float scale, int align)
        {
            Image image = Image.GetInstance("");
            image.ScalePercent(scale);
            PdfPCell cell = new PdfPCell(image);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 0f;
            cell.PaddingTop = 0f;
            return cell;
        }
        public static PdfPCell pdfPCell(string content, BaseColor cellBackgroundColor, int colspan = 1, int rowspan = 1,int border = 15,int textFontSize = 4, int horizontalAlignment = 0, int verticalAlignment = 1)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, FontFactory.GetFont("Times New Roman", textFontSize, BaseColor.BLACK)));
            cell.Colspan = colspan;
            cell.Rowspan = rowspan;
            cell.Border = border;
            cell.BorderColor = BaseColor.GRAY;
            cell.HorizontalAlignment = horizontalAlignment;// PdfPCell.ALIGN_LEFT;
            cell.VerticalAlignment = verticalAlignment;// PdfPCell.ALIGN_CENTER;
            cell.BackgroundColor = cellBackgroundColor;
            return cell;
        }
        public static string FormatPDFCellValues(string cellValueDataType, string cellValue)
        {
            switch (cellValueDataType)
            {
                case "System.DateTime":
                    {
                        cellValue = Convert.ToDateTime(cellValue).Date.ToString("dd/MM/yyyy");
                        break;
                    }
                case "System.Boolean":
                    {
                        cellValue = (cellValue == "true" ? "Yes" : "No");
                        break;
                    }
                default:
                    break;
            }

            return cellValue;
        }

        public static int ExtractImagesFromPDF(string sourcePdf, string outputPath, string fileName)
        {

            // NOTE:  This will only get the first image it finds per page.
            PdfReader pdf = new PdfReader(sourcePdf);
            RandomAccessFileOrArray raf = new iTextSharp.text.pdf.RandomAccessFileOrArray(sourcePdf);
            try
            {
                //if (pdf.NumberOfPages > 1)
                //{
                //    return pdf.NumberOfPages;
                //}
                for (int pageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++)
                {
                    PdfDictionary pg = pdf.GetPageN(pageNumber);
                    PdfDictionary res =
                      (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
                    PdfDictionary xobj =
                      (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
                    if (xobj != null)
                    {
                        foreach (PdfName name in xobj.Keys)
                        {
                            PdfObject obj = xobj.Get(name);
                            if (obj.IsIndirect())
                            {
                                PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                                PdfName type =
                                  (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                                if (PdfName.IMAGE.Equals(type))
                                {

                                    int XrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                                    PdfObject pdfObj = pdf.GetPdfObject(XrefIndex);
                                    PdfStream pdfStrem = (PdfStream)pdfObj;
                                    byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
                                    if ((bytes != null))
                                    {
                                        using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(bytes))
                                        {
                                            memStream.Position = 0;
                                            System.Drawing.Image img = System.Drawing.Image.FromStream(memStream);
                                            // must save the file while stream is open.
                                            // if (!Directory.Exists(outputPath))
                                            //   Directory.CreateDirectory(outputPath);
                                            string path = Path.Combine(outputPath, String.Format(@"{0}.jpeg", fileName));
                                            System.Drawing.Imaging.EncoderParameters parms = new System.Drawing.Imaging.EncoderParameters(1);
                                            parms.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 0);
                                            // GetImageEncoder is found below this method
                                            System.Drawing.Imaging.ImageCodecInfo jpegEncoder = GetImageEncoder("jpeg");
                                            img.Save(path, jpegEncoder, parms);
                                            //lblMsgImage.Text = "Image Saved" + path; 
                                            return -1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                    }
                }
            }

            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + ", StackTrace: " + ex.StackTrace + ", DateTimeStamp: " + DateTime.Now);
                //return false;
            }
            finally
            {
                pdf.RemoveFields();
                raf.Close();
                pdf.Close();
            }
            return -5;

        }

        public static System.Drawing.Imaging.ImageCodecInfo GetImageEncoder(string imageType)
        {
            imageType = imageType.ToUpperInvariant();

            foreach (System.Drawing.Imaging.ImageCodecInfo info in System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders())
            {
                if (info.FormatDescription == imageType)
                {
                    return info;
                }
            }

            return null;
        }
    }
}
