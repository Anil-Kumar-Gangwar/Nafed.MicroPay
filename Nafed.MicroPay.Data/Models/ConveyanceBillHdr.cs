
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
    
public partial class ConveyanceBillHdr
{

    public int ConveyanceHdrID { get; set; }

    public string Year { get; set; }

    public int EmployeeId { get; set; }

    public int ConveyanceBillDetailID { get; set; }

    public int StatusID { get; set; }

    public int CreatedBy { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }



    public virtual tblMstEmployee tblMstEmployee { get; set; }

    public virtual User User { get; set; }

    public virtual User User1 { get; set; }

    public virtual ConveyanceBillHdrDetail ConveyanceBillHdrDetail { get; set; }

}

}
