//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nafed.MicroPay.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblMstEmployee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblMstEmployee()
        {
            this.APARSkillSetFormHdrs = new HashSet<APARSkillSetFormHdr>();
            this.EmpAttendanceDetails = new HashSet<EmpAttendanceDetail>();
            this.EmployeeDependents = new HashSet<EmployeeDependent>();
            this.EmployeeLeaves = new HashSet<EmployeeLeave>();
            this.EmployeeProcessApprovals = new HashSet<EmployeeProcessApproval>();
            this.EmployeeProcessApprovals1 = new HashSet<EmployeeProcessApproval>();
            this.EmployeeProcessApprovals2 = new HashSet<EmployeeProcessApproval>();
            this.EmployeeProcessApprovals3 = new HashSet<EmployeeProcessApproval>();
            this.TblArrearDetails = new HashSet<TblArrearDetail>();
            this.tblarrearmanualdatas = new HashSet<tblarrearmanualdata>();
            this.TblArrearMonthlyInputs = new HashSet<TblArrearMonthlyInput>();
            this.tblBranchManagerDetails = new HashSet<tblBranchManagerDetail>();
            this.tblEmployeeQualificationdetails = new HashSet<tblEmployeeQualificationdetail>();
            this.tblFinalMonthlySalaries = new HashSet<tblFinalMonthlySalary>();
            this.TBLGRATIADUMMies = new HashSet<TBLGRATIADUMMY>();
            this.tblLeaveBals = new HashSet<tblLeaveBal>();
            this.tblLoanTrans = new HashSet<tblLoanTran>();
            this.tblLoanTrans1 = new HashSet<tblLoanTran>();
            this.TBLMONTHLYINPUTs = new HashSet<TBLMONTHLYINPUT>();
            this.TBLMONTHLYINPUTs1 = new HashSet<TBLMONTHLYINPUT>();
            this.TBLMONTHLYINPUTs2 = new HashSet<TBLMONTHLYINPUT>();
            this.tblmsttransfers = new HashSet<tblmsttransfer>();
            this.TblMstEmployeeSalaries = new HashSet<TblMstEmployeeSalary>();
            this.tblmstProjectedEmployeeSalaries = new HashSet<tblmstProjectedEmployeeSalary>();
            this.tblpromotions = new HashSet<tblpromotion>();
            this.tblPropertyReturns = new HashSet<tblPropertyReturn>();
            this.TrainingFeedbackDetails = new HashSet<TrainingFeedbackDetail>();
            this.TrainingParticipants = new HashSet<TrainingParticipant>();
            this.Users = new HashSet<User>();
            this.LoanApplications = new HashSet<LoanApplication>();
            this.TblBonusAmts = new HashSet<TblBonusAmt>();
            this.TblExGratia_Cal = new HashSet<TblExGratia_Cal>();
            this.OTASlips = new HashSet<OTASlip>();
            this.EmployeeProvidentFundOrganisations = new HashSet<EmployeeProvidentFundOrganisation>();
            this.tblMstLoanPriorities = new HashSet<tblMstLoanPriority>();
            this.tblPropertyReturnHDRs = new HashSet<tblPropertyReturnHDR>();
            this.EPFNominations = new HashSet<EPFNomination>();
            this.ChildrenEducationDocuments = new HashSet<ChildrenEducationDocument>();
            this.ConveyanceBillHdrs = new HashSet<ConveyanceBillHdr>();
            this.NonRefundablePFLoans = new HashSet<NonRefundablePFLoan>();
            this.EmpPFOrgHDRs = new HashSet<EmpPFOrgHDR>();
            this.EmpDeductionUnderChapterVI_A = new HashSet<EmpDeductionUnderChapterVI_A>();
            this.EmployeeForm12BB = new HashSet<EmployeeForm12BB>();
            this.ChildrenEducationHdrs = new HashSet<ChildrenEducationHdr>();
            this.AssetmanagementDetails = new HashSet<AssetmanagementDetail>();
            this.ConveyanceBillHdrDetails = new HashSet<ConveyanceBillHdrDetail>();
            this.NRPFLoanHDRs = new HashSet<NRPFLoanHDR>();
            this.ConfirmationFormAHeaders = new HashSet<ConfirmationFormAHeader>();
            this.ConfirmationFormBHeaders = new HashSet<ConfirmationFormBHeader>();
            this.EmployeeSuspensionPeriods = new HashSet<EmployeeSuspensionPeriod>();
            this.tblMstLTCs = new HashSet<tblMstLTC>();
            this.tickets = new HashSet<ticket>();
            this.tickets1 = new HashSet<ticket>();
            this.ConfirmationStatus = new HashSet<ConfirmationStatu>();
            this.PayrollApprovalRequests = new HashSet<PayrollApprovalRequest>();
            this.PayrollApprovalSettings = new HashSet<PayrollApprovalSetting>();
            this.PayrollApprovalSettings1 = new HashSet<PayrollApprovalSetting>();
            this.PayrollApprovalSettings2 = new HashSet<PayrollApprovalSetting>();
            this.ConfirmationFormHdrs = new HashSet<ConfirmationFormHdr>();
            this.FormGroupBHdrs = new HashSet<FormGroupBHdr>();
            this.FormGroupCHdrs = new HashSet<FormGroupCHdr>();
            this.FormGroupEHdrs = new HashSet<FormGroupEHdr>();
            this.FormGroupFHdrs = new HashSet<FormGroupFHdr>();
            this.FormGroupGHdrs = new HashSet<FormGroupGHdr>();
            this.AppraisalFormHdrs = new HashSet<AppraisalFormHdr>();
            this.SubordinatesTrainings = new HashSet<SubordinatesTraining>();
            this.EmployeeAchievements = new HashSet<EmployeeAchievement>();
            this.EmployeeCertifications = new HashSet<EmployeeCertification>();
            this.EmpAttendanceHdrs = new HashSet<EmpAttendanceHdr>();
            this.Insurances = new HashSet<Insurance>();
            this.SeparationClearances = new HashSet<SeparationClearance>();
            this.Seprations = new HashSet<Sepration>();
            this.Resignations = new HashSet<Resignation>();
            this.FormGroupAHdr = new HashSet<FormGroupAHdr>();
            this.FileWorkflow = new HashSet<FileWorkflow>();
            this.FileWorkflow1 = new HashSet<FileWorkflow>();
            this.NREmployeesContractExtention = new HashSet<NREmployeesContractExtention>();
            this.MailFailedLog = new HashSet<MailFailedLog>();
            this.LeaveEncashmentDetails = new HashSet<LeaveEncashmentDetails>();
        }
    
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public int TitleID { get; set; }
        public string Name { get; set; }
        public string HBName { get; set; }
        public int EmployeeTypeID { get; set; }
        public int BranchID { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public int DesignationID { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public System.DateTime DOJ { get; set; }
        public string PlaceOfJoin { get; set; }
        public int GenderID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> ReligionID { get; set; }
        public Nullable<int> MTongueID { get; set; }
        public Nullable<int> BGroupID { get; set; }
        public Nullable<int> MaritalStsID { get; set; }
        public Nullable<int> EmpCatID { get; set; }
        public Nullable<int> CadreID { get; set; }
        public string CadreRank { get; set; }
        public Nullable<int> DivisionID { get; set; }
        public Nullable<int> SectionID { get; set; }
        public Nullable<System.DateTime> Pr_Loc_DOJ { get; set; }
        public Nullable<System.DateTime> S_DOJ { get; set; }
        public string Pr_desg { get; set; }
        public Nullable<int> Sen_Code { get; set; }
        public string P_Scale { get; set; }
        public string ID_Mark { get; set; }
        public string PAdd { get; set; }
        public string PStreet { get; set; }
        public string PCity { get; set; }
        public string PPin { get; set; }
        public string TelPh { get; set; }
        public string PmtAdd { get; set; }
        public string PmtStreet { get; set; }
        public string PmtCity { get; set; }
        public string PmtPin { get; set; }
        public string FileNo { get; set; }
        public Nullable<int> PFNO { get; set; }
        public Nullable<int> Folio_No { get; set; }
        public Nullable<int> ACR_No { get; set; }
        public Nullable<int> SL_No { get; set; }
        public string PANNo { get; set; }
        public Nullable<System.DateTime> ConfirmationDate { get; set; }
        public Nullable<System.DateTime> DOSupAnnuating { get; set; }
        public string PassPortNo { get; set; }
        public Nullable<System.DateTime> PPIDate { get; set; }
        public Nullable<System.DateTime> PPEDate { get; set; }
        public string GISNominee { get; set; }
        public Nullable<int> NomineeRelationID { get; set; }
        public Nullable<System.DateTime> IncrementDate { get; set; }
        public Nullable<System.DateTime> DateOfRetirement { get; set; }
        public string GraAssuranceNo { get; set; }
        public Nullable<int> QAcademicID { get; set; }
        public Nullable<int> QProfessionalID { get; set; }
        public string SpecialSkills { get; set; }
        public string Specialincr { get; set; }
        public Nullable<System.DateTime> DOSpecialIncr { get; set; }
        public Nullable<System.DateTime> PromotionDate { get; set; }
        public Nullable<System.DateTime> orderofpromotion { get; set; }
        public Nullable<System.DateTime> PeriodAppointment { get; set; }
        public Nullable<System.DateTime> ExtnAppointment { get; set; }
        public string PerfLCT { get; set; }
        public Nullable<int> IncrementMonth { get; set; }
        public Nullable<bool> ValidateIncrement { get; set; }
        public string Reason { get; set; }
        public Nullable<bool> Dept_Enq { get; set; }
        public Nullable<bool> Cer_Given { get; set; }
        public Nullable<bool> Inv_Issued { get; set; }
        public Nullable<bool> Books_LIB { get; set; }
        public Nullable<System.DateTime> DOLeaveOrg { get; set; }
        public string ReasonOfLeaving { get; set; }
        public string CadOfEmp { get; set; }
        public string Conv { get; set; }
        public string HBA { get; set; }
        public string Zone { get; set; }
        public Nullable<bool> IsForceFully { get; set; }
        public Nullable<bool> IsDismissal { get; set; }
        public Nullable<bool> IsTermination { get; set; }
        public Nullable<bool> IsExpire { get; set; }
        public Nullable<bool> IsResigned { get; set; }
        public Nullable<bool> IsRM { get; set; }
        public Nullable<bool> IsVRS { get; set; }
        public Nullable<bool> IsBM { get; set; }
        public string PayScale { get; set; }
        public Nullable<bool> ByLCT { get; set; }
        public Nullable<int> FirstDesgID { get; set; }
        public Nullable<int> FirstBranchID { get; set; }
        public Nullable<int> PENSIONSCHNO { get; set; }
        public Nullable<System.DateTime> DATEENTITLEMENTPS { get; set; }
        public bool IsSalgenrated { get; set; }
        public Nullable<bool> PENSIONDEDUCT { get; set; }
        public string PensionUAN { get; set; }
        public bool IsCadreOfficer { get; set; }
        public Nullable<int> OtaCode { get; set; }
        public Nullable<System.DateTime> DOPENSIONJOIN { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string OfficialEmail { get; set; }
        public string PersonalEmail { get; set; }
        public Nullable<System.DateTime> VisaValidUpTo { get; set; }
        public Nullable<int> WorkfromHome { get; set; }
        public Nullable<int> BiometricID { get; set; }
        public string MobileNo { get; set; }
        public string AadhaarNo { get; set; }
        public string PanCardFilePath { get; set; }
        public string AadhaarCardFilePath { get; set; }
        public Nullable<bool> IsCompulsaryRetirement { get; set; }
        public Nullable<int> DossierNo { get; set; }
        public string MotherName { get; set; }
        public string IFSCCode { get; set; }
        public Nullable<int> ReportingTo { get; set; }
        public Nullable<int> ReviewerTo { get; set; }
        public Nullable<int> AcceptanceAuthority { get; set; }
        public Nullable<System.DateTime> DOSupAnnuating1 { get; set; }
        public Nullable<int> PState { get; set; }
        public Nullable<int> PmtState { get; set; }
        public Nullable<System.DateTime> StopIncrementEffectiveDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string PensionNumber { get; set; }
        public string EPFOMemberID { get; set; }
        public bool IsJoinAfterNoon { get; set; }
        public Nullable<int> BranchID_Pay { get; set; }
    
        public virtual AcadmicProfessionalDetail AcadmicProfessionalDetail { get; set; }
        public virtual AcadmicProfessionalDetail AcadmicProfessionalDetail1 { get; set; }
        public virtual AcadmicProfessionalDetail AcadmicProfessionalDetail2 { get; set; }
        public virtual AcadmicProfessionalDetail AcadmicProfessionalDetail3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APARSkillSetFormHdr> APARSkillSetFormHdrs { get; set; }
        public virtual BloodGroup BloodGroup { get; set; }
        public virtual BloodGroup BloodGroup1 { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Branch Branch1 { get; set; }
        public virtual Branch Branch2 { get; set; }
        public virtual Branch Branch3 { get; set; }
        public virtual Cadre Cadre { get; set; }
        public virtual Cadre Cadre1 { get; set; }
        public virtual Category Category { get; set; }
        public virtual Category Category1 { get; set; }
        public virtual Department Department { get; set; }
        public virtual Department Department1 { get; set; }
        public virtual Division Division { get; set; }
        public virtual Division Division1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpAttendanceDetail> EmpAttendanceDetails { get; set; }
        public virtual EmployeeCategory EmployeeCategory { get; set; }
        public virtual EmployeeCategory EmployeeCategory1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeDependent> EmployeeDependents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeLeave> EmployeeLeaves { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeProcessApproval> EmployeeProcessApprovals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeProcessApproval> EmployeeProcessApprovals1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeProcessApproval> EmployeeProcessApprovals2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeProcessApproval> EmployeeProcessApprovals3 { get; set; }
        public virtual EmployeeType EmployeeType { get; set; }
        public virtual EmployeeType EmployeeType1 { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual Gender Gender1 { get; set; }
        public virtual MaritalStatu MaritalStatu { get; set; }
        public virtual MaritalStatu MaritalStatu1 { get; set; }
        public virtual MotherTongue MotherTongue { get; set; }
        public virtual MotherTongue MotherTongue1 { get; set; }
        public virtual Relation Relation { get; set; }
        public virtual Relation Relation1 { get; set; }
        public virtual Religion Religion { get; set; }
        public virtual Religion Religion1 { get; set; }
        public virtual Section Section { get; set; }
        public virtual Section Section1 { get; set; }
        public virtual State State { get; set; }
        public virtual State State1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblArrearDetail> TblArrearDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblarrearmanualdata> tblarrearmanualdatas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblArrearMonthlyInput> TblArrearMonthlyInputs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBranchManagerDetail> tblBranchManagerDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblEmployeeQualificationdetail> tblEmployeeQualificationdetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFinalMonthlySalary> tblFinalMonthlySalaries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBLGRATIADUMMY> TBLGRATIADUMMies { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblLeaveBal> tblLeaveBals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblLoanTran> tblLoanTrans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblLoanTran> tblLoanTrans1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBLMONTHLYINPUT> TBLMONTHLYINPUTs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBLMONTHLYINPUT> TBLMONTHLYINPUTs1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBLMONTHLYINPUT> TBLMONTHLYINPUTs2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblmsttransfer> tblmsttransfers { get; set; }
        public virtual User User { get; set; }
        public virtual Title Title { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        public virtual Title Title1 { get; set; }
        public virtual User User3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMstEmployeeSalary> TblMstEmployeeSalaries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblmstProjectedEmployeeSalary> tblmstProjectedEmployeeSalaries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblpromotion> tblpromotions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPropertyReturn> tblPropertyReturns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingFeedbackDetail> TrainingFeedbackDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingParticipant> TrainingParticipants { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LoanApplication> LoanApplications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblBonusAmt> TblBonusAmts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblExGratia_Cal> TblExGratia_Cal { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OTASlip> OTASlips { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeProvidentFundOrganisation> EmployeeProvidentFundOrganisations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMstLoanPriority> tblMstLoanPriorities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPropertyReturnHDR> tblPropertyReturnHDRs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPFNomination> EPFNominations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChildrenEducationDocument> ChildrenEducationDocuments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConveyanceBillHdr> ConveyanceBillHdrs { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual Designation Designation1 { get; set; }
        public virtual Designation Designation2 { get; set; }
        public virtual Designation Designation3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NonRefundablePFLoan> NonRefundablePFLoans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpPFOrgHDR> EmpPFOrgHDRs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpDeductionUnderChapterVI_A> EmpDeductionUnderChapterVI_A { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeForm12BB> EmployeeForm12BB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChildrenEducationHdr> ChildrenEducationHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssetmanagementDetail> AssetmanagementDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConveyanceBillHdrDetail> ConveyanceBillHdrDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NRPFLoanHDR> NRPFLoanHDRs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfirmationFormAHeader> ConfirmationFormAHeaders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfirmationFormBHeader> ConfirmationFormBHeaders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeSuspensionPeriod> EmployeeSuspensionPeriods { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMstLTC> tblMstLTCs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ticket> tickets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ticket> tickets1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfirmationStatu> ConfirmationStatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayrollApprovalRequest> PayrollApprovalRequests { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayrollApprovalSetting> PayrollApprovalSettings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayrollApprovalSetting> PayrollApprovalSettings1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayrollApprovalSetting> PayrollApprovalSettings2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfirmationFormHdr> ConfirmationFormHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupBHdr> FormGroupBHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupCHdr> FormGroupCHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupEHdr> FormGroupEHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupFHdr> FormGroupFHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupGHdr> FormGroupGHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AppraisalFormHdr> AppraisalFormHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubordinatesTraining> SubordinatesTrainings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeAchievement> EmployeeAchievements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeCertification> EmployeeCertifications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpAttendanceHdr> EmpAttendanceHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Insurance> Insurances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SeparationClearance> SeparationClearances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sepration> Seprations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Resignation> Resignations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormGroupAHdr> FormGroupAHdr { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FileWorkflow> FileWorkflow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FileWorkflow> FileWorkflow1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NREmployeesContractExtention> NREmployeesContractExtention { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MailFailedLog> MailFailedLog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LeaveEncashmentDetails> LeaveEncashmentDetails { get; set; }
    }
}
