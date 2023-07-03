using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class Relation
    {
        public int RelationID { get; set; }
        [Display(Name = "Relation Code :")]
        [Required(ErrorMessage = "Relation Code is required.")]
        public string RelationCode { get; set; }
        [Display(Name = "Relation Name :")]
        [Required(ErrorMessage = "Relation Name is required.")]
        public string RelationName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; } 
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
