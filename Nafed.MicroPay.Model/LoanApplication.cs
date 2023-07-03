using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
    public class LoanApplication
    {
        public int ApplicationID { get; set; }
        public int EmployeeID { get; set; }
        [Range(1, 9999999999, ErrorMessage = "Please enter Loan Amount")]
        public decimal LoanAmount { get; set; }
        [Range(1, 8, ErrorMessage = "Please select Reason for Loan Applied")]
        public byte LoanReason { get; set; }
        [Required(ErrorMessage = "Please enter Ceremony")]
        public string Ceremony { get; set; }
        [Required(ErrorMessage = "Please enter Ceremony Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CeremonyDate { get; set; }
        [Required(ErrorMessage = "Please enter Ceremony Place")]
        public string CeremonyPlace { get; set; }
        public string DeathPlace { get; set; }
        public byte StatusID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string MembershipNo { get; set; }
        public Nullable<System.DateTime> Dateofjoining { get; set; }
        public string PFNo { get; set; }

        public string Remarks { get; set; }
        public short ApplicationStatusID { get; set; }
        public EmployeeProcessApproval approvalSetting { set; get; } = new EmployeeProcessApproval();
        public int loggedInEmpID { get; set; }
        public int ReportingTo { get; set; }
        public string DepartmentName { get; set; }
        public ProcessWorkFlow _ProcessWorkFlow { get; set; } = new ProcessWorkFlow();
    }

    public enum LoanApplicationStatus
    {
        Pending=1,
        Accept=2,
        Reject=3
    }

    public class LoanApplicationFilter
    {
      
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FromDate { get; set; }

       
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ToDate { get; set; }

        public int? EmployeeID { get; set; }
        public List<SelectListModel> EmployeeList { set; get; } = new List<SelectListModel>();
        public LoanApplicationStatus ? ApplicationStatusID { get; set; }


    }
}
