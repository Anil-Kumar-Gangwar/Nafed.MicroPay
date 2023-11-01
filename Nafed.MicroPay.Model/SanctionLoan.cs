using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class SanctionLoan
    {
        [Display(Name = "Serial No")]
        public Nullable<int> SerialNo { get; set; }
        public string Branchcode { get; set; }
        [Display(Name = "Loan No.")]
        [Required(ErrorMessage = "Loan number is required")]
        public string PriorityNo { get; set; }
        public string LoanType { get; set; }
        public string EmpCode { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Application")]
        [Required(ErrorMessage = "Date of Application is required")]
        public Nullable<System.DateTime> DateofApp { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "App Receipt Date")]
        [Required(ErrorMessage = "App Receipt Date is required")]
        public Nullable<System.DateTime> DateRcptApp { get; set; }
        [Display(Name = "Requested Amount")]
        [Required(ErrorMessage = "Requested Amount is required")]
        public Nullable<decimal> ReqAmt { get; set; }
        public string Surety { get; set; }
        [Display(Name = "Loan Sanctioned")]
        //public bool LoanSanc { get; set; }
        public bool LoanSanction { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Sanction")]
        public Nullable<System.DateTime> DateofSanc { get; set; }
        [Display(Name = "Sanction Amount")]
        public Nullable<decimal> SancAmt { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Availing Loan")]
        public Nullable<System.DateTime> DateAvailLoan { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Effective Date")]
        public Nullable<System.DateTime> EffDate { get; set; }
        [Display(Name = "Reason")]
        public string Reasonref { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Refusal")]
        public Nullable<System.DateTime> Dateofref { get; set; }
        public Nullable<decimal> MaxLoanAmt { get; set; }
        public Nullable<decimal> ROI { get; set; }
        [Display(Name = "A")]
        public bool Asubmitted { get; set; }
        [Display(Name = "B")]
        public bool Bsubmitted { get; set; }
        [Display(Name = "C")]
        public bool Csubmitted { get; set; }
        [Display(Name = "D")]
        public bool Dsubmitted { get; set; }
        [Display(Name = "E")]
        public bool Esubmitted { get; set; }
        [Display(Name = "F")]
        public bool Fsubmitted { get; set; }
        [Display(Name = "G")]
        public bool Gsubmitted { get; set; }
        [Display(Name = "H")]
        public bool Hsubmitted { get; set; }
        [Display(Name = "Others Details")]
        public string Detail { get; set; }
        public string LoanMode { get; set; }
        public Nullable<bool> IsFloatingRate { get; set; }
        public Nullable<bool> IsInterestPayable { get; set; }
        [Display(Name = "Total No. of Principal Amt. Installment")]
        public Nullable<int> OriginalPInstNo { get; set; }
        [Display(Name = "Total No. of Interest Amt. Installment")]
        public Nullable<int> OriginalIInstNo { get; set; }
        [Display(Name = "Original Principal Inst. Amount")]
        public Nullable<decimal> OriginalPinstAmt { get; set; }
        [Display(Name = "Interest Inst. Amount")]
        public Nullable<decimal> InterestInstAmt { get; set; }
        [Display(Name = "Balance Principal Amount")]
        public Nullable<decimal> BalancePAmt { get; set; }
        [Display(Name = "Balance Interest Amount")]
        public Nullable<decimal> BalanceIAmt { get; set; }
        [Display(Name = "Total Balance Amount")]
        public Nullable<decimal> TotalBalanceAmt { get; set; }
        [Display(Name = "Remaining Principal Inst. No")]
        public Nullable<int> RemainingPInstNo { get; set; }
        [Display(Name = "Remaining Interest Inst. No")]
        public Nullable<int> RemainingIInstNo { get; set; }
        [Display(Name = "Last Paid Inst. Date")]
        public string LastPaidInstDeduDt { get; set; }
        [Display(Name = "Last Paid Principal Inst.")]
        public Nullable<decimal> LastPaidPInstAmt { get; set; }
        [Display(Name = "Last Paid Interest Inst.")]
        public Nullable<decimal> LastPaidIInstAmt { get; set; }
        [Display(Name = "Last Paid Principal Inst. No")]
        public Nullable<int> LastPaidPInstNo { get; set; }
        [Display(Name = "Last Paid Interest Inst. No")]
        public Nullable<int> LastPaidIInstNo { get; set; }
        [Display(Name = "Last Month Interest")]
        public Nullable<decimal> LastMonthInterest { get; set; }
        [Display(Name = "No. Of Installment Skipped")]
        public Nullable<int> TotalSkippedInst { get; set; }
        [Display(Name = "Current Rate Of Interest")]
        public Nullable<decimal> CurrentROI { get; set; }
        public Nullable<bool> IsNewLoanAfterDevelop { get; set; }
        public Nullable<bool> Status { get; set; }
        [Display(Name = "Last Installment Amount")]
        public Nullable<decimal> LastInstAmt { get; set; }
        public Nullable<decimal> BkpLastInstAmt { get; set; }
        public Nullable<decimal> AdjustedSancAmt { get; set; }

        public Nullable<decimal> withdrawlAmt { get; set; }
        public Nullable<decimal> WithdrawlInterestAmt { get; set; }
        public Nullable<System.DateTime> Wdate { get; set; }
        public Nullable<byte> Rmonth { get; set; }
        public Nullable<short> Ryear { get; set; }
        public Nullable<decimal> PARTREFUND { get; set; }
        public Nullable<decimal> PARTREFUNDINT { get; set; }

        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        [Display(Name = "Employee")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Employee")]
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<int> BranchId { get; set; }
        [Display(Name = "Loan Type")]
        //[Range(1, Int32.MaxValue, ErrorMessage = "Please Select Loan Type")]
        public int LoanTypeId { get; set; }
        public bool IsDeleted { get; set; }

        public List<SanctionLoan> SanctionLoanList { get; set; }

        public string EmployeeName { get; set; }

        public string LoanTypeDesc { get; set; }

        [Display(Name = "Search By PF No.")]
        public string PFNumber { get; set; }
        public List<SelectListModel> EmployeeList { get; set; }
        public List<SelectListModel> LoanTypeList { get; set; }
        [Display(Name = "Round to 1")]
        public bool ChkRound { get; set; }
        [Display(Name = "Application Status")]
        public string AppStatus { get; set; }
        [Display(Name = "Rate Of Intrest")]
        public decimal? RateOfIntrest { get; set; }
        public int OriginalPrincipleInstallments { get; set; }
        public int OriginalInterestInstallments { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Loan Type")]
        public int AssignLoanTypeId { get; set; }
        public bool IsModelValid { get; set; }

        public int? ActualInstNoRem { get; set; }
        public int ChangedInstNoRem { get; set; }
        public decimal? ChangedInstAmt { get; set; }
        public decimal? ChangeIntAmt { get; set; }
        public bool OldLoanEmployee { get; set; }
        public SanctionLoanRadio sanctionLoanRadio { get; set; }
        public Nullable<DateTime> RefundDate { get; set; }
        [Display(Name = "Inst No.")]
        public decimal? InstNo { get; set; }
        public decimal? oldP1 { get; set; }
        public decimal? oldI2 { get; set; }
        public Nullable<int> RemainingPInstNo1 { get; set; }
        public string DateAvailLoan1 { get; set; }
        public int totlRefundMonthId { get; set; }
        public int totlRefundYearId { get; set; }
        public int MstLoanID { get; set; }
    }

    public class LoanTypeDetail
    {
        public decimal MaxLnAmount { get; set; }
        public decimal LoanRate { get; set; }
        public bool checkRound { get; set; }
        public bool isFloatingRate { get; set; }
        public bool isinterestcalc { get; set; }
        public bool isslabdependent { get; set; }
        public string loanMode { get; set; }
        public decimal rateofInterest { get; set; }

    }

    public class PriorityNoDetails
    {
        public string PriorityNo { get; set; }
        public string Message { get; set; }
        public int MaxInstallmentP { get; set; }
        public int MaxInstallmentI { get; set; }
    }

    public enum SanctionLoanRadio
    {
        TotalRefund = 1,
        PartialRefund = 2,
        LastInstallAdjust = 3,
        DebitCreditEntry = 4,
        InstallmentAdjestment = 5,
        LoanFinish = 6,
    }
}
