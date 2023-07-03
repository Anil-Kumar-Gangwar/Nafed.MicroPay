using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using System.Data;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
   public interface IConveyanceRepository
    {
        List<ConveyanceBillHdr> EmployeeSelfConveyanceList(int userEmpID, string empCode, string empName);
        List<GetConveyanceFormDetails_Result> GetConveyanceBillDetail(int employeeID, int conveyanceDetailID);
        List<GetConveyanceBillDescription_Result> GetConveyanceBillDescription(int employeeId, int conveyanceBillDetailID);
        DataTable GetConveyanceBillDetails(string selectedReportingYear, int selectedEmployeeID, int? statusId, string procName);
        DataTable GetConveyanceEmployeeHistory(string fromDate, string toDate, int? employeeId);
    }
}
