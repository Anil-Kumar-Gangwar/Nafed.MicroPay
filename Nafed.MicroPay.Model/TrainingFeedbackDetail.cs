using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class TrainingFeedbackDetail
    {
        public int FeedbackDetailID { get; set; }
        public int TrainingID { get; set; }
        public int FeedBackFormHdrID { get; set; }      
        public string RatingType { get; set; }

        public Nullable<byte> UpperRatingValue { get; set; }
        [Display(Name = "Action Plan")]
                public string ActionPlan { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string TrainingTypeName { get; set; }
        public string TrainingTitle { get; set; }

        public List<TrainingFeedbackDetailListing> TrainingFeedbackDetailList { get; set; } = new List<TrainingFeedbackDetailListing>();
        public List<TrainingFeedbackDetailListing> TrainingFeedbackDetailPart2List { get; set; } = new List<TrainingFeedbackDetailListing>();
        public List<TrainingFeedbackDetailListing> TrainingFeedbackDetailPart3List { get; set; } = new List<TrainingFeedbackDetailListing>();
        public List<SelectListModel> RatingList { set; get; }

        public string TrainerType { get; set; }
        public string TrainingVenue { get; set; }
        public string TrainingInstructor { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

    }

    public class TrainingFeedbackDetailListing
    {
        public int FeedBackFormHdrID { get; set; }
     //   [Range(1, 10, ErrorMessage = "Please select Rating")]
        public Nullable<int> RatingNo { get; set; }
        public Nullable<int> Grade { get; set; }
        public int QuestionID { get; set; }
        public string SectionPrefix { get; set; }
        public string Section { get; set; }
        public int DisplayOrder { get; set; }
        public bool DisplayInBold { get; set; }        
       // [Range(1, 50, ErrorMessage = "Please select Grade")]
        public EnumRatingGrade enumRatingGrade { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string GradeName { get; set; }
        public int PartNo { get; set; }
        [Required(ErrorMessage = "Please select Scale")]
        [Range(0, 10, ErrorMessage = "Please select Scale")]
        public EnumRatingNo? enumRatingNo { get; set; }
    }
    public enum EnumRatingGrade
    {
        OUTSTANDING = 1,
        [Display(Name = "VERY GOOD")]
        VERYGOOD = 2,
        GOOD = 3,
        FAIR = 4,
        POOR = 5
    }

    public enum EnumRatingNo
    {
        [Display(Name = "0")]
        one = 1,
        [Display(Name = "1")]
        two = 2,
        [Display(Name = "2")]
        three = 3,
        [Display(Name = "3")]
        four = 4,
        [Display(Name = "4")]
        five = 5,
        [Display(Name = "5")]
        six = 6
    }
}

