using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface INonRegularEmployeesService
    {
        string GetEmployeeProfilePath(string empCode);
        bool EmployeeDetailsExists(int iD, string value);
        int InsertEmployeeDetails(Model.Employees.NonRegularEmployee employeeDetails);
        bool UpdatetEmployeePersonalDetails(Model.Employees.NonRegularEmployee employeeDetails);
        List<Model.Employees.NonRegularEmployee> GetEmployeeList(string empName, string empCode, int? designationID, int? empTypeID);
        Model.Employees.NonRegularEmployee GetEmployeeByID(int employeeID);
        bool UpdateEmployeeGeneralDetails(Model.Employees.NonRegularEmployee employee);
        bool DeleteEmployee(int employeeID);
        Model.EmployeeProfile GetEmployeeProfile(int employeeID);
        bool CheckEmployeeCodeExistance(string empCode);
        List<Model.Employees.NonRegularEmployee> GetEmployeeDetailsByCode(string empCode);
        List<Model.Employees.NonRegularEmployee> GetEmployeesByBranchID(int? branchID);
        EmployeeQualification GetQualificationDetail(int employeeID);
        Model.EmployeeSalary GetEmployeeSalaryDtls(string empCode);
        Model.Employees.NonRegularEmployee GetEmployeePaymentModeDtls(string empCode);
        Model.Employees.NonRegularEmployeesExtension GetEmployeeContractExtension(int employeeID,int id);
        List<Model.Employees.NonRegularEmployeesExtension> GetEmployeeContractExtensionList(int employeeID);
        bool InsertUpdateContractExtension(Model.Employees.NonRegularEmployeesExtension model);
        bool DeleteContractExtension(int id);
    }
}
