
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
    
public partial class AppraisalFormHistory
{

    public int HFormID { get; set; }

    public int FormID { get; set; }

    public string FormName { get; set; }

    public string ReportingYr { get; set; }

    public Nullable<System.DateTime> EmployeeSubmissionDueDate { get; set; }

    public Nullable<System.DateTime> ReportingSubmissionDueDate { get; set; }

    public Nullable<System.DateTime> ReviewerSubmissionDueDate { get; set; }

    public Nullable<System.DateTime> AcceptanceAuthSubmissionDueDate { get; set; }

}

}
