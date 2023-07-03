using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
   public interface IDataImportRepository
    {
        DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtBranchCodes);
        int ImportMonthlyInputData(int userID, DataTable monthlyInputDT, int month, int year, int branchId);
        DataTable GetMonthlyInputData(int branchID, int? employeeTypeId, int? month, int? year);
    }
}
