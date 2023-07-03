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
    
    public partial class ConfirmationStatu
    {
        public int CnfStatusID { get; set; }
        public Nullable<int> FormHdrID { get; set; }
        public int EmployeeID { get; set; }
        public int StatusID { get; set; }
        public int EmpProcessAppID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public int ProcessId { get; set; }
        public Nullable<System.DateTime> DueConfirmationDate { get; set; }
        public bool CertificatesReceived { get; set; }
        public bool PoliceVerification { get; set; }
        public Nullable<byte> Point8_1 { get; set; }
        public Nullable<byte> Point8_2 { get; set; }
        public Nullable<byte> Point8_3 { get; set; }
        public Nullable<byte> Point8_4 { get; set; }
        public Nullable<byte> Point8_5 { get; set; }
        public Nullable<byte> Point8_6 { get; set; }
        public Nullable<byte> Point8_7 { get; set; }
        public Nullable<byte> Point8_8 { get; set; }
        public Nullable<byte> Point8_9 { get; set; }
        public Nullable<byte> Point8_10 { get; set; }
        public Nullable<byte> Point8_11 { get; set; }
        public Nullable<byte> Point8_12 { get; set; }
        public Nullable<byte> Point8_13 { get; set; }
        public bool Point9 { get; set; }
        public string Point9_Remark { get; set; }
        public bool Point10 { get; set; }
        public string Point10_Remark { get; set; }
        public bool Point11 { get; set; }
        public string Point11_Remark { get; set; }
        public bool Point12 { get; set; }
        public string Point12_Remark { get; set; }
        public Nullable<bool> ConfirmationRecommendedReporting { get; set; }
        public bool ConfirmationRecommended { get; set; }
        public string ReportingOfficerComment { get; set; }
        public string ReviewingOfficerComment { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> FormState { get; set; }
        public string PersonalSectionRemark { get; set; }
        public Nullable<bool> PRSubmit { get; set; }
        public bool ConfirmationClarification { get; set; }
        public bool IsRejected { get; set; }
        public Nullable<int> FormAHeaderID { get; set; }
        public Nullable<int> FormBHeaderID { get; set; }
        public Nullable<byte> RVPoint8_1 { get; set; }
        public Nullable<byte> RVPoint8_2 { get; set; }
        public Nullable<byte> RVPoint8_3 { get; set; }
        public Nullable<byte> RVRVPoint8_4 { get; set; }
        public Nullable<byte> RVRVPoint8_5 { get; set; }
        public Nullable<byte> RVPoint8_6 { get; set; }
        public Nullable<byte> RVPoint8_7 { get; set; }
        public Nullable<byte> RVPoint8_8 { get; set; }
        public Nullable<byte> RVPoint8_9 { get; set; }
        public Nullable<byte> RVPoint8_10 { get; set; }
        public Nullable<byte> RVPoint8_11 { get; set; }
        public Nullable<byte> RVPoint8_12 { get; set; }
        public Nullable<byte> RVPoint8_13 { get; set; }
        public bool RVPoint9 { get; set; }
        public string RVPoint9_Remark { get; set; }
        public bool RVPoint10 { get; set; }
        public string RVPoint10_Remark { get; set; }
        public bool RVPoint11 { get; set; }
        public string RVPoint11_Remark { get; set; }
        public bool RVPoint12 { get; set; }
        public string RVPoint12_Remark { get; set; }
    
        public virtual EmployeeProcessApproval EmployeeProcessApproval { get; set; }
        public virtual tblMstEmployee tblMstEmployee { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ConfirmationFormAHeader ConfirmationFormAHeader { get; set; }
        public virtual ConfirmationFormBHeader ConfirmationFormBHeader { get; set; }
        public virtual ConfirmationFormHdr ConfirmationFormHdr { get; set; }
        public virtual ConfirmationFormHdr ConfirmationFormHdr1 { get; set; }
        public virtual ConfirmationFormHdr ConfirmationFormHdr2 { get; set; }
    }
}
