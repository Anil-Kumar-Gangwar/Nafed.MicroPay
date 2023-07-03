using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Models
{
    public class AdjustOldLoanViewModel
    {
        public List<SanctionLoan> adjustLoanList { get; set; }

        #region Search Filter 
        [Display(Name = "Loan Type")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Loan Type.")]
        public int LoanTypeId { set; get; }
        public List<SelectListModel> LoanTypeList { get; set; }
        [Display(Name = "Employee")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Employee.")]
        public int EmployeeId { set; get; }
        public List<SelectListModel> EmployeeList { get; set; }
        [Display(Name = "Show Old Loan Employee")]
        public bool OldLoanEmployee { get; set; }
        [Display(Name = "Search By PF No.")]
        public string PFNumber { get; set; }
        #endregion
    }
}