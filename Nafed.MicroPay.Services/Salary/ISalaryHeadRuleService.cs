using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.Salary
{
    public  interface ISalaryHeadRuleService
    {
        #region Salary Head Rules
        List<SalaryHeadField> GetSalaryHeadFields(int? employeeTypeID = null);
        SalaryHead GetSalaryHeadFormulaRule(string fieldName, int? employeeTypeID = null);
        bool PostSalaryFormData(SalaryHead salHead);
        bool PostBranchHeadRule(BranchSalaryHeadRule bHeadRule);
        IEnumerable<BranchSalaryHeadRule> GetBranchSalaryHeadRules(string fieldName, int? employeeTypeID);
        bool DeleteBranchHeadFieldRule(int branchFormulaID, string fieldName);
        BranchSalaryHeadRule GetBranchHeadFieldRule(int bFormulaID);
        bool IsBranchHeadRuleExists(string fieldName, int branchID, int? employeeTypeID);


        #endregion

        #region Salary Head Slab Configuration
        List<SalaryHeadSlab> GetSalaryHeadSlabList(string fieldName);
        SalaryHeadSlab GetSalaryHeadSlab(SalaryHeadSlab salaryHeadSlab);
        bool SalaryHeadSlabExists(SalaryHeadSlab salaryHeadSlab);
        bool InsertSalaryHeadSlab(SalaryHeadSlab createSalaryHeadSlab);
        bool UpdateSalaryHeadSlab(SalaryHeadSlab updateSalaryHeadSlab);
        bool Delete(SalaryHeadSlab salaryHeadSlab);

        #endregion


        #region Sanction Loan
        List<Model.SanctionLoan> GetSanctionLoanList();
        LoanTypeDetail GetLoanTypeDetails(int loanTypeID);
        PriorityNoDetails GetLoanNumberDetails(int loanTypeID, string empCode);
        bool SaveSanctionLoan(SanctionLoan sanctionLoan);
        SanctionLoan GetSanctionLoanByID(string priorityNo, int mstLoanID);
        bool DeleteSanction(string priorityNo,int mstLoanID, int updatedBy);
        bool UpdateSanctionLoan(SanctionLoan sanctionLoan);
        #endregion

        #region Monthly Input
        List<SelectListModel> GetEmployeeList(int empTypeID, int salMonth, int salYear);
        bool CheckMonthlyInputDetails(int currentYear, int currentMonth, int PreYear, int PreMonth);
        DataTable GetBranchDetails(string SalaryHead, int EmployeeId, int currentMonth, int currentYear);

        bool UpdateSalaryMonthlyInput(Model.SalaryMonthlyInput salaryMonthlyInput);
        decimal? GetMedicalReimbursement(int EmployeeId, string fromYear, string toYear);

        bool GetFinancialMonthlyDetails(int selectedMonth, int selectedYear, int employeeId);

        #endregion

        #region Individual Head Report
        bool GeneralReports(string branchCode, int month, int year, string headName, string headDesc, string slcode, string payslipno, int employeeTypeId);
        #endregion

        bool GeneralReportsSummation(string branchcode, string salmonthF, string salyearF, string salmonthT, string salyearT, List<string> lst, int empType);
    }
}
