using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IArchiveDataRepository
    {
        DataTable GetArchivedDataTransList();
        bool ArchiveData(int userID, DateTime fromdata, DateTime todate, Dictionary<int, int> indexs);
        bool CheckArchiveExistForGivenYr(int year);
    }
}
