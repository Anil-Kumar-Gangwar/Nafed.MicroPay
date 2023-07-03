using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
   public class TicketPriority
    {
        public int ID { get; set; }
        [Display(Name = "Code")]
        [Required(ErrorMessage = "Please enter Code.")]
        public string code { get; set; }
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Please enter Description.")]
        public string description { get; set; }
        public string color_code { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
