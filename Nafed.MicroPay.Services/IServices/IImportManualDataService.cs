using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nafed.MicroPay.Services.IServices
{
    public interface IImportManualDataService
    {
        List<Model.ArrearManualData> ReadAttendanceImportExcelData(string fileFullPath, bool isHeader, out string message, out int error, out int warning);
    }
}
