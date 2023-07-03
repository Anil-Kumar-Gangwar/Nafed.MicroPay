using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
   public class AppraisalFormDueDate
    {
        public int FormID { get; set; }
        public string FormName { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Employee Submission")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        // [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage ="Please enter date")]
        public DateTime EmployeeSubmissionDueDate { get; set; }

        [Display(Name = "Reporting Submission")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [Required(ErrorMessage = "Please enter date")]
        // [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReportingSubmissionDueDate { get; set; }

        [Display(Name = "Reviewer Submission")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [Required(ErrorMessage = "Please enter date")]
        // [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReviewerSubmissionDueDate { get; set; }

        [Display(Name = "AcceptanceAuth Submission")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [Required(ErrorMessage = "Please enter date")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AcceptanceAuthSubmissionDueDate { get; set; }

        [Display(Name = "Reporting Yr")]
        public string ReportingYear { get; set; }
    }
}
