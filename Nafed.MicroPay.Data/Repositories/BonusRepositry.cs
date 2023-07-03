using System;
using System.Data;
using System.Data.SqlClient;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Data.Models;
using System.Collections.Generic;
using System.Linq;
    

namespace Nafed.MicroPay.Data.Repositories
{

    public class BonusRepositry : BaseRepository, IBonusRepository
    {

        public int calculateBonus(decimal bonusrate, string fromYear, string toYear, int branchID, int financialyear,int empType)
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
                dbSqlCommand.CommandText = "[CalculateBonus]";

                dbSqlCommand.Parameters.Add("@BonusRate", SqlDbType.Decimal).Value = bonusrate;
                dbSqlCommand.Parameters.Add("@fromYear", SqlDbType.VarChar).Value = fromYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;
                dbSqlCommand.Parameters.Add("@financialyear", SqlDbType.Int).Value = financialyear;
                dbSqlCommand.Parameters.Add("@emptype", SqlDbType.Int).Value = empType;
                SqlParameter output = dbSqlCommand.Parameters.AddWithValue("@output", SqlDbType.Int);


                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                output.Direction = ParameterDirection.Output;
                int res = dbSqlCommand.ExecuteNonQuery();
                dbSqlconnection.Close();
                return res = Convert.ToInt32(output.Value);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbSqlconnection.Close();
            }
        }

        public DataTable GetBonusExport(string fromYear, string toYear, int fyear, int branchID, int emptype)
        {
            SqlConnection dbSqlconnection;
            dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
            DataTable dbSqlDataTable = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[ExportBonus]";

                dbSqlCommand.Parameters.Add("@fromYear", SqlDbType.VarChar).Value = fromYear;
                dbSqlCommand.Parameters.Add("@toYear", SqlDbType.VarChar).Value = toYear;
                dbSqlCommand.Parameters.Add("@branchID", SqlDbType.Int).Value = branchID;
                dbSqlCommand.Parameters.Add("@financialyear", SqlDbType.Int).Value = fyear;
                dbSqlCommand.Parameters.Add("@emptype", SqlDbType.Int).Value = emptype;

                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataTable);
                dbSqlconnection.Close();

                return dbSqlDataTable;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbSqlconnection.Close();
            }
        }

        public DataTable GetBonusEntryFormData(int branch, int month, int year)
        {
            log.Info($"BonusRepositry/GetBonusEntryFormData/branch:{branch}&month:{month}&year:{year}");

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
                dbSqlCommand.CommandText = "[GetBonusManualSheet]";

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
            catch (Exception)
            {
                throw;
            }
        }

        public DataSet GetImportFieldValues(DataTable dtEmpCodes, DataTable dtBranch)
        {
            log.Info($"BonusRepositry/GetImportFieldValues");
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
                dbSqlCommand.CommandText = "[GetBonusImportFieldDtls]";

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


        public int ImportBonusManualData(int userID, DataTable dt, int month, int year)
        {
            log.Info($"BonusRepositry/ImportBonusManualData");

            try
            {
                SqlCommand dbSqlCommand;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(db.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[ImportBonusManualData]";

                SqlParameter paramAttendanceDT = dbSqlCommand.Parameters.AddWithValue("@bonusInputDT", dt);
                paramAttendanceDT.SqlDbType = SqlDbType.Structured;
                paramAttendanceDT.TypeName = "[udtBonusData]";

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
