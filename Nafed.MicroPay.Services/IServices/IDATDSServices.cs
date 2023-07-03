using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IDATDSServices
    {

        string GetDATDSTemplate(int branchID, byte month, int year, string fileName, string sFullPath);

        int ImportDATDSManualData(int userID, List<ImportDATDSData> dataList);
    }
}
