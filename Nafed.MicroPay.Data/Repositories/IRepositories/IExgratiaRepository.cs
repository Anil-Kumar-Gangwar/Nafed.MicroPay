using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using System.Data;


namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IExgratiaRepository
    {
        int CalculateExgratia(int noofdays, string fromYear, string toYear, int branchID, string financialyear, bool isPercentage,int emptype);
         DataTable GetExgratiaExport(string fromYear, string toYear, string fyear, int branchID, int emptype);
        DataTable GetExportExgratiaTemplate(string fromYear, string toYear, string fyear, int branchID, int emptype);

        DataTable GetExgratiaEntryFormData(int branch, int month, int year);
        DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtBranch);

        int ImportExGratiaManualData(int userID, DataTable dt, int month, int year);
        int ImportExGratiaIncomeTaxData(int userID, DataTable dt, string financialYear);
        void PublishExGratia(string financialyear, int emptype, int branchID);
    }
}
