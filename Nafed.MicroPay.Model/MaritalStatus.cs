using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class MaritalStatus
    {
        public int MaritalStatusID { get; set; }
        [Display(Name = "Marital Status Code :")]
        [Required(ErrorMessage = "Marital Status Code is required.")]
        public string MaritalStatusCode { get; set; }
        [Display(Name = "Marital Status Name :")]
        [Required(ErrorMessage = "Marital Status Name is required.")]
        public string MaritalStatusName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
