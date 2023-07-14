
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
    
public partial class EmployeeLeave
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public EmployeeLeave()
    {

        this.tblLeaveTrans = new HashSet<tblLeaveTran>();

    }


    public int LeaveID { get; set; }

    public int EmployeeId { get; set; }

    public Nullable<System.DateTime> DateFrom { get; set; }

    public Nullable<System.DateTime> DateTo { get; set; }

    public int LeaveCategoryID { get; set; }

    public int StatusID { get; set; }

    public string Reason { get; set; }

    public decimal Unit { get; set; }

    public Nullable<int> ReportingOfficer { get; set; }

    public decimal LeaveBalance { get; set; }

    public string DocumentName { get; set; }

    public string ServerDocumentFilePath { get; set; }

    public int LeavePurposeID { get; set; }

    public string PurposeOthers { get; set; }

    public int CreatedBy { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }

    public bool IsDeleted { get; set; }

    public Nullable<System.DateTime> VisitFrom { get; set; }

    public Nullable<int> DateFrom_DayType { get; set; }

    public Nullable<int> DateTo_DayType { get; set; }

    public string ReportingToRemark { get; set; }

    public string ReviewerToRemark { get; set; }

    public Nullable<int> ReviewerTo { get; set; }

    public Nullable<bool> IsWithdrawl { get; set; }



    public virtual User User { get; set; }

    public virtual LeavePurpose LeavePurpose { get; set; }

    public virtual LeaveStatu LeaveStatu { get; set; }

    public virtual User User1 { get; set; }

    public virtual LeaveCategory LeaveCategory { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblLeaveTran> tblLeaveTrans { get; set; }

    public virtual tblMstEmployee tblMstEmployee { get; set; }

}

}
