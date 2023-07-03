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
    
    public partial class SalaryHead
    {
        public string FieldName { get; set; }
        public string FieldDesc { get; set; }
        public string Abbreviation { get; set; }
        public Nullable<bool> FormulaColumn { get; set; }
        public string LookUpHead { get; set; }
        public Nullable<int> RoundingUpto { get; set; }
        public Nullable<bool> AttendanceDep { get; set; }
        public Nullable<bool> RoundToHigher { get; set; }
        public Nullable<bool> MonthlyInput { get; set; }
        public int SeqNo { get; set; }
        public string LookUpHeadName { get; set; }
        public Nullable<bool> ActiveField { get; set; }
        public Nullable<bool> SpecialField { get; set; }
        public Nullable<bool> SpecialFieldMaster { get; set; }
        public Nullable<bool> FromMaster { get; set; }
        public Nullable<bool> LoanHead { get; set; }
        public Nullable<bool> Conditional { get; set; }
        public Nullable<bool> MT { get; set; }
        public Nullable<bool> C { get; set; }
        public Nullable<bool> DW { get; set; }
        public Nullable<bool> A { get; set; }
        public Nullable<bool> CT { get; set; }
        public Nullable<bool> DC { get; set; }
        public Nullable<bool> CW { get; set; }
        public Nullable<bool> FixedValueFormula { get; set; }
        public Nullable<decimal> FixedValue { get; set; }
        public Nullable<bool> Slab { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<bool> LocationDependent { get; set; }
        public Nullable<decimal> LowerRange { get; set; }
        public Nullable<decimal> UpperRange { get; set; }
        public int EmployeeTypeID { get; set; }
        public bool CheckHeadInEmpSalTable { get; set; }
    
        public virtual EmployeeType EmployeeType { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
