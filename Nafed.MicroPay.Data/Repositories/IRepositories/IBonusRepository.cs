using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using System.Data;


namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IBonusRepository
    {
        int calculateBonus(decimal bonusrate, string fromYear, string toYear, int branchID, int financialyear, int empType);
        DataTable GetBonusExport(string fromYear, string toYear, int fyear, int branchID, int emptype);

        DataTable GetBonusEntryFormData(int branch, int month, int year);

        DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtBranch);

        int ImportBonusManualData(int userID, DataTable dt, int month, int year);
    }
}
