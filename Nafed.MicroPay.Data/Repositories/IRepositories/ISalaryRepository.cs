using Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface ISalaryRepository
    {
        int InsertMonthlyInput(Int16 currentYear, Int16 currentMonth, Int16 PreYear, Int16 PreMonth);
        int UpdateSalaryHeadsInMonthlyInput(string Head, Int16 currentMonth, Int16 currentYear, string flag);
        DataTable GetBranchCodeDetails(Int16 PreMonth, Int16 PreYear);
        DataTable GetBranchDetails(string SalaryHead, Int16 currentMonth, Int16 currentYear, int EmployeeId);
        int UpdateSalaryMonthlyInput(int EmployeeId, string MonthlyInputHeadId, decimal Amount, int currentMonth, int currentYear);
        decimal? GetMedicalReimbursement(int EmployeeId, string fromYear, string toYear);

        #region Sanction Loan
        IEnumerable<GetSanctionLoanList_Result> GetSanctionLoanList();
        bool DeleteSanction(string priorityNo, DateTime DateAvailLoan);
        #endregion 

        List<AnonymousSelectList> GetEmployeeByLoanType(int loanTypeId, bool oldLoanEmployee);

        bool UpdateSalaryPublishField(int? selectedEmployeeID, int? selectedBranchID, int? selectedEmployeeTypeID, int salMonth, int salYear, string buttonType);

        bool GeneralReports(string branchCode, int month, int year, string headName, string headDesc, string slcode, string payslipno, int employeeTypeId);
        bool UpdatePublishDAArrer(int? selectedEmployeeID, int? selectedBranchID, int? selectedEmployeeTypeID, int salMonth, int salYear, string buttonType);

        bool UpdatePublishPayArrer(int? selectedEmployeeID, int? selectedBranchID, int? selectedEmployeeTypeID, int salMonth, int salYear, string buttonType);

        DataTable GetArrearPeriodsDetails(string arrerType);

        DataTable PFLoanSummary(int month, int year,int ? toMonth);

        #region Yearly Report
        DataTable GetPFOpBalance(string fYear, string toYear);
        DataTable GetPFOpBalanceIU(string fYear, string toYear);
        DataTable GetEDLIStatement(int mon, int yr1);
        bool Update_FORM7PSdata(string fryear, string tryear);
        #endregion


        #region ====  Publish Final month salary===
        bool PublishSalary(PayrollApprovalRequest request);
        bool UndoPublishSalary(DTOModel.PayrollApprovalRequest request);
        #endregion


        #region === Publish DA Arrear==============
        bool PublishDAArrer(PayrollApprovalRequest request);
        bool UndoPublishDAArrear(PayrollApprovalRequest request);

        #endregion


        #region ==== Publish Pay Arrear ===========

        bool PublishPayArrear(PayrollApprovalRequest request);
        bool UndoPublishPayArrear(PayrollApprovalRequest request);

        #endregion

        DataTable GetEmployeeByLoanTypedt(int loanTypeId, bool oldLoanEmployee);
        bool UpdateTotalRefundData(string priorityNo, int serialNo, string period, decimal? balancePAmt, decimal? balanceIAmt, DateTime? refundDate, byte totlRefundMonthId, Int16 totlRefundYearId);

        DataSet GetECRDataForExport(int salMonth, int salYear, int? intRate);
        bool GeneralReportsSummation(string branchcode, string salmonthF, string salyearF, string salmonthT, string salyearT, List<string> lst, int empType);
    }
}
