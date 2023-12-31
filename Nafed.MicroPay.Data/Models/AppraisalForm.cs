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
    
    public partial class AppraisalForm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AppraisalForm()
        {
            this.AppraisalFormHdrs = new HashSet<AppraisalFormHdr>();
            this.FormGroupAHdrs = new HashSet<FormGroupAHdr>();
            this.FormGroupAHdrs1 = new HashSet<FormGroupAHdr>();
            this.FormGroupBHdrs = new HashSet<FormGroupBHdr>();
            this.FormGroupCHdrs = new HashSet<FormGroupCHdr>();
            this.FormGroupDHdrs = new HashSet<FormGroupDHdr>();
            this.FormGroupEHdrs = new HashSet<FormGroupEHdr>();
            this.FormGroupGHdrs = new HashSet<FormGroupGHdr>();
            this.FormGroupHHdrs = new HashSet<FormGroupHHdr>();
            this.DesignationAppraisalForms = new HashSet<DesignationAppraisalForm>();
            this.APARReviewedSignedCopies = new HashSet<APARReviewedSignedCopy>();
            this.FormGroupTrainingDtls = new HashSet<FormGroupTrainingDtl>();
            this.FormGroupFHdrs = new HashSet<FormGroupFHdr>();
        }
    
        public int FormID { get; set; }
        public string ReportingYr { get; set; }
        public string FormName { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> EmployeeSubmissionDueDate { get; set; }
        public Nullable<System.DateTime> ReportingSubmissionDueDate { get; set; }
        public Nullable<System.DateTime> ReviewerSubmissionDueDate { get; set; }
        public Nullable<System.DateTime> AcceptanceAuthSubmissionDueDate { get; set; }
        public System.DateTime CreatedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AppraisalFormHdr> AppraisalFormHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupAHdr> FormGroupAHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupAHdr> FormGroupAHdrs1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupBHdr> FormGroupBHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupCHdr> FormGroupCHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupDHdr> FormGroupDHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupEHdr> FormGroupEHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupGHdr> FormGroupGHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupHHdr> FormGroupHHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DesignationAppraisalForm> DesignationAppraisalForms { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APARReviewedSignedCopy> APARReviewedSignedCopies { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupTrainingDtl> FormGroupTrainingDtls { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupFHdr> FormGroupFHdrs { get; set; }
    }
}
