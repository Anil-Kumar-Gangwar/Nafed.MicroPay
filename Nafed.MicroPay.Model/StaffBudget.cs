using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class StaffBudget
    {
        public int StaffBudgetId { get; set; }
        [Display(Name = "Designation")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Designation")]
        public Nullable<int> DesignationID { get; set; }
        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please Select Year")]
        //[StringLength(7,ErrorMessage ="Please Select Year")]
        public string Year { get; set; }
        [Display(Name = "Month")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Month")]
        public Nullable<int> Month { get; set; }
        [Display(Name = "Budgeted Staff Strenght")]
        [Required(ErrorMessage = "Please Enter Budget Staff")]
        public Nullable<short> BudgetedStaff { get; set; }
        [Display(Name = "Existing Staff Strenght")]
        public Nullable<short> PresentStaff { get; set; }
        [Display(Name = "Vacancies")]
        public Nullable<short> Vac { get; set; }
        public Nullable<short> FLTC { get; set; }
        public Nullable<short> FPromotion { get; set; }
        public Nullable<short> FDirect { get; set; }
        [Display(Name = "VRS")]
        public Nullable<short> VRS { get; set; }
        public Nullable<short> Upgrade { get; set; }
        public Nullable<short> Creation { get; set; }
        public Nullable<short> ForceFully { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        [Display(Name = "Total Staff Strenght")]
        public Nullable<short> TotalStaffStrenght { get; set; }

        public Nullable<int> PromotionPer { get; set; }

        public Nullable<int> LTCPerc { get; set; }

        public Nullable<int> DirectPerc { get; set; }
        public string DesignationName { get; set; }

        public string Event { get; set; }

        public bool IsDeleted { get; set; }

        public string AlertMessage { get; set; }
    }
}
