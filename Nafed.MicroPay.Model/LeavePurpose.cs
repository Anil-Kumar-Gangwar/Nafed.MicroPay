using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
  public class LeavePurpose
    {
        public int LeavePurposeID { get; set; }
        [DisplayName("Leave Category :")]
        [Required(ErrorMessage = "Please select leave category")]
        public int LeaveCategoryID { get; set; }
        [DisplayName("Leave Purpose :")]
        [Required(ErrorMessage = "Please Enter Leave Purpose")]
        public string LeavePurposeName { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        public string CategoryName { get; set; }
    }
}
