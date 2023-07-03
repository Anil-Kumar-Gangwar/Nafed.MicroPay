using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{ 
    public class EmployeeDetails
    {
        public string outputValue { get; set; }
        public EmployeeDetail EmployeeDetailSummary { get; set; }
    } 
    public class EmployeeDetail
    {
        public string Name { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string Designation { get; set; }
        public string Location { get; set; }
        public string ReportingOfficer { get; set; }
        public string PresentAddress { get; set; }
        public string MobileNo { get; set; }

        public int ? BranchID { set; get; }
    } 
}
