using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ImportMonthlyInput
    {
        public int Sno { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeType { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string E_Basic { get; set; }
        public string E_SP { get; set; }
        public string E_FDA { get; set; }
        public string E_01 { get; set; }
        public string E_02 { get; set; }
        public string E_03 { get; set; }
        public string E_04 { get; set; }
        public string E_05 { get; set; }
        public string E_06 { get; set; }
        public string E_07 { get; set; }
        public string E_08 { get; set; }
        public string E_09 { get; set; }
        public string E_10 { get; set; }
        public string E_11 { get; set; }
        public string E_12 { get; set; }
        public string E_13 { get; set; }
        public string E_14 { get; set; }
        public string E_15 { get; set; }
        public string E_16 { get; set; }
        public string E_17 { get; set; }
        public string E_18 { get; set; }
        public string E_19 { get; set; }
        public string E_20 { get; set; }
        public string E_21 { get; set; }
        public string E_22 { get; set; }
        public string E_23 { get; set; }
        public string E_24 { get; set; }
        public string E_25 { get; set; }
        public string E_26 { get; set; }
        public string E_27 { get; set; }
        public string E_28 { get; set; }
        public string E_29 { get; set; }
        public string E_30 { get; set; }
        public string D_PF { get; set; }
        public string D_VPF { get; set; }
        public string D_01 { get; set; }
        public string D_02 { get; set; }
        public string D_03 { get; set; }
        public string D_04 { get; set; }
        public string D_05 { get; set; }
        public string D_06 { get; set; }
        public string D_07 { get; set; }
        public string D_08 { get; set; }
        public string D_09 { get; set; }
        public string D_10 { get; set; }
        public string D_11 { get; set; }
        public string D_12 { get; set; }
        public string D_13 { get; set; }
        public string D_14 { get; set; }
        public string D_15 { get; set; }
        public string D_16 { get; set; }
        public string D_17 { get; set; }
        public string D_18 { get; set; }
        public string D_19 { get; set; }
        public string D_20 { get; set; }
        public string D_21 { get; set; }
        public string D_22 { get; set; }
        public string D_23 { get; set; }
        public string D_24 { get; set; }
        public string D_25 { get; set; }
        public string D_26 { get; set; }
        public string D_27 { get; set; }
        public string D_28 { get; set; }
        public string D_29 { get; set; }
        public string D_30 { get; set; }
        public string C_TotEarn { get; set; }
        public string C_TotDedu { get; set; }
        public string C_NetSal { get; set; }
        public string C_Pension { get; set; }
        public string C_GrossSalary { get; set; }
        public string SalaryLock { get; set; }
        public string LWP { get; set; }
        public string OTHrs { get; set; }
        public string AOTHrs { get; set; }
        public string DeductPFLoan { get; set; }
        public string DeductNB { get; set; }
        public string DeductTCS { get; set; }
        public string DeductHouseLoan { get; set; }
        public string DeductFestivalLoan { get; set; }
        public string DeductCarLoan { get; set; }
        public string DeductScooterLoan { get; set; }
        public string DeductSundryAdv { get; set; }
        public string E_31 { get; set; }
        public string E_32 { get; set; }
        //public System.DateTime CreatedOn { get; set; }
        //public int CreatedBy { get; set; }
        //public Nullable<System.DateTime> UpdatedOn { get; set; }
        //public Nullable<int> UpdatedBy { get; set; }
        //public int MonthlyInputId { get; set; }
        public string EmployeeId { get; set; }
        public string BranchId { get; set; }

        public string error { set; get; }
        public string warning { set; get; }
        public bool isDuplicatedRow { set; get; }
    }

    public class MonthlyInputColValues
    {
        public List<string> empCode { set; get; }
        public List<string> BranchCode { set; get; }
    }

    public class MonthlyInputExport
    {
        public int Sno { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeType { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal E_Basic { get; set; }
        public decimal E_SP { get; set; }
        public decimal E_FDA { get; set; }
        public decimal E_01 { get; set; }
        public decimal E_02 { get; set; }
        public decimal E_03 { get; set; }
        public decimal E_04 { get; set; }
        public decimal E_05 { get; set; }
        public decimal E_06 { get; set; }
        public decimal E_07 { get; set; }
        public decimal E_08 { get; set; }
        public decimal E_09 { get; set; }
        public decimal E_10 { get; set; }
        public decimal E_11 { get; set; }
        public decimal E_12 { get; set; }
        public decimal E_13 { get; set; }
        public decimal E_14 { get; set; }
        public decimal E_15 { get; set; }
        public decimal E_16 { get; set; }
        public decimal E_17 { get; set; }
        public decimal E_18 { get; set; }
        public decimal E_19 { get; set; }
        public decimal E_20 { get; set; }
        public decimal E_21 { get; set; }
        public decimal E_22 { get; set; }
        public decimal E_23 { get; set; }
        public decimal E_24 { get; set; }
        public decimal E_25 { get; set; }
        public decimal E_26 { get; set; }
        public decimal E_27 { get; set; }
        public decimal E_28 { get; set; }
        public decimal E_29 { get; set; }
        public decimal E_30 { get; set; }
        public decimal D_PF { get; set; }
        public decimal D_VPF { get; set; }
        public decimal D_01 { get; set; }
        public decimal D_02 { get; set; }
        public decimal D_03 { get; set; }
        public decimal D_04 { get; set; }
        public decimal D_05 { get; set; }
        public decimal D_06 { get; set; }
        public decimal D_07 { get; set; }
        public decimal D_08 { get; set; }
        public decimal D_09 { get; set; }
        public decimal D_10 { get; set; }
        public decimal D_11 { get; set; }
        public decimal D_12 { get; set; }
        public decimal D_13 { get; set; }
        public decimal D_14 { get; set; }
        public decimal D_15 { get; set; }
        public decimal D_16 { get; set; }
        public decimal D_17 { get; set; }
        public decimal D_18 { get; set; }
        public decimal D_19 { get; set; }
        public decimal D_20 { get; set; }
        public decimal D_21 { get; set; }
        public decimal D_22 { get; set; }
        public decimal D_23 { get; set; }
        public decimal D_24 { get; set; }
        public decimal D_25 { get; set; }
        public decimal D_26 { get; set; }
        public decimal D_27 { get; set; }
        public decimal D_28 { get; set; }
        public decimal D_29 { get; set; }
        public decimal D_30 { get; set; }
        public decimal C_TotEarn { get; set; }
        public decimal C_TotDedu { get; set; }
        public decimal C_NetSal { get; set; }
        public decimal C_Pension { get; set; }
        public decimal C_GrossSalary { get; set; }
        public string SalaryLock { get; set; }
        public decimal LWP { get; set; }
        public Single OTHrs { get; set; }
        public Single AOTHrs { get; set; }
        public string DeductPFLoan { get; set; }
        public string DeductNB { get; set; }
        public string DeductTCS { get; set; }
        public string DeductHouseLoan { get; set; }
        public string DeductFestivalLoan { get; set; }
        public string DeductCarLoan { get; set; }
        public string DeductScooterLoan { get; set; }
        public string DeductSundryAdv { get; set; }
        public decimal E_31 { get; set; }
        public decimal E_32 { get; set; }

        public int EmployeeId { get; set; }
        public int BranchId { get; set; }

        public string error { set; get; }
        public string warning { set; get; }
        public bool isDuplicatedRow { set; get; }
    }
}
