using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IDATdsRepository
    {
        DataTable GetDATDSManualEntryFormData(int branch, int month, int year);
        DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtBranch);
        int ImportDATDSManualData(int userID, DataTable dt, int month, int year);
    }
}
