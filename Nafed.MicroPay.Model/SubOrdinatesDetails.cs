using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class SubOrdinatesDetails
    {
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public Nullable<int> Appraisal { get; set; }
        public Nullable<int> Attendance_Approval { get; set; }
        public Nullable<int> Leave_Approval { get; set; }
    }
}
