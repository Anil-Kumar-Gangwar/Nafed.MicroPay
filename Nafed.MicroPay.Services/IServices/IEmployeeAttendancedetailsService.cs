using System.Collections.Generic;
using System.Data;
using System;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IEmployeeAttendancedetailsService
    {
        // List<Model.EmployeeAttendanceDetails> GetEmployeeAttendanceList(int? branchID, int? reportingTo, string Employeename, string month, string Year, string Day);

        List<Model.EmpAttendance> GetAttendanceDetails(Model.EmpAttendance tabletProxyAttendanceDetails);

        List<Model.EmployeeAttendanceDetails> GetEmployeeAttendanceList(int ? branchId, int ? employeeID, string month, string year, int usertypeID, int EmployeeID, int? departmentId=null);
        List<Model.EmpAttendance> GetEmployeeAttendanceByManagerID(Model.EmpAttendance tabletProxyAttendanceDetails, out int attendanceCount);
        List<Model.EmployeeAttendanceDetails> GetReportingTo(int ReportingTo);
        bool ExportAttendance(DataSet dsSource, string sFullPath, string fileName);
        List<Model.MyAttendanceDetails> GetMyAttendanceList(int branchId, int employeeID, string month, string year, int usertypeID, int LoggedInEmployeeID);
        int getAttendancemanually(System.DateTime fromdate, System.DateTime todate);

    }
}
