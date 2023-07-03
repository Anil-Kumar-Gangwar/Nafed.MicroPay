using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nafed.MicroPay.Model
{
    public class EmployeeSalaryNonRegular
    {
        public int EmpSalaryID { get; set; }

        public int EmployeeTypeID { get; set; }

        [Display(Name ="Employee")]
        public Nullable<int> EmployeeID { get; set; }
        [Display(Name = "Branch")]
        public Nullable<int> BranchID { get; set; }
        public string BranchCode { get; set; }
        [Display(Name = "Month")]
        public string SalMonth { get; set; }
        [Display(Name = "Year")]
        public short SalYear { get; set; }
        [Display(Name = "Employee Type")]
        public string RecordType { get; set; }
        public Nullable<int> SeqNo { get; set; }
        public string EmployeeCode { get; set; }
        [Display(Name = "Basic Salary")]
        public Nullable<decimal> E_Basic { get; set; }
        public Nullable<decimal> E_SP { get; set; }
        public Nullable<decimal> E_FDA { get; set; }
        public Nullable<decimal> E_01 { get; set; }
        public Nullable<decimal> E_02 { get; set; }
        public Nullable<decimal> E_03 { get; set; }
        public Nullable<decimal> E_04 { get; set; }
        public Nullable<decimal> E_05 { get; set; }
        public Nullable<decimal> E_06 { get; set; }
        public Nullable<decimal> E_07 { get; set; }
        public Nullable<decimal> E_08 { get; set; }
        public Nullable<decimal> E_09 { get; set; }
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
        public Nullable<decimal> E_Basic_A { get; set; }
        public Nullable<decimal> E_SP_A { get; set; }
        public Nullable<decimal> E_FDA_A { get; set; }
        public Nullable<decimal> E_01_A { get; set; }
        public Nullable<decimal> E_02_A { get; set; }
        public Nullable<decimal> E_03_A { get; set; }
        public Nullable<decimal> E_04_A { get; set; }
        public Nullable<decimal> E_05_A { get; set; }
        public Nullable<decimal> E_06_A { get; set; }
        public Nullable<decimal> E_07_A { get; set; }
        public Nullable<decimal> E_08_A { get; set; }
        public Nullable<decimal> E_09_A { get; set; }
        public Nullable<decimal> E_10_A { get; set; }
        public Nullable<decimal> E_11_A { get; set; }
        public Nullable<decimal> E_12_A { get; set; }
        public Nullable<decimal> E_13_A { get; set; }
        public Nullable<decimal> E_14_A { get; set; }
        public Nullable<decimal> E_15_A { get; set; }
        public Nullable<decimal> E_16_A { get; set; }
        public Nullable<decimal> E_17_A { get; set; }
        public Nullable<decimal> E_18_A { get; set; }
        public Nullable<decimal> E_19_A { get; set; }
        public Nullable<decimal> E_20_A { get; set; }
        public Nullable<decimal> E_21_A { get; set; }
        public Nullable<decimal> E_22_A { get; set; }
        public Nullable<decimal> E_23_A { get; set; }
        public Nullable<decimal> E_24_A { get; set; }
        public Nullable<decimal> E_25_A { get; set; }
        public Nullable<decimal> E_26_A { get; set; }
        public Nullable<decimal> E_27_A { get; set; }
        public Nullable<decimal> E_28_A { get; set; }
        public Nullable<decimal> E_29_A { get; set; }
        public Nullable<decimal> E_30_A { get; set; }
        public Nullable<decimal> D_PF_A { get; set; }
        public Nullable<decimal> D_VPF_A { get; set; }
        public Nullable<decimal> D_01_A { get; set; }
        public Nullable<decimal> D_02_A { get; set; }
        public Nullable<decimal> D_03_A { get; set; }
        public Nullable<decimal> D_04_A { get; set; }
        public Nullable<decimal> D_05_A { get; set; }
        public Nullable<decimal> D_06_A { get; set; }
        public Nullable<decimal> D_07_A { get; set; }
        public Nullable<decimal> D_08_A { get; set; }
        public Nullable<decimal> D_09_A { get; set; }
        public Nullable<decimal> D_10_A { get; set; }
        public Nullable<decimal> D_11_A { get; set; }
        public Nullable<decimal> D_12_A { get; set; }
        public Nullable<decimal> D_13_A { get; set; }
        public Nullable<decimal> D_14_A { get; set; }
        public Nullable<decimal> D_15_A { get; set; }
        public Nullable<decimal> D_16_A { get; set; }
        public Nullable<decimal> D_17_A { get; set; }
        public Nullable<decimal> D_18_A { get; set; }
        public Nullable<decimal> D_19_A { get; set; }
        public Nullable<decimal> D_20_A { get; set; }
        public Nullable<decimal> D_21_A { get; set; }
        public Nullable<decimal> D_22_A { get; set; }
        public Nullable<decimal> D_23_A { get; set; }
        public Nullable<decimal> D_24_A { get; set; }
        public Nullable<decimal> D_25_A { get; set; }
        public Nullable<decimal> D_26_A { get; set; }
        public Nullable<decimal> D_27_A { get; set; }
        public Nullable<decimal> D_28_A { get; set; }
        public Nullable<decimal> D_29_A { get; set; }
        public Nullable<decimal> D_30_A { get; set; }
        public Nullable<decimal> C_TotEarn { get; set; }
        public Nullable<decimal> C_TotDedu { get; set; }
        public Nullable<decimal> C_NetSal { get; set; }
        public Nullable<decimal> C_Pension { get; set; }
        public Nullable<decimal> C_GrossSalary { get; set; }
        public Nullable<bool> SalaryLock { get; set; }
        public Nullable<byte> Attendance { get; set; }
        public Nullable<byte> LWP { get; set; }
        public Nullable<byte> WorkingDays { get; set; }
        public Nullable<short> OTHrs { get; set; }
        public Nullable<short> AOTHrs { get; set; }
        public Nullable<bool> DeductLoan { get; set; }
        public Nullable<bool> DeductNB { get; set; }
        public Nullable<short> ELInstallNo { get; set; }
        public Nullable<short> GLInstallNo { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
