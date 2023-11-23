using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IArrearRepository : IDisposable
    {
        DataTable GetImportFieldValues(DataTable dtEmpCodes);
        int ImportManualData(int userID, DataTable DT);

        #region Calculate DA Arrear

        DataTable GetMultipleTableDataResult(string fromYear, string toYear, string Flag);
        DataTable GetDatatableResult(string HeadValue);
        DataTable DeleteDuplicateData(int empID, string fromYear, string toYear, int BranchID, int flag, string arrearType);
        DataTable TransferarreardetailinFinalmonthlysalary(int empID, string fromYear, string toYear, int BranchID);
        int UpdateTblArreardummy(DataTable DTarreardummy);
        int UpdateTblArrearDetail(DataTable DTarreardetail);
        void UpdateWorkingDayslwp();
        void Calculateperiod();
        int UpdateTransportDetails(int empID, double newta, int month, int year, string fromYear, string toYear);

        #endregion

        List<getemployeeByBranchID_Result> getemployeeByBranchID(int branchID, string fromDate, string toDate, string empID);
        int updatePayArrearDifference(int empID, string fromYear, string toYear, int BranchID, string DOG);

        int CreatePayArrearDetail(string fROMPERIOD, string tOPERIOD, string arrearType, string eMP, string brcode, string generateDate);

        DataTable GetArrearPeriodsDetailsforPay(string arrerType, int? branchId);

        DataTable GetArrearReport(dynamic rFilter);
      void UpdateDateBranch_Pay(int salMonth, int salYear);
        bool UPdateOrderNumberDate(string DOG, string orderNumber, DateTime? orderDate);
    }
}
