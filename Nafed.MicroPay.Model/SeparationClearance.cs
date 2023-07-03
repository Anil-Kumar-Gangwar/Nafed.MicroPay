using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class SeparationClearance
    {
        public int ClearanceId { get; set; }
        public int SeparationId { get; set; }
        public int ReportingTo { get; set; }
        public Nullable<bool> StatusId { get; set; }
        public string Remark { get; set; }
        public string ReportingManager { get; set; }
        public string EmployeeName { get; set; }
        public ProcessWorkFlow _ProcessWorkFlow { get; set; } = new ProcessWorkFlow();
        public string ReportingDepartment { get; set; }
        public string ApprovalType { get; set; }
        public DateTime?  DateofAction { get; set; }
        public Nullable<System.DateTime> ClearanceDateUpto { get; set; }
    }
}
