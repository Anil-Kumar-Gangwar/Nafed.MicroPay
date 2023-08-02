using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class MailFailedLog
    {
        public int EmployeeId { get; set; }
        public int? BranchId { get; set; }
        public int? EmployeeTypeId { get; set; }
        public int WorkingArea { get; set; }
        public string Reason { get; set; }
        public byte SalMonth { get; set; }
        public short SalYear { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
