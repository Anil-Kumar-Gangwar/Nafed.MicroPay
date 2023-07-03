using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
    public class CommonFilter
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? StatusID { get; set; }
        public int? EmployeeID { get; set; }
        public int loggedInEmployee { get; set; }
        public int? DesignationID { get; set; }
        public string ReportType { get; set; }
        public int? BranchID { get; set; }
        public int ProcessID { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> StartDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DateCompare("StartDate")]
        public Nullable<System.DateTime> EndDate { get; set; }

        public int? Year { get; set; }
        public MonthEnum? Month { get; set; }

        public int[] CheckBoxList { get; set; }

        public string ReportingYear { get; set; }

    }

    public enum MonthEnum
    {
        [Display(Name = "January")]
        one = 1,
        [Display(Name = "February")]
        two = 2,
        [Display(Name = "March")]
        three = 3,
        [Display(Name = "April")]
        four = 4,
        [Display(Name = "May")]
        five = 5,
        [Display(Name = "June")]
        six = 6,
        [Display(Name = "July")]
        seven = 7,
        [Display(Name = "August")]
        eight = 8,
        [Display(Name = "September")]
        nine = 9,
        [Display(Name = "October")]
        ten = 10,
        [Display(Name = "November")]
        eleven = 11,
        [Display(Name = "December")]
        tewlve = 12
    }
}
