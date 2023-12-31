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
    
    public partial class MailFailedLog
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> EmployeeTypeId { get; set; }
        public Nullable<int> WorkingArea { get; set; }
        public string Reason { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<byte> SalMonth { get; set; }
        public Nullable<short> SalYear { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual EmployeeType EmployeeType { get; set; }
        public virtual tblMstEmployee tblMstEmployee { get; set; }
        public virtual User User { get; set; }
    }
}
