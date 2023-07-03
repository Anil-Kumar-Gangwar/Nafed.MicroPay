using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IEmpApprovalProcessService
    {
                

        List<Model.Employee> GetEmployeeList(int? branchID, int? employeeID);
        List<EmployeeProcessApproval> GetConfirmationApprovalProcesses(int employeeID, int[] processList);
        bool InsertMultiProcessApproval(List<Model.EmployeeProcessApproval> empProcessApproval);
        bool UpdateMultiProcessApproval(List<Model.EmployeeProcessApproval> empProcessApproval);
        bool IsProcessStarted(int empProcessAppID, int empID);
    }
}
