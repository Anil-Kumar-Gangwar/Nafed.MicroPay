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
    
    public partial class tblMstLoanType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblMstLoanType()
        {
            this.tblLoanTrans = new HashSet<tblLoanTran>();
            this.tblmstslabs = new HashSet<tblmstslab>();
            this.tblMstLoanPriorities = new HashSet<tblMstLoanPriority>();
        }
    
        public string LoanType { get; set; }
        public string LoanDesc { get; set; }
        public string PaymentType { get; set; }
        public Nullable<short> MaxInstallmentP { get; set; }
        public Nullable<short> MaxInstallmentI { get; set; }
        public Nullable<decimal> MaxLnAmount { get; set; }
        public Nullable<bool> IsOnFloatingRate { get; set; }
        public Nullable<bool> IsCalcInterest { get; set; }
        public string Abbriviation { get; set; }
        public Nullable<bool> IsSlabDependent { get; set; }
        public Nullable<bool> IsTDSRebet { get; set; }
        public Nullable<decimal> TDSRebetROI { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public int LoanTypeId { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblLoanTran> tblLoanTrans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblmstslab> tblmstslabs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMstLoanPriority> tblMstLoanPriorities { get; set; }
    }
}
