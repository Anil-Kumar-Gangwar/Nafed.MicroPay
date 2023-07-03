using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class BankRates
    {
        [DisplayName("Effective Date :")]
        [Required(ErrorMessage = "Effective Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
      
        public System.DateTime EffectiveDate { get; set; }
        [DisplayName("Bank Rate :")]
        [Required(ErrorMessage = "Bank Rate is required.")]
        [DisplayFormat(DataFormatString = "{0:0.##}",ApplyFormatInEditMode =true)]
        public decimal BankLoanRate { get; set; }
        [DisplayName("Bank Accural Rate :")]
        [Required(ErrorMessage = "Bank Accural Rate is required.")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public decimal BankAccuralRate { get; set; }
        [DisplayName("PF Loan Rate :")]
        [Required(ErrorMessage = "PF Loan Rate is required.")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public decimal PFLoanRate { get; set; }
        [DisplayName("PF Accural Rate :")]
        [Required(ErrorMessage = "PF Accural Rate is required.")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public decimal PFLoanAccuralRate { get; set; }
        [DisplayName("TDS Rebet Rate :")]
        [Required(ErrorMessage = "TDS Rebet Rate is required.")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public decimal TDSRebetRate { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
