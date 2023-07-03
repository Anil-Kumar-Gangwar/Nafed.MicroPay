using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ConveyanceBillForm
    {
        public int ConveyanceDetailID { get; set; }
        public int FormID { get; set; }
        public string FormName { get; set; }
        public bool IsDeleted { get; set; }
        public int EmployeeID { get; set; }
        public int? ReportingTo { get; set; }
        public int? ReviewingTo { get; set; }
        public int? AcceptanceAuthorityTo { set; get; }
        public int? loggedInEmpID { set; get; }
        public ConveyanceSubmittedBy submittedBy { set; get; }
        public int DepartmentID { set; get; }
        public int DesignationID { set; get; }
        public ProcessWorkFlow _ProcessWorkFlow { get; set; } = new ProcessWorkFlow();
        public string ReportingYr { get; set; }
        public int FormGroupID { get; set; }
        public int FormState { get; set; }
        public double ApprovalHierarchy { get; set; }
        public ConveyanceRulesAttributes frmAttributes { get; set; }
        public bool isAdmin { get; set; } = false;
        public List<Model.ConveyanceBillDescription> conveyanceBillDescriptionList { get; set; }
        public Model.ConveyanceBillDetails conveyanceBillDetails { get; set; } = new Model.ConveyanceBillDetails();
        public string ReportingName { get; set; }
        public string ReportingDesignation { get; set; }
        public string ReviewingName { get; set; }
        public string ReviewingDesignation { get; set; }
        public string AAName { get; set; }
        public string AADesignation { get; set; }
    }

    public class ConveyanceRulesAttributes
    {
        public int ConveyanceBillDetailID { get; set; }
        public int FormState { get; set; }
        public int? SenderID { get; set; }
        public int? ReciverID { get; set; }
        public ConveyanceSubmittedBy SubmittedBy { get; set; }
        public bool EmployeeSection { get; set; }
        public bool ReportingSection { get; set; }
        public bool ReviewerSection { get; set; }
        public bool ReportingButton { get; set; }
        public bool ReviewerButton { get; set; }
        public bool AcceptanceButton { get; set; }
    }

    public enum ConveyanceSubmittedBy
    {
        Employee = 1,
        ReportingOfficer = 2,
        ReviewingOfficer = 3,
        AcceptanceAuthority = 4
    }
}
