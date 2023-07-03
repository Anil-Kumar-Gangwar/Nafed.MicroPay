using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class UpdateBasic
    {
        public int EmployeeSalaryID { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "Employee Code :")]
        [Required(ErrorMessage = "Employee Code is required.")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Employee Name :")]
        [Required(ErrorMessage = "Employee Name is required.")]
        public string EmployeeName { get; set; }
        [Display(Name = "Current Basic :")]
        public int ExistingBasic { get; set; }
        [Display(Name = "New Basic :")]
        [Required(ErrorMessage = "New Basic is required.")]
        public decimal NewBasic { get; set; }

        [Display(Name = "Current Designation :")]
        public string ExistingDesg { get; set; }
        [Display(Name = "New Designation :")]
        public int NewDesg { get; set; }

        [Display(Name = "Current Branch :")]
        public string ExistingBranch { get; set; }
        [Display(Name = "New Branch :")]
        public int NewBranch { get; set; }


        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
