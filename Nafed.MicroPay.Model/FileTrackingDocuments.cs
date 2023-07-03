using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
   public class FileTrackingDocuments
    {
        public int DocID { get; set; }
        public int FileID { get; set; }
        public int Sno { get; set; }
        [Display(Name = "Document Name")]
        [Required(ErrorMessage = "Please Enter Document Name.")]
        public string DocName { get; set; }
        public string DocOrignalName { get; set; }
        public string DocPathName { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }       
    }
}
