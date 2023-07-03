using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class EmployeePFORG
    {
        public int ID { get; set; }
        public int EmpPFID { get; set; }
        public IEnumerable<dynamic> EmployeeList { set; get; } = new List<dynamic>();

        [DisplayName("Employee :")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select employee")]
        public Nullable<int> EmployeeId { get; set; }
        public string Employeecode { get; set; }
        public string Employeename { get; set; }
        public string Branchname { get; set; }

        public string AcceptanceAuthority { get; set; }
        public string ReportingToEmail { get; set; }
        public string ReviewingToEmail { get; set; }
        public string AcceptanceAuthorityEmail { get; set; }
            
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }

        public int? DepartmentID { get; set; }
        public int? DesignationID { get; set; }

        public Nullable<System.DateTime> DOB { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DOJ { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> currentDate { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> currentDateAA { get; set; }
        public string HBName { get; set; }
        public string Gender { get; set; }
        public string MaritalSts { get; set; }
        public string OfficialEmail { get; set; }

        public string MobileNo { get; set; }

        public int Employee_PF_Scheme_1952 { get; set; }
        public string Employee_PF_Scheme_1952_View { get; set; }
        public int Employee_Pension_Scheme_1995 { get; set; }
        public string Employee_Pension_Scheme_1995_View { get; set; }
        public string Universal_Acc_No { get; set; }
        public int Previous_PF_Acc_No { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime>  Dateof_Exit_Previos_Employment { get; set; }
        public string Scheme_Certificate_No { get; set; }

        public string Pension_payment_Order_No { get; set; }
        public int International_Worker { get; set; }
        public string International_Worker_View { get; set; }
        public string State_Country_Origin { get; set; }

        public string Passport_No { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Validity_Passport_from { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Validity_Passport_to { get; set; }
        public string BankAcNo { get; set; }
        public string IFSCCode { get; set; }
        public string AadhaarNo { get; set; }
        public string Permanent_AcNo { get; set; }

        public string AadhaarCardFilePath { get; set; }
        public string PanCardFilePath { get; set; }
        public string PanCardUNCFilePath { set; get; }
        public string AadhaarCardUNCFilePath { set; get; }

        public string PassportFilePath { get; set; }
        public string PassportUNCFilePath { set; get; }

        public string BankAccFilePath { get; set; }
        public string BankAccUNCFilePath { set; get; }
        public int PFNo { set; get; }
        
        public string PersonalEmailID { set; get; }
        public Nullable<int> StatusID { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        public int? loggedInEmpID { set; get; }
        public short FormStatus { get; set; }
        public EmployeeProcessApproval EmpProceeApproval { set; get; } = new EmployeeProcessApproval();

        public double? ApprovalHierarchy { get; set; }

        public FormRulesAttributes frmAttributes { get; set; }

        public bool EmployeeDeclaration { get; set; }
    }

    public enum form11FormState
    {
        [Display(Name = "Pending")]
        Pending = 0,
        [Display(Name = "Submitted")]
        SubmitedByEmployee = 1,
        [Display(Name = "Reviwed by Personal")]
        ReviewedByPersonal =2,
        [Display(Name = "Accepted by Reporting Officer")]
        AcceptedByReporting = 3,
        [Display(Name = "Rejected by Reporting Officer")]
        RejectedByReporting = 4,
        [Display(Name = "Accepted by Reviewing Officer")]
        AcceptedByReviewer = 5,
        [Display(Name = "Rejected by Reviewing Officer")]
        RejectedByReviewer =6,
        [Display(Name = "Accepted by Acceptance Authority")]
        SubmitedByAcceptanceAuth = 7,
        [Display(Name = "Rejected by Acceptance Authority")]
        RejectedByAcceptanceAuthority = 8

    }
}
