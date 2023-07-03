using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class AcadmicProfessionalDetailsModel
    {
        public int ID { get; set; }
        [DisplayName("Type :")]
        [Required(ErrorMessage = "Please Select Type")]
        public int TypeID { get; set; }
        [DisplayName("Value :")]
        [Required(ErrorMessage = "Please Enter Value")]
        public string Value { get; set; }

        public string Type { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
