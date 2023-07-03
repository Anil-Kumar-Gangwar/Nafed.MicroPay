using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class DependentViewModel
    {
        [Display(Name = "Branch")]
        [Range(1,Int32.MaxValue,ErrorMessage = "Please Select Branch")]
        public int BranchID { set; get; }
        public List<SelectListModel> branchList { set; get; }

        [Display(Name = "Employee Name")]
        public int? EmployeeID { set; get; }

        [Display(Name = "Name")]
        public List<SelectListModel> employeeList { set; get; }

        public List<EmployeeDependent> listDependent { get; set; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();

        [Display(Name = "Employee Name")]
        public string EmployeeName { set; get; }
        [Display(Name = "Code")]
        public string EmployeeCode { set; get; }
        [Display(Name = "Department")]
        public string DepartmentName { set; get; }

        public int userTypeID { set; get; }
    }
}