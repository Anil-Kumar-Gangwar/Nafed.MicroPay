using Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using System.Data;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
   public interface IIncrementRepository
    {
        int InsertProjectedEmployee();
        IEnumerable<GetProjectedIncrementDetails_Result> GetProjectedIncrementDetails(int? BranchID, string EmployeeCode, string EmployeeName, int? incrementMonthId);
        bool UpdateProjectedEmployeeSalaryDetails(List<DTOModel.tblmstProjectedEmployeeSalary> lstProjectedEmployeeSalary);
        bool UpdateStopIncrementDetails(List<DTOModel.tblMstEmployee> lstStopIncrementEmployeeDetails);
        IEnumerable<GetValidateNewBasicAmountDetails_Result> GetValidateNewBasicAmountDetails(int? employeeId, int? incrementMonth);

        bool UpdateEmployeeSalaryDetails(List<DTOModel.TblMstEmployeeSalary> employeeSalaryDetails);
        IEnumerable<GetUpdateIncrementDetails_Result> GetUpdateIncrementDetails(int? BranchID, string EmployeeCode, string EmployeeName, int? incrementMonthId);
        IEnumerable<GetUpdateValidateNewBasicAmountDetails_Result> GetUpdateValidateNewBasicAmountDetails(int? employeeId, int? incrementMonth);

        DataTable GetExportApplicableIncrement(int? branchId, int? incrementMonth, string employeeName, string employeeCode, string fileName, string fullPath, string type);
    }
}
