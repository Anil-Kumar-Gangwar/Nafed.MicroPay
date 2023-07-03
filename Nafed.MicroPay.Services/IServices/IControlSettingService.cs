using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices 
{
    public interface IControlSettingService
    {
        List<MonthlyTCSFile> GetMonthlyTCSFiles();
        int InsertMonthlyTCSFile(MonthlyTCSFile mTCSFile);
        bool UpdateMonthlyTCSFile(MonthlyTCSFile mTCSFile);
        bool IsMonthlyTCSFileExists(string tcsFilePeriod);
        bool ReadTCSFileData(string fileFullPath, bool isHeader, out List<string> missingHeaders,out List<string> invalidColumns, out string message, out int error, out int warning);



    }
}
