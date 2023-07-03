using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;

namespace Nafed.MicroPay.Services.IServices
{
  public interface ILeaveBalanceAsOfNowService
    {
        List<LeaveBalanceAsOfNow> GetEmployeeLeaveBalanceAsOfNowDetails(string employeecode, string branchId, string year);

        int UpdateLeavesBalance(Model.LeaveBalanceAsOfNow leaveBalanceAsOfNow);
    }
}
