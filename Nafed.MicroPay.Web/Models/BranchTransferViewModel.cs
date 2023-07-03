using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class BranchTransferViewModel
    {
        [Display(Name = "Branch")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Branch")]
        public int BranchID { set; get; }
        public List<SelectListModel> branchList { set; get; }
        [Display(Name = "Employee Name")]
        public int? EmployeeID { set; get; }

        [Display(Name = "Name")]
        public List<SelectListModel> employeeList { set; get; }
        public BranchTransfer branchTransfer { set; get; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
    }
}