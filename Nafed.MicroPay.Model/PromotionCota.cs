using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
   public class PromotionCota
    {
        public int PromotionCotaId { get; set; }
        [Display(Name = "Designation")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Designation")]
        public int DesignationID { get; set; }
        [Display(Name = "Promotion")]
        public Nullable<int> Promotion { get; set; }
        [Display(Name = "Direct")]
        public Nullable<int> Direct { get; set; }
        [Display(Name = "LCT")]
        public Nullable<int> LCT { get; set; }
        public string NotCheck { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> NPromotion { get; set; }
        public Nullable<int> NDirect { get; set; }
        public Nullable<int> NLCT { get; set; }

    }
}
