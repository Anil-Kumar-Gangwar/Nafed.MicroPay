using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IDashBoardRepository : IDisposable
    {
        IEnumerable<GetEmployeeDetailDashBoard_Result> GetEmployeeDetailForDashBoard(int? empCode);
        IEnumerable<GetEmployeeDOBDOJ_Result> GetEmployeeDOBWorkAnniversary(int? branchID, DateTime ? todayDate);
        IEnumerable<GetGreetingNotification_Result> TodayGreetingNotifications(DateTime todayDate);

    }
}
