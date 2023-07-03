using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class YearlyReportFilters
    {
        [Display(Name = "Branch")]
        public int branchId { set; get; }
        public List<SelectListModel> branchList { set; get; }
        [Display(Name = "Financial Year")]
        public string financialYear { get; set; }
        public List<SelectListModel> financialYearList { get; set; }
        [Display(Name = "Month")]
        public int salMonth { set; get; }
        [Display(Name = "Employee")]
        public int employeeId { set; get; }
        public List<SelectListModel> employeeList { set; get; }
        [Display(Name = "Employee Type")]
        public int selectedEmployeeTypeID { set; get; }
        public List<SelectListModel> employeeTypesList { set; get; }
        [Display(Name = "Bank Rate")]
        public decimal? bankRate { get; set; }
        #region Date Range
        [Display(Name = "From")]
        public int fromMonth { get; set; }
        public int fromYear { get; set; }
        [Display(Name = "To")]
        public int toMonth { get; set; }
        public int toYear { get; set; }
        #endregion
        [Display(Name = "All Employee")]
        public bool AllEmployee { get; set; }
        [Display(Name = "All Branch")]
        public bool AllBranch { get; set; }
        public YearlyReportRadio yearlyReportRadio { get; set; }
        public bool yearly { get; set; }
        public bool dateRange { get; set; }
        public FilterRadio fltrRadio { get; set; }
    }

    public enum YearlyReportRadio
    {
        [Display(Name = "Anuual PF Register")]
        AnuualPFRegister = 1,
        [Display(Name = "LIC Register")]
        LICRegister = 2,
        [Display(Name = "Income Tax Register")]
        IncomeTaxRegister = 3,
        [Display(Name = "TCS Register")]
        TCSRegister = 4,
        [Display(Name = "PF Loan Yearly")]
        PFLoanYearly = 5,
        [Display(Name = "Welfare Fund Register")]
        WelfareFundRegister = 6,
        [Display(Name = "House Building Loan Deduction Statement")]
        HouseBuildingLoanDeductionStatement = 7,
        [Display(Name = "Interest calculation on House Building Loan")]
        InterestcalculationonHouseBuildingLoan = 8,
        [Display(Name = "Interest calculation on Car Loan")]
        InterestcalculationonCarLoan = 9,
        [Display(Name = "Interest calculation on Scooter Loan")]
        InterestcalculationonScooterLoan = 10,
        [Display(Name = "Gratuity LIC New Employees")]
        GratuityLICNewEmployees = 11,
        [Display(Name = "Gratuity LIC Employees Left")]
        GratuityLICEmployeesLeft = 12,
        [Display(Name = "Leave Encashment Register(For 1 Year)")]
        LeaveEncashmentRegisterFor1Year = 13,
        [Display(Name = "Leave Encashment Register(For Total Year)")]
        LeaveEncashmentRegisterForTotalYear = 14,
        [Display(Name = "Form 7 (Singe Year)")]
        Form7SingeYear = 15,
        [Display(Name = "Form 7 (MULTIPLE Years)")]
        Form7MultipleYears = 16,
        [Display(Name = "EDLI Statement")]
        EDLIStatement = 17,
        [Display(Name = "Form 3(PS)")]
        Form3PS = 18,
        [Display(Name = "Form 8(PS)")]
        Form8PS = 19,
        [Display(Name = "HBA CERTIFICATION")]
        HBACERTIFICATION = 20,
        [Display(Name = "PF YEARLY REPORT")]
        PFYEARLYREPORT = 21,
        [Display(Name = "PF Summary")]
        PFSummary = 22,
        [Display(Name = "PF Total")]
        PFTotal = 23,
        [Display(Name = "Certificate of Interest of HBA")]
        CertificateofInterestofHBA = 24,
        [Display(Name = "Festival Loan Register")]
        FestivalLoanRegister = 25,
        [Display(Name = "House Building Register")]
        HouseBuildingRegister = 26,
        [Display(Name = "Car Loan Register")]
        CarLoanRegister = 27,
        [Display(Name = "Scooter Loan Register")]
        ScooterLoanRegister = 28,
        [Display(Name = "PF Loan Register")]
        PFLoanRegister = 29,
        [Display(Name = "PF I U Yearly Report")]
        PFIUYearlyReport = 30,
        [Display(Name = "L E R Excel Report")]
        LERExcelReport = 31,
        [Display(Name = "HBA CERTIFICATION BRANCH WISE")]
        HBACERTIFICATIONBRANCHWISE = 32,
        [Display(Name = "Form 7 IU (MULTIPLE Years)")]
        Form7IUMULTIPLEYears = 33,
        [Display(Name = "Leave Encashment")]
        LeaveEncashment = 34,
        [Display(Name = "Employee PF Balance more than 3 Years")]
        PFBalMoreThan3Years = 35,
    }

    public enum FilterRadio
    {
        [Display(Name = "Yearly")]
        rbdyearly = 1,
        [Display(Name = "Date Range")]
        rbddateRange = 2
    }
}
