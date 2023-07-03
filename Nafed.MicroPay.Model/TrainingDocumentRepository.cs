using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
   public class TrainingDocumentRepository
    {
        public int sno { get; set; }
        public int TrainingDocumentID { get; set; }
        public int TrainingID { get; set; }
        [Display(Name = "Document Name")]
        [Required(ErrorMessage = "Please Enter Document Name")]
        public string DocumentName { get; set; }
        public string DocumentPathName { get; set; }
        [Display(Name = "Document Detail")]
        [Required(ErrorMessage = "Please Enter Document Details")]
        public string DocumentDetail { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
    }
}
