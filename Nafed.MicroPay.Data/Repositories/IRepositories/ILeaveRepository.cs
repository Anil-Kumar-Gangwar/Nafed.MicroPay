using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nafed.MicroPay.Data.Models;


namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface ILeaveRepository
    {
        DataTable GetEmployeeLeaveAccruedList(int LeaveCategoryID, string Month, string Year);
        DataTable GetFillDropdown(string Flag, int leavecategoryId);
        DataTable GetLeavevalidatemonth(int leavecategoryId, int month, int year);
        DataTable GetRegularEmployee();
        DataTable UpdateCLAccured(DateTime DOJ,string EmpCode, int prevYear, int curryear, int month,int userID);
        DataTable UpdateELAccured(DateTime DOJ, string EmpCode, int prevYear, int curryear, int month, int userID);
        DataTable GetELMLAccumulationdata(string EmpCode, int prevYear, int curryear, string flag);
        //   DataTable UpdateELMLCreaditLeaves(string EmpCode, string prevYear, double currELdays, double dblOpBal);
        bool AddLeaveBalance(tblLeaveBal leaveBalance);
        bool AddLeaveTrans(tblLeaveTran leaveTrans);
        bool AddLeaveUpdate(tblLeaveUpdate leaveUpdate);
        bool AddMedicalLeaveUpdate(tblMedicalLeaveUpdate MedicalleaveUpdate);
        IEnumerable<GetEmployeeLeaveBal_Result> GetEmployeeLeaveBal(string empCode);
        IEnumerable<EmployeeLeave> GetLeaveApplied(int reportingTo);
        DataTable GetEmployeeLeaveDetails(string employeecode, string year);
        DataTable getUnitDetails(DateTime fromDate, DateTime toDate, int BranchID, int leavecategoryID, int EmployeeID);
        DataTable checkMapLeave(int leavecategoryID, DateTime fromDate, int EmployeeID);
        DataTable GetLeaveBalanceAsOfNowDetails(string employeecode, string branchId, string year);
        int UpdateLeavesBalance(string EmployeeCode, string LeaveYear, decimal? ELBal, decimal? MLBal, decimal? CLBal, Double? MLExtraBal);
        DataTable chkdateforLE(int EmployeeID);
        List<SP_GetEncashmentForF_A_Result> GetLeaveEncashForF_A(DateTime fromDate, DateTime toDate);
    }
}
