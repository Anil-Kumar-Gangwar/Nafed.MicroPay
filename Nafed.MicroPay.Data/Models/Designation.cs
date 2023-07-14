
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
    
public partial class Designation
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Designation()
    {

        this.APARSkillSets = new HashSet<APARSkillSet>();

        this.BonusMinimumMonthlyWages = new HashSet<BonusMinimumMonthlyWage>();

        this.tblFinalMonthlySalaries = new HashSet<tblFinalMonthlySalary>();

        this.Requirements = new HashSet<Requirement>();

        this.tblarrearmanualdatas = new HashSet<tblarrearmanualdata>();

        this.tblMstEmployees = new HashSet<tblMstEmployee>();

        this.tblMstEmployees1 = new HashSet<tblMstEmployee>();

        this.tblMstEmployees2 = new HashSet<tblMstEmployee>();

        this.tblMstEmployees3 = new HashSet<tblMstEmployee>();

        this.tblpromotions = new HashSet<tblpromotion>();

        this.tblPropertyReturnHDRs = new HashSet<tblPropertyReturnHDR>();

        this.tblStaffBudgets = new HashSet<tblStaffBudget>();

        this.tblStaffBudgets1 = new HashSet<tblStaffBudget>();

        this.EmpPFOrgHDRs = new HashSet<EmpPFOrgHDR>();

        this.ConveyanceBillHdrDetails = new HashSet<ConveyanceBillHdrDetail>();

        this.NRPFLoanHDRs = new HashSet<NRPFLoanHDR>();

        this.ConfirmationFormAHeaders = new HashSet<ConfirmationFormAHeader>();

        this.ConfirmationFormBHeaders = new HashSet<ConfirmationFormBHeader>();

        this.TicketWorkFlows = new HashSet<TicketWorkFlow>();

        this.tickets = new HashSet<ticket>();

        this.FormGroupAHdrs = new HashSet<FormGroupAHdr>();

        this.FormGroupBHdrs = new HashSet<FormGroupBHdr>();

        this.FormGroupCHdrs = new HashSet<FormGroupCHdr>();

        this.FormGroupDHdrs = new HashSet<FormGroupDHdr>();

        this.FormGroupEHdrs = new HashSet<FormGroupEHdr>();

        this.FormGroupFHdrs = new HashSet<FormGroupFHdr>();

        this.FormGroupGHdrs = new HashSet<FormGroupGHdr>();

        this.FormGroupHHdrs = new HashSet<FormGroupHHdr>();

        this.DesignationAppraisalForms = new HashSet<DesignationAppraisalForm>();

        this.EmpAttendanceHdrs = new HashSet<EmpAttendanceHdr>();

        this.promotioncota1 = new HashSet<promotioncota1>();

        this.FileWorkflow = new HashSet<FileWorkflow>();

        this.FileWorkflow1 = new HashSet<FileWorkflow>();

    }


    public int DesignationID { get; set; }

    public string DesignationCode { get; set; }

    public string DesignationName { get; set; }

    public string Level { get; set; }

    public bool IsOfficer { get; set; }

    public Nullable<int> Rank { get; set; }

    public Nullable<int> CadreID { get; set; }

    public Nullable<int> CategoryID { get; set; }

    public Nullable<decimal> LCT { get; set; }

    public Nullable<decimal> Promotion { get; set; }

    public Nullable<decimal> Direct { get; set; }

    public Nullable<bool> IsUpgradedDesignation { get; set; }

    public bool IsDeleted { get; set; }

    public Nullable<decimal> B1 { get; set; }

    public Nullable<decimal> E1 { get; set; }

    public Nullable<decimal> B2 { get; set; }

    public Nullable<decimal> E2 { get; set; }

    public Nullable<decimal> B3 { get; set; }

    public Nullable<decimal> E3 { get; set; }

    public Nullable<decimal> B4 { get; set; }

    public Nullable<decimal> E4 { get; set; }

    public Nullable<decimal> B5 { get; set; }

    public Nullable<decimal> E5 { get; set; }

    public Nullable<decimal> B6 { get; set; }

    public Nullable<decimal> E6 { get; set; }

    public Nullable<decimal> B7 { get; set; }

    public Nullable<decimal> E7 { get; set; }

    public Nullable<decimal> B8 { get; set; }

    public Nullable<decimal> E8 { get; set; }

    public Nullable<decimal> B9 { get; set; }

    public Nullable<decimal> E9 { get; set; }

    public Nullable<decimal> B10 { get; set; }

    public Nullable<decimal> E10 { get; set; }

    public Nullable<decimal> B11 { get; set; }

    public Nullable<decimal> E11 { get; set; }

    public Nullable<decimal> B12 { get; set; }

    public Nullable<decimal> E12 { get; set; }

    public Nullable<decimal> B13 { get; set; }

    public Nullable<decimal> E13 { get; set; }

    public Nullable<decimal> B14 { get; set; }

    public Nullable<decimal> E14 { get; set; }

    public Nullable<decimal> B15 { get; set; }

    public Nullable<decimal> E15 { get; set; }

    public Nullable<decimal> B16 { get; set; }

    public Nullable<decimal> E16 { get; set; }

    public Nullable<decimal> B17 { get; set; }

    public Nullable<decimal> E17 { get; set; }

    public Nullable<decimal> B18 { get; set; }

    public Nullable<decimal> E18 { get; set; }

    public Nullable<decimal> B19 { get; set; }

    public Nullable<decimal> E19 { get; set; }

    public Nullable<decimal> B20 { get; set; }

    public Nullable<decimal> E20 { get; set; }

    public Nullable<decimal> B21 { get; set; }

    public Nullable<decimal> E21 { get; set; }

    public Nullable<decimal> B22 { get; set; }

    public Nullable<decimal> E22 { get; set; }

    public Nullable<decimal> B23 { get; set; }

    public Nullable<decimal> E23 { get; set; }

    public Nullable<decimal> B24 { get; set; }

    public Nullable<decimal> E24 { get; set; }

    public Nullable<decimal> B25 { get; set; }

    public Nullable<decimal> E25 { get; set; }

    public Nullable<decimal> B26 { get; set; }

    public Nullable<decimal> E26 { get; set; }

    public Nullable<decimal> B27 { get; set; }

    public Nullable<decimal> E27 { get; set; }

    public Nullable<decimal> B28 { get; set; }

    public Nullable<decimal> E28 { get; set; }

    public Nullable<decimal> B29 { get; set; }

    public Nullable<decimal> E29 { get; set; }

    public Nullable<decimal> B30 { get; set; }

    public Nullable<decimal> E30 { get; set; }

    public Nullable<decimal> B31 { get; set; }

    public Nullable<decimal> E31 { get; set; }

    public Nullable<decimal> B32 { get; set; }

    public Nullable<decimal> E32 { get; set; }

    public Nullable<decimal> B33 { get; set; }

    public Nullable<decimal> E33 { get; set; }

    public Nullable<decimal> B34 { get; set; }

    public Nullable<decimal> E34 { get; set; }

    public Nullable<decimal> B35 { get; set; }

    public Nullable<decimal> E35 { get; set; }

    public Nullable<decimal> B36 { get; set; }

    public Nullable<decimal> E36 { get; set; }

    public Nullable<decimal> B37 { get; set; }

    public Nullable<decimal> E37 { get; set; }

    public Nullable<decimal> B38 { get; set; }

    public Nullable<decimal> E38 { get; set; }

    public Nullable<decimal> B39 { get; set; }

    public Nullable<decimal> E39 { get; set; }

    public Nullable<decimal> B40 { get; set; }

    public Nullable<decimal> E40 { get; set; }

    public Nullable<int> CreatedBy { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }

    public Nullable<byte> LCTInNo { get; set; }

    public Nullable<byte> PromotionInNo { get; set; }

    public Nullable<byte> DirectInNo { get; set; }

    public Nullable<decimal> FamilyAssured { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<APARSkillSet> APARSkillSets { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<BonusMinimumMonthlyWage> BonusMinimumMonthlyWages { get; set; }

    public virtual Cadre Cadre { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblFinalMonthlySalary> tblFinalMonthlySalaries { get; set; }

    public virtual User User { get; set; }

    public virtual EmployeeCategory EmployeeCategory { get; set; }

    public virtual User User1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Requirement> Requirements { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblarrearmanualdata> tblarrearmanualdatas { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblMstEmployee> tblMstEmployees { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblMstEmployee> tblMstEmployees1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblMstEmployee> tblMstEmployees2 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblMstEmployee> tblMstEmployees3 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblpromotion> tblpromotions { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblPropertyReturnHDR> tblPropertyReturnHDRs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblStaffBudget> tblStaffBudgets { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblStaffBudget> tblStaffBudgets1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<EmpPFOrgHDR> EmpPFOrgHDRs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ConveyanceBillHdrDetail> ConveyanceBillHdrDetails { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<NRPFLoanHDR> NRPFLoanHDRs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ConfirmationFormAHeader> ConfirmationFormAHeaders { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ConfirmationFormBHeader> ConfirmationFormBHeaders { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<TicketWorkFlow> TicketWorkFlows { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ticket> tickets { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<FormGroupAHdr> FormGroupAHdrs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<FormGroupBHdr> FormGroupBHdrs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<FormGroupCHdr> FormGroupCHdrs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<FormGroupDHdr> FormGroupDHdrs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<FormGroupEHdr> FormGroupEHdrs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<FormGroupFHdr> FormGroupFHdrs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<FormGroupGHdr> FormGroupGHdrs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<FormGroupHHdr> FormGroupHHdrs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DesignationAppraisalForm> DesignationAppraisalForms { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<EmpAttendanceHdr> EmpAttendanceHdrs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<promotioncota1> promotioncota1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<FileWorkflow> FileWorkflow { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<FileWorkflow> FileWorkflow1 { get; set; }

}

}
