
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
    
public partial class tblMstHillComp
{

    public int BranchID { get; set; }

    public string BranchCode { get; set; }

    public decimal UpperLimit { get; set; }

    public Nullable<decimal> Amount { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public int CreatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }



    public virtual Branch Branch { get; set; }

}

}
