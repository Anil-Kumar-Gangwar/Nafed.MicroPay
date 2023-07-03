using Nafed.MicroPay.Model;
using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using dtoModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Services.IServices
{
   public interface ILeaveService
    {
        #region Leave Category
        List<Model.LeaveCategory> GetLeaveCategoryList();
        int LeaveCategoryExists(Model.LeaveCategory createLeaveCategoryDetails, out int exists);

        int InsertLeaveCategoryDetails(Model.LeaveCategory createLeaveCategoryDetails);

        Model.LeaveCategory GetLeaveCategoryByID(int leaveCategoryID);

        bool UpdateLeaveCategoryDetails(Model.LeaveCategory createLeaveCategoryDetails);

        bool DeleteLeaveCategory(int leaveCategoryId);
        #endregion

        #region Leave Rule
        List<Model.LeaveRule> GetLeaveRuleList();

        bool LeaveRuleExists(int leaveRuleID, int leaveCategoryID);

        int InsertLeaveRuleDetails(Model.LeaveRule createLeaveCategoryDetails);

        Model.LeaveRule GetLeaveRuleByID(int leaveRuleID);

        bool UpdateLeaveRuleDetails(Model.LeaveRule createLeaveRuleDetails);

        bool DeleteLeaveRule(int leaveRuleId);
        #endregion

        #region Leave Credit Rule
        List<Model.LeaveCreditRule> GetLeaveCreditList();

        int LeaveCreditRuleExists(Model.LeaveCreditRule createLeaveCreditRule, out int exists);

        int InsertLeaveCreditRule(Model.LeaveCreditRule createLeaveCreditRule);

        Model.LeaveCreditRule GetLeaveCreditRuleByID(int leaveCreditRuleID);

        bool UpdateLeaveCreditRuleDetails(Model.LeaveCreditRule createLeaveCreditRule);

        bool DeleteLeaveCreditRule(int leaveCreditRuleID);
        #endregion

        #region Leave Type 
        List<Model.LeaveType> GetLeaveTypeList();

        int LeaveTypeExists(Model.LeaveType leaveType, out int exists);

        int InsertLeaveTypeDetails(Model.LeaveType createLeaveType);

        Model.LeaveType GetLeaveTypeByID(int leaveTypeID);

        bool UpdateLeaveTypeDetails(Model.LeaveType updateLeaveType);

        bool DeleteLeaveType(int leaveTypeID);
        #endregion

        #region Leave Purpose
        List<Model.LeavePurpose> GetleavePurposeList();

        bool LeavePurposeExists(int leavePurposeID, string leavePurposeName);

        int InsertLeavePurposeDetails(Model.LeavePurpose createLeavePurpose);

        Model.LeavePurpose GetLeavePurposeByID(int leavePurposeID);

        bool UpdateLeavePurposeDetails(Model.LeavePurpose updateLeavePurpose);

        bool DeleteLeavePurpose(int leavePurposeID);
        #endregion

        #region Leave Accrued
        
        List<LeaveAccruedDetails> GetEmployeeLeaveAccruedList(Model.LeaveAccruedDetails leaveaccrued);
        List<LeaveAccruedDetails> GetFillDropdown(string flag, int leavecategoryId);
        List<LeaveAccruedDetails> GetRegularEmployee();
        List<LeaveAccruedDetails> GetELMLAccumulationdata(string EmpCode, int prevYear, int curryear, string flag);
        int ELAccumulation(Model.LeaveAccruedDetails leaveaccrued, int userID);
        List<LeaveAccruedDetails> GetLeavevalidatemonth(Model.LeaveAccruedDetails leaveaccrued);
        int PuttingMedicalLeave(Model.LeaveAccruedDetails leaveaccrued, int userID);
        int CLAccumulation(Model.LeaveAccruedDetails leaveaccrued, int userID);
        #endregion
        IList<Model.EmployeeLeaveBalance> GetEmployeeLeaveBal(string empCode);
        IList<Model.EmployeeLeave> GetLeaveApplied(int reportingTo,out int leaveCount);

        bool UpdateLeaveStatus(EmployeeLeave empObj, int UserID);
        bool UpdateLeaveTransStatus(EmployeeLeave empLeave, int UserID);       
        bool SenderSendMail(int senderID, string leaveType, EmployeeLeave empleave, string mailType);
        bool RecieverSendMail(int recieverID, string leaveType, EmployeeLeave empleave, string mailType);
        bool SendMessageOnMobile(string mobileNo, string message);
    }
}
