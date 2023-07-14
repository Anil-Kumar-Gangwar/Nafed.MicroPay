using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model.Employees
{
   public class NonRegularEmployeesExtension
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "Notice Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Select Notice Date")]
        public Nullable<System.DateTime> ExtentionNoticeDate { get; set; }
        [Display(Name = "From Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Select From Date")]
        public Nullable<System.DateTime> ExtentionFromDate { get; set; }
        [Display(Name = "To Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Select To Date")]
        public Nullable<System.DateTime> ExtentionToDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }
        public bool Deleted { get; set; }
        public String EmplyeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string ValidationMessage { get; set; }
        public bool IsValidInputs { get; set; } = true;
    }
}
