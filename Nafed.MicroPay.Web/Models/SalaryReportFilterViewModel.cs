using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MicroPay.Web.Models
{
    public class SalaryReportFilterViewModel
    {
        [Display(Name = "Employee")]
        public int? employeeID { get; set; }
        public List<SelectListModel> employeeList { set; get; }

        [Display(Name = "Branch")]
        public int? branchID { get; set; }
        public List<SelectListModel> branchList { set; get; }

        [Display(Name = "Year")]
        public int? yearID { get; set; }
        public List<SelectListModel> yearList { set; get; }

        [Display(Name = "Month")]
        public int? monthID { get; set; }
        public List<SelectListModel> monthList { set; get; }

        [Display(Name = "Employee Type")]
        public int? employeeTypeID { get; set; }
        public List<SelectListModel> employeeTypeList { set; get; }

        [Display(Name = "All Employee")]
        public bool AllEmployee { get; set; }

        [Display(Name = "All Branch")]
        public bool AllBranch { get; set; }

        public SalaryReportRadio salaryReportRadio { get; set; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
    }

    public enum SalaryReportRadio
    {
        [Display(Name = "PaySlip")]
        PaySlip = 1,
        [Display(Name ="PaySummary (Individual Branch)")]
        PaySummary = 2,
        [Display(Name = "Covering letter (Individual)")]
        CoveringLetter = 3,
        [Display(Name = "PaySummary (All Branch)")]
        PaySummaryAllBranch = 4,
    }
}