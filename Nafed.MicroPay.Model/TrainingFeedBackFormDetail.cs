using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class TrainingFeedBackFormDetail
    {
        public int sno { get; set; }
        public int FeedBackFormDtlID { get; set; }
        public int FeedBackFormHdrID { get; set; }
        [Required(ErrorMessage = "Please enter Prefix")]
        public string SectionPrefix { get; set; }
        [Required(ErrorMessage = "Please enter Question")]
        public string Section { get; set; }
       // [Required(ErrorMessage = "Please enter Display Order")]
        public int DisplayOrder { get; set; }
        public bool DisplayInBold { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public int PartNo { get; set; }
    }

    
}
