using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MicroPay.Web.Models
{
    public class ExgratiaViewModel
    {
        public List<ExgratiaList> listExgratia { set; get; }
        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please Select Year")]
        public string selectYearID { get; set; }
        public List<SelectListItem> yearList { set; get; }

        [Display(Name = "Branch")]
        public int? branchID { set; get; }

        public List<SelectListModel> branchList { set; get; }

        [Display(Name = "Employee")]
        [Required(ErrorMessage = "Please Select Employee")]
        public int EmployeeID { get; set; }
        [Display(Name = "No. of days")]
        public int noofdays { get; set; }

        [Display(Name = "Financial Year")]
        [Required(ErrorMessage = "Please Select Year")]
        public string FinancialYear { get; set; }

        [Display(Name = "Employee Type")]
        public int EmpTypeID { set; get; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
    }
}