using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class PFLoanReportFilters
    {
        [Display(Name = "Month")]
        public int salMonth { set; get; }
        [Display(Name = "Year")]
        public int salYear { set; get; }
        [Display(Name = "Employee Type")]
        public int selectedEmployeeTypeID { set; get; }
        public List<SelectListModel> employeeTypesList { set; get; }
        [Display(Name = "Branch")]
        public int branchId { set; get; }
        public List<SelectListModel> branchList { set; get; }

        [Display(Name = "All Employee")]
        public bool AllEmployee { get; set; }

        [Display(Name = "All Branch")]
        public bool AllBranch { get; set; }

        [Display(Name = "Financial Year")]
        public string financialYear { get; set; }
        public List<SelectListModel> financialYearList { get; set; }

        public PFLoanReportRadio pfLoanReportRadio { get; set; }
        public bool dateRange { get; set; }
        public DateTypeRadio dateTypeRadio { get; set; }
        #region Date Range
        [Display(Name = "From")]
        public int fromMonth { get; set; }

        [Display(Name = "To")]
        public int toMonth { get; set; }
        #endregion
        [Display(Name = "Employee")]
        public int employeeId { set; get; }
        public List<SelectListModel> employeeList { set; get; }
        public bool IsPfNo { get; set; } = true;
    }
    public enum DateTypeRadio
    {
        [Display(Name = "Yearly")]
        rbdyearly = 1,
        [Display(Name = "Date Range")]
        rbddateRange = 2
    }
    public enum PFLoanReportRadio
    {
        [Display(Name = "Monthly PF Statement")]
        MPFStatement = 1,
        [Display(Name = "Monthly PF Loan Deduction Statement")]
        MPFLDSatement = 2,
        [Display(Name = "Monthly PF Summary (Location Wise Total)")]
        MPDSummary = 3,
        [Display(Name = "List of Employees (Refundable PF Loan Issued-Monthly)")]
        LOERefundable = 4,
        [Display(Name = "List of Employees (Non-Refundable PF Loan Issued-Monthly)")]
        LOENonRefundable = 5,
        [Display(Name = "List of Employees (Refundable PF Loan Finished-Monthly)")]
        LOERefundableFinished = 6,
        [Display(Name = "Monthly PF Loan Ledger")]
        MPFLLedger = 7,
        [Display(Name = "PF Checklist")]
        PFChecklist = 8,
        [Display(Name = "PF Nafed Summary")]
        PFNSummary = 9,
        [Display(Name = "PF Loan Last Installment Adjust")]
        PFLLIAdjust = 10,
        [Display(Name = "PF Loan Total Recovered")]
        PFLTRecovered = 11,
        [Display(Name = "PF Loan Partial Payment")]
        PFLPPayment = 12,
        [Display(Name = "PF Loan Other Adjustment")]
        PFLOAdjustment = 13,
        [Display(Name = "ECR Report")]
        ECRReport = 14,
        [Display(Name = "Dependent Report")]
        DReport = 15,
        [Display(Name = "In Dependent Report")]
        IDReport = 16,
    }

    public class PFLoanSummaryReport
    {
        public int EmployeeID { get; set; }
        public string EMPLOYEECODE { get; set; }
        public int PFNO { get; set; }
        public decimal TOTAL { get; set; }
        public decimal C_PENSION { get; set; }
        public decimal cpension { get; set; }
        public decimal D_pf_A { get; set; }
        public int SalMonth { get; set; }
    }
}
