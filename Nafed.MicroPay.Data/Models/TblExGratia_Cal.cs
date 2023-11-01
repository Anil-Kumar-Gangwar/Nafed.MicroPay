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
    
    public partial class TblExGratia_Cal
    {
        public int ExgratiaID { get; set; }
        public int BranchID { get; set; }
        public int EmployeeID { get; set; }
        public string EmpCode { get; set; }
        public int EmployeeTypeID { get; set; }
        public decimal Amt_Tot { get; set; }
        public string FinancialYear { get; set; }
        public Nullable<int> salyear { get; set; }
        public Nullable<int> salmonth { get; set; }
        public Nullable<decimal> ExGratiaOneday { get; set; }
        public Nullable<decimal> Incometax { get; set; }
        public Nullable<decimal> other { get; set; }
        public Nullable<decimal> ExGratiaAmt { get; set; }
        public Nullable<decimal> net_amount { get; set; }
        public Nullable<bool> isPercentage { get; set; }
        public Nullable<int> DaysCount { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsPublish { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual User User { get; set; }
        public virtual tblMstEmployee tblMstEmployee { get; set; }
        public virtual User User1 { get; set; }
    }
}
