using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model.Employees
{
    public class NonRegularEmployee
    {
        public int EmployeeID { get; set; }
        [DisplayName("Employee Code :")]
        [Required(ErrorMessage = "Please Enter Employee Code")]

        public string EmployeeCode { get; set; }

        [DisplayName("Title :")]
        [Required(ErrorMessage = "Please Enter Title")]
        public int TitleID { get; set; }
        [DisplayName("Name :")]
        [Required(ErrorMessage = "Please Enter Employee Name")]
        public string Name { get; set; }
        [DisplayName("Husband/ Father Name :")]
        public string HBName { get; set; }

        [DisplayName("Employee Type :")]
        [Required(ErrorMessage = "Please Select Employee Type")]
        public int EmployeeTypeID { get; set; }

        public string EmpProfilePhotoUNCPath { set; get; }

        [DisplayName("Branch :")]
        //  [Required(ErrorMessage = "Please Select Employee Branch")]
        public int BranchID { get; set; }

        public string BranchCode { set; get; }

        [DisplayName("Department :")]
        //[Required(ErrorMessage = "Please Select Employee Department")]
        public Nullable<int> DepartmentID { get; set; }

        [DisplayName("Designation :")]
        //  [Required(ErrorMessage = "Please Select Employee Designation")]
        public int DesignationID { get; set; }

        [DisplayName("Basic Salary  :")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999999999.99)]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]

        public Nullable<decimal> E_Basic { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Of Birth :")]
        [Required(ErrorMessage = "Please Enter Date Of Birth")]
        public Nullable<System.DateTime> DOB { get; set; }

        [DisplayName("Place Of Joining :")]
        public string PlaceOfJoin { get; set; }
        [DisplayName("Gender :")]
        [Required(ErrorMessage = "Gender is required")]
        public int GenderID { get; set; }

        [DisplayName("Category :")]
        public Nullable<int> CategoryID { get; set; }

        [DisplayName("Religion :")]
        public Nullable<int> ReligionID { get; set; }
        [DisplayName("Mother Tongue :")]
        public Nullable<int> MTongueID { get; set; }
        [DisplayName("Blood Group:")]
        public Nullable<int> BGroupID { get; set; }
        [DisplayName("Marital Status:")]
        public Nullable<int> MaritalStsID { get; set; }

        [DisplayName("Emp Category :")]
        public Nullable<int> EmpCatID { get; set; }

        [DisplayName("Cadre Code :")]
        public Nullable<int> CadreID { get; set; }
        public string CadreRank { get; set; }
        public Nullable<int> DivisionID { get; set; }
        [DisplayName("Section Code :")]
        public Nullable<int> SectionID { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date of Joining on New Desig/Branch :")]
        [Required(ErrorMessage = "Please Enter Date Of Joining")]
        public Nullable<System.DateTime> Pr_Loc_DOJ { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Of Joining :")]
        [Required(ErrorMessage = "Please Enter Date Of Joining on New Desig/Branch")]
        public Nullable<System.DateTime> DOJ { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayName("Date of Joining on New Desig/Branch:")]
        //public Nullable<System.DateTime> S_DOJ { get; set; }

        [DisplayName("Service Join Date:")]
        public Nullable<System.DateTime> S_DOJ { get; set; }

        public string Pr_desg { get; set; }

        [DisplayName("Seniority Code :")]
        public Nullable<int> Sen_Code { get; set; }
        public string P_Scale { get; set; }

        [DisplayName("ID Mark :")]
        public string ID_Mark { get; set; }

        [DisplayName("Present Address :")]
        public string PAdd { get; set; }

        [DisplayName("Present Street :")]
        public string PStreet { get; set; }

        [DisplayName("Present City :")]
        public string PCity { get; set; }

        [DataType(DataType.PostalCode, ErrorMessage = "Invalid Pin.")]
        [DisplayName("Present Pin :")]
        public string PPin { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid Telephone No.")]
        [DisplayName("Present Telephone No. :")]
        public string TelPh { get; set; }

        [DisplayName("Permanent Address :")]
        public string PmtAdd { get; set; }
        [DisplayName("Permanent Street :")]
        public string PmtStreet { get; set; }
        [DisplayName("Permanent City :")]
        public string PmtCity { get; set; }

        [DisplayName("Permanent Pin :")]
        [DataType(DataType.PostalCode, ErrorMessage = "Invalid Pin.")]
        public string PmtPin { get; set; }
        [DisplayName("File No. :")]
        public string FileNo { get; set; }
        [DisplayName("PF No. :")]
        public Nullable<int> PFNO { get; set; }
        [DisplayName("Folio No. :")]
        public Nullable<int> Folio_No { get; set; }
        [DisplayName("ACR No. :")]
        public Nullable<int> ACR_No { get; set; }

        [DisplayName("Serial No.")]
        public Nullable<int> SL_No { get; set; }

        [DisplayName("PAN No.")]
        public string PANNo { get; set; }

        [DisplayName("Aadhaar No : ")]
        public string AadhaarNo { get; set; } = string.Empty;

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Confirmation Date :")]
        public Nullable<System.DateTime> ConfirmationDate { get; set; }
        public Nullable<System.DateTime> DOSupAnnuating1 { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Of Superannuation :")]
        public Nullable<System.DateTime> DOSupAnnuating { get; set; }


        [DisplayName("Passport No. :")]
        public string PassPortNo { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Passport Issuing Date. :")]
        public Nullable<System.DateTime> PPIDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Passport Expiry Date. :")]
        public Nullable<System.DateTime> PPEDate { get; set; }
        [DisplayName("Nominee. :")]
        public string GISNominee { get; set; }

        [DisplayName("Relation. :")]
        public Nullable<int> NomineeRelationID { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Next Increment Date. :")]
        public Nullable<System.DateTime> IncrementDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateOfRetirement { get; set; }

        [DisplayName("Gratuity Assurance No. :")]
        public string GraAssuranceNo { get; set; }

        [DisplayName("Academic Qualification :")]
        public Nullable<int> QAcademicID { get; set; }

        [DisplayName("Professional Qualification :")]
        public Nullable<int> QProfessionalID { get; set; }

        [DisplayName("Special Skills :")]
        public string SpecialSkills { get; set; }

        public string Specialincr { get; set; }

        public Nullable<System.DateTime> DOSpecialIncr { get; set; }

        [DisplayName("Promotion Date :")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> PromotionDate { get; set; }

        [DisplayName("Order Date of Appointment :")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> orderofpromotion { get; set; }


        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Period of Appoinment. :")]
        public Nullable<System.DateTime> PeriodAppointment { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Extension of Appoinment. :")]
        public Nullable<System.DateTime> ExtnAppointment { get; set; }

        [DisplayName("Performance in LCT :")]
        public string PerfLCT { get; set; }

        [DisplayName("Increment Month:")]
        public int? IncrementMonth { get; set; }

        [DisplayName("Stop Increment:")]
        public bool ValidateIncrement { get; set; }

        [DisplayName("Reason For Stop Increment:")]
        public string Reason { get; set; }

        [DisplayName("Departmental Enquiry :")]
        public bool Dept_Enq { get; set; } = false;

        [DisplayName("Certificate Given :")]
        public bool Cer_Given { get; set; }

        [DisplayName("Inventory Issued :")]
        public bool Inv_Issued { get; set; }

        [DisplayName("Books Library :")]
        public bool Books_LIB { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date of Leaving Org :")]
        public Nullable<System.DateTime> DOLeaveOrg { get; set; }

        [DisplayName("Reason for Leaving Organization :")]
        public string ReasonOfLeaving { get; set; }
        public string CadOfEmp { get; set; }
        public string Conv { get; set; }
        public string HBA { get; set; }
        public string Zone { get; set; }
        public string LastUpdateByUser { get; set; }
        public Nullable<System.DateTime> LastUpdateOn { get; set; }

        public string PayScale { get; set; }
        [DisplayName("By LCT :")]
        public Nullable<bool> ByLCT { get; set; } = false;

        [DisplayName("Cadre Officer :")]
        public bool IsCadreOfficer { get; set; }

        [DisplayName("First Designation:")]
        //   [Required(ErrorMessage = "Please Select First Designation")]

        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select First Designation")]
        public int FirstDesg { get; set; }

        [DisplayName("First Branch :")]
        // [Required(ErrorMessage = "Please Select First Branch")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select First Branch")]
        public int FirstBranch { get; set; }
        public Nullable<int> PENSIONSCHNO { get; set; }
        public Nullable<System.DateTime> DATEENTITLEMENTPS { get; set; }
        public Nullable<bool> IsSalgenrated { get; set; }
        public Nullable<bool> PENSIONDEDUCT { get; set; }

        [DisplayName("UAN No")]
        public string PensionUAN { get; set; }

        [DisplayName("EPFO No")]
        public string EPFoNo { get; set; }

        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string EmployeeTypeName { set; get; }

        public string BranchName { set; get; }
        public string DepartmentName { set; get; }

        public string DesignationName { set; get; }

        [DisplayName("Cota")]
        public Cota cota { set; get; }

        [DisplayName("Division")]
        public Divison divison { set; get; }
        public WayOfLeavingOrg WayOfleavingOrg { set; get; }

        public Nullable<int> OtaCode { get; set; }
        [DisplayName("Is VRS ")]
        public bool IsVRS { get; set; }
        [DisplayName("By Other Mean ")]
        public bool IsForceFully { get; set; }

        [DisplayName("Is Zonal Cord")]
        public bool IsRM { get; set; }

        [DisplayName("Is SuperAnnuated")]
        public bool IsSuperAnnuated { set; get; }

        [DisplayName("Is Dismissed ")]
        public bool IsDismissal { get; set; }

        [DisplayName("Is Terminated ")]
        public bool IsTermination { get; set; }

        [DisplayName("Is Expire ")]
        public bool IsExpire { get; set; }

        [DisplayName("Is Resignation ")]
        public bool IsResigned { get; set; }

        [DisplayName("Is Branch Manager")]
        public bool IsBM { get; set; }

        [DisplayName("Not Applicable ")]
        public bool NotApplicable { set; get; }

        public WayOfLeavingOrg wayOfLeaving { get; set; }

        [DisplayName("Offical Email ")]
        //  [Required(ErrorMessage = "Please Enter Offical Email")]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email.")]
        public string OfficialEmail { get; set; }

        [DisplayName("Personal Email : ")]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email.")]
        public string PersonalEmail { get; set; }

        [DisplayName("Mobile No : ")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10, ErrorMessage = "{0} not be exceed 10 char")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid mobile number")]
        //  [Required(ErrorMessage = "Please Enter Mobile No")]
        public string MobileNo { get; set; }
        public Nullable<System.DateTime> VisaValidUpTo { get; set; }
        public Nullable<int> WorkfromHome { get; set; }
        public Nullable<int> BiometricID { get; set; }
        public string PanCardFilePath { get; set; }
        public string PanCardUNCFilePath { set; get; }
        public string AadhaarCardFilePath { get; set; }
        public string AadhaarCardUNCFilePath { set; get; }
        // added by ibrahim on 23-08-2019
        public string TitleName { get; set; }
        public string CategoryName { get; set; }
        public string ReligionName { get; set; }
        public string Gender { get; set; }
        public string MotherTongueName { get; set; }
        public string BloodGroupName { get; set; }
        public string MaritalStatusName { get; set; }
        public string EmplCatName { get; set; }
        public string CadreName { get; set; }
        public string DivisionName { get; set; }
        public string SectionName { get; set; }
        public string RelationName { get; set; }
        public string QAcademic { get; set; } // from AcadmicProfessionalDetails table 
        public string QProfessional { get; set; }
        public string ReportingTo { get; set; }

        [DisplayName("Bank ")]
        public string BankCode { get; set; }

        [DisplayName("Bank Account No")]
        public string BankAcNo { get; set; }

        [DisplayName("Reporting To :")]
        public int? ReportingToID { get; set; }

        [DisplayName("Reviewer To :")]
        public int? ReviewerTo { set; get; }

        [DisplayName("Dossier No :")]
        public Nullable<int> DossierNo { get; set; }
        [DisplayName("Acceptance Authority :")]
        public Nullable<int> AcceptanceAuthority { get; set; }

        public int? AppraisalFormID { set; get; }

        public string AppraisalFormName { set; get; }

        public string ReportingYr { set; get; }
        public Nullable<int> UserTypeID { get; set; }

        [DisplayName("Mother Name :")]
        public string MotherName { get; set; }

        [DisplayName("IFSC Code :")]
        public string IFSCCode { get; set; }

        [DisplayName("Present State:")]
        public Nullable<int> PState { get; set; }

        [DisplayName("Permenant State:")]
        public Nullable<int> PmtState { get; set; }

        public string PresentState { get; set; }
        public string PermenantState { get; set; }

        [DisplayName("Mode of Payment")]
        public ModeOfPayment modOfPayment { get; set; } = ModeOfPayment.Bank;
        [DisplayName("Pension Number")]
        public string PensionNumber { get; set; }
        [DisplayName("EPFO Member ID")]
        public string EPFOMemberID { get; set; }

        public string EmpPayScale { get; set; }

        [DisplayName("Employee Joining After Noon (Joining after 12 P.M.)")]
        public bool IsJoinAfterNoon { get; set; }
    }


    public enum ModeOfPayment
    {
        [Display(Name = "BANK")]
        Bank = 1,
        [Display(Name = "CASH")]
        Cash = 2
    }

    public enum Cota
    {
        Promotion = 1,
        LTC = 2,
        Direct = 3,
        VRS = 4
    }

    public enum WayOfLeavingOrg
    {
        IsVRS = 1,
        IsDismissal = 2,
        IsExpire = 3,
        IsResigned = 4,
        IsForceFully = 5,
        IsTermination = 6,
        IsSuperAnnuated = 7,
        NotApplicable = 8
    }

    public enum Divison
    {
        IsRM = 1,
        IsBM = 2
    }

}
