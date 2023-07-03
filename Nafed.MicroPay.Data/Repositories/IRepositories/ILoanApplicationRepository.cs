using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface ILoanApplicationRepository : IDisposable
    {
        List<GetLoanApplicationList_Result> GetLoanApplicationList(int EmpID, int? LoanApp);
        List<GetLoanApplicationDetails_Result> GetLoanApplicationDetails(int? EmpID, int? statusID, DateTime? fromDate, DateTime? toDate);
    }
}
