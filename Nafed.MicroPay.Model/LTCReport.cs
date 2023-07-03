using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class LTCReport
    {
        public int LTCID { get; set; }
        public int LTCNo { get; set; }
        public string RefrenceNumber { get; set; }
        public int EmployeeId { get; set; }
        public string Employeecode { get; set; }
        public string Employeename { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string WhereDetail { get; set; }
        public int HomeTown { get; set; }
        public string HomeTownDetails { get; set; }
        public string DateofApplication { get; set; }
        public string DateofReturn { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public string DateAvailLTC { get; set; }
        public string ReportingYear { get; set; }
        public string ReportingTo { get; set; }
        public string ReportingDesignation { get; set; }
        public string ReportingDepartment { get; set; }
        public string ReviewerTo { get; set; }
        public string ReviewerDesignation { get; set; }
        public string ReviewerDepartment { get; set; }
        public string AcceptanceAuth { get; set; }
        public string AcceptanceDesignation { get; set; }
        public string AcceptanceDepartment { get; set; }
        public List<LTCDependentList> listDependentList { get; set; }
        public int FormStatus { get; set; }
        public string leaveType { get; set; }
        public string Dependent { get; set; }
        public DateTime EmployeeDOB { get; set; }
        public int TotalLeaveDays { get; set; }
        public int? ReportingToID { get; set; }
        public int? ReviewerToID { get; set; }
        public int? AcceptanceAuthID { get; set; }
        [Required(ErrorMessage = "Please Enter Reference Number")]
        public string LTCReferenceNumber { get; set; }

        public string AppliedLeave { get; set; }
        public string HolidayLeave { get; set; }
        public bool Validate { get; set; } = true;
        public string ValidationMessage { get; set; } = "";
    }
}
