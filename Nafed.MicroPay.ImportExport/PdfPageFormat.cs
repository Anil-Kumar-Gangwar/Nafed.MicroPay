using System;


namespace Nafed.MicroPay.ImportExport
{
    public class PdfPageFormat
    {
        public string waterMarkText { set; get; }
        public string pageHeaderLogo { set; get; }
        public float marginLeft { set; get; }
        public float marginRight { set; get; }
        public float marginTop { set; get; }
        public float marginBottom { set; get; }
        public int itemPerPage { set; get; }
        public string pageHeaderText { set; get; }

        public float bodyFontSize { set; get; }

        public string pageFooterText { set; get; }
        public DateTime createdOn
        {
            get
            {
                return System.DateTime.Now;
            }
        }
    }
}
