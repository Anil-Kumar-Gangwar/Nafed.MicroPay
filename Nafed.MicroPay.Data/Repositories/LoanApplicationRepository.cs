using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
namespace Nafed.MicroPay.Data.Repositories
{
    public class LoanApplicationRepository : BaseRepository, ILoanApplicationRepository
    {

        public List<GetLoanApplicationList_Result> GetLoanApplicationList(int EmpID,int ? LoanApp)
        {
            return db.GetLoanApplicationList(EmpID, LoanApp).ToList();
        }

        public List<GetLoanApplicationDetails_Result> GetLoanApplicationDetails(int? EmpID, int? statusID,DateTime ? fromDate,DateTime ? toDate)
        {
            return db.GetLoanApplicationDetails(EmpID, statusID, fromDate, toDate).ToList();
        }
    }
}
