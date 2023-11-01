//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nafed.MicroPay.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblLeaveTran
    {
        public string EmpCode { get; set; }
        public Nullable<System.DateTime> CurrDate { get; set; }
        public string LeaveType { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string Reason { get; set; }
        public string Longillness { get; set; }
        public Nullable<double> Unit { get; set; }
        public Nullable<double> AccruedLeave { get; set; }
        public Nullable<System.DateTime> DateOfLeaving { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string TransactionType { get; set; }
        public Nullable<int> LeaveCategoryID { get; set; }
        public int EmpLeaveTransID { get; set; }
        public Nullable<int> EmpLeaveID { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Nullable<bool> IsWithdrawl { get; set; }
    
        public virtual EmployeeLeave EmployeeLeave { get; set; }
    }
}
