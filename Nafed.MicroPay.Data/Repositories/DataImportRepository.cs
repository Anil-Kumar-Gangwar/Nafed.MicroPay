using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Nafed.MicroPay.Data.Repositories
{
    public class DataImportRepository : BaseRepository, IDataImportRepository
    {
        public DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtBranchCodes)
        {
            DataSet dbSqlDataTable = new DataSet();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[GetImportMonthlyInputFieldDtls]";

                SqlParameter paramItemCode = dbSqlCommand.Parameters.AddWithValue("@tblEmpCode", dtEmpCodes);
                paramItemCode.SqlDbType = SqlDbType.Structured;
                paramItemCode.TypeName = "[udtGenericStringList]";

                SqlParameter paramBranchCode = dbSqlCommand.Parameters.AddWithValue("@tblBranchCode", dtBranchCodes);
                paramBranchCode.SqlDbType = SqlDbType.Structured;
                paramBranchCode.TypeName = "[udtBranchCodeList]";

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();

                return dbSqlDataTable;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int ImportMonthlyInputData(int userID, DataTable monthlyInputDT, int month, int year, int branchId)
        {
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[ImportMonthlyInputData]";

                SqlParameter paramAttendanceDT = dbSqlCommand.Parameters.AddWithValue("@monthlyInputDT", monthlyInputDT);
                paramAttendanceDT.SqlDbType = SqlDbType.Structured;
                paramAttendanceDT.TypeName = "[tbl_monthlyInput]";

                SqlParameter paramuserID = dbSqlCommand.Parameters.AddWithValue("@userID", userID);
                paramuserID.SqlDbType = SqlDbType.Int;

                SqlParameter paramMonth = dbSqlCommand.Parameters.AddWithValue("@month", month);
                paramMonth.SqlDbType = SqlDbType.Int;

                SqlParameter paramYear = dbSqlCommand.Parameters.AddWithValue("@year", year);
                paramYear.SqlDbType = SqlDbType.Int;

                SqlParameter paramBranchId = dbSqlCommand.Parameters.AddWithValue("@branchId", branchId);
                paramBranchId.SqlDbType = SqlDbType.Int;

                SqlParameter output = dbSqlCommand.Parameters.AddWithValue("@output", SqlDbType.Int);
                // output.SqlDbType = SqlDbType.Int;

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
        }

        public DataTable GetMonthlyInputData(int branchID, int? employeeTypeId, int? month, int? year)
        {
            log.Info("DataImportRepository/GetMonthlyInputData");
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
                dbSqlCommand.CommandText = "SP_MonthlyInputData";
                dbSqlCommand.Parameters.Add("@BranchId", SqlDbType.Int).Value = branchID;
                dbSqlCommand.Parameters.Add("@EmployeeTypeId", SqlDbType.Int).Value = employeeTypeId;
                dbSqlCommand.Parameters.Add("@SalMonth", SqlDbType.Int).Value = month;
                dbSqlCommand.Parameters.Add("@SalYear", SqlDbType.Int).Value = year;
                dbSqlDataTable = new DataTable();
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
