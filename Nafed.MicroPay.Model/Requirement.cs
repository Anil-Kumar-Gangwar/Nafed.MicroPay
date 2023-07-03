using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Nafed.MicroPay.Model
{
    public class Requirement
    {
        public int RequirementID { get; set; }
        [Display(Name = "Designation")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Designation")]
        public int DesinationID { get; set; }
        [Display(Name = "Post")]
        [Required(ErrorMessage = "Post Description is required.")]
        public string Post { get; set; }
        [Display(Name = "No of Position")]
        [Required(ErrorMessage = "No of Position is required.")]
        [Range(1, 100, ErrorMessage = "No of Position is required.")]
        public int NoofPosition { get; set; }
        [Display(Name = "Fees Applicable")]
        public bool FeesApplicable { get; set; }
        [Display(Name = "Application Fees")]
        public Nullable<decimal> ApplicationFees { get; set; }
        [Display(Name = "How to pay")]
        public string HowToPay { get; set; }
        [Display(Name = "Publish Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Publish Date is required.")]
        public Nullable<System.DateTime> PublishDate { get; set; }
        [Display(Name = "Pay Scale/Fixed Monthly Revaluation")]
        [Required(ErrorMessage = "Pay Scale is required.")]
        public string PayScale { get; set; }
        [Display(Name = "Last Date of Application Received")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Last Date of Application Received is required.")]
        public Nullable<System.DateTime> LastDateofApplicationReceived { get; set; }
        [Display(Name = "Minimum Qualification")]
        [Required(ErrorMessage = "Minimum Qualification is required.")]
        public string MinimumQualification { get; set; }
        [Display(Name = "Minimum Experience Detail")]
        [Required(ErrorMessage = "Minimum Experience Detail is required.")]
        public string MinimumExperinceDetail { get; set; }
        [Display(Name = "Minimum Experience No")]
        [Required(ErrorMessage = "Minimum Experience No is required.")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Minimum Experience is required.")]
        public int MimimumExpenrienceNo { get; set; }
        [Display(Name = "Desired Key Skills")]

        public string DesiredKeySkills { get; set; }
        [Display(Name = "Optional Key Skills")]

        public string OptionalKeySkills { get; set; }
        [Display(Name = "Essential Duties and Responsibilities")]
        [Required(ErrorMessage = "Essential Duties and Responsibilities is required.")]
        public string Essential_Duties_and_Responsibilities { get; set; }
        [Display(Name = "Maximum Age Limit")]
        [Required(ErrorMessage = "Maximum Age Limit is required.")]
        [Range(18, 100, ErrorMessage = "Maximum age limit should be 18 or 18 plus.")]
        public Nullable<decimal> MaximumAgeLimit { get; set; }
        [Display(Name = "Maximum Age Limit Desc")]
        [Required(ErrorMessage = "Maximum Age Limit Desc is required.")]
        public string MaximumAgeLimitDesc { get; set; }
        [Display(Name = "Method of Recruitment")]
        [Required(ErrorMessage = "Method of Recruitment is required.")]
        public string MethodofRecruitment { get; set; }
        [Display(Name = "Branch")]
        public Nullable<int> BranchID { get; set; }
        [Display(Name = "Instructions")]
        [Required(ErrorMessage = "Instructions is required.")]
        public string INSTRUCTIONS { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string DesignationName { get; set; }
        public string BranchName { get; set; }
        [Display(Name = "Written Exam Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Written Exam Date is required.")]
        public Nullable<System.DateTime> WrittenExamDate { get; set; }
        [Display(Name = "GD Exam Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> GDExamDate { get; set; }

        public RequirementOptionType requirementOptionType { set; get; }
        [DisplayName("Anywhere In India")]
        public bool AnyWhereInIndia { get; set; }
        [DisplayName("Specific Branch")]
        public bool SpecificBranch { get; set; }
        [DisplayName("Specific Zone")]
        public bool SpecificZone { get; set; }

        public EnumZoneAppliedFor enumZoneAppliedFor { set; get; }
        public int? ZoneApplied { get; set; }

        public Nullable<int> Employementtype { get; set; }
        [Required(ErrorMessage = "Minimum Age Limit is required.")]
        [Range(0, 100, ErrorMessage = "Minimum age limit should be 18 or 18 plus.")]
        [Display(Name = "Minimum Age Limit")]
        public Nullable<decimal> MinimumAgeLimits { get; set; }
        [Display(Name = "Qualification")]
        public string Qualification { get; set; }
        public string ZoneAppliedFor { get; set; }
        public EmployementType enumEmployementType { set; get; }
        public string employementName { get; set; }

        public int[] jobLocations { set; get; }
        public int JLocTypeId { get; set; }

        public dynamic CheckboxList { get; set; }

        public dynamic QualificationCheckboxList { get; set; }

        public int editFlag { get; set; }
        public List<JobRequirementLocation> jobRequirementLocs { set; get; }
        public List<JobRequirementQualification> JobRequirementQualification { get; set; } = new List<Model.JobRequirementQualification>();
        public int[] JobQualification { set; get; }

        public string LocationName { get; set; }
        public List<Model.SelectListModel> LocationList { set; get; }

        public List<JobExamCenterDetails> JobExamCenterDetails { get; set; }

        public int ModelIsValid { get; set; }

    }

    public enum RequirementOptionType
    {
        [Display(Name = "Branch")]
        SpecificBranch = 1,
        [Display(Name = "Zone")]
        SpecificZone = 2,
        [Display(Name = "Anywhere in India ")]
        Anywhereinindia = 3
    }

    public enum EmployementType
    {
        Regular = 1,
        Contract = 2,
        Consultant = 3,
            Advisor = 4
    }

}
