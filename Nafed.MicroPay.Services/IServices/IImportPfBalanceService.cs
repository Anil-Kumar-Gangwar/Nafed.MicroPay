using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IImportPfBalanceService
    {
       
        IEnumerable<dynamic> ReadImportMonthlyExcelData(string fileFullPath, bool isHeader, out string message, out int error, out int warning, out List<Model.ImportPfBalanceData> DATA, out List<string> missingHeaders, out List<string> columnName);
    }
}
