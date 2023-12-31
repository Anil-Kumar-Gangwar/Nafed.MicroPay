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
    
    public partial class PayrollApprovalRequest
    {
        public int ApprovalRequestID { get; set; }
        public int ProcessID { get; set; }
        public string Period { get; set; }
        public int EmployeeTypeID { get; set; }
        public string BranchCode { get; set; }
        public byte Status { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> BranchID { get; set; }
        public string Comments { get; set; }
        public Nullable<int> EmployeeID { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual EmployeeType EmployeeType { get; set; }
        public virtual Process Process { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual tblMstEmployee tblMstEmployee { get; set; }
    }
}
