using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class HolidayViewModel
    {
        public List<Holiday> holidays { set; get; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();


        [Display(Name ="Calendar Year")]
        public int ? CYear { set; get; }

        [Display(Name = "Branch")]
        public int? BranchID { set; get; }


        public List<SelectListModel> CYearList { set; get; }
        public List<SelectListModel> BranchList { set; get; }
    }
}