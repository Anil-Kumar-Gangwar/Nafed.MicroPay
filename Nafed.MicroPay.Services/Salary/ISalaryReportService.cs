using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.Salary
{
    public interface ISalaryReportService 
    {
        string GetMonthlyBranchWiseReport(SalaryReportFilter rFilter);
        string GetMonthlyEmployeeWiseReport(SalaryReportFilter rFilter);
        bool GetEmployeeWiseAnnualReport(SalaryReportFilter rFilter);
        bool GetBranchWiseAnnualReport(SalaryReportFilter rFilter);
        string GetMonthlyEmployeeWisePaySlip(SalaryReportFilter rFilter);
        string GetBranchEmployeeWisePaySlip(SalaryReportFilter rFilter);
        bool ExportEmpDtlWithBankAccount(MasterReports rFilter, string sFullPath, string fileName);
        SalarySlipModel GetSalarySlip(SalaryReportFilter rFilter);
        bool CheckSalaryReportPublish(int salYear, int salMonth, int empId);
    }
}
