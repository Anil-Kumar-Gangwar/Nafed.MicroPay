using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class FormGroupADetail2
    {
        public int FormDetailID { get; set; }
        public int FormGroupID { get; set; }
        public Nullable<byte> SectionID { get; set; }
        public string Activities { get; set; }      
        [Required (ErrorMessage ="Please enter Grade.")]   
        public Nullable<decimal> ReportingAuthorityWeightage { get; set; }
        [Required(ErrorMessage = "Please enter Grade.")]
        public Nullable<decimal> ReviewingAuthorityWeightage { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int ActivityID { get; set; }
        public string PartNo { get; set; }
    }
}
