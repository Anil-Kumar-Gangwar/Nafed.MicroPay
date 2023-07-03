using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Data.Entity;
using System.Linq;

namespace Nafed.MicroPay.Data.Repositories
{

    public class EmployeeAttendanceRepository : BaseRepository, IEmployeeAttendanceRepository
    {
        public IQueryable<EmpAttendanceDetail> GetAttendanceDetails(EmpAttendanceHdr tblAttendanceDtls)
        {
            log.Info("EmployeeAttendanceRepository/AttendanceDetails");
            IQueryable<EmpAttendanceDetail> result = null;
            result = db.EmpAttendanceDetails.AsNoTracking().Where(x => !x.IsDeleted && x.Mode != "T" && x.EmployeeId ==
           tblAttendanceDtls.EmployeeId && (default(DateTime) == tblAttendanceDtls.ProxydateIn ? (1 > 0) :
          (DbFunctions.TruncateTime(x.ProxydateIn) >= DbFunctions.TruncateTime(tblAttendanceDtls.ProxydateIn)
          && DbFunctions.TruncateTime(x.ProxyOutDate) <= DbFunctions.TruncateTime(tblAttendanceDtls.ProxyOutDate))));


            //var result = (from tablet in db.EmpAttendanceHdr
            //              join tblproxy in db.EmpAttendanceDetail on new { tablet.EmployeeId, tablet.EmpAttendanceID }
            //              equals new { tblproxy.EmployeeId, tblproxy.EmpAttendanceID }
            //              select new
            //              {
            //                  tablet.Attendancestatus,
            //                  tablet.EmployeeId,
            //                  tblproxy.EmpAttendanceID,
            //                  tblproxy.ProxydateIn,
            //                  tblproxy.ProxyOutDate,
            //                  tblproxy.InTime,
            //                  tblproxy.OutTime,
            //                  tblproxy.Remarks,
            //                  tblproxy.IsDeleted,
            //                 tblproxy.EmpAttendanceHdr
            //              }).ToList().OrderBy(x => x.ProxydateIn).Where(x => !x.IsDeleted && x.EmployeeId ==
            //   tabletProxyAttendanceDetails.EmployeeId &&
            //  (DbFunctions.TruncateTime(x.ProxydateIn) >= DbFunctions.TruncateTime(tabletProxyAttendanceDetails.ProxydateIn)
            //  || DbFunctions.TruncateTime(x.ProxyOutDate) <= DbFunctions.TruncateTime(tabletProxyAttendanceDetails.ProxyOutDate)));
            return result;
        }

        public DataTable GetEmployeeAttendanceList(int? branchID, int? EmployeeID, string month, string Year, string Day, int reportingto,int ? departmentid=null)
        {
            log.Info("EmployeeAttendanceRepository/GetEmployeeAttendanceList");
            DataTable dt = new DataTable();
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "GetEmployeeAttendanceReport";
                dbSqlCommand.Parameters.Add("@month", SqlDbType.VarChar).Value = month;
                dbSqlCommand.Parameters.Add("@year", SqlDbType.VarChar).Value = Year;
                dbSqlCommand.Parameters.Add("@day", SqlDbType.VarChar).Value = Day;
                dbSqlCommand.Parameters.Add("@BranchId", SqlDbType.Int).Value = branchID;
                dbSqlCommand.Parameters.Add("@Employeeid", SqlDbType.Int).Value = EmployeeID;
                dbSqlCommand.Parameters.Add("@reportingto", SqlDbType.Int).Value = reportingto;
                dbSqlCommand.Parameters.Add("@departmentId", SqlDbType.Int).Value = departmentid;
                dbSqlDataTable = new DataTable();
                DataSet ds = new DataSet();
                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                dbSqlAdapter.SelectCommand.CommandTimeout = 9999;
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return dbSqlDataTable;
        }

