using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
   public interface IMarkAttendance
    {
        int InsertTabletMarkAttendanceDetails(Model.EmpAttendance tabletProxyAttendanceDetails,int userTypeID);
        int InsertMarkAttendanceDetails(Model.EmpAttendance tabletProxyAttendanceDetails);

        List<Model.EmpAttendance> AttendanceDetails(Model.EmpAttendance tabletProxyAttendanceDetails);
        string GetAttendanceForm(int branchID, string fileName, string sFullPath, int? employeeType, DateTime date);       
        int ApproveRejectAttendance(Model.EmpAttendance attendanceDetail);
        bool AttendanceExists(int branchID, int empID, DateTime inDate, DateTime outDate);
        int InsertTourDetail(Model.EmpAttendance attendanceDetails);
        List<Model.EmpAttendance> GetTourDetails(Model.EmpAttendance empAttend);
        bool ExportTourDetail(System.Data.DataTable dtTable, string sFullPath, string fileName, string tFilter);

    }
}
