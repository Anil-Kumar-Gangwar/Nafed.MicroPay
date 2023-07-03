using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class SalaryMonthlyInput
    {
        [Display(Name = "Select Monthly Input Head")]
        [Required(ErrorMessage = "Select Monthly Input Head")]
        public string MonthlyInputHeadId { get; set; }
        public IEnumerable<SalaryHeadField> MonthlyInputHead { set; get; }
        [Display(Name = "Employee")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Select Employee")]
        public int EmployeeId { get; set; }
        public List<SelectListModel> RegularEmployeeList { get; set; }
        [Display(Name = "Enter Amount / Hours / Monthly Input For This Salary")]
        //public int Amount { get; set; }
        public decimal Amount { get; set; }
        public string BranchName { get; set; }
        //public int OldAmount { get; set; }

        public decimal OldAmount { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "Select Employee Tye")]
        [Display(Name = "Employee Type")]
        public int EmployeeTypeId { get; set; }
        public List<SelectListModel> EmployeeTypeList { get; set; }

        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayName("Monthly Date :")]
        //[Required(ErrorMessage = "Please Enter Date")]
        //public DateTime? MonthlyDate { get; set; }

        [DisplayName("Month :")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select month")]
        public int monthId { get; set; }
        [DisplayName("Year :")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select year")]
        public int yearId { get; set; }
    }

    public class SalaryMonthlyBranchAndAmount
    {
        public string BranchName { get; set; }
        public int Amount { get; set; }
    }
}
