using Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories
{
   public class BudgetRepository:BaseRepository,IBudgetRepository
    {
       public IEnumerable<GetStaffBudgetDetailsList_Result> GetStaffBudgetList(string year, int? designationID)
        {
            return db.GetStaffBudgetDetailsList(year, designationID);
        }

        public int GenerateStaffBudget(string forYear, string fromYear)
        {
            return db.GenerateStaffBudget(forYear, fromYear);
        }

    }
}
