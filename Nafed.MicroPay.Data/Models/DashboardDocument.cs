
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
    
public partial class DashboardDocument
{

    public int DocID { get; set; }

    public int DocTypeID { get; set; }

    public string DocumentName { get; set; }

    public string DocumentDesc { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }

    public string DocumentPathName { get; set; }



    public virtual DashboardDocumentHdr DashboardDocumentHdr { get; set; }

    public virtual User User { get; set; }

    public virtual User User1 { get; set; }

}

}
