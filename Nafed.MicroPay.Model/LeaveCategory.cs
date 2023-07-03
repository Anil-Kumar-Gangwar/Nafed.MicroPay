using Nafed.MicroPay.Common;
using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class LeaveCategory
    {
       
        public int LeaveCategoryID { get; set; }
        [Display(Name = "Leave Code :")]
        [Required(ErrorMessage = "Please Enter Leave Code")]
        public string LeaveCode { get; set; }

        [Display(Name = "Leave Category :")]
        [Required(ErrorMessage = "Please Enter Leave Category")]
        public string LeaveCategoryName { get; set; }
        [Display(Name = "Deduct From Salary :")]

        public bool DeductFromSalary { get; set; }
        [Display(Name = "Carry Forward :")]
        public bool CarryForward { get; set; }
        [Display(Name = "Max Carry Forward Unit :")]
        [Required(ErrorMessage = "Please Enter Max Carry Forward Unit")]
        public int MaxCFUnit { get; set; }
        [Display(Name = "Max leave apply in one shot :")]
        [Required(ErrorMessage = "Please Enter Max Leave")]
        public int MaxLeave { get; set; }
        [Display(Name = "Remarks :")]
        public string Remarks { get; set; }
        [Display(Name = "Sequence No.:")]
        public Nullable<int> SeqNo { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Apply Sandwich system :")]
        public bool IsSanwichSystem { get; set; }
      
        [Display(Name = "Leave Guidelines:")]
        public string leaveGuidelines { get; set; }
        [Display(Name = "Employee Type :")]
        public Nullable<int> EmployeeTypeID { get; set; }
        [Display(Name = "Approval required up to :")]
        public Nullable<int> ApprovalRequiredUpto { get; set; }

        public ApprovalRequiredLevel ApprovalLevel { get; set; }
        public Nullable<int> MinLeave { get; set; }
        public Nullable<int> AllowLevelUpto { get; set; }

    }
}
