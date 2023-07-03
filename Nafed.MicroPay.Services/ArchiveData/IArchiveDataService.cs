using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Nafed.MicroPay.Model;
using System.IO;

namespace Nafed.MicroPay.Services.ArchiveData
{
    public interface IArchiveDataService
    {
        IEnumerable<ArchivedDataTransaction> GetArchivedDataTransList();
        bool ArchiveData(int userID, DateTime fromdata, DateTime todate);
        long DirectorySize(DirectoryInfo sourceDir, string destDirName, out long destDirSize);
        bool CheckArchiveExistForGivenYr(int year);
    }
}
