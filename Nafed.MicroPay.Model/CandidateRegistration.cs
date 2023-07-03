using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class CandidateRegistration
    {
        public string CPhotoUNCPath { set; get; }
        public string CSignatureUNCPath { set; get; }

        [Display(Name = "Name of the Post:")]
        public string PostName { set; get; }

        [Display(Name = "Location:")]
        public string AppliedJobLocation { set; get; }

        public string GenderName { set; get; }

        public string ReligionName { set; get; }

        public string Nationality { set; get; }

        public string MaritalStatus { set; get; }

        public string StateName { set; get; }

        public int DesignationID { set; get; }
        public int RegistrationID { get; set; }
        public string RegistrationNo { get; set; }
        public int RequirementID { get; set; }

        [Required(ErrorMessage = "Please Enter Candidate Name")]
        [Display(Name = "Candidate Name:")]
        public string CandidateFullName { get; set; }

        public string CandidatePicture { get; set; }
        public string CandidateSignature { get; set; }

        [Required(ErrorMessage = "Please Enter Personal EmailID")]
        [Display(Name = "Personal EmailID:")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email.")]
        public string PersonalEmailID { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(10, ErrorMessage = "{0} not be exceed 10 char")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid mobile number")]
        [Required(ErrorMessage = "Please Enter Mobile No")]
        [Display(Name = "Mobile No:")]
        public string MobileNo { get; set; }

        [Display(Name = "Father's Name:")]
        [Required(ErrorMessage = "Please Father's Name")]
        public string Husband_FatherName { get; set; }

        [MinimumAge(18)]
        [Display(Name = "Date of Birth:")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? DOB { get; set; }

        [Display(Name = "Gender:")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Gender")]
        public Nullable<int> GenderID { get; set; }

        public int Age { get; set; }

        [Display(Name = "Blood Group:")]
        public Nullable<int> BGroupID { get; set; }

        [Display(Name = "Religion:")]
        public Nullable<int> ReligionID { get; set; }

        [Display(Name = "Marital Status:")]
        public Nullable<int> MaritalStsID { get; set; }

        [Display(Name = "Nationality:")]
        public int? NationalityID { get; set; }

        [Required]
        [Display(Name = "Address:")]
        public string PmtAdd { get; set; }

        [Required]
        [Display(Name = "Street:")]
        public string PmtStreet { get; set; }

        [Required]
        [Display(Name = "City:")]
        public string PmtCity { get; set; }

        [Display(Name = "Pin:")]
        [Required]
        public string PmtPin { get; set; }

        [Display(Name = "State:")]
        public Nullable<int> PmtStateID { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email.")]
        [Display(Name = "Alternate EmailID:")]
        public string AlternateEmailId { get; set; }

        public bool HaveYouEverAppliedinNafedBefore { get; set; }
        public string IfYesPleaseExplain { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AppliedDate { get; set; }
        public bool FriendsAndRelativesInNafed { get; set; }
        public string IfYesStateNameAndRelationship { get; set; }

        [Range(1, Int16.MaxValue, ErrorMessage = "Please select declaration statement")]
        public bool Declaration { get; set; }

        [Display(Name = "Name:")]

        [Required(ErrorMessage = "Please enter name")]
        public string DeclarationName { get; set; }

        [Display(Name = "Place:")]
        [Required(ErrorMessage = "Please enter place")]
        public string DeclarationPlace { get; set; }

        [Display(Name = "Date:")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter date")]

        public System.DateTime? DeclarationDate { get; set; }
        public Nullable<int> ZoneAppliedFor { get; set; }
        public bool PaymentReceived { get; set; }
        [Display(Name = "Payment Mode:")]
        public int PaymentType { get; set; }
        public string PaymentTypeName { get; set; }
        [Display(Name = "Payment Date:")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> PaymentDate { get; set; }
        [Display(Name = "UTR No./Transaction ID:")]
        public string PaymentTransactionID { get; set; }
        public Nullable<int> TotalExperienceInYear { get; set; }
        public Nullable<int> TotalExperienceInMonth { get; set; }
        public Nullable<int> RelevantExperienceInYear { get; set; }
        public Nullable<int> RelevantExperienceInMonth { get; set; }
        public Nullable<int> GovtExperienceInYear { get; set; }
        public Nullable<int> GovtExperienceInMonth { get; set; }
        public Nullable<bool> EligibleForWrittenExam { get; set; }
        public Nullable<bool> EligibleForGDExam { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string EarilerAppliedPostInNAFED { get; set; }
        public Nullable<int> GovtReleExpInYear { get; set; }
        public Nullable<int> GovtReleExpInMonth { get; set; }
        public Nullable<int> BranchID { get; set; }
        public EnumZoneAppliedFor enumZoneAppliedFor { set; get; }
        public List<SelectListModel> JobLocation { get; set; } = new List<SelectListModel>();

        [Required(ErrorMessage = "Please select location.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select location.")]
        public int SelectedLocationID { get; set; }
        public int JobLocTypeID { get; set; }
        public int JLocTypeId { get; set; }
        public RequirementOptionType reqOptionType { set; get; }
        public List<CandidateGovtWorkExperience> candidateGovtWrkExp { set; get; } = new List<CandidateGovtWorkExperience>();
        public EnumHaveYouEverAppliedinNafedBefore enumhaveYouEverAppliedInNafedBefore { set; get; }
        public EnumFriendsAndRelativesInNafed enumfriendsAndRelativesInNafed { set; get; }
        public List<CandidateWorkExperince> candidateWorkExperience { get; set; }
        public List<CandidateEducationSummary> CandidateEducationSummary { get; set; }
        public EnumNatureOfAppointment enumNatureOfAppointment { set; get; }
        public int otpType { set; get; } = 1;
        public List<JobRequirementQualification> JobRequirementQualification { get; set; } = new List<Model.JobRequirementQualification>();

        public Nullable<decimal> JobMinimumAgeValue { get; set; }
        public Nullable<decimal> JobMaximumAgeValue { get; set; }

        [Display(Name = "Annual Gross Salary:")]
        [Required(ErrorMessage = "Please enter annual gross salary")]
        public decimal? AnnualGrossSalary { get; set; }
        public Nullable<short> FormStatus { get; set; }

        public EnumPaymentType enumPaymentType { get; set; }

        public bool FeesApplicable { get; set; }
        public int MimimumExpenrienceNo { set; get; }
        public bool IssueAdmitCard { get; set; }
        public string EligiblityRemark { get; set; }


    }

    public enum EnumZoneAppliedFor
    {
        [Display(Name = "Eastern India")]
        East = 1,
        [Display(Name = "Western India")]
        West = 2,
        [Display(Name = "Northern India")]
        North = 3,
        [Display(Name = "Southern India")]
        South = 4

    }

    public enum EnumHaveYouEverAppliedinNafedBefore
    {
        Yes = 1,
        No = 0
    }

    public enum EnumFriendsAndRelativesInNafed
    {
        Yes = 1,
        No = 0
    }
    public enum EnumNationality
    {
        Indian = 1
    }
    //public enum EnumNatureOfAppointment
    //{
    //    Regular=1,
    //    Adhoc=2,
    //    Deputation =3
    //}


    public enum EnumReqQualification
    {
        [Display(Name = "10th")]
        Tenth = 1,
        [Display(Name = "12th")]
        Twelveth = 2,
        [Display(Name = "Graduation")]
        Graduation = 3,
        [Display(Name = "Post Graduation")]
        PostGraduation = 4,
        [Display(Name = "Diploma")]
        Diploma = 5,
        [Display(Name = "Professional")]
        Professional = 6

    }

    public enum EnumPaymentType
    {
        NEFT = 1,
        IMPS = 2,
        RTGS = 3


    }

}