        public int ApproveRejectAttendance(EmpAttendanceHdr attendanceDetail)
        {
            int result = 0;
            log.Info("EmployeeAttendanceRepository/ApproveRejectAttendance");
            try
            {
                var tablet = db.EmpAttendanceHdrs.Where(x => x.EmpAttendanceID == attendanceDetail.EmpAttendanceID && x.EmployeeId == attendanceDetail.EmployeeId).FirstOrDefault();
                if (tablet != null)
                {
                    tablet.Attendancestatus = attendanceDetail.Attendancestatus;
                    tablet.ReportingToRemark = string.IsNullOrEmpty(attendanceDetail.ReportingToRemark) == true ? tablet.ReportingToRemark : attendanceDetail.ReportingToRemark;
                    tablet.ReviewerToRemark = string.IsNullOrEmpty(attendanceDetail.ReviewerToRemark) == true ? tablet.ReviewerToRemark : attendanceDetail.ReviewerToRemark;
                    tablet.AcceptanceAuthRemark = string.IsNullOrEmpty(attendanceDetail.AcceptanceAuthRemark) == true ? tablet.AcceptanceAuthRemark : attendanceDetail.AcceptanceAuthRemark;
                    result = db.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public IQueryable<EmpAttendanceDetail> GetEmployeeAttendanceByManagerID(EmpAttendanceHdr tblAttendanceDtls, int? processID = 4)
        {
            IQueryable<EmpAttendanceDetail> result = null;

            result = db.EmpAttendanceDetails.AsNoTracking().Where(x => !x.IsDeleted && x.Mode != "T" &&
            (x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.ProcessID == processID && y.ToDate == null &&
            (y.ReportingTo == tblAttendanceDtls.EmployeeId || y.ReviewingTo == tblAttendanceDtls.EmployeeId
            || y.AcceptanceAuthority == tblAttendanceDtls.EmployeeId))
                    )
                     && (default(DateTime) == tblAttendanceDtls.ProxydateIn ? (1 > 0) :
                     (DbFunctions.TruncateTime(x.ProxydateIn) >= DbFunctions.TruncateTime(tblAttendanceDtls.ProxydateIn)
                     && DbFunctions.TruncateTime(x.ProxyOutDate) <= DbFunctions.TruncateTime(tblAttendanceDtls.ProxyOutDate))));

            return result;
        }

        public GetOTAslipDetail_Result GetOTAslipDetail(int employeeID, int applicationID)
        {
            return db.GetOTAslipDetail(employeeID, applicationID).FirstOrDefault();
        }

        public int getAttendancemanually(DateTime fromdate, DateTime todate)
        {
            SqlConnection dbSqlconnection;
            dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
            try
            {
                SqlCommand dbSqlCommand;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "getAttendancemanually";

                dbSqlCommand.Parameters.Add("@fromdate", SqlDbType.DateTime).Value = fromdate;
                dbSqlCommand.Parameters.Add("@todate", SqlDbType.DateTime).Value = todate;

                SqlParameter output = dbSqlCommand.Parameters.AddWithValue("@output", SqlDbType.Int);


                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                output.Direction = ParameterDirection.Output;
                int res = dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();
                return res = Convert.ToInt32(output.Value);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            finally
            {
                dbSqlconnection.Close();
            }
        }

        public IQueryable<EmpAttendanceDetail> GetTourDetails(EmpAttendanceHdr empAtnd, int branchID)
        {
            log.Info("EmployeeAttendanceRepository/GetTourDetails");

            IQueryable<EmpAttendanceDetail> result = null;
            result = db.EmpAttendanceDetails.AsNoTracking().Where(x => !x.IsDeleted && x.Mode == "T"
            && (branchID != 0 ? x.BranchID == branchID : (1 > 0))
            && (empAtnd.EmployeeId != 0 ? x.EmployeeId == empAtnd.EmployeeId : (1 > 0)) && (default(DateTime) == empAtnd.ProxydateIn ? (1 > 0) :
          (DbFunctions.TruncateTime(x.ProxydateIn) >= DbFunctions.TruncateTime(empAtnd.ProxydateIn)
          && DbFunctions.TruncateTime(x.ProxyOutDate) <= DbFunctions.TruncateTime(empAtnd.ProxyOutDate))));
            return result;
        }


        public DataTable GetEmpAttendanceInTimeOutTime(int branchID, int? employeeID, string month, string year)
        {
            log.Info("EmployeeAttendanceRepository/GetEmpAttendanceInTimeOutTime");
            log.Info($"EmployeeAttendanceRepository/month:{month},year:{year}");

            if (employeeID == 0)
                employeeID = null;
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "GetEmployeeAttendanceInTimeOutTime";
                dbSqlCommand.Parameters.Add("@month", SqlDbType.VarChar).Value = month;
                dbSqlCommand.Parameters.Add("@year", SqlDbType.VarChar).Value = year;

                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;
                dbSqlCommand.Parameters.Add("@employeeID", SqlDbType.Int).Value = employeeID;

                dbSqlDataTable = new DataTable();
              //  DataSet ds = new DataSet();
                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                dbSqlAdapter.SelectCommand.CommandTimeout = 9999;
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return dbSqlDataTable;
        }

    }
}
