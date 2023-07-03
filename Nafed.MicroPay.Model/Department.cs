using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class Department
    {
        public int DepartmentID { get; set; }
        [Display(Name = "Department Code :")]
      //  [Required(ErrorMessage = "Department Code is required.")]
        public string DepartmentCode { get; set; }

        [Display(Name = "Department Name :")]
        [Required(ErrorMessage = "Department Name is required.")]
        public string DepartmentName { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        [Display(Name = "Is Active :")]
        public bool IsActive { get; set; }
    }
}
