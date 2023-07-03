using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class TBLMONTHLYINPUT
    {
        public string BranchCode { get; set; }
        public byte SalMonth { get; set; }
        public short SalYear { get; set; }

        public int DaysInMonth {
            get
            {
                return DateTime.DaysInMonth(SalYear, SalMonth);
            }
        }

        public string EmployeeCode { get; set; }
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
        public Nullable<decimal> C_TotEarn { get; set; }
        public Nullable<decimal> C_TotDedu { get; set; }
        public Nullable<decimal> C_NetSal { get; set; }
        public Nullable<decimal> C_Pension { get; set; }
        public Nullable<decimal> C_GrossSalary { get; set; }
        public Nullable<bool> SalaryLock { get; set; }
        public Nullable<decimal> LWP { get; set; }
        public Nullable<float> OTHrs { get; set; }
        public Nullable<float> AOTHrs { get; set; }
        public Nullable<bool> DeductPFLoan { get; set; }
        public Nullable<bool> DeductNB { get; set; }
        public Nullable<bool> DeductTCS { get; set; }
        public Nullable<bool> DeductHouseLoan { get; set; }
        public Nullable<bool> DeductFestivalLoan { get; set; }
        public Nullable<bool> DeductCarLoan { get; set; }
        public Nullable<bool> DeductScooterLoan { get; set; }
        public Nullable<bool> DeductSundryAdv { get; set; }
        public Nullable<decimal> E_31 { get; set; }
        public Nullable<decimal> E_32 { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }


    }
}
