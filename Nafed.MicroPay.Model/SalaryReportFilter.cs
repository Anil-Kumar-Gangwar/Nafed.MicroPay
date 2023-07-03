using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class SalaryReportFilter
    {
        public string BranchName { set; get; }

        [Display(Name = "Year")]
        public short salYear { set; get; }

        [Display(Name = "Month")]
        public byte salMonth { set; get; }
        //public byte ? employeeTypeID { set; get; }

        //public int ? branchID { set; get; }
        public SalaryReportType enumSalReportType { set; get; }
        
        public string fileName { set; get; }
        public string filePath { set; get; }

        [Display(Name = "Branch")]
        public int? branchID { get; set; }
        public List<SelectListModel> branchList { set; get; }

        [Display(Name = "Employee")]
        public int? employeeID { get; set; }
        public List<SelectListModel> employeeList { set; get; }

        [Display(Name = "Financial Year")]
        public string financialYear { get; set; }
        public List<SelectListModel> financialYearList { get; set; }

        [Display(Name = "Employee Type")]
        public int? employeeTypeID { get; set; }
        public List<SelectListModel> employeeTypeList { set; get; }
        public int financialFrom { get; set; }
        public int financialTo { get; set; }
        //[Display(Name = "All Branch Except Head Office")]
        //public bool AllbranchExceptHeadOffice { get; set; }
    }

    public enum SalaryReportType
    {
        [Display(Name = "Branch Wise Monthly")]
        MonthlyBranchWise=1,
        [Display(Name = "Employee Wise Monthly")]
        MonthlyEmployeeWise =2,
        [Display(Name = "Employee Wise Annual")]
        EmployeeWiseAnnual =3,
        [Display(Name = "Branch Wise Annual")]
        BranchWiseAnnual =4
    }
}
