using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class LeaveType
    {
        public int LeaveTypeID { get; set; }
        [DisplayName("Leave Type :")]
        [Required(ErrorMessage = "Please Enter Leave Code")]
        public string LeaveCode { get; set; }
        [DisplayName("Leave Description :")]
        [Required(ErrorMessage = "Please Enter Leave Description")]
        public string LeaveDesc { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
