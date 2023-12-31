﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class SalarySlipModel
    {
        public Nullable<int> PaySlipNo { get; set; }
        public string Employeecode { get; set; }
        public string EmployeeType { get; set; }
        public byte SalMonth { get; set; }
        public Nullable<decimal> Medical { get; set; }
        public Nullable<int> SEN_CODE { get; set; }
        public short SalYear { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public Nullable<int> PFNO { get; set; }
        public string AccountNo { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public string BankCode { get; set; }
        public Nullable<decimal> TotalDeduction { get; set; }
        public Nullable<decimal> TotalEarning { get; set; }
        public Nullable<decimal> IncTaxDeduc { get; set; }
        public Nullable<decimal> ProfTax { get; set; }
        public Nullable<decimal> TCSLoan { get; set; }
        public Nullable<decimal> LifeIns { get; set; }
        public Nullable<decimal> BFund { get; set; }
        public Nullable<decimal> SCM { get; set; }
        public Nullable<decimal> FlatDeduc { get; set; }
        public Nullable<decimal> NafBazDeduc { get; set; }
        public Nullable<decimal> GISDeduc { get; set; }
        public Nullable<decimal> RelFundDeduc { get; set; }
        public Nullable<decimal> MISCD1 { get; set; }
        public Nullable<decimal> MISCD2 { get; set; }
        public Nullable<decimal> MISCD3 { get; set; }
        public Nullable<decimal> UnionFee { get; set; }
        public Nullable<decimal> PFLoan { get; set; }
        public Nullable<decimal> ConvLoan { get; set; }
        public Nullable<decimal> HBLoan { get; set; }
        public Nullable<decimal> FestLoan { get; set; }
        public Nullable<decimal> CarLoan { get; set; }
        public Nullable<decimal> SunAdv { get; set; }
        public Nullable<decimal> ScooterLoan { get; set; }
        public Nullable<decimal> D_PF_A { get; set; }
        public Nullable<decimal> D_VPF_A { get; set; }
        public Nullable<decimal> AddDA { get; set; }
        public Nullable<decimal> HRAll { get; set; }
        public Nullable<decimal> OTimeAll { get; set; }
        public Nullable<decimal> AddiOverAll { get; set; }
        public Nullable<decimal> OTARATE { get; set; }
        public Nullable<decimal> CCA { get; set; }
        public Nullable<decimal> CEA { get; set; }
        public Nullable<decimal> WashAll { get; set; }
        public Nullable<decimal> HillCompenAll { get; set; }
        public Nullable<decimal> SplAll { get; set; }
        public Nullable<decimal> WintAll { get; set; }
        public Nullable<decimal> MISCI1 { get; set; }
        public Nullable<decimal> MISCI2 { get; set; }
        public Nullable<decimal> MISCI3 { get; set; }
        public Nullable<decimal> BasicSalary { get; set; }
        public Nullable<decimal> FixedDA { get; set; }
        public Nullable<decimal> SplPay { get; set; }
        public Nullable<decimal> NetPay { get; set; }
        public Nullable<decimal> Attendance { get; set; }
        public Nullable<decimal> LWP { get; set; }
        public Nullable<byte> WorkingDays { get; set; }
        public Nullable<decimal> OTHRs { get; set; }
        public Nullable<decimal> AOTHrs { get; set; }
        public Nullable<decimal> E_BASIC { get; set; }
        public Nullable<decimal> E_18_A { get; set; }
        public Nullable<decimal> E_17_A { get; set; }
        public Nullable<decimal> E_14_A { get; set; }
        public Nullable<decimal> E_15_A { get; set; }
        public Nullable<decimal> E_16_A { get; set; }
        public Nullable<int> PFLOANINSTNO { get; set; }
        public Nullable<int> HBLOANINSTNO { get; set; }
        public Nullable<int> FESTIVALLOANINSTNO { get; set; }
        public Nullable<int> CARLOANINSTNO { get; set; }
        public Nullable<int> SCOOTERLOANINSTNO { get; set; }
        public Nullable<decimal> E_19_A { get; set; }
        public Nullable<int> SUNDRYADVINSTNO { get; set; }
        public Nullable<decimal> GLINSTALLNO { get; set; }
        public Nullable<decimal> ELINSTALLNO { get; set; }
        public Nullable<decimal> maxrateperhour { get; set; }
        public Nullable<decimal> Petrol_Reimb { get; set; }
        public Nullable<decimal> Drv__All_ { get; set; }
        public decimal Sal_Deduc { get; set; }
        public Nullable<decimal> Transport_Allowance { get; set; }
    }
}
