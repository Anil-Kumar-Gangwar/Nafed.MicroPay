using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Nafed.MicroPay.Model
{
   public class BonusWages
    {
        public int ID { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please Select Year")]
        public string selectYearID { get; set; }

        [Display(Name = "From Period")]
        [Required(ErrorMessage = "Please enter From Period")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> From_date { get; set; }

        [Display(Name = "To Period")]
        [Required(ErrorMessage = "Please enter To period")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> To_date { get; set; }

        [Display(Name = "Restricted Salary")]
        [Required(ErrorMessage = "Restricted Salary is required.")]
        public decimal Restricted_Salary_as_Per_bonus { get; set; }
        [Display(Name = "Minimum monthly wages")]
        [Required(ErrorMessage = "Monthly Wages is required.")]
       
        public decimal Minimum_monthly_wages { get; set; }

        public decimal ActBonus_Amt { get; set; }
        public int Month { get; set; }

        public int Days { get; set; }
        public bool IsDeleted { get; set; }

        [Display(Name = "Employee Type")]
        public int EmpTypeID { set; get; }

        [Display(Name = "Branch")]
        public int? branchID { set; get; }
        public List<SelectListModel> branchList { set; get; }
        [Display(Name = "Designation")]
        public int designationID { set; get; }
        public string designationName { set; get; }


        [Display(Name = "Employee")]
        [Required(ErrorMessage = "Please Select Employee")]
        public int EmployeeID { get; set; }
        public string EmpCode { get; set; }
        public string branchName { get; set; }
        
        public string Name { get; set; }
        public decimal Amt_Tot { get; set; }

        public decimal net_amount { get; set; }
        [Display(Name = "Financial Year")]
        [Required(ErrorMessage = "Please Select Year")]
        public string FinancialYear { get; set; }

        [Display(Name = "Bonus Rate")]
       
        public decimal BonusRate { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
