using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IImportAttendanceService
    {
        List<Model.EmpAttendanceForm> ReadAttendanceImportExcelData(string fileFullPath, bool isHeader, out string message, out int error, out int warning);
        int ImportAttendanceDetails(int userID, int userType, List<EmpAttendanceForm> attendanceDetails);
    }
}
