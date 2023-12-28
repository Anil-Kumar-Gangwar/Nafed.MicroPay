using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.Increment
{
    public interface IIncrement
    {
        #region Projected Increment
        int InsertProjectedEmployee();
        List<Model.ProjectedIncrement> GetProjectedIncrementDetails(int? BranchID, string EmployeeCode, string EmployeeName, int? incrementMonthId);

        bool UpdateProjectedEmployeeSalaryDetails(List<Model.ProjectedIncrement> projectedIncrement);

        List<ValidateNewBasicAmount> GetValidateNewBasicAmountDetails(int? employeeId, int? incrementMonth);

        List<Model.StopIncrement> GetStopIncrementDetails(int?[] incrmentMonth, bool validateincrement);
        bool UpdateStopIncrementDetails(List<Model.StopIncrement> stopIncrementDetails);

        string GetLastIncrementMonthDetails(int? incrementMonthId);

        bool InsertUpdateLastIncrementDetails(Model.LastIncrementMonth lastincMonth, int userID);
        int GetMaxRankDesignation();

        #endregion

        bool UpdateEmployeeSalaryDetails(List<Model.ProjectedIncrement> projectedIncrement);
        List<Model.ProjectedIncrement> GetUpdateIncrementDetails(int? BranchID, string EmployeeCode, string EmployeeName, int? incrementMonthId);
        List<ValidateNewBasicAmount> GetUpdateValidateNewBasicAmountDetails(int? employeeId, int? incrementMonth);

        string ExportIncrement(int? branchId, int? incrementMonth, string employeeName, string employeeCode, string fileName, string fullPath, string type);
        string GetDesignationName(int designationId);
    }
}
