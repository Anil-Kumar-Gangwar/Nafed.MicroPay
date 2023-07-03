using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IImportBonusService
    {
        string GetBonusEntryForm(int branchID, int salMonth, int salYear, string fileName, string filePath);
        IEnumerable<dynamic> ReadImportMonthlyExcelData(string fileFullPath, bool isHeader,
          out string message, out int error, out int warning, out List<Model.ImportBounsData> DATA,
          out List<string> missingHeaders, out List<string> columnName);
        int ImportBonusManualData(int userID, List<ImportBounsData> dataList);
    }
}
