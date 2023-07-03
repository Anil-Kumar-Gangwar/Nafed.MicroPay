using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class APARSkillSetFormHdr
    {
        public int APARHdrID { get; set; }
        public int EmployeeID { get; set; }
        public string ReportingYr { get; set; }
        public string DepartmentName { set; get; }
        public string DesignationName { set; get; }
        public string EmployeeName { set; get; }

        public string EmployeeCode { set; get; }
        public int StatusID { get; set; }
        public int DepartmentID { get; set; }
        public int DesignationID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string Status { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> WorkedPeriodUnderROFrom { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> WorkedPeriodUnderROTo { get; set; }
        public string PlaceOfJoin { get; set; }
        public string Qualification { get; set; }
        public System.DateTime DOJ { get; set; }
        [Display(Name = "Date of Joining")]
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DOPP { get; set; }
        public string ReportingTo { get; set; }
        public string ReportingDesignation { get; set; }
        public Nullable<System.DateTime> RTSenddate { get; set; }
        public Nullable<System.DateTime> EmpSenddate { get; set; }
        public APARFormState formState { set; get; }
        public EmployeeProcessApproval EmpProceeApproval { set; get; } = new EmployeeProcessApproval();
    }
    public enum APARFormState
    {
        [Display(Name = "Save")]
        SavedByEmployee = 1,
        [Display(Name = "Submitted")]
        SubmitedByEmployee = 2,
        [Display(Name = "Saved by Reporting Officer")]
        SavedByReporting = 3,
        [Display(Name = "Reviewed by Reporting Officer")]
        SubmitedByReporting = 4,
        [Display(Name = "Saved by Reviewer Officer")]
        SavedByReviewer = 5,
        [Display(Name = "Reviewed by Reviewer Officer")]
        SubmitedByReviewer = 6,
        [Display(Name = "Saved by Acceptance Authority")]
        SavedByAcceptanceAuth = 7,
        [Display(Name = "Accepted by Acceptance Authority")]
        SubmitedByAcceptanceAuth = 8

    }
}
