using System;
using System.Collections.Generic;
using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;


namespace Nafed.MicroPay.Data.Repositories
{
    public class DashBoardRepository : BaseRepository, IDashBoardRepository
    {
        public IEnumerable<GetEmployeeDetailDashBoard_Result> GetEmployeeDetailForDashBoard(int? empCode)
        {
            return db.GetEmployeeDetailDashBoard(empCode);
        }

        public IEnumerable<GetEmployeeDOBDOJ_Result> GetEmployeeDOBWorkAnniversary(int? branchID, DateTime? todayDate)
        {
            return db.GetEmployeeDOBDOJ(branchID, todayDate);
        }

        public IEnumerable<GetGreetingNotification_Result> TodayGreetingNotifications(DateTime todayDate)
        {
            log.Info($"DashBoardRepository/TodayGreetingNotifications /todayDate:{todayDate}");

            try
            {
                return db.GetGreetingNotification(todayDate);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
