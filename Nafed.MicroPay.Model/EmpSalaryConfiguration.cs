using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Nafed.MicroPay.Model
{
    public class EmpSalaryConfiguration
    {
        public Nullable<decimal> D_VPF { get; set; }

        public string EmployeeName { get; set; }
        public int EmpBranchID { get; set; }
        public int EmployeeID { set; get; }
       

        public string BranchCode { set; get; }
        public string EmpCode { set; get; }

        [DisplayName("Basic")]
        public Nullable<decimal> E_Basic { get; set; }

        [DisplayName("Actual Salary")]
        public Nullable<decimal> ActualSalary { get; set; }

        [DisplayName("CCA ")]
        public bool CCA { get; set; }

        [DisplayName("Washing Alw ")]
        public bool WASHING { get; set; }

        [DisplayName("HRA")]
        public Nullable<bool> HRA { get; set; }

        [DisplayName("Flat Deduction")]
        public bool IsFlatDeduction { get; set; }

        [DisplayName("None (HRA Flat Deduction)")]
        public bool None { get; set; }

        [DisplayName("Union Fee")]
        public bool UnionFee { get; set; }

        [DisplayName("Sport Club Fee ")]
        public bool SportClub { get; set; }

        [DisplayName("Professional Tax ")]
        public bool ProfTax { get; set; }

        [DisplayName("OTA Code ")]
        public bool IsOTACode { get; set; }

        [DisplayName("Pension Deduct ")]
        public bool IsPensionDeducted { get; set; }


        [DisplayName("VPFRate(in %) ")]
        public bool IsRateVPF { get; set; }

        [DisplayName("Voluntary PF  ")]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999999999.99)]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> VPFValueRA { get; set; }

        public Deduction deduction { get; set; }


        [DisplayName("Suspend Employee")]
        public bool IsSuspended { get; set; }

        public SalPercetageRatioForSuspendedEmployee salPerForSuspendedEmp { get; set; }

        [DisplayName("50%")]
        public bool Fifty { get; set; }

        [DisplayName("75%")]
        public bool SeventyFive { get; set; }

        [DisplayName("OTA Code")]
        public string OTACode { set; get; }

        [DisplayName("From")]
        public Nullable<DateTime> SuspensionPeriodFrom { set; get; }

        [DisplayName("To")]
        public Nullable<DateTime> SuspensionPeriodTo { set; get; }

        public Nullable<decimal> BasicSalPercentageForSuspendedEmp { get; set; }

        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        public ICollection<EmployeeSuspensionPeriod> SuspensionPeriods { set; get; }

        public Nullable<int> BranchID_Pay { get; set; }
        public Nullable<decimal> E_Basic_Pay { get; set; }
        public DateTime? DOJ { get; set; }
        public DateTime? DOLeaveOrg { get; set; }
    }

    public enum Deduction
    {
        HRA = 1,
        FlatDeduction = 2,
        None = 3
    }

    public enum SalPercetageRatioForSuspendedEmployee
    {
        Fifty = 1,
        SeventyFive = 2
    }
}
