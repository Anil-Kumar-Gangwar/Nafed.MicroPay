using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class tblTDSTaxRulesSlab
    {
        public string ResponseText { set; get; }

        public List<SelectListModel> fYList { set; get; }

        [Required(ErrorMessage ="Please Select Financial Year")]
     
        [Display(Name ="Financial Year")]
        public string FinancialYear { get; set; }


        [Required]
        [Display(Name = "Standard Deduction")]
        public Nullable<decimal> StandardDeduction { get; set; }

        [Required]
        [Display(Name = "No Tax Till Salary")]
        public Nullable<decimal> NoTaxTillSal { get; set; }

        [Required]
        [Display(Name = "Upper Limit")]
        public Nullable<decimal> FirstSlabUpperLimit { get; set; }

        [Required]
        [Display(Name = "Tax Percentage")]
        public Nullable<decimal> FirstSlabTaxPercentage { get; set; }

        [Required]
        [Display(Name = "Tax Surcharge")]
        public Nullable<decimal> FirstSlabTaxSurchage { get; set; }

        [Required]
        [Display(Name = "Upper Limit")]
        public Nullable<decimal> SecondSlabUpperLimit { get; set; }

        [Required]
        [Display(Name = "Tax Percentage")]
        public Nullable<decimal> SecondSlabTaxPercentage { get; set; }

        [Required]
        [Display(Name = "Tax Surcharge")]

        public Nullable<decimal> SecondSlabTaxSurchage { get; set; }

        [Required]
        [Display(Name = "Upper Limit")]
        public Nullable<decimal> ThirdSlabUpperLimit { get; set; }

        [Required]
        [Display(Name = "Tax Percentage")]
        public Nullable<decimal> ThirdSlabTaxPercentage { get; set; }

        [Required]
        [Display(Name = "Tax Surcharge")]
        public Nullable<decimal> ThirdSlabTaxSurchage { get; set; }

        [Required]
        [Display(Name = "No Tax Till Salary")]
        public Nullable<decimal> FemaleNoTaxTillSal { get; set; }

        [Required]
        [Display(Name = "Upper Limit")]
        public Nullable<decimal> FemaleFirstSlabUpperLimit { get; set; }

        [Required]
        [Display(Name = "Tax Percentage")]
        public Nullable<decimal> FemaleFirstSlabTaxPercentage { get; set; }

        [Required]
        [Display(Name = "Tax Surcharge")]
        public Nullable<decimal> FemaleFirstSlabTaxSurchage { get; set; }

        [Required]
        [Display(Name = "Upper Limit")]
        public Nullable<decimal> FemaleSecondSlabUpperLimit { get; set; }


        [Display(Name = "Tax Percentage")]
        [Required]

        public Nullable<decimal> FemaleSecondSlabTaxPercentage { get; set; }

        [Display(Name = "Tax Surcharge")]
        [Required]
        public Nullable<decimal> FemaleSecondSlabTaxSurchage { get; set; }

        [Display(Name = "Upper Limit")]
        [Required]
        public Nullable<decimal> FemaleThirdSlabUpperLimit { get; set; }

        [Display(Name = "Tax Percentage")]
        [Required]
        public Nullable<decimal> FemaleThirdSlabTaxPercentage { get; set; }

        [Display(Name = "Tax Surcharge")]
        [Required]
        public Nullable<decimal> FemaleThirdSlabTaxSurchage { get; set; }

        [Display(Name = "No Tax Till Salary")]
        public Nullable<decimal> SnrctnNoTaxTillSal { get; set; }

        [Display(Name = "Upper Limit")]
        public Nullable<decimal> SnrctnFirstSlabUpperLimit { get; set; }
        [Display(Name = "Tax Percentage")]
        public Nullable<decimal> SnrctnFirstSlabTaxPercentage { get; set; }
        [Display(Name = "Tax Surcharge")]
        public Nullable<decimal> SnrctnFirstSlabTaxSurcharge { get; set; }

        [Display(Name = "Upper Limit")]
        public Nullable<decimal> SnrctnSecondSlabUpperLimit { get; set; }
        [Display(Name = "Tax Percentage")]
        public Nullable<decimal> SnrctnSecondSlabTaxPercentage { get; set; }
        [Display(Name = "Tax Surcharge")]
        public Nullable<decimal> SnrctnSecondSlabTaxSurcharge { get; set; }

        [Display(Name = "Senior Citizen Age Limit")]
        public Nullable<decimal> SnrctnAgeLimit { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
