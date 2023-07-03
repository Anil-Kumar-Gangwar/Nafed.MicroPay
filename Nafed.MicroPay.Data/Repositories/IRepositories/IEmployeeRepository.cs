using Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
   public interface IEmployeeRepository
    {
        IEnumerable<GetEmployeeDetails_Result> GetEmployeeList(string empName, string empCode, int? designationID, int? empTypeID);
        IEnumerable<GetDesignationPayScale_Result> GetDesignationPayScaleList(int? designationID);
        int GeneratePasswordforAllEmployees(DataTable usersDT);
        List<GetEmployeePromotions_Result> GetPromotionList(int? employeeID, int? transID);
        List<GetUnMappedEmployeeList_Result> GetUnMappedEmployees(int? branchID);

        int UpdateEmployeeDeputation(int? empDeputationID, DateTime fromDate, DateTime toDate, string organizationName, int? updatedBy);
        IEnumerable<GetSubOrdinatesDetails_Result> GetSubOrdinatesDetails(int? employeeID);
        int UpdateSeniorityCode(int oldDesignationId, int? currentDesignationId, int? employeeId);

        DataTable GetEmployeeConfirmationDetails(int? formTypeId, DateTime? fromDate, DateTime? toDate);
        DataTable GetBranchWiseSalaryConfig(int branchID);
        DataTable GetEmployeeTransferDetail(int? branchID, string employeeCode);
        List<GetSeniorityList_Result> GetSeniorityList(int employeeID, int desID);
        GetLastEmployeeCode_Result GetLastEmployeeCode(int empTypeID);
        List<GetSuperAnnuating_Result> GetSuperAnnuating();
    }
}
