using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ArrearApprovalRequest
    {
        public Nullable<int> PeriodForm { get; set; }
        public Nullable<int> PeriodTo { get; set; }
        public bool requestExist { set; get; }
        public int CreatedBy { set; get; }
        public DateTime CreatedOn { set; get; }
        public int ProcessID { set; get; }

        public int salMonth { set; get; }
        public int salYear { set; get; }

        [Display(Name = "Single Branch")]
        public bool SingleBranch { set; get; }

        public bool IsUpdateDone { get; set; }
        public List<SelectListModel> Branchs { set; get; }
        public List<SelectListModel> Employees { set; get; }
        public List<SelectListModel> EmployeeTypes { set; get; }

        [Display(Name = "All Branches(Except Head-Office)")]
        public bool BranchesExcecptHO { set; get; }


        [Display(Name = "Single Employee")]
        public bool SingleEmployee { set; get; }

        [Display(Name = "All Employees")]
        public bool AllEmployees { set; get; }

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
}
