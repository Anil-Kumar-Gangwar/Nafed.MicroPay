
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
    
public partial class BranchSalaryHeadRule
{

    public int BranchFormulaID { get; set; }

    public string FieldName { get; set; }

    public int BranchID { get; set; }

    public bool FormulaColumn { get; set; }

    public bool FixedValueFormula { get; set; }

    public string LookUpHead { get; set; }

    public string LookUpHeadName { get; set; }

    public Nullable<decimal> FixedValue { get; set; }

    public Nullable<decimal> LowerRange { get; set; }

    public Nullable<decimal> UpperRange { get; set; }

    public bool IsDeleted { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public int CreatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }

    public Nullable<int> EmployeeTypeID { get; set; }



    public virtual Branch Branch { get; set; }

    public virtual User User { get; set; }

    public virtual User User1 { get; set; }

    public virtual EmployeeType EmployeeType { get; set; }

}

}
