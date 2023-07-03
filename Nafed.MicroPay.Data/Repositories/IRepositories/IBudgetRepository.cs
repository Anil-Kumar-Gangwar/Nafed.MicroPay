using Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IBudgetRepository
    {
        IEnumerable<GetStaffBudgetDetailsList_Result> GetStaffBudgetList(string year, int? designationID);
        int GenerateStaffBudget(string forYear, string fromYear);
    }
}
