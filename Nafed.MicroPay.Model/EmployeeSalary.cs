using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmployeeSalary 
    {
        public bool JoinedAfterNoon { set; get; }
        public int EmpSalaryID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public string EmployeeCategory { set; get; }


        public string EmployeeName { set; get; }

        public int BranchID { get; set; }
        public string BranchCode { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<decimal> E_Basic { get; set; }
        public Nullable<decimal> LastBasic { get; set; }
        public Nullable<decimal> E_SP { get; set; }
        public Nullable<decimal> LastIncrement { get; set; }
        public Nullable<decimal> SPCashALW { get; set; }
        public Nullable<decimal> SPTelexALW { get; set; }
        public Nullable<decimal> SPStenographyALW { get; set; }
        public Nullable<decimal> SPAccWorkALW { get; set; }
        public Nullable<decimal> SPODupMachineALW { get; set; }
        public Nullable<decimal> SPFaxMachineALW { get; set; }
        public Nullable<decimal> SPAsDriverALW { get; set; }
        public Nullable<decimal> E_FDA { get; set; }

        public Nullable<decimal> BasicSalPercentageForSuspendedEmp { get; set; }
        public string OTACode { get; set; }
        public Nullable<decimal> E_01 { get; set; }
        public Nullable<decimal> E_02 { get; set; }
        public Nullable<decimal> E_03 { get; set; }
        public Nullable<decimal> E_04 { get; set; }
        public Nullable<decimal> E_05 { get; set; }
        public Nullable<decimal> E_06 { get; set; }
        public Nullable<decimal> E_07 { get; set; }
        public Nullable<decimal> E_08 { get; set; }
        public Nullable<decimal> E_09 { get; set; }
        public Nullable<decimal> SANightWatch { get; set; }
        public Nullable<decimal> SADCAllowance { get; set; }
        public Nullable<decimal> SAAssamComp { get; set; }
        public Nullable<decimal> SAFairEPart { get; set; }
        public Nullable<decimal> SAToSplPost { get; set; }
        public Nullable<decimal> E_10 { get; set; }
        public Nullable<decimal> E_11 { get; set; }
        public Nullable<decimal> E_12 { get; set; }
        public Nullable<decimal> E_13 { get; set; }
        public Nullable<decimal> E_14 { get; set; }
        public Nullable<decimal> E_15 { get; set; }
        public Nullable<decimal> E_16 { get; set; }
        public Nullable<decimal> E_17 { get; set; }
        public Nullable<decimal> E_18 { get; set; }
        public Nullable<decimal> E_19 { get; set; }
        public Nullable<decimal> E_20 { get; set; }
        public Nullable<decimal> E_21 { get; set; }
        public Nullable<decimal> E_22 { get; set; }
        public Nullable<decimal> E_23 { get; set; }
        public Nullable<decimal> E_24 { get; set; }
        public Nullable<decimal> E_25 { get; set; }
        public Nullable<decimal> E_26 { get; set; }
        public Nullable<decimal> E_27 { get; set; }
        public Nullable<decimal> E_28 { get; set; }
        public Nullable<decimal> E_29 { get; set; }
        public Nullable<decimal> E_30 { get; set; }
        public Nullable<decimal> D_PF { get; set; }
        public Nullable<decimal> D_VPF { get; set; }
        public Nullable<decimal> D_01 { get; set; }
        public Nullable<decimal> D_02 { get; set; }
        public Nullable<decimal> D_03 { get; set; }
        public Nullable<decimal> D_04 { get; set; }
        public Nullable<decimal> D_05 { get; set; }
        public Nullable<decimal> D_06 { get; set; }
        public Nullable<decimal> D_07 { get; set; }
        public Nullable<decimal> D_08 { get; set; }
        public Nullable<decimal> D_09 { get; set; }
        public Nullable<decimal> D_10 { get; set; }
        public Nullable<decimal> D_11 { get; set; }
        public Nullable<decimal> D_12 { get; set; }
        public Nullable<decimal> D_13 { get; set; }
        public Nullable<decimal> D_14 { get; set; }
        public Nullable<decimal> D_15 { get; set; }
        public Nullable<decimal> D_16 { get; set; }
        public Nullable<decimal> D_17 { get; set; }
        public Nullable<decimal> D_18 { get; set; }
        public Nullable<decimal> D_19 { get; set; }
        public Nullable<decimal> D_20 { get; set; }
        public Nullable<decimal> D_21 { get; set; }
        public Nullable<decimal> D_22 { get; set; }
        public Nullable<decimal> D_23 { get; set; }
        public Nullable<decimal> D_24 { get; set; }
        public Nullable<decimal> D_25 { get; set; }
        public Nullable<decimal> D_26 { get; set; }
        public Nullable<decimal> D_27 { get; set; }
        public Nullable<decimal> D_28 { get; set; }
        public Nullable<decimal> D_29 { get; set; }
        public Nullable<decimal> D_30 { get; set; }
        public string ModePay { get; set; }
        public string BankCode { get; set; }
        public string BankAcNo { get; set; }
        public Nullable<bool> HRA { get; set; }
        public Nullable<bool> UnionFee { get; set; }
        public Nullable<bool> ProfTax { get; set; }
        public Nullable<byte> NoofChildren { get; set; }
        public Nullable<bool> SportClub { get; set; }
        public Nullable<bool> IsRateVPF { get; set; }
        public Nullable<decimal> VPFValueRA { get; set; }
        public Nullable<System.DateTime> LastIncrementDate { get; set; }
        public Nullable<bool> CCA { get; set; }
        public Nullable<bool> WASHING { get; set; }
        public Nullable<bool> None { get; set; }
        public Nullable<bool> DELETEDEMPLOYEE { get; set; }
        public Nullable<bool> IsSalgenrated { get; set; }
        public Nullable<decimal> E_31 { get; set; }
        public Nullable<decimal> E_32 { get; set; }

        public Nullable<int> PFNO { get; set; }
        public decimal pFLoanAccuralRate { set; get; }
        public bool IsOTACode { get; set; }
        public bool IsSuspended { get; set; }

        public bool IsPensionDeducted { get; set; }

        public bool IsFlatDeduction { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        public ICollection<EmployeeSuspensionPeriod> SuspensionPeriods { set; get; }
        
        ///// <summary>
        ///// E_Baic_S mean -- Basic in suspension period..
        ///// </summary>
        //public Nullable<decimal> E_Basic_S { get; set; } 

        public Nullable<DateTime> SuspensionFromData { set; get; }
        public Nullable<DateTime> SuspensionToData { set; get; }

        #region TCS File Fields

        public string emplcd { set; get;}
        public decimal tcsln { set; get;}
        public decimal gl_bal { set; get;}
        public decimal el_bal {set;get;}
        #endregion

        public Nullable<int> BranchID_Pay { get; set; }
        public Nullable<decimal> E_Basic_Pay { get; set; }

        public DateTime ?  DOJ { get; set; }
        public DateTime? DOLeaveOrg { get; set; }

    }
}
