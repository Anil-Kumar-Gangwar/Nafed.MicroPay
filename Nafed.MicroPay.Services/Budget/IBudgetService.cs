using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.Budget
{
   public interface IBudgetService
    {
        #region ConfigureBudget
        PromotionCota GetPromotionDetails(int designationID);
        bool UpdatePromotionDetails(PromotionCota promotionCota);
        #endregion

        #region StaffBudget
        List<StaffBudget> GetStaffBudgetList(string year, int? designationID);
        int GetPresentStaff(int? designationID);
        int SaveStaffBudget(Model.StaffBudget staffBudget);
        StaffBudget GetStaffBudgetById(int staffBudgetId);
        bool UpdateStaffBudget(Model.StaffBudget staffBudget);
        bool Delete(int staffBudgetId);
        bool StaffBudgetExist(Model.StaffBudget staffBudget);
        int GenerateStaffBudget(string forYear, string fromYear);
        #endregion
    }
}
