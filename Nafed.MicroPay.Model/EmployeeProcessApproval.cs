using Nafed.MicroPay.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class EmployeeProcessApproval
    {
        public int EmpProcessAppID { get; set; }
        public int ProcessID { get; set; }
        public int EmployeeID { get; set; }
        public int RoleID { get; set; }
              
        [Display(Name = "Reporting 1")]
        //[Required]
        //[Range(1,Int64.MaxValue,ErrorMessage = "Please select Reporting 1")]
        public int ReportingTo { get; set; }
        [Display(Name = "Reporting 2")]
        public Nullable<int> ReviewingTo { get; set; }
        [Display(Name = "Reporting 3")]
        public Nullable<int> AcceptanceAuthority { get; set; }
        public System.DateTime Fromdate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        [Display(Name ="Process") ]
        [Required(ErrorMessage ="Please select process.")]
        public WorkFlowProcess EmpProcessApproval { get; set; }
               
        public string ProcessName { get; set; }

        public int OldReportingTo { get; set; }    
        public Nullable<int> OldReviewingTo { get; set; }       
        public Nullable<int> OldAcceptanceAuthority { get; set; }
        // public ProcessApprovalManager LeaveApproverManagers { get;set;}
        public string ReportingToName { get; set; }
        public string ReviewerName { get; set; }

        public string AcceptanceAuthorityName { get; set; }
        public int sno { get; set; }
        public bool ? MultiReporting { get; set; }
        public bool IsDeleted { get; set; }
    }


    public class EmployeeProcessApprovalVM
    {
        public List<EmployeeProcessApproval> empProcessApp { set; get; } = new List<EmployeeProcessApproval>();
        public List<SelectListModel> EmployeeList { get; set; }

        public EmployeeProcessApproval empProcess { set; get; }
        public bool _reportingError { get; set; } = false;
        public int ProcessId { get; set; }
        public int ApprovalType { get; set; }
        public DateTime ? ClearanceDateUpto { get; set; }
    }

    public class ProcessApprovalManager {
        public int ProcessID { get; set; }
        public string ReportingToName { get; set; }
        public string ReviewerName { get; set; }
        public string AcceptanceAuthName { get; set; }

    }
}
