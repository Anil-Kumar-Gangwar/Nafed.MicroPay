using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class TrainingPrerequisite
    {


        public int TrainingID { set; get; }
        public int sno { set; get; }
        public int PrerequisiteID { get; set; }

        [Required(ErrorMessage = "Item is required.")]
        public string Item { get; set; }

        [Required(ErrorMessage = "Serial No is required.")]
        public string SerialNo { get; set; }

        [Required(ErrorMessage = "Make is required.")]
        public string Make { get; set; }

        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }


}
