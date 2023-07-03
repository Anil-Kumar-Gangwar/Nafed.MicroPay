using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
   public interface IPFAccountBalanceServices
    {     
        string GetPfBalanceManualEntryForm(int branchID, byte month, int year, string fileName, string sFullPath);
        int ImportPfBalanceManualData(int userID, List<ImportPfBalanceData> dataList);

        Model.EmpPFOpBalance GetEmployeePFDetail(int employeeID, int month, int year);
        Model.EmpPFOpBalance SearchEmployeePFDetail(int pfNo, int month, int year);
        bool InsertPFDetail(Model.EmpPFOpBalance empPFBal);

        int UpdateInterest(int year, decimal rate);
    }
}
