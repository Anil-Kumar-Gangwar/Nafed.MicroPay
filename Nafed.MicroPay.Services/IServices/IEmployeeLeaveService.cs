using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System.Data;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IEmployeeLeaveService
    {
        List<EmployeeLeave> GetEmployeeLeaveList(Model.EmployeeLeave empLeave);
        bool LeaveExists(int? leaveID, DateTime DateFrom, DateTime DateTo);
        bool UpdateEmployeeLeave(Model.EmployeeLeave editEmployeeLeaveEntity);
        int InsertEmployeeLeave(Model.EmployeeLeave createEmployeeLeave);
        Model.EmployeeLeave GetEmployeeLeaveByID(int leaveID);
        bool WithdrawlLeave(int leaveID, int empleavetransID, Model.ProcessWorkFlow workFlow);
        bool LeaveDataExists(int EmployeeID, DateTime DateFrom, DateTime DateTo, int? leaveCategoryID = null);
        List<EmployeeLeave> GetEmployeeLeaveDetails(string employeecode, string year);
        bool UpdateEmployeeLeaveDocument(Model.EmployeeLeave editEmployeeLeaveEntity);
        string GetLeaveForm(int employeeID, int statusID, int leavecategoryID, string fromdate, string todate, string fileName, string sFullPath, string Name);
        List<Model.EmployeeLeave> GetUnitDetails(DateTime fromDate, DateTime toDate, int BranchID, int leavecategoryID, int EmployeeID,out string remark);
        bool InsertLeaveTrans(Model.EmployeeLeave createEmployeeLeave, int LeaveID);
        List<Model.LeaveCategory> GetLeaveCategoryGuidlines(int leaveCategoryID);
        List<Model.EmployeeLeave> checkMapLeave(int leavecategoryID, DateTime fromDate, int EmployeeID);
        EmployeeLeave GetLeaveTransDetails(int LeaveID);

        #region Export Leave Balance As Of Now
        bool ExportLeaveBalanceAsOfNow(DataSet dsSource, string sFullPath, string fileName);
        #endregion
        bool SenderSendMail(int senderID, string leaveType, EmployeeLeave empleave, string mailType);
        bool RecieverSendMail(int recieverID, string leaveType, EmployeeLeave empleave,string mailType);
        bool SendMessageOnMobile(string mobileno, string message, SMSConfiguration smssetting);

        List<Model.EmployeeLeave> chkdateforLE(int EmployeeID);

        List<EmployeeLeave> GetLeaveEncashForF_A(DateTime fromDate, DateTime toDate);
        bool ExportLeaveEncashForF_A(DataTable dtTable, string sFullPath, string fileName, string tFilter, string reportType);
        List<Model.LeaveEncashment> GetLeaveEncashment(CommonFilter cFilter);
        bool UpdateEncashmentTDS(List<Model.LeaveEncashment> model, int userId);
        List<Model.LeaveEncashment> GetDAArrearLeaveEncashment(CommonFilter cFilter);
    }
}
