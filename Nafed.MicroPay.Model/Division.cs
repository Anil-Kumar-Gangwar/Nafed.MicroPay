using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class Division
    {
        public int DivisionID { get; set; }
        [Display(Name = "Division Code :")]
        [Required(ErrorMessage = "Division Code is required.")]
        public string DivisionCode { get; set; }
        [Display(Name = "Division Name :")]
        [Required(ErrorMessage = "Division Name is required.")]
        public string DivisionName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
