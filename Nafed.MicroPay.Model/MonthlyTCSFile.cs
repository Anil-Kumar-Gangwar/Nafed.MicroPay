using System;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class MonthlyTCSFile
    {
        [Display(Name = "Month")]
        [Required(ErrorMessage ="Please select month")]
        public int month { set; get; }
        
        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please select year")]
        public int year { set; get; }


        public int TCSFileID { get; set; }
        public string TCSFilePeriod { get; set; }

        [Display(Name ="File Name")]
        public string TCSFileName { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string TCSFileUNCPath { set; get; }
    }
}
