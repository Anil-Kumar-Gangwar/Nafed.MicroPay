using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
     public class EmployeeType
     {
        public int EmployeeTypeID { get; set; }

        [Display(Name = "EmployeeType Code :")]
        [Required(ErrorMessage = "EmployeeType Code is required.")]
        public string EmployeeTypeCode { get; set; }

        [Display(Name = "EmployeeType Name :")]
        [Required(ErrorMessage = "EmployeeType Name is required.")]
        public string EmployeeTypeName { get; set; }

        public bool IsDeleted { get; set; }
    }
}
