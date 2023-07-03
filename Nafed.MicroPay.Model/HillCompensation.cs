using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class HillCompensation
    {
        [DisplayName("Branch :")]
        [Required(ErrorMessage = "Branch is required.")]
        public int BranchID { get; set; }
       
        public string BranchCode { get; set; }

        public string Branch { get; set; }

        [DisplayName("Upper Limit :")]
        [Required(ErrorMessage = "Upper Limit is required.")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public decimal UpperLimit { get; set; }

        [DisplayName("Amount :")]
        [Required(ErrorMessage = "Amount is required.")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Amount { get; set; }

        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }       

        public List<SelectListModel> BranchList { get; set; }
    }
}
