using Nafed.MicroPay.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MicroPay.Web.Models
{
    public class LeaveCategoryVM
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
        [Display(Name = "Max Leave Apply In One Shot :")]
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
        [Display(Name = "Apply Sandwich System :")]
        public bool IsSanwichSystem { get; set; }
        [AllowHtml]
        [Display(Name = "Leave Guidelines:")]
        public string leaveGuidelines { get; set; }
        [Display(Name = "Employee Type :")]
        public Nullable<int> EmployeeTypeID { get; set; }
        [Display(Name = "Approval Required Up To :")]
        public Nullable<int> ApprovalRequiredUpto { get; set; }

        public ApprovalRequiredLevel ApprovalLevel { get; set; }
        public Nullable<int> MinLeave { get; set; }
        public Nullable<int> AllowLevelUpto { get; set; }
    }
}