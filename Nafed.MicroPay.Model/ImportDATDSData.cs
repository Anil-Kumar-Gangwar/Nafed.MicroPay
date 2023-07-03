using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ImportDATDSData
    {
        //public int Sno { set; get; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string BranchName { get; set; }

        public string DesignationName { set; get; }
        public string Month { get; set; }
        public string Year { get; set; }
       
      

        public string DA { set; get; }

        public string IncomeTaxDeduction { set; get; }

        public string NetPay { set; get; }

        
        public string EmployeeId { get; set; }
        public string BranchID { get; set; }

        public string error { set; get; }
        public string warning { set; get; }
        public bool isDuplicatedRow { set; get; }
    }
    public class DATDSColValues
    {
        public List<string> empCode { set; get; }
        public List<string> BranchName { set; get; }

    }
}
