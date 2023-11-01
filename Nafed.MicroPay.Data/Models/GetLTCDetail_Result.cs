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
    
    public partial class GetLTCDetail_Result
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string ReportingTo { get; set; }
        public string ReportingDesignation { get; set; }
        public string ReviewerTo { get; set; }
        public string ReviewerDesignation { get; set; }
        public string AcceptanceAuth { get; set; }
        public string AcceptanceDesignation { get; set; }
        public Nullable<System.DateTime> RTSenddate { get; set; }
        public Nullable<System.DateTime> RVSenddate { get; set; }
        public Nullable<System.DateTime> AASenddate { get; set; }
        public Nullable<System.DateTime> EmpSenddate { get; set; }
        public Nullable<int> LTCID { get; set; }
        public Nullable<int> LTCNo { get; set; }
        public Nullable<int> BranchID { get; set; }
        public string DependentsList { get; set; }
        public Nullable<bool> WhetherSelf { get; set; }
        public Nullable<int> HomeTown { get; set; }
        public string WhereDetail { get; set; }
        public Nullable<decimal> Distance { get; set; }
        public Nullable<decimal> InitialcalAmount { get; set; }
        public Nullable<System.DateTime> DateAvailLTC { get; set; }
        public Nullable<System.DateTime> DateofReturn { get; set; }
        public Nullable<System.DateTime> DateofApplication { get; set; }
        public Nullable<decimal> TentativeAdvance { get; set; }
        public Nullable<decimal> LTCBillAmt { get; set; }
        public Nullable<decimal> Settlementdone { get; set; }
        public Nullable<int> Natureofleave { get; set; }
        public string Detail { get; set; }
        public Nullable<System.DateTime> LeaveFrom { get; set; }
        public Nullable<System.DateTime> LeaveTo { get; set; }
        public string SpouseOrg { get; set; }
        public string EmployeeCode { get; set; }
        public string DesignationName { get; set; }
        public string BranchName { get; set; }
        public Nullable<short> FormStatus { get; set; }
        public string EMPDepartment { get; set; }
        public string SHComment { get; set; }
        public string DHComment { get; set; }
        public string DealingAssistant { get; set; }
    }
}
