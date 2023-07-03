using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmployeeLeave
    {

        public int LeaveID { get; set; }
        [DisplayName("Employee Name : ")]
        public int EmployeeId { get; set; }


        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [DisplayName("From :")]
        [Required(ErrorMessage = "Please Enter From Date")]
        public Nullable<System.DateTime> DateFrom { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [DisplayName("To :")]
        [Required(ErrorMessage = "Please Enter To Date")]
        public Nullable<System.DateTime> DateTo { get; set; }

        [Required(ErrorMessage = "Please Select Leave Type")]
        [DisplayName("Leave Type :")]
        public int LeaveCategoryID { get; set; }
        [DisplayName("Status :")]
        public int StatusID { get; set; }
        [Required(ErrorMessage = "Please Enter Reason")]
        [DisplayName("Reason :")]
        public string Reason { get; set; }
        [DisplayName("Days :")]
        public decimal Unit { get; set; }
        [DisplayName("Reporting Officer : ")]
        public Nullable<int> ReportingOfficer { get; set; }
        [DisplayName("Leave Balance : ")]
        public decimal LeaveBalance { get; set; }
        
        [DisplayName("Document :")]
        public string DocumentName { get; set; }
        public string DocumentFilePath { get; set; }

        //[Required(ErrorMessage = "Please Select Pupose")]
        [DisplayName("Purpose :")]
        public int LeavePurposeID { get; set; }
        [DisplayName("Purpose Other : ")]
        public string PurposeOthers { get; set; }
        public int CreatedBy { get; set; }


        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string EmployeeCode { get; set; }
        public string TitleName { get; set; }
        [DisplayName("Staff :")]
        public string EmployeeName { get; set; }

        public string LeaveCategoryName { get; set; }
        public string StatusName { get; set; }
        [DisplayName("Branch :")]
        public string Branch { get; set; }

        public int BranchId { set; get; }

        [DisplayName("Designation :")]
        public string DesignationName { set; get; }
        [DisplayName("Office :")]
        public string OfficeName { set; get; }

        [DisplayName("Supervisor :")]

        public string ReportingOfficerName { set; get; }

        [DisplayName("Contact :")]
        public string ReportingOfficeContactNumber { set; get; }

        [DisplayName("Address :")]
        public string ReportingOfficeAddress { set; get; }
       
        public DayType FromdayType { set; get; }     
        public DayType TodayType { set; get; }
        public Nullable<System.Int32> DateFrom_DayType { set; get; }

        public Nullable<System.Int32> DateTo_DayType { set; get; }

        public EmpLeaveBalance empLeaveBalance { set; get; }

        public string LeaveCode { get; set; }

        public string LeaveType { get; set; }
        public Nullable<double> OB { get; set; }
        public Nullable<double> CR { get; set; }
        public Nullable<double> DR { get; set; }
        public Nullable<double> Bal { get; set; }

        public int ? ReportingTo  { set; get; }

        public Nullable<int> ReviewerTo { set; get; }
        public Nullable<int> AcceptanceAuthority { set; get; }

        public string ReporotingToRemark { set; get; }
        public string ReviewerToRemark { set; get; }


        [DisplayName("Reviewer Name :")]

        public string ReviewerName { set; get; }

        public int? ApproverType { set; get; }
        public string ActionType { set; get; }

        public int totalUnit { set; get; }
        public int Sno { set; get; }
        public int? DependentonLeaveBal { set; get; }
        public bool mapleave { set; get; }
        public ProcessWorkFlow _ProcessWorkFlow { get; set; } = new ProcessWorkFlow();
        [Display(Name ="Approval Upto : ")]
        public int ApprovalRequiredUpto { get; set; }
        public string AcceptanceAuthorityRemark { get; set; }

        public int DesignationID { set; get; }

        public int? RequestType { get; set; }

        public Nullable<int> EmployeeTypeID { get; set; }

    }

    public class EmpLeaveBalance 
    {
       public  Nullable< double> ELOpeningBal { set; get; }
        public Nullable<double> ELAccrued { get; set; }
        public Nullable<double> ELAvailed { set; get; }
        public Nullable<double> ELBal {
            get;
            set;
        }

        public Nullable<double> MLOpeningBal { set; get; }
        public Nullable<double> MLAccrued { get; set; }
        public Nullable<double> MLAvailed { set; get; }


      //  public Nullable<double> MLBalance { get; set; } 
        public Nullable<double> MLBal
        {

            set;
            get;
            
        }

        public Nullable<double> CLOpeningBal { set; get; }
        public Nullable<double> CLAccrued { get; set; }
        public Nullable<double> CLAvailed { set; get; }
        public Nullable<double>CLBal
        {
            set;
            get
            ;
        }

        public Nullable<double> MEOpeningBal { set; get; }
        public Nullable<double> MEAccrued { get; set; }
        public Nullable<double> MEAvailed { set; get; }
        public Nullable<double> MEBal
        {
            set;
            get
            ;
        }


        //public Nullable<double> ELBalance { get; set; }
        //public Nullable<double> CLBalance { get; set; }

    }

    public enum DayType
    {
        HalfDay = 1,
        FullDay = 2
    }

}
