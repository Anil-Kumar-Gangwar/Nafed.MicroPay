using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class FinalMonthlySalary
    {

        public DateTime periodInDateFormat
        {
            get
            {
                return (DateTime)new DateTime(SalYear, SalMonth, 1);
            }

        }

        public int EmployeeTypeID { set; get; }

        public string BranchCode { get; set; }
        public byte SalMonth { get; set; }
        public short SalYear { get; set; }
        public bool IsSupensionPeriodExists { set; get; } = false;


        public Nullable<decimal> E_Basic_S { get; set; }

        /// <summary>
        /// E_Basic_D mean -- Default Basic (From tblmstEmployeeSalary table)..
        /// </summary>
        public Nullable<decimal> E_Basic_D { get; set; }

        public byte PrevMonth
        {
            get
            {
                if (SalMonth == 1)
                    return 12;
                else
                    return (byte)(SalMonth - (byte)1);
            }
        }

        public short PrevYear
        {
            get
            {
                if (SalMonth == 1)
                    return (short)(SalYear - ((short)1));
                else
                    return SalYear;
            }
        }
        public string RecordType { get; set; }
        public int SeqNo { get; set; }
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
        public decimal E_Basic_A { get; set; }
        public decimal E_SP_A { get; set; }
        public decimal E_FDA_A { get; set; }
        public decimal E_01_A { get; set; }
        public decimal E_02_A { get; set; }
        public decimal E_03_A { get; set; }
        public decimal E_04_A { get; set; }
        public decimal E_05_A { get; set; }
        public decimal E_06_A { get; set; }
        public decimal E_07_A { get; set; }
        public decimal E_08_A { get; set; }
        public decimal E_09_A { get; set; }
        public decimal E_10_A { get; set; }
        public decimal E_11_A { get; set; }
        public decimal E_12_A { get; set; }
        public decimal E_13_A { get; set; }
        public decimal E_14_A { get; set; }
        public decimal E_15_A { get; set; }
        public decimal E_16_A { get; set; }
        public decimal E_17_A { get; set; }
        public decimal E_18_A { get; set; }
        public decimal E_19_A { get; set; }
        public decimal E_20_A { get; set; }
        public decimal E_21_A { get; set; }
        public decimal E_22_A { get; set; }
        public decimal E_23_A { get; set; }
        public decimal E_24_A { get; set; }
        public decimal E_25_A { get; set; }
        public decimal E_26_A { get; set; }
        public decimal E_27_A { get; set; }
        public decimal E_28_A { get; set; }
        public decimal E_29_A { get; set; }
        public decimal E_30_A { get; set; }
        public decimal D_PF_A { get; set; }
        public decimal D_VPF_A { get; set; }
        public decimal D_01_A { get; set; }
        public decimal D_02_A { get; set; }
        public decimal D_03_A { get; set; }
        public decimal D_04_A { get; set; }
        public decimal D_05_A { get; set; }
        public decimal D_06_A { get; set; }
        public decimal D_07_A { get; set; }
        public decimal D_08_A { get; set; }
        public decimal D_09_A { get; set; }
        public decimal D_10_A { get; set; }
        public decimal D_11_A { get; set; }
        public decimal D_12_A { get; set; }
        public decimal D_13_A { get; set; }
        public decimal D_14_A { get; set; }
        public decimal D_15_A { get; set; }
        public decimal D_16_A { get; set; }
        public decimal D_17_A { get; set; }
        public decimal D_18_A { get; set; }
        public decimal D_19_A { get; set; }
        public decimal D_20_A { get; set; }
        public decimal D_21_A { get; set; }
        public decimal D_22_A { get; set; }
        public decimal D_23_A { get; set; }
        public decimal D_24_A { get; set; }
        public decimal D_25_A { get; set; }
        public decimal D_26_A { get; set; }
        public decimal D_27_A { get; set; }
        public decimal D_28_A { get; set; }
        public decimal D_29_A { get; set; }
        public decimal D_30_A { get; set; }
        public decimal C_TotEarn { get; set; }
        public decimal C_TotDedu { get; set; }
        public decimal C_NetSal { get; set; }
        public decimal C_Pension { get; set; }
        public decimal C_GrossSalary { get; set; }
        public Nullable<bool> SalaryLock { get; set; }
        public Nullable<decimal> Attendance { get; set; }
        public Nullable<decimal> LWP { get; set; }
        public Nullable<byte> WorkingDays { get; set; }
        public decimal OTHrs { get; set; }
        public decimal AOTHrs { get; set; }
        public Nullable<bool> DeductPFLoan { get; set; }
        public Nullable<bool> DeductNB { get; set; }
        public Nullable<bool> DeductTCS { get; set; }
        public Nullable<decimal> ELInstallNo { get; set; }
        public Nullable<decimal> GLInstallNo { get; set; }
        public string PaidInPeriod { get; set; }
        public Nullable<bool> DeductHouseLoan { get; set; }
        public Nullable<bool> DeductFestivalLoan { get; set; }
        public Nullable<bool> DeductCarLoan { get; set; }
        public Nullable<bool> DeductScooterLoan { get; set; }
        public Nullable<bool> DeductSundryAdv { get; set; }
        public Nullable<bool> chkNegative { get; set; }
        public Nullable<bool> chkAlwaysNegative { get; set; }
        public Nullable<int> PFLoanInstNo { get; set; }
        public Nullable<int> HBLoanInstNo { get; set; }
        public Nullable<int> FestivalLoanInstNo { get; set; }
        public Nullable<int> CarLoanInstNo { get; set; }
        public Nullable<int> ScooterLoanInstNo { get; set; }
        public Nullable<int> SundryAdvInstNo { get; set; }
        public System.DateTime DateofGenerateSalary { get; set; }
        public Nullable<int> PaySlipNo { get; set; }
        public Nullable<double> E_BASIC_INC { get; set; }
        public Nullable<double> RateADAA { get; set; }
        public Nullable<double> RateHRAA { get; set; }
        public Nullable<double> RatePFA { get; set; }
        public Nullable<double> RateVPFA { get; set; }
        public string ArrearType { get; set; }
        public Nullable<bool> DeductLIC { get; set; }
        public string BANKCODE { get; set; }
        public string BANKACNO { get; set; }
        public string DESIGNATIONCODE { get; set; }
        public Nullable<decimal> E_31 { get; set; }
        public decimal E_31_A { get; set; }
        public Nullable<decimal> E_32 { get; set; }
        public decimal E_32_A { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> DesignationID { get; set; }

        public EmpPFOpBalance empPFOpBalance { set; get; }

        public DataTable newLoanPriority { set; get; }
        public DataTable newLoanTrans { get; set; }

    }
}
