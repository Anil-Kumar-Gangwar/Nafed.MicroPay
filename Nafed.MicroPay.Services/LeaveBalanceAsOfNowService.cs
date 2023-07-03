using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using AutoMapper;
using System.Data;
using Nafed.MicroPay.Common;
using System.IO;

namespace Nafed.MicroPay.Services
{
   public class LeaveBalanceAsOfNowService: BaseService, ILeaveBalanceAsOfNowService
    {
        private readonly IGenericRepository genericRepo;
        private readonly ILeaveRepository empLeaveRepo;

        public LeaveBalanceAsOfNowService(IGenericRepository genericRepo, ILeaveRepository empLeaveRepo)
        {
            this.genericRepo = genericRepo;
            this.empLeaveRepo = empLeaveRepo;
        }

        public List<Model.LeaveBalanceAsOfNow> GetEmployeeLeaveBalanceAsOfNowDetails(string employeecode, string branchId, string year)
        {
            var Leavelist = new List<Model.LeaveBalanceAsOfNow>();
            DataTable result = null;
            try
            {
                result = empLeaveRepo.GetLeaveBalanceAsOfNowDetails(employeecode, branchId, year);
                Leavelist = Common.ExtensionMethods.ConvertToList<LeaveBalanceAsOfNow>(result);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return Leavelist;
        }

        public int UpdateLeavesBalance(Model.LeaveBalanceAsOfNow leaveBalanceAsOfNow)
        {
            
            try
            {
               int flag = empLeaveRepo.UpdateLeavesBalance(leaveBalanceAsOfNow.EmployeeCode, leaveBalanceAsOfNow.LeaveYear, Convert.ToDecimal(leaveBalanceAsOfNow.ELBal), Convert.ToDecimal(leaveBalanceAsOfNow.MLBal), Convert.ToDecimal(leaveBalanceAsOfNow.CLBal), Convert.ToDouble(leaveBalanceAsOfNow.MLExtraBal));
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
