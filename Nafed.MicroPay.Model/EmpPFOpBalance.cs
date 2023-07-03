using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmpPFOpBalance
    {

        public List<SelectListModel> branches { set; get; }

        public string Employeecode { get; set; }
        public int? PFAcNo { get; set; }
        public byte Salmonth { get; set; }
        public int Salyear { get; set; }
        public string BRANCHCODE { get; set; }
        public double EmplOpBal { get; set; }
        public double EmplrOpBal { get; set; }
        public decimal EmployeePFCont { get; set; }
        public decimal VPF { get; set; }
        public decimal Pension { get; set; }
        public decimal EmployerPFCont { get; set; }
        public decimal EmployeeInterest { get; set; }
        public decimal EmployerInterest { get; set; }
        public decimal InterestNRPFLoan { get; set; }
        public decimal InterestRate { get; set; }
        public Nullable<decimal> NonRefundLoan { get; set; }
        public Nullable<decimal> TotalPFBalance { get; set; }
        public Nullable<decimal> InterestTotal { get; set; }
        public Nullable<double> TotalPFOpeningEmpl { get; set; }
        public Nullable<double> TotalPFOpeningEmplr { get; set; }

        public Nullable<double> EmplClBal
        {
            get
            {
                return EmplOpBal + (double)(EmployeePFCont + VPF + EmployeeInterest 
                    + AdditionEmployeeAc ?? 0 + PF_DAarear ?? 0
                    + VPF_DAarear ?? 0+ PF_payarear ?? 0 + VPF_payarear ?? 0 - (WithdrawlEmployeeAc ?? 0 + IntWDempl ?? 0));
            }
        }
        public Nullable<double> EmplrClBal {
            get
            {
                return EmplrOpBal + (double)(EmployerPFCont + EmployerInterest 
                    + AdditionEmployerAc ?? 0 - (WithdrawlEmployerAc ?? 0 + IntWDemplr ??0));
            }
        }

        public Nullable<decimal> WithdrawlEmployeeAc { get; set; }
        public Nullable<decimal> WithdrawlEmployerAc { get; set; }
        public Nullable<decimal> AdditionEmployeeAc { get; set; }
        public Nullable<decimal> AdditionEmployerAc { get; set; }
        public Nullable<decimal> IntWDempl { get; set; }
        public Nullable<decimal> IntWDemplr { get; set; }
        public string Reason { get; set; }
        public Nullable<decimal> PF_DAarear { get; set; }
        public Nullable<decimal> VPF_DAarear { get; set; }
        public Nullable<decimal> PF_payarear { get; set; }
        public Nullable<decimal> VPF_payarear { get; set; }
        public Nullable<bool> PensionDeduct { get; set; }
        public Nullable<bool> IsInterestCalculate { get; set; }
        public Nullable<decimal> additionEmpInt { get; set; }
        public Nullable<decimal> additionEmployerInt { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}
