using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nafed.MicroPay.Model
{
    public class ExgratiaList
    {
       public Nullable<int> ExgratiaID { get; set; }

        [Display(Name = "Employee Type")]
        public int EmpTypeID { set; get; }

        [Display(Name = "Branch")]
        public int? branchID { set; get; }
        public string branchName { set; get; }
        public List<SelectListModel> branchList { set; get; }

        [Display(Name = "Employee")]
        [Required(ErrorMessage = "Please Select Employee")]
        public int EmployeeID { get; set; }
        public string EmpCode { get; set; }

        

        public string Name { get; set; }
        public decimal Amt_Tot { get; set; }
        [Display(Name = "Is Percentage")]
        public bool flag { get; set; }

        public decimal net_amount { get; set; }
        [Display(Name = "Financial Year")]
        [Required(ErrorMessage = "Please Select Year")]
        public string FinancialYear { get; set; }

        public int salyear { get; set; }
        public int salmonth { get; set; }

        public decimal TOT1 { get; set; }
        [Display(Name = "No. of days")]
        public int noofdays { get; set; }

        public decimal Other { get; set; }
        [Display(Name = "Other Deduction")]
        public bool OtherDeduction { get; set; }
        public decimal ExGratiaOneday { get; set; }
        [Display(Name = "Income Tax")]
        public decimal Incometax { get; set; }
        public string FormActionType { get; set; }

       
        public bool Saved { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        
    }
}
