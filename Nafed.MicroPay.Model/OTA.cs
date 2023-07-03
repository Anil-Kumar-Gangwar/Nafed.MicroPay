using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class OTA
    {
        public int SNo { get; set; }

        [DisplayName("From Pay Scale :")]
        [Required(ErrorMessage = "From Pay Scale is required.")]
        public string FromPay { get; set; }
        [DisplayName("To Pay Scale :")]
        [Required(ErrorMessage = "To Pay Scale required.")]
        public string ToPay { get; set; }

        [DisplayName("OTA Code :")]
        [Required(ErrorMessage = "OTA Code is required.")]
        public Nullable<short> OTACode { get; set; }

        [DisplayName("Maximum Amount :")]
        [Required(ErrorMessage = "Maximum Amount is required.")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> MaxAmt { get; set; }
        [DisplayName("Maximum Rate per Hour :")]
        [Required(ErrorMessage = "Maximum Rate per Hour is required.")]
        public Nullable<decimal> MaxRateperHour { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public List<SelectListModel> FromPayList { get; set; }
        public List<SelectListModel> ToPayList { get; set; }
    }
}
