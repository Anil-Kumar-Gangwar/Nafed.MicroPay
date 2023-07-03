using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.ImportExport.Model;
using System.Data;
using System.Dynamic;

namespace Nafed.MicroPay.ImportExport.Interfaces
{
    public interface IImport : IBase
    {
        List<ExcelSheetList> GetWorksheetNames(string sFileFullPath);
        DataTable ReadExcel(string sFileFullPath, bool IsHeader, int iSheetIndex = 0, string sSheetName = "");
        DataTable ReadExcelWithNoHeader(string sFileFullPath, int iSheetIndex = 0, string sSheetName = "");
        StringBuilder ReadExcelInSB(string sFileFullPath, int iSheetIndex = 0, string sSheetName = "");
        List<ExpandoObject> ReadExcelDynamic(string sFileFullPath, int iSheetIndex = 0, string sSheetName = "");

        DataTable GetDataWithHeader(DataTable dt);
        DataTable ReadAndValidateHeader(string filePath, List<string> headerList, bool isHeader, out bool isValid, out List<string> lstMissingHeader);
    }
}
