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
    
    public partial class GetConveyanceFormDetails_Result
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string Branch { get; set; }
        public string ReportingTo { get; set; }
        public string ReportingDesignation { get; set; }
        public string ReviewerTo { get; set; }
        public string ReviewerDesignation { get; set; }
        public string AcceptanceAuth { get; set; }
        public string AcceptanceDesignation { get; set; }
        public string ReportingYear { get; set; }
        public Nullable<int> ConveyanceBillDetailID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public Nullable<System.DateTime> Dated { get; set; }
        public string Section { get; set; }
        public Nullable<decimal> TotalAmountClaimed { get; set; }
        public Nullable<bool> VehicleProvided { get; set; }
        public Nullable<int> FormState { get; set; }
        public Nullable<bool> IsReportingRejected { get; set; }
        public Nullable<bool> IsReviewingRejected { get; set; }
        public string ReportingRemarks { get; set; }
        public string ReviewingRemarks { get; set; }
    }
}
