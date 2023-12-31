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
    
    public partial class NonRefundablePFLoan
    {
        public int ID { get; set; }
        public int NRPFLoanID { get; set; }
        public int EmployeeId { get; set; }
        public decimal Amount_of_Advanced { get; set; }
        public int Purpose_of_Advanced { get; set; }
        public Nullable<System.DateTime> Date_of_Sanction { get; set; }
        public string LocationOfDwellingSite { get; set; }
        public string NamePresentOwner { get; set; }
        public string AddressPresentOwner { get; set; }
        public string PresentStateofDwelling { get; set; }
        public Nullable<int> DesiredModeofRemittance { get; set; }
        public string ListofDocuments { get; set; }
        public Nullable<System.DateTime> CurrentDate { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    
        public virtual User User { get; set; }
        public virtual tblMstEmployee tblMstEmployee { get; set; }
        public virtual User User1 { get; set; }
        public virtual NRPFLoanHDR NRPFLoanHDR { get; set; }
    }
}
