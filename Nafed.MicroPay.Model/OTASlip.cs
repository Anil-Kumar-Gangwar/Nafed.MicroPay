using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
   public class OTASlip
    {
        public int ApplicationID { get; set; }
        public int EmployeeID { get; set; }
    
        [Required(ErrorMessage = "Please enter Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> HolidayDate { get; set; }

        [Required(ErrorMessage = "Please enter Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AttendedDate { get; set; }

        [Required(ErrorMessage = "Please enter Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> IndicatedDate { get; set; }

        
        public byte StatusID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }

        public EmployeeProcessApproval approvalSetting { set; get; } = new EmployeeProcessApproval();
        public int loggedInEmpID { get; set; }
        public int ReportingTo { get; set; }
        public int ? ReviewingTo { get; set; }
        [Required(ErrorMessage ="Please enter Instructed By.")]
        public string InstructedBy { get; set; }
        public string AllowedBy { get; set; }
        public string ReportingName { get; set; }
        public string ReviewingName { get; set; }
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Please select Time")]
        [Display(Name = "From Time")]
        public Nullable<TimeSpan> HolidayFromTime { get; set; }

        [Required(ErrorMessage = "Please select Time")]
        [Display(Name = "To Time")]
        public Nullable<TimeSpan> HolidayToTime { get; set; }

        [Required(ErrorMessage = "Please select Time")]
        [Display(Name = "From Time")]
        public Nullable<TimeSpan> AttendedFromTime { get; set; }

        [Required(ErrorMessage = "Please select Time")]
        [Display(Name = "To Time")]
        public Nullable<TimeSpan> AttendedToTime { get; set; }

       public List<SelectListModel> EmployeeList = new List<SelectListModel>();
        public ProcessWorkFlow _ProcessWorkFlow { get; set; } = new ProcessWorkFlow();

       public int? receiverID { get; set; }
        public string ReportingRemark { get; set; }
        public string ReviewingRemark { get; set; }
    }
}
