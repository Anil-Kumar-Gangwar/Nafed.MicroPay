using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
  public class PublishSalaryFilters
    {
        public int salMonth { set; get; }
        public int salYear { set; get; }
        [Display(Name = "All Employees")]
        public bool AllEmployees { set; get; }
        [Display(Name = "Single Employee")]
        public bool SingleEmployee { set; get; }

        [Display(Name = "All Branches(Except Head-Office)")]
        public bool BranchesExcecptHO { set; get; }

        [Display(Name = "Single Branch")]
        public bool SingleBranch { set; get; }

        public int? selectedEmployeeID { set; get; }
        public List<SelectListModel> Employees { set; get; }

        public int? selectedBranchID { set; get; }
        public List<SelectListModel> Branchs { set; get; }

        [Display(Name = "Employee Type")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Employee Type")]
        public int? selectedEmployeeTypeID { set; get; }
        public List<SelectListModel> EmployeeTypes { set; get; }

        [Display(Name = "Select Branch")]
        public EnumBranch enumBranch { set; get; }

        [Display(Name = "Select Category")]
        public EnumEmpCategory enumEmpCategory { set; get; }
        public bool IsUpdateDone { get; set; }

        #region DA Arrer Filters
        [Display(Name = "Employee Type")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Employee Type")]
        public int? selectedDAEmpTypeID { set; get; }
        public int? selectedDAEmpID { set; get; }
        public int? selectedDABranchID { set; get; }
        public EnumDABranch enumDABranch { set; get; }
        public EnumEmpDACategory enumEmpDACategory { set; get; }
        #endregion DA Arrer Filters

        #region Pay Arrer Filters
        [Display(Name = "Employee Type")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Employee Type")]
        public int? selectedPayEmpTypeID { set; get; }
        public int? selectedPayEmpID { set; get; }
        public int? selectedPayBranchID { set; get; }
        public EnumPayBranch enumPayBranch { set; get; }
        public EnumEmpPayCategory enumEmpPayCategory { set; get; }
        #endregion Pay Arrer Filters

        public string ArrerPeriodDetailsDA { get; set; }
        public string ArrerPeriodDetailsPay { get; set; }
        public List<SelectListModel> listArrerPeriod { get; set; }
    }

    public enum EnumEmpDACategory
    {
        AllEmployees = 1,
        SingleEmployee = 2
    }
    public enum EnumDABranch
    {
        BranchesExcecptHO = 1,
        SingleBranch = 2
    }

    public enum EnumEmpPayCategory
    {
        AllEmployees = 1,
        SingleEmployee = 2
    }

    public enum EnumPayBranch
    {
        BranchesExcecptHO = 1,
        SingleBranch = 2
    }

    public class ArrerPeriodDetails
    {
        public string details { get; set; }
        public string fromperiod { get; set; }
        public string toperiod { get; set; }
    }
}
