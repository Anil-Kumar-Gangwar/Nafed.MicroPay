using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.Salary
{
    public interface IRegularEmpSalaryService
    {
        string GetMonthlyFileName(string tcsFilePeriod);
        RegularEmployeeSalary DataInputsValidation(RegularEmployeeSalary regEmpSalary);
        RegularEmployeeSalary SendApprovalRequest(RegularEmployeeSalary regEmpSalary);
        RegularEmployeeSalary RevertProcessedLoanEntries(RegularEmployeeSalary regEmpSalary);
        List<FinalMonthlySalary> GeneratedSalaryList(int salYear);
        FinalMonthlySalary GetEmployeeSalary(int empId, int salYear, int salMonth);
        Task<bool> UpdateEmployeeSalary(FinalMonthlySalary empSalary);
        void UpdateDateBranch_Pay(int salMonth, int salYear);
    }
}
