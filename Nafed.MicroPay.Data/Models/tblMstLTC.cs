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
    
    public partial class tblMstLTC
    {
        public int LTCID { get; set; }
        public int LTCNo { get; set; }
        public int EmployeeId { get; set; }
        public bool WhetherSelf { get; set; }
        public string DependentsList { get; set; }
        public Nullable<int> HomeTown { get; set; }
        public string WhereDetail { get; set; }
        public Nullable<decimal> Distance { get; set; }
        public Nullable<decimal> InitialcalAmount { get; set; }
        public System.DateTime DateAvailLTC { get; set; }
        public Nullable<System.DateTime> DateofReturn { get; set; }
        public Nullable<System.DateTime> DateofApplication { get; set; }
        public Nullable<decimal> TentativeAdvance { get; set; }
        public Nullable<decimal> LTCBillAmt { get; set; }
        public Nullable<decimal> Settlementdone { get; set; }
        public Nullable<int> Natureofleave { get; set; }
        public string Detail { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> BranchID { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> LeaveFrom { get; set; }
        public Nullable<System.DateTime> LeaveTo { get; set; }
        public string SHComment { get; set; }
        public string DHComment { get; set; }
        public string SpouseOrg { get; set; }
        public Nullable<short> FormStatus { get; set; }
        public string LTCReferenceNumber { get; set; }
        public string DealingAssistant { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual tblMstEmployee tblMstEmployee { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
