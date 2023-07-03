using System.Collections.Generic;

namespace Nafed.MicroPay.Model
{
    public class BaseReportModel<T> where T : class 
    {
        public string ReportName { set; get; }
        public string ReportPath { set; get; }
        public System.Data.DataTable ReportDataSource { set; get; }
        public T ReportParameter { set; get; }      
      //  public void ReportDataSourceList<U>(List<U> items) where U : T {/*...*/}
    }
}
