using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
namespace Nafed.MicroPay.Services.IServices
{
    public interface ILoanApplicationService
    {
        List<LoanApplication> GetLoanApplicationList(int EmpID, int? LoanApp);
        LoanApplication GetEmployeeDtl(int EmpID);
        bool InsertUpdateLoanApplication(LoanApplication loanApp);
        List<LoanApplication> GetLoanApplicationDetails(int? EmpID, int? statusID, DateTime? fromDate, DateTime? toDate);
        bool InsertLoanApplicationHistory(LoanApplication loanApp);
        List<LoanApplication> GetLoanApplicationForApproval(int loggedInEmpID);
    }
}
