using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class LoanSlab
    {
        [DisplayName("Effective Date :")]
        [Required(ErrorMessage = "Effective Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime EffectiveDate { get; set; }
        [DisplayName("Slab No. :")]
        [Required(ErrorMessage = "Slab No. is required.")]
        public Nullable<int> SlabNo { get; set; }
        [DisplayName("Amount :")]
        [Required(ErrorMessage = "Amount is required.")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> AmountOfSlab { get; set; }
        [DisplayName("Rate Of Interest :")]
        [Required(ErrorMessage = "Rate Of Interest is required.")]
        public Nullable<decimal> RateOfInterest { get; set; }
        
        public string LoanType { get; set; }
        public string LoanDesc { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        [DisplayName("Loan Type :")]
        [Required(ErrorMessage = "Loan Type is required.")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public int LoanTypeId { get; set; }
        public List<SelectListModel> LoanTypeList { get; set; }
    }
}
