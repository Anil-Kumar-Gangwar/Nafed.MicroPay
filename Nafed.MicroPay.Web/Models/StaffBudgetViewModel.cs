using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MicroPay.Web.Models
{
    public class StaffBudgetViewModel
    {
        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please Select Year")]
        public string selectYearID { get; set; }
        public List<SelectListItem> yearList { set; get; }

        [Display(Name = "Designation")]
        public int? designationID { set; get; }
        public List<SelectListModel> designationList { set; get; }

        [Display(Name = "For Year")]
        [Required(ErrorMessage = "Please Select Year")]
        public string genrateYearID { get; set; }

        [Display(Name = "Month")]
        //[Range(0,Int32.MaxValue,ErrorMessage ="Please Select Month")]
        public int monthID { get; set; }

        [Display(Name = "PeriodWise")]
        public bool periodWise { get; set; }

        public List<StaffBudget> listStaffBudget { set; get; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
    }
}