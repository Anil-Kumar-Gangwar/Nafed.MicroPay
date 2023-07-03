using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class GisDeduction
    {

        [Required]
        [DisplayName("Category :")]
        public string Category { get; set; }

        [Required]
        [DisplayName("Code :")]
        public byte Code { get; set; }

        [Required]
        [DisplayName("Rate :")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Rate { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public List<SelectListModel> CategoryList { get; set; }
        public List<SelectListModel> CodeList { get; set; }
    }
}
