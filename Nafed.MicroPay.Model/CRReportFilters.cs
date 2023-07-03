using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nafed.MicroPay.Model
{
    public class CRReportFilters
    {
        [Display(Name = "Employee")]
        public int? employeeID { get; set; }
        public List<SelectListModel> employeeList { set; get; }

        [Display(Name = "Employee")]
        public int? employeeID1 { get; set; }
        public List<SelectListModel> employeeList1 { set; get; }

        [Display(Name = "For Year")]
        public int? yearID { get; set; }
        public List<SelectListModel> yearList { set; get; }

        [Display(Name = "For Year")]
        public int? yearID1 { get; set; }
        public List<SelectListModel> yearList1 { set; get; }

        [Display(Name = "All Employee")]
        public bool AllEmployee { get; set; }
        public CRFormRadios crFormRadio { get; set; }
        //public CRCoveringRadio crCoveringRadio { get; set; }
    }
    public enum CRFormRadios
    {
        [Display(Name = "Form A (Order)")]
        FormA = 1,
        [Display(Name = "Form B")]
        FormB = 2,
        [Display(Name = "Form C")]
        FormC = 3,
        [Display(Name = "Form D")]
        FormD = 4,
        [Display(Name = "Form B(Order)")]
        FormBOrder = 5,
        [Display(Name = "Covering Letter (Head Office)")]
        CoveringLetterHO = 6,
        [Display(Name = "Covering Letter (Branch)")]
        CoveringLetterBranch = 7,
    }

}
