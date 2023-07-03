using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmployeeLeaveBalance
    {
        public string EmpCode { get; set; }
        public string LeaveType { get; set; }
        public Nullable<double> OpeningBal { get; set; }
        public Nullable<double> Accrued { get; set; }
        public Nullable<double> availed { get; set; }
        public Nullable<double> Balance { get; set; }
    }
}
