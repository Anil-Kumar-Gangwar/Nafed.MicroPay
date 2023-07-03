using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class DeductionSection
    {
        public int SectionID { get; set; }

        [Display(Name ="Section Name")]
        [Required(ErrorMessage ="Please enter section name")]
        public string SectionName { get; set; }
        
        public string FYear { get; set; }

        public decimal ? Amount { set; get; }

        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

    }
}
