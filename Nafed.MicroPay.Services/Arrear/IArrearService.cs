using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.Arrear
{
    public interface IArrearService
    {
        #region ArrearManualData
        List<ArrearManualData> GetArrearManualdata(int branchID, int employeeID, int month, int year);
        List<ArrearManualData> GetArrearManualdataList();
        List<SelectListModel> GetAllEmployee();
        List<SelectListModel> GetSalHeads();
        bool ArrearDataExists(int EmployeeID, int month, int year, string Headname);
        Model.ArrearManualData GetArreardataByID(int arrearmonthlyinputID);
        int Insertarrearmanualdata(Model.ArrearManualData createArrearmanualdata);
        int Updatearrearmanualdata(Model.ArrearManualData editArrearmanualdata);
        bool Delete(int arrearmonthlyinputID);
        string GetManualdataForm(int branchID, int month, int year, string fileName, string sFullPath);
        int ImportManualDataDetails(int userID, int userType, List<ArrearManualData> dataDetails);
        #endregion

        #region ActualDArates

        List<ArrearManualData> GetDAratesList(int month, int year);
        bool DArateexists(int month, int year, double E_01);

        Model.ArrearManualData GetDAratesByID(int ID);
        int InsertDAratesdata(Model.ArrearManualData createDAratesdatadata);
        int UpdateDAratesdata(Model.ArrearManualData editDAratesdatadata);
        bool DeleteDArates(int ID);

        #endregion

        #region SalaryManualData
        List<SalaryManualData> GetSalarymanualData(int branchID, int employeeID, int DesignationID, int month, int year);
        bool salaryDataExists(int BranchID, int EmployeeID, int month, int year, int arrearType);
        int InsertSalarymanualdata(Model.SalaryManualData createsalarymanualdata);
        Model.SalaryManualData GetSalaryManualbyID(int ID);
        int Updatesalarymanualdata(Model.SalaryManualData editsalarymanualdata);
        bool DeleteArrearmanualdata(int ID);
        #endregion

        #region Calculate DA Arrear

        DataSet GetMultipleTableDataResult(string fromYear, string toYear);
        int CalculateDAArrear(int listcount, int[] empIDs, string ArrearType, int arrPeriods, DataSet searchResult, int BranchID, DateTime fromdate, DateTime todate, int vpfRate, string formula, string orderNumber, DateTime? orderDate);
        SalaryHead GetSalaryHeadFormulaRule(string fieldName);
        List<SelectListModel> getemployeeByBranchID(int branchID, string fromDate, string toDate, string empID);

        #endregion

        #region Calculate PAY Arrear
        int CalculatePayArrear(int listcount, DataTable dtPayArrear, string ArrearType, int arrPeriods, DataSet searchResult, int BranchID, DateTime fromdate, DateTime todate, int vpfRate, string orderNumber, DateTime? orderDate);

        #endregion

        #region Export/Import Pay Arrear Details
        string GetPayArrearentryform(int branchID, string fromYear, string toYear, string fileName, string sFullPath, string empID);
        List<Model.ArrearManualData> ReadArrearImportExcelData(string fileFullPath, bool isHeader, out string message, out int error, out int warning);
        DataTable ImportPayArrearDataDetails(List<ArrearManualData> dataDetails);
        #endregion
        int getPayArrearEmployee(string branchCode, int fromYear, int toYear, string DOG);
        //List<ArrerPeriodDetails> GetArrearPeriodsDetails(string arrerType);
        List<ArrerPeriodDetailsForPAYDA> GetArrearPeriodsDetailsforPay(string arrerType);
        bool getemployeebranchDetails(int branchID, out ArrearFilters BRdetail);

        string getSelectedEmployeecode(int listcount, int[] empIDs);
    }
}
