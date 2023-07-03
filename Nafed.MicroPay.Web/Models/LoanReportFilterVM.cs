using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MicroPay.Web.Models
{
    public class LoanReportFilterVM
    {
        [Display(Name = "Branch")]
        public int? branchID { get; set; }
        public List<SelectListModel> branchList { set; get; }

        [Display(Name = "Year")]
        public int yearID { get; set; }
        public List<SelectListModel> yearList { set; get; }

        [Display(Name = "Month")]
        public int monthID { get; set; }
        public List<SelectListModel> monthList { set; get; }

       
        [Display(Name = "Employee Type")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Employee Type")]
        public int employeeTypeID { get; set; }
        public List<SelectListModel> employeeTypeList { set; get; }

        [Display(Name = "All Branch")]
        public bool AllBranch { get; set; }

        public LoanReportOptions loanReportOptions { get; set; }

    }

    public enum LoanReportOptions
    {
        [Display(Name = "Loan Sanction")]
        LoanSanction = 1,

        [Display(Name = "Scooter Loan")]
        ScooterLoan = 2,

        [Display(Name = "Loan Finish")]
        LoanFinish = 3,

        [Display(Name = "TCS Recovery Loan")]
        TCSRecoveryLoan = 4,

        [Display(Name = "Festival Loan")]
        FestivalLoan = 5,

        [Display(Name = "Car Loan")]
        CarLoan = 6,

        [Display(Name = "House Building Loan")]
        HouseBuildingLoan = 7,

        [Display(Name = "Income Tax")]
        IncomeTax = 8
    }
}