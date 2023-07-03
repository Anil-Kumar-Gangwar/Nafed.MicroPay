using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class RetiredEmployeeReportFilter
    {
        [Required(ErrorMessage = "Please Enter From Date")]
        [Display(Name ="From Date :")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ? fromDate { set; get; }

        [Required(ErrorMessage = "Please Enter To Date")]
        [Display(Name = "To Date :")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ? toDate { set; get; }

        [Display(Name = "All Employee")]
        public bool allEmployee { set; get; }
    }
}