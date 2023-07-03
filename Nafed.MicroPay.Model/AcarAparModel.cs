using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class AcarAparModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string ReportingYr { get; set; }
        public string FormName { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentID { get; set; }
        public int DesignationID { get; set; }
        public string DesignationName { get; set; }
        public int APARStatus { get; set; }
        public int ACARStatus { get; set; }
        public int FormID { get; set; }
        public int ReportingTo { get; set; }
        public int? ReviewingTo { get; set; }
        public int? AcceptanceAuthority { get; set; }
    }
}
