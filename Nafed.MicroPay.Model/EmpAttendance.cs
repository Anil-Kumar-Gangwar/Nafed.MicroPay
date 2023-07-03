using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmpAttendance
    {
        public int EmpAttendanceID { get; set; }
        public int EmployeeId { get; set; }
        public System.DateTime ProxydateIn { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string Remarks { get; set; }
        public int MarkedBy { get; set; }
        public int Attendancestatus { get; set; }
        public Nullable<int> Reportingofficer { get; set; }
        public string RejectionRemarks { get; set; }
        public int TypeID { get; set; }
        public string Mode { get; set; }
        public string Old_Intime { get; set; }
        public string Old_outtime { get; set; }
        public System.DateTime ProxyOutDate { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int BranchID { get; set; }
        public Nullable<int> ReviewerTo { get; set; }
        public string ReportingToRemark { get; set; }
        public string ReviewerToRemark { get; set; }
        public int? ApproverType { set; get; }

        public string ActionType { set; get; }

        public string EmployeeName { set; get; }

        public EmployeeProcessApproval EmpAttendanceApprovalSettings { get; set; } = new EmployeeProcessApproval();

        public string AcceptanceAuthRemark { get; set; }
        public ProcessWorkFlow _ProcessWorkFlow { get; set; } = new ProcessWorkFlow();

        public bool Approved { get; set; }
        public bool Rejected { get; set; }

        public string SenderName{ get; set; }
        public string SenderDesignation { get; set; }

        public string LocationName { get; set; }
        public int ? LocationID { get; set; }
        public string BranchName { get; set; }

        public DateTime OrderDate { get; set; }
        public Nullable<System.DateTime> ReleavingDate { get; set; }
        public Nullable<System.DateTime> JoiningDate { get; set; }
        public Nullable<System.DateTime> JoinBackDate { get; set; }
        public Nullable<System.DateTime> ReleavDateFromLoc { get; set; }
    }
}
