using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class AppraisalFormApprovalFilter
    {
        public int formID { set; get; }
        public List<SelectListModel> appraisalForms { set; get; }
        public List<SelectListModel> reportingYrs { set; get; } = new List<SelectListModel>();
        public List<SelectListModel> employees { set; get; } = new List<SelectListModel>();

        public int selectedFormID { set; get; }
        public int selectedReportingYr { set; get; }
        public int selectedEmployeeID { set; get; }

        public int loggedInEmployeeID { get; set; }
        public string selectedReportingYear { get; set; }

        public int? statusId { get; set; }

        public AppraisalFormState appraisalFormStatus { get; set; }
        public CompetencyFormState competencyFormState { get; set; }
        public ConveyanceFormStatus conveyanceFormState { get; set; }
        public bool ACAR { get; set; }
        public string reportingYr { get; set; }
        public int selectedBranchId { set; get; }
        public List<Model.SelectListModel> ddlBranch { get; set; }
    }

    public enum CompetencyFormState
    {
        [Display(Name = "Save")]
        SavedByEmployee = 1,
        [Display(Name = "Submitted")]
        SubmitedByEmployee = 2,
        [Display(Name = "Saved by Reporting Officer")]
        SavedByReporting = 3,
        [Display(Name = "Reviewed by Reporting Officer")]
        SubmitedByReporting = 4,
    }

    public enum ConveyanceFormStatus
    {
        [Display(Name = "Save")]
        SavedByEmployee = 1,
        [Display(Name = "Submitted")]
        SubmitedByEmployee = 2,
        [Display(Name = "Saved by Sectional Head")]
        SavedBySectionalHead = 3,
        [Display(Name = "Reviewed by Sectional Head")]
        SubmitedBySectionalHead = 4,
        [Display(Name = "Saved by Divisional Head")]
        SavedByDivisionalHead = 6,
        [Display(Name = "Approved by Divisional Head")]
        SubmitedByDivisionalHead = 7,
    }
}
