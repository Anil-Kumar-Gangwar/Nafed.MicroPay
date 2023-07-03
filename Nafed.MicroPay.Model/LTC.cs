using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class LTC
    {
        public int LTCID { get; set; }
        public int LTCNo { get; set; }
        public string hdnCheckedVal { get; set; }
        public IEnumerable EmployeeList { get; set; }
        [DisplayName("Branch :")]
        public int BranchId { get; set; }
        [DisplayName("Employee :")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select employee")]
        public int EmployeeId { get; set; }
        public string Employeecode { get; set; }
        public string Employeename { get; set; }
        [DisplayName("Particulars of family members :")]
        public string Dependents { get; set; }
        public int[] Dependentid { get; set; }

        [DisplayName("Whether Self Avail :")]
        public bool WhetherSelf { get; set; }

        [DisplayName("Where Wants to Go :")]
        public int HomeTown { get; set; }

        [DisplayName("Where in India :")]
        public string WhereDetail { get; set; }

        [DisplayName("Initial Calculation of Amt :")]
        public decimal InitialcalAmount { get; set; }
        [DisplayName("Tentative Advance Given:")]
        public decimal TentativeAdvance { get; set; }

        [DisplayName("Distance in K.M. :")]
        public decimal Distance { get; set; }
        [DisplayName("Date of Application :")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateofApplication { get; set; }

        [Required(ErrorMessage = "Please provide Date")]
        [DisplayName("Date of Return :")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public Nullable<System.DateTime> DateofReturn { get; set; }
        [Required(ErrorMessage = "Please provide Date")]
        [DisplayName("Date  of Availing LTC :")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public Nullable<System.DateTime> DateAvailLTC { get; set; }
        [DisplayName("LTC  Bill Amount : ")]
        public decimal LTCBillAmt { get; set; }

        [DisplayName("Settlement Done : ")]
        public decimal Settlementdone { get; set; }
        [DisplayName("Detail : ")]
        public string Detail { get; set; }
        [DisplayName("Natue Of Leave : ")]
        public int Natureofleave { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        [Required(ErrorMessage = "Please provide Date")]
        [DisplayName("From :")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LeaveFrom { get; set; }
        [Required(ErrorMessage = "Please provide Date")]
        [DisplayName("To :")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LeaveTo { get; set; }
        public string SHComment { get; set; }
        public string DHComment { get; set; }
        public string SpouseOrg { get; set; }

        public string ReportingYear { get; set; }

        public EmployeeProcessApproval approvalSetting { set; get; } = new EmployeeProcessApproval();
        public int loggedInEmpID { get; set; }

        public double? ApprovalHierarchy { get; set; }
        public string ReportingTo { get; set; }
        public string ReportingDesignation { get; set; }
        public string ReviewerTo { get; set; }
        public string ReviewerDesignation { get; set; }
        public string AcceptanceAuth { get; set; }
        public string AcceptanceDesignation { get; set; }
        public Nullable<System.DateTime> RTSenddate { get; set; }
        public Nullable<System.DateTime> RVSenddate { get; set; }
        public Nullable<System.DateTime> AASenddate { get; set; }
        public Nullable<System.DateTime> EmpSenddate { get; set; }
        public short FormStatus { get; set; }
        public FormRulesAttributes frmAttributes { get; set; }

        public string LTCRefrenceNumber { get; set; }

        public string DependentName { get; set; }
        public string DealingAssistant { get; set; }
    }
}
