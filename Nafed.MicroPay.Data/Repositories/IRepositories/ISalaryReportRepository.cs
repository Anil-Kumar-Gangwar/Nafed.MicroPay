using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface ISalaryReportRepository
    {

        DataSet GetMonthlyBranchWiseReport(int salMonth, int salYear, int employeeTypeID, int? branchID, out int nE_Cols, out int nD_Cols);
        DataSet GetMonthlyEmployeeWiseReport(int salMonth, int salYear, int? employeeTypeID, int? branchID, out int nE_Cols, out int nD_Cols);
        DataSet GetEmployeeWiseAnnualReport(int financialFrom, int financialTo, int? employeeTypeID, int? employeeId, int? branchId, out int nE_Cols, out int nD_Cols);
        DataSet GetBranchWiseAnnualReport(int financialFrom, int financialTo, int? employeeTypeID, int? branchId, out int nE_Cols, out int nD_Cols);
        DataSet GetMonthlyEmployeeWisePaySlip(int salMonth, int salYear, int employeeTypeID, int? branchID, out int nE_Cols, out int nD_Cols);
        DataSet GetBranchEmployeeWisePaySlip(int salMonth, int salYear, int employeeTypeID, int? branchID, out int nE_Cols, out int nD_Cols);

        DataTable GetEmpDtlWithBankAccount(int salMonth, int salYear, int employeeTypeID, string bankCode);

        Models.SP_EmpWiseSalaryReport_Result GetSalarySlip(byte salMonth, short salYear, int employeeID);
    }
}
