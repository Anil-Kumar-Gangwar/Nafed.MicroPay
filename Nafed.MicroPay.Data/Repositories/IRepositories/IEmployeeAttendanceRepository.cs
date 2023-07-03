using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;

namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IEmployeeAttendanceRepository
    {
        DataTable GetEmployeeAttendanceList(int? branchID, int? EmployeeID, string month, string Year, string Day, int reportingto, int? departmentid = null);
        IQueryable<DTOModel.EmpAttendanceDetail> GetAttendanceDetails(DTOModel.EmpAttendanceHdr tabletProxyAttendanceDetails);
        int ApproveRejectAttendance(DTOModel.EmpAttendanceHdr attendanceDetail);
        IQueryable<DTOModel.EmpAttendanceDetail> GetEmployeeAttendanceByManagerID(DTOModel.EmpAttendanceHdr tabletProxyAttendanceDetails, int? processID) ;
        DTOModel.GetOTAslipDetail_Result GetOTAslipDetail(int employeeID, int applicationID);
        int getAttendancemanually(DateTime fromdate, DateTime todate);
        IQueryable<DTOModel.EmpAttendanceDetail> GetTourDetails(DTOModel.EmpAttendanceHdr empAtnd,int branchID);
        DataTable GetEmpAttendanceInTimeOutTime(int branchID, int? employeeID, string month, string year);

    }
}
