using Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IGenerateSalaryRepository
    {

        List<string> GetEmployeeMasterRecords(int salMonth, int salYear, bool branchesExcecptHO, bool allEmployees, int employeeType, int? branchID, int? employeeID);
        int NoOfMonthlyInputRows(bool branchesExcecptHO, bool allEmployees, int salMonth, int salYear, int employeeType, int? branchID, int? employeeID, out List<string> empCodes);
        int NofEmployeeSalaryRows(int salMonth, int salYear,bool BranchesExcecptHO, bool allEmployees, int employeeType, int? branchID, int? employeeID, out List<string> empCodes);
        int NoOfLastGeneratedSalaryRows(bool branchesExcecptHO, bool allEmployees, int salMonth, int salYear, int employeeType, int? branchID, int? employeeID);

        List<TblMstEmployeeSalary> GetMonthlySalaryList(int salMonth, int salYear,bool branchesExcecptHO, bool allEmployees, int employeeType, int? branchID, int? employeeID);

        List<TBLMONTHLYINPUT> GetMonthlyInputList(bool branchesExcecptHO, bool allEmployees, int salMonth, int salYear, int employeeType, int? branchID, int? employeeID);
        void UpdateLoanTransData(bool branchesExcecptHO, bool allEmployees, int salMonth, int salYear, int employeeType, int? branchID, int? employeeID);
        DataSet GetLoanPriorityMasterData(DataTable empCodeDT, string periofOfPayment);
        DataTable GetOTARates();
        bool UpdateSalaryLoanData(DataTable newLoanPriorityDT, DataTable newLoanTransDT, DataTable oldLoanPriorityDT, DataTable oldLoanTransDT);

        DataTable GetPFOpBalance(int prevYear, byte prevMonth, DataTable dtEmpCodes);

        void InsertIntoLoanPriorityHistory(string period);
        DataSet GetLoanPriorityHistoryData(DataTable empCodeDT, string periofOfPayment, string currentPeriod);
        DataTable RevertProcessedLoanEntries(bool branchesExcecptHO, bool allBranches, bool allEmployees, int salMonth, int salYear, int? branchID, int? employeeID, int employeeTypeID);
        void UpdateEmpPensionDeduction(int salMonth, int salYear, bool branchesExcecptHO, bool allEmployees, int employeeType, int? branchID, int? employeeID);
       void UpdateDateBranch_Pay(int salMonth, int salYear);

    }
}
