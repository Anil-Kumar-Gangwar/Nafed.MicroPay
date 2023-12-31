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
    
    public partial class CandidateRegistration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CandidateRegistration()
        {
            this.CandidateGovtWorkExperiences = new HashSet<CandidateGovtWorkExperience>();
            this.CandidateWorkExperiences = new HashSet<CandidateWorkExperience>();
            this.CandidateEducationSummaries = new HashSet<CandidateEducationSummary>();
        }
    
        public int RegistrationID { get; set; }
        public string RegistrationNo { get; set; }
        public int RequirementID { get; set; }
        public string CandidateFullName { get; set; }
        public string CandidatePicture { get; set; }
        public string CandidateSignature { get; set; }
        public string PersonalEmailID { get; set; }
        public string MobileNo { get; set; }
        public string Husband_FatherName { get; set; }
        public System.DateTime DOB { get; set; }
        public int Age { get; set; }
        public Nullable<int> BGroupID { get; set; }
        public Nullable<int> ReligionID { get; set; }
        public Nullable<int> MaritalStsID { get; set; }
        public int NationalityID { get; set; }
        public string PmtAdd { get; set; }
        public string PmtStreet { get; set; }
        public string PmtCity { get; set; }
        public string PmtPin { get; set; }
        public Nullable<int> PmtStateID { get; set; }
        public string AlternateEmailId { get; set; }
        public bool HaveYouEverAppliedinNafedBefore { get; set; }
        public string IfYesPleaseExplain { get; set; }
        public Nullable<System.DateTime> AppliedDate { get; set; }
        public bool FriendsAndRelativesInNafed { get; set; }
        public string IfYesStateNameAndRelationship { get; set; }
        public bool Declaration { get; set; }
        public string DeclarationName { get; set; }
        public string DeclarationPlace { get; set; }
        public System.DateTime DeclarationDate { get; set; }
        public bool PaymentReceived { get; set; }
        public int PaymentType { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string PaymentTransactionID { get; set; }
        public Nullable<int> TotalExperienceInYear { get; set; }
        public Nullable<int> TotalExperienceInMonth { get; set; }
        public Nullable<int> RelevantExperienceInYear { get; set; }
        public Nullable<int> RelevantExperienceInMonth { get; set; }
        public Nullable<bool> EligibleForWrittenExam { get; set; }
        public Nullable<bool> EligibleForGDExam { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public int GenderID { get; set; }
        public Nullable<int> ZoneAppliedFor { get; set; }
        public Nullable<int> GovtExperienceInYear { get; set; }
        public Nullable<int> GovtExperienceInMonth { get; set; }
        public string EarilerAppliedPostInNAFED { get; set; }
        public Nullable<int> GovtReleExpInYear { get; set; }
        public Nullable<int> GovtReleExpInMonth { get; set; }
        public Nullable<int> BranchID { get; set; }
        public decimal AnnualGrossSalary { get; set; }
        public Nullable<short> FormStatus { get; set; }
        public bool IssueAdmitCard { get; set; }
        public string EligiblityRemark { get; set; }
    
        public virtual BloodGroup BloodGroup { get; set; }
        public virtual Branch Branch { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CandidateGovtWorkExperience> CandidateGovtWorkExperiences { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual MaritalStatu MaritalStatu { get; set; }
        public virtual Religion Religion { get; set; }
        public virtual Requirement Requirement { get; set; }
        public virtual State State { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CandidateWorkExperience> CandidateWorkExperiences { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CandidateEducationSummary> CandidateEducationSummaries { get; set; }
    }
}
