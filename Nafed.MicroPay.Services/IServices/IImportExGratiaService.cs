using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IImportExGratiaService
    {
        IEnumerable<dynamic> ReadImportMonthlyExcelData(string fileFullPath, bool isHeader,
            out string message, out int error, out int warning, out List<Model.ImportExgratiaData> DATA,
            out List<string> missingHeaders, out List<string> columnName);
        IEnumerable<dynamic> ReadImportExGratiaIncomeTaxExcelData(string fileFullPath, bool isHeader,
            out string message, out int error, out int warning, out List<Model.ImportExgratiaIncomeTaxData> DATA,
            out List<string> missingHeaders, out List<string> columnName);
        int ImportExGratiaManualData(int userID, List<ImportExgratiaData> dataList);
        int ImportExGratiaIncomeTaxData(int userID, List<ImportExgratiaIncomeTaxData> dataList);
    }
}
