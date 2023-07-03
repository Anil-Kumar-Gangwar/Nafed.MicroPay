using System.Collections.Generic;
using System.Data;

namespace Nafed.MicroPay.Model
{
    public class PayrollMaster
    {
        private PayrollMaster()
        {

        }
        public static List<BranchSalaryHeadRule> branchSalaryHeadRule { get; set; }
        public static IEnumerable<SalaryHead> salHeads { set; get; }
        public static DataTable empPFBalanceDT { set; get; }

        public static decimal ? PFLoanRate { set; get; }
        public static int InstNo { set; get; }
        public static List<FDA> fdaSlab { set; get; }
        public static List<HillCompensation> hillCompensationSlab { set; get; }
        public static List<CCA> CCASlabs { set; get; }
        public static List<Branch> branchList { set; get; }
        public static List<tblMstLoanType> loanTypes { set; get; }
        public static List<GisDeduction> gisDeductions { set; get; }
        public static DataTable OTARates { set; get; }

        //public static DataTable LoanPriority { get; set; }
        public static List<LoanSlab> loanSlab { set; get; }
        public static DataTable LoanPriorityOld { set; get; }

        public static DataTable LoanPriorityNew { set; get; }

        public static DataTable LoanTransOld { set; get; }
        public static DataTable LoanTransNew { set; get; }

        #region finally dt
        public static DataTable F_LoanPriorityOld { set; get; }
        public static DataTable F_LoanPriorityNew { set; get; }
        public static DataTable F_LoanTransOld { set; get; }
        public static DataTable F_LoanTransNew { set; get; }
        #endregion
    }
}
