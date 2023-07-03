using System;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class Trainer
    {
        public int sno { set; get; }
        public int TrainerID { get; set; }
        public int TrainingID { get; set; }

        [Display(Name = "Trainer Name")]
        [Required(ErrorMessage = "Please Enter Trainer Name")]
        public string TrainerName { get; set; }

        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Please Enter Designation")]
        public string Designation { get; set; }

        [Display(Name = "Qualification")]
        [Required(ErrorMessage = "Please Enter Qualification")]
        public string Qualification { get; set; }

        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
    }

  
}