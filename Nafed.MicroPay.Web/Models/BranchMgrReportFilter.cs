using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class BranchMgrReportFilter
    {
        [Display(Name ="Select")]
        public int ? selectedID { get; set; }

        [Display(Name = "Employee")]
        public int selectedEmployee { get; set; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
        public List<SelectListModel> filterList { set; get; }
    }
}