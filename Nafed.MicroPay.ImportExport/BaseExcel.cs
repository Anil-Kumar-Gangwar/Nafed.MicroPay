using System;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using System.IO;
using System.IO.Compression;

namespace Nafed.MicroPay.ImportExport
{

    /// <summary>
    ///  All common method of excel exports ===== 
    /// </summary>
   abstract public class BaseExcel :Base
    {
       
        public static XSSFWorkbook CreateWookBook()
        {
            XSSFWorkbook wb = new XSSFWorkbook();
            return wb;
        }
        public static XSSFSheet CreateSheet(string sSheetName, XSSFWorkbook wb)
        {
            return (XSSFSheet)wb.CreateSheet(sSheetName);
        }
        public static XSSFRow CreateRow(XSSFSheet sheet, int iRowNumber)
        {
            return (XSSFRow)sheet.CreateRow(iRowNumber);
        }
        public static SXSSFRow CreateRow(SXSSFSheet sheet, int iRowNumber)
        {
            return (SXSSFRow)sheet.CreateRow(iRowNumber);
        }

        public static XSSFFont CreateFont(XSSFWorkbook wb)
        {
            return (XSSFFont)wb.CreateFont();
        }
        public static void AddHyperLink(XSSFCell cell, string address, HyperlinkType type)
        {
            if (HyperlinkType.Url == type)
            {
                XSSFHyperlink hyperlink = new XSSFHyperlink(HyperlinkType.Url);
                hyperlink.Address = address;
                cell.Hyperlink = hyperlink;
            }
        }
        public static XSSFPicture AddImage(XSSFWorkbook workbook, XSSFSheet sheet, int row, int col, string sDefaultOrderImage, string sItemImageUNCPath, string itemImageName)
        {
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
                }
            }
            else
            {
                itemImageArray = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sDefaultOrderImage));
            }
            int picInd = workbook.AddPicture(itemImageArray, (int)XSSFWorkbook.PICTURE_TYPE_JPEG);
            XSSFCreationHelper helper = workbook.GetCreationHelper() as XSSFCreationHelper;
            XSSFDrawing drawing = sheet.CreateDrawingPatriarch() as XSSFDrawing;
            XSSFClientAnchor anchor = helper.CreateClientAnchor() as XSSFClientAnchor;
            anchor.Col1 = col;
            anchor.Row1 = row;
            XSSFPicture pict = drawing.CreatePicture(anchor, picInd) as XSSFPicture;
            pict.Resize(1.0, 1.0);

            return pict;
        }

        public static byte[] Compress(Byte[] buffer)
        {
            byte[] compressedByte;
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress))
                {
                    ds.Write(buffer, 0, buffer.Length);
                }

                compressedByte = ms.ToArray();
            }
            return compressedByte;
        }
        public static ICellStyle GetCellStyle(IWorkbook workbook, short ForgroundColor, short fontColor, short fontHeightSize, short FontBoldWeight, FillPattern FillPatternType)
        {
            ICellStyle CellStyle = workbook.CreateCellStyle();
            CellStyle.FillForegroundColor = ForgroundColor;
            IFont font = workbook.CreateFont();
            font.Color = fontColor;
            font.Boldweight = FontBoldWeight;
            font.FontHeightInPoints = fontHeightSize;
            CellStyle.SetFont(font);
            //CellStyle.ShrinkToFit = true;                            
            CellStyle.FillPattern = FillPatternType;
            return CellStyle;
        }

        public static IComment GetCommentBox(XSSFWorkbook workbook, XSSFSheet sheet, int row, int col, string sAuthor, string Message)
        {
            XSSFCreationHelper helper = workbook.GetCreationHelper() as XSSFCreationHelper;
            XSSFDrawing drawing = sheet.CreateDrawingPatriarch() as XSSFDrawing;
            XSSFClientAnchor anchor = helper.CreateClientAnchor() as XSSFClientAnchor;
            IComment comment = drawing.CreateCellComment(new XSSFClientAnchor(0, 0, 0, 0, col, row, col + 3, row + 3));
            comment.Author = sAuthor;
            comment.String = new XSSFRichTextString($"{comment.Author}:{Environment.NewLine}" + Message);
            IFont font = workbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            comment.String.ApplyFont(0, comment.Author.Length, font);
            comment.Visible = false;
            return comment;
        }
    }
}
