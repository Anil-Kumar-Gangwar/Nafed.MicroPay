using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class LeaveRule
    {
        public int LeaveRuleID { get; set; }
        [DisplayName("Leave category :")]
        [Required(ErrorMessage = "Please select leave category")]
        public int LeaveCategoryID { get; set; }
        [DisplayName("Calendar Year :")]
        [Required(ErrorMessage = "Please select salendar year")]
        public int C_YearID { get; set; }
        [DisplayName("Leave limit for the calendar year :")]
        public int LeaveLimit { get; set; }
        [DisplayName("Total individual leave limit (all years):")]
        public int IndividualLeaveLimit { get; set; }

        public string LeaveCategory { get; set; }

        public string FinancialYear { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
