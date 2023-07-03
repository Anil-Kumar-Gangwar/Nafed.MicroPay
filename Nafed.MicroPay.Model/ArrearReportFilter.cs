using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public  class ArrearReportFilter
    {
        public char arrearType { set; get; }
        public int? employeeTypeID { get; set; }
        public string fileName { set; get; }
        public string filePath { set; get; }
        public int? employeeID { get; set; }
        public int? branchID { get; set; }

        public int pFrom { get; set; }
        public int pTo { get; set; }
    }
}
