using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using System.Data;
using System.Data.SqlClient;

namespace Nafed.MicroPay.Data.Repositories
{
   public class ConveyanceRepository : BaseRepository, IConveyanceRepository
    {
        public List<ConveyanceBillHdr> EmployeeSelfConveyanceList(int userEmpID, string empCode, string empName)
        {
            return db.ConveyanceBillHdrs.AsNoTracking().Where(x => x.EmployeeId == userEmpID
             && x.tblMstEmployee.EmployeeProcessApprovals.Any(y => y.EmployeeID == userEmpID && y.ProcessID == 15 && y.ToDate == null)
           ).ToList();
        }

        public List<GetConveyanceFormDetails_Result> GetConveyanceBillDetail(int employeeID, int conveyanceDetailID)
        {
            return db.GetConveyanceFormDetails(employeeID, conveyanceDetailID).ToList();
        }

        public List<GetConveyanceBillDescription_Result> GetConveyanceBillDescription(int employeeId, int conveyanceBillDetailID)
        {
            return db.GetConveyanceBillDescription(employeeId, conveyanceBillDetailID).ToList();
        }

        public DataTable GetConveyanceBillDetails(string selectedReportingYear, int selectedEmployeeID, int? statusId, string procName)
        {
            log.Info("ConveyanceRepository/GetConveyanceBillDetails");
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
                dbSqlCommand.CommandText = procName;
                dbSqlCommand.Parameters.Add("@ReportingYear", SqlDbType.VarChar).Value = selectedReportingYear;
                dbSqlCommand.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = selectedEmployeeID;
                dbSqlCommand.Parameters.Add("@Status", SqlDbType.Int).Value = statusId;
                dbSqlDataTable = new DataTable();
                DataSet ds = new DataSet();
                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
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

       public DataTable GetConveyanceEmployeeHistory(string fromDate, string toDate, int? employeeId)
        {
            log.Info("ConveyanceRepository/GetConveyanceEmployeeHistory");
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
                dbSqlCommand.CommandText = "GetConveyanceEmployeeHistory";
                dbSqlCommand.Parameters.Add("@FromDate", SqlDbType.VarChar).Value = fromDate == "" ? null : fromDate;
                dbSqlCommand.Parameters.Add("@ToDate", SqlDbType.VarChar).Value = toDate == "" ? null : toDate;
                dbSqlCommand.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employeeId;
                dbSqlDataTable = new DataTable();
                DataSet ds = new DataSet();
                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
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
