using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class RegularEmployeeSalary
    {
        public int loggedInUserID { set; get; }
        public bool RegeneratedCase { set; get; }

        [Range(1, Int16.MaxValue, ErrorMessage = "Please select month.")]
        [Display(Name = "Month")]

        public int salMonth { set; get; }

        public byte PrevMonth
        {
            get
            {
                if (salMonth == 1)
                    return 12;
                else
                    return (byte)(salMonth - (byte)1);
            }

        }

        public int PrevYear
        {
            get
            {
                if (salMonth == 1)
                    return (short)(salYear - ((short)1));
                else
                    return salYear;
            }
        }

        [Display(Name = "Year")]
        public int salYear { set; get; }

        public bool IsSalaryCalculationDone { set; get; }

        public bool CustomErrorFound { get; set; }

        public string CustomErrorMsg { get; set; }

        public string TCSFileName { set; get; }

        public string TCSFilePeriod { get; set; }

        [Display(Name = "All Employees")]
        public bool AllEmployees { set; get; }

        [Display(Name = "Single Employee")]
        public bool SingleEmployee { set; get; }

        [Display(Name = "All Branches(Except Head-Office)")]
        public bool BranchesExcecptHO { set; get; }

        [Display(Name = "All Branches")]
        public bool AllBranches { set; get; }


        [Display(Name = "Single Branch")]
        public bool SingleBranch { set; get; }

        [Display(Name = "Deduct Relief Fund")]

        public bool DeductReliefFund { set; get; }

        [Display(Name = "Medical Reimbursement")]
        public bool MedicalReimbursement { set; get; }

        [Display(Name = "Non Regular")]
        public bool NonRegular { set; get; }

        [Display(Name = "Regular Employee")]
        public bool RegularEmployee { set; get; }

        [Display(Name = "No Reflection in Loan")]
        public bool NoReflectionInLoan { set; get; }

        [Display(Name = "P.F.No.")]
        public string PFNo { set; get; }

        [Display(Name = "Is Salary Generated")]
        public bool IsSalaryGenerated { set; get; }

        public int? selectedEmployeeID { set; get; }
        public List<SelectListModel> Employees { set; get; }

        public int? selectedBranchID { set; get; }
        public List<SelectListModel> Branchs { set; get; }

        [Display(Name = "Employee Type")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Employee Type")]

        public int? selectedEmployeeTypeID { set; get; }
        public List<SelectListModel> EmployeeTypes { set; get; }

        [Display(Name = "Select Branch")]
        public EnumBranch enumBranch { set; get; }

        [Display(Name = "Select Category")]
        public EnumEmpCategory enumEmpCategory { set; get; }

        #region Rates ===

        public decimal? otaMaxRatePerHour { set; get; }
        public decimal? otaMaxAmt { set; get; }

        public decimal? washingAllowanceRate { set; get; }

        public decimal? pFLoanAccuralRate { set; get; }

        #endregion
        public bool ApprovalRequestSent { set; get; }

        public bool Reverted { set; get; }

        public IList<string> NegativeSalEmp { get; set; }
    }

    public enum EnumEmpCategory
    {
        AllEmployees = 1,
        SingleEmployee = 2
    }
    public enum EnumBranch
    {
        BranchesExcecptHO = 1,
        SingleBranch = 2,
        AllBranches=3
    }
}
