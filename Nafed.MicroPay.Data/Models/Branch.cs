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
    
    public partial class Branch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Branch()
        {
            this.StaffGrievances = new HashSet<StaffGrievance>();
            this.Holidays = new HashSet<Holiday>();
            this.tblMstHillComps = new HashSet<tblMstHillComp>();
            this.SlabWiseHeadBreakups = new HashSet<SlabWiseHeadBreakup>();
            this.EmpAttendanceDetails = new HashSet<EmpAttendanceDetail>();
            this.tblmsttransfers = new HashSet<tblmsttransfer>();
            this.tblBranchManagerDetails = new HashSet<tblBranchManagerDetail>();
            this.JobRequirementLocations = new HashSet<JobRequirementLocation>();
            this.Requirements = new HashSet<Requirement>();
            this.ExamCenterDetails = new HashSet<ExamCenterDetail>();
            this.CandidateRegistrations = new HashSet<CandidateRegistration>();
            this.tblFinalMonthlySalaries = new HashSet<tblFinalMonthlySalary>();
            this.TBLMONTHLYINPUTs = new HashSet<TBLMONTHLYINPUT>();
            this.tblLoanTrans = new HashSet<tblLoanTran>();
            this.tblarrearmanualdatas = new HashSet<tblarrearmanualdata>();
            this.TblArrearDetails = new HashSet<TblArrearDetail>();
            this.tblmstProjectedEmployeeSalaries = new HashSet<tblmstProjectedEmployeeSalary>();
            this.TblMstEmployeeSalaries = new HashSet<TblMstEmployeeSalary>();
            this.tblMstEmployees = new HashSet<tblMstEmployee>();
            this.tblMstEmployees1 = new HashSet<tblMstEmployee>();
            this.tblMstEmployees2 = new HashSet<tblMstEmployee>();
            this.tblMstEmployees3 = new HashSet<tblMstEmployee>();
            this.TblBonusAmts = new HashSet<TblBonusAmt>();
            this.TblExGratia_Cal = new HashSet<TblExGratia_Cal>();
            this.BranchSalaryHeadRules = new HashSet<BranchSalaryHeadRule>();
            this.tblMstLoanPriorities = new HashSet<tblMstLoanPriority>();
            this.PayrollApprovalRequests = new HashSet<PayrollApprovalRequest>();
            this.tblPropertyReturnHDRs = new HashSet<tblPropertyReturnHDR>();
            this.EmpPFOrgHDRs = new HashSet<EmpPFOrgHDR>();
            this.ChildrenEducationHdrs = new HashSet<ChildrenEducationHdr>();
            this.NRPFLoanHDRs = new HashSet<NRPFLoanHDR>();
            this.ConfirmationFormAHeaders = new HashSet<ConfirmationFormAHeader>();
            this.ConfirmationFormBHeaders = new HashSet<ConfirmationFormBHeader>();
            this.tblMstLTCs = new HashSet<tblMstLTC>();
            this.EmpAttendanceHdrs = new HashSet<EmpAttendanceHdr>();
            this.tblPFOpBalances = new HashSet<tblPFOpBalance>();
            this.MailFailedLog = new HashSet<MailFailedLog>();
        }
    
        public int BranchID { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public bool IsHillComp { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Pin { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<int> GradeID { get; set; }
        public string Region { get; set; }
        public string PhoneSTD { get; set; }
        public string PhoneNo { get; set; }
        public string FaxSTD { get; set; }
        public string FaxNo { get; set; }
        public string Remarks { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string EmailId { get; set; }
    
        public virtual City City { get; set; }
        public virtual User User { get; set; }
        public virtual Grade Grade { get; set; }
        public virtual User User1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffGrievance> StaffGrievances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Holiday> Holidays { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMstHillComp> tblMstHillComps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SlabWiseHeadBreakup> SlabWiseHeadBreakups { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpAttendanceDetail> EmpAttendanceDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblmsttransfer> tblmsttransfers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBranchManagerDetail> tblBranchManagerDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobRequirementLocation> JobRequirementLocations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Requirement> Requirements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamCenterDetail> ExamCenterDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CandidateRegistration> CandidateRegistrations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFinalMonthlySalary> tblFinalMonthlySalaries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBLMONTHLYINPUT> TBLMONTHLYINPUTs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblLoanTran> tblLoanTrans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblarrearmanualdata> tblarrearmanualdatas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblArrearDetail> TblArrearDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblmstProjectedEmployeeSalary> tblmstProjectedEmployeeSalaries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblMstEmployeeSalary> TblMstEmployeeSalaries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMstEmployee> tblMstEmployees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMstEmployee> tblMstEmployees1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMstEmployee> tblMstEmployees2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMstEmployee> tblMstEmployees3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblBonusAmt> TblBonusAmts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblExGratia_Cal> TblExGratia_Cal { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BranchSalaryHeadRule> BranchSalaryHeadRules { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMstLoanPriority> tblMstLoanPriorities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayrollApprovalRequest> PayrollApprovalRequests { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPropertyReturnHDR> tblPropertyReturnHDRs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpPFOrgHDR> EmpPFOrgHDRs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChildrenEducationHdr> ChildrenEducationHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NRPFLoanHDR> NRPFLoanHDRs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfirmationFormAHeader> ConfirmationFormAHeaders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfirmationFormBHeader> ConfirmationFormBHeaders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMstLTC> tblMstLTCs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpAttendanceHdr> EmpAttendanceHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPFOpBalance> tblPFOpBalances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MailFailedLog> MailFailedLog { get; set; }
    }
}
