using Nafed.MicroPay.Model;
using System.Collections.Generic;
using System.Data;

namespace Nafed.MicroPay.Services.Salary
{
    public interface IAdjustOldLoanService
    {
        List<SelectListModel> GetEmployeeByLoanType(int loanTypeId, bool oldLoanEmployee);
        SanctionLoan GetAdjustLoanOldDetails(int employeeId, int loanTypeId, bool showOld, out List<SanctionLoan> sanctionloan);
        bool UpdateTotalRefundDetails(SanctionLoan sanctionLoan);
        bool UpdateLastInstallAdjust(SanctionLoan sanctionLoan);
        bool UpdateInstallmentAdjestment(SanctionLoan sanctionLoan);
        bool UpdateLoanFinish(SanctionLoan sanctionLoan);
        bool UpdatePartialRefund(SanctionLoan sanctionLoan);
        bool UpdateLoanpriority(string priority, decimal remainingInstallments, decimal installmentAmt, decimal intInstallment, decimal intInstallmentAmount, bool status, bool flag);
        bool UpdateAllLastInstallment(int loanTypeId);

        bool UpdateSalaryPublishField(PublishSalaryFilters publishSalary, string buttonType);
        bool UpdatePublishDAArrer(PublishSalaryFilters publishSalary, string buttonType);
        bool UpdatePublishPayArrer(PublishSalaryFilters publishSalary, string buttonType);

        List<ArrerPeriodDetails> GetArrearPeriodsDetails(string arrerType);

        List<PFLoanSummaryReport> PFLoanSummary(int month, int year,int ?toMonth);

        bool UpdatePFLoanSummary(decimal cpension, string employeeCode, int month, int year, int userId);

        #region Yearly Report
        int AnuualPFRegister(string fYear, string toYear);
        int PFIUYearlyReport(string fromYear, string tYear);
        bool EDLIStatement(int mon, int yr1);
        bool Update_FORM7PSdata(string fryear, string tryear);
        #endregion 
        DataSet GetECRData(int salMonth, int salYear, int? intRate);
        bool ExportECR(DataTable dsSource, string sFullPath, string fileName);
    }
}
