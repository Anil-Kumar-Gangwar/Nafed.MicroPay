using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models.Employees
{
    public class NREmployeeViewModel
    {
        public int EmployeeID { get; set; }
        public List<Model.Employees.NonRegularEmployee> Employee { set; get; }

        public Model.Employees.NonRegularEmployee _Employee { get; set; }
        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();

        public CheckBoxListViewModel CheckBoxListAcademic { get; set; } = new CheckBoxListViewModel();
        public CheckBoxListViewModel CheckBoxListProfessional { get; set; } = new CheckBoxListViewModel();


        #region Employee Search Filter 

        public int? BranchID { set; get; }
        public List<Model.SelectListModel> branchList { set; get; }


        [Display(Name = "Type")]
        public int? EmployeeTypeID { set; get; }
        public List<Model.SelectListModel> employeeType { set; get; }

        [Display(Name = "Code")]
        public string EmployeeCode { set; get; }

        [Display(Name = "Name")]
        public string EmployeeName { set; get; }

        public List<Model.SelectListModel> designation { set; get; }

        [Display(Name = "Designation")]
        public int? DesignationID { set; get; }

        public List<Model.SelectListModel> EmployeeList { set; get; }
        #endregion

        #region  Comfirmed Employee Report Filter

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date From")]
        public DateTime? fromDate { set; get; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date To")]
        public DateTime? toDate { set; get; }
        #endregion

        #region Confirmation Due Upto Report
        [Display(Name = "UpTo")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ConfirmationDueDateUpTo { get; set; }
        #endregion
        public bool AllEmployee { get; set; }
        public int? _viewmode { get; set; }
    }
}