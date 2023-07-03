using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public  class DeductionSubSection
    {
        public int SectionID { get; set; }


        public string SectionName { get; set; }

        public int SubSectionID { get; set; }

        [Display(Name = "Sub Section Name")]
        [Required(ErrorMessage = "Please enter sub section name")]
        public string SubSectionName { get; set; }


        public decimal? Amount { set; get; }


        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}
