using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class LeaveCreditRule
    {
        public int CreditRuleID { get; set; }

        [DisplayName("Leave category :")]
        [Required(ErrorMessage = "Please select leave category")]
        public int LeaveCategoryID { get; set; }

        [DisplayName("Calendar Year :")]
        [Required(ErrorMessage = "Please select calendar year")]
        public int C_YearID { get; set; }
        [DisplayName("From Month :")]
        [Required(ErrorMessage = "Please select from month")]
        public int FromMonth { get; set; }
        [DisplayName("To Month :")]
        [Required(ErrorMessage = "Please select to month")]
        public int ToMonth { get; set; }
        [DisplayName("Credit :")]
        [Required(ErrorMessage = "Please enter credit")]
        public int Credit { get; set; }
        public string FinancialYear { get; set; }

        public string LeaveCategory { get; set; }
        public string FMonth { get; set; }
        public string TMonth { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        [DisplayName("Employee Type :")]
        public Nullable<int> EmployeeTypeID { get; set; }
    }
}
