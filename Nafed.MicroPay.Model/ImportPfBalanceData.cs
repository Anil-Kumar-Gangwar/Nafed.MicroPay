using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ImportPfBalanceData
    {
        public int Sno { set; get; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string BranchName { get; set; }
        public string PFNo { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string EmployeePFContribution { set; get; }
        public string VPF { set; get; }

        public string EmployerPFContribution { get; set; }

        public string Pension { set; get; }

        public string WithdrawlToEmployeeContribution { set; get; }

        public string WithdrawlToEmployerContribution { set; get; }

        public string AdditionToEmployeeContribution { set; get; }

        public string AdditionToEmployerContribution { set; get; }

        public string InterestWithdrawlToEmployeeContribution { set; get; }

        public string InterestWithdrawlToEmployerContribution { set; get; }

        public string EmployeeId { get; set; }
        public string BranchID { get; set; }

        public string DB_PFNo { set; get; }
        public string error { set; get; }
        public string warning { set; get; }
        public bool isDuplicatedRow { set; get; }
    }
    public class PfBalanceColValues
    {
        public List<string> empCode { set; get; }
        public List<string> BranchName { set; get; }
        public List<string> PfNo { set; get; }

    }
}
