using System;
using System.Data;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Data.SqlClient;

namespace Nafed.MicroPay.Data.Repositories
{
    public class DATdsRepository : BaseRepository, IDATdsRepository
    {

        public DataTable GetDATDSManualEntryFormData(int branch, int month, int year)
        {
            log.Info($"DATdsRepository/GetDATDSManualEntryFormData/branch:{branch}/month:{month}/year:{year}");

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
                dbSqlCommand.CommandText = "[getDATDSreport]";

                dbSqlCommand.Parameters.AddWithValue("@branchID", branch);
                dbSqlCommand.Parameters.AddWithValue("@month", month);
                dbSqlCommand.Parameters.AddWithValue("@year", year);
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


        public DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtBranch)
        {
            log.Info($"DATdsRepository/GetImportFieldValues");
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
                dbSqlCommand.CommandText = "[GetDATDSImportFieldDtls]";

                SqlParameter paramEmpCode = dbSqlCommand.Parameters.AddWithValue("@tblEmpCode", dtEmpCodes);
                paramEmpCode.SqlDbType = SqlDbType.Structured;
                paramEmpCode.TypeName = "[udtGenericStringList]";


                SqlParameter paramBName = dbSqlCommand.Parameters.AddWithValue("@tblBranchName", dtBranch);
                paramBName.SqlDbType = SqlDbType.Structured;
                paramBName.TypeName = "[udtGenericStringList]";

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

        public int ImportDATDSManualData(int userID, DataTable dt, int month, int year)
        {
            log.Info($"PFAccountBalanceRepository/ImportDATDSManualData");

            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[ImportDATDSManualData]";

                SqlParameter paramAttendanceDT =
                    dbSqlCommand.Parameters.AddWithValue("@DATDSManualData", dt);
                paramAttendanceDT.SqlDbType = SqlDbType.Structured;
                paramAttendanceDT.TypeName = "[udtDATDSManual]";

                SqlParameter paramuserID = dbSqlCommand.Parameters.AddWithValue("@userID", userID);
                paramuserID.SqlDbType = SqlDbType.Int;

                SqlParameter paramMonth = dbSqlCommand.Parameters.AddWithValue("@month", month);
                paramMonth.SqlDbType = SqlDbType.Int;

                SqlParameter paramYear = dbSqlCommand.Parameters.AddWithValue("@year", year);
                paramYear.SqlDbType = SqlDbType.Int;

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




    }
}
