using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Nafed.MicroPay.Model
{
    public class Section
    {
        public int SectionID { get; set; }
        [Display(Name = "Section Code :")]
        [Required(ErrorMessage = "Section Code is required.")]
        public string SectionCode { get; set; }
        [Display(Name = "Section Name :")]
        [Required(ErrorMessage = "Section Name is required.")]
        public string SectionName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }

}
