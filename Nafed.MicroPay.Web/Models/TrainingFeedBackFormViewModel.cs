using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MicroPay.Web.Models
{
    public class TrainingFeedBackFormViewModel
    {
        public int TrainingID { set; get; }
        public int FeedBackFormHdrID { get; set; }
        public string RatingType { get; set; }
        [Display(Name = "Lower Rating Value")]
      //  [Required(ErrorMessage ="Please enter Lower Rating")]
        public Nullable<byte> LowerRatingValue { get; set; }
        [Display(Name = "Upto Rating Value")]
    //    [Required(ErrorMessage = "Please enter Upto Rating")]
        public Nullable<byte> UpperRatingValue { get; set; }
        public List<TrainingFeedBackFormDetail> TrainingFeedBackFormList { get; set; }
        public List<TrainingFeedBackFormDetail> TrainingFeedBackFormPart2List { get; set; }
        public List<TrainingFeedBackFormDetail> TrainingFeedBackFormPart3List { get; set; }
        [Display(Name = "Rating Type")]
      //  [Range(1, 2, ErrorMessage = "Please Choose Rating Type")]
        public EnumRatingType enumRatingType { set; get; }
        //[Display(Name = "Employee Action Plan")]
        //public string ActionPlan { get; set; }

        [Display(Name = "Number")]
        public bool RatingNumber { get; set; }

        [Display(Name = "Grade")]
        public bool RatingGrade { get; set; }       
        public Training _Training { get; set; }

        
    }

    public enum EnumRatingType
    {
        Number = 1,  
        Grade = 2    
    }  
}