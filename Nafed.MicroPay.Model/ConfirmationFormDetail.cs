using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ConfirmationFormDetail
    {
        public int FormAHeaderId { get; set; }
        public int FormBHeaderId { get; set; }
        public int EmployeeId { get; set; }
        public int DesignationId { get; set; }
        public int BranchId { get; set; }
        public int ProcessId { get; set; }
        //public Nullable<System.DateTime> DueConfirmationDate { get; set; }
        public bool CertificatesReceived { get; set; }
        public bool PoliceVerification { get; set; }
        [Display(Name = "Intelligance & general ability")]
        public Nullable<byte> Point8_1 { get; set; }
        [Display(Name = "Job knowledge")]
        public Nullable<byte> Point8_2 { get; set; }
        [Display(Name = "Initiative")]
        public Nullable<byte> Point8_3 { get; set; }
        [Display(Name = "Promptness & accuracy in doing work")]
        public Nullable<byte> Point8_4 { get; set; }
        [Display(Name = "Reliability")]
        public Nullable<byte> Point8_5 { get; set; }
        [Display(Name = "Punctuality in attendance")]
        public Nullable<byte> Point8_6 { get; set; }
        [Display(Name = "Capacity for assuming responsibility")]
        public Nullable<byte> Point8_7 { get; set; }
        [Display(Name = "Integrity")]
        public Nullable<byte> Point8_8 { get; set; }
        [Display(Name = "Ability to control subordinates")]
        public Nullable<byte> Point8_9 { get; set; }
        [Display(Name = "How to officer performed the specific duties & responsibilities assigned to him")]
        public Nullable<byte> Point8_10 { get; set; }
        [Display(Name = "Self expression (Oral/ in Writing)")]
        public Nullable<byte> Point8_11 { get; set; }
        [Display(Name = "Decision making")]
        public Nullable<byte> Point8_12 { get; set; }
        [Display(Name = "Knowledge of trends, developments, new techniques, current practices etc. pertaining to his/her work")]
        public Nullable<byte> Point8_13 { get; set; }
        [Display(Name = "Any lapses on the part of the officer")]
        public bool Point9 { get; set; }
        [Display(Name = "Remark")]
        public string Point9_Remark { get; set; }
        [Display(Name = "Whether any disciplinary case is pending against him/her")]
        public bool Point10 { get; set; }
        [Display(Name = "Remark")]
        public string Point10_Remark { get; set; }
        [Display(Name = "Give details of the job assigned to the offical job assigned.")]
        public bool Point11 { get; set; }
        [Display(Name = "Remark")]
        public string Point11_Remark { get; set; }
        [Display(Name = "Whether any target for doing the work was fixed for the officer? If yes, whether the targets were achived? Please give details")]
        public bool Point12 { get; set; }
        [Display(Name = "Remark")]
        public string Point12_Remark { get; set; }
        [Display(Name = "RECOMMENDED FOR CONFIRMATION/ NOT RECOMMENDED FOR CONFIRMATION")]
        public bool ConfirmationRecommended { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        //public Nullable<DateTime> DOJ { get; set; }
        public Nullable<DateTime> appointmentConfirmationDate { get; set; }
        public Nullable<DateTime> promotionConfirmationDate { get; set; }

        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeName { get; set; }

        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_1_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_2_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_3_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_4_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_5_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_6_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_7_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_8_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_9_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_10_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_11_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_12_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide point8_13_Gr { set; get; }


        public OptionType point_9_Option { set; get; }

        public OptionType point_10_Option { set; get; }

        public OptionType point_11_Option { set; get; }

        public OptionType point_12_Option { set; get; }

        public OptionType recommnded_Option { set; get; }

        public OptionType recommnded_ReviewingOption { set; get; }

        public string PayScale { get; set; }
        public int FormTypeId { get; set; }
        public bool IsReviewingOfficer { get; set; }
        public Nullable<int> FormState { get; set; }
        //public DateTime orderOfPromotion { get; set; }
        //public DateTime DOJ { get; set; }
        //public DateTime DueConfirmationDate { get; set; }
        public string orderOfPromotion { get; set; }
        public string DOJ { get; set; }
        public string DueConfirmationDate { get; set; }
        [Display(Name = "RECOMMENDED FOR CONFIRMATION/ NOT RECOMMENDED FOR CONFIRMATION")]
        public bool ConfirmationRecommendedReporting { get; set; }
        public string ReportingOfficerComment { get; set; }
        [Required(ErrorMessage = "Reviewing Officer Comment Required")]
        public string ReviewingOfficerComment { get; set; }
        public bool ConfirmationClarification { get; set; }

        [Display(Name = "Reporting Officer Clarification Remark")]
        public string ROClarificationRemark { set; get; }

        public string CalrificationRemark { get; set; }
        public string EmployeeCode { get; set; }

        //--------------------- Reveiwer Points ------------------------


        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_1_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_2_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_3_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_4_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_5_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_6_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_7_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_8_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_9_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_10_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_11_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_12_Gr { set; get; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select one of them.")]
        public ConfirmationRatingGuide RVpoint8_13_Gr { set; get; }

        public OptionType RVpoint_9_Option { set; get; }

        public OptionType RVpoint_10_Option { set; get; }

        public OptionType RVpoint_11_Option { set; get; }

        public OptionType RVpoint_12_Option { set; get; }



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
    }

    public enum ConfirmationRatingGuide
    {
        [Display(Name = "OUTSTANDING")]
        OUTSTANDING = 1,
        [Display(Name = "VERY GOOD")]
        VERYGOOD = 2,
        [Display(Name = "GOOD")]
        GOOD = 3,
        [Display(Name = "FAIR")]
        FAIR = 4,
        [Display(Name = "POOR")]
        POOR = 5
    }

    public enum OptionType
    {
        Yes = 1,
        No = 2,
        Clarification = 3
    }
}
