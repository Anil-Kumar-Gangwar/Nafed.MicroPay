
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
    
public partial class EmployeeProcessApproval
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public EmployeeProcessApproval()
    {

        this.ConfirmationStatus = new HashSet<ConfirmationStatu>();

    }


    public int EmpProcessAppID { get; set; }

    public int ProcessID { get; set; }

    public int EmployeeID { get; set; }

    public int RoleID { get; set; }

    public int ReportingTo { get; set; }

    public Nullable<int> ReviewingTo { get; set; }

    public Nullable<int> AcceptanceAuthority { get; set; }

    public System.DateTime Fromdate { get; set; }

    public Nullable<System.DateTime> ToDate { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public int CreatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }

    public Nullable<bool> MultiReporting { get; set; }



    public virtual User User { get; set; }

    public virtual Process Process { get; set; }

    public virtual User User1 { get; set; }

    public virtual UserType UserType { get; set; }

    public virtual tblMstEmployee tblMstEmployee { get; set; }

    public virtual tblMstEmployee tblMstEmployee1 { get; set; }

    public virtual tblMstEmployee tblMstEmployee2 { get; set; }

    public virtual tblMstEmployee tblMstEmployee3 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ConfirmationStatu> ConfirmationStatus { get; set; }

}

}
