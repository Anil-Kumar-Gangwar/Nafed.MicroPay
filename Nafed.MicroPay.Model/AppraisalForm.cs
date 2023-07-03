using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class AppraisalForm
    {
        public int FormID { get; set; }
        public string FormName { get; set; }
        public bool IsDeleted { get; set; }
        public int EmployeeID { get; set; }
        public int? ReportingTo { get; set; }
        public int? ReviewingTo { get; set; }
        public int? AcceptanceAuthorityTo { set; get; }
        public int? loggedInEmpID { set; get; }
        public Model.FormGroupAHdr formGroupAHdr { get; set; } = new Model.FormGroupAHdr();
        public List<Model.FormGroupDetail1> formGroupADetail1List { get; set; }
        public List<Model.FormGroupDetail2> formGroupADetail2List { get; set; }

        public List<Model.FormTrainingDtls> formGroupATrainingDtls { set; get; }

        public SubmittedBy submittedBy { set; get; }

        public Model.FormGroupCHdr formGroupCHdr { get; set; } = new Model.FormGroupCHdr();

        [Required]
        public FormGroupDHdr formGroupDHdr { get; set; } = new FormGroupDHdr();

        [Required]
        public Model.FormGroupHHdr formGroupHHdr { get; set; } = new Model.FormGroupHHdr();

        [Required]
        public FormGroupGHdr formGroupGHdr { get; set; } = new FormGroupGHdr();

        public int DepartmentID { set; get; }
        public int DesignationID { set; get; }

        public ProcessWorkFlow _ProcessWorkFlow { get; set; } = new ProcessWorkFlow();
        public List<ProcessWorkFlow> _AutoPushWorkFlow { set; get; } = new List<ProcessWorkFlow>();
        public string ReportingYr { get; set; }
        public int FormGroupID { get; set; }
        public int FormState { get; set; }

        public double ApprovalHierarchy { get; set; }

        public FormRulesAttributes frmAttributes { get; set; }
        public bool isAdmin { get; set; } = false;

        public bool AnnualPropertyReturnSubmitted { set; get; }

        public DateTime? AnnualPropertySubmittedOn { set; get; }
        public bool empSection { get; set; } = false;
        public bool reportingSection { get; set; } = false;
        public bool reviewingSection { get; set; } = false;
        public bool acceptanceSection { get; set; } = false;
        public Nullable<System.DateTime> EmployeeSubmissionDueDate { get; set; }
        public Nullable<System.DateTime> ReportingSubmissionDueDate { get; set; }
        public Nullable<System.DateTime> ReviewerSubmissionDueDate { get; set; }
        public Nullable<System.DateTime> AcceptanceAuthSubmissionDueDate { get; set; }

        #region Upload Documents
        public string UploadRemarks { get; set; }
        public List<Model.APARReviewedSignedCopy> UploadedDocList { get; set; }
        public string UploadRemarksLength { get; set; }
        #endregion 

        public bool isValid { get; set; }
    }
    public enum FormGrade
    {
        Outstanding = 1,
        Good = 2,
        Poor = 3
    }
    public enum FormActivities
    {
        Target = 1,
        Objective = 2,
        Goal = 3
    }
    public enum SubmittedBy
    {
        Employee = 1,
        ReportingOfficer = 2,
        ReviewingOfficer = 3,
        AcceptanceAuthority = 4
    }
    public enum AppraisalFormState
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
        SubmitedByAcceptanceAuth = 8,
        [Display(Name = "Rejected by Reporting Officer")]
        RejectedbyReporting = 9

    }

    public enum FormPart4Integrity
    {
        [Display(Name = "Above Board")]
        AboveBoard = 1,
        [Display(Name = " Nothing Definite can be said")]
        NothingDefinite = 2,
        [Display(Name = "Doubtful")]
        Doubtful = 3,
        [Display(Name = "Beyond Doubt")]
        BeyondDoubtful = 4
    }
    public enum RatingGuide
    {
        OUTSTANDING = 1,
        [Display(Name = "VERY GOOD")]
        VERYGOOD = 2,
        GOOD = 3,
        FAIR = 4,
        POOR = 5
    }
    public class FormRulesAttributes
    {
        public int FormID { get; set; }
        public int FormGroupID { get; set; }
        public int FormState { get; set; }
        public int? SenderID { get; set; }
        public int? ReciverID { get; set; }
        public SubmittedBy SubmittedBy { get; set; }

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
}
