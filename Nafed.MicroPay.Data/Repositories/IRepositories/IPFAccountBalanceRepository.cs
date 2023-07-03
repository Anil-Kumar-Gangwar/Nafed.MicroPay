using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IPFAccountBalanceRepository
    {
        DataTable GetPfBalanceManualEntryFormData(int branch, byte month, int year);
        DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtPfNo, DataTable dtBranch);
        int ImportPfBalanceManualData(int userID, DataTable dt, int month, int year);
        void UpdatePfNo(int employeeID, int pfNo);
        void UpdateEmpAccountDetail(int eID, int? pfNo, string uan, string epfo, string pan, string aadhar, string ac, string bankCode, string ifscCode, int userID);
        DataTable GetUnAssignedPFRecords(int? branchID);

        int UpdateInterest(int year, decimal rate);
    }
}
