using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class NonRefundablePFLoan
    {
        public int ID { get; set; }
        [DisplayName("Employee Name : ")]

        public int NRPFLoanID { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public int StatusID { get; set; }
        public string Employeecode { get; set; }
        public string Employeename { get; set; }

        public string HBName { get; set; }

        public string AccountNo { get; set; }
        public string Branchname { get; set; }

        public string Paddress { get; set; }

        public string AcceptanceAuthority { get; set; }

        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }

        public int? DepartmentID { get; set; }
        public int? DesignationID { get; set; }

        public string BankAcNo { get; set; }
        

        public decimal BasicPay { get; set; }

        public decimal DA { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> currentDate { get; set; }
        [Required(ErrorMessage = "Please enter amount of advanced")]


        [Range(0,9999999999, ErrorMessage = "Invalid Number")]
        //[Range(0, 99999999, ErrorMessage = "Invalid Number")]
        public decimal Amount_of_Advanced { get; set; }

         [Range(1, Int32.MaxValue, ErrorMessage = "Please select status")]
        public int Purpose_of_Advanced { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Date_of_Sanction { get; set; }
        public string LocationOfDwellingSite { get; set; }
        public string NamePresentOwner { get; set; }
        public string AddressPresentOwner { get; set; }
        public string PresentStateofDwelling { get; set; }
        public int DesiredModeofRemittance { get; set; }
        public string ListofDocuments { get; set; }
        public int[] ListID { get; set; }
        public string hdnCheckedVal { get; set; }
        public bool IsDeleted { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }

        public int? loggedInEmpID { set; get; }
        public short FormStatus { get; set; }
        public EmployeeProcessApproval EmpProceeApproval { set; get; } = new EmployeeProcessApproval();

        public double? ApprovalHierarchy { get; set; }

        public FormRulesAttributes frmAttributes { get; set; }
    
        public List<Model.SelectListModel> RemittanceDesiredMode { get; set; }
        public List<Model.SelectListModel> POA { get; set; }

        public List<Model.SelectListModel> listDetails { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Requestdate { get; set; }

    }


    public enum NRPFLoanFormState
    {
        [Display(Name = "Pending")]
        Pending = 0,
        [Display(Name = "Saved")]
        SavedByEmployee = 1,
        [Display(Name = "Submitted")]
        SubmitedByEmployee = 2,
        [Display(Name = "Accepted by Seceratry")]
        AcceptedByReporting = 3,
        [Display(Name = "Rejected by Seceratry")]
        RejectedByReporting =4
       

    }
}
