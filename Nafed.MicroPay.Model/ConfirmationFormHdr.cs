using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ConfirmationFormHdr
    {
        public int FormHdrID { get; set; }
        public Nullable<int> FormTypeID { get; set; }

        public int ProcessID { get; set; }
        public int EmployeeID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string FormTypeName { set; get; }

        public string FormConfirmationType { set; get; }

        public string EmployeeCode { set; get; }
        public string EmpName { set; get; }
        public Nullable<int> StatusID { get; set; }
        public EmployeeProcessApproval EmpProceeApproval { set; get; } = new EmployeeProcessApproval();
        public ConfirmationFormDetail confirmationFormDetail { get; set; }
        public List<ConfirmationClarification> confirnationClarification { get; set; }
        public int? ReportingTo { get; set; }
        public int? ReviewingTo { get; set; }
        public ConfirmationFormRulesAttributes confirmationFormRulesAttributes { get; set; }
        public int ApprovalHierarchy { get; set; }
        public int? loggedInEmpID { set; get; }

        public ConfirmationSubmittedBy submittedBy { set; get; }
        public ProcessWorkFlow _ProcessWorkFlow { get; set; } = new ProcessWorkFlow();

        public ConfirmationClarification _ConfirmationClarification { get; set; }

        public string ReviewingName { get; set; }
        public string ReviewingDesignation { get; set; }
        public string ReportingName { get; set; }
        public string ReportingDesignation { get; set; }

        public bool ToShow { get; set; }
        public int EmpProcessAppID { get; set; }

        public string ReportingRemark { get; set; }
        public string ReviewerRemark { get; set; }
        public int FormABHeaderID { get; set; }

    }

    public class ConfirmationFormRulesAttributes
    {
        public int FormID { get; set; }
        public int FormGroupID { get; set; }
        public int? FormState { get; set; }
        public int? SenderID { get; set; }
        public int? ReciverID { get; set; }
        public ConfirmationSubmittedBy SubmittedBy { get; set; }

        public bool EmployeeSection { get; set; }
        public bool ReportingSection { get; set; }
        public bool ReviewerSection { get; set; }
        public bool ReportingButton { get; set; }
        public bool ReviewerButton { get; set; }
        public bool AcceptanceButton { get; set; }
        public DateTime EmployeeSubmissionDate { get; set; }
        public DateTime ReportingSubmissionDate { get; set; }
        public DateTime ReviewerSubmissionDate { get; set; }
        public DateTime AcceptanceSubmissionDate { get; set; }
    }

    public enum ConfirmationSubmittedBy
    {
        Employee = 1,
        ReportingOfficer = 2,
        ReviewingOfficer = 3,
        AcceptanceAuthority = 4
    }

    public enum ConfirmationFormState
    {
        [Display(Name = "Pending")]
        Pending = 1,
        //[Display(Name = "Submitted")]
        //SubmitedByEmployee = 2,
        [Display(Name = "Saved by Reporting Officer")]
        SavedByReporting = 2,
        [Display(Name = "Accepted by Reporting Officer")]
        SubmitedByReporting = 3,
        [Display(Name = "Rejected by Reporting Officer")]
        RejectedByReporting = 4,
        [Display(Name = "Saved by Reviewer Officer")]
        SavedByReviewer = 5,
        [Display(Name = "Accepted by Reviewing Officer")]
        SubmitedByReviewer = 6,
        [Display(Name = "Rejected by Reviewing Officer")]
        RejectedByReviewer = 7,
        [Display(Name = "Confirmed")]
        Confirmed = 8,
        [Display(Name = "Rejected")]
        Rejected = 9
    }

    public class CommonDetails
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
    }
}
