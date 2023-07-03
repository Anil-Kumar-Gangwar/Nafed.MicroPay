using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public  class tblMstLoanType
    {
        public LoanPaymentType enumLoanPymtType { set; get; }

        [Display(Name = "Loan Type")]
        [Required(ErrorMessage = "Please enter Loan Type")]

        public string LoanType { get; set; }

        [Display(Name = "Loan Description")]
        public string LoanDesc { get; set; }

        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }

        [Display(Name = "Max Installment Period")]
        public Nullable<short> MaxInstallmentP { get; set; }

        [Display(Name = "Max Installment Interest")]
        public Nullable<short> MaxInstallmentI { get; set; }

        [Display(Name = "Max Laon Amount")]
        public Nullable<decimal> MaxLnAmount { get; set; }

        [Display(Name = "Is Floating Rate")]
        public bool IsOnFloatingRate { get; set; }

        [Display(Name = "Is Calculate Rate")]
        public bool IsCalcInterest { get; set; }

        [Display(Name = "Abbriviation")]
        public string Abbriviation { get; set; }

        [Display(Name = "Is Slab Dependent")]
        public bool IsSlabDependent { get; set; }

        [Display(Name = "Is TDS Rebet")]
        public bool IsTDSRebet { get; set; }        

        [Display(Name = "TDS Rebet ROI")]
        public Nullable<decimal> TDSRebetROI { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public int LoanTypeId { get; set; }
        public bool IsDeleted { get; set; }
    }

    public enum LoanPaymentType
    {
        [Display(Name ="EMI")]
        EMI =1,
        [Display(Name = "Installment")]
        Installment =2
    }
}
