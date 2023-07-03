using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace Nafed.MicroPay.Model
{
    public class SalaryHeadSlab
    {
        public int SlabID { get; set; }
        [Display(Name = "Branch")]
        public Nullable<int> BranchID { get; set; }

        [Display(Name = "Field Name")]
        public string FieldName { get; set; }
        [Required]
        [Display(Name = "Basic Salary Lower Range")]        
        [Range(0, 9999999999.99, ErrorMessage = "Range cannot be less than 0 and greater than 9999999999.99")]
        //[MaxLength(13, ErrorMessage = "Maximum 13 digit allowed")]        
        public Nullable<decimal> LowerRange { get; set; }

        [Required]
        [Display(Name = "Basic Salary Upper Range")]     
        [Range(0, 9999999999.99, ErrorMessage = "Range cannot be less than 0 and greater than 9999999999.99")]  
      //  [MaxLength(13,ErrorMessage = "Maximum 13 digit allowed")]     
        public Nullable<decimal> UpperRange { get; set; }
        [Required]
       // [MaxLength(13, ErrorMessage = "Maximum 13 digit allowed")]
        [Display(Name = "Amount")]
        public Nullable<decimal> Amount { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public IEnumerable<dynamic> fieldList { set; get; } = new List<dynamic>();

        public string SelectedFieldName
        { get; set;}
    }

}
