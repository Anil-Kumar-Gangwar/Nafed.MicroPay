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
    
    public partial class PayrollApprovalSetting
    {
        public int ProcessAppID { get; set; }
        public int ProcessID { get; set; }
        public int Reporting1 { get; set; }
        public Nullable<int> Reporting2 { get; set; }
        public Nullable<int> Reporting3 { get; set; }
        public System.DateTime FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    
        public virtual tblMstEmployee tblMstEmployee { get; set; }
        public virtual tblMstEmployee tblMstEmployee1 { get; set; }
        public virtual tblMstEmployee tblMstEmployee2 { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
