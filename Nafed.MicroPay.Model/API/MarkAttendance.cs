using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model.API
{
    public class MarkAttendance
    {
        public int EmployeeId { get; set; }
        public System.DateTime ProxydateIn { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string Remarks { get; set; }
        public System.DateTime ProxyOutDate { get; set; }
        public int TypeID { get; set; }
        public string Mode { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string Attendancestatus { get; set; }
    }
}
