using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IDependentService
    {
        List<Model.EmployeeDependent> GetDependentList(int? employeeID, int? branchID=null);
        bool InsertDependentDetails(Model.EmployeeDependent employeeDependent);
        Model.EmployeeDependent GetDependentByID(int empDependentID);

        bool UpdateDependentDetails(Model.EmployeeDependent employeeDependent);

        bool DeleteDependentDetails(int empDependentID);
        int GetLastDependentCode(int employeeID);

       decimal? GetSumOfPfDistibution(int? employeeId, int? rowId);
    }
}
