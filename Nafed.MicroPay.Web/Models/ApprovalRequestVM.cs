using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
namespace MicroPay.Web.Models
{
    public class ApprovalRequestVM
    {
        public Nullable<int> BranchID { get; set; }
        public Nullable<DateTime> fromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public List<EmpAttendance> attendanceList { get; set; }
        public List<EmployeeLeave> leaveList { get; set; }

        public string RequestApprovalRemark { set; get; }

        public List<AppraisalFormHdr> appraisalForms { set; get; }

        public AppraisalFormApprovalFilter AppraisalFilters { set; get; }

        public List<APARSkillSetFormHdr> skillAssessmentForms { set; get; }

        public SkillAssessmentApprovalFilters skillAssessmentFilters { set; get; }
        public ConfirmationFormApprovalFilter ConfirmationFilters { get; set; }
        public List<ConfirmationFormHdr> confirmationFormsHdr { set; get; }

        public LTC ltcFilter { get; set; }

        #region Conveyance Bill Filter
        public ConveyanceBillApprovalFilter conveyanceBillFilters { get; set; }
        public List<ConveyanceBillFormHdr> conveyanceBillFormsHdr { set; get; }

        public NRPFLoanFormApprovalFilter NRPFloanFilters { get; set; }

        public List<NonRefundablePFLoan> NRPFloanHdr { set; get; }

        public Form11FormApprovalFilter Form11Filters { get; set; }

        public List<EmployeePFORG> Form11Hdr { set; get; }

        public FormRulesAttributes FormAttributes { get; set; }

        public List<AcarAparModel> acarAparModel { get; set; }

        #endregion
        public List<TrainingParticipant> participantsData { get; set; }
        public List<SeparationClearance> ClearanceList { get; set; }
    }

}