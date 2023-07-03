using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IImportMonthlyInputService
    {
        string GetMonthlyInputForm(int branchID, int month, int year, string fileName, string sFullPath, int? employeeTypeId);
        //List<Model.ImportMonthlyInput> ReadImportMonthlyExcelData(string fileFullPath, bool isHeader, out string message, out int error, out int warning, out List<string> columnName);
        int ImportMonthlyInputDetails(int userID, List<Model.ImportMonthlyInput> monthlyInputDetails);
        List<Model.SalaryHead> GetHeadsFields(int employeeTypeID);
        IEnumerable<dynamic> ReadImportMonthlyExcelData(string fileFullPath, bool isHeader, out string message, out int error, out int warning, out List<Model.ImportMonthlyInput> importMonthlyInputSave, out int employeeTypeId, out List<string> missingHeaders, out List<string> columnName);

        DataTable GetMonthlyInputData(int branchID, int? employeeTypeId, int? month, int? year);
    }
}
