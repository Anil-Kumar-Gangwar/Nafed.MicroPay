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
    
    public partial class tblLoanTran
    {
        public string BranchCode { get; set; }
        public string EmployeeCode { get; set; }
        public string PriorityNo { get; set; }
        public string LoanType { get; set; }
        public decimal SancAmt { get; set; }
        public decimal CurrentROI { get; set; }
        public int CurrentPInstNoPaid { get; set; }
        public decimal CurrentPInstAmtPaid { get; set; }
        public decimal RemainingPAmt { get; set; }
        public decimal CurrentInterestAmt { get; set; }
        public int CurrentIInstNoPaid { get; set; }
        public Nullable<decimal> CurrentIInstAmtPaid { get; set; }
        public Nullable<decimal> RemainingIAmt { get; set; }
        public string PeriodOfPayment { get; set; }
        public Nullable<int> SkippedInstNo { get; set; }
        public Nullable<decimal> TDSRebetAmt { get; set; }
        public Nullable<short> IsNewLoanAfterDevelop { get; set; }
        public Nullable<int> SerialNo { get; set; }
        public Nullable<decimal> CLOSINGBALANCE { get; set; }
        public Nullable<decimal> PDebit { get; set; }
        public Nullable<decimal> PDinterest { get; set; }
        public Nullable<decimal> PCredit { get; set; }
        public Nullable<decimal> PCinterest { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public int LoanTypeID { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual tblMstLoanType tblMstLoanType { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual tblMstEmployee tblMstEmployee { get; set; }
        public virtual tblMstEmployee tblMstEmployee1 { get; set; }
    }
}
