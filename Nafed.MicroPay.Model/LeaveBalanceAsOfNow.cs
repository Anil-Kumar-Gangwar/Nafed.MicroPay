using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class LeaveBalanceAsOfNow
    {
        [DisplayName("Employee Name :")]
        public string EmployeeName { get; set; }
        [DisplayName("Employee Code :")]
        public string EmployeeCode { get; set; }
        public string Branch { get; set; }
        public string DesignationName { set; get; }
        [DisplayName("EL Balance :")]
        public Nullable<double> ELBal { get; set; }
        [DisplayName("ML Balance :")]
        public Nullable<double> MLBal { get; set; }
        [DisplayName("CL Balance :")]
        public Nullable<double> CLBal { get; set; }
        [DisplayName("Medical Extra :")]
        public Nullable<double> MLExtraBal { get; set; }
        public string LeaveYear { get; set; }

    }
}
