using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class TrainingDateWiseTimeSlot
    {
        public int sNo { set; get; }

        public int TrainingTimeSlotID { get; set; }
        public int TrainingID { get; set; }

        [Display(Name = "Date")]
        public System.DateTime TrainingDate { get; set; }

        [Display(Name = "From Time")]
        [Required(ErrorMessage = "Please Select From Time")]
        public System.TimeSpan? FromTime { get; set; }

        [Display(Name = "To Time")]
        [Required(ErrorMessage = "Please Select To Time")]
        public System.TimeSpan? ToTime { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }

    }
}
