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
    
    public partial class ConfirmationFormAHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ConfirmationFormAHeader()
        {
            this.ConfirmationStatus = new HashSet<ConfirmationStatu>();
            this.ConfirmationClarifications = new HashSet<ConfirmationClarification>();
        }
    
        public int FormAHeaderId { get; set; }
        public int EmployeeId { get; set; }
        public int DesignationId { get; set; }
        public int BranchId { get; set; }
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
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> FormState { get; set; }
        public int FormHdrID { get; set; }
        public string PersonalSectionRemark { get; set; }
        public Nullable<bool> PRSubmit { get; set; }
        public bool ConfirmationClarification { get; set; }
        public string FileNo { get; set; }
        public string GeneralManager { get; set; }
        public string ManagingDirector { get; set; }
        public string GmEmailID { get; set; }
        public string EMail1 { get; set; }
        public string EMail2 { get; set; }
        public string EMail3 { get; set; }
        public string EMail4 { get; set; }
        public string EMail5 { get; set; }
        public string DVHeadEmpCode { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual Process Process { get; set; }
        public virtual tblMstEmployee tblMstEmployee { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfirmationStatu> ConfirmationStatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfirmationClarification> ConfirmationClarifications { get; set; }
        public virtual ConfirmationFormHdr ConfirmationFormHdr { get; set; }
    }
}
