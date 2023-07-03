using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IAttendanceImportRepository :IDisposable
    {
        DataTable GetImportFieldValues(DataTable dtEmpCodes);
        int ImportAttendanceData(int userID, DataTable attendanceDT);
    }
}
