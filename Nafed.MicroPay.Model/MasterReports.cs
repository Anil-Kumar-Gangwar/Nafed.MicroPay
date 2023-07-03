using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
   public class MasterReports
    {
        public string ReportType { get; set; }
        [Display(Name = "Year")]
        public int? Year { get; set; }
        [Display(Name = "Month")]
        public int? Month { get; set; }
        [Display(Name = "Branch Code")]
        public int? BranchID { get; set; }
        [Display(Name = "Employee Type")]
        public int? EmployeeTypeID { get; set; }
        [Display(Name = "Bank")]
        public string BankCode { get; set; }
        [Display(Name = "All Employees")]
        public bool AllEmployee { get; set; }
        [Display(Name = "All Branches")]
        public bool AllBranch { get; set; }

        public ReportTypes RadioReportType { get; set; }
    }

    public enum ReportTypes
    {
        [Display(Name = "Employee With Bank Account (Floppy)")]
        EmployeewiseBankAcnt = 1,
        [Display(Name = "Bank Statement (Complete)")]
        BankAcnt = 2,
        [Display(Name = "CCA ")]
        CCA = 3,
        [Display(Name = "Special Pay")]
        SpeciaPay = 4,
        [Display(Name = "Income Tax Deduction")]
        IncometaxDedc = 5,
        [Display(Name = "Classification of Cities Grade")]
        Classificationofcitiesgrade = 6,
        [Display(Name = "Location Wise Total")]
        Locationwisettl = 7,
        [Display(Name = "Location Wise List of Total Employees (PF No.)")]
        Locationwiselstttlemployess = 8,
        [Display(Name = "Location Wise Net Pay")]
        locationwisenetPay = 9,
        [Display(Name = "Employee Listing DOB")]
        EmployeelstDOB = 10,
        [Display(Name = "Employee Listing Admin-Gift")]
        EmployeelstAdmngft = 11,
    }
}
