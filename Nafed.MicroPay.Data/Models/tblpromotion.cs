
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
    
public partial class tblpromotion
{

    public int TransID { get; set; }

    public string EmployeeCode { get; set; }

    public string CadreCode { get; set; }

    public string DesignationCode { get; set; }

    public Nullable<System.DateTime> FromDate { get; set; }

    public Nullable<System.DateTime> ToDate { get; set; }

    public Nullable<int> SeniorityCode { get; set; }

    public Nullable<System.DateTime> ConfirmationDate { get; set; }

    public Nullable<bool> Confirmed { get; set; }

    public Nullable<System.DateTime> UpdateDate { get; set; }

    public Nullable<decimal> E_Basic { get; set; }

    public Nullable<decimal> OldBasic { get; set; }

    public string WayOfPosting { get; set; }

    public Nullable<bool> NewTS { get; set; }

    public string DESGSTUFF { get; set; }

    public string SCALE { get; set; }

    public string DesgName { get; set; }

    public string NotionalIncrement { get; set; }

    public string AnnualIncrementWEF { get; set; }

    public string NextIncremantdate { get; set; }

    public string NewBasic { get; set; }

    public string FLAG { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public int CreatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }

    public Nullable<int> EmployeeID { get; set; }

    public Nullable<int> CadreID { get; set; }

    public Nullable<int> DesignationID { get; set; }

    public Nullable<int> WayOfPostingID { get; set; }

    public bool IsDeleted { get; set; }

    public Nullable<System.DateTime> OrderOfPromotion { get; set; }



    public virtual Cadre Cadre { get; set; }

    public virtual tblMstEmployee tblMstEmployee { get; set; }

    public virtual Designation Designation { get; set; }

}

}
