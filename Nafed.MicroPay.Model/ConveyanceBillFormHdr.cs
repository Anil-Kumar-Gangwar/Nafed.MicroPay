using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ConveyanceBillFormHdr
    {
        public int ConveyanceHdrID { get; set; }
        public string Year { get; set; }
        public int EmployeeId { get; set; }
        public int StatusID { get; set; }
        public string FormName { set; get; }
        public string EmployeeCode { set; get; }
        public string DepartmentName { set; get; }
        public string DesignationName { set; get; }
        public string EmpName { set; get; }
        public int EmployeeID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string Status { get; set; }
        public EmployeeProcessApproval EmpProceeApproval { set; get; } = new EmployeeProcessApproval();
        public int? DepartmentID { get; set; }
        public int? DesignationID { get; set; }
        public int ConveyanceBillDetailID { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool IsReportingRejected { get; set; }
        public bool IsReviewingRejected { get; set; }
        public string ReportingRemarks { get; set; }
        public string ReviewingRemarks { get; set; }
    }

    public enum ConveyanceFormState
    {
        [Display(Name = "Save")]
        SavedByEmployee = 1,
        [Display(Name = "Submitted")]
        SubmitedByEmployee = 2,
        [Display(Name = "Saved by Reporting Officer")]
        SavedByReporting = 3,
        [Display(Name = "Reviewed by Reporting Officer")]
        SubmitedByReporting = 4,
        [Display(Name = "Rejected by Reporting Officer")]
        RejectedByReporting = 5,
        [Display(Name = "Saved by Reviewer Officer")]
        SavedByReviewer = 6,
        [Display(Name = "Approved by Reviewer Officer")]
        SubmitedByReviewer = 7,
        [Display(Name = "Rejected by Reviewer Officer")]
        RejectedByReviewer = 8,
        [Display(Name = "Saved by Acceptance Authority")]
        SavedByAcceptanceAuth = 9,
        [Display(Name = "Accepted by Acceptance Authority")]
        SubmitedByAcceptanceAuth = 10
    }
}
