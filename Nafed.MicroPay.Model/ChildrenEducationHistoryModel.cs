using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ChildrenEducationHistoryModel
    {
        public string ReportingYear { get; set; }
        public int EmployeeId { get; set; }
        public string ReceiptNo { get; set; }
        public Nullable<DateTime> ReceiptDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Branch { get; set; }
        public int ChildrenEduHdrID { get; set; }
        public int StatusId { get; set; }
    }
}
