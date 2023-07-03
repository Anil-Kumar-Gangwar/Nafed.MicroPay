using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
     public class ProcessWorkFlow
    {

        public int WorkflowID { get; set; }
        public int ProcessID { get; set; }
        public Nullable<int> ReferenceID { get; set; }
        public Nullable<int> SenderID { get; set; }
        public Nullable<int> ReceiverID { get; set; }
        public Nullable<System.DateTime> Senddate { get; set; } = System.DateTime.Now;
        public Nullable<System.DateTime> Readdate { get; set; }
        public Nullable<int> Readflag { get; set; }
        public string Scomments { get; set; }
        public Nullable<int> Forwarded_WorkflowID { get; set; }
        public string Purpose { get; set; }
        public Nullable<int> StatusID { get; set; }

        public int EmployeeID { get; set; }
        public Nullable<int> SenderDesignationID { get; set; }
        public Nullable<int> ReceiverDesignationID { get; set; }
        public Nullable<int> SenderDepartmentID { get; set; }
        public Nullable<int> ReceiverDepartmentID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; } = System.DateTime.Now;
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string Remark { get; set; }


        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string SenderName { get; set; }
        public string Receivername { get; set; }

        public string Status { get; set; }
    }
}
