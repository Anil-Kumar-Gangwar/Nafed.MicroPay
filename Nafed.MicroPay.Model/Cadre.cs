using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Nafed.MicroPay.Model
{
   public class Cadre 
    {
        public int CadreID { get; set; }
        [Display(Name = "Cadre Code :")]
        [Required(ErrorMessage = "Cadre Code is required.")]
        public string CadreCode { get; set; }
        [Display(Name = "Cadre Name :")]
        [Required(ErrorMessage = "Cadre Name is required.")]
        public string CadreName { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
